using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Realtor.Web.Models
{
    public class PhoneBook
    {
        [Key]
        public int No { get; set; }
        public short PhoneType { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public string Content { get; set; }
        public DateTime RegistDate { get; set; }
        public string Files { get; set; }
    }
}