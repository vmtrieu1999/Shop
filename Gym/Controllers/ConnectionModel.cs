using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    }
}