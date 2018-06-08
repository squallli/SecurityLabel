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
using System.Data.Entity;

namespace Aitag.Controllers
{
     [DoAuthorizeFilter]
    public class WebmaincontentController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /Checkcode/

        public ActionResult Index()
        {
            return RedirectToAction("List",new {cid="0",cid1="0" });
        }

        [HttpGet]
        public ActionResult Add(string cid,string cid1)
        {
            Webmaincontent col = new Webmaincontent();
            if (cid1 != "0")
            {
                col.AC_Cid = int.Parse(cid1);
                col.AC_Ccid = int.Parse(cid);               
            }
            else
            {
                col.AC_Cid = int.Parse(cid);
                col.AC_Ccid = 0;              
            }

            ViewBag.cid = cid;
            ViewBag.cid1 = cid1;
            return View(col);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult add(Webmaincontent col, HttpPostedFileBase Mcfile,HttpPostedFileBase Mpic)
        {
            ViewBag.Cid = col.AC_Cid.ToString();
            ViewBag.Ccid = col.AC_Ccid.ToString();
            /*
            if (Mcfile == null)
            {
                ModelState.AddModelError("Mcfile", "請選擇檔案");
            }
            */
            
            if (!ModelState.IsValid)
            {
                return View(col);
            }

            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            //上傳檔案****開始
            int sernonum;
            if (Mcfile != null)
            {

                // Uploadfile.SaveAs(Server.MapPath("~/Uploads/") + Apic.FileName);

                //上傳檔案
                //找序號
                SqlConnection conn = dbobj.get_conn("Aitag_DBContext");
                SqlDataReader dr;
                SqlCommand sqlsmd = new SqlCommand();
                sqlsmd.Connection = conn;
                string sqlstr = "select sno from Serno where sid=1";
                sqlsmd.CommandText = sqlstr;
                dr = sqlsmd.ExecuteReader();

                if (dr.Read())
                {
                    //重新命名，存入檔案
                    sernonum = int.Parse(dr[0].ToString()) + 1;
                    string fileName = "contente" + sernonum.ToString() + Mcfile.FileName.Substring(Mcfile.FileName.IndexOf("."), Mcfile.FileName.Length - Mcfile.FileName.IndexOf("."));
                    Mcfile.SaveAs(Server.MapPath("~/Upload/") + fileName);
                    col.AC_Mcfile = fileName;

                    //序號+1後存入
                    Aitag.Models.NDcommon dbobj1 = new Aitag.Models.NDcommon();
                    SqlConnection conn1 = dbobj1.get_conn("Aitag_DBContext");
                    SqlCommand sqlsmd1 = new SqlCommand();
                    sqlsmd1.Connection = conn1;
                    string sqlstr1 = "UPDATE  Serno SET  sno ='" + sernonum + "' where sid=1";
                    sqlsmd1.CommandText = sqlstr1;
                    sqlsmd1.ExecuteReader();


                    sqlsmd1.Dispose();
                    conn1.Close();
                    conn1.Dispose();
                }


                sqlsmd.Dispose();
                conn.Close();
                conn.Dispose();
            }


            //上傳圖片****開始

            if (Mpic != null)
            {

               //上傳檔案
                //找序號
                SqlConnection conn = dbobj.get_conn("Aitag_DBContext");
                SqlDataReader dr;
                SqlCommand sqlsmd = new SqlCommand();
                sqlsmd.Connection = conn;
                string sqlstr = "select sno from Serno where sid=1";
                sqlsmd.CommandText = sqlstr;
                dr = sqlsmd.ExecuteReader();

                if (dr.Read())
                {
                    //重新命名，存入檔案
                    sernonum = int.Parse(dr[0].ToString()) + 1;
                    string fileName = "contente" + sernonum.ToString() + Mpic.FileName.Substring(Mpic.FileName.IndexOf("."), Mpic.FileName.Length - Mpic.FileName.IndexOf("."));
                    Mpic.SaveAs(Server.MapPath("~/Upload/") + fileName);
                    col.AC_Mpic = fileName;

                    //序號+1後存入
                    Aitag.Models.NDcommon dbobj1 = new Aitag.Models.NDcommon();
                    SqlConnection conn1 = dbobj1.get_conn("Aitag_DBContext");
                    SqlCommand sqlsmd1 = new SqlCommand();
                    sqlsmd1.Connection = conn1;
                    string sqlstr1 = "UPDATE  Serno SET  sno ='" + sernonum + "' where sid=1";
                    sqlsmd1.CommandText = sqlstr1;
                    sqlsmd1.ExecuteReader();


                    sqlsmd1.Dispose();
                    conn1.Close();
                    conn1.Dispose();
                }


                sqlsmd.Dispose();
                conn.Close();
                conn.Dispose();
            }

            col.AC_Baddid = Session["tempid"].ToString();
            col.AC_Badddate = DateTime.Now;
            col.AC_Bmodid = Session["tempid"].ToString();
            col.AC_Bmoddate = DateTime.Now;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                con.Webmaincontents.Add(col);
                con.SaveChanges();
            }
            //系統LOG檔 //================================================= //
            //Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            string syssubname = "網站管理作業 > 網站內容管理";
            string sysnote = "名稱:" + col.AC_Mctitle;
            string sysflag = "A";
            SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
            dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
            sysconn.Close();
            sysconn.Dispose();
            //=================================================

            if (col.AC_Ccid != 0)
            {
                return RedirectToAction("List", new { cid = col.AC_Ccid, cid1 = col.AC_Cid });
            }
            else {
                return RedirectToAction("List", new { cid = col.AC_Cid, cid1 = 0 });
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {

            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                var data = con.Webmaincontents.Where(r => r.AC_Mcid == id).FirstOrDefault();
                Webmaincontent Webmaincontents = con.Webmaincontents.Find(id);
                if (Webmaincontents == null)
                {
                    return HttpNotFound();
                }
                return View(Webmaincontents);
            }


        }



        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Webmaincontent chks, HttpPostedFileBase Mcfile1, HttpPostedFileBase Mpic1)
        {
            ViewBag.Cid = chks.AC_Cid;
            ViewBag.Ccid = chks.AC_Ccid;

            if (ModelState.IsValid)
            {
                Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
                using (Aitag_DBContext con = new Aitag_DBContext())
                {

                    chks.AC_Bmodid = Session["tempid"].ToString();
                    chks.AC_Bmoddate = DateTime.Now;
                    con.Entry(chks).State = EntityState.Modified;
                    //檔案ㄋ
                    int sernonum;
                    if (Mcfile1 != null)
                    {
                        // Uploadfile.SaveAs(Server.MapPath("~/Uploads/") + Apic.FileName);
                        //上傳檔案
                        //找序號
                        SqlConnection conn = dbobj.get_conn("Aitag_DBContext");
                        SqlDataReader dr;
                        SqlCommand sqlsmd = new SqlCommand();
                        sqlsmd.Connection = conn;
                        string sqlstr = "select sno from Serno where sid=1";
                        sqlsmd.CommandText = sqlstr;
                        dr = sqlsmd.ExecuteReader();

                        if (dr.Read())
                        {
                            //重新命名，存入檔案
                            sernonum = int.Parse(dr[0].ToString()) + 1;
                            string fileName = "contente" + sernonum.ToString() + Mcfile1.FileName.Substring(Mcfile1.FileName.IndexOf("."), Mcfile1.FileName.Length - Mcfile1.FileName.IndexOf("."));
                            Mcfile1.SaveAs(Server.MapPath("~/Upload/") + fileName);
                            chks.AC_Mcfile = fileName;

                            //序號+1後存入
                            Aitag.Models.NDcommon dbobj1 = new Aitag.Models.NDcommon();
                            SqlConnection conn1 = dbobj1.get_conn("Aitag_DBContext");
                            SqlCommand sqlsmd1 = new SqlCommand();
                            sqlsmd1.Connection = conn1;
                            string sqlstr1 = "UPDATE  Serno SET  sno ='" + sernonum + "' where sid=1";
                            sqlsmd1.CommandText = sqlstr1;
                            sqlsmd1.ExecuteReader();


                            sqlsmd1.Dispose();
                            conn1.Close();
                            conn1.Dispose();
                        }


                        sqlsmd.Dispose();
                        conn.Close();
                        conn.Dispose();

                    }
                    else
                    {
                        chks.AC_Mcfile = chks.AC_Mcfile;
                    }
                    //圖片
                    if (Mpic1 != null)
                    {
                        // Uploadfile.SaveAs(Server.MapPath("~/Uploads/") + Apic.FileName);
                        //上傳檔案
                        //找序號
                        //Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
                        SqlConnection conn = dbobj.get_conn("Aitag_DBContext");
                        SqlDataReader dr;
                        SqlCommand sqlsmd = new SqlCommand();
                        sqlsmd.Connection = conn;
                        string sqlstr = "select sno from Serno where sid=1";
                        sqlsmd.CommandText = sqlstr;
                        dr = sqlsmd.ExecuteReader();

                        if (dr.Read())
                        {
                            //重新命名，存入檔案
                            sernonum = int.Parse(dr[0].ToString()) + 1;
                            string fileName = "contente" + sernonum.ToString() + Mpic1.FileName.Substring(Mpic1.FileName.IndexOf("."), Mpic1.FileName.Length - Mpic1.FileName.IndexOf("."));
                            Mpic1.SaveAs(Server.MapPath("~/Upload/") + fileName);
                            chks.AC_Mpic = fileName;

                            //序號+1後存入
                            Aitag.Models.NDcommon dbobj1 = new Aitag.Models.NDcommon();
                            SqlConnection conn1 = dbobj1.get_conn("Aitag_DBContext");
                            SqlCommand sqlsmd1 = new SqlCommand();
                            sqlsmd1.Connection = conn1;
                            string sqlstr1 = "UPDATE  Serno SET  sno ='" + sernonum + "' where sid=1";
                            sqlsmd1.CommandText = sqlstr1;
                            sqlsmd1.ExecuteReader();


                            sqlsmd1.Dispose();
                            conn1.Close();
                            conn1.Dispose();
                        }


                        sqlsmd.Dispose();
                        conn.Close();
                        conn.Dispose();

                    }
                    else
                    {
                        chks.AC_Mcfile = chks.AC_Mcfile;
                    }
                    con.SaveChanges();


                    //系統LOG檔 //================================================= //
                    //Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
                    string syssubname = "網站管理作業 > 網站內容管理";
                    string sysnote = "名稱:" + chks.AC_Mctitle;
                    string sysflag = "M";
                    SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    sysconn.Close();
                    sysconn.Dispose();
                    //=================================================


                    if (chks.AC_Ccid != 0)
                    {
                        return RedirectToAction("List", new { cid = chks.AC_Ccid, cid1 = chks.AC_Cid });
                    }
                    else
                    {
                        return RedirectToAction("List", new { cid = chks.AC_Cid, cid1 = 0 });
                    }
                }
            }
            return View(chks);
        }


