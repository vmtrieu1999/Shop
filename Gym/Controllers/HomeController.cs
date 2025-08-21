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
        public ActionResult Login(string username = "", string password = "")
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return View();
            }

            username = username.ToLower();
            password = ConnectionModel.MaHoa(password);

            var con = new ConnectionModel();
            var session = con.fnLoginUser(username, password);
            if (session != null)
            {
                Session["USER_SESSION"] = session;

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Sai tài khoản hoặc mật khẩu");
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            return RedirectToAction("Login", "Home");
        }
        #endregion
        #region Customer
        public ActionResult Index()
        {
            var user = Session["USER_SESSION"] as UserSession;
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                ViewBag.User = user;
                return View();
            }
        }

        public ContentResult fnGetCustomer(string strInput)
        {
            var user = Session["USER_SESSION"] as UserSession;
            if (Request.IsAjaxRequest() && user != null)
            {
                var model = JObject.Parse(strInput);
                model["COMPANY_CODE"] = user.CompanyCode;
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
            var user = Session["USER_SESSION"] as UserSession;
            if (Request.IsAjaxRequest() && user != null)
            {
                var model = JObject.Parse(strInput);
                model["USER"] = user.Username;
                model["COMPANY_CODE"] = user.CompanyCode;
                var cus = new CustomerModel();
                var jsonResult = cus.fnPostCustomer(model);

                return Content(JsonConvert.SerializeObject(jsonResult), "application/json", Encoding.UTF8);
            }
            return Content(JsonConvert.SerializeObject(new { error_session = "1" }), "application/json", Encoding.UTF8);
        }

        public ContentResult fnGetCustomerShip(string strInput)
        {
            var user = Session["USER_SESSION"] as UserSession;
            if (Request.IsAjaxRequest() && user != null)
            {
                var model = JObject.Parse(strInput);
                model["COMPANY_CODE"] = user.CompanyCode;
                var cus = new CustomerModel();
                var list = cus.fnGetCustomerShip(model);
                var jsonResult = new
                {
                    list
                };
                return Content(JsonConvert.SerializeObject(jsonResult), "application/json", Encoding.UTF8);
            }
            return Content(JsonConvert.SerializeObject(new { error_session = "1" }), "application/json", Encoding.UTF8);
        }

        public ContentResult fnPostCustomerShip(string strInput)
        {
            var user = Session["USER_SESSION"] as UserSession;
            if (Request.IsAjaxRequest() && user != null)
            {
                var model = JObject.Parse(strInput);
                model["USER"] = user.Username;
                model["COMPANY_CODE"] = user.CompanyCode;
                var cus = new CustomerModel();
                var jsonResult = cus.fnPostCustomerShip(model);

                return Content(JsonConvert.SerializeObject(jsonResult), "application/json", Encoding.UTF8);
            }
            return Content(JsonConvert.SerializeObject(new { error_session = "1" }), "application/json", Encoding.UTF8);
        }
        #endregion
        #region Packages
        public ActionResult Packages()
        {
            var user = Session["USER_SESSION"] as UserSession;
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                ViewBag.User = user;
                return View();
            }
        }

        public ContentResult fnGetPackages(string strInput)
        {
            var user = Session["USER_SESSION"] as UserSession;
            if (Request.IsAjaxRequest() && user != null)
            {
                var model = JObject.Parse(strInput);
                model["COMPANY_CODE"] = user.CompanyCode;
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
            var user = Session["USER_SESSION"] as UserSession;
            if (Request.IsAjaxRequest() && user != null)
            {
                var model = JObject.Parse(strInput);
                model["COMPANY_CODE"] = user.CompanyCode;
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
            var user = Session["USER_SESSION"] as UserSession;
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                ViewBag.User = user;
                return View();
            }
        }

        public ContentResult fnGetRoom(string strInput)
        {
            var user = Session["USER_SESSION"] as UserSession;
            if (Request.IsAjaxRequest() && user != null)
            {
                var model = JObject.Parse(strInput);
                model["COMPANY_CODE"] = user.CompanyCode;
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
            var user = Session["USER_SESSION"] as UserSession;
            if (Request.IsAjaxRequest() && user != null)
            {
                var model = JObject.Parse(strInput);
                model["COMPANY_CODE"] = user.CompanyCode;
                var room = new RoomModel();
                var jsonResult = room.fnPostRoom(model);
                return Content(JsonConvert.SerializeObject(jsonResult), "application/json", Encoding.UTF8);
            }
            return Content(JsonConvert.SerializeObject(new { error_session = "1" }), "application/json", Encoding.UTF8);
        }
        #endregion
        #region Company
        
        public ActionResult CompanyPage()
        {
            var user = Session["USER_SESSION"] as UserSession;
            if(user != null && user.CompanyCode == "VMT" && user.Username == "admin")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public ContentResult fnGetCompany(string strInput)
        {
            var user = Session["USER_SESSION"] as UserSession;
            if (Request.IsAjaxRequest() && user != null && user.CompanyCode == "VMT" && user.Username == "admin")
            {
                var model = JObject.Parse(strInput);
                var company = new CompanyModel();
                var list = company.fnGetCompany(model);
                var jsonResult = new
                {
                    list
                };
                return Content(JsonConvert.SerializeObject(jsonResult), "application/json", Encoding.UTF8);
            }
            return Content(JsonConvert.SerializeObject(new { error_session = "1" }), "application/json", Encoding.UTF8);
        }
        public ContentResult fnPostCompany(string strInput)
        {
            var user = Session["USER_SESSION"] as UserSession;
            if (Request.IsAjaxRequest() && user != null && user.CompanyCode == "VMT" && user.Username == "admin")
            {
                var model = JObject.Parse(strInput);
                model["USER"] = user.Username;
                var company = new CompanyModel();
                var jsonResult = company.fnPostCompany(model);
                return Content(JsonConvert.SerializeObject(jsonResult), "application/json", Encoding.UTF8);
            }
            return Content(JsonConvert.SerializeObject(new { error_session = "1" }), "application/json", Encoding.UTF8);
        }
        #endregion
        #region User
        public ActionResult UserPage()
        {
            var user = Session["USER_SESSION"] as UserSession;
            if (user != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ContentResult fnGetUser(string strInput)
        {
            var user = Session["USER_SESSION"] as UserSession;
            if (Request.IsAjaxRequest() && user != null)
            {
                var model = JObject.Parse(strInput);
                var userModel = new UserModel();
                model["USER_MASTER"] = user.Username;
                model["COMPANY_CODE"] = user.CompanyCode;
                var list = userModel.fnGetUser(model);
                var jsonResult = new
                {
                    list
                };
                return Content(JsonConvert.SerializeObject(jsonResult), "application/json", Encoding.UTF8);
            }
            return Content(JsonConvert.SerializeObject(new { error_session = "1" }), "application/json", Encoding.UTF8);
        }

        public ContentResult fnPostUser(string strInput)
        {
            var user = Session["USER_SESSION"] as UserSession;
            if (Request.IsAjaxRequest() && user != null)
            {
                var model = JObject.Parse(strInput);
                var userModel = new UserModel();
                var jsonResult = userModel.fnPostUser(model);
                return Content(JsonConvert.SerializeObject(jsonResult), "application/json", Encoding.UTF8);
            }
            return Content(JsonConvert.SerializeObject(new { error_session = "1" }), "application/json", Encoding.UTF8);
        }
        #endregion
    }
}