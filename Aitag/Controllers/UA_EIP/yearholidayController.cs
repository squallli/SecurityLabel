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
    public class yearholidayController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /yearholiday/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.Msid = Session["Msid"].ToString();
        //    yearholiday col = new yearholiday();
        //    return View(col);
        //}

        //[HttpPost]
        public ActionResult add(yearholiday col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "yhid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qyhid = "", qyhtitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qyhid"]))
            {
                qyhid = Request["qyhid"].Trim();
                ViewBag.qyhid = qyhid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qyhtitle"]))
            {

                qyhtitle = Request["qyhtitle"].Trim();
                ViewBag.qyhtitle = qyhtitle;
            }

            if (sysflag != "A")
            {             
                yearholiday newcol = new yearholiday();
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
                    string sqlstr = "select yhid from yearholiday where yhid = '" + col.yhid + "'";
                    sqlsmd.CommandText = sqlstr;
                    dr = sqlsmd.ExecuteReader();

                    if (dr.Read())
                    {

                        ModelState.AddModelError("", "假勤代碼重覆，請重新填寫!");
                        return View(col);
                    }
                    dr.Close();
                    dr.Dispose();
                    sqlsmd.Dispose();
                    conn.Close();
                    conn.Dispose();


                    col.bmodid = Session["tempid"].ToString();
                    col.bmoddate = DateTime.Now;            
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.yearholiday.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "年假代碼:" + col.yhid + "年假名稱:" + col.yhtitle;     
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/yearholiday/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qyhid' name='qyhid' value='" + qyhid + "'>";
                    tmpform += "<input type=hidden id='qyhtitle' name='qyhtitle' value='" + qyhtitle + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                   // return RedirectToAction("List");

                }
            }

           
        }

    
        [ValidateInput(false)]
        public ActionResult Edit(yearholiday chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "yhid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qyhid = "", qyhtitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qyhid"]))
            {
                qyhid = Request["qyhid"].Trim();
                ViewBag.qyhid = qyhid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qyhtitle"]))
            {

                qyhtitle = Request["qyhtitle"].Trim();
                ViewBag.qyhtitle = qyhtitle;
            }
            
            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.yearholiday.Where(r => r.yhid == chks.yhid).FirstOrDefault();
                    yearholiday eyearholidays = con.yearholiday.Find(chks.yhid);
                    if (eyearholidays == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eyearholidays);
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

                    //string oldmsid = Request["oldmsid"];                 
        
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                     
                        NDcommon dbobj = new NDcommon();
                        chks.bmodid = Session["tempid"].ToString();
                        chks.bmoddate = DateTime.Now;
                        con.Entry(chks).State = EntityState.Modified;
                        con.SaveChanges();

                        
                        //系統LOG檔
                        //================================================= //                     
                         SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                         string sysrealsid = Request["sysrealsid"].ToString();
                         string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                         string sysnote = "年假代碼:" + chks.yhid + "年假名稱:" + chks.yhtitle;     
                         dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                         sysconn.Close();
                         sysconn.Dispose();
                        //=================================================

                         string tmpform="";
                         tmpform = "<body onload=qfr1.submit();>";
                         tmpform += "<form name='qfr1' action='/yearholiday/List' method='post'>";
                         tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                         tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                         tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                         tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                         tmpform += "<input type=hidden id='qyhid' name='qyhid' value='" + qyhid + "'>";
                         tmpform += "<input type=hidden id='qyhtitle' name='qyhtitle' value='" + qyhtitle + "'>";
                         tmpform +="</form>";
                         tmpform +="</body>";


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
            { orderdata = "yhid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qyhid = "", qyhtitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qyhid"]))
            {
                qyhid = Request["qyhid"].Trim();
                ViewBag.qyhid = qyhid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qyhtitle"]))
            {

                qyhtitle = Request["qyhtitle"].Trim();
                ViewBag.qyhtitle = qyhtitle;
            }

            IPagedList<yearholiday> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from yearholiday where";
                if (qyhid != "")
                    sqlstr += " yhid like '%" + qyhid + "%'  and";
                if (qyhtitle != "")
                    sqlstr += " yhtitle like '%" + qyhtitle + "%'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.yearholiday.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<yearholiday>(page.Value - 1, (int)Session["pagesize"]);
            
            }
            return View(result);
        }

     


        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qyhid = "", qyhtitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qyhid"]))
            {
                qyhid = Request["qyhid"].Trim();
                ViewBag.qyhid = qyhid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qyhtitle"]))
            {

                qyhtitle = Request["qyhtitle"].Trim();
                ViewBag.qyhtitle = qyhtitle;
            }
            string cdel = Request["cdel"];

            if (string.IsNullOrWhiteSpace(cdel))
            {

                string tgourl = "/yearholiday/List?page=" + page + "&qyhid=" + qyhid + "&qyhtitle=" + qyhtitle;
                return new ContentResult() { Content = @"<script>alert('請勾選要刪除的資料!!');window.history.go(-1);</script>" };
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
                            string eyearholidays = dbobj.get_dbvalue(conn1, "select yhtitle from yearholiday where yhid ='" + condtionArr[i].ToString() + "'");

                            sysnote += "假勤名稱:" + eyearholidays + "，序號:" + condtionArr[i].ToString() + "<br>";

                            dbobj.dbexecute("Aitag_DBContext", "DELETE FROM yearholiday where yhid = '" + condtionArr[i].ToString() + "'");
                           
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
                        string tgourl = "/yearholiday/List?page=" + page +"&qyhid=" + qyhid + "&qyhtitle=" + qyhtitle;
                        return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                        //return RedirectToAction("List");
                    }
            }
        }

        [ActionName("detdel")]
        public ActionResult detdelConfirmed(string id, int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "yhid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qyhid = "", qyhtitle = "", yhid="";
            if (!string.IsNullOrWhiteSpace(Request["qyhid"]))
            {
                qyhid = Request["qyhid"].Trim();
                ViewBag.qyhid = qyhid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qyhtitle"]))
            {

                qyhtitle = Request["qyhtitle"].Trim();
                ViewBag.qyhtitle = qyhtitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["yhid"]))
            {

                yhid = Request["yhid"].Trim();
                ViewBag.yhid = yhid;
            }
            
            
                using (Aitag_DBContext con = new Aitag_DBContext())
                {

                    NDcommon dbobj = new NDcommon();
                    SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext");
                    string sysnote = "";

                    string yhid1 = dbobj.get_dbvalue(conn1, "select yhid from yearhddet where hid ='" + Request["hid"].ToString() + "' and comid='"+ Session["comid"].ToString() +"'");
                    string hdayid1 = dbobj.get_dbvalue(conn1, "select hdayid from yearhddet where hid ='" + Request["hid"].ToString() + "' and comid='" + Session["comid"].ToString() + "'");

                    sysnote += "年假代碼:" + yhid1 + "，假別代碼:" + hdayid1 + "<br>";

                        dbobj.dbexecute("Aitag_DBContext", "DELETE FROM yearhddet where hid = '" + Request["hid"].ToString() + "'");

                   

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
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/yearholiday/Edit' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qyhid' name='qyhid' value='" + qyhid + "'>";
                    tmpform += "<input type=hidden id='qyhtitle' name='qyhtitle' value='" + qyhtitle + "'>";
                    tmpform += "<input type=hidden id='yhid' name='yhid' value='" + yhid + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";
                    return new ContentResult() { Content = @"<script>alert('刪除成功!!')</script>" + tmpform };
                    //return RedirectToAction("List");
                }
            
        }

        [ActionName("yeardel")]
        public ActionResult yeardelConfirmed(string id, int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "yhid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qyhid = "", qyhtitle = "", yhid = "";
            if (!string.IsNullOrWhiteSpace(Request["qyhid"]))
            {
                qyhid = Request["qyhid"].Trim();
                ViewBag.qyhid = qyhid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qyhtitle"]))
            {

                qyhtitle = Request["qyhtitle"].Trim();
                ViewBag.qyhtitle = qyhtitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["yhid"]))
            {

                yhid = Request["yhid"].Trim();
                ViewBag.yhid = yhid;
            }


            using (Aitag_DBContext con = new Aitag_DBContext())
            {

                NDcommon dbobj = new NDcommon();
                SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext");
                string sysnote = "";

                string yhid1 = dbobj.get_dbvalue(conn1, "select yhid from yearholidaydet where hdid ='" + Request["hdid"].ToString() + "' and comid='" + Session["comid"].ToString() + "'");
                string yhsyear = dbobj.get_dbvalue(conn1, "select yhsyear from yearholidaydet where hdid ='" + Request["hdid"].ToString() + "' and comid='" + Session["comid"].ToString() + "'");

                sysnote += "年假代碼:" + yhid1 + "，假別代碼:" + yhsyear + "<br>";

                dbobj.dbexecute("Aitag_DBContext", "DELETE FROM yearholidaydet where hdid = '" + Request["hdid"].ToString() + "'");



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
                string tmpform = "";
                tmpform = "<body onload=qfr1.submit();>";
                tmpform += "<form name='qfr1' action='/yearholiday/Edit' method='post'>";
                tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                tmpform += "<input type=hidden id='qyhid' name='qyhid' value='" + qyhid + "'>";
                tmpform += "<input type=hidden id='qyhtitle' name='qyhtitle' value='" + qyhtitle + "'>";
                tmpform += "<input type=hidden id='yhid' name='yhid' value='" + yhid + "'>";
                tmpform += "</form>";
                tmpform += "</body>";
                return new ContentResult() { Content = @"<script>alert('刪除成功!!')</script>" + tmpform };
                //return RedirectToAction("List");
            }

        }

        public ActionResult yearadd(yearholidaydet col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "yhid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qyhid = "", qyhtitle = "", yhid="";
            if (!string.IsNullOrWhiteSpace(Request["qyhid"]))
            {
                qyhid = Request["qyhid"].Trim();
                ViewBag.qyhid = qyhid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qyhtitle"]))
            {

                qyhtitle = Request["qyhtitle"].Trim();
                ViewBag.qyhtitle = qyhtitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["yhid"]))
            {

                yhid = Request["yhid"].Trim();
                ViewBag.yhid = yhid;
            }


            if (sysflag != "A")
            {
                yearholidaydet newcol = new yearholidaydet();
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
                    string sqlstr = "select * from yearholidaydet where yhid = '" + col.yhid + "' and comid='" + Session["comid"] + "' and yhsyear='" + col.yheyear + "'";
                    sqlsmd.CommandText = sqlstr;
                    dr = sqlsmd.ExecuteReader();

                    if (dr.Read())
                    {

                        ModelState.AddModelError("", "年資資料重覆，請重新填寫!");
                        return View(col);
                    }
                    dr.Close();
                    dr.Dispose();
                    sqlsmd.Dispose();
                    conn.Close();
                    conn.Dispose();

                    col.comid = Session["comid"].ToString();
                    col.bmodid = Session["tempid"].ToString();
                    col.bmoddate = DateTime.Now;
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.yearholidaydet.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "年假代碼:" + col.yhid + "年資起：:" + col.yhsyear + "年資迄:" + col.yheyear;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/yearholiday/Edit' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qyhid' name='qyhid' value='" + qyhid + "'>";
                    tmpform += "<input type=hidden id='qyhtitle' name='qyhtitle' value='" + qyhtitle + "'>";
                    tmpform += "<input type=hidden id='yhid' name='yhid' value='" + yhid + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    // return RedirectToAction("List");

                }
            }


        }

        public ActionResult detadd(yearhddet col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "yhid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qyhid = "", qyhtitle = "", yhid = "";
            if (!string.IsNullOrWhiteSpace(Request["qyhid"]))
            {
                qyhid = Request["qyhid"].Trim();
                ViewBag.qyhid = qyhid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qyhtitle"]))
            {

                qyhtitle = Request["qyhtitle"].Trim();
                ViewBag.qyhtitle = qyhtitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["yhid"]))
            {

                yhid = Request["yhid"].Trim();
                ViewBag.yhid = yhid;
            }


            if (sysflag != "A")
            {
                yearhddet newcol = new yearhddet();
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
                    string sqlstr = "select * from yearhddet where yhid = '" + col.yhid + "' and hdayid='" + col.hdayid + "'";
                    sqlsmd.CommandText = sqlstr;
                    dr = sqlsmd.ExecuteReader();

                    if (dr.Read())
                    {

                        ModelState.AddModelError("", "假勤代碼重覆，請重新填寫!");
                        return View(col);
                    }
                    dr.Close();
                    dr.Dispose();
                    sqlsmd.Dispose();
                    conn.Close();
                    conn.Dispose();

                    col.comid = Session["comid"].ToString();
                    col.bmodid = Session["tempid"].ToString();
                    col.bmoddate = DateTime.Now;
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.yearhddet.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "年假代碼:" + col.yhid + "假別代碼:" + col.hdayid + "發放時數:" + col.allhour;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/yearholiday/Edit' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qyhid' name='qyhid' value='" + qyhid + "'>";
                    tmpform += "<input type=hidden id='qyhtitle' name='qyhtitle' value='" + qyhtitle + "'>";
                    tmpform += "<input type=hidden id='yhid' name='yhid' value='" + yhid + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    // return RedirectToAction("List");

                }
            }


        }

        [ValidateInput(false)]
        public ActionResult detedit(yearhddet chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "hid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qyhid = "", qyhtitle = "", yhid="";
            if (!string.IsNullOrWhiteSpace(Request["qyhid"]))
            {
                qyhid = Request["qyhid"].Trim();
                ViewBag.qyhid = qyhid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qyhtitle"]))
            {

                qyhtitle = Request["qyhtitle"].Trim();
                ViewBag.qyhtitle = qyhtitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["yhid"]))
            {

                yhid = Request["yhid"].Trim();
                ViewBag.yhid = yhid;
            }

            if (sysflag != "Q")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.yearhddet.Where(r => r.hid == chks.hid).FirstOrDefault();
                    yearhddet eyearholidays = con.yearhddet.Find(chks.hid);
                    if (eyearholidays == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eyearholidays);
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

                    //string oldmsid = Request["oldmsid"];                 

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {

                        NDcommon dbobj = new NDcommon();
                        chks.comid = Session["comid"].ToString();
                        chks.bmodid = Session["tempid"].ToString();
                        chks.bmoddate = DateTime.Now;
                        con.Entry(chks).State = EntityState.Modified;
                        con.SaveChanges();


                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "年假代碼:" + chks.yhid + "假別代碼:" + chks.hdayid + "發放時數:" + chks.allhour;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/yearholiday/Edit' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                        tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                        tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                        tmpform += "<input type=hidden id='qyhid' name='qyhid' value='" + qyhid + "'>";
                        tmpform += "<input type=hidden id='qyhtitle' name='qyhtitle' value='" + qyhtitle + "'>";
                        tmpform += "<input type=hidden id='yhid' name='yhid' value='" + yhid + "'>";
                        tmpform += "</form>";
                        tmpform += "</body>";


                        return new ContentResult() { Content = @"" + tmpform };
                        //return RedirectToAction("List");
                    }
                }
            }

        }

        [ValidateInput(false)]
        public ActionResult yearedit(yearholidaydet chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "hdid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qyhid = "", qyhtitle = "", yhid = "";
            if (!string.IsNullOrWhiteSpace(Request["qyhid"]))
            {
                qyhid = Request["qyhid"].Trim();
                ViewBag.qyhid = qyhid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qyhtitle"]))
            {

                qyhtitle = Request["qyhtitle"].Trim();
                ViewBag.qyhtitle = qyhtitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["yhid"]))
            {

                yhid = Request["yhid"].Trim();
                ViewBag.yhid = yhid;
            }

            if (sysflag != "Q")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.yearholidaydet.Where(r => r.hdid == chks.hdid).FirstOrDefault();
                    yearholidaydet eyearholidays = con.yearholidaydet.Find(chks.hdid);
                    if (eyearholidays == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eyearholidays);
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

                    //string oldmsid = Request["oldmsid"];                 

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {

                        NDcommon dbobj = new NDcommon();
                        chks.comid = Session["comid"].ToString();
                        chks.bmodid = Session["tempid"].ToString();
                        chks.bmoddate = DateTime.Now;
                        con.Entry(chks).State = EntityState.Modified;
                        con.SaveChanges();


                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "年假代碼:" + chks.yhid + "年資起：" + chks.yhsyear + "年資迄：" + chks.yheyear;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/yearholiday/Edit' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                        tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                        tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                        tmpform += "<input type=hidden id='qyhid' name='qyhid' value='" + qyhid + "'>";
                        tmpform += "<input type=hidden id='qyhtitle' name='qyhtitle' value='" + qyhtitle + "'>";
                        tmpform += "<input type=hidden id='yhid' name='yhid' value='" + yhid + "'>";
                        tmpform += "</form>";
                        tmpform += "</body>";


                        return new ContentResult() { Content = @"" + tmpform };
                        //return RedirectToAction("List");
                    }
                }
            }

        }


    }
}
