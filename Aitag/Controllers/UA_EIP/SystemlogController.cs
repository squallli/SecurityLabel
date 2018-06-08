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
    public class systemlogController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /systemlog/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult csvsystemlogout(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "slid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcomid = "", qsflag = "", qslaccount = "", qsltext="", qsodate1 = "", qsodate2 = "";
            
            if (!string.IsNullOrWhiteSpace(Request["qcomid"])) {
                qcomid = Request["qcomid"].Trim();
                ViewBag.qcomid = qcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsflag"])) {
                qsflag = Request["qsflag"].Trim();
                ViewBag.qsflag = qsflag;
            }
            if (!string.IsNullOrWhiteSpace(Request["qslaccount"])) {
                qslaccount = Request["qslaccount"].Trim();
                ViewBag.qslaccount = qslaccount;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsltext"]))
            {
                qsltext = Request["qsltext"].Trim();
                ViewBag.qsltext = qsltext;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsodate1"]))
            {
                qsodate1 = Request["qsodate1"].Trim();
                ViewBag.qsodate1 = qsodate1;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsodate2"]))
            {
                qsodate2 = Request["qsodate2"].Trim();
                ViewBag.qsodate2 = qsodate2;
            }

            //IPagedList<systemlog> result;
            string Excel = "", Excel2 = "";
            string sqlstr = "";
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                sqlstr = "select * from systemlog where";

                if (qcomid != "")
                    sqlstr += " comid = '" + qcomid + "'  and";
                if (qsflag != "")
                    sqlstr += " sflag = '" + qsflag + "'  and";
                if (qslaccount != "")
                    sqlstr += " slaccount = '" + qslaccount + "'  and";
                if (qsltext != "")
                    sqlstr += " (slevent like N'%" + qsltext + "%' or sname like N'%" + qsltext + "%')  and";
                if (qsodate1 == "")
                {
                    qsodate1 = DateTime.Now.ToString("yyyy") + "/1/1";
                }
                if (qsodate2 == "") 
                {
                    DateTime date2 = new DateTime(DateTime.Now.Year+1 , 1, 1);
                    date2 = date2.AddDays(-1);
                    qsodate2 = date2.ToString("yyyy/MM/dd");
                }
                

                string DateEx = "";
                try
                {
                    DateTime.Parse(qsodate1);
                    sqlstr += " sodate >= '" + qsodate1 + "'  and";
                }
                catch
                {
                    DateEx += @"異動時間起格式錯誤!!\n";
                }
                try
                {
                    DateTime.Parse(qsodate2);
                    sqlstr += " sodate <= '" + qsodate2 + "'  and";
                }
                catch
                {
                    DateEx += @"異動時間訖格式錯誤!!\n";
                }
                if (DateEx != "" ) 
                {
                    ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>";
                }

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                //var query = con.systemlog.SqlQuery(sqlstr).AsQueryable();
                //result = query.ToPagedList<systemlog>(0, 10000);


            }


            Excel += "<HTML>";
            Excel += "<HEAD>";
            Excel += @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">";
            Excel += "</HEAD>";
            Excel += "<body>";
            Excel += "<table  border=1  cellpadding=0 cellspacing=0 bordercolor=#000000 bordercolordark=#ffffff width=900 >";
            Excel += "<tr align=center>";
            Excel += "<td>異動狀態</td>";
            Excel += "<td>使用者帳號</td>";
            Excel += "<td>使用功能</td>";
            Excel += "<td>內容</td>";
            Excel += "<td>登入IP</td>";
            Excel += "<td>異動時間</td>";
            Excel += "</tr>";

            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    string sflag = "", empname = "";
                    while (dr.Read())
                    {
                        switch (dbobj.get_dbnull2(dr["sflag"]))
                        {
                            case "A":
                                sflag = "新增";
                                break;
                            case "M":
                                sflag = "修改";
                                break;
                            case "D":
                                sflag = "刪除";
                                break;
                            case "L":
                                sflag = "登入";
                                break;
                            default:
                                sflag = "";
                                break;
                        }
                        empname = dbobj.get_dbvalue(conn, "select empname from employee where empid='" + dbobj.get_dbnull2(dr["slaccount"]) + "'");



                        Excel2 += "<tr>";
                        Excel2 += "<td>" + sflag.Trim() + "</td>";
                        Excel2 += "<td>" + empname.Trim() + "</td>";
                        Excel2 += "<td>" + dbobj.get_dbnull2((dr["sname"])).Trim() + "</td>";
                        Excel2 += "<td>" + dbobj.get_dbnull2((dr["slevent"])).Trim() + "</td>";
                        Excel2 += "<td>" + dbobj.get_dbnull2((dr["sfromip"])).Trim() + "</td>";
                        Excel2 += "<td>" + dr["sodate"] + "</td>";
                        Excel2 += "</tr>";
                    }
                    if (Excel2 == "")
                    {
                        Excel += "<tr align=left><td colspan=6>目前沒有資料</td></tr>";
                    }
                    else
                    {
                        Excel += Excel2;
                    }
                    dr.Close();
                }
            }


            Excel += "</table>";
            Excel += "</body>";
            Excel += "</HTML>";



            ViewBag.Excel = Excel;
            return View();
        }
        public ActionResult systemlogshow()
        {
            return View();
        }

        public ActionResult List(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "slid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcomid = "", qsflag = "", qslaccount = "", qsltext="", qsodate1 = "", qsodate2 = "";
            
            if (!string.IsNullOrWhiteSpace(Request["qcomid"])) {
                qcomid = Request["qcomid"].Trim();
                ViewBag.qcomid = qcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsflag"])) {
                qsflag = Request["qsflag"].Trim();
                ViewBag.qsflag = qsflag;
            }
            if (!string.IsNullOrWhiteSpace(Request["qslaccount"])) {
                qslaccount = Request["qslaccount"].Trim();
                ViewBag.qslaccount = qslaccount;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsltext"]))
            {
                qsltext = Request["qsltext"].Trim();
                ViewBag.qsltext = qsltext;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsodate1"]))
            {
                qsodate1 = Request["qsodate1"].Trim();
                ViewBag.qsodate1 = qsodate1;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsodate2"]))
            {
                qsodate2 = Request["qsodate2"].Trim();
                ViewBag.qsodate2 = qsodate2;
            }

            IPagedList<systemlog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from systemlog where";

                if (qcomid != "")
                    sqlstr += " comid = '" + qcomid + "'  and";
                if (qsflag != "")
                    sqlstr += " sflag = '" + qsflag + "'  and";
                if (qslaccount != "")
                    sqlstr += " slaccount = '" + qslaccount + "'  and";
                if (qsltext != "")
                    sqlstr += " (slevent like N'%" + qsltext + "%' or sname like N'%" + qsltext + "%')  and";
                if (qsodate1 == "")
                {
                    qsodate1 = DateTime.Now.ToString("yyyy") + "/1/1";
                    ViewBag.qsodate1 = qsodate1;
                }
                if (qsodate2 == "") 
                {
                    DateTime date2 = new DateTime(DateTime.Now.Year+1 , 1, 1);
                    date2 = date2.AddDays(-1);
                    qsodate2 = date2.ToString("yyyy/MM/dd");
                    ViewBag.qsodate2 = qsodate2;
                }
                

                string DateEx = "";
                try
                {
                    DateTime.Parse(qsodate1);
                    sqlstr += " sodate >= '" + qsodate1 + "'  and";
                }
                catch
                {
                    DateEx += @"異動時間起格式錯誤!!\n";
                }
                try
                {
                    DateTime.Parse(qsodate2);
                    sqlstr += " sodate <= '" + qsodate2 + "'  and";
                }
                catch
                {
                    DateEx += @"異動時間訖格式錯誤!!\n";
                }
                if (DateEx != "" ) 
                {
                    ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>";
                }

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.systemlog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<systemlog>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch(orderdata, orderdata1);
            return View(result);
        }

        private string SetOrder_ch(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "sflag", "slaccount", "sname", "sfromip" };
            string[] od_ch1 = { "asc", "asc", "asc", "asc" };
            Order_ch += @"var orderdata = '" + orderdata + "';";
            Order_ch += @"var orderdata1 = '" + orderdata1 + "';";

            Order_ch += @"var od_ch = new Array(""""";
            foreach (string i in od_ch){Order_ch += @", '" + i + "'";}
            Order_ch += @");";

            Order_ch += @"var od_ch1 = new Array(""""";
            foreach (string i in od_ch1) { Order_ch += @", '" + i + "'"; }
            Order_ch += @");";

            Order_ch += @"switch(orderdata){ ";
            int ii = 0;
            foreach (string i in od_ch) {
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




        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {

            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qcomid = "", qsflag = "", qslaccount = "", qsltext = "", qsodate1 = "", qsodate2 = "";
            string sql ="", sqlwhere= "";
            string maxcomid = System.Configuration.ConfigurationManager.AppSettings["maxcomid"].ToString();

            if (maxcomid == (string)(Session["maxcomid"]) || (string)(Session["maxcomid"]) == null)
            {
                sql = "delete from systemlog where";
            }
            else
            {
                sql = "delete from systemlog where comid='" + (string)(Session["maxcomid"]) + "'  and";
            }


            if (!string.IsNullOrWhiteSpace(Request["qcomid"]))
            {
                qcomid = Request["qcomid"].Trim();
                sqlwhere += " comid = '" + qcomid + "'  and";
                //ViewBag.qcomid = qcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsflag"]))
            {
                qsflag = Request["qsflag"].Trim();
                sqlwhere += " sflag = '" + qsflag + "'  and";
                //ViewBag.qsflag = qsflag;
            }
            if (!string.IsNullOrWhiteSpace(Request["qslaccount"]))
            {
                qslaccount = Request["qslaccount"].Trim();
                sqlwhere += " slaccount = '" + qslaccount + "'  and";
                //ViewBag.qslaccount = qslaccount;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsltext"]))
            {
                qsltext = Request["qsltext"].Trim();
                sqlwhere += " (slevent like N'%" + qsltext + "%' or sname like N'%" + qsltext + "%')  and";
                //ViewBag.qsltext = qsltext;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsodate1"]))
            {
                qsodate1 = Request["qsodate1"].Trim();
                //ViewBag.qsodate1 = qsodate1;
            }
            else 
            {
                qsodate1 = DateTime.Now.ToString("yyyy") + "/1/1";
            }
            if (!string.IsNullOrWhiteSpace(Request["qsodate2"]))
            {
                qsodate2 = Request["qsodate2"].Trim();
                //ViewBag.qsodate2 = qsodate2;
            }
            else
            {
                DateTime date2 = new DateTime(DateTime.Now.Year + 1, 1, 1);
                date2 = date2.AddDays(-1);
                qsodate2 = date2.ToString("yyyy/MM/dd");
            }

            string DateEx = "";
            try
            {
                DateTime.Parse(qsodate1);
                sqlwhere += " sodate >= '" + qsodate1 + "'  and";
            }
            catch
            {
                DateEx += @"異動時間起格式錯誤!!\n";
            }
            try
            {
                DateTime.Parse(qsodate2);
                sqlwhere += " sodate <= '" + qsodate2 + "'  and";
            }
            catch
            {
                DateEx += @"異動時間訖格式錯誤!!\n";
            }
            if (DateEx != "")
            {
                ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>";
            }
            else
            {
                sqlwhere = sqlwhere.Substring(0, sqlwhere.Length - 5);
                sql = sql + sqlwhere;
                NDcommon dbobj = new NDcommon();
                dbobj.dbexecute("Aitag_DBContext", sql);
            }



            string tmpform = "";
            //tmpform += @"<script>alert('刪除成功!!');</script>";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/systemlog/List' method='post'>";

            tmpform += @"<input type=hidden name='qcomid' value='" + qcomid + "' >";
            tmpform += @"<input type=hidden name='qsflag' value='" + qsflag + "' >";
            tmpform += @"<input type=hidden name='qslaccount' value='" + qslaccount + "' >";
            tmpform += @"<input type=hidden name='qsltext' value='" + qsltext + "' >";
            tmpform += @"<input type=hidden name='qsodate1' value='" + qsodate1 + "' >";
            tmpform += @"<input type=hidden name='qsodate2' value='" + qsodate2 + "' >";
            
            //不回傳順序、頁碼
            tmpform += @"<input type=hidden name='page' value='' >";
            tmpform += @"<input type=hidden name='orderdata' value='' >";
            tmpform += @"<input type=hidden name='orderdata1' value='' >";
            tmpform += @"</form>";
            tmpform += @"</body>";

            return new ContentResult() { Content = tmpform };

        }



    }
}
