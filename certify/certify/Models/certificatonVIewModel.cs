using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace certify.Models
{
    public class certificationViewModel
    {
        public looknumber lookNumber { get; set; }

        public certifyorder certifYorder { set; get; }

        public farmer Farmer { get; set; }

        public List<viewcertifycheckdet> viewCertifycheckdet { get; set; }
    }
}