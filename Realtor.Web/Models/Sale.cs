using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Realtor.Web.Models
{
    public class Sale
    {
        [Key]
        public int No { get; set; }
        public short SaleType { get; set; }
        public short? RentType { get; set; }
        public short BuildType { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Floor { get; set; }
        public string Amount { get; set; }
        public string Width { get; set; }
        public string Owner { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string RenterTel1 { get; set; }
        public string RenterTel2 { get; set; }
        public string Memo1 { get; set; }
        public string Memo2 { get; set; }
        public string Memo3 { get; set; }
        public string Content { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegistDate { get; set; }
        public string Files { get; set; }
    }
}