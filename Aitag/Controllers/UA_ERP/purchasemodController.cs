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


namespace Aitag.Controllers
{
     [DoAuthorizeFilter]
    public class purchasemodController : BaseController
    {
      
        public ActionResult Index()
        {

            
            return View();
        }

        public ActionResult list(int? page, string orderdata, string orderdata1)
        {
            IPagedList<purchasemod> result;
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "pmid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qpdno = "", qallcomid = "", qpcomment = "", qspdate = "", qepdate = "" , projno = "";

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

            if (!string.IsNullOrWhiteSpace(Request["projno"]))
            {
                projno = Request["projno"].Trim();
                ViewBag.projno = projno;
            }

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "";
                sqlstr = "select * from purchasemod where comid = '" + Session["comid"].ToString() + "'  and";
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
                if (projno != "")
                    sqlstr += " projno = '" + projno + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.purchasemod.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<purchasemod>(page.Value - 1, (int)Session["pagesize"]);
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
            string projno = Request["projno"].ToString();
            string pserno = dbobj.get_dbvalue(conn, "select isnull(count(*),0) as count1 from purchasemod where projno = '" + projno + "' and comid = '" + Session["comid"] + "'"); 

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {


                #region 頭的值
                purchase mainobj = con.purchase.Where(r => r.projno == projno).FirstOrDefault();
                string moddate = Request["moddate"];
                //string projno = mainobj.pdno;
                purchasemod addobj = new purchasemod();
                string pdno = mainobj.pdno;

                string tmoppid = DateTime.Now.ToString("yyyyMMddhhmmssfffff");
                addobj.pmid = tmoppid;
                addobj.pdate = DateTime.Parse(moddate);
                addobj.psdate = mainobj.psdate;
                addobj.pedate = mainobj.pedate;
                addobj.comid = mainobj.comid;
                addobj.projno = mainobj.projno;
                //產品編號
                addobj.prodid = mainobj.prodid;
                addobj.pdno = pdno;
                addobj.pserno = int.Parse(pserno) + 1 ;
                addobj.pstatus = "0";
                addobj.taxtype = mainobj.taxtype;
                addobj.ownman = mainobj.ownman;
                
               // addobj.prodid = mainobj.prodid;

                addobj.pmoney = mainobj.pmoney;
                addobj.ptaxmoney = mainobj.ptaxmoney;
                addobj.pallmoney = mainobj.pallmoney;

                addobj.allcomid = mainobj.allcomid;
                addobj.pcomment = mainobj.pcomment;

                addobj.bmodid = Session["empid"].ToString();
                addobj.bmoddate = DateTime.Now;
                con.purchasemod.Add(addobj);
                con.SaveChanges();

                #endregion


                #region 尾的值
                int pitemno = 10;

                string listsql = "";
                listsql = "select * from purchase_det where projno = '" + projno + "' order by pitemno";

                SqlDataReader dr = dbobj.dbselect(conn, listsql);
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {

                        purchasemod_det addobjdet = new purchasemod_det();
                        addobjdet.pmid = tmoppid;
                        addobjdet.pdno = pdno;
                        addobjdet.comid = dr["comid"].ToString();
                        addobjdet.projno = projno;
                        addobjdet.vendcomid = dr["vendcomid"].ToString();
                        addobjdet.pdsdate = DateTime.Parse(dr["pdsdate"].ToString());
                        addobjdet.pdedate = DateTime.Parse(dr["pdedate"].ToString());
                        addobjdet.mdno = dr["mdno"].ToString().Trim();
                        addobjdet.mdcomment = dr["mdcomment"].ToString();

                        addobjdet.pitemno = pitemno;

                        addobjdet.pdcount = Decimal.Parse(dr["pdcount"].ToString()) + Decimal.Parse(dr["mdcount"].ToString());
                        addobjdet.pdmoney = Decimal.Parse(dr["pdmoney"].ToString()) + Decimal.Parse(dr["mdmoney"].ToString());
                        addobjdet.pdallmoney = Decimal.Parse(dr["pdallmoney"].ToString()) + Decimal.Parse(dr["mdallmoney"].ToString());

                        addobjdet.pdcomment = dr["pdcomment"].ToString();
                        //  addobjdet.projno ="";

                        addobjdet.bmodid = Session["empid"].ToString();
                        addobjdet.bmoddate = DateTime.Now;
                        addobjdet.mdcount = 0;
                        addobjdet.mdmoney = 0;
                        addobjdet.mdallmoney = 0;

                        con.purchasemod_det.Add(addobjdet);
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

            conn.Close();
            conn.Dispose();

            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/purchasemod/list' method='post'>";
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
                        string pdno = dbobj.get_dbvalue(conn1, "select pdno from purchasemod where pmid='" + condtionArr[i].ToString() + "'");

                        sysnote += "單號:" + pdno + "<br>";
                        //刪除憑單
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM purchasemod where pmid = '" + condtionArr[i].ToString() + "'");
                        //刪除明細
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM purchasemod_det where pmid = '" + condtionArr[i].ToString() + "'");
                      
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
                    tmpform += "<form name='qfr1' action='/purchasemod/list' method='post'>";
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

            ViewBag.pmid = Request["pmid"].ToString();
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

            string pmid = Request["pmid"].ToString().Trim();
            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                purchasemod modobj = con.purchasemod.Where(r => r.pmid == pmid).FirstOrDefault();

                modobj.pdate = DateTime.Parse(Request["pdate"].ToString());
                modobj.comid = Request["comid"];
                modobj.projno = Request["projno"];

                modobj.taxtype = Request["taxtype"];          
                modobj.pmoney = int.Parse(Request["pmoney"]);
                modobj.ptaxmoney = int.Parse(Request["ptaxmoney"]);
                modobj.pallmoney = int.Parse(Request["pallmoney"]);
                modobj.pcomment = Request["pcomment"];
                modobj.allcomid = Request["allcomid"];
                modobj.bmodid = Session["empid"].ToString();
                modobj.bmoddate = DateTime.Now;              
                con.Entry(modobj).State = EntityState.Modified;
                con.SaveChanges();
                con.Dispose();
            }
            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/purchasemod/list' method='post'>";
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

        public ActionResult detlist()
        {
            string tmppid = "";
            tmppid = Request["pmid"].Trim();
            ViewBag.pmid = tmppid.ToString();

            List<purchasemod_det> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                var query = con.purchasemod_det.AsQueryable();
                result = query.Where(r => r.pmid == tmppid).AsQueryable().ToList();

            }
            return View(result);
      
        }

        public ActionResult detlistdo(string sysflag, int? page, string orderdata, string orderdata1)
        {

            string pmid = "";
            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            decimal allmoney = 0;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {

                page = ((!page.HasValue || page < 1) ? 1 : page);
                ViewBag.page = page;
                ViewBag.orderdata = orderdata;
                ViewBag.orderdata1 = orderdata1;
                string sqlstr = "";
                string cdel1 = Request["pdid"];

                //string bdprodno1 = Request["bdprodno"];
                //string bdprodtitle1 = Request["bdprodtitle"];
                string vendcomid1 = Request["vendcomid"];
                string pdsdate1 = Request["pdsdate"];
                string pdedate1 = Request["pdedate"];
                string mdno1 = Request["mdno"];
                string mdcomment1 = Request["mdcomment"];
                //string pdunit1 = Request["pdunit"];
                //string pdcount1 = Request["pdcount"];
                //string pdmoney1 = Request["pdmoney"];
                string pdallmoney1 = Request["pdallmoney"];
                string pdcomment1 = Request["pdcomment"];
                string mdallmoney1 = Request["mdallmoney"];

                string[] cdelarr = cdel1.Split(',');
               // string[] bdprodnoarr = bdprodno1.Split(',');
               // string[] bdprodtitlearr = bdprodtitle1.Split(',');
                string[] vendcomidarr = vendcomid1.Split(',');
                string[] pdsdatearr = pdsdate1.Split(',');
                string[] pdedatearr = pdedate1.Split(',');
                string[] mdnoarr = mdno1.Split(',');
                string[] mdcommentarr = mdcomment1.Split(',');
               // string[] pdunitarr = pdunit1.Split(',');
               // string[] pdcountarr = pdcount1.Split(',');
               // string[] pdmoneyarr = pdmoney1.Split(',');
                string[] pdallmoneyarr = pdallmoney1.Split(',');
                string[] pdcommentarr = pdcomment1.Split(',');
                string[] mdallmoneyarr = mdallmoney1.Split(',');
                SqlConnection conn = dbobj.get_conn("AitagBill_DBContext");
                pmid = Request["pmid"].ToString();
                string projno = dbobj.get_dbvalue(conn, "select projno from purchasemod where pmid = '" + pmid + "'");
                int pitemno = 10;
                for (int i = 0; i < cdelarr.Length; i++)
                {
                    if (cdelarr[i].Trim() == "")
                    {
                        if (!(mdnoarr[i].Trim() == ""))
                        {
                                purchasemod_det addobj = new purchasemod_det();
                                addobj.pmid = pmid;
                                addobj.pdno = Request["pdno"].ToString();
                                addobj.comid = Request["comid"].ToString();                             

                               // addobj.bdprodno = bdprodnoarr[i].Trim();
                              //  addobj.bdprodtitle = bdprodtitlearr[i].Trim();
                                addobj.vendcomid = vendcomidarr[i].ToString();
                                addobj.pdsdate = DateTime.Parse(pdsdatearr[i].ToString());
                                addobj.pdedate = DateTime.Parse(pdedatearr[i].ToString());
                                addobj.mdno = mdnoarr[i].ToString().Trim();
                                addobj.mdcomment = mdcommentarr[i].Trim();

                                addobj.pitemno = pitemno;
                               // addobj.pdunit = pdunitarr[i].Trim();
                                addobj.pdcount = 1;
                                addobj.pdmoney = Decimal.Parse(pdallmoneyarr[i].ToString());
                                addobj.pdallmoney = Decimal.Parse(pdallmoneyarr[i].ToString());
                                addobj.mdallmoney = Decimal.Parse(mdallmoneyarr[i].ToString());
                                addobj.mdmoney = Decimal.Parse(mdallmoneyarr[i].ToString());
                                addobj.mdcount = 1;

                                addobj.pdcomment = pdcommentarr[i].Trim();
                                addobj.projno = projno;

                                addobj.bmodid = Session["empid"].ToString();
                                addobj.bmoddate = DateTime.Now;
                                allmoney += Decimal.Parse(pdallmoneyarr[i].ToString()); 
                                con.purchasemod_det.Add(addobj);
                                con.SaveChanges();

                                purchase_det addobj1 = new purchase_det();
                                addobj1.pid = dbobj.get_dbvalue(conn, "select pid from purchase where projno = '" + projno + "' and comid = '" + Session["comid"] + "'");
                                addobj1.pdno = dbobj.get_dbvalue(conn, "select pdno from purchase where projno = '" + projno + "' and comid = '" + Session["comid"] + "'");
                                addobj1.comid = Request["comid"].ToString();

                                // addobj.bdprodno = bdprodnoarr[i].Trim();
                                //  addobj.bdprodtitle = bdprodtitlearr[i].Trim();
                                addobj1.vendcomid = vendcomidarr[i].ToString();
                                addobj1.pdsdate = DateTime.Parse(pdsdatearr[i].ToString());
                                addobj1.pdedate = DateTime.Parse(pdedatearr[i].ToString());
                                addobj1.mdno = mdnoarr[i].ToString().Trim();
                                addobj1.mdcomment = mdcommentarr[i].Trim();

                                addobj1.pitemno = pitemno;
                                // addobj.pdunit = pdunitarr[i].Trim();
                                addobj1.pdcount = 1;
                                addobj1.pdmoney = 0;
                                addobj1.pdallmoney = 0;
                                addobj1.mdallmoney = Decimal.Parse(mdallmoneyarr[i].ToString());
                                addobj1.mdmoney = Decimal.Parse(mdallmoneyarr[i].ToString());
                                addobj1.mdcount = 1;

                                addobj1.pdcomment = pdcommentarr[i].Trim();
                                addobj1.projno = projno;

                                addobj1.bmodid = Session["empid"].ToString();
                                addobj1.bmoddate = DateTime.Now;
                                allmoney += Decimal.Parse(pdallmoneyarr[i].ToString()) + Decimal.Parse(addobj1.mdallmoney.ToString()); 
                                con.purchase_det.Add(addobj1);
                                con.SaveChanges();

                                pitemno = pitemno + 10;
                        }
                    }
                    else
                    {
                        //修改
                        int pdid = int.Parse(cdelarr[i].Trim());
                        purchasemod_det modobj = con.purchasemod_det.Where(r => r.pmdid == pdid).FirstOrDefault();


                     //   modobj.bdprodno = bdprodnoarr[i].Trim();
                     //   modobj.bdprodtitle = bdprodtitlearr[i].Trim();
                        modobj.vendcomid = vendcomidarr[i].ToString();
                        modobj.pdsdate = DateTime.Parse(pdsdatearr[i].ToString());
                        modobj.pdedate = DateTime.Parse(pdedatearr[i].ToString());
                        modobj.mdno = mdnoarr[i].ToString().Trim();
                        modobj.mdcomment = mdcommentarr[i].Trim();
                     //   modobj.pdunit = pdunitarr[i].Trim();
                        modobj.pdcount = 1;
                        modobj.pdmoney = Decimal.Parse(pdallmoneyarr[i].ToString());
                        modobj.pdallmoney = Decimal.Parse(pdallmoneyarr[i].ToString());
                        modobj.pdcomment = pdcommentarr[i].Trim();
                        modobj.mdallmoney = Decimal.Parse(mdallmoneyarr[i].ToString());
                        modobj.mdmoney = Decimal.Parse(mdallmoneyarr[i].ToString());
                        modobj.mdcount = 1;
                        modobj.bmodid = Session["empid"].ToString();
                        modobj.bmoddate = DateTime.Now;
                        allmoney += Decimal.Parse(pdallmoneyarr[i].ToString()) + Decimal.Parse(modobj.mdallmoney.ToString()); 

                        con.Entry(modobj).State = EntityState.Modified;
                        con.SaveChanges();

                        dbobj.dbexecute("AitagBill_DBContext", "update purchase_det set mdallmoney = mdallmoney + " + mdallmoneyarr[i].ToString() + " ,  mdmoney = mdmoney + " + mdallmoneyarr[i].ToString() + " where projno = '" + projno + "' and mdno = '" + mdnoarr[i].ToString().Trim() + "' and comid = '" + Session["comid"] + "'");
                    }
                    
                }
                string tmpsql = "update purchasemod set pallmoney = " + allmoney + ", pmoney = " + allmoney + " where pmid = " + pmid;
                dbobj.dbexecute("AitagBill_DBContext", tmpsql);

                tmpsql = "update purchase set pallmoney = " + allmoney + ", pmoney = " + allmoney + " where projno = '" + projno + "' and comid = '" + Session["comid"] + "'";
                dbobj.dbexecute("AitagBill_DBContext", tmpsql);
                con.Dispose();


            }

            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/purchasemod/detlist' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden name='pmid' id='pmid' value='" + pmid + "'>";        
            tmpform += "</form>";
            tmpform += "</body>";

 

            return new ContentResult() { Content = @"" + tmpform };
        }

        public ActionResult detdel(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);        
            string cdel = Request["cdel"];
            string pmid = Request["pmid"];

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
                        string money1 = dbobj.get_dbvalue(conn1, "select ('採購(委刊單)號' + pdno + ',品項' + bdprodno + ',金額' + convert(char,pdallmoney)) as st1  from purchasemod_det where pdid = '" + condtionArr[i].ToString() + "'");

                        sysnote += money1 + "<br>";                    
                        //刪除明細資料
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM purchasemod_det where pdid = '" + condtionArr[i].ToString() + "'");

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
                    tmpform += "<form name='qfr1' action='/purchasemod/detlist' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden id='pmid' name='pmid' value='" + pmid + "'>";               
                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');</script>" + tmpform };

               
                }
            }
        }

        public ActionResult workflowdo(string sysflag, int? page, string orderdata, string orderdata1)
        {
            string tmparolestampid = "";
            string tmprole = "";
            string tmpbillid = "";
            NDcommon dbobj = new NDcommon();

            string pmid = Request["pmid"].ToString().Trim();
       
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
                purchasemod modobj = con.purchasemod.Where(r => r.pmid == pmid).FirstOrDefault();

                if (Request["pstatus"].ToString() == "0")  //第一次送時,補件修正不用
                {
                    if (Request["arolestampid"].ToString() != "")
                        tmparolestampid = "'" + Request["arolestampid"].ToString() + "'";
                    else
                        tmparolestampid = "'" + Request["arolestampid1"].ToString() + "'";

                    string tmpmoney = Request["pallmoney"].ToString().Replace(",", "");
                    //找出下一個角色是誰               
                    string impallstring = dbobj.getnewcheck1("S", tmparolestampid, tmparolestampid, tmpmoney, "", "", "");

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

            mailtitle = "媒體委刊單申請要求審核通知" + tmpmtitle;
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
            tmpform += "<form name='qfr1' action='/purchasemod/list' method='post'>";
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

                    string pmid = condtionArr[i].Trim();

                    #region 找值並異動資料庫

                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {
                        purchasemod modobj = con.purchasemod.Where(r => r.pmid == pmid).FirstOrDefault();
                        string tmpmoney = modobj.pallmoney.ToString();

                        //找出下一個角色是誰
                        string impallstring = dbobj.getnewcheck1("S", tmparolestampid, tmparolestampid, tmpmoney, "", "", "");
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

                            mailtitle ="媒體委刊單申請要求審核通知";
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
                tmpform += "<form name='qfr1' action='/purchasemod/list' method='post'>";
                tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                tmpform += "</form>";
                tmpform += "</body>";
            }

            return new ContentResult() { Content = @"" + tmpform };
        }

        public ActionResult chklist(int? page, string orderdata, string orderdata1)
        {
            IPagedList<purchasemod> result;
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;

            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "pmid"; }

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

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "";
                sqlstr = "select * from purchasemod where pstatus='1' and replace(rolestampid,'''','')='" + Session["rid"].ToString() + "'  and";
               
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

                var query = con.purchasemod.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<purchasemod>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);

        }

        public ActionResult chkEdit()
        {
            ViewBag.pmid = Request["pmid"].ToString();

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

            string pmid = Request["pmid"].ToString().Trim();

            NDcommon dbobj = new NDcommon();
            //SqlConnection conn0 = dbobj.get_conn("AitagBill_DBContext");
            //SqlCommand cmd = new SqlCommand();
            //cmd.Connection = conn0;
            //string sqlstr = "select * from vend_contractinv_det where vcinvid in (" + vcinvid + ") and (itemcode = '' or itemcode is null)";
            //cmd.CommandText = sqlstr;
            //SqlDataReader alldr = cmd.ExecuteReader();
            //if (alldr.HasRows)
            //{
            //    return new ContentResult() { Content = @"<body onload=javascript:alert('表單內分攤明細之歸帳代號並未設定!!');window.history.go(-1);>" };
            //}
            //alldr.Close();
            //alldr.Dispose();

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

                tmprole = dbobj.getnewcheck1("S", tmprolestampid, roleall, "0", "", billflowid);

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
                purchasemod modobj = con.purchasemod.Where(r => r.pmid == pmid).FirstOrDefault();
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
                    mailtitle = "媒體委刊單申請要求審核通知";
                    txt_comment = "有一筆申請單提出，請至系統進行審核作業。<br>資料如下：";
                    break;
                case "2"://已審核        
                    mailtitle ="媒體委刊單申請完成審核通知";
                    txt_comment = "您的申請單已通過審核。<br>資料如下：";
                    break;
                case "3"://退回補正   
                    mailtitle ="媒體委刊單申請退回補正通知";
                    txt_comment = "您的申請單有問題，審核者要求您修正。<br>資料如下：";
                    break;
                case "D"://退回      
                    mailtitle = "媒體委刊單申請退回通知";
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
                tmpform += "<form name='qfr1' action='/purchasemod/chklist' method='post'>";
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
                sqlstr = "select * from purchasemod where pmid in (" + cdel + ")";
                cmd.CommandText = sqlstr;
                SqlDataReader alldr = cmd.ExecuteReader();
                while (alldr.Read())
                {
                    string pmid = alldr["pmid"].ToString().Trim();
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

                        tmprole = dbobj.getnewcheck1("S", tmprolestampid, roleall, "0", "", billflowid);

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
                        purchasemod modobj = con.purchasemod.Where(r => r.pmid == pmid).FirstOrDefault();
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
                            mailtitle = "媒體委刊單申請要求審核通知";
                            txt_comment = "有一筆申請單提出，請至系統進行審核作業。<br>資料如下：";
                            break;
                        case "2"://已審核        
                            mailtitle = "媒體委刊單申請完成審核通知";
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
                tmpform += "<form name='qfr1' action='/purchasemod/chklist' method='post'>";
                tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                tmpform += "</form>";
                tmpform += "</body>";
            }

            return new ContentResult() { Content = @"" + tmpform };
        }
    }
}
