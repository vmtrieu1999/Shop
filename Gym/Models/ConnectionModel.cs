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
using System.Net.Mail;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

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

        static public string GiaiMa(string _input)
        {
            string token = _input;
            string secret = apikey;
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    IJsonSerializer serializer = new JsonNetSerializer();
                    IDateTimeProvider provider = new UtcDateTimeProvider();
                    IJwtValidator validator = new JwtValidator(serializer, provider);
                    IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                    IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                    IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);

                    string decoded = decoder.Decode(token, secret, verify: true);
                    var obj = JObject.Parse(decoded);
                    return obj["password"]?.ToString() ?? string.Empty;
                }
                return string.Empty;
            }
            catch (TokenExpiredException)
            {
                Console.WriteLine("Token has expired");
                return string.Empty;
            }
            catch (SignatureVerificationException)
            {
                Console.WriteLine("Token has invalid signature");
                return string.Empty;
            }
        }

        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
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

        public async Task<JObject> SendEmail(string toEmail, string subject, string body, string company_code = "")
        {
            var result = new JObject();
            result["ErrCode"] = "0";
            result["ErrBack"] = "";
            result["ErrMsg"] = "";
            try
            {
                using (var db = ConnectionModel.GymShopDataContext())
                {
                    var company = db.COMPANies.Where(x => x.COMPANY_CODE == company_code).FirstOrDefault();
                    if (company != null)
                    {
                        //check mail
                        if (IsValidEmail(company.COMPANY_EMAIL))
                        {
                            var smtp = new SmtpClient
                            {
                                Host = "smtp.gmail.com",
                                Port = 587,
                                EnableSsl = true,
                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                Credentials = new NetworkCredential(company.COMPANY_EMAIL, GiaiMa(company.COMPANY_EMAIL_APP_PASS)),
                                Timeout = 20000
                            };

                            using (var message = new MailMessage(company.COMPANY_EMAIL, toEmail)
                            {
                                Subject = subject,
                                Body = body,
                                IsBodyHtml = true
                            })
                            {
                                await smtp.SendMailAsync(message);

                                result["ErrCode"] = "1";
                                result["ErrMsg"] = "Send mail success";
                                result["ErrBack"] = "1";
                                return result;
                            }
                        }
                        else
                        {
                            result["ErrCode"] = "0";
                            result["ErrMsg"] = "Invalid email address";
                            result["ErrBack"] = "0";
                            return result;
                        }
                    }
                    else
                    {
                        result["ErrCode"] = "0";
                        result["ErrMsg"] = "Company not found";
                        result["ErrBack"] = "0";
                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                result["ErrCode"] = "0";
                result["ErrMsg"] = e.ToString();
                result["ErrBack"] = "0";
                return result;
            }
        }
    }
}