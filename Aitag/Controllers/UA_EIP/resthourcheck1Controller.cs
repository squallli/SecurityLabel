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
    public class resthourcheck1Controller : BaseController
    {
        string DateEx = "";

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /resthourlog/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.rsid = Session["rsid"].ToString();
        //    resthourlog col = new resthourlog();
        //    return View(col);
        //}

        //[HttpPost]
        public ActionResult add(resthourlog col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "rsid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qdptid = "", qempname = "", qaddtype = "", qaddsdate = "", qaddedate = "";
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
            if (!string.IsNullOrWhiteSpace(Request["qaddtype"]))
            {
                qaddtype = Request["qaddtype"].Trim();
                ViewBag.qaddtype = qaddtype;
            }
            if (!string.IsNullOrWhiteSpace(Request["qaddsdate"]))
            {
                qaddsdate = Request["qaddsdate"].Trim();
                ViewBag.qaddsdate = qaddsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qaddedate"]))
            {
                qaddedate = Request["qaddedate"].Trim();
                ViewBag.qaddedate = qaddedate;
            }

            if (sysflag != "A")
            {
                resthourlog newcol = new resthourlog();
                return View(newcol);
            }
            else
            {

                if (!ModelState.IsValid)
                {
                    return View(col);
                }
                else
                {
                    Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
                    SqlConnection conn = dbobj.get_conn("Aitag_DBContext");
                    SqlDataReader dr;
                    SqlCommand sqlsmd = new SqlCommand();
                    sqlsmd.Connection = conn;
                    string sqlstr = "select rsid from resthourlog where rsid = '" + col.rsid + "'";
                    sqlsmd.CommandText = sqlstr;
                    dr = sqlsmd.ExecuteReader();

                    if (dr.Read())
                    {

                        ModelState.AddModelError("", "權限代碼重複!");
                        return View(col);
                    }
                    dr.Close();
                    dr.Dispose();
                    sqlsmd.Dispose();
                    conn.Close();
                    conn.Dispose();


                    col.otlogid = 0;
                    col.resmoney = 0;
                    col.inout = "0";
                    col.comid = Session["comid"].ToString();

                    col.bmodid = Session["tempid"].ToString();
                    col.bmoddate = DateTime.Now;
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.resthourlog.Add(col);
                        try
                        {
                            con.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }

                        

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "代碼:" + col.rsid + "名稱:" + col.empid;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/resthourcheck1/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qdptid' name='qdptid' value='" + qdptid + "'>";
                    tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
                    tmpform += "<input type=hidden id='qaddtype' name='qaddtype' value='" + qaddtype + "'>";
                    tmpform += "<input type=hidden id='qaddsdate' name='qaddsdate' value='" + qaddsdate + "'>";
                    tmpform += "<input type=hidden id='qaddedate' name='qaddedate' value='" + qaddedate + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    // return RedirectToAction("List");

                }
            }


        }


        [ValidateInput(false)]
        public ActionResult Edit(resthourlog chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "rsid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qdptid = "", qempname = "", qaddtype = "", qaddsdate = "", qaddedate = "";
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
            if (!string.IsNullOrWhiteSpace(Request["qaddtype"]))
            {
                qaddtype = Request["qaddtype"].Trim();
                ViewBag.qaddtype = qaddtype;
            }
            if (!string.IsNullOrWhiteSpace(Request["qaddsdate"]))
            {
                qaddsdate = Request["qaddsdate"].Trim();
                ViewBag.qaddsdate = qaddsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qaddedate"]))
            {
                qaddedate = Request["qaddedate"].Trim();
                ViewBag.qaddedate = qaddedate;
            }

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.resthourlog.Where(r => r.rsid == chks.rsid).FirstOrDefault();
                    resthourlog eresthourlogs = con.resthourlog.Find(chks.rsid);
                    if (eresthourlogs == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eresthourlogs);
                }

            }
            else
            {

                if (!ModelState.IsValid)
                {
                    string otlogstime = "";
                    Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
                    using(SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                    {
    //                    string tmpwhere = " where osno = '" + Model.osno + "'", otlogstime = "", otlogetime = "";
    //otlogstime = dbobj.get_dbvalue(comconn,"select otlogstime from otworklog" + tmpwhere);
    //otlogetime = dbobj.get_dbvalue(comconn,"select otlogetime from otworklog" + tmpwhere);
    //if (otlogstime != "" && otlogetime != "")
    //{
    //    @:@otlogstime ~ @otlogetime
    //}
                    }
                    return View(chks);
                }
                else
                {

                    //string oldrsid = Request["oldrsid"];                 

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        NDcommon dbobj = new NDcommon();
                        chks.rsid = int.Parse(Request["rsid"].Trim());
                        chks.bmodid = Session["tempid"].ToString();
                        chks.bmoddate = DateTime.Now;
                        if (chks.inout == null)
                        {
                            chks.inout = "";
                        }
                        con.Entry(chks).State = EntityState.Modified;
                        con.SaveChanges();


                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "代碼:" + chks.rsid + "名稱:" + chks.empid;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/resthourcheck1/List' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                        tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                        tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                        tmpform += "<input type=hidden id='qdptid' name='qdptid' value='" + qdptid + "'>";
                        tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
                        tmpform += "<input type=hidden id='qaddtype' name='qaddtype' value='" + qaddtype + "'>";
                        tmpform += "<input type=hidden id='qaddsdate' name='qaddsdate' value='" + qaddsdate + "'>";
                        tmpform += "<input type=hidden id='qaddedate' name='qaddedate' value='" + qaddedate + "'>";
                        tmpform += "</form>";
                        tmpform += "</body>";


                        return new ContentResult() { Content = @"" + tmpform };
                        //return RedirectToAction("List");
                    }
                }
            }

        }
        public ActionResult resthourcheck1rpt(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "resthourlog.adddate"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qdptid = "", qempname = "", qaddtype = "", qaddsdate = "", qaddedate = "";
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
            if (!string.IsNullOrWhiteSpace(Request["qaddtype"]))
            {
                qaddtype = Request["qaddtype"].Trim();
                ViewBag.qaddtype = qaddtype;
            }
            if (!string.IsNullOrWhiteSpace(Request["qaddedate"]))
            {
                qaddedate = Request["qaddedate"].Trim();
                ViewBag.qaddedate = qaddedate;
            }

            qaddsdate = NullStDate(Request["qaddsdate"]);
            ViewBag.qaddsdate = qaddsdate;
            qaddedate = NullTeDate(Request["qaddedate"]);
            ViewBag.qaddedate = qaddedate;
            //NullStDate 跟 NullTeDate 會判斷格式，有錯誤就 修改全域的DateEx
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }


            string Excel = "", Excel2 = "";
            string sqlstr = "";
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                sqlstr = "SELECT * FROM resthourlog ";
                sqlstr += " INNER JOIN employee ON resthourlog.empid = employee.empid ";
                sqlstr += " where resthourlog.rstype='2' and resthourlog.ifactive = 'y'";

                if (qdptid != "")
                { sqlstr += " and employee.empworkdepid ='" + qdptid + "'"; }
                if (qempname != "")
                { sqlstr += " and employee.empname like N'%" + qempname + "%'"; }
                if (qaddtype != "")
                {
                    switch (qaddtype)
                    {
                        case "1":
                            sqlstr += " and resthourlog.resthour > 0";
                            break;
                        case "2":
                            sqlstr += " and resthourlog.moneyhour > 0";
                            break;
                        case "3":
                            sqlstr += " and resthourlog.ifdinner = 'y'";
                            break;
                        default:
                            break;
                    }
                }
                if (qaddsdate != "")
                //{ sqlstr += " and resthourlog.adddate >= '" + qaddsdate + "'"; }
                { sqlstr += " and resthourlog.adddate >= '2016/03/01'"; }
                if (qaddedate != "")
                { sqlstr += " and resthourlog.adddate <= '" + qaddedate + "'"; }

                sqlstr += " order by " + orderdata + " " + orderdata1;
            }


            #region 組 Excel 格式
            Excel += "<HTML>";
            Excel += "<HEAD>";
            Excel += @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">";
            Excel += "</HEAD>";
            Excel += "<body>";
            Excel += "<table  border=1  cellpadding=0 cellspacing=0 bordercolor=#000000 bordercolordark=#ffffff width=900 >";
            Excel += "<tr align=center>";
            Excel += @"<td colspan=""11"" style=""font-size:14pt"">加班時數紀錄表";
            if (qaddsdate != "" || qaddedate != "")
            {
                Excel += qaddsdate + "~" + qaddedate;
            }
            Excel += "</td>";
            Excel += "</tr>";
            Excel += "<tr align=center>";
            Excel += "<td>加班日期</td>";
            Excel += "<td>部門</td>";
            Excel += "<td>申請人</td>";
            Excel += "<td>加班時數</td>";
            Excel += "<td>請款時數</td>";
            Excel += "<td>1.34</td>";
            Excel += "<td>1.67</td>";
            Excel += "<td>1.0(假日)</td>";
            Excel += "<td>補休時數</td>";
            Excel += "<td>已休時數</td>";
            Excel += "<td>補休期限</td>";
            Excel += "</tr>";
            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();

                    string dpttitle = "", empname = "";
                    double hour = 0;
                    while (dr.Read())
                    {
                        using(SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext"))
                        {
                            dpttitle = dbobj.get_dbvalue(conn1, "select dpttitle form Department where dptid='" + dr["empworkdepid"] + "'");
                            empname = dbobj.get_name(conn1, dbobj.get_dbnull2(dr["empid"]));
                        }
                        hour = Convert.ToDouble(dr["resthour"]) + Convert.ToDouble(dr["moneyhour"]);


                        Excel2 += "<tr>";
                        Excel2 += "<td>" + Convert.ToDateTime(dr["adddate"]).ToString("yyyy/MM/dd") + "</td>";
                        Excel2 += "<td>" + dpttitle  + "</td>";
                        Excel2 += "<td>" + empname + "</td>";
                        Excel2 += "<td>" + hour + "</td>";
                        Excel2 += "<td>" + dr["moneyhour"] + "</td>";
                        Excel2 += "<td>" + dr["moneyh1"] + "</td>";
                        Excel2 += "<td>" + dr["moneyh2"] + "</td>";
                        Excel2 += "<td>" + dr["moneyh3"] + "</td>";
                        Excel2 += "<td>" + dr["resthour"] + "</td>";
                        Excel2 += "<td>" + dr["usehour"] + "</td>";
                        Excel2 += "<td>" + dr["rsdeaddate"] + "</td>";
                        Excel2 += "</tr>";
                        //dbobj.get_dbnull2().Trim()
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


            #endregion
            Excel += "</table>";
            Excel += "</body>";
            Excel += "</HTML>";

            ViewBag.Excel = Excel;
            return View();
        }
        public ActionResult List(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "resthourlog.adddate"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qdptid = "", qempname = "", qaddtype = "", qaddsdate = "", qaddedate = "";
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
            if (!string.IsNullOrWhiteSpace(Request["qaddtype"]))
            {
                qaddtype = Request["qaddtype"].Trim();
                ViewBag.qaddtype = qaddtype;
            }
            if (!string.IsNullOrWhiteSpace(Request["qaddedate"]))
            {
                qaddedate = Request["qaddedate"].Trim();
                ViewBag.qaddedate = qaddedate;
            }

            qaddsdate = NullStDate(Request["qaddsdate"]);
            ViewBag.qaddsdate = qaddsdate;
            qaddedate = NullTeDate(Request["qaddedate"]);
            ViewBag.qaddedate = qaddedate;
            //NullStDate 跟 NullTeDate 會判斷格式，有錯誤就 修改全域的DateEx
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }

            IPagedList<resthourlog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "SELECT * FROM resthourlog ";
                sqlstr += " INNER JOIN employee ON resthourlog.empid = employee.empid ";
                sqlstr += " where resthourlog.rstype='2' and resthourlog.ifactive = 'y'";

                if (qdptid != "")
                { sqlstr += " and employee.empworkdepid ='" + qdptid + "'"; }
                if (qempname != "")
                { sqlstr += " and employee.empname like N'%" + qempname + "%'"; }
                if (qaddtype != "")
                {
                    switch (qaddtype)
                    {
                        case "1":
                            sqlstr += " and resthourlog.resthour > 0";
                            break;
                        case "2":
                            sqlstr += " and resthourlog.moneyhour > 0";
                            break;
                        case "3":
                            sqlstr += " and resthourlog.ifdinner = 'y'";
                            break;
                        default:
                            break;
                    }
                }
                if (qaddsdate != "")
                { sqlstr += " and resthourlog.adddate >= '" + qaddsdate + "'"; }
                //{ sqlstr += " and resthourlog.adddate >= '2016/03/01'"; }
                if (qaddedate != "")
                { sqlstr += " and resthourlog.adddate <= '" + qaddedate + "'"; }

                sqlstr += " order by " + orderdata + " " + orderdata1;
                var query = con.resthourlog.SqlQuery(sqlstr).AsQueryable();
                result = query.ToPagedList<resthourlog>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch(orderdata, orderdata1);
            return View(result);
        }
        private string SetOrder_ch(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "adddate", "resthourlog.empid" };
            string[] od_ch1 = { "asc", "asc" };
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
                DateEx += @"加班日期起格式錯誤!!\n";
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
                DateEx += @"加班日期訖格式錯誤!!\n";
                tedate = "";
            }

            return tedate;
        }




        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string orderdata = "", orderdata1 = "";
            if (!string.IsNullOrWhiteSpace(Request["orderdata"]))
            {
                orderdata = Request["orderdata"].Trim();
            }
            if (!string.IsNullOrWhiteSpace(Request["orderdata1"]))
            {
                orderdata1 = Request["orderdata1"].Trim();
            }

            string qdptid = "", qempname = "", qaddtype = "", qaddsdate = "", qaddedate = "";
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
            if (!string.IsNullOrWhiteSpace(Request["qaddtype"]))
            {
                qaddtype = Request["qaddtype"].Trim();
                ViewBag.qaddtype = qaddtype;
            }
            if (!string.IsNullOrWhiteSpace(Request["qaddsdate"]))
            {
                qaddsdate = Request["qaddsdate"].Trim();
                ViewBag.qaddsdate = qaddsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qaddedate"]))
            {
                qaddedate = Request["qaddedate"].Trim();
                ViewBag.qaddedate = qaddedate;
            }


            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/resthourcheck1/List' method='post'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

            tmpform += "<input type=hidden id='qdptid' name='qdptid' value='" + qdptid + "'>";
            tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
            tmpform += "<input type=hidden id='qaddtype' name='qaddtype' value='" + qaddtype + "'>";
            tmpform += "<input type=hidden id='qaddsdate' name='qaddsdate' value='" + qaddsdate + "'>";
            tmpform += "<input type=hidden id='qaddedate' name='qaddedate' value='" + qaddedate + "'>";

            tmpform += "</form>";
            tmpform += "</body>";

            string cdel = Request["cdel"];
            if (string.IsNullOrWhiteSpace(cdel))
            {
                return new ContentResult() { Content = @"<script>alert('請勾選要刪除的資料!!');</script>" + tmpform }; 
            }
            else
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {

                    NDcommon dbobj = new NDcommon();
                    SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext");
                    string sysnote = "";
                    string[] condtionArr = cdel.Split(',');
                    int condtionLen = condtionArr.Length;
                    for (int i = 0; i < condtionLen; i++)
                    {
                        string eresthourlogs = dbobj.get_dbvalue(conn1, "select empid from resthourlog where rsid ='" + condtionArr[i].ToString() + "'");

                        sysnote += "代碼名稱:" + eresthourlogs + "，序號:" + condtionArr[i].ToString() + "<br>";

                        dbobj.dbexecute("Aitag_DBContext", "DELETE FROM resthourlog where rsid = '" + condtionArr[i].ToString() + "'");
                     
                    }

                    conn1.Close();
                    conn1.Dispose();
                    string sysrealsid = Request["sysrealsid"].ToString();
                    //系統LOG檔
                    //================================================= //                  
                    SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    string sysflag = "D";
                    dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    sysconn.Close();
                    sysconn.Dispose();
                    //======================================================          
                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');</script>" + tmpform }; 
                    //return RedirectToAction("List");
                }
            }
        }



    }
}
