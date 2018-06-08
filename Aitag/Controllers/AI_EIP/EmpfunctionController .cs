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
    public class empfunctionController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /empfunction/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.empdid = Session["empdid"].ToString();
        //    empfunction col = new empfunction();
        //    return View(col);
        //}

        //[HttpPost]
        public ActionResult add(empfunction col, string sysflag, int? page, string orderdata, string orderdata1,string test)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "empdid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qempdid = "", funid = "";
            if (!string.IsNullOrWhiteSpace(Request["qempdid"]))
            {
                qempdid = Request["qempdid"].Trim();
                ViewBag.qempdid = qempdid;
            }
            if (!string.IsNullOrWhiteSpace(Request["funid"]))
            {

                funid = Request["funid"].Trim();
                ViewBag.funid = funid;
            }

            if (sysflag != "A")
            {
                empfunction newcol = new empfunction();
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
                   // SqlDataReader dr;
                    SqlCommand sqlsmd = new SqlCommand();
                    sqlsmd.Connection = conn;
                    #region 註解
                    //string sqlstr = "select empdid from empfunction where empdid = '" + col.empdid + "'";
                    //sqlsmd.CommandText = sqlstr;
                    //dr = sqlsmd.ExecuteReader();

                    //if (dr.Read())
                    //{

                    //    ModelState.AddModelError("", "權限代碼重複!");
                    //    return View(col);
                    //}
                    //dr.Close();
                    //dr.Dispose();
                    //sqlsmd.Dispose();
                    //conn.Close();
                    //conn.Dispose();
                    #endregion

                    //密碼加密
                    //col.emppasswd = dbobj.Encrypt(col.emppasswd);
                    //col.empdid = col.emppasswd;
                    //col.baddid = Session["tempid"].ToString();
                    col.bmodid = Session["tempid"].ToString();
                    col.empid = "99999999";
                    //col.badddate = DateTime.Now;
                    col.bmoddate = DateTime.Now;
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.empfunction.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string dbdata = dbobj.get_dbvalue(sysconn, "select chkitem from checkcode where chkclass='08' and chkcode='" + col.funid +"'");
                        string sysnote = "共用首頁設定:" + dbdata + "的資料";
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/empfunction/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qempdid' name='qempdid' value='" + qempdid + "'>";
                    tmpform += "<input type=hidden id='funid' name='funid' value='" + funid + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    // return RedirectToAction("List");

                }
            }


        }


        [ValidateInput(false)]
        public ActionResult Edit(empfunction chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "funorder"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qempdid = "", funid = "";
            if (!string.IsNullOrWhiteSpace(Request["qempdid"]))
            {
                qempdid = Request["qempdid"].Trim();
                ViewBag.qempdid = qempdid;
            }
            if (!string.IsNullOrWhiteSpace(Request["funid"]))
            {

                funid = Request["funid"].Trim();
                ViewBag.funid = funid;
            }

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.empfunction.Where(r => r.empdid == chks.empdid).FirstOrDefault();
                    empfunction eempfunctions = con.empfunction.Find(chks.empdid);
                    if (eempfunctions == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eempfunctions);
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

                    //string empdid = Request["empdid"];                 

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {

                        NDcommon dbobj = new NDcommon();
                        chks.empid = "99999999";
                        chks.bmodid = Session["tempid"].ToString();
                        chks.bmoddate = DateTime.Now;
                        con.Entry(chks).State = EntityState.Modified;
                        con.SaveChanges();


                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string dbdata = dbobj.get_dbvalue(sysconn, "select chkitem from checkcode where chkclass='08' and chkcode='" + chks.funid + "'");
                        string sysnote = "共用首頁設定:" + dbdata + "的資料";
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/empfunction/List' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                        tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                        tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                        tmpform += "<input type=hidden id='qempdid' name='qempdid' value='" + qempdid + "'>";
                        tmpform += "<input type=hidden id='funid' name='funid' value='" + funid + "'>";
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
            { orderdata = "funorder"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qempdid = "", funid = "";
            if (!string.IsNullOrWhiteSpace(Request["qempdid"]))
            {
                qempdid = Request["qempdid"].Trim();
                ViewBag.qempdid = qempdid;
            }
            if (!string.IsNullOrWhiteSpace(Request["funid"]))
            {

                funid = Request["funid"].Trim();
                ViewBag.funid = funid;
            }

            IPagedList<empfunction> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from empfunction where empid ='99999999' ";

                //sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
   
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.empfunction.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<empfunction>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }

        public ActionResult getallpage(int? page, empfunction emp, string orderdata, string orderdata1)
        {
            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            SqlConnection conn = dbobj.get_conn("Aitag_DBContext");
            SqlConnection conn2 = dbobj.get_conn("Aitag_DBContext");
            SqlDataReader dr,dr2;
            SqlCommand sqlsmd = new SqlCommand();
            SqlCommand sqlcmd = new SqlCommand();
            //List<empfunction> datalist = new List<empfunction>();
            sqlsmd.Connection = conn;
            sqlcmd.Connection = conn2;
            sqlcmd.CommandText = "delete empfunction where empid <> '99999999'";
            sqlcmd.ExecuteNonQuery();
            string sqlstr = "select * from employee where empstatus not in ('3','4') and ifuse='y'";
            sqlcmd.CommandText = sqlstr;
           string  qempdid = Request["qempdid"].Trim();
           string funid = Request["funid"].Trim(); 
          
            dr2 = sqlcmd.ExecuteReader();
           
                if (dr2.HasRows)
                {
                    while (dr2.Read())
                    {
                        string sql = "select * from empfunction where empid = '99999999' ";
                        sqlsmd.CommandText = sql;
                         dr = sqlsmd.ExecuteReader();
                            while (dr.Read())
                            {
                                emp.empid = dr2["empid"].ToString();
                                emp.funid = dr["funid"].ToString();
                                emp.funposition = dr["funposition"].ToString();
                                emp.funorder = Convert.ToInt32(dr["funorder"]);
                                emp.ifshowalert = dr["ifshowalert"].ToString();
                                emp.funrowcount = Convert.ToInt32(dr["funrowcount"]);
                                emp.comid = Session["comid"].ToString();
                                emp.bmodid = Session["tempid"].ToString();
                                emp.bmoddate = DateTime.Now;
                                using (Aitag_DBContext con = new Aitag_DBContext())
                                {
                                    con.empfunction.Add(emp);
                                    con.SaveChanges();
                                }
                            }
                            dr.Close();
                            dr.Dispose();
                    }
                }
            dr2.Close();
            dr2.Dispose();
            sqlsmd.Dispose();
            sqlcmd.Dispose();
            conn.Close();
            conn.Dispose();
            conn2.Close();
            conn2.Dispose();
          
            //系統LOG檔 //================================================= //
            SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
            string sysrealsid = Request["sysrealsid"].ToString();
            string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
            string sysnote = "公司代碼：:" + Session["comid"].ToString() + "的員工個人化首頁重新產生";
            string sysflag = "M";
            dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
            sysconn.Close();
            sysconn.Dispose();
            //=================================================
            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/empfunction/List' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden id='qempdid' name='qempdid' value='" + qempdid + "'>";
            tmpform += "<input type=hidden id='funid' name='funid' value='" + funid + "'>";
            tmpform += "</form>";
            tmpform += "</body>";


            return new ContentResult() { Content = @"<script>alert('員工個人化首頁產生成功!!');</script>" + tmpform };
           
            
        }


        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qempdid = "", funid = "";
            if (!string.IsNullOrWhiteSpace(Request["qempdid"]))
            {
                qempdid = Request["qempdid"].Trim();
                ViewBag.qempdid = qempdid;
            }
            if (!string.IsNullOrWhiteSpace(Request["funid"]))
            {

                funid = Request["funid"].Trim();
                ViewBag.funid = funid;
            }
            string cdel = Request["cdel"];

            if (string.IsNullOrWhiteSpace(cdel))
            {

                string tgourl = "/empfunction/List?page=" + page + "&qempdid=" + qempdid + "&funid=" + funid;
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
                        string eempfunctions = dbobj.get_dbvalue(conn1, "select mttitle from empfunction where empdid ='" + condtionArr[i].ToString() + "'");

                        sysnote += "代碼名稱:" + eempfunctions + "，序號:" + condtionArr[i].ToString() + "<br>";

                        dbobj.dbexecute("Aitag_DBContext", "DELETE FROM empfunction where empdid = '" + condtionArr[i].ToString() + "'");
                     
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
                    string tgourl = "/empfunction/List?page=" + page + "&qempdid=" + qempdid + "&funid=" + funid;
                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                    //return RedirectToAction("List");
                }
            }
        }



    }
}
