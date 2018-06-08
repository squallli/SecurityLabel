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
    public class oaController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /Checkcode/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult maincontentview()
        {
            return View();
        }

     
        public ActionResult maincontentadd(maincontent col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "mcid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qkeyword = "", qsmdate = "", qemdate = "", qmclassid = "";
            if (!string.IsNullOrWhiteSpace(Request["qkeyword"]))
            {
                qkeyword = Request["qkeyword"].Trim();
                ViewBag.qkeyword = qkeyword;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsmdate"]))
            {

                qsmdate = Request["qsmdate"].Trim();
                ViewBag.qsmdate = qsmdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qemdate"]))
            {

                qemdate = Request["qemdate"].Trim();
                ViewBag.qemdate = qemdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qmclassid"]))
            {
                qmclassid = Request["qmclassid"].Trim();
                ViewBag.qmclassid = qmclassid;
            }

            if (sysflag != "A")
            {
                maincontent newcol = new maincontent();
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
                    Session["mcid"] = "";


                    SqlConnection conn = dbobj.get_conn("Aitag_DBContext");
                    SqlDataReader dr;
                    SqlCommand sqlsmd = new SqlCommand();
                    sqlsmd.Connection = conn;
                    string sqlstr = "select * from sublevel1 where sid = '" + Request["realsid"].ToString() + "'";
                    sqlsmd.CommandText = sqlstr;
                    dr = sqlsmd.ExecuteReader();
                    string functype = "";
                    if (dr.Read())
                    {
                        //取得功能代號
                        functype = dr["functype"].ToString();
                    }

                    dr.Close();
                    dr.Dispose();
                    conn.Close();
                    conn.Dispose();
                   
                    int mcid = 0;
                    maincontent mobj;
                     using (Aitag_DBContext con = new Aitag_DBContext())
                    {
       		                if(Request["mcid"].ToString()!="")
                            {
                            mcid = int.Parse(Request["mcid"].ToString()) ;
	                        //sqlstr = "select * from maincontent where mcid = '" +  Request["mcid"].ToString() + "'";
                            var data = con.maincontent.Where(r => r.mcid == mcid).FirstOrDefault();
                            mobj = con.maincontent.Find(mcid);
			                }
                            else
                            {
                            mobj = new maincontent();
                            }
	                        mobj.mctype = functype;
                            mobj.mctitle = Request["mctitle"].ToString().Trim();
                            mobj.mchttp = Request["mchttp"].ToString().Trim();
                            mobj.mccontent = Request["mccontent"].ToString().Trim();
	               
	                        if(Request["qmcparentid"].ToString() != "")
                            {
                             mobj.mcparentid = int.Parse(Request["mcparentid"].ToString().Trim());
	                        }
                            mobj.mcfiletype = Request["mcfiletype"].ToString().Trim();
                            mobj.mclassid = int.Parse(Request["mclassid"].ToString().Trim());
                            //        mobj.mcplace = Request["mcplace"].ToString().Trim();
	                        mobj.sid = int.Parse(Request["realsid"].ToString().Trim());
	                        mobj.mdate = DateTime.Parse(Request["mdate"].ToString());
	                        mobj.mclick = 0;
                        //%>
                        //<!--#include file=addprivtbcount.asp-->
                        //<%	
	                        mobj.ownman = Session["empid"].ToString();
                            mobj.comid = Session["comid"].ToString();
                            mobj.bmodid = Session["empid"].ToString();
                            mobj.bmoddate = DateTime.Now;
	                    if(Request["mcid"].ToString()!="")
                        {
                            con.Entry(mobj).State = EntityState.Modified;
                            con.SaveChanges();
                      
                        }else{
                            con.maincontent.Add(mobj);
                            con.SaveChanges();
                        }

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "類別:" ;     
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/oa/maincontent' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qkeyword' name='qkeyword' value='" + qkeyword + "'>";
                    tmpform += "<input type=hidden id='qsmdate' name='qsmdate' value='" + qsmdate + "'>";
                    tmpform += "<input type=hidden id='qemdate' name='qemdate' value='" + qemdate + "'>";
                    tmpform += "<input type=hidden id='qmclassid' name='qmclassid' value='" + qmclassid + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                   // return RedirectToAction("List");

                }
            }

           
        }

    
        [ValidateInput(false)]
        public ActionResult maincontentmod(maincontent chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "mcid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qkeyword = "", qsmdate = "", qemdate = "", qmclassid = "" ;
            int mcid = 0;
            if (!string.IsNullOrWhiteSpace(Request["mcid"]))
            {
                mcid = int.Parse(Request["mcid"].Trim());
               // ViewBag.qkeyword = qkeyword;
            }
            if (!string.IsNullOrWhiteSpace(Request["qkeyword"]))
            {
                qkeyword = Request["qkeyword"].Trim();
                ViewBag.qkeyword = qkeyword;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsmdate"]))
            {

                qsmdate = Request["qsmdate"].Trim();
                ViewBag.qsmdate = qsmdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qemdate"]))
            {

                qemdate = Request["qemdate"].Trim();
                ViewBag.qemdate = qemdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qmclassid"]))
            {
                qmclassid = Request["qmclassid"].Trim();
                ViewBag.qmclassid = qmclassid;
            }
            
            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.maincontent.Where(r => r.mcid == mcid).FirstOrDefault();
                    maincontent eCheckcodes = con.maincontent.Find(mcid);
                    if (eCheckcodes == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eCheckcodes);
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
                         string sysnote = "序號:" + chks.mcid + "標題名稱:" + chks.mctitle;     
                         dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                         sysconn.Close();
                         sysconn.Dispose();
                        //=================================================

                         string tmpform="";
                         tmpform = "<body onload=qfr1.submit();>";
                         tmpform += "<form name='qfr1' action='/oa/maincontent' method='post'>";
                         tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                         tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                         tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                         tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                         tmpform += "<input type=hidden id='qkeyword' name='qkeyword' value='" + qkeyword + "'>";
                         tmpform += "<input type=hidden id='qsmdate' name='qsmdate' value='" + qsmdate + "'>";
                         tmpform += "<input type=hidden id='qemdate' name='qemdate' value='" + qemdate + "'>";
                         tmpform += "<input type=hidden id='qmclassid' name='qmclassid' value='" + qmclassid + "'>";
                         tmpform +="</form>";
                         tmpform +="</body>";


                         return new ContentResult() { Content = @"" + tmpform };
                        //return RedirectToAction("List");
                    }
               }
            }
         
        }

        public ActionResult maincontent(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "mcid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qkeyword = "", qsmdate = "", qemdate = "" , qmclassid="";
            if (!string.IsNullOrWhiteSpace(Request["qkeyword"]))
            {
                qkeyword = Request["qkeyword"].Trim();
                ViewBag.qkeyword = qkeyword;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsmdate"]))
            {

                qsmdate = Request["qsmdate"].Trim();
                ViewBag.qsmdate = qsmdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qemdate"]))
            {

                qemdate = Request["qemdate"].Trim();
                ViewBag.qemdate = qemdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qmclassid"]))
            {
                qmclassid = Request["qmclassid"].Trim();
                ViewBag.qmclassid = qmclassid;
            }

            IPagedList<maincontent> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from maincontent where sid = '" + Session["realsid"].ToString() + "'   and";
                if (qkeyword != "")
                    sqlstr += " (mctitle like '%" + qkeyword + "%' or  mccontent like '%" + qkeyword + "%')  and";
                if (qsmdate != "")
                    sqlstr += " mdate >= '" + qsmdate + "'  and";
                if (qemdate != "")
                    sqlstr += " mdate <= '" + qemdate + "'  and";
                if (qmclassid != "")
                    sqlstr += " mclassid = '" + qmclassid + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.maincontent.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<maincontent>(page.Value - 1, (int)Session["pagesize"]);
            
            }
            return View(result);
        }

     


        [ActionName("maincontentdel")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qkeyword = "", qsmdate = "", qemdate = "",qmclassid="";
            if (!string.IsNullOrWhiteSpace(Request["qkeyword"]))
            {
                qkeyword = Request["qkeyword"].Trim();
                ViewBag.qkeyword = qkeyword;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsmdate"]))
            {

                qsmdate = Request["qsmdate"].Trim();
                ViewBag.qsmdate = qsmdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qemdate"]))
            {

                qemdate = Request["qemdate"].Trim();
                ViewBag.qemdate = qemdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qmclassid"]))
            {

                qmclassid = Request["qmclassid"].Trim();
                ViewBag.qmclassid = qmclassid;
            }
            string cdel = Request["cdel"];

            if (string.IsNullOrWhiteSpace(cdel))
            {

                string tgourl = "/oa/maincontent?page=" + page + "&qkeyword=" + qkeyword + "&qsmdate=" + qsmdate + "&qemdate=" + qemdate + "&qmclassid=" + qmclassid;
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
                            string maincontent1 = dbobj.get_dbvalue(conn1, "select mctitle from maincontent where mcid ='" + condtionArr[i].ToString() + "'");

                            sysnote += "標題名稱:" + maincontent1 + "，序號:" + condtionArr[i].ToString() + "<br>";

                            dbobj.dbexecute("Aitag_DBContext", "DELETE FROM maincontent where mcid = '" + condtionArr[i].ToString() + "'");
                            
                             //砍檔案
	                        string sql = "select * from contupload where mcid = '" + condtionArr[i].ToString() + "'";
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = conn1 ;
                            cmd.CommandText = sql ;
                            SqlDataReader rs1 =  cmd.ExecuteReader();
	                        while(rs1.Read()){
                                //砍檔案
                                try
                                {
                                    System.IO.File.Delete(Server.MapPath("/upload/" + rs1["cupfile"].ToString()));
                                    System.IO.File.Delete(Server.MapPath("/downfile/" + rs1["cfilename"].ToString()));
                                }
                                catch (Exception e)
                                { ; }
	                                                
	                        }
	                        rs1.Close();
                            rs1.Dispose();
                            
                            dbobj.dbexecute("Aitag_DBContext", "DELETE FROM contupload where mcid = '" + condtionArr[i].ToString() + "'");
                           
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
                        string tgourl = "/oa/maincontent?page=" + page + "&qkeyword=" + qkeyword + "&qsmdate=" + qsmdate + "&qemdate=" + qemdate + "&qmclassid=" + qmclassid;
                        return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                        //return RedirectToAction("List");
                    }
            }
        }

       /* public ActionResult cupid()
        {
           return View();
        }*/

        public ActionResult cupfile()
        {
            return View();
        }

        public ActionResult contuploaddel(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qkeyword = "", qsmdate = "", qemdate = "", qmclassid = "";
            if (!string.IsNullOrWhiteSpace(Request["qkeyword"]))
            {
                qkeyword = Request["qkeyword"].Trim();
                ViewBag.qkeyword = qkeyword;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsmdate"]))
            {

                qsmdate = Request["qsmdate"].Trim();
                ViewBag.qsmdate = qsmdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qemdate"]))
            {

                qemdate = Request["qemdate"].Trim();
                ViewBag.qemdate = qemdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qmclassid"]))
            {

                qmclassid = Request["qmclassid"].Trim();
                ViewBag.qmclassid = qmclassid;
            }
            string cdel = Request["cdel"];

            if (string.IsNullOrWhiteSpace(cdel))
            {

                string tgourl = "/oa/maincontentmod?page=" + page + "&qkeyword=" + qkeyword + "&qsmdate=" + qsmdate + "&qemdate=" + qemdate + "&qmclassid=" + qmclassid;
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
                    string mcid = "";
                    for (int i = 0; i < condtionLen; i++)
                    {
                        string maincontent1 = dbobj.get_dbvalue(conn1, "select cfilename from contupload where cupid ='" + condtionArr[i].ToString() + "'");
                        mcid = dbobj.get_dbvalue(conn1, "select mcid from contupload where cupid ='" + condtionArr[i].ToString() + "'");

                        sysnote += "檔案名稱:" + maincontent1 + "，序號:" + condtionArr[i].ToString() + "，內容編號" + mcid + "<br>";

                        dbobj.dbexecute("Aitag_DBContext", "DELETE FROM contupload where cupid = '" + condtionArr[i].ToString() + "'");
                    }

                    conn1.Close();
                    conn1.Dispose();
                    string sysrealsid = Session["realsid"].ToString();
                    //系統LOG檔
                    //================================================= //                  
                    SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    string sysflag = "D";
                    dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    sysconn.Close();
                    sysconn.Dispose();
                    //======================================================          
                    string tgourl = "/oa/maincontentmod?page=" + page + "&qkeyword=" + qkeyword + "&qsmdate=" + qsmdate + "&qemdate=" + qemdate + "&qmclassid=" + qmclassid + "&mcid=" + mcid + "&sid=" + Session["sid"] + "&realsid=" + Session["realsid"] + "&qmcparentid=" + Request["qmcparentid"].ToString();
                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                    //return RedirectToAction("List");
                }
            }
        }



    }
}
