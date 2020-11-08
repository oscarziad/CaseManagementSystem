using DataAccess.Data;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace DataAccess.Models
{
    public class Issue
    {
        public Issue()
        {

        }
        public Issue(long id, long customerId, string titel, string category, string description, string status, DateTime created)
        {
            Id = id;
            CustomerId = customerId;
            Titel = titel;
            Category = category;
            Description = description;
            Status = status;
            Created = created;
        }

        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string Titel { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime Created { get; set; }

        public Customer customer { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }  
}
