using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aitag.Models;
using System.Data;
using System.Data.SqlClient;
using Aitag.Filters;
using MvcPaging;


namespace Aitag.Controllers
{
    public class openwinController : BaseController
    {
        //
        // GET: /common/

        public ActionResult Index()
        {

            
            return View();
        }


        public ActionResult chooseemployee2(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "empworkdepid,empid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qempname = "", qdptid = "";

            if (!string.IsNullOrWhiteSpace(Request["qdptid"]))
            {
                qdptid = Request["qdptid"].Trim();
                ViewBag.qdptid = qdptid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempname"]))
            {
                qempname = Request["qempname"].Trim();
                ViewBag.qempname = qempname;
            }

            IPagedList<employee> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from employee where empstatus <>'4' and empworkcomp='" + (string)Session["comid"] + "'   and";


                if (qdptid != "")
                    sqlstr += " empworkdepid = '" + qdptid + "'   and";
                if (qempname != "")
                    sqlstr += " empname like N'%" + qempname + "%'   and";


                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.employee.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<employee>(page.Value - 1, 10);

            }
            return View(result);
        }

         

    }
}
