using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aitag.Models;
using System.Data;
using System.Data.SqlClient;
using Aitag.Filters;


namespace Aitag.Controllers
{
     [DoAuthorizeFilter]
    public class mreportController : BaseController
    {
        //
        // GET: /main/

        public ActionResult Index()
        {

            
            return View();
        }

        public ActionResult m1()
        {


            return View();
        }

        public ActionResult m2()
        {


            return View();
        }

        public ActionResult m3()
        {


            return View();
        }

        public ActionResult m4()
        {


            return View();
        }

        public ActionResult m5()
        {


            return View();
        }

    }
}
