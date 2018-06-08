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
    public class progparamController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /progparam/

        public ActionResult Index()
        {
            return RedirectToAction("Edit");
        }

         
        [ValidateInput(false)]
        public ActionResult Edit(progparam chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "comid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qchkclass = "", qchkitem = "";
            string comid = Session["comid"].ToString();
            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.progparam.Where(r => r.comid == comid).FirstOrDefault();
                    progparam eprogparams = con.progparam.Find(comid);
                    
                    if (eprogparams == null)
                    {
                       eprogparams = new progparam();
                      // return HttpNotFound();
                    }
                    //else { 
                    return View(eprogparams);
                    //}
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
                        chks.comid = Session["comid"].ToString();
                        con.Entry(chks).State = EntityState.Modified;
                        con.SaveChanges();

                        
                        //系統LOG檔
                        //================================================= //                     
                         SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                         string sysrealsid = Session["realsid"].ToString();
                         string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                         string sysnote = "";
                        //string sysnote = "後來使用者登入後踢掉前面使用者：:" + Request["ifkickuser"].ToString() + "<br>POP3 Server：" + Request["pop3server"].ToString() + "<br>SMTP Server：" + Request["smtpserver"].ToString() + "的資料";
                         dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                         sysconn.Close();
                         sysconn.Dispose();
                        //=================================================

                         string tmpform="";
                         tmpform = "<body onload=qfr1.submit();>";
                         tmpform += "<form name='qfr1' action='/progparam/Edit' method='post'>";
                         tmpform += "<input type=hidden name='sysflag' id='sysflag' value='A'>";
                         tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                         tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                         tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                         tmpform += "<input type=hidden name='comid' id='comid' value='" + Session["comid"].ToString() + "'>";
                         tmpform +="</form>";
                         tmpform +="</body>";


                         return new ContentResult() { Content = @"" + tmpform };
                        //return RedirectToAction("List");
                    }
               }
            }
         
        }       
    }
}
