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
    public class landcodeController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /landcode/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.Msid = Session["Msid"].ToString();
        //    landcode col = new landcode();
        //    return View(col);
        //}

        //[HttpPost]
        public ActionResult add(landcode col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "lcode"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qltitle = "", qlcode = "";
            if (!string.IsNullOrWhiteSpace(Request["qltitle"]))
            {
                qltitle = Request["qltitle"].Trim();
                ViewBag.qltitle = qltitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["qlcode"]))
            {

                qlcode = Request["qlcode"].Trim();
                ViewBag.qlcode = qlcode;
            }

            if (sysflag != "A")
            {             
                landcode newcol = new landcode();
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
                    /*
                    SqlConnection conn = dbobj.get_conn("Aitag_DBContext");
                    SqlDataReader dr;
                    SqlCommand sqlsmd = new SqlCommand();
                    sqlsmd.Connection = conn;
                    string sqlstr = "select * from landcode where 1<>1";
                    sqlsmd.CommandText = sqlstr;
                    dr = sqlsmd.ExecuteReader();

                    if (dr.Read())
                    {

                        ModelState.AddModelError("", "no");
                        return View(col);
                    }
                    dr.Close();
                    dr.Dispose();
                    sqlsmd.Dispose();
                    conn.Close();
                    conn.Dispose();
                    */

                    col.comid = Session["comid"].ToString();
                    col.bmodid = Session["tempid"].ToString();
                    //col.badddate = DateTime.Now;
                    col.bmoddate = DateTime.Now;            
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.landcode.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "段名名稱：" + col.ltitle ;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/landcode/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qltitle' name='qltitle' value='" + qltitle + "'>";
                    tmpform += "<input type=hidden id='qlcode' name='qlcode' value='" + qlcode + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                   // return RedirectToAction("List");

                }
            }

           
        }

    
        [ValidateInput(false)]
        public ActionResult Edit(landcode chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "lcode"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qltitle = "", qlcode = "";
            if (!string.IsNullOrWhiteSpace(Request["qltitle"]))
            {
                qltitle = Request["qltitle"].Trim();
                ViewBag.qltitle = qltitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["qlcode"]))
            {

                qlcode = Request["qlcode"].Trim();
                ViewBag.qlcode = qlcode;
            }
            
            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.landcode.Where(r => r.lcode == chks.lcode).FirstOrDefault();
                    landcode elandcodes = con.landcode.Find(chks.lcode);
                    if (elandcodes == null)
                    {
                        return HttpNotFound();
                    }
                    return View(elandcodes);
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
                        //chks.comid = Session["comid"].ToString();
                        chks.bmodid = Session["tempid"].ToString();
                        chks.bmoddate = DateTime.Now;
                        con.Entry(chks).State = EntityState.Modified;
                        con.SaveChanges();

                        
                        //系統LOG檔
                        //================================================= //                     
                         SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                         string sysrealsid = Request["sysrealsid"].ToString();
                         string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                         string sysnote = "段名名稱：" + chks.ltitle;
                         dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                         sysconn.Close();
                         sysconn.Dispose();
                        //=================================================

                         string tmpform="";
                         tmpform = "<body onload=qfr1.submit();>";
                         tmpform += "<form name='qfr1' action='/landcode/List' method='post'>";
                         tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                         tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                         tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                         tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                         tmpform += "<input type=hidden id='qltitle' name='qltitle' value='" + qltitle + "'>";
                         tmpform += "<input type=hidden id='qlcode' name='qlcode' value='" + qlcode + "'>";
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
            { orderdata = "lcode"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qltitle = "", qlcode = "", qvcode = "";
            if (!string.IsNullOrWhiteSpace(Request["qltitle"]))
            {
                qltitle = Request["qltitle"].Trim();
                ViewBag.qltitle = qltitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["qlcode"]))
            {

                qlcode = Request["qlcode"].Trim();
                ViewBag.qlcode = qlcode;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcode"]))
            {

                qvcode = Request["qvcode"].Trim();
                ViewBag.qvcode = qvcode;
            }

            IPagedList<landcode> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from landcode where";
                if (qltitle != "")
                    sqlstr += " ltitle like '%" + qltitle + "%'  and";
                if (qlcode != "")
                    sqlstr += " lcode = '" + qlcode + "'  and";
                if (qvcode != "")
                    sqlstr += " vcode = '" + qvcode + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.landcode.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<landcode>(page.Value - 1, (int)Session["pagesize"]);
            
            }
            return View(result);
        }

     


        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qltitle = "", qlcode = "";
            if (!string.IsNullOrWhiteSpace(Request["qltitle"]))
            {
                qltitle = Request["qltitle"].Trim();
                ViewBag.qltitle = qltitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["qlcode"]))
            {

                qlcode = Request["qlcode"].Trim();
                ViewBag.qlcode = qlcode;
            }
            string cdel = Request["cdel"];

            if (string.IsNullOrWhiteSpace(cdel))
            {

                string tgourl = "/landcode/List?page=" + page + "&qltitle=" + qltitle + "&qlcode=" + qlcode;
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
                            string ltitle = dbobj.get_dbvalue(conn1, "select ltitle from landcode where lcode ='" + condtionArr[i].ToString() + "'");

                            sysnote += "段名名稱:" + ltitle + "<br>";

                            dbobj.dbexecute("Aitag_DBContext", "DELETE FROM landcode where lcode = '" + condtionArr[i].ToString() + "'");
                           
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
                        string tgourl = "/landcode/List?page=" + page +"&qltitle=" + qltitle + "&qlcode=" + qlcode;
                        return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                        //return RedirectToAction("List");
                    }
            }
        }



    }
}
