using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Customer
    {
        public Customer()
        {

        }
        public Customer(long id, string name, DateTime created)
        {
            Id = id;
            Name = name;
            Created = created;
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime Created{ get; set; }
    }
}
