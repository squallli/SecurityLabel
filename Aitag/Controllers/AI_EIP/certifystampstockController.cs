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
    public class certifystampstockController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /certifystampstock/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.Msid = Session["Msid"].ToString();
        //    certifystampstock col = new certifystampstock();
        //    return View(col);
        //}

        //[HttpPost]
        public ActionResult add(certifystampstock col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "csid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qbarcode = "", qcerno = "";
            if (!string.IsNullOrWhiteSpace(Request["qbarcode"]))
            {
                qbarcode = Request["qbarcode"].Trim();
                ViewBag.qbarcode = qbarcode;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcerno"]))
            {

                qcerno = Request["qcerno"].Trim();
                ViewBag.qcerno = qcerno;
            }

            if (sysflag != "A")
            {             
                certifystampstock newcol = new certifystampstock();
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
                    string sqlstr = "select * from certifystampstock where 1<>1";
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
                        con.certifystampstock.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "標章認證編號：" + col.barcode ;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/certifystampstock/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qbarcode' name='qbarcode' value='" + qbarcode + "'>";
                    tmpform += "<input type=hidden id='qcerno' name='qcerno' value='" + qcerno + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                   // return RedirectToAction("List");

                }
            }

           
        }

    
        [ValidateInput(false)]
        public ActionResult Edit(certifystampstock chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "cerno"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qbarcode = "", qcerno = "";
            if (!string.IsNullOrWhiteSpace(Request["qbarcode"]))
            {
                qbarcode = Request["qbarcode"].Trim();
                ViewBag.qbarcode = qbarcode;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcerno"]))
            {

                qcerno = Request["qcerno"].Trim();
                ViewBag.qcerno = qcerno;
            }
            
            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.certifystampstock.Where(r => r.cerno == chks.cerno).FirstOrDefault();
                    certifystampstock ecertifystampstocks = con.certifystampstock.Find(chks.cerno);
                    if (ecertifystampstocks == null)
                    {
                        return HttpNotFound();
                    }
                    return View(ecertifystampstocks);
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
                         string sysnote = "標章認證編號：" + chks.barcode;
                         dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                         sysconn.Close();
                         sysconn.Dispose();
                        //=================================================

                         string tmpform="";
                         tmpform = "<body onload=qfr1.submit();>";
                         tmpform += "<form name='qfr1' action='/certifystampstock/List' method='post'>";
                         tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                         tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                         tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                         tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                         tmpform += "<input type=hidden id='qbarcode' name='qbarcode' value='" + qbarcode + "'>";
                         tmpform += "<input type=hidden id='qcerno' name='qcerno' value='" + qcerno + "'>";
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
            { orderdata = "barcode"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qbarcode = "", qcerno = "", qcsstatus = "";
            if (!string.IsNullOrWhiteSpace(Request["qbarcode"]))
            {
                qbarcode = Request["qbarcode"].Trim();
                ViewBag.qbarcode = qbarcode;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcerno"]))
            {

                qcerno = Request["qcerno"].Trim();
                ViewBag.qcerno = qcerno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcsstatus"]))
            {

                qcsstatus = Request["qcsstatus"].Trim();
                ViewBag.qcsstatus = qcsstatus;
            }

            IPagedList<certifystampstock> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from certifystampstock where";
                if (qbarcode != "")
                    sqlstr += " barcode like '%" + qbarcode + "%'  and";
                if (qcerno != "")
                    sqlstr += " cerno = '" + qcerno + "'  and";
                if (qcsstatus != "")
                    sqlstr += " csstatus = '" + qcsstatus + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.certifystampstock.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<certifystampstock>(page.Value - 1, (int)Session["pagesize"]);
            
            }
            return View(result);
        }

        public ActionResult rpt(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "barcode"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qbarcode = "", qcerno = "", qcsstatus = "";
            if (!string.IsNullOrWhiteSpace(Request["qbarcode"]))
            {
                qbarcode = Request["qbarcode"].Trim();
                ViewBag.qbarcode = qbarcode;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcerno"]))
            {

                qcerno = Request["qcerno"].Trim();
                ViewBag.qcerno = qcerno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcsstatus"]))
            {

                qcsstatus = Request["qcsstatus"].Trim();
                ViewBag.qcsstatus = qcsstatus;
            }

            IPagedList<certifystampstock> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from certifystampstock where";
                if (qbarcode != "")
                    sqlstr += " barcode like '%" + qbarcode + "%'  and";
                if (qcerno != "")
                    sqlstr += " cerno = '" + qcerno + "'  and";
                if (qcsstatus != "")
                    sqlstr += " csstatus = '" + qcsstatus + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.certifystampstock.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<certifystampstock>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }

     


        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qbarcode = "", qcerno = "";
            if (!string.IsNullOrWhiteSpace(Request["qbarcode"]))
            {
                qbarcode = Request["qbarcode"].Trim();
                ViewBag.qbarcode = qbarcode;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcerno"]))
            {

                qcerno = Request["qcerno"].Trim();
                ViewBag.qcerno = qcerno;
            }
            string cdel = Request["cdel"];

            if (string.IsNullOrWhiteSpace(cdel))
            {

                string tgourl = "/certifystampstock/List?page=" + page + "&qbarcode=" + qbarcode + "&qcerno=" + qcerno;
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
                            string barcode = dbobj.get_dbvalue(conn1, "select barcode from certifystampstock where cerno ='" + condtionArr[i].ToString() + "'");

                            sysnote += "標章認證編號:" + barcode + "<br>";

                            dbobj.dbexecute("Aitag_DBContext", "DELETE FROM certifystampstock where cerno = '" + condtionArr[i].ToString() + "'");
                           
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
                        string tgourl = "/certifystampstock/List?page=" + page +"&qbarcode=" + qbarcode + "&qcerno=" + qcerno;
                        return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                        //return RedirectToAction("List");
                    }
            }
        }



    }
}
