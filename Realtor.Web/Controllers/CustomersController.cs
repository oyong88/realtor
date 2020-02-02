using Realtor.Web.Framework;
using Realtor.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Realtor.Web.Controllers
{
    public class CustomersController : BaseController
    {
        private RealtorEntities realtorDB = new RealtorEntities();

        public ActionResult Index(int? no)
        {
            ViewBag.Title = "Customers";

            ViewData["SaleType"] = ConvertToJson(Util.SaleType);
            ViewData["RentType"] = ConvertToJson(Util.RentType);
            ViewData["BuildType"] = ConvertToJson(Util.BuildType);

            ViewData["No"] = no;

            return View();
        }

        public ActionResult History(int? no)
        {
            ViewBag.Title = "CustomersHistory";

            ViewData["SaleType"] = ConvertToJson(Util.SaleType);
            ViewData["RentType"] = ConvertToJson(Util.RentType);
            ViewData["BuildType"] = ConvertToJson(Util.BuildType);

            ViewData["No"] = no;

            return View("Index");
        }

        [HttpGet]
        public ActionResult List(int? no, bool isActive, int? saleType, int? rentType, int? buildType, int type, string keyword, int page = 1, int pageSize = 1)
        {
            var query = realtorDB.Customers.AsQueryable().Where(c => c.IsActive == isActive);

            //no
            if (no.HasValue)
            {
                query = query.Where(s => s.No == no);
            }
            //searching by keywork
            else if (!string.IsNullOrEmpty(keyword))
            {
                //address
                if (type == 1)
                {
                    query = query.Where(s => s.Tel.Contains(keyword));
                }

                //name
                if (type == 2)
                {
                    query = query.Where(s => s.Name.Contains(keyword));
                }
            }
            //searching by category
            else
            {
                if (saleType.HasValue)
                {
                    query = query.Where(s => s.SaleType == saleType.Value);
                }

                if (rentType.HasValue)
                {
                    query = query.Where(s => s.RentType == rentType.Value);
                }

                if (buildType.HasValue)
                {
                    query = query.Where(s => s.BuildType == buildType.Value);
                }
            }

            var total = query.Count();

            query = query.OrderByDescending(s => s.RegistDate);
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return Json(new { list = query.ToList(), total = total });
        }

        [HttpPost]
        public ActionResult Active(int no)
        {
            Customer existCustomer = realtorDB.Customers.Find(no);

            existCustomer.IsActive = !existCustomer.IsActive;

            realtorDB.SaveChanges();

            return Json(new { Result = true });
        }

        [HttpGet]
        public ActionResult Load(int no)
        {
            Customer existCustomer = realtorDB.Customers.Find(no);

            if (existCustomer == null)
            {
                return HttpNotFound();
            }

            return Json(existCustomer);
        }

        [HttpPost]
        public ActionResult Save(Customer customer)
        {
            //new
            if (customer.No == 0)
            {

                realtorDB.Customers.Add(customer);

                realtorDB.SaveChanges();
            }
            //update
            else
            {
                Customer existCustomer = realtorDB.Customers.Find(customer.No);

                if (customer != null)
                {
                    existCustomer.Name = customer.Name;
                    existCustomer.BuildType = customer.BuildType;
                    existCustomer.SaleType = customer.SaleType;
                    existCustomer.RentType = customer.RentType;
                    existCustomer.Amount = customer.Amount;
                    existCustomer.Tel = customer.Tel;
                    existCustomer.Memo1 = customer.Memo1;
                    existCustomer.Memo2 = customer.Memo2;
                    existCustomer.Memo3 = customer.Memo3;
                    existCustomer.Content = customer.Content;
                    existCustomer.RegistDate = customer.RegistDate;
                }

                realtorDB.SaveChanges();
            }


            return Json(new { Result = true });

        }

        public ActionResult Export(bool isActive)
        {
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=고객정보.xml");
            Response.ContentType = "application/vnd.ms-excel";

            var customers = realtorDB.Customers.Where(c => c.IsActive == isActive).OrderByDescending(c => c.RegistDate).ToList();
            return PartialView("Export", customers);
        }
    }
}