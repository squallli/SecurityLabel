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
    public class filemainController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /billsubject/
         public ActionResult filemain()
         {
             return View();
         }


        public ActionResult cupfile()
        {
            return View();
        }

        public ActionResult contuploaddel(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string vcno = "", billtype = "";
            
            if (!string.IsNullOrWhiteSpace(Request["vcno"]))
            {

                vcno = Request["vcno"].Trim();
                ViewBag.qsmdate = vcno;
            }
            if (!string.IsNullOrWhiteSpace(Request["billtype"]))
            {

                billtype = Request["billtype"].Trim();
                ViewBag.billtype = billtype;
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

                    SqlConnection conn1 = dbobj.get_conn("AitagBill_DBContext");
                    string sysnote = "";
                    string[] condtionArr = cdel.Split(',');
                    int condtionLen = condtionArr.Length;
                    string mcid = "";
                    for (int i = 0; i < condtionLen; i++)
                    {
                        string maincontent1 = dbobj.get_dbvalue(conn1, "select cfilename from erpbilldoc where cupid ='" + condtionArr[i].ToString() + "'");
                        mcid = dbobj.get_dbvalue(conn1, "select vcno from erpbilldoc where cupid ='" + condtionArr[i].ToString() + "'");

                        sysnote += "檔案名稱:" + maincontent1 + "，序號:" + condtionArr[i].ToString() + "，單據編號" + mcid + "<br>";

                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM erpbilldoc where cupid = '" + condtionArr[i].ToString() + "'");
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
                    string tgourl = "/filemain/filemain?vcno=" + vcno + "&billtype=" + billtype + "&sid=" + Session["sid"] + "&realsid=" + Session["realsid"];
                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                    //return RedirectToAction("List");
                }
            }
        }



    }
}
