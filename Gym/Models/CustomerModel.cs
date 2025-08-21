using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;

namespace Gym.Models
{
    public class CustomerModel
    {
        public JArray fnGetCustomer(JObject model)
        {
            var list = new JArray();
            try
            {
                DateTime dnow = DateTime.Now;
                var room_code = (model["ROOM_CODE"] ?? "").ToString();
                var customer_code = (model["CUSTOMER_CODE"] ?? "").ToString();
                var customer_name = (model["CUSTOMER_NAME"] ?? "").ToString();
                var customer_cccd = (model["CUSTOMER_CCCD"] ?? "").ToString();
                var company_code = (model["COMPANY_CODE"] ?? "").ToString();
                using (var db = ConnectionModel.GymShopDataContext())
                {
                    list = JArray.FromObject(
                            db.CUSTOMERs
                            .Where(x => (customer_code == "" || x.CUSTOMER_CODE == customer_code) &&
                            (room_code == "" || x.ROOM_CODE == room_code) &&
                            (customer_name == "" || x.CUSTOMER_NAME == customer_name) &&
                            (customer_cccd == "" || x.CUSTOMER_CCCD == customer_cccd) && x.STATUS != "INACTIVE"
                            && x.COMPANY_CODE == company_code)
                            .Select(s => new
                            {
                                s.ROOM_CODE,
                                ROOM_NAME = db.ROOMs.Where(v=> v.ROOM_CODE == s.ROOM_CODE).Select(n=> n.ROOM_NAME).FirstOrDefault(),
                                s.CUSTOMER_ID,
                                s.CUSTOMER_CODE,
                                s.CUSTOMER_NAME,
                                s.CUSTOMER_EMAIL,
                                s.CUSTOMER_CCCD,
                                s.CUSTOMER_PHONE,
                                s.CUSTOMER_GENDER,
                                s.CUSTOMER_ADDRESS,
                                s.CUSTOMER_EXPIRYDATE,
                                STATUS = s.CUSTOMER_EXPIRYDATE == null
                                        ? "PROCESSING"
                                        : (s.CUSTOMER_EXPIRYDATE < dnow ? "OBSOLETE" : "ACTIVE"),
                                s.NOTE,
                                s.CREATE_DATE,
                                CREATE_USER = db.USERs.Where(x => x.USERNAME == s.CREATE_USER).Select(v => v.FULLNAME).FirstOrDefault(),
                                s.LAST_UPDATE_DATE, 
                                s.LAST_UPDATE_USER,
                            }).OrderBy(s => s.CUSTOMER_NAME).ToList()

                        );
                }
            }
            catch { }
            return list;
        }

        public JArray fnGetCustomerShip(JObject model)
        {
            var list = new JArray();
            try
            {
                var customer_code = (model["CUSTOMER_CODE"] ?? "").ToString();
                var company_code = (model["COMPANY_CODE"] ?? "").ToString();
                using (var db = ConnectionModel.GymShopDataContext())
                {
                    list = JArray.FromObject(
                             db.CUSTOMER_SHIPs
                            .Where(x => (customer_code == "" || x.CUSTOMER_CODE == customer_code) && x.COMPANY_CODE == company_code)
                            .Select(s => new
                            {
                                s.CUSTOMER_SHIPID,
                                CUSTOMER_NAME = db.CUSTOMERs.Where(x=> x.CUSTOMER_CODE == s.CUSTOMER_CODE).Select(x => x.CUSTOMER_NAME).FirstOrDefault(),
                                s.CUSTOMER_CODE,
                                PACKAGE_NAME = db.PACKAGEs.Where(x => x.PACKAGEID == s.PACKAGEID).Select(x => x.PACKAGENAME).FirstOrDefault(),
                                s.PACKAGEID,
                                s.STARTDATE,
                                s.ENDDATE,
                                s.TOTALPRICE,
                                s.CREATEDAT,
                                s.CREATE_USER,
                                s.PAYMENT,
                                s.PAYMENT_DATE,
                                s.NOTE,
                            }).OrderBy(s => s.CREATEDAT).ToList()

                        );
                }
            }
            catch { }
            return list;
        }