        [HttpPost]
        public ActionResult List(string qMctitle, string cid, string cid1)
        {
            // List<string> condition = new List<string>();     
            //string orignal = "1,2,3";
            //condition = orignal.Split(',').ToList();


            IPagedList<Webmaincontent> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                var query = con.Webmaincontents
                    .AsQueryable();

                if (cid1.ToString() != "0")
                {
                    int cvid = int.Parse(cid1);
                    query = query.Where(r => r.AC_Cid == cvid).AsQueryable();
                    ViewBag.cid1 = cid1;
                    ViewBag.cid = cid;
                }
                else
                {
                    int cvid = int.Parse(cid);
                    query = query.Where(r => r.AC_Cid == cvid).AsQueryable();
                    ViewBag.cid = cid;
                    ViewBag.cid1 = "0";
                 
                }

                if (!string.IsNullOrWhiteSpace(qMctitle))
                {
                    query = query.Where(r => r.AC_Mctitle.Contains(qMctitle.Trim())).AsQueryable();
                ViewBag.qMctitle = qMctitle;
                }


                result = query.OrderByDescending(r => r.AC_Iftop).ThenBy(r => r.AC_Mcorder).ToPagedList<Webmaincontent>(0, (int)Session["pagesize"]);

            }
            return View(result);
        }

        [HttpGet]
        public ActionResult List(int? page, string qMctitle, string cid, string cid1)
        {

            page = ((!page.HasValue || page < 1) ? 1 : page);
            IPagedList<Webmaincontent> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                var query = con.Webmaincontents
                  .AsQueryable();
                
                if (!string.IsNullOrWhiteSpace(cid1))
                {

                    if (cid1.ToString() != "0")
                    {
                        int cvid = int.Parse(cid1);

                        query = query.Where(r => r.AC_Cid == cvid).AsQueryable();
                        ViewBag.cid1 = cid1;
                        ViewBag.cid = cid;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(cid))
                        {
                            int cvid = int.Parse(cid);
                            query = query.Where(r => r.AC_Cid == cvid).AsQueryable();
                            ViewBag.cid = cid;
                            ViewBag.cid1 = "0";
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(cid))
                    {
                        int cvid = int.Parse(cid);
                        query = query.Where(r => r.AC_Cid == cvid).AsQueryable();
                        ViewBag.cid = cid;
                        ViewBag.cid1 = "0";
                    }
                    else
                    {
                        ViewBag.cid = "0";
                        ViewBag.cid1 = "0";
                    }
                }


                if (!string.IsNullOrWhiteSpace(qMctitle))
                {
                    query = query.Where(r => r.AC_Mctitle.Contains(qMctitle.Trim())).AsQueryable();
                    ViewBag.qMctitle = qMctitle;
                }

                result = query.OrderByDescending(r => r.AC_Iftop).ThenBy(r => r.AC_Mcorder).ToPagedList(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);
        }




        [HttpGet, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id,string cid,string cid1)
        {
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                Webmaincontent Webmaincontents = con.Webmaincontents.Find(id);
                con.Webmaincontents.Remove(Webmaincontents);
                con.SaveChanges();

                //系統LOG檔 //================================================= //
                Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
                string syssubname = "網站管理作業 > 網站內容管理";
                string sysnote = "名稱:" + Webmaincontents.AC_Mctitle;
                string sysflag = "D";
                SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                sysconn.Close();
                sysconn.Dispose();
                //=================================================

                if (cid1 == "0")
                { return RedirectToAction("List", new { cid = cid, cid1 = cid1 }); }
                else
                { return RedirectToAction("List", new { cid = cid1, cid1 = cid }); }
            }
        }

        public ActionResult DownLoad(string Mcfile)
        {
            return File("~/Uploads/" + Mcfile, System.Net.Mime.MediaTypeNames.Application.Octet, Server.HtmlEncode(Mcfile));
        }


        /*
                [HttpGet]
            public ActionResult Delete(int? id)
            {
           
              
                        dbquery dbobj = new dbquery();
                        dbobj.dbexecute("delete from checkcode where cid = " + id);

                    return RedirectToAction("List");
           
            }
          */


    }
}
