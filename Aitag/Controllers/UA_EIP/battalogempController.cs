using MvcPaging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aitag.Models;
using Aitag.Filters;
using System.Collections.ObjectModel;
using System.Data.Entity;

namespace Aitag.Controllers
{
    [DoAuthorizeFilter]
    public class battalogempController : BaseController
    {
        public string mfrom = System.Configuration.ConfigurationManager.AppSettings["mail_from"].ToString();
        string DateEx = "";
        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /battalog/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

       
        public ActionResult List(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "bsno"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qblogstatus = "", qempname = "", qdptid = "", qblogsdate = "", qblogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qblogstatus"]))
            {
                qblogstatus = Request["qblogstatus"].Trim();
                ViewBag.qblogstatus = qblogstatus;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempname"]))
            {
                qempname = Request["qempname"].Trim();
                ViewBag.qempname = qempname;
            }
            if (!string.IsNullOrWhiteSpace(Request["qdptid"]))
            {
                qdptid = Request["qdptid"].Trim();
                ViewBag.qdptid = qdptid;
            }
            qblogsdate = NullStDate(Request["qblogsdate"]);
            ViewBag.qblogsdate = qblogsdate;
            qblogedate = NullTeDate(Request["qblogedate"]);
            ViewBag.qblogedate = qblogedate;
            //NullStDate 跟 NullTeDate 會判斷格式，有錯誤就 修改全域的DateEx
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }

            IPagedList<battalog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string viewid = "";
                string[] mpriv = (string[])Session["priv"];
                //viewid = get_viewpriv(int.Parse(funcpriv(2)), int.Parse(mpriv(realsid, 2)));

                string sqlstr = "select * from battalog where (blogtype='1' or (blogtype='2' and (pbsno='' or pbsno is null )) ) and comid='" + (string)Session["comid"] + "'";

                if (viewid != "")
                {
                    sqlstr += " and bmodid = '" + viewid + "'";
                }
                if (qblogstatus != "" && qblogstatus != "all")
                {
                    sqlstr += " and blogstatus = '" + qblogstatus + "'";
                }
                else if (qblogstatus == "")
                {
                    sqlstr += " and blogstatus = '1'";
                    ViewBag.qblogstatus = "1";

                }
                if (qempname != "")
                {
                    sqlstr += " and empname like N'%" + qempname + "%'";
                }
                if (qdptid != "")
                {
                    sqlstr += " and dptid='" + qdptid + "'";
                }

                sqlstr += " and (( blogsdate >= '" + qblogsdate + "' and blogsdate <= '" + qblogedate + "' ) or ";
                sqlstr += "( blogedate >= '" + qblogsdate + "' and blogedate <= '" + qblogedate + "'))";

                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.battalog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<battalog>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch(orderdata, orderdata1);
            return View(result);
        }
        private string SetOrder_ch(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "blogstatus", "ifhdell", "empid", "empname", "dptid", "blogsdate" };
            string[] od_ch1 = { "asc", "asc", "asc", "asc", "asc", "asc" };
            Order_ch += @"var orderdata = '" + orderdata + "';";
            Order_ch += @"var orderdata1 = '" + orderdata1 + "';";

            Order_ch += @"var od_ch = new Array(""""";
            foreach (string i in od_ch) { Order_ch += @", '" + i + "'"; }
            Order_ch += @");";

            Order_ch += @"var od_ch1 = new Array(""""";
            foreach (string i in od_ch1) { Order_ch += @", '" + i + "'"; }
            Order_ch += @");";

            Order_ch += @"switch(orderdata){ ";
            int ii = 0;
            foreach (string i in od_ch)
            {
                ii += 1;
                Order_ch += @"case""" + i + @""":od_ch1[" + ii + "]=orderdata1;break;";
            }
            Order_ch += @"};";

            ii = 0;
            foreach (string i in od_ch)
            {
                ii += 1;
                Order_ch += @"SetOrder_A('order" + ii + "', od_ch[" + ii + "], od_ch1[" + ii + "]);";
            }

            //Order_ch += @"";
            Order_ch += "  }  ";
            return SetOrder_A + Order_ch;
        }
        private string NullStDate(string stdate)
        {
            if (string.IsNullOrWhiteSpace(stdate))
            {
                stdate = DateTime.Now.ToString("yyyy/MM") + "/1";
            }
            try
            {
                DateTime.Parse(stdate);
            }
            catch
            {
                DateEx += @"出差日期起格式錯誤!!\n";
                stdate = "";
            }
            return stdate;
        }
        private string NullTeDate(string tedate)
        {
            if (string.IsNullOrWhiteSpace(tedate))
            {
                var dat = new DateTime(DateTime.Now.Year - 1, 12, 31);
                tedate = dat.AddMonths(DateTime.Now.Month).ToString("yyyy/MM/dd");
            }
            try
            {
                DateTime.Parse(tedate);
            }
            catch
            {
                DateEx += @"出差日期訖格式錯誤!!\n";
                tedate = "";
            }

            return tedate;
        }

        #region   getviewpriv  '取得觀看權限

        public string get_viewpriv(int dptval, int perval)
        {
            if (dptval == 0)
            {
                return "9999999999";//無
            }
            else if (dptval == 1)
            {
                //個人
                if (dptval > perval)
                {
                    return "9999999999";
                }
                else
                {
                    return (string)Session["empid"];
                }

            }
            else if (dptval == 2)
            {
                //全部
                if (dptval > perval)
                {
                    if(perval == 1)
                    {
                        return (string)Session["empid"];
                    }
                    else
                    {
                        return "9999999999";
                    }
                }
                else if (dptval == perval)
                {
                    return "";
                }
            }
            return "";
        }

        #endregion



    }
}