        public JObject fnPostCustomerShip(JObject model)
        {
            var result = new JObject();
            result["ErrCode"] = "0";
            result["ErrBack"] = "";
            result["ErrMsg"] = "";
            try
            {
                DateTime dnow = DateTime.Now;
                var customer_ship_id = (model["CUSTOMER_SHIPID"] ?? 0).ToObject<int?>() ?? 0;
                var customer_code = (model["CUSTOMER_CODE"] ?? "").ToString();
                var action = (model["ACTION"] ?? "").ToString();
                var package_id = (model["PACKAGE_ID"] ?? 0).ToObject<int?>() ?? 0;
                var fromDate = DateTime.TryParse(model["STARTDATE"]?.ToString(), out var fd) ? fd.Date : dnow;
                var toDate = DateTime.TryParse(model["ENDDATE"]?.ToString(), out var td) ? td.Date : dnow;
                var note = (model["NOTE"] ?? "").ToString();
                var payment = (model["PAYMENT"] ?? "").ToString();
                var user = (model["USER"] ?? "").ToString();
                var payment_date = DateTime.TryParse(model["PAYMENT_DATE"]?.ToString(), out var pd) ? pd.Date : dnow;
                var company_code = (model["COMPANY_CODE"] ?? "").ToString();
                using (var db = ConnectionModel.GymShopDataContext())
                {
                    var package = db.PACKAGEs.Where(x => x.PACKAGEID == package_id).FirstOrDefault();
                    if (package == null)
                    {
                        result["ErrCode"] = "0";
                        result["ErrMsg"] = "Chưa chọn gói";
                        result["ErrBack"] = customer_code;
                        return result;
                    }
                    fromDate = payment_date;
                    toDate = payment_date.AddDays(package.DURATIONDAYS);

                    var customer = db.CUSTOMERs.Where(x => x.CUSTOMER_CODE == customer_code).FirstOrDefault();
                    if (customer == null)
                    {
                        result["ErrCode"] = "0";
                        result["ErrMsg"] = "Không tìm thấy khách hàng";
                        result["ErrBack"] = customer_code;
                        return result;
                    }

                    if (action == "ADD")
                    {
                        customer.CUSTOMER_EXPIRYDATE = toDate;
                        customer.LAST_UPDATE_DATE = dnow;
                        customer.LAST_UPDATE_USER = user;

                        var item_ship = new CUSTOMER_SHIP();
                        item_ship.CUSTOMER_CODE = customer_code;
                        item_ship.PACKAGEID = package_id;
                        item_ship.STARTDATE = fromDate;
                        item_ship.ENDDATE = toDate;
                        item_ship.TOTALPRICE = package.PRICE;
                        item_ship.CREATEDAT = dnow;
                        item_ship.CREATE_USER = user;
                        item_ship.PAYMENT_DATE = payment_date;
                        item_ship.PAYMENT = payment;
                        item_ship.NOTE = note;
                        item_ship.COMPANY_CODE = company_code;

                        db.CUSTOMER_SHIPs.InsertOnSubmit(item_ship);
                        db.SubmitChanges();

                        result["ErrCode"] = "1";
                        result["ErrMsg"] = "Success";
                        result["ErrBack"] = customer_code;
                    }
                    else if (action == "EDIT")
                    {
                        var ship = db.CUSTOMER_SHIPs.FirstOrDefault(x => x.CUSTOMER_SHIPID == customer_ship_id);
                        if (ship != null)
                        {
                            customer.CUSTOMER_EXPIRYDATE = toDate;
                            customer.LAST_UPDATE_DATE = dnow;
                            customer.LAST_UPDATE_USER = user;

                            ship.PACKAGEID = package_id;
                            ship.STARTDATE = fromDate;
                            ship.ENDDATE = toDate;
                            ship.TOTALPRICE = package.PRICE;
                            ship.PAYMENT_DATE = payment_date;
                            ship.PAYMENT = payment;
                            ship.LAST_UPDATE_DATE = dnow;
                            ship.LAST_UPDATE_USER = user;
                            ship.NOTE = note;


                            db.SubmitChanges();

                            result["ErrCode"] = "1";
                            result["ErrMsg"] = "Success";
                            result["ErrBack"] = customer_code;
                        }
                        else
                        {
                            result["ErrCode"] = "0";
                            result["ErrMsg"] = "Không tìm thấy thành viên";
                            result["ErrBack"] = customer_code;
                        }
                    }
                    else
                    {
                        var ship = db.CUSTOMER_SHIPs.FirstOrDefault(x => x.CUSTOMER_SHIPID == customer_ship_id);
                        if (ship != null)
                        {
                            db.CUSTOMER_SHIPs.DeleteOnSubmit(ship);
                            db.SubmitChanges();
                        }
                        else
                        {
                            result["ErrCode"] = "0";
                            result["ErrMsg"] = "Không tìm thấy thành viên";
                            result["ErrBack"] = customer_code;

                        }
                    }
                }
            }
            catch (Exception e)
            {
                result["ErrCode"] = "0";
                result["ErrMsg"] = e.ToString();
            }
            return result;
        }

