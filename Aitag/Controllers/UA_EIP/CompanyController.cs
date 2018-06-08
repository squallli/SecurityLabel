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
    public class CompanyController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /Company/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.comid = Session["comid"].ToString();
        //    Company col = new Company();
        //    return View(col);
        //}

        //[HttpPost]
        public ActionResult add(Company col, string sysflag, int? page, string orderdata, string orderdata1, HttpPostedFileBase logopic1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "comid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcomid = "", qcsno = "", qcomtitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qcomid"]))
            {
                qcomid = Request["qcomid"].Trim();
                ViewBag.qcomid = qcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcsno"]))
            {
                qcsno = Request["qcsno"].Trim();
                ViewBag.qcsno = qcsno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomtitle"]))
            {

                qcomtitle = Request["qcomtitle"].Trim();
                ViewBag.qcomtitle = qcomtitle;
            }

            if (sysflag != "A")
            {
                Company newcol = new Company();
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
                    SqlConnection conn = dbobj.get_conn("Aitag_DBContext");
                    SqlDataReader dr;
                    SqlCommand sqlsmd = new SqlCommand();
                    sqlsmd.Connection = conn;
                    string sqlstr = "select comid from Company where comid = '" + col.comid + "'";
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

                    if (logopic1.ContentLength > 0)
                    {
                        col.logopic = Request["comid"] + "." + logopic1.FileName.Substring(logopic1.FileName.Length - 3, 3);
                        logopic1.SaveAs(Server.MapPath("/upload/" + col.logopic));
                    }

                    //密碼加密
                    //col.emppasswd = dbobj.Encrypt(col.emppasswd);
                    //col.comid = col.emppasswd;
                    //col.baddid = Session["tempid"].ToString();
                    col.bmodid = Session["tempid"].ToString();
                    //col.badddate = DateTime.Now;
                    col.bmoddate = DateTime.Now;
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.Company.Add(col);
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
                        string sysnote = "代碼:" + col.comid + "名稱:" + col.comtitle;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/Company/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qcomid' name='qcomid' value='" + qcomid + "'>";
                    tmpform += "<input type=hidden id='qcsno' name='qcsno' value='" + qcsno + "'>";
                    tmpform += "<input type=hidden id='qcomtitle' name='qcomtitle' value='" + qcomtitle + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    // return RedirectToAction("List");

                }
            }


        }


        [ValidateInput(false)]
        public ActionResult Edit(Company chks, string sysflag, int? page, string orderdata, string orderdata1, HttpPostedFileBase logopic1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "comid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcomid = "", qcsno = "", qcomtitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qcomid"]))
            {
                qcomid = Request["qcomid"].Trim();
                ViewBag.qcomid = qcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcsno"]))
            {
                qcsno = Request["qcsno"].Trim();
                ViewBag.qcsno = qcsno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomtitle"]))
            {

                qcomtitle = Request["qcomtitle"].Trim();
                ViewBag.qcomtitle = qcomtitle;
            }

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.Company.Where(r => r.comid == chks.comid).FirstOrDefault();
                    Company eCompanys = con.Company.Find(chks.comid);
                    if (eCompanys == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eCompanys);
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

                    //string oldcomid = Request["oldcomid"];                 

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        NDcommon dbobj = new NDcommon();
                        if (logopic1 != null) { 
                            if (logopic1.ContentLength > 0)
                            {
                                chks.logopic = Request["comid"] + "." + logopic1.FileName.Substring(logopic1.FileName.Length - 3, 3);
                                logopic1.SaveAs(Server.MapPath("/upload/" + chks.logopic));
                            }
                        }

                        chks.comid = Request["comid"].Trim();
                        chks.bmodid = Session["tempid"].ToString();
                        chks.bmoddate = DateTime.Now;
                        con.Entry(chks).State = EntityState.Modified;
                        con.SaveChanges();


                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "代碼:" + chks.comid + "名稱:" + chks.comtitle;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/Company/List' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                        tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                        tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                        tmpform += "<input type=hidden id='qcomid' name='qcomid' value='" + qcomid + "'>";
                        tmpform += "<input type=hidden id='qcsno' name='qcsno' value='" + qcsno + "'>";
                        tmpform += "<input type=hidden id='qcomtitle' name='qcomtitle' value='" + qcomtitle + "'>";
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
            { orderdata = "comid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcomid = "", qcsno = "", qcomtitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qcomid"]))
            {
                qcomid = Request["qcomid"].Trim();
                ViewBag.qcomid = qcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcsno"]))
            {
                qcsno = Request["qcsno"].Trim();
                ViewBag.qcsno = qcsno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomtitle"]))
            {

                qcomtitle = Request["qcomtitle"].Trim();
                ViewBag.qcomtitle = qcomtitle;
            }

            IPagedList<Company> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from Company where";
                if (qcomid != "")
                    sqlstr += " comid like '%" + qcomid + "%'  and";
                if (qcsno != "")
                    sqlstr += " csno like '%" + qcsno + "%'  and";
                if (qcomtitle != "")
                    sqlstr += " comtitle like '%" + qcomtitle + "%'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.Company.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<Company>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch(orderdata, orderdata1);
            return View(result);
        }
        private string SetOrder_ch(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "comid", "comtitle" };
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

            string qcomid = "", qcsno = "", qcomtitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qcomid"]))
            {
                qcomid = Request["qcomid"].Trim();
                ViewBag.qcomid = qcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcsno"]))
            {
                qcsno = Request["qcsno"].Trim();
                ViewBag.qcsno = qcsno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomtitle"]))
            {

                qcomtitle = Request["qcomtitle"].Trim();
                ViewBag.qcomtitle = qcomtitle;
            }


            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/Company/List' method='post'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

            tmpform += "<input type=hidden id='qcomid' name='qcomid' value='" + qcomid + "'>";
            tmpform += "<input type=hidden id='qcsno' name='qcsno' value='" + qcsno + "'>";
            tmpform += "<input type=hidden id='qcomtitle' name='qcomtitle' value='" + qcomtitle + "'>";

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
                        string eCompanys = dbobj.get_dbvalue(conn1, "select comtitle from Company where comid ='" + condtionArr[i].ToString() + "'");

                        sysnote += "代碼名稱:" + eCompanys + "，序號:" + condtionArr[i].ToString() + "<br>";

                        dbobj.dbexecute("Aitag_DBContext", "DELETE FROM Company where comid = '" + condtionArr[i].ToString() + "'");
                     
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



    }
}
