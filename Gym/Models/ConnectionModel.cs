using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Helpers;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using JWT.Exceptions;

namespace Gym
{
    public class ConnectionModel
    {
        private static string GymShopConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["gymshopConnectionString"].ConnectionString;
        private const string apikey = "GCRWFJESOZCOVVWYHKALFDMEJJCYVDCO";

        public static GymShopDataContext GymShopDataContext()
        {
            var strConnection = GymShopConnectionString;
            return new GymShopDataContext(strConnection);
        }

        public static string MaHoa(string plainText)
        {
            try
            {
                string secret = apikey;
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                IJsonSerializer serializer = new JsonNetSerializer();
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

                var payload = new Dictionary<string, object>
                {
                    { "password", plainText }
                };

                return encoder.Encode(payload, secret);
            }
            catch
            {
                return string.Empty;
            }
        }

        public UserSession fnLoginUser(string username = "", string password = "")
        {
            try
            {
                using (var db = GymShopDataContext())
                {
                    var user = db.USERs.FirstOrDefault(u => u.USERNAME == username && u.PASSWORDHASH == password && u.ISACTIVE == true);
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