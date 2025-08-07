using Gym.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace Gym.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ContentResult fnGetCustomer(string strInput)
        {
            if (Request.IsAjaxRequest())
            {
                var model = JObject.Parse(strInput);
                var cus = new CustomerModel();
                var list = cus.fnGetCustomer(model);
                var jsonResult = new
                {
                    list
                };
                return Content(JsonConvert.SerializeObject(jsonResult), "application/json", Encoding.UTF8);
            }
            return Content(JsonConvert.SerializeObject(new { error_session = "1" }), "application/json", Encoding.UTF8);
        }

        public ContentResult fnPostCustomer(string strInput)
        {
            if (Request.IsAjaxRequest())
            {
                var model = JObject.Parse(strInput);
                var cus = new CustomerModel();
                var jsonResult = cus.fnPostCustomer(model);

                return Content(JsonConvert.SerializeObject(jsonResult), "application/json", Encoding.UTF8);
            }
            return Content(JsonConvert.SerializeObject(new { error_session = "1" }), "application/json", Encoding.UTF8);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}