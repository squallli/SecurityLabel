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
    public class CheckcodeController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /Checkcode/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

       
    
        public ActionResult add(Checkcode col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "cid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qchkclass = "", qchkitem = "";
            if (!string.IsNullOrWhiteSpace(Request["qchkclass"]))
            {
                qchkclass = Request["qchkclass"].Trim();
                ViewBag.qchkclass = qchkclass;
            }
            if (!string.IsNullOrWhiteSpace(Request["qchkitem"]))
            {

                qchkitem = Request["qchkitem"].Trim();
                ViewBag.qchkitem = qchkitem;
            }

            if (sysflag != "A")
            {             
                Checkcode newcol = new Checkcode();
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
                    string sqlstr = "select cid from Checkcode where chkclass = '" + col.chkclass + "' and chkcode = '" + col.chkcode + "'";
                    sqlsmd.CommandText = sqlstr;
                    dr = sqlsmd.ExecuteReader();

                    if (dr.Read())
                    {

                        ModelState.AddModelError("", "共用代碼重複!");
                        return View(col);
                    }
                    dr.Close();
                    dr.Dispose();
                    sqlsmd.Dispose();
                    conn.Close();
                    conn.Dispose();

                    //密碼加密
                    //col.emppasswd = dbobj.Encrypt(col.emppasswd);
                    //col.chkclass = col.emppasswd;
                    //col.baddid = Session["tempid"].ToString();
                    col.bmodid = Session["tempid"].ToString();
                    //col.badddate = DateTime.Now;
                    col.bmoddate = DateTime.Now;
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.Checkcode.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "類別:" + col.chkclasstitle + "代碼名稱:" + col.chkitem;     
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/Checkcode/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qchkclass' name='qchkclass' value='" + qchkclass + "'>";
                    tmpform += "<input type=hidden id='qchkitem' name='qchkitem' value='" + qchkitem + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    string tgourl = "/Checkcode/List?page=" + page + "&qchkclass=" + qchkclass + "&qchkitem=" + qchkitem;
                    return new ContentResult() { Content = @"<script>alert('新增成功!!');location.href='" + tgourl + "'</script>" };
                   // return RedirectToAction("List");

                }
            }

           
        }

    
        [ValidateInput(false)]
        public ActionResult Edit(Checkcode chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "cid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qchkclass = "", qchkitem = "";
            if (!string.IsNullOrWhiteSpace(Request["qchkclass"]))
            {
                qchkclass = Request["qchkclass"].Trim();
                ViewBag.qchkclass = qchkclass;
            }
            if (!string.IsNullOrWhiteSpace(Request["qchkitem"]))
            {

                qchkitem = Request["qchkitem"].Trim();
                ViewBag.qchkitem = qchkitem;
            }
            
            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.Checkcode.Where(r => r.cid == chks.cid).FirstOrDefault();
                    Checkcode eCheckcodes = con.Checkcode.Find(chks.cid);
                    if (eCheckcodes == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eCheckcodes);
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
                         string sysnote = "類別:" + chks.chkclasstitle + "代碼名稱:" + chks.chkitem;     
                         dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                         sysconn.Close();
                         sysconn.Dispose();
                        //=================================================

                         string tmpform="";
                         tmpform = "<body onload=qfr1.submit();>";
                         tmpform += "<form name='qfr1' action='/Checkcode/List' method='post'>";
                         tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                         tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                         tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                         tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                         tmpform += "<input type=hidden id='qchkclass' name='qchkclass' value='" + qchkclass + "'>";
                         tmpform += "<input type=hidden id='qchkitem' name='qchkitem' value='" + qchkitem + "'>";
                         tmpform +="</form>";
                         tmpform +="</body>";

                         string tgourl = "/Checkcode/List?page=" + page + "&qchkclass=" + qchkclass + "&qchkitem=" + qchkitem;
                         return new ContentResult() { Content = @"<script>alert('修改成功!!');location.href='" + tgourl + "'</script>" };
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
            { orderdata = "chkclass,chkcode"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qchkclass = "", qchkitem = "";
            if (!string.IsNullOrWhiteSpace(Request["qchkclass"]))
            {
                qchkclass = Request["qchkclass"].Trim();
                ViewBag.qchkclass = qchkclass;
            }
            if (!string.IsNullOrWhiteSpace(Request["qchkitem"]))
            {

                qchkitem = Request["qchkitem"].Trim();
                ViewBag.qchkitem = qchkitem;
            }

            IPagedList<Checkcode> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from checkcode where";
                if (qchkclass != "")
                    sqlstr += " chkclass like '%" + qchkclass + "%'  and";
                if (qchkitem != "")
                    sqlstr += " chkitem like '%" + qchkitem + "%'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.Checkcode.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<Checkcode>(page.Value - 1, (int)Session["pagesize"]);
            
            }
            return View(result);
        }

     


        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qchkclass = "", qchkitem = "";
            if (!string.IsNullOrWhiteSpace(Request["qchkclass"]))
            {
                qchkclass = Request["qchkclass"].Trim();
                ViewBag.qchkclass = qchkclass;
            }
            if (!string.IsNullOrWhiteSpace(Request["qchkitem"]))
            {

                qchkitem = Request["qchkitem"].Trim();
                ViewBag.qchkitem = qchkitem;
            }
            string cdel = Request["cdel"];

            if (string.IsNullOrWhiteSpace(cdel))
            {

                string tgourl = "/Checkcode/List?page=" + page + "&qchkclass=" + qchkclass + "&qchkitem=" + qchkitem;
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
                            string eCheckcodes = dbobj.get_dbvalue(conn1, "select chkitem from Checkcode where cid ='" + condtionArr[i].ToString() + "'");

                            sysnote += "代碼名稱:" + eCheckcodes + "，序號:" + condtionArr[i].ToString() +"<br>";

                            dbobj.dbexecute("Aitag_DBContext", "DELETE FROM Checkcode where cid = '" + condtionArr[i].ToString() + "'");
                           
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
                        string tgourl = "/Checkcode/List?page=" + page +"&qchkclass=" + qchkclass + "&qchkitem=" + qchkitem;
                        return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                        //return RedirectToAction("List");
                    }
            }
        }



    }
}
