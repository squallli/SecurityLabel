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

namespace Aitag.Controllers
{
     [DoAuthorizeFilter]
    public class holidayscheduleController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /holidayschedule/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.Msid = Session["Msid"].ToString();
        //    holidayschedule col = new holidayschedule();
        //    return View(col);
        //}

        //[HttpPost]
        public ActionResult add(holidayschedule col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "wsid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qwstitle = "", qyear = "", qcomid = "";
            if (!string.IsNullOrWhiteSpace(Request["qwstitle"]))
            {
                qwstitle = Request["qwstitle"].Trim();
                ViewBag.qwstitle = qwstitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["qyear"]))
            {

                qyear = Request["qyear"].Trim();
                ViewBag.qyear = qyear;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomid"]))
            {

                qcomid = Request["qcomid"].Trim();
                ViewBag.qcomid = qcomid;
            }

            if (sysflag != "A")
            {             
                holidayschedule newcol = new holidayschedule();
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
                    string sqlstr = "select * from holidayschedule where wsdate = '" + Request["wsdate"] + "' and comid = '" + Session["comid"] + "'";
                    sqlsmd.CommandText = sqlstr;
                    dr = sqlsmd.ExecuteReader();

                    if (dr.Read())
                    {

                        ModelState.AddModelError("", "此日期己存在，請重新輸入!");
                        return View(col);
                    }
                    dr.Close();
                    dr.Dispose();
                    sqlsmd.Dispose();
                    conn.Close();
                    conn.Dispose();

                    //密碼加密
                    col.yhid = Request["yhid"].ToString();
                    col.comid = Session["comid"].ToString();
                    col.bmodid = Session["tempid"].ToString();
                    col.bmoddate = DateTime.Now;            
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.holidayschedule.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "行事曆標題:" + col.wstitle + "<br>日期:" + col.wsdate + "的資料";     
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/holidayschedule/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qwstitle' name='qchkclass' value='" + qwstitle + "'>";
                    tmpform += "<input type=hidden id='qyear' name='qchkitem' value='" + qyear + "'>";
                    tmpform += "<input type=hidden id='qcomid' name='qchkitem' value='" + qcomid + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                   // return RedirectToAction("List");

                }
            }

           
        }

    
        [ValidateInput(false)]
        public ActionResult Edit(holidayschedule chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "wsid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qwstitle = "", qyear = "", qcomid = "";
            if (!string.IsNullOrWhiteSpace(Request["qwstitle"]))
            {
                qwstitle = Request["qwstitle"].Trim();
                ViewBag.qwstitle = qwstitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["qyear"]))
            {

                qyear = Request["qyear"].Trim();
                ViewBag.qyear = qyear;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomid"]))
            {

                qcomid = Request["qcomid"].Trim();
                ViewBag.qcomid = qcomid;
            }
            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.holidayschedule.Where(r => r.wsid == chks.wsid).FirstOrDefault();
                    holidayschedule eholidayschedules = con.holidayschedule.Find(chks.wsid);
                    if (eholidayschedules == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eholidayschedules);
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

                        //try
                        //{
                        //    con.SaveChanges();
                        //    con.Dispose();
                        //}
                        //catch (Exception ex)
                        //{
                        //    throw;
                        //}

                        
                        //系統LOG檔
                        //================================================= //                     
                         SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                         string sysrealsid = Request["sysrealsid"].ToString();
                         string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                         string sysnote = "行事曆標題:" + chks.wstitle + "<br>日期:" + chks.wsdate + "的資料";     
                         dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                         sysconn.Close();
                         sysconn.Dispose();
                        //=================================================

                         string tmpform="";
                         tmpform = "<body onload=qfr1.submit();>";
                         tmpform += "<form name='qfr1' action='/holidayschedule/List' method='post'>";
                         tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                         tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                         tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                         tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                         tmpform += "<input type=hidden id='qwstitle' name='qchkclass' value='" + qwstitle + "'>";
                         tmpform += "<input type=hidden id='qyear' name='qchkitem' value='" + qyear + "'>";
                         tmpform += "<input type=hidden id='qcomid' name='qchkitem' value='" + qcomid + "'>";
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
            { orderdata = "wsdate"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcomid = "", qyear = "", qwstitle="";
            if (!string.IsNullOrWhiteSpace(Request["qcomid"]))
            {
                qcomid = Request["qcomid"].Trim();
                ViewBag.qcomid = qcomid;
            }
            else
            {
                qcomid = (string)Session["comid"];
                ViewBag.qcomid = qcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qyear"]))
            {

                qyear = Request["qyear"].Trim();
                ViewBag.qyear = qyear;
            }
            if (!string.IsNullOrWhiteSpace(Request["qwstitle"]))
            {

                qwstitle = Request["qwstitle"].Trim();
                ViewBag.qwstitle = qwstitle;
            }

            IPagedList<holidayschedule> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from holidayschedule where";

                if (qcomid != "")
                { sqlstr += " comid = '" + qcomid + "'  and"; }


                if (qyear != "")
                { sqlstr += " year(wsdate) = '" + qyear + "'  and"; }

                if (qwstitle != "")
                { sqlstr += " wstitle like N'%" + qwstitle + "%'  and"; }

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.holidayschedule.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<holidayschedule>(page.Value - 1, (int)Session["pagesize"]);
            
            }
            return View(result);
        }

     


        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qwstitle = "", qyear = "", qcomid = "";
            if (!string.IsNullOrWhiteSpace(Request["qwstitle"]))
            {
                qwstitle = Request["qwstitle"].Trim();
                ViewBag.qwstitle = qwstitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["qyear"]))
            {

                qyear = Request["qyear"].Trim();
                ViewBag.qchkitem = qyear;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomid"]))
            {

                qcomid = Request["qcomid"].Trim();
                ViewBag.qchkitem = qcomid;
            }
            string cdel = Request["cdel"];

            if (string.IsNullOrWhiteSpace(cdel))
            {

                string tgourl = "/holidayschedule/List?page=" + page + "&qwstitle=" + qwstitle + "&qyear=" + qyear + "&qcomid=" + qcomid;
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
                            string eholidayschedules = dbobj.get_dbvalue(conn1, "select wstitle from holidayschedule where wsid ='" + condtionArr[i].ToString() + "'");

                            sysnote += "行事曆標題：:" + eholidayschedules + "，序號:" + condtionArr[i].ToString() + "<br>";

                            dbobj.dbexecute("Aitag_DBContext", "DELETE FROM holidayschedule where wsid = '" + condtionArr[i].ToString() + "'");
                           
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
                        string tgourl = "/holidayschedule/List?page=" + page + "&qwstitle=" + qwstitle + "&qyear=" + qyear + "&qcomid=" + qcomid;
                        return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                        //return RedirectToAction("List");
                    }
            }
        }

        public ActionResult Transfer1()
        {
            return View();
        }

        public ActionResult Transfer(holidayschedule col, string sysflag, HttpPostedFileBase upfile)
        {
            if (sysflag != "A")
            {
                holidayschedule newcol = new holidayschedule();
                return View(newcol);
            }
            else
            {
                Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
                string tmpform = "";
                if (upfile != null)
                {
                    String sernonum = "";
                    //重新命名，存入檔案
                    DateTime myDate = DateTime.Now;
                    sernonum = myDate.ToString("yyyyMMddHHmmss");
                    string BasicPath = Server.MapPath("~/upload/");
                    int inputcount = 0;
                    string fileName = upfile.FileName.Substring(upfile.FileName.IndexOf("."), upfile.FileName.Length - upfile.FileName.IndexOf("."));

                    if (fileName != ".exe" && fileName != ".asp" && fileName != ".aspx" && fileName != ".jsp" && fileName != ".php")
                    {
                        fileName = "udf-" + sernonum.ToString() + fileName;
                        upfile.SaveAs(Server.MapPath("~/upload/") + fileName);

                        string tmppath = BasicPath + fileName;

                        //StreamReader sr = new StreamReader(@tmppath); //讀取檔案
                        StreamReader sr = new StreamReader(@tmppath, System.Text.Encoding.Default);
                        string allstr = sr.ReadToEnd(); //從資料流末端存取檔案
                        sr.Close();

                        string[] tmpstridno; //匯入資料
                        string[] toptmparry; //匯入的第一筆資料(欄位)
                        int tmparrycount = 0; //匯入欄位數
                        tmpstridno = allstr.Split(System.Environment.NewLine.ToCharArray());

                        //找第一筆的欄位數
                        toptmparry = tmpstridno[0].Split(',');

                        //先暫時停掉20160827
                        for (int tmpi = 0; tmpi <= toptmparry.Length - 1; tmpi++)
                        {
                            if (toptmparry[tmpi] != "")
                            {
                                tmparrycount++;
                            }
                        }

                        SqlConnection conn = dbobj.get_conn("Aitag_DBContext");
                        string[] tmparry;
                        string tmpvalue = "";
                        string tmpaddsql = "";
                        string cmid = "";

                        for (int i = 1; i <= tmpstridno.Length - 1; i++)
                        {
                            if (tmpstridno[i] != "")
                            {
                                inputcount++;
                                tmparry = tmpstridno[i].Split(',');
                                //判斷必填欄位
                                if (tmparry[0] != "" && tmparry[1] != "" && tmparry[2] != "")
                                {
                                    string wstype = "";
                                    if (tmparry[2] == "假日")
                                    { wstype = "0"; }
                                    if (tmparry[2] == "上班")
                                    { wstype = "1"; }
                                    if (tmparry[2] == "年假")
                                    { wstype = "2"; }
                                    col.wstitle = tmparry[0];
                                    col.wsdate = Convert.ToDateTime(tmparry[1]);
                                    col.wstype = wstype;
                                    col.comid = Session["comid"].ToString();
                                    col.bmodid = Session["tempid"].ToString();
                                    col.bmoddate = DateTime.Now;
                                    using (Aitag_DBContext con = new Aitag_DBContext())
                                    {
                                        con.holidayschedule.Add(col);
                                        con.SaveChanges();

                                        //系統LOG檔 //================================================= //
                                        //SqlConnection sysconn = dbobj.get_conn("MatsuEip_DBContext");
                                        //string sysrealsid = Request["sysrealsid"].ToString();
                                        //string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                                        //string sysnote = "行事曆標題:" + col.wstitle + "<br>日期:" + col.wsdate + "的資料";
                                        //dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                                        //sysconn.Close();
                                        //sysconn.Dispose();
                                        //=================================================

                                    }


                                    //tmpform = "<body onload=javascript:alert('轉檔成功！');parent.opener.location.href='/holidayschedule/List?sid=" + Request["sid"].ToString() + "&realsid=" + Request["realsid"].ToString() + "';window.close();>";
                                }
                            }


                        }


                    }
                    else
                    {
                        ViewBag.AddModelError = @"alert('上傳格式錯誤！');";
                        return View();
                    }

                }
                ViewBag.AddModel = @"alert('轉檔成功！');" +
                    @"parent.opener.location.href = '/holidayschedule/list?sid=" + Request["sid"].ToString() + "&realsid=" + Request["realsid"].ToString() + "';" +
                    @"window.close();";


                return View();
            }



        }



    }
}
