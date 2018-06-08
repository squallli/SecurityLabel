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
    public class PrivroleController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /privrole/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }
        
        public ActionResult PriCheck()
        {
            ViewBag.psid = Request["psid"].ToString();
            ViewBag.msid = Request["msid"].ToString();
            return View();
        }
       
        public ActionResult PriCheckdo()
        {
            string bid = Request["bid"];
            //權限角色同步
            if (Request["fsubmit"].ToString() == "1")
            {
                string msid = bid;
                NDcommon dbobj = new NDcommon();
                SqlConnection conn = dbobj.get_conn("Aitag_DBContext");

                SqlDataReader dr;
                SqlCommand sqlsmd = new SqlCommand();
                sqlsmd.Connection = conn;

                string sqlstr = "select * from employee where msid = '" + msid + "'";
                //string sqlstr = "select * from Privtb where bid = '" + Request["id"].ToString() + "'";
                sqlsmd.CommandText = sqlstr;
                dr = sqlsmd.ExecuteReader();

                while (dr.Read())
                {
                    dbobj.dbexecute("Aitag_DBContext", "DELETE FROM Privtb where bid = '" + dr["empid"].ToString() + "'");
                    dbobj.addPrivtb(msid, dr["empid"].ToString());
                }
                dr.Close();
                dr.Dispose();
                conn.Close();
                conn.Dispose();



                return new ContentResult() { Content = @"<script>alert('完成權限角色同步!!');location.href='/Privrole/PriCheck?msid=" + bid + "&psid=2'</script>" };
            }
            else
            {//修改權限
                string psid = Request["psid"].ToString(); 
                if (Request["privdata"] != null)
                {
                    NDcommon dbobj = new NDcommon();
                    dbobj.dbexecute("Aitag_DBContext", "DELETE FROM Privtb where bid = '" + bid + "' and psid = '" + psid + "'");
                    string privstr = Request["privdata"];
                    string[] pvarr = privstr.Split(',');
                     
                    //NDcommon dbobj = new NDcommon();
                    SqlConnection conn = dbobj.get_conn("Aitag_DBContext");
                    SqlCommand sqlsmd = new SqlCommand();
                    sqlsmd.Connection = conn;
                    for (int i = 0; i < pvarr.Length; i++)
                    {
                        //string psid = dbobj.get_dbvalue(conn, "select distinct psid from sublevel1 where sid = '" + pvarr[i].ToString().Trim() + "'");
                        if (pvarr[i].ToString().Trim() != "")
                        {
                            sqlsmd.CommandText = "insert into Privtb(sid,bid,psid,chk,subread,subadd,submod,subdel,Bmodid,Bmoddate) values('" + pvarr[i].ToString().Trim() + "','" + bid + "','" + psid + "','1','1','1','1','1','" + Session["empid"].ToString() + "',getdate())";
                            sqlsmd.ExecuteNonQuery();
                        }
                    }
                    conn.Close();
                    conn.Dispose();

                    //系統LOG檔 //================================================= //
                    // iMedia.Models.NDcommon dbobj = new iMedia.Models.NDcommon();
                    string syssubname = "系統管理作業 > 使用者管理作業(權限)";
                    string sysnote = "帳號:" + bid;
                    string sysflag = "M";
                    SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    sysconn.Close();
                    sysconn.Dispose();
                    //=================================================
                    //var oldPolicyManagement = con.Privtbs.Where(r => r.bid == "adm" && data.PolicyManagement.Contains(r.sid)).ToList();

                    return new ContentResult() { Content = @"<script>alert('權限修改成功!!');location.href='/Privrole/PriCheck/?msid=" + bid + "&psid=2'</script>" };
                }
                else
                {
                    return new ContentResult() { Content = @"<script>alert('請挑選功能權限!!');location.href='/Privrole/PriCheck/?msid=" + bid + "&psid=2'</script>" };
                }
                //return RedirectToAction("PriCheck");
            }
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.Msid = Session["Msid"].ToString();
        //    privrole col = new privrole();
        //    return View(col);
        //}

        //[HttpPost]
        public ActionResult add(privrole col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "msid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qmsid = "", qmstitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qmsid"]))
            {
                qmsid = Request["qmsid"].Trim();
                ViewBag.qmsid = qmsid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qmstitle"]))
            {

                qmstitle = Request["qmstitle"].Trim();
                ViewBag.qmstitle = qmstitle;
            }

            if (sysflag != "A")
            {
                privrole newcol = new privrole();
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
                    string sqlstr = "select msid from privrole where msid = '" + col.msid + "'";
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
                    //col.msid = col.emppasswd;
                    //col.baddid = Session["tempid"].ToString();
                    col.bmodid = Session["tempid"].ToString();
                    //col.badddate = DateTime.Now;
                    col.bmoddate = DateTime.Now;
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.privrole.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "代碼:" + col.msid + "名稱:" + col.mstitle;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/privrole/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qmsid' name='qmsid' value='" + qmsid + "'>";
                    tmpform += "<input type=hidden id='qmstitle' name='qmstitle' value='" + qmstitle + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    // return RedirectToAction("List");

                }
            }


        }


        [ValidateInput(false)]
        public ActionResult Edit(privrole chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "msid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qmsid = "", qmstitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qmsid"]))
            {
                qmsid = Request["qmsid"].Trim();
                ViewBag.qmsid = qmsid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qmstitle"]))
            {

                qmstitle = Request["qmstitle"].Trim();
                ViewBag.qmstitle = qmstitle;
            }

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.privrole.Where(r => r.msid == chks.msid).FirstOrDefault();
                    privrole eprivroles = con.privrole.Find(chks.msid);
                    if (eprivroles == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eprivroles);
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
                        string sysnote = "代碼:" + chks.msid + "名稱:" + chks.mstitle;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/privrole/List' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                        tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                        tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                        tmpform += "<input type=hidden id='qmsid' name='qmsid' value='" + qmsid + "'>";
                        tmpform += "<input type=hidden id='qmstitle' name='qmstitle' value='" + qmstitle + "'>";
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
            { orderdata = "msid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qmsid = "", qmstitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qmsid"]))
            {
                qmsid = Request["qmsid"].Trim();
                ViewBag.qmsid = qmsid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qmstitle"]))
            {

                qmstitle = Request["qmstitle"].Trim();
                ViewBag.qmstitle = qmstitle;
            }

            IPagedList<privrole> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from privrole where";
                if (qmsid != "")
                    sqlstr += " msid like '%" + qmsid + "%'  and";
                if (qmstitle != "")
                    sqlstr += " mstitle like '%" + qmstitle + "%'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.privrole.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<privrole>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }




        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qmsid = "", qmstitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qmsid"]))
            {
                qmsid = Request["qmsid"].Trim();
                ViewBag.qmsid = qmsid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qmstitle"]))
            {

                qmstitle = Request["qmstitle"].Trim();
                ViewBag.qmstitle = qmstitle;
            }
            string cdel = Request["cdel"];

            if (string.IsNullOrWhiteSpace(cdel))
            {

                string tgourl = "/privrole/List?page=" + page + "&qmsid=" + qmsid + "&qmstitle=" + qmstitle;
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
                        string eprivroles = dbobj.get_dbvalue(conn1, "select mstitle from privrole where msid ='" + condtionArr[i].ToString() + "'");

                        sysnote += "代碼名稱:" + eprivroles + "，序號:" + condtionArr[i].ToString() + "<br>";

                        dbobj.dbexecute("Aitag_DBContext", "DELETE FROM privrole where msid = '" + condtionArr[i].ToString() + "'");
                        dbobj.dbexecute("Aitag_DBContext", "DELETE FROM privroledet where bid = '" + condtionArr[i].ToString() + "'");

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
                    string tgourl = "/privrole/List?page=" + page + "&qmsid=" + qmsid + "&qmstitle=" + qmstitle;
                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                    //return RedirectToAction("List");
                }
            }
        }



    }
}
