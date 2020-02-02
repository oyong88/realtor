using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;

namespace Realtor.Web.Framework
{
    public class BaseController : Controller
    {
        private readonly ILog Logger = LogManager.GetLogger(typeof(BaseController).ToString());

        public ActionResult Download(int no, string fileName)
        {
            string root = ConfigurationManager.AppSettings["upload.path"];

            string filePath = Path.Combine(root, fileName);

            if(!System.IO.File.Exists(filePath))
            {
                return HttpNotFound();
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(filePath));
        }

        public string ConvertToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter>
                {
                    new IsoDateTimeConverter()
                }
            });
        }

        public new ActionResult Json(object obj)
        {
            return Content(ConvertToJson(obj), "application/json");
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                filterContext.Result = Json(
                  new
                  {
                      //TODO: Localize this message, and change for product, some more user friendly error.
                      success = false,
                      message = filterContext.Exception.Message
                  });

                Logger.Error(filterContext.Exception.Message, filterContext.Exception);

                filterContext.ExceptionHandled = true;
            }
            else
            {
                // Exception was not handled, so show a general error page
                // add error message localization if found
                bool isLocal = false;
                if (Request.UserHostName != null)
                {
                    // show missing error message if localhost (otherwise find or show default)
                    isLocal = Request.UserHostName.Contains("localhost");
                }

                string errorMessage = filterContext.Exception.Message;

                Logger.Error(errorMessage, filterContext.Exception);
            }
        }

    }
}