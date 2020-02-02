using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Realtor.Web.Framework;
using Realtor.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Realtor.Web.Controllers
{
    public class HomeController : BaseController
    {
        private RealtorEntities realtorDB = new RealtorEntities();

        // GET: Home
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string password)
        {
            if (password == ConfigurationManager.AppSettings["password"])
            {
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1,
                    "Realtor",
                    DateTime.Now,
                    DateTime.Parse("2999-12-31"),
                    true,
                    string.Empty,
                    FormsAuthentication.FormsCookiePath
                );

                string encryptedTicket = FormsAuthentication.Encrypt(ticket);

                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                cookie.Path = FormsAuthentication.FormsCookiePath;
                cookie.Expires = DateTime.Parse("2999-12-31"); // good for one year

                Response.Cookies.Add(cookie);

                return Json(new { result = 0 });
            }
            else
            {
                return Json(new { result = 1 });
            }
        }

        [AllowAnonymous]
        public ActionResult Logout(string password)
        {
            FormsAuthentication.SignOut();

            return View("Index");
        }

        public ActionResult Search(string searchWord)
        {
            //계약관리
            var contracts = realtorDB.Contracts.AsQueryable();

            contracts = contracts.Where(c => 
                c.Address.Contains(searchWord) || 
                c.Name.Contains(searchWord) || 
                c.OwnerName.Contains(searchWord) || 
                c.OwnerTel.Contains(searchWord) ||
                c.RenterName.Contains(searchWord) ||
                c.RenterTel.Contains(searchWord)
            );

            var customers = realtorDB.Customers.AsQueryable();

            customers = customers.Where(c =>
                c.Name.Contains(searchWord) ||
                c.Tel.Contains(searchWord)
            );

            var phoneBooks = realtorDB.PhoneBooks.AsQueryable();

            phoneBooks = phoneBooks.Where(p =>
                p.Name.Contains(searchWord) ||
                p.Tel.Contains(searchWord) ||
                p.Address.Contains(searchWord)
            );

            var sales = realtorDB.Sales.AsQueryable();

            sales = sales.Where(s =>
                s.Name.Contains(searchWord) ||
                s.Address.Contains(searchWord) ||
                s.Owner.Contains(searchWord) ||
                s.RenterTel1.Contains(searchWord) ||
                s.RenterTel2.Contains(searchWord) ||
                s.Tel1.Contains(searchWord) ||
                s.Tel2.Contains(searchWord)
            );

            return Json(new
            {
                contracts = contracts.ToList(),
                sales = sales.Where(c => c.IsActive == true).ToList(),
                salesHistory = sales.Where(c => c.IsActive == false).ToList(),
                phoneBooks = phoneBooks.ToList(),
                customers = customers.Where(c => c.IsActive == true).ToList(),
                customersHistory = customers.Where(c => c.IsActive == false).ToList()
            });
        }
    }
}