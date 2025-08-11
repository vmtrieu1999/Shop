using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym.Models
{
    public class PackagesModel
    {
        public JArray fnGetPackages(JObject model)
        {
            var list = new JArray();
            try
            {
                var packages_name = model["PACKAGENAME"]?.ToString() ?? "";
                int day = model["DURATIONDAYS"]?.ToObject<int?>() ?? 0;
                using (var db = ConnectionModel.GymShopDataContext())
                {
                    list = JArray.FromObject(
                        db.PACKAGEs
                        .Where(x => (x.PACKAGENAME == packages_name || packages_name == "") &&
                        (x.DURATIONDAYS == day || day == 0)) 
                        .Select(s => new { 
                            s.PACKAGEID,
                            s.PACKAGENAME,
                            s.DURATIONDAYS,
                            s.PRICE
                        }).OrderBy(y => y.PACKAGENAME).ToList());
                }
            }
            catch { }
            return list;
        }

        public JObject fnPostPackages(JObject model)
        {
            var result = new JObject();
            result["ErrCode"] = "0";
            result["ErrMsg"] = "";
            result["ErrBack"] = "";
            try
            {
                var action = model["ACTION"]?.ToString() ?? "";
                int packages_id = model["PACKAGEID"]?.ToObject<int?>() ?? 0;
                var packages_name = model["PACKAGENAME"]?.ToString() ?? "";
                int day = model["DURATIONDAYS"]?.ToObject<int?>() ?? 0;
                decimal price = model["PRICE"]?.ToObject<decimal?>() ?? 0;

                using (var db = ConnectionModel.GymShopDataContext())
                {
                    if(action == "INSERT")
                    {
                        var packages = new PACKAGE();
                        packages.PACKAGENAME = packages_name;
                        packages.DURATIONDAYS = day;
                        packages.PRICE = price;

                        db.PACKAGEs.InsertOnSubmit(packages);
                    }
                    else if(action == "UPDATE")
                    {
                        var packages = db.PACKAGEs.Where(x => x.PACKAGEID == packages_id).FirstOrDefault();
                        packages.PACKAGENAME = packages_name;
                        packages.DURATIONDAYS = day;
                        packages.PRICE = price;
                    }
                    else
                    {
                        var packages = db.PACKAGEs.Where(x => x.PACKAGEID == packages_id).FirstOrDefault();
                        db.PACKAGEs.DeleteOnSubmit(packages);
                    }

                    db.SubmitChanges();
                    result["ErrCode"] = "1";
                    result["ErrMsg"] = "Success";
                    result["ErrBack"] = $"{packages_name}";
                }
            }
            catch(Exception e)
            {
                result["ErrCode"] = "0";
                result["ErrMsg"] = $"{e.ToString()}";
                result["ErrBack"] = "0";
            }
            return result;
        }
    }
}