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
    public class emphdlogController : BaseController
    {
        string DateEx = "";

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /cardjudgelog/

        public ActionResult Index()
        {
            return RedirectToAction("List");
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
                DateEx += @"日期起格式錯誤!!\n";
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
                DateEx += @"日期訖格式錯誤!!\n";
                tedate = "";
            }

            return tedate;
        }

        private string SetOrder_ch(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "hlogstatus", "hdayid", "dptid", "empid", "hlogedate" };
            string[] od_ch1 = { "asc", "asc", "asc", "asc", "desc" };
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





        public ActionResult csvemphdreport(int? page, string orderdata, string orderdata1)
        {

            string sql = "select emphdlog.slyear,emphdlog.empid, emphdlog.empname from ";
            sql += "emphdlog inner join employee on emphdlog.empid = employee.empid where emphdlog.comid='" + Session["comid"].ToString() + "'  and ";

            if (!string.IsNullOrWhiteSpace(Request["qempname"]))
            {
                sql += " emphdlog.empname like '%" + Request["qempname"] + "%'  and ";
            }
            if (!string.IsNullOrWhiteSpace(Request["slyear"]))
            {
                sql += " year(clogdate) = " + Request["slyear"] + "  and ";
            }
            if (!string.IsNullOrWhiteSpace(Request["qhchange"]))
            {
                sql += " hchange = '" + Request["qhchange"] + "'  and ";
            }
            sql = sql.Substring(0, sql.Length - 5);
            sql += " GROUP BY emphdlog.slyear,emphdlog.empid, emphdlog.empname";
            sql += " order by emphdlog.slyear desc";

            string Excel = "", Excel2 = "";

            Excel += "<HTML>";
            Excel += "<HEAD>";
            Excel += @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">";
            Excel += "</HEAD>";
            Excel += "<body>";
            Excel += "<table  border=1  cellpadding=0 cellspacing=0 bordercolor=#000000 bordercolordark=#ffffff width=900 >";
            Excel += "<tr align=center>";
            Excel += "<td>年度</td>";
            Excel += "<td>員工編號</td>";
            Excel += "<td>姓名</td>";
            Excel += "<td>有效期限</td>";
            Excel += "<td>特休假<br>(年假)</td>";
            Excel += "<td>已休<br>時數</td>";
            Excel += "<td>剩餘<br>時數</td>";
            Excel += "<td>不扣薪<br>病假</td>";
            Excel += "<td>已休<br>時數</td>";
            Excel += "<td>剩餘<br>時數</td>";
            Excel += "<td>補休</td>";
            Excel += "<td>已休<br>時數</td>";
            Excel += "<td>剩餘<br>時數</td>";
            Excel += "</tr>";
           
            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            SqlConnection comconn = dbobj.get_conn("Aitag_DBContext");
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        DateTime dt = DateTime.Now;
                        string slyear = dr["slyear"].ToString();
                        string empid = dr["empid"].ToString();
                        string allhour01 = "0", usehour01 = "0", effectiveday01 = "", lefthour01 = "0";
                        string allhour20 = "0", usehour20 = "0", lefthour20 = "0";
                        string resthourrh = "0", usehourrh = "0", lefthourrh = "0";

                        // 年假A01
                        sql = "select isnull(allhour,0) as allhour,isnull(usehour,0) as usehour , effectiveday from emphdlog where slyear=" + slyear + " and empid='" + empid + "' and hdayid='A01'";
                        using (SqlConnection tconn = dbobj.get_conn("Aitag_DBContext"))
                        {
                            using (SqlCommand cmd1 = new SqlCommand(sql, tconn))
                            {
                                SqlDataReader dr1 = cmd1.ExecuteReader();
                                if (dr1.HasRows)
                                {
                                    dr1.Read();
                                    allhour01 = dr1["allhour"].ToString(); //特休假年假
                                    usehour01 = dr1["usehour"].ToString(); //已休
                                    DateTime tmpday = Convert.ToDateTime(dr1["effectiveday"]); //有效期限
                                    effectiveday01 = tmpday.ToShortDateString();
                                    double inthour = Convert.ToDouble(allhour01) - Convert.ToDouble(usehour01);
                                    lefthour01 = inthour.ToString();
                                }
                                dr1.Close();
                            }

                            //不扣薪病假A20 

                            sql = "select isnull(allhour,0) as allhour,isnull(usehour,0) as usehour , effectiveday from emphdlog where slyear=" + slyear + " and empid='" + empid + "' and hdayid='A20'";


                            using (SqlCommand cmd1 = new SqlCommand(sql, tconn))
                            {
                                SqlDataReader dr1 = cmd1.ExecuteReader();
                                if (dr1.HasRows)
                                {
                                    dr1.Read();
                                    allhour20 = dr1["allhour"].ToString(); //特休假年假
                                    usehour20 = dr1["usehour"].ToString(); //已休
                                    double inthour = Convert.ToDouble(allhour20) - Convert.ToDouble(usehour20);
                                    lefthour20 = inthour.ToString();
                                }
                                dr1.Close();
                            }

                            //補休
                            sql = "select isnull(sum(resthour),0) as resthour,isnull(sum(usehour),0) as usehour from resthourlog where rsdeaddate >= '" + dt.ToShortDateString() + "' and empid = '" + empid + "'";

                            using (SqlCommand cmd1 = new SqlCommand(sql, tconn))
                            {
                                SqlDataReader dr1 = cmd1.ExecuteReader();
                                if (dr1.HasRows)
                                {
                                    dr1.Read();
                                    resthourrh = dr1["resthour"].ToString(); //特休假年假
                                    usehourrh = dr1["usehour"].ToString(); //已休
                                    double inthour = Convert.ToDouble(resthourrh) - Convert.ToDouble(usehourrh);
                                    lefthourrh = inthour.ToString();
                                }
                                dr1.Close();
                            }
                        }
                        
                        Excel2 += "<tr>";
                        Excel2 += "<td>" + slyear + "</td>";
                        Excel2 += "<td>" + dbobj.get_dbvalue(comconn, "select empno from employee where empno <> '' and empid = '" + dr["empid"] + "' and empworkcomp = '" + Session["comid"].ToString() + "'") + "</td>";
                        Excel2 += "<td>" + dbobj.get_dbnull2(dr["empname"]) + "</td>";
                        Excel2 += "<td>" + effectiveday01 + "</td>";

                        Excel2 += "<td>" + allhour01 + "</td>";
                        Excel2 += "<td>" + usehour01 + "</td>";
                        Excel2 += "<td>" + lefthour01 + "</td>";

                        Excel2 += "<td>" + allhour20 + "</td>";
                        Excel2 += "<td>" + usehour20 + "</td>";
                        Excel2 += "<td>" + lefthour20 + "</td>";

                        Excel2 += "<td>" + resthourrh + "</td>";
                        Excel2 += "<td>" + usehourrh + "</td>";
                        Excel2 += "<td>" + lefthourrh + "</td>";

                        Excel2 += "</tr>";
                    }
                    if (Excel2 == "")
                    {
                        Excel += "<tr align=left><td colspan=13>目前沒有資料</td></tr>";
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
            comconn.Close();
            comconn.Dispose();
            ViewBag.Excel = Excel;
            return View();
        }


        public ActionResult emphdreport(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = ""; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = ""; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qslyear = "", qempname = "", qhchange = "";

            if (!string.IsNullOrWhiteSpace(Request["qempname"]))
            {
                qempname = Request["qempname"];
                ViewBag.qempname = qempname;
            }
            if (!string.IsNullOrWhiteSpace(Request["qslyear"]))
            {
                qslyear = Request["qslyear"];
                ViewBag.qslyear = qslyear;
            }
            else
            {
                DateTime dt = DateTime.Now;
                qslyear = dt.Year.ToString();
                ViewBag.qslyear = qslyear;
            }
            if (!string.IsNullOrWhiteSpace(Request["qhchange"]))
            {
                qhchange = Request["qhchange"].Trim();
                ViewBag.qhchange = qhchange;
            }
            else
            {
                qhchange = "n";
                ViewBag.qhchange = qhchange;
            }

            //NullStDate 跟 NullTeDate 會判斷格式，有錯誤就 修改全域的DateEx
            string sql = "select distinct slyear , empid , empname , getdate() as factsday , dptid , '' as hdayid , getdate() as effectiveday , 0.0 as allhour , 0.0 as usehour  , comid , '' as bmodid , getdate() as bmoddate from emphdlog where comid = '" + Session["comid"].ToString() + "' and slyear = '" + qslyear + "'";
            IPagedList<tmpemphdlog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                var query = con.tmpemphdlog.SqlQuery(sql).AsQueryable();
                result = query.ToPagedList<tmpemphdlog>(page.Value - 1, (int)Session["pagesize"]);
            }
            ViewBag.SetOrder_ch = SetOrder_ch(orderdata, orderdata1);
            return View(result);
            //return View();
        }

        public ActionResult empholiday(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = ""; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = ""; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qslyear = "", qempname = "", qhchange = "", qdptid = "";

            if (!string.IsNullOrWhiteSpace(Request["qempname"]))
            {
                qempname = Request["qempname"];
                ViewBag.qempname = qempname;
            }
            if (!string.IsNullOrWhiteSpace(Request["qslyear"]))
            {
                qslyear = Request["qslyear"];
                ViewBag.qslyear = qslyear;
            }
            else
            {
                DateTime dt = DateTime.Now;
                qslyear = dt.Year.ToString();
                ViewBag.qslyear = qslyear;
            }

            //if (!string.IsNullOrWhiteSpace(Request["qhchange"]))
            //{
            //    qhchange = Request["qhchange"].Trim();
            //    ViewBag.qhchange = qhchange;
            //}
            //else
            //{
            //    qhchange = "n";
            //    ViewBag.qhchange = qhchange;
            //}

            if (!string.IsNullOrWhiteSpace(Request["qdptid"]))
            {
                qdptid = Request["qdptid"];
                ViewBag.qdptid = qdptid;
            }

            //NullStDate 跟 NullTeDate 會判斷格式，有錯誤就 修改全域的DateEx
            
            IPagedList<emphdlog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sql = "select CAST(CAST(NEWID() AS binary(3)) AS int) as hid, slyear,empid, empname , 'n' as hchange , NULL as factsday, dptid, '' as hdayid, NULL as effectiveday, 0.0 as allhour, 0.0 as usehour, '' as comid, '' as bmodid, NULL as bmoddate  from emphdlog where comid='" + Session["comid"].ToString() + "'  and";
                if (!string.IsNullOrWhiteSpace(ViewBag.qempname))
                {
                    sql += " empname like '%" + ViewBag.qempname + "%'  and ";
                }
                if (!string.IsNullOrWhiteSpace(ViewBag.qslyear))
                {
                    sql += " slyear = " + ViewBag.qslyear + "  and ";
                }
               // if (!string.IsNullOrWhiteSpace(ViewBag.qhchange))
               // {
               //     sql += " hchange = '" + ViewBag.qhchange + "'  and ";
               // }

                if (!string.IsNullOrWhiteSpace(ViewBag.qdptid))
                {
                    sql += " dptid = '" + ViewBag.qdptid + "'  and ";
                }
                sql = sql.Substring(0, sql.Length - 5);
                sql += " GROUP BY slyear, empid, empname , dptid , hchange";
                sql += " order by slyear desc,dptid";
                var query = con.emphdlog.SqlQuery(sql).AsQueryable();
                result = query.ToPagedList<emphdlog>(page.Value - 1, (int)Session["pagesize"]);
            }
            ViewBag.SetOrder_ch = SetOrder_ch(orderdata, orderdata1);
            return View(result);
            //return View();
        }

        public ActionResult empholidayview(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = ""; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = ""; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qslyear = "", qempid = "", qhchange = "";

            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {
                qempid = Request["qempid"];
                ViewBag.qempid = qempid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qslyear"]))
            {
                qslyear = Request["qslyear"];
                ViewBag.qslyear = qslyear;
            }
            if (!string.IsNullOrWhiteSpace(Request["qhchange"]))
            {
                qhchange = Request["qhchange"];
                ViewBag.qhchange = qhchange;
            }

            //NullStDate 跟 NullTeDate 會判斷格式，有錯誤就 修改全域的DateEx
            string sql = "select * from emphdlog where ";
            sql += " empid = '" + qempid + "'  and ";
            sql += " slyear = " + qslyear + "  and ";
            //sql += " hchange = '" + qhchange + "' and";
            sql += " comid = '" + (string)Session["comid"] + "' ";

            IPagedList<emphdlog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                var query = con.emphdlog.SqlQuery(sql).AsQueryable();
                result = query.ToPagedList<emphdlog>(page.Value - 1, (int)Session["pagesize"]);
            }
            ViewBag.SetOrder_ch = SetOrder_ch(orderdata, orderdata1);
            return View(result);
            //return View();
        }

        public ActionResult empholidaybatch(emphdlog col, int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;

            string qslyear = Request["qslyear"].Trim();
            //到職日與年頭
            string effectiveday = Request["qeffectiveday"].Trim();
            string sql = "select stopholidayyear from progparam where comid = '" + Session["comid"] + "'";
            string stopholidayyear = null;

            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                using (SqlCommand sqlcmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = sqlcmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            stopholidayyear = dr["stopholidayyear"].ToString();
                        }
                    }
                    dr.Close();
                    sqlcmd.Dispose();
                }
                conn.Close();
                conn.Dispose();
            }
            if (Convert.ToInt32(stopholidayyear) >=  Convert.ToInt32(qslyear))
            {
                return new ContentResult() { Content = @"<body onload=javascript:alert('本年年假發假已確認!!');window.history.go(-1);>" };
            }


            //刪除之前的(查詢年)
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                sql = "delete from emphdlog where slyear = " + qslyear + " and comid = '" + Session["comid"] + "'";
                using (SqlCommand sqlcmd = new SqlCommand(sql, conn))
                {
                    sqlcmd.ExecuteNonQuery();

                    sqlcmd.Dispose();
                }
                conn.Close();
                conn.Dispose();
            }


            //sqlcmd2.CommandText = sql;
            int yearcount1 = 0;
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                sql = "select empid , empname , empworkdepid , jobdate, yhid from employee where empstatus <> '4' and empworkcomp='" + Session["comid"] + "' and not (jobdate is null)  order by empid";
                using (SqlCommand sqlcmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = sqlcmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            string tmpdate = qslyear + "/" + dr["jobdate"].ToString().Substring(5, 2) + "/" + dr["jobdate"].ToString().Substring(8, 2);
                             yearcount1 = Convert.ToInt32(qslyear) - Convert.ToInt32(dr["jobdate"].ToString().Substring(0, 4));

                            //if (yearcount1 == 1)
                            //{
                            //    if (dr["jobdate"].ToString().Substring(5, 2) == "1")
                            //    {
                            //        yearcount1 = 1;
                            //    }
                            //    else
                            //    {
                            //        yearcount1 = 0;
                            //    }
                            //}
                            //年假產生
                            int yhhours1 = 0;
                            using (SqlConnection conn2 = dbobj.get_conn("Aitag_DBContext"))
                            {
                                sql = "select * from yearholidaydet where yhid='" + dr["yhid"] + "' and comid='" + Session["comid"] + "' and yhsyear <= " + yearcount1 + " and yheyear > " + yearcount1;
                                using (SqlCommand sqlcmd2 = new SqlCommand(sql, conn2))
                                {
                                    SqlDataReader dr2 = sqlcmd2.ExecuteReader();
                                    if (dr2.HasRows)
                                    {
                                        while (dr2.Read())
                                        {
                                            if (Convert.ToInt32(dr2["yhsyear"]) > 240)
                                            {
                                                int addday = yearcount1 - Convert.ToInt32(dr2["yhsyear"]);
                                                if (addday > 30)
                                                {
                                                    addday = 30;
                                                }
                                                yhhours1 = Convert.ToInt32(dr2["yhours"]) + (addday * 8);
                                            }
                                            else
                                            {
                                                yhhours1 = Convert.ToInt32(dr2["yhours"]);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        yhhours1 = 0;
                                    }
                                    if (yhhours1 < 0)
                                    {
                                        yhhours1 = 0;
                                    }

                                    dr2.Close();
                                    dr2.Dispose();
                                    sqlcmd2.Dispose();
                                }
                                conn2.Close();
                                conn2.Dispose();
                            }
                            col.slyear = Convert.ToInt32(qslyear);
                            col.empid = dr["empid"].ToString();
                            col.empname = dr["empname"].ToString();
                            col.dptid = dr["empworkdepid"].ToString();
                            col.effectiveday = Convert.ToDateTime(effectiveday);
                            col.hdayid = "A01";
                            col.allhour = yhhours1;
                            col.usehour = 0;
                            col.hchange = "n";
                            //col.comid = Session["comid"].ToString();
                            //col.bmodid = Session["tempid"].ToString();
                            //col.bmoddate = DateTime.Now;

                            using (Aitag_DBContext con = new Aitag_DBContext())
                            {
                                col.comid = Session["comid"].ToString();
                                col.bmodid = Session["tempid"].ToString();
                                col.bmoddate = DateTime.Now;
                                con.emphdlog.Add(col);
                                con.SaveChanges();
                            }
                            //算其它的假(年假辦法維護key的)

                            using (SqlConnection conn2 = dbobj.get_conn("Aitag_DBContext"))
                            {
                                sql = @"SELECT yearhddet.hdayid,yearhddet.allhour FROM yearholiday INNER JOIN yearhddet ON yearholiday.yhid = yearhddet.yhid AND yearholiday.comid = yearhddet.comid
                                    where yearholiday.yhid='" + dr["yhid"] + "' and yearholiday.comid='" + Session["comid"] + "' order by yearhddet.hdayid";
                                using (SqlCommand sqlcmd2 = new SqlCommand(sql, conn2))
                                {
                                    SqlDataReader dr2 = sqlcmd2.ExecuteReader();
                                    if (dr2.HasRows)
                                    {
                                        while (dr2.Read())
                                        {
                                            col.slyear = Convert.ToInt32(qslyear);
                                            col.empid = dr["empid"].ToString();
                                            col.empname = dr["empname"].ToString();
                                            col.dptid = dr["empworkdepid"].ToString();
                                            col.effectiveday = Convert.ToDateTime(effectiveday);
                                            col.hdayid = dr2["hdayid"].ToString();
                                            col.allhour = Convert.ToDecimal(dr2["allhour"]);
                                            col.usehour = 0;
                                            col.hchange = "n";
                                            col.comid = Session["comid"].ToString();
                                            col.bmodid = Session["tempid"].ToString();
                                            col.bmoddate = DateTime.Now;

                                        }
                                    }
                                    dr2.Close();
                                    dr2.Dispose();
                                    sqlcmd2.Dispose();
                                }
                                conn2.Close();
                                conn2.Dispose();
                            }
                            
                            using (Aitag_DBContext con = new Aitag_DBContext())
                            {
                                col.comid = Session["comid"].ToString();
                                col.bmodid = Session["tempid"].ToString();
                                col.bmoddate = DateTime.Now;
                                con.emphdlog.Add(col);
                                con.SaveChanges();
                            }

                        }
                        dr.Close();
                        dr.Dispose();
                    }
                    sqlcmd.Dispose();
                }

                conn.Close();
                conn.Dispose();
            }

          

                //系統LOG檔
                //================================================= //                     
                SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                string sysrealsid = Request["sysrealsid"].ToString();
                string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                string sysnote = "批次新增年度:" + qslyear + "，所有員工的年假資料";
                string sysflag = "A";
                dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                sysconn.Close();
                sysconn.Dispose();
                //=================================================

                string tmpform = "";
                tmpform = "<body onload=qfr1.submit();>";
                tmpform += "<form name='qfr1' action='/emphdlog/empholiday' method='post'>";
                tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                //tmpform += "<input type=hidden id='qdptid' name='qdptid' value='" + col.dptid + "'>";
                tmpform += "<input type=hidden id='qempid' name='qempid' value='" + col.empid + "'>";
                tmpform += "<input type=hidden id='qslyear' name='qslyear' value='" + qslyear + "'>";
                tmpform += "<input type=hidden id='hid' name='hid' value='" + col.hid + "'>";
                tmpform += "</form>";
                tmpform += "</body>";
                return new ContentResult() { Content = @"" + tmpform };
                //return RedirectToAction("List");
           
        }

        public ActionResult empholidaybatch1(emphdlog col, int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;

            string qslyear = Request["qslyear"].Trim();
            //到職日與年頭
            string effectiveday = Request["qeffectiveday"].Trim();
            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            string sql = "select * from emphdlog where slyear = '" + (Convert.ToInt32(qslyear) - 1) + "' and comid = '" + Session["comid"] + "' and hdayid = 'A01'";
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                using (SqlCommand sqlcmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = sqlcmd.ExecuteReader();
                    if(dr.HasRows)
                    {

                        //刪除之前的(查詢年)
                        using (SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext"))
                        {
                            sql = "delete from emphdlog where slyear = " + qslyear + " and comid = '" + Session["comid"] + "' and hdayid = 'A22' ";
                            using (SqlCommand sqlcmd1 = new SqlCommand(sql, conn1))
                            {
                                sqlcmd1.ExecuteNonQuery();

                                sqlcmd1.Dispose();
                            }
                            conn.Close();
                            conn.Dispose();
                        }


                        //sqlcmd2.CommandText = sql;
                        int allhour = 0;
                        int usehour = 0;
                        int lasthour = 0;
                        using (SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext"))
                        {
                            sql = "select empid , empname , empworkdepid , jobdate, yhid from employee where empstatus <> '4' and empworkcomp='" + Session["comid"] + "' and not (jobdate is null)  order by empid";
                            using (SqlCommand sqlcmd1 = new SqlCommand(sql, conn1))
                            {
                                SqlDataReader dr1 = sqlcmd1.ExecuteReader();
                                if (dr1.HasRows)
                                {
                                    while (dr1.Read())
                                    {
                                        
                                        //去年保留休假
                                        string tmphour = "";
                                        SqlConnection conn2 = dbobj.get_conn("Aitag_DBContext");
                                        sql = "select isnull(sum(allhour - usehour),0) as allhour from emphdlog where empid = '" + dr1["empid"].ToString() + "' and slyear ='" + (Convert.ToInt32(qslyear) - 1).ToString() + "' and comid ='" + Session["comid"] + "' and hdayid='A01'";
                                       // if (!string.IsNullOrEmpty(dbobj.get_dbvalue(conn2, "select (allhour - usehour) as allhour from emphdlog where empid = '" + dr1["empid"].ToString() + "' and slyear ='" + (Convert.ToInt32(qslyear) - 1).ToString() + "' and comid ='" + Session["comid"] + "' and hdayid='A01'")))
                                       // {
                                        tmphour = decimal.Parse(dbobj.get_dbvalue(conn2, sql)).ToString("###0");
                                        allhour = int.Parse(tmphour);
                                       // }
                                       // else
                                       // {
                                       //     allhour = 0;
                                       // }
                                        //Mark 調整 20170106減少依次select 
                                        //if (!string.IsNullOrEmpty(dbobj.get_dbvalue(conn2, "select usehour from emphdlog where empid = '" + dr1["empid"].ToString() + "' and slyear ='" + qslyear + "' and comid ='" + Session["comid"] + "' and hdayid='A01'")))
                                        //{
                                        //    usehour = Convert.ToInt32(dbobj.get_dbvalue(conn2, "select usehour from emphdlog where empid = '" + dr1["empid"].ToString() + "' and slyear ='" + qslyear + "' and comid ='" + Session["comid"] + "' and hdayid='A01'"));
                                        //}

                                        if (allhour != 0)
                                        {
                                            //lasthour = allhour - usehour;
                                            lasthour = allhour;
                                            if (lasthour > 0)
                                            {
                                                col.slyear = Convert.ToInt32(qslyear);
                                                col.empid = dr1["empid"].ToString();
                                                col.empname = dr1["empname"].ToString();
                                                col.dptid = dr1["empworkdepid"].ToString();
                                                //string ddate = (Convert.ToInt64(qslyear) + 1).ToString() + "/" + effectiveday.Substring(5, 2) + "/" + effectiveday.Substring(8, 2);
                                                col.effectiveday = DateTime.Parse(effectiveday);
                                                col.hdayid = "A22";
                                                col.allhour = lasthour;
                                                col.usehour = 0;
                                                col.hchange = "n";
                                                col.comid = Session["comid"].ToString();
                                                col.bmodid = Session["tempid"].ToString();
                                                col.bmoddate = DateTime.Now;
                                            }

                                            using (Aitag_DBContext con = new Aitag_DBContext())
                                            {
                                                col.comid = Session["comid"].ToString();
                                                col.bmodid = Session["tempid"].ToString();
                                                col.bmoddate = DateTime.Now;
                                                con.emphdlog.Add(col);
                                                con.SaveChanges();
                                            }
                                        }
                                        conn2.Close();
                                        conn2.Dispose();

                                       

                                    }
                                    dr1.Close();
                                    dr1.Dispose();
                                   
                                }
                                sqlcmd1.Dispose();
                            }

                            conn1.Close();
                            conn1.Dispose();
                        }

                        dr.Close();
                        dr.Dispose();
                        conn.Close();
                        conn.Dispose();

                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "批次新增年度:" + qslyear + "，所有員工的年假資料";
                        string sysflag = "A";
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/emphdlog/empholiday' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                        tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                        tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                     //   tmpform += "<input type=hidden id='qdptid' name='qdptid' value='" + col.dptid + "'>";
                        tmpform += "<input type=hidden id='qempid' name='qempid' value='" + col.empid + "'>";
                        tmpform += "<input type=hidden id='qslyear' name='qslyear' value='" + qslyear + "'>";
                        tmpform += "<input type=hidden id='hid' name='hid' value='" + col.hid + "'>";
                        tmpform += "</form>";
                        tmpform += "</body>";
                        return new ContentResult() { Content = @"" + tmpform };

                      
                    }
                    else
                    {
                        return new ContentResult() { Content = @"<body onload=javascript:alert('請先批次產生年假!!!!');window.history.go(-1);>" };
                    }
                   
                }
            }

        }

        public ActionResult empholidaymod(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = ""; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = ""; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qslyear = "", qempid = "", qhchange = "";

            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {
                qempid = Request["qempid"];
                ViewBag.qempid = qempid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qslyear"]))
            {
                qslyear = Request["qslyear"];
                ViewBag.qslyear = qslyear;
            }
            if (!string.IsNullOrWhiteSpace(Request["qhchange"]))
            {
                qhchange = Request["qhchange"];
                ViewBag.qhchange = qhchange;
            }

            string slyear = Request["slyear"];
            string empid = Request["empid"];
            //NullStDate 跟 NullTeDate 會判斷格式，有錯誤就 修改全域的DateEx
            string sql = "select * from emphdlog where ";
            sql += " empid = '" + empid + "'  and ";
            sql += " slyear = " + slyear + "  and ";
            //sql += " hchange = '" + qhchange + "' and";
            sql += " comid = '" + (string)Session["comid"] + "' ";

            IPagedList<emphdlog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                var query = con.emphdlog.SqlQuery(sql).AsQueryable();
                result = query.ToPagedList<emphdlog>(page.Value - 1, (int)Session["pagesize"]);
            }
            ViewBag.SetOrder_ch = SetOrder_ch(orderdata, orderdata1);
            return View(result);
            //return View();
        }

        public ActionResult hdlogadd(emphdlog col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "hid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qslyear = "", qdptid = "";
            if (!string.IsNullOrWhiteSpace(Request["qslyear"]))
            {
                qslyear = Request["qslyear"].Trim();
                ViewBag.qslyear = qslyear;
            }
            if (!string.IsNullOrWhiteSpace(Request["qdptid"]))
            {
                qdptid = Request["qdptid"].Trim();
                ViewBag.qdptid = qdptid;
            }

            if (sysflag != "A")
            {
                emphdlog newcol = new emphdlog();
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

                    System.Data.SqlClient.SqlConnection comconn = dbobj.get_conn("Aitag_DBContext");

                    col.empname = dbobj.get_dbvalue(comconn, "select empname from employee where empid = '" + col.empid + "'");
                    col.dptid = dbobj.get_dbvalue(comconn, "select empworkdepid from employee where empid = '" + col.empid + "'");
                    col.comid = Session["comid"].ToString();
                    col.bmodid = Session["tempid"].ToString();
                    //col.badddate = DateTime.Now;
                    col.bmoddate = DateTime.Now;
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.emphdlog.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "年度:" + col.slyear + ",員工:" + col.empid + ",假別:" + col.hdayid;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/emphdlog/empholidaymod' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qslyear' name='qslyear' value='" + col.slyear + "'>";
                    tmpform += "<input type=hidden id='qdptid' name='qdptid' value='" + col.dptid + "'>";
                    tmpform += "<input type=hidden id='qempid' name='qempid' value='" + col.empid + "'>";
                    tmpform += "<input type=hidden id='empid' name='empid' value='" + col.empid + "'>";
                    tmpform += "<input type=hidden id='slyear' name='slyear' value='" + col.slyear + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    // return RedirectToAction("List");

                }
            }


        }

        [ValidateInput(false)]
        public ActionResult hdlogmod(emphdlog chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "hid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qdptid = "", qslyear = "", qempid = "";
            if (!string.IsNullOrWhiteSpace(Request["qdptid"]))
            {
                qdptid = Request["qdptid"].Trim();
                ViewBag.qdptid = qdptid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qslyear"]))
            {

                qslyear = Request["qslyear"].Trim();
                ViewBag.qslyear = qslyear;
            }

            qempid = Request["qempid"].Trim();

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.emphdlog.Where(r => r.hid == chks.hid).FirstOrDefault();
                    emphdlog eCheckcodes = con.emphdlog.Find(chks.hid);
                    if (eCheckcodes == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eCheckcodes);
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

                    //string oldmsid = Request["oldmsid"];                 

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {

                        NDcommon dbobj = new NDcommon();
                        chks.comid = Session["comid"].ToString();
                        chks.bmodid = Session["tempid"].ToString();
                        chks.bmoddate = DateTime.Now;
                        con.Entry(chks).State = EntityState.Modified;
                        con.SaveChanges();


                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "員工編號:" + chks.empid + "假別:" + chks.hdayid;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/emphdlog/empholidaymod' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                        tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                        tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                        tmpform += "<input type=hidden id='qdptid' name='qdptid' value='" + qdptid + "'>";
                        tmpform += "<input type=hidden id='qempid' name='qempid' value='" + qempid + "'>";
                        tmpform += "<input type=hidden id='qslyear' name='qslyear' value='" + qslyear + "'>";
                        tmpform += "<input type=hidden id='empid' name='empid' value='" + qempid + "'>";
                        tmpform += "<input type=hidden id='slyear' name='slyear' value='" + qslyear + "'>";
                        tmpform += "<input type=hidden id='hid' name='hid' value='" + chks.hid + "'>";
                        tmpform += "</form>";
                        tmpform += "</body>";


                        return new ContentResult() { Content = @"" + tmpform };
                        //return RedirectToAction("List");
                    }
                }
            }

        }

        [ActionName("hdlogdel")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string empid = "", slyear = "", hdayid="";
            if (!string.IsNullOrWhiteSpace(Request["empid"]))
            {
                empid = Request["empid"].Trim();
                ViewBag.empid = empid;
            }
            if (!string.IsNullOrWhiteSpace(Request["slyear"]))
            {

                slyear = Request["slyear"].Trim();
                ViewBag.slyear = slyear;
            }
            if (!string.IsNullOrWhiteSpace(Request["hdayid"]))
            {

                hdayid = Request["hdayid"].Trim();
                ViewBag.hdayid = hdayid;
            }
            string hid = Request["hid"];

            if (string.IsNullOrWhiteSpace(hid))
            {

                string tgourl = "/emphdlog/empholidaymod?empid=" + empid + "&slyear=" + slyear;
                return new ContentResult() { Content = @"<script>alert('請勾選要刪除的資料!!');window.history.go(-1);</script>" };
            }
            else
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {

                    NDcommon dbobj = new NDcommon();
                    SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext");
                    string sysnote = "";
                    string[] condtionArr = hid.Split(',');
                    int condtionLen = condtionArr.Length;
                    for (int i = 0; i < condtionLen; i++)
                    {
                        string empname = dbobj.get_dbvalue(conn1, "selectempname from employee where empid='"+ empid +"'");

                        sysnote += "年度：" + slyear + "，員工：" + empname + "，假別代碼：" + hdayid + "<br>";

                        dbobj.dbexecute("Aitag_DBContext", "DELETE FROM emphdlog where hid = '" + condtionArr[i].ToString() + "'");

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
                    string tgourl = "/emphdlog/empholidaymod?empid=" + empid + "&slyear=" + slyear;
                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                    //return RedirectToAction("List");
                }
            }
        }

    }
}
