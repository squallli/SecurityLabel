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
    public class billflowController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /billflow/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.billid = Session["billid"].ToString();
        //    billflow col = new billflow();
        //    return View(col);
        //}

        //[HttpPost]
        public ActionResult add(billflow col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "billid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qbillid = "", qcomclass = "";
            if (!string.IsNullOrWhiteSpace(Request["qbillid"]))
            {
                qbillid = Request["qbillid"].Trim();
                ViewBag.qbillid = qbillid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomclass"]))
            {

                qcomclass = Request["qcomclass"].Trim();
                ViewBag.qcomclass = qcomclass;
            }

            if (sysflag != "A")
            {
                billflow newcol = new billflow();
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
                    //SqlConnection conn = dbobj.get_conn("Aitag_DBContext");
                    //SqlDataReader dr;
                    //SqlCommand sqlsmd = new SqlCommand();
                    //sqlsmd.Connection = conn;
                    //string sqlstr = "select billid from billflow where 1<>1";
                    //sqlsmd.CommandText = sqlstr;
                    //dr = sqlsmd.ExecuteReader();

                    //if (dr.Read())
                    //{

                    //    //ModelState.AddModelError("", "權限代碼重複!");
                    //    return View(col);
                    //}
                    //dr.Close();
                    //dr.Dispose();
                    //sqlsmd.Dispose();
                    //conn.Close();
                    //conn.Dispose();
                    col.billtype = Request["billtype"];
                    col.addr = Request["addr"];
                    col.bmodid = Session["tempid"].ToString();
                    col.bmoddate = DateTime.Now;
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.billflow.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string tmpbillidname = dbobj.get_dbvalue(sysconn, "select doctitle from docgroup where docid ='" + Request["billid"].Trim() + "' and comid='"+ Session["comid"] +"'");

                        string flowcheck = "";
                        if (!string.IsNullOrWhiteSpace(Request["flowcheck"].Trim()))
                        {
                            string sqlstr1 = "select roletitle from roleplay where rid in (" + Request["flowcheck"].Trim() + ")";
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = sysconn;
                            cmd.CommandText = sqlstr1;
                            SqlDataReader dr1 = cmd.ExecuteReader();
                            while (dr1.Read())
                            {
                                flowcheck = flowcheck + dr1["roletitle"].ToString() + " → ";
                            }
                            if (flowcheck != "")
                            {

                                flowcheck = flowcheck.Substring(0, flowcheck.Length - 2);
                            }
                            dr1.Close();
                            dr1.Dispose();
                        }
                        string sysnote = "呈核單類別:" + tmpbillidname + "<br>呈核人員:" + flowcheck;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/billflow/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qbillid' name='qbillid' value='" + qbillid + "'>";
                    tmpform += "<input type=hidden id='qcomclass' name='qcomclass' value='" + qcomclass + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    // return RedirectToAction("List");

                }
            }


        }


        [ValidateInput(false)]
        public ActionResult Edit(billflow chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "bid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qbillid = "", qcomclass = "";
            if (!string.IsNullOrWhiteSpace(Request["qbillid"]))
            {
                qbillid = Request["qbillid"].Trim();
                ViewBag.qbillid = qbillid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomclass"]))
            {

                qcomclass = Request["qcomclass"].Trim();
                ViewBag.qcomclass = qcomclass;
            }

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.billflow.Where(r => r.bid == chks.bid).FirstOrDefault();
                    billflow ebillflows = con.billflow.Find(chks.bid);
                    if (ebillflows == null)
                    {
                        return HttpNotFound();
                    }
                    return View(ebillflows);
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

                    //string oldbillid = Request["oldbillid"];                 

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {

                        NDcommon dbobj = new NDcommon();

                        chks.addr = Request["addr"];
                        chks.billtype = Request["billtype"];
                        chks.bmodid = Session["tempid"].ToString();
                        chks.bmoddate = DateTime.Now;
                        con.Entry(chks).State = EntityState.Modified;

                        con.SaveChanges();


                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string tmpbillidname = dbobj.get_dbvalue(sysconn, "select doctitle from docgroup where docid ='" + Request["billid"].Trim() + "' and comid='" + Session["comid"] + "'");
                        string flowcheck = "";
                        if (!string.IsNullOrWhiteSpace(Request["flowcheck"].Trim()))
                        {
                            string sqlstr1 = "select * from roleplay where rid in (" + Request["flowcheck"].Trim() + ")";
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = sysconn;
                            cmd.CommandText = sqlstr1;
                            SqlDataReader dr1 = cmd.ExecuteReader();
                            while (dr1.Read())
                            {
                                flowcheck = flowcheck + dr1["roletitle"].ToString() + " → ";
                            }
                            if (flowcheck != "")
                            {

                                flowcheck = flowcheck.Substring(0, flowcheck.Length - 2);
                            }
                            dr1.Close();
                            dr1.Dispose();
                        }
                        string sysnote = "呈核單類別：" + tmpbillidname + "<br>呈核人員:" + flowcheck;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/billflow/List' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                        tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                        tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                        tmpform += "<input type=hidden id='qbillid' name='qbillid' value='" + qbillid + "'>";
                        tmpform += "<input type=hidden id='qcomclass' name='qcomclass' value='" + qcomclass + "'>";
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
            { orderdata = "billid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qbillid = "", qcomclass = "";
            if (!string.IsNullOrWhiteSpace(Request["qbillid"]))
            {
                qbillid = Request["qbillid"].Trim();
                ViewBag.qbillid = qbillid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomclass"]))
            {

                qcomclass = Request["qcomclass"].Trim();
                ViewBag.qcomclass = qcomclass;
            }

            IPagedList<billflow> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from billflow where";
                if (qbillid != "")
                    sqlstr += " billid ='" + qbillid + "'  and";
                if (qcomclass != "")
                    sqlstr += " comclass = '" + qcomclass + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.billflow.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<billflow>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }




        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qbillid = "", qcomclass = "";
            if (!string.IsNullOrWhiteSpace(Request["qbillid"]))
            {
                qbillid = Request["qbillid"].Trim();
                ViewBag.qbillid = qbillid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomclass"]))
            {

                qcomclass = Request["qcomclass"].Trim();
                ViewBag.qcomclass = qcomclass;
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
                        string billid = dbobj.get_dbvalue(conn1, "select billid from billflow where bid ='" + condtionArr[i].ToString() + "'");
                        string flowcheck = dbobj.get_dbvalue(conn1, "select flowcheck from billflow where bid ='" + condtionArr[i].ToString() + "'");
                        string flowcheck1 = "";
                        string tmpbillidname = dbobj.get_dbvalue(conn1, "select doctitle from docgroup where docid ='" + billid + "' and comid='" + Session["comid"] + "'");
                        if (!string.IsNullOrWhiteSpace(flowcheck))
                        {
                            string sqlstr1 = "select roletitle from roleplay where rid in (" + flowcheck + ")";
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = conn1;
                            cmd.CommandText = sqlstr1;
                            SqlDataReader dr1 = cmd.ExecuteReader();
                            while (dr1.Read())
                            {
                                flowcheck1 = flowcheck1 + dr1["roletitle"].ToString() + " → ";
                            }
                            if (flowcheck1 != "")
                            {

                                flowcheck1 = flowcheck1.Substring(0, flowcheck1.Length - 2);
                            }
                            dr1.Close();
                            dr1.Dispose();
                        }
                        sysnote += "呈核單類別:" + tmpbillidname + "，呈核流程:" + flowcheck1 + "<br>";

                        dbobj.dbexecute("Aitag_DBContext", "DELETE FROM billflow where bid = '" + condtionArr[i].ToString() + "'");
                     
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
                    string tgourl = "/billflow/List?page=" + page + "&qbillid=" + qbillid + "&qcomclass=" + qcomclass;
                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                    //return RedirectToAction("List");
                }
            }
        }

        public ActionResult choosebid(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "rid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string tmp1 = "", tmp2 = "", fname = "", ctype = "", qroletitle = "";
            if (Request["tmp1"] != null)
            { tmp1 = Request["tmp1"].Trim(); }

            if (Request["tmp2"] != null)
            { tmp2 = Request["tmp2"].Trim(); }

            if (Request["fname"] != null)
            { fname = Request["fname"].Trim(); }

            ViewBag.tmp1 = tmp1;
            ViewBag.tmp2 = tmp2;
            ViewBag.fname = fname;
            if (!string.IsNullOrWhiteSpace(Request["qroletitle"]))
            {
                qroletitle = Request["qroletitle"].Trim();
                ViewBag.qroletitle = qroletitle;
            }

            IPagedList<roleplay> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from roleplay where";
                if (qroletitle != "")
                    sqlstr += " roletitle like'%" + qroletitle + "%'  and";
                
                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.roleplay.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<roleplay>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }



    }
}
