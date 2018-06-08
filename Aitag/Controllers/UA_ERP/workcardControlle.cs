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
    public class workcardController : BaseController
    {

        //private AitagBill_DBContext db = new AitagBill_DBContext();
        //
        // GET: /workhour/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }



        public ActionResult add(workcard col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = " wno"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = " asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qwno = "", qcustno = "";
            if (!string.IsNullOrWhiteSpace(Request["qwno"]))
            {
                qwno = Request["qwno"].Trim();
                ViewBag.qwno = qwno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcustno"]))
            {
                qcustno = Request["qcustno"].Trim();
                ViewBag.qcustno = qcustno;
            }

            NDcommon dbobj = new NDcommon();

       

            if (sysflag != "A")
            {
                workcard newcol = new workcard();
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
                    SqlConnection conn = dbobj.get_conn("AitagBill_DBContext");
                    col.wno = dbobj.get_billno(conn, "W", "", col.comid.ToString(), "", col.adddate.ToString());
                    conn.Close();
                    conn.Dispose();
                    col.wstatus = "0";
                    col.bmodid = Session["tempid"].ToString();              
                    col.bmoddate = DateTime.Now;
                    col.ownman = Session["empid"].ToString();
                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {

                        con.workcard.Add(col);
                        con.SaveChanges();                       
                    }

                    //系統LOG檔 //================================================= //
                    SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    string sysrealsid = Request["sysrealsid"].ToString();
                    string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    string sysnote = "客戶:" + col.custno + "工作卡號:" + col.wno;
                    dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    sysconn.Close();
                    sysconn.Dispose();
                    //=================================================

                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/workcard/list' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qwno' name='qwno' value='" + qwno + "'>";
                    tmpform += "<input type=hidden id='qcustno' name='qcustno' value='" + qcustno + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";
                    return new ContentResult() { Content = @"<script>alert('新增成功!!');</script>" + tmpform };
                    // return RedirectToAction("List");

                }
            }


        }


        [ValidateInput(false)]
        public ActionResult Edit(workcard chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);         
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = " wno"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = " asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qwno = "", qcustno = "";
            if (!string.IsNullOrWhiteSpace(Request["qwno"]))
            {
                qwno = Request["qwno"].Trim();
                ViewBag.qwno = qwno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcustno"]))
            {
                qcustno = Request["qcustno"].Trim();
                ViewBag.qcustno = qcustno;
            }
            if (sysflag != "E")
            {
                using (AitagBill_DBContext con = new AitagBill_DBContext())
                {
                    var data = con.workcard.Where(r => r.wno == chks.wno && r.comid==chks.comid).FirstOrDefault();
                    workcard eCompanys = con.workcard.Find(chks.wno);
                    if (eCompanys == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eCompanys);
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


                    NDcommon dbobj = new NDcommon();

                    string wno = Request["wno"].ToString();
                    string comid = Request["comid"].ToString(); 

                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {

                        workcard modobj = con.workcard.Where(r => r.wno == wno && r.comid==comid).FirstOrDefault();

                      
                        modobj.projno = Request["projno"];
                        modobj.custno = Request["custno"];
                        modobj.wktitle = Request["wktitle"];

                        modobj.wkbudget = int.Parse(Request["wkbudget"]);

                        modobj.prodno = Request["prodno"];
                        modobj.pwno = Request["pwno"];
                        modobj.ifwh = Request["ifwh"];
                        modobj.whno = Request["whno"];

                        modobj.prclosedate = DateTime.Parse(Request["prclosedate"].ToString());
                        modobj.putoffday = int.Parse(Request["putoffday"]);
                        modobj.closedate = DateTime.Parse(Request["closedate"].ToString());
                        modobj.closeman = Request["closeman"];
                        modobj.slyear = int.Parse(Request["slyear"]);
                        modobj.slmonth = int.Parse(Request["slmonth"]);
                        modobj.tkyear = int.Parse(Request["tkyear"]);
                        modobj.tkmonth = int.Parse(Request["tkmonth"]);
                     
                    
                        modobj.bmodid = Session["empid"].ToString();
                        modobj.bmoddate = DateTime.Now;
                        con.Entry(modobj).State = EntityState.Modified;
                        con.SaveChanges();
                        con.Dispose();
                    }

                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "客戶:" + Request["custno"] + "工作卡號:" + wno;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/workcard/list' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                        tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                        tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                        tmpform += "<input type=hidden id='qwno' name='qwno' value='" + qwno + "'>";
                        tmpform += "<input type=hidden id='qcustno' name='qcustno' value='" + qcustno + "'>";
                        tmpform += "</form>";
                        tmpform += "</body>";
                        return new ContentResult() { Content = @"<script>alert('修改成功!!');</script>" + tmpform };
                        // return RedirectToAction("List");
                   
                }
            }

        }


        public ActionResult List(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = " wno"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = " asc"; }

            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qwno = "", qcustno = "";
            if (!string.IsNullOrWhiteSpace(Request["qwno"]))
            {
                qwno = Request["qwno"].Trim();
                ViewBag.qwno = qwno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcustno"]))
            {
                qcustno = Request["qcustno"].Trim();
                ViewBag.qcustno = qcustno;
            }


            IPagedList<workcard> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "select * from workcard where";
                if (qcustno != "")
                    sqlstr += " custno in (select comid from allcompany where comid like '%" + qcustno + "%' or comtitle like '%" + qcustno + "%')   and";
                if (qwno != "")
                    sqlstr += " wno like '%" + qwno + "%'  and";
             
                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;


                var query = con.workcard.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<workcard>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }


        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = " wno"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = " asc"; }

            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qwno = "", qcustno = "";
            if (!string.IsNullOrWhiteSpace(Request["qwno"]))
            {
                qwno = Request["qwno"].Trim();
                ViewBag.qwno = qwno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcustno"]))
            {
                qcustno = Request["qcustno"].Trim();
                ViewBag.qcustno = qcustno;
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

                        string custno = dbobj.get_dbvalue(conn1, "select custno from workcard where wno='" + condtionArr[i].Split('/')[0].ToString() + "' and comid='" + condtionArr[i].Split('/')[1].ToString() + "'");
                        string custname = dbobj.get_dbvalue(conn1, "select comtitle from allcompany where comid='" + custno + "'");

                        sysnote += "客戶:" + custname + "，工作卡號:" + condtionArr[i].Split('/')[0].ToString() + "<br>";

                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM workcard where wno = '" + condtionArr[i].Split('/')[0].ToString() + "' and comid='" + condtionArr[i].Split('/')[1].ToString() + "'");
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM workcard_det where wno = '" + condtionArr[i].Split('/')[0].ToString() + "' and comid='" + condtionArr[i].Split('/')[1].ToString() + "'");
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
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/workcard/list' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qwno' name='qwno' value='" + qwno + "'>";
                    tmpform += "<input type=hidden id='qcustno' name='qcustno' value='" + qcustno + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";
                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');</script>" + tmpform };
                }
            }
        }


        public ActionResult detlist()
        {

            string wno = "", wcomid = "";
            wno = Request["wno"].ToString();
            wcomid = Request["comid"].ToString();
            ViewBag.wno = wno;
            ViewBag.wcomid = wcomid;

            List<workcard_det> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                var query = con.workcard_det
                  .AsQueryable();
                result = query.Where(r => r.wno == wno).Where(r => r.comid == wcomid).AsQueryable().ToList();

            }
            return View(result);
          
        }


        public ActionResult detlistdo(string sysflag, int? page, string orderdata, string orderdata1)
        {
           
            NDcommon dbobj = new NDcommon();
            SqlConnection erpconn = dbobj.get_conn("AitagBill_DBContext");
            SqlCommand cmd = new SqlCommand();
            string wno = "",comid="";

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {

                page = ((!page.HasValue || page < 1) ? 1 : page);
                ViewBag.page = page;
                ViewBag.orderdata = orderdata;
                ViewBag.orderdata1 = orderdata1;
                string sqlstr = "";

                string cdel1 = Request["wkdid"];
                string wkcdate1 = Request["wkcdate"];
                string wkstep1 = Request["wkstep"];
                string getman1 = Request["getman"];
                string prdate1 = Request["prdate"];
                string realdate1 = Request["realdate"];
                string worktype1 = Request["worktype"];
                string makeitem1 = Request["makeitem"];
               
                string[] cdelarr = cdel1.Split(',');
                string[] wkcdatedarr = wkcdate1.Split(',');
                string[] wksteparr = wkstep1.Split(',');
                string[] getmanarr = getman1.Split(',');
                string[] prdatearr = prdate1.Split(',');
                string[] realdatearr = realdate1.Split(',');
                string[] worktypearr = worktype1.Split(',');
                string[] makeitemrr = makeitem1.Split(',');

                wno = Request["wno"].ToString();
                comid = Request["comid"].ToString();
              
                for (int i = 0; i < cdelarr.Length; i++)
                {
                    if (cdelarr[i].Trim() == "")
                    {
                        if (!(wkcdatedarr[i].Trim() == "" && wksteparr[i].Trim() == ""))
                        {
                           
                                workcard_det addobj = new workcard_det();                                
                                addobj.comid = comid;
                                addobj.wno = wno;
                                addobj.wkcdate = DateTime.Parse(wkcdatedarr[i].ToString());     
                                addobj.wkstep = wksteparr[i].Trim();
                                addobj.getman = getmanarr[i].Trim();

                         
                                addobj.bmodid = Session["empid"].ToString();
                                addobj.bmoddate = DateTime.Now;
                                if (prdatearr[i].ToString() != "")
                                    addobj.prdate = DateTime.Parse(prdatearr[i].ToString());
                                else
                                    addobj.prdate = null;
                                if (realdatearr[i].ToString() != "")
                                    addobj.realdate = DateTime.Parse(realdatearr[i].ToString());
                                else
                                    addobj.realdate = null;
                                addobj.worktype = worktypearr[i].Trim();
                                addobj.makeitem = makeitemrr[i].Trim();
                               

                                con.workcard_det.Add(addobj);
                                con.SaveChanges();
                           
                        }
                    }
                    else
                    {

                        //修改
                        int wkdid = int.Parse(cdelarr[i].Trim());
                        workcard_det modobj = con.workcard_det.Where(r => r.wkdid == wkdid).FirstOrDefault();



                        modobj.wkcdate = DateTime.Parse(wkcdatedarr[i].ToString());
                        modobj.wkstep = wksteparr[i].Trim();
                        modobj.getman = getmanarr[i].Trim();
                                               
                        if (prdatearr[i].ToString() != "")
                            modobj.prdate = DateTime.Parse(prdatearr[i].ToString());
                        else
                            modobj.prdate = null;
                        if (realdatearr[i].ToString() != "")
                            modobj.realdate = DateTime.Parse(realdatearr[i].ToString());
                        else
                            modobj.realdate = null;
                        modobj.worktype = worktypearr[i].Trim();
                        modobj.makeitem = makeitemrr[i].Trim();
                               

                        modobj.bmodid = Session["empid"].ToString();
                        modobj.bmoddate = DateTime.Now;
                      

                        con.Entry(modobj).State = EntityState.Modified;
                        con.SaveChanges();

                    }
                }
                con.Dispose();


            }

            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/workcard/detlist' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden name='wno' id='wno' value='" + wno + "'>";
            tmpform += "<input type=hidden name='comid' id='comid' value='" + comid + "'>";
            tmpform += "</body>";

            erpconn.Close();
            erpconn.Dispose();

            return new ContentResult() { Content = @"" + tmpform };
        }
    }
}
