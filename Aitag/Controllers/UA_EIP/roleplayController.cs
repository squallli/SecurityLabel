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
    public class roleplayController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /roleplay/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.rid = Session["rid"].ToString();
        //    roleplay col = new roleplay();
        //    return View(col);
        //}

        //[HttpPost]
        public ActionResult add(roleplay col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "rid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qrid = "", qroletitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qrid"]))
            {
                qrid = Request["qrid"].Trim();
                ViewBag.qrid = qrid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qroletitle"]))
            {

                qroletitle = Request["qroletitle"].Trim();
                ViewBag.qroletitle = qroletitle;
            }

            if (sysflag != "A")
            {
                roleplay newcol = new roleplay();
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
                    string sqlstr = "select rid from roleplay where rid = '" + col.rid + "'";
                    sqlsmd.CommandText = sqlstr;
                    dr = sqlsmd.ExecuteReader();

                    if (dr.Read())
                    {

                        ModelState.AddModelError("", "代碼重複!");
                        return View(col);
                    }
                    dr.Close();
                    dr.Dispose();
                    sqlsmd.Dispose();
                    conn.Close();
                    conn.Dispose();

                    //密碼加密
                    //col.emppasswd = dbobj.Encrypt(col.emppasswd);
                    //col.rid = col.emppasswd;
                    //col.baddid = Session["tempid"].ToString();
                    col.comid = Session["comid"].ToString();
                    col.bmodid = Session["tempid"].ToString();
                    //col.badddate = DateTime.Now;
                    col.bmoddate = DateTime.Now;
                    col.comid = Session["comid"].ToString();
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.roleplay.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "代碼:" + col.rid + "名稱:" + col.roletitle;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/roleplay/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qrid' name='qrid' value='" + qrid + "'>";
                    tmpform += "<input type=hidden id='qroletitle' name='qroletitle' value='" + qroletitle + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    // return RedirectToAction("List");

                }
            }


        }


        [ValidateInput(false)]
        public ActionResult Edit(roleplay chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "rid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qrid = "", qroletitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qrid"]))
            {
                qrid = Request["qrid"].Trim();
                ViewBag.qrid = qrid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qroletitle"]))
            {

                qroletitle = Request["qroletitle"].Trim();
                ViewBag.qroletitle = qroletitle;
            }

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.roleplay.Where(r => r.rid == chks.rid).FirstOrDefault();
                    roleplay eroleplays = con.roleplay.Find(chks.rid);
                    if (eroleplays == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eroleplays);
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

                    //string oldrid = Request["oldrid"];                 

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
                        string sysnote = "代碼:" + chks.rid + "名稱:" + chks.roletitle;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/roleplay/List' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                        tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                        tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                        tmpform += "<input type=hidden id='qrid' name='qrid' value='" + qrid + "'>";
                        tmpform += "<input type=hidden id='qroletitle' name='qroletitle' value='" + qroletitle + "'>";
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
            { orderdata = "rid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qrid = "", qroletitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qrid"]))
            {
                qrid = Request["qrid"].Trim();
                ViewBag.qrid = qrid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qroletitle"]))
            {

                qroletitle = Request["qroletitle"].Trim();
                ViewBag.qroletitle = qroletitle;
            }

            IPagedList<roleplay> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from roleplay where";
                if (qrid != "")
                    sqlstr += " rid like '%" + qrid + "%'  and";
                if (qroletitle != "")
                    sqlstr += " roletitle like '%" + qroletitle + "%'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.roleplay.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<roleplay>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }




        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qrid = "", qroletitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qrid"]))
            {
                qrid = Request["qrid"].Trim();
                ViewBag.qrid = qrid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qroletitle"]))
            {

                qroletitle = Request["qroletitle"].Trim();
                ViewBag.qroletitle = qroletitle;
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
                        string eroleplays = dbobj.get_dbvalue(conn1, "select roletitle from roleplay where rid ='" + condtionArr[i].ToString() + "'");

                        sysnote += "代碼名稱:" + eroleplays + "，序號:" + condtionArr[i].ToString() + "<br>";

                        dbobj.dbexecute("Aitag_DBContext", "DELETE FROM roleplay where rid = '" + condtionArr[i].ToString() + "'");
                     
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
                    string tgourl = "/roleplay/List?page=" + page + "&qrid=" + qrid + "&qroletitle=" + qroletitle;
                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                    //return RedirectToAction("List");
                }
            }
        }

        public ActionResult tmpchooserole()
        {
            return View();
        }


        public ActionResult chooserole(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "rid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qrid = "", qroletitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qrid"]))
            {
                qrid = Request["qrid"].Trim();
                ViewBag.qrid = qrid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qroletitle"]))
            {

                qroletitle = Request["qroletitle"].Trim();
                ViewBag.qroletitle = qroletitle;
            }

            IPagedList<roleplay> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from roleplay where ifrtype = 'n'   and";
                if (qrid != "")
                    sqlstr += " rid like '%" + qrid + "%'  and";
                if (qroletitle != "")
                    sqlstr += " roletitle like '%" + qroletitle + "%'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.roleplay.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<roleplay>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }



    }
}