        public JObject fnPostCustomer(JObject model)
        {
            var result = new JObject();
            result["ErrCode"] = 0;
            result["ErrBack"] = "";
            result["ErrMsg"] = "";
            try
            {
                DateTime dnow = DateTime.Now;
                var action = (model["ACTION"] ?? "").ToString();
                var customer_code = (model["CUSTOMER_CODE"] ?? "").ToString();
                var fullname = (model["CUSTOMER_NAME"] ?? "").ToString();
                var mail = (model["CUSTOMER_EMAIL"] ?? "").ToString();
                var phone = (model["CUSTOMER_PHONE"] ?? "").ToString();
                var cccd = (model["CUSTOMER_CCCD"] ?? "").ToString();
                var gender = (model["CUSTOMER_GENDER"] ?? "").ToString();
                var address = (model["CUSTOMER_ADDRESS"] ?? "").ToString();
                var note = (model["NOTE"] ?? "").ToString();
                var room_code = (model["ROOM_CODE"] ?? "").ToString();
                var status = (model["STATUS"] ?? "").ToString();
                var user = (model["USER"] ?? "").ToString();
                var company_code = (model["COMPANY_CODE"] ?? "").ToString();
                using (var db = ConnectionModel.GymShopDataContext())
                {
                    if (action == "INSERT")
                    {
                            customer_code = string.Concat(
                            Regex.Replace(fullname.Normalize(NormalizationForm.FormD), @"\p{Mn}", "")
                                 .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(w => w[0])
                        ).ToUpper() + DateTime.Now.ToString("yyyyMMddHHmmss");

                        var item = new CUSTOMER();
                        item.ROOM_CODE = room_code;
                        item.CUSTOMER_CODE = customer_code;
                        item.CUSTOMER_NAME = fullname.Trim();
                        item.CUSTOMER_EMAIL = mail.Trim();
                        item.CUSTOMER_PHONE = phone.Trim();
                        item.CUSTOMER_CCCD = cccd.Trim();
                        item.CUSTOMER_GENDER = gender.Trim();
                        item.CUSTOMER_ADDRESS = address.Trim();
                        item.CUSTOMER_PHOTOURL = "";
                        item.STATUS = "PROCESSING";
                        item.NOTE = note.Trim() ?? "";
                        item.CREATE_DATE = dnow;
                        item.CREATE_USER = user;
                        item.COMPANY_CODE = company_code;

                        db.CUSTOMERs.InsertOnSubmit(item);

                        db.SubmitChanges();
                        result["ErrCode"] = "1";
                        result["ErrMsg"] = "Success";
                        result["ErrBack"] = customer_code;


                    }
                    else if(action == "EDIT")
                    {
                        var customer = db.CUSTOMERs.FirstOrDefault(x => x.CUSTOMER_CODE == customer_code);
                        if(customer != null)
                        {
                            customer.ROOM_CODE = room_code;
                            customer.CUSTOMER_NAME = fullname.Trim();
                            customer.CUSTOMER_EMAIL = mail.Trim();
                            customer.CUSTOMER_PHONE = phone.Trim();
                            customer.CUSTOMER_CCCD = cccd.Trim();
                            customer.CUSTOMER_GENDER = gender.Trim();
                            customer.CUSTOMER_ADDRESS = address.Trim();
                            customer.NOTE = note.Trim();
                            customer.LAST_UPDATE_DATE = dnow;
                            customer.LAST_UPDATE_USER = user;
                            customer.COMPANY_CODE = company_code;

                            db.SubmitChanges();

                            result["ErrCode"] = "1";
                            result["ErrMsg"] = "Success";
                            result["ErrBack"] = customer_code;
                        }
                        else
                        {
                            result["ErrCode"] = "0";
                            result["ErrMsg"] = "Không tìm thấy thành viên";
                            result["ErrBack"] = customer_code;

                        }

                    }
                    else
                    {
                        var customer = db.CUSTOMERs.FirstOrDefault(x => x.CUSTOMER_CODE == customer_code);
                        if(customer != null)
                        {
                            customer.STATUS = "INACTIVE";
                            customer.LAST_UPDATE_DATE = dnow;
                            customer.LAST_UPDATE_USER = user;

                            db.SubmitChanges();

                            result["ErrCode"] = "1";
                            result["ErrMsg"] = "Success";
                            result["ErrBack"] = customer_code;
                        }
                        else
                        {
                            result["ErrCode"] = "0";
                            result["ErrMsg"] = "Không tìm thấy thành viên";
                            result["ErrBack"] = customer_code;

                        }
                    }

                    
                    
                }
            }
            catch (Exception e)
            {
                result["ErrCode"] = "0";
                result["ErrMsg"] = e.ToString();
            }
            return result;
        }
    }
}