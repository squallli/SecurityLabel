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
    public class scheduleController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /billflow/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }
           
        public ActionResult List()
        {

            string Mflag = Request["Mflag"];
            if (string.IsNullOrWhiteSpace(Mflag))          
            { Mflag = "M"; }

            ViewBag.Mflag = Mflag; 

            if (!string.IsNullOrWhiteSpace(Request["year1"]))
            { ViewBag.year1 = Request["year1"].ToString();}

             if (!string.IsNullOrWhiteSpace(Request["month1"]))
            { ViewBag.month1 = Request["month1"].ToString(); }

             if (!string.IsNullOrWhiteSpace(Request["qschtitle"]))
            { ViewBag.qschtitle = Request["qschtitle"].ToString(); }

            //20161125  增加查詢 公司>部門>員工 連動下拉
            //不可以使用string.IsNullOrWhiteSpace , 第一次預設session 20161215 Mark
            if (Request["qcomid"]!=null)
            { ViewBag.qcomid = Request["qcomid"].ToString(); }
            if (Request["qdptid"]!=null)
            { ViewBag.qdptid = Request["qdptid"].ToString(); }
            if (Request["qschowner"]!=null)
            { ViewBag.qschowner = Request["qschowner"].ToString(); }

     
            if (!string.IsNullOrWhiteSpace(Request["year1"]) && (Mflag == "M"))
            {

                ViewBag.yy = ViewBag.year1;
                ViewBag.mm = ViewBag.month1;
                ViewBag.dd = 1;

            }
            else
            {

                if (Mflag == "W")
                {
                    if (!string.IsNullOrWhiteSpace(Request["qschdate"]))
                    {
                        ViewBag.mdate = Request["qschdate"];
                    }
                    else
                    {
                        ViewBag.mdate = System.Convert.ToDateTime(DateTime.Now).ToString("yyyy/MM/dd");

                    }
                }
                else
                { 
                    string yy = "", mm = "", dd = "";
                    yy = DateTime.Now.Year.ToString();
                    mm = DateTime.Now.Month.ToString();
                    dd = DateTime.Now.Day.ToString();

                    ViewBag.yy = yy;
                    ViewBag.mm = mm;
                    ViewBag.dd = dd;
                }

            }
   


            return View();
        }


        public ActionResult add(schedule col, string sysflag, HttpPostedFileBase sfile)
        {
            ModelState.Clear();

            if (!string.IsNullOrWhiteSpace(Request["year1"]))
            { ViewBag.year1 = Request["year1"].ToString(); }
            else
            { ViewBag.year1 = ""; }

            if (!string.IsNullOrWhiteSpace(Request["month1"]))
            { ViewBag.month1 = Request["month1"].ToString(); }
            else
            { ViewBag.month1 = ""; }


            if (!string.IsNullOrWhiteSpace(Request["qschdate"]))
            { ViewBag.qschdate = Request["qschdate"].ToString(); }
            else
            { ViewBag.qschdate = ""; }

            

              string schtype = Request["schtype"].ToString();
              ViewBag.schtype = schtype;
              ViewBag.Mflag = Request["Mflag"].ToString(); ;
            
            if (Request["tmpadate"] != "" && Request["tmpadate"] != null)
            { ViewBag.tmpadate = Request["tmpadate"].ToString(); }
            
            if (sysflag != "A")
            {
                schedule newcol = new schedule();
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


                        string fileName = "";
                        #region 上傳
                        if (sfile != null)
                        {
                            String sernonum = "";
                            //重新命名，存入檔案
                            DateTime myDate = DateTime.Now;
                            sernonum = myDate.ToString("yyyyMMddHHmmss");

                            fileName = sfile.FileName.Substring(sfile.FileName.IndexOf("."), sfile.FileName.Length - sfile.FileName.IndexOf("."));

                            if (fileName != ".exe" && fileName != ".asp" && fileName != ".aspx" && fileName != ".jsp" && fileName != ".php")
                            {
                                fileName = "F-" + sernonum.ToString() + fileName;
                                sfile.SaveAs(Server.MapPath("~/upload/") + fileName);                            
                            }
                            //else
                            //{
                            //    ModelState.AddModelError("Ebpic", "上傳圖片格式錯誤");
                            //    return View(col);
                            //}
                        }
                        #endregion 

                      #region    
                            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
                    
                            schedule mobj;
                            using (Aitag_DBContext con = new Aitag_DBContext())
                            {                          
                               mobj = new schedule();
                               mobj.schtype = schtype;

                               string schtitle = Request["schtitle"].ToString().Trim();
                                schtitle = schtitle.Replace("'","’");


                                mobj.schtitle = schtitle;
                                mobj.schcontent = Request["schcontent"].ToString().Trim();
                                mobj.schplace = Request["schplace"].ToString().Trim();
                                mobj.schowner = Session["empid"].ToString();
                                mobj.schloginer = Session["empid"].ToString();          
                                mobj.schdate =DateTime.Parse(Request["schdate"].ToString());
                                mobj.schhour = Request["schhour"].ToString().Trim();
                                mobj.schmin = Request["schmin"].ToString().Trim();
                                mobj.schehour = Request["schehour"].ToString().Trim();
                                mobj.schemin = Request["schemin"].ToString().Trim();
                                mobj.comid = Session["comid"].ToString();
                                mobj.bmodid = Session["empid"].ToString();
                                mobj.bmoddate = DateTime.Now;
                                mobj.sfile = fileName;
                         
                                con.schedule.Add(mobj);
                                con.SaveChanges();
                         
                        #endregion
                                string tmptitle = "";
                                switch (schtype)
                                {
                                    case "0":
                                        tmptitle = "個人備忘";
                                        break;
                                    case "1":
                                        tmptitle = "個人行程";
                                        break;
                                    case "2":
                                        tmptitle = "公司行程";
                                        break;

                                }

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "分類：" + tmptitle + "<br>標題：" + schtitle + "<br>日期：" + Request["schdate"].ToString();
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/schedule/list' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='year1' id='year1' value='" + ViewBag.year1 + "'>";
                    tmpform += "<input type=hidden name='month1' id='month1' value='" + ViewBag.month1 + "'>";
                    tmpform += "<input type=hidden name='Mflag' id='Mflag' value='" + ViewBag.Mflag + "'>";
                    tmpform += "<input type=hidden name='qschdate' id='qschdate' value='" + ViewBag.qschdate + "'>";  
                   
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"<script>alert('新增成功!!');</script>" + tmpform };   
                 

                }
            }


        }


        [ValidateInput(false)]
        public ActionResult Edit(schedule chks, string sysflag, HttpPostedFileBase sfile1)
        {

            if (!string.IsNullOrWhiteSpace(Request["year1"]))
            { ViewBag.year1 = Request["year1"].ToString(); }
            else
            { ViewBag.year1 = ""; }

            if (!string.IsNullOrWhiteSpace(Request["month1"]))
            { ViewBag.month1 = Request["month1"].ToString(); }
            else
            { ViewBag.month1 = ""; }

            if (!string.IsNullOrWhiteSpace(Request["qschdate"]))
            { ViewBag.qschdate = Request["qschdate"].ToString(); }
            else
            { ViewBag.qschdate = ""; }

            ViewBag.schtype = Request["schtype"].ToString();
            ViewBag.Mflag = Request["Mflag"].ToString();

            int schid = int.Parse(Request["schid"].ToString());
            
            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.schedule.Where(r => r.schid == chks.schid).FirstOrDefault();
                    schedule schedule = con.schedule.Find(chks.schid);
                    if (schedule == null)
                    {
                        return HttpNotFound();
                    }
                    return View(schedule);
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

                        schedule modobj = con.schedule.Where(r => r.schid == schid).FirstOrDefault();
                                                
                        string schtitle = Request["schtitle"].ToString().Trim();
                        schtitle = schtitle.Replace("'", "’");

                        modobj.schtitle = schtitle;
                        modobj.schcontent = Request["schcontent"].ToString().Trim();
                        modobj.schplace = Request["schplace"].ToString().Trim();                       
                        modobj.schdate = DateTime.Parse(Request["schdate"].ToString());
                        modobj.schhour = Request["schhour"].ToString().Trim();
                        modobj.schmin = Request["schmin"].ToString().Trim();
                        modobj.schehour = Request["schehour"].ToString().Trim();
                        modobj.schemin = Request["schemin"].ToString().Trim();

                     
                        #region 上傳檔案

                        if (sfile1 != null)
                        {
                            string sernonum;
                            //重新命名，存入檔案
                            DateTime myDate = DateTime.Now;
                            sernonum = myDate.ToString("yyyyMMddHHmmss");

                            string fileName = sfile1.FileName.Substring(sfile1.FileName.IndexOf("."), sfile1.FileName.Length - sfile1.FileName.IndexOf("."));

                            if (fileName != ".exe" && fileName != ".asp" && fileName != ".aspx" && fileName != ".jsp" && fileName != ".php")
                            {
                                fileName = "F-" + sernonum.ToString() + fileName;

                                sfile1.SaveAs(Server.MapPath("~/Upload/") + fileName);
                                modobj.sfile = fileName;
                            }

                        }                       
                        #endregion
                                      

                        con.Entry(modobj).State = EntityState.Modified;
                        con.SaveChanges();
                        con.Dispose();

                        Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "分類：" + Request["tmptitle"].ToString() + "<br>標題：" + schtitle + "<br>日期：" + Request["schdate"].ToString();
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/schedule/List' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='year1' id='year1' value='" + ViewBag.year1 + "'>";
                        tmpform += "<input type=hidden name='month1' id='month1' value='" + ViewBag.month1 + "'>";
                        tmpform += "<input type=hidden name='Mflag' id='Mflag' value='" + ViewBag.Mflag + "'>";
                        tmpform += "<input type=hidden name='qschdate' id='qschdate' value='" + ViewBag.qschdate + "'>";  
                        tmpform += "</form>";
                        tmpform += "</body>";


                        return new ContentResult() { Content = @"<script>alert('修改成功!!');</script>" + tmpform };            
                        
                    }
                }
            }

        }

        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string schid)
        {
            if (!string.IsNullOrWhiteSpace(Request["year1"]))
            { ViewBag.year1 = Request["year1"].ToString(); }
            else
            { ViewBag.year1 = ""; }

            if (!string.IsNullOrWhiteSpace(Request["month1"]))
            { ViewBag.month1 = Request["month1"].ToString(); }
            else
            { ViewBag.month1 = ""; }

            if (!string.IsNullOrWhiteSpace(Request["qschdate"]))
            { ViewBag.qschdate = Request["qschdate"].ToString(); }
            else
            { ViewBag.qschdate = ""; }

            string schtype = Request["schtype"].ToString();
            ViewBag.schtype = schtype;
            ViewBag.Mflag = Request["Mflag"].ToString();
            
            string tmptitle = "";
            switch (schtype)
            {
                case "0":
                    tmptitle = "個人備忘";
                    break;
                case "1":
                    tmptitle = "個人行程";
                    break;
                case "2":
                    tmptitle = "公司行程";
                    break;

            }

                using (Aitag_DBContext con = new Aitag_DBContext())
                {

                    NDcommon dbobj = new NDcommon();
                    SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext");
                    string sysnote = "";
                    string schtitle = dbobj.get_dbvalue(conn1, "select schtitle from schedule where schid =" + schid);                        
                    sysnote = "類別:" + tmptitle + "，標題:" + schtitle + "<br>";
                    dbobj.dbexecute("Aitag_DBContext", "DELETE FROM schedule where schid = " + schid); 
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

                    string tmpform = "";
                    tmpform = "<body onload='qfr1.submit();'>";
                    tmpform += "<form name='qfr1' action='/schedule/list' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='year1' id='year1' value='" + ViewBag.year1 + "'>";
                    tmpform += "<input type=hidden name='month1' id='month1' value='" + ViewBag.month1 + "'>";
                    tmpform += "<input type=hidden name='Mflag' id='Mflag' value='" + ViewBag.Mflag + "'>";
                    tmpform += "<input type=hidden name='qschdate' id='qschdate' value='" + ViewBag.qschdate + "'>";  
                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');</script>" + tmpform };                

                  
                }
           
        }
        public ActionResult scheduleshow()
        {
            ViewBag.schid=Request["schid"].ToString();
            ViewBag.schtype=Request["schtype"].ToString();
            return View();
        }
     
        
 



    }
}
