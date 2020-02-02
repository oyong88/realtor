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
    public class ContractsController : BaseController
    {
        private RealtorEntities realtorDB = new RealtorEntities();

        public ActionResult Index(int? no)
        {
            ViewBag.Title = "Contracts";

            ViewData["SaleType"] = ConvertToJson(Util.SaleType);
            ViewData["RentType"] = ConvertToJson(Util.RentType);
            ViewData["BuildType"] = ConvertToJson(Util.BuildType);

            ViewData["No"] = no;
            return View();
        }

        [HttpGet]
        public ActionResult List(int? no, int type, string keyword, int page = 1, int pageSize = 1)
        {
            var query = realtorDB.Contracts.AsQueryable();

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
                    query = query.Where(s => s.OwnerTel.Contains(keyword) || s.RenterTel.Contains(keyword));
                }

                //owner
                if (type == 3)
                {
                    query = query.Where(s => s.Name.Contains(keyword) || s.OwnerName.Contains(keyword) || s.RenterName.Contains(keyword));
                }
            }

            var total = query.Count();

            query = query.OrderByDescending(s => s.RegistDate);
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return Json(new { list = query.ToList(), total = total });
        }

        [HttpGet]
        public ActionResult Load(int no)
        {
            Contract existContract = realtorDB.Contracts.Find(no);

            if (existContract == null)
            {
                return HttpNotFound();
            }

            return Json(existContract);
        }

        [HttpPost]
        public ActionResult Save(Contract contract)
        {
            //new
            if (contract.No == 0)
            {
                contract.Files = null;
                realtorDB.Contracts.Add(contract);

                realtorDB.SaveChanges();
            }
            //update
            else
            {
                Contract existContract = realtorDB.Contracts.Find(contract.No);

                if (contract != null)
                {
                    existContract.Name = contract.Name;
                    existContract.Address = contract.Address;
                    existContract.OwnerTel = contract.OwnerTel;
                    existContract.OwnerName = contract.OwnerName;
                    existContract.RenterTel = contract.RenterTel;
                    existContract.RenterName = contract.RenterName;
                    existContract.Partner = contract.Partner;
                    existContract.Payment = contract.Payment;
                    existContract.Amount = contract.Amount;
                    existContract.SecondPayment = contract.SecondPayment;
                    existContract.SecondPaymentDate = contract.SecondPaymentDate;
                    existContract.Balance = contract.Balance;
                    existContract.BalanceDate = contract.BalanceDate;
                    existContract.Memo1 = contract.Memo1;
                    existContract.Memo2 = contract.Memo2;
                    existContract.Memo3 = contract.Memo3;
                    existContract.Content = contract.Content;
                    existContract.RegistDate = contract.RegistDate;
                }

                realtorDB.SaveChanges();
            }


            return Json(new { Result = true });

        }

        public ActionResult Export()
        {
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=계약정보.xml");
            Response.ContentType = "application/vnd.ms-excel";

            var contracts = realtorDB.Contracts.OrderByDescending(c => c.RegistDate).ToList();
            return PartialView("Export", contracts);
        }

        [HttpPost]
        public ActionResult Upload(int no)
        {
            Contract contract = realtorDB.Contracts.Find(no);

            if (contract != null)
            {
                var files = new List<string>();

                if (!string.IsNullOrEmpty(contract.Files))
                {
                    files = contract.Files.Split(new char[] { ',' }).ToList();
                }

                for (var i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];

                    string root = ConfigurationManager.AppSettings["upload.path"];

                    string fileName = string.Format("({0})_{1}", no, file.FileName);

                    string filePath = Path.Combine(root, fileName);

                    file.SaveAs(filePath);

                    if (!files.Contains(fileName))
                    {
                        files.Add(fileName);
                    }
                }

                contract.Files = string.Join(",", files);

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
            Contract contract = realtorDB.Contracts.Find(no);

            if (contract != null)
            {
                var files = new List<string>();

                if (!string.IsNullOrEmpty(contract.Files))
                {
                    files = contract.Files.Split(new char[] { ',' }).ToList();
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

                contract.Files = string.Join(",", files);

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
