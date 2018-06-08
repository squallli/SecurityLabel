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
    public class delholidayController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /delholidaylog/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }


        #region  銷假作業 > 銷假單申請
        public ActionResult logadd(delholidaylog col, string sysflag, int? page, string orderdata, string orderdata1, HttpPostedFileBase logopic1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "hdellogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;


            if (sysflag != "A")
            {
                delholidaylog newcol = new delholidaylog();
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
                    string impallstring = dbobj.getnewcheck1("H", tmparolestampid, tmparolestampid, Request["hloghour"], "", "");
                    tmprole = impallstring.Split(';')[0].ToString();
                    tmpbillid = impallstring.Split(';')[1].ToString();
                    string errmsg = "";
                    if (tmprole == "")
                    {
                        errmsg = "請先至表單流程設定中設定首長信箱的呈核流程!";
                        ViewBag.errmsg = "<script>alert('" + errmsg + "');</script>";
                        return View(col);
                    }
                    //簽核
                    //if ((string)Session["mplayrole"] == "")
                    //{
                    //    errmsg = "您並未設定呈核角色!";
                    //    ViewBag.errmsg = "<script>alert('" + errmsg + "');</script>";
                    //    return View(col);
                    //}


                    //'找單據編號(自動產生編號)
                    string tmpbsno = "select hdno from delholidaylog where year(cdate) = " + DateTime.Now.Year + " and month(cdate) = " + DateTime.Now.Month + "  and hdno is not null order by hdno desc";
                    using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        tmpbsno = dbobj.get_dbvalue(conn, tmpbsno);
                    }

                    if (tmpbsno != "")
                    {
                        tmpbsno = tmpbsno.Substring(tmpbsno.Length - 3, 3);
                        tmpbsno = (int.Parse(tmpbsno) + 1).ToString("000");
                    }
                    else
                    {
                        tmpbsno = "001";
                    }
                    string tmpyear = (DateTime.Now.Year - 1911).ToString();
                    string tmpmonth = DateTime.Now.Month.ToString("00");
                    tmpbsno = "A" + tmpyear + tmpmonth + tmpbsno;
                    //======


                    col.hdellogstatus = "0";
                    col.hdno = tmpbsno;


                    //呈核人員
                    //======
                    if (dbobj.get_dbnull2(col.arolestampid) == "")
                    {
                        col.arolestampid = Request["arolestampid1"];
                    }
                    col.rolestampid = tmprole; //'下個呈核角色
                    col.rolestampidall = tmparolestampid; //'所有呈核角色
                    col.empstampidall = "'" + Request["empid"] + "'"; //'所有人員帳號
                    col.billflowid = int.Parse(tmpbillid);
                    //======

                    col.comid = (string)Session["comid"];
                    col.bmodid = (string)Session["empid"];
                    col.bmoddate = DateTime.Now;
                    col.deldate = DateTime.Now;
                    col.billtime = DateTime.Now.ToString();

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.delholidaylog.Add(col);
                        con.SaveChanges();
                    }



                    //    '寄信
                    //'======================
                    using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        string fromadd = "", fromaddname = "", mailtitle = "", MailContext = "";
                        fromadd = dbobj.get_dbvalue(conn, "select enemail from employee where empid='" + Request["empid"] + "'");
                        fromaddname = dbobj.get_dbvalue(conn, "select empname from employee where empid='" + Request["empid"] + "'");

                        //'寄送mail給下一個審核角色
                        #region 寄送mail給下一個審核角
                        mailtitle = "銷假單資料要求簽核通知";
                        MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body>";
                        MailContext = MailContext + "以下為明細資料：<BR>";
                        MailContext = MailContext + "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                        MailContext = MailContext + "<tr><td align=right width=130>申請單號：</td><td>" + tmpbsno + "</td></tr>";
                        MailContext = MailContext + "<tr><td align=right width=130>請假單號：</td><td>" + col.hsno + "</td></tr>";
                        MailContext = MailContext + "<tr><td align=right width=130>申請人：</td><td>" + col.empname + "</td></tr>";
                        MailContext = MailContext + "<tr><td align=right width=130>假別：</td><td>" + Request["hdaytitle"] + "</td></tr>";

                        string hlogsdate = dbobj.get_dbDate(col.hlogsdate, "yyyy/MM/dd");
                        string hlogedate = dbobj.get_dbDate(col.hlogedate, "yyyy/MM/dd");
                        string SEDate = "自{0} ({1})<br>至{2} ({3})";
                        SEDate = string.Format(SEDate, hlogsdate, col.hlogstime, hlogedate, col.hlogetime);
                        MailContext = MailContext + "<tr><td align=right width=130>日期起訖：</td><td>" + SEDate + "</td></tr>";

                        if (dbobj.get_dbnull2(col.hlogcomment) != "")
                        {
                            MailContext = MailContext + "<tr><td align=right width=130>備註：</td><td>" + col.hlogcomment.ToString().Trim().Replace(Environment.NewLine, "<br>") + "</td></tr>";
                        }
                        else
                        {
                            MailContext = MailContext + "<tr><td align=right width=130>備註：</td><td>&nbsp;</td></tr>";
                        }


                        MailContext = MailContext + "</table>";
                        MailContext = MailContext + "</body></HTML>";

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
                        #endregion


                    }


                    //系統LOG檔
                    string sysnote = "申請人：{0}<br>申請單號：{1}的資料";
                    sysnote = string.Format(sysnote, Request["empid"], tmpbsno);
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
                    tmpform += "<form name='qfr1' action='/delholiday/logcheckList' method='post'>";
                    //tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    //tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    //tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    //tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";
                    return new ContentResult() { Content = @"" + tmpform };
                }
            }
        }
        #endregion

        #region 銷假作業 > 銷假單審核

        public ActionResult logcheckEdit(delholidaylog chks, string sysflag, int? page, string orderdata, string orderdata1, HttpPostedFileBase logopic1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "hdellogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qhlogsdate = "", qhlogedate = "";
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

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    //var data = con.delholidaylog.Where(r => r.hdellogid == chks.hdellogid).FirstOrDefault();
                    delholidaylog edelholidaylogs = con.delholidaylog.Find(chks.hdellogid);
                    if (edelholidaylogs == null)
                    {
                        return HttpNotFound();
                    }
                    return View(edelholidaylogs);
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
                    delholidaylog col = new delholidaylog();
                    string sysnote = "";
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        col = con.delholidaylog.Find(chks.hdellogid);
                    }

                    string hdellogstatus = "";
                    if (dbobj.get_dbnull2(Request["hdellogstatus"]) == "1")
                    {
                        string tmprolestampid = col.rolestampid;
                        string rolea_1 = col.rolestampidall;
                        string roleall = rolea_1 + "," + tmprolestampid; //'簽核過角色(多個)
                        string billflowid = col.billflowid.ToString();

                        //找出下一個角色是誰
                        string tmprole = dbobj.getnewcheck1("H", tmprolestampid, roleall, "0", "", billflowid);

                        if (tmprole == "'topman'")
                        {
                            tmprole = "";
                        }
                        if (tmprole == "")
                        {
                            hdellogstatus = "1";// '己簽核
                        }
                        else
                        {
                            hdellogstatus = "0";
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
                                hdellogstatus = "1"; // '己簽核
                            }
                            //'==========================
                        }

                        col.hdellogstatus = hdellogstatus;
                        col.rolestampid = tmprole;
                        col.rolestampidall = roleall;
                        col.empstampidall = col.empstampidall + ",'" + (string)Session["empid"] + "'"; //'所有人員帳號
                        col.bmodid = (string)Session["empid"];
                        col.bmoddate = DateTime.Now;
                        col.billtime = col.billtime + "," + DateTime.Now.ToString();

                        if (tmprole != "")
                        {
                            //寄信
                            logcheckEditMail(col, tmprole);
                        }
                        else
                        {
                            
                            //(己通過)  寄信
                            logcheckEditMailPass(col);
                        }
                    }
                    else
                    {

                        col.hdellogstatus = "2";
                        col.delback = chks.delback;
                        col.bmodid = (string)Session["empid"];
                        col.bmoddate = DateTime.Now;
                      //  col.billtime = col.billtime + "," + DateTime.Now.ToString();

                        //(己通過)  寄信
                        logcheckEditMailBack(col);
                    }

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.Entry(col).State = EntityState.Modified;
                        con.SaveChanges();
                    }


                    if (hdellogstatus == "1")
                    {
                        //銷假時數補回
                        delholidayMode(col, hdellogstatus);
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
                    tmpform += "<form name='qfr1' action='/delholiday/logcheckList' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

                    tmpform += "<input type=hidden id='qhlogsdate' name='qhlogsdate' value='" + qhlogsdate + "'>";
                    tmpform += "<input type=hidden id='qhlogedate' name='qhlogedate' value='" + qhlogedate + "'>";

                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"" + tmpform };
                }
            }
        }


        private void delholidayMode(delholidaylog col, string hdellogstatus)
        {
            NDcommon dbobj = new NDcommon();
            using (SqlConnection comconn = dbobj.get_conn("Aitag_DBContext"))
            {
                if (col.hdayid == "A04")
                {
                    #region  '請補假部分
                    resthourlog chks = new resthourlog();
                    chks.empid = col.empid;
                    chks.rsdeaddate = col.hlogsdate;

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        var eresthourlogs = con.resthourlog.Where(r => r.empid == chks.empid && r.rsdeaddate >= chks.rsdeaddate).OrderBy(r => r.rsdeaddate);
                        float tmphloghour = float.Parse(col.hloghour.ToString());
                        foreach (resthourlog es in eresthourlogs)
                        {
                            var lefthour = es.usehour - tmphloghour;
                            if (lefthour >= 0)
                            {
                                es.usehour -= tmphloghour;
                                using (Aitag_DBContext con2 = new Aitag_DBContext())
                                {
                                    con2.Entry(es).State = EntityState.Modified;
                                    con2.SaveChanges();
                                }
                                if (lefthour == 0)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                es.usehour = 0;
                                using (Aitag_DBContext con2 = new Aitag_DBContext())
                                {
                                    con2.Entry(es).State = EntityState.Modified;
                                    con2.SaveChanges();
                                }
                                tmphloghour = float.Parse(Math.Abs(decimal.Parse(lefthour.ToString())).ToString());
                            }
                            
                        }
                        
                    }
                    #endregion
                }
                else
                {
                    #region  事實發生假部分
                    string mergehdayid = dbobj.get_dbvalue(comconn, "select * from holidaycode where hdayid = '" + col.hdayid + "'"); ;
                    string sql = "";
                    emphdlog chks = new emphdlog();


                    if (mergehdayid != "")
                    {
                        chks.empid = col.empid;
                        chks.hdayid = mergehdayid;
                    }
                    else
                    {
                        chks.empid = col.empid;
                        chks.hdayid = col.hdayid;
                    }
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        emphdlog eemphdlogs = con.emphdlog.Where(r => r.empid == chks.empid && r.hdayid == chks.hdayid).FirstOrDefault();

                        if (eemphdlogs != null)
                        {
                            eemphdlogs.usehour = eemphdlogs.usehour - decimal.Parse(col.hloghour.ToString());
                            con.Entry(eemphdlogs).State = EntityState.Modified;
                            con.SaveChanges();

                            if (eemphdlogs.usehour == 0)
                            {
                                if (mergehdayid != "")
                                {
                                    sql = "delete emphdlog where empid = '" + col.empid + "' and hdayid = '" + mergehdayid + "'";
                                }
                                else
                                {
                                    sql = "delete emphdlog where empid = '" + col.empid + "' and hdayid = '" + col.hdayid + "'";
                                }
                                dbobj.dbexecute("Aitag_DBContext", sql);
                            }
                        }
                    }
                    #endregion
                }
            }
        }

        private void logcheckEditMailPass(delholidaylog col)
        {
            NDcommon dbobj = new NDcommon();
            #region 寄送mail給申請人
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                string fromadd = "", fromaddname = "", toadd = "";
                fromadd = dbobj.get_dbvalue(conn, "select enemail from employee where empid='" + Request["empid"] + "'");
                fromaddname = dbobj.get_dbvalue(conn, "select empname from employee where empid='" + Request["empid"] + "'");
                toadd = dbobj.get_dbvalue(conn, "select enemail from employee where empid='" + col.empid + "'");
                string mailtitle = "", MailContext = "";
                mailtitle = "銷假單資料已通過核准";
                MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body>";
                MailContext = MailContext + "以下為明細資料：<BR>";
                MailContext = MailContext + "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                MailContext = MailContext + "<tr><td align=right width=130>請假單號：</td><td>" + col.hsno + "</td></tr>";
                MailContext = MailContext + "<tr><td align=right width=130>申請人：</td><td>" + col.empname + "</td></tr>";
                MailContext = MailContext + "<tr><td align=right width=130>假別：</td><td>" + Request["hdaytitle"] + "</td></tr>";

                string hlogsdate = dbobj.get_dbDate(col.hlogsdate, "yyyy/MM/dd");
                string hlogedate = dbobj.get_dbDate(col.hlogedate, "yyyy/MM/dd");
                string SEDate = "自{0} ({1})<br>至{2} ({3})";
                SEDate = string.Format(SEDate, hlogsdate, col.hlogstime, hlogedate, col.hlogetime);
                MailContext = MailContext + "<tr><td align=right width=130>日期起訖：</td><td>" + SEDate + "</td></tr>";
                MailContext = MailContext + "<tr><td align=right width=130>共計時數：</td><td>" + col.hloghour + "</td></tr>";

                if (dbobj.get_dbnull2(col.hlogcomment) != "")
                {
                    MailContext = MailContext + "<tr><td align=right width=130>備註：</td><td>" + col.hlogcomment.ToString().Trim().Replace(Environment.NewLine, "<br>") + "</td></tr>";
                }
                else
                {
                    MailContext = MailContext + "<tr><td align=right width=130>備註：</td><td>&nbsp;</td></tr>";
                }


                MailContext = MailContext + "</table>";
                MailContext = MailContext + "</body></HTML>";

                dbobj.send_mailfile("", toadd, mailtitle, MailContext, null, null);
            }
            #endregion
            
        }

        private void logcheckEditMailBack(delholidaylog col)
        {
            NDcommon dbobj = new NDcommon();
            #region 寄送mail給申請人
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                string fromadd = "", fromaddname = "", toadd = "";
                fromadd = dbobj.get_dbvalue(conn, "select enemail from employee where empid='" + Request["empid"] + "'");
                fromaddname = dbobj.get_dbvalue(conn, "select empname from employee where empid='" + Request["empid"] + "'");
                toadd = dbobj.get_dbvalue(conn, "select enemail from employee where empid='" + col.empid + "'");
                string mailtitle = "", MailContext = "";
                mailtitle = "銷假單資料退回";
                MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body>";
                MailContext = MailContext + "以下為明細資料：<BR>";
                MailContext = MailContext + "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                MailContext = MailContext + "<tr><td align=right width=130>請假單號：</td><td>" + col.hsno + "</td></tr>";
                MailContext = MailContext + "<tr><td align=right width=130>申請人：</td><td>" + col.empname + "</td></tr>";
                MailContext = MailContext + "<tr><td align=right width=130>假別：</td><td>" + Request["hdaytitle"] + "</td></tr>";

                string hlogsdate = dbobj.get_dbDate(col.hlogsdate, "yyyy/MM/dd");
                string hlogedate = dbobj.get_dbDate(col.hlogedate, "yyyy/MM/dd");
                string SEDate = "自{0} ({1})<br>至{2} ({3})";
                SEDate = string.Format(SEDate, hlogsdate, col.hlogstime, hlogedate, col.hlogetime);
                MailContext = MailContext + "<tr><td align=right width=130>日期起訖：</td><td>" + SEDate + "</td></tr>";
                MailContext = MailContext + "<tr><td align=right width=130>共計時數：</td><td>" + col.hloghour + "</td></tr>";

                if (dbobj.get_dbnull2(col.hlogcomment) != "")
                {
                    MailContext = MailContext + "<tr><td align=right width=130>備註：</td><td>" + col.hlogcomment.ToString().Trim().Replace(Environment.NewLine, "<br>") + "</td></tr>";
                }
                else
                {
                    MailContext = MailContext + "<tr><td align=right width=130>備註：</td><td>&nbsp;</td></tr>";
                }


                MailContext = MailContext + "</table>";
                MailContext = MailContext + "</body></HTML>";

                dbobj.send_mailfile("", toadd, mailtitle, MailContext, null, null);
            }
            #endregion

        }

        private void logcheckEditMail(delholidaylog col, string tmprole)
        {
            NDcommon dbobj = new NDcommon();
            #region 寄給下一個承辦人
            string mailtitle = "", MailContext = "";
            mailtitle = "【" + col.empname + "】出差單資料要求審核通知";
            MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body>";
            MailContext = MailContext + "以下為明細資料：<BR>";
            MailContext = MailContext + "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
            MailContext = MailContext + "<tr><td align=right width=130>請假單號：</td><td>" + col.hsno + "</td></tr>";
            MailContext = MailContext + "<tr><td align=right width=130>申請人：</td><td>" + col.empname + "</td></tr>";
            MailContext = MailContext + "<tr><td align=right width=130>假別：</td><td>" + Request["hdaytitle"] + "</td></tr>";

            string hlogsdate = dbobj.get_dbDate(col.hlogsdate, "yyyy/MM/dd");
            string hlogedate = dbobj.get_dbDate(col.hlogedate, "yyyy/MM/dd");
            string SEDate = "自{0} ({1})<br>至{2} ({3})";
            SEDate = string.Format(SEDate, hlogsdate, col.hlogstime, hlogedate, col.hlogetime);
            MailContext = MailContext + "<tr><td align=right width=130>日期起訖：</td><td>" + SEDate + "</td></tr>";
            MailContext = MailContext + "<tr><td align=right width=130>共計時數：</td><td>" + col.hloghour + "</td></tr>";

            if (dbobj.get_dbnull2(col.hlogcomment) != "")
            {
                MailContext = MailContext + "<tr><td align=right width=130>備註：</td><td>" + col.hlogcomment.ToString().Trim().Replace(Environment.NewLine, "<br>") + "</td></tr>";
            }
            else
            {
                MailContext = MailContext + "<tr><td align=right width=130>備註：</td><td>&nbsp;</td></tr>";
            }


            MailContext = MailContext + "</table>";
            MailContext = MailContext + "</body></HTML>";

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


        public ActionResult logcheckList(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "hdellogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qhlogsdate = "", qhlogedate = "";
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

            NDcommon dbobj = new NDcommon();

            string DateEx = "", DateEx1 = "";
            if (dbobj.get_dbnull2(Request["qhlogsdate"]) != "")
            {
                dbobj.get_dateRang(Request["qhlogsdate"], "m", "min", @"出差日期起格式錯誤!!\n", out qhlogsdate, out DateEx);
                ViewBag.qhlogsdate = qhlogsdate;
            }
            if (dbobj.get_dbnull2(Request["qhlogedate"]) != "")
            {
                dbobj.get_dateRang(Request["qhlogedate"], "m", "max", @"出差日期訖格式錯誤!!\n", out qhlogedate, out DateEx1);
                ViewBag.qhlogedate = qhlogedate;
            }
            DateEx += DateEx1;

            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }


            IPagedList<delholidaylog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                //多個角色時
                string tmpplay = (string)Session["mplayrole"];
                tmpplay = tmpplay.Replace("'", "");
                string[] tmpa = tmpplay.Split(',');
                string sql_1 = "";
                foreach (string s in tmpa)
                {
                    sql_1 += "'''" + s + "''',";
                }
                sql_1 = sql_1.Substring(0, sql_1.Length - 1);
                //====

                string sqlstr = "SELECT * FROM delholidaylog where hdellogstatus = '0'";
                   
                if (sql_1 != "")
                {
                    sqlstr += " and rolestampid in (" + sql_1 + ")";
                }
                if (qhlogsdate != "")
                {
                    sqlstr += " and hlogsdate >= '" + qhlogsdate + "'";
                }
                if (qhlogedate != "")
                {
                    sqlstr += " and hlogedate <= '" + qhlogedate + "'";
                }

                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.delholidaylog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<delholidaylog>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch(orderdata, orderdata1);
            return View(result);
        }
        private string SetOrder_ch(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "hsno", "hdayid", "empid", "hlogsdate" };
            string[] od_ch1 = { "asc", "asc", "asc", "asc" };
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

        #region 銷假作業 > 銷假單審核

        public ActionResult logqryEdit(delholidaylog chks, string sysflag, int? page, string orderdata, string orderdata1, HttpPostedFileBase logopic1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "hdellogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qhdellogstatus = "", qhlogsdate = "", qhlogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qhdellogstatus"]))
            {
                qhdellogstatus = Request["qhdellogstatus"].Trim();
                ViewBag.qhdellogstatus = qhdellogstatus;
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

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    //var data = con.delholidaylog.Where(r => r.hdellogid == chks.hdellogid).FirstOrDefault();
                    delholidaylog edelholidaylogs = con.delholidaylog.Find(chks.hdellogid);
                    if (edelholidaylogs == null)
                    {
                        return HttpNotFound();
                    }
                    return View(edelholidaylogs);
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
                    

                    //using (Aitag_DBContext con = new Aitag_DBContext())
                    //{
                    //    con.Entry(col).State = EntityState.Modified;
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
                    tmpform += "<form name='qfr1' action='/delholiday/logqryList' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

                    tmpform += "<input type=hidden id='qhdellogstatus' name='qhdellogstatus' value='" + qhdellogstatus + "'>";
                    tmpform += "<input type=hidden id='qhlogsdate' name='qhlogsdate' value='" + qhlogsdate + "'>";
                    tmpform += "<input type=hidden id='qhlogedate' name='qhlogedate' value='" + qhlogedate + "'>";

                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"" + tmpform };
                }
            }
        }


        public ActionResult logqryList(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "hdellogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qhdellogstatus = "", qhlogsdate = "", qhlogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qhdellogstatus"]))
            {
                qhdellogstatus = Request["qhdellogstatus"].Trim();
                ViewBag.qhdellogstatus = qhdellogstatus;
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

            NDcommon dbobj = new NDcommon();

            string DateEx = "", DateEx1 = "";
            if (dbobj.get_dbnull2(Request["qhlogsdate"]) != "")
            {
                dbobj.get_dateRang(Request["qhlogsdate"], "m", "min", @"出差日期起格式錯誤!!\n", out qhlogsdate, out DateEx);
                ViewBag.qhlogsdate = qhlogsdate;
            }
            if (dbobj.get_dbnull2(Request["qhlogedate"]) != "")
            {
                dbobj.get_dateRang(Request["qhlogedate"], "m", "max", @"出差日期訖格式錯誤!!\n", out qhlogedate, out DateEx1);
                ViewBag.qhlogedate = qhlogedate;
            }
            DateEx += DateEx1;

            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }


            IPagedList<delholidaylog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "SELECT * FROM delholidaylog where 1=1 "
//                    + " and empid='" + (string)Session["empid"] + "'"
                    + " and comid='" + (string)Session["comid"] + "'";

                if (qhdellogstatus != "")
                {
                    sqlstr += " and hdellogstatus = '" + qhdellogstatus + "'";
                }
                if (qhlogsdate != "")
                {
                    sqlstr += " and hlogsdate >= '" + qhlogsdate + "'";
                }
                if (qhlogedate != "")
                {
                    sqlstr += " and hlogedate <= '" + qhlogedate + "'";
                }

                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.delholidaylog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<delholidaylog>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch1(orderdata, orderdata1);
            return View(result);
        }
        private string SetOrder_ch1(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "hdellogstatus", "hdayid", "hlogsdate" };
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
}
