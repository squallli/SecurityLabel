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
    public class cardabnormallogController : BaseController
    {
        string DateEx = "";

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /cardjudgelog/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        
        public ActionResult List(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "clogdate"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "ASC"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qdptid = "", qempname = "", qclogsdate = "", qclogedate = "", qclogstatus = "";
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
            if (!string.IsNullOrWhiteSpace(Request["qclogstatus"]))
            {
                qclogstatus = Request["qclogstatus"].Trim();
                ViewBag.qclogstatus = qclogstatus;
            }
            qclogsdate = NullStDate(Request["qclogsdate"]);
            ViewBag.qclogsdate = qclogsdate;
            qclogedate = NullTeDate(Request["qclogedate"]);
            ViewBag.qclogedate = qclogedate;
            //NullStDate 跟 NullTeDate 會判斷格式，有錯誤就 修改全域的DateEx
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }


            if (!string.IsNullOrWhiteSpace(Request["qclogstatus"]))
            {
                if (Request["qclogstatus"] == "")
                {
                    qclogstatus = "";
                    ViewBag.qclogstatus = qclogstatus;
                }
                else
                {
                    qclogstatus = Request["qclogstatus"].Trim();
                    ViewBag.qclogstatus = qclogstatus;
                }
            }
            else
            {
                qclogstatus = "";
                ViewBag.qclogstatus = qclogstatus;
            }




            IPagedList<cardjudgelog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "SELECT * FROM cardjudgelog where comid='" + (string)Session["comid"] + "'  and";
                if (qdptid != "") 
                {
                    sqlstr += " dptid='" + qdptid + "'  and";
                }
                if (qempname != "")
                {
                    sqlstr += " empname like N'%" + qempname + "%'  and";
                }
                if (qclogsdate != "")
                {
                    sqlstr += " clogdate >= '" + qclogsdate + "'  and";
                }
                if (qclogedate != "")
                {
                    sqlstr += " clogdate <= '" + qclogedate + "'  and";
                }
                if (qclogstatus != "")
                {
                    sqlstr += " clogstatus = '" + qclogstatus + "'  and";
                }


                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by dptid,empid ," + orderdata + " " + orderdata1;

                var query = con.cardjudgelog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<cardjudgelog>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch(orderdata, orderdata1);
            return View(result);
        }

        private string NullStDate(string stdate)
        {
            if (string.IsNullOrWhiteSpace(stdate))
            {
                stdate = DateTime.Now.ToString("yyyy/MM/dd");
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
                //var dat = new DateTime(DateTime.Now.Year - 1, 12, 31);
                tedate = DateTime.Now.ToString("yyyy/MM/dd");
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


        public ActionResult csvcardabnormallog(int? page, string orderdata, string orderdata1)
        {
           
            string qdptid = "", qempname = "", qclogsdate = "", qclogedate = "", qclogstatus = "", qempid="";
            if (!string.IsNullOrWhiteSpace(Request["qdptid"]))
            {
                qdptid = Request["qdptid"].Trim();
            }
            if (!string.IsNullOrWhiteSpace(Request["qempname"]))
            {
                qempname = Request["qempname"].Trim();
            }
            if (!string.IsNullOrWhiteSpace(Request["qclogstatus"]))
            {
                qclogstatus = Request["qclogstatus"].Trim();
            }
            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {
                qempid = Request["qempid"].Trim();
                
            }

            if (!string.IsNullOrWhiteSpace(Request["qclogsdate"]))
            {
                qclogsdate = Request["qclogsdate"].Trim();
                ViewBag.qclogsdate = qclogsdate;
            }

            if (!string.IsNullOrWhiteSpace(Request["qclogedate"]))
            {
                qclogedate = Request["qclogedate"].Trim();
                ViewBag.qclogedate = qclogedate;
            }
            
            //NullStDate 跟 NullTeDate 會判斷格式，有錯誤就 修改全域的DateEx
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }


            string Excel = "", Excel2 = "";
            string sqlstr = "";
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                sqlstr = "SELECT * FROM cardjudgelog where comid='" + (string)Session["comid"] + "'  and";
                if (qdptid != "")
                {
                    sqlstr += " dptid='" + qdptid + "'  and";
                }
                if (qempname != "")
                {
                    sqlstr += " empname like N'%" + qempname + "%'  and";
                }
                if (qclogsdate != "")
                {
                    sqlstr += " clogdate >= '" + qclogsdate + "'  and";
                }
                if (qclogedate != "")
                {
                    sqlstr += " clogdate <= '" + qclogedate + "'  and";
                }
                if (qclogstatus != "")
                {
                    sqlstr += " clogstatus = '" + qclogstatus + "'  and";
                }
                if (qempid != "")
                {
                    sqlstr += " empid = '" + qempid + "'  and";
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
            Excel += "<td>部門</td>";
            Excel += "<td>員工姓名</td>";
            Excel += "<td>刷卡日期</td>";
            Excel += "<td>上班刷卡</td>";
            Excel += "<td>下班刷卡</td>";
            Excel += "<td>星期</td>";
            Excel += "<td>工時</td>";
            Excel += "<td>差勤狀態</td>";
            Excel += "<td>備註</td>";
            Excel += "</tr>";

            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    string clogstatus = "";
                    while (dr.Read())
                    {
                        switch (dbobj.get_dbnull2(dr["clogstatus"]))
                        {
                            case "0":
                                clogstatus = "正常";
                                break;
                            case "1":
                                clogstatus = "遲到";
                                break;
                            case "2":
                                clogstatus = "早退";
                                break;
                            case "3":
                                clogstatus = "曠職";
                                break;
                            case "4":
                                clogstatus = "未到職";
                                break;
                            default:
                                clogstatus = "";
                                break;
                        }
                        //string dpttitle = dbobj.get_dbvalue(conn, "select dpttitle from department where dptid='" + dbobj.get_dbnull2(dr["dptid"]) + "'");
                        string tmpsql = "select dpttitle from Department where dptid='" + dbobj.get_dbnull2(dr["dptid"]) + "' and comid='" + (string)Session["comid"] + "'";
                        SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext");
                        string dpttitle = dbobj.get_dbvalue(conn1, tmpsql);
                        conn1.Close();
                        conn1.Dispose();

                        string tmpstime = dbobj.get_dbnull2((dr["clogstime"])).Trim();
                        if (tmpstime.Length == 6)
                        {
                            string tmpstime1 = tmpstime.Substring(0, 2);
                            if (tmpstime1 == "24")
                            {
                                tmpstime1 = "00";
                            }
                            tmpstime = tmpstime1 + ":" + tmpstime.Substring(2, 2) + ":" + tmpstime.Substring(4, 2);
                        }
                        else { tmpstime = ""; }

                        string tmpetime = dbobj.get_dbnull2((dr["clogetime"])).Trim();
                        if (tmpetime.Length == 6)
                        {
                            string tmpetime1 = tmpetime.Substring(0, 2);
                            if (tmpetime1 == "24")
                            {
                                tmpetime1 = "00";
                            }
                            tmpetime = tmpetime1 + ":" + tmpetime.Substring(2, 2) + ":" + tmpetime.Substring(4, 2);
                        }
                        else { tmpetime = ""; }

                        string weekofday = Convert.ToDateTime(dbobj.get_dbnull2((dr["clogdate"])).Trim()).ToString("ddd");
                        weekofday = weekofday.Substring(1, 1);

                        string thours = dr["cloghour"].ToString(); //工時                        
                        if (Convert.ToDouble(dr["cloghour"]) < 0)
                        {
                            thours = "0";
                        }

                        Excel2 += "<tr>";
                        Excel2 += "<td>" + dpttitle + "</td>";
                        Excel2 += "<td>" + dbobj.get_dbnull2(dr["empname"]) + "</td>";
                        Excel2 += "<td>" + Convert.ToDateTime(dr["clogdate"]).ToString("yyyy/MM/dd") + "</td>";
                        Excel2 += "<td>" + tmpstime + "</td>";
                        Excel2 += "<td>" + tmpetime + "</td>";
                        Excel2 += "<td>" + weekofday + "</td>";
                        Excel2 += "<td>" + thours + "</td>";
                        Excel2 += "<td>" + clogstatus.Trim() + "</td>";
                        Excel2 += "<td>" + dbobj.get_dbnull2((dr["clogcomment"])).Trim() + "</td>";
                        Excel2 += "</tr>";
                    }
                    if (Excel2 == "")
                    {
                        Excel += "<tr align=left><td colspan=9>目前沒有資料</td></tr>";
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


        public ActionResult monthcard(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "dptid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = ""; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            string qdptid = "", qempname = "", qclogsdate = "", qclogedate = "", qcchkstatus = "";
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
            if (!string.IsNullOrWhiteSpace(Request["qcchkstatus"]))
            {
                qcchkstatus = Request["qcchkstatus"].Trim();
                ViewBag.qcchkstatus = qcchkstatus;
            }
            string DateEx = "", DateEx1 = "";
                dbobj.get_dateRang(Request["qclogsdate"], "acd", "min", @"日期起格式錯誤!!\n", out qclogsdate, out DateEx);
                ViewBag.qclogsdate = qclogsdate;
            
                dbobj.get_dateRang(Request["qclogedate"], "acd", "max", @"日期訖格式錯誤!!\n", out qclogedate, out DateEx1);
                ViewBag.qclogedate = qclogedate;
            
            DateEx += DateEx1;
            
            //NullStDate 跟 NullTeDate 會判斷格式，有錯誤就 修改全域的DateEx
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }



            IPagedList<cardjudgelog> result;            
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select CAST(CAST(NEWID() AS binary(3)) AS int) as cjid, empid, empname, dptid ,NULL as clogdate,'' as clogstime, '' as clogetime, '' as clogstatus, '' as clogcomment,   sum(cloghour) as cloghour , '' as comid, '' as bmodid, NULL as bmoddate, cchkstatus, '' as cchkcomment, '' as cchkownman, NULL as cchkowndate  from cardjudgelog where comid='" + Session["comid"].ToString() + "'  and";
                
                if (!string.IsNullOrWhiteSpace(ViewBag.qdptid))
                {
                    sqlstr += " dptid = '" + ViewBag.qdptid + "'  and ";
                }
                if (!string.IsNullOrWhiteSpace(ViewBag.qempname))
                {
                    sqlstr += " empname like '%" + ViewBag.qempname + "%'  and ";
                }
                if (!string.IsNullOrWhiteSpace(ViewBag.qclogsdate))
                {
                    sqlstr += " clogdate >= '" + ViewBag.qclogsdate + "'  and ";
                }
                if (!string.IsNullOrWhiteSpace(ViewBag.qclogedate))
                {
                    sqlstr += " clogdate <= '" + ViewBag.qclogedate + "'  and ";
                }
                if (!string.IsNullOrWhiteSpace(ViewBag.qcchkstatus))
                {
                    sqlstr += " cchkstatus = '" + ViewBag.qcchkstatus + "'  and ";
                }
                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " group by dptid, empid, empname, cchkstatus order by dptid,empid";
                var query = con.cardjudgelog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<cardjudgelog>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch(orderdata, orderdata1);
            return View(result);
        }


        public ActionResult csvmonthcard(int? page, string orderdata, string orderdata1)
        {

            string qdptid = "", qempname = "", qclogsdate = "", qclogedate = "", qcchkstatus = "";
            string sql = "select CAST(CAST(NEWID() AS binary(3)) AS int) as cjid, empid, empname, dptid ,NULL as clogdate,'' as clogstime, '' as clogetime, '' as clogstatus, '' as clogcomment,   sum(cloghour) as cloghour , '' as comid, '' as bmodid, NULL as bmoddate, cchkstatus, '' as cchkcomment, '' as cchkownman, NULL as cchkowndate  from cardjudgelog where comid='" + Session["comid"].ToString() + "'  and";
            if (!string.IsNullOrWhiteSpace(Request["qdptid"].ToString()))
            {
                sql += " dptid = '" + Request["qdptid"].ToString() + "'  and ";
            }
            if (!string.IsNullOrWhiteSpace(Request["qempname"].ToString()))
            {
                sql += " empname like '%" + Request["qempname"].ToString() + "%'  and ";
            }
            if (!string.IsNullOrWhiteSpace(Request["qclogsdate"].ToString()))
            {
                sql += " clogdate >= '" + Request["qclogsdate"].ToString() + "'  and ";
            }
            if (!string.IsNullOrWhiteSpace(Request["qclogedate"].ToString()))
            {
                sql += " clogdate <= '" + Request["qclogedate"].ToString() + "'  and ";
            }
            if (!string.IsNullOrWhiteSpace(Request["qcchkstatus"].ToString()))
            {
                sql += " cchkstatus = '" + Request["qcchkstatus"].ToString() + "'  and ";
            }
            sql = sql.Substring(0, sql.Length - 5);
            sql += " group by dptid, empid, empname, cchkstatus order by dptid,empid";


            string Excel = "", Excel2 = "";

            Excel += "<HTML>";
            Excel += "<HEAD>";
            Excel += @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">";
            Excel += "</HEAD>";
            Excel += "<body>";
            Excel += "<table  border=1  cellpadding=0 cellspacing=0 bordercolor=#000000 bordercolordark=#ffffff width=900 >";
            Excel += "<tr align=center>";
            Excel += "<td>確認狀態</td>";
            Excel += "<td>差勤日期</td>";
            Excel += "<td>部門</td>";
            Excel += "<td>員工姓名</td>";
            Excel += "<td>工時</td>";
            Excel += "</tr>";

            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        string cchkstatus = "";
                        switch (dr["cchkstatus"].ToString())
                        {
                            case "0":
                                cchkstatus = "未確認";
                                break;
                            case "1":
                                cchkstatus = "發信通知";
                                break;
                            case "2":
                                cchkstatus = "已確認";
                                break;
                            default:
                                break;
                        }
                        
                        Excel2 += "<tr>";
                        Excel2 += "<td>" + cchkstatus.Trim() + "</td>";
                        Excel2 += "<td>" + Request["qclogsdate"].ToString() + "~" + Request["qclogedate"].ToString() + "</td>";
                        using (SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext"))
                        {
                            Excel2 += "<td>" + dbobj.get_dbvalue(conn1, "select dpttitle from Department where dptid='" + dr["dptid"].ToString() + "'") + "</td>";
                        }                        
                        Excel2 += "<td>" + dbobj.get_dbnull2(dr["empname"]) + "</td>";
                        Excel2 += "<td>" + dbobj.get_dbnull2(dr["cloghour"]) + "</td>";
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


        public ActionResult monthcardlist(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "clogdate"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = ""; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qclogstatus = "", qempid = "", qclogsdate = "", qclogedate = "";
            
            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {
                qempid = Request["qempid"].Trim();
                ViewBag.qempid = qempid;
            }
            qclogsdate = NullStDate(Request["qclogsdate"]);
            ViewBag.qclogsdate = qclogsdate;
            qclogedate = NullTeDate(Request["qclogedate"]);
            ViewBag.qclogedate = qclogedate;
            if (!string.IsNullOrWhiteSpace(Request["qclogstatus"]))
            {
                qclogstatus = Request["qclogstatus"].Trim();
                ViewBag.qclogstatus = qclogstatus;
            }
            
            //NullStDate 跟 NullTeDate 會判斷格式，有錯誤就 修改全域的DateEx
            
            IPagedList<cardjudgelog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "SELECT * FROM cardjudgelog where comid='" + (string)Session["comid"] + "'  and";

                if (qempid != "")
                {
                    sqlstr += " empid = '" + qempid + "'  and";
                }                
                if (qclogstatus != "")
                {
                    sqlstr += " clogstatus = '" + qclogstatus + "'  and";
                }
                if (qclogsdate != "")
                {
                    sqlstr += " clogdate >= '" + qclogsdate + "'  and";
                }
                if (qclogedate != "")
                {
                    sqlstr += " clogdate <= '" + qclogedate + "'  and";
                }


                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.cardjudgelog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<cardjudgelog>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch(orderdata, orderdata1);
            return View(result);
        }


        public ActionResult monthcardsave(int? page, string orderdata, string orderdata1)
        {
            //save................
            string strcjid = Request["cjid"].ToString();
            string[] arrcjid = strcjid.Split(',');
            string cchkstatus = Request["cchkstatus"].ToString();
            string today = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");

            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            foreach (var cjid in arrcjid)
            {
                string clogstime = Request["clogstime" + cjid].ToString().Replace(":", "");
                string clogetime = Request["clogetime" + cjid].ToString().Replace(":", "");
                string cchkcomment = Request["cchkcomment" + cjid];
                double cloghour = 0;
                if (clogstime != "" && clogetime != "") {
                    double hours = Convert.ToDouble("0" + clogetime.Substring(0, 2)) - Convert.ToDouble("0" + clogstime.Substring(0, 2));
                    double mints = Convert.ToDouble("0" + clogetime.Substring(2, 2)) - Convert.ToDouble("0" + clogstime.Substring(2, 2));
                    cloghour = hours + mints / 60 - 1.5;
                }


                if (cloghour > 0 || cchkcomment != "")
                {
                    string sql = "update cardjudgelog set ";
                    sql += "clogstime = '" + clogstime + "', ";
                    sql += "clogetime = '" + clogetime + "', ";
                    sql += "cloghour = '" + cloghour.ToString() + "', ";
                    if (cloghour >= 8)
                    {
                        sql += "clogstatus = '0', ";
                    }
                    if (cchkstatus == "2")
                    {
                        sql += "cchkownman = '" + Request["qempid"].ToString() + "', cchkowndate = '" + today + "',";
                    }
                    sql += "cchkcomment = '" + cchkcomment + "', ";
                    sql += "bmodid = '" + (string)Session["empid"] + "', ";
                    sql += "bmoddate = '" + today + "' ";
                    sql += "where cjid = '" + cjid + "' ";

                    dbobj.dbexecute("Aitag_DBContext", sql);
                }

                
            }

            //回monthcardlist..............
            string qempid = Request["qempid"].ToString();
            string qtheday = Request["qtheday"].ToString();
            string qclogstatus = Request["qclogstatus"].ToString();
            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/cardabnormallog/monthcardlist' method='post'>";
            tmpform += "<input type=hidden id='qempid' name='qempid' value='" + qempid + "'>";
            tmpform += "<input type=hidden id='qtheday' name='qtheday' value='" + qtheday + "'>";
            tmpform += "<input type=hidden id='qclogstatus' name='qclogstatus' value='" + qclogstatus + "'>";
            tmpform += "</form>";
            tmpform += "</body>";
            return new ContentResult() { Content = @"" + tmpform };
        }

        public ActionResult holidaylogcal(int? page, string orderdata, string orderdata1)
        {

            string qdptid = "", qempname = "", qclogsdate = "", qclogedate = "", qclogstatus = "", qempid = "";
            if (!string.IsNullOrWhiteSpace(Request["qdptid"]))
            {
                qdptid = Request["qdptid"].Trim();
            }
            if (!string.IsNullOrWhiteSpace(Request["qempname"]))
            {
                qempname = Request["qempname"].Trim();
            }
            if (!string.IsNullOrWhiteSpace(Request["qclogstatus"]))
            {
                qclogstatus = Request["qclogstatus"].Trim();
            }
            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {
                qempid = Request["qempid"].Trim();

            }
            if (!string.IsNullOrWhiteSpace(Request["qtheday"]))
            {
                DateTime qtheday = Convert.ToDateTime(Request["qtheday"]);
                DateTime qendday = qtheday.AddMonths(1).AddDays(-1);

                qclogsdate = qtheday.ToString("yyyy/MM/dd");
                qclogedate = qendday.ToString("yyyy/MM/dd");
            }
            else
            {
                qclogsdate = NullStDate(Request["qclogsdate"]);
                qclogedate = NullTeDate(Request["qclogedate"]);
            }

            //NullStDate 跟 NullTeDate 會判斷格式，有錯誤就 修改全域的DateEx
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }

            string sqlstr = "";
            int jstart = 0;
            int countday = 0;

            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            countday = Convert.ToDateTime(qclogedate).Subtract(Convert.ToDateTime(qclogsdate)).Days;
            dbobj.dbexecute("Aitag_DBContext", "delete cardjudgelog where clogdate>='" + qclogsdate + "' and clogdate<='" + qclogedate + "' and comid = '" + Session["comid"] + "'");

            SqlConnection comconn1 = dbobj.get_conn("Aitag_DBContext");




            for (int j = jstart; j <= countday; j++)
            {
                DateTime tmpdate = Convert.ToDateTime(qclogsdate).AddDays(j);

                using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                {
                    string sql = "select * from employee where empstatus<>'4' and empworkcomp='" + (string)Session["comid"] + "'";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        SqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            cardjudgelog rscard = new cardjudgelog();

                            string clogstime = "000000", clogetime = "000000", clogcomment = "";
                            string clogstatus = "";
                            using (SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext"))
                            {
                                int count1 = 0;
                                count1 = int.Parse(dbobj.get_dbvalue(conn1, "select isnull(count(*),0) as count1 from cardreallog where empid='" + dr["empid"] + "' and clogdate ='" + dbobj.get_date(tmpdate.ToString(), "1") + "'"));
                                sql = "select clogtime from cardreallog where empid='" + dr["empid"] + "' and clogdate ='" + dbobj.get_date(tmpdate.ToString(),"1") + "' order by clogtime";
                                using (SqlCommand cmd1 = new SqlCommand(sql, conn1))
                                {
                                    SqlDataReader dr1 = cmd1.ExecuteReader();
                                    if (dr1.HasRows)
                                    {
                                        if (count1 < 2)//'cardreallog 判斷小於每天兩次打卡時間
                                        {
                                            if(dr1.Read())
                                            { 
                                            clogstime = dr1["clogtime"].ToString();
                                            }
                                            else
                                            {
                                                clogstime = "000000";
                                            }
                                        }
                                        else
                                        {
                                            int k = 0;
                                            while (dr1.Read())
                                            {
                                                k++;
                                                if (k == 1)
                                                {
                                                    clogstime = dbobj.get_dbvalue(comconn1, "select top(1) clogtime from cardreallog where empid='" + dr["empid"].ToString() + "' and clogdate ='" + dbobj.get_date(tmpdate.ToString(), "1") + "' order by clogtime");
                                                }
                                                if (k == 2)
                                                {
                                                    clogetime = dbobj.get_dbvalue(comconn1, "select top(1) clogtime from cardreallog where empid='" + dr["empid"].ToString() + "' and clogdate ='" + dbobj.get_date(tmpdate.ToString(), "1") + "' order by clogtime desc");
                                                }

                                            }
                                        }
                                    }
                                    else
                                    {
                                        clogstime = "000000";
                                        clogetime = "000000";
                                    }
                                    dr1.Close();
                                     #region 整理rscard
                                    rscard.empid = dr["empid"].ToString();
                                    rscard.empname = dr["empname"].ToString();
                                    rscard.comid = dr["empworkcomp"].ToString();
                                    rscard.dptid = dr["empworkdepid"].ToString();
                                    rscard.clogdate = tmpdate;
                                    if (clogstime == "000000")
                                    { rscard.clogstime = "";}
                                    else
                                    { rscard.clogstime = clogstime; }
                                    if (clogetime == "000000")
                                    { rscard.clogetime = ""; }
                                    else
                                    { rscard.clogetime = clogetime; }
                                    if (clogstime.Length != 6)
                                    { clogstime = "000000"; }
                                    if (clogetime.Length != 6)
                                    { clogstime = "000000"; }
                                    string clogetime1 = "", clogstime1 = "";
                                    if (clogstime != "" || clogetime != "")
                                    {
                                        clogetime1 = (int.Parse(clogetime.Substring(0, 2)) * 60 + int.Parse(clogetime.Substring(2, 2))).ToString("000000");
                                        clogstime1 = (int.Parse(clogstime.Substring(0, 2)) * 60 + int.Parse(clogstime.Substring(2, 2))).ToString("000000");
                                        rscard.cloghour = Math.Round((decimal.Parse(clogetime1) - decimal.Parse(clogstime1)) / 60, 1);
                                    }
                                    else
                                    {
                                        rscard.cloghour = 0;
                                    }

                                    string hlogstatus = "",hloghour= "",hdayid="";
                                    dr1 = dbobj.dbselect(comconn1, "select * from holidaylog where empid='" + dr["empid"] + "' and comid='" + dr["empworkcomp"] + "' and hlogstatus='1' and hlogsdate<='" + dbobj.get_date(tmpdate.ToString(), "1") + "' and hlogedate>='" + dbobj.get_date(tmpdate.ToString(), "1") + "'");
                                    if(dr1.Read())
                                    {
                                        hlogstatus = dr1["hlogstatus"].ToString();
                                        hloghour = dr1["hloghour"].ToString();
                                        hdayid = dr1["hdayid"].ToString();
                                    }
                                    dr1.Close();
                                    //string hlogstatus = dbobj.get_dbvalue(comconn1, "select hlogstatus from holidaylog where empid='" + dr["empid"] + "' and comid='" + dr["empworkcomp"] + "' and hlogstatus='1' and hlogsdate<='" + dbobj.get_date(tmpdate.ToString(), "1") + "' and hlogedate>='" + dbobj.get_date(tmpdate.ToString(), "1") + "'");
                                    //string hloghour = dbobj.get_dbvalue(comconn1, "select hloghour from holidaylog where empid='" + dr["empid"] + "' and comid='" + dr["empworkcomp"] + "' and hlogstatus='1' and hlogsdate<='" + dbobj.get_date(tmpdate.ToString(), "1") + "' and hlogedate>='" + dbobj.get_date(tmpdate.ToString(), "1") + "'");
                                    //string hdayid = dbobj.get_dbvalue(comconn1, "select hdayid from holidaylog where empid='" + dr["empid"] + "' and comid='" + dr["empworkcomp"] + "' and hlogstatus='1' and hlogsdate<='" + dbobj.get_date(tmpdate.ToString(), "1") + "' and hlogedate>='" + dbobj.get_date(tmpdate.ToString(), "1") + "'");
                                    clogcomment = dbobj.get_dbvalue(comconn1, "select hdaytitle from holidaycode where hdayid='" + hdayid + "'");

                                    if (tmpdate.DayOfWeek.ToString("d") == "0" || tmpdate.DayOfWeek.ToString("d") == "6") //'例假日
                                    {
                                        using (SqlConnection conn3 = dbobj.get_conn("Aitag_DBContext"))
                                        {
                                            sql = "select wstitle from holidayschedule where comid='" + dr["empworkcomp"] + "'  and wstype='1' and wsdate='" + dbobj.get_date(tmpdate.ToString(), "1") + "'"; //'20160920因無年假身分別故先取消
                                            using (SqlCommand cmd3 = new SqlCommand(sql, conn3))
                                            {
                                                SqlDataReader dr3 = cmd3.ExecuteReader();
                                                if (dr3.Read())
                                                {

                                                    clogcomment = dr3["wstitle"].ToString();
                                                    if (hlogstatus == "1")
                                                    {
                                                        clogstatus = "0";
                                                    }
                                                    else
                                                    {
                                                        if (rscard.cloghour < 8)
                                                        {
                                                            if (int.Parse(clogstime) > 093000)
                                                            { clogstatus = "1"; }
                                                            if (int.Parse(clogetime) < 183000)
                                                            { clogstatus = "2"; }
                                                            if (rscard.cloghour == 0)
                                                            {
                                                                if (tmpdate >= DateTime.Parse(dr["jobdate"].ToString()))
                                                                { clogstatus = "3"; }
                                                                else
                                                                { clogstatus = "4"; }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            clogstatus = "0";
                                                        }
                                                    }
                                                }
                                                else
                                                { 
                                                clogstatus = "0";
                                                clogcomment = "假日";
                                                }

                                                dr3.Close();
                                            }
                                        }
                                    }
                                    else //'上班日
                                    {
                                        using (SqlConnection conn3 = dbobj.get_conn("Aitag_DBContext"))
                                        {
                                            sql = "select wstitle from holidayschedule where comid='" + dr["empworkcomp"] + "'  and wstype='0' and wsdate='" + dbobj.get_date(tmpdate.ToString(), "1") + "'"; //'20160920因無年假身分別故先取消
                                            using (SqlCommand cmd3 = new SqlCommand(sql, conn3))
                                            {
                                                SqlDataReader dr3 = cmd3.ExecuteReader();
                                                if (dr3.Read())
                                                {
                                                    clogcomment = dr3["wstitle"].ToString();
                                                    clogstatus = "0";                                                    
                                                }
                                                else
                                                {
                                                    if (hlogstatus == "1")
                                                    {
                                                        clogstatus = "0";
                                                    }
                                                    else
                                                    {
                                                        if (rscard.cloghour < 8)
                                                        {
                                                            if (int.Parse(clogstime) > 093000)
                                                            { clogstatus = "1"; }
                                                            if (int.Parse(clogetime) < 183000)
                                                            { clogstatus = "2"; }
                                                            if (rscard.cloghour == 0)
                                                            {
                                                                if (tmpdate >= DateTime.Parse(dr["jobdate"].ToString()))
                                                                { clogstatus = "3"; }
                                                                else
                                                                { clogstatus = "4"; }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            clogstatus = "0";
                                                        }
                                                    }
                                                }

                                                dr3.Close();
                                            }
                                        }
                                    }
                                    rscard.clogstime = clogstime;
                                    rscard.clogetime = clogetime;
                                    rscard.clogstatus = clogstatus;
			                        rscard.clogcomment = clogcomment;
			                        rscard.comid = (string)Session["comid"];
			                        rscard.bmodid = (string)Session["empid"];
                                    rscard.bmoddate = DateTime.Now;    
                                     #endregion
                                }
                            }

                            #region 整理rscard



                            #endregion
                            using (Aitag_DBContext con = new Aitag_DBContext())
                            {
                                con.cardjudgelog.Add(rscard);
                                con.SaveChanges();
                            }
                        }
                        dr.Close();
                    }
                }
            }
            comconn1.Close();
            comconn1.Dispose();

            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/cardabnormallog/List' method='post'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='qclogsdate' id='qclogsdate' value='" + qclogsdate + "'>";
            tmpform += "<input type=hidden name='qclogedate' id='qclogedate' value='" + qclogedate + "'>";
            tmpform += "</form>";
            tmpform += "</body>";
            return new ContentResult() { Content = @"<script>alert('差勤轉入成功!!');</script>" + tmpform };
        }
    }
}
