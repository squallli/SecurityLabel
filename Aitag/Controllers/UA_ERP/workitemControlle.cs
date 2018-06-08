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
    public class workitemController : BaseController
    {

        //private AitagBill_DBContext db = new AitagBill_DBContext();
        //
        // GET: /workitem/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.Msid = Session["Msid"].ToString();
        //    workitem col = new workitem();
        //    return View(col);
        //}

        //[HttpPost]
        public ActionResult add(workitem col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "corp_no"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcorp_no = "", qworkitem = "";
            if (!string.IsNullOrWhiteSpace(Request["qcorp_no"]))
            {
                qcorp_no = Request["qcorp_no"].Trim();
                ViewBag.qcorp_no = qcorp_no;
            }
            if (!string.IsNullOrWhiteSpace(Request["qworkitem"]))
            {

                qworkitem = Request["qworkitem"].Trim();
                ViewBag.qworkitem = qworkitem;
            }

            if (sysflag != "A")
            {
                workitem newcol = new workitem();
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
                    SqlConnection conn = dbobj.get_conn("AitagBill_DBContext");
                    SqlDataReader dr;
                    SqlCommand sqlsmd = new SqlCommand();
                    sqlsmd.Connection = conn;
                    string sqlstr = "select corp_no from workitem where corp_no = '" + col.corp_no + "' and work_no_code = '" + col.work_no_code + "'";
                    sqlsmd.CommandText = sqlstr;
                    dr = sqlsmd.ExecuteReader();

                    if (dr.Read())
                    {
                        ModelState.AddModelError("", "項目代碼重複!");
                        return View(col);
                    }
                    dr.Close();
                    dr.Dispose();
                    sqlsmd.Dispose();
                    conn.Close();
                    conn.Dispose();

                    col.comid = Session["comid"].ToString();
                    col.bmodid = Session["tempid"].ToString();
                    col.hourgroup = Request["hourgroup"].Trim();
                    col.bmoddate = DateTime.Now;
                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {
                        con.workitem.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "所屬公司:" + col.corp_no + "項目代碼:" + col.work_no_code + "項目名稱:" + col.workitemname;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/workitem/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qcorp_no' name='qcorp_no' value='" + qcorp_no + "'>";
                    tmpform += "<input type=hidden id='qworkitem' name='qworkitem' value='" + qworkitem + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    // return RedirectToAction("List");

                }
            }


        }


        [ValidateInput(false)]
        public ActionResult Edit(workitem chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "corp_no"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcorp_no = "", qwork_no_code = "", qworkitemname = "";
            if (!string.IsNullOrWhiteSpace(Request["qcorp_no"]))
            {
                qcorp_no = Request["qcorp_no"].Trim();
                ViewBag.qcorp_no = qcorp_no;
            }
            if (!string.IsNullOrWhiteSpace(Request["qwork_no_code"]))
            {

                qwork_no_code = Request["qwork_no_code"].Trim();
                ViewBag.qwork_no_code = qwork_no_code;
            }
            if (!string.IsNullOrWhiteSpace(Request["qworkitemname"]))
            {

                qworkitemname = Request["qworkitemname"].Trim();
                ViewBag.qwork_no_code = qworkitemname;
            }

            if (sysflag != "E")
            {
                using (AitagBill_DBContext con = new AitagBill_DBContext())
                {
                    var data = con.workitem.Where(r => r.corp_no == chks.corp_no && r.work_no_code == chks.work_no_code).FirstOrDefault();
                    
                    
                    if (data == null)
                    {
                        return HttpNotFound();
                    }
                    return View(data);
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
               
                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {

                        NDcommon dbobj = new NDcommon();
                        chks.hourgroup = Request["hourgroup"].Trim();
                        chks.bmodid = Session["tempid"].ToString();
                        chks.bmoddate = DateTime.Now;
                        con.Entry(chks).State = EntityState.Modified;
                        con.SaveChanges();


                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "所屬公司:" + chks.corp_no + "項目代碼:" + chks.work_no_code + "項目名稱:" + chks.workitemname;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/workitem/List' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                        tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                        tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                        tmpform += "<input type=hidden id='qcorp_no' name='qcorp_no' value='" + qcorp_no + "'>";
                        tmpform += "<input type=hidden id='qwork_no_code' name='qwork_no_code' value='" + qwork_no_code + "'>";
                        tmpform += "<input type=hidden id='qworkitemname' name='qworkitemname' value='" + qworkitemname + "'>";
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
            { orderdata = " corp_no"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = " asc"; }

            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcorp_no = "", qworkitem = "", qwork_no_code = "";
            if (!string.IsNullOrWhiteSpace(Request["qcorp_no"]))
            {
                qcorp_no = Request["qcorp_no"].Trim();
                ViewBag.qcorp_no = qcorp_no;
            }
            if (!string.IsNullOrWhiteSpace(Request["qwork_no_code"]))
            {
                qwork_no_code = Request["qwork_no_code"].Trim();
                ViewBag.qwork_no_code = qwork_no_code;
            }
            if (!string.IsNullOrWhiteSpace(Request["qworkitem"]))
            {

                qworkitem = Request["qworkitem"].Trim();
                ViewBag.qworkitem = qworkitem;
            }

            IPagedList<workitem> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "select * from workitem where";
                if (qcorp_no != "")
                    sqlstr += " corp_no like '%" + qcorp_no + "%'  and";
                if (qwork_no_code != "")
                    sqlstr += " work_no_code like '%" + qwork_no_code + "%'  and";
                if (qworkitem != "")
                    sqlstr += " workitemname like '%" + qworkitem + "%'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1 ;

                var query = con.workitem.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<workitem>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }




        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qcorp_no = "", qworkitem = "";
            if (!string.IsNullOrWhiteSpace(Request["qcorp_no"]))
            {
                qcorp_no = Request["qcorp_no"].Trim();
                ViewBag.qcorp_no = qcorp_no;
            }
            if (!string.IsNullOrWhiteSpace(Request["qworkitem"]))
            {

                qworkitem = Request["qworkitem"].Trim();
                ViewBag.qworkitem = qworkitem;
            }
            string cdel = Request["cdel"];

            if (string.IsNullOrWhiteSpace(cdel))
            {
          
                return new ContentResult() { Content = @"<script>alert('請勾選要刪除的資料!!');window.history.go(-1);</script>" };
            }
            else
            {
                using (AitagBill_DBContext con = new AitagBill_DBContext())
                {

                    NDcommon dbobj = new NDcommon();
                    SqlConnection conn1 = dbobj.get_conn("AitagBill_DBContext");
                    string sysnote = "";
                    string[] condtionArr = cdel.Split(',');
                    int condtionLen = condtionArr.Length;
                    for (int i = 0; i < condtionLen; i++)
                    {
                        string[] condtionArr1 = condtionArr[i].Split('#');
                        string eworkitems = dbobj.get_dbvalue(conn1, "select workitemname from workitem where corp_no ='" + condtionArr1[0].ToString() + "' and work_no_code='" + condtionArr1[1].ToString() + "'");

                        sysnote += "項目名稱:" + eworkitems + "，所屬公司:" + condtionArr1[0].ToString() + "，項目代碼:" + condtionArr1[1].ToString() + "<br>";

                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM workitem where corp_no = '" + condtionArr1[0].ToString() + "' and work_no_code='" + condtionArr1[1].ToString() + "'");

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
                    string tgourl = "/workitem/List?page=" + page + "&qcorp_no=" + qcorp_no + "&qworkitem=" + qworkitem;
                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };
                   
                }
            }
        }



    }
}
