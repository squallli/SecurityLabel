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
    public class vend_contractinvcloseController : BaseController
    {
      
        public ActionResult Index()
        {

            
            return View();
        }

        public ActionResult list(int? page, string orderdata, string orderdata1)
        {
            IPagedList<vend_contractinvclose> result;
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "vcinvid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qvcno = "", qvendno = "", qvcomment = "", qsvadate = "", qevadate = "";

            if (!string.IsNullOrWhiteSpace(Request["qvcno"]))
            {
                qvcno = Request["qvcno"].Trim();
                ViewBag.qvcno = qvcno;
            }

            //客戶Campaine
            if (!string.IsNullOrWhiteSpace(Request["qvendno"]))
            {
                qvendno = Request["qvendno"].Trim();
                ViewBag.qvendno = qvendno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcomment"]))
            {
                qvcomment = Request["qvcomment"].Trim();
                ViewBag.qvcomment = qvcomment;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsvadate"]))
            {
                qsvadate = Request["qsvadate"].Trim();
                ViewBag.qsvadate = qsvadate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qevadate"]))
            {
                qevadate = Request["qevadate"].Trim();
                ViewBag.qvadate = qevadate;
            }

           

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "";
                sqlstr = "select * from vend_contractinvclose where comid = '" + Session["comid"].ToString() + "' and vserno > 0   and";
                if (qvcno != "")
                    sqlstr += " vcno like '%" + qvcno + "%'  and";

                //客戶Campaine

                if (qvendno != "")
                    sqlstr += " projno in (select pdno from view_custpurchase where comsttitle like '%" + qvendno + "%' or campainetitle like '%" + qvendno + "%')   and";             
                if (qvcomment != "")
                    sqlstr += " vcomment like '%" + qvcomment + "%'  and";
                if (qsvadate != "")
                    sqlstr += " vadate >= '" + qsvadate + "'  and";
                if (qevadate != "")
                    sqlstr += " vadate <= '" + qevadate + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.vend_contractinvclose.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<vend_contractinvclose>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);
        }

        public ActionResult prelist(int? page, string orderdata, string orderdata1)
        {
            IPagedList<vend_contractinvclose> result;
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "vcinvid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qvcno = "", qvendno = "", qvcomment = "", qsvadate = "", qevadate = "";

            if (!string.IsNullOrWhiteSpace(Request["qvcno"]))
            {
                qvcno = Request["qvcno"].Trim();
                ViewBag.qvcno = qvcno;
            }

            //客戶Campaine
            if (!string.IsNullOrWhiteSpace(Request["qvendno"]))
            {
                qvendno = Request["qvendno"].Trim();
                ViewBag.qvendno = qvendno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcomment"]))
            {
                qvcomment = Request["qvcomment"].Trim();
                ViewBag.qvcomment = qvcomment;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsvadate"]))
            {
                qsvadate = Request["qsvadate"].Trim();
                ViewBag.qsvadate = qsvadate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qevadate"]))
            {
                qevadate = Request["qevadate"].Trim();
                ViewBag.qvadate = qevadate;
            }



            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "";
                sqlstr = "select * from vend_contractinvclose where comid = '" + Session["comid"].ToString() + "' and vserno = 0   and";
                if (qvcno != "")
                    sqlstr += " vcno like '%" + qvcno + "%'  and";

                //客戶Campaine

                if (qvendno != "")
                    sqlstr += " projno in (select pdno from view_custpurchase where comsttitle like '%" + qvendno + "%' or campainetitle like '%" + qvendno + "%')   and";
                if (qvcomment != "")
                    sqlstr += " vcomment like '%" + qvcomment + "%'  and";
                if (qsvadate != "")
                    sqlstr += " vadate >= '" + qsvadate + "'  and";
                if (qevadate != "")
                    sqlstr += " vadate <= '" + qevadate + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.vend_contractinvclose.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<vend_contractinvclose>(page.Value - 1, (int)Session["pagesize"]);
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

            string qvcno = "", qvendno = "", qvcomment = "", qsvadate = "", qevadate = "";

            if (!string.IsNullOrWhiteSpace(Request["qvcno"]))
            {
                qvcno = Request["qvcno"].Trim();
                ViewBag.qvcno = qvcno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvendno"]))
            {
                qvendno = Request["qvendno"].Trim();
                ViewBag.qvendno = qvendno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcomment"]))
            {
                qvcomment = Request["qvcomment"].Trim();
                ViewBag.qvcomment = qvcomment;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsvadate"]))
            {
                qsvadate = Request["qsvadate"].Trim();
                ViewBag.qsvadate = qsvadate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qevadate"]))
            {
                qevadate = Request["qevadate"].Trim();
                ViewBag.qvadate = qevadate;
            }


            NDcommon dbobj = new NDcommon();
            SqlConnection conn = dbobj.get_conn("AitagBill_DBContext");
            string pid = Request["pid"].Trim();

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {

                System.Data.SqlClient.SqlConnection tmpconn = dbobj.get_conn("AitagBill_DBContext");
                System.Data.SqlClient.SqlConnection erpconn = dbobj.get_conn("AitagBill_DBContext");

                #region 頭的值              
                        purchase mainobj = con.purchase.Where(r => r.pid == pid).FirstOrDefault();

                        //count 
                        string vserno = dbobj.get_dbvalue(tmpconn, "select count(*) as tmp1 from vend_contractinvclose where vcno = '" + mainobj.pdno + "'");
                        vserno = (int.Parse(vserno) + 1).ToString();
                        vend_contractinvclose addobj = new vend_contractinvclose();
                        string vcinvid = DateTime.Now.ToString("yyyyMMddhhmmssfffff");
                        addobj.slyear = int.Parse(Request["slyear"].ToString());
                        addobj.slmonth = int.Parse(Request["slmonth"].ToString());
                        addobj.vcinvid = vcinvid;
                        addobj.vcno = mainobj.pdno;
                        addobj.vserno = int.Parse(vserno);
                        addobj.vadate = DateTime.Parse(Request["vadate"].ToString());
                        addobj.comid = mainobj.comid;
                        addobj.projno = mainobj.projno;          
                        addobj.vstatus = "0";
                        addobj.ownman = mainobj.ownman;    
                        addobj.totalmoney = mainobj.pallmoney;
                        addobj.vendno = mainobj.allcomid;
                        addobj.vcomment = mainobj.pcomment;
                        addobj.bmodid = Session["empid"].ToString();
                        addobj.bmoddate = DateTime.Now;
                        addobj.prodid = mainobj.prodid;

                        con.vend_contractinvclose.Add(addobj);
                        con.SaveChanges();     
                #endregion 

                       
                        #region 尾的值
                        decimal allmoney = 0;
                        string listsql = "",tmpsql="";
                        listsql = "select * from purchase_det where pid=" + pid + " order by pitemno";

                        SqlDataReader dr = dbobj.dbselect(tmpconn, listsql);
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {

                                vend_contractinvclose_det addobjdet = new vend_contractinvclose_det();
                                addobjdet.itemno = int.Parse(dr["pitemno"].ToString());
                                addobjdet.vcinvid = vcinvid;
                                addobjdet.vcno = mainobj.pdno;
                                addobjdet.vserno = int.Parse(vserno);

                                string ftotalmoney = "0";
                                tmpsql = "select isnull(sum(psummoney),0) as psummoney from vend_contractinvclose_det where vcno = '" + mainobj.pdno.ToString() + "' and bdprodno='" + dr["mdno"].ToString() + "' and vserno>0 and comid = '" + Session["comid"] + "'";
                                ftotalmoney = dbobj.get_dbvalue(erpconn, tmpsql);

                                addobjdet.pcount =1;
                                decimal tmpallmoney = Decimal.Parse(dr["pdallmoney"].ToString()) + Decimal.Parse(dr["mdallmoney"].ToString());
                                allmoney += tmpallmoney - decimal.Parse(ftotalmoney);
                                addobjdet.pmoney = tmpallmoney - decimal.Parse(ftotalmoney);
                                addobjdet.psummoney = tmpallmoney - decimal.Parse(ftotalmoney);
                                addobjdet.remoney = 0;
                                addobjdet.remoneypki = 0;
                                addobjdet.projno = dr["projno"].ToString();
                                addobjdet.vendcomid = dr["vendcomid"].ToString();
                                addobjdet.pdsdate = DateTime.Parse(dr["pdsdate"].ToString());
                                addobjdet.pdedate = DateTime.Parse(dr["pdedate"].ToString());

                                addobjdet.bdprodno = dr["mdno"].ToString();
                                addobjdet.bdprodtitle = dr["mdcomment"].ToString();

                                addobjdet.pcomment = dr["pdcomment"].ToString();
                                addobjdet.comid = mainobj.comid;
                                addobjdet.bmodid = Session["empid"].ToString();
                                addobjdet.bmoddate = DateTime.Now;
                                addobjdet.vendac = 0;
                                addobjdet.payvendcomid = "";

                                tmpsql = "select mdsummary from cust_contractinvclose_det where vcno = '" + mainobj.pdno.ToString() + "' and bdprodno='" + dr["mdno"].ToString() + "' and vserno=" + addobjdet.vserno + " and comid = '" + Session["comid"] + "'";
                                addobjdet.mdsummary = dbobj.get_dbvalue(erpconn, tmpsql);
                                tmpsql = "select closetype from cust_contractinvclose_det where vcno = '" + mainobj.pdno.ToString() + "' and bdprodno='" + dr["mdno"].ToString() + "' and vserno=" + addobjdet.vserno + " and comid = '" + Session["comid"] + "'";
                                addobjdet.closetype = dbobj.get_dbvalue(erpconn, tmpsql);

                                con.vend_contractinvclose_det.Add(addobjdet);
                                con.SaveChanges();     
                            }
                         }
                        dr.Close();
                        dr.Dispose();
                   
                       #endregion

                        tmpsql = "update vend_contractinvclose set totalmoney = " + allmoney + " where vcinvid = " + vcinvid;
                        dbobj.dbexecute("AitagBill_DBContext", tmpsql);
                        
                        erpconn.Close();
                        erpconn.Dispose();
                        tmpconn.Close();
                        tmpconn.Dispose();
        
           }

     

            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/vend_contractinvclose/list' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden id='qvcno' name='qvcno' value='" + qvcno + "'>";
            tmpform += "<input type=hidden id='qvendno' name='qvendno' value='" + qvendno + "'>";
            tmpform += "<input type=hidden id='qvcomment' name='qvcomment' value='" + qvcomment + "'>";
            tmpform += "<input type=hidden id='qsvadate' name='qsvadate' value='" + qsvadate + "'>";
            tmpform += "<input type=hidden id='qevadate' name='qevadate' value='" + qevadate + "'>";
            tmpform += "</form>";
            tmpform += "</body>";

      
            return new ContentResult() { Content = @"" + tmpform };
        }

        public ActionResult preadd()
        {
            return View();
        }

        public ActionResult preadddo(string sysflag, int? page, string orderdata, string orderdata1)
        {

            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qvcno = "", qvendno = "", qvcomment = "", qsvadate = "", qevadate = "";

            if (!string.IsNullOrWhiteSpace(Request["qvcno"]))
            {
                qvcno = Request["qvcno"].Trim();
                ViewBag.qvcno = qvcno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvendno"]))
            {
                qvendno = Request["qvendno"].Trim();
                ViewBag.qvendno = qvendno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcomment"]))
            {
                qvcomment = Request["qvcomment"].Trim();
                ViewBag.qvcomment = qvcomment;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsvadate"]))
            {
                qsvadate = Request["qsvadate"].Trim();
                ViewBag.qsvadate = qsvadate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qevadate"]))
            {
                qevadate = Request["qevadate"].Trim();
                ViewBag.qvadate = qevadate;
            }


            NDcommon dbobj = new NDcommon();
            SqlConnection conn = dbobj.get_conn("AitagBill_DBContext");
            string pid = Request["pid"].Trim();
            string perrate = Request["perrate"];

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {

                System.Data.SqlClient.SqlConnection tmpconn = dbobj.get_conn("AitagBill_DBContext");

                #region 頭的值
                purchase mainobj = con.purchase.Where(r => r.pid == pid).FirstOrDefault();

                //count 
               // string vserno = dbobj.get_dbvalue(tmpconn, "select count(*) as tmp1 from vend_contractinvclose where vcno = '" + mainobj.pdno + "'");
                string vserno = "0";
              // vserno = (int.Parse(vserno) + 1).ToString();
                vend_contractinvclose addobj = new vend_contractinvclose();
                string vcinvid = DateTime.Now.ToString("yyyyMMddhhmmssfffff");
                addobj.slyear = int.Parse(Request["slyear"].ToString());
                addobj.slmonth = int.Parse(Request["slmonth"].ToString());
                addobj.vcinvid = vcinvid;
                addobj.vcno = mainobj.pdno;
                addobj.vserno = int.Parse(vserno);
                addobj.vadate = DateTime.Parse(Request["vadate"].ToString());
                addobj.comid = mainobj.comid;
                addobj.projno = mainobj.projno;
                addobj.vstatus = "0";
                addobj.ownman = mainobj.ownman;
                addobj.totalmoney = mainobj.pallmoney * decimal.Parse(perrate);
                addobj.vendno = mainobj.allcomid;
                addobj.vcomment = mainobj.pcomment;
                addobj.bmodid = Session["empid"].ToString();
                addobj.bmoddate = DateTime.Now;
                addobj.prodid = mainobj.prodid;

                con.vend_contractinvclose.Add(addobj);
                con.SaveChanges();
                #endregion


                #region 尾的值

                string listsql = "";
                listsql = "select * from purchase_det where pid=" + pid + " order by pitemno";

                SqlDataReader dr = dbobj.dbselect(tmpconn, listsql);
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {

                        vend_contractinvclose_det addobjdet = new vend_contractinvclose_det();
                        addobjdet.itemno = int.Parse(dr["pitemno"].ToString());
                        addobjdet.vcinvid = vcinvid;
                        addobjdet.vcno = mainobj.pdno;
                        addobjdet.vserno = int.Parse(vserno);

                        addobjdet.pcount = 1;
                        addobjdet.pmoney = Decimal.Parse(dr["pdmoney"].ToString()) * decimal.Parse(perrate);
                        addobjdet.psummoney = Decimal.Parse(dr["pdallmoney"].ToString()) * decimal.Parse(perrate);
                        addobjdet.remoney = 0;
                        addobjdet.remoneypki = 0;
                        addobjdet.projno = dr["projno"].ToString();
                        addobjdet.vendcomid = dr["vendcomid"].ToString();
                        addobjdet.pdsdate = DateTime.Parse(dr["pdsdate"].ToString());
                        addobjdet.pdedate = DateTime.Parse(dr["pdedate"].ToString());

                        addobjdet.bdprodno = dr["mdno"].ToString();
                        addobjdet.bdprodtitle = dr["mdcomment"].ToString();

                        addobjdet.pcomment = dr["pdcomment"].ToString();
                        addobjdet.comid = mainobj.comid;
                        addobjdet.bmodid = Session["empid"].ToString();
                        addobjdet.bmoddate = DateTime.Now;
                        addobjdet.vendac = 0;
                        addobjdet.payvendcomid = "";
                        addobjdet.mdsummary = "";
                        addobjdet.closetype = "";

                        con.vend_contractinvclose_det.Add(addobjdet);
                        con.SaveChanges();
                    }
                }
                dr.Close();
                dr.Dispose();

                #endregion
                tmpconn.Close();
                tmpconn.Dispose();

            }



            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/vend_contractinvclose/list' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden id='qvcno' name='qvcno' value='" + qvcno + "'>";
            tmpform += "<input type=hidden id='qvendno' name='qvendno' value='" + qvendno + "'>";
            tmpform += "<input type=hidden id='qvcomment' name='qvcomment' value='" + qvcomment + "'>";
            tmpform += "<input type=hidden id='qsvadate' name='qsvadate' value='" + qsvadate + "'>";
            tmpform += "<input type=hidden id='qevadate' name='qevadate' value='" + qevadate + "'>";
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

            string qvcno = "", qvendno = "", qvcomment = "", qsvadate = "", qevadate = "";

            if (!string.IsNullOrWhiteSpace(Request["qvcno"]))
            {
                qvcno = Request["qvcno"].Trim();
                ViewBag.qvcno = qvcno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvendno"]))
            {
                qvendno = Request["qvendno"].Trim();
                ViewBag.qvendno = qvendno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcomment"]))
            {
                qvcomment = Request["qvcomment"].Trim();
                ViewBag.qvcomment = qvcomment;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsvadate"]))
            {
                qsvadate = Request["qsvadate"].Trim();
                ViewBag.qsvadate = qsvadate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qevadate"]))
            {
                qevadate = Request["qevadate"].Trim();
                ViewBag.qvadate = qevadate;
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
                        string vcno = dbobj.get_dbvalue(conn1, "select ('單號：' + vcno + ',次數' + convert(char,vserno)) as st1 from vend_contractinvclose where vcinvid='" + condtionArr[i].ToString() + "'");
                       
                        sysnote += vcno + "<br>";
                        //刪除憑單
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM vend_contractinvclose where vcinvid = '" + condtionArr[i].ToString() + "'");
                        //刪除明細
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM vend_contractinvclose_det where vcinvid = '" + condtionArr[i].ToString() + "'");
                      
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
                    tmpform += "<form name='qfr1' action='/vend_contractinvclose/list' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qvcno' name='qvcno' value='" + qvcno + "'>";
                    tmpform += "<input type=hidden id='qvendno' name='qvendno' value='" + qvendno + "'>";
                    tmpform += "<input type=hidden id='qvcomment' name='qvcomment' value='" + qvcomment + "'>";
                    tmpform += "<input type=hidden id='qsvadate' name='qsvadate' value='" + qsvadate + "'>";
                    tmpform += "<input type=hidden id='qevadate' name='qevadate' value='" + qevadate + "'>";
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

            string pid = Request["pid"].ToString().Trim();
            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                purchase modobj = con.purchase.Where(r => r.pid == pid).FirstOrDefault();

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
            tmpform += "<form name='qfr1' action='/purchase/list' method='post'>";
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
            tmppid = Request["vcinvid"].ToString().Trim();
            ViewBag.vcinvid = tmppid.ToString();

            List<vend_contractinvclose_det> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                var query = con.vend_contractinvclose_det.AsQueryable();
                result = query.Where(r => r.vcinvid == tmppid).AsQueryable().ToList();

            }
            return View(result);
      
        }

        public ActionResult detlistdo(string sysflag, int? page, string orderdata, string orderdata1)
        {

            string vcinvid = "";
            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
           // System.Data.SqlClient.SqlConnection erpconn = dbobj.get_conn("AitagBill_DBContext");
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {

                page = ((!page.HasValue || page < 1) ? 1 : page);
                ViewBag.page = page;
                ViewBag.orderdata = orderdata;
                ViewBag.orderdata1 = orderdata1;

                string cdel1 = Request["vctid"];
                string psummoney1 = Request["psummoney"];
                string remoney1 = Request["remoney"];
                string remoneypki1 = Request["remoneypki"];
                string mdsummary1 = Request["mdsummary"];

                string closetype1 = Request["closetype"];
                string payvendcomid1 = Request["payvendcomid"];
                string vendac1 = Request["vendac"];

                string[] cdelarr = cdel1.Split(',');
                string[] psummoneyarr = psummoney1.Split(',');
                string[] remoneyarr = remoney1.Split(',');
                string[] remoneypkiarr = remoneypki1.Split(',');
                string[] mdsummaryarr = mdsummary1.Split(',');

                string[] closetypearr = closetype1.Split(',');
                string[] payvendcomidarr = payvendcomid1.Split(',');
                string[] vendacarr = vendac1.Split(',');

                vcinvid = Request["vcinvid"].ToString();
                decimal allmoney = 0;
                for (int i = 0; i < cdelarr.Length; i++)
                {
                    if (cdelarr[i].Trim() == "")
                    {
                        //if (!(mdnoarr[i].Trim() == ""))
                        //{
                        //        purchase_det addobj = new purchase_det();
                        //        addobj.pid = decimal.Parse(pid);
                        //        addobj.pdno = Request["pdno"].ToString();
                        //        addobj.comid = Request["comid"].ToString();                             

                        //       // addobj.bdprodno = bdprodnoarr[i].Trim();
                        //      //  addobj.bdprodtitle = bdprodtitlearr[i].Trim();
                        //        addobj.mdno = mdnoarr[i].Trim();
                        //        addobj.mdcomment = mdcommentarr[i].Trim();

                        //        addobj.pitemno = pitemno;
                        //       // addobj.pdunit = pdunitarr[i].Trim();
                        //        addobj.pdcount = 1;
                        //        addobj.pdmoney = Decimal.Parse(pdallmoneyarr[i].ToString());
                        //        addobj.pdallmoney = Decimal.Parse(pdallmoneyarr[i].ToString());

                        //        addobj.pdcomment = pdcommentarr[i].Trim();
                        //        addobj.projno = Request["projno"].ToString();

                        //        addobj.bmodid = Session["empid"].ToString();
                        //        addobj.bmoddate = DateTime.Now;

                        //        con.purchase_det.Add(addobj);
                        //        con.SaveChanges();
                        //        pitemno = pitemno + 10;
                        //}
                    }
                    else
                    {
                       
                        //修改
                        int vctid = int.Parse(cdelarr[i].Trim());
                        vend_contractinvclose_det modobj = con.vend_contractinvclose_det.Where(r => r.vctid == vctid).FirstOrDefault();

                     
                 
                        modobj.psummoney = Decimal.Parse(psummoneyarr[i].ToString());
                        allmoney += Decimal.Parse(psummoneyarr[i].ToString());
                        modobj.remoney = Decimal.Parse(remoneyarr[i].ToString());
                        modobj.remoneypki = Decimal.Parse(remoneypkiarr[i].ToString());
                        modobj.mdsummary = mdsummaryarr[i].ToString();

                        modobj.vendac = Decimal.Parse(vendacarr[i].ToString());
                        modobj.closetype = closetypearr[i].ToString();
                        modobj.payvendcomid = payvendcomidarr[i].ToString();

                        modobj.bmodid = Session["empid"].ToString();
                        modobj.bmoddate = DateTime.Now;
                 

                        con.Entry(modobj).State = EntityState.Modified;
                        con.SaveChanges();

                        string tmpsql = "update vend_contractinvclose set totalmoney = '" + allmoney + "' where vcinvid =  '"+ vcinvid +"' ";
                        dbobj.dbexecute("AitagBill_DBContext", tmpsql);
                    }
                }
                con.Dispose();


            }

            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/vend_contractinvclose/detlist' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden name='vcinvid' id='vcinvid' value='" + vcinvid + "'>";        
            tmpform += "</form>";
            tmpform += "</body>";

 

            return new ContentResult() { Content = @"" + tmpform };
        }

        public ActionResult detdel(string id, int? page)
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
                        string money1 = dbobj.get_dbvalue(conn1, "select ('採購(委刊單)號' + pdno + ',品項' + bdprodno + ',金額' + convert(char,pdallmoney)) as st1  from purchase_det where pdid = '" + condtionArr[i].ToString() + "'");

                        sysnote += money1 + "<br>";                    
                        //刪除明細資料
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM purchase_det where pdid = '" + condtionArr[i].ToString() + "'");

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
                    tmpform += "<form name='qfr1' action='/purchase/detlist' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden id='pid' name='pid' value='" + pid + "'>";               
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

            string vcinvid = Request["vcinvid"].ToString().Trim();
       
            #region 寄信參數
            string bccemail = "";        
            string tmpsno = "";  //單號
            string tmpdate = "";//申請日期        
            string tmpnote = "";//摘要說明
            string tmpownman = "";//申請人
            string tmpmtitle = "";
            string tmpvserno = "";//結帳次數
            string tomail = "";

            #endregion


            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                vend_contractinvclose modobj = con.vend_contractinvclose.Where(r => r.vcinvid == vcinvid).FirstOrDefault();

                if (Request["vstatus"].ToString() == "0")  //第一次送時,補件修正不用
                {
                    if (Request["arolestampid"].ToString() != "")
                        tmparolestampid = "'" + Request["arolestampid"].ToString() + "'";
                    else
                        tmparolestampid = "'" + Request["arolestampid1"].ToString() + "'";

                    string tmpmoney = Request["totalmoney"].ToString().Replace(",","");
                    //找出下一個角色是誰               
                    string impallstring = dbobj.getnewcheck1("V", tmparolestampid, tmparolestampid, tmpmoney, "", "", "");

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

                tmpsno = modobj.vcno;
                tmpvserno = modobj.vserno.ToString();
                tmpdate = dbobj.get_date(modobj.vadate.ToString(), "1");
                // tmpvendno = modobj.vendno + "－" + dbobj.get_dbvalue(tmpconn1, "select comsttitle from allcompany where comid = '" + modobj.vendno + "'");
                // tmpamoney = modobj.totalmoney.ToString();
                tmpnote = modobj.vcomment;
                tmpownman = modobj.ownman;
                bccemail = dbobj.get_dbvalue(tmpconn1, "select enemail from employee where empid = '" + tmpownman + "'");
                tmpconn1.Close();
                tmpconn1.Dispose();
                //===============
                #endregion

                //呈核人員
                //======================
                //當不是代申請且未選擇(就是沒有下拉)申請呈核角色時，tmparolestampid就接arolestampid1的值

                if (Request["vstatus"].ToString() == "0")  //第一次送時,補件修正不用
                {
                    modobj.vstatus = "1";
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
                    modobj.vstatus = "1";
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

            mailtitle = "媒體結帳單申請請要求審核通知" + tmpmtitle;
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
            mailcontext += "<td align=right width='90'>結帳次數：</td>";
            mailcontext += "<td>" + tmpvserno + "</td>";
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
            tmpform += "<form name='qfr1' action='/vend_contractinvclose/list' method='post'>";
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
                    string tmpvserno = "";//結帳次數    
                    string tomail = "";
         
                    #endregion

                    string vcinvid = condtionArr[i].Trim();

                    #region 找值並異動資料庫

                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {
                        vend_contractinvclose modobj = con.vend_contractinvclose.Where(r => r.vcinvid == vcinvid).FirstOrDefault();
                        string tmpmoney = modobj.totalmoney.ToString();

                        //找出下一個角色是誰
                        string impallstring = dbobj.getnewcheck1("V", tmparolestampid, tmparolestampid, tmpmoney, "", "", "");
                        string[] tmpstrarr = impallstring.Split(';');
                        tmprole = tmpstrarr[0].ToString();
                        tmpbillid = tmpstrarr[1].ToString();
                        if (tmprole != "")
                        {
                            #region 寄信參數值
                            //===============               
                            System.Data.SqlClient.SqlConnection tmpconn1 = dbobj.get_conn("Aitag_DBContext");
                            tmpsno = modobj.vcno;
                            tmpvserno = modobj.vserno.ToString();
                            tmpdate = dbobj.get_date(modobj.vadate.ToString(), "1");
                            // tmpvendno = modobj.vendno + "－" + dbobj.get_dbvalue(tmpconn1, "select comsttitle from allcompany where comid = '" + modobj.vendno + "'");
                            // tmpamoney = modobj.totalmoney.ToString();
                            tmpnote = modobj.vcomment;
                            tmpownman = modobj.ownman;
                            bccemail = dbobj.get_dbvalue(tmpconn1, "select enemail from employee where empid = '" + tmpownman + "'");
                            tmpconn1.Close();
                            tmpconn1.Dispose();
                            //===============
                            #endregion

                            //呈核人員
                            //======================
                            //當不是代申請且未選擇(就是沒有下拉)申請呈核角色時，tmparolestampid就接arolestampid1的值

                            modobj.vstatus = "1";
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

                            mailtitle = "媒體結帳單申請要求審核通知";
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
                            mailcontext += "<td align=right width='90'>結帳次數：</td>";
                            mailcontext += "<td>" + tmpvserno + "</td>";
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

                            allsno += "【" + modobj.vcno + "】、";
                        }
                        else
                        { errmsg += "【" + modobj.vcno + "】、"; }


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
                tmpform += "<form name='qfr1' action='/vend_contractinvclose/list' method='post'>";
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
            IPagedList<vend_contractinvclose> result;
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "vcinvid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qvcno = "", qvendno = "", qvcomment = "", qsvadate = "", qevadate = "";

            if (!string.IsNullOrWhiteSpace(Request["qvcno"]))
            {
                qvcno = Request["qvcno"].Trim();
                ViewBag.qvcno = qvcno;
            }

            //客戶Campaine
            if (!string.IsNullOrWhiteSpace(Request["qvendno"]))
            {
                qvendno = Request["qvendno"].Trim();
                ViewBag.qvendno = qvendno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcomment"]))
            {
                qvcomment = Request["qvcomment"].Trim();
                ViewBag.qvcomment = qvcomment;
            }
            if (!string.IsNullOrWhiteSpace(Request["qsvadate"]))
            {
                qsvadate = Request["qsvadate"].Trim();
                ViewBag.qsvadate = qsvadate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qevadate"]))
            {
                qevadate = Request["qevadate"].Trim();
                ViewBag.qevadate = qevadate;
            }



            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "";
                sqlstr = "select * from vend_contractinvclose where vstatus = '1' and replace(rolestampid,'''','')='" + Session["rid"].ToString() + "' and comid = '" + Session["comid"].ToString() + "'  and";
                if (qvcno != "")
                    sqlstr += " vcno like '%" + qvcno + "%'  and";

                //客戶Campaine

                if (qvendno != "")
                    sqlstr += " projno in (select pdno from view_custpurchase where comsttitle like '%" + qvendno + "%' or campainetitle like '%" + qvendno + "%')   and";
                if (qvcomment != "")
                    sqlstr += " vcomment like '%" + qvcomment + "%'  and";
                if (qsvadate != "")
                    sqlstr += " vadate >= '" + qsvadate + "'  and";
                if (qevadate != "")
                    sqlstr += " vadate <= '" + qevadate + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.vend_contractinvclose.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<vend_contractinvclose>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);

        }

        public ActionResult chkEdit()
        {
            ViewBag.vcinvid = Request["vcinvid"].ToString();

            if (!string.IsNullOrWhiteSpace(Request["qvcno"]))
            {

                ViewBag.qvcno = Request["qvcno"];
            }
            if (!string.IsNullOrWhiteSpace(Request["qvendno"]))
            {

                ViewBag.qvendno = Request["qvendno"];
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcomment"]))
            {

                ViewBag.qvcomment = Request["qvcomment"];
            }
            if (!string.IsNullOrWhiteSpace(Request["qsvadate"]))
            {

                ViewBag.qsvadate = Request["qsvadate"];
            }
            if (!string.IsNullOrWhiteSpace(Request["qevadate"]))
            {

                ViewBag.qevadate = Request["qevadate"];
            }

            return View();
        }

        public ActionResult chkEditdo(string sysflag, int? page, string orderdata, string orderdata1)
        {

            string vcinvid = Request["vcinvid"].ToString().Trim();

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

            string pstatus = Request["vstatus"].ToString();
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
            string tmpvserno = "";//結帳次數
            string tomail = "";
            #endregion
            if (pstatus == "1")
            {
                #region  通過時

                //找出下一個角色是誰

                tmprole = dbobj.getnewcheck1("V", tmprolestampid, roleall, "0", "", billflowid);

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
                vend_contractinvclose modobj = con.vend_contractinvclose.Where(r => r.vcinvid == vcinvid).FirstOrDefault();
                System.Data.SqlClient.SqlConnection tmpconn1 = dbobj.get_conn("Aitag_DBContext");

                tmpsno = modobj.vcno;
                tmpvserno = modobj.vserno.ToString();
                tmpdate = dbobj.get_date(modobj.vadate.ToString(), "1");
                //tmpvendno = modobj.vendno +"－" + dbobj.get_dbvalue(tmpconn1, "select comsttitle from allcompany where comid = '" + modobj.vendno + "'");
                //tmpamoney = modobj.totalmoney.ToString();
                tmpnote = modobj.vcomment;
                tmpownman = modobj.ownman;   
                bccemail = dbobj.get_dbvalue(tmpconn1, "select enemail from employee where empid = '" + tmpownman + "'");
                tmpconn1.Close();
                tmpconn1.Dispose();
                //===============
                #endregion
                //呈核人員
                //======================             
                modobj.vstatus = tmpstatus;
                if (pstatus == "1")
                { 
                    modobj.rolestampid = tmprole;  //下個呈核角色

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
                    mailtitle = "媒體結帳單申請要求審核通知";
                    txt_comment = "有一筆申請單提出，請至系統進行審核作業。<br>資料如下：";
                    break;
                case "2"://已審核        
                    mailtitle = "媒體結帳單申請完成審核通知";
                    txt_comment = "您的申請單已通過審核。<br>資料如下：";
                    break;
                case "3"://退回補正   
                    mailtitle = "媒體結帳單申請退回補正通知";
                    txt_comment = "您的申請單有問題，審核者要求您修正。<br>資料如下：";
                    break;
                case "D"://退回      
                    mailtitle = "媒體結帳單申請退回通知";
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
            mailcontext += "<td align=right width='90'>結帳次數：</td>";
            mailcontext += "<td>" + tmpvserno + "</td>";
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
                tmpform += "<form name='qfr1' action='/vend_contractinvclose/chk' method='post'>";
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
            sqlstr = "select * from vend_contractinvclose where vcinvid in (" + cdel + ")";
            cmd.CommandText = sqlstr;
            SqlDataReader alldr = cmd.ExecuteReader();
            while (alldr.Read())
            {
                string vcinvid = alldr["vcinvid"].ToString().Trim();
                string pstatus = Request["vstatus"].ToString();
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
                string tmpvserno = "";//結帳次數
                string tomail = "";
                #endregion
                if (pstatus == "1")
                {
                    #region  通過時

                    //找出下一個角色是誰

                    tmprole = dbobj.getnewcheck1("U", tmprolestampid, roleall, "0", "", billflowid);

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
                    vend_contractinvclose modobj = con.vend_contractinvclose.Where(r => r.vcinvid == vcinvid).FirstOrDefault();
                    System.Data.SqlClient.SqlConnection tmpconn1 = dbobj.get_conn("Aitag_DBContext");

                    tmpsno = modobj.vcno;
                    tmpvserno = modobj.vserno.ToString();
                    tmpdate = dbobj.get_date(modobj.vadate.ToString(), "1");
                    //tmpvendno = modobj.vendno +"－" + dbobj.get_dbvalue(tmpconn1, "select comsttitle from allcompany where comid = '" + modobj.vendno + "'");
                    //tmpamoney = modobj.totalmoney.ToString();
                    tmpnote = modobj.vcomment;
                    tmpownman = modobj.ownman;
                    bccemail = dbobj.get_dbvalue(tmpconn1, "select enemail from employee where empid = '" + tmpownman + "'");
                    tmpconn1.Close();
                    tmpconn1.Dispose();
                    //===============
                    #endregion
                    //呈核人員
                    //======================             
                    modobj.vstatus = tmpstatus;
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
                        mailtitle = "媒體結帳單申請要求審核通知";
                        txt_comment = "有一筆申請單提出，請至系統進行審核作業。<br>資料如下：";
                        break;
                    case "2"://已審核        
                        mailtitle = "媒體結帳單申請完成審核通知";
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
                mailcontext += "<td align=right width='90'>結帳次數：</td>";
                mailcontext += "<td>" + tmpvserno + "</td>";
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
            tmpform += "<form name='qfr1' action='/vend_contractinvclose/chk' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "</form>";
            tmpform += "</body>";
            }

            return new ContentResult() { Content = @"" + tmpform };
        }

        public ActionResult vendlistxls()
        {
            NDcommon dbobj = new NDcommon();string sql = "";

            //準備 結帳類別 欄位
            List<mmino> mminoList = new List<mmino>();
            using (SqlConnection conn1 = dbobj.get_conn("AitagBill_DBContext"))
            {
                sql = "select mino , mititle from mediaitemtype order by mitype";
                using (SqlCommand cmd1 = new SqlCommand(sql, conn1))
                {
                    SqlDataReader dr1 = cmd1.ExecuteReader();
                    while (dr1.Read())
                    {
                        mminoList.Add(new mmino() { mino = dr1["mino"].ToString(), mititle = dr1["mititle"].ToString() });
                    }
                    dr1.Close();
                }
            }




            string Excel = "";/*HTML Excel*/

            string vcinvid = Request["vcinvid"].ToString();
            sql = " select vend_contractinvclose_det.*,(select distinct comsttitle from view_custpurchase where comid = vend_contractinvclose.comid and pdno=vend_contractinvclose.projno ) as comsttitle,(select distinct campainetitle from view_custpurchase where comid = vend_contractinvclose.comid and pdno=vend_contractinvclose.projno ) as campainetitle,(select distinct psdate from view_custpurchase where comid = vend_contractinvclose.comid and pdno=vend_contractinvclose.projno ) as psdate,(select distinct pedate from view_custpurchase where comid = vend_contractinvclose.comid and pdno=vend_contractinvclose.projno ) as pedate,vend_contractinvclose.prodid,vend_contractinvclose.vcno FROM vend_contractinvclose INNER JOIN  vend_contractinvclose_det ON vend_contractinvclose.vcinvid = vend_contractinvclose_det.vcinvid ";
            sql += " where vend_contractinvclose_det.vcinvid=" + vcinvid;
            sql += " order by vend_contractinvclose_det.vendcomid ";

            SqlConnection tmpconn = dbobj.get_conn("AitagBill_DBContext");
            SqlConnection tmpconn1 = dbobj.get_conn("AitagBill_DBContext");
            SqlDataReader dr = dbobj.dbselect(tmpconn, sql);
            string tmpprodid = "";
            string tmptoday =  dbobj.get_date(DateTime.Now.ToString(), "1");

            //準備資料
            List<vendlistxls_det> vendlistxls_detList = new List<vendlistxls_det>();
            if (dr.HasRows)
            {
                Excel = "<HTML>";
                Excel += "<HEAD>";
                Excel += @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">";
                Excel += "</HEAD>";
                Excel += "<body style=font-family:微軟正黑體>";

                Excel += "<center><b style='font-size:17pt'>2008傳媒行銷股份有限公司<BR>";
                Excel += "2008 Media Marketing Company Limited</b><p><p>";
                Excel += " <b style='font-size:17pt'>客戶媒體預算總表</b></center>";
                string mmoney = "0";
                decimal tmppsummoney = 0;
                decimal sumtmpmoney = 0;
                string tmpvendname = "";
                string tmpvendname2 = "";
                string tmpMedia = "";
                string tmpMateriral = "";
                string tmpmdsummary = "";
                string projno = "";

                int tmpi = 0;
                int row = -1;
                Boolean rowEnable = false;
                while (dr.Read())
                {
                    rowEnable = false;
                    projno = dr["projno"].ToString();
                    if (tmpi == 0)
                    {
                        tmpprodid = dr["prodid"].ToString();
                        tmpprodid = dbobj.get_dbvalue(tmpconn1, "select prodtitle from custproduct where prodid = " + tmpprodid);
                        tmpMateriral = dr["bdprodtitle"].ToString();
                        ViewBag.vcno = dr["vcno"].ToString();
                        Excel += "<table border=0 cellpadding=0 cellspacing=0 style=font-family:微軟正黑體;font-size:12pt bordercolor=#000000 bordercolordark=#ffffff width=850>";
                        Excel += "<tr><td></td><td>製表日期：" + tmptoday + "</td></tr>";
                        Excel += "<tr><td width=50%>客戶：" + dr["comsttitle"].ToString() + "</td><td width=50%>結帳月份：2016/04</td></tr>";
                        Excel += "<tr><td>產品：" + tmpprodid + "</td><td>Campaigne：" + dr["campainetitle"].ToString() + "</td></tr>";
                        Excel += "<tr><td>Materiral：" + tmpMateriral + "</td><td>Period：" + dbobj.get_date(dr["psdate"].ToString(), "1") + "~" + dbobj.get_date(dr["pedate"].ToString(), "1") + "</td></tr>";
                        Excel += "<tr height=10><td colspan=2></td></tr>";
                        Excel += "<tr align=center><td colspan=2>";
                        tmpi++;
                    }
                    //結帳類別 欄位 判斷 是否呈現
                    foreach (mmino v in mminoList) 
                    {
                        if (v.mino == dbobj.get_dbnull2(dr["closetype"]))
                        {
                            v.enable = true;
                            rowEnable = true;
                        }
                    }

                 
                    //廠商
                    tmpvendname = dbobj.get_dbvalue(tmpconn1, "select comsttitle from allcompany where comid = '" + dr["vendcomid"] + "'");
                    //Media/結帳類別
                    tmpMedia = dbobj.get_dbvalue(tmpconn1, "select mdtitle from mediachannel where mdno = '" + dr["bdprodno"] + "'");
                    //摘要
                    tmpmdsummary = dr["mdsummary"].ToString();
                    
                    //CF(委刊金額) 
                    mmoney = dbobj.get_dbvalue(tmpconn1, "select round(pdallmoney,0) as pdallmoney from purchase_det where pdno = '" + dr["vcno"].ToString() + "' and mdno='" + dr["bdprodno"].ToString() + "'")+"";
                    if (mmoney == "")
                    { mmoney = "0"; }
                    //實收價(本期結帳)
                    tmppsummoney =decimal.Parse(dr["psummoney"].ToString());

                    sumtmpmoney = sumtmpmoney + tmppsummoney;

                    if (rowEnable)
                    {/*有對到結帳類別  才會進來*/
                        if (tmpvendname2 != tmpvendname)
                        {
                            if (row > -1)
                            {//算總數
                                foreach (mmoney v in vendlistxls_detList[row].mmoney)
                                {
                                    if (v.mino != "")
                                    {
                                        vendlistxls_detList[row].tmppsummoney += v.num;
                                    }
                                }
                            }

                            //新增下一筆資料
                            row++;
                            vendlistxls_detList.Add(new vendlistxls_det() { tmpvendname = tmpvendname, tmpMedia = tmpMedia, tmpmdsummary = tmpmdsummary });
                            vendlistxls_detList[row].mmoney.Add(new mmoney() { mino = dbobj.get_dbnull2(dr["closetype"]), num = tmppsummoney });

                            tmpvendname2 = tmpvendname;
                        }
                        else
                        {
                            vendlistxls_detList[row].mmoney.Add(new mmoney() { mino = dbobj.get_dbnull2(dr["closetype"]), num = tmppsummoney });
                        }
                    }
                }

                if (row > -1)
                {//算總數
                    foreach (mmoney v in vendlistxls_detList[row].mmoney)
                    {
                        if (v.mino != "")
                        {
                            vendlistxls_detList[row].tmppsummoney += v.num;
                        }
                    }
                }

                //準備欄位
                mminoList.RemoveAll(p => p.enable == false);
                


                //欄位

                Excel += "<table  border=1  cellpadding=0 cellspacing=0 style=font-family:微軟正黑體;font-size:12pt width=100% bordercolor=#000000 bordercolordark=#ffffff>";
                Excel += "<tr align=center>";
                Excel += "<th width='120'>供應商</th> ";
                Excel += "<th width='150'>Media/結帳類別</th>";
                Excel += " <th width='150'>媒體摘要</th>";
                foreach (mmino v in mminoList)
                {
                    Excel += string.Format("<th width='120'>{0}</th>", v.mititle) ;
                }
                Excel += "<th width='150'>實收價/Actual Net</th> ";
                Excel += "</tr>";

                note_det Setnote_det = new note_det();/*備註*/

                //資料
                decimal num = 0;
                foreach (vendlistxls_det v in vendlistxls_detList)
                {

                    Excel += "<tr align=center>";
                    Excel += "<td >" + v.tmpvendname + "</td>";
                    Excel += "<td>" + v.tmpMedia + "</td>";
                    Excel += "<td>" + v.tmpmdsummary + "</td>";
                    foreach (mmino v2 in mminoList)
                    {
                        num = 0;
                        foreach (mmoney v3 in v.mmoney)
                        {
                            if (v2.mino == v3.mino)
                            {
                                num += v3.num;
                                
                            }
                        }
                        Setnote_det.Total += num;
                        Excel += string.Format("<td align=right>{0}</td>", num.ToString("###,###,##0"));
                    }
                    Excel += "<td align=right>" + v.tmppsummoney.ToString("###,###,##0") + "</td>";
                    Excel += "</tr>";

                }

                //備註：
                string acrate = "";
                acrate = dbobj.get_dbvalue(tmpconn1, "select acrate from custpurchase where pdno = '" + projno + "' and comid = '" + Session["comid"] + "'");
                Setnote_det.A_C = Math.Round(Setnote_det.Total * decimal.Parse("0" + acrate) / 100, 0);
                Setnote_det.VAT = Math.Round((Setnote_det.Total + Setnote_det.A_C) * decimal.Parse("0.05"), 0);
                Setnote_det.Grand_Tota = Setnote_det.Total + Setnote_det.A_C + Setnote_det.VAT;


                Excel += "<tr >";
                Excel += "<td colspan=" + (mminoList.Count + 2) + " rowspan=4 valign=top>備註：</td>";
                Excel += "<td align=right>Total：</td>";
                Excel += "<td align=right>" + Setnote_det.Total.ToString("###,###,##0") + "</td>";
                Excel += "</tr>";
                
                Excel += "<tr align=right>";
                Excel += "<td>A/C " + decimal.Parse("0"+acrate).ToString("##0.0##") + " %：</td>";
                Excel += "<td>" + Setnote_det.A_C.ToString("###,###,##0") + "</td>";
                Excel += "</tr>";

                Excel += "<tr align=right>";
                Excel += "<td>VAT5%：</td>";
                Excel += "<td>" + Setnote_det.VAT.ToString("###,###,##0") + "</td>";
                Excel += "</tr>";

                Excel += "<tr align=right>";
                Excel += "<td>Grand Total：</td>";
                Excel += "<td>" + Setnote_det.Grand_Tota.ToString("###,###,##0") + "</td>";
                Excel += "</tr>";
               

                Excel += "</table>";
                Excel += "</td></tr>";     
                Excel += "</table><p><p>";
                Excel += "主管：　　　　　　　　　　經辦人員：<br>";
                Excel += "</body>";
                Excel += "</HTML>";
            }

            tmpconn1.Close();
            tmpconn1.Dispose();

            tmpconn.Close();
            tmpconn.Dispose();

            ViewBag.Excel = Excel;
            return View();


        }

        public ActionResult vendlist1xls()
        {
            NDcommon dbobj = new NDcommon(); string sql = "";
            string Excel = "";

            //準備 結帳類別 欄位
            List<mmino> mminoList = new List<mmino>();
            using (SqlConnection conn1 = dbobj.get_conn("AitagBill_DBContext"))
            {
                sql = "select mino , mititle from mediaitemtype order by mitype";
                using (SqlCommand cmd1 = new SqlCommand(sql, conn1))
                {
                    SqlDataReader dr1 = cmd1.ExecuteReader();
                    while (dr1.Read())
                    {
                        mminoList.Add(new mmino() { mino = dr1["mino"].ToString(), mititle = dr1["mititle"].ToString() });
                    }
                    dr1.Close();
                }
            }

            string vcinvid = Request["vcinvid"].ToString();
            sql = " select vend_contractinvclose_det.*,(select distinct comsttitle from view_custpurchase where comid = vend_contractinvclose.comid and pdno=vend_contractinvclose.projno ) as comsttitle,(select distinct campainetitle from view_custpurchase where comid = vend_contractinvclose.comid and pdno=vend_contractinvclose.projno ) as campainetitle,(select distinct psdate from view_custpurchase where comid = vend_contractinvclose.comid and pdno=vend_contractinvclose.projno ) as psdate,(select distinct pedate from view_custpurchase where comid = vend_contractinvclose.comid and pdno=vend_contractinvclose.projno ) as pedate,vend_contractinvclose.prodid,vend_contractinvclose.vcno FROM vend_contractinvclose INNER JOIN  vend_contractinvclose_det ON vend_contractinvclose.vcinvid = vend_contractinvclose_det.vcinvid ";
            sql += " where vend_contractinvclose_det.vcinvid=" + vcinvid;
            sql += " order by vend_contractinvclose_det.vendcomid ";
            
            SqlConnection tmpconn = dbobj.get_conn("AitagBill_DBContext");
            SqlConnection tmpconn1 = dbobj.get_conn("AitagBill_DBContext");
            SqlDataReader dr = dbobj.dbselect(tmpconn, sql);
            string tmpprodid = "";

            string tmptoday = dbobj.get_date(DateTime.Now.ToString(), "1");

            //準備資料
            List<vendlistxls_det> vendlistxls_detList = new List<vendlistxls_det>();
            if (dr.HasRows)
            {

                Excel = "<HTML>";
                Excel += "<HEAD>";
                Excel += @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">";
                Excel += "</HEAD>";
                Excel += "<body style=font-family:微軟正黑體>";

                Excel += "<center><b style='font-size:17pt'>2008傳媒行銷股份有限公司<BR>";
                Excel += "2008 Media Marketing Company Limited</b><p><p>";
                Excel += " <b style='font-size:17pt'>【2008Media媒體結帳總表】－媒體庫</b></center>";
                string mmoney = "0";
                decimal tmppsummoney = 0;
                decimal sumtmpmoney = 0;
                string tmpvendname = "";
                string tmpvendname2 = "";
                string tmpMedia = "";
                string tmpMateriral = "";
                string tmpmdsummary = "";
                string projno = "";

                int tmpi = 0;
                int row = -1;
                Boolean rowEnable = false;
                while (dr.Read())
                {
                    rowEnable = false;
                    projno = dr["projno"].ToString();
                    if (tmpi == 0)
                    {
                        tmpprodid = dr["prodid"].ToString();
                        tmpprodid = dbobj.get_dbvalue(tmpconn1, "select prodtitle from custproduct where prodid = " + tmpprodid);
                        tmpMateriral = dr["bdprodtitle"].ToString();
                        ViewBag.vcno = dr["vcno"].ToString();
                        Excel += "<table border=0 cellpadding=0  style=font-family:微軟正黑體 cellspacing=0 bordercolor=#000000 bordercolordark=#ffffff>";
                        Excel += "<tr><td></td><td>製表日期：" + tmptoday + "</td></tr>";
                        Excel += "<tr><td width=50%>客戶：" + dr["comsttitle"].ToString() + "</td><td width=50%>結帳月份：2016/04</td></tr>";
                        Excel += "<tr><td>產品：" + tmpprodid + "</td><td>Campaigne：" + dr["campainetitle"].ToString() + "</td></tr>";
                        Excel += "<tr><td>Materiral：" + tmpMateriral + "</td><td>Period：" + dbobj.get_date(dr["psdate"].ToString(), "1") + "~" + dbobj.get_date(dr["pedate"].ToString(), "1") + "</td></tr>";
                        Excel += "<tr height=10><td colspan=2></td></tr>";
                        Excel += "<tr align=center><td colspan=2>";
                        tmpi++;
                    }

                    //結帳類別 欄位 判斷 是否呈現
                    foreach (mmino v in mminoList)
                    {
                        if (v.mino == dbobj.get_dbnull2(dr["closetype"]))
                        {
                            v.enable = true;
                            rowEnable = true;
                        }
                    }

                    //廠商
                    tmpvendname = dbobj.get_dbvalue(tmpconn1, "select comsttitle from allcompany where comid = '" + dr["vendcomid"] + "'");
                    //Media/結帳類別
                    tmpMedia = dbobj.get_dbvalue(tmpconn1, "select mdtitle from mediachannel where mdno = '" + dr["bdprodno"] + "'");
                    //摘要
                    tmpmdsummary = dr["mdsummary"].ToString();

                    //CF(委刊金額) 
                    mmoney = dbobj.get_dbvalue(tmpconn1, "select round(pdallmoney,0) as pdallmoney from purchase_det where pdno = '" + dr["vcno"].ToString() + "' and mdno='" + dr["bdprodno"].ToString() + "'") + "";
                    if (mmoney == "")
                    { mmoney = "0"; }
                    //實收價(本期結帳)
                    tmppsummoney = decimal.Parse(dr["psummoney"].ToString());

                    sumtmpmoney = sumtmpmoney + tmppsummoney;

                    if (rowEnable)
                    {/*有對到結帳類別  才會進來*/
                        if (tmpvendname2 != tmpvendname)
                        {
                            if (row > -1)
                            {//算總數
                                foreach (mmoney v in vendlistxls_detList[row].mmoney)
                                {
                                    if (v.mino != "")
                                    {
                                        vendlistxls_detList[row].tmppsummoney += v.num;
                                    }
                                }
                            }

                            //新增下一筆資料
                            row++;
                            vendlistxls_detList.Add(new vendlistxls_det() { tmpvendname = tmpvendname, tmpMedia = tmpMedia, tmpmdsummary = tmpmdsummary });
                            vendlistxls_detList[row].mmoney.Add(new mmoney() { mino = dbobj.get_dbnull2(dr["closetype"]), num = tmppsummoney });

                            tmpvendname2 = tmpvendname;
                        }
                        else
                        {
                            vendlistxls_detList[row].mmoney.Add(new mmoney() { mino = dbobj.get_dbnull2(dr["closetype"]), num = tmppsummoney });
                        }
                    }
                }

                if (row > -1)
                {//算總數
                    foreach (mmoney v in vendlistxls_detList[row].mmoney)
                    {
                        if (v.mino != "")
                        {
                            vendlistxls_detList[row].tmppsummoney += v.num;
                        }
                    }
                }

                //準備欄位
                mminoList.RemoveAll(p => p.enable == false);



                //欄位

                Excel += "<table  border=1  cellpadding=0 cellspacing=0 style=font-family:微軟正黑體;font-size:12pt width=100% bordercolor=#000000 bordercolordark=#ffffff>";
                Excel += "<tr align=center>";
                //Excel += "<th width='120'>供應商</th> ";
                Excel += "<th width='150'>Media/結帳類別</th>";
                Excel += " <th width='150'>媒體摘要</th>";
                foreach (mmino v in mminoList)
                {
                    Excel += string.Format("<th width='120'>{0}</th>", v.mititle);
                }
                Excel += "<th width='150'>實收價/Actual Net</th> ";
                Excel += "</tr>";

                note_det Setnote_det = new note_det();/*備註*/

                //資料
                decimal num = 0;
                foreach (vendlistxls_det v in vendlistxls_detList)
                {
                    Excel += "<tr align=center>";
                    //Excel += "<td >" + v.tmpvendname + "</td>";
                    Excel += "<td>" + v.tmpMedia + "</td>";
                    Excel += "<td>" + v.tmpmdsummary + "</td>";
                    foreach (mmino v2 in mminoList)
                    {
                        num = 0;
                        foreach (mmoney v3 in v.mmoney)
                        {
                            if (v2.mino == v3.mino)
                            {
                                num += v3.num;

                            }
                        }
                        Setnote_det.Total += num;
                        Excel += string.Format("<td align=right>{0}</td>", num.ToString("###,###,##0"));
                    }
                    Excel += "<td align=right>" + v.tmppsummoney.ToString("###,###,##0") + "</td>";
                    Excel += "</tr>";

                }

                //備註：
                string acrate = "";
                acrate = dbobj.get_dbvalue(tmpconn1, "select acrate from custpurchase where pdno = '" + projno + "' and comid = '" + Session["comid"] + "'");
                Setnote_det.A_C = Math.Round(Setnote_det.Total * decimal.Parse("0" + acrate) / 100, 0);
                Setnote_det.VAT = Math.Round((Setnote_det.Total + Setnote_det.A_C) * decimal.Parse("0.05"), 0);
                Setnote_det.Grand_Tota = Setnote_det.Total + Setnote_det.A_C + Setnote_det.VAT;


                Excel += "<tr >";
                Excel += "<td colspan=" + (mminoList.Count + 1) + " rowspan=4 valign=top>備註：</td>";
                Excel += "<td align=right>Total：</td>";
                Excel += "<td align=right>" + Setnote_det.Total.ToString("###,###,##0") + "</td>";
                Excel += "</tr>";

                Excel += "<tr align=right>";
                Excel += "<td>A/C " + decimal.Parse("0" + acrate).ToString("##0.0##") + " %：</td>";
                Excel += "<td>" + Setnote_det.A_C.ToString("###,###,##0") + "</td>";
                Excel += "</tr>";

                Excel += "<tr align=right>";
                Excel += "<td>VAT5%：</td>";
                Excel += "<td>" + Setnote_det.VAT.ToString("###,###,##0") + "</td>";
                Excel += "</tr>";

                Excel += "<tr align=right>";
                Excel += "<td>Grand Total：</td>";
                Excel += "<td>" + Setnote_det.Grand_Tota.ToString("###,###,##0") + "</td>";
                Excel += "</tr>";


                Excel += "</table>";
                Excel += "</td></tr>";
                Excel += "</table><p><p>";
                Excel += "主管：　　　　　　　　　　經辦人員：<br>";
                Excel += "</body>";
                Excel += "</HTML>";
            }

            tmpconn1.Close();
            tmpconn1.Dispose();

            tmpconn.Close();
            tmpconn.Dispose();

            ViewBag.Excel = Excel;
            return View();


        }


    }
     public class note_det
     {
         public decimal Total = 0;
         public decimal A_C = 0;
         public decimal VAT = 0;
         public decimal Grand_Tota = 0;
     }
     public class vendlistxls_det
     {
         public string tmpvendname = "";
         public string tmpMedia = "";
         public string tmpmdsummary = "";
         public List<mmoney> mmoney = new List<mmoney>();
         public decimal tmppsummoney = 0;
     }
     public class mmoney
     {
         public string mino = "";
         public decimal num = 0;
     }
     public class mmino
     {
         public string mino = "";
         public string mititle = "";
         public Boolean enable = false;

     }

}
