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
    public class PhoneBooksController : BaseController
    {
        private RealtorEntities realtorDB = new RealtorEntities();

        public ActionResult Index(int? no)
        {
            ViewBag.Title = "PhoneBooks";

            ViewData["PhoneType"] = ConvertToJson(Util.PhoneType);
            ViewData["No"] = no;

            return View();
        }

        [HttpGet]
        public ActionResult List(int? no, int? phoneType, int type, string keyword, int page = 1, int pageSize = 1)
        {

            var query = realtorDB.PhoneBooks.AsQueryable();

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
                if (phoneType.HasValue)
                {
                    query = query.Where(s => s.PhoneType == phoneType.Value);
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
            PhoneBook existPhoneBook = realtorDB.PhoneBooks.Find(no);

            if (existPhoneBook == null)
            {
                return HttpNotFound();
            }

            return Json(existPhoneBook);
        }

        [HttpPost]
        public ActionResult Save(PhoneBook phoneBook)
        {
            //new
            if (phoneBook.No == 0)
            {
                phoneBook.Files = null;
                realtorDB.PhoneBooks.Add(phoneBook);

                realtorDB.SaveChanges();
            }
            //update
            else
            {
                PhoneBook existPhoneBook = realtorDB.PhoneBooks.Find(phoneBook.No);

                if (existPhoneBook != null)
                {
                    existPhoneBook.Name = phoneBook.Name;
                    existPhoneBook.PhoneType = phoneBook.PhoneType;
                    existPhoneBook.Address = phoneBook.Address;
                    existPhoneBook.Tel = phoneBook.Tel;
                    existPhoneBook.Content = phoneBook.Content;
                    existPhoneBook.RegistDate = phoneBook.RegistDate;
                }

                realtorDB.SaveChanges();
            }


            return Json(new { Result = true });

        }

        public ActionResult Export()
        {
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=전화번호부.xml");
            Response.ContentType = "application/vnd.ms-excel";

            var phoneBooks = realtorDB.PhoneBooks.OrderByDescending(c => c.RegistDate).ToList();
            return PartialView("Export", phoneBooks);
        }

        [HttpPost]
        public ActionResult Upload(int no)
        {
            
            PhoneBook phoneBook = realtorDB.PhoneBooks.Find(no);

            if (phoneBook != null)
            {
                var files = new List<string>();

                if (!string.IsNullOrEmpty(phoneBook.Files))
                {
                    files = phoneBook.Files.Split(new char[] { ',' }).ToList();
                }

                for(var i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];

                    string root = ConfigurationManager.AppSettings["upload.path"];

                    string fileName = string.Format("P({0})_{1}", no, file.FileName);

                    string filePath = Path.Combine(root, fileName);

                    file.SaveAs(filePath);

                    if (!files.Contains(fileName))
                    {
                        files.Add(fileName);
                    }
                }

                phoneBook.Files = string.Join(",", files);

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
            PhoneBook phoneBook = realtorDB.PhoneBooks.Find(no);

            if (phoneBook != null)
            {
                var files = new List<string>();

                if (!string.IsNullOrEmpty(phoneBook.Files))
                {
                    files = phoneBook.Files.Split(new char[] { ',' }).ToList();
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

                phoneBook.Files = string.Join(",", files);

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