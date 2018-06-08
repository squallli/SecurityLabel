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
    public class holidaycodeController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /holidaycode/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.Msid = Session["Msid"].ToString();
        //    holidaycode col = new holidaycode();
        //    return View(col);
        //}

        //[HttpPost]
        public ActionResult add(holidaycode col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "hdayid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qhdayid = "", qhdaytitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qhdayid"]))
            {
                qhdayid = Request["qhdayid"].Trim();
                ViewBag.qhdayid = qhdayid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qhdaytitle"]))
            {

                qhdaytitle = Request["qhdaytitle"].Trim();
                ViewBag.qhdaytitle = qhdaytitle;
            }

            if (sysflag != "A")
            {             
                holidaycode newcol = new holidaycode();
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
                    string sqlstr = "select hdayid from holidaycode where hdayid = '" + col.hdayid + "'";
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

                    col.htype = Request["htype"];
                    col.bmodid = Session["tempid"].ToString();
                    col.bmoddate = DateTime.Now;            
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.holidaycode.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "假勤代碼:" + col.hdayid + "假勤名稱:" + col.hdaytitle;     
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/holidaycode/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qhdayid' name='qhdayid' value='" + qhdayid + "'>";
                    tmpform += "<input type=hidden id='qhdaytitle' name='qhdaytitle' value='" + qhdaytitle + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                   // return RedirectToAction("List");

                }
            }

           
        }

    
        [ValidateInput(false)]
        public ActionResult Edit(holidaycode chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "hdayid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qhdayid = "", qhdaytitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qhdayid"]))
            {
                qhdayid = Request["qhdayid"].Trim();
                ViewBag.qhdayid = qhdayid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qhdaytitle"]))
            {

                qhdaytitle = Request["qhdaytitle"].Trim();
                ViewBag.qhdaytitle = qhdaytitle;
            }
            
            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.holidaycode.Where(r => r.hdayid == chks.hdayid).FirstOrDefault();
                    holidaycode eholidaycodes = con.holidaycode.Find(chks.hdayid);
                    if (eholidaycodes == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eholidaycodes);
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
                        chks.htype = Request["htype"];
                        chks.bmodid = Session["tempid"].ToString();
                        chks.bmoddate = DateTime.Now;
                        con.Entry(chks).State = EntityState.Modified;
                        con.SaveChanges();

                        
                        //系統LOG檔
                        //================================================= //                     
                         SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                         string sysrealsid = Request["sysrealsid"].ToString();
                         string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                         string sysnote = "假勤代碼:" + chks.hdayid + "假勤名稱:" + chks.hdaytitle;     
                         dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                         sysconn.Close();
                         sysconn.Dispose();
                        //=================================================

                         string tmpform="";
                         tmpform = "<body onload=qfr1.submit();>";
                         tmpform += "<form name='qfr1' action='/holidaycode/List' method='post'>";
                         tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                         tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                         tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                         tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                         tmpform += "<input type=hidden id='qhdayid' name='qhdayid' value='" + qhdayid + "'>";
                         tmpform += "<input type=hidden id='qhdaytitle' name='qhdaytitle' value='" + qhdaytitle + "'>";
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
            { orderdata = "hdayid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qhdayid = "", qhdaytitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qhdayid"]))
            {
                qhdayid = Request["qhdayid"].Trim();
                ViewBag.qhdayid = qhdayid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qhdaytitle"]))
            {

                qhdaytitle = Request["qhdaytitle"].Trim();
                ViewBag.qhdaytitle = qhdaytitle;
            }

            IPagedList<holidaycode> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from holidaycode where";
                if (qhdayid != "")
                    sqlstr += " hdayid like '%" + qhdayid + "%'  and";
                if (qhdaytitle != "")
                    sqlstr += " hdaytitle like '%" + qhdaytitle + "%'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.holidaycode.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<holidaycode>(page.Value - 1, (int)Session["pagesize"]);
            
            }
            return View(result);
        }

     


        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qhdayid = "", qhdaytitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qhdayid"]))
            {
                qhdayid = Request["qhdayid"].Trim();
                ViewBag.qhdayid = qhdayid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qhdaytitle"]))
            {

                qhdaytitle = Request["qhdaytitle"].Trim();
                ViewBag.qhdaytitle = qhdaytitle;
            }
            string cdel = Request["cdel"];

            if (string.IsNullOrWhiteSpace(cdel))
            {

                string tgourl = "/holidaycode/List?page=" + page + "&qhdayid=" + qhdayid + "&qhdaytitle=" + qhdaytitle;
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
                            string eholidaycodes = dbobj.get_dbvalue(conn1, "select hdaytitle from holidaycode where hdayid ='" + condtionArr[i].ToString() + "'");

                            sysnote += "假勤名稱:" + eholidaycodes + "，序號:" + condtionArr[i].ToString() + "<br>";

                            dbobj.dbexecute("Aitag_DBContext", "DELETE FROM holidaycode where hdayid = '" + condtionArr[i].ToString() + "'");
                           
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
                        string tgourl = "/holidaycode/List?page=" + page +"&qhdayid=" + qhdayid + "&qhdaytitle=" + qhdaytitle;
                        return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                        //return RedirectToAction("List");
                    }
            }
        }



    }
}
