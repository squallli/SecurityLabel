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
    public class cardlogController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /cardlog/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        #region 刷卡作業 > 刷卡異常單申請
        public ActionResult cardlogadd(cardlog col, string sysflag, int? page, string orderdata, string orderdata1, HttpPostedFileBase logopic1)
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


            if (sysflag != "A")
            {
                cardlog newcol = new cardlog();
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
                    string errmsg = "";
                    //簽核
                    string tmparolestampid = "";
                    if (dbobj.get_dbnull2(Request["arolestampid"]) != "")
                    {
                        tmparolestampid = "'" + Request["arolestampid"].ToString() + "'";
                    }
                    else
                    {
                        tmparolestampid = "'" + Request["arolestampid1"].ToString() + "'";
                    }

                    string tmpmoney = "0", tmprole = "", tmpbillid = "";
                    //找出下一個角色是誰              
                    string impallstring = dbobj.getnewcheck1("Z", tmparolestampid, tmparolestampid, tmpmoney, "", "");
                    tmprole = impallstring.Split(';')[0].ToString();
                    tmpbillid = impallstring.Split(';')[1].ToString();
                    if (tmprole == "")
                    {
                        errmsg = "請先至表單流程設定中設定首長信箱的呈核流程!";
                        ViewBag.errmsg = "<script>alert('" + errmsg + "');</script>";
                        return View(col);
                    }
                    //簽核
                    if ((string)Session["mplayrole"] == "")
                    {
                        errmsg = "您並未設定呈核角色!";
                        ViewBag.errmsg = "<script>alert('" + errmsg + "');</script>";
                        return View(col);
                    }
                    else
                    {

                    }

                    //'找單據編號(自動產生編號)
                    string tmpcsno = "select csno from cardlog where year(cdate) = " + DateTime.Now.Year + " and month(cdate) = " + DateTime.Now.Month + " and csno is not null order by csno desc";
                    using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        tmpcsno = dbobj.get_dbvalue(conn, tmpcsno);
                    }

                    if (tmpcsno != "")
                    {
                        tmpcsno = tmpcsno.Substring(tmpcsno.Length - 3, 3);
                        tmpcsno = (int.Parse(tmpcsno) + 1).ToString("000");
                    }
                    else
                    {
                        tmpcsno = "001";
                    }
                    string tmpyear = (DateTime.Now.Year - 1911).ToString();
                    string tmpmonth = DateTime.Now.Month.ToString("00");
                    tmpcsno = "Z" + tmpyear + tmpmonth + tmpcsno;
                    //======





                    string clogstatus = "1"; //己簽核
                    if (tmprole != "")
                    {
                        clogstatus = "0";
                    }
                    col.clogstatus = clogstatus;
                    col.csno = tmpcsno;
                    col.clogtime = int.Parse(Request["cloghour"]).ToString("00") + int.Parse(Request["clogmin"]).ToString("00");
                    if (dbobj.get_dbnull2(Request["clogtype"]) == "3")
                    {
                        col.clogtime1 = int.Parse(Request["cloghour1"]).ToString("00") + int.Parse(Request["clogmin1"]).ToString("00");
                    }
                    //    '呈核人員
                    //'======================
                    string roleall = tmparolestampid;
                    col.arolestampid = tmparolestampid.Replace("'", "");  //'申請角色;
                    col.rolestampid = tmprole; //'下個呈核角色;
                    col.rolestampidall = roleall; //'所有呈核角色;
                    col.empstampidall = "'" + Request["empid"] + "'"; //'所有人員帳號;
                    col.billflowid = int.Parse(tmpbillid);
                    //'======================


                    //    '寄信
                    //'======================
                    //'找申請人mail
                    string fromadd = "", fromaddname = "", mailtitle = "", MailContext = "";
                    string chkitem = ""; //刷卡異常原因
                    using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        fromadd = dbobj.get_dbvalue(conn, "select enemail from employee where empid='" + Request["empid"] + "'");
                        fromaddname = dbobj.get_dbvalue(conn, "select empname from employee where empid='" + Request["empid"] + "'");
                        chkitem = dbobj.get_dbvalue(conn, "select chkitem from checkcode where chkcode='" + col.clogreason + "' and chkclass='66'");
                    }
                    //'寄送mail給下一個審核角色
                    #region 寄送mail給下一個審核角
                    mailtitle = "【" + fromaddname + "】刷卡異常單要求審核通知";
                    MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body>";
                    MailContext = MailContext + "以下為明細資料：<BR>";
                    MailContext = MailContext + "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                    MailContext = MailContext + "<tr><td align=right width=130>申請人：</td><td>" + col.empname + "</td></tr>";
                    if (dbobj.get_dbnull2(Request["clogtype"]) == "3")
                    {
                        MailContext = MailContext + "<tr><td align=right>刷卡日期：</td><td>" + col.clogdate + "　" + int.Parse(Request["cloghour"]).ToString("00") + "：" + int.Parse(Request["clogmin"]).ToString("00") + "-" + int.Parse(Request["cloghour1"]).ToString("00") + "：" + int.Parse(Request["clogmin1"]).ToString("00") + "</td></tr>";
                    }
                    else
                    {
                        MailContext = MailContext + "<tr><td align=right>刷卡日期：</td><td>" + col.clogdate + "　" + int.Parse(Request["cloghour"]).ToString("00") + "：" + int.Parse(Request["clogmin"]).ToString("00") + "</td></tr>";
                    }
                    MailContext = MailContext + "<tr><td align=right>刷卡異常原因：</td><td>" + chkitem + "</td></tr>";
                    MailContext = MailContext + "</table>";
                    MailContext = MailContext + "</body></HTML>";

                    using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        string tmproleid = tmprole.Replace("'", "");
                        string sql = "select enemail from viewemprole where rid = '" + tmproleid + "' and empstatus <> '4' and enemail<>''";
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

                    //寄送mail給支會人員
                    if (dbobj.get_dbnull2(Request["otherman"]) != "")
                    {
                        #region 支會人員
                        mailtitle = "【" + fromaddname + "】人員刷卡異常知會通知信";
                        MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body>";
                        MailContext = MailContext + "以下為明細資料：<BR>";
                        MailContext = MailContext + "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                        MailContext = MailContext + "<tr><td align=right width=130>申請人：</td><td>" + col.empname + "</td></tr>";
                        if (dbobj.get_dbnull2(Request["clogtype"]) == "3")
                        {
                            MailContext = MailContext + "<tr><td align=right>刷卡日期：</td><td>" + col.clogdate + "　" + int.Parse(Request["cloghour"]).ToString("00") + "：" + int.Parse(Request["clogmin"]).ToString("00") + "-" + int.Parse(Request["cloghour1"]).ToString("00") + "：" + int.Parse(Request["clogmin1"]).ToString("00") + "</td></tr>";
                        }
                        else
                        {
                            MailContext = MailContext + "<tr><td align=right>刷卡日期：</td><td>" + col.clogdate + "　" + int.Parse(Request["cloghour"]).ToString("00") + "：" + int.Parse(Request["clogmin"]).ToString("00") + "</td></tr>";
                        }
                        MailContext = MailContext + "<tr><td align=right>刷卡異常原因：</td><td>" + chkitem + "</td></tr>";
                        MailContext = MailContext + "</table>";
                        MailContext = MailContext + "</body></HTML>";

                        using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                        {
                            string otherman = Request["otherman"];
                            otherman = otherman.Substring(1, otherman.Length - 1);
                            string sql = "select enemail from employee where empid in (" + otherman + ") and enemail <> '' and enemail is not null";
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


                    //'======================


                    col.comid = (string)Session["comid"];
                    col.bmodid = (string)Session["empid"];
                    col.bmoddate = DateTime.Now;
                    col.cdate = DateTime.Now;
                    col.billtime = DateTime.Now.ToString();
                    if (dbobj.get_dbnull2(Request["otherman"]) != "")
                    {
                        col.empmeetsign = Request["otherman"];
                    }

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.cardlog.Add(col);
                        con.SaveChanges();
                    }
                    //系統LOG檔
                    string sysnote = "";
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
                    tmpform += "<form name='qfr1' action='/cardlog/cardlogmainList' method='post'>";
                    //tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    //tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    //tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    //tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    // return RedirectToAction("List");

                }
            }


        }

        #endregion

        #region 刷卡作業 > 刷卡異常單撤回
        public ActionResult cardlogmainEdit(cardlog chks, string sysflag, int? page, string orderdata, string orderdata1, HttpPostedFileBase logopic1)
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

            string qclogstatus = "", qclogsdate = "", qclogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qclogstatus"]))
            {
                qclogstatus = Request["qclogstatus"].Trim();
                ViewBag.qclogstatus = qclogstatus;
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

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    //var data = con.cardlog.Where(r => r.clogid == chks.clogid).FirstOrDefault();
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
                    //NDcommon dbobj = new NDcommon();



                    //chks.bmodid = Session["tempid"].ToString();
                    //chks.bmoddate = DateTime.Now;

                    //using (Aitag_DBContext con = new Aitag_DBContext())
                    //{
                    //    con.Entry(chks).State = EntityState.Modified;
                    //    con.SaveChanges();
                    //}

                    //系統LOG檔
                    //string sysnote = "";
                    //if (sysnote.Length > 4000) { sysnote = sysnote.Substring(0, 4000); }
                    ////================================================= //

                    //SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    //string sysrealsid = Request["sysrealsid"].ToString();
                    //string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    //dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    //sysconn.Close();
                    //sysconn.Dispose();
                    //=================================================

                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/cardlog/cardlogmainList' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

                    tmpform += "<input type=hidden id='qclogstatus' name='qclogstatus' value='" + qclogstatus + "'>";
                    tmpform += "<input type=hidden id='qclogsdate' name='qclogsdate' value='" + qclogsdate + "'>";
                    tmpform += "<input type=hidden id='qclogedate' name='qclogedate' value='" + qclogedate + "'>";

                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"" + tmpform };
                }
            }

        }

        public ActionResult cardlogmainList(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "clogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qclogstatus = "", qclogsdate = "", qclogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qclogstatus"]))
            {
                qclogstatus = Request["qclogstatus"].Trim();
                ViewBag.qclogstatus = qclogstatus;
            }

            NDcommon dbobj = new NDcommon();

            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qclogsdate"], "m", "min", @"請假起格式錯誤!!\n", out qclogsdate, out DateEx);
            ViewBag.qclogsdate = qclogsdate;
            dbobj.get_dateRang(Request["qclogedate"], "m", "max", @"請假訖格式錯誤!!\n", out qclogedate, out DateEx1);
            ViewBag.qclogedate = qclogedate;
            DateEx += DateEx1;

            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }


            IPagedList<cardlog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "SELECT * FROM cardlog where clogstatus not in ('1','D') and empid='" + (string)Session["empid"] + "' and  clogstatus in ('0','1')  ";

                if (qclogstatus != "")
                {
                    sqlstr += " and clogstatus = '" + qclogstatus + "'";
                }
                if (qclogsdate != "" && qclogedate != "")
                {
                    sqlstr += " and clogdate >= '" + qclogsdate + "' and clogdate <= '" + qclogedate + "'";
                }

                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.cardlog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<cardlog>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch1(orderdata, orderdata1);
            return View(result);
        }
        private string SetOrder_ch1(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "clogstatus", "clogreason", "clogdate" };
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

        [ActionName("cardlogmainDelete")]/*取別名*/
        public ActionResult cardlogmainDelete(string id, int? page)
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

            string qclogstatus = "", qclogsdate = "", qclogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qclogstatus"]))
            {
                qclogstatus = Request["qclogstatus"].Trim();
                ViewBag.qclogstatus = qclogstatus;
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



            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/cardlog/cardlogmainList' method='post'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

            tmpform += "<input type=hidden id='qclogstatus' name='qclogstatus' value='" + qclogstatus + "'>";
            tmpform += "<input type=hidden id='qclogsdate' name='qclogsdate' value='" + qclogsdate + "'>";
            tmpform += "<input type=hidden id='qclogedate' name='qclogedate' value='" + qclogedate + "'>";

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

                //執行撤回
                dbobj.dbexecute("Aitag_DBContext", "UPDATE cardlog SET clogstatus = 'D' where clogid in (" + cdel + ")");


                //系統LOG檔
                //================================================= //
                string sysnote = "";
                string sqlstr = "select * from cardlog where clogid in (" + cdel + ")";
                using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
                    {
                        SqlDataReader dr = cmd.ExecuteReader();
                        int drconn = 0;
                        while (dr.Read())
                        {
                            drconn++;
                            sysnote += "申請人：" + dr["empname"] + "," + "申請單號：" + dr["csno"];
                            //寄信
                            cardRevokeMail(dr);
                        }
                        sysnote = sysnote + "的資料" + drconn + "筆";
                        dr.Close();
                    }
                }
                if (sysnote.Length > 4000) { sysnote.Substring(0, 4000); }

                string sysrealsid = Request["sysrealsid"].ToString();//使用功能

                SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                string sysflag = "D";
                dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                sysconn.Close();
                sysconn.Dispose();
                //======================================================     


                return new ContentResult() { Content = @"<script>alert('撤回成功!!');</script>" + tmpform };
                //return RedirectToAction("List");
            }
        }
        //cardlog撤回  寄信
        private void cardRevokeMail(SqlDataReader dr)
        {
            NDcommon dbobj = new NDcommon();


            string mailtitle = "", MailContext = "";
            #region cardlog撤回  寄信
            string chkitem = "";
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                chkitem = dbobj.get_dbvalue(conn, "select chkitem from checkcode where chkcode='" + dr["clogreason"] + "' and chkclass='66'");
            }



            mailtitle = "刷卡異常單撤回通知";
            MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body>";
            MailContext = MailContext + "以下為明細資料：<BR>";
            MailContext = MailContext + "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
            MailContext = MailContext + "<tr><td align=right width=130>申請人：</td><td>" + dr["empname"] + "</td></tr>";
            if (dbobj.get_dbnull2(dr["clogtype"]) == "3")
            {
                MailContext = MailContext + "<tr><td align=right>刷卡日期：</td><td>" + dr["clogdate"] + "　" + int.Parse(dr["clogtime"].ToString()).ToString("00:00") + "-" + int.Parse(dr["clogtime1"].ToString()).ToString("00:00") + "</td></tr>";
            }
            else
            {
                MailContext = MailContext + "<tr><td align=right>刷卡日期：</td><td>" + dr["clogdate"] + "　" + int.Parse(dr["clogtime"].ToString()).ToString("00:00") + "</td></tr>";
            }
            MailContext = MailContext + "<tr><td align=right>刷卡異常原因：</td><td>" + chkitem + "</td></tr>";
            MailContext = MailContext + "</table>";
            MailContext = MailContext + "</body></HTML>";

            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                string sql = "select enemail from viewemprole where rid in (" + dr["rolestampid"] + ") and empstatus <> '4' and enemail<>''";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr2 = cmd.ExecuteReader();
                    string tomail = "";
                    while (dr.Read())
                    {
                        tomail += dr["enemail"] + ",";
                    }
                    dbobj.send_mailfile("", tomail, mailtitle, MailContext, null, null);
                    dr2.Close();
                }
            }
            #endregion

        }
        #endregion

        #region 刷卡作業 > 刷卡異常單審核
        public ActionResult cardlogchkEdit(cardlog chks, string sysflag, int? page, string orderdata, string orderdata1, HttpPostedFileBase logopic1)
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

            string qclogstatus = "", qclogsdate = "", qclogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qclogstatus"]))
            {
                qclogstatus = Request["qclogstatus"].Trim();
                ViewBag.qclogstatus = qclogstatus;
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

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    //var data = con.cardlog.Where(r => r.clogid == chks.clogid).FirstOrDefault();
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
                    NDcommon dbobj = new NDcommon();
                    cardlog col = new cardlog();
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        col = con.cardlog.Find(chks.clogid);
                    }

                    string sysnote = "刷卡異常單退回作業";

                    if(dbobj.get_dbnull2(Request["clogstatus"]) == "1")
                    {
                        string tmprolestampid = col.rolestampid;
                        string rolea_1 = col.rolestampidall;
                        string roleall = rolea_1 + "," + tmprolestampid; //'簽核過角色(多個)
                        string billflowid = col.billflowid.ToString();

                        //找出下一個角色是誰
                        string tmprole = dbobj.getnewcheck1("Z", tmprolestampid, roleall, "0", "", billflowid);

                        if(tmprole == "'topman'")
                        {
                            tmprole = "";
                        }
                        string clogstatus = "";
                        if (tmprole == "")
                        {
                            clogstatus = "1";// '己簽核
                        }
                        else
                        {
                            clogstatus = "0";
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
                                clogstatus = "1"; // '己簽核
                            }
                            //'==========================
                        }

                        col.clogstatus = clogstatus;
                        col.rolestampid = tmprole;
                        col.rolestampidall = roleall;
                        col.empstampidall = col.empstampidall + ",'" + (string)Session["empid"] + "'"; //'所有人員帳號
                        col.bmodid = (string)Session["empid"];
                        col.bmoddate = DateTime.Now;
                        col.billtime = col.billtime + "," + DateTime.Now.ToString();

                        if (tmprole != "")
                        {
                            //寄信
                            cardlogchkEditMail(col, tmprole);
                        }
                        else
                        {
                            //沒有下一個承辦人  (己通過)
                            //資料通過後 搬移到cardreallog
                            cardlogchkEditMove(col);

                            //(退回)  寄信
                            cardlogchkEditMailPass(col);
                        }
                        sysnote = "刷卡異常單審核通過作業";
                    }
                    else
                    {
                       // string tmprolestampid = "'" + col.rolestampid + "'";
                       // string roleall = col.rolestampidall + "," + tmprolestampid;
                        col.cback = chks.cback;
                        col.clogstatus = "2"; // '下個呈核角色
                       // col.rolestampid = "1"; // '下個呈核角色
                       // col.rolestampidall = roleall;  // '所有呈核角色
                       // col.empstampidall = col.empstampidall + ",'" + (string)Session["empid"] + "'";  // '所有人員帳號
                        col.bmodid = (string)Session["empid"];
                        col.bmoddate = DateTime.Now;
                       // col.billtime = col.billtime + "," + DateTime.Now.ToString();

                        //資料通過後 搬移到cardreallog
                        //cardlogchkEditMove(col);

                        //(己通過)  寄信
                        cardlogchkEditMailBack(col);
                        sysnote = "刷卡異常單退回作業";
                    }

                    
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.Entry(col).State = EntityState.Modified;
                        con.SaveChanges();
                    }

                    //系統LOG檔
                   
                    if (sysnote.Length > 4000) { sysnote = sysnote.Substring(0, 4000); }
                    ////================================================= //

                    SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    string sysrealsid = Request["sysrealsid"].ToString();
                    string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    sysconn.Close();
                    sysconn.Dispose();
                    //=================================================

                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/cardlog/cardlogchkList' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

                    tmpform += "<input type=hidden id='qclogstatus' name='qclogstatus' value='" + qclogstatus + "'>";
                    tmpform += "<input type=hidden id='qclogsdate' name='qclogsdate' value='" + qclogsdate + "'>";
                    tmpform += "<input type=hidden id='qclogedate' name='qclogedate' value='" + qclogedate + "'>";

                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"" + tmpform };
                }
            }

        }

        private void cardlogchkEditMailBack(cardlog col)
        {
            NDcommon dbobj = new NDcommon();
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                string fromadd = "", fromaddname = "", chkitem = "", toadd = "";
                fromadd = dbobj.get_dbvalue(conn, "select enemail from employee where empid='" + Request["empid"] + "'");
                fromaddname = dbobj.get_dbvalue(conn, "select empname from employee where empid='" + Request["empid"] + "'");
                chkitem = dbobj.get_dbvalue(conn, "select chkitem from checkcode where chkcode='" + col.clogreason + "' and chkclass='66'");
                toadd = dbobj.get_dbvalue(conn, "select enemail from employee where empid='" + col.empid + "'");

                #region 寄送mail給申請人
                string mailtitle = "", MailContext = "";
                mailtitle = "刷卡異常單退回";
                MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body>";
                MailContext = MailContext + "以下為明細資料：<BR>";
                MailContext = MailContext + "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                MailContext = MailContext + "<tr><td align=right width=130>申請人：</td><td>" + col.empname + "</td></tr>";
                if (dbobj.get_dbnull2(col.clogtype) == "3")
                {
                    MailContext = MailContext + "<tr><td align=right>刷卡日期：</td><td>" + col.clogdate + "　" + int.Parse(col.clogtime.ToString()).ToString("00:00") + "-" + int.Parse(col.clogtime1.ToString()).ToString("00:00") + "</td></tr>";
                }
                else
                {
                    MailContext = MailContext + "<tr><td align=right>刷卡日期：</td><td>" + col.clogdate + "　" + int.Parse(col.clogtime.ToString()).ToString("00:00") + "</td></tr>";
                }
                MailContext = MailContext + "<tr><td align=right>刷卡異常原因：</td><td>" + chkitem + "</td></tr>";
                MailContext = MailContext + "</table>";
                MailContext = MailContext + "</body></HTML>";

                dbobj.send_mailfile("", toadd, mailtitle, MailContext, null, null);

                #endregion
            }
        }

        private void cardlogchkEditMailPass(cardlog col)
        {
            NDcommon dbobj = new NDcommon();
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                string fromadd = "", fromaddname = "", chkitem = "", toadd = "";
                fromadd = dbobj.get_dbvalue(conn, "select enemail from employee where empid='" + Request["empid"] + "'");
                fromaddname = dbobj.get_dbvalue(conn, "select empname from employee where empid='" + Request["empid"] + "'");
                chkitem = dbobj.get_dbvalue(conn, "select chkitem from checkcode where chkcode='" + col.clogreason + "' and chkclass='66'");
                toadd = dbobj.get_dbvalue(conn, "select enemail from employee where empid='" + col.empid + "'");

                #region 寄送mail給申請人
                string mailtitle = "", MailContext = "";
                mailtitle = "刷卡異常單已通過核准";
                MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body>";
                MailContext = MailContext + "以下為明細資料：<BR>";
                MailContext = MailContext + "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                MailContext = MailContext + "<tr><td align=right width=130>申請人：</td><td>" + col.empname + "</td></tr>";
                if (dbobj.get_dbnull2(col.clogtype) == "3")
                {
                    MailContext = MailContext + "<tr><td align=right>刷卡日期：</td><td>" + col.clogdate + "　" + int.Parse(col.clogtime.ToString()).ToString("00:00") + "-" + int.Parse(col.clogtime1.ToString()).ToString("00:00") + "</td></tr>";
                }
                else
                {
                    MailContext = MailContext + "<tr><td align=right>刷卡日期：</td><td>" + col.clogdate + "　" + int.Parse(col.clogtime.ToString()).ToString("00:00") + "</td></tr>";
                }
                MailContext = MailContext + "<tr><td align=right>刷卡異常原因：</td><td>" + chkitem + "</td></tr>";
                MailContext = MailContext + "</table>";
                MailContext = MailContext + "</body></HTML>";

                dbobj.send_mailfile("", toadd, mailtitle, MailContext, null, null);

                #endregion
            }
        }

        private void cardlogchkEditMove(cardlog col)
        {
            string clogreason = col.clogreason;
            //01:忘記刷卡,02:忘記帶卡,03:卡片遺失,04:其他
            if((clogreason == "01") || (clogreason == "02") || (clogreason == "04"))
            { }
            else
            {
                return;
            }
            string clogtype = col.clogtype;
            //1:上班, 2:下班, 3:上下班
            int k = 1;
            if(clogtype == "3")
            {
                k = 2;
            }

            cardreallog chks = new cardreallog();
            for (int i=0; i<k; i++)
            {
                chks.empid = col.empid;
                chks.empname = col.empname;
                chks.dptid = col.dptid;
                chks.clogdate = col.clogdate;
                if (i == 0)
                {
                    chks.clogtime = col.clogtime + "00";
                }
                else
                {
                    chks.clogtime = col.clogtime1 + "00";
                }
                chks.clogcomment = col.clogcomment;
                chks.comid = (string)Session["comid"];
                chks.bmodid = (string)Session["empid"];
                chks.bmoddate = DateTime.Now;

                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    con.cardreallog.Add(chks);
                    con.SaveChanges();
                }
            }
        }

        private void cardlogchkEditMail(cardlog col, string tmprole)
        {
            NDcommon dbobj = new NDcommon();
            #region 寄給下一個承辦人
            string mailtitle = "", MailContext = "";
            mailtitle = "【" + col.empname + "】人員刷卡異常知會通知信";
            MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body>";
            MailContext = MailContext + "以下為明細資料：<BR>";
            MailContext = MailContext + "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
            MailContext = MailContext + "<tr><td align=right width=130>申請人：</td><td>" + col.empname + "</td></tr>";
            if (dbobj.get_dbnull2(col.clogtype) == "3")
            {
                MailContext = MailContext + "<tr><td align=right>刷卡日期：</td><td>" + col.clogdate + "　" + int.Parse(col.clogtime.ToString()).ToString("00:00") + "-" + int.Parse(col.clogtime1.ToString()).ToString("00:00") + "</td></tr>";
            }
            else
            {
                MailContext = MailContext + "<tr><td align=right>刷卡日期：</td><td>" + col.clogdate + "　" + int.Parse(col.clogtime.ToString()).ToString("00:00") + "</td></tr>";
            }
            string chkitem = "";

            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                chkitem = dbobj.get_dbvalue(conn, "select chkitem from checkcode where chkcode='" + col.clogreason + "' and chkclass='66'");

                MailContext = MailContext + "<tr><td align=right>刷卡異常原因：</td><td>" + chkitem + "</td></tr>";
                MailContext = MailContext + "</table>";
                MailContext = MailContext + "</body></HTML>";

                tmprole = tmprole.Replace("'","");
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

        public ActionResult cardlogchkList(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "clogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qclogstatus = "", qclogsdate = "", qclogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qclogstatus"]))
            {
                qclogstatus = Request["qclogstatus"].Trim();
                ViewBag.qclogstatus = qclogstatus;
            }

            NDcommon dbobj = new NDcommon();

            qclogsdate = Request["qclogsdate"];
            ViewBag.qclogsdate = qclogsdate;
            qclogedate = Request["qclogedate"];
            ViewBag.qclogedate = qclogedate;
           

            IPagedList<cardlog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {


                //多個角色時
                string tmpplay = (string)Session["mplayrole"];
                tmpplay = tmpplay.Replace("'", "");
                string[] tmpa = tmpplay.Split(',');
                string sql_1 = "";
                foreach (string s in tmpa)
                {
                    sql_1 += "'''"+s+"''',";
                }
                sql_1 = sql_1.Substring(0, sql_1.Length - 1);
                //====


                string sqlstr = "SELECT * FROM cardlog where clogstatus in ('0')  ";
 //               + " and comid='" + (string)Session["comid"] + "'";
                if (sql_1 != "")
                {
                    sqlstr += " and rolestampid in (" + sql_1 + ")";
                }

                if (qclogstatus != "")
                {
                    sqlstr += " and clogstatus = '" + qclogstatus + "'";
                }
               
                if (!string.IsNullOrEmpty(qclogsdate) && !string.IsNullOrEmpty(qclogedate))
                {
                    sqlstr += " and clogdate >= '" + qclogsdate + "' and clogdate <= '" + qclogedate + "'";
                }

                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.cardlog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<cardlog>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch2(orderdata, orderdata1);
            return View(result);
        }
        private string SetOrder_ch2(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "clogstatus", "clogreason", "clogdate" };
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

        #region 刷卡作業 > 刷卡異常單明細
        public ActionResult cardlogqryEdit(cardlog chks, string sysflag, int? page, string orderdata, string orderdata1, HttpPostedFileBase logopic1)
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

            string qclogstatus = "", qclogsdate = "", qclogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qclogstatus"]))
            {
                qclogstatus = Request["qclogstatus"].Trim();
                ViewBag.qclogstatus = qclogstatus;
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

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    //var data = con.cardlog.Where(r => r.clogid == chks.clogid).FirstOrDefault();
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
                    //NDcommon dbobj = new NDcommon();



                    //chks.bmodid = Session["tempid"].ToString();
                    //chks.bmoddate = DateTime.Now;

                    //using (Aitag_DBContext con = new Aitag_DBContext())
                    //{
                    //    con.Entry(chks).State = EntityState.Modified;
                    //    con.SaveChanges();
                    //}

                    //系統LOG檔
                    //string sysnote = "";
                    //if (sysnote.Length > 4000) { sysnote = sysnote.Substring(0, 4000); }
                    ////================================================= //

                    //SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    //string sysrealsid = Request["sysrealsid"].ToString();
                    //string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    //dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    //sysconn.Close();
                    //sysconn.Dispose();
                    //=================================================

                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/cardlog/cardlogqryList' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

                    tmpform += "<input type=hidden id='qclogstatus' name='qclogstatus' value='" + qclogstatus + "'>";
                    tmpform += "<input type=hidden id='qclogsdate' name='qclogsdate' value='" + qclogsdate + "'>";
                    tmpform += "<input type=hidden id='qclogedate' name='qclogedate' value='" + qclogedate + "'>";

                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"" + tmpform };
                }
            }

        }

        public ActionResult cardlogqryList(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "clogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qclogstatus = "", qclogsdate = "", qclogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qclogstatus"]))
            {
                qclogstatus = Request["qclogstatus"].Trim();
                ViewBag.qclogstatus = qclogstatus;
            }

            NDcommon dbobj = new NDcommon();

            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qclogsdate"], "m", "min", @"請假起格式錯誤!!\n", out qclogsdate, out DateEx);
            ViewBag.qclogsdate = qclogsdate;
            dbobj.get_dateRang(Request["qclogedate"], "m", "max", @"請假訖格式錯誤!!\n", out qclogedate, out DateEx1);
            ViewBag.qclogedate = qclogedate;
            DateEx += DateEx1;
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }


            IPagedList<cardlog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "SELECT * FROM cardlog where empid='" + (string)Session["empid"] + "' and comid = '" + Session["comid"] + "'";

                if (qclogstatus != "" && qclogstatus != "all")
                {
                    sqlstr += " and clogstatus = '" + qclogstatus + "'";
                }
                if (qclogsdate != "" && qclogedate != "")
                {
                    sqlstr += " and clogdate >= '" + qclogsdate + "' and clogdate <= '" + qclogedate + "'";
                }

                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.cardlog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<cardlog>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch3(orderdata, orderdata1);
            return View(result);
        }
        private string SetOrder_ch3(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "clogstatus", "clogreason", "clogdate" };
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

        #region 刷卡作業 > 每日刷卡明細
        public ActionResult cardreallogqryList(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "crid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qclogsdate = "", qclogedate = "";

            NDcommon dbobj = new NDcommon();

            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qclogsdate"], "m", "min", @"刷卡日期起格式錯誤!!\n", out qclogsdate, out DateEx);
            ViewBag.qclogsdate = qclogsdate;
            dbobj.get_dateRang(Request["qclogedate"], "m", "max", @"刷卡日期訖格式錯誤!!\n", out qclogedate, out DateEx1);
            ViewBag.qclogedate = qclogedate;
            DateEx += DateEx1;
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }




            IPagedList<cardreallog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "SELECT * FROM cardreallog where empid = '" + (string)Session["empid"] + "'";
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
            ViewBag.SetOrder_ch = SetOrder_ch4(orderdata, orderdata1);
            return View(result);
        }
        private string SetOrder_ch4(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "dptid", "empid" };
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
        #endregion

        #region 刷卡作業 > 差勤狀態查詢
        public ActionResult cardabnormalqryList(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "clogdate,dptid,empid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qclogsdate = "", qclogedate = "";
            

            NDcommon dbobj = new NDcommon();

            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qclogsdate"], "y", "min", @"請假起格式錯誤!!\n", out qclogsdate, out DateEx);
            ViewBag.qclogsdate = qclogsdate;
            dbobj.get_dateRang(Request["qclogedate"], "y", "max", @"請假訖格式錯誤!!\n", out qclogedate, out DateEx1);
            ViewBag.qclogedate = qclogedate;
            DateEx += DateEx1;

            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }




            IPagedList<cardjudgelog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from cardjudgelog where comid='" + (string)Session["comid"] + "'  and empid='" + (string)Session["empid"] + "'";
                if (qclogsdate != "")
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
        #endregion

        #region 刷卡作業 > 每月刷卡異動確認單
        public ActionResult csvcardabnormallog()
        {
            string qempid = "", qSdate = "", qEdate = "", qcchkstatus = "";
            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {
                qempid = Request["qempid"].Trim();
                ViewBag.qempid = qempid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcchkstatus"]))
            {
                qcchkstatus = Request["qcchkstatus"].Trim();
                ViewBag.qcchkstatus = qcchkstatus;
            }

            NDcommon dbobj = new NDcommon();

            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qSdate"], "y", "min", @"日期起格式錯誤!!\n", out qSdate, out DateEx);
            ViewBag.qSdate = qSdate;
            dbobj.get_dateRang(Request["qEdate"], "y", "max", @"日期訖格式錯誤!!\n", out qEdate, out DateEx1);
            ViewBag.qEdate = qEdate;
            DateEx += DateEx1;

            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }


            string Excel = "", Excel2 = "";
            string sqlstr = "";


            sqlstr = "SELECT * FROM cardjudgelog where comid='" + (string)Session["comid"] + "'  and";

            if (qempid != "")
            {
                sqlstr += " empid = '" + qempid + "'  and";
            }
            if (qSdate != "")
            {
                sqlstr += " clogdate >= '" + qSdate + "'  and";
            }
            if (qEdate != "")
            {
                sqlstr += " clogdate <= '" + qEdate + "'  and";
            }
            if (qcchkstatus != "")
            {
                sqlstr += " cchkstatus = '" + qcchkstatus + "'  and";
            }

            sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
            sqlstr += " order by clogdate";


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
        public ActionResult csvmonthcard()
        {
            string qempid = "", qSdate = "", qEdate = "", qcchkstatus = "";
            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {
                qempid = Request["qempid"].Trim();
                ViewBag.qempid = qempid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcchkstatus"]))
            {
                qcchkstatus = Request["qcchkstatus"].Trim();
                ViewBag.qcchkstatus = qcchkstatus;
            }

            NDcommon dbobj = new NDcommon();

            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qSdate"], "y", "min", @"日期起格式錯誤!!\n", out qSdate, out DateEx);
            ViewBag.qSdate = qSdate;
            dbobj.get_dateRang(Request["qEdate"], "y", "max", @"日期訖格式錯誤!!\n", out qEdate, out DateEx1);
            ViewBag.qEdate = qEdate;
            DateEx += DateEx1;

            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }

            string sql = "select cchkstatus,";
            sql += " (select dpttitle from department where dptid = cj.dptid)as dpttitle,";
            sql += " empid";
            sql += " from cardjudgelog as cj";
            string sqlwhere = " where 1=1";

            if (qempid != "")
            {
                sqlwhere += " and empid = '" + qempid + "'";
            }
            if (qSdate != "")
            {
                sqlwhere += " and clogdate >= '" + qSdate + "'";
            }
            if (qEdate != "")
            {
                sqlwhere += " and clogdate <= '" + qEdate + "'";
            }
            sql += sqlwhere;
            sql += " group by cchkstatus, dptid, empid";


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

            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                System.Data.SqlClient.SqlConnection comconn = dbobj.get_conn("Aitag_DBContext");
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
                        string allhour = "select sum(cloghour) as allhour from cardjudgelog " + sqlwhere + " and cchkstatus = '" + dr["cchkstatus"] + "'";
                        allhour = dbobj.get_dbvalue(comconn, allhour);


                        Excel2 += "<tr>";
                        Excel2 += "<td>" + cchkstatus.Trim() + "</td>";
                        Excel2 += "<td>" + qSdate + "~" + qEdate +"</td>";
                        Excel2 += "<td>" + dbobj.get_dbnull2(dr["dpttitle"]) + "</td>";
                        Excel2 += "<td>" + dbobj.get_dbnull2(dr["empid"]) + "</td>";
                        Excel2 += "<td>" + allhour + "</td>";
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
                comconn.Close();
                comconn.Dispose();
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
            DateTime dt = DateTime.Now;

            string qempid = "", qSdate = "", qEdate = "", qcchkstatus = "";
            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {
                qempid = Request["qempid"].Trim();
                ViewBag.qempid = qempid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcchkstatus"]))
            {
                qcchkstatus = Request["qcchkstatus"].Trim();
                ViewBag.qcchkstatus = qcchkstatus;
            }

            NDcommon dbobj = new NDcommon();

            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qSdate"], "ACD", "min", @"日期起格式錯誤!!\n", out qSdate, out DateEx);
            ViewBag.qSdate = qSdate;
            dbobj.get_dateRang(Request["qEdate"], "ACD", "max", @"日期訖格式錯誤!!\n", out qEdate, out DateEx1);
            ViewBag.qEdate = qEdate;
            DateEx += DateEx1;

            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }


            IPagedList<cardjudgelog> result;

            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "SELECT top 1 *  FROM cardjudgelog where comid='" + (string)Session["comid"] + "' ";
                var query = con.cardjudgelog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<cardjudgelog>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
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

            string qempid = "", qSdate = "", qEdate = "", qcchkstatus = "";
            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {
                qempid = Request["qempid"].Trim();
                ViewBag.qempid = qempid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qSdate"]))
            {
                qSdate = Request["qSdate"].Trim();
                ViewBag.qSdate = qSdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qEdate"]))
            {
                qEdate = Request["qEdate"].Trim();
                ViewBag.qEdate = qEdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcchkstatus"]))
            {
                qcchkstatus = Request["qcchkstatus"].Trim();
                ViewBag.qcchkstatus = qcchkstatus;
            }

            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {
                qempid = Request["qempid"].Trim();
                ViewBag.qempid = qempid;
            }






            if (!string.IsNullOrWhiteSpace(Request["qcchkstatus"]))
            {
                qcchkstatus = Request["qcchkstatus"].Trim();
                ViewBag.qcchkstatus = qcchkstatus;
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
                if (qSdate != "")
                {
                    sqlstr += " clogdate >= '" + qSdate + "'  and";
                }
                if (qEdate != "")
                {
                    sqlstr += " clogdate <= '" + qEdate + "'  and";
                }
                if (qcchkstatus != "")
                {
                    sqlstr += " cchkstatus = '" + qcchkstatus + "'  and";
                }


                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by clogdate";

                var query = con.cardjudgelog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<cardjudgelog>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }
        public ActionResult monthcardsave(int? page, string orderdata, string orderdata1)
        {
            string cchkstatus = "";
            if (!string.IsNullOrWhiteSpace(Request["cchkstatus"]))
            {
                cchkstatus = Request["cchkstatus"].Trim();
            }

            string qempid = "", qSdate = "", qEdate = "";
            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {
                qempid = Request["qempid"].Trim();
                ViewBag.qempid = qempid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qSdate"]))
            {
                qSdate = Request["qSdate"].Trim();
                ViewBag.qSdate = qSdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qEdate"]))
            {
                qEdate = Request["qEdate"].Trim();
                ViewBag.qEdate = qEdate;
            }


            //save................
            string strcjid = Request["cjid"].ToString();
            string[] arrcjid = strcjid.Split(',');
            string today = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");

            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            foreach (var cjid in arrcjid)
            {
                string clogstime = Request["clogstime" + cjid].ToString().Replace(":", "");
                string clogetime = Request["clogetime" + cjid].ToString().Replace(":", "");
                string cchkcomment = Request["cchkcomment" + cjid];
                double cloghour = 0;
                if (clogstime != "" && clogetime != "")
                {
                    double hours = Convert.ToDouble("0" + clogetime.Substring(0, 2)) - Convert.ToDouble("0" + clogstime.Substring(0, 2));
                    double mints = Convert.ToDouble("0" + clogetime.Substring(2, 2)) - Convert.ToDouble("0" + clogstime.Substring(2, 2));
                    cloghour = hours + mints / 60 - 1.5;
                }


                if (cloghour >= 0 || cchkcomment != "")
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
                        sql += "cchkstatus = '2', ";
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
            

            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/cardlog/monthcardlist' method='post'>";
            tmpform += "<input type=hidden id='qempid' name='qempid' value='" + qempid + "'>";
            tmpform += "<input type=hidden id='qSdate' name='qSdate' value='" + qSdate + "'>";
            tmpform += "<input type=hidden id='qEdate' name='qEdate' value='" + qEdate + "'>";
            tmpform += "<input type=hidden id='qcchkstatus' name='qcchkstatus' value='" + cchkstatus + "'>";
            tmpform += "</form>";
            tmpform += "</body>";
            return new ContentResult() { Content = @"" + tmpform };
        }


        #endregion

        #region 管理作業 > 刷卡異常單明細
        public ActionResult admcardlogList(int? page, string orderdata, string orderdata1)
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
            dbobj.get_dateRang(Request["qhclogsdate"], "m", "min", @"刷卡異常起格式錯誤!!\n", out qhclogsdate, out DateEx);
            ViewBag.qhclogsdate = qhclogsdate;
            dbobj.get_dateRang(Request["qhclogedate"], "m", "max", @"刷卡異常迄格式錯誤!!\n", out qhclogedate, out DateEx1);
            ViewBag.qhclogedate = qhclogedate;
            DateEx += DateEx1;
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }


            IPagedList<cardlog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {

                string sqlstr = "select * from cardlog  where  1 = 1";

                System.Collections.Generic.IEnumerable<cardlog> query;

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
                query = con.cardlog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<cardlog>(page.Value - 1, (int)Session["pagesize"]);

            }

            ViewBag.SetOrder_ch = SetOrder_ch4(orderdata, orderdata1);
            return View(result);
        }

        #endregion

        #region 管理作業 > 刷卡異常單觀看
        public ActionResult admcardlogEdit(cardlog chks, string sysflag, int? page, string orderdata, string orderdata1)
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
                        tmpform += "<form name='qfr1' action='/cardlog/admcardlogList' method='post'>";
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
        #endregion

        #region 管理作業 > 刷卡異常單報表
        public ActionResult admcardlogrpt(int? page, string orderdata, string orderdata1)
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
            dbobj.get_dateRang(Request["qhclogsdate"], "m", "min", @"刷卡異常起格式錯誤!!\n", out qhclogsdate, out DateEx);
            ViewBag.qhclogsdate = qhclogsdate;
            dbobj.get_dateRang(Request["qhclogedate"], "m", "max", @"刷卡異常迄格式錯誤!!\n", out qhclogedate, out DateEx1);
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
        #endregion
    }
}
