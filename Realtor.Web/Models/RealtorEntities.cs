
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Realtor.Web.Models
{
    public class RealtorEntities : DbContext
    {
        public RealtorEntities()
            : base("name=RealtorEntities")
        {
            
        }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<PhoneBook> PhoneBooks { get; set; }
    }
}