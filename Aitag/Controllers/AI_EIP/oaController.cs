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
using System.IO;
using System.Configuration;
using System.Collections;

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
            NDcommon dbobj = new NDcommon();
            //變更選單SESSION
            //*********************************************           
            string sid = dbobj.checknumber(Request["sid"]);
            string realsid = dbobj.checknumber(Request["realsid"]);
            if (!string.IsNullOrEmpty(realsid))
            {
                System.Data.SqlClient.SqlConnection menuconn = dbobj.get_conn("Aitag_DBContext");
                string tmpmid = dbobj.get_MenuMtid(menuconn, realsid);
                menuconn.Close();
                menuconn.Dispose();
                if (!string.IsNullOrEmpty(tmpmid))
                {
                    Session["mtid"] = tmpmid;
                    Session["sid"] = sid;
                    Session["realsid"] = realsid;
                }
            }
            //*********************************************

            return View();
        }

         [ValidateInput(false)]
        public ActionResult maincontentadd(maincontent col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            string functype = "";
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "mcid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qkeyword = "", qsmdate = "", qemdate = "", qmclassid = "", qmcparentid= "";
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
            if (!string.IsNullOrWhiteSpace(Request["qmcparentid"]))
            {
                qmcparentid = Request["qmcparentid"].Trim();
                ViewBag.qmcparentid = qmcparentid;
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
                    functype = dbobj.get_dbvalue(conn, "select functype from sublevel1 where sid = '" + Request["realsid"].ToString() + "'");
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
                            if (functype == "0" || functype == "1" || functype == "2" )
                            { 
                                //3.是網路連結去掉
                            mobj.mccontent = col.mccontent.Trim();
                            }
	                        
	                        if(Request["qmcparentid"].ToString().Trim() != "")
                            {
                                mobj.mcparentid = int.Parse(Request["qmcparentid"].ToString());
	                        }
                            else
                            {
                                mobj.mcparentid = 0;
                            }

                            mobj.mcfiletype = Request["mcfiletype"].ToString().Trim();
                            mobj.mclassid = int.Parse(Request["mclassid"].ToString().Trim());
                            //        mobj.mcplace = Request["mcplace"].ToString().Trim();
	                        mobj.sid = int.Parse(Request["realsid"].ToString().Trim());
	                        mobj.mdate = DateTime.Now;
	                        mobj.mclick = 0;
                        //%>
                        //<!--#include file=addprivtbcount.asp-->
                        //<%	
	                        mobj.ownman = Session["empid"].ToString();
                            mobj.dptid = Session["dptid"].ToString();
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
                    if (functype == "4" || functype == "7")
                    {
                        tmpform += "<form name='qfr1' action='/oa/filesystem' method='post'>";
                    }
                    else
                    {
                        tmpform += "<form name='qfr1' action='/oa/maincontent' method='post'>";
                    }
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qkeyword' name='qkeyword' value='" + qkeyword + "'>";
                    tmpform += "<input type=hidden id='qsmdate' name='qsmdate' value='" + qsmdate + "'>";
                    tmpform += "<input type=hidden id='qemdate' name='qemdate' value='" + qemdate + "'>";
                    tmpform += "<input type=hidden id='qmclassid' name='qmclassid' value='" + qmclassid + "'>";
                    tmpform += "<input type=hidden id='qmcparentid' name='qmcparentid' value='" + Request["qmcparentid"] + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";

                    string alertstr = @"<script>alert('新增成功!!');</script>";
                    //tmpform = "<body onload=qfr1.submit();>";
                    //tmpform += "<form name='qfr1' action='/oa/maincontent' method='post'></form>";
                    //tmpform += "</body>";
                    return new ContentResult() { Content = alertstr + tmpform };
                    
                   // return RedirectToAction("List");

                }
            }

           
        }

    
        [ValidateInput(false)]
        public ActionResult maincontentmod(maincontent chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            string functype = "";
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "mcid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qkeyword = "", qsmdate = "", qemdate = "", qmclassid = "", qmcparentid= "";
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
            if (!string.IsNullOrWhiteSpace(Request["qmcparentid"]))
            {
                qmcparentid = Request["qmcparentid"].Trim();
                ViewBag.qmcparentid = qmcparentid;
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

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                     
                        NDcommon dbobj = new NDcommon();
                        SqlConnection conn = dbobj.get_conn("Aitag_DBContext");
                        string sid = dbobj.get_dbvalue(conn, "select sid from maincontent where mcid = '" + Request["mcid1"].ToString() + "'");
                        functype = dbobj.get_dbvalue(conn, "select functype from sublevel1 where sid = '" + sid + "'");
                        conn.Close();
                        conn.Dispose();

                        chks.sid = Convert.ToInt32(Request["tmpsid"]);
                        chks.mcid = Convert.ToInt32(Request["mcid1"]);
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
                         if (functype == "4" || functype == "7")
                         {
                             tmpform += "<form name='qfr1' action='/oa/filesystem' method='post'>";
                         }
                         else
                         {
                             tmpform += "<form name='qfr1' action='/oa/maincontent' method='post'>";
                         }
                         
                         tmpform += "<input type=hidden id='qmcparentid' name='qmcparentid' value='" + Request["qmcparentid"] + "'>";
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


                         string alertstr = @"<script>alert('修改成功!!');</script>";
                         //tmpform = "<body onload=qfr1.submit();>";
                         //tmpform += "<form name='qfr1' action='/oa/maincontent' method='post'></form>";
                         //tmpform += "</body>";
                         return new ContentResult() { Content = alertstr + tmpform };
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
            { orderdata = "mdate"; }

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
            
            if (!string.IsNullOrWhiteSpace(Request["qmclassid"]))
            {
                qmclassid = Request["qmclassid"].Trim();
                ViewBag.qmclassid = qmclassid;
            }


            NDcommon dbobj = new NDcommon();


            string DateEx = "", DateEx1 = "";
            if (!string.IsNullOrEmpty(Request["qsmdate"]))
            {
                dbobj.get_dateRang(Request["qsmdate"], "m", "min", @"發布日期起格式錯誤!!\n", out qsmdate, out DateEx);
                ViewBag.qsmdate = qsmdate;
            }
            if (!string.IsNullOrEmpty(Request["qemdate"]))
            {
                dbobj.get_dateRang(Request["qemdate"], "m", "max", @"發布日期迄格式錯誤!!\n", out qemdate, out DateEx1);
                ViewBag.qemdate = qemdate;
            }
            DateEx += DateEx1;
            if (DateEx != "")
            {
                ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>";
            }
            
            System.Data.SqlClient.SqlConnection comconn = dbobj.get_conn("Aitag_DBContext");

            string[] filearr = { "2", "2", "2", "2" };
            string sqlstr = "select top 1 * from subreadwrite where sid = " + Session["realsid"] + " and (dptgroup like '%" + Session["dptid"] + "%' or empgroup like '%" + Session["empid"] + "%') order by subread desc , subadd desc , submod desc , subdel desc";
            System.Data.SqlClient.SqlDataReader dr = dbobj.dbselect(comconn, sqlstr);
            if (dr.Read())
            {
                filearr[0] = dr["subread"].ToString();
                filearr[1] = dr["subadd"].ToString();
                filearr[2] = dr["submod"].ToString();
                filearr[3] = dr["subdel"].ToString();
            }
            dr.Close();
            dr.Dispose();
            comconn.Close();
            comconn.Dispose();

            IPagedList<maincontent> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                sqlstr = "select * from maincontent where sid = '" + Session["realsid"].ToString() + "' and dptid = '"+Session["dptid"]+"'  and ";
                if (qkeyword != "")
                    sqlstr += " (mctitle like '%" + qkeyword + "%' or  mccontent like '%" + qkeyword + "%')  and";
                if (qsmdate != "")
                    sqlstr += " mdate >= '" + qsmdate + "'  and";
                if (qemdate != "")
                    sqlstr += " mdate <= '" + qemdate + "'  and";
                if (qmclassid != "")
                    sqlstr += " mclassid = '" + qmclassid + "'  and";
                if (filearr[0] == "1")
                {
                    sqlstr += " ownman = '" + Session["empid"] + "'   and";
                }
                else if (filearr[0] == "0")
                {
                    sqlstr += " 1 <> 1   and";
                }

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.maincontent.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<maincontent>(page.Value - 1, (int)Session["pagesize"]);
            
            }
            return View(result);
        }

         public ActionResult filesystem()
        {
            NDcommon dbobj = new NDcommon();
            //變更選單SESSION
            //*********************************************           
            string sid = dbobj.checknumber(Request["sid"]);
            string realsid = dbobj.checknumber(Request["realsid"]);
            if (!string.IsNullOrEmpty(realsid))
            {
                System.Data.SqlClient.SqlConnection menuconn = dbobj.get_conn("Aitag_DBContext");
                string tmpmid = dbobj.get_MenuMtid(menuconn, realsid);
                menuconn.Close();
                menuconn.Dispose();
                if (!string.IsNullOrEmpty(tmpmid))
                {
                    Session["mtid"] = tmpmid;
                    Session["sid"] = sid;
                    Session["realsid"] = realsid;
                }
            }
            //*********************************************

            return View();
        }

         //[ValidateInput(false)]
         //public ActionResult rssread(string sysflag, int? page, string orderdata, string orderdata1)
         //{
         //    ModelState.Clear();
         //    string functype = "";
         //    page = ((!page.HasValue || page < 1) ? 1 : page);
         //    ViewBag.page = page;
         //    if (string.IsNullOrWhiteSpace(orderdata))
         //    { orderdata = "mcid"; }

         //    if (string.IsNullOrWhiteSpace(orderdata1))
         //    { orderdata1 = "desc"; }
         //    ViewBag.orderdata = orderdata;
         //    ViewBag.orderdata1 = orderdata1;
         //    string qkeyword = "", qsmdate = "", qemdate = "", qmclassid = "", qmcparentid = "";
         //    if (!string.IsNullOrWhiteSpace(Request["qkeyword"]))
         //    {
         //        qkeyword = Request["qkeyword"].Trim();
         //        ViewBag.qkeyword = qkeyword;
         //    }
         //    if (!string.IsNullOrWhiteSpace(Request["qsmdate"]))
         //    {

         //        qsmdate = Request["qsmdate"].Trim();
         //        ViewBag.qsmdate = qsmdate;
         //    }
         //    if (!string.IsNullOrWhiteSpace(Request["qemdate"]))
         //    {

         //        qemdate = Request["qemdate"].Trim();
         //        ViewBag.qemdate = qemdate;
         //    }
         //    if (!string.IsNullOrWhiteSpace(Request["qmclassid"]))
         //    {
         //        qmclassid = Request["qmclassid"].Trim();
         //        ViewBag.qmclassid = qmclassid;
         //    }
         //    if (!string.IsNullOrWhiteSpace(Request["qmcparentid"]))
         //    {
         //        qmcparentid = Request["qmcparentid"].Trim();
         //        ViewBag.qmcparentid = qmcparentid;
         //    }

         //            NDcommon dbobj = new NDcommon();
         //            SqlConnection comconn = dbobj.get_conn("Aitag_DBContext");
         //            SqlConnection comconn1 = dbobj.get_conn("Aitag_DBContext");
         //            string strsql = "select * from rsslink where empid = '" + Session["empid"] + "'";
         //            SqlDataReader dr = dbobj.dbselect(comconn,strsql);
         //            SqlDataReader dr1;
         //           //過濾的條件
         //            string condtext = "";
         //           // maincontent mobj;
         //            using (Aitag_DBContext con = new Aitag_DBContext())
         //            {
                         
         //                while(dr.Read()){
         //                 condtext = dr["condtext"].ToString();
         //                 string[] condarr={""} ;
         //                 if(condtext!="")
         //                 {
         //                  condarr = condtext.Split(',')  ;
         //                 }
                         
                          
         //                 IEnumerable<MatsuRSSread> rssobj = Aitag.RSSAction.RssReader.GetRssFeed(dr["rssurl"].ToString());
         //                 foreach (var item in rssobj) 
         //                 {
         //                       if (item != null) { 
         //                            strsql = "select * from maincontent where ownman = '" + Session["empid"] + "' and mctitle like '" + item.title + "%'";
         //                            dr1 = dbobj.dbselect(comconn1,strsql);
         //                            if(!dr1.HasRows){
         //                                if (condtext=="")
         //                                {
         //                                    maincontent mobj = new maincontent();
         //                                    mobj.mctype = "0";
         //                                    mobj.mctitle = item.title;
         //                                    mobj.mchttp = item.link;

         //                                    mobj.mccontent = item.Description;

         //                                    mobj.mcparentid = 0;

         //                                    // mobj.mcfiletype = "0";
         //                                    mobj.mclassid = 0;

         //                                    mobj.sid = int.Parse(Request["realsid"].ToString().Trim());
         //                                    mobj.mdate = DateTime.Parse(item.pubDate.ToString());
         //                                    mobj.mclick = 0;

         //                                    mobj.ownman = Session["empid"].ToString();
         //                                    mobj.comid = Session["comid"].ToString();
         //                                    mobj.bmodid = Session["empid"].ToString();
         //                                    mobj.bmoddate = DateTime.Now;

         //                                    con.maincontent.Add(mobj);
         //                                    con.SaveChanges();
         //                                }
         //                                else
         //                                {
         //                                    for(int k = 0 ; k < condarr.Length ; k ++)
         //                                    {
         //                                        if (item.title.IndexOf(condarr[k].Trim()) >= 0 || item.Description.IndexOf(condarr[k].Trim()) >= 0)
         //                                        { 
         //                                            maincontent mobj = new maincontent();
         //                                            mobj.mctype = "0";
         //                                            mobj.mctitle = item.title;
         //                                            mobj.mchttp = item.link ;
                         
         //                                            mobj.mccontent = item.Description;
    
         //                                            mobj.mcparentid = 0;
                               
         //                                           // mobj.mcfiletype = "0";
         //                                            mobj.mclassid = 0 ;
                                     
         //                                            mobj.sid = int.Parse(Request["realsid"].ToString().Trim());
         //                                            mobj.mdate = DateTime.Parse(item.pubDate.ToString());
         //                                            mobj.mclick = 0;
                        
         //                                            mobj.ownman = Session["empid"].ToString();
         //                                            mobj.comid = Session["comid"].ToString();
         //                                            mobj.bmodid = Session["empid"].ToString();
         //                                            mobj.bmoddate = DateTime.Now;
                               
         //                                            con.maincontent.Add(mobj);
         //                                            con.SaveChanges();
         //                                            //過濾新增後 , 跳出
         //                                            break;
         //                                        }
         //                                    }
         //                                }
         //                            }
         //                            dr1.Close();
         //                            dr1.Dispose();
         //                       }
         //                   }
         //                 rssobj = null; 
         //               }
         //            }
         //            dr.Close();
         //            dr.Dispose();
         //            comconn.Close();
         //            comconn.Dispose();
         //            comconn1.Close();
         //            comconn1.Dispose();

         //                //系統LOG檔 //================================================= //
         //                SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
         //                string sysrealsid = Request["sysrealsid"].ToString();
         //                string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
         //                string sysnote = "類別:";
         //                dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
         //                sysconn.Close();
         //                sysconn.Dispose();
         //                //=================================================
         //            string tmpform = "";
         //            tmpform = "<body onload=qfr1.submit();>";
         //            tmpform += "<form name='qfr1' action='/oa/maincontent' method='post'>";
         //            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
         //            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
         //            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
         //            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
         //            tmpform += "<input type=hidden id='qkeyword' name='qkeyword' value='" + qkeyword + "'>";
         //            tmpform += "<input type=hidden id='qsmdate' name='qsmdate' value='" + qsmdate + "'>";
         //            tmpform += "<input type=hidden id='qemdate' name='qemdate' value='" + qemdate + "'>";
         //            tmpform += "<input type=hidden id='qmclassid' name='qmclassid' value='" + qmclassid + "'>";
         //            tmpform += "<input type=hidden id='qmcparentid' name='qmcparentid' value='" + Request["qmcparentid"] + "'>";
         //            tmpform += "</form>";
         //            tmpform += "</body>";

         //            string alertstr = @"<script>alert('RSS資料讀取成功!!');</script>";
                    
         //            return new ContentResult() { Content = alertstr + tmpform };

         
         //}

        /*public ActionResult filesystem(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "mcid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qkeyword = "", qsmdate = "", qemdate = "", qmclassid = "", qmcparentid = "";
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
            if (!string.IsNullOrWhiteSpace(Request["qmcparentid"]))
            {
                qmcparentid = Request["qmcparentid"].Trim();
                ViewBag.qmcparentid = qmcparentid;
            }

            IPagedList<viewmaincontent> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from viewmaincontent where sid = '" + Session["realsid"].ToString() + "'   and";
                if (qkeyword != "")
                    sqlstr += " (mctitle like '%" + qkeyword + "%' or  mccontent like '%" + qkeyword + "%')  and";
                if (qsmdate != "")
                    sqlstr += " mdate >= '" + qsmdate + "'  and";
                if (qemdate != "")
                    sqlstr += " mdate <= '" + qemdate + "'  and";
                if (qmclassid != "")
                    sqlstr += " mclassid = '" + qmclassid + "'  and";
                if (qmcparentid != "")
                    sqlstr += " mcparentid = '" + qmcparentid + "'  and";
                else
                    sqlstr += " mcparentid = 0   and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.viewmaincontent.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<viewmaincontent>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }*/

     


        [ActionName("maincontentdel")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qkeyword = "", qsmdate = "", qemdate = "", qmclassid = "", qmcparentid="";
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

            if (!string.IsNullOrWhiteSpace(Request["qmcparentid"]))
            {
                qmcparentid = Request["qmcparentid"].Trim();
                ViewBag.qmcparentid = qmcparentid;
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
                        string tgourl = "";
                        if(Request["functype"]=="4"||Request["functype"]=="7")
                        {
                            tgourl = "/oa/filesystem?page=" + page + "&qkeyword=" + qkeyword + "&qsmdate=" + qsmdate + "&qemdate=" + qemdate + "&qmclassid=" + qmclassid + "&qmcparentid=" + qmcparentid;
                        }
                        else
                        {
                            tgourl = "/oa/maincontent?page=" + page + "&qkeyword=" + qkeyword + "&qsmdate=" + qsmdate + "&qemdate=" + qemdate + "&qmclassid=" + qmclassid;
                        }
                       
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


        public ActionResult PublishNews(string id, int? page)
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

                string tgourl = "/oa/maincontent?page=" + page + "&qkeyword=" + qkeyword + "&qsmdate=" + qsmdate + "&qemdate=" + qemdate + "&qmclassid=" + qmclassid;
                return new ContentResult() { Content = @"<script>alert('請勾選要發佈到網站上的資料!!');window.history.go(-1);</script>" };
            }
            else
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {

                    NDcommon dbobj = new NDcommon();

                    SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext");
                    SqlConnection webconn = dbobj.get_conn("MatsuWeb_DBContext");
                    SqlDataReader dr;

                    string sysnote = "發佈標題名稱:", condtionstr = "", insertsql = "", updatesql = "", mcstartdate = "NULL";
                    string[] condtionArr = cdel.Split(',');
                    foreach (string item in condtionArr) {
                        condtionstr += "'" + item + "',";
                    }
                    if (!string.IsNullOrEmpty(condtionstr)) { condtionstr = condtionstr.Substring(0, condtionstr.Length - 1); }

                    //判斷發布到的目的地是否正確的環境
                    string cid = dbobj.get_dbvalue(webconn, "select cid from category WHERE ctitle = '新聞櫥窗' and dptid = '" + Session["dptid"].ToString() + "'");
                    if (cid == "")
                    {
                        return new ContentResult() { Content = @"<script>alert('請先新增名稱為(新聞櫥窗)的目錄!!');history.go(-1)</script>" };
                    }
                    else {
                        string cfunctype = dbobj.get_dbvalue(webconn, "select cfunctype from category WHERE cid = '" + cid + "'");
                        if (cfunctype != "01") {
                            return new ContentResult() { Content = @"<script>alert('新聞櫥窗目錄的選單功能必須是(最新消息)，連結位置必須是(news)!!');history.go(-1)</script>" };
                        }
                    }
                    
                    dr = dbobj.dbselect(conn1, "select * from maincontent where mcid in (" + condtionstr + ")");
                    
                    if(dr.HasRows){
                        while(dr.Read()){
                            try
                            {
                                mcstartdate = "'" + Convert.ToDateTime(dr["mdate"].ToString()).ToString("yyyy/MM/dd HH:mm:ss") + "'";
                            }
                            catch { }

                            sysnote += dr["mctitle"] + "(" + dr["mcid"] + ")<br>"; //systemlog
                            
                            //發佈至webmaincontent
                            insertsql = "insert into webmaincontent ";
                            insertsql += " (cid, sid, mctitle, mchttp, mccontent, mdate, mcstartdate, mcorder, mcstatus,ifforever,ifpass, bmodid, bmoddate, Mclick, dptid) ";
                            insertsql += " values ( ";
                            insertsql += cid + ", "; //對應該部門的新聞櫥窗
                            insertsql += "'" + dr["sid"] + "', "; //webmaincontent.sid=maincontent.sid
                            insertsql += "'" + dr["mctitle"] + "', ";
                            insertsql += "'" + dr["mchttp"] + "', ";
                            string mccontent = dr["mccontent"].ToString();
                            mccontent = mccontent.Replace("'", "''");
                            insertsql += "'" + mccontent + "', ";
                            insertsql += "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', "; //發佈時間
                            insertsql += "" + mcstartdate + ", "; //日報提供的日期
                            insertsql += " 5, ";
                            insertsql += " 'y', ";
                            insertsql += " '1', ";
                            insertsql += " '1', ";
                            insertsql += "'" + Session["empid"].ToString() + "', "; //發佈者
                            insertsql += "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', "; //修改時間
                            insertsql += " 0, ";
                            insertsql += "'" + Session["dptid"].ToString()+"' "; //發佈部門
                            insertsql += " ) ";

                            dbobj.dbexecute("MatsuWeb_DBContext", insertsql);

                            //update maincontent 的 發佈日期
                            updatesql = "update maincontent set msdate = '" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' where mcid = " + dr["mcid"];
                            dbobj.dbexecute("Aitag_DBContext", updatesql);
                        }
                    }

                    dr.Close();
                    dr.Dispose();
                    webconn.Close();
                    webconn.Dispose();
                    conn1.Close();
                    conn1.Dispose();
                    string sysrealsid = Request["sysrealsid"].ToString();
                    //系統LOG檔
                    //================================================= //                  
                    SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    string sysflag = "A";
                    dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    sysconn.Close();
                    sysconn.Dispose();
                    //======================================================  
                    string tgourl = "";
                    if (Request["functype"] == "4" || Request["functype"] == "7")
                    {
                        tgourl = "/oa/filesystem?page=" + page + "&qkeyword=" + qkeyword + "&qsmdate=" + qsmdate + "&qemdate=" + qemdate + "&qmclassid=" + qmclassid;
                    }
                    else
                    {
                        tgourl = "/oa/maincontent?page=" + page + "&qkeyword=" + qkeyword + "&qsmdate=" + qsmdate + "&qemdate=" + qemdate + "&qmclassid=" + qmclassid;
                    }

                    return new ContentResult() { Content = @"<script>alert('發佈成功!!');location.href='" + tgourl + "'</script>" };

                    //return RedirectToAction("List");
                }
            }
        }


        public ActionResult searchList(int? page, string orderdata, string orderdata1)
        {

            Session["mtid"] = "A001";
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "mcid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qkeyword = "";
            if (!string.IsNullOrWhiteSpace(Request["qkeyword"]))
            {
                qkeyword = Request["qkeyword"].Trim();
                ViewBag.qkeyword = qkeyword;
            }

            NDcommon dbobj = new NDcommon();


            
            IPagedList<maincontent> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from maincontent where comid = '" + Session["comid"].ToString() + "'   and";
                if (qkeyword != "")
                { sqlstr += " (mctitle like '%" + qkeyword + "%' or  mccontent like '%" + qkeyword + "%')  and"; }
                else
                { sqlstr += " 1<>1  and"; }
                

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.maincontent.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<maincontent>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }


        public ActionResult maincopyrar()
        {
            ArrayList arrympath = new ArrayList();

            string zipdownpath = ConfigurationManager.AppSettings["zipdownpath"].ToString();//'抓到webconfig設定的值     
            NDcommon dbobj = new NDcommon();   
            string  realsid = dbobj.checknumber(Request["realsid"].Trim());
            string  qmcparentid = dbobj.checknumber(Request["qmcparentid"].Trim());
            string empiddir = ""; //目錄名
            DateTime myDate = DateTime.Now;
            empiddir = myDate.ToString("yyyyMMddHHmmss");         
            string newpath = zipdownpath + "\\alldir\\" + empiddir;

            if (!Directory.Exists(newpath))
            {
                Directory.CreateDirectory(newpath);
            }        

            SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext");

            string[] filearr = { "2", "2", "2", "2" };
            string sqlstr = "select top 1 * from subreadwrite where sid = " + realsid + " and (dptgroup like '%" + Session["dptid"] + "%' or empgroup like '%" + Session["empid"] + "%') order by subread desc , subadd desc , submod desc , subdel desc";
            SqlDataReader dr = dbobj.dbselect(conn1, sqlstr);
            if (dr.Read())
            {
                filearr[0] = dr["subread"].ToString();
                filearr[1] = dr["subadd"].ToString();
                filearr[2] = dr["submod"].ToString();
                filearr[3] = dr["subdel"].ToString();
            }
            dr.Close();
            dr.Dispose();

            string sourceFileName = "";
            string destFileName = "";
            string listsql = "";
            listsql = "select * FROM viewmaincontent where sid = '" + realsid + "'";

            if (string.IsNullOrWhiteSpace(qmcparentid) || qmcparentid=="0")
            { listsql += " and mcparentid=0"; }
            else
            { listsql += " and mcparentid=" + qmcparentid; }

            if (filearr[0] == "1")
            {
                listsql += "and ownman = '" + Session["empid"] + "'";
            }
            else if (filearr[0] == "0")
            {
                listsql += "and 1 <> 1 ";
            }

             listsql +=" order by mcfiletype desc";
             dr = dbobj.dbselect(conn1, listsql);
            while (dr.Read())
            {
                if(dr["mcfiletype"].ToString()=="0")
                {
                    //文件夾時                 
                    arrympath.Add(newpath);
                    arrympath.Add("");
                    arrympath.Add("");
                    arrympath.Add("");
                    arrympath.Add("");
                    arrympath.Add("");
                    arrympath.Add("");
                    arrympath.Add("");
                    arrympath.Add("");
                    arrympath.Add("");
                    arrympath.Add("");
                    arrympath.Add("");
                    arrympath.Add("");
                    arrympath.Add("");
                    arrympath.Add("");
                    dbobj.copyall(arrympath, dr["mcid"].ToString(), 1);
                }
                else
                {
                   
                //把檔案COPY來
                    sourceFileName = zipdownpath + "\\upload\\" + dr["cupfile"].ToString();
                    if (System.IO.File.Exists(sourceFileName))
                    {
                        destFileName = newpath + "\\" + dr["cfilename"].ToString();
                        System.IO.File.Copy(sourceFileName, destFileName);
                    }
                }      

            }
          
          
            dr.Close();
            dr.Dispose();
            conn1.Close();
            conn1.Dispose();

          //  try
           // {
                //要執行的exe
            System.Diagnostics.ProcessStartInfo pInfo = new System.Diagnostics.ProcessStartInfo(zipdownpath + @"\alldir\Rar.exe");
                //參數

                
                pInfo.Arguments =" a " + newpath  + ".rar" + " " + newpath;
                using (System.Diagnostics.Process p = new System.Diagnostics.Process())
                {
                    //執行
                    
                    p.StartInfo = pInfo;
                    p.Start();
          
                    string sourceFileName1 = zipdownpath + "\\alldir\\" + empiddir + ".rar";
                    p.WaitForExit(3000);
                    while (true)
                    {
                           if(p.HasExited)
                             break ;
                    }
                }
                 ViewBag.tmppath = "/alldir/" + empiddir + ".rar";
                 return View();
            
        }

    }
}
