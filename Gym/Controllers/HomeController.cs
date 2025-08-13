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
        #region Login,Logout
        [HttpGet]
        public ActionResult Login(string username = "", string password = "")
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return View();
            }

            username = username.ToLower();
            password = password.ToLower();

            //var pr = new PRModel();
            //var user = db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            //var session = pr.fnLoginUser(username, password);
            var session = "a";
            if (session != null)
            {
                Session["USER_SESSION"] = session;

                if (session == "a")
                    return RedirectToAction("Index", "Home");
                else
                    return RedirectToAction("About", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Sai tài khoản hoặc mật khẩu");
                return View();
            }
        }

        #endregion
        #region Customer
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
        #endregion
        #region Packages
        public ActionResult Packages()
        {
            return View();
        }

        public ContentResult fnGetPackages(string strInput)
        {
            if (Request.IsAjaxRequest())
            {
                var model = JObject.Parse(strInput);
                var packages = new PackagesModel();
                var list = packages.fnGetPackages(model);
                var jsonResult = new
                {
                    list
                };
                return Content(JsonConvert.SerializeObject(jsonResult), "application/json", Encoding.UTF8);
            }
            return Content(JsonConvert.SerializeObject(new { error_session = "1" }), "application/json", Encoding.UTF8);
        }

        public ContentResult fnPostPackages(string strInput)
        {
            if (Request.IsAjaxRequest())
            {
                var model = JObject.Parse(strInput);
                var packages = new PackagesModel();
                var jsonResult = packages.fnPostPackages(model);

                return Content(JsonConvert.SerializeObject(jsonResult), "application/json", Encoding.UTF8);
            }
            return Content(JsonConvert.SerializeObject(new { error_session = "1" }), "application/json", Encoding.UTF8);
        }
        #endregion
        #region Room
        public ActionResult RoomPage()
        {
            return View();
        }

        public ContentResult fnGetRoom(string strInput)
        {
            if (Request.IsAjaxRequest())
            {
                var model = JObject.Parse(strInput);
                var room = new RoomModel();
                var list = room.fnGetRoom(model);
                var jsonResult = new
                {
                    list
                };
                return Content(JsonConvert.SerializeObject(jsonResult), "application/json", Encoding.UTF8);
            }
            return Content(JsonConvert.SerializeObject(new { error_session = "1" }), "application/json", Encoding.UTF8);
        }

        public ContentResult fnPostRoom(string strInput)
        {
            if (Request.IsAjaxRequest())
            {
                var model = JObject.Parse(strInput);
                var room = new RoomModel();
                var jsonResult = room.fnPostRoom(model);
                return Content(JsonConvert.SerializeObject(jsonResult), "application/json", Encoding.UTF8);
            }
            return Content(JsonConvert.SerializeObject(new { error_session = "1" }), "application/json", Encoding.UTF8);
        }
        #endregion
    }
}