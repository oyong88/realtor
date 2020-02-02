using Realtor.Web.Framework;
using Realtor.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Realtor.Web.Controllers
{
    public class SalesController : BaseController
    {
        private RealtorEntities realtorDB = new RealtorEntities();

        public ActionResult Index(int? no)
        {
            ViewBag.Title = "Sales";

            ViewData["SaleType"] = ConvertToJson(Util.SaleType);
            ViewData["RentType"] = ConvertToJson(Util.RentType);
            ViewData["BuildType"] = ConvertToJson(Util.BuildType);

            ViewData["No"] = no;

            return View();
        }

        public ActionResult History(int? no)
        {
            ViewBag.Title = "SalesHistory";

            ViewData["SaleType"] = ConvertToJson(Util.SaleType);
            ViewData["RentType"] = ConvertToJson(Util.RentType);
            ViewData["BuildType"] = ConvertToJson(Util.BuildType);

            ViewData["No"] = no;

            return View("Index");
        }

        [HttpGet]
        public ActionResult List(int? no, bool isActive, int? saleType, int? rentType, int? buildType, int type, string keyword, int page = 1, int pageSize = 1)
        {
            var query = realtorDB.Sales.AsQueryable().Where(s => s.IsActive == isActive);

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
                    query = query.Where(s => s.Address.Contains(keyword));
                }

                //phone
                if (type == 2)
                {
                    query = query.Where(s => s.Tel1.Contains(keyword) || s.Tel2.Contains(keyword) ||
                        s.RenterTel1.Contains(keyword) || s.RenterTel2.Contains(keyword));
                }

                //name
                if (type == 3)
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
            Sale existSale = realtorDB.Sales.Find(no);

            existSale.IsActive = !existSale.IsActive;

            realtorDB.SaveChanges();

            return Json(new { Result = true });
        }

        [HttpPost]
        public ActionResult Save(Sale sale)
        {
            //new
            if (sale.No == 0)
            {
                sale.Files = null;
                realtorDB.Sales.Add(sale);

                realtorDB.SaveChanges();
            }
            //update
            else
            {
                Sale existSale = realtorDB.Sales.Find(sale.No);

                if (sale != null)
                {
                    existSale.Name = sale.Name;
                    existSale.Address = sale.Address;
                    existSale.Amount = sale.Amount;
                    existSale.Width = sale.Width;
                    existSale.BuildType = sale.BuildType;
                    existSale.SaleType = sale.SaleType;
                    existSale.RentType = sale.RentType;
                    existSale.Floor = sale.Floor;
                    existSale.Owner = sale.Owner;
                    existSale.Tel1 = sale.Tel1;
                    existSale.Tel2 = sale.Tel2;
                    existSale.RenterTel1 = sale.RenterTel1;
                    existSale.RenterTel2 = sale.RenterTel2;
                    existSale.Memo1 = sale.Memo1;
                    existSale.Memo2 = sale.Memo2;
                    existSale.Memo3 = sale.Memo3;
                    existSale.Content = sale.Content;
                    existSale.RegistDate = sale.RegistDate;
                }

                realtorDB.SaveChanges();
            }


            return Json(new { Result = true });

        }

        [HttpGet]
        public ActionResult Load(int no)
        {
            Sale existSale = realtorDB.Sales.Find(no);

            if (existSale == null)
            {
                return HttpNotFound();
            }

            return Json(existSale);
        }

        public ActionResult Export(bool isActive)
        {
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=매물정보.xml");
            Response.ContentType = "application/vnd.ms-excel";

            var sales = realtorDB.Sales.Where(s => s.IsActive == isActive).OrderByDescending(s => s.RegistDate).ToList();
            return PartialView("Export", sales);
        }

        [HttpPost]
        public ActionResult Upload(int no)
        {
            
            Sale sale = realtorDB.Sales.Find(no);

            if (sale != null)
            {
                var files = new List<string>();

                if (!string.IsNullOrEmpty(sale.Files))
                {
                    files = sale.Files.Split(new char[] { ',' }).ToList();
                }

                for (var i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];

                    string root = ConfigurationManager.AppSettings["upload.path"];

                    string fileName = string.Format("S({0})_{1}", no, file.FileName);

                    string filePath = Path.Combine(root, fileName);

                    file.SaveAs(filePath);

                    if (!files.Contains(fileName))
                    {
                        files.Add(fileName);
                    }
                }

                sale.Files = string.Join(",", files);

                realtorDB.SaveChanges();

                return Json(new { Result = true, Files = string.Join(",", files) });

            }
            else
            {
                return HttpNotFound();
            }
            
        }

        [HttpPost]
        public ActionResult Delete(int no, string fileName)
        {
            
            Sale sale = realtorDB.Sales.Find(no);

            if (sale != null)
            {
                var files = new List<string>();

                if (!string.IsNullOrEmpty(sale.Files))
                {
                    files = sale.Files.Split(new char[] { ',' }).ToList();
                }

                if (files.Contains(fileName))
                {
                    files.Remove(fileName);

                    string root = ConfigurationManager.AppSettings["upload.path"];

                    string filePath = Path.Combine(root, fileName);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                sale.Files = string.Join(",", files);

                realtorDB.SaveChanges();

                return Json(new { Result = true, Files = string.Join(",", files) });
            }
            else
            {
                return HttpNotFound();
            }
            
        }


    }
}