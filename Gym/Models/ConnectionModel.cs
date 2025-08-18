using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Helpers;

namespace Gym
{
    public class ConnectionModel
    {
        private static string GymShopConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["gymshopConnectionString"].ConnectionString;
        public static GymShopDataContext GymShopDataContext()
        {
            var strConnection = GymShopConnectionString;
            return new GymShopDataContext(strConnection);
        }

        public UserSession fnLoginUser(string username = "", string password = "")
        {
            try
            {
                using (var db = GymShopDataContext())
                {
                    var user = db.USERs.FirstOrDefault(u => u.USERNAME == username && u.PASSWORDHASH == password);
                    if (user == null)
                        return null;
                    else
                    {
                        return new UserSession(
                        user.USERID,
                        user.USERNAME,
                        user.ROLE,
                        user.ISACTIVE ?? false,
                        user.COMPANY_CODE ?? ""
                        );
                    }
                    
                }
            }
            catch
            {
                return null;
            }
        }
    }
}