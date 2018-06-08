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
    public class mainController : BaseController
    {
        //
        // GET: /main/

        public ActionResult Index()
        {

            
            return View();
        }

    }
}
