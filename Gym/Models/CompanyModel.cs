using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;

namespace Gym.Models
{
    public class CompanyModel
    {
        public JArray fnGetCompany(JObject model)
        {
            var list = new JArray();
            try
            {
                var company_code = model["COMPANY_CODE"]?.ToString() ?? "";
                var comapany_name = model["COMPANY_NAME"]?.ToString() ?? "";
                using (var db = ConnectionModel.GymShopDataContext())
                {
                    list = JArray.FromObject(
                        db.COMPANies.Where(x => (comapany_name == "" || x.COMPANY_NAME == comapany_name) 
                                            && (company_code == "" || x.COMPANY_CODE == company_code))
                        .Select(s => new { 
                            s.COMPANY_ID,
                            s.COMPANY_NAME, 
                            s.COMPANY_CODE,
                            s.COMPANY_ADDRESS,
                            s.COMPANY_EMAIL,
                            s.COMPANY_PHONE,
                            s.CREATED_BY, 
                            s.CREATED_DATE,
                            COMPANY_EMAIL_APP_PASS = ConnectionModel.GiaiMa(s.COMPANY_EMAIL_APP_PASS),
                        }).ToList());
                }
            }
            catch { }
            return list;
        }

        public JObject fnPostCompany(JObject model)
        {
            var result = new JObject();
            result["ErrCode"] = "0";
            result["ErrMsg"] = "";
            result["ErrBack"] = "0";
            try
            {
                var action = model["ACTION"]?.ToString() ?? "";
                int company_id = model["COMPANY_ID"]?.ToObject<int>() ?? 0;
                var company_code = model["COMPANY_CODE"]?.ToString() ?? "";
                var comapany_name = model["COMPANY_NAME"]?.ToString() ?? "";
                var company_address = model["COMPANY_ADDRESS"]?.ToString() ?? "";
                var company_email = model["COMPANY_EMAIL"]?.ToString() ?? "";
                var company_phone = model["COMPANY_PHONE"]?.ToString() ?? "";
                var company_email_app_pass = model["COMPANY_EMAIL_APP_PASS"]?.ToString() ?? "";
                var dnow = DateTime.Now;
                var user = model["USER"]?.ToString() ?? "";
                using (var db = ConnectionModel.GymShopDataContext())
                {
                    if (action == "INSERT")
                    {
                        company_code = string.Concat(
                            Regex.Replace(comapany_name.Normalize(NormalizationForm.FormD), @"\p{Mn}", "")
                                 .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(w => w[0])
                        ).ToUpper() + DateTime.Now.ToString("yyyyMMddHHmmss");

                        var company = new COMPANY();
                        company.COMPANY_CODE = company_code;
                        company.COMPANY_NAME = comapany_name;
                        company.COMPANY_ADDRESS = company_address;
                        company.COMPANY_EMAIL = company_email;
                        company.COMPANY_EMAIL_APP_PASS = ConnectionModel.MaHoa(company_email_app_pass);
                        company.COMPANY_PHONE = company_phone;
                        company.CREATED_DATE = dnow;
                        company.CREATED_BY = user;

                        db.COMPANies.InsertOnSubmit(company);
                        db.SubmitChanges();

                        result["ErrCode"] = "1";
                        result["ErrMsg"] = $"Insert Success";
                        result["ErrBack"] = $"{company_code}";
                    }
                    else if (action == "EDIT")
                    {
                        var company = db.COMPANies.Where(x => x.COMPANY_ID == company_id).FirstOrDefault();
                        if(company != null)
                        {
                            company.COMPANY_NAME = comapany_name;
                            company.COMPANY_ADDRESS = company_address;
                            company.COMPANY_EMAIL = company_email;
                            company.COMPANY_EMAIL_APP_PASS = ConnectionModel.MaHoa(company_email_app_pass);
                            company.COMPANY_PHONE = company_phone;

                            db.SubmitChanges();

                            result["ErrCode"] = "1";
                            result["ErrMsg"] = $"Update Success";
                            result["ErrBack"] = $"{company_code}";
                        }
                    }
                    else
                    {
                        
                        var company = db.COMPANies.Where(x => x.COMPANY_ID == company_id).FirstOrDefault();
                        if (company != null)
                        {
                            db.COMPANies.DeleteOnSubmit(company);
                            db.SubmitChanges();

                            result["ErrCode"] = "1";
                            result["ErrMsg"] = $"Delete Success";
                            result["ErrBack"] = $"{company_id}";
                        }
                        
                    }
                }
            }
            catch(Exception ex) 
            {
                result["ErrCode"] = "0";
                result["ErrMsg"] = $"{ex.ToString()}";
                result["ErrBack"] = "0";
            }
            return result;
        }
    }
}