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
    public class battalogController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /battalog/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        #region 出差作業 > 出差單申請
        public ActionResult battaadd(battalog col, string sysflag, int? page, string orderdata, string orderdata1, HttpPostedFileBase logopic1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "blogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;


            if (sysflag != "A")
            {
                battalog newcol = new battalog();
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
                    string impallstring = dbobj.getnewcheck1("A", tmparolestampid, tmparolestampid, tmpmoney, "", "");
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
                    if ((string)Session["mplayrole"] == "")
                    {
                        errmsg = "您並未設定呈核角色!";
                        ViewBag.errmsg = "<script>alert('" + errmsg + "');</script>";
                        return View(col);
                    }

                    //組sql  找是否有出差
                    Boolean ifbrepman = false, ifpriv = false;
                    string sql1 = "select blogid from battalog where blogstatus in ('0','1','3')"
                                    + " and ((blogsdate < '" + Request["blogsdate"] + "' and blogedate >'" + Request["blogsdate"] + "')"
                                        + " or (blogsdate='" + Request["blogedate"] + "')"
                                        + " or ( blogsdate >='" + Request["blogsdate"] + "' and blogedate <='" + Request["blogedate"] + "')"
                                        + " or ( blogsdate < '" + Request["blogedate"] + "' and blogedate >'" + Request["blogedate"] + "' )"
                                        + " or (blogedate ='" + Request["blogsdate"] + "'))"
                                    + " and empid = '" + Request["empid"] + "'"
                                    + " and comid='" + (string)Session["comid"] + "'";
                    //若日期小於今天要看是否可以請
                    string sql2 = "select hdid from hdafterpriv where hdtype='A' and empid ='" + Request["empid"]
                    + "' and hdsdate <= '" + Request["blogsdate"]
                    + "' and hdedate >='" + Request["blogedate"] + "'";


                    using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        string HasRows = dbobj.get_dbvalue(conn, sql1);
                        if (!string.IsNullOrEmpty(HasRows))
                        {/*找是否出差了*/
                            ifbrepman = true;
                        }
                        HasRows = dbobj.get_dbvalue(conn, sql1);
                        if (!string.IsNullOrEmpty(HasRows))
                        {/*找是否出差了*/
                            ifpriv = true;
                        }
                    }

                    if (ifbrepman)
                    {
                        errmsg = "此日期區間已經有請示過出差!";
                        ViewBag.errmsg = "<script>alert('" + errmsg + "');</script>";
                        return View(col);
                    }
                    if (ifpriv)
                    {
                        errmsg = "不能補請出差單!";
                        ViewBag.errmsg = "<script>alert('" + errmsg + "');</script>";
                        return View(col);
                    }

                    //'找單據編號(自動產生編號)
                    string tmpbsno = "select bsno from battalog where year(cdate) = " + DateTime.Now.Year + " and month(cdate) = " + DateTime.Now.Month + " and blogtype='1' and bsno is not null order by bsno desc";
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

                    
                    col.blogtype = "1";
                    col.blogstatus = "3";
                    col.bsno = tmpbsno;

                    //單據張數
                    if (dbobj.get_dbnull2(col.bbillcount) == "")
                    {
                        col.bbillcount = 0;
                    }
                    col.resthour = 0;

                    //呈核人員
                    //======
                    if (dbobj.get_dbnull2(col.arolestampid) == "")
                    {
                        col.arolestampid = Request["arolestampid1"];
                    }
                    col.rolestampid = tmprole; //'下個呈核角色
                  //  col.tmprolestampid = tmprole;
                    col.rolestampidall = tmparolestampid; //'所有呈核角色
                    col.empstampidall = "'" + Request["empid"] + "'"; //'所有人員帳號
                    col.billflowid = int.Parse(tmpbillid);
                    //======

                    col.comid = (string)Session["comid"];
                    col.bmodid = (string)Session["empid"];
                    col.bmoddate = DateTime.Now;
                    col.iftraholiday = "n";
                    col.ifhdell = "n";
                    col.bdate = DateTime.Now;
                    col.billtime = DateTime.Now.ToString();

                    col.addempid = (string)Session["empid"]; //'代申請人
                    col.empmeetsign = Request["otherman"];  //  知會人員

                    HttpPostedFileBase File1 = new HttpPostedFileWrapper(System.Web.HttpContext.Current.Request.Files[0]);
                    string bfile = dbobj.FUsingle(File1);
                    if (!string.IsNullOrWhiteSpace(bfile))
                    {
                        col.bfile = bfile;
                    }

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.battalog.Add(col);
                        con.SaveChanges();
                    }

                    //??
                    battadet bat = new battadet();
                    string tmpid = "select blogid from battalog where bsno ='" + tmpbsno + "' and comid='" + (string)Session["comid"] + "' order by blogid desc";
                    using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        tmpid = dbobj.get_dbvalue(conn, tmpid);
                    }
                    if (tmpid == "")
                    {
                        //return View(col);/*找不到id就回到Add頁面*/
                    }
                    DateTime blogsdate = Convert.ToDateTime(Request["blogsdate"]);
                    DateTime blogedate = Convert.ToDateTime(Request["blogedate"]);
                    TimeSpan tmpday = blogedate.Subtract(blogsdate);
                    int max = 0;
                    if (tmpday.Days > 0)
                    {
                        max = tmpday.Days;
                    }
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        for (int i = 0; i <= max; i++)
                        {
                            //bat.blogid = int.Parse(tmpid);
                            bat.bddate = blogsdate;
                            bat.bdmonth = blogsdate.Month.ToString();
                            bat.bdday = blogsdate.Day.ToString();
                            bat.bdplace = "";
                            bat.bdwork = "";
                            bat.bdplane = 0;
                            bat.bdcar = 0;
                            bat.bdtrain = 0;
                            bat.bdship = 0;
                            bat.bdliving1 = 0;
                            bat.bdliving2 = 0;
                            bat.bdother = 0;
                            bat.bdbillno = "";
                            bat.bdcomment = "";
                            bat.bland = 0;
                            bat.blive = 0;
                            bat.bvisa = 0;
                            bat.binsurance = 0;
                            bat.badmin = 0;
                            bat.bgift = 0;
                            blogsdate = blogsdate.AddDays(1);

                            //con.battadet.Add(bat);
                            //con.SaveChanges();
                        }
                        
                    }
                    //======

                    //    '寄信
                    //'======================
                    using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        string fromadd = "", fromaddname = "", mailtitle = "", MailContext = "";
                        fromadd = dbobj.get_dbvalue(conn, "select enemail from employee where empid='" + Request["empid"] + "'");
                        fromaddname = dbobj.get_dbvalue(conn, "select empname from employee where empid='" + Request["empid"] + "'");

                        //'寄送mail給下一個審核角色
                        #region 寄送mail給下一個審核角
                        mailtitle = "【" + col.empname + "】出差單資料要求審核通知";
                        MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body>";
                        MailContext = MailContext + "以下為明細資料：<BR>";
                        MailContext = MailContext + "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                        MailContext = MailContext + "<tr><td align=right width=130>申請人：</td><td>" + col.empname + "</td></tr>";
                        //出差起迄日期
                        string SEDate = "自{0}({1}時)<BR>至{2}({3}時)";
                        SEDate = string.Format(SEDate, dbobj.get_dbDate(col.blogsdate, "d"), col.blogstime
                            , dbobj.get_dbDate(col.blogedate, "d"), col.blogetime);
                        MailContext = MailContext + "<tr><td align=right>出差起迄日期：</td><td>" + SEDate + "</td></tr>";

                        string bloghour = dbobj.get_dbnull2(col.bloghour);
                        bloghour = (double.Parse(bloghour) / 8).ToString("0.#");
                        MailContext = MailContext + "<tr><td align=right>共計天數：</td><td>" + bloghour + "</td></tr>";
                        if (dbobj.get_dbnull2(col.blogcomment) != "")
                        {
                            MailContext = MailContext + "<tr><td align=right>出差事由：</td><td>" + col.blogcomment.ToString().Trim().Replace(Environment.NewLine, "<br>") + "</td></tr>";
                        }
                        else
                        {
                            MailContext = MailContext + "<tr><td align=right>出差事由：</td><td>&nbsp;</td></tr>";
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

                        //寄給職務代理人
                        #region 職務代理人
                        string toadd = dbobj.get_dbvalue(conn, "select enemail from employee where empid='" + col.replaceempid + "'");
                        mailtitle = "【" + fromaddname + "】申請出差單指定您為他的代理人-審核通知";
                        //MailContext = MailContext;


                        if (!string.IsNullOrEmpty(toadd))
                        {
                            dbobj.send_mailfile("", toadd, mailtitle, MailContext, null, null);
                        }
                        #endregion


                        //寄送mail給支會人員
                        #region 支會人員
                        if (dbobj.get_dbnull2(Request["otherman"]) != "")
                        {

                            mailtitle = "【" + fromaddname + "】申請出差單知會通知信";
                            //MailContext = MailContext;
                            

                            string otherman = Request["otherman"];
                            otherman = otherman.Substring(1, otherman.Length - 1);
                            sql = "select enemail from employee where empid in (" + otherman + ") and enemail <> '' and enemail is not null";
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
                    

                    //using (Aitag_DBContext con = new Aitag_DBContext())
                    //{
                    //    con.battalog.Add(col);
                    //    con.SaveChanges();
                    //}



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
                    tmpform += "<form name='qfr1' action='/battalog/battamainList' method='post'>";
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

        #region 出差作業 > 出差單撤回及修改
        public ActionResult battamainEdit(battalog chks, string sysflag, int? page, string orderdata, string orderdata1, HttpPostedFileBase logopic1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "blogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qblogstatus = "", qblogsdate = "", qblogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qblogstatus"]))
            {
                qblogstatus = Request["qblogstatus"].Trim();
                ViewBag.qblogstatus = qblogstatus;
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
                    //var data = con.battalog.Where(r => r.blogid == chks.blogid).FirstOrDefault();
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
                    NDcommon dbobj = new NDcommon();
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        battalog ebattalogs = con.battalog.Find(chks.blogid);

                        ebattalogs.blogsdate = chks.blogsdate;
                        ebattalogs.blogstime = chks.blogstime;

                        ebattalogs.blogedate = chks.blogedate;
                        ebattalogs.blogetime = chks.blogetime;

                        ebattalogs.bloghour = int.Parse(dbobj.get_dbnull2(Request["bloghour1"]));

                        ebattalogs.bmodid = Session["tempid"].ToString();
                        ebattalogs.bmoddate = DateTime.Now;

                        using (Aitag_DBContext con1 = new Aitag_DBContext())
                        {
                            con1.Entry(ebattalogs).State = EntityState.Modified;
                            con1.SaveChanges();
                        }
                    }


                    //系統LOG檔
                    string sysnote = "申請人：{0}<br>申請單號：{1}的資料";
                    sysnote = string.Format(sysnote, chks.empid, chks.bsno);
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
                    tmpform += "<form name='qfr1' action='/battalog/battamainList' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

                    tmpform += "<input type=hidden id='qblogstatus' name='qblogstatus' value='" + qblogstatus + "'>";
                    tmpform += "<input type=hidden id='qblogsdate' name='qblogsdate' value='" + qblogsdate + "'>";
                    tmpform += "<input type=hidden id='qblogedate' name='qblogedate' value='" + qblogedate + "'>";

                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"" + tmpform };
                }
            }

        }

        public ActionResult battamainList(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "blogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qblogstatus = "", qblogsdate = "", qblogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qblogstatus"]))
            {
                qblogstatus = Request["qblogstatus"].Trim();
                ViewBag.qblogstatus = qblogstatus;
            }

            NDcommon dbobj = new NDcommon();

            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qblogsdate"], "m", "min", @"出差日期起格式錯誤!!\n", out qblogsdate, out DateEx);
            ViewBag.qblogsdate = qblogsdate;
            dbobj.get_dateRang(Request["qblogedate"], "m", "max", @"出差日期訖格式錯誤!!\n", out qblogedate, out DateEx1);
            ViewBag.qblogedate = qblogedate;
            DateEx += DateEx1;

            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }


            IPagedList<battalog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "SELECT * FROM battalog where blogtype='1' and blogstatus in ('0','1','3')"
                    + " and empid='" + (string)Session["empid"] + "'"
                    + " and comid='" + (string)Session["comid"] + "'";

                if (qblogstatus != "")
                {
                    sqlstr += " and blogstatus = '" + qblogstatus + "'";
                }
                if (qblogsdate != "" && qblogedate != "")
                {
                    sqlstr += " and (( blogsdate >= '" + qblogsdate + "' and blogsdate <= '" + qblogedate + "' ) or "
                        + "( blogedate >= '" + qblogsdate + "' and blogedate <= '" + qblogedate + "'))";
                }

                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.battalog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<battalog>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch1(orderdata, orderdata1);
            return View(result);
        }
        private string SetOrder_ch1(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "blogstatus", "ifhdell", "empname", "blogsdate" };
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

        [ActionName("battamainDelete")]/*取別名*/
        public ActionResult battamainDelete(string id, int? page)
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

            string qblogstatus = "", qblogsdate = "", qblogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qblogstatus"]))
            {
                qblogstatus = Request["qblogstatus"].Trim();
                ViewBag.qblogstatus = qblogstatus;
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



            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/battalog/battamainList' method='post'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

            tmpform += "<input type=hidden id='qblogstatus' name='qblogstatus' value='" + qblogstatus + "'>";
            tmpform += "<input type=hidden id='qblogsdate' name='qblogsdate' value='" + qblogsdate + "'>";
            tmpform += "<input type=hidden id='qblogedate' name='qblogedate' value='" + qblogedate + "'>";

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
                dbobj.dbexecute("Aitag_DBContext", "UPDATE battalog SET blogstatus = 'D' where blogid in (" + cdel + ")");


                //系統LOG檔
                //================================================= //
                string sysnote = "";
                string sqlstr = "select * from battalog where blogid in (" + cdel + ")";
                using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
                    {
                        SqlDataReader dr = cmd.ExecuteReader();
                        int drconn = 0;
                        while (dr.Read())
                        {
                            drconn++;
                            sysnote += "姓名：" + dr["empname"] + "," + "申請單號：" + dr["bsno"];
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
        //battalog撤回  寄信
        private void cardRevokeMail(SqlDataReader dr)
        {
            NDcommon dbobj = new NDcommon();


            string mailtitle = "", MailContext = "";
            #region battalog撤回  寄信
            string fromaddname = "";
            //using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            //{
            //    fromaddname = dbobj.get_dbvalue(conn, "select empname from employee where empid='" + dr["empid"] + "'");
            //}



            mailtitle = "出差單撤回通知";
            MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body>";
            MailContext = MailContext + "以下為明細資料：<BR>";
            MailContext = MailContext + "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
            MailContext = MailContext + "<tr><td align=right width=130>申請人：</td><td>" + dr["empname"] + "</td></tr>";
            MailContext = MailContext + "<tr><td align=right>出差起迄日期：</td><td>自2016 09 時<BR>至2016 09 時</td></tr>";
            MailContext = MailContext + "<tr><td align=right>出差事由：</td><td>" + dr["blogcomment"] + "</td></tr>";
            MailContext = MailContext + "</table>";
            MailContext = MailContext + "</body></HTML>";

            if (dbobj.get_dbnull2(dr["rolestampid"]) != "")
            {
                using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                {
                    string sql = "select enemail from viewemprole where empstatus <> '4' and enemail<>''"
                        + " and rid in (" + dr["rolestampid"] + ")"
                        + " and comid = '" + (string)Session["comid"] + "'";
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
            }
            #endregion

        }
        #endregion

        #region 出差作業 > 出差單審核

        public ActionResult battacheckmainEdit(battalog chks, string sysflag, int? page, string orderdata, string orderdata1, HttpPostedFileBase logopic1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "blogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qblogstatus = "", qblogsdate = "", qblogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qblogstatus"]))
            {
                qblogstatus = Request["qblogstatus"].Trim();
                ViewBag.qblogstatus = qblogstatus;
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
                    //var data = con.battalog.Where(r => r.blogid == chks.blogid).FirstOrDefault();
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
                    NDcommon dbobj = new NDcommon();
                    battalog col = new battalog();
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        col = con.battalog.Find(chks.blogid);
                    }

                    string sysnote = "";
                    if (dbobj.get_dbnull2(Request["blogstatus"]) == "1")
                    {
                        string tmprolestampid = col.rolestampid;
                        string rolea_1 = col.rolestampidall;
                        string roleall = rolea_1 + "," + tmprolestampid; //'簽核過角色(多個)
                        string billflowid = col.billflowid.ToString();

                        //找出下一個角色是誰
                        string tmprole = dbobj.getnewcheck1("Z", tmprolestampid, roleall, "0", "", billflowid);

                        if (tmprole == "'topman'")
                        {
                            tmprole = "";
                        }
                        string blogstatus = "";
                        if (tmprole == "")
                        {
                            blogstatus = "1";// '己簽核
                        }
                        else
                        {
                            blogstatus = "0";
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
                                blogstatus = "1"; // '己簽核
                            }
                            //'==========================
                        }

                        col.blogstatus = blogstatus;
                        col.rolestampid = tmprole;
                        col.rolestampidall = roleall;
                        col.empstampidall = col.empstampidall + ",'" + (string)Session["empid"] + "'"; //'所有人員帳號
                        col.bmodid = (string)Session["empid"];
                        col.bmoddate = DateTime.Now;
                        col.billtime = col.billtime + "," + DateTime.Now.ToString();

                        if (tmprole != "")
                        {
                            //寄信
                            battacheckmainEditMail(col, tmprole);
                        }
                        else
                        {
                            //沒有下一個承辦人  (己通過)
                            ////資料通過後 搬移到cardreallog
                            //battacheckmainEditMove(col);

                            //(己通過)  寄信
                            battacheckmainEditMailPass(col);
                        }
                        sysnote = "出差單審核通過作業";
                    }
                    else
                    {
                        col.blogstatus = "2";
                        col.bback = chks.bback;
                        col.bmodid = (string)Session["empid"];
                        col.bmoddate = DateTime.Now;
                        col.billtime = col.billtime + "," + DateTime.Now.ToString();

                        //(退回)  寄信
                        battacheckmainEditMailBack(col);
                        sysnote = "出差單退回作業";
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
                    tmpform += "<form name='qfr1' action='/battalog/battacheckmainList' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

                    tmpform += "<input type=hidden id='qblogstatus' name='qblogstatus' value='" + qblogstatus + "'>";
                    tmpform += "<input type=hidden id='qblogsdate' name='qblogsdate' value='" + qblogsdate + "'>";
                    tmpform += "<input type=hidden id='qblogedate' name='qblogedate' value='" + qblogedate + "'>";

                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"" + tmpform };
                }
            }
        }

        private void battacheckmainEditMailPass(battalog col)
        {
            NDcommon dbobj = new NDcommon();
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                string fromadd = "", fromaddname = "", toadd = "";
                fromadd = dbobj.get_dbvalue(conn, "select enemail from employee where empid='" + Request["empid"] + "'");
                fromaddname = dbobj.get_dbvalue(conn, "select empname from employee where empid='" + Request["empid"] + "'");
                toadd = dbobj.get_dbvalue(conn, "select enemail from employee where empid='" + col.empid + "'");

                #region 寄送mail給申請人
                string mailtitle = "", MailContext = "";
                mailtitle = "出差單資料已通過核准";
                MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body>";
                MailContext = MailContext + "以下為明細資料：<BR>";
                MailContext = MailContext + "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                MailContext = MailContext + "<tr><td align=right width=130>申請人：</td><td>" + col.empname + "</td></tr>";
                //出差起迄日期
                string SEDate = "自{0}({1}時)<BR>至{2}({3}時)";
                SEDate = string.Format(SEDate, dbobj.get_dbDate(col.blogsdate, "d"), col.blogstime
                    , dbobj.get_dbDate(col.blogedate, "d"), col.blogetime);
                MailContext = MailContext + "<tr><td align=right>出差起迄日期：</td><td>" + SEDate + "</td></tr>";

                string bloghour = dbobj.get_dbnull2(col.bloghour);
                bloghour = (double.Parse(bloghour) / 8).ToString("0.#");
                MailContext = MailContext + "<tr><td align=right>共計天數：</td><td>" + bloghour + "</td></tr>";
                if (dbobj.get_dbnull2(col.blogcomment) != "")
                {
                    MailContext = MailContext + "<tr><td align=right>出差事由：</td><td>" + col.blogcomment.ToString().Trim().Replace(Environment.NewLine, "<br>") + "</td></tr>";
                }
                else
                {
                    MailContext = MailContext + "<tr><td align=right>出差事由：</td><td>&nbsp;</td></tr>";
                }
                MailContext = MailContext + "</table>";
                MailContext = MailContext + "</body></HTML>";

                dbobj.send_mailfile("", toadd, mailtitle, MailContext, null, null);

                #endregion
            }
        }

        private void battacheckmainEditMailBack(battalog col)
        {
            NDcommon dbobj = new NDcommon();
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                string fromadd = "", fromaddname = "", toadd = "";
                fromadd = dbobj.get_dbvalue(conn, "select enemail from employee where empid='" + Request["empid"] + "'");
                fromaddname = dbobj.get_dbvalue(conn, "select empname from employee where empid='" + Request["empid"] + "'");
                toadd = dbobj.get_dbvalue(conn, "select enemail from employee where empid='" + col.empid + "'");

                #region 寄送mail給申請人
                string mailtitle = "", MailContext = "";
                mailtitle = "出差單資料退回";
                MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body>";
                MailContext = MailContext + "以下為明細資料：<BR>";
                MailContext = MailContext + "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                MailContext = MailContext + "<tr><td align=right width=130>申請人：</td><td>" + col.empname + "</td></tr>";
                //出差起迄日期
                string SEDate = "自{0}({1}時)<BR>至{2}({3}時)";
                SEDate = string.Format(SEDate, dbobj.get_dbDate(col.blogsdate, "d"), col.blogstime
                    , dbobj.get_dbDate(col.blogedate, "d"), col.blogetime);
                MailContext = MailContext + "<tr><td align=right>出差起迄日期：</td><td>" + SEDate + "</td></tr>";

                string bloghour = dbobj.get_dbnull2(col.bloghour);
                bloghour = (double.Parse(bloghour) / 8).ToString("0.#");
                MailContext = MailContext + "<tr><td align=right>共計天數：</td><td>" + bloghour + "</td></tr>";
                if (dbobj.get_dbnull2(col.blogcomment) != "")
                {
                    MailContext = MailContext + "<tr><td align=right>出差事由：</td><td>" + col.blogcomment.ToString().Trim().Replace(Environment.NewLine, "<br>") + "</td></tr>";
                }
                else
                {
                    MailContext = MailContext + "<tr><td align=right>出差事由：</td><td>&nbsp;</td></tr>";
                }
                MailContext = MailContext + "</table>";
                MailContext = MailContext + "</body></HTML>";

                dbobj.send_mailfile("", toadd, mailtitle, MailContext, null, null);

                #endregion
            }
        }

        private void battacheckmainEditMail(battalog col, string tmprole)
        {
            NDcommon dbobj = new NDcommon();
            #region 寄給下一個承辦人
            string mailtitle = "", MailContext = "";
            mailtitle = "【" + col.empname + "】出差單資料要求審核通知";
            MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body>";
            MailContext = MailContext + "以下為明細資料：<BR>";
            MailContext = MailContext + "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
            MailContext = MailContext + "<tr><td align=right width=130>申請人：</td><td>" + col.empname + "</td></tr>";
            //出差起迄日期
            string SEDate = "自{0}({1}時)<BR>至{2}({3}時)";
            SEDate = string.Format(SEDate, dbobj.get_dbDate(col.blogsdate, "d"), col.blogstime
                , dbobj.get_dbDate(col.blogedate, "d"), col.blogetime);
            MailContext = MailContext + "<tr><td align=right>出差起迄日期：</td><td>" + SEDate + "</td></tr>";

            string bloghour = dbobj.get_dbnull2(col.bloghour);
            bloghour = (double.Parse(bloghour) / 8).ToString("0.#");
            MailContext = MailContext + "<tr><td align=right>共計天數：</td><td>" + bloghour + "</td></tr>";
            if (dbobj.get_dbnull2(col.blogcomment) != "")
            {
                MailContext = MailContext + "<tr><td align=right>出差事由：</td><td>" + col.blogcomment.ToString().Trim().Replace(Environment.NewLine, "<br>") + "</td></tr>";
            }
            else
            {
                MailContext = MailContext + "<tr><td align=right>出差事由：</td><td>&nbsp;</td></tr>";
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


        public ActionResult battacheckmainList(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "blogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qblogstatus = "", qblogsdate = "", qblogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qblogstatus"]))
            {
                qblogstatus = Request["qblogstatus"].Trim();
                ViewBag.qblogstatus = qblogstatus;
            }

            NDcommon dbobj = new NDcommon();

            qblogsdate = Request["qblogsdate"];
            ViewBag.qblogsdate = qblogsdate;
            qblogedate = Request["qblogedate"];
            ViewBag.qblogedate = qblogedate;
         


            IPagedList<battalog> result;
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

               // string sqlstr = "SELECT * FROM battalog where blogtype='1' and blogstatus in ('0','1','3')";
                string sqlstr = "SELECT * FROM battalog where blogtype='1' and blogstatus in ('0')";
                if (sql_1 != "")
                {
                    sqlstr += " and rolestampid in (" + sql_1 + ")";
                }
                if (qblogstatus != "")
                {
                    sqlstr += " and blogstatus = '" + qblogstatus + "'";
                }

                if (!string.IsNullOrEmpty(qblogsdate)&& !string.IsNullOrEmpty(qblogedate))
                {
                    sqlstr += " and (( blogsdate >= '" + qblogsdate + "' and blogsdate <= '" + qblogedate + "' ) or "
                        + "( blogedate >= '" + qblogsdate + "' and blogedate <= '" + qblogedate + "'))";
                }

                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.battalog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<battalog>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch2(orderdata, orderdata1);
            return View(result);
        }
        private string SetOrder_ch2(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "empname", "blogsdate" };
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

        #region 出差作業 > 出差明細查詢

        public ActionResult battalogqryEdit(battalog chks, string sysflag, int? page, string orderdata, string orderdata1, HttpPostedFileBase logopic1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "blogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qblogstatus = "", qblogsdate = "", qblogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qblogstatus"]))
            {
                qblogstatus = Request["qblogstatus"].Trim();
                ViewBag.qblogstatus = qblogstatus;
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
                    //var data = con.battalog.Where(r => r.blogid == chks.blogid).FirstOrDefault();
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
                    tmpform += "<form name='qfr1' action='/battalog/battalogqryList' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

                    tmpform += "<input type=hidden id='qblogstatus' name='qblogstatus' value='" + qblogstatus + "'>";
                    tmpform += "<input type=hidden id='qblogsdate' name='qblogsdate' value='" + qblogsdate + "'>";
                    tmpform += "<input type=hidden id='qblogedate' name='qblogedate' value='" + qblogedate + "'>";

                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"" + tmpform };
                }
            }
        }


        public ActionResult battalogqryList(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "blogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qblogstatus = "", qblogsdate = "", qblogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qblogstatus"]))
            {
                qblogstatus = Request["qblogstatus"].Trim();
                ViewBag.qblogstatus = qblogstatus;
            }

            NDcommon dbobj = new NDcommon();

            string DateEx = "", DateEx1 = "";
            dbobj.get_dateRang(Request["qblogsdate"], "m", "min", @"出差日期起格式錯誤!!\n", out qblogsdate, out DateEx);
            ViewBag.qblogsdate = qblogsdate;
            dbobj.get_dateRang(Request["qblogedate"], "m", "max", @"出差日期訖格式錯誤!!\n", out qblogedate, out DateEx1);
            ViewBag.qblogedate = qblogedate;
            DateEx += DateEx1;

            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }


            IPagedList<battalog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                //多個角色時
                //string tmpplay = (string)Session["mplayrole"];
                //tmpplay = tmpplay.Replace("'", "");
                //string[] tmpa = tmpplay.Split(',');
                //string sql_1 = "";
                //foreach (string s in tmpa)
                //{
                //    sql_1 += "'''" + s + "''',";
                //}
                //sql_1 = sql_1.Substring(0, sql_1.Length - 1);
                //====

                string sqlstr = "SELECT * FROM battalog where blogtype='1' and (blogtype='1' or (blogtype='2' and (pbsno='' or pbsno is null )))"
                    + " and empid='" + (string)Session["empid"] + "'"
                    + " and comid='" + (string)Session["comid"] + "'";

                //if (sql_1 != "")
                //{
                //    sqlstr += " and rolestampid in (" + sql_1 + ")";
                //}
                if (qblogstatus != "")
                {
                    sqlstr += " and blogstatus = '" + qblogstatus + "'";
                }
                if (qblogsdate != "" && qblogedate != "")
                {
                    sqlstr += " and (( blogsdate >= '" + qblogsdate + "' and blogsdate <= '" + qblogedate + "' ) or "
                        + "( blogedate >= '" + qblogsdate + "' and blogedate <= '" + qblogedate + "'))";
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
            string[] od_ch = { "empname", "blogsdate" };
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


    }
}
