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
    public class farmerController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /farmer/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.Msid = Session["Msid"].ToString();
        //    farmer col = new farmer();
        //    return View(col);
        //}

        //[HttpPost]
        public ActionResult add(farmer col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "farmerno"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qfarmername = "", qfarmerno = "";
            if (!string.IsNullOrWhiteSpace(Request["qfarmername"]))
            {
                qfarmername = Request["qfarmername"].Trim();
                ViewBag.qfarmername = qfarmername;
            }
            if (!string.IsNullOrWhiteSpace(Request["qfarmerno"]))
            {

                qfarmerno = Request["qfarmerno"].Trim();
                ViewBag.qfarmerno = qfarmerno;
            }

            if (sysflag != "A")
            {             
                farmer newcol = new farmer();
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
                    string sqlstr = "select * from farmer where 1<>1";
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
                        con.farmer.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "農戶姓名：" + col.farmername ;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/farmer/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qfarmername' name='qfarmername' value='" + qfarmername + "'>";
                    tmpform += "<input type=hidden id='qfarmerno' name='qfarmerno' value='" + qfarmerno + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                   // return RedirectToAction("List");

                }
            }

           
        }

    
        [ValidateInput(false)]
        public ActionResult Edit(farmer chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "farmerno"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qfarmername = "", qfarmerno = "";
            if (!string.IsNullOrWhiteSpace(Request["qfarmername"]))
            {
                qfarmername = Request["qfarmername"].Trim();
                ViewBag.qfarmername = qfarmername;
            }
            if (!string.IsNullOrWhiteSpace(Request["qfarmerno"]))
            {

                qfarmerno = Request["qfarmerno"].Trim();
                ViewBag.qfarmerno = qfarmerno;
            }
            
            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.farmer.Where(r => r.farmerno == chks.farmerno).FirstOrDefault();
                    farmer efarmers = con.farmer.Find(chks.farmerno);
                    if (efarmers == null)
                    {
                        return HttpNotFound();
                    }
                    return View(efarmers);
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
                         string sysnote = "農戶姓名：" + chks.farmername;
                         dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                         sysconn.Close();
                         sysconn.Dispose();
                        //=================================================

                         string tmpform="";
                         tmpform = "<body onload=qfr1.submit();>";
                         tmpform += "<form name='qfr1' action='/farmer/List' method='post'>";
                         tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                         tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                         tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                         tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                         tmpform += "<input type=hidden id='qfarmername' name='qfarmername' value='" + qfarmername + "'>";
                         tmpform += "<input type=hidden id='qfarmerno' name='qfarmerno' value='" + qfarmerno + "'>";
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
            { orderdata = "farmerno"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qfarmername = "", qfarmerno = "", qvcode = "";
            if (!string.IsNullOrWhiteSpace(Request["qfarmername"]))
            {
                qfarmername = Request["qfarmername"].Trim();
                ViewBag.qfarmername = qfarmername;
            }
            if (!string.IsNullOrWhiteSpace(Request["qfarmerno"]))
            {

                qfarmerno = Request["qfarmerno"].Trim();
                ViewBag.qfarmerno = qfarmerno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcode"]))
            {

                qvcode = Request["qvcode"].Trim();
                ViewBag.qvcode = qvcode;
            }

            IPagedList<farmer> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from farmer where";
                if (qfarmername != "")
                    sqlstr += " farmername like '%" + qfarmername + "%'  and";
                if (qfarmerno != "")
                    sqlstr += " farmerno = '" + qfarmerno + "'  and";
                if (qvcode != "")
                    sqlstr += " vcode = '" + qvcode + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.farmer.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<farmer>(page.Value - 1, (int)Session["pagesize"]);
            
            }
            return View(result);
        }

        public ActionResult chk(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "farmerno"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qfarmername = "", qfarmerno = "", qvcode = "";
            if (!string.IsNullOrWhiteSpace(Request["qfarmername"]))
            {
                qfarmername = Request["qfarmername"].Trim();
                ViewBag.qfarmername = qfarmername;
            }
            if (!string.IsNullOrWhiteSpace(Request["qfarmerno"]))
            {

                qfarmerno = Request["qfarmerno"].Trim();
                ViewBag.qfarmerno = qfarmerno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcode"]))
            {

                qvcode = Request["qvcode"].Trim();
                ViewBag.qvcode = qvcode;
            }

            IPagedList<farmer> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from farmer where";
                if (qfarmername != "")
                    sqlstr += " farmername like '%" + qfarmername + "%'  and";
                if (qfarmerno != "")
                    sqlstr += " farmerno = '" + qfarmerno + "'  and";
                if (qvcode != "")
                    sqlstr += " vcode = '" + qvcode + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.farmer.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<farmer>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }

        public ActionResult rpt(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "farmerno"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qfarmername = "", qfarmerno = "", qvcode = "";
            if (!string.IsNullOrWhiteSpace(Request["qfarmername"]))
            {
                qfarmername = Request["qfarmername"].Trim();
                ViewBag.qfarmername = qfarmername;
            }
            if (!string.IsNullOrWhiteSpace(Request["qfarmerno"]))
            {

                qfarmerno = Request["qfarmerno"].Trim();
                ViewBag.qfarmerno = qfarmerno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcode"]))
            {

                qvcode = Request["qvcode"].Trim();
                ViewBag.qvcode = qvcode;
            }

            IPagedList<farmer> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from farmer where";
                if (qfarmername != "")
                    sqlstr += " farmername like '%" + qfarmername + "%'  and";
                if (qfarmerno != "")
                    sqlstr += " farmerno = '" + qfarmerno + "'  and";
                if (qvcode != "")
                    sqlstr += " vcode = '" + qvcode + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.farmer.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<farmer>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }

     


        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qfarmername = "", qfarmerno = "";
            if (!string.IsNullOrWhiteSpace(Request["qfarmername"]))
            {
                qfarmername = Request["qfarmername"].Trim();
                ViewBag.qfarmername = qfarmername;
            }
            if (!string.IsNullOrWhiteSpace(Request["qfarmerno"]))
            {

                qfarmerno = Request["qfarmerno"].Trim();
                ViewBag.qfarmerno = qfarmerno;
            }
            string cdel = Request["cdel"];

            if (string.IsNullOrWhiteSpace(cdel))
            {

                string tgourl = "/farmer/List?page=" + page + "&qfarmername=" + qfarmername + "&qfarmerno=" + qfarmerno;
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
                            string farmername = dbobj.get_dbvalue(conn1, "select farmername from farmer where farmerno ='" + condtionArr[i].ToString() + "'");

                            sysnote += "農戶姓名:" + farmername + "<br>";

                            dbobj.dbexecute("Aitag_DBContext", "DELETE FROM farmer where farmerno = '" + condtionArr[i].ToString() + "'");
                           
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
                        string tgourl = "/farmer/List?page=" + page +"&qfarmername=" + qfarmername + "&qfarmerno=" + qfarmerno;
                        return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                        //return RedirectToAction("List");
                    }
            }
        }



    }
}
