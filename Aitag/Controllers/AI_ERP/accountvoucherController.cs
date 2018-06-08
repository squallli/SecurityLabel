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
    public class accountvoucherController : BaseController
    {

        //private AitagBill_DBContext db = new AitagBill_DBContext();
        //
        // GET: /accountvoucher/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult Listxls()
        {
            string qvcsno = "", qsubjectcode = "", qvouabstract = "", qspinvdate = "", qepinvdate = "" , qvouno="";
            if (!string.IsNullOrWhiteSpace(Request["qvcsno"]))
            {
                qvcsno = Request["qvcsno"].Trim();
                ViewBag.qvcsno = qvcsno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsubjectcode"]))
            {

                qsubjectcode = Request["qsubjectcode"].Trim();
                ViewBag.qsubjectcode = qsubjectcode;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvouabstract"]))
            {

                qvouabstract = Request["qvouabstract"].Trim();
                ViewBag.qvouabstract = qvouabstract;
            }

            if (!string.IsNullOrWhiteSpace(Request["qspinvdate"]))
            {

                qspinvdate = Request["qspinvdate"].Trim();
                ViewBag.qspinvdate = qspinvdate;
            }

            if (!string.IsNullOrWhiteSpace(Request["qepinvdate"]))
            {

                qepinvdate = Request["qepinvdate"].Trim();
                ViewBag.qepinvdate = qepinvdate;
            }

            if (!string.IsNullOrWhiteSpace(Request["qvouno"]))
            {

                qvouno = Request["qvouno"].Trim();
                ViewBag.qvouno = qvouno;
            }

            return View();
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.Msid = Session["Msid"].ToString();
        //    accountvoucher col = new accountvoucher();
        //    return View(col);
        //}

        //[HttpPost]
        public ActionResult add(accountvoucher col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "voudid"; }

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
                accountvoucher newcol = new accountvoucher();
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
                    //SqlConnection conn = dbobj.get_conn("AitagBill_DBContext");
                    //SqlDataReader dr;
                    //SqlCommand sqlsmd = new SqlCommand();
                    //sqlsmd.Connection = conn;
                    //string sqlstr = "select voudid from accountvoucher where chkclass = '" + col.chkclass + "' and chkcode = '" + col.chkcode + "'";
                    //sqlsmd.CommandText = sqlstr;
                    //dr = sqlsmd.ExecuteReader();

                    //if (dr.Read())
                    //{

                    //    ModelState.AddModelError("", "共用代碼重複!");
                    //    return View(col);
                    //}
                    //dr.Close();
                    //dr.Dispose();
                    //sqlsmd.Dispose();
                    //conn.Close();
                    //conn.Dispose();

                    //密碼加密
                    //col.emppasswd = dbobj.Encrypt(col.emppasswd);
                    //col.chkclass = col.emppasswd;
                    //col.baddid = Session["tempid"].ToString();
                    col.bmodid = Session["tempid"].ToString();
                    //col.badddate = DateTime.Now;
                    col.bmoddate = DateTime.Now;            
                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {
                        con.accountvoucher.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("AitagBill_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "類別:" + col.vouabstract + "代碼名稱:" + col.vouabstract;     
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/accountvoucher/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qchkclass' name='qchkclass' value='" + qchkclass + "'>";
                    tmpform += "<input type=hidden id='qchkitem' name='qchkitem' value='" + qchkitem + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                   // return RedirectToAction("List");

                }
            }

           
        }

    
        [ValidateInput(false)]
        public ActionResult Edit(accountvoucher chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "voudid"; }

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
                using (AitagBill_DBContext con = new AitagBill_DBContext())
                {
                    var data = con.accountvoucher.Where(r => r.voudid == chks.voudid).FirstOrDefault();
                    accountvoucher eaccountvouchers = con.accountvoucher.Find(chks.voudid);
                    if (eaccountvouchers == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eaccountvouchers);
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
                        chks.bmodid = Session["tempid"].ToString();
                        chks.bmoddate = DateTime.Now;
                        con.Entry(chks).State = EntityState.Modified;
                        con.SaveChanges();

                        
                        //系統LOG檔
                        //================================================= //                     
                         SqlConnection sysconn = dbobj.get_conn("AitagBill_DBContext");
                         string sysrealsid = Request["sysrealsid"].ToString();
                         string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                         string sysnote = "類別:" + chks.projno + "代碼名稱:" + chks.projno;     
                         dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                         sysconn.Close();
                         sysconn.Dispose();
                        //=================================================

                         string tmpform="";
                         tmpform = "<body onload=qfr1.submit();>";
                         tmpform += "<form name='qfr1' action='/accountvoucher/List' method='post'>";
                         tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                         tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                         tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                         tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                         tmpform += "<input type=hidden id='qchkclass' name='qchkclass' value='" + qchkclass + "'>";
                         tmpform += "<input type=hidden id='qchkitem' name='qchkitem' value='" + qchkitem + "'>";
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
            { orderdata = "voudid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qvcsno = "" , qvouno ="", qsubjectcode = "", qvouabstract = "", qspinvdate = "", qepinvdate="";
            if (!string.IsNullOrWhiteSpace(Request["qvcsno"]))
            {
                qvcsno = Request["qvcsno"].Trim();
                ViewBag.qvcsno = qvcsno;
            }

            if (!string.IsNullOrWhiteSpace(Request["qvouno"]))
            {
                qvouno = Request["qvouno"].Trim();
                ViewBag.qvouno = qvouno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsubjectcode"]))
            {

                qsubjectcode = Request["qsubjectcode"].Trim();
                ViewBag.qsubjectcode = qsubjectcode;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvouabstract"]))
            {

                qvouabstract = Request["qvouabstract"].Trim();
                ViewBag.qvouabstract = qvouabstract;
            }

            if (!string.IsNullOrWhiteSpace(Request["qspinvdate"]))
            {

                qspinvdate = Request["qspinvdate"].Trim();
                ViewBag.qspinvdate = qspinvdate;
            }

            if (!string.IsNullOrWhiteSpace(Request["qepinvdate"]))
            {

                qepinvdate = Request["qepinvdate"].Trim();
                ViewBag.qepinvdate = qepinvdate;
            }

            

            IPagedList<accountvoucher> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "select * from accountvoucher where";
                if (qvouno != "")
                    sqlstr += " vouno like '%" + qvouno + "%'  and";
                if (qvcsno != "")
                    sqlstr += " vcsno like '%" + qvcsno + "%'  and";
                if (qsubjectcode != "")
                    sqlstr += " subjectcode like '%" + qsubjectcode + "%'  and";
                if (qvouabstract != "")
                    sqlstr += " vouabstract like '%" + qvouabstract + "%'  and";

                if (qspinvdate != "")
                    sqlstr += " pinvdate >= '" + qspinvdate + "'  and";

                if (qepinvdate != "")
                    sqlstr += " pinvdate <= '" + qepinvdate + "'  and";
                

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.accountvoucher.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<accountvoucher>(page.Value - 1, (int)Session["pagesize"]);
            
            }
            return View(result);
        }

        public ActionResult dlist(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "voudid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qvcsno = "", qvouno = "", qsubjectcode = "", qvouabstract = "", qspinvdate = "", qepinvdate = "";
            if (!string.IsNullOrWhiteSpace(Request["qvcsno"]))
            {
                qvcsno = Request["qvcsno"].Trim();
                ViewBag.qvcsno = qvcsno;
            }

            if (!string.IsNullOrWhiteSpace(Request["qvouno"]))
            {
                qvouno = Request["qvouno"].Trim();
                ViewBag.qvouno = qvouno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsubjectcode"]))
            {

                qsubjectcode = Request["qsubjectcode"].Trim();
                ViewBag.qsubjectcode = qsubjectcode;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvouabstract"]))
            {

                qvouabstract = Request["qvouabstract"].Trim();
                ViewBag.qvouabstract = qvouabstract;
            }

            if (!string.IsNullOrWhiteSpace(Request["qspinvdate"]))
            {

                qspinvdate = Request["qspinvdate"].Trim();
                ViewBag.qspinvdate = qspinvdate;
            }

            if (!string.IsNullOrWhiteSpace(Request["qepinvdate"]))
            {

                qepinvdate = Request["qepinvdate"].Trim();
                ViewBag.qepinvdate = qepinvdate;
            }



            IPagedList<accountvoucher> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "select * from accountvoucher where";
                if (qvouno != "")
                    sqlstr += " vouno like '%" + qvouno + "%'  and";
                if (qvcsno != "")
                    sqlstr += " vcsno like '%" + qvcsno + "%'  and";
                if (qsubjectcode != "")
                    sqlstr += " subjectcode like '%" + qsubjectcode + "%'  and";
                if (qvouabstract != "")
                    sqlstr += " vouabstract like '%" + qvouabstract + "%'  and";

                if (qspinvdate != "")
                    sqlstr += " pinvdate >= '" + qspinvdate + "'  and";

                if (qepinvdate != "")
                    sqlstr += " pinvdate <= '" + qepinvdate + "'  and";


                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.accountvoucher.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<accountvoucher>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }


        public ActionResult voucherworkdo(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "itemno"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qvcsno = "", qsubjectcode = "", qvouabstract = "";
            if (!string.IsNullOrWhiteSpace(Request["qvcsno"]))
            {
                qvcsno = Request["qvcsno"].Trim();
                ViewBag.qvcsno = qvcsno;
            }
            else
            {
                return new ContentResult() { Content = @"<script>alert('請輸入單號進行傳票產生!!');window.history.go(-1);</script>" };
            }

            
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                //刪除單號
                NDcommon dbobj = new NDcommon();      
                string sqlstr = "delete from accountvoucher where vcsno = '" + qvcsno + "'";
                SqlConnection conn = dbobj.get_conn("AitagBill_DBContext");
                SqlConnection conn1 = dbobj.get_conn("AitagBill_DBContext");
                SqlCommand sqlsmd = new SqlCommand();
                SqlCommand sqlsmd1 = new SqlCommand();
                sqlsmd.Connection = conn;
                sqlsmd1.Connection = conn1;
                sqlsmd.CommandText = sqlstr;
                sqlsmd.ExecuteNonQuery();
                sqlsmd.CommandText = "select * from vend_contractinv where vcno = '" + qvcsno + "'";
                SqlDataReader dr = sqlsmd.ExecuteReader();
                SqlDataReader dr1;
                int itemno = 10;
                while (dr.Read()) {
                    sqlsmd1.CommandText = "select isnull(sum(taxmoney),0) as taxmoney from incomeaccounts where vcno = '" + qvcsno + "'";
                    dr1 = sqlsmd1.ExecuteReader();
                    int taxmoney = 0;
                    if (dr1.Read())
                    {
                        taxmoney = int.Parse(dr1["taxmoney"].ToString());
                    }
                    dr1.Close();
                    dr1.Dispose();

                    //借方金額
                    accountvoucher col = new accountvoucher();
                    col.billtype = dr["vctype"].ToString();
                    col.vcsubtype = dr["vcsubtype"].ToString();
                    col.vcsno = dr["vcno"].ToString();
                    col.deliveryno = 1;
                    col.pinvdate = DateTime.Parse(dr["vadate"].ToString());
                    col.itemno = itemno;
                    col.subjectcode = "221100";
                    col.subjectname = "(銷/管/研)旅費-國內/外";
                    col.cedeptcode = dr["owndptid"].ToString();
                    col.objno = dr["ownman"].ToString();
                    col.debitsum = int.Parse(dr["totalmoney"].ToString()) - taxmoney;
                    col.creditsum = 0; 
                    //col.baddid = Session["tempid"].ToString();
                    col.bmodid = Session["empid"].ToString();
                    //col.badddate = DateTime.Now;
                    col.bmoddate = DateTime.Now;
                  
                    con.accountvoucher.Add(col);
                    con.SaveChanges();

                    //借方稅額
                    itemno = itemno + 10;
                    col = new accountvoucher();
                    col.billtype = dr["vctype"].ToString();
                    col.vcsubtype = dr["vcsubtype"].ToString();
                    col.vcsno = dr["vcno"].ToString();
                    col.deliveryno = 1;
                    col.pinvdate = DateTime.Parse(dr["vadate"].ToString());
                    col.itemno = itemno;
                    col.subjectcode = "132001";
                    col.subjectname = "進項稅額";
                    col.cedeptcode = dr["owndptid"].ToString();
                    col.objno = dr["ownman"].ToString();
                    col.debitsum = taxmoney;
                    col.creditsum = 0;
                    //col.baddid = Session["tempid"].ToString();
                    col.bmodid = Session["empid"].ToString();
                    //col.badddate = DateTime.Now;
                    col.bmoddate = DateTime.Now;

                    con.accountvoucher.Add(col);
                    con.SaveChanges();
                    
                    //貸方金額
                    itemno = itemno + 10;
                        col = new accountvoucher();
                        col.billtype = dr["vctype"].ToString();
                        col.vcsubtype = dr["vcsubtype"].ToString();
                        col.vcsno = dr["vcno"].ToString();
                        col.deliveryno = 1;
                        col.pinvdate = DateTime.Parse(dr["vadate"].ToString());
                        col.itemno = itemno;
                        col.subjectcode = "2200";
                        col.subjectname = "應付費用";
                        col.cedeptcode = dr["owndptid"].ToString();
                        col.objno = dr["ownman"].ToString();
                        col.debitsum = 0;
                        col.creditsum = int.Parse(dr["totalmoney"].ToString()); 
                        //col.baddid = Session["tempid"].ToString();
                        col.bmodid = Session["empid"].ToString();
                        //col.badddate = DateTime.Now;
                        col.bmoddate = DateTime.Now;

                        con.accountvoucher.Add(col);
                        con.SaveChanges();
                       

                       

                }

                //系統LOG檔 //================================================= //
                SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                string sysrealsid = Request["sysrealsid"].ToString();
                string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                string sysnote = "單號:" + qvcsno;
                dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), "A");
                sysconn.Close();
                sysconn.Dispose();
                //=================================================

                conn.Close();
                conn.Dispose();
                conn1.Close();
                conn1.Dispose();
               
            }
            return new ContentResult() { Content = @"<script>alert('傳票產生成功!!');location.href='/accountvoucher/list';</script>" };
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

                string tgourl = "/accountvoucher/List?page=" + page + "&qchkclass=" + qchkclass + "&qchkitem=" + qchkitem;
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
                            string eaccountvouchers = dbobj.get_dbvalue(conn1, "select chkitem from accountvoucher where voudid ='" + condtionArr[i].ToString() + "'");

                            sysnote += "代碼名稱:" + eaccountvouchers + "，序號:" + condtionArr[i].ToString() +"<br>";

                            dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM accountvoucher where voudid = '" + condtionArr[i].ToString() + "'");
                           
                        }

                        conn1.Close();
                        conn1.Dispose();
                        string sysrealsid = Request["sysrealsid"].ToString();
                        //系統LOG檔
                        //================================================= //                  
                        SqlConnection sysconn = dbobj.get_conn("AitagBill_DBContext");
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");                        
                        string sysflag = "D";                       
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //======================================================          
                        string tgourl = "/accountvoucher/List?page=" + page +"&qchkclass=" + qchkclass + "&qchkitem=" + qchkitem;
                        return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                        //return RedirectToAction("List");
                    }
            }
        }



        public ActionResult Listedit()
        {
            string qvcsno = "", qsubjectcode = "", qvouabstract = "", qspinvdate = "", qepinvdate = "", qvouno = "";

            if (!string.IsNullOrWhiteSpace(Request["qvcsno"]))
            {
                qvcsno = Request["qvcsno"].Trim();
                ViewBag.qvcsno = qvcsno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsubjectcode"]))
            {

                qsubjectcode = Request["qsubjectcode"].Trim();
                ViewBag.qsubjectcode = qsubjectcode;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvouabstract"]))
            {

                qvouabstract = Request["qvouabstract"].Trim();
                ViewBag.qvouabstract = qvouabstract;
            }

        
            if (!string.IsNullOrWhiteSpace(Request["qspinvdate"]))
            {

                qspinvdate = Request["qspinvdate"].Trim();
                ViewBag.qspinvdate = qspinvdate;
               
            }

            if (!string.IsNullOrWhiteSpace(Request["qepinvdate"]))
            {

                qepinvdate = Request["qepinvdate"].Trim();
                ViewBag.qepinvdate = qepinvdate;
            }

            if (!string.IsNullOrWhiteSpace(Request["qvouno"]))
            {

                qvouno = Request["qvouno"].Trim();
                ViewBag.qvouno = qvouno;
            }

            List<accountvoucher> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                var query = con.accountvoucher
                  .AsQueryable();
                result = query.AsQueryable().ToList();               
             

                if (!string.IsNullOrWhiteSpace(qspinvdate))
                {
                     DateTime myDate = System.Convert.ToDateTime(qspinvdate.ToString()); 
                     result = query.Where(r => r.pinvdate >= myDate).AsQueryable().ToList();
                }

                if (!string.IsNullOrWhiteSpace(qepinvdate))
                {
                    DateTime myDate1 = System.Convert.ToDateTime(qepinvdate.ToString()); 
                    result = query.Where(r => r.pinvdate <= myDate1).AsQueryable().ToList();
                }

                if (!string.IsNullOrWhiteSpace(qvcsno))
                {

                    result = query.Where(r => r.vcsno.Contains(qvcsno)).AsQueryable().ToList();
                }

                if (!string.IsNullOrWhiteSpace(qvouno))
                {

                    result = query.Where(r => r.vouno.Contains(qvouno)).AsQueryable().ToList();
                }
            }
            return View(result);
        }


        public ActionResult Listeditdo(string sysflag, int? page, string orderdata, string orderdata1)
        {


            string qvcsno = "", qsubjectcode = "", qvouabstract = "", qspinvdate = "", qepinvdate = "", qvouno = "";

            if (!string.IsNullOrWhiteSpace(Request["qvouno"]))
            {
                qvouno = Request["qvouno"].Trim();
                ViewBag.qvouno = qvouno;
            }
            
            if (!string.IsNullOrWhiteSpace(Request["qvcsno"]))
            {
                qvcsno = Request["qvcsno"].Trim();
                ViewBag.qvcsno = qvcsno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsubjectcode"]))
            {

                qsubjectcode = Request["qsubjectcode"].Trim();
                ViewBag.qsubjectcode = qsubjectcode;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvouabstract"]))
            {

                qvouabstract = Request["qvouabstract"].Trim();
                ViewBag.qvouabstract = qvouabstract;
            }


            if (!string.IsNullOrWhiteSpace(Request["qspinvdate"]))
            {

                qspinvdate = Request["qspinvdate"].Trim();
                ViewBag.qspinvdate = qspinvdate;

            }

            if (!string.IsNullOrWhiteSpace(Request["qepinvdate"]))
            {

                qepinvdate = Request["qepinvdate"].Trim();
                ViewBag.qepinvdate = qepinvdate;
            }

            NDcommon dbobj = new NDcommon();
            SqlConnection erpconn = dbobj.get_conn("AitagBill_DBContext");
            SqlCommand cmd = new SqlCommand();
     
      
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {

                page = ((!page.HasValue || page < 1) ? 1 : page);
                ViewBag.page = page;
                ViewBag.orderdata = orderdata;
                ViewBag.orderdata1 = orderdata1;
                string cdel1 = Request["voudid"];
                string pinvdate1 = Request["pinvdate"];
                string itemno1 = Request["itemno"];
                string subjectcode1 = Request["subjectcode"];
                string subjectname1 = Request["subjectname"];
                string cedeptcode1 = Request["cedeptcode"];
                string projno1 = Request["projno"];
                string objno1 = Request["objno"];
                string ceenddate1 = Request["ceenddate"];
                string vouabstract1 = Request["vouabstract"];
                string debitsum1 = Request["debitsum"];
                string creditsum1 = Request["creditsum"];
                string[] cdelarr = cdel1.Split(',');

                string[] pinvdatearr = pinvdate1.Split(',');
                string[] itemnoarr = itemno1.Split(',');
                string[] subjectcodearr = subjectcode1.Split(',');
                string[] subjectnamearr = subjectname1.Split(',');
                string[] cedeptcodearr = cedeptcode1.Split(',');
                string[] projnoarr = projno1.Split(',');
                string[] objnoarr = objno1.Split(',');
                string[] ceenddatearr = ceenddate1.Split(',');
                string[] vouabstractarr = vouabstract1.Split(',');
                string[] debitsumarr = debitsum1.Split(',');
                string[] creditsumarr = creditsum1.Split(',');

                for (int i = 0; i < cdelarr.Length; i++)
                {
                    if (cdelarr[i].Trim() == "")
                    {
                        if (!(pinvdatearr[i].Trim() == "" && subjectcodearr[i].Trim() == "" && subjectnamearr[i].Trim() == ""))
                        {
                            //accountvoucher addobj = new accountvoucher();
                            //addobj.billtype = "P";
                            //addobj.vcsubtype = Request["vcsubtype"].ToString();
                            //addobj.vcsno = vcno;
                            //addobj.deliveryno = 1;
                            //addobj.pinvdate = DateTime.Parse(pinvdatearr[i].Trim());
                            //addobj.itemno = int.Parse(itemnoarr[i].Trim());
                            //addobj.subjectcode = subjectcodearr[i].Trim();
                            //addobj.subjectname = subjectnamearr[i].Trim();
                            //addobj.cedeptcode = cedeptcodearr[i].Trim();
                            //addobj.objno = objnoarr[i].Trim();

                            //if (ceenddatearr[i] == null || ceenddatearr[i] == "")
                            //{ addobj.ceenddate = null; }
                            //else
                            //{ addobj.ceenddate = DateTime.Parse(ceenddatearr[i].Trim()); }

                            //addobj.projno = projnoarr[i].Trim();
                            //addobj.vouabstract = vouabstractarr[i].Trim();

                            //if (debitsumarr[i] == null || debitsumarr[i] == "")
                            //{ addobj.debitsum = 0; }
                            //else
                            //{ addobj.debitsum = int.Parse(debitsumarr[i].Trim()); }

                            //if (creditsumarr[i] == null || creditsumarr[i] == "")
                            //{ addobj.creditsum = 0; }
                            //else
                            //{ addobj.creditsum = int.Parse(creditsumarr[i].Trim()); }

                            //addobj.bmodid = Session["empid"].ToString();
                            //addobj.bmoddate = DateTime.Now;

                            //con.accountvoucher.Add(addobj);
                            //con.SaveChanges();

                        }
                    }
                    else
                    {

                        int voudid = int.Parse(cdelarr[i].Trim());
                        accountvoucher modobj = con.accountvoucher.Where(r => r.voudid == voudid).FirstOrDefault();

                     
                        modobj.pinvdate = DateTime.Parse(pinvdatearr[i].Trim());
                        modobj.itemno = int.Parse(itemnoarr[i].Trim());
                        modobj.subjectcode = subjectcodearr[i].Trim();
                        modobj.subjectname = subjectnamearr[i].Trim();
                        modobj.cedeptcode = cedeptcodearr[i].Trim();
                        modobj.objno = objnoarr[i].Trim();

                        if (ceenddatearr[i] == null || ceenddatearr[i] == "")
                        { modobj.ceenddate = null; }
                        else
                        { modobj.ceenddate = DateTime.Parse(ceenddatearr[i].Trim()); }

                        modobj.projno = projnoarr[i].Trim();
                        modobj.vouabstract = vouabstractarr[i].Trim();

                        if (debitsumarr[i] == null || debitsumarr[i] == "")
                        { modobj.debitsum = 0; }
                        else
                        { modobj.debitsum = decimal.Parse(debitsumarr[i].Trim()); }

                        if (creditsumarr[i] == null || creditsumarr[i] == "")
                        { modobj.creditsum = 0; }
                        else
                        { modobj.creditsum = decimal.Parse(creditsumarr[i].Trim()); }

                        modobj.bmodid = Session["empid"].ToString();
                        modobj.bmoddate = DateTime.Now;
                        modobj.comid = modobj.comid;


                        con.Entry(modobj).State = EntityState.Modified;
                        con.SaveChanges();



                    }
                }
                con.Dispose();


            }
         
            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/accountvoucher/Listedit' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden name='qspinvdate' id='qspinvdate' value='" + qspinvdate + "'>";
            tmpform += "<input type=hidden name='qepinvdate' id='qepinvdate' value='" + qepinvdate + "'>";
            tmpform += "<input type=hidden name='qvcsno' id='qvcsno' value='" + qvcsno + "'>";
            tmpform += "<input type=hidden name='qvouno' id='qvouno' value='" + qvouno + "'>";
            tmpform += "</form>";
            tmpform += "</body>";

            erpconn.Close();
            erpconn.Dispose();

            return new ContentResult() { Content = @"" + tmpform };
        }


        public ActionResult Listeditdel(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);


            string qvcsno = "", qsubjectcode = "", qvouabstract = "", qspinvdate = "", qepinvdate = "";

            if (!string.IsNullOrWhiteSpace(Request["qvcsno"]))
            {
                qvcsno = Request["qvcsno"].Trim();
                ViewBag.qvcsno = qvcsno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsubjectcode"]))
            {

                qsubjectcode = Request["qsubjectcode"].Trim();
                ViewBag.qsubjectcode = qsubjectcode;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvouabstract"]))
            {

                qvouabstract = Request["qvouabstract"].Trim();
                ViewBag.qvouabstract = qvouabstract;
            }


            if (!string.IsNullOrWhiteSpace(Request["qspinvdate"]))
            {

                qspinvdate = Request["qspinvdate"].Trim();
                ViewBag.qspinvdate = qspinvdate;

            }

            if (!string.IsNullOrWhiteSpace(Request["qepinvdate"]))
            {

                qepinvdate = Request["qepinvdate"].Trim();
                ViewBag.qepinvdate = qepinvdate;
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
                        string money1 = dbobj.get_dbvalue(conn1, "select ('請款單號' + vcsno + ',科目編號' + subjectcode) as st1  from accountvoucher where voudid = '" + condtionArr[i].ToString() + "'");

                        sysnote += money1 + "<br>";

                        //刪除
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM accountvoucher where voudid = '" + condtionArr[i].ToString() + "'");

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
                    //  string tgourl = "/paybill/expenseaccount?vcno=" + vcno;
                    //  return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                    string tmpform = "";
                    tmpform = "<body onload=alert('刪除成功!!');qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/accountvoucher/Listedit' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='qspinvdate' id='qspinvdate' value='" + qspinvdate + "'>";
                    tmpform += "<input type=hidden name='qepinvdate' id='qepinvdate' value='" + qepinvdate + "'>";

                    tmpform += "</form>";
                    tmpform += "</body>";



                    return new ContentResult() { Content = @"" + tmpform };


                }
            }
        }

        //產生批號 vouno yyyy+mm+0001
        public ActionResult Listeditgetno(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);


            string qvcsno = "", qsubjectcode = "", qvouabstract = "", qspinvdate = "", qepinvdate = "";

            if (!string.IsNullOrWhiteSpace(Request["qvcsno"]))
            {
                qvcsno = Request["qvcsno"].Trim();
                ViewBag.qvcsno = qvcsno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsubjectcode"]))
            {

                qsubjectcode = Request["qsubjectcode"].Trim();
                ViewBag.qsubjectcode = qsubjectcode;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvouabstract"]))
            {

                qvouabstract = Request["qvouabstract"].Trim();
                ViewBag.qvouabstract = qvouabstract;
            }


            if (!string.IsNullOrWhiteSpace(Request["qspinvdate"]))
            {

                qspinvdate = Request["qspinvdate"].Trim();
                ViewBag.qspinvdate = qspinvdate;

            }

            if (!string.IsNullOrWhiteSpace(Request["qepinvdate"]))
            {

                qepinvdate = Request["qepinvdate"].Trim();
                ViewBag.qepinvdate = qepinvdate;
            }

            string cdel = Request["cdel"];


            if (string.IsNullOrWhiteSpace(cdel))
            {

                return new ContentResult() { Content = @"<script>alert('請勾選要產生批號的資料!!');window.history.go(-1);</script>" };
            }
            else
            {
                using (AitagBill_DBContext con = new AitagBill_DBContext())
                {

                    NDcommon dbobj = new NDcommon();
                    SqlConnection conn1 = dbobj.get_conn("AitagBill_DBContext");
                    #region 產生批號

                         string  yy = DateTime.Now.Year.ToString();
                         string mm = DateTime.Now.Month.ToString().PadLeft(2, '0');
                         string tmpsql = "select top 1 vouno  from accountvoucher where vouno like '" + yy + mm + "%' order by vouno desc ";
                         string vouno = dbobj.get_dbvalue(conn1, tmpsql) + "";
                          string addvouno = "";
                          if (string.IsNullOrWhiteSpace(vouno))
                          {
                              addvouno = yy + mm + "0001";
                          }
                          else
                          {
                         
                              string tmpsno = vouno.Substring(6, 4);
                              tmpsno = (int.Parse(tmpsno) + 1).ToString().PadLeft(4,'0');
                              addvouno = vouno.Substring(0, 6) + tmpsno;
                          }
                    #endregion
                    string sysnote = "";
                    string[] condtionArr = cdel.Split(',');
                    int condtionLen = condtionArr.Length;
                    for (int i = 0; i < condtionLen; i++)
                    {
                        string money1 = dbobj.get_dbvalue(conn1, "select ('請款單號' + vcsno + '產生批號') as st1  from accountvoucher where vcsno = '" + condtionArr[i].ToString() + "'");

                        sysnote += money1 + "<br>";

                        //更新
                        string upsql = "UPDATE accountvoucher SET vouno ='" + addvouno + "' where vcsno = '" + condtionArr[i].ToString() + "' and comid = '" + Session["comid"] + "'";
                        dbobj.dbexecute("AitagBill_DBContext", upsql);

                    }

                    conn1.Close();
                    conn1.Dispose();
                    string sysrealsid = Request["sysrealsid"].ToString();
                    //系統LOG檔
                    //================================================= //                  
                    SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    string sysflag = "M";
                    dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    sysconn.Close();
                    sysconn.Dispose();
                    //======================================================          
                  

                    string tmpform = "";
                    tmpform = "<body onload=alert('產生成功!!');qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/paybill/accexpensemain' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='qvouno' id='qvouno' value='" + addvouno + "'>";
                    tmpform += "<input type=hidden name='qspinvdate' id='qspinvdate' value='" + qspinvdate + "'>";
                    tmpform += "<input type=hidden name='qepinvdate' id='qepinvdate' value='" + qepinvdate + "'>";

                    tmpform += "</form>";
                    tmpform += "</body>";



                    return new ContentResult() { Content = @"" + tmpform };


                }
            }
        }

        //產生批號 vouno yyyy+mm+0001
        public ActionResult Listeditungetno(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);


            string qvcsno = "", qsubjectcode = "", qvouabstract = "", qspinvdate = "", qepinvdate = "";

            if (!string.IsNullOrWhiteSpace(Request["qvcsno"]))
            {
                qvcsno = Request["qvcsno"].Trim();
                ViewBag.qvcsno = qvcsno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsubjectcode"]))
            {

                qsubjectcode = Request["qsubjectcode"].Trim();
                ViewBag.qsubjectcode = qsubjectcode;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvouabstract"]))
            {

                qvouabstract = Request["qvouabstract"].Trim();
                ViewBag.qvouabstract = qvouabstract;
            }


            if (!string.IsNullOrWhiteSpace(Request["qspinvdate"]))
            {

                qspinvdate = Request["qspinvdate"].Trim();
                ViewBag.qspinvdate = qspinvdate;

            }

            if (!string.IsNullOrWhiteSpace(Request["qepinvdate"]))
            {

                qepinvdate = Request["qepinvdate"].Trim();
                ViewBag.qepinvdate = qepinvdate;
            }

            string cdel = Request["cdel"];


            if (string.IsNullOrWhiteSpace(cdel))
            {

                return new ContentResult() { Content = @"<script>alert('請勾選要產生批號的資料!!');window.history.go(-1);</script>" };
            }
            else
            {
                using (AitagBill_DBContext con = new AitagBill_DBContext())
                {

                    NDcommon dbobj = new NDcommon();
                    SqlConnection conn1 = dbobj.get_conn("AitagBill_DBContext");
                    //#region 產生批號

                    //string yy = DateTime.Now.Year.ToString();
                    //string mm = DateTime.Now.Month.ToString().PadLeft(2, '0');

                    //string vouno = dbobj.get_dbvalue(conn1, "select vouno  from accountvoucher where year(pinvdate)=" + yy + " and Month(pinvdate)=" + mm + " order by vouno desc ") + "";
                    //string addvouno = "";
                    //if (string.IsNullOrWhiteSpace(vouno))
                    //{
                    //    addvouno = yy + mm + "0001";
                    //}
                    //else
                    //{

                    //    string tmpsno = vouno.Substring(6, 4);
                    //    tmpsno = (int.Parse(tmpsno) + 1).ToString().PadLeft(4, '0');
                    //    addvouno = vouno.Substring(0, 6) + tmpsno;
                    //}
                    //#endregion
                    string sysnote = "";
                    string[] condtionArr = cdel.Split(',');
                    int condtionLen = condtionArr.Length;
                    for (int i = 0; i < condtionLen; i++)
                    {
                        string money1 = dbobj.get_dbvalue(conn1, "select ('請款單號' + vcsno + '批號恢復空白') as st1  from accountvoucher where vcsno = '" + condtionArr[i].ToString() + "'");

                        sysnote += money1 + "<br>";

                        //更新
                        string upsql = "UPDATE accountvoucher SET vouno ='' where vcsno = '" + condtionArr[i].ToString() + "' and comid = '" + Session["comid"] + "'";
                        dbobj.dbexecute("AitagBill_DBContext", upsql);

                    }

                    conn1.Close();
                    conn1.Dispose();
                    string sysrealsid = Request["sysrealsid"].ToString();
                    //系統LOG檔
                    //================================================= //                  
                    SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    string sysflag = "M";
                    dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    sysconn.Close();
                    sysconn.Dispose();
                    //======================================================          


                    string tmpform = "";
                    tmpform = "<body onload=alert('還原成功!!');qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/paybill/accexpensemain' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                   // tmpform += "<input type=hidden name='qvouno' id='qvouno' value='" + addvouno + "'>";
                    tmpform += "<input type=hidden name='qspinvdate' id='qspinvdate' value='" + qspinvdate + "'>";
                    tmpform += "<input type=hidden name='qepinvdate' id='qepinvdate' value='" + qepinvdate + "'>";

                    tmpform += "</form>";
                    tmpform += "</body>";



                    return new ContentResult() { Content = @"" + tmpform };


                }
            }
        }
    }
}
