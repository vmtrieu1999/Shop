using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;

namespace Gym.Models
{
    public class UserModel
    {
        public JArray fnGetUser(JObject model)
        {
            var list = new JArray();
            try
            {
                
                var username = model["USERNAME"]?.ToString() ?? "";
                var user_fullname = model["FULLNAME"]?.ToString() ?? "";
                var userMaster = model["USER_MASTER"]?.ToString() ?? "";
                var company_code = model["COMPANY_CODE"]?.ToString() ?? "";
                using (var db = ConnectionModel.GymShopDataContext())
                {
                    if(userMaster == "admin" && company_code == "VMT")
                    {
                        list = JArray.FromObject(
                        db.USERs.Where(x => (username == "" || x.USERNAME == username)
                                            && (user_fullname == "" || x.FULLNAME == user_fullname))
                        .Select(s => new
                        {
                            s.USERID,
                            s.USERNAME,
                            s.FULLNAME,
                            s.PASSWORDHASH,
                            s.ROLE,
                            s.ISACTIVE,
                            s.COMPANY_CODE,
                            COMPANY_NAME = db.COMPANies.Where(x => x.COMPANY_CODE == s.COMPANY_CODE).Select(v => v.COMPANY_NAME).FirstOrDefault()
                        }).ToList());
                    }
                    else
                    {
                        list = JArray.FromObject(
                        db.USERs.Where(x => (username == "" || x.USERNAME == username)
                                            && (user_fullname == "" || x.FULLNAME == user_fullname) && x.COMPANY_CODE == company_code)
                        .Select(s => new
                        {
                            s.USERID,
                            s.USERNAME,
                            s.FULLNAME,
                            s.PASSWORDHASH,
                            s.ROLE,
                            s.ISACTIVE,
                            s.COMPANY_CODE,
                            COMPANY_NAME = db.COMPANies.Where(x => x.COMPANY_CODE == s.COMPANY_CODE).Select(v => v.COMPANY_NAME).FirstOrDefault()
                        }).ToList());
                    }
                }
            }
            catch { }
            return list;
        }

        public JObject fnPostUser(JObject model)
        {
            var result = new JObject();
            result["ErrCode"] = "0";
            result["ErrMsg"] = "";
            result["ErrBack"] = "0";
            try
            {
                var action = model["ACTION"]?.ToString() ?? "";
                int userid = model["USERID"]?.ToObject<int>() ?? 0;
                var username = model["USERNAME"]?.ToString() ?? "";
                var fullname = model["FULLNAME"]?.ToString() ?? "";
                var role = model["ROLE"]?.ToString() ?? "1";
                var status = model["STATUS_ID"]?.ToString() ?? "1";
                var company_code = model["COMPANY_CODE"]?.ToString() ?? "";
                string pass = "456789";
                using (var db = ConnectionModel.GymShopDataContext())
                {
                    if (action == "INSERT")
                    {
                        var userCheck = db.USERs.Where(x => x.USERNAME == username && x.COMPANY_CODE == company_code).FirstOrDefault();
                        if(userCheck != null)
                        {
                            result["ErrCode"] = "0";
                            result["ErrMsg"] = $"Tài khoản đã tồn tại";
                            result["ErrBack"] = $"{username}";
                            return result;
                        }
                        var user = new USER();
                        user.COMPANY_CODE = company_code;
                        user.USERNAME = username;
                        user.PASSWORDHASH = ConnectionModel.MaHoa(pass);
                        user.FULLNAME = fullname;
                        user.ROLE = role;
                        user.ISACTIVE = Convert.ToBoolean(status);
                        

                        db.USERs.InsertOnSubmit(user);
                        db.SubmitChanges();

                        result["ErrCode"] = "1";
                        result["ErrMsg"] = $"Insert Success";
                        result["ErrBack"] = $"{company_code}";
                    }
                    else if (action == "EDIT")
                    {
                        var user = db.USERs.Where(x => x.USERID == userid).FirstOrDefault();
                        if (user != null)
                        {
                            user.FULLNAME = fullname;
                            user.ROLE = role;
                            user.ISACTIVE = Convert.ToBoolean(status);

                            db.SubmitChanges();

                            result["ErrCode"] = "1";
                            result["ErrMsg"] = $"Update Success";
                            result["ErrBack"] = $"{fullname}";
                        }
                    }
                    else
                    {

                        var user = db.USERs.Where(x => x.USERID == userid).FirstOrDefault();
                        if (user != null)
                        {
                            db.USERs.DeleteOnSubmit(user);
                            db.SubmitChanges();

                            result["ErrCode"] = "1";
                            result["ErrMsg"] = $"Delete Success";
                            result["ErrBack"] = $"{userid}";
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                result["ErrCode"] = "0";
                result["ErrMsg"] = $"{ex.ToString()}";
                result["ErrBack"] = "0";
            }
            return result;
        }
    }
}