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
    public class billsubjectController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /billsubject/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.Msid = Session["Msid"].ToString();
        //    billsubject col = new billsubject();
        //    return View(col);
        //}

        //[HttpPost]
        public ActionResult add(billsubject col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "accid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcomclass = "",qitemcode="" , qsubjecttitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qcomclass"]))
            {
                qcomclass = Request["qcomclass"].Trim();
                ViewBag.qcomclass = qcomclass;
            }
            if (!string.IsNullOrWhiteSpace(Request["qitemcode"]))
            {

                qitemcode = Request["qitemcode"].Trim();
                ViewBag.qitemcode = qitemcode;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsubjecttitle"]))
            {

                qsubjecttitle = Request["qsubjecttitle"].Trim();
                ViewBag.qsubjecttitle = qsubjecttitle;
            }

            if (sysflag != "A")
            {             
                billsubject newcol = new billsubject();
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
                    string sqlstr = "select accid from billsubject where comid = '" + Session["comid"] + "' and itemcode = '" + col.itemcode + "'";
                    sqlsmd.CommandText = sqlstr;
                    dr = sqlsmd.ExecuteReader();

                    if (dr.Read())
                    {

                        ModelState.AddModelError("", "歸帳代碼重複!");
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
                    col.comid = Session["tempid"].ToString();
                    col.bmodid = Session["tempid"].ToString();
                    //col.badddate = DateTime.Now;
                    col.bmoddate = DateTime.Now;
                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {
                        con.billsubject.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "類別:" + col.comclass + "代碼名稱:" + col.subjecttitle;     
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/billsubject/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qcomclass' name='qcomclass' value='" + qcomclass + "'>";
                    tmpform += "<input type=hidden id='qsubjecttitle' name='qsubjecttitle' value='" + qsubjecttitle + "'>";
                    tmpform += "<input type=hidden id='qitemcode' name='qitemcode' value='" + qitemcode + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                   // return RedirectToAction("List");

                }
            }

           
        }

    
        [ValidateInput(false)]
        public ActionResult Edit(billsubject chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "accid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcomclass = "",qitemcode="", qsubjecttitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qcomclass"]))
            {
                qcomclass = Request["qcomclass"].Trim();
                ViewBag.qcomclass = qcomclass;
            }
            if (!string.IsNullOrWhiteSpace(Request["qitemcode"]))
            {

                qitemcode = Request["qitemcode"].Trim();
                ViewBag.qitemcode = qitemcode;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsubjecttitle"]))
            {

                qsubjecttitle = Request["qsubjecttitle"].Trim();
                ViewBag.qsubjecttitle = qsubjecttitle;
            }
            
            if (sysflag != "E")
            {
                using (AitagBill_DBContext con = new AitagBill_DBContext())
                {
                    var data = con.billsubject.Where(r => r.accid == chks.accid).FirstOrDefault();
                    billsubject ebillsubjects = con.billsubject.Find(chks.accid);
                    if (ebillsubjects == null)
                    {
                        return HttpNotFound();
                    }
                    return View(ebillsubjects);
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

                    using (AitagBill_DBContext con = new AitagBill_DBContext())
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
                         string sysnote = "類別:" + chks.comclass + "代碼名稱:" + chks.subjecttitle;     
                         dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                         sysconn.Close();
                         sysconn.Dispose();
                        //=================================================

                         string tmpform="";
                         tmpform = "<body onload=qfr1.submit();>";
                         tmpform += "<form name='qfr1' action='/billsubject/List' method='post'>";
                         tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                         tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                         tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                         tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                         tmpform += "<input type=hidden id='qcomclass' name='qcomclass' value='" + qcomclass + "'>";
                         tmpform += "<input type=hidden id='qsubjecttitle' name='qsubjecttitle' value='" + qsubjecttitle + "'>";
                         tmpform += "<input type=hidden id='qitemcode' name='qitemcode' value='" + qitemcode + "'>";
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
            { orderdata = "accid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcomclass = "", qitemcode = "",qsubjecttitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qcomclass"]))
            {
                qcomclass = Request["qcomclass"].Trim();
                ViewBag.qcomclass = qcomclass;
            }
            if (!string.IsNullOrWhiteSpace(Request["qitemcode"]))
            {

                qitemcode = Request["qitemcode"].Trim();
                ViewBag.qitemcode = qitemcode;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsubjecttitle"]))
            {

                qsubjecttitle = Request["qsubjecttitle"].Trim();
                ViewBag.qsubjecttitle = qsubjecttitle;
            }

            IPagedList<billsubject> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "select * from billsubject where";
                if (qcomclass != "")
                    sqlstr += " comclass like '%" + qcomclass + "%'  and";
                if (qitemcode != "")
                    sqlstr += " itemcode like '%" + qitemcode + "%'  and";
                if (qsubjecttitle != "")
                    sqlstr += " subjecttitle like '%" + qsubjecttitle + "%'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.billsubject.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<billsubject>(page.Value - 1, (int)Session["pagesize"]);
            
            }
            return View(result);
        }

     


        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qcomclass = "",qitemcode="", qsubjecttitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qcomclass"]))
            {
                qcomclass = Request["qcomclass"].Trim();
                ViewBag.qcomclass = qcomclass;
            }
            if (!string.IsNullOrWhiteSpace(Request["qitemcode"]))
            {

                qitemcode = Request["qitemcode"].Trim();
                ViewBag.qitemcode = qitemcode;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsubjecttitle"]))
            {

                qsubjecttitle = Request["qsubjecttitle"].Trim();
                ViewBag.qsubjecttitle = qsubjecttitle;
            }
            string cdel = Request["cdel"];

            if (string.IsNullOrWhiteSpace(cdel))
            {

                string tgourl = "/billsubject/List?page=" + page + "&qcomclass=" + qcomclass + "&qsubjecttitle=" + qsubjecttitle + "&qitemcode=" + qitemcode;
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
                            string ebillsubjects = dbobj.get_dbvalue(conn1, "select subjecttitle from billsubject where accid ='" + condtionArr[i].ToString() + "'");

                            sysnote += "代碼名稱:" + ebillsubjects + "，序號:" + condtionArr[i].ToString() +"<br>";

                            dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM billsubject where accid = '" + condtionArr[i].ToString() + "'");
                           
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
                        string tgourl = "/billsubject/List?page=" + page + "&qcomclass=" + qcomclass + "&qsubjecttitle=" + qsubjecttitle + "&qitemcode=" + qitemcode;
                        return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                        //return RedirectToAction("List");
                    }
            }
        }



    }
}
