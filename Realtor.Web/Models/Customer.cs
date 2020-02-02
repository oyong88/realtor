using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Realtor.Web.Models
{
    public class Customer
    {
        [Key]
        public int No { get; set; }
        public short SaleType { get; set; }
        public short? RentType { get; set; }
        public short BuildType { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
        
        public string Tel { get; set; }
        public string Memo1 { get; set; }
        public string Memo2 { get; set; }
        public string Memo3 { get; set; }
        public string Content { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegistDate { get; set; }
        
    }
}