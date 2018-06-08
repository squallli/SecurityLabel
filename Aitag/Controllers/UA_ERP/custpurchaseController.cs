using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aitag.Models;
using System.Data;
using System.Data.SqlClient;
using Aitag.Filters;
using MvcPaging;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;


namespace Aitag.Controllers
{
     [DoAuthorizeFilter]
    public class custpurchaseController : BaseController
    {
      
        public ActionResult Index()
        {

            
            return View();
        }

        public ActionResult list(int? page, string orderdata, string orderdata1)
        {
            IPagedList<custpurchase> result;
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "pid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qpdno = "", qallcomid = "", qpcomment = "", qspdate = "", qepdate = "";

            if (!string.IsNullOrWhiteSpace(Request["qpdno"]))
            {
                qpdno = Request["qpdno"].Trim();
                ViewBag.qpdno = qpdno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qallcomid"]))
            {
                qallcomid = Request["qallcomid"].Trim();
                ViewBag.qallcomid = qallcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qpcomment"]))
            {
                qpcomment = Request["qpcomment"].Trim();
                ViewBag.qpcomment = qpcomment;
            }
            if (!string.IsNullOrWhiteSpace(Request["qspdate"]))
            {
                qspdate = Request["qspdate"].Trim();
                ViewBag.qspdate = qspdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qepdate"]))
            {
                qepdate = Request["qepdate"].Trim();
                ViewBag.qevadate = qepdate;
            }

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "";
                sqlstr = "select * from custpurchase where comid = '" + Session["comid"].ToString() + "' and vendtype='1'  and";
                if (qpdno != "")
                    sqlstr += " pdno like '%" + qpdno + "%'  and";
                if (qallcomid != "")
                    sqlstr += " allcomid in (select comid from allcompany where comid like '%" + qallcomid + "%' or comtitle like '%" + qallcomid + "%')   and";
                if (qpcomment != "")
                    sqlstr += " pcomment like '%" + qpcomment + "%'  and";
                if (qspdate != "")
                    sqlstr += " pdate >= '" + qspdate + "'  and";
                if (qepdate != "")
                    sqlstr += " pdate <= '" + qepdate + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.custpurchase.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<custpurchase>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);
        }

        public ActionResult budgetlist(int? page, string orderdata, string orderdata1)
        {
            IPagedList<custpurchase> result;
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "pid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qpdno = "", qallcomid = "", qpcomment = "", qspdate = "", qepdate = "";

            if (!string.IsNullOrWhiteSpace(Request["qpdno"]))
            {
                qpdno = Request["qpdno"].Trim();
                ViewBag.qpdno = qpdno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qallcomid"]))
            {
                qallcomid = Request["qallcomid"].Trim();
                ViewBag.qallcomid = qallcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qpcomment"]))
            {
                qpcomment = Request["qpcomment"].Trim();
                ViewBag.qpcomment = qpcomment;
            }
            if (!string.IsNullOrWhiteSpace(Request["qspdate"]))
            {
                qspdate = Request["qspdate"].Trim();
                ViewBag.qspdate = qspdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qepdate"]))
            {
                qepdate = Request["qepdate"].Trim();
                ViewBag.qevadate = qepdate;
            }

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "";
                sqlstr = "select * from custpurchase where comid = '" + Session["comid"].ToString() + "'  and";
                if (qpdno != "")
                    sqlstr += " pdno like '%" + qpdno + "%'  and";
                if (qallcomid != "")
                    sqlstr += " allcomid in (select comid from allcompany where comid like '%" + qallcomid + "%' or comtitle like '%" + qallcomid + "%')   and";
                if (qpcomment != "")
                    sqlstr += " pcomment like '%" + qpcomment + "%'  and";
                if (qspdate != "")
                    sqlstr += " pdate >= '" + qspdate + "'  and";
                if (qepdate != "")
                    sqlstr += " pdate <= '" + qepdate + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.custpurchase.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<custpurchase>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);
        }
         
        public ActionResult add()
        {
            return View();
        }

        public ActionResult adddo(string sysflag, int? page, string orderdata, string orderdata1)
        {

            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qpdno = "", qallcomid = "", qpcomment = "", qspdate = "", qepdate = "";

            if (!string.IsNullOrWhiteSpace(Request["qpdno"]))
            {
                qpdno = Request["qpdno"].Trim();
                ViewBag.qpdno = qpdno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qallcomid"]))
            {
                qallcomid = Request["qallcomid"].Trim();
                ViewBag.qvendno = qallcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qpcomment"]))
            {
                qpcomment = Request["qpcomment"].Trim();
                ViewBag.qvcomment = qpcomment;
            }
            if (!string.IsNullOrWhiteSpace(Request["qspdate"]))
            {
                qspdate = Request["qspdate"].Trim();
                ViewBag.qspdate = qspdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qepdate"]))
            {
                qepdate = Request["qepdate"].Trim();
                ViewBag.qevadate = qepdate;
            }


            NDcommon dbobj = new NDcommon();
            SqlConnection conn = dbobj.get_conn("AitagBill_DBContext");
            string pdno = "";
            string pid = "";
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {

                custpurchase addobj = new custpurchase();              
                pdno = dbobj.get_billno(conn, "K", "", Session["comid"].ToString(), "", Request["pdate"].ToString());
                addobj.pid = DateTime.Now.ToString("yyyyMMddhhmmssfffff");
                pid = addobj.pid;
                addobj.vendtype = "1";
                addobj.pdate = DateTime.Parse(Request["pdate"].ToString());
                addobj.psdate = DateTime.Parse(Request["psdate"].ToString());
                addobj.pedate = DateTime.Parse(Request["pedate"].ToString());
                addobj.material = Request["material"].ToString();
                addobj.comid = Session["comid"].ToString();
                addobj.projno = pdno;
                addobj.pdno = pdno;            
                addobj.pstatus = "0";
                addobj.taxtype = Request["taxtype"];
                addobj.ownman = Session["empid"].ToString();
                //產品編號
                addobj.prodid = Request["prodid"].ToString();
                
                addobj.pmoney = int.Parse(Request["pmoney"]);
                addobj.ptaxmoney = int.Parse(Request["ptaxmoney"]);
                addobj.pallmoney = int.Parse(Request["pallmoney"]);
                addobj.acrate = decimal.Parse(Request["acrate"]);
               // addobj.moneyrate = decimal.Parse(Request["moneyrate"]);
               // addobj.prerate = decimal.Parse(Request["prerate"]);
               // addobj.pregetdate = DateTime.Parse(Request["pregetdate"].ToString());
                addobj.allcomid = Request["allcomid"].Trim();
                addobj.ifagent = Request["ifagent"].Trim();
                addobj.campainetitle = Request["campainetitle"].Trim();
                addobj.pcomment = Request["pcomment"].Trim();              
          
                addobj.bmodid = Session["empid"].ToString();
                addobj.bmoddate = DateTime.Now;

                con.custpurchase.Add(addobj);

                con.SaveChanges();
                con.Dispose();

            }

            conn.Close();
            conn.Dispose();

            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/custpurchase/conbudgetlist' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden id='qpdno' name='qpdno' value='" + qpdno + "'>";
            tmpform += "<input type=hidden id='qallcomid' name='qallcomid' value='" + qallcomid + "'>";
            tmpform += "<input type=hidden id='qpcomment' name='qpcomment' value='" + qpcomment + "'>";
            tmpform += "<input type=hidden id='qspdate' name='qspdate' value='" + qspdate + "'>";
            tmpform += "<input type=hidden id='qepdate' name='qepdate' value='" + qepdate + "'>";
            tmpform += "<input type=hidden id='pid' name='pid' value='" + pid + "'>";
            tmpform += "</form>";
            tmpform += "</body>";

      
            return new ContentResult() { Content = @"" + tmpform };
        }

        public ActionResult Delete(string id, int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qpdno = "", qallcomid = "", qpcomment = "", qspdate = "", qepdate = "";

            if (!string.IsNullOrWhiteSpace(Request["qpdno"]))
            {
                qpdno = Request["qpdno"].Trim();
                ViewBag.qpdno = qpdno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qallcomid"]))
            {
                qallcomid = Request["qallcomid"].Trim();
                ViewBag.qvendno = qallcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qpcomment"]))
            {
                qpcomment = Request["qpcomment"].Trim();
                ViewBag.qvcomment = qpcomment;
            }
            if (!string.IsNullOrWhiteSpace(Request["qspdate"]))
            {
                qspdate = Request["qspdate"].Trim();
                ViewBag.qspdate = qspdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qepdate"]))
            {
                qepdate = Request["qepdate"].Trim();
                ViewBag.qevadate = qepdate;
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
                        string pdno = dbobj.get_dbvalue(conn1, "select pdno from custpurchase where pid='" + condtionArr[i].ToString() + "'");

                        sysnote += "單號:" + pdno + "<br>";
                        //刪除憑單
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM custpurchase where pid = '" + condtionArr[i].ToString() + "'");
                        //刪除明細
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM conbudgetdet where pid = '" + condtionArr[i].ToString() + "'");
                        //dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM custpurchase_det where pid = '" + condtionArr[i].ToString() + "'");
                      
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
                    tmpform += "<form name='qfr1' action='/custpurchase/budgetlist' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qpdno' name='qpdno' value='" + qpdno + "'>";
                    tmpform += "<input type=hidden id='qallcomid' name='qallcomid' value='" + qallcomid + "'>";
                    tmpform += "<input type=hidden id='qpcomment' name='qpcomment' value='" + qpcomment + "'>";
                    tmpform += "<input type=hidden id='qspdate' name='qspdate' value='" + qspdate + "'>";
                    tmpform += "<input type=hidden id='qepdate' name='qepdate' value='" + qepdate + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');</script>" + tmpform };
                }
            }
        }

        public ActionResult Edit()
        {
    
            ViewBag.pid = Request["pid"].ToString();
            return View();
        }

        public ActionResult Editdo(string sysflag, int? page, string orderdata, string orderdata1)
        {

            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qpdno = "", qallcomid = "", qpcomment = "", qspdate = "", qepdate = "";

            if (!string.IsNullOrWhiteSpace(Request["qpdno"]))
            {
                qpdno = Request["qpdno"].Trim();
                ViewBag.qpdno = qpdno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qallcomid"]))
            {
                qallcomid = Request["qallcomid"].Trim();
                ViewBag.qvendno = qallcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qpcomment"]))
            {
                qpcomment = Request["qpcomment"].Trim();
                ViewBag.qvcomment = qpcomment;
            }
            if (!string.IsNullOrWhiteSpace(Request["qspdate"]))
            {
                qspdate = Request["qspdate"].Trim();
                ViewBag.qspdate = qspdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qepdate"]))
            {
                qepdate = Request["qepdate"].Trim();
                ViewBag.qevadate = qepdate;
            }

            string pid = Request["pid"].ToString();
            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                custpurchase modobj = con.custpurchase.Where(r => r.pid == pid).FirstOrDefault();

                modobj.pdate = DateTime.Parse(Request["pdate"].ToString());
                modobj.psdate = DateTime.Parse(Request["psdate"].ToString());
                modobj.pedate = DateTime.Parse(Request["pedate"].ToString());
                //modobj.comid = Session["comid"].ToString();
               // modobj.projno = Request["projno"];

                modobj.taxtype = Request["taxtype"];          
                modobj.pmoney = int.Parse(Request["pmoney"]);
                modobj.ptaxmoney = int.Parse(Request["ptaxmoney"]);
                modobj.pallmoney = int.Parse(Request["pallmoney"]);
                modobj.acrate = decimal.Parse(Request["acrate"]);
          //      modobj.moneyrate = decimal.Parse(Request["moneyrate"]);
          //      modobj.prerate = decimal.Parse(Request["prerate"]);
          //      modobj.pregetdate = DateTime.Parse(Request["pregetdate"].ToString());
                modobj.pcomment = Request["pcomment"];
                modobj.campainetitle = Request["campainetitle"];
                modobj.material = Request["material"];
                modobj.prodid = Request["prodid"];
                modobj.allcomid = Request["allcomid"];
                modobj.ifagent = Request["ifagent"].Trim();
                modobj.bmodid = Session["empid"].ToString();
                modobj.bmoddate = DateTime.Now;              
                con.Entry(modobj).State = EntityState.Modified;
                con.SaveChanges();
                con.Dispose();
            }
            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/custpurchase/budgetlist' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden id='qpdno' name='qpdno' value='" + qpdno + "'>";
            tmpform += "<input type=hidden id='qallcomid' name='qallcomid' value='" + qallcomid + "'>";
            tmpform += "<input type=hidden id='qpcomment' name='qpcomment' value='" + qpcomment + "'>";
            tmpform += "<input type=hidden id='qspdate' name='qspdate' value='" + qspdate + "'>";
            tmpform += "<input type=hidden id='qepdate' name='qepdate' value='" + qepdate + "'>";
            tmpform += "</form>";
            tmpform += "</body>";


            return new ContentResult() { Content = @"" + tmpform };
        }

        #region 資料庫custpurchase_det 目前沒用到
        //public ActionResult detlist()
        //{
        //    string tmppid = "";
        //    tmppid = Request["pid"].ToString();
        //    ViewBag.pid = tmppid.ToString();

        //    List<custpurchase_det> result;
        //    using (AitagBill_DBContext con = new AitagBill_DBContext())
        //    {
        //        var query = con.custpurchase_det.AsQueryable();
        //        result = query.Where(r => r.pid == tmppid).AsQueryable().ToList();

        //    }
        //    return View(result);
      
        //}

        
        //public ActionResult detlistdo(string sysflag, int? page, string orderdata, string orderdata1)
        //{
          
        //    string pid = "";
        //    Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
        //    decimal allmoney = 0;
        //    using (AitagBill_DBContext con = new AitagBill_DBContext())
        //    {
                
        //        page = ((!page.HasValue || page < 1) ? 1 : page);
        //        ViewBag.page = page;
        //        ViewBag.orderdata = orderdata;
        //        ViewBag.orderdata1 = orderdata1;
        //        string sqlstr = "";
        //        string cdel1 = Request["pdid"];

        //      //  string bdprodno1 = Request["bdprodno"];
        //      //  string bdprodtitle1 = Request["bdprodtitle"];
        //        string mdno1 = Request["mdno"];
        //        string mdcomment1 = Request["mdcomment"];
               
        //       // string pdunit1 = Request["pdunit"];
        //       // string pdcount1 = Request["pdcount"];
        //       // string pdmoney1 = Request["pdmoney"];
        //        string pdallmoney1 = Request["pdallmoney"];
        //        string pdcomment1 = Request["pdcomment"];

        //        string[] cdelarr = cdel1.Split(',');
        //       // string[] bdprodnoarr = bdprodno1.Split(',');
        //       // string[] bdprodtitlearr = bdprodtitle1.Split(',');
                
        //        string[] mdnoarr = mdno1.Split(',');
        //        string[] mdcommentarr = mdcomment1.Split(',');
        //      //  string[] pdunitarr = pdunit1.Split(',');
        //      //  string[] pdcountarr = pdcount1.Split(',');
        //      //  string[] pdmoneyarr = pdmoney1.Split(',');
        //        string[] pdallmoneyarr = pdallmoney1.Split(',');
        //        string[] pdcommentarr = pdcomment1.Split(',');              

        //        pid = Request["pid"].ToString();
        //        int pitemno = 10;
        //        for (int i = 0; i < cdelarr.Length; i++)
        //        {
        //            if (cdelarr[i].Trim() == "")
        //            {
        //                if (!(mdnoarr[i].Trim() == ""))
        //                {
        //                        custpurchase_det addobj = new custpurchase_det();
        //                        addobj.pid = pid;
        //                        addobj.pdno = Request["pdno"].ToString();
        //                        addobj.comid = Request["comid"].ToString();                             

        //                       // addobj.bdprodno = bdprodnoarr[i].Trim();
        //                       // addobj.bdprodtitle = bdprodtitlearr[i].Trim();
        //                        addobj.mdno = mdnoarr[i].Trim();
        //                        addobj.mdcomment = mdcommentarr[i].Trim();
                              

        //                        addobj.pitemno = pitemno;
        //                     //   addobj.pdunit = pdunitarr[i].Trim();
        //                     //   addobj.pdcount = Decimal.Parse(pdcountarr[i].ToString());
        //                     //   addobj.pdmoney = Decimal.Parse(pdmoneyarr[i].ToString());
        //                        addobj.pdcount = 1;
        //                        addobj.pdallmoney = Decimal.Parse(pdallmoneyarr[i].ToString());
        //                        addobj.pdmoney = Decimal.Parse(pdallmoneyarr[i].ToString());
        //                        addobj.pdcomment = pdcommentarr[i].Trim();
        //                        //addobj.projno = Request["projno"].ToString();

        //                        addobj.bmodid = Session["empid"].ToString();
        //                        addobj.bmoddate = DateTime.Now;

        //                        con.custpurchase_det.Add(addobj);
        //                        con.SaveChanges();
        //                        pitemno = pitemno + 10;
        //                        allmoney += Decimal.Parse(pdallmoneyarr[i].ToString()); 
        //                }
        //            }
        //            else
        //            {
        //                //修改
        //                int pdid = int.Parse(cdelarr[i].Trim());
        //                custpurchase_det modobj = con.custpurchase_det.Where(r => r.pdid == pdid).FirstOrDefault();


        //           //     modobj.bdprodno = bdprodnoarr[i].Trim();
        //           //     modobj.bdprodtitle = bdprodtitlearr[i].Trim();
        //                modobj.mdno = mdnoarr[i].Trim();
        //                modobj.mdcomment = mdcommentarr[i].Trim();
        //           //     modobj.pdunit = pdunitarr[i].Trim();
        //           //     modobj.pdcount = Decimal.Parse(pdcountarr[i].ToString());
        //                modobj.pdmoney = Decimal.Parse(pdallmoneyarr[i].ToString());
        //                modobj.pdallmoney = Decimal.Parse(pdallmoneyarr[i].ToString());
        //                modobj.pdcomment = pdcommentarr[i].Trim();  
        //                modobj.bmodid = Session["empid"].ToString();
        //                modobj.bmoddate = DateTime.Now;
        //                allmoney += Decimal.Parse(pdallmoneyarr[i].ToString()); 

        //                con.Entry(modobj).State = EntityState.Modified;
        //                con.SaveChanges();
        //            }
        //            string tmpsql = "update custpurchase set pallmoney = " + allmoney + " where pid = " + pid;
        //            dbobj.dbexecute("AitagBill_DBContext", tmpsql);
        //        }
        //        con.Dispose();


        //    }

        //    string tmpform = "";
        //    tmpform = "<body onload=qfr1.submit();>";
        //    tmpform += "<form name='qfr1' action='/custpurchase/detlist' method='post'>";
        //    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
        //    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
        //    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
        //    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
        //    tmpform += "<input type=hidden name='pid' id='pid' value='" + pid + "'>";        
        //    tmpform += "</form>";
        //    tmpform += "</body>";

 

        //    return new ContentResult() { Content = @"" + tmpform };
        //}

        //public ActionResult detdel(string id, int? page)
        //{
        //    page = ((!page.HasValue || page < 1) ? 1 : page);
        //    string cdel = Request["cdel"];
        //    string pid = Request["pid"];

        //    if (string.IsNullOrWhiteSpace(cdel))
        //    {

        //        return new ContentResult() { Content = @"<script>alert('請勾選要刪除的資料!!');window.history.go(-1);</script>" };
        //    }
        //    else
        //    {
        //        using (AitagBill_DBContext con = new AitagBill_DBContext())
        //        {

        //            NDcommon dbobj = new NDcommon();
        //            SqlConnection conn1 = dbobj.get_conn("AitagBill_DBContext");
        //            string sysnote = "";
        //            string[] condtionArr = cdel.Split(',');
        //            int condtionLen = condtionArr.Length;
        //            for (int i = 0; i < condtionLen; i++)
        //            {
        //                string money1 = dbobj.get_dbvalue(conn1, "select ('Campaine號' + pdno + ',品項' + bdprodno + ',金額' + convert(char,pdallmoney)) as st1  from custpurchase_det where pdid = '" + condtionArr[i].ToString() + "'");

        //                sysnote += money1 + "<br>";
        //                //刪除明細資料
        //                dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM custpurchase_det where pdid = '" + condtionArr[i].ToString() + "'");

        //            }

        //            conn1.Close();
        //            conn1.Dispose();
        //            string sysrealsid = Request["sysrealsid"].ToString();
        //            //系統LOG檔
        //            //================================================= //                  
        //            SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
        //            string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
        //            string sysflag = "D";
        //            dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
        //            sysconn.Close();
        //            sysconn.Dispose();
        //            //======================================================          
        //            string tmpform = "";
        //            tmpform = "<body onload=qfr1.submit();>";
        //            tmpform += "<form name='qfr1' action='/custpurchase/detlist' method='post'>";
        //            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
        //            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
        //            tmpform += "<input type=hidden id='pid' name='pid' value='" + pid + "'>";
        //            tmpform += "</form>";
        //            tmpform += "</body>";

        //            return new ContentResult() { Content = @"<script>alert('刪除成功!!');</script>" + tmpform };


        //        }
        //    }
        //}

        #endregion

        public ActionResult workflowdo(string sysflag, int? page, string orderdata, string orderdata1)
        {
            string tmparolestampid = "";
            string tmprole = "";
            string tmpbillid = "";
            NDcommon dbobj = new NDcommon();

            string pid = Request["pid"].ToString().Trim();
       
            #region 寄信參數
            string bccemail = "";        
            string tmpsno = "";  //單號
            string tmpdate = "";//申請日期        
            string tmpnote = "";//摘要說明
            string tmpownman = "";//申請人
            string tmpmtitle = "";
            string tomail = "";
            #endregion


            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                custpurchase modobj = con.custpurchase.Where(r => r.pid == pid).FirstOrDefault();

                if (Request["pstatus"].ToString() == "0")  //第一次送時,補件修正不用
                {
                    if (Request["arolestampid"].ToString() != "")
                        tmparolestampid = "'" + Request["arolestampid"].ToString() + "'";
                    else
                        tmparolestampid = "'" + Request["arolestampid1"].ToString() + "'";

                    string tmpmoney = Request["pmoney"].ToString();
                    //找出下一個角色是誰               
                    string impallstring = dbobj.getnewcheck1("M", tmparolestampid, tmparolestampid, tmpmoney, "", "", "");

                    string[] tmpstrarr = impallstring.Split(';');
                    tmprole = tmpstrarr[0].ToString();
                    tmpbillid = tmpstrarr[1].ToString();
                    if (tmprole == "")
                    {
                        return new ContentResult() { Content = @"<body onload=javascript:alert('請先至表單流程設定中設定呈核單據的呈核流程!');window.history.go(-1);>" };
                    }
                }
                else
                {
                    tmprole = Request["rolestampid"].ToString();
                }

            

                #region 寄信參數值
                //===============               
                System.Data.SqlClient.SqlConnection tmpconn1 = dbobj.get_conn("Aitag_DBContext");
              
                tmpsno = modobj.pdno;
                tmpdate = dbobj.get_date(modobj.pdate.ToString(), "1");
                // tmpvendno = modobj.vendno + "－" + dbobj.get_dbvalue(tmpconn1, "select comsttitle from allcompany where comid = '" + modobj.vendno + "'");
                // tmpamoney = modobj.totalmoney.ToString();
                tmpnote = modobj.pcomment;
                tmpownman = modobj.ownman;
                bccemail = dbobj.get_dbvalue(tmpconn1, "select enemail from employee where empid = '" + tmpownman + "'");
                tmpconn1.Close();
                tmpconn1.Dispose();
                //===============
                #endregion

                //呈核人員
                //======================
                //當不是代申請且未選擇(就是沒有下拉)申請呈核角色時，tmparolestampid就接arolestampid1的值

                if (Request["pstatus"].ToString() == "0")  //第一次送時,補件修正不用
                {
                    modobj.pstatus = "1";
                    modobj.arolestampid = tmparolestampid; //申請角色
                    modobj.rolestampid = tmprole; //下個呈核角色
                    modobj.rolestampidall = tmparolestampid; //呈核角色
                    modobj.empstampidall = "'" + Session["empid"].ToString() + "'"; //所有人員帳號
                    modobj.billtime = DateTime.Now.ToString(); //所有時間
                    modobj.billflowid = int.Parse(tmpbillid);
                    modobj.bmodid = Session["empid"].ToString();
                    modobj.bmoddate = DateTime.Now;
                }
                else
                {
                    tmpmtitle = "(補件修正)";
                    modobj.pstatus = "1";
                    modobj.bmodid = Session["empid"].ToString();
                    modobj.bmoddate = DateTime.Now;
                }
                con.Entry(modobj).State = EntityState.Modified;
                con.SaveChanges();
                con.Dispose();
            }

            #region  寄信
            string txt_comment = "";
            string mailtitle = "";
            string mailcontext = "";

            mailtitle = "媒體預算分配表作業申請要求審核通知" + tmpmtitle;
            txt_comment = "有一筆申請單提出，請至系統進行審核作業。<br>資料如下：";

            mailcontext += "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=450 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
            mailcontext += "<tr>";
            mailcontext += "<td colspan=2>" + txt_comment + "</td>";
            mailcontext += "</tr>";

     
            mailcontext += "<tr>";
            mailcontext += "<td align=right width='90'>採購(委刊單)號：</td>";
            mailcontext += "<td>" + tmpsno + "</td>";
            mailcontext += "</tr>";

            mailcontext += "<tr>";
            mailcontext += "<td align=right>申請日期：</td>";
            mailcontext += "<td>" + tmpdate + "</td>";
            mailcontext += "</tr>";

            mailcontext += "<tr>";
            mailcontext += "<td align=right>申請人：</td>";
            mailcontext += "<td>" + tmpownman + "</td>";
            mailcontext += "</tr>";


            mailcontext += "<tr>";
            mailcontext += "<td align=right>摘要說明：</td>";
            mailcontext += "<td>" + tmpnote + "</td>";
            mailcontext += "</tr>";

            mailcontext += "</table>";
            mailcontext += "<br><font size='9pt' color=#404040>此為系統自動發信，請勿直接回覆！</font>";

            //寄給下一個審核者
            System.Data.SqlClient.SqlConnection tmpconn2 = dbobj.get_conn("Aitag_DBContext");
            SqlDataReader dr;
            SqlCommand sqlsmd = new SqlCommand();
            sqlsmd.Connection = tmpconn2;
            string sqlstr = "select employee.enemail FROM emprole INNER JOIN employee ON emprole.empid =employee.empid where emprole.rid =" + tmprole + " and employee.enemail <>''";
            sqlsmd.CommandText = sqlstr;
            dr = sqlsmd.ExecuteReader();

            while (dr.Read())
            {
                tomail += dr["enemail"].ToString() + ",";
            }
            dbobj.send_mailfile("", tomail, mailtitle, mailcontext, null, null);
            dr.Close();
            dr.Dispose();
            sqlsmd.Dispose();

            tmpconn2.Close();
            tmpconn2.Dispose();


            #endregion

            string tmpform = "";
            tmpform = "<body onload=javascript:alert('送出審核成功!!');qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/custpurchase/budgetlist' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";         
            tmpform += "</form>";
            tmpform += "</body>";


            return new ContentResult() { Content = @"" + tmpform };
        }

        public ActionResult send(int? page, string orderdata, string orderdata1)
        {
            NDcommon dbobj = new NDcommon();           
            string cdel = Request["cdel"];
            string tmpform = "";
            string errmsg = "";
            if (string.IsNullOrWhiteSpace(cdel))
            {
                tmpform = "<script>alert('請勾選要送簽的資料!!');window.history.go(-1);</script>";
            }
            else
            {
                string tmparolestampid = "'" + Session["rid"].ToString() + "'";
                string tmprole = "";
                string tmpbillid = "";
                string allsno = "";         
                string[] condtionArr = cdel.Split(',');
                int condtionLen = condtionArr.Length;
                for (int i = 0; i < condtionLen; i++)
                {

                    #region 寄信參數
                    string bccemail = "";                
                    string tmpsno = "";  //請款單號
                    string tmpdate = "";//申請日期        
                    string tmpnote = "";//摘要說明
                    string tmpownman = "";//申請人
                    string tomail = ""; 
                    #endregion

                    string pid = condtionArr[i].Trim();

                    #region 找值並異動資料庫

                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {
                        custpurchase modobj = con.custpurchase.Where(r => r.pid == pid).FirstOrDefault();
                        string tmpmoney = modobj.pallmoney.ToString();

                        //找出下一個角色是誰
                        string impallstring = dbobj.getnewcheck1("M", tmparolestampid, tmparolestampid, tmpmoney, "", "", "");
                        string[] tmpstrarr = impallstring.Split(';');
                        tmprole = tmpstrarr[0].ToString();
                        tmpbillid = tmpstrarr[1].ToString();
                        if (tmprole != "")
                        {
                            #region 寄信參數值
                            //===============               
                            System.Data.SqlClient.SqlConnection tmpconn1 = dbobj.get_conn("Aitag_DBContext");                        
                            tmpsno = modobj.pdno;
                            tmpdate = dbobj.get_date(modobj.pdate.ToString(), "1");
                            // tmpvendno = modobj.vendno + "－" + dbobj.get_dbvalue(tmpconn1, "select comsttitle from allcompany where comid = '" + modobj.vendno + "'");
                            // tmpamoney = modobj.totalmoney.ToString();
                            tmpnote = modobj.pcomment;
                            tmpownman = modobj.ownman;
                            bccemail = dbobj.get_dbvalue(tmpconn1, "select enemail from employee where empid = '" + tmpownman + "'");
                            tmpconn1.Close();
                            tmpconn1.Dispose();
                            //===============
                            #endregion

                            //呈核人員
                            //======================
                            //當不是代申請且未選擇(就是沒有下拉)申請呈核角色時，tmparolestampid就接arolestampid1的值

                            modobj.pstatus = "1";
                            modobj.arolestampid = tmparolestampid; //申請角色
                            modobj.rolestampid = tmprole; //下個呈核角色
                            modobj.rolestampidall = tmparolestampid; //呈核角色
                            modobj.empstampidall = "'" + Session["empid"].ToString() + "'"; //所有人員帳號
                            modobj.billtime = DateTime.Now.ToString(); //所有時間
                            modobj.billflowid = int.Parse(tmpbillid);
                            modobj.bmodid = Session["empid"].ToString();
                            modobj.bmoddate = DateTime.Now;
                            con.Entry(modobj).State = EntityState.Modified;
                            con.SaveChanges();
                            con.Dispose();

                            #region  寄信
                            string txt_comment = "";
                            string mailtitle = "";
                            string mailcontext = "";

                            mailtitle = "媒體預算分配表作業申請要求審核通知";
                            txt_comment = "有一筆申請單提出，請至系統進行審核作業。<br>資料如下：";

                            mailcontext += "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=450 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                            mailcontext += "<tr>";
                            mailcontext += "<td colspan=2>" + txt_comment + "</td>";
                            mailcontext += "</tr>";
                    
                            mailcontext += "<tr>";
                            mailcontext += "<td align=right width='90'>採購(委刊單)號：</td>";
                            mailcontext += "<td>" + tmpsno + "</td>";
                            mailcontext += "</tr>";

                            mailcontext += "<tr>";
                            mailcontext += "<td align=right>申請日期：</td>";
                            mailcontext += "<td>" + tmpdate + "</td>";
                            mailcontext += "</tr>";

                            mailcontext += "<tr>";
                            mailcontext += "<td align=right>申請人：</td>";
                            mailcontext += "<td>" + tmpownman + "</td>";
                            mailcontext += "</tr>";


                            mailcontext += "<tr>";
                            mailcontext += "<td align=right>摘要說明：</td>";
                            mailcontext += "<td>" + tmpnote + "</td>";
                            mailcontext += "</tr>";

                            mailcontext += "</table>";
                            mailcontext += "<br><font size='9pt' color=#404040>此為系統自動發信，請勿直接回覆！</font>";

                            //寄給下一個審核者
                            System.Data.SqlClient.SqlConnection tmpconn2 = dbobj.get_conn("Aitag_DBContext");
                            SqlDataReader dr;
                            SqlCommand sqlsmd = new SqlCommand();
                            sqlsmd.Connection = tmpconn2;
                            string sqlstr = "select employee.enemail FROM emprole INNER JOIN employee ON emprole.empid =employee.empid where emprole.rid =" + tmprole + " and employee.enemail <>''";
                            sqlsmd.CommandText = sqlstr;
                            dr = sqlsmd.ExecuteReader();

                            while (dr.Read())
                            {
                                tomail += dr["enemail"].ToString() + ",";
                            }
                            dbobj.send_mailfile("", tomail, mailtitle, mailcontext, null, null);
                            dr.Close();
                            dr.Dispose();
                            sqlsmd.Dispose();

                            tmpconn2.Close();
                            tmpconn2.Dispose();


                            #endregion

                            allsno += "【" + modobj.pdno + "】、";
                        }
                        else
                        { errmsg += "【" + modobj.pdno + "】、"; }


                    }

                    #endregion
                }

                #region 系統LOG檔

                if (allsno != "")
                {
                    allsno = allsno.Substring(0, allsno.Length - 1);
                    string sysrealsid = Request["sysrealsid"].ToString();
                    //系統LOG檔
                    //================================================= //                  
                    SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2") + "(群送)";
                    string sysflag = "A";
                    string sysnote = "單號：" + allsno;
                    dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    sysconn.Close();
                    sysconn.Dispose();
                    //======================================================          
                }
                #endregion

                if (errmsg != "")
                {
                    errmsg = errmsg.Substring(0, errmsg.Length - 1);
                    tmpform = "<body onload=javascript:alert('以下單號：" + errmsg + "無設定單據的呈核流程，請先至表單流程設定中設定!!');qfr1.submit();>";
                }
                else
                { tmpform = "<body onload=javascript:alert('送出成功!!');qfr1.submit();>"; }
                tmpform += "<form name='qfr1' action='/custpurchase/budgetlist' method='post'>";
                tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                tmpform += "</form>";
                tmpform += "</body>";
            }

            return new ContentResult() { Content = @"" + tmpform };
        }

        public ActionResult chk(int? page, string orderdata, string orderdata1)
        {
            IPagedList<custpurchase> result;
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;

            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "pid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qpdno = "", qallcomid = "", qpcomment = "", qspdate = "", qepdate = "";

            if (!string.IsNullOrWhiteSpace(Request["qpdno"]))
            {
                qpdno = Request["qpdno"].Trim();
                ViewBag.qpdno = qpdno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qallcomid"]))
            {
                qallcomid = Request["qallcomid"].Trim();
                ViewBag.qallcomid = qallcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qpcomment"]))
            {
                qpcomment = Request["qpcomment"].Trim();
                ViewBag.qpcomment = qpcomment;
            }
            if (!string.IsNullOrWhiteSpace(Request["qspdate"]))
            {
                qspdate = Request["qspdate"].Trim();
                ViewBag.qspdate = qspdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qepdate"]))
            {
                qepdate = Request["qepdate"].Trim();
                ViewBag.qevadate = qepdate;
            }

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "";
                sqlstr = "select * from custpurchase where pstatus='1' and replace(rolestampid,'''','')='" + Session["rid"].ToString() + "'  and";
               
                if (qpdno != "")
                    sqlstr += " pdno like '%" + qpdno + "%'  and";
                if (qallcomid != "")
                    sqlstr += " allcomid in (select comid from allcompany where comid like '%" + qallcomid + "%' or comtitle like '%" + qallcomid + "%')   and";
                if (qpcomment != "")
                    sqlstr += " pcomment like '%" + qpcomment + "%'  and";
                if (qspdate != "")
                    sqlstr += " pdate >= '" + qspdate + "'  and";
                if (qepdate != "")
                    sqlstr += " pdate <= '" + qepdate + "'  and";


                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.custpurchase.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<custpurchase>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);

        }

        public ActionResult chkEdit()
        {
            ViewBag.pid = Request["pid"].ToString();

            if (!string.IsNullOrWhiteSpace(Request["qpdno"]))
            {

                ViewBag.qpdno = Request["qpdno"];
            }
            if (!string.IsNullOrWhiteSpace(Request["qallcomid"]))
            {

                ViewBag.qvendno = Request["qallcomid"];
            }
            if (!string.IsNullOrWhiteSpace(Request["qpcomment"]))
            {

                ViewBag.qvcomment = Request["qpcomment"];
            }
            if (!string.IsNullOrWhiteSpace(Request["qspdate"]))
            {

                ViewBag.qspdate = Request["qspdate"];
            }
            if (!string.IsNullOrWhiteSpace(Request["qepdate"]))
            {

                ViewBag.qevadate = Request["qepdate"];
            }

            return View();
        }

        public ActionResult chkEditdo(string sysflag, int? page, string orderdata, string orderdata1)
        {

            string pid = Request["pid"].ToString().Trim();

            NDcommon dbobj = new NDcommon();
            
            string pstatus = Request["pstatus"].ToString();
            string tmpstatus = "";
            string tmprback = Request["rback"].ToString();

            string tmprolestampid = Request["rolestampid"].ToString(); //目前簽核角色(一個)           
            string billflowid = Request["billflowid"].ToString();
            string rolea_1 = Request["rolestampidall"].ToString();
            string roleall = "";
            roleall = rolea_1 + "," + tmprolestampid; //簽核過角色(多個)
            string tmprole = "";

            #region 寄信參數      
            string bccemail = "";      
            string tmpsno = "";  //請款單號
            string tmpdate = "";//申請日期
            // string tmpvendno = "";//廠商
            // string tmpamoney = "";//請款金額
            string tmpnote = "";//摘要說明
            string tmpownman = "";//申請人
            string tomail = "";
            #endregion
            if (pstatus == "1")
            {
                #region  通過時

                //找出下一個角色是誰

                tmprole = dbobj.getnewcheck1("M", tmprolestampid, roleall, "0", "", billflowid);

                if (tmprole == "")
                {
                    return new ContentResult() { Content = @"<body onload=javascript:alert('請先至表單流程設定中設定呈核單據的呈核流程!');window.history.go(-1);>" };
                }

                if (tmprole == "'topman'")
                {
                    tmpstatus = "2";//'己簽核
                }
                else
                {
                    #region
                    //找往上呈核長管級數
                    //==========================               
                    SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext");
                    string tmpflowlevel = dbobj.get_dbvalue(conn1, "select flowlevel from billflow where bid=" + billflowid);
                    conn1.Close();
                    conn1.Dispose();

                    if (tmpflowlevel == "")
                    { tmpflowlevel = "0"; }

                    string[] tmpa = rolea_1.Split(',');

                    int tmpacount = tmpa.Length;

                    //if cint(tmpflowlevel) = cint(tmpacount+1) 
                    if (int.Parse(tmpflowlevel.ToString()) == tmpacount + 1)
                    {
                        tmprole = "'topman'";
                        tmpstatus = "2";//'己簽核
                    }
                    else
                    { tmpstatus = "1"; }

                    //==========================                
                    #endregion

                }
                #endregion
            }
            else
            {
                tmpstatus = pstatus;
            }
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                #region 寄信參數值
                //===============
                custpurchase modobj = con.custpurchase.Where(r => r.pid == pid).FirstOrDefault();
                System.Data.SqlClient.SqlConnection tmpconn1 = dbobj.get_conn("Aitag_DBContext");
             
                tmpsno = modobj.pdno;
                tmpdate = dbobj.get_date(modobj.pdate.ToString(), "1");
                //tmpvendno = modobj.vendno +"－" + dbobj.get_dbvalue(tmpconn1, "select comsttitle from allcompany where comid = '" + modobj.vendno + "'");
                //tmpamoney = modobj.totalmoney.ToString();
                tmpnote = modobj.pcomment;
                tmpownman = modobj.ownman;   
                bccemail = dbobj.get_dbvalue(tmpconn1, "select enemail from employee where empid = '" + tmpownman + "'");
                tmpconn1.Close();
                tmpconn1.Dispose();
                //===============
                #endregion
                //呈核人員
                //======================             
                modobj.pstatus = tmpstatus;
                if (pstatus == "1")
                { modobj.rolestampid = tmprole;  //下個呈核角色

                modobj.rolestampidall = roleall; //所有呈核角色
                modobj.empstampidall = modobj.empstampidall + ",'" + Session["empid"].ToString() + "'"; //所有人員帳號
                modobj.billtime = modobj.billtime + "," + DateTime.Now.ToString(); //所有時間               
                modobj.bmodid = Session["empid"].ToString();
                modobj.bmoddate = DateTime.Now;
                }

                if (pstatus == "D")
                {
                    #region  退件時
                    if (modobj.backrolestampidall != null && modobj.backrolestampidall != "")
                    {
                        modobj.backrolestampidall = modobj.backrolestampidall + "," + tmprolestampid;
                        modobj.backempstampidall = modobj.backempstampidall + ",'" + Session["empid"].ToString() + "'"; //所有人員帳號
                        modobj.backbilltime = modobj.backbilltime + "," + DateTime.Now.ToString();
                    }
                    else
                    {
                        modobj.backrolestampidall = tmprolestampid;
                        modobj.backempstampidall = Session["empid"].ToString() + "'"; //所有人員帳號
                        modobj.backbilltime = DateTime.Now.ToString();
                    }


                    //原因
                    if (tmprback != "")
                    {
                        if (modobj.rback != null && modobj.rback != "")
                        {
                            modobj.rback = modobj.rback + "," + tmprback;
                        }
                        else
                        { modobj.rback = tmprback; }
                    }

                    #endregion
                }
                else
                {
                    if (pstatus == "3")
                    {
                        #region 3 退回補正

                        if (modobj.modbackrolestampidall != null && modobj.modbackrolestampidall != "")
                        {
                            modobj.modbackrolestampidall = modobj.modbackrolestampidall + "," + tmprolestampid;
                            modobj.modbackempstampidall = modobj.modbackempstampidall + ",'" + Session["empid"].ToString() + "'"; //所有人員帳號
                            modobj.modbackbilltime = modobj.modbackbilltime + "," + DateTime.Now.ToString();
                        }
                        else
                        {
                            modobj.modbackrolestampidall = tmprolestampid;
                            modobj.modbackempstampidall = Session["empid"].ToString() + "'"; //所有人員帳號
                            modobj.modbackbilltime = DateTime.Now.ToString();
                        }


                        //原因
                        if (tmprback != "")
                        {
                            if (modobj.modback != null && modobj.modback != "")
                            {
                                modobj.modback = modobj.modback + "," + tmprback;
                            }
                            else
                            { modobj.modback = tmprback; }
                        }
                        #endregion
                    }
                }
                con.Entry(modobj).State = EntityState.Modified;
                con.SaveChanges();
                con.Dispose();
            }


            #region  寄信
            string msgerr = "";
            string txt_comment = "";
            string mailtitle = "";
            string mailcontext = "";

            switch (tmpstatus)
            {
                case "1"://審核中      
                    mailtitle = "媒體預算分配表申請要求審核通知";
                    txt_comment = "有一筆申請單提出，請至系統進行審核作業。<br>資料如下：";
                    break;
                case "2"://已審核        
                    mailtitle = "媒體預算分配表申請完成審核通知";
                    txt_comment = "您的申請單已通過審核。<br>資料如下：";
                    break;
                case "3"://退回補正   
                    mailtitle = "媒體預算分配表申請退回補正通知";
                    txt_comment = "您的申請單有問題，審核者要求您修正。<br>資料如下：";
                    break;
                case "D"://退回      
                    mailtitle = "媒體預算分配表申請退回通知";
                    txt_comment = "您的申請單被退回。<br>資料如下：";
                    break;
            }

            mailcontext += "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=450 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
            mailcontext += "<tr>";
            mailcontext += "<td colspan=2>" + txt_comment + "</td>";
            mailcontext += "</tr>";


            mailcontext += "<tr>";
            mailcontext += "<td align=right width='90'>採購(委刊單)號：</td>";
            mailcontext += "<td>" + tmpsno + "</td>";
            mailcontext += "</tr>";

            mailcontext += "<tr>";
            mailcontext += "<td align=right>申請日期：</td>";
            mailcontext += "<td>" + tmpdate + "</td>";
            mailcontext += "</tr>";

            mailcontext += "<tr>";
            mailcontext += "<td align=right>申請人：</td>";
            mailcontext += "<td>" + tmpownman + "</td>";
            mailcontext += "</tr>";

            //mailcontext += "<tr>";
            //mailcontext += "<td align=right>請款金額：</td>";
            //mailcontext += "<td>" + tmpamoney + "</td>";
            //mailcontext += "</tr>";

            mailcontext += "<tr>";
            mailcontext += "<td align=right>摘要說明：</td>";
            mailcontext += "<td>" + tmpnote + "</td>";
            mailcontext += "</tr>";

            if (!string.IsNullOrWhiteSpace(tmprback))
            {
                tmprback = tmprback.Replace("\r\n", "<br>");
                mailcontext += "<tr>";
                mailcontext += "<td align=right>原因：</td>";
                mailcontext += "<td>" + tmprback + "</td>";
                mailcontext += "</tr>";
            }

            mailcontext += "</table>";
            mailcontext += "<br><br><font size='9pt' color=#404040>此為系統自動發信，請勿直接回覆！</font>";


            if (tmpstatus == "1")
            {  //寄給下一個審核者
                System.Data.SqlClient.SqlConnection tmpconn2 = dbobj.get_conn("Aitag_DBContext");
                SqlDataReader dr;
                SqlCommand sqlsmd = new SqlCommand();
                string sqlstr = "";
                sqlsmd.Connection = tmpconn2;
                sqlstr = "select employee.enemail FROM emprole INNER JOIN employee ON emprole.empid =employee.empid where emprole.rid =" + tmprole + " and employee.enemail <>''";
                sqlsmd.CommandText = sqlstr;
                dr = sqlsmd.ExecuteReader();

                while (dr.Read())
                {
                    tomail += dr["enemail"].ToString() + ",";
                }
                dbobj.send_mailfile("", tomail, mailtitle, mailcontext, null, null);
                dr.Close();
                dr.Dispose();
                sqlsmd.Dispose();

                tmpconn2.Close();
                tmpconn2.Dispose();
            }
            else
            {
                dbobj.send_mailfile("", bccemail, mailtitle, mailcontext, null, null);
            }


            // if (dbobj.send_mailfile("", bccemail, mailtitle, mailcontext, null,null) == "err")
            //{
            //    msgerr = "<script>alert('Email通知有誤,請確認收件者郵件" + bccemail + "是否正確!!');window.history.go(-1);</script>";                      
            //}

            #endregion

            string tmpform = "";
            if (msgerr != "")
            { tmpform = msgerr; }
            else
            {
                tmpform = "<body onload=javascript:alert('送出審核成功!!');qfr1.submit();>";
                tmpform += "<form name='qfr1' action='/custpurchase/chk' method='post'>";
                tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                tmpform += "</form>";
                tmpform += "</body>";
            }




            return new ContentResult() { Content = @"" + tmpform };
        }

        public ActionResult chkall(string sysflag, int? page, string orderdata, string orderdata1)
        {
            NDcommon dbobj = new NDcommon();
            string cdel = Request["cdel"];
            string tmpform = "";

            if (string.IsNullOrWhiteSpace(cdel))
            {
                tmpform = "<script>alert('請勾選要送簽的資料!!');window.history.go(-1);</script>";
            }
            else
            {
                SqlConnection conn0 = dbobj.get_conn("AitagBill_DBContext");
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn0;
                string sqlstr = "";
                //string sqlstr = "select * from vend_contractinv_det where vcinvid in (" + cdel + ") and (itemcode = '' or itemcode is null)";
                //cmd.CommandText = sqlstr;
                //SqlDataReader alldr = cmd.ExecuteReader();
                //if (alldr.HasRows)
                //{
                //    return new ContentResult() { Content = @"<body onload=javascript:alert('表單內分攤明細之歸帳代號並未設定!!');window.history.go(-1);>" };
                //}
                //alldr.Close();
                //alldr.Dispose();
                sqlstr = "select * from custpurchase where pid in (" + cdel + ")";
                cmd.CommandText = sqlstr;
                SqlDataReader alldr = cmd.ExecuteReader();
                while (alldr.Read())
                {
                    string pid = alldr["pid"].ToString().Trim();
                    string pstatus = Request["pstatus"].ToString();
                    string tmpstatus = "";

                    string tmprolestampid = Request["rolestampid"].ToString(); //目前簽核角色(一個)           
                    string billflowid = alldr["billflowid"].ToString();
                    string rolea_1 = alldr["rolestampidall"].ToString();
                    string roleall = "";
                    roleall = rolea_1 + "," + tmprolestampid; //簽核過角色(多個)
                    string tmprole = "";

                    #region 寄信參數
                    string bccemail = "";
                    string tmpsno = "";  //請款單號
                    string tmpdate = "";//申請日期
                    // string tmpvendno = "";//廠商
                    // string tmpamoney = "";//請款金額
                    string tmpnote = "";//摘要說明
                    string tmpownman = "";//申請人
                    string tomail = "";
                    #endregion
                    if (pstatus == "1")
                    {
                        #region  通過時

                        //找出下一個角色是誰

                        tmprole = dbobj.getnewcheck1("M", tmprolestampid, roleall, "0", "", billflowid);

                        if (tmprole == "")
                        {
                            return new ContentResult() { Content = @"<body onload=javascript:alert('請先至表單流程設定中設定呈核單據的呈核流程!');window.history.go(-1);>" };
                        }

                        if (tmprole == "'topman'")
                        {
                            tmpstatus = "2";//'己簽核
                        }
                        else
                        {
                            #region
                            //找往上呈核長管級數
                            //==========================               
                            SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext");
                            string tmpflowlevel = dbobj.get_dbvalue(conn1, "select flowlevel from billflow where bid=" + billflowid);
                            conn1.Close();
                            conn1.Dispose();

                            if (tmpflowlevel == "")
                            { tmpflowlevel = "0"; }

                            string[] tmpa = rolea_1.Split(',');

                            int tmpacount = tmpa.Length;

                            //if cint(tmpflowlevel) = cint(tmpacount+1) 
                            if (int.Parse(tmpflowlevel.ToString()) == tmpacount + 1)
                            {
                                tmprole = "'topman'";
                                tmpstatus = "2";//'己簽核
                            }
                            else
                            { tmpstatus = "1"; }

                            //==========================                
                            #endregion

                        }
                        #endregion
                    }
                    else
                    {
                        tmpstatus = pstatus;
                    }
                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {
                        #region 寄信參數值
                        //===============
                        custpurchase modobj = con.custpurchase.Where(r => r.pid == pid).FirstOrDefault();
                        System.Data.SqlClient.SqlConnection tmpconn1 = dbobj.get_conn("Aitag_DBContext");

                        tmpsno = modobj.pdno;
                        tmpdate = dbobj.get_date(modobj.pdate.ToString(), "1");
                        //tmpvendno = modobj.vendno +"－" + dbobj.get_dbvalue(tmpconn1, "select comsttitle from allcompany where comid = '" + modobj.vendno + "'");
                        //tmpamoney = modobj.totalmoney.ToString();
                        tmpnote = modobj.pcomment;
                        tmpownman = modobj.ownman;
                        bccemail = dbobj.get_dbvalue(tmpconn1, "select enemail from employee where empid = '" + tmpownman + "'");
                        tmpconn1.Close();
                        tmpconn1.Dispose();
                        //===============
                        #endregion
                        //呈核人員
                        //======================             
                        modobj.pstatus = tmpstatus;
                        if (pstatus == "1")
                        { modobj.rolestampid = tmprole; } //下個呈核角色

                        modobj.rolestampidall = roleall; //所有呈核角色
                        modobj.empstampidall = modobj.empstampidall + ",'" + Session["empid"].ToString() + "'"; //所有人員帳號
                        modobj.billtime = modobj.billtime + "," + DateTime.Now.ToString(); //所有時間                  
                        modobj.bmodid = Session["empid"].ToString();
                        modobj.bmoddate = DateTime.Now;

                        con.Entry(modobj).State = EntityState.Modified;
                        con.SaveChanges();
                        con.Dispose();
                    }

                    #region  寄信
                    string msgerr = "";
                    string txt_comment = "";
                    string mailtitle = "";
                    string mailcontext = "";

                    switch (tmpstatus)
                    {
                        case "1"://審核中      
                            mailtitle = "媒體預算分配表申請要求審核通知";
                            txt_comment = "有一筆申請單提出，請至系統進行審核作業。<br>資料如下：";
                            break;
                        case "2"://已審核        
                            mailtitle = "媒體預算分配表申請完成審核通知";
                            txt_comment = "您的申請單已通過審核。<br>資料如下：";
                            break;

                    }

                    mailcontext += "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=450 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                    mailcontext += "<tr>";
                    mailcontext += "<td colspan=2>" + txt_comment + "</td>";
                    mailcontext += "</tr>";

                    mailcontext += "<tr>";
                    mailcontext += "<td align=right width='90'>採購(委刊單)號：</td>";
                    mailcontext += "<td>" + tmpsno + "</td>";
                    mailcontext += "</tr>";


                    mailcontext += "<tr>";
                    mailcontext += "<td align=right>申請日期：</td>";
                    mailcontext += "<td>" + tmpdate + "</td>";
                    mailcontext += "</tr>";

                    mailcontext += "<tr>";
                    mailcontext += "<td align=right>申請人：</td>";
                    mailcontext += "<td>" + tmpownman + "</td>";
                    mailcontext += "</tr>";

                    //mailcontext += "<tr>";
                    //mailcontext += "<td align=right>請款金額：</td>";
                    //mailcontext += "<td>" + tmpamoney + "</td>";
                    //mailcontext += "</tr>";

                    mailcontext += "<tr>";
                    mailcontext += "<td align=right>摘要說明：</td>";
                    mailcontext += "<td>" + tmpnote + "</td>";
                    mailcontext += "</tr>";


                    mailcontext += "</table>";
                    mailcontext += "<br><br><font size='9pt' color=#404040>此為系統自動發信，請勿直接回覆！</font>";


                    if (tmpstatus == "1")
                    {  //寄給下一個審核者
                        System.Data.SqlClient.SqlConnection tmpconn2 = dbobj.get_conn("Aitag_DBContext");
                        SqlDataReader dr;
                        SqlCommand sqlsmd = new SqlCommand();
                        sqlsmd.Connection = tmpconn2;
                        sqlstr = "select employee.enemail FROM emprole INNER JOIN employee ON emprole.empid =employee.empid where emprole.rid =" + tmprole + " and employee.enemail <>''";
                        sqlsmd.CommandText = sqlstr;
                        dr = sqlsmd.ExecuteReader();

                        while (dr.Read())
                        {
                            tomail += dr["enemail"].ToString() + ",";
                        }
                        dbobj.send_mailfile("", tomail, mailtitle, mailcontext, null, null);
                        dr.Close();
                        dr.Dispose();
                        sqlsmd.Dispose();

                        tmpconn2.Close();
                        tmpconn2.Dispose();
                    }
                    else
                    {
                        dbobj.send_mailfile("", bccemail, mailtitle, mailcontext, null, null);
                    }



                    #endregion

                }
                alldr.Close();
                alldr.Dispose();
                conn0.Close();
                conn0.Dispose();

                //string tmpform = "";

                tmpform = "<body onload=javascript:alert('送出審核成功!!');qfr1.submit();>";
                tmpform += "<form name='qfr1' action='/custpurchase/chk' method='post'>";
                tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                tmpform += "</form>";
                tmpform += "</body>";
            }

            return new ContentResult() { Content = @"" + tmpform };
        }

        #region  預算轉檔



        public ActionResult Transfer(string sysflag, vend_contractdet col, HttpPostedFileBase upfile)
        {
            ViewBag.vcid = Request["vcid"].ToString();

            if (sysflag != "A")
            {
                return View();
            }
            else
            {
                Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
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
                        fileName = "cust-" + sernonum.ToString() + fileName;
                        upfile.SaveAs(Server.MapPath("~/upload/") + fileName);

                        string tmppath = BasicPath + fileName;


                        StreamReader sr = new StreamReader(@tmppath, System.Text.Encoding.Default);
                        string allstr = sr.ReadToEnd(); //從資料流末端存取檔案
                        sr.Close();

                        string[] tmpstridno; //匯入資料
                        string[] toptmparry; //匯入的第一筆資料(欄位)
                        int tmparrycount = 0; //匯入欄位數
                        tmpstridno = allstr.Split(System.Environment.NewLine.ToCharArray());

                        //找第一筆的欄位數
                        toptmparry = tmpstridno[0].Split(',');

                        for (int tmpi = 0; tmpi <= toptmparry.Length - 1; tmpi++)
                        {
                            if (toptmparry[tmpi] != "")
                            {
                                tmparrycount++;
                            }
                        }

                        SqlConnection conn = dbobj.get_conn("Aitag_DBContext");
                        string[] tmparry;
                        int vitemno = 0;
                        for (int i = 1; i <= tmpstridno.Length - 1; i++)
                        {
                            if (tmpstridno[i] != "")
                            {
                                #region

                                inputcount++;
                                tmparry = tmpstridno[i].Split(',');
                                //判斷必填欄位
                                if (tmparry[0] != "" && tmparry[1] != "")
                                {
                                    // 品項編號0	品項名稱1	單位2	數量3	單價4	金額5	備註6
                                    vitemno = vitemno + 2;
                                    col.vcid = int.Parse(ViewBag.vcid);
                                    col.bdprodno = tmparry[0];
                                    col.bdprodtitle = tmparry[1];
                                    col.vitemno = vitemno;
                                    col.pdunit = tmparry[2];
                                    col.vccount = int.Parse(tmparry[3]);
                                    col.vcmoney = int.Parse(tmparry[4]);
                                    col.vcallmoney = int.Parse(tmparry[5]);
                                    col.vcdcomment = tmparry[6];

                                    col.comid = Session["comid"].ToString();
                                    col.bmodid = Session["tempid"].ToString();
                                    col.bmoddate = DateTime.Now;
                                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                                    {
                                        con.vend_contractdet.Add(col);
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

                                }
                                #endregion
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
                @"parent.opener.location.href = '/custcontract/detlist?vcid=" + ViewBag.vcid + "&sid=" + Request["sid"].ToString() + "&realsid=" + Request["realsid"].ToString() + "';" +
                @"window.close();";


                return View();
            }



        }

        public ActionResult budgetTransfer(string sysflag, conbudgetdet col, HttpPostedFileBase upfile)
        {
            ViewBag.pid = Request["pid"].ToString();

            if (sysflag != "A")
            {
                return View();
            }
            else
            {
                Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
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
                        fileName = "cust-" + sernonum.ToString() + fileName;
                        upfile.SaveAs(Server.MapPath("~/upload/") + fileName);

                        string tmppath = BasicPath + fileName;


                        StreamReader sr = new StreamReader(@tmppath, System.Text.Encoding.Default);
                        string allstr = sr.ReadToEnd(); //從資料流末端存取檔案
                        sr.Close();

                        string[] tmpstridno; //匯入資料
                        string[] toptmparry; //匯入的第一筆資料(欄位)
                        int tmparrycount = 0; //匯入欄位數
                        tmpstridno = allstr.Split(System.Environment.NewLine.ToCharArray());

                        //找第一筆的欄位數
                        toptmparry = tmpstridno[0].Split(',');

                        for (int tmpi = 0; tmpi <= toptmparry.Length - 1; tmpi++)
                        {
                            if (toptmparry[tmpi] != "")
                            {
                                tmparrycount++;
                            }
                        }

                        SqlConnection conn = dbobj.get_conn("Aitag_DBContext");
                        string[] tmparry;
                        int vitemno = 0;
                        for (int i = 1; i <= tmpstridno.Length - 1; i++)
                        {
                            if (tmpstridno[i] != "")
                            {
                                #region

                                inputcount++;
                                tmparry = tmpstridno[i].Split(',');
                                //判斷必填欄位
                                if (tmparry[0] != "" && tmparry[1] != "")
                                {
                                    // 品項編號0	品項名稱1	單位2	數量3	單價4	金額5	備註6
                                    vitemno = vitemno + 2;
                                    col.pid = decimal.Parse(ViewBag.pid);
                                    col.bdprodno = tmparry[0];
                                    col.bdprodtitle = tmparry[1];
                                    col.itemno = vitemno;
                                    col.punit = tmparry[2];
                                    col.pcount = int.Parse(tmparry[3]);
                                    col.pmoney = int.Parse(tmparry[4]);
                                    col.psummoney = int.Parse(tmparry[5]);
                                    col.pcomment = tmparry[6];

                                    col.comid = Session["comid"].ToString();
                                    col.bmodid = Session["tempid"].ToString();
                                    col.bmoddate = DateTime.Now;
                                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                                    {
                                        con.conbudgetdet.Add(col);
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

                                }
                                #endregion
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
                @"parent.opener.location.href = '/custcontract/budgetlist?vcid=" + ViewBag.vcid + "&sid=" + Request["sid"].ToString() + "&realsid=" + Request["realsid"].ToString() + "';" +
                @"window.close();";


                return View();
            }



        }


        public ActionResult conbudgetlist()
        {
            string tmppid = "";
            tmppid = Request["pid"].ToString();
            ViewBag.pid = tmppid.ToString();

            List<conbudgetdet> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                var query = con.conbudgetdet.AsQueryable();
                result = query.Where(r => r.pid == tmppid).AsQueryable().ToList();

            }
            return View(result);

        }

        public ActionResult conbudgetdetdo(string sysflag, int? page, string orderdata, string orderdata1)
        {
            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            SqlConnection erpconn = dbobj.get_conn("AitagBill_DBContext");
           
            decimal allmoney = 0;
            string pid = "";
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {

                page = ((!page.HasValue || page < 1) ? 1 : page);
                ViewBag.page = page;
                ViewBag.orderdata = orderdata;
                ViewBag.orderdata1 = orderdata1;
                string sqlstr = "";
                string cdel1 = Request["cdid"];

               // string ctno1 = Request["ctno"];
                string ctname1 = Request["ctname"];
                string bdprodno1 = Request["bdprodno"];
             //   string bdprodtitle1 = Request["bdprodtitle"];
                string vendcomid1 = Request["vendcomid"];
                string psdate1 = Request["psdate"];
                string pedate1 = Request["pedate"];
                //string mdno1 = Request["mdno"];
                //string mdcomment1 = Request["mdcomment"];
                string punit1 = Request["punit"];
                string pcount1 = Request["pcount"];
                string pmoney1 = Request["pmoney"];
                string psummoney1 = Request["psummoney"];
                string pcomment1 = Request["pcomment"];
                string ctpercent1 = Request["ctpercent"];
                string acmoney1 = Request["acmoney"];
                string varmoney1 = Request["varmoney"];
                string moneyrate1 = Request["moneyrate"];

                string[] cdelarr = cdel1.Split(',');
                //string[] ctnoarr = ctno1.Split(',');
                string[] ctnamearr = ctname1.Split(',');
                string[] bdprodnoarr = bdprodno1.Split(',');
            //    string[] bdprodtitlearr = bdprodtitle1.Split(',');
               string[] vendcomidarr = vendcomid1.Split(',');
                string[] psdatearr = psdate1.Split(',');
                string[] pedatearr = pedate1.Split(',');
                // string[] mdnoarr = mdno1.Split(',');
                // string[] mdcommentarr = mdcomment1.Split(',');
          //      string[] punitarr = punit1.Split(',');
             //   string[] pcountarr = pcount1.Split(',');
             //   string[] pmoneyarr = pmoney1.Split(',');
                string[] psummoneyarr = psummoney1.Split(',');
                string[] acmoneyarr = acmoney1.Split(',');
                string[] varmoneyarr = varmoney1.Split(',');
                string[] moneyratearr = moneyrate1.Split(',');
          //      string[] ctpercentarr = ctpercent1.Split(',');
                string[] pcommentarr = pcomment1.Split(',');
                string projno = Request["pdno"]; 
                pid = Request["pid"].ToString();
                int pitemno = 10;
                for (int i = 0; i < cdelarr.Length; i++)
                {
                    if (cdelarr[i].Trim() == "")
                    {
                        if (!(bdprodnoarr[i].Trim() == ""))
                        {
                            string mcno = dbobj.get_dbvalue(erpconn, "select mcno from mediachannel where mdno = '" + bdprodnoarr[i].Trim() + "'");
                            conbudgetdet addobj = new conbudgetdet();
                            addobj.projno = projno;
                            addobj.pid = pid;
                            //addobj.vcno = Request["vcno"].ToString();
                            addobj.comid = Request["comid"].ToString();
                            addobj.ctno = mcno ;
                            addobj.ctname = ctnamearr[i].Trim();
                            addobj.bdprodno = bdprodnoarr[i].Trim();
                      //      addobj.bdprodtitle = bdprodtitlearr[i].Trim();
                            addobj.vendcomid = vendcomidarr[i].Trim();
                            addobj.psdate = DateTime.Parse(psdatearr[i].Trim());
                            addobj.pedate = DateTime.Parse(pedatearr[i].Trim());
                            // addobj.mdno = mdnoarr[i].Trim();
                            // addobj.mdcomment = mdcommentarr[i].Trim();

                            addobj.itemno = pitemno;
                   //         addobj.punit = punitarr[i].Trim();
                            addobj.pcount = 1;
                            addobj.pmoney = Decimal.Parse(psummoneyarr[i].ToString());
                            addobj.psummoney = Decimal.Parse(psummoneyarr[i].ToString());
                            addobj.acmoney = Decimal.Parse(acmoneyarr[i].ToString());
                            addobj.varmoney = Decimal.Parse(varmoneyarr[i].ToString());
                            addobj.moneyrate = Decimal.Parse(moneyratearr[i].ToString());
                   //         addobj.ctpercent = Decimal.Parse(ctpercentarr[i].ToString());
                            addobj.pcomment = pcommentarr[i].Trim();
                            //addobj.projno = Request["projno"].ToString();

                            addobj.bmodid = Session["empid"].ToString();
                            addobj.bmoddate = DateTime.Now;
                            allmoney += Decimal.Parse(psummoneyarr[i].ToString()); 

                            con.conbudgetdet.Add(addobj);
                            con.SaveChanges();
                            pitemno = pitemno + 10;
                        }
                    }
                    else
                    {
                        //修改
                        int cdid = int.Parse(cdelarr[i].Trim());
                        string mcno = dbobj.get_dbvalue(erpconn, "select mcno from mediachannel where mdno = '" + bdprodnoarr[i].Trim() + "'");
                        conbudgetdet modobj = con.conbudgetdet.Where(r => r.cdid == cdid).FirstOrDefault();
                        modobj.projno = projno;
                        modobj.ctno = mcno ;
                        modobj.ctname = ctnamearr[i].Trim();
                        modobj.bdprodno = bdprodnoarr[i].Trim();
                        modobj.vendcomid = vendcomidarr[i].Trim();
                        modobj.psdate = DateTime.Parse(psdatearr[i].Trim());
                        modobj.pedate = DateTime.Parse(pedatearr[i].Trim());
                   //    modobj.bdprodtitle = bdprodtitlearr[i].Trim();
                        //  modobj.mdno = mdnoarr[i].Trim();
                        //  modobj.mdcomment = mdcommentarr[i].Trim();
               //        modobj.punit = punitarr[i].Trim();
                        modobj.pcount = 1;
                        modobj.pmoney = Decimal.Parse(psummoneyarr[i].ToString());
                        modobj.psummoney = Decimal.Parse(psummoneyarr[i].ToString());
                        modobj.acmoney = Decimal.Parse(acmoneyarr[i].ToString());
                        modobj.varmoney = Decimal.Parse(varmoneyarr[i].ToString());
                        modobj.moneyrate = Decimal.Parse(moneyratearr[i].ToString());
                        modobj.pcomment = pcommentarr[i].Trim();
                 //       modobj.ctpercent = Decimal.Parse(ctpercentarr[i].ToString());
                        modobj.bmodid = Session["empid"].ToString();
                        modobj.bmoddate = DateTime.Now;
                        allmoney += Decimal.Parse(psummoneyarr[i].ToString()); 

                        con.Entry(modobj).State = EntityState.Modified;
                        con.SaveChanges();
                    }
                    
                }
                string tmpsql = "update custpurchase set pallmoney = " + allmoney + " , pmoney = " + allmoney + " where pid = " + pid;
                dbobj.dbexecute("AitagBill_DBContext", tmpsql);
                con.Dispose();
                erpconn.Close();
                erpconn.Dispose();


            }

            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/custpurchase/conbudgetlist' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden name='pid' id='pid' value='" + pid + "'>";
            tmpform += "</form>";
            tmpform += "</body>";



            return new ContentResult() { Content = @"" + tmpform };
        }


        public ActionResult conbudgetdetdel(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string cdel = Request["cdel"];
            string pid = Request["pid"];
            string pdid = Request["pdid"];

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
                    decimal allmoney = 0;
                    for (int i = 0; i < condtionLen; i++)
                    {
                        string money1 = dbobj.get_dbvalue(conn1, "select ('專案編號' + pdno + ',品項' + bdprodno + ',金額' + convert(char,pallmoney)) as st1  from custpurchase where pdid = '" + condtionArr[i].ToString() + "'");
                        //allmoney = allmoney + decimal.Parse(dbobj.get_dbvalue(conn1, "select psummoney from conbudgetdet where cdid = '" + condtionArr[i].ToString() + "'"));
                        sysnote += money1 + "<br>";
                        //刪除明細資料
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM conbudgetdet where cdid = '" + condtionArr[i].ToString() + "'");

                    }

                    //string tmpsql = "update custpurchase set pallmoney = pallmoney - " + allmoney.ToString() + " , pmoney =  pmoney - " + allmoney.ToString() + " where pid = " + pid;
                   // dbobj.dbexecute("AitagBill_DBContext", tmpsql);

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
                    tmpform += "<form name='qfr1' action='/custpurchase/conbudgetlist' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden id='pid' name='pid' value='" + pid + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');</script>" + tmpform };


                }
            }
        }
        #endregion  預算轉檔

        #region  拋轉委刊單
        public ActionResult turndo(string sysflag, int? page, string orderdata, string orderdata1)
        {

            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;


            NDcommon dbobj = new NDcommon();
            SqlConnection conn = dbobj.get_conn("AitagBill_DBContext");
            string pid = Request["pid"].Trim();
           
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {


                #region 頭的值
                custpurchase mainobj = con.custpurchase.Where(r => r.pid == pid).FirstOrDefault();

                string projno = mainobj.pdno;
                purchase addobj = new purchase();
                string pdno = dbobj.get_billno(conn, "S", "", mainobj.comid, "", mainobj.pdate.ToString());
                string tmoppid = DateTime.Now.ToString("yyyyMMddhhmmssfffff");
                addobj.pid = tmoppid;
                addobj.pdate = mainobj.pdate;
                addobj.psdate = mainobj.psdate;
                addobj.pedate = mainobj.pedate;
                addobj.comid = mainobj.comid;
                addobj.projno = mainobj.pdno;
                addobj.prodid = mainobj.prodid ;
                addobj.pdno = pdno;
                addobj.pstatus = "0";
                addobj.taxtype = mainobj.taxtype;
                addobj.ownman = mainobj.ownman; 
                //產品編號
                addobj.prodid = mainobj.prodid;

                addobj.pmoney = mainobj.pmoney;
                addobj.ptaxmoney = mainobj.ptaxmoney;
                addobj.pallmoney = mainobj.pallmoney;

                addobj.allcomid = mainobj.allcomid;
                addobj.pcomment = mainobj.pcomment;

                addobj.bmodid = Session["empid"].ToString();
                addobj.bmoddate = DateTime.Now;
                con.purchase.Add(addobj);
                con.SaveChanges();

                #endregion


                #region 尾的值
                int pitemno = 10;

                string listsql = "";
                listsql = "select * from conbudgetdet where pid=" + pid + " order by itemno";

                SqlDataReader dr = dbobj.dbselect(conn, listsql);
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {

                        purchase_det addobjdet = new purchase_det();
                        addobjdet.pid = tmoppid;
                        addobjdet.pdno = pdno;
                        addobjdet.comid = dr["comid"].ToString();
                        addobjdet.projno = projno;
                        addobjdet.vendcomid = dr["vendcomid"].ToString();
                        addobjdet.pdsdate = DateTime.Parse(dr["psdate"].ToString());
                        addobjdet.pdedate = DateTime.Parse(dr["pedate"].ToString());
                        addobjdet.mdno = dr["bdprodno"].ToString().Trim();
                        addobjdet.mdcomment = dr["ctname"].ToString();

                        addobjdet.pitemno = pitemno;

                        addobjdet.pdcount = Decimal.Parse(dr["pcount"].ToString());
                        addobjdet.pdmoney = Decimal.Parse(dr["pmoney"].ToString());
                        addobjdet.pdallmoney = Decimal.Parse(dr["psummoney"].ToString());

                        addobjdet.pdcomment = dr["pcomment"].ToString();
                      //  addobjdet.projno ="";

                        addobjdet.bmodid = Session["empid"].ToString();
                        addobjdet.bmoddate = DateTime.Now;
                        addobjdet.mdcount = 0;
                        addobjdet.mdmoney = 0;
                        addobjdet.mdallmoney = 0;

                        con.purchase_det.Add(addobjdet);
                        con.SaveChanges();
                        pitemno = pitemno + 10;

                    }
                }
                dr.Close();
                dr.Dispose();

                #endregion


                conn.Close();
                conn.Dispose();

            }



            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/custpurchase/conbudgetlist' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden id='pid' name='pid' value='" + pid + "'>";
          
            tmpform += "</form>";
            tmpform += "</body>";


            return new ContentResult() { Content = @"" + tmpform };
        }

        #endregion

        public ActionResult quotelist(int? page, string orderdata, string orderdata1)
        {
            IPagedList<custpurchase> result;
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "pid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qpdno = "", qallcomid = "", qcampainetitle = "", qspdate = "", qepdate = "";

            if (!string.IsNullOrWhiteSpace(Request["qpdno"]))
            {
                qpdno = Request["qpdno"].Trim();
                ViewBag.qpdno = qpdno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qallcomid"]))
            {
                qallcomid = Request["qallcomid"].Trim();
                ViewBag.qvendno = qallcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcampainetitle"]))
            {
                qcampainetitle = Request["qcampainetitle"].Trim();
                ViewBag.qcampainetitle = qcampainetitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["qspdate"]))
            {
                qspdate = Request["qspdate"].Trim();
                ViewBag.qspdate = qspdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qepdate"]))
            {
                qepdate = Request["qepdate"].Trim();
                ViewBag.qevadate = qepdate;
            }

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "";
                sqlstr = "select * from custpurchase where comid = '" + Session["comid"].ToString() + "' and vendtype='2'  and";
                if (qpdno != "")
                    sqlstr += " pdno like '%" + qpdno + "%'  and";
                if (qallcomid != "")
                    sqlstr += " allcomid in (select comid from allcompany where comid like '%" + qallcomid + "%' or comtitle like '%" + qallcomid + "%')   and";
                if (qcampainetitle != "")
                    sqlstr += " campainetitle like '%" + qcampainetitle + "%'  and";
                if (qspdate != "")
                    sqlstr += " pdate >= '" + qspdate + "'  and";
                if (qepdate != "")
                    sqlstr += " pdate <= '" + qepdate + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.custpurchase.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<custpurchase>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);
        }

        public ActionResult quoteadd()
        {
            return View();
        }

        public ActionResult quoteadddo(string sysflag, int? page, string orderdata, string orderdata1)
        {

            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qpdno = "", qallcomid = "", qcampainetitle = "", qspdate = "", qepdate = "";

            if (!string.IsNullOrWhiteSpace(Request["qpdno"]))
            {
                qpdno = Request["qpdno"].Trim();
                ViewBag.qpdno = qpdno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qallcomid"]))
            {
                qallcomid = Request["qallcomid"].Trim();
                ViewBag.qvendno = qallcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcampainetitle"]))
            {
                qcampainetitle = Request["qcampainetitle"].Trim();
                ViewBag.qcampainetitle = qcampainetitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["qspdate"]))
            {
                qspdate = Request["qspdate"].Trim();
                ViewBag.qspdate = qspdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qepdate"]))
            {
                qepdate = Request["qepdate"].Trim();
                ViewBag.qevadate = qepdate;
            }


            NDcommon dbobj = new NDcommon();
            SqlConnection conn = dbobj.get_conn("AitagBill_DBContext");
            string pdno = "";
            string pid = "";
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {

                custpurchase addobj = new custpurchase();              
                pdno = dbobj.get_billno(conn, "Q", "", Session["comid"].ToString(), "", Request["pdate"].ToString());
                addobj.pid = DateTime.Now.ToString("yyyyMMddhhmmssfffff");
                pid = addobj.pid;
                addobj.vendtype = "2";
                addobj.pdate = DateTime.Parse(Request["pdate"].ToString());   
                addobj.comid = Session["comid"].ToString();
                addobj.projno = pdno;
                addobj.pdno = pdno;            
                addobj.pstatus = "0";
              
                addobj.ownman = Session["empid"].ToString();
                //產品編號
                addobj.prodid = Request["prodid"].ToString();
                addobj.wno = Request["wno"].ToString(); //工作卡號

                addobj.pmoney = decimal.Parse(Request["pmoney"]);//底價              
                addobj.acrate = decimal.Parse(Request["acrate"]);  //AC%
                addobj.adrate = decimal.Parse(Request["adrate"]);//調%
                addobj.acadjust = decimal.Parse(Request["acadjust"]);//調AC
                addobj.pspmoney = decimal.Parse(Request["pspmoney"]);//優惠
                addobj.allcomid = Request["allcomid"].Trim();//客戶
              
                addobj.campainetitle = Request["campainetitle"].Trim();//內容說明
                addobj.pcomment = Request["pcomment"].Trim(); //說明
                addobj.bmodid = Session["empid"].ToString();
                addobj.bmoddate = DateTime.Now;

                con.custpurchase.Add(addobj);

                con.SaveChanges();
                con.Dispose();

            }

            conn.Close();
            conn.Dispose();

            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/custpurchase/conbudgetlist2' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden id='qpdno' name='qpdno' value='" + qpdno + "'>";
            tmpform += "<input type=hidden id='qallcomid' name='qallcomid' value='" + qallcomid + "'>";
            tmpform += "<input type=hidden id='qcampainetitle' name='qcampainetitle' value='" + qcampainetitle + "'>";
            tmpform += "<input type=hidden id='qspdate' name='qspdate' value='" + qspdate + "'>";
            tmpform += "<input type=hidden id='qepdate' name='qepdate' value='" + qepdate + "'>";
            tmpform += "<input type=hidden id='pid' name='pid' value='" + pid + "'>";
            tmpform += "</form>";
            tmpform += "</body>";


            return new ContentResult() { Content = @"<script>alert('新增成功!!');</script>" + tmpform };
        }

        public ActionResult quoteEdit()
        {

            ViewBag.pid = Request["pid"].ToString();
            return View();
        }

        public ActionResult quoteEditdo(string sysflag, int? page, string orderdata, string orderdata1)
        {

            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qpdno = "", qallcomid = "", qcampainetitle = "", qspdate = "", qepdate = "";

            if (!string.IsNullOrWhiteSpace(Request["qpdno"]))
            {
                qpdno = Request["qpdno"].Trim();
                ViewBag.qpdno = qpdno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qallcomid"]))
            {
                qallcomid = Request["qallcomid"].Trim();
                ViewBag.qvendno = qallcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcampainetitle"]))
            {
                qcampainetitle = Request["qcampainetitle"].Trim();
                ViewBag.qcampainetitle = qcampainetitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["qspdate"]))
            {
                qspdate = Request["qspdate"].Trim();
                ViewBag.qspdate = qspdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qepdate"]))
            {
                qepdate = Request["qepdate"].Trim();
                ViewBag.qevadate = qepdate;
            }

            string pid = Request["pid"].ToString();
            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                custpurchase modobj = con.custpurchase.Where(r => r.pid == pid).FirstOrDefault();

                modobj.pdate = DateTime.Parse(Request["pdate"].ToString());             
                //產品編號
                modobj.prodid = Request["prodid"].ToString();
                modobj.wno = Request["wno"].ToString(); //工作卡號

                modobj.pmoney = decimal.Parse(Request["pmoney"]);//底價              
                modobj.acrate = decimal.Parse(Request["acrate"]);  //AC%
                modobj.adrate = decimal.Parse(Request["adrate"]);//調%
                modobj.acadjust = decimal.Parse(Request["acadjust"]);//調AC
                modobj.pspmoney = decimal.Parse(Request["pspmoney"]);//優惠
                modobj.allcomid = Request["allcomid"].Trim();//客戶
                modobj.campainetitle = Request["campainetitle"].Trim();//內容說明
                modobj.pcomment = Request["pcomment"].Trim(); //說明
                modobj.bmodid = Session["empid"].ToString();
                modobj.bmoddate = DateTime.Now;

                con.Entry(modobj).State = EntityState.Modified;
                con.SaveChanges();
                con.Dispose();
            }
            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/custpurchase/quotelist' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden id='qpdno' name='qpdno' value='" + qpdno + "'>";
            tmpform += "<input type=hidden id='qallcomid' name='qallcomid' value='" + qallcomid + "'>";
            tmpform += "<input type=hidden id='qcampainetitle' name='qcampainetitle' value='" + qcampainetitle + "'>";
            tmpform += "<input type=hidden id='qspdate' name='qspdate' value='" + qspdate + "'>";
            tmpform += "<input type=hidden id='qepdate' name='qepdate' value='" + qepdate + "'>";
            tmpform += "</form>";
            tmpform += "</body>";


            return new ContentResult() { Content = @"<script>alert('修改成功!!');</script>" + tmpform };
        }

        public ActionResult quoteDelete(string id, int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qpdno = "", qallcomid = "", qcampainetitle = "", qspdate = "", qepdate = "";

            if (!string.IsNullOrWhiteSpace(Request["qpdno"]))
            {
                qpdno = Request["qpdno"].Trim();
                ViewBag.qpdno = qpdno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qallcomid"]))
            {
                qallcomid = Request["qallcomid"].Trim();
                ViewBag.qvendno = qallcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcampainetitle"]))
            {
                qcampainetitle = Request["qcampainetitle"].Trim();
                ViewBag.qcampainetitle = qcampainetitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["qspdate"]))
            {
                qspdate = Request["qspdate"].Trim();
                ViewBag.qspdate = qspdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qepdate"]))
            {
                qepdate = Request["qepdate"].Trim();
                ViewBag.qevadate = qepdate;
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
                        string pdno = dbobj.get_dbvalue(conn1, "select pdno from custpurchase where pid='" + condtionArr[i].ToString() + "'");

                        sysnote += "單號:" + pdno + "<br>";
                        //刪除憑單
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM custpurchase where pid ='" + condtionArr[i].ToString() + "'");
                        //刪除明細
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM custpurchase_det where pid = '" + condtionArr[i].ToString() + "'");

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
                    tmpform += "<form name='qfr1' action='/custpurchase/quotelist' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qpdno' name='qpdno' value='" + qpdno + "'>";
                    tmpform += "<input type=hidden id='qallcomid' name='qallcomid' value='" + qallcomid + "'>";
                    tmpform += "<input type=hidden id='qcampainetitle' name='qcampainetitle' value='" + qcampainetitle + "'>";
                    tmpform += "<input type=hidden id='qspdate' name='qspdate' value='" + qspdate + "'>";
                    tmpform += "<input type=hidden id='qepdate' name='qepdate' value='" + qepdate + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');</script>" + tmpform };
                }
            }
        }


        public ActionResult conbudgetlist1()
        {
            string tmppid = "";
            tmppid = Request["pid"].ToString();
            ViewBag.pid = tmppid.ToString();

            List<conbudgetdet> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                var query = con.conbudgetdet.AsQueryable();
                result = query.Where(r => r.pid == tmppid).AsQueryable().ToList();

            }
            return View(result);

        }

        public ActionResult conbudgetdet1do(string sysflag, int? page, string orderdata, string orderdata1)
        {
            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
         
            string pid = "";
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {

                page = ((!page.HasValue || page < 1) ? 1 : page);
                ViewBag.page = page;
                ViewBag.orderdata = orderdata;
                ViewBag.orderdata1 = orderdata1;

                string cdel1 = Request["cdid"];
                string vendcomid1 = Request["vendcomid"];
                string bdprodtitle1 = Request["bdprodtitle"];
                string workitem1 = Request["workitem"];  
                string pcount1 = Request["pcount"];
                string pmoney1 = Request["pmoney"];
                string psummoney1 = Request["psummoney"];
                string varmoney1 = Request["varmoney"];
                string npmoney1 = Request["npmoney"];
                string npsummoney1 = Request["npsummoney"];
                string acmoney1 = Request["acmoney"];              
                string punit1 = Request["punit"];
                string wno1 = Request["wno"];

                string[] cdelarr = cdel1.Split(',');
                string[] vendcomidarr = vendcomid1.Split(',');
                string[] bdprodtitlearr = bdprodtitle1.Split(',');
                string[] workitemarr = workitem1.Split(',');
                string[] pcountarr = pcount1.Split(',');
                string[] pmoneyarr = pmoney1.Split(',');
                string[] psummoneyarr = psummoney1.Split(',');
                string[] varmoneyarr = varmoney1.Split(',');
                string[] npmoneyarr = npmoney1.Split(',');
                string[] npsummoneyarr = npsummoney1.Split(',');
                string[] acmoneyarr = acmoney1.Split(',');  
                string[] punitarr = punit1.Split(',');
                string[] wnoarr = wno1.Split(',');

  
                pid = Request["pid"].ToString();
                int pitemno = 10;
                for (int i = 0; i < cdelarr.Length; i++)
                {
                    if (cdelarr[i].Trim() == "")
                    {
                        if (!(vendcomidarr[i].Trim() == ""))
                        {
                         
                            conbudgetdet addobj = new conbudgetdet();
                            addobj.itemno = pitemno;
                            addobj.pid = pid;                      
                            addobj.comid = Request["comid"].ToString();
                            addobj.vendcomid = vendcomidarr[i].Trim();
                            addobj.bdprodtitle = bdprodtitlearr[i].Trim();
                            addobj.workitem = workitemarr[i].Trim();
                          
                            if (pcountarr[i].ToString() == "")
                            { addobj.pcount = 0; }
                            else
                            { addobj.pcount = Decimal.Parse(pcountarr[i].ToString()); }

                            if (pmoneyarr[i].ToString() == "")
                            { addobj.pmoney = 0; }
                            else
                            { addobj.pmoney = Decimal.Parse(pmoneyarr[i].ToString()); }

                            if (psummoneyarr[i].ToString() == "")
                            { addobj.psummoney = 0; }
                            else
                            { addobj.psummoney = Decimal.Parse(psummoneyarr[i].ToString()); }

                            if (acmoneyarr[i].ToString() == "")
                            { addobj.acmoney = 0; }
                            else
                            { addobj.acmoney = Decimal.Parse(acmoneyarr[i].ToString()); }

                            if (npmoneyarr[i].ToString() == "")
                            { addobj.npmoney = 0; }
                            else
                            { addobj.npmoney = Decimal.Parse(npmoneyarr[i].ToString()); }

                            if (npsummoneyarr[i].ToString() == "")
                            { addobj.npsummoney = 0; }
                            else
                            { addobj.npsummoney = Decimal.Parse(npsummoneyarr[i].ToString()); }

                            if (varmoneyarr[i].ToString() == "")
                            { addobj.varmoney = 0; }
                            else
                            { addobj.varmoney = Decimal.Parse(varmoneyarr[i].ToString()); }
                          
                            addobj.punit = punitarr[i].Trim();
                            addobj.wno = wnoarr[i].Trim();
                            addobj.vendtype = "2";
                            addobj.bmodid = Session["empid"].ToString();
                            addobj.bmoddate = DateTime.Now;
                      
                            con.conbudgetdet.Add(addobj);
                            con.SaveChanges();
                            pitemno = pitemno + 10;
                        }
                    }
                    else
                    {
                        //修改
                        int cdid = int.Parse(cdelarr[i].Trim());
                      
                        conbudgetdet modobj = con.conbudgetdet.Where(r => r.cdid == cdid).FirstOrDefault();
                        modobj.vendcomid = vendcomidarr[i].Trim();
                        modobj.bdprodtitle = bdprodtitlearr[i].Trim();
                        modobj.workitem = workitemarr[i].Trim();

                        if (pcountarr[i].ToString() == "")
                        { modobj.pcount = 0; }
                        else
                        { modobj.pcount = Decimal.Parse(pcountarr[i].ToString()); }


                        if (pmoneyarr[i].ToString() == "")
                        { modobj.pmoney = 0; }
                        else
                        { modobj.pmoney = Decimal.Parse(pmoneyarr[i].ToString()); }

                        if (psummoneyarr[i].ToString() == "")
                        { modobj.psummoney = 0; }
                        else
                        { modobj.psummoney = Decimal.Parse(psummoneyarr[i].ToString()); }

                        if (acmoneyarr[i].ToString() == "")
                        { modobj.acmoney = 0; }
                        else
                        { modobj.acmoney = Decimal.Parse(acmoneyarr[i].ToString()); }

                        if (npmoneyarr[i].ToString() == "")
                        { modobj.npmoney = 0; }
                        else
                        { modobj.npmoney = Decimal.Parse(npmoneyarr[i].ToString()); }

                        if (npsummoneyarr[i].ToString() == "")
                        { modobj.npsummoney = 0; }
                        else
                        { modobj.npsummoney = Decimal.Parse(npsummoneyarr[i].ToString()); }

                        if (varmoneyarr[i].ToString() == "")
                        { modobj.varmoney = 0; }
                        else
                        { modobj.varmoney = Decimal.Parse(varmoneyarr[i].ToString()); }
                        modobj.punit = punitarr[i].Trim();
                        modobj.wno = wnoarr[i].Trim();

                        con.Entry(modobj).State = EntityState.Modified;
                        con.SaveChanges();
                    }

                }
            
        
            }

            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/custpurchase/conbudgetlist1' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden name='pid' id='pid' value='" + pid + "'>";
            tmpform += "</form>";
            tmpform += "</body>";



            return new ContentResult() { Content = @"<script>alert('修改成功!!');</script>" + tmpform };
        }


        public ActionResult conbudgetdet1del(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string cdel = Request["cdel"];
            string pid = Request["pid"];
        
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

                        string pdno = dbobj.get_dbvalue(conn1, "select pdno from custpurchase where pid = '" + pid + "'");
                        string note = dbobj.get_dbvalue(conn1, "select ('廠商:' + vendcomid + ',項目:' + bdprodtitle + ',金額' + convert(char,psummoney)) as st1  from conbudgetdet where cdid = " + condtionArr[i].ToString() );

                        sysnote += "估價單號:" + pdno +","+ note + "<br>";
                        //刪除明細資料
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM conbudgetdet where cdid = '" + condtionArr[i].ToString() + "'");

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
                    tmpform += "<form name='qfr1' action='/custpurchase/conbudgetlist1' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden id='pid' name='pid' value='" + pid + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');</script>" + tmpform };


                }
            }
        }
    }
}
