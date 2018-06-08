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
    public class sublevel1Controller : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /sublevel1/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.Msid = Session["Msid"].ToString();
        //    sublevel1 col = new sublevel1();
        //    return View(col);
        //}

        //[HttpPost]
        public ActionResult add(sublevel1 col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "corder"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qsublevelname = "";
            if (!string.IsNullOrWhiteSpace(Request["qsublevelname"]))
            {
                qsublevelname = Request["qsublevelname"].Trim();
                ViewBag.qsublevelname = qsublevelname;
            }
            

            if (sysflag != "A")
            {             
                sublevel1 newcol = new sublevel1();
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
                    string sqlstr = "select * from sublevel1 where 1<>1 ";
                    sqlsmd.CommandText = sqlstr;
                    dr = sqlsmd.ExecuteReader();

                    if (dr.Read())
                    {

                        ModelState.AddModelError("", "重複!");
                        return View(col);
                    }
                    dr.Close();
                    dr.Dispose();
                    sqlsmd.Dispose();
                    conn.Close();
                    conn.Dispose();

                    col.lid = "2";
                    col.uplink = Int32.Parse(Request["psid"]);
                    col.counttype = "00"; 
                    col.subread = "2"; //全部
                    col.subadd = "2"; //全部
                    col.submod = "2"; //個人
                    col.subdel = "2"; //個人
                    col.comid = Session["comid"].ToString();
                    col.BMODID = Session["tempid"].ToString();
                    col.BMODDATE = DateTime.Now;            
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.sublevel1.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "主目錄名稱：" + col.sublevelname;     
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/sublevel1/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qsublevelname' name='qsublevelname' value='" + qsublevelname + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";

                    string tgourl = "/sublevel1/List?page=" + page + "&qsublevelname=" + qsublevelname;
                    return new ContentResult() { Content = @"<script>alert('新增成功!!');location.href='" + tgourl + "'</script>" };
                   // return RedirectToAction("List");

                }
            }

           
        }

    
        [ValidateInput(false)]
        public ActionResult Edit(sublevel1 chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "corder"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qsublevelname = "";
            if (!string.IsNullOrWhiteSpace(Request["qsublevelname"]))
            {
                qsublevelname = Request["qsublevelname"].Trim();
                ViewBag.qsublevelname = qsublevelname;
            }

            NDcommon dbobj = new NDcommon();          
            string tmpsid = dbobj.checknumber(Request["tmpsid"]);
            int tmpsid1 = 0;
            if (!string.IsNullOrEmpty(tmpsid))
            { tmpsid1 = int.Parse(tmpsid); }

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.sublevel1.Where(r => r.sid == tmpsid1).FirstOrDefault();
                    sublevel1 esublevel1s = con.sublevel1.Find(tmpsid1);
                    if (esublevel1s == null)
                    {
                        return HttpNotFound();
                    }
                    return View(esublevel1s);
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
                        chks.sid = tmpsid1;
                        chks.lid = "2";
                        chks.counttype = "00";
                        chks.uplink = Int32.Parse(Request["psid"]);
                        chks.comid = Session["comid"].ToString();
                        chks.BMODID = Session["tempid"].ToString();
                        chks.BMODDATE = DateTime.Now;
                        con.Entry(chks).State = EntityState.Modified;
                        con.SaveChanges();

                        
                        //系統LOG檔
                        //================================================= //                     
                         SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                         string sysrealsid = Request["sysrealsid"].ToString();
                         string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                         string sysnote = "主目錄名稱：" + chks.sublevelname;     
                         dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                         sysconn.Close();
                         sysconn.Dispose();
                        //=================================================

                         string tmpform="";
                         tmpform = "<body onload=qfr1.submit();>";
                         tmpform += "<form name='qfr1' action='/sublevel1/List' method='post'>";
                         tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                         tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                         tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                         tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                         tmpform += "<input type=hidden id='qsublevelname' name='qsublevelname' value='" + qsublevelname + "'>";
                         tmpform +="</form>";
                         tmpform +="</body>";

                         return new ContentResult() { Content = @"<script>alert('修改成功!!');</script>" + tmpform };                    
                    }
               }
            }
         
        }

        public ActionResult List(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "corder"; }

            
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qsublevelname = "";
            if (!string.IsNullOrWhiteSpace(Request["qsublevelname"]))
            {
                qsublevelname = Request["qsublevelname"].Trim();
                ViewBag.qsublevelname = qsublevelname;
            }
            string qmtid = "";
            if (!string.IsNullOrWhiteSpace(Request["qmtid"]))
            {
                qmtid = Request["qmtid"].Trim();
                ViewBag.qmtid = qmtid;
            }

            IPagedList<sublevel1> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from sublevel1 where lid = '2'   and";
                if (qsublevelname != "")
                    sqlstr += " sublevelname like '%" + qsublevelname + "%'  and";
                if (qmtid != "")
                    sqlstr += " mtid = '" + qmtid + "'  and";                

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.sublevel1.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<sublevel1>(page.Value - 1, (int)Session["pagesize"]);
            
            }
            return View(result);
        }

     


        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qsublevelname = "";
            if (!string.IsNullOrWhiteSpace(Request["qsublevelname"]))
            {
                qsublevelname = Request["qsublevelname"].Trim();
                ViewBag.qsublevelname = qsublevelname;
            }
            
            string cdel = Request["cdel"];

            if (string.IsNullOrWhiteSpace(cdel))
            {
         
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
                            string esid = dbobj.get_dbvalue(conn1, "select sid from sublevel1 where sid ='" + condtionArr[i].ToString() + "'");
                            string esublevelname = dbobj.get_dbvalue(conn1, "select sublevelname from sublevel1 where sid ='" + condtionArr[i].ToString() + "'");

                            sysnote += "表單代碼：" + esid + "，主目錄名稱：" + esublevelname + "<br>";

                            dbobj.dbexecute("Aitag_DBContext", "DELETE FROM sublevel1 where sid = '" + condtionArr[i].ToString() + "'");
                           
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
                        string tgourl = "/sublevel1/List?page=" + page +"&qsublevelname=" + qsublevelname ;
                        return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                        //return RedirectToAction("List");
                    }
            }
        }

         //子目錄
         
        public ActionResult add1(sublevel1 col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "corder"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }

            

            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            
            string qsublevelname1 = "";
            string uplink = "";
            if (!string.IsNullOrWhiteSpace(Request["qsublevelname1"]))
            {
                qsublevelname1 = Request["qsublevelname1"].Trim();
                ViewBag.qsublevelname1 = qsublevelname1;
            }

            if (string.IsNullOrWhiteSpace(Request["uplink"]))
            { uplink = "0"; }
            else
            { uplink = Request["uplink"].Trim(); }
            ViewBag.uplink = uplink;

            if (sysflag != "A")
            {
                sublevel1 newcol = new sublevel1();
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
                                       

                    col.lid = "3";
                    col.uplink = Int32.Parse(uplink);
                    col.counttype = "00";
                    col.subread = "2"; //全部
                    col.subadd = "2"; //全部
                    col.submod = "2"; //個人
                    col.subdel = "2"; //個人
                    col.comid = Session["comid"].ToString();
                    col.BMODID = Session["tempid"].ToString();
                    col.BMODDATE = DateTime.Now;
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.sublevel1.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "主目錄名稱：" + col.sublevelname;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/sublevel1/List1' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden name='uplink' id='uplink' value='" + uplink + "'>";
                    tmpform += "<input type=hidden id='qsublevelname1' name='qsublevelname1' value='" + qsublevelname1 + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"<script>alert('新增成功!!');</script>" + tmpform };                    
                    // return RedirectToAction("List");

                }
            }


        }


        [ValidateInput(false)]
        public ActionResult Edit1(sublevel1 chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "corder"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qsublevelname1 = "";
            string uplink = "0";
            if (!string.IsNullOrWhiteSpace(Request["qsublevelname1"]))
            {
                qsublevelname1 = Request["qsublevelname1"].Trim();
                ViewBag.qsublevelname1 = qsublevelname1;
            }
            if (string.IsNullOrWhiteSpace(Request["uplink"]))
            { uplink = "0"; }
            else
            { uplink = Request["uplink"].Trim(); }
            ViewBag.uplink = uplink;

            NDcommon dbobj = new NDcommon();
            string tmpsid = dbobj.checknumber(Request["tmpsid"]);
            int tmpsid1 = 0;
            if (!string.IsNullOrEmpty(tmpsid))
            { tmpsid1 = int.Parse(tmpsid); }

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.sublevel1.Where(r => r.sid == tmpsid1).FirstOrDefault();
                    sublevel1 esublevel1s = con.sublevel1.Find(tmpsid1);
                    if (esublevel1s == null)
                    {
                        return HttpNotFound();
                    }
                    return View(esublevel1s);
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

                        chks.sid = tmpsid1;
                        chks.lid = "3";
                        chks.counttype = "00";
                        chks.uplink = Int32.Parse(uplink);
                        chks.comid = Session["comid"].ToString();
                        chks.BMODID = Session["tempid"].ToString();
                        chks.BMODDATE = DateTime.Now;
                        con.Entry(chks).State = EntityState.Modified;
                        con.SaveChanges();


                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "主目錄名稱：" + chks.sublevelname;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/sublevel1/List1' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                        tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                        tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                        tmpform += "<input type=hidden name='uplink' id='uplink' value='" + uplink + "'>"; 
                        tmpform += "<input type=hidden id='qsublevelname1' name='qsublevelname1' value='" + qsublevelname1 + "'>";
                        tmpform += "</form>";
                        tmpform += "</body>";

                        return new ContentResult() { Content = @"<script>alert('修改成功!!');</script>" + tmpform }; 

                        //return RedirectToAction("List");
                    }
                }
            }

        }

        public ActionResult List1(int? page, string orderdata, string orderdata1)
        {
            ViewBag.uplink = Int32.Parse(Request["uplink"]);
            
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "corder"; }
            
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qsublevelname1 = "";
            if (!string.IsNullOrWhiteSpace(Request["qsublevelname1"]))
            {
                qsublevelname1 = Request["qsublevelname1"].Trim();
                ViewBag.qsublevelname1 = qsublevelname1;
            }

            IPagedList<sublevel1> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                NDcommon dbobj = new NDcommon();
                SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                ViewBag.uplinkname = dbobj.get_dbvalue(sysconn , "select sublevelname from sublevel1 where sid = '"+Request["uplink"]+"'");
                sysconn.Close();
                sysconn.Dispose();
                string sqlstr = "select * from sublevel1 where lid = '3' and uplink = '" + Request["uplink"] + "'  and ";
                if (qsublevelname1 != "")
                    sqlstr += " sublevelname like '%" + qsublevelname1 + "%'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.sublevel1.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<sublevel1>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }

        [ActionName("Delete1")]
        public ActionResult DeleteConfirmed1(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qsublevelname1 = "";
            string uplink = "";
            if (!string.IsNullOrWhiteSpace(Request["qsublevelname1"]))
            {
                qsublevelname1 = Request["qsublevelname1"].Trim();
                ViewBag.qsublevelname1 = qsublevelname1;
            }
            if (!string.IsNullOrWhiteSpace(Request["uplink"]))
            {
                uplink = Request["uplink"].Trim();
                ViewBag.uplink = uplink;
            }

            string cdel = Request["cdel"];

            if (string.IsNullOrWhiteSpace(cdel))
            {
       
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
                        string esid = dbobj.get_dbvalue(conn1, "select sid from sublevel1 where sid ='" + condtionArr[i].ToString() + "'");
                        string esublevelname = dbobj.get_dbvalue(conn1, "select sublevelname from sublevel1 where sid ='" + condtionArr[i].ToString() + "'");

                        sysnote += "表單代碼：" + esid + "，主目錄名稱：" + esublevelname + "<br>";

                        dbobj.dbexecute("Aitag_DBContext", "DELETE FROM sublevel1 where sid = '" + condtionArr[i].ToString() + "'");

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
                   // string tgourl = "/sublevel1/List1?page=" + page + "&qsublevelname1=" + qsublevelname1 + "&uplink=" + uplink;
                   // return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/sublevel1/List1' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='uplink' id='uplink' value='" + uplink + "'>";
                    tmpform += "<input type=hidden id='qsublevelname1' name='qsublevelname1' value='" + qsublevelname1 + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');</script>" + tmpform };

                    //return RedirectToAction("List");
                }
            }
        }

        public ActionResult subreadwritelist(int? page, string orderdata, string orderdata1)
        {
           
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "srwid"; }

            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qsublevelname1 = "" , tmpsid = "" ;
            if (!string.IsNullOrWhiteSpace(Request["qsublevelname1"]))
            {
                qsublevelname1 = Request["qsublevelname1"].Trim();
                ViewBag.qsublevelname1 = qsublevelname1;
            }

            tmpsid = Request["tmpsid"];
            IPagedList<subreadwrite> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                NDcommon dbobj = new NDcommon();
                SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                ViewBag.uplinkname = dbobj.get_dbvalue(sysconn, "select sublevelname from sublevel1 where sid = '" + tmpsid + "'");
                sysconn.Close();
                sysconn.Dispose();
                string sqlstr = "select * from subreadwrite where sid = '" + tmpsid + "'  and ";
              
                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by srwid";

                var query = con.subreadwrite.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<subreadwrite>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }

        public ActionResult subreadwriteadd(subreadwrite col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "srwid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qsublevelname = "";
            if (!string.IsNullOrWhiteSpace(Request["qsublevelname"]))
            {
                qsublevelname = Request["qsublevelname"].Trim();
                ViewBag.qsublevelname = qsublevelname;
            }


            if (sysflag != "A")
            {
                subreadwrite newcol = new subreadwrite();
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

                    col.dptgroup = Request["dptgroup"];
                    col.empgroup = Request["empgroup"];
                    col.sid = int.Parse(Request["tmpsid"].ToString());
                    col.comid = Session["comid"].ToString();
                    col.BMODID = Session["tempid"].ToString();
                    col.BMODDATE = DateTime.Now;
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.subreadwrite.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "主目錄id：" + Request["tmpsid"];
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/sublevel1/subreadwritelist' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qsublevelname' name='qsublevelname' value='" + qsublevelname + "'>";
                    tmpform += "<input type=hidden id='tmpsid' name='tmpsid' value='" + Request["tmpsid"] + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";

                    string tgourl = "/sublevel1/List?page=" + page + "&qsublevelname=" + qsublevelname;
                    return new ContentResult() { Content = @"<script>alert('新增成功!!');</script>" + tmpform };
                    // return RedirectToAction("List");

                }
            }


        }


        [ValidateInput(false)]
        public ActionResult subreadwriteedit(subreadwrite chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "srwid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qsublevelname = "";
            if (!string.IsNullOrWhiteSpace(Request["qsublevelname"]))
            {
                qsublevelname = Request["qsublevelname"].Trim();
                ViewBag.qsublevelname = qsublevelname;
            }

            NDcommon dbobj = new NDcommon();
            string tmpsid = dbobj.checknumber(Request["tmpsid"]);
            int tmpsid1 = 0;
            int tmpsrwid = int.Parse(Request["srwid"].ToString());
            if (!string.IsNullOrEmpty(tmpsid))
            { tmpsid1 = int.Parse(tmpsid); }

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.subreadwrite.Where(r => r.srwid == tmpsrwid).FirstOrDefault();
                    subreadwrite esublevel1s = con.subreadwrite.Find(tmpsrwid);
                    if (esublevel1s == null)
                    {
                        return HttpNotFound();
                    }
                    return View(esublevel1s);
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
                        chks.sid = int.Parse(Request["tmpsid"].ToString());
                        chks.dptgroup = Request["dptgroup"];
                        chks.empgroup = Request["empgroup"];
                        chks.comid = Session["comid"].ToString();
                        chks.BMODID = Session["tempid"].ToString();
                        chks.BMODDATE = DateTime.Now;
                        con.Entry(chks).State = EntityState.Modified;
                        con.SaveChanges();

                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "調整id：" + Request["tmpsid"].ToString() + "的權限" ;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/sublevel1/subreadwritelist' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                        tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                        tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                        tmpform += "<input type=hidden id='qsublevelname' name='qsublevelname' value='" + qsublevelname + "'>";
                        tmpform += "<input type=hidden name='tmpsid' id='tmpsid' value='" + Request["tmpsid"].ToString() + "'>";
                        tmpform += "</form>";
                        tmpform += "</body>";

                        return new ContentResult() { Content = @"<script>alert('修改成功!!');</script>" + tmpform };
                    }
                }
            }

        }

         public ActionResult subreadwritedel(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qsublevelname1 = "";
            string uplink = "";
            if (!string.IsNullOrWhiteSpace(Request["qsublevelname1"]))
            {
                qsublevelname1 = Request["qsublevelname1"].Trim();
                ViewBag.qsublevelname1 = qsublevelname1;
            }
           
            string cdel = Request["cdel"];

            if (string.IsNullOrWhiteSpace(cdel))
            {

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
                        //string esid = dbobj.get_dbvalue(conn1, "select sid from sublevel1 where sid ='" + condtionArr[i].ToString() + "'");
                        //string esublevelname = dbobj.get_dbvalue(conn1, "select sublevelname from sublevel1 where sid ='" + condtionArr[i].ToString() + "'");

                        sysnote += "主目錄編號：" + Request["tmpsid"].ToString() + "，權限id：" + condtionArr[i].ToString() + "<br>";

                        dbobj.dbexecute("Aitag_DBContext", "DELETE FROM subreadwrite where srwid = '" + condtionArr[i].ToString() + "'");

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
                    // string tgourl = "/sublevel1/List1?page=" + page + "&qsublevelname1=" + qsublevelname1 + "&uplink=" + uplink;
                    // return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/sublevel1/subreadwritelist' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='tmpsid' id='tmpsid' value='" + Request["tmpsid"] + "'>";
                    tmpform += "<input type=hidden id='qsublevelname1' name='qsublevelname1' value='" + qsublevelname1 + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');</script>" + tmpform };

                    //return RedirectToAction("List");
                }
            }
        }


        //權限
         [ValidateInput(false)]
        public ActionResult privsublevel1(sublevel1 chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();       

            NDcommon dbobj = new NDcommon();
            string tmpsid = dbobj.checknumber(Request["tmpsid"]);
            int tmpsid1 = 0;
            if (!string.IsNullOrEmpty(tmpsid))
            { tmpsid1 = int.Parse(tmpsid); }


            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.sublevel1.Where(r => r.sid == tmpsid1).FirstOrDefault();
                    sublevel1 esublevel1s = con.sublevel1.Find(tmpsid1);
                    if (esublevel1s == null)
                    {
                        return HttpNotFound();
                    }
                    return View(esublevel1s);
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
                        string keysid = tmpsid1.ToString();                  

                        string sql = "";
                        sql = "update sublevel1 set ";
                        sql += " subread = '" + Request["subread"] + "', ";
                        sql += " subadd = '" + Request["subadd"] + "', ";
                        sql += " submod = '" + Request["submod"] + "', ";
                        sql += " subdel = '" + Request["subdel"] + "', ";
                        sql += " BMODID = '" + Session["tempid"].ToString() + "', ";
                        sql += " BMODDATE = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                        sql += " where sid = '" + keysid + "' ";

                        dbobj.dbexecute("Aitag_DBContext", sql);

                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "主目錄名稱：" + chks.sublevelname;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=alert('異動權限修改成功！');window.close();>";
                        tmpform += "</body>";
                        return new ContentResult() { Content = @"" + tmpform };
                    
                    }
                }
            }

        }


    }
}
