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
using System.IO;
using System.Text;

namespace Aitag.Controllers
{
    [DoAuthorizeFilter]
    public class cardreallogController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /cardreallog/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.crid = Session["crid"].ToString();
        //    cardreallog col = new cardreallog();
        //    return View(col);
        //}

        //[HttpPost]
        public ActionResult add(cardreallog col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
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

            if (sysflag != "A")
            {
                cardreallog newcol = new cardreallog();

                ViewBag.cloghour = ViewBagcloghour("");
                ViewBag.clogmin = ViewBagclogmin("");

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
                    SqlConnection conn = dbobj.get_conn("Aitag_DBContext");
                    SqlDataReader dr;
                    SqlCommand sqlsmd = new SqlCommand();
                    sqlsmd.Connection = conn;
                    string sqlstr = "select crid from cardreallog where crid = '" + col.crid + "'";
                    sqlsmd.CommandText = sqlstr;
                    dr = sqlsmd.ExecuteReader();

                    if (dr.Read())
                    {

                        ModelState.AddModelError("", "權限代碼重複!");
                        return View(col);
                    }
                    dr.Close();
                    dr.Dispose();
                    sqlsmd.Dispose();
                    conn.Close();
                    conn.Dispose();

                    col.clogtime = Request["cloghour"].Trim() + Request["clogmin"].Trim() + "00";
                    col.comid = Session["comid"].ToString();
                    col.tmpcardno = Request["cardno"].Trim();
                    col.tmpdepid = Request["dptidname"].Trim();
                    col.bmodid = Session["tempid"].ToString();
                    col.bmoddate = DateTime.Now;
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.cardreallog.Add(col);
                        try
                        {
                            con.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }

                        

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "申請人：" + col.empname + "<br>刷卡日期：" + col.clogdate + "　" + Request["cloghour"].Trim() + Request["clogmin"].Trim() + "的資料";
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/cardreallog/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

                    tmpform += "<input type=hidden id='qdptid' name='qdptid' value='" + qdptid + "'>";
                    tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
                    tmpform += "<input type=hidden id='qclogsdate' name='qclogsdate' value='" + qclogsdate + "'>";
                    tmpform += "<input type=hidden id='qclogedate' name='qclogedate' value='" + qclogedate + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    // return RedirectToAction("List");

                }
            }


        }

        private string ViewBagcloghour(string cloghour)
        {
            string option = "", selected= "";
            for (int i = 0; i <= 23; i++)
            {
                if (i.ToString("00") == cloghour) { selected = "selected"; } else { selected = ""; }
                option += @"<option value=""" + i.ToString("00") + @""" " + selected + " >" + i.ToString("00") + "</option>";
            }
            return option;
        }
        private string ViewBagclogmin(string clogmin)
        {
            string option = "", selected = "";
            for (int i = 0; i <= 59; i++)
            {
                if (i.ToString("00") == clogmin) { selected = "selected"; } else { selected = ""; }
                option += @"<option value=""" + i.ToString("00") + @""" " + selected + " >" + i.ToString("00") + "</option>";
            }
            return option;
        }

        [ValidateInput(false)]
        public ActionResult Edit(cardreallog chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
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
                    var data = con.cardreallog.Where(r => r.crid == chks.crid).FirstOrDefault();
                    cardreallog ecardreallogs = con.cardreallog.Find(chks.crid);
                    if (ecardreallogs == null)
                    {
                        return HttpNotFound();
                    }


                    ViewBag.cloghour = ViewBagcloghour(ecardreallogs.clogtime.Substring(0, 2));
                    ViewBag.clogmin = ViewBagclogmin(ecardreallogs.clogtime.Substring(2, 2));

                    return View(ecardreallogs);
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

                    //string oldcrid = Request["oldcrid"];                 

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        NDcommon dbobj = new NDcommon();

                        chks.clogtime = Request["cloghour"].Trim() + Request["clogmin"].Trim() + "00";
                        chks.tmpcardno = Request["cardno"].Trim();
                        chks.tmpdepid = Request["dptidname"].Trim();
                        chks.bmodid = Session["tempid"].ToString();
                        chks.bmoddate = DateTime.Now;
                        con.Entry(chks).State = EntityState.Modified;
                        con.SaveChanges();


                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "申請人：" + chks.empname + "<br>刷卡日期：" + chks.clogdate + "　" + Request["cloghour"].Trim() + Request["clogmin"].Trim() + "的資料";
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/cardreallog/List' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                        tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                        tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

                        tmpform += "<input type=hidden id='qdptid' name='qdptid' value='" + qdptid + "'>";
                        tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
                        tmpform += "<input type=hidden id='qclogsdate' name='qclogsdate' value='" + qclogsdate + "'>";
                        tmpform += "<input type=hidden id='qclogedate' name='qclogedate' value='" + qclogedate + "'>";
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

            IPagedList<cardreallog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                //'權限
                //mpriv = session("priv")
                //viewid = getviewpriv(funcpriv(2),mpriv(realsid,2))



                string sqlstr = "SELECT * FROM cardreallog where comid = '" + (string)Session["comid"] + "'   and";
                if (qdptid != "")
                    sqlstr += " dptid = '" + qdptid + "'  and";
                if (qempname != "")
                    sqlstr += " empname like N'%" + qempname + "%'  and";
                if (qclogsdate == "")
                {
                    qclogsdate = DateTime.Now.ToString("yyyy/MM") + "/1";
                    ViewBag.qclogsdate = qclogsdate;
                }
                if (qclogedate == "")
                {
                    var dat = new DateTime(DateTime.Now.Year -1, 12, 31);
                    qclogedate = dat.AddMonths(DateTime.Now.Month).ToString("yyyy/MM/dd");
                    ViewBag.qclogedate = qclogedate;
                }
                string DateEx = "";
                try
                {
                    DateTime.Parse(qclogsdate);
                    sqlstr += " clogdate >= '" + qclogsdate + "'  and";
                }
                catch
                {
                    DateEx += @"刷卡日期起格式錯誤!!\n";
                }
                try
                {
                    DateTime.Parse(qclogedate);
                    sqlstr += " clogdate <= '" + qclogedate + "'  and";
                }
                catch
                {
                    DateEx += @"刷卡日期訖格式錯誤!!\n";
                }
                if (DateEx != "")
                {
                    ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>";
                }

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
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
            string[] od_ch = { "dptid", "empid", "empname", "clogdate" };
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
            tmpform += "<form name='qfr1' action='/cardreallog/List' method='post'>";
            //tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

            tmpform += "<input type=hidden id='qdptid' name='qdptid' value='" + qdptid + "'>";
            tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
            tmpform += "<input type=hidden id='qclogsdate' name='qclogsdate' value='" + qclogsdate + "'>";
            tmpform += "<input type=hidden id='qclogedate' name='qclogedate' value='" + qclogedate + "'>";

            tmpform += "</form>";
            tmpform += "</body>";


            string cdel = Request["cdel"];

            if (string.IsNullOrWhiteSpace(cdel))
            {
                return new ContentResult() { Content = @"<script>alert('請勾選要刪除的資料!!');</script>" + tmpform };
            }
            else
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {

                    NDcommon dbobj = new NDcommon();
                    SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext");
                    string sysnote = "";
                    string[] condtionArr = cdel.Split(',');
                    int condtionLen = condtionArr.Length;
                    for (int i = 0; i < condtionLen; i++)
                    {
                        string ecardreallogs = dbobj.get_dbvalue(conn1, "select empname from cardreallog where crid ='" + condtionArr[i].ToString() + "'");

                        sysnote += "代碼名稱:" + ecardreallogs + "，序號:" + condtionArr[i].ToString() + "<br>";

                        dbobj.dbexecute("Aitag_DBContext", "DELETE FROM cardreallog where crid = '" + condtionArr[i].ToString() + "'");
                     
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
                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');</script>" + tmpform };
                    //return RedirectToAction("List");
                }
            }
        }

        #region 批次產生刷卡資料
        public ActionResult cardbatch(string sysflag)
        {
            ModelState.Clear();

            string sid = "", realsid = "", yhid = "", carddate = "", ctype = "";
            if (!string.IsNullOrWhiteSpace(Request["sid"]))
            {
                sid = Request["sid"].Trim();
                ViewBag.sid = sid;
            }
            if (!string.IsNullOrWhiteSpace(Request["realsid"]))
            {
                realsid = Request["realsid"].Trim();
                ViewBag.realsid = realsid;
            }
            if (!string.IsNullOrWhiteSpace(Request["yhid"]))
            {
                yhid = Request["yhid"].Trim();
                ViewBag.yhid = yhid;
            }
            if (!string.IsNullOrWhiteSpace(Request["carddate"]))
            {
                carddate = Request["carddate"].Trim();
                ViewBag.carddate = DateTime.Parse(carddate);
            }
            if (!string.IsNullOrWhiteSpace(Request["ctype"]))
            {
                ctype = Request["ctype"].Trim();
                ViewBag.ctype = ctype;
            }
            if (sysflag != "A")
            {
                return View();
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }
                else
                {
                    NDcommon dbobj = new NDcommon();
                    //'取得這類人員的上下班時間
                    string ytstime = "", ydetime = "";
                    using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        string sql = "select * from yearholiday where yhid = '" + yhid + "'";
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            SqlDataReader dr = cmd.ExecuteReader();
                            if (dr.HasRows)
                            {
                                dr.Read();
                                ytstime = dbobj.get_dbnull2(dr["ytstime"]);
                                ydetime = dbobj.get_dbnull2(dr["ydetime"]);
                            }
                            dr.Close();
                        }
                    }
                    using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        string sql = "select * from employee where empstatus in ('1','2') and yhid = '" + yhid + "'";
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            SqlDataReader dr = cmd.ExecuteReader();

                            string clogtime = "";
                            if (ctype == "1")
                            {
                                clogtime = ytstime;
                            }
                            else
                            {
                                clogtime = ydetime;
                            }
                            var bmoddate = DateTime.Now;
                            while (dr.Read())
                            {
                                cardreallog col = new cardreallog();

                                col.empid = dbobj.get_dbnull2(dr["empid"]);
                                col.empname = dbobj.get_dbnull2(dr["empname"]);
                                col.dptid = dbobj.get_dbnull2(dr["empworkdepid"]);
                                col.clogdate = ViewBag.carddate;
                                col.clogtime = clogtime;

                                col.comid = Session["comid"].ToString();
                                col.bmodid = Session["tempid"].ToString();
                                col.bmoddate = bmoddate;

                                using (Aitag_DBContext con = new Aitag_DBContext())
                                {
                                    con.cardreallog.Add(col);
                                    con.SaveChanges();
                                }

                            }
                            dr.Close();
                        }
                    }

                    //系統LOG檔 //================================================= //
                    SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    string sysrealsid = Request["sysrealsid"].ToString();
                    string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    string sysnote = "班別代碼：" + yhid + "<br>產生日期：" + carddate + "的資料";
                    dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    sysconn.Close();
                    sysconn.Dispose();
                    //=================================================

                    string tmpform = "";
                    tmpform += "<script>";
                    tmpform += "function SetParentOpener() {";
                    tmpform += "alert('刷卡資料批次產生成功!!');";
                    tmpform += "parent.opener.location.href='/cardreallog/List';";
                    tmpform += "window.close();";
                    tmpform += "}";
                    tmpform += "</script>";

                    tmpform += "<body onload=SetParentOpener();>";
                    tmpform += "</body>";
                    return new ContentResult() { Content = @"" + tmpform };
                }
            }
        }
        #endregion

        #region 中興刷卡資料轉入
        public ActionResult cardlogtransfer1(string sysflag, conbudgetdet col, HttpPostedFileBase upfile)
        {
           // ViewBag.pid = Request["pid"].ToString();

            if (sysflag != "A")
            {
                return View();
            }
            else
            {
                NDcommon dbobj = new NDcommon();
                string errmsg = "";
                if (upfile != null)
                {
                    String sernonum = "";
                    //重新命名，存入檔案
                    DateTime myDate = DateTime.Now;
                    sernonum = myDate.ToString("yyyyMMddHHmmss");
                    string BasicPath = Server.MapPath("~/upload/");
                    string fileName = upfile.FileName.Substring(upfile.FileName.IndexOf("."), upfile.FileName.Length - upfile.FileName.IndexOf("."));

                    if (fileName != ".exe" && fileName != ".asp" && fileName != ".aspx" && fileName != ".jsp" && fileName != ".php")
                    {
                        fileName = "cust-" + sernonum.ToString() + fileName;
                        upfile.SaveAs(Server.MapPath("~/upload/") + fileName);

                        string tmppath = BasicPath + fileName;


                        StreamReader sr = new StreamReader(@tmppath, System.Text.Encoding.Default);
                        string allstr = sr.ReadToEnd(); //從資料流末端存取檔案
                        sr.Close();

                        string[] tmpstridno; //匯入資料

                        allstr = allstr.Replace(Environment.NewLine, "");
                        allstr = allstr.Substring(0, allstr.Length - 1);
                        tmpstridno = allstr.Split('\"');

                        tmpstridno[0] = GetBytesCount(tmpstridno[0]);


                        #region

                        string tempcard = "", tempcard1 = "", tempdatetime = "", tmptime = "";
                        SqlConnection comconn = dbobj.get_conn("Aitag_DBContext");
                        foreach (string tmptxt in tmpstridno)
                        {
                            if (tmptxt != "")
                            {
                                tempcard = tmptxt.Trim().Substring(0, 7);
                                tempcard1 = tmptxt.Trim().Substring(9, 4);
                                tempdatetime = tmptxt.Trim().Substring(17, 8);
                                tmptime = tmptxt.Trim().Substring(27, 5);

                                if (tempcard != "" && tempcard1 != "" && tempdatetime != "" && tmptime != "")
                                {
                                    string date1 = tempdatetime.Substring(0, 4) + "/" + tempdatetime.Substring(4, 2) + "/" + tempdatetime.Substring(6, 2);
                                    tmptime = tmptime.Replace(":", "") + "00";

                                    string rs1_Open = ""; rs1_Open = dbobj.get_dbvalue(comconn, "select * from cardreallog where clogtime = '" + tmptime + "' and clogdate = '" + date1 + "' and tmpcardno = '" + tempcard1 + "'");
                                    if (rs1_Open == "")
                                    {
                                        using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                                        {
                                            string sql = "select empid,empname,empworkdepid,empworkcomp from employee where empno = '" + tempcard1 + "'";
                                            using (SqlCommand cmd = new SqlCommand(sql, conn))
                                            {
                                                SqlDataReader dr = cmd.ExecuteReader();
                                                if (dr.HasRows)
                                                {
                                                    cardreallog rs = new cardreallog();
                                                    dr.Read();

                                                    rs.empid = dr["empid"] + "";
                                                    rs.empname = dr["empname"] + "";
                                                    rs.dptid = dr["empworkdepid"] + "";
                                                    rs.comid = dr["empworkcomp"] + "";
                                                    rs.clogdate = DateTime.Parse(date1);
                                                    rs.clogtime = tmptime;
                                                    rs.tmpcardno = tempcard1;
                                                    rs.tmpdepid = dbobj.get_dbvalue(comconn, "select dpttitle from department where dptid='" + dr["empworkdepid"] + "'");

                                                    using (Aitag_DBContext con = new Aitag_DBContext())
                                                    {
                                                        con.cardreallog.Add(rs);
                                                        con.SaveChanges();
                                                    }
                                                }
                                                else
                                                {
                                                    errmsg += tempcard1 + ",";
                                                }
                                                dr.Close();
                                            }
                                        }



                                    }
                                }
                            }
                        }

                        comconn.Close();
                        comconn.Dispose();
                        #endregion

                    }
                    else
                    {
                        ViewBag.AddModelError = @"alert('上傳格式錯誤！');";
                        return View();
                    }

                }


                string tmpform = "";
                if (errmsg != "")
                {

                    tmpform += "<script>";
                    tmpform += "function SetParentOpener() {";
                    tmpform += "alert('以下員編尚未轉檔，請確認!!" + errmsg + "');";
                    tmpform += "parent.opener.location.href='/cardreallog/List';";
                    tmpform += "window.close();";
                    tmpform += "}";
                    tmpform += "</script>";
                    tmpform += "<body onload=SetParentOpener();>";
                    //tmpform += errmsg;
                    tmpform += "</body>";
                }
                else
                {
                    tmpform += "<script>";
                    tmpform += "function SetParentOpener() {";
                    tmpform += "alert('轉檔成功!!');";
                    tmpform += "parent.opener.location.href='/cardreallog/List';";
                    tmpform += "window.close();";
                    tmpform += "}";
                    tmpform += "</script>";
                    tmpform += "<body onload=SetParentOpener();>";
                    tmpform += "</body>";
                }


                return new ContentResult() { Content = @"" + tmpform };
            }
        }

        private string GetBytesCount(string p)
        {
            ASCIIEncoding ascii = new ASCIIEncoding();
            Byte[] encodedBytes = ascii.GetBytes(p);

            p = ascii.GetString(encodedBytes);

            return p.Replace("?", "8");
        }

        #endregion

        #region 聯準刷卡資料轉入
        public ActionResult cardlogtransfer3(string sysflag, conbudgetdet col, HttpPostedFileBase upfile)
        {
            // ViewBag.pid = Request["pid"].ToString();

            if (sysflag != "A")
            {
                return View();
            }
            else
            {
                NDcommon dbobj = new NDcommon();
                string errmsg = "";
                if (upfile != null)
                {
                    String sernonum = "";
                    //重新命名，存入檔案
                    DateTime myDate = DateTime.Now;
                    sernonum = myDate.ToString("yyyyMMddHHmmss");
                    string BasicPath = Server.MapPath("~/upload/");
                    string fileName = upfile.FileName.Substring(upfile.FileName.IndexOf("."), upfile.FileName.Length - upfile.FileName.IndexOf("."));

                    if (fileName != ".exe" && fileName != ".asp" && fileName != ".aspx" && fileName != ".jsp" && fileName != ".php")
                    {
                        fileName = "cust-" + sernonum.ToString() + fileName;
                        upfile.SaveAs(Server.MapPath("~/upload/") + fileName);

                        string tmppath = BasicPath + fileName;


                        StreamReader sr = new StreamReader(@tmppath, System.Text.Encoding.Default);
                        string allstr = sr.ReadToEnd(); //從資料流末端存取檔案
                        sr.Close();

                        string[] tmpstridno; //匯入資料

                        allstr = allstr.Replace(Environment.NewLine, "\"");
                        allstr = allstr.Substring(0, allstr.Length - 1);
                        tmpstridno = allstr.Split('\"');

                        //tmpstridno[0] = GetBytesCount1(tmpstridno[0]);


                        #region

                        string tempcard = "", tempcard1 = "", tempdatetime = "", tmptime = "";
                        SqlConnection comconn = dbobj.get_conn("Aitag_DBContext");
                        foreach (string tmptxt in tmpstridno)
                        {
                            if (tmptxt != "")
                            {
                                tempcard = tmptxt.Trim().Substring(0, 10);
                                tempcard1 = tmptxt.Trim().Substring(11, 4);
                                tempdatetime = tmptxt.Trim().Substring(16, 8);
                                tmptime = tmptxt.Trim().Substring(25, 4);

                                if (tempcard != "" && tempcard1 != "" && tempdatetime != "" && tmptime != "")
                                {
                                    string date1 = tempdatetime.Substring(0, 4) + "/" + tempdatetime.Substring(4, 2) + "/" + tempdatetime.Substring(6, 2);
                                    tmptime = tmptime + "00";

                                    string rs1_Open = ""; rs1_Open = dbobj.get_dbvalue(comconn, "select * from cardreallog where clogtime = '" + tmptime + "' and clogdate = '" + date1 + "' and tmpcardno = '" + tempcard1 + "'");
                                    if (rs1_Open == "")
                                    {
                                        using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                                        {
                                            string sql = "select empid,empname,empworkdepid,empworkcomp from employee where empno = '" + tempcard1 + "'";
                                            using (SqlCommand cmd = new SqlCommand(sql, conn))
                                            {
                                                SqlDataReader dr = cmd.ExecuteReader();
                                                if (dr.HasRows)
                                                {
                                                    cardreallog rs = new cardreallog();
                                                    dr.Read();

                                                    rs.empid = dr["empid"] + "";
                                                    rs.empname = dr["empname"] + "";
                                                    rs.dptid = dr["empworkdepid"] + "";
                                                    rs.comid = dr["empworkcomp"] + "";
                                                    rs.clogdate = DateTime.Parse(date1);
                                                    rs.clogtime = tmptime;
                                                    rs.tmpcardno = tempcard1;
                                                    rs.tmpdepid = dbobj.get_dbvalue(comconn, "select dpttitle from department where dptid='" + dr["empworkdepid"] + "'");

                                                    using (Aitag_DBContext con = new Aitag_DBContext())
                                                    {
                                                        con.cardreallog.Add(rs);
                                                        con.SaveChanges();
                                                    }
                                                }
                                                else
                                                {
                                                    errmsg += tempcard1 + ",";
                                                }
                                                dr.Close();
                                            }
                                        }



                                    }
                                }
                            }
                        }

                        comconn.Close();
                        comconn.Dispose();
                        #endregion

                    }
                    else
                    {
                        ViewBag.AddModelError = @"alert('上傳格式錯誤！');";
                        return View();
                    }

                }


                string tmpform = "";
                if (errmsg != "")
                {

                    tmpform += "<script>";
                    tmpform += "function SetParentOpener() {";
                    tmpform += "alert('以下員編尚未轉檔，請確認!!" + errmsg + "');";
                    tmpform += "parent.opener.location.href='/cardreallog/List';";
                    tmpform += "window.close();";
                    tmpform += "}";
                    tmpform += "</script>";
                    tmpform += "<body onload=SetParentOpener();>";
                    //tmpform += errmsg;
                    tmpform += "</body>";
                }
                else
                {
                    tmpform += "<script>";
                    tmpform += "function SetParentOpener() {";
                    tmpform += "alert('轉檔成功!!');";
                    tmpform += "parent.opener.location.href='/cardreallog/List';";
                    tmpform += "window.close();";
                    tmpform += "}";
                    tmpform += "</script>";
                    tmpform += "<body onload=SetParentOpener();>";
                    tmpform += "</body>";
                }


                return new ContentResult() { Content = @"" + tmpform };
            }
        }

        //private string GetBytesCount1(string p)
        //{
        //    ASCIIEncoding ascii = new ASCIIEncoding();
        //    Byte[] encodedBytes = ascii.GetBytes(p);

        //    p = ascii.GetString(encodedBytes);

        //    return p.Replace("?", "8");
        //}
        #endregion


    }
}
