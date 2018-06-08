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
    public class otapplylogController : BaseController
    {
        string DateEx = "";

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /otapply/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        #region Add
        public ActionResult add(otapply col, string sysflag, int? page, string orderdata, string orderdata1)
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

            string qotstatus = "", qtmpsdate = "", qtmpedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qotstatus"]))
            {
                qotstatus = Request["qotstatus"].Trim();
                ViewBag.qotstatus = qotstatus;
            }
            if (!string.IsNullOrWhiteSpace(Request["qtmpsdate"]))
            {
                qtmpsdate = Request["qtmpsdate"].Trim();
                ViewBag.qtmpsdate = qtmpsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qtmpedate"]))
            {
                qtmpedate = Request["qtmpedate"].Trim();
                ViewBag.qtmpedate = qtmpedate;
            }

            //組假別
            //furloughtype();

            if (sysflag != "A")
            {


                otapply newcol = new otapply();
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
                    NDcommon dbobj = new NDcommon();
                    string roleall = "", tmpbsno = "";

                    //找出下一個呈核角色
                    #region 找出下一個呈核角色
                    string tmparolestampid = "";
                    string tmprole = "";
                    string tmpbillid = "";
                    if (Request["arolestampid"].ToString() != "")
                    {
                        tmparolestampid = "'" + Request["arolestampid"].ToString() + "'";
                    }

                    string tmphour = "0", tmpaddr = "";
                    tmphour = col.otloghour.ToString();
                    string impallstring = dbobj.getnewcheck1("D", tmparolestampid, tmparolestampid, tmphour, "", "");
                    tmprole = impallstring.Split(';')[0].ToString();
                    tmpbillid = impallstring.Split(';')[1].ToString();
                    if (tmprole == "")
                    {
                        ViewBag.ErrMsg = @"<script>alert(""請先至表單流程設定中設定加班呈核流程!"");</script>";
                        return View(col);
                    }
                    roleall = tmparolestampid;
                    #endregion

                    using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        //找是否有加過班了找時間
                        #region 找是否有加過班了找時間
                        string ostime = dbobj.get_dbnull2(Request["hour1"]) + dbobj.get_dbnull2(Request["min1"]);
                        string oetime = dbobj.get_dbnull2(Request["hour2"]) + dbobj.get_dbnull2(Request["min2"]);

                        string tmpsdate = Convert.ToDateTime(col.otlogsdate).ToString("yyyyMMdd");
                        string tmpedate = Convert.ToDateTime(col.otlogedate).ToString("yyyyMMdd");
                        string alltime = tmpsdate + ostime;
                        string alltime1 = tmpedate + oetime;
                        string startdate = " (RTRIM(REPLACE(CONVERT(char, otlogsdate, 111), '/', '')) + otlogstime)";
                        string enddate = " (RTRIM(REPLACE(CONVERT(char, otlogedate, 111), '/', '')) + otlogetime)";

                        string sql = "select otlogid from otapply where otstatus in ('0','1') and empid = '" + col.empid + "' and ";
                        sql += " ((" + startdate + " >= '" + alltime + "' and " + enddate + " <='" + alltime1 + "') or ";
                        sql += " (" + startdate + " <= '" + alltime + "' and  " + enddate + " >='" + alltime + "') or ";
                        sql += " (" + startdate + " <= '" + alltime1 + "' and  " + enddate + " >='" + alltime1 + "')) and comid='" + (string)Session["comid"] + "'";

                        if("" != dbobj.get_dbvalue(conn, sql)){
                            ViewBag.ErrMsg = @"<script>alert(""此日期區間已經有請示過加班!"");</script>";
                            ViewBag.otcomment = dbobj.get_dbnull2(Request["otcomment"]);
                            return View(col);
                        }
                        #endregion

                        //單據編號(自動產生編號)  申請單號
                        #region 單據編號(自動產生編號)
                        string sql_no = "select osno from otapply where year(odate) = " + DateTime.Now.Year
                            + " and month(odate) = " + DateTime.Now.Month
                            + " and comid='" + (string)Session["comid"] + "' order by osno desc";
                        string hsno = dbobj.get_dbvalue(conn, sql_no);
                        if (!string.IsNullOrEmpty(hsno))
                        {
                            hsno = hsno.Substring(hsno.Length - 3, 3);
                            hsno = (Int16.Parse(hsno) + 1).ToString();
                        }
                        else
                        {
                            hsno = "1";
                        }
                        string tmpyear = (DateTime.Now.Year - 1911).ToString();
                        string tmpmonth = DateTime.Now.ToString("MM");
                        string tmpno1 = "000" + hsno;
                        tmpno1 = tmpno1.Substring(tmpno1.Length - 3, 3);

                        tmpbsno = "D" + tmpyear + tmpmonth + tmpno1;
                        //====

                        #endregion

                        
                    }

                    #region 整理欄位 準備新增

                    //'呈核人員
                    //'======================
                    col.rolestampid = tmprole; /*下個呈核角色*/
                    col.rolestampidall = roleall; /*所有呈核角色*/
                    col.empstampidall = "'" + (string)Session["empid"] + "'"; 
                    col.billflowid = int.Parse(tmpbillid); /*下個呈核流程id*/
                    //'======================



                    if (!string.IsNullOrWhiteSpace(Request["arolestampid"]))
                    {
                        col.arolestampid = Request["arolestampid"];
                    }
                    else
                    {
                        col.arolestampid = Request["arolestampid1"];
                    }

                    col.osno = tmpbsno;/*申請單號*/

                    col.otlogstime = dbobj.get_dbnull2(Request["hour1"]) + dbobj.get_dbnull2(Request["min1"]);
                    col.otlogetime = dbobj.get_dbnull2(Request["hour2"]) + dbobj.get_dbnull2(Request["min2"]);

                    if (!string.IsNullOrWhiteSpace(Request["otloghour"]))
                    {
                        col.otloghour = float.Parse(Request["otloghour"]);
                        /*加班(otapply)的  [otloghour] [real] 
                         跟加班的  [hloghour] [numeric](10, 3)
                         資料庫型態不一樣??*/
                    }
                    else
                    {
                        col.otloghour = 0;
                    }

                    col.otresthour = 0;
                    col.otmoneyhour = 0;
                    if (dbobj.get_dbnull2(Request["otherman"]) != "")
                    {
                        col.empmeetsign = dbobj.get_dbnull2(Request["otherman"]); /*知會人員*/
                        col.empmeetsign = col.empmeetsign.Substring(1, col.empmeetsign.Length - 1);
                    }
                    


                    col.otstatus = "0"; // 己簽核:1  :0
                    col.otype = "1";
                    col.ifotdell = "n";
                    col.comid = (string)Session["comid"];
                    col.bmodid = (string)Session["empid"];
                    col.bmoddate = DateTime.Now;
                    col.billtime = DateTime.Now.ToString("yyyy/MM/dd");/*??*/
                    #endregion

                    using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        //寄信
                        SetMail(conn, dbobj, col);
                    }
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.otapply.Add(col);
                        try
                        {
                            con.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }

                    //系統LOG檔
                    string tmpevent = "申請人：" + col.empname + "<br>申請單號：" + col.osno + "的資料<br>";


                    string sysnote = tmpevent;
                    if (sysnote.Length > 4000) { sysnote = sysnote.Substring(0, 4000); }
                    //================================================= //
                    SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    string sysrealsid = Request["sysrealsid"].ToString();
                    string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    sysconn.Close();
                    sysconn.Dispose();
                    //=================================================


                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/otapplylog/main' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                }
            }


        }

        private void SetMail(SqlConnection conn, NDcommon dbobj, otapply col)
        {
            
            string fromaddname = "select empname from employee where empid='" + col.empid + "'"; fromaddname = dbobj.get_dbvalue(conn, fromaddname);

            List<string> MailContextList = new List<string>();

            MailContextList.Add("<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body>");
            MailContextList.Add("以下為明細資料：<BR>");
            MailContextList.Add("<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>");
            MailContextList.Add("<tr><td align=right width=130>申請單號：</td><td>" + col.osno + "</td></tr>");
            MailContextList.Add("<tr><td align=right width=130>申請人：</td><td>" + col.empname + "</td></tr>");
            MailContextList.Add("<tr><td align=right>起迄時間：</td><td>自 " + dbobj.get_dbDate(col.otlogsdate, "yyyy/MM/dd") + "(" + dbobj.get_dbnull2(Request["hour1"]) + dbobj.get_dbnull2(Request["min1"]) + ")<BR>至 " + dbobj.get_dbDate(col.otlogedate, "yyyy/MM/dd") + "(" + dbobj.get_dbnull2(Request["hour2"]) + dbobj.get_dbnull2(Request["min2"]) + ")</td></tr>");
            MailContextList.Add("<tr><td align=right>共計小時：</td><td>" + col.otloghour + "時</td></tr>");
            if (!string.IsNullOrWhiteSpace(col.otcomment))
            {
                MailContextList.Add("<tr><td align=right>事由：</td><td>" + col.otcomment.ToString().Trim().Replace(Environment.NewLine, "<br>") + "</td></tr>");
            }
            else
            {
                MailContextList.Add("<tr><td align=right>事由：</td><td>&#160;</td></tr>");
            }
            MailContextList.Add("</table></body></HTML>");


            string mailtitle = "", MailContext = "";
            //寄信給下一個審核角色
            #region 寄信給下一個審核角色
            string tmproleid = col.rolestampid.Replace("'", "");
            if (tmproleid != "")
            {
                mailtitle = "【" + fromaddname + "】加班申請單資料要求審核通知";
                foreach (string v in MailContextList)
                {
                    MailContext += v;
                }

                string sql_m = "select enemail from viewemprole where rid = '" + tmproleid + "' and empstatus <> '4' and enemail<>''";
                using (SqlConnection comconn = dbobj.get_conn("Aitag_DBContext"))
                {
                    using (SqlCommand cmd = new SqlCommand(sql_m, comconn))
                    {
                        SqlDataReader dr = cmd.ExecuteReader();
                        string tomail = "";
                        while (dr.Read())
                        {
                            tomail += dr["enemail"] + ",";
                        }
                        dbobj.send_mailfile("", tomail, mailtitle, MailContext, null, null);
                        dr.Close();
                    }
                }
            }
            #endregion

            mailtitle = ""; MailContext = "";
            //寄信給支會人員
            #region 寄信給支會人員
            if (!string.IsNullOrWhiteSpace(col.empmeetsign))
            {
                mailtitle = "【" + fromaddname + "】加班申請單知會通知信";
                for (int i = 0; i <= 2; i++)
                {
                    MailContext += MailContextList[i];
                }
                for (int i = 4; i <= 8; i++)
                {
                    MailContext += MailContextList[i];
                }


                //'找mail
                using (SqlConnection comconn = dbobj.get_conn("Aitag_DBContext"))
                {
                    string sql = "select enemail from employee where empid in (" + col.empmeetsign + ") and enemail <> '' and enemail is not null";
                    using (SqlCommand cmd = new SqlCommand(sql, comconn))
                    {
                        SqlDataReader dr = cmd.ExecuteReader();
                        string tomail = "";
                        while (dr.Read())
                        {
                            tomail += dr["enemail"] + ",";
                        }
                        dbobj.send_mailfile("", tomail, mailtitle, MailContext, null, null);
                        dr.Close();
                    }
                }
            }
            #endregion



        }

        #endregion

        #region 加班作業 > 加班申請單撤回與確認
        public ActionResult otapplymod1(otapply chks, string sysflag, int? page, string orderdata, string orderdata1)
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

            string qotstatus = "", qtmpsdate = "", qtmpedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qotstatus"]))
            {
                qotstatus = Request["qotstatus"].Trim();
                ViewBag.qotstatus = qotstatus;
            }
            if (!string.IsNullOrWhiteSpace(Request["qtmpsdate"]))
            {
                qtmpsdate = Request["qtmpsdate"].Trim();
                ViewBag.qtmpsdate = qtmpsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qtmpedate"]))
            {
                qtmpedate = Request["qtmpedate"].Trim();
                ViewBag.qtmpedate = qtmpedate;
            }

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    otapply eotapplylogs = con.otapply.Find(chks.otlogid);
                    if (eotapplylogs == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eotapplylogs);
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
                    string roleall = "", tmpbsno = "";
                    otapply col = new otapply();
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        col = con.otapply.Find(chks.otlogid);
                    }

                    



                    //找出下一個呈核角色
                    #region 找出下一個呈核角色
                    string tmparolestampid = "";
                    string tmprole = "";
                    string tmpbillid = "";
                    if (Request["arolestampid"].ToString() != "")
                    {
                        tmparolestampid = "'" + Request["arolestampid"].ToString() + "'";
                    }

                    string tmphour = "0", tmpaddr = "";
                    tmphour = col.otloghour.ToString();
                    string impallstring = dbobj.getnewcheck1("D", tmparolestampid, tmparolestampid, tmphour, "", "");
                    tmprole = impallstring.Split(';')[0].ToString();
                    tmpbillid = impallstring.Split(';')[1].ToString();
                    if (tmprole == "")
                    {
                        ViewBag.ErrMsg = @"<script>alert(""請先至表單流程設定中設定加班呈核流程!"");</script>";
                        return View(col);
                    }
                    roleall = tmparolestampid;
                    #endregion


                    using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        //找是否有加過班了找時間
                        #region 找是否有加過班了找時間
                        string ostime = dbobj.get_dbnull2(Request["hour1"]) + dbobj.get_dbnull2(Request["min1"]);
                        string oetime = dbobj.get_dbnull2(Request["hour2"]) + dbobj.get_dbnull2(Request["min2"]);

                        string tmpsdate = Convert.ToDateTime(col.otlogsdate).ToString("yyyyMMdd");
                        string tmpedate = Convert.ToDateTime(col.otlogedate).ToString("yyyyMMdd");
                        string alltime = tmpsdate + ostime;
                        string alltime1 = tmpedate + oetime;
                        string startdate = " (RTRIM(REPLACE(CONVERT(char, otlogsdate, 111), '/', '')) + otlogstime)";
                        string enddate = " (RTRIM(REPLACE(CONVERT(char, otlogedate, 111), '/', '')) + otlogetime)";

                        string sql = "select otlogid from otapply where otstatus in ('0','1') and empid = '" + col.empid + "' and ";
                        sql += " ((" + startdate + " >= '" + alltime + "' and " + enddate + " <='" + alltime1 + "') or ";
                        sql += " (" + startdate + " <= '" + alltime + "' and  " + enddate + " >='" + alltime + "') or ";
                        sql += " (" + startdate + " <= '" + alltime1 + "' and  " + enddate + " >='" + alltime1 + "')) and comid='" + (string)Session["comid"] + "'";

                        if ("" != dbobj.get_dbvalue(conn, sql))
                        {
                            ViewBag.ErrMsg = @"<script>alert(""此日期區間已經有請示過加班!"");</script>";
                            ViewBag.otcomment = dbobj.get_dbnull2(Request["otcomment"]);
                            return View(col);
                        }
                        #endregion

                        //單據編號(自動產生編號)  申請單號
                        #region 單據編號(自動產生編號)
                        string sql_no = "select osno from otworklog where year(odate) = " + DateTime.Now.Year
                            + " and month(odate) = " + DateTime.Now.Month
                            + " and comid='" + (string)Session["comid"] + "' order by osno desc";
                        string hsno = dbobj.get_dbvalue(conn, sql_no);
                        if (!string.IsNullOrEmpty(hsno))
                        {
                            hsno = hsno.Substring(hsno.Length - 3, 3);
                            hsno = (Int16.Parse(hsno) + 1).ToString();
                        }
                        else
                        {
                            hsno = "1";
                        }
                        string tmpyear = (DateTime.Now.Year - 1911).ToString();
                        string tmpmonth = DateTime.Now.ToString("MM");
                        string tmpno1 = "000" + hsno;
                        tmpno1 = tmpno1.Substring(tmpno1.Length - 3, 3);

                        tmpbsno = "C" + tmpyear + tmpmonth + tmpno1;
                        //====

                        #endregion

                    }

                    #region 整理欄位 準備新增
                    otworklog col2 = new otworklog();
                    //'呈核人員
                    //'======================
                    col2.rolestampid = tmprole; /*下個呈核角色*/
                    col2.rolestampidall = roleall; /*所有呈核角色*/
                    col2.empstampidall = "'" + (string)Session["empid"] + "'";
                    col2.billflowid = int.Parse(tmpbillid); /*下個呈核流程id*/
                    //'======================

                    col2.osno = tmpbsno;/*申請單號*/

                    if (!string.IsNullOrWhiteSpace(Request["arolestampid"]))
                    {
                        col2.arolestampid = Request["arolestampid"];
                    }
                    else
                    {
                        col2.arolestampid = Request["arolestampid1"];
                    }

                    col2.otlogstime = col.otlogstime;
                    col2.otlogetime = col.otlogetime;
                    col2.otloghour = col.otloghour;


                    col2.empid = col.empid;
                    col2.empname = col.empname;
                    col2.dptid = col.dptid;
                    col2.otid = col.otid;
                    col2.otlogsdate = col.otlogsdate;
                    col2.otlogedate = col.otlogedate;
                    col2.otcomment = col.otcomment;
                    col2.odate = col.odate;
                    col2.ottype = col.ottype;
                    col2.empmeetsign = "";
                    col2.inout = "";


                    if (dbobj.get_dbnull2(Request["otresthour"]) != "")
                    {
                        col2.otresthour = float.Parse(Request["otresthour"]);
                    }
                    if (dbobj.get_dbnull2(Request["otmoneyhour"]) != "")
                    {
                        col2.otmoneyhour = float.Parse(Request["otmoneyhour"]);
                    }

                    if (dbobj.get_dbnull2(Request["otherman"]) != "")
                    {
                        col2.empmeetsign = dbobj.get_dbnull2(Request["otherman"]); /*知會人員*/
                        col2.empmeetsign = col2.empmeetsign.Substring(1, col2.empmeetsign.Length - 1);
                    }



                    col2.otstatus = "0"; // 己簽核:1  :0
                    col2.otype = "1";
                    col2.ifotdell = "n";
                    col2.comid = (string)Session["comid"];
                    col2.bmodid = (string)Session["empid"];
                    col2.bmoddate = DateTime.Now;
                    col2.billtime = DateTime.Now.ToString("yyyy/MM/dd");/*??*/
                    #endregion


                    using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        //寄信
                        SetMail2(conn, dbobj, col2);
                    }
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.otworklog.Add(col2);
                        try
                        {
                            con.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }


                    ////系統LOG檔
                    string tmpevent = "申請人：" + col.empname + "<br>申請單號：" + col.osno + "的資料<br>";
                    string sysnote = tmpevent;
                    if (sysnote.Length > 4000) { sysnote = sysnote.Substring(0, 4000); }
                    //================================================= //
                    SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    string sysrealsid = Request["sysrealsid"].ToString();
                    string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    sysconn.Close();
                    sysconn.Dispose();
                    ////=================================================

                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/otapplylog/main' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

                    tmpform += "<input type=hidden id='qotstatus' name='qotstatus' value='" + qotstatus + "'>";
                    tmpform += "<input type=hidden id='qtmpsdate' name='qtmpsdate' value='" + qtmpsdate + "'>";
                    tmpform += "<input type=hidden id='qtmpedate' name='qtmpedate' value='" + qtmpedate + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"" + tmpform };
                }
            }

        }
        private void SetMail2(SqlConnection conn, NDcommon dbobj, otworklog col)
        {

            string fromaddname = "select empname from employee where empid='" + col.empid + "'"; fromaddname = dbobj.get_dbvalue(conn, fromaddname);

            List<string> MailContextList = new List<string>();

            MailContextList.Add("<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body>");
            MailContextList.Add("以下為明細資料：<BR>");
            MailContextList.Add("<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>");
            MailContextList.Add("<tr><td align=right width=130>申請單號：</td><td>" + col.osno + "</td></tr>");
            MailContextList.Add("<tr><td align=right width=130>申請人：</td><td>" + col.empname + "</td></tr>");
            MailContextList.Add("<tr><td align=right>起迄時間：</td><td>自 " + dbobj.get_dbDate(col.otlogsdate, "yyyy/MM/dd") + "(" + dbobj.get_dbnull2(Request["hour1"]) + dbobj.get_dbnull2(Request["min1"]) + ")<BR>至 " + dbobj.get_dbDate(col.otlogedate, "yyyy/MM/dd") + "(" + dbobj.get_dbnull2(Request["hour2"]) + dbobj.get_dbnull2(Request["min2"]) + ")</td></tr>");
            MailContextList.Add("<tr><td align=right>共計小時：</td><td>" + col.otloghour + "時</td></tr>");
            if (!string.IsNullOrWhiteSpace(col.otcomment))
            {
                MailContextList.Add("<tr><td align=right>事由：</td><td>" + col.otcomment.ToString().Trim().Replace(Environment.NewLine, "<br>") + "</td></tr>");
            }
            else
            {
                MailContextList.Add("<tr><td align=right>事由：</td><td>&#160;</td></tr>");
            }
            MailContextList.Add("</table></body></HTML>");


            string mailtitle = "", MailContext = "";
            //寄信給下一個審核角色
            #region 寄信給下一個審核角色
            string tmproleid = col.rolestampid.Replace("'", "");
            if (tmproleid != "")
            {
                mailtitle = "【" + fromaddname + "】加班單資料要求審核通知";
                foreach (string v in MailContextList)
                {
                    MailContext += v;
                }

                string sql_m = "select enemail from viewemprole where rid = '" + tmproleid + "' and empstatus <> '4' and enemail<>''";
                using (SqlConnection comconn = dbobj.get_conn("Aitag_DBContext"))
                {
                    using (SqlCommand cmd = new SqlCommand(sql_m, comconn))
                    {
                        SqlDataReader dr = cmd.ExecuteReader();
                        string tomail = "";
                        while (dr.Read())
                        {
                            tomail += dr["enemail"] + ",";
                        }
                        dbobj.send_mailfile("", tomail, mailtitle, MailContext, null, null);
                        dr.Close();
                    }
                }
            }
            #endregion

            mailtitle = ""; MailContext = "";
            //寄信給支會人員
            #region 寄信給支會人員
            if (!string.IsNullOrWhiteSpace(col.empmeetsign))
            {
                mailtitle = "【" + fromaddname + "】加班單知會通知信";
                for (int i = 0; i <= 2; i++)
                {
                    MailContext += MailContextList[i];
                }
                for (int i = 4; i <= 8; i++)
                {
                    MailContext += MailContextList[i];
                }


                //'找mail
                using (SqlConnection comconn = dbobj.get_conn("Aitag_DBContext"))
                {
                    string sql = "select enemail from employee where empid in (" + col.empmeetsign + ") and enemail <> '' and enemail is not null";
                    using (SqlCommand cmd = new SqlCommand(sql, comconn))
                    {
                        SqlDataReader dr = cmd.ExecuteReader();
                        string tomail = "";
                        while (dr.Read())
                        {
                            tomail += dr["enemail"] + ",";
                        }
                        dbobj.send_mailfile("", tomail, mailtitle, MailContext, null, null);
                        dr.Close();
                    }
                }
            }
            #endregion



        }
        public ActionResult Edit(otapply chks, string sysflag, int? page, string orderdata, string orderdata1)
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

            string qotstatus = "", qtmpsdate = "", qtmpedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qotstatus"]))
            {
                qotstatus = Request["qotstatus"].Trim();
                ViewBag.qotstatus = qotstatus;
            }
            if (!string.IsNullOrWhiteSpace(Request["qtmpsdate"]))
            {
                qtmpsdate = Request["qtmpsdate"].Trim();
                ViewBag.qtmpsdate = qtmpsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qtmpedate"]))
            {
                qtmpedate = Request["qtmpedate"].Trim();
                ViewBag.qtmpedate = qtmpedate;
            }

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    otapply eotapplylogs = con.otapply.Find(chks.otlogid);
                    if (eotapplylogs == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eotapplylogs);
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


                    ////系統LOG檔
                    //string sysnote = "";
                    //if (sysnote.Length > 4000) { sysnote = sysnote.Substring(0, 4000); }
                    ////================================================= //
                    //SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    //string sysrealsid = Request["sysrealsid"].ToString();
                    //string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    //dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    //sysconn.Close();
                    //sysconn.Dispose();
                    ////=================================================

                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/otapplylog/main' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

                    tmpform += "<input type=hidden id='qotstatus' name='qotstatus' value='" + qotstatus + "'>";
                    tmpform += "<input type=hidden id='qtmpsdate' name='qtmpsdate' value='" + qtmpsdate + "'>";
                    tmpform += "<input type=hidden id='qtmpedate' name='qtmpedate' value='" + qtmpedate + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"" + tmpform };
                }
            }

        }
        public ActionResult main(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "otlogsdate"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qotstatus = "", qtmpsdate = "", qtmpedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qotstatus"]))
            {
                qotstatus = Request["qotstatus"].Trim();
                ViewBag.qotstatus = qotstatus;
            }
            
            NDcommon dbobj = new NDcommon();
            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qtmpsdate"], "m", "min", @"加班日期起格式錯誤!!\n", out qtmpsdate, out DateEx);
            ViewBag.qtmpsdate = qtmpsdate;
            dbobj.get_dateRang(Request["qtmpedate"], "m", "max", @"加班日期訖格式錯誤!!\n", out qtmpedate, out DateEx1);
            ViewBag.qtmpedate = qtmpedate;
            DateEx += DateEx1;


            IPagedList<otapply> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from otapply  where empid='" + (string)Session["empid"] + "' and otstatus in ('0','1') and comid='" + (string)Session["comid"] + "'   and";
                if (qotstatus != "")
                {
                    sqlstr += " otstatus='" + qotstatus + "'  and";
                }


                sqlstr += " ((otlogsdate >= '" + qtmpsdate + "' and otlogsdate <= '" + qtmpedate + "')   or";


                sqlstr += " (otlogedate >= '" + qtmpsdate + "' and otlogedate <= '" + qtmpedate + "'))   and";
                


                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.otapply.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<otapply>(page.Value - 1, (int)Session["pagesize"]);

            }

            string[] od_ch = { "ottype", "otstatus", "otlogsdate" };
            string[] od_ch1 = { "asc", "asc", "asc" };
            ViewBag.SetOrder_ch = dbobj.SetOrder(orderdata, orderdata1, od_ch, od_ch1);
            return View(result);
        }
        

        [ActionName("otapplydel")]
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

            string qotstatus = "", qtmpsdate = "", qtmpedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qotstatus"]))
            {
                qotstatus = Request["qotstatus"].Trim();
                ViewBag.qotstatus = qotstatus;
            }
            if (!string.IsNullOrWhiteSpace(Request["qtmpsdate"]))
            {
                qtmpsdate = Request["qtmpsdate"].Trim();
                ViewBag.qtmpsdate = qtmpsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qtmpedate"]))
            {
                qtmpedate = Request["qtmpedate"].Trim();
                ViewBag.qtmpedate = qtmpedate;
            }


            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/otapplylog/main' method='post'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

            tmpform += "<input type=hidden id='qotstatus' name='qotstatus' value='" + qotstatus + "'>";
            tmpform += "<input type=hidden id='qtmpsdate' name='qtmpsdate' value='" + qtmpsdate + "'>";
            tmpform += "<input type=hidden id='qtmpedate' name='qtmpedate' value='" + qtmpedate + "'>";

            tmpform += "</form>";
            tmpform += "</body>";


            string cdel = Request["cdel"];

            if (string.IsNullOrWhiteSpace(cdel))
            {

                return new ContentResult() { Content = @"<script>alert('請勾選要撤回的資料!!');</script>" + tmpform };
            }
            else
            {
                NDcommon dbobj = new NDcommon();
                string tmpcomment = ""; int tmpcount = 0;


                using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                {
                    string mailtitle = "加班申請單撤回通知", MailContext = "";
                    


                    string sql = "select * from otapply WHERE otlogid in (" + cdel + ")";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        string setime = "自 {0}({2}) <BR>至 {1}({3})";

                        SqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            //寄信(通知給目前簽核角色)
                            MailContext = "";
                            MailContext += "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body>";
                            MailContext += "以下為明細資料：<BR>";
                            MailContext += "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                            MailContext += "<tr><td align=right width=130>申請單號：</td><td>" + dbobj.get_dbnull2(dr["osno"]) + "</td></tr>";
                            MailContext += "<tr><td align=right width=130>申請人：</td><td>" + dbobj.get_dbnull2(dr["empname"]) + "</td></tr>";
                            MailContext += "<tr><td align=right>起迄時間：</td><td>" + string.Format(setime, dbobj.get_dbDate(dr["otlogsdate"], "yyyy/MM/dd"), dbobj.get_dbDate(dr["otlogedate"], "yyyy/MM/dd"), dbobj.get_dbnull2(dr["otlogstime"]), dbobj.get_dbnull2(dr["otlogetime"])) + "</td></tr>";
                            MailContext += "<tr><td align=right>共計小時：</td><td>" + dbobj.get_dbnull2(dr["otloghour"]) + "時</td></tr>";
                            if (dbobj.get_dbnull2(dr["otcomment"]) != "")
                            {
                                MailContext += "<tr><td align=right>事由：</td><td>" + dr["otcomment"].ToString().Trim().Replace(Environment.NewLine, "<br>") + "</td></tr>";
                            }
                            else
                            {
                                MailContext += "<tr><td align=right>事由：</td><td>&#160;</td></tr>";
                            }
                            MailContext += "</table></body></HTML>";

                            #region 寄信(通知給目前簽核角色)
                            if (dbobj.get_dbnull2(dr["rolestampid"]) != "")
                            {
                                string rolestampid = dbobj.get_dbnull2(dr["rolestampid"]);
                                string sql_m = "select enemail from viewemprole where rid in(" + rolestampid + ") and empstatus <> '4' and enemail<>'' and comid='" + (string)Session["comid"] + "'";
                                using (SqlConnection comconn = dbobj.get_conn("Aitag_DBContext"))
                                {
                                    using (SqlCommand cmd1 = new SqlCommand(sql_m, comconn))
                                    {
                                        SqlDataReader dr1 = cmd1.ExecuteReader();
                                        string tomail = "";
                                        while (dr.Read())
                                        {
                                            tomail += dr["enemail"] + ",";
                                        }
                                        dbobj.send_mailfile("", tomail, mailtitle, MailContext, null, null);
                                        dr1.Close();
                                    }
                                }
                            }
                            #endregion
                            //======


                            


                            //LOG檔
                            tmpcount++;
                            tmpcomment += "申請單號：,";
                            //======
                        }
                        dr.Close();
                    }
                }

                //執行撤回
                dbobj.dbexecute("Aitag_DBContext", "UPDATE otapply SET otstatus = 'D' WHERE otlogid in (" + cdel + ")");


                //系統LOG檔
                string sysnote = "";
                if (sysnote.Length > 4000) { sysnote = sysnote.Substring(0, 4000); }
                //================================================= //
                SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                string sysrealsid = Request["sysrealsid"].ToString();
                string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                string sysflag = "D";
                dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                sysconn.Close();
                sysconn.Dispose();
                //=================================================

                

                return new ContentResult() { Content = @"<script>alert('撤回成功!!');</script>" + tmpform };
                //return RedirectToAction("List");
            }
        }

        #endregion

        #region 加班作業 > 加班申請單審核
        public ActionResult chkmod(otapply chks, string sysflag, int? page, string orderdata, string orderdata1)
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

            string qotstatus = "", qtmpsdate = "", qtmpedate = "";
            
            if (!string.IsNullOrWhiteSpace(Request["qtmpsdate"]))
            {
                qtmpsdate = Request["qtmpsdate"].Trim();
                ViewBag.qtmpsdate = qtmpsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qtmpedate"]))
            {
                qtmpedate = Request["qtmpedate"].Trim();
                ViewBag.qtmpedate = qtmpedate;
            }

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    otapply eotapplylogs = con.otapply.Find(chks.otlogid);
                    if (eotapplylogs == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eotapplylogs);
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
                    string sysnote = "";

                    otapply col = new otapply();
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        col = con.otapply.Find(chks.otlogid);
                    }
                    if (dbobj.get_dbnull2(Request["otstatus"]) == "1")
                    {
                        //審核通過
                        string tmprolestampid = col.rolestampid;
                        string rolea_1 = col.rolestampidall;
                        string roleall = rolea_1 + "," + tmprolestampid; //'簽核過角色(多個)
                        string billflowid = col.billflowid.ToString();

                        //找出下一個角色是誰
                        string tmprole = dbobj.getnewcheck1("B", tmprolestampid, roleall, "", "", billflowid);

                        if (tmprole == "'topman'")
                        {
                            tmprole = "";
                        }
                        string otstatus = "";
                        if (tmprole == "")
                        {
                            otstatus = "1";// '己簽核
                        }
                        else
                        {
                            otstatus = "0";
                            //'找往上呈核長管級數
                            //'==========================
                            string tmpflowlevel = "";
                            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                            {
                                tmpflowlevel = dbobj.get_dbvalue(conn, "select billflow from flowlevel where bid=" + billflowid);
                            }
                            if (tmpflowlevel == "")
                            {
                                tmpflowlevel = "0";
                            }
                            string[] tmpa = rolea_1.Split(',');
                            int tmpacount = tmpa.Length;
                            if (int.Parse(tmpflowlevel) == (tmpacount + 1))
                            {
                                tmprole = "";
                                otstatus = "1"; // '己簽核
                            }
                            //'==========================
                        }

                        col.otstatus = otstatus;
                        col.rolestampid = tmprole;
                        col.rolestampidall = roleall;
                        col.empstampidall = col.empstampidall + ",'" + (string)Session["empid"] + "'"; //'所有人員帳號
                        col.bmodid = (string)Session["empid"];
                        col.bmoddate = DateTime.Now;
                        col.billtime = col.billtime + "," + DateTime.Now.ToString();

                        if (tmprole != "")
                        {
                            //寄信
                            SetMail1(col, tmprole);
                            sysnote = "加班單審核作業";
                        }
                        else
                        {
                            //沒有下一個承辦人  (己通過)
                            ////資料通過後 搬移到cardreallog
                            //battacheckmainEditMove(col);

                            //(己通過)  寄信
                            SetMail1(col, "PassMail");
                            sysnote = "加班單審核通過作業";
                        }

                    }
                    else
                    {
                        //退回
                        col.otstatus = "2";
                        col.otback = chks.otback;
                        col.bmodid = (string)Session["empid"];
                        col.bmoddate = DateTime.Now;
                        col.billtime = col.billtime + "," + DateTime.Now.ToString();


                        //退回作業  寄信
                        SetMail1(col, "otbackMail");
                        sysnote = "加班單退回作業";

                    }

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.Entry(col).State = EntityState.Modified;
                        con.SaveChanges();
                    }

                    ////系統LOG檔
                    sysnote = "申請人：" + col.empname + "<br>申請單號：" + col.osno + "的資料(" + sysnote + ")<br>";
                    if (sysnote.Length > 4000) { sysnote = sysnote.Substring(0, 4000); }
                    //================================================= //
                    SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    string sysrealsid = Request["sysrealsid"].ToString();
                    string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    sysconn.Close();
                    sysconn.Dispose();
                    //=================================================

                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/otapplylog/chk' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

                    tmpform += "<input type=hidden id='qotstatus' name='qotstatus' value='" + qotstatus + "'>";
                    tmpform += "<input type=hidden id='qtmpsdate' name='qtmpsdate' value='" + qtmpsdate + "'>";
                    tmpform += "<input type=hidden id='qtmpedate' name='qtmpedate' value='" + qtmpedate + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"" + tmpform };
                }
            }

        }

        private void SetMail1(otapply col, string tmprole)
        {
            NDcommon dbobj = new NDcommon();
            string toadd = "";

            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                toadd = dbobj.get_dbvalue(conn, "select enemail from employee where empid='" + col.empid + "'");
            }

            #region Mail 內容MailContext
            string mailtitle = "", MailContext = "";
            MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body>";
            MailContext = MailContext + "以下為明細資料：<BR>";
            MailContext = MailContext + "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
            MailContext = MailContext + "<tr><td align=right width=130>申請人：</td><td>" + col.empname + "</td></tr>";
            //出差起迄日期
            string SEDate = "自{0}({1})<BR>至{2}({3})";
            SEDate = string.Format(SEDate, dbobj.get_dbDate(col.otlogsdate, "d"), col.otlogstime
                , dbobj.get_dbDate(col.otlogedate, "d"), col.otlogetime);
            MailContext = MailContext + "<tr><td align=right>加班起迄日期：</td><td>" + SEDate + "</td></tr>";

            string hloghour = dbobj.get_dbnull2(col.otloghour);
            //bloghour = (double.Parse(bloghour) / 8).ToString("0.#");
            MailContext = MailContext + "<tr><td align=right>共計小時：</td><td>" + hloghour + "</td></tr>";
            if (dbobj.get_dbnull2(col.otcomment) != "")
            {
                MailContext = MailContext + "<tr><td align=right>事由：</td><td>" + col.otcomment.ToString().Trim().Replace(Environment.NewLine, "<br>") + "</td></tr>";
            }
            else
            {
                MailContext = MailContext + "<tr><td align=right>事由：</td><td>&nbsp;</td></tr>";
            }
            MailContext = MailContext + "</table>";
            MailContext = MailContext + "</body></HTML>";
            #endregion

            if (tmprole != "PassMail" && tmprole != "otbackMail")
            {
                #region 寄給下一個承辦人

                mailtitle = "【" + col.empname + "】加班申請單資料要求審核通知";

                using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                {
                    tmprole = tmprole.Replace("'", "");
                    string sql = "select enemail from viewemprole where rid = '" + tmprole + "' and empstatus <> '4' and enemail<>''";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        SqlDataReader dr = cmd.ExecuteReader();
                        string tomail = "";
                        while (dr.Read())
                        {
                            tomail += dr["enemail"] + ",";
                        }
                        dbobj.send_mailfile("", tomail, mailtitle, MailContext, null, null);
                        dr.Close();
                    }
                }
                #endregion
            }
            else if (tmprole == "PassMail")
            {
                #region 寄送mail給申請人

                mailtitle = "加班單資料已通過核准";

                dbobj.send_mailfile("", toadd, mailtitle, MailContext, null, null);

                #endregion
            }
            else if (tmprole == "otbackMail")
            {
                #region 寄送mail給申請人

                mailtitle = "出差單資料退回";

                dbobj.send_mailfile("", toadd, mailtitle, MailContext, null, null);
                #endregion
            }
        }
        public ActionResult chk(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "otlogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qtmpsdate = "", qtmpedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qtmpsdate"]))
            {
                qtmpsdate = Request["qtmpsdate"].Trim();
                ViewBag.qtmpsdate = qtmpsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qtmpedate"]))
            {
                qtmpedate = Request["qtmpedate"].Trim();
                ViewBag.qtmpedate = qtmpedate;
            }


            IPagedList<otapply> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from otapply  where (otype='1' and otstatus = '0' and rolestampid = '''" + Session["rid"] + "''')  and";

                if (qtmpsdate != "")
                {
                    sqlstr += " otlogsdate >= '" + qtmpsdate + "'  and";
                }
                if (qtmpedate != "")
                {
                    sqlstr += " otlogedate <= '" + qtmpedate + "'  and";
                }


                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.otapply.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<otapply>(page.Value - 1, (int)Session["pagesize"]);

            }
            NDcommon dbobj = new NDcommon();
            string[] od_ch = { "ottype", "empid", "otlogsdate" };
            string[] od_ch1 = { "asc", "asc", "asc" };
            ViewBag.SetOrder_ch = dbobj.SetOrder(orderdata, orderdata1, od_ch, od_ch1);
            return View(result);
        }
        

        #endregion

        #region 加班作業 > 加班確認單撤回

        public ActionResult otworklog(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "otlogsdate"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qotstatus = "", qtmpsdate = "", qtmpedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qotstatus"]))
            {
                qotstatus = Request["qotstatus"].Trim();
                ViewBag.qotstatus = qotstatus;
            }

            NDcommon dbobj = new NDcommon();
            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qtmpsdate"], "m", "min", @"加班日期起格式錯誤!!\n", out qtmpsdate, out DateEx);
            ViewBag.qtmpsdate = qtmpsdate;
            dbobj.get_dateRang(Request["qtmpedate"], "m", "max", @"加班日期訖格式錯誤!!\n", out qtmpedate, out DateEx1);
            ViewBag.qtmpedate = qtmpedate;
            DateEx += DateEx1;


            IPagedList<otworklog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from otworklog  where empid='" + Session["empid"] + "' and otstatus in ('0','1') and comid='" + Session["comid"] +"'   and";
                if (qotstatus != "")
                {
                    sqlstr += " otstatus='" + qotstatus + "'  and";
                }


                sqlstr += " ((otlogsdate >= '" + qtmpsdate + "' and otlogsdate <= '" + qtmpedate + "')   or";

                sqlstr += " (otlogedate <= '" + qtmpsdate + "' and otlogedate <= '" + qtmpedate + "'))  and";
                


                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.otworklog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<otworklog>(page.Value - 1, (int)Session["pagesize"]);

            }

            string[] od_ch = { "ottype", "otstatus", "otlogsdate" };
            string[] od_ch1 = { "asc", "asc", "asc" };
            ViewBag.SetOrder_ch = dbobj.SetOrder(orderdata, orderdata1, od_ch, od_ch1);
            return View(result);
        }

        [ActionName("otworklogdel")]
        public ActionResult DeleteConfirmed1(string id, int? page)
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

            string qotstatus = "", qtmpsdate = "", qtmpedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qotstatus"]))
            {
                qotstatus = Request["qotstatus"].Trim();
                ViewBag.qotstatus = qotstatus;
            }
            if (!string.IsNullOrWhiteSpace(Request["qtmpsdate"]))
            {
                qtmpsdate = Request["qtmpsdate"].Trim();
                ViewBag.qtmpsdate = qtmpsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qtmpedate"]))
            {
                qtmpedate = Request["qtmpedate"].Trim();
                ViewBag.qtmpedate = qtmpedate;
            }


            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/otapplylog/otworklog' method='post'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

            tmpform += "<input type=hidden id='qotstatus' name='qotstatus' value='" + qotstatus + "'>";
            tmpform += "<input type=hidden id='qtmpsdate' name='qtmpsdate' value='" + qtmpsdate + "'>";
            tmpform += "<input type=hidden id='qtmpedate' name='qtmpedate' value='" + qtmpedate + "'>";

            tmpform += "</form>";
            tmpform += "</body>";


            string cdel = Request["cdel"];

            if (string.IsNullOrWhiteSpace(cdel))
            {

                return new ContentResult() { Content = @"<script>alert('請勾選要撤回的資料!!');</script>" + tmpform };
            }
            else
            {
                NDcommon dbobj = new NDcommon();
                string tmpcomment = ""; int tmpcount = 0;


                using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                {
                    string mailtitle = "加班確認單撤回通知", MailContext = "";



                    string sql = "select * from otworklog WHERE otlogid in (" + cdel + ") and comid='"+ Session["comid"] +"'";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        string setime = "自 {0}({2}) <BR>至 {1}({3})";

                        SqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            //寄信(通知給目前簽核角色)
                            MailContext = "";
                            MailContext += "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body>";
                            MailContext += "以下為明細資料：<BR>";
                            MailContext += "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                            //MailContext += "<tr><td align=right width=130>申請單號：</td><td>" + dbobj.get_dbnull2(dr["osno"]) + "</td></tr>";
                            MailContext += "<tr><td align=right width=130>申請人：</td><td>" + dbobj.get_dbnull2(dr["empname"]) + "</td></tr>";
                            MailContext += "<tr><td align=right>起迄時間：</td><td>自" + string.Format(setime, dbobj.get_dbDate(dr["otlogsdate"], "yyyy/MM/dd"), dbobj.get_dbDate(dr["otlogedate"], "yyyy/MM/dd"), dbobj.get_dbnull2(dr["otlogstime"]), dbobj.get_dbnull2(dr["otlogetime"])) + "</td></tr>";
                            MailContext += "<tr><td align=right>共計小時：</td><td>" + dbobj.get_dbnull2(dr["otloghour"]) + "時</td></tr>";
                            if (dbobj.get_dbnull2(dr["otcomment"]) != "")
                            {
                                MailContext += "<tr><td align=right>事由：</td><td>" + dr["otcomment"].ToString().Trim().Replace(Environment.NewLine, "<br>") + "</td></tr>";
                            }
                            else
                            {
                                MailContext += "<tr><td align=right>事由：</td><td>&nbsp;</td></tr>";
                            }
                            MailContext += "</table></body></HTML>";

                            #region 寄信(通知給目前簽核角色)
                            if (dbobj.get_dbnull2(dr["rolestampid"]) != "")
                            {
                                string rolestampid = dbobj.get_dbnull2(dr["rolestampid"]);
                                string sql_m = "select enemail from viewemprole where rid in(" + rolestampid + ") and empstatus <> '4' and enemail<>'' and comid='" + (string)Session["comid"] + "'";
                                using (SqlConnection comconn = dbobj.get_conn("Aitag_DBContext"))
                                {
                                    using (SqlCommand cmd1 = new SqlCommand(sql_m, comconn))
                                    {
                                        SqlDataReader dr1 = cmd1.ExecuteReader();
                                        string tomail = "";
                                        while (dr.Read())
                                        {
                                            tomail += dr["enemail"] + ",";
                                        }
                                        dbobj.send_mailfile("", tomail, mailtitle, MailContext, null, null);
                                        dr1.Close();
                                    }
                                }
                            }
                            #endregion
                            //======





                            //LOG檔
                            tmpcount++;
                            tmpcomment += "申請單號：" + ViewBag.osno + "";
                            //======
                        }
                        dr.Close();
                    }
                }

                //執行撤回
                dbobj.dbexecute("Aitag_DBContext", "UPDATE otworklog SET otstatus = 'D' WHERE otlogid in (" + cdel + ")");


                //系統LOG檔
                string tmpevent = tmpcomment + "的資料" + tmpcount + "筆";
                string sysnote = tmpevent;
                if (sysnote.Length > 4000) { sysnote = sysnote.Substring(0, 4000); }
                //================================================= //
                SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                string sysrealsid = Request["sysrealsid"].ToString();
                string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                string sysflag = "D";
                dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                sysconn.Close();
                sysconn.Dispose();
                //=================================================



                return new ContentResult() { Content = @"<script>alert('撤回成功!!');</script>" + tmpform };
                //return RedirectToAction("List");
            }
        }

        #endregion

        #region 加班作業 > 加班確認單審核
        public ActionResult otworklogcheckmod(otworklog chks, string sysflag, int? page, string orderdata, string orderdata1)
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

            string qotstatus = "", qtmpsdate = "", qtmpedate = "";

            if (!string.IsNullOrWhiteSpace(Request["qtmpsdate"]))
            {
                qtmpsdate = Request["qtmpsdate"].Trim();
                ViewBag.qtmpsdate = qtmpsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qtmpedate"]))
            {
                qtmpedate = Request["qtmpedate"].Trim();
                ViewBag.qtmpedate = qtmpedate;
            }

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    otworklog eotworkloglogs = con.otworklog.Find(chks.otlogid);
                    if (eotworkloglogs == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eotworkloglogs);
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
                    string sysnote = "";

                    otworklog col = new otworklog();
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        col = con.otworklog.Find(chks.otlogid);
                    }
                    if (dbobj.get_dbnull2(Request["otstatus"]) == "1")
                    {
                        //審核通過
                        string tmprolestampid = col.rolestampid;
                        string rolea_1 = col.rolestampidall;
                        string roleall = rolea_1 + "," + tmprolestampid; //'簽核過角色(多個)
                        string billflowid = col.billflowid.ToString();

                        //找出下一個角色是誰
                        string tmprole = dbobj.getnewcheck1("B", tmprolestampid, roleall, "", "", billflowid);

                        if (tmprole == "'topman'")
                        {
                            tmprole = "";
                        }
                        string otstatus = "";
                        if (tmprole == "")
                        {
                            otstatus = "1";// '己簽核
                        }
                        else
                        {
                            otstatus = "0";
                            //'找往上呈核長管級數
                            //'==========================
                            string tmpflowlevel = "";
                            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                            {
                                tmpflowlevel = dbobj.get_dbvalue(conn, "select billflow from flowlevel where bid=" + billflowid);
                            }
                            if (tmpflowlevel == "")
                            {
                                tmpflowlevel = "0";
                            }
                            string[] tmpa = rolea_1.Split(',');
                            int tmpacount = tmpa.Length;
                            if (int.Parse(tmpflowlevel) == (tmpacount + 1))
                            {
                                tmprole = "";
                                otstatus = "1"; // '己簽核
                            }
                            //'==========================
                        }

                        col.otstatus = otstatus;
                        col.rolestampid = tmprole;
                        col.rolestampidall = roleall;
                        col.empstampidall = col.empstampidall + ",'" + (string)Session["empid"] + "'"; //'所有人員帳號
                        col.bmodid = (string)Session["empid"];
                        col.bmoddate = DateTime.Now;
                        col.billtime = col.billtime + "," + DateTime.Now.ToString();

                        if (tmprole != "")
                        {
                            //寄信
                            SetMail2(col, tmprole);
                        }
                        else
                        {
                            //沒有下一個承辦人  (己通過)


                            //已通過  將資料轉到 resthourlog
                            Setresthourlog(dbobj, col);



                            //(己通過)  寄信
                            SetMail2(col, "PassMail");
                        }
                        sysnote = "加班單審核通過作業";

                    }
                    else
                    {
                        //退回
                        col.otstatus = "2";
                        col.otback = chks.otback;
                        col.bmodid = (string)Session["empid"];
                        col.bmoddate = DateTime.Now;
                        col.billtime = col.billtime + "," + DateTime.Now.ToString();


                        //退回作業  寄信
                        SetMail2(col, "otbackMail");
                        sysnote = "加班單退回作業";

                    }

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.Entry(col).State = EntityState.Modified;
                        con.SaveChanges();
                    }

                    //系統LOG檔
                    sysnote = "申請人：" + col.empname + "<br>申請單號：" + col.osno + "的資料(" + sysnote + ")<br>";
                    if (sysnote.Length > 4000) { sysnote = sysnote.Substring(0, 4000); }
                    //================================================= //
                    SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    string sysrealsid = Request["sysrealsid"].ToString();
                    string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    sysconn.Close();
                    sysconn.Dispose();
                    //=================================================

                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/otapplylog/chk' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

                    tmpform += "<input type=hidden id='qotstatus' name='qotstatus' value='" + qotstatus + "'>";
                    tmpform += "<input type=hidden id='qtmpsdate' name='qtmpsdate' value='" + qtmpsdate + "'>";
                    tmpform += "<input type=hidden id='qtmpedate' name='qtmpedate' value='" + qtmpedate + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"" + tmpform };
                }
            }

        }

        private void Setresthourlog(NDcommon dbobj, Models.otworklog Pcol)
        {
            resthourlog rs1 = new resthourlog();

            rs1.rstype = "2";
            rs1.otlogid = Pcol.otlogid;
            rs1.osno = Pcol.osno;
            rs1.empid = Pcol.empid;

            //Pcol.otlogsdate

            DateTime otlogsdate = Pcol.otlogsdate.Value;

            if (dbobj.check_holiday(otlogsdate.ToString("yyyy/MM/dd"), Session["comid"].ToString()) == "1")
            {
                if (Pcol.otmoneyhour > 2)
                {
                    rs1.moneyh1 = 2;
                    rs1.moneyh2 = Pcol.otmoneyhour - 2;
                }
                else
                {
                    rs1.moneyh1 = Pcol.otmoneyhour;
                }
            }
            else
            {
                rs1.moneyh3 = Pcol.otmoneyhour;
            }


            //誤餐費 20101229 Mark , 超過2個小時有誤餐費
            if (dbobj.check_holiday(otlogsdate.ToString("yyyy/MM/dd"), Session["comid"].ToString()) == "1")
            {
                if (Pcol.otmoneyhour > 2 || Pcol.otresthour > 2)
                {
                    rs1.ifdinner = "y";
                }
                else
                {
                    rs1.ifdinner = "n";
                }
            }
            else
            {
                if ( Pcol.otresthour > 4)
                {
                    rs1.ifdinner = "y";
                }
                else
                {
                    rs1.ifdinner = "n";
                }
            }

            rs1.resthour = Pcol.otresthour;
            rs1.moneyhour = Pcol.otmoneyhour;
            rs1.ottype = Pcol.ottype;
            rs1.inout = Pcol.inout;
            rs1.ifactive = "y";

            rs1.adddate = Pcol.otlogsdate;
            rs1.rsdeaddate = otlogsdate.AddDays(183);

            int tmpmonth = otlogsdate.Month;
            int tmpyear = otlogsdate.Year;
            if (tmpmonth == 12)
            {
                tmpyear++;
                tmpmonth = 1;
            }
            else
            {
                tmpmonth++;
            }

            if (otlogsdate.Month == 12)
            {
                rs1.mydeaddate = Convert.ToDateTime(otlogsdate.Year.ToString() + "/" + otlogsdate.Month.ToString() + "/31").AddDays(1);
            }
            else
            {
                rs1.mydeaddate = Convert.ToDateTime(tmpyear.ToString() + "/" + tmpmonth.ToString() + "/5");
            }


            rs1.comid = Session["comid"].ToString();
            rs1.bmodid = Session["empid"].ToString();
            rs1.bmoddate = DateTime.Now;

            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                con.resthourlog.Add(rs1);
                try
                {
                    con.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        private void SetMail2(otworklog col, string tmprole)
        {
            NDcommon dbobj = new NDcommon();
            string toadd = "";

            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                toadd = dbobj.get_dbvalue(conn, "select enemail from employee where empid='" + col.empid + "'");
            }

            #region Mail 內容MailContext
            string mailtitle = "", MailContext = "";
            MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body>";
            MailContext = MailContext + "以下為明細資料：<BR>";
            MailContext = MailContext + "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
            MailContext = MailContext + "<tr><td align=right width=130>申請人：</td><td>" + col.empname + "</td></tr>";
            //出差起迄日期
            string SEDate = "自{0}({1})<BR>至{2}({3})";
            SEDate = string.Format(SEDate, dbobj.get_dbDate(col.otlogsdate, "d"), col.otlogstime
                , dbobj.get_dbDate(col.otlogedate, "d"), col.otlogetime);
            MailContext = MailContext + "<tr><td align=right>起迄日期：</td><td>" + SEDate + "</td></tr>";

            string hloghour = dbobj.get_dbnull2(col.otloghour);
            //bloghour = (double.Parse(bloghour) / 8).ToString("0.#");
            MailContext = MailContext + "<tr><td align=right>共計小時：</td><td>" + hloghour + "時</td></tr>";
            if (dbobj.get_dbnull2(col.otcomment) != "")
            {
                MailContext = MailContext + "<tr><td align=right>事由：</td><td>" + col.otcomment.ToString().Trim().Replace(Environment.NewLine, "<br>") + "</td></tr>";
            }
            else
            {
                MailContext = MailContext + "<tr><td align=right>事由：</td><td>&nbsp;</td></tr>";
            }
            MailContext = MailContext + "</table>";
            MailContext = MailContext + "</body></HTML>";
            #endregion

            if (tmprole != "PassMail" && tmprole != "otbackMail")
            {
                #region 寄給下一個承辦人

                mailtitle = "【" + col.empname + "】加班單資料要求審核通知";

                using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                {
                    tmprole = tmprole.Replace("'", "");
                    string sql = "select enemail from viewemprole where rid = '" + tmprole + "' and empstatus <> '4' and enemail<>''";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        SqlDataReader dr = cmd.ExecuteReader();
                        string tomail = "";
                        while (dr.Read())
                        {
                            tomail += dr["enemail"] + ",";
                        }
                        dbobj.send_mailfile("", tomail, mailtitle, MailContext, null, null);
                        dr.Close();
                    }
                }
                #endregion
            }
            else if (tmprole == "PassMail")
            {
                #region 寄送mail給申請人

                mailtitle = "加班確認單資料已通過核准";

                dbobj.send_mailfile("", toadd, mailtitle, MailContext, null, null);

                #endregion
            }
            else if (tmprole == "otbackMail")
            {
                #region 寄送mail給申請人

                mailtitle = "加班確認單資料退回";

                dbobj.send_mailfile("", toadd, mailtitle, MailContext, null, null);
                #endregion
            }
        }
        public ActionResult otworklogcheck(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "otlogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qtmpsdate = "", qtmpedate = "";

            if (!string.IsNullOrWhiteSpace(Request["qtmpsdate"]))
            {
                qtmpsdate = Request["qtmpsdate"].Trim();
                ViewBag.qtmpsdate = qtmpsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qtmpedate"]))
            {
                qtmpedate = Request["qtmpedate"].Trim();
                ViewBag.qtmpedate = qtmpedate;
            }


            IPagedList<otworklog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {

                string sqlstr = "select * from otworklog  where (otstatus='0' and rolestampid = '''" + Session["rid"] + "''')  and";

                if (qtmpsdate != "")
                {
                    sqlstr += " otlogsdate >= '" + qtmpsdate + "'  and";
                }
                if (qtmpedate != "")
                {
                    sqlstr += " otlogedate <= '" + qtmpedate + "'  and";
                }


                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.otworklog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<otworklog>(page.Value - 1, (int)Session["pagesize"]);

            }
            NDcommon dbobj = new NDcommon();
            string[] od_ch = { "ottype", "otstatus", "otlogsdate" };
            string[] od_ch1 = { "asc", "asc", "asc" };
            ViewBag.SetOrder_ch = dbobj.SetOrder(orderdata, orderdata1, od_ch, od_ch1);
            return View(result);
        }
        

        #endregion

        #region 加班作業 > 加班補休異動查詢
        public ActionResult resthourlog1(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "otlogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qottype = "", qaddsdate = "", qaddedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qottype"]))
            {
                qottype = Request["qottype"].Trim();
                ViewBag.qottype = qottype;
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


            IPagedList<resthourlog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string dt = DateTime.Now.ToString("yyyy/MM/dd");
                string sqlstr = "select * from resthourlog where rstype = '2' and empid = '" + Session["empid"] + "' and adddate <= '" + dt + "' and mydeaddate >= '" + dt + "' and  ifactive = 'y'  and comid='" + Session["comid"] + "'  and";
                if (qottype != "")
                {
                    sqlstr += " ottype='" + qottype + "'  and";
                }

                if (qaddsdate != "")
                {
                    sqlstr += " adddate >= '" + qaddsdate + "'  and";
                }
                if (qaddedate != "")
                {
                    sqlstr += " adddate <= '" + qaddedate + "'  and";
                }


                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.resthourlog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<resthourlog>(page.Value - 1, (int)Session["pagesize"]);

            }
            NDcommon dbobj = new NDcommon();
            string[] od_ch = { "ottype", "adddate" };
            string[] od_ch1 = { "asc", "asc", "asc" };
            ViewBag.SetOrder_ch = dbobj.SetOrder(orderdata, orderdata1, od_ch, od_ch1);
            return View(result);
        }



        #endregion

        #region 加班作業 > 加班確認明細查詢
        public ActionResult otworklogqry(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "otlogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qotstatus = "", qtmpsdate = "", qtmpedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qotstatus"]))
            {
                qotstatus = Request["qotstatus"].Trim();
                ViewBag.qotstatus = qotstatus;
            }
            
            NDcommon dbobj = new NDcommon();
            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qtmpsdate"], "m", "min", @"otlogsdate!!\n", out qtmpsdate, out DateEx);
            ViewBag.qtmpsdate = qtmpsdate;
            dbobj.get_dateRang(Request["qtmpedate"], "m", "max", @"otlogedate!!\n", out qtmpedate, out DateEx1);
            ViewBag.qtmpedate = qtmpedate;
            DateEx += DateEx1;

            if (!string.IsNullOrWhiteSpace(Request["qotstatus"]))
            {
                if (Request["qotstatus"] == "all")
                {
                    qotstatus = "";
                    ViewBag.qotstatus = qotstatus;
                }
                else
                {
                    qotstatus = Request["qotstatus"].Trim();
                    ViewBag.qotstatus = qotstatus;
                }
            }
            else
            {
                qotstatus = "1";
                ViewBag.qotstatus = qotstatus;
            }

            IPagedList<otworklog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from otworklog where empid = '" + Session["empid"] + "' and comid='" + Session["comid"] + "'  and";
                if (qotstatus != "")
                {
                    sqlstr += " otstatus='" + qotstatus + "'  and";
                }

                if (qtmpsdate != "")
                {
                    sqlstr += " otlogsdate >= '" + qtmpsdate + "'  and";
                }
                if (qtmpedate != "")
                {
                    sqlstr += " otlogedate <= '" + qtmpedate + "'  and";
                }


                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.otworklog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<otworklog>(page.Value - 1, (int)Session["pagesize"]);

            }

            string[] od_ch = { "otstatus", "ifotdell", "empname", "otlogsdate" };
            string[] od_ch1 = { "asc", "asc", "asc", "asc" };
            ViewBag.SetOrder_ch = dbobj.SetOrder(orderdata, orderdata1, od_ch, od_ch1);
            return View(result);
        }
        
        //觀看
        public ActionResult otworklogmod(otworklog chks, string sysflag, int? page, string orderdata, string orderdata1, HttpPostedFileBase logopic1)
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

            string qotstatus = "", qtmpsdate = "", qtmpedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qotstatus"]))
            {
                qotstatus = Request["qotstatus"].Trim();
                ViewBag.qotstatus = qotstatus;
            }
            if (!string.IsNullOrWhiteSpace(Request["qtmpsdate"]))
            {
                qtmpsdate = Request["qtmpsdate"].Trim();
                ViewBag.qtmpsdate = qtmpsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qtmpedate"]))
            {
                qtmpedate = Request["qtmpedate"].Trim();
                ViewBag.qtmpedate = qtmpedate;
            }

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    //var data = con.battalog.Where(r => r.blogid == chks.blogid).FirstOrDefault();
                    otworklog eotworklog = con.otworklog.Find(chks.otlogid);
                    if (eotworklog == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eotworklog);
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

                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/otapplylog/otworklogqry' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

                    tmpform += "<input type=hidden id='qotstatus' name='qotstatus' value='" + qotstatus + "'>";
                    tmpform += "<input type=hidden id='qtmpsdate' name='qtmpsdate' value='" + qtmpsdate + "'>";
                    tmpform += "<input type=hidden id='qtmpedate' name='qtmpedate' value='" + qtmpedate + "'>";

                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"" + tmpform };
                }
            }
        }

        #endregion


        
    }
}
