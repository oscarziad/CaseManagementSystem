﻿using DataAccess.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.AI.MachineLearning.Preview;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Diagnostics;
using Windows.Storage;

namespace DataAccess.Data
{
    public static class SqliteContext
    {

        #region Configuration and Properties
        private static string _dbpath { get; set; }
        public static async void UseSQLite(string databaseName = "sqlite.db")
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync(databaseName, CreationCollisionOption.OpenIfExists);
            _dbpath = $"Filename={Path.Combine(ApplicationData.Current.LocalFolder.Path, databaseName)}";

            using (var db = new SqliteConnection(_dbpath))
            {
                db.Open();

                var query = "CREATE TABLE IF NOT EXISTS Customers (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL, Created DATETIME); CREATE TABLE IF NOT EXISTS Issues (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, CustomerId INTEGER NOT NULL, Titel TEXT NOT NULL, Category TEXT NOT NULL, Description TEXT NOT NULL, Status TEXT NOT NULL, Created DATETIME, FOREIGN KEY (CustomerId) REFERENCES Customers(Id)); CREATE TABLE IF NOT EXISTS Comments (Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, IssueId INTEGER NOT NULL, Description TEXT NOT NULL, Created DATETIME, FOREIGN KEY (IssueId) REFERENCES Issues(Id));";
                SqliteCommand cmd = new SqliteCommand(query, db);
                await cmd.ExecuteNonQueryAsync();

                db.Close();
            }
        }
        #endregion

        #region Create Methods
        public static async Task<long> CreateCustomerAsync(Customer customer)
        {
            long id = 0;

            using (var db = new SqliteConnection(_dbpath))
            {
                db.Open();

                var query = "INSERT INTO Customers VALUES (null,@Name,@Created)";
                var cmd = new SqliteCommand(query, db);

                cmd.Parameters.AddWithValue("@Name", customer.Name);
                cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = "SELECT Id FROM Customers WHERE Name = @Name";
                id = (long) await cmd.ExecuteScalarAsync();

                db.Close();
            }
            return id;
        }

        public static async Task<long> CreateIssueAsync(Issue issue)
        {
            long id = 0;
            using (var db = new SqliteConnection(_dbpath))
            {
                db.Open();

                var query = "INSERT INTO Issues VALUES(null, @CustomerId, @Titel, @Category, @Description, @Status, @Created);";
                var cmd = new SqliteCommand(query, db);

                cmd.Parameters.AddWithValue("@CustomerId", issue.CustomerId);
                cmd.Parameters.AddWithValue("@Titel", issue.Titel);
                cmd.Parameters.AddWithValue("@Category", issue.Category);
                cmd.Parameters.AddWithValue("@Description", issue.Description);
                cmd.Parameters.AddWithValue("@Status", "Nytt");
                cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                await cmd.ExecuteNonQueryAsync();

                cmd.CommandText = "SELECT Id FROM Issues WHERE Titel = @Titel";
                id = (long)await cmd.ExecuteScalarAsync();

                db.Close();
            }
            return id;
        }

        public static async Task CreateCommentAsync(Comment comment)
        {
            using (var db = new SqliteConnection(_dbpath))
            {
                db.Open();

                var query = "INSERT INTO Comments VALUES (null, @IssueId, @Description, @Created)";
                var cmd = new SqliteCommand(query, db);

                cmd.Parameters.AddWithValue("@IssueId", comment.IssueId);
                cmd.Parameters.AddWithValue("@Description", comment.Description);
                cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                await cmd.ExecuteNonQueryAsync();

                db.Close();
            }
        }
        #endregion

        #region Get Methods

        public static async Task<IEnumerable<Customer>> GetCustomers()
        {
            var customers = new List<Customer>();

            using (var db = new SqliteConnection(_dbpath))
            {
                db.Open();

                var query = "SELECT * FROM Customers";
                var cmd = new SqliteCommand(query, db);

                var result = await cmd.ExecuteReaderAsync();

                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        customers.Add(new Customer(result.GetInt32(0), result.GetString(1), result.GetDateTime(2)));
                    }
                }

                db.Close();
            }
            return customers;
        }

        public static async Task<IEnumerable<string>> GetCustomerNames()
        {
            var customernames = new List<string>();

            using (var db = new SqliteConnection(_dbpath))
            {
                db.Open();

                var query = "SELECT Name FROM Customers";
                var cmd = new SqliteCommand(query, db);

                var result = await cmd.ExecuteReaderAsync();

                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        customernames.Add(result.GetString(0));
                    }
                }

                db.Close();
            }
            return customernames;
        }

        public static async Task<Customer> GetCustomerById(int id)
        {
            var customer = new Customer();

            using (var db = new SqliteConnection(_dbpath))
            {
                db.Open();

                var query = "SELECT * FROM Customers WHERE Id = @Id";
                var cmd = new SqliteCommand(query, db);

                cmd.Parameters.AddWithValue("@Id", id);
                var result = await cmd.ExecuteReaderAsync();

                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        customer = new Customer(result.GetInt32(0), result.GetString(1), result.GetDateTime(2));
                    }
                }

                db.Close();
            }
            return customer;
        }

        public static async Task<long> GetCustomerIdByNames(string name)
        {
            long customerid = 0;

            using (var db = new SqliteConnection(_dbpath))
            {
                db.Open();

                var query = "SELECT Id FROM Customers WHERE Name = @Name";
                var cmd = new SqliteCommand(query, db);

                cmd.Parameters.AddWithValue("@Name", name);
                customerid = (long)await cmd.ExecuteScalarAsync();

                db.Close();
            }
            return customerid;
        }

        public static async Task<ICollection<Comment>> GetCommentsByIssueId(int issueid)
        {
            var comments = new List<Comment>();

            using (var db = new SqliteConnection(_dbpath))
            {
                db.Open();

                var query = "SELECT * FROM Comments WHERE IssueId = @IssueId";
                var cmd = new SqliteCommand(query, db);

                cmd.Parameters.AddWithValue("IssueId", issueid);
                var result = await cmd.ExecuteReaderAsync();

                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        comments.Add(new Comment(result.GetInt32(0), result.GetInt32(1), result.GetString(2), result.GetDateTime(3)));
                    }
                }

                db.Close();
            }
            return comments;
        }

        public static async Task<IEnumerable<Issue>> GetIssues()
        {
            var issues = new List<Issue>();

            using (var db = new SqliteConnection(_dbpath))
            {
                db.Open();

                var query = "SELECT * FROM Issues";
                var cmd = new SqliteCommand(query, db);

                var result = await cmd.ExecuteReaderAsync();
                 
                if (result.HasRows)
                {
                    while (result.Read())
                    {
                        var issue = new Issue(result.GetInt32(0), result.GetInt32(1), result.GetString(2), result.GetString(3), result.GetString(4), result.GetString(5), result.GetDateTime(6));
                        issue.customer = await GetCustomerById(result.GetInt32(1));
                        issue.Comments = await GetCommentsByIssueId(result.GetInt32(0));

                        issues.Add(issue);
                    }
                }

                db.Close();
            }
            return issues;
        }
        
        public static async Task UpdateIssue(Issue issue)
        {
            using (var db = new SqliteConnection(_dbpath))
            {
                db.Open();
                
                var query = "UPDATE Issues SET Status = @Status WHERE Id = @Id";
                var cmd = new SqliteCommand(query, db);

                cmd.Parameters.AddWithValue("@Status", issue.Status);
                cmd.Parameters.AddWithValue("@Id", issue.Id);
                await cmd.ExecuteNonQueryAsync();

                db.Close();
            }
        }
        
        #endregion
    }
}
