using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Realtor.Web.Models
{
    public class Contract
    {
        [Key]
        public int No { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string OwnerTel { get; set; }
        public string OwnerName { get; set; }
        public string RenterTel { get; set; }
        public string RenterName { get; set; }
        public string Partner { get; set; }
        public string Balance { get; set; }
        public DateTime? BalanceDate { get; set; }
        public string Payment { get; set; }
        public string Amount { get; set; }
        public string SecondPayment { get; set; }
        public DateTime? SecondPaymentDate { get; set; }
        public string Files { get; set; }
        public string Memo1 { get; set; }
        public string Memo2 { get; set; }
        public string Memo3 { get; set; }
        public string Content { get; set; }
        public DateTime RegistDate { get; set; }
    }
}