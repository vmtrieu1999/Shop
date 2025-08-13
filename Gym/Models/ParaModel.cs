using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gym.Models
{
    public class ParaModel
    {
        public string strPara { get; set; }
        public string returnUrl { get; set; }

        public ParaModel()
        {
            this.strPara = "";
            this.returnUrl = "";
        }
    }
}
