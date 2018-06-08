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
    public class respController : BaseController
    {
        string DateEx = "";

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }



        #region 主管專區 > 員工請假查詢

        public ActionResult empholidaylogEdit(holidaylog chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "holidaylog.hlogsdate"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qdptid = "", qempname = "", qhdayid = "", qhlogsdate = "", qhlogedate = "", qhlogstatus = "";
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
            if (!string.IsNullOrWhiteSpace(Request["qhdayid"]))
            {
                qhdayid = Request["qhdayid"].Trim();
                ViewBag.qhdayid = qhdayid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qhlogsdate"]))
            {
                qhlogsdate = Request["qhlogsdate"].Trim();
                ViewBag.qhlogsdate = qhlogsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qhlogedate"]))
            {
                qhlogedate = Request["qhlogedate"].Trim();
                ViewBag.qhlogedate = qhlogedate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qhlogstatus"]))
            {
                qhlogstatus = Request["qhlogstatus"].Trim();
                ViewBag.qhlogstatus = qhlogstatus;
            }


            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.holidaylog.Where(r => r.hlogid == chks.hlogid).FirstOrDefault();
                    holidaylog eholidaylogs = con.holidaylog.Find(chks.hlogid);
                    if (eholidaylogs == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eholidaylogs);
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
                    NDcommon dbobj = new NDcommon();
                    if (dbobj.get_dbnull2(Request["hlogstime"]) == "")
                    {
                        string hlogstime = "", hlogetime = "";
                        hlogstime = Request["hlogshours"] + Request["hlogsmin"];
                        hlogetime = Request["hlogehours"] + Request["hlogemin"];
                        chks.hlogstime = hlogstime;
                        chks.hlogetime = hlogetime;
                    }

                    chks.empmeetsign = Request["empmeetsign"];
                    chks.hfile = Request["hfile"];
                    chks.hlogcomment = Request["hlogcomment"];


                    chks.hlogid = int.Parse(Request["hlogid"].Trim());
                    chks.bmodid = Session["tempid"].ToString();
                    chks.bmoddate = DateTime.Now;

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        //con.Entry(chks).State = EntityState.Modified;
                        //con.SaveChanges();
                    }


                    //系統LOG檔
                    string sysnote = "";
                    if (sysnote.Length > 4000) { sysnote = sysnote.Substring(0, 4000); }
                    //================================================= //
                    SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    string sysrealsid = Request["sysrealsid"].ToString();
                    string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    //string sysnote = "代碼:" + chks.hlogid + "名稱:" + chks.empname;
                    dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    sysconn.Close();
                    sysconn.Dispose();
                    //=================================================

                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/resp/empholidaylogList' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qdptid' name='qdptid' value='" + qdptid + "'>";
                    tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
                    tmpform += "<input type=hidden id='qhdayid' name='qhdayid' value='" + qhdayid + "'>";
                    tmpform += "<input type=hidden id='qhlogsdate' name='qhlogsdate' value='" + qhlogsdate + "'>";
                    tmpform += "<input type=hidden id='qhlogedate' name='qhlogedate' value='" + qhlogedate + "'>";
                    tmpform += "<input type=hidden id='qhlogstatus' name='qhlogstatus' value='" + qhlogstatus + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    //return RedirectToAction("List");
                }
            }

        }

        public ActionResult empholidaylogrpt(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "holidaylog.hlogsdate"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qdptid = "", qempname = "", qhdayid = "", qhlogsdate = "", qhlogedate = "", qhlogstatus = "";
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
            if (!string.IsNullOrWhiteSpace(Request["qhdayid"]))
            {
                qhdayid = Request["qhdayid"].Trim();
                ViewBag.qhdayid = qhdayid;
            }

            NDcommon dbobj = new NDcommon();

            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qhlogsdate"], "m", "min", @"請假起格式錯誤!!\n", out qhlogsdate, out DateEx);
            ViewBag.qhlogsdate = qhlogsdate;
            dbobj.get_dateRang(Request["qhlogedate"], "m", "max", @"請假訖格式錯誤!!\n", out qhlogedate, out DateEx1);
            ViewBag.qhlogedate = qhlogedate;
            DateEx += DateEx1;
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }

            if (!string.IsNullOrWhiteSpace(Request["qhlogstatus"]))
            {
                qhlogstatus = Request["qhlogstatus"].Trim();
                ViewBag.qhlogstatus = qhlogstatus;
            }
            else
            {
                qhlogstatus = "1";
                ViewBag.qhlogstatus = qhlogstatus;
            }


            string Excel = "", Excel2 = "";
            string sqlstr = "", sqlstr_1 = "";

            string viewid = "";
            string[] mpriv = (string[])Session["priv"];
            //viewid = get_viewpriv(int.Parse(funcpriv(2)), int.Parse(mpriv(realsid, 2)));
            string tmpall = "";
            //tmpall = dbobj.get_allempid((string)Session["rid"]);
            tmpall = "%";


            sqlstr = "";
            if (tmpall == "%")
            {
                sqlstr = "SELECT holidaylog.* FROM employee INNER JOIN holidaylog ON employee.empid = holidaylog.empid where (holidaylog.hlogtype='1' or (holidaylog.hlogtype='2' and (holidaylog.phsno='' or holidaylog.phsno is null )) or (holidaylog.hlogtype='3' and (holidaylog.phsno='' or holidaylog.phsno is null )) ) and holidaylog.comid='" + (string)Session["comid"] + "'  and";
            }
            else
            {
                sqlstr = "SELECT holidaylog.* FROM employee INNER JOIN holidaylog ON employee.empid = holidaylog.empid where holidaylog.empid in (" + tmpall + ") and (holidaylog.hlogtype='1' or (holidaylog.hlogtype='2' and (holidaylog.phsno='' or holidaylog.phsno is null )) or (holidaylog.hlogtype='3' and (holidaylog.phsno='' or holidaylog.phsno is null )) ) and holidaylog.comid='" + (string)Session["comid"] + "'  and";
            }

            if (qdptid != "")
            {
                sqlstr += " holidaylog.dptid='" + qdptid + "'  and";
            }
            if (qempname != "")
            {
                sqlstr += " holidaylog.empname like N'%" + qempname + "%'  and";
            }
            if (qhdayid != "")
            {
                sqlstr += " holidaylog.hdayid='" + qhdayid + "'  and";
            }
            if (qhlogsdate != "" && qhlogedate != "")
            {
                sqlstr += " (( holidaylog.hlogsdate >= '" + qhlogsdate + "' and holidaylog.hlogsdate <= '" + qhlogedate + "' ) or ";
                sqlstr += "( holidaylog.hlogedate >= '" + qhlogsdate + "' and holidaylog.hlogedate <= '" + qhlogedate + "'))  and";
            }
            if (qhlogstatus != "")
            {
                sqlstr += " holidaylog.hlogstatus like '" + qhlogstatus + "'  and";
            }
            sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
            sqlstr += " order by " + orderdata + " " + orderdata1;

            #region 組 Excel 格式
            Excel += "<HTML>";
            Excel += "<HEAD>";
            Excel += @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">";
            Excel += "</HEAD>";
            Excel += "<body>";
            Excel += "<table  border=1  cellpadding=0 cellspacing=0 bordercolor=#000000 bordercolordark=#ffffff width=900 >";
            Excel += "<tr align=center>";
            Excel += @"<td colspan=""8"" style=""font-size:14pt"">請假明細表";
            Excel += "</td>";
            Excel += "</tr>";
            Excel += "<tr align=center>";
            int count = 7;
            Excel += "<td colspan='" + count + "' ></td><td>列印日期：" + DateTime.Now.ToString("yyyy/MM/dd") + "</td>";
            Excel += "</tr>";
            Excel += "<tr align=center>";
            Excel += "<td>狀態</td>";
            Excel += "<td>假別</td>";
            Excel += "<td>部門</td>";
            Excel += "<td>申請人</td>";
            Excel += "<td>請假起迄日期</td>";
            Excel += "<td>請假時數</td>";
            Excel += "<td>地點</td>";
            Excel += "<td>待簽核角色</td>";
            Excel += "</tr>";
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    string hlogstatus = "", hlogtype = "", hdaytitle = "", dpttitle = "", chkitem = "";

                    string SEtime = "{0} {1}<br>{2} {3}";
                    string hlogsdate = "", hlogstime = "", hlogedate = "", hlogetime = "";

                    using (SqlConnection comconn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        while (dr.Read())
                        {
                            hlogstatus = dbobj.get_dbnull2(dr["hlogstatus"]);
                            switch (hlogstatus)
                            {
                                case "0":
                                    hlogstatus = "簽核中";
                                    break;
                                case "1":
                                    hlogstatus = "已核准";
                                    break;
                                case "2":
                                    hlogstatus = "退回";
                                    break;
                                case "3":
                                    hlogstatus = "代理人簽核中";
                                    break;
                                case "D":
                                    hlogstatus = "撤回";
                                    break;
                                default:
                                    break;
                            }
                            if (dbobj.get_dbnull2(dr["hlogtype"]) == "2")
                            {
                                hlogtype = "集體<br>";
                            }
                            else if (dbobj.get_dbnull2(dr["hlogtype"]) == "3")
                            {
                                hlogtype = "週期<br>";
                            }
                            if (hlogtype != "")
                            {
                                hlogtype = "<b><font color=cc3322>" + hlogtype + "</font></b>";
                            }

                            hlogsdate = Convert.ToDateTime(dbobj.get_dbnull2(dr["hlogsdate"])).ToString("yyyy/MM/dd");
                            hlogstime = decimal.Parse("0" + dbobj.get_dbnull2(dr["hlogstime"]).ToString()).ToString("####");
                            hlogedate = Convert.ToDateTime(dbobj.get_dbnull2(dr["hlogedate"])).ToString("yyyy/MM/dd");
                            hlogetime = decimal.Parse("0" + dbobj.get_dbnull2(dr["hlogetime"]).ToString()).ToString("####");

                            hdaytitle = "select hdaytitle from holidaycode where hdayid = '" + dbobj.get_dbnull2(dr["hdayid"]) + "'"; hdaytitle = dbobj.get_dbvalue(comconn, hdaytitle);
                            dpttitle = "select dpttitle from Department where dptid='" + dbobj.get_dbnull2(dr["dptid"]) + "' and comid='" + (string)Session["comid"] + "'"; dpttitle = dbobj.get_dbvalue(comconn, dpttitle);
                            chkitem = "select chkitem from checkcode where chkclass = '90' and chkcode = '" + dbobj.get_dbnull2(dr["hlogaddr"]) + "'"; chkitem = dbobj.get_dbvalue(comconn, chkitem);


                            Excel2 += "<tr>";
                            Excel2 += "<td>" + hlogstatus + "</td>";
                            Excel2 += "<td>" + hlogtype + hdaytitle + "</td>";
                            Excel2 += "<td>" + dpttitle + "</td>";
                            Excel2 += "<td>" + dbobj.get_dbnull2(dr["empname"]) + "</td>";
                            Excel2 += "<td>" + string.Format(SEtime, hlogsdate, hlogstime, hlogedate, hlogetime) + "</td>";
                            Excel2 += "<td>" + decimal.Parse("0" + dbobj.get_dbnull2(dr["hloghour"])).ToString("#") + "</td>";
                            Excel2 += "<td>" + chkitem + "</td>";
                            Excel2 += "<td>" + dbobj.getrole(dbobj.get_dbnull2(dr["rolestampid"])) + "</td>";
                            Excel2 += "</tr>";
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
            ViewBag.Excel = Excel;
            #endregion


            return View();
        }

        public ActionResult empholidaylogList(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "holidaylog.hlogsdate"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qdptid = "", qempname = "", qhdayid = "", qhlogsdate = "", qhlogedate = "", qhlogstatus = "";
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
            if (!string.IsNullOrWhiteSpace(Request["qhdayid"]))
            {
                qhdayid = Request["qhdayid"].Trim();
                ViewBag.qhdayid = qhdayid;
            }
            NDcommon dbobj = new NDcommon();

            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qhlogsdate"], "m", "min", @"請假起格式錯誤!!\n", out qhlogsdate, out DateEx);
            ViewBag.qhlogsdate = qhlogsdate;
            dbobj.get_dateRang(Request["qhlogedate"], "m", "max", @"請假訖格式錯誤!!\n", out qhlogedate, out DateEx1);
            ViewBag.qhlogedate = qhlogedate;
            DateEx += DateEx1;
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }

            if (!string.IsNullOrWhiteSpace(Request["qhlogstatus"]))
            {
                qhlogstatus = Request["qhlogstatus"].Trim();
                ViewBag.qhlogstatus = qhlogstatus;
            }
            else
            {
                qhlogstatus = "1";
                ViewBag.qhlogstatus = qhlogstatus;
            }



            IPagedList<holidaylog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string viewid = "";
                string[] mpriv = (string[])Session["priv"];
                //viewid = get_viewpriv(int.Parse(funcpriv(2)), int.Parse(mpriv(realsid, 2)));
                string tmpall = "";
                tmpall = dbobj.get_allempid((string)Session["rid"], (string)Session["empid"]);
                


                string sqlstr = "";
                if (tmpall == "%")
                {
                    sqlstr = "SELECT holidaylog.* FROM employee INNER JOIN holidaylog ON employee.empid = holidaylog.empid where (holidaylog.hlogtype='1' or (holidaylog.hlogtype='2' and (holidaylog.phsno='' or holidaylog.phsno is null )) or (holidaylog.hlogtype='3' and (holidaylog.phsno='' or holidaylog.phsno is null )) ) and holidaylog.comid='" + (string)Session["comid"] + "'  and";
                }
                else
                {
                    sqlstr = "SELECT holidaylog.* FROM employee INNER JOIN holidaylog ON employee.empid = holidaylog.empid where holidaylog.empid in (" + tmpall + ") and (holidaylog.hlogtype='1' or (holidaylog.hlogtype='2' and (holidaylog.phsno='' or holidaylog.phsno is null )) or (holidaylog.hlogtype='3' and (holidaylog.phsno='' or holidaylog.phsno is null )) ) and holidaylog.comid='" + (string)Session["comid"] + "'  and";
                }

                if (qdptid != "")
                {
                    sqlstr += " holidaylog.dptid='" + qdptid + "'  and";
                }
                if (qempname != "")
                {
                    sqlstr += " holidaylog.empname like N'%" + qempname + "%'  and";
                }
                if (qhdayid != "")
                {
                    sqlstr += " holidaylog.hdayid='" + qhdayid + "'  and";
                }
                if (qhlogsdate != "" && qhlogedate != "")
                {
                    sqlstr += " (( holidaylog.hlogsdate >= '" + qhlogsdate + "' and holidaylog.hlogsdate <= '" + qhlogedate + "' ) or ";
                    sqlstr += "( holidaylog.hlogedate >= '" + qhlogsdate + "' and holidaylog.hlogedate <= '" + qhlogedate + "'))  and";
                }
                if (qhlogstatus != "")
                {
                    sqlstr += " holidaylog.hlogstatus like '" + qhlogstatus + "'  and";
                }







                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.holidaylog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<holidaylog>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch1(orderdata, orderdata1);
            return View(result);
        }
        private string SetOrder_ch1(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "hlogstatus", "hdayid", "dptid", "employee.empid", "hlogsdate" };
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

        #endregion

        #region 主管專區 > 員工請假統計

        public ActionResult empdaliyreport(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "empworkdepid,empname"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "ASC"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qempname = "", qworksdate = "", qworkedate = "";

            if (!string.IsNullOrWhiteSpace(Request["qempname"]))
            {
                qempname = Request["qempname"].Trim();
                ViewBag.qempname = qempname;
            }

            NDcommon dbobj = new NDcommon();
            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qworksdate"], "y", "min", @"日期起格式錯誤!!\n", out qworksdate, out DateEx);
            ViewBag.qworksdate = qworksdate;/*傳到前端做子查詢*/
            dbobj.get_dateRang(Request["qworkedate"], "y", "max", @"日期訖格式錯誤!!\n", out qworkedate, out DateEx1);
            ViewBag.qworkedate = qworkedate;
            DateEx += DateEx1;
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }


            //getCol 繞欄位
            List<getCol> getColList = new List<getCol>();
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                string sql = "select * from holidaycode order by hdayid";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        getColList.Add(new getCol() { hdayid = dr["hdayid"].ToString(), hdaytitle = dr["hdaytitle"].ToString() });
                    }
                    dr.Close();
                }
            }


            ViewBag.getCol = getColList;
            IPagedList<employee> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {

                string tmpall = "";
                tmpall = dbobj.get_allempid((string)Session["rid"], (string)Session["empid"]);

                string sqlstr = "select * from employee where empstatus<>'4'";

                if (tmpall == "%")
                {
                    sqlstr += " and empworkcomp ='" + (string)Session["comid"] + "'";
                }
                else
                {
                    sqlstr += " and empid in (" + tmpall + ") and empworkcomp='" + (string)Session["comid"] + "'";
                }

                if (qempname != "")
                {
                    sqlstr += " and empname like '%" + qempname + "%'";
                }

                sqlstr += " order by " + orderdata + " " + orderdata1;
                var query = con.employee.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<employee>(page.Value - 1, (int)Session["epagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch2(orderdata, orderdata1);
            return View(result);
        }
        private string SetOrder_ch2(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "empworkdepid", "empname", "jobdate" };
            string[] od_ch1 = { "asc", "asc", "asc" };
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
        #endregion

        #region 主管專區 > 員工假況統計
        public ActionResult emphdreport1List(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "slyear"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qslyear = "", qempname = "", qhchange = "";
            if (!string.IsNullOrWhiteSpace(Request["qslyear"]))
            {
                qslyear = Request["qslyear"].Trim();
                ViewBag.qslyear = qslyear;
            }
            else
            {
                qslyear = DateTime.Now.Year.ToString();
                ViewBag.qslyear = qslyear;
            }

            if (!string.IsNullOrWhiteSpace(Request["qempname"]))
            {
                qempname = Request["qempname"].Trim();
                ViewBag.qempname = qempname;
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



            NDcommon dbobj = new NDcommon();
            //string empidIN = "";
            //using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            //{

                

            //    using (SqlCommand cmd = new SqlCommand(sql, conn))
            //    {
            //        SqlDataReader dr = cmd.ExecuteReader();
            //        while (dr.Read())
            //        {
            //            empidIN += "'" + dr["empid"] + "',";
            //        }
            //        dr.Close();
            //    }
            //} 
            //if(empidIN != "")
            //{
            //    empidIN = empidIN.Substring(0, empidIN.Length -1);
            //}
            //else
            //{
            //    empidIN = "null";
            //}

            IPagedList<emphdlog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string tmpall = "";
                tmpall = dbobj.get_allempid((string)Session["rid"], (string)Session["empid"]);

                string sql = "select CAST(CAST(NEWID() AS binary(3)) AS int) as hid, ";
                sql += " slyear, empid, empname,'n' as hchange,  null as factsday, null as dptid, null as hdayid, null as effectiveday, null as allhour, null as usehour, null as comid, null as bmodid, null as bmoddate from emphdlog";
                sql += " where empid in (select   empid from employee) ";
                sql += " ";

                if (tmpall == "%")
                {
                    sql += " and comid='" + (string)Session["comid"] + "'";
                }
                else
                {
                    sql += " and empid in (" + tmpall + ") and comid='" + (string)Session["comid"] + "'";
                }

                if (qslyear != "")
                {
                    sql += " and slyear = '" + qslyear + "'";
                }
                if (qempname != "")
                {
                    sql += " and empname like N'%" + qempname + "%'";
                }
               // if (qhchange != "")
               // {
               //     sql += " and hchange = '" + qhchange + "'";
               // }
                sql += " GROUP BY slyear,empid,empname ,hchange ";
                sql += " order by " + orderdata + " " + orderdata1;
                var query = con.emphdlog.SqlQuery(sql).AsQueryable();
                result = query.ToPagedList<emphdlog>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }

        public ActionResult emphdreport1Edit(emphdlog chks, string sysflag, int? page, string orderdata, string orderdata1, HttpPostedFileBase logopic1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "clogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qslyear = "", qempname = "", qhchange = "";
            if (!string.IsNullOrWhiteSpace(Request["qslyear"]))
            {
                qslyear = Request["qslyear"].Trim();
                ViewBag.qslyear = qslyear;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempname"]))
            {
                qempname = Request["qempname"].Trim();
                ViewBag.qempname = qempname;
            }
            if (!string.IsNullOrWhiteSpace(Request["qhchange"]))
            {
                qhchange = Request["qhchange"].Trim();
                ViewBag.qhchange = qhchange;
            }

            if (sysflag != "E")
            {
                NDcommon dbobj = new NDcommon();
                using (Aitag_DBContext con = new Aitag_DBContext())
                {

                    chks.comid = (string)Session["comid"];
                    var data = con.emphdlog.Where(r => r.empid == chks.empid && r.slyear == chks.slyear && r.hchange == chks.hchange && r.comid == chks.comid);
                    //emphdlog eemphdlogs = con.emphdlog.Find(chks.empid);
                    if (data == null)
                    {
                        return HttpNotFound();
                    }
                    var result = data.ToList();
                    ViewBag.result = result;
                }

                #region 補休狀況 

                string sqlstr = "select isnull(sum(resthour-usehour),0) as countall from resthourlog where rsdeaddate >= '" + DateTime.Now.ToString("yyyy/MM/dd") + "' and empid = '" + chks.empid + "' and resthour - usehour > 0 and comid='" + chks.comid + "'";
                using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                {
                    ViewBag.countall = dbobj.get_dbvalue(conn, sqlstr);
                }

                sqlstr = "select * from resthourlog where rsdeaddate >= '" + DateTime.Now.ToString("yyyy/MM/dd") + "' and empid = '" + chks.empid + "' and resthour - usehour > 0 and comid='" + chks.comid + "' order by rsdeaddate";
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var query = con.resthourlog.SqlQuery(sqlstr).AsQueryable();
                    ViewBag.tmprs = query.ToList();
                }

                #endregion

                return View();


            }
            else
            {

                if (!ModelState.IsValid)
                {
                    return View(chks);
                }
                else
                {
                    //NDcommon dbobj = new NDcommon();



                    //chks.bmodid = Session["tempid"].ToString();
                    //chks.bmoddate = DateTime.Now;

                    //using (Aitag_DBContext con = new Aitag_DBContext())
                    //{
                    //    con.Entry(chks).State = EntityState.Modified;
                    //    con.SaveChanges();
                    //}

                    //系統LOG檔
                    string sysnote = "";
                    if (sysnote.Length > 4000) { sysnote = sysnote.Substring(0, 4000); }
                    //================================================= //

                    //SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    //string sysrealsid = Request["sysrealsid"].ToString();
                    //string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    //dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    //sysconn.Close();
                    //sysconn.Dispose();
                    //=================================================

                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/resp/emphdreport1List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

                    tmpform += "<input type=hidden id='qslyear' name='qslyear' value='" + qslyear + "'>";
                    tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
                    tmpform += "<input type=hidden id='qhchange' name='qhchange' value='" + qhchange + "'>";

                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"" + tmpform };
                }
            }

        }


        #endregion

        #region 主管專區 > 員工出差查詢

        public ActionResult empbattalogEdit(battalog chks, string sysflag, int? page, string orderdata, string orderdata1, HttpPostedFileBase logopic1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "clogid"; }

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
            if (!string.IsNullOrWhiteSpace(Request["qblogsdate"]))
            {
                qblogsdate = Request["qblogsdate"].Trim();
                ViewBag.qblogsdate = qblogsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qblogedate"]))
            {
                qblogedate = Request["qblogedate"].Trim();
                ViewBag.qblogedate = qblogedate;
            }

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    //var data = con.battalog.Where(r => r.clogid == chks.clogid).FirstOrDefault();
                    battalog ebattalogs = con.battalog.Find(chks.blogid);
                    if (ebattalogs == null)
                    {
                        return HttpNotFound();
                    }
                    return View(ebattalogs);
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
                    //NDcommon dbobj = new NDcommon();



                    //chks.bmodid = Session["tempid"].ToString();
                    //chks.bmoddate = DateTime.Now;

                    //using (Aitag_DBContext con = new Aitag_DBContext())
                    //{
                    //    con.Entry(chks).State = EntityState.Modified;
                    //    con.SaveChanges();
                    //}

                    //系統LOG檔
                    string sysnote = "";
                    if (sysnote.Length > 4000) { sysnote = sysnote.Substring(0, 4000); }
                    //================================================= //

                    //SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    //string sysrealsid = Request["sysrealsid"].ToString();
                    //string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    //dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    //sysconn.Close();
                    //sysconn.Dispose();
                    //=================================================

                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/resp/empbattalogList' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

                    tmpform += "<input type=hidden id='qblogstatus' name='qblogstatus' value='" + qblogstatus + "'>";
                    tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
                    tmpform += "<input type=hidden id='qdptid' name='qdptid' value='" + qdptid + "'>";
                    tmpform += "<input type=hidden id='qblogsdate' name='qblogsdate' value='" + qblogsdate + "'>";
                    tmpform += "<input type=hidden id='qblogedate' name='qblogedate' value='" + qblogedate + "'>";

                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"" + tmpform };
                }
            }

        }

        public ActionResult empbattalogrpt(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "blogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qblogstatus = "", qempname = "", qdptid = "", qblogsdate = "", qblogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qblogstatus"]))
            {
                qblogstatus = Request["qblogstatus"].Trim();
                ViewBag.qblogstatus = qblogstatus;
            }
            else
            {
                qblogstatus = "1";
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

            NDcommon dbobj = new NDcommon();

            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qblogsdate"], "m", "min", @"出差日期起格式錯誤!!\n", out qblogsdate, out DateEx);
            ViewBag.qblogsdate = qblogsdate;
            dbobj.get_dateRang(Request["qblogedate"], "m", "max", @"出差日期格式錯誤!!\n", out qblogedate, out DateEx1);
            ViewBag.qblogedate = qblogedate;
            DateEx += DateEx1;
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }

            string sqlstr = "SELECT * FROM battalog where (blogtype='1' or (blogtype='2' and (pbsno='' or pbsno is null )) ) and comid='" + (string)Session["comid"] + "'";

            string tmpall = "";
            tmpall = dbobj.get_allempid((string)Session["rid"], (string)Session["empid"]);


            if (tmpall == "%")
            {
                sqlstr += " and comid ='" + (string)Session["comid"] + "'";
            }
            else
            {
                sqlstr += " and empid in (" + tmpall + ") and comid='" + (string)Session["comid"] + "'";
            }

            if (qblogstatus != "")
            {
                sqlstr += "  and blogstatus like '" + qblogstatus + "'";
            }
            if (qempname != "")
            {
                sqlstr += "  and empname like N'%" + qempname + "%'";
            }
            if (qdptid != "")
            {
                sqlstr += " and dptid = '" + qdptid + "'";
            }
            if (qblogsdate != "" && qblogedate != "")
            {
                sqlstr += " and (( blogsdate >= '" + qblogsdate + "' and blogsdate <= '" + qblogedate + "' ) or ";
                sqlstr += "( blogedate >= '" + qblogsdate + "' and blogedate <= '" + qblogedate + "')) ";
            }

            sqlstr += " order by " + orderdata + " " + orderdata1;

            string Excel = "", Excel2 = "";

            #region 組 Excel 格式
            int count = 8;
            Excel += "<HTML>";
            Excel += "<HEAD>";
            Excel += @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">";
            Excel += "</HEAD>";
            Excel += "<body>";
            Excel += "<table  border=1  cellpadding=0 cellspacing=0 bordercolor=#000000 bordercolordark=#ffffff width=900 >";
            Excel += "<tr align=center>";
            Excel += @"<td colspan='" + count + @"' style=""font-size:14pt"">刷卡異常明細表";
            Excel += "</td>";
            Excel += "</tr>";
            Excel += "<tr align=center>";
            Excel += "<td colspan='" + (count - 1) + "' ></td><td>列印日期：" + DateTime.Now.ToString("yyyy/MM/dd") + "</td>";
            Excel += "</tr>";
            Excel += "<tr align=center>";
            Excel += "<td>狀態</td>";
            Excel += "<td>核銷</td>";
            Excel += "<td>員工編號</td>";
            Excel += "<td>姓名</td>";
            Excel += "<td>部門</td>";
            Excel += "<td>出差起迄日</td>";
            Excel += "<td>出差天數</td>";
            Excel += "<td>地點</td>";
            Excel += "</tr>";
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    string dpttitle = "", hlogstatus = "", ifhdell="";

                    string SEtime = "自{0}({1}時)<br>至{2}({3}時)";
                    string blogsdate = "", blogedate = "", blogstime = "", blogetime = "", chkitem = "";

                    using (SqlConnection comconn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        while (dr.Read())
                        {
                            hlogstatus = dbobj.get_dbnull2(dr["blogstatus"]);
                            switch (hlogstatus)
                            {
                                case "0":
                                    hlogstatus = "簽核中";
                                    break;
                                case "1":
                                    hlogstatus = "已核准";
                                    break;
                                case "2":
                                    hlogstatus = "退回";
                                    break;
                                case "3":
                                    hlogstatus = "代理人簽核中";
                                    break;
                                case "D":
                                    hlogstatus = "撤回";
                                    break;
                                default:
                                    break;
                            }
                            if (dbobj.get_dbnull2(dr["ifhdell"])=="y")
                            {
                                ifhdell = "是";
                            }
                            else
                            {
                                ifhdell = "否";
                            }
                            blogsdate = Convert.ToDateTime(dbobj.get_dbnull2(dr["blogsdate"])).ToString("yyyy/MM/dd");
                            blogstime = decimal.Parse("0" + dbobj.get_dbnull2(dr["blogstime"]).ToString()).ToString("####");
                            blogedate = Convert.ToDateTime(dbobj.get_dbnull2(dr["blogedate"])).ToString("yyyy/MM/dd");
                            blogetime = decimal.Parse("0" + dbobj.get_dbnull2(dr["blogetime"]).ToString()).ToString("####");

                            dpttitle = "select dpttitle from Department where dptid='" + dbobj.get_dbnull2(dr["dptid"]) + "' and comid='" + (string)Session["comid"] + "'"; dpttitle = dbobj.get_dbvalue(comconn, dpttitle);
                            chkitem = "select chkitem from checkcode where chkclass = '90' and chkcode = '" + dbobj.get_dbnull2(dr["blogaddr"]) + "'"; chkitem = dbobj.get_dbvalue(comconn, chkitem);

                            Excel2 += "<tr>";
                            Excel2 += "<td>" + hlogstatus + "</td>";
                            Excel2 += "<td>" + ifhdell + "</td>";
                            Excel2 += "<td>" + dbobj.get_dbnull2(dr["empid"]) + "</td>";
                            Excel2 += "<td>" + dbobj.get_dbnull2(dr["empname"]) + "</td>";
                            Excel2 += "<td>" + dpttitle + "</td>";
                            Excel2 += "<td>" + string.Format(SEtime, blogsdate, blogstime, blogedate, blogetime) + "</td>";
                            Excel2 += "<td>" + (decimal.Parse("0" + dbobj.get_dbnull2(dr["bloghour"]))/8).ToString("#") + "</td>";
                            Excel2 += "<td>" + chkitem + "</td>";
                            Excel2 += "</tr>";
                        }
                    }
                    if (Excel2 == "")
                    {
                        Excel += "<tr align=left><td colspan='" + count + @"'>目前沒有資料</td></tr>";
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
            #endregion

            return View();
        }

        public ActionResult empbattalogList(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "blogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qblogstatus = "", qempname = "", qdptid = "", qblogsdate = "", qblogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qblogstatus"]))
            {
                qblogstatus = Request["qblogstatus"].Trim();
                ViewBag.qblogstatus = qblogstatus;
            }
            else
            {
                qblogstatus = "1";
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

            NDcommon dbobj = new NDcommon();

            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qblogsdate"], "m", "min", @"出差日期起格式錯誤!!\n", out qblogsdate, out DateEx);
            ViewBag.qblogsdate = qblogsdate;
            dbobj.get_dateRang(Request["qblogedate"], "m", "max", @"出差日期格式錯誤!!\n", out qblogedate, out DateEx1);
            ViewBag.qblogedate = qblogedate;
            DateEx += DateEx1;
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }


            IPagedList<battalog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {

                string tmpall = "";
                tmpall = dbobj.get_allempid((string)Session["rid"], (string)Session["empid"]);

                string sqlstr = "SELECT * FROM battalog where (blogtype='1' or (blogtype='2' and (pbsno='' or pbsno is null )) ) and comid='" + (string)Session["comid"] + "'";


                if (tmpall == "%")
                {
                    sqlstr += " and comid ='" + (string)Session["comid"] + "'";
                }
                else
                {
                    sqlstr += " and empid in (" + tmpall + ") and comid='" + (string)Session["comid"] + "'";
                }


                if (qblogstatus != "")
                {
                    sqlstr += "  and blogstatus like '" + qblogstatus + "'";
                }
                if (qempname != "")
                {
                    sqlstr += "  and empname like N'%" + qempname + "%'";
                }
                if (qdptid != "")
                {
                    sqlstr += " and dptid = '" + qdptid + "'";
                }
                if (qblogsdate != "" && qblogedate != "")
                {
                    sqlstr += " and (( blogsdate >= '" + qblogsdate + "' and blogsdate <= '" + qblogedate + "' ) or ";
                    sqlstr += "( blogedate >= '" + qblogsdate + "' and blogedate <= '" + qblogedate + "')) ";
                }


                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.battalog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<battalog>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch3(orderdata, orderdata1);
            return View(result);
        }
        private string SetOrder_ch3(string orderdata, string orderdata1)
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

        #endregion

        #region  主管專區 > 員工差勤狀態查詢
        //empcardabnormalqry

        public ActionResult empcardabnormalqryList(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "dptid,empid,clogdate"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qdptid = "", qempname = "", qclogsdate = "", qclogedate = "";
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

            qclogsdate = NullStDate(Request["qclogsdate"]);
            ViewBag.qclogsdate = qclogsdate;
            qclogedate = NullTeDate(Request["qclogedate"]);
            ViewBag.qclogedate = qclogedate;
            //NullStDate 跟 NullTeDate 會判斷格式，有錯誤就 修改全域的DateEx
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }
            NDcommon dbobj = new NDcommon();
            IPagedList<cardjudgelog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from cardjudgelog where comid='" + (string)Session["comid"] + "'";

                string tmpall = "";
                tmpall = dbobj.get_allempid((string)Session["rid"], (string)Session["empid"]);


                if (tmpall == "%")
                {
                    sqlstr += " and comid ='" + (string)Session["comid"] + "'";
                }
                else
                {
                    sqlstr += " and empid in (" + tmpall + ") and comid='" + (string)Session["comid"] + "'";
                }

                if (qdptid != "")
                {
                    sqlstr += " and dptid = '" + qdptid + "'";
                }
                if (qempname != "")
                {
                    sqlstr += " and empname like '%" + qempname + "%'";
                }

                if (qclogsdate != "" )
                {
                    sqlstr += " and clogdate >= '" + qclogsdate + "'";
                }
                if (qclogedate != "")
                {
                    sqlstr += " and clogdate <= '" + qclogedate + "'";
                }

                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.cardjudgelog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<cardjudgelog>(page.Value - 1, (int)Session["pagesize"]);

            }
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

        #endregion

        #region 主管專區 > 員工刷卡資料查詢


        public ActionResult emprealcardlogrpt(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "crid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qdptid = "", qempname = "", qclogsdate = "", qclogedate = "";
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
            NDcommon dbobj = new NDcommon();

            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qclogsdate"], "m", "min", @"出差日期起格式錯誤!!\n", out qclogsdate, out DateEx);
            ViewBag.qclogsdate = qclogsdate;
            dbobj.get_dateRang(Request["qclogedate"], "m", "max", @"出差日期格式錯誤!!\n", out qclogedate, out DateEx1);
            ViewBag.qclogedate = qclogedate;
            DateEx += DateEx1;
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }

            string sqlstr = "SELECT * FROM cardreallog where 1=1";

            string tmpall = "";
            tmpall = dbobj.get_allempid((string)Session["rid"], (string)Session["empid"]);


            if (tmpall == "%")
            {
                sqlstr += " and comid ='" + (string)Session["comid"] + "'";
            }
            else
            {
                sqlstr += " and empid in (" + tmpall + ") and comid='" + (string)Session["comid"] + "'";
            }

            if (qdptid != "")
            {
                sqlstr += " and dptid = '" + qdptid + "'";
            }
            if (qempname != "")
            {
                sqlstr += " and empname like '%" + qempname + "%'";
            }

            if (qclogsdate != "")
            {
                sqlstr += " and clogdate >= '" + qclogsdate + "'";
            }
            if (qclogedate != "")
            {
                sqlstr += " and clogdate <= '" + qclogedate + "'";
            }

            sqlstr += " order by " + orderdata + " " + orderdata1;

            string Excel = "", Excel2 = "";

            #region 組 Excel 格式
            int count = 5;
            Excel += "<HTML>";
            Excel += "<HEAD>";
            Excel += @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">";
            Excel += "</HEAD>";
            Excel += "<body>";
            Excel += "<table  border=1  cellpadding=0 cellspacing=0 bordercolor=#000000 bordercolordark=#ffffff width=900 >";
            Excel += "<tr align=center>";
            Excel += @"<td colspan='" + count + @"' style=""font-size:14pt"">刷卡明細表";
            Excel += "</td>";
            Excel += "</tr>";
            Excel += "<tr align=center>";
            Excel += "<td colspan='" + (count - 1) + "' ></td><td>列印日期：" + DateTime.Now.ToString("yyyy/MM/dd") + "</td>";
            Excel += "</tr>";
            Excel += "<tr align=center>";
            Excel += "<td>部門</td>";
            Excel += "<td>申請人</td>";
            Excel += "<td>刷卡日期</td>";
            Excel += "<td>異動人員</td>";
            Excel += "<td>異動日期</td>";
            Excel += "</tr>";
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    string dpttitle = "", pColgdate = "";


                    using (SqlConnection comconn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        while (dr.Read())
                        {
                            pColgdate = dbobj.get_dbDate(dr["clogdate"], "yyyy/MM/dd");
                           
                            dpttitle = "select dpttitle from Department where dptid='" + dbobj.get_dbnull2(dr["dptid"]) + "' and comid='" + (string)Session["comid"] + "'"; dpttitle = dbobj.get_dbvalue(comconn, dpttitle);

                            Excel2 += "<tr>";
                            Excel2 += "<td>" + dpttitle + "</td>";
                            Excel2 += "<td>" + dbobj.get_dbnull2(dr["empname"]) + "</td>";
                            Excel2 += "<td>" + pColgdate + dbobj.get_dbnull2(dr["clogtime"]) + "</td>";
                            Excel2 += "<td>" + dbobj.get_name(comconn, dbobj.get_dbnull2(dr["bmodid"])) + "</td>";
                            Excel2 += "<td>" + dbobj.get_dbnull2(dr["bmoddate"]) + "</td>";
                            Excel2 += "</tr>";
                        }
                    }
                    if (Excel2 == "")
                    {
                        Excel += "<tr align=left><td colspan='" + count + @"'>目前沒有資料</td></tr>";
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
            #endregion



            return View();
        }

        public ActionResult emprealcardlogList(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "crid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qdptid = "", qempname = "", qclogsdate = "", qclogedate = "";
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
            NDcommon dbobj = new NDcommon();

            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qclogsdate"], "m", "min", @"出差日期起格式錯誤!!\n", out qclogsdate, out DateEx);
            ViewBag.qclogsdate = qclogsdate;
            dbobj.get_dateRang(Request["qclogedate"], "m", "max", @"出差日期格式錯誤!!\n", out qclogedate, out DateEx1);
            ViewBag.qclogedate = qclogedate;
            DateEx += DateEx1;
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }


            IPagedList<cardreallog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "SELECT * FROM cardreallog where 1=1";

                string tmpall = "";
                tmpall = dbobj.get_allempid((string)Session["rid"], (string)Session["empid"]);

                if (tmpall == "%")
                {
                    sqlstr += " and comid ='" + (string)Session["comid"] + "'";
                }
                else
                {
                    sqlstr += " and empid in (" + tmpall + ") and comid='" + (string)Session["comid"] + "'";
                }

                if (qdptid != "")
                {
                    sqlstr += " and dptid = '" + qdptid + "'";
                }
                if (qempname != "")
                {
                    sqlstr += " and empname like '%" + qempname + "%'";
                }

                if (qclogsdate != "")
                {
                    sqlstr += " and clogdate >= '" + qclogsdate + "'";
                }
                if (qclogedate != "")
                {
                    sqlstr += " and clogdate <= '" + qclogedate + "'";
                }

                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.cardreallog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<cardreallog>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch(orderdata, orderdata1);
            return View(result);
        }
        private string SetOrder_ch(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "dptid", "empname", "clogdate" };
            string[] od_ch1 = { "asc", "asc", "asc" };
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

        #endregion

        

        #region 主管專區 > 員工加班查詢

        public ActionResult empotworklogEdit(otworklog chks, string sysflag, int? page, string orderdata, string orderdata1)
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

            string qotstatus = "", qdptid = "", qempname = "", qtmpsdate = "", qtmpedate = "";
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
            NDcommon dbobj = new NDcommon();

            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qtmpsdate"], "m", "min", @"請假起格式錯誤!!\n", out qtmpsdate, out DateEx);
            ViewBag.qtmpsdate = qtmpsdate;
            dbobj.get_dateRang(Request["qtmpedate"], "m", "max", @"請假訖格式錯誤!!\n", out qtmpedate, out DateEx1);
            ViewBag.qtmpedate = qtmpedate;
            DateEx += DateEx1;
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }

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
                    
                    //chks.bmodid = Session["tempid"].ToString();
                    //chks.bmoddate = DateTime.Now;

                    //using (Aitag_DBContext con = new Aitag_DBContext())
                    //{
                    //    con.Entry(chks).State = EntityState.Modified;
                    //    con.SaveChanges();
                    //}


                    //系統LOG檔
                    string sysnote = "";
                    if (sysnote.Length > 4000) { sysnote = sysnote.Substring(0, 4000); }
                    //================================================= //
                    //SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    //string sysrealsid = Request["sysrealsid"].ToString();
                    //string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    ////string sysnote = "代碼:" + chks.hlogid + "名稱:" + chks.empname;
                    //dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    //sysconn.Close();
                    //sysconn.Dispose();
                    //=================================================

                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/resp/empotworklogList' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

                    tmpform += "<input type=hidden id='qotstatus' name='qotstatus' value='" + qotstatus + "'>";
                    tmpform += "<input type=hidden id='qdptid' name='qdptid' value='" + qdptid + "'>";
                    tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
                    tmpform += "<input type=hidden id='qtmpsdate' name='qtmpsdate' value='" + qtmpsdate + "'>";
                    tmpform += "<input type=hidden id='qtmpedate' name='qtmpedate' value='" + qtmpedate + "'>";

                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    //return RedirectToAction("List");
                }
            }

        }

        //empotworklogrpt
        public ActionResult empotworklogList(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "otlogsdate"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qotstatus = "", qdptid = "", qempname = "", qtmpsdate = "", qtmpedate = "";
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

            NDcommon dbobj = new NDcommon();

            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qtmpsdate"], "m", "min", @"請假起格式錯誤!!\n", out qtmpsdate, out DateEx);
            ViewBag.qtmpsdate = qtmpsdate;
            dbobj.get_dateRang(Request["qtmpedate"], "m", "max", @"請假訖格式錯誤!!\n", out qtmpedate, out DateEx1);
            ViewBag.qtmpedate = qtmpedate;
            DateEx += DateEx1;
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }

            string sqlstr = "SELECT * FROM otworklog where comid='" + (string)Session["comid"] + "'";

            string tmpall = "";
            tmpall = dbobj.get_allempid((string)Session["rid"], (string)Session["empid"]);


            if (tmpall == "%")
            {
                sqlstr += " and comid ='" + (string)Session["comid"] + "'";
            }
            else
            {
                sqlstr += " and empid in (" + tmpall + ") and comid='" + (string)Session["comid"] + "'";
            }

            string whereSql = "";

            if (qotstatus != "")
            {
                whereSql += "  and otstatus like '%" + qotstatus + "%'";
            }
            if (qempname != "")
            {
                whereSql += "  and empname like N'%" + qempname + "%'";
            }
            if (qdptid != "")
            {
                whereSql += " and dptid = '" + qdptid + "'";
            }
            if (qtmpsdate != "" && qtmpedate != "")
            {
                whereSql += " and (( otlogsdate >= '" + qtmpsdate + "' and otlogsdate <= '" + qtmpedate + "' ) or ";
                whereSql += "( otlogedate >= '" + qtmpsdate + "' and otlogedate <= '" + qtmpedate + "')) ";
            }
            sqlstr += whereSql;
            sqlstr += " order by " + orderdata + " " + orderdata1;

            //算總時數  (放在第一列)
            ViewBag.Sumhour = Sumhour(whereSql);


            IPagedList<otworklog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                var query = con.otworklog.SqlQuery(sqlstr).AsQueryable();
                result = query.ToPagedList<otworklog>(page.Value - 1, (int)Session["pagesize"]);
            }
            ViewBag.SetOrder_ch = SetOrder_ch5(orderdata, orderdata1);
            return View(result);
        }

        private List<string> Sumhour(string whereSql)
        {
            NDcommon dbobj = new NDcommon();
            List<string> SumhourList = new List<string>();
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                string sql = "";
                sql += "select (select isnull(sum(otloghour),0) FROM otworklog ";
                sql += " where 1=1 " + whereSql;
                sql += ") as otloghour,";
                sql += " isnull(sum(resthour),0) as resthour, isnull(sum(moneyhour),0) as moneyhour";
                sql += " from resthourlog where comid = '11330555' and osno in (SELECT osno FROM otworklog ";
                sql += " where 1=1 " + whereSql;
                sql += ")";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        SumhourList.Add(dr["otloghour"].ToString());
                        SumhourList.Add(dr["resthour"].ToString());
                        SumhourList.Add(dr["moneyhour"].ToString());
                    }
                    dr.Close();
                }
            }  


            return SumhourList;
        }
        private string SetOrder_ch5(string orderdata, string orderdata1)
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

        #endregion




        #region 主管專區 > 員工刷卡異常查詢

        public ActionResult empcardlogEdit(cardlog chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "funorder"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qdptid = "", qempname = "", qhclogsdate = "", qhclogedate = "";
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
            if (!string.IsNullOrWhiteSpace(Request["qhclogsdate"]))
            {
                qhclogsdate = Request["qhclogsdate"].Trim();
                ViewBag.qhclogsdate = qhclogsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qhclogedate"]))
            {
                qhclogedate = Request["qhclogedate"].Trim();
                ViewBag.qhclogedate = qhclogedate;
            }

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.cardlog.Where(r => r.clogid == chks.clogid).FirstOrDefault();
                    cardlog ecardlogs = con.cardlog.Find(chks.clogid);
                    if (ecardlogs == null)
                    {
                        return HttpNotFound();
                    }
                    return View(ecardlogs);
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

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {

                        //NDcommon dbobj = new NDcommon();
                        //chks.empid = "99999999";
                        //chks.bmodid = Session["tempid"].ToString();
                        //chks.bmoddate = DateTime.Now;
                        //con.Entry(chks).State = EntityState.Modified;
                        //con.SaveChanges();


                        //系統LOG檔
                        //================================================= //                     
                        //SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        //string sysrealsid = Request["sysrealsid"].ToString();
                        //string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        ////string dbdata = dbobj.get_dbvalue(sysconn, "select chkitem from checkcode where chkclass='08' and chkcode='" + chks.funid + "'");
                        //string sysnote = "共用首頁設定:的資料";
                        //dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        //sysconn.Close();
                        //sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/resp/empcardlogList' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                        tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                        tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

                        tmpform += "<input type=hidden id='qdptid' name='qdptid' value='" + qdptid + "'>";
                        tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
                        tmpform += "<input type=hidden id='qhclogsdate' name='qhclogsdate' value='" + qhclogsdate + "'>";
                        tmpform += "<input type=hidden id='qhclogedate' name='qhclogedate' value='" + qhclogedate + "'>";

                        tmpform += "</form>";
                        tmpform += "</body>";


                        return new ContentResult() { Content = @"" + tmpform };
                        //return RedirectToAction("List");
                    }
                }
            }

        }

        public ActionResult empcardlogrpt(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "clogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qdptid = null, qempname = null, qhclogsdate = null, qhclogedate = null;
            if (!string.IsNullOrEmpty(Request["qdptid"]))
            {
                qdptid = Request["qdptid"];
                ViewBag.qdptid = qdptid;
            }
            if (!string.IsNullOrEmpty(Request["qempname"]))
            {
                qempname = Request["qempname"];
                ViewBag.qempname = qempname;
            }

            NDcommon dbobj = new NDcommon();

            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qhclogsdate"], "m", "min", @"請假起格式錯誤!!\n", out qhclogsdate, out DateEx);
            ViewBag.qhclogsdate = qhclogsdate;
            dbobj.get_dateRang(Request["qhclogedate"], "m", "max", @"請假訖格式錯誤!!\n", out qhclogedate, out DateEx1);
            ViewBag.qhclogedate = qhclogedate;
            DateEx += DateEx1;
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }


            string sqlstr = "";

            sqlstr = "select * from cardlog  where  1 = 1";

            #region 查詢條件
            if (!string.IsNullOrEmpty(qdptid))
            {
                sqlstr += " and dptid = '" + qdptid + "'";
            }
            if (!string.IsNullOrEmpty(qempname))
            {
                sqlstr += " and empname = '" + qempname + "'";
            }
            if (!string.IsNullOrEmpty(qhclogsdate))
            {
                sqlstr += " and clogdate >= '" + qhclogsdate + "'";
            }
            if (!string.IsNullOrEmpty(qhclogedate))
            {
                sqlstr += " and clogdate <='" + qhclogedate + "'";
            }
            #endregion
            sqlstr += " order by " + orderdata + " " + orderdata1;

            string Excel = "", Excel2 = "";

            #region 組 Excel 格式
            int count = 5;
            Excel += "<HTML>";
            Excel += "<HEAD>";
            Excel += @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">";
            Excel += "</HEAD>";
            Excel += "<body>";
            Excel += "<table  border=1  cellpadding=0 cellspacing=0 bordercolor=#000000 bordercolordark=#ffffff width=900 >";
            Excel += "<tr align=center>";
            Excel += @"<td colspan='" + count + @"' style=""font-size:14pt"">刷卡異常明細表";
            Excel += "</td>";
            Excel += "</tr>";
            Excel += "<tr align=center>";
            Excel += "<td colspan='" + (count - 1) + "' ></td><td>列印日期：" + DateTime.Now.ToString("yyyy/MM/dd") + "</td>";
            Excel += "</tr>";
            Excel += "<tr align=center>";
            Excel += "<td>部門</td>";
            Excel += "<td>申請人</td>";
            Excel += "<td>刷卡日期</td>";
            Excel += "<td>異動人員</td>";
            Excel += "<td>異動日期</td>";
            Excel += "</tr>";
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    string dpttitle = "";
                    
                    using (SqlConnection comconn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        while (dr.Read())
                        {


                            dpttitle = "select dpttitle from Department where dptid='" + dbobj.get_dbnull2(dr["dptid"]) + "' and comid='" + (string)Session["comid"] + "'"; dpttitle = dbobj.get_dbvalue(comconn, dpttitle);


                            Excel2 += "<tr>";
                            Excel2 += "<td>" + dpttitle + "</td>";
                            Excel2 += "<td>" + dbobj.get_dbnull2(dr["empname"]) + "</td>";

                            Excel2 += "<td>" + dbobj.get_dbDate(dr["clogdate"], "yyyy/MM/dd") + dr["clogtime"] + "</td>";

                            Excel2 += "<td>" + dbobj.get_dbnull2(dr["bmodid"]) + "</td>";
                            Excel2 += "<td>" + dbobj.get_dbnull2(dr["bmoddate"]) + "</td>";



                            Excel2 += "</tr>";
                        }
                    }
                    if (Excel2 == "")
                    {
                        Excel += "<tr align=left><td colspan='" + count + @"'>目前沒有資料</td></tr>";
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
            #endregion

            return View();
        }

        public ActionResult empcardlogList(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "clogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qdptid = null, qempname = null, qhclogsdate = null, qhclogedate = null;
            if (!string.IsNullOrEmpty(Request["qdptid"]))
            {
                qdptid = Request["qdptid"];
                ViewBag.qdptid = qdptid;
            }
            if (!string.IsNullOrEmpty(Request["qempname"]))
            {
                qempname = Request["qempname"];
                ViewBag.qempname = qempname;
            }

            NDcommon dbobj = new NDcommon();

            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qhclogsdate"], "m", "min", @"請假起格式錯誤!!\n", out qhclogsdate, out DateEx);
            ViewBag.qhclogsdate = qhclogsdate;
            dbobj.get_dateRang(Request["qhclogedate"], "m", "max", @"請假訖格式錯誤!!\n", out qhclogedate, out DateEx1);
            ViewBag.qhclogedate = qhclogedate;
            DateEx += DateEx1;
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }


            IPagedList<cardlog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string tmpall = "";
                tmpall = dbobj.get_allempid((string)Session["rid"], (string)Session["empid"]);

                string sqlstr = "select * from cardlog  where  1 = 1";

                System.Collections.Generic.IEnumerable<cardlog> query;

                #region 查詢條件

                if (tmpall == "%")
                {
                    sqlstr += " and comid ='" + (string)Session["comid"] + "'";
                }
                else
                {
                    sqlstr += " and empid in (" + tmpall + ") and comid='" + (string)Session["comid"] + "'";
                }

                if (!string.IsNullOrEmpty(qdptid))
                {
                    sqlstr += " and dptid = '" + qdptid + "'";
                }
                if (!string.IsNullOrEmpty(qempname))
                {
                    sqlstr += " and empname = '" + qempname + "'";
                }
                if (!string.IsNullOrEmpty(qhclogsdate))
                {
                    sqlstr += " and clogdate >= '" + qhclogsdate + "'";
                }
                if (!string.IsNullOrEmpty(qhclogedate))
                {
                    sqlstr += " and clogdate <='" + qhclogedate + "'";
                }
                #endregion
                sqlstr += " order by " + orderdata + " " + orderdata1;
                query = con.cardlog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<cardlog>(page.Value - 1, (int)Session["pagesize"]);

            }

            ViewBag.SetOrder_ch = SetOrder_ch4(orderdata, orderdata1);
            return View(result);
        }
        private string SetOrder_ch4(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "dptid", "empname", "clogdate" };
            string[] od_ch1 = { "asc", "asc", "asc" };
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
        #endregion

    }

    //假別種類
    public class getCol
    {
        public string hdayid = "";
        public string hdaytitle = "";
        public string sum = "0";
    }
}
