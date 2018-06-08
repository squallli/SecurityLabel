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
    public class empotworklogController : BaseController
    {
        string DateEx = "";

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /otworklog/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }


        [ValidateInput(false)]
        public ActionResult Edit(otworklog chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "otlogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qotstatus = "", qempname = "", qdptid = "", otlogsdate = "", otlogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qotstatus"]))
            {
                qotstatus = Request["qotstatus"].Trim();
                ViewBag.qotstatus = qotstatus;
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
            if (!string.IsNullOrWhiteSpace(Request["otlogsdate"]))
            {
                otlogsdate = Request["otlogsdate"].Trim();
                ViewBag.otlogsdate = otlogsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["otlogedate"]))
            {
                otlogedate = Request["otlogedate"].Trim();
                ViewBag.otlogedate = otlogedate;
            }

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.otworklog.Where(r => r.otlogid == chks.otlogid).FirstOrDefault();
                    otworklog eotworklogs = con.otworklog.Find(chks.otlogid);
                    if (eotworklogs == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eotworklogs);
                }

            }
            else
            {

                if (!ModelState.IsValid)
                {
                    return View(chks);
                }
                else
                {

                    //string oldotlogid = Request["oldotlogid"];                 

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        NDcommon dbobj = new NDcommon();
                        chks.otlogid = int.Parse(Request["otlogid"].Trim());
                        chks.bmodid = Session["tempid"].ToString();
                        chks.bmoddate = DateTime.Now;
                        con.Entry(chks).State = EntityState.Modified;
                        //con.SaveChanges();


                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "代碼:" + chks.otlogid + "名稱:" + chks.empname;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/otworklog/List' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                        tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                        tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                        tmpform += "<input type=hidden id='qotstatus' name='qotstatus' value='" + qotstatus + "'>";
                        tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
                        tmpform += "<input type=hidden id='qdptid' name='qdptid' value='" + qdptid + "'>";
                        tmpform += "<input type=hidden id='otlogsdate' name='otlogsdate' value='" + otlogsdate + "'>";
                        tmpform += "<input type=hidden id='otlogedate' name='otlogedate' value='" + otlogedate + "'>";
                        tmpform += "</form>";
                        tmpform += "</body>";


                        return new ContentResult() { Content = @"" + tmpform };
                        //return RedirectToAction("List");
                    }
                }
            }

        }
        public ActionResult otworklogrpt(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "otlogsdate"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qotstatus = "", qempname = "", qdptid = "", otlogsdate = "", otlogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qotstatus"]))
            {
                qotstatus = Request["qotstatus"].Trim();
                ViewBag.qotstatus = qotstatus;
            }
            else
            {
                qotstatus = "1";
                ViewBag.qotstatus = qotstatus;
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
            otlogsdate = NullStDate(Request["otlogsdate"]);
            ViewBag.otlogsdate = otlogsdate;
            otlogedate = NullTeDate(Request["otlogedate"]);
            ViewBag.otlogedate = otlogedate;
            //NullStDate 跟 NullTeDate 會判斷格式，有錯誤就 修改全域的DateEx
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }
            NDcommon dbobj = new NDcommon();
            string Excel = "", Excel2 = "";
            string sqlstr = "", sqlstr_1 = "";
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string viewid = "";
                string[] mpriv = (string[])Session["priv"];
                //viewid = get_viewpriv(int.Parse(funcpriv(2)), int.Parse(mpriv(realsid, 2)));
                string tmpall = "";
                //tmpall = dbobj.get_allempid((string)Session["rid"]);
                tmpall = "%";


                if (tmpall == "%")
                {
                    sqlstr = "SELECT * FROM otworklog where comid='" + (string)Session["comid"] + "'  and";
                    sqlstr_1 = "SELECT isnull(sum(otloghour),0) as otloghour FROM otworklog where comid='" + (string)Session["comid"] + "'  and";
                }
                else
                {
                    sqlstr = "SELECT * FROM otworklog where empid in(" + tmpall + ") and comid='" + (string)Session["comid"] + "'  and";
                    sqlstr_1 = "SELECT isnull(sum(otloghour),0) as otloghour FROM otworklog where empid in(" + tmpall + ") and comid='" + (string)Session["comid"] + "'  and";
                }

                if (qotstatus != "all")
                {
                    string sql_otstatus = " otstatus like '" + qotstatus + "'  and";
                    sqlstr = sqlstr + sql_otstatus;
                    sqlstr_1 = sqlstr_1 + sql_otstatus;
                }
                if (otlogsdate != "" && otlogedate != "")
                {
                    string sql_date = " (( '" + otlogsdate + "' <= otlogsdate and otlogsdate <= '" + otlogedate + "' ) or " +
                                        "( '" + otlogsdate + "' <= otlogedate and otlogedate <= '" + otlogedate + "'  ))  and";
                    sqlstr = sqlstr + sql_date;
                    sqlstr_1 = sqlstr_1 + sql_date;
                }
                if (qempname != "")
                {
                    string sql_empname = " empname like N'%" + qempname + "%'  and";
                    sqlstr = sqlstr + sql_empname;
                    sqlstr_1 = sqlstr_1 + sql_empname;
                }
                if (qdptid != "")
                {
                    string sql_dptid = " dptid='" + qdptid + "'  and";
                    sqlstr = sqlstr + sql_dptid;
                    sqlstr_1 = sqlstr_1 + sql_dptid;
                }


                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr_1 = sqlstr_1.Substring(0, sqlstr_1.Length - 5);
                getSUMhour(sqlstr, sqlstr_1);
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
            Excel += @"<td colspan=""9"" style=""font-size:14pt"">加班明細表";
            Excel += "</td>";
            Excel += "</tr>";
            Excel += "<tr align=center>";
            int count = 8;
            Excel += "<td colspan='" + count + "' ></td><td>列印日期：" + DateTime.Now.ToString("yyyy/MM/dd") + "</td>";
            Excel += "</tr>";
            Excel += "<tr align=center>";
            Excel += "<td>狀態</td>";
            Excel += "<td>核銷</td>";
            Excel += "<td>員工編號</td>";
            Excel += "<td>姓名</td>";
            Excel += "<td>部門</td>";
            Excel += "<td>加班起迄日期</td>";
            Excel += "<td>加班時數</td>";
            Excel += "<td>補休時數</td>";
            Excel += "<td>請款時數</td>";
            Excel += "</tr>";
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();

                    string blogstatus = "", ifhdell = "", empno = "", dpttitle="";
                    string SEtime = "自{0}({1}時)<br>至{2}({3}時)";
                    string blogsdate = "", blogstime = "", blogedate = "", blogetime = "";
                    string resthour = "", moneyhour = "",sql ="";

                    while (dr.Read())
                    {
                        blogstatus = dbobj.get_dbnull2(dr["otstatus"]);
                        switch (blogstatus)
                        {
                            case "0":
                                blogstatus = "簽核中";
                                break;
                            case "1":
                                blogstatus = "已核准";
                                break;
                            case "2":
                                blogstatus = "退回";
                                break;
                            case "D":
                                blogstatus = "撤回";
                                break;
                            default:
                                break;
                        }
                        ifhdell = dbobj.get_dbnull2(dr["ifotdell"]);
                        if (ifhdell == "y")
                        {
                            ifhdell = "是";
                        }
                        else if (ifhdell == "n")
                        {
                            ifhdell = "否";
                        }
                        blogsdate = Convert.ToDateTime(dbobj.get_dbnull2(dr["otlogsdate"])).ToString("yyyy/MM/dd");
                        blogstime = int.Parse(dbobj.get_dbnull2(dr["otlogstime"])).ToString("00");
                        blogedate = Convert.ToDateTime(dbobj.get_dbnull2(dr["otlogedate"])).ToString("yyyy/MM/dd");
                        blogetime = int.Parse(dbobj.get_dbnull2(dr["otlogetime"])).ToString("00");
                        
                        using (SqlConnection comconn = dbobj.get_conn("Aitag_DBContext"))
                        {
                            empno = "select empno from employee where empid='" + dbobj.get_dbnull2(dr["empid"]) + "'"; empno = dbobj.get_dbvalue(comconn, empno);
                            dpttitle = "select dpttitle from Department where dptid='" + dbobj.get_dbnull2(dr["dptid"]) + "' and comid='" + (string)Session["comid"] + "'"; dpttitle = dbobj.get_dbvalue(comconn, dpttitle);
                            sql = "select * from resthourlog where osno = '" + dbobj.get_dbnull2(dbobj.get_dbnull2(dr["osno"])) + "' and comid='" + (string)Session["comid"] + "'";
                            using (SqlCommand cmd1 = new SqlCommand(sql, comconn))
                            {
                                SqlDataReader dr1 = cmd1.ExecuteReader();
                                if (dr1.HasRows)
                                {
                                    dr1.Read();
                                    resthour = dbobj.get_dbnull2(dr1["resthour"]);
                                    moneyhour = dbobj.get_dbnull2(dr1["moneyhour"]);
                                }
                                else
                                {
                                    resthour = "0";
                                    moneyhour = "0";
                                }
                                dr1.Close();
                            }
                        }


                        Excel2 += "<tr>";
                        Excel2 += "<td>" + blogstatus + "</td>";
                        Excel2 += "<td>" + ifhdell + "</td>";
                        Excel2 += "<td>" + empno + "</td>";
                        Excel2 += "<td>" + dbobj.get_dbnull2(dr["empname"]) + "</td>";
                        Excel2 += "<td>" + dpttitle + "</td>";
                        Excel2 += "<td>" + String.Format(SEtime, blogsdate, blogstime, blogedate, blogetime) + "</td>";
                        Excel2 += "<td>" + dbobj.get_dbnull2(dr["otloghour"]) + "</td>";
                        Excel2 += "<td>" + resthour + "</td>";
                        Excel2 += "<td>" + moneyhour + "</td>";
                        Excel2 += "</tr>";
                    }
                    if (Excel2 == "")
                    {
                        Excel += "<tr align=left><td colspan=6>目前沒有資料</td></tr>";
                    }
                    else
                    {
                        Excel2 += "<tr>";
                        Excel2 += "<td>總計</td>";
                        Excel2 += "<td>&nbsp;</td>";
                        Excel2 += "<td>&nbsp;</td>";
                        Excel2 += "<td>&nbsp;</td>";
                        Excel2 += "<td>&nbsp;</td>";
                        Excel2 += "<td>&nbsp;</td>";
                        Excel2 += "<td>" + ViewBag.otloghour + "</td>";
                        Excel2 += "<td>" + ViewBag.sumresthour + "</td>";
                        Excel2 += "<td>" + ViewBag.summoneyhour + "</td>";
                        Excel2 += "</tr>";
                        Excel += Excel2;
                    }
                    dr.Close();
                }
            }
            Excel += "</table>";
            Excel += "</body>";
            Excel += "</HTML>";
            ViewBag.Excel = Excel;
            #endregion

            return View();

        }

        public ActionResult List(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "otlogsdate"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qotstatus = "", qempname = "", qdptid = "", otlogsdate = "", otlogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qotstatus"]))
            {
                qotstatus = Request["qotstatus"].Trim();
                ViewBag.qotstatus = qotstatus;
            }
            else
            {
                qotstatus = "1";
                ViewBag.qotstatus = qotstatus;
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
            otlogsdate = NullStDate(Request["otlogsdate"]);
            ViewBag.otlogsdate = otlogsdate;
            otlogedate = NullTeDate(Request["otlogedate"]);
            ViewBag.otlogedate = otlogedate;
            //NullStDate 跟 NullTeDate 會判斷格式，有錯誤就 修改全域的DateEx
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }
            NDcommon dbobj = new NDcommon();
            IPagedList<otworklog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string viewid = "";
                string[] mpriv = (string[])Session["priv"];
                //viewid = get_viewpriv(int.Parse(funcpriv(2)), int.Parse(mpriv(realsid, 2)));
                string tmpall = "";
                //tmpall = dbobj.get_allempid((string)Session["rid"]);
                tmpall = "%";


                string sqlstr = "", sqlstr_1 = "";
                if(tmpall == "%")
                {
                    sqlstr = "SELECT * FROM otworklog where comid='" + (string)Session["comid"] + "'  and";
                    sqlstr_1 = "SELECT isnull(sum(otloghour),0) as otloghour FROM otworklog where comid='" + (string)Session["comid"] + "'  and";
                }
                else
                {
                    sqlstr = "SELECT * FROM otworklog where empid in(" + tmpall + ") and comid='" + (string)Session["comid"] + "'  and";
                    sqlstr_1 = "SELECT isnull(sum(otloghour),0) as otloghour FROM otworklog where empid in(" + tmpall + ") and comid='" + (string)Session["comid"] + "'  and";
                }

                if (qotstatus != "all")
                {
                    string sql_otstatus = " otstatus like '" + qotstatus + "'  and";
                     sqlstr = sqlstr + sql_otstatus;
                     sqlstr_1 = sqlstr_1 + sql_otstatus;
                }
                if (otlogsdate != "" && otlogedate != "")
                {
                    string sql_date = " (( '" + otlogsdate + "' <= otlogsdate and otlogsdate <= '" + otlogedate + "' ) or " +
                                        "( '" + otlogsdate + "' <= otlogedate and otlogedate <= '" + otlogedate + "'  ))  and";
                    sqlstr = sqlstr + sql_date;
                    sqlstr_1 = sqlstr_1 + sql_date;
                }
                if (qempname != "")
                {
                    string sql_empname = " empname like N'%" + qempname + "%'  and";
                    sqlstr = sqlstr + sql_empname;
                    sqlstr_1 = sqlstr_1 + sql_empname;
                }
                if (qdptid != "")
                {
                    string sql_dptid = " dptid='" + qdptid + "'  and";
                    sqlstr = sqlstr + sql_dptid;
                    sqlstr_1 = sqlstr_1 + sql_dptid;
                }


                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr_1 = sqlstr_1.Substring(0, sqlstr_1.Length - 5);
                getSUMhour(sqlstr, sqlstr_1);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.otworklog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<otworklog>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch(orderdata, orderdata1);
            return View(result);
        }

        private void getSUMhour(string sqlstr, string sqlstr_1)
        {
            NDcommon dbobj = new NDcommon();
            using (SqlConnection comconn = dbobj.get_conn("Aitag_DBContext"))
            {
                ViewBag.otloghour = dbobj.get_dbvalue(comconn, sqlstr_1);
                using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                string txtosno = "", sqlstr_2 = "";
                using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            txtosno += "'" + dbobj.get_dbnull2(dr["osno"]) + "',";
                        }
                        sqlstr_2 = "select isnull(sum(resthour),0) as resthour,isnull(sum(moneyhour),0) as moneyhour from resthourlog where osno in (" + txtosno.Substring(0, txtosno.Length -1) + ") and comid='" + (string)Session["comid"] + "'";
                    }
                    else
                    {
                        sqlstr_2 = "select isnull(sum(resthour),0) as resthour,isnull(sum(moneyhour),0) as moneyhour from resthourlog where 1<>1";
                    }
                    dr.Close();


                }
                using (SqlCommand cmd = new SqlCommand(sqlstr_2, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        dr.Read();
                        ViewBag.sumresthour = dbobj.get_dbnull2(dr["resthour"]);
                        ViewBag.summoneyhour = dbobj.get_dbnull2(dr["moneyhour"]);
                    }
                    else
                    {
                        ViewBag.sumresthour = "0";
                        ViewBag.summoneyhour = "0";
                    }
                }
            }
            }
        }
        private string SetOrder_ch(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "otype", "otstatus", "ifotdell", "empid", "empname", "dptid", "otlogsdate" };
            string[] od_ch1 = { "asc", "asc", "asc", "asc", "asc", "asc", "asc" };
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
                    if (perval == 1)
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
