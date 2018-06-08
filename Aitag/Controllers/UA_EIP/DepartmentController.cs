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
    public class DepartmentController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /Department/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.dptid = Session["dptid"].ToString();
        //    Department col = new Department();
        //    return View(col);
        //}

        //[HttpPost]
        public ActionResult add(Department col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "dptid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string comid = "", qdptid = "", qdpttitle = "";
            if (!string.IsNullOrWhiteSpace(Request["comid"]))
            {
                comid = Request["comid"].Trim();
                ViewBag.comid = comid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qdptid"]))
            {

                qdptid = Request["qdptid"].Trim();
                ViewBag.qdptid = qdptid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qdpttitle"]))
            {

                qdpttitle = Request["qdpttitle"].Trim();
                ViewBag.qdpttitle = qdpttitle;
            }

            if (sysflag != "A")
            {
                Department newcol = new Department();
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
                    string sqlstr = "select dptid from Department where dptid = '" + col.dptid + "'";
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

                    //密碼加密
                    //col.emppasswd = dbobj.Encrypt(col.emppasswd);
                    //col.dptid = col.emppasswd;
                    //col.baddid = Session["tempid"].ToString();
                    col.bmodid = Session["tempid"].ToString();
                    //col.badddate = DateTime.Now;
                    col.bmoddate = DateTime.Now;
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.Department.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "代碼:" + col.dptid + "名稱:" + col.dpttitle;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/Department/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qdptid' name='qdptid' value='" + qdptid + "'>";
                    tmpform += "<input type=hidden id='qdpttitle' name='qdpttitle' value='" + qdpttitle + "'>";
                    tmpform += "<input type=hidden id='comid' name='comid' value='" + comid + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    // return RedirectToAction("List");

                }
            }


        }


        [ValidateInput(false)]
        public ActionResult Edit(Department chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "dptid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string comid = "", qdptid = "", qdpttitle = "";
            if (!string.IsNullOrWhiteSpace(Request["comid"]))
            {
                comid = Request["comid"].Trim();
                ViewBag.comid = comid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qdptid"]))
            {

                qdptid = Request["qdptid"].Trim();
                ViewBag.qdptid = qdptid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qdpttitle"]))
            {

                qdpttitle = Request["qdpttitle"].Trim();
                ViewBag.qdpttitle = qdpttitle;
            }

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.Department.Where(r => r.dptid == chks.dptid).FirstOrDefault();
                    Department eDepartments = con.Department.Find(chks.dptid, chks.comid);
                    if (eDepartments == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eDepartments);
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

                    //string olddptid = Request["olddptid"];                 

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
                        string sysnote = "代碼:" + chks.dptid + "名稱:" + chks.dpttitle;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/Department/List' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                        tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                        tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                        tmpform += "<input type=hidden id='qdptid' name='qdptid' value='" + qdptid + "'>";
                        tmpform += "<input type=hidden id='qdpttitle' name='qdpttitle' value='" + qdpttitle + "'>";
                        tmpform += "<input type=hidden id='comid' name='comid' value='" + comid + "'>";
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
            { orderdata = "dptid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string comid = "", qdptid = "", qdpttitle = "";
            if (!string.IsNullOrWhiteSpace(Request["comid"]))
            {
                comid = Request["comid"].Trim();
                ViewBag.comid = comid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qdptid"]))
            {
                qdptid = Request["qdptid"].Trim();
                ViewBag.qdptid = qdptid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qdpttitle"]))
            {

                qdpttitle = Request["qdpttitle"].Trim();
                ViewBag.qdpttitle = qdpttitle;
            }

            IPagedList<Department> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from Department where";
                if (comid != "")
                    sqlstr += " comid like '%" + comid + "%'  and";
                if (qdptid != "")
                    sqlstr += " dptid like '%" + qdptid + "%'  and";
                if (qdpttitle != "")
                    sqlstr += " dpttitle like '%" + qdpttitle + "%'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.Department.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<Department>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);
        }




        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string comid = "", qdptid = "", qdpttitle = "";
            if (!string.IsNullOrWhiteSpace(Request["comid"]))
            {
                comid = Request["comid"].Trim();
                ViewBag.comid = comid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qdptid"]))
            {

                qdptid = Request["qdptid"].Trim();
                ViewBag.qdptid = qdptid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qdpttitle"]))
            {

                qdpttitle = Request["qdpttitle"].Trim();
                ViewBag.qdpttitle = qdpttitle;
            }
            string cdel = Request["cdel"];

            if (string.IsNullOrWhiteSpace(cdel))
            {

                string tgourl = "/Department/List?page=" + page + "&qdptid=" + qdptid + "&qdpttitle=" + qdpttitle;
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
                        string eDepartments = dbobj.get_dbvalue(conn1, "select dpttitle from Department where dptid ='" + condtionArr[i].ToString() + "'");

                        sysnote += "代碼名稱:" + eDepartments + "，序號:" + condtionArr[i].ToString() + "<br>";

                        dbobj.dbexecute("Aitag_DBContext", "DELETE FROM Department where dptid = '" + condtionArr[i].ToString() + "'");
                     
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
                    string tgourl = "/Department/List?page=" + page + "&comid=" + comid + "&qdptid=" + qdptid + "&qdpttitle=" + qdpttitle;
                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                    //return RedirectToAction("List");
                }
            }
        }



    }
}
