﻿using MvcPaging;
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
    public class holidaylogtraceController : BaseController
    {
        string DateEx = "";

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /holidaylog/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.hlogid = Session["hlogid"].ToString();
        //    holidaylog col = new holidaylog();
        //    return View(col);
        //}

        //[HttpPost]

        #region Add
        public ActionResult add(holidaylog col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "hlogid"; }

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

            //組假別
            furloughtype();

            if (sysflag != "A")
            {
                

                holidaylog newcol = new holidaylog();
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
                    using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                    {

                        //比對職務代理人 請假起訖
                        string tmpgo = get_tmpgo(conn, dbobj, col);
                        
                        if (tmpgo == "1")
                        {
                            ViewBag.ErrMsg = @"<script>alert(""職務代理人請假中，請重新選擇！"");</script>";
                            return View(col);
                        }
                        else
                        {
                            //此日期區間是否有請示過請假
                            conn.Close(); conn.Open();
                            string OHoliday = OwnerHolidaylog(conn, dbobj, col);
                            if (OHoliday == "1")
                            {
                                ViewBag.ErrMsg = @"<script>alert(""此日期區間已經有請示過請假！"");</script>";
                                return View(col);
                            }
                            //====

                            //找出下一個呈核角色
                            #region 找出下一個呈核角色
                            string tmparolestampid = "";
                            string tmprole = "";
                            string tmpbillid = "";
                            if (Request["arolestampid"].ToString() != "")
                            {
                                tmparolestampid = "'" + Request["arolestampid"].ToString() + "'";
                            }

                            string tmpmoney = "0";
                            string impallstring = dbobj.getnewcheck1("B", tmparolestampid, tmparolestampid, tmpmoney, "", "");
                            tmprole = impallstring.Split(';')[0].ToString();
                            tmpbillid = impallstring.Split(';')[1].ToString();
                            //if (tmprole == "")
                            //{
                            //    ViewBag.ErrMsg = @"<script>alert(""請先至表單流程設定中設定請假呈核流程!"");</script>";
                            //    return View(col);
                            //}
                            #endregion

                            //單據編號(自動產生編號)  申請單號
                            #region 單據編號(自動產生編號)
                            string tmphsno = "";
                            string sql_no = "select hsno from holidaylog where year(hdate) = " + DateTime.Now.Year + " and month(hdate) = " + DateTime.Now.Month + " and hlogtype='1' order by hsno desc";
                            string hsno = dbobj.get_dbvalue(conn, sql_no);
                            if(!string.IsNullOrEmpty(hsno))
                            {
                                //hsno = hsno.Substring(hsno.Length - 3, hsno.Length);
                                hsno = hsno.Substring(hsno.Length - 3, 3);
                                hsno = (Int16.Parse(hsno) + 1).ToString();
                            }
                            else
                            {
                                hsno = "1";
                            }
                            string tmpyear = (DateTime.Now.Year - 1911).ToString();
                            string tmpmonth = DateTime.Now.ToString("MM");
                            string tmpno1 = "000"+hsno;
                            tmpno1 = tmpno1.Substring(tmpno1.Length - 3, 3);

                            tmphsno = "B" + tmpyear + tmpmonth + tmpno1;
                            //====

                            #endregion


                            //扣假動作
                            string beforehour = "", lasthour = "";  //請假前已請時數 、 請假後已請時數 
                            #region 扣假動作
                            if (dbobj.get_dbnull2(Request["factsday"]) != "") //事實發生日 != ""
                            {
                                string sql = "select * from holidaycode where hdayid = '" + Request["hdayid"] + "'";
                                string allhour = "", mergehdayid = "";
                                conn.Close(); conn.Open();
                                using (SqlCommand cmd = new SqlCommand(sql, conn)) //假別資訊
                                {
                                    SqlDataReader dr = cmd.ExecuteReader();
                                    if (dr.HasRows)
                                    {
                                        dr.Read();
                                        allhour = dr["cgivehour"].ToString(); //事實發生時數
                                        mergehdayid = dr["mergehdayid"].ToString(); //合併扣假ID
                                    }
                                    dr.Close();
                                }

                                if (mergehdayid != "")//有合併扣假
                                {
                                    emphdlog chks = new emphdlog();
                                    chks.empid = col.empid;
                                    chks.hdayid = mergehdayid;
                                    chks.comid = (string)Session["comid"];
                                    using (Aitag_DBContext con = new Aitag_DBContext())
                                    {
                                        chks = con.emphdlog.Where(r => r.empid == chks.empid && r.hdayid == chks.hdayid && r.comid == chks.comid).FirstOrDefault();
                                        if (chks != null)
                                        {
                                            if (decimal.Parse("0" + chks.allhour.ToString()) < decimal.Parse("0" + chks.usehour.ToString()) + decimal.Parse("0" + col.hloghour.ToString()))
                                            {
                                                //Mark 停用時數 2016/11/17
                                               // ViewBag.ErrMsg = @"<script>alert(""請假時數已經超過，請重新申請！"");</script>";
                                               // return View(col);
                                            }
                                            else
                                            {
                                                //Mark 停用時數 2016/11/17
                                                //chks.usehour = decimal.Parse("0" + chks.usehour.ToString()) + decimal.Parse("0" + col.hloghour.ToString());
                                                //con.Entry(chks).State = EntityState.Modified;
                                                //con.SaveChanges();
                                            }
                                        }
                                        //??如果等於null 不做處理?
                                    }
                                }
                                else //無合併扣假
                                {
                                    if(decimal.Parse("0" + col.hloghour.ToString()) > decimal.Parse("0" + allhour))
                                    {
                                        ViewBag.ErrMsg = @"<script>alert(""請假時數已經超過，請重新申請！"");</script>";
                                        return View(col);
                                    }

                                    emphdlog chks = new emphdlog();
                                    chks.empid = col.empid;
                                    chks.hdayid = mergehdayid;
                                    chks.comid = (string)Session["comid"];
                                    chks.factsday = col.factsday;
                                    using (Aitag_DBContext con = new Aitag_DBContext())
                                    {
                                        chks = con.emphdlog.Where(r => r.empid == chks.empid && r.hdayid == chks.hdayid && r.comid == chks.comid && r.factsday == chks.factsday).FirstOrDefault();
                                        if (chks != null)
                                        {
                                            if (decimal.Parse("0" + chks.allhour.ToString()) < decimal.Parse("0" + chks.usehour.ToString()) + decimal.Parse("0" + col.hloghour.ToString()))
                                            {
                                                //Mark 停用時數 2016/11/17
                                                // ViewBag.ErrMsg = @"<script>alert(""請假時數已經超過，請重新申請！"");</script>";
                                               // return View(col);
                                            }
                                            else
                                            {
                                                //Mark 停用時數 2016/11/17
                                                // chks.usehour = decimal.Parse("0" + chks.usehour.ToString()) + decimal.Parse("0" + col.hloghour.ToString());
                                               // con.Entry(chks).State = EntityState.Modified;
                                               // con.SaveChanges();
                                            }
                                        }
                                        //??如果等於null 不做處理?
                                    }

                                }
                            }
                            else //事實發生日 == ""
                            {
                                if(dbobj.get_dbnull2(col.hdayid) == "A04")//補休
                                {
                                    string sql = "select isnull(sum(resthour - usehour),0) as allhour from resthourlog where empid = '" + col.empid + "' and rsdeaddate >= '" + col.hlogsdate.Value.ToString("yyy/MM/dd") + "' and adddate <= '" + col.hlogsdate.Value.ToString("yyy/MM/dd") + "' and comid='" + (string)Session["comid"] + "'";
                                    string allhour = dbobj.get_dbvalue(conn, sql);


                                    if(decimal.Parse("0" + allhour) < decimal.Parse("0" + col.hloghour.ToString()))
                                    {
                                        //Mark 停用時數 2016/11/17
                                        //ViewBag.ErrMsg = @"<script>alert(""請補休假時數已經超過，請重新申請或調整補休時數！"");</script>";
                                        //return View(col);
                                    }
                                    else
                                    {
                                        decimal tmphloghour = decimal.Parse("0" + col.hloghour.ToString());
                                        sql = "select * from resthourlog";
                                        string sqlwhere = " where empid = '" + col.empid + "' and rsdeaddate >= '" + col.hlogsdate.Value.ToString("yyy/MM/dd") + "' and resthour > usehour and comid='" + (string)Session["comid"] + "' order by rsdeaddate";
                                        int usehour = 0;
                                        using (SqlCommand cmd = new SqlCommand(sql + sqlwhere, conn))
                                        {
                                            SqlDataReader dr = cmd.ExecuteReader();
                                            decimal lefthour = 0;
                                            while (dr.Read())
                                            {
                                                lefthour = decimal.Parse("0" + dr["resthour"].ToString()) - decimal.Parse("0" + dr["usehour"].ToString()) - tmphloghour;


                                                restlogdet chks = new restlogdet();
                                                if(lefthour > 0)
                                                {
                                                    usehour = int.Parse(dr["usehour"].ToString()) + int.Parse(tmphloghour.ToString());
                                                    chks.hsno = tmphsno;
                                                    chks.rsid = int.Parse(dr["rsid"].ToString());
                                                    chks.empid = col.empid;
                                                    chks.usehour = int.Parse(tmphloghour.ToString()); //decimal 硬轉 int (有小數點時會錯誤)
                                                }
                                                else
                                                {
                                                    chks.hsno = tmphsno;
                                                    chks.rsid = int.Parse(dr["rsid"].ToString());
                                                    chks.empid = col.empid;
                                                    chks.usehour = int.Parse(dr["resthour"].ToString()) - int.Parse(dr["usehour"].ToString());

                                                    tmphloghour -= int.Parse(dr["resthour"].ToString()) - int.Parse(dr["usehour"].ToString());
                                                }
                                                using (Aitag_DBContext con = new Aitag_DBContext())
                                                {
                                                    //Mark 停用時數 2016/11/17
                                                    //con.restlogdet.Add(chks);
                                                    //con.SaveChanges();
                                                }

                                                //缺同時更新多筆
                                            }
                                            dr.Close();
                                        }
                                    }
                                }
                                else
                                {
                                    //假別 屬性
                                    string ifdelholiday = "select ifdelholiday from holidaycode where hdayid='" + col.hdayid + "'"; ifdelholiday = dbobj.get_dbvalue(conn, ifdelholiday);
                                    if(ifdelholiday == "y")
                                    {
                                        emphdlog chks = new emphdlog();
                                        chks.empid = col.empid;
                                        chks.hdayid = col.hdayid;
                                        chks.effectiveday = col.hlogsdate;
                                        chks.slyear = col.hlogsdate.Value.Year;

                                        using (Aitag_DBContext con = new Aitag_DBContext())
                                        {
                                            chks = con.emphdlog.Where(r => r.empid == chks.empid && 
                                                r.hdayid == chks.hdayid && 
                                                r.effectiveday >= chks.effectiveday && 
                                                r.slyear == chks.slyear && 
                                                r.usehour != r.allhour).FirstOrDefault();
                                            if (chks != null)
                                            {
                                                if (decimal.Parse("0" + chks.allhour.ToString()) < decimal.Parse("0" + chks.usehour.ToString()) + decimal.Parse("0" + col.hloghour.ToString()))
                                                {
                                                    //Mark 停用時數 2016/11/17
                                                    //ViewBag.ErrMsg = @"<script>alert(""請假時數已經超過，請重新申請其他假別！"");</script>";
                                                    //return View(col);
                                                }
                                                else
                                                {
                                                   
                                                    beforehour = chks.usehour.ToString();
                                                    chks.usehour = decimal.Parse("0" + chks.usehour.ToString()) + decimal.Parse("0" + col.hloghour.ToString());
                                                    lasthour = chks.usehour.ToString();
                                                    con.Entry(chks).State = EntityState.Modified;
                                                    //Mark 停用時數 2016/11/17
                                                    // con.SaveChanges();
                                                }
                                            }
                                            else
                                            {
                                                //Mark 停用時數 2016/11/17
                                                //ViewBag.ErrMsg = @"<script>alert(""請假時數已經超過，請重新申請其他假別！"");</script>";
                                                //return View(col);
                                            }
                                        }
                                    }





                                }





                            }



                            #endregion


                            //整理欄位 準備新增
                            #region 整理欄位 準備新增
                            //col.hlogstatus  ; // 己簽核:1  :0
                            col.hsno = tmphsno;

                            
                            col.hlogaddr = col.hlogaddr;
                            //col.hlogsdate = col.hlogsdate ; //[datetime]
                            //col.hlogedate = col.hlogedate

                            col.hlogstime = ViewBag.hlogstime; //[varchar](4)
                            col.hlogetime = ViewBag.hlogetime;
                            if (!string.IsNullOrWhiteSpace(Request["hloghour"]))
                            {
                                col.hloghour = decimal.Parse("0" + Request["hloghour"]);
                            }
                            else
                            {
                                col.hloghour = 0;
                            }
                            col.iftransfered = "n";
                            //if (!string.IsNullOrWhiteSpace(Request["arolestampid"]))
                            //{
                            //    col.arolestampid = Request["arolestampid"];
                            //}
                            //else
                            //{
                            //    col.arolestampid = Request["arolestampid1"];
                            //}
                            col.arolestampid = "";
                            col.hlogtype = "1";
                            col.hlogstatus = "1";
                            //col.rolestampid = tmprole;
                            //col.rolestampidall = tmparolestampid;
                            //col.empstampidall = "'" + col.empid + "'";
                            col.rolestampid = "";
                            col.rolestampidall = "";
                            col.empstampidall = "";
                            
                            //col.billflowid = int.Parse(tmpbillid);
                            col.billflowid = 0;

                            col.bmodid = Session["tempid"].ToString();
                            col.bmoddate = DateTime.Now;

                            col.comid = Session["comid"].ToString();
                            col.bmodid = Session["tempid"].ToString();
                            col.bmoddate = DateTime.Now;
                            col.hdate = DateTime.Now.Date;
                            col.hjumpdate = DateTime.Now.Date;
                            col.billtime = DateTime.Now.ToString();

                            col.addempid = Session["tempid"].ToString();

                            HttpPostedFileBase rfile = new HttpPostedFileWrapper(System.Web.HttpContext.Current.Request.Files[0]);
                            string Rfile = dbobj.FUsingle(rfile);
                            if (!string.IsNullOrWhiteSpace(Rfile))
                            {
                                col.hfile = Rfile;
                            }
                            #endregion

                            //寄信
                           // SetMail(conn, dbobj, col);


                        }


                    }



                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.holidaylog.Add(col);
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
                    string tmpevent = "申請人：" + col.empname + "<br>起迄日期：" + ViewBag.hlogsdate + "~" + ViewBag.hlogedate + "的資料<br>";
                    if (!string.IsNullOrWhiteSpace(col.hdayid))
                    {
                        string hdayid = col.hdayid.ToString();
                        if (hdayid == "A01" || hdayid == "A02" || hdayid == "A03" || hdayid == "A24")
                        {
                            //tmpevent = tmpevent + "請假前已請時數：" + beforehour + "小時，請假後已請時數：" + lasthour + "小時，共計：" + cint(lasthour) - cint(beforehour) + "小時";
                        }
                    }


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
                   
                    tmpform += "<form name='qfr1' action='/holidaylogtrace/List' method='post'>";
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
                    // return RedirectToAction("List");

                }
            }


        }

        private void SetMail(SqlConnection conn, NDcommon dbobj, holidaylog col)
        {
            string Mnote = "";
            

            string hlogsdate = ViewBag.hlogsdate;
            string hlogedate = ViewBag.hlogedate;
            string hlogstime = ViewBag.hlogstime;
            string hlogetime = ViewBag.hlogetime;

            //假別
            string hdaytitle = "select hdaytitle from holidaycode where hdayid='" + col.hdayid + "'"; hdaytitle = dbobj.get_dbvalue(conn, hdaytitle);

            Mnote += "<tr><td align=right width=130>申請單號：</td><td>" + col.hsno + "</td></tr>";
            Mnote = "<tr><td align=right width=130>申請人：</td><td>" + col.empname + "</td></tr>";
            Mnote += "<tr><td align=right>假別：</td><td>" + hdaytitle + "</td></tr>";
            Mnote += "<tr><td align=right>起迄日期：</td><td>自 " + hlogsdate + "(" + hlogstime + ")<BR>至 " + hlogedate + "(" + hlogetime + ")</td></tr>";
            Mnote += "<tr><td align=right>共計時數：</td><td>" + col.hloghour + "時</td></tr>";

            if(!string.IsNullOrEmpty(col.hlogcomment))
            {
                Mnote = Mnote + "<tr><td align=right>事由：</td><td>" + col.hlogcomment.ToString().Trim().Replace(Environment.NewLine, "<br>") + "</td></tr>";
            }

            string fromaddname = "select empname from employee where empid='" + col.empid + "'"; fromaddname = dbobj.get_dbvalue(conn, fromaddname);

            //寄信給下一個審核角色
            #region 寄信給下一個審核角色
            string mailtitle = "", MailContext = "";
            //找申請人mail
            string fromadd = "select enemail from employee where empid='" + col.empid + "'";

            mailtitle = "【" + fromaddname + "】請假單資料要求審核通知";
	        MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body><font face='Arial'>";
	        MailContext += "以下為明細資料：<BR>";
            MailContext += "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
	        MailContext += Mnote;
            MailContext += "</table>";
            MailContext += "</font></body></HTML>";

            string tmproleid = col.rolestampid.Replace("'", "");
            if(tmproleid != "")
            {
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

            //寄信給職務代理人
            #region 寄信給職務代理人
            mailtitle = ""; 
            MailContext = "";

            if (!string.IsNullOrWhiteSpace(col.replaceempid))
            {
                string toadd = "select enemail from employee where empid='" + col.replaceempid + "'"; toadd = dbobj.get_dbvalue(conn, toadd);

                mailtitle = "【" + fromaddname + "】請假單代理人知會通知";
                MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body><font face='Arial'>";
                MailContext = MailContext + "以下為明細資料：<BR>";
                MailContext = MailContext + "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                MailContext = MailContext + Mnote;
                MailContext = MailContext + "</table>";
                MailContext = MailContext + "</font></body></HTML>";

                //寄信
                dbobj.send_mailfile("", toadd, mailtitle, MailContext, null, null);
            }
            #endregion


            //寄信給支會人員
            #region 寄信給支會人員
            mailtitle = ""; 
            MailContext = "";

            if (!string.IsNullOrWhiteSpace(col.empmeetsign))
            {
                mailtitle= "【" + fromaddname + "】申請請假知會通知信";
				MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body><font face='Arial'>";
				MailContext = MailContext + "以下為明細資料：<BR>";
                MailContext = MailContext + "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
				MailContext = MailContext + Mnote;
				MailContext = MailContext + "</table>";
				MailContext = MailContext + "</font></body></HTML>";

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

        private string OwnerHolidaylog(SqlConnection conn, NDcommon dbobj, holidaylog col)
        {
            string OHoliday = "";
            string hlogshours = "", hlogsmin = "", hlogehours = "", hlogemin = "";
            if (!string.IsNullOrWhiteSpace(Request["hlogshours"]))
            {
                hlogshours = Request["hlogshours"].Trim();
            }
            if (!string.IsNullOrWhiteSpace(Request["hlogsmin"]))
            {
                hlogsmin = Request["hlogsmin"].Trim();
            }
            if (!string.IsNullOrWhiteSpace(Request["hlogehours"]))
            {
                hlogehours = Request["hlogehours"].Trim();
            }
            if (!string.IsNullOrWhiteSpace(Request["hlogemin"]))
            {
                hlogemin = Request["hlogemin"].Trim();
            }

            string hlogsdate = Convert.ToDateTime(col.hlogsdate).ToString("yyyyMMdd");
            string hlogedate = Convert.ToDateTime(col.hlogedate).ToString("yyyyMMdd");
            string hlogstime = hlogshours + hlogsmin;
            string hlogetime = hlogehours + hlogemin;

            ViewBag.hlogsdate = hlogsdate;
            ViewBag.hlogedate = hlogedate;
            ViewBag.hlogstime = hlogstime;
            ViewBag.hlogetime = hlogetime;

            string alltime = hlogsdate + hlogstime;
            string alltime1 = hlogedate + hlogetime;

            string startdate = " (RTRIM(REPLACE(CONVERT(char, hlogsdate, 111), '/', '')) + hlogstime)";
            string enddate = " (RTRIM(REPLACE(CONVERT(char, hlogedate, 111), '/', '')) + hlogetime)";

            string sql = "select * from holidaylog where hlogstatus in ('0','1','3') and empid = '" + col.empid + "' and ";
            sql += " ((" + startdate + " >= '" + alltime + "' and " + enddate + " <='" + alltime1 + "') or ";
            sql += " (" + startdate + " <= '" + alltime + "' and  " + enddate + " >='" + alltime + "') or ";
            sql += " (" + startdate + " <= '" + alltime1 + "' and  " + enddate + " >='" + alltime1 + "')) and comid='" + (string)Session["comid"] + "'";

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    OHoliday = "1";
                }
                else
                {
                    OHoliday = "0";
                }
                dr.Close();
            }
            return OHoliday;
        }

        private string get_tmpgo(SqlConnection conn, NDcommon dbobj, holidaylog col)
        {
            string sql = "select empid,hlogsdate,hlogedate from holidaylog where empid='" + col.replaceempid + "' and comid ='" + (string)Session["comid"] + "' order by hlogid desc";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Read();

                    DateTime hlogsdate = Convert.ToDateTime(col.hlogsdate);
                    DateTime hlogedate = Convert.ToDateTime(col.hlogedate);
                    DateTime hlogsdate1 = Convert.ToDateTime(dr["hlogsdate"]);
                    DateTime hlogedate1 = Convert.ToDateTime(dr["hlogedate"]);


                    if ((hlogsdate1 >= hlogsdate) && (hlogsdate1 <= hlogedate))
                    {
                        return "1";
                    }
                    else
                    {
                        if ((hlogedate1 >= hlogsdate) && (hlogedate1 <= hlogedate))
                        {
                            return "1";
                        }
                        else
                        {
                            return "2";
                        }
                    }
                }
                else
                {
                    return "2";
                }
                dr.Close();
            }
            //return "1"; 職代人請假迄日在使用者請假日期之中
            //return "2"; 職代人請假日期非在使用者請假日期中，或者沒有請假
        }

        //組假別
        private void furloughtype()
        {
            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                string sql = "select * from holidaycode order by hdayid";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();

                    List<string> option = new List<string>();
                    option.Add(":請選擇");

                    string hunit_1 = @"var hunit_1 = "",";
                    string hdayidList = "var hdayidList = [ ";
                    string StringList = "var StringList = [ ";
                    string factList = "var factList = [ ";
                    while (dr.Read())
                    {
                        hdayidList += @"""" + dr["hdayid"] + @""",";
                        StringList += @"""" + dr["cnote"].ToString().Trim().Replace(Environment.NewLine, "<br>") + @""",";
                        factList += @"""" + dr["ifdelholiday"] + @""",";

                        option.Add(dr["hdayid"] + ":" + dr["hdaytitle"]);
                        if (dbobj.get_dbnull2(dr["hunit"]) == "1")
                        {
                            hunit_1 += dr["hdayid"] + ",";
                        }
                    }
                    hunit_1 += @"""";
                    hdayidList += "];";
                    StringList += "];";
                    factList += "];";
                    dr.Close();
                    ViewBag.hdayid = option;
                    ViewBag.hunit_1 = hunit_1;
                    ViewBag.hdayidList = hdayidList; //組hdayid 陣列
                    ViewBag.StringList = StringList; //組cnote 陣列
                    ViewBag.factList = factList; //組事實發生 陣列
                }
            }

        }

        #endregion

        

        [ValidateInput(false)]
        public ActionResult Edit(holidaylog chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "hlogid"; }

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

                    //string oldhlogid = Request["oldhlogid"];                 

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        NDcommon dbobj = new NDcommon();
                        //chks.hlogid = Request["hlogid"].Trim();
                        chks.bmodid = Session["tempid"].ToString();
                        chks.bmoddate = DateTime.Now;
                        con.Entry(chks).State = EntityState.Modified;
                        con.SaveChanges();


                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "代碼:" + chks.hlogid + "名稱:" + chks.empname;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/holidaylog/List' method='post'>";
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

        }

        public ActionResult List(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "hlogsdate"; }

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
            qhlogsdate = NullStDate(Request["qhlogsdate"]);
            ViewBag.qhlogsdate = qhlogsdate;
            qhlogedate = NullTeDate(Request["qhlogedate"]);
            ViewBag.qhlogedate = qhlogedate;
            //NullStDate 跟 NullTeDate 會判斷格式，有錯誤就 修改全域的DateEx
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }


            if (!string.IsNullOrWhiteSpace(Request["qhlogstatus"]))
            {
                if (Request["qhlogstatus"] == "all")
                {
                    qhlogstatus = "";
                    ViewBag.qhlogstatus = qhlogstatus;
                }
                else
                {
                    qhlogstatus = Request["qhlogstatus"].Trim();
                    ViewBag.qhlogstatus = qhlogstatus;
                }
            }
            else
            {
                qhlogstatus = "1";
                ViewBag.qhlogstatus = qhlogstatus;
            }




            IPagedList<holidaylog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "SELECT * FROM holidaylog where (hlogtype='1' or (hlogtype='2' and (phsno='' or phsno is null )) or (hlogtype='3' and (phsno='' or phsno is null )) ) and comid='" + (string)Session["comid"] + "'  and";
                if (qdptid != "") 
                {
                    sqlstr += " dptid='" + qdptid + "'  and";
                }
                if (qempname != "")
                {
                    sqlstr += " empname like N'%" + qempname + "%'  and";
                }
                if (qhdayid != "")
                {
                    sqlstr += " hdayid='" + qhdayid + "'  and";
                }
                if (qhdayid != "")
                {
                    sqlstr += " hdayid='" + qhdayid + "'  and";
                }
                if (qhlogsdate != "")
                {
                    sqlstr += " hlogsdate >= '" + qhlogsdate + "'  and";
                }
                if (qhlogedate != "")
                {
                    sqlstr += " hlogedate <= '" + qhlogedate + "'  and";
                }
                if (qhlogstatus != "")
                {
                    sqlstr += " hlogstatus = '" + qhlogstatus + "'  and";
                }




                



                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.holidaylog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<holidaylog>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch(orderdata, orderdata1);
            return View(result);
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
                DateEx += @"請假日期起格式錯誤!!\n";
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
                DateEx += @"請假日期訖格式錯誤!!\n";
                tedate = "";
            }

            return tedate;
        }
        private string SetOrder_ch(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "hlogstatus", "hdayid", "dptid", "empid", "hlogsdate" };
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

            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/holidaylogtrace/List' method='post'>";
            //tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
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


            string hlogid = Request["hlogid"];
            string cdel = hlogid;

            if (string.IsNullOrWhiteSpace(cdel))
            {

                return new ContentResult() { Content = @"<script>alert('請勾選要刪除的資料!!');</script>" + tmpform };
            }
            else
            {
                NDcommon dbobj = new NDcommon();


                //系統LOG檔
                //================================================= //
                string sysnote = "";
                string sqlstr = "select * from holidaylog where hlogid = " + cdel;
                using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
                    {
                        SqlDataReader dr = cmd.ExecuteReader();
                        int drconn = 0;
                        while (dr.Read())
                        {
                            drconn ++;
                            sysnote += "請假資料" + dr["hlogid"] + "," + "請假人員" + dr["empid"];
                        }
                        sysnote = sysnote + "的資料" + drconn + "筆";
                        dr.Close();
                    }
                }
                if (sysnote.Length > 4000) { sysnote.Substring(0, 4000);}

                string sysrealsid = Request["sysrealsid"].ToString();//使用功能
                                  
                SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                string sysflag = "D";
                dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                sysconn.Close();
                sysconn.Dispose();
                //======================================================     

                //執行刪除
                dbobj.dbexecute("Aitag_DBContext", "DELETE FROM holidaylog where hlogid in (" + cdel + ")");

                return new ContentResult() { Content = @"<script>alert('刪除成功!!');</script>" + tmpform };
                //return RedirectToAction("List");
            }
        }

        public ActionResult holidaylogtracedel(string id, int? page)
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
                qhlogstatus = "D";  //顯示狀態:撤回
                ViewBag.qhlogstatus = qhlogstatus;
            }

            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/holidaylogtrace/List' method='post'>";
            //tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
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


            string cdel = Request["cdel"];

            if (string.IsNullOrWhiteSpace(cdel))
            {

                return new ContentResult() { Content = @"<script>alert('請選擇要撤回的資料!!');</script>" + tmpform };
            }
            else
            {
                NDcommon dbobj = new NDcommon();
                int tmpcount = 0;
                string tmpcomment = "", xhlogstime = "", xhlogetime = "", sysnote = "", mailtitle="";
                string sql = "select * from holidaylog";
                string sqlWHERE = " where hlogid in (" + cdel + ") and comid='" + (string)Session["comid"] + "'";
                sql += sqlWHERE;
                using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        SqlDataReader dr = cmd.ExecuteReader();
                        int drconn = 0;
                        while (dr.Read())
                        {
                            xhlogstime = "（" + dr["hlogstime"] + "）";
                            xhlogetime = "（" + dr["hlogetime"] + "）";

                            #region 寄信(通知給目前簽核角色)
                            string hdaytitle = "", hlogcomment="";
                            hdaytitle = dbobj.get_dbvalue(conn, "select hdaytitle from holidaycode where hdayid='" + dr["hdayid"] + "'");

                            
                            if (dbobj.get_dbnull2(dr["hlogcomment"]).Trim() != "")
                            {
                                hlogcomment = dr["hlogcomment"].ToString().Trim().Replace(Environment.NewLine, "<br>");
                            }

                            switch (dbobj.get_dbnull2(dr["hlogcomment"]))
                            {
                                case "1":
                                    mailtitle = "請假單撤回通知";
                                    break;
                                case "2":
                                    mailtitle = "集體請假單撤回通知";
                                    break;
                                case "3":
                                    mailtitle = "週期性請假單撤回通知";
                                    break;
                                default:
                                    break;
                            }

                            string MailContext = "";
                            MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=UTF-8'></HEAD><body>";
                            
                            MailContext += "以下為明細資料：<BR>";
                            MailContext += "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                            MailContext += "<tr><td align=right width=130>申請人：</td><td>" + dr["empname"] + "</td></tr>";
                            MailContext += "<tr><td align=right>假別：</td><td>" + hdaytitle + "</td></tr>";
                            MailContext += "<tr><td align=right>起迄日期：</td><td>自 " + dr["hlogsdate"] + xhlogstime + "<BR>至 " + dr["hlogedate"] + xhlogetime + "</td></tr>";
                            MailContext += "<tr><td align=right>共計時數：</td><td>" + dr["hloghour"] + "時</td></tr>";
                            MailContext += "<tr><td align=right>事由：</td><td>" + hlogcomment + "+nbsp;</td></tr>";
                            MailContext += "</table>";
                            MailContext += "</body></HTML>";

                            //'寄件者
                            string fromadd = "", fromaddname = "";
                            fromadd = dbobj.get_dbvalue(conn, "select enemail from employee  where empid='" + (string)Session["empid"] + "'");
	                        fromaddname = (string)Session["empname"];

                            //'寄給申請人
                            string toadd = "";
                            toadd = dbobj.get_dbvalue(conn, "select enemail from employee  where empid='" + dr["empid"] + "'");

                            if (toadd != "")
                            {
                               //發信先註解
                            }

                            //'收件者	
                            if (dbobj.get_dbnull2(dr["rolestampid"]).Trim() != "")
                            {
                                string rolestampid = dbobj.get_dbnull2(dr["rolestampid"]).Trim();
                                string sql_m = "select enemail from viewemprole where rid in (" + rolestampid + ") and empstatus <> '4' and enemail<>'' and comid='" + (string)Session["comid"] + "'";
                                using (SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext"))
                                {
                                    using (SqlCommand cmd1 = new SqlCommand(sql_m, conn1))
                                    {
                                        SqlDataReader dr_m = cmd1.ExecuteReader();
                                        while (dr_m.Read())
                                        {
                                            toadd = dbobj.get_dbnull2(dr_m["enemail"]);
                                            //發信先註解
                                        }
                                        dr_m.Close();
                                    }
                                }
                            }

                            //'寄給職務代理人
                            if (dbobj.get_dbnull2(dr["hlogtype"]).Trim() != "")
                            {
                                toadd = dbobj.get_dbvalue(conn, "select enemail from employee where empid='" + dr["replaceempid"] + "'");
                                if (toadd != "")
                                {
                                    //發信先註解
                                }
                            }

                            #endregion

                            #region 當撤回的請假單為個人的時候做以下動作
                            //'當撤回的請假單為個人的時候做以下動作
                            if (dbobj.get_dbnull2(dr["hlogtype"]).Trim() == "1")
                            {
                                string slyear = "", hloghour = "", hlasthour = "";
                                double lefthour = 0;
                                slyear = Convert.ToDateTime(dr["hlogsdate"]).ToString("yyyy");
                                hloghour = dbobj.get_dbnull2(dr["hloghour"]).Trim();
                                hlasthour = dbobj.get_dbnull2(dr["hlasthour"]).Trim();

                                if (string.IsNullOrEmpty(hlasthour))
                                { hlasthour = "0"; }
                                lefthour = Convert.ToDouble(hloghour) - Convert.ToDouble(hlasthour);
                                //'退回要減假回去
                                string mergehdayid = "";
                                if (dbobj.get_dbnull2(dr["factsday"]).Trim() != "")
                                {
                                    mergehdayid = dbobj.get_dbvalue(conn, "select mergehdayid from from holidaycode where hdayid = '" + dr["hdayid"] + "' and comid='" + (string)Session["comid"] + "'");
                                }
                                
                                //'事實發生假部分
                                if (mergehdayid != "")
                                {
                                    sql = "select * from emphdlog where empid = '" + dr["empid"] + "' and hdayid = '" + mergehdayid + "' and comid='" + (string)Session["comid"] + "'";
                                }
                                else
                                {
                                    sql = "select * from emphdlog where empid = '" + dr["empid"] + "' and hdayid = '" + dr["hdayid"] + "' and factsday = '" + dr["factsday"] + "' and comid='" + (string)Session["comid"] + "'";
                                }
                                using (SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext")) 
                                {
                                    using (SqlCommand cmd1 = new SqlCommand(sql, conn1))
                                    {
                                        SqlDataReader dr1 = cmd1.ExecuteReader();
                                        if (dr1.HasRows)
                                        {
                                            dr1.Read();
                                            if (Convert.ToDouble(dbobj.get_dbnull2(dr1["usehour"])) == 0)
                                            {
                                                if (mergehdayid != "")
                                                {
                                                    sql = "delete from emphdlog where empid = '" + dr["empid"] + "' and hdayid = '" + mergehdayid + "' and comid='" + (string)Session["comid"] + "'";
                                                }
                                                else
                                                {
                                                    sql = "delete from emphdlog where empid = '" + dr["empid"] + "' and hdayid = '" + dr["hdayid"] + "' and factsday = '" + dr["factsday"] + "' and comid='" + (string)Session["comid"] + "'";
                                                }
                                                //Mark 停用時數 2016/11/17
                                                //dbobj.dbexecute("Aitag_DBContext", sql);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (dbobj.get_dbnull2(dr["hdayid"]).Trim() == "A04")
                                {//'請補假部分
                                    sql = "select * from resthourlog";
                                    string sqlwhere = " where empid = '" + dr["empid"] + "' and rsdeaddate >= '" + dr["hlogsdate"] + "' and usehour <> 0 and comid='" + (string)Session["comid"] + "' order by rsdeaddate desc";
                                    sql += sqlwhere;
                                    using (SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext"))
                                    {
                                        using (SqlCommand cmd1 = new SqlCommand(sql, conn1))
                                        {
                                            SqlDataReader dr1 = cmd1.ExecuteReader();
                                            double tmphloghour = Convert.ToDouble(dr["hloghour"]);
                                            double lefthour = 0;
                                            while (dr1.Read())
                                            {
                                                lefthour = Convert.ToDouble(dr1["usehour"]) - tmphloghour;
                                                if (lefthour > 0)
                                                {
                                                    sql = "UPDATE resthourlog SET usehour = " + lefthour.ToString();
                                                }
                                                else if (lefthour == 0)
                                                {
                                                    break;
                                                }
                                                else
                                                {
                                                    sql = "UPDATE resthourlog SET usehour =  0";
                                                }
                                                //Mark 停用時數 2016/11/17
                                                //sql += sqlwhere;
                                                //dbobj.dbexecute("Aitag_DBContext", sql);
                                            }
                                        }
                                    }
                                }
                                else
                                {//{'請休假發假部分
                                    double usehour = 0;
                                    sql = "select usehour from emphdlog";
                                    string sqlwhere = " where empid = '" + dr["empid"] + "' and hdayid = '" + dr["hdayid"] + "'  and effectiveday >= '" + dr["hlogsdate"] + "' and slyear = " + Convert.ToDateTime(dr["hlogsdate"]).ToString("yyyy/MM/dd") + " and comid='" + (string)Session["comid"] + "'";
                                    sql += sqlwhere;
                                    using (SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext"))
                                    { usehour = Convert.ToDouble(dbobj.get_dbvalue(conn1, sql)); }

                                    usehour -= Convert.ToDouble(dr["hloghour"]);
                                    //Mark 停用時數 2016/11/17
                                    //sql = "UPDATE emphdlog SET usehour = " + usehour;
                                    //sql += sqlwhere;
                                    //dbobj.dbexecute("Aitag_DBContext", sql);
                                }
                            }
                            #endregion


                            sql = "UPDATE holidaylog SET hlogstatus = 'D'";
                            sql += sqlWHERE;
                            dbobj.dbexecute("Aitag_DBContext", sql);

                            if (dbobj.get_dbnull2(dr["hlogtype"]).Trim() != "1")
                            {
                                sql = "update holidaylog set hlogstatus='D' where phsno ='" + dr["hsno"] + "' and comid='" + (string)Session["comid"] + "'";
                            }
                            else
                            {
                                drconn++;
                                sysnote += "申請人：" + dr["empname"] + "申請單號：" + dr["hsno"] + ",";
                            }

                        }


                        sysnote = sysnote.Substring(0, sysnote.Length - 1);
                        sysnote = sysnote + "的資料" + drconn + "筆";
                        dr.Close();
                    }
                }






                //系統LOG檔
                //================================================= //
                if (sysnote.Length > 4000) { sysnote.Substring(0, 4000); }
                string sysrealsid = Request["sysrealsid"].ToString() + "(撤回)";//使用功能

                SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                string sysflag = "D";
                dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                sysconn.Close();
                sysconn.Dispose();
                //======================================================     


                return new ContentResult() { Content = @"<script>alert('資料撤回成功!!');</script>" + tmpform };
                //return RedirectToAction("List");
            }
        }


        public ActionResult holidaylogtracerpt(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "hlogsdate"; }

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
            qhlogsdate = NullStDate(Request["qhlogsdate"]);
            ViewBag.qhlogsdate = qhlogsdate;
            qhlogedate = NullTeDate(Request["qhlogedate"]);
            ViewBag.qhlogedate = qhlogedate;
            //NullStDate 跟 NullTeDate 會判斷格式，有錯誤就 修改全域的DateEx
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }


            if (!string.IsNullOrWhiteSpace(Request["qhlogstatus"]))
            {
                if (Request["qhlogstatus"] == "all")
                {
                    qhlogstatus = "";
                    ViewBag.qhlogstatus = qhlogstatus;
                }
                else
                {
                    qhlogstatus = Request["qhlogstatus"].Trim();
                    ViewBag.qhlogstatus = qhlogstatus;
                }
            }
            else
            {
                qhlogstatus = "1";
                ViewBag.qhlogstatus = qhlogstatus;
            }


            string sqlstr = "";
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                sqlstr = "SELECT * FROM holidaylog where (hlogtype='1' or (hlogtype='2' and (phsno='' or phsno is null )) or (hlogtype='3' and (phsno='' or phsno is null )) ) and comid='" + (string)Session["comid"] + "'  and";
                if (qdptid != "")
                {
                    sqlstr += " dptid='" + qdptid + "'  and";
                }
                if (qempname != "")
                {
                    sqlstr += " empname like N'%" + qempname + "%'  and";
                }
                if (qhdayid != "")
                {
                    sqlstr += " hdayid='" + qhdayid + "'  and";
                }
                if (qhdayid != "")
                {
                    sqlstr += " hdayid='" + qhdayid + "'  and";
                }
                if (qhlogsdate != "")
                {
                    sqlstr += " hlogsdate >= '" + qhlogsdate + "'  and";
                }
                if (qhlogedate != "")
                {
                    sqlstr += " hlogedate <= '" + qhlogedate + "'  and";
                }
                if (qhlogstatus != "")
                {
                    sqlstr += " hlogstatus = '" + qhlogstatus + "'  and";
                }

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

            }

            #region 組 Excel 格式
            string Excel = "", Excel1 = "", Excel2 = "";

            List<string> Excel2List = new List<string>();

            #region
            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    int drcount = 0;
                    using (SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext"))
                    {
                        while (dr.Read())
                        {

                            string hlogstatus = dr["hlogstatus"].ToString();
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
                                //case "3":
                                //    hlogstatus = "代理人簽核";
                                //    break;
                                case "D":
                                    hlogstatus = "撤回";
                                    break;
                                default:
                                    break;
                            }

                            string hdaytitle = "";
                            string hdayid = dr["hdayid"].ToString();
                            hdaytitle = dbobj.get_dbvalue(conn1, "select hdaytitle from holidaycode where hdayid = '" + hdayid + "'");

                            string dpttitle = "";
                            string dptid = dr["dptid"].ToString();
                            dpttitle = dbobj.get_dbvalue(conn1, "select dpttitle from Department where dptid='" + dptid + "' and comid='" + (string)Session["comid"] + "'");

                            string empname = dr["empname"].ToString();

                            DateTime hlogsdate = DateTime.Now, hlogedate = DateTime.Now;
                            hlogsdate = Convert.ToDateTime(dr["hlogsdate"]);
                            string hlogstime = dr["hlogstime"].ToString();
                            hlogedate = Convert.ToDateTime(dr["hlogedate"]);
                            string hlogetime = dr["hlogetime"].ToString();
                            string thlogsdate = "";
                            string thlogetime = "";
                            string datetime = "";
                            thlogsdate = hlogsdate.ToString("yyyy/MM/dd") + "\t\t" + "\t\t" + hlogstime;
                            thlogetime = hlogedate.ToString("yyyy/MM/dd") + "\t\t" + "\t\t" + hlogetime;
                            datetime = thlogsdate + "<br>" + thlogetime;

                            string hloghour = dr["hloghour"].ToString();

                            string tmpwhere = "";
                            string hlogaddr = dr["hlogaddr"].ToString();
                            tmpwhere = dbobj.get_dbvalue(conn1, "select chkitem from checkcode where chkclass = '90' and chkcode = '" + hlogaddr + "'");

                            
                            string rolestampid = dr["rolestampid"].ToString();
                            rolestampid = dbobj.getrole(rolestampid);
                            
                            Excel2 += "<tr align=left>";
                            Excel2 += "<td>" + hlogstatus + "</td>";
                            Excel2 += "<td>" + hdaytitle + "</td>";
                            Excel2 += "<td>" + dpttitle + "</td>";
                            Excel2 += "<td>" + empname + "</td>";
                            Excel2 += "<td>" + datetime + "</td>";
                            Excel2 += "<td>" + hloghour + "</td>";
                            Excel2 += "<td>" + tmpwhere + "</td>";
                            Excel2 += "<td>" + rolestampid + "</td>";
                            Excel2 += "</tr>";
                            
                            if (drcount + 1  == 22)
                            {
                                Excel2List.Add(Excel2);
                                Excel2 = "";
                                drcount = 0;
                            }
                            else
                            {
                                drcount++;
                            }

                        }

                        if (Excel2 != "")
                        {
                            Excel2List.Add(Excel2);
                            Excel2 = "";
                            drcount = 0;
                        }



                    }
                    dr.Close();
                }
            }

            #endregion


            //Excel1 += "<div STYLE='page-break-after: always;'>";/*換頁用*/
            Excel1 += "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=0 width=100%  bgcolor=ffffff style=font-size:10pt>";
            Excel1 += "<tr align=center><td colspan=2 ><font style=font-size:15pt>請假明細表</font></td></tr>";
            Excel1 += "<tr><td>列印日期：" + DateTime.Now.ToString("yyyy/MM/dd") + "</td> <td align=right>第1頁/共 1頁</td>";
            Excel1 += "<tr>";
            Excel1 += "<td colspan=2>";
            Excel1 += "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=100% bgcolor=ffffff style=font-size:10pt>";
            Excel1 += "<tr>";
            Excel1 += "<td width=60>狀態</td>";
            Excel1 += "<td width=60>假別</td>";
            Excel1 += "<td width=60>部門</td>";
            Excel1 += "<td width=60>申請人</td>";
            Excel1 += "<td width=220>請假起迄日期</td>";
            Excel1 += "<td width=60>請假時數</td>";
            Excel1 += "<td width=40>地點</td>";
            Excel1 += "<td width=200>待簽核角色</td>";
            Excel1 += "</tr>";
            foreach (var obj in Excel2List.Select((val2, idx2) => new { Index = idx2, Value = val2 }))
            {
                Excel1 += obj.Value;

                
                //if (obj.Index == 1)
                //{
                //    break;
                //}
            }
            Excel1 += "</table>";
            Excel1 += "</td>";
            Excel1 += "</tr>";
            Excel1 += "</table>";
            //Excel1 += "</div>";

            if (Excel2List.Count == 0)
            {
                Excel1 = "目前沒有資料!";
            }


            Excel += "<HTML>";
            Excel += "<HEAD>";
            Excel += @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">";
            Excel += "</HEAD>";
            Excel += "<body>";
            
            Excel += Excel1;

            Excel += "</body>";
            Excel += "</HTML>";


            ViewBag.Excel = Excel;
            return View();
        }

        #endregion

        //public string rolestampid { get; set; }
    }
}
