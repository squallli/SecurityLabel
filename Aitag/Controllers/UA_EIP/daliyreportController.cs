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
    public class daliyreportController : BaseController
    {
        string DateEx = "";
        
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }


        public ActionResult daliyreportrpt(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "empworkdepid,empid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qdptid = "", qhlogstatus = "", qempname = "", qworksdate = "", qworkedate = "", hdayid = "";
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

            //繞細項用 qhlogstatus qworksdate qworkedate
            if (!string.IsNullOrWhiteSpace(Request["qhlogstatus"]))
            {
                qhlogstatus = Request["qhlogstatus"].Trim();
                ViewBag.qhlogstatus = qhlogstatus;
            }
            qworksdate = NullStDate(Request["qworksdate"]);
            ViewBag.qworksdate = qworksdate;
            qworkedate = NullTeDate(Request["qworkedate"]);
            ViewBag.qworkedate = qworkedate;
            //NullStDate 跟 NullTeDate 會判斷格式，有錯誤就 修改全域的DateEx
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }

            if (!string.IsNullOrWhiteSpace(Request["hdayid"]))
            {
                hdayid = Request["hdayid"].Trim();
                ViewBag.hdayid1 = get_hdaytitle(hdayid);
            }
            else
            {
                hdayid = "A01,A02,A03,A04,A05,A06";
                ViewBag.hdayid1 = get_hdaytitle(hdayid);

            }

            string Excel = "", Excel2 = "";
            string sqlstr = "";
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                sqlstr = "select * from employee where empstatus<>'4' and empworkcomp='" + (string)Session["comid"] + "'  and";

                //'部門 組多筆
                if (qdptid != "")
                {
                    string tmpa = "";
                    tmpa += "'";
                    tmpa += qdptid.Replace(",", "','");
                    tmpa += "'";
                    sqlstr += " empworkdepid in (" + tmpa + ")  and";
                }
                if (qempname != "")
                {
                    sqlstr += " empname like N'%" + qempname + "%'  and";
                }

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
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
            Excel += @"<td colspan=""11"" style=""font-size:14pt"">員工請假統計表  ";
            if (qworksdate != "" || qworkedate != "")
            {
                Excel += qworksdate + "~" + qworkedate;
            }
            Excel += "</td>";
            Excel += "</tr>";
            Excel += "<tr align=center>";
            int count = ViewBag.hdayid1.Count + 3;
            Excel += "<td colspan='" + count + "' ></td><td>製表日期：" + DateTime.Now.ToString("yyyy/MM/dd") + "</td>";
            Excel += "</tr>";
            Excel += "<tr align=center>";
            Excel += "<td>部門</td>";
            Excel += "<td>員工姓名</td>";
            Excel += "<td>到職日</td>";
            foreach (string v in ViewBag.hdayid1)
            { Excel += "<td>" + v + "</td>"; }
            Excel += "<td>合計時</td>";
                        

            Excel += "</tr>";
            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();

                    string dpttitle = "", empname = "";
                    using (SqlConnection comconn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        while (dr.Read())
                        {
                            using (SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext"))
                            {
                                dpttitle = dbobj.get_dbvalue(conn1, "select dpttitle form Department where dptid='" + dr["empworkdepid"] + "'");
                                empname = dbobj.get_dbnull2(dr["empname"]);
                            }

                            Excel2 += "<tr>";
                            Excel2 += "<td>" + dpttitle + "</td>";
                            Excel2 += "<td>" + empname + "</td>";
                            Excel2 += "<td>" + Convert.ToDateTime(dr["jobdate"]).ToString("yyyy/MM/dd") + "</td>";

                            Excel2 += get_daliyreportHour(comconn, dbobj.get_dbnull2(dr["empid"]), dbobj.get_dbnull2(dr["empworkcomp"]));

                            Excel2 += "</tr>";
                            //dbobj.get_dbnull2().Trim()
                        }
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

            #endregion

            ViewBag.Excel = Excel;
            return View();
        }

        public ActionResult List(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "empworkdepid,empid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qdptid = "", qhlogstatus = "", qempname = "", qworksdate = "", qworkedate = "", hdayid = "";
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

            //繞細項用 qhlogstatus qworksdate qworkedate
            if (!string.IsNullOrWhiteSpace(Request["qhlogstatus"]))
            {
                qhlogstatus = Request["qhlogstatus"].Trim();
                ViewBag.qhlogstatus = qhlogstatus;
            }
            qworksdate = NullStDate(Request["qworksdate"]);
            ViewBag.qworksdate = qworksdate;
            qworkedate = NullTeDate(Request["qworkedate"]);
            ViewBag.qworkedate = qworkedate;
            //NullStDate 跟 NullTeDate 會判斷格式，有錯誤就 修改全域的DateEx
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }

            if (!string.IsNullOrWhiteSpace(Request["hdayid"]))
            {
                hdayid = Request["hdayid"].Trim();
                ViewBag.hdayid1 = get_hdaytitle(hdayid);
            }
            else 
            {
                hdayid = "A01,A02,A03,A04,A05,A06";
                ViewBag.hdayid1 = get_hdaytitle(hdayid);
                
            }

            IPagedList<employee> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from employee where empstatus<>'4' and empworkcomp='" + (string)Session["comid"] + "'  and";

                //'部門 組多筆
                if(qdptid != "")
                {
                    string tmpa = "";
                    tmpa += "'";
                    tmpa += qdptid.Replace(",","','");
                    tmpa += "'";
                    sqlstr += " empworkdepid in (" + tmpa + ")  and";
                }
                if (qempname != "")
                {
                    sqlstr += " empname like N'%" + qempname + "%'  and";
                }

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.employee.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<employee>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }

        private object get_hdaytitle(string hdayid)
        {
            string hdayid1 = hdayid.Replace(",", "','").Replace(" ","");
            hdayid1 = "'" + hdayid1 + "'";
            string sqlstr = "select hdayid, hdaytitle from holidaycode where hdayid in (" + hdayid1 + ") order by hdayid";
            System.Collections.ArrayList list = new System.Collections.ArrayList();

            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    hdayid = "";
                    while (dr.Read())
                    {
                        list.Add(dbobj.get_dbnull2(dr["hdaytitle"]));
                        hdayid += dr["hdayid"] + ",";
                    }
                    dr.Close();
                }
            }
            hdayid = hdayid.Substring(0, hdayid.Length - 1);
            ViewBag.hdayid = hdayid;
            return list;
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

        private string get_daliyreportHour(SqlConnection tmpconn, string empid, string empworkcomp)
        {
            string sql_1 = "";
            string[] hdayid1 = ViewBag.hdayid.Split(',');
            foreach (string id in hdayid1)
            {
                if (id != "")
                {
                    sql_1 += "(select isnull(sum(hloghour),0) from holidaylog where empid='" + empid + "' and hdayid='" + id + "'  and";

                    sql_1 += " ( hlogtype='1' or ( hlogtype='2' and (phsno<>'' or phsno is not null) ) ) and comid='" + empworkcomp + "'";
                    if (string.IsNullOrWhiteSpace(ViewBag.qhlogstatus))
                    {
                        sql_1 += " and hlogstatus in('0','1')";
                    }
                    else
                    {
                        sql_1 += " and hlogstatus IN ('" + ViewBag.qhlogstatus + "')";
                    }
                    if (!string.IsNullOrWhiteSpace(ViewBag.qworksdate))
                    {
                        sql_1 += " and hlogsdate >='" + ViewBag.qworksdate + "'";
                    }
                    if (!string.IsNullOrWhiteSpace(ViewBag.qworkedate))
                    {
                        sql_1 += " and hlogsdate <='" + ViewBag.qworkedate + "'";
                    }
                    sql_1 += ") as " + id + ", ";
                }
            }
            sql_1 = sql_1.Substring(0, sql_1.Length - 2);
            sql_1 = "select top(1) " + sql_1 + " from holidaylog";

            string Tdlist = "";
            double idhour = 0, sumhour = 0;

            using (SqlCommand cmd = new SqlCommand(sql_1, tmpconn))
            {
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Read();
                    foreach (string id in hdayid1)
                    {
                        if (id != "")
                        {
                            idhour = double.Parse(dr[id].ToString());
                            Tdlist += String.Format("<td>{0}</td>", idhour.ToString("###0"));
                            sumhour += idhour;
                        }
                    }

                }
                Tdlist += String.Format("<td>{0}</td>", sumhour.ToString("###0"));
                dr.Close();
            }


            return Tdlist;
        }








    }
}
