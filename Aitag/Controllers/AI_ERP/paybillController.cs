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
    public class paybillController : BaseController
    {
        //
        // GET: /common/

        public ActionResult Index()
        {

            
            return View();
        }

        public ActionResult expense1list(int? page, string orderdata, string orderdata1)
        {
            IPagedList<vend_contractinv> result;
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
                sqlstr = "select * from vend_contractinv where vctype = 'P' and vcsubtype='2' and comid = '" + Session["comid"].ToString() + "' and ownman = '" + Session["empid"].ToString() + "'  and";
                if (qvcno != "")
                    sqlstr += " vcno like '%" + qvcno + "%'  and";
                if (qvendno != "")
                    sqlstr += " vendno in (select comid from allcompany where comid like '%" + qvendno + "%' or comtitle like '%" + qvendno + "%')   and";
                if (qvcomment != "")
                    sqlstr += " vcomment like '%" + qvcomment + "%'  and";
                if (qsvadate != "")
                    sqlstr += " vadate >= '" + qsvadate + "'  and";
                if (qevadate != "")
                    sqlstr += " vadate <= '" + qevadate + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.vend_contractinv.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<vend_contractinv>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);
        }


        public ActionResult expense2dp()
        {
           // vend_contractinv_det col = new vend_contractinv_det();
            string tvcno = "",tcomid = "" ;
            tvcno = Request["vcno"].ToString();
            if(Request["comid"]!=null)
            tcomid = Request["comid"].ToString();
            else
            tcomid = Session["comid"].ToString();
            List<vend_contractinv_det> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                var query = con.vend_contractinv_det
                  .AsQueryable();
                result = query.Where(r => r.vcno == tvcno).Where(r => r.comid == tcomid).AsQueryable().ToList();
                
            }
            return View(result);
           // return View(col);
        }

        public ActionResult expenseinv()
        {
            // vend_contractinv_det col = new vend_contractinv_det();
            string tvcno = "" , tcomid = "";
            tvcno = Request["vcno"].ToString();
            if (Request["comid"] != null)
                tcomid = Request["comid"].ToString();
            else
                tcomid = Session["comid"].ToString();

            List<incomeaccounts> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                var query = con.incomeaccounts
                  .AsQueryable();
                result = query.Where(r => r.vcno == tvcno).Where(r => r.comid == tcomid).AsQueryable().ToList();

            }
            return View(result);
            // return View(col);
        }

        public ActionResult expenseinvadddo(string sysflag, int? page, string orderdata, string orderdata1)
        {
             ModelState.Clear();
            //string uaccount = Request["UserAccount"];
            NDcommon dbobj = new NDcommon();
            SqlConnection erpconn = dbobj.get_conn("AitagBill_DBContext");
            SqlCommand cmd = new SqlCommand();
            string vcno = "";
            vcno = Request["vcno"].ToString();
            int tvcinvid = int.Parse(dbobj.get_vcinvid(erpconn, Request["comid"].ToString(), vcno));
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {

                page = ((!page.HasValue || page < 1) ? 1 : page);
                ViewBag.page = page;
                ViewBag.orderdata = orderdata;
                ViewBag.orderdata1 = orderdata1;
                string sqlstr = "";
                string cdel1 = Request["icid"];
                string invno1 = Request["invno"];
                string invmoney1 = Request["invmoney"];
                string salemoney1 = Request["salemoney"];
                string taxmoney1 = Request["taxmoney"];
                string invdate1 = Request["invdate"];
                string[] cdelarr = cdel1.Split(',');
                string[] invnoarr = invno1.Split(',');
                string[] invmoneyarr = invmoney1.Split(',');
                string[] salemoneyarr = salemoney1.Split(',');
                string[] taxmoneyarr = taxmoney1.Split(',');
                string[] invdatearr = invdate1.Split(',');

              
                // vend_contractinv Userobj = con.vend_contractinv.Where(r => r.UserAccount == uaccount).FirstOrDefault();
                for (int i = 0; i < cdelarr.Length; i++)
                {
                    if (cdelarr[i].Trim() == "")
                    {
                        if (!(invmoneyarr[i].Trim() == "" && salemoneyarr[i].Trim() == ""))
                        {
                            if (taxmoneyarr[i].Trim() != "")
                            {
                                incomeaccounts addobj = new incomeaccounts();
                                addobj.vcinvid = tvcinvid;
                                addobj.comid = Request["comid"].ToString();
                                addobj.vcno = vcno;
                                addobj.vserno = 1;
                                addobj.invno = invnoarr[i].Trim();
                                addobj.taxmoney = int.Parse(taxmoneyarr[i].ToString());
                               // addobj.punit = "";
                                addobj.invmoney = int.Parse(invmoneyarr[i].Trim());
                                addobj.salemoney = int.Parse(salemoneyarr[i].Trim());
                                //addobj.psummoney = int.Parse(taxmoneyarr[i].ToString());
                                addobj.invdate = DateTime.Parse(invdatearr[i].Trim());
                                addobj.bmodid = Session["empid"].ToString();
                                addobj.bmoddate = DateTime.Now;
                                addobj.invtax = Decimal.Parse("0.05");
                                addobj.catcount = 0;
                                con.incomeaccounts.Add(addobj);
                                //try { 
                                con.SaveChanges();
                                //    }catch(Exception e)
                                //{

                                //}
                            }
                        }
                    }
                    else
                    {
                        if (invmoneyarr[i].Trim() != "" || salemoneyarr[i].Trim() != "")
                        {
                            sqlstr = "update incomeaccounts set taxmoney = " + taxmoneyarr[i].Trim() + ",salemoney=" + salemoneyarr[i].Trim() + ",invmoney='" + invmoneyarr[i].Trim() + "',invno='" + invnoarr[i].Trim() + "',invdate='" + invdatearr[i].Trim() + "' where icid = '" + cdelarr[i].Trim() + "'";
                            cmd.CommandText = sqlstr;
                            cmd.Connection = erpconn;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                con.Dispose();


            }

            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/paybill/expenseinv' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden name='vcno' id='vcno' value='" + vcno + "'>";
            // tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
            tmpform += "</form>";
            tmpform += "</body>";

            erpconn.Close();
            erpconn.Dispose();

            return new ContentResult() { Content = @"" + tmpform };
        }

       /* public ActionResult expense1inv()
        {
            Employee col = new Employee();
            return View(col);
        }*/

       
        public ActionResult expenserpt()
        {
            ViewBag.vcno = Request["vcno"].ToString();
            return View();
        }

        public ActionResult expense2list(int? page, string orderdata, string orderdata1)
        {
            IPagedList<vend_contractinv> result;
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "vcinvid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qvcno = "" , qvendno = "" , qvcomment = "" , qsvadate = "" , qevadate = "";
           
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
                ViewBag.qevadate = qevadate;
            }
            
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "";
                sqlstr = "select * from vend_contractinv where vctype = 'P' and vcsubtype='1'  and comid = '" + Session["comid"].ToString() + "' and ownman = '" + Session["empid"].ToString() + "'   and";
                if (qvcno != "")
                    sqlstr += " vcno like '%" + qvcno +"%'  and";
                if (qvendno != "")
                    sqlstr += " vendno in (select comid from allcompany where comid like '%" + qvendno + "%' or comtitle like '%" + qvendno + "%')   and";
                if (qvcomment != "")
                    sqlstr += " vcomment like '%" + qvcomment + "%'  and";
                if (qsvadate != "")
                    sqlstr += " vadate >= '" + qsvadate + "'  and";
                if (qevadate != "")
                    sqlstr += " vadate <= '" + qevadate + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1 ; 
               
                var query = con.vend_contractinv.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<vend_contractinv>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);
        }

        public ActionResult expense2add()
        {
           // Employee col = new Employee();
            return View();
        }

        public ActionResult expense2adddo(string sysflag, int? page, string orderdata, string orderdata1)
        {
        
            NDcommon dbobj = new NDcommon();
            SqlConnection conn = dbobj.get_conn("AitagBill_DBContext");
            string vcno = "";

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {

                page = ((!page.HasValue || page < 1) ? 1 : page);
                ViewBag.page = page;
                ViewBag.orderdata = orderdata;
                ViewBag.orderdata1 = orderdata1;

                string qempname = Request["qempname"];
                ViewBag.qempname = qempname;
                // vend_contractinv Userobj = con.vend_contractinv.Where(r => r.UserAccount == uaccount).FirstOrDefault();
                vend_contractinv addobj = new vend_contractinv();
                // if (Userobj == null)
                // {
                vcno = dbobj.get_billno(conn, "P", Request["vcsubtype"].ToString() , Request["comid"].ToString(), "", Request["vadate"].ToString());
                //     Userobj = new User();
                //Guid 產生的方式
                //Guid Uid = Guid.NewGuid();
                addobj.vadate = DateTime.Parse(Request["vadate"].ToString());
                addobj.comid = Request["comid"];
                addobj.vendno = Request["vendno"];
                addobj.vcno = vcno;
                addobj.vserno = 1;
                addobj.vstatus = "0";
                addobj.vctype = "P";
                addobj.vcsubtype = Request["vcsubtype"];
                if(Request["vcsubtype"].ToString()=="2")
                {
                    addobj.payman = Session["empid"].ToString();
                }
                addobj.costtype = Request["costtype"];
                addobj.paytype = Request["paytype"];
                if (Request["spdate"] != "")
                {
                    addobj.spdate = DateTime.Parse(Request["spdate"]);
                }
                else
                {
                    addobj.spdate = null;
                }

                if (Request["vpdate"] != "")
                {
                    addobj.vpdate = DateTime.Parse(Request["vpdate"]);
                }
                else
                {
                    addobj.vpdate = null;
                }
                addobj.ownman = Session["empid"].ToString();
                addobj.owndptid = Session["dptid"].ToString();
                addobj.paycomment = Request["paycomment"].Trim();
                addobj.delvcno = Request["delvcno"].Trim();
                addobj.currency = Request["currency"];
                addobj.totalmoney = Decimal.Parse(Request["totalmoney"]);
                addobj.vcomment = Request["vcomment"];
                addobj.othercomment = Request["othercomment"];
                addobj.bmodid = Session["empid"].ToString();
                addobj.bmoddate = DateTime.Now;

                con.vend_contractinv.Add(addobj);

                con.SaveChanges();
                con.Dispose();

                //系統LOG檔 //================================================= //
                SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                string sysrealsid = Request["sysrealsid"].ToString();
                string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                string sysnote = "單號:" + vcno + "<br>申請人:" + Session["empid"].ToString() + "<br>申請日期:" + DateTime.Parse(Request["vadate"].ToString()).ToString("yyyy/MM/dd") ;
                dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                sysconn.Close();
                sysconn.Dispose();
                //=================================================


            }

         
        


            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            if (Request["vcsubtype"].ToString()=="3")
            {
                tmpform += "<form name='qfr1' action='/paybill/expense5list' method='post'>";
            }
            else { 
            tmpform += "<form name='qfr1' action='/paybill/expense2dp?vcno=" + vcno + "' method='post'>";
            }
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            //tmpform += "<input type=hidden id='qvcno' name='qvcno' value='" + qempname + "'>";
            tmpform += "</form>";
            tmpform += "</body>";

            conn.Close();
            conn.Dispose();

            return new ContentResult() { Content = @"" + tmpform };
        }

        public ActionResult expensedet2adddo(string sysflag, int? page, string orderdata, string orderdata1)
        {
            //string uaccount = Request["UserAccount"];
            NDcommon dbobj = new NDcommon();
            SqlConnection erpconn = dbobj.get_conn("AitagBill_DBContext");
            SqlCommand cmd = new SqlCommand();
            string vcno = "";

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {

                page = ((!page.HasValue || page < 1) ? 1 : page);
                ViewBag.page = page;
                ViewBag.orderdata = orderdata;
                ViewBag.orderdata1 = orderdata1;
                string sqlstr = "";
                string cdel1 = Request["vctid"];
                string allcomid1 = Request["allcomid"];
                string wcardno1 = Request["wcardno"];
                string dptid1 = Request["dptid"];
                string psummoney1 = Request["psummoney"];
                string dealdate1 = Request["dealdate"];
                string fromplace1 = Request["fromplace"];
                string toplace1 = Request["toplace"];
                string pcomment1 = Request["pcomment"];
                string planemoney1 = Request["planemoney"];
                string carmoney1 = Request["carmoney"];
                string othercarmoney1 = Request["othercarmoney"];
                string livemoney1 = Request["livemoney"];
                string eatmoney1 = Request["eatmoney"];
                string othermoney1 = Request["othermoney"];
                string otherbill1 = Request["otherbill"];
                string itemcode1 = Request["itemcode"];
                string pobill1 = Request["pobill"];
                string[] cdelarr = cdel1.Split(',');
                string[] allcomidarr = allcomid1.Split(',');
                string[] wcardnoarr = wcardno1.Split(',');
                string[] dptidarr = dptid1.Split(',');
                string[] psummoneyarr = psummoney1.Split(',');
                string[] dealdatearr = dealdate1.Split(',');
                string[] fromplacearr = fromplace1.Split(',');
                string[] toplacearr = toplace1.Split(',');
                string[] pcommentarr = pcomment1.Split(',');
                string[] planemoneyarr = planemoney1.Split(',');
                string[] carmoneyarr = carmoney1.Split(',');
                string[] othercarmoneyarr = othercarmoney1.Split(',');
                string[] livemoneyarr = livemoney1.Split(',');
                string[] eatmoneyarr = eatmoney1.Split(',');
                string[] othermoneyarr = othermoney1.Split(',');
                string[] otherbillarr = otherbill1.Split(',');
                string[] itemcodearr = itemcode1.Split(',');
                string[] pobillarr = pobill1.Split(',');

                vcno = Request["vcno"].ToString();
                // vend_contractinv Userobj = con.vend_contractinv.Where(r => r.UserAccount == uaccount).FirstOrDefault();
                for (int i = 0; i < cdelarr.Length; i++) {
                    if (cdelarr[i].Trim() == "") {
                        if (!(wcardnoarr[i].Trim() == "" && dptidarr[i].Trim() == ""))
                        {
                            if (psummoneyarr[i].Trim() != "") { 
                            vend_contractinv_det addobj = new vend_contractinv_det();
                            addobj.vcinvid = int.Parse(dbobj.get_vcinvid(erpconn, Request["comid"].ToString(), vcno));
                            addobj.comid = Request["comid"].ToString();
                            addobj.vcno = vcno;
                            addobj.vserno = 1;
                            addobj.pcount = 1;
                            addobj.pmoney = Decimal.Parse(psummoneyarr[i].ToString());
                            addobj.punit = "";
                            addobj.allcomid = allcomidarr[i].Trim();
                            addobj.wcardno = wcardnoarr[i].Trim();
                            addobj.dptid = dptidarr[i].Trim();
                            addobj.psummoney = Decimal.Parse(psummoneyarr[i].ToString());
                            addobj.pcomment = pcommentarr[i].Trim();
                            addobj.bmodid = Session["empid"].ToString();
                            addobj.bmoddate = DateTime.Now;
                            if(dealdatearr[i].ToString()!="")
                            addobj.dealdate = DateTime.Parse(dealdatearr[i].ToString());
                            else
                            addobj.dealdate = null;
                            addobj.fromplace = fromplacearr[i].Trim();
                            addobj.toplace = toplacearr[i].Trim();
                            addobj.planemoney = planemoneyarr[i].Trim();
                            addobj.carmoney = carmoneyarr[i].Trim();
                            addobj.othercarmoney = othercarmoneyarr[i].Trim();
                            addobj.livemoney = livemoneyarr[i].Trim();
                            addobj.eatmoney = eatmoneyarr[i].Trim();
                            addobj.othermoney = othermoneyarr[i].Trim();
                            addobj.otherbill = otherbillarr[i].Trim();
                            addobj.itemcode = itemcodearr[i].Trim();
                            addobj.pobill = pobillarr[i].Trim();
                            con.vend_contractinv_det.Add(addobj);
                            con.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        if (wcardnoarr[i].Trim() != "" || dptidarr[i].Trim() != "")
                        {
                            if (dealdatearr[i].Trim()!="")
                                sqlstr = "update vend_contractinv_det set allcomid = '" + allcomidarr[i].Trim() + "', pmoney = " + psummoneyarr[i].Trim() + ",psummoney=" + psummoneyarr[i].Trim() + ",wcardno='" + wcardnoarr[i].Trim() + "',dptid='" + dptidarr[i].Trim() + "',pcomment='" + pcommentarr[i].Trim() + "',dealdate='" + dealdatearr[i].Trim() + "',fromplace='" + fromplacearr[i].Trim() + "',toplace='" + toplacearr[i].Trim() + "'";
                            else
                                sqlstr = "update vend_contractinv_det set allcomid = '" + allcomidarr[i].Trim() + "', pmoney = " + psummoneyarr[i].Trim() + ",psummoney=" + psummoneyarr[i].Trim() + ",wcardno='" + wcardnoarr[i].Trim() + "',dptid='" + dptidarr[i].Trim() + "',pcomment='" + pcommentarr[i].Trim() + "',dealdate=null ,fromplace='" + fromplacearr[i].Trim() + "',toplace='" + toplacearr[i].Trim() + "'";

                            sqlstr += ",planemoney='" + planemoneyarr[i].Trim() + "',carmoney='" + carmoneyarr[i].Trim() + "',othercarmoney='" + othercarmoneyarr[i].Trim() + "',livemoney='" + livemoneyarr[i].Trim() + "',eatmoney='" + eatmoneyarr[i].Trim() + "',othermoney='" + othermoneyarr[i].Trim() + "',otherbill='" + otherbillarr[i].Trim() + "',itemcode='" + itemcodearr[i].Trim() + "',pobill='" + pobillarr[i].Trim() + "'";
                            sqlstr += " where vctid = '" + cdelarr[i].Trim() + "'";
                            cmd.CommandText = sqlstr;
                            cmd.Connection = erpconn;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                con.Dispose();


            }

            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/paybill/expense2dp?vcno=" + vcno + "' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden name='comid' id='comid' value='" + Request["comid"].ToString() + "'>";
            // tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
            tmpform += "</form>";
            tmpform += "</body>";

            erpconn.Close();
            erpconn.Dispose();

            return new ContentResult() { Content = @"" + tmpform };
        }

        public ActionResult expense2mod()
        {
            //Employee col = new Employee();
            ViewBag.vcno = Request["vcno"].ToString();
            return View();
        }

        public ActionResult expenserep()
        {
            //Employee col = new Employee();
            ViewBag.vcno = Request["vcno"].ToString();
            return View();
        }

        public ActionResult expense2moddo(string sysflag, int? page, string orderdata, string orderdata1)
        {
            string rdlink = "";
            string vcno = Request["vcno"].ToString();
            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                vend_contractinv modobj = con.vend_contractinv.Where(r => r.vcno == vcno).FirstOrDefault();

                rdlink = dbobj.get_billlink("P", modobj.vcsubtype);

                //string vcno = "1";
                //UserId是key值
                //  Userobj.UserId = Uid;
                modobj.vadate = DateTime.Parse(Request["vadate"].ToString());
                modobj.comid = Request["comid"];
                modobj.vendno = Request["vendno"];
                //modobj.vcno = vcno;
               // modobj.vserno = 1;
               //  modobj.vstatus = "0";
               // modobj.vctype = "P";
              //  modobj.ownman = Session["empid"].ToString();
              //  modobj.owndptid = Session["dptid"].ToString();
                modobj.costtype = Request["costtype"];
                modobj.currency = Request["currency"];
                modobj.totalmoney = int.Parse(Request["totalmoney"]);
                modobj.vcomment = Request["vcomment"];
                modobj.othercomment = Request["othercomment"];
                modobj.bmodid = Session["empid"].ToString();
                modobj.bmoddate = DateTime.Now;
                if (Request["spdate"].ToString() != "")
                    modobj.spdate = DateTime.Parse(Request["spdate"].Trim());
                else
                    modobj.spdate = null;
                modobj.paytype = Request["paytype"];
                modobj.paycomment = Request["paycomment"].Trim();
                modobj.delvcno = Request["delvcno"].Trim();                
                con.Entry(modobj).State = EntityState.Modified;
                con.SaveChanges();
                con.Dispose();


                //系統LOG檔 //================================================= //
                SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                string sysrealsid = Request["sysrealsid"].ToString();
                string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                string sysnote = "單號:" + vcno + "<br>申請人:" + modobj.ownman + "<br>申請日期:" + DateTime.Parse(Request["vadate"].ToString()).ToString("yyyy/MM/dd");
                dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), "E");
                sysconn.Close();
                sysconn.Dispose();
                //=================================================


            }
            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/paybill/" + rdlink + "' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            // tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
            tmpform += "</form>";
            tmpform += "</body>";


            return new ContentResult() { Content = @"" + tmpform };
        }

         [ValidateInput(false)]
        public ActionResult expense2workflowdo(string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            string tmparolestampid = "";
            string tmprole = "";
            string tmpbillid = "";
            NDcommon dbobj = new NDcommon();

            int vcinvid = int.Parse(Request["vcinvid"].ToString());

            string actionlink = "";
            #region 寄信參數
            string bccemail = "";
            string tmpstype = "";  //單據別
            string tmpsno = "";  //請款單號
            string tmpdate = "";//申請日期        
            string tmpnote = "";//摘要說明
            string tmpownman = "";//申請人
            string tmpmtitle = "";
            #endregion


            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                vend_contractinv modobj = con.vend_contractinv.Where(r => r.vcinvid == vcinvid).FirstOrDefault();

            if (Request["vstatus"].ToString() == "0")  //第一次送時,補件修正不用
            {
                if (Request["arolestampid"].ToString() != "")
                    tmparolestampid = "'" + Request["arolestampid"].ToString() + "'";
                else
                    tmparolestampid = "'" + Request["arolestampid1"].ToString() + "'";

                string tmpmoney = Request["totalmoney"].ToString();
                //找出下一個角色是誰               
                string impallstring = dbobj.getnewcheck1("P", tmparolestampid, tmparolestampid, tmpmoney, "", "", modobj.vcsubtype.ToString());

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

           

                actionlink = dbobj.get_billlink("P", modobj.vcsubtype);

                #region 寄信參數值
                //===============               
                System.Data.SqlClient.SqlConnection tmpconn1 = dbobj.get_conn("Aitag_DBContext");
                tmpstype = dbobj.get_billname("P", modobj.vcsubtype);
                tmpsno = modobj.vcno;
                tmpdate = dbobj.get_date(modobj.vadate.ToString(), "1");
               // tmpvendno = modobj.vendno + "－" + dbobj.get_dbvalue(tmpconn1, "select comsttitle from allcompany where comid = '" + modobj.vendno + "'");
               // tmpamoney = modobj.totalmoney.ToString();
                tmpnote = modobj.vcomment;
                tmpownman = modobj.ownman;
                bccemail = dbobj.get_dbvalue(tmpconn1, "select enemail from employee where empstatus <> '4' and empid = '" + tmpownman + "'");
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

                mailtitle = tmpstype + "申請要求審核通知" + tmpmtitle;
                txt_comment = "有一筆申請單提出，請至系統進行審核作業。<br>資料如下：";

                mailcontext += "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=450 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                mailcontext += "<tr>";
                mailcontext += "<td colspan=2>" + txt_comment + "</td>";
                mailcontext += "</tr>";

                mailcontext += "<tr>";
                mailcontext += "<td align=right width='90'>單據別：</td>";
                mailcontext += "<td>" + tmpstype + "</td>";
                mailcontext += "</tr>";

                mailcontext += "<tr>";
                mailcontext += "<td align=right>請款單號：</td>";
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
                string sqlstr = "select employee.enemail FROM emprole INNER JOIN employee ON emprole.empid =employee.empid where emprole.rid =" + tmprole + " and empstatus <> '4' and employee.enemail <>''";
                sqlsmd.CommandText = sqlstr;
                dr = sqlsmd.ExecuteReader();
                string tomail = "";
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



            //系統LOG檔 //================================================= //
            SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
            string sysrealsid = Request["sysrealsid"].ToString();
            string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2") + "(送審)";
            string sysnote = "單號:" + tmpsno + "<br>申請人:" + tmpownman + "<br>申請日期:" + tmpdate ;
            dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), "E");
            sysconn.Close();
            sysconn.Dispose();
            //=================================================


            string tmpform = "";
            tmpform = "<body onload=javascript:alert('送出審核成功!!');qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/paybill/" + actionlink+ "' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            // tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
            tmpform += "</form>";
            tmpform += "</body>";


            return new ContentResult() { Content = @"" + tmpform };
        }

        public ActionResult 
            expenseallsend(int? page, string orderdata, string orderdata1)
        {
            NDcommon dbobj = new NDcommon();
            string actionlink = dbobj.get_billlink("P", Request["vcsubtype"].ToString());
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
                string tmpstype = "";  //單據別
                    string[] condtionArr = cdel.Split(',');
                    int condtionLen = condtionArr.Length;
                    for (int i = 0; i < condtionLen; i++)
                    {

                        #region 寄信參數
                        string bccemail = "";
                        tmpstype = "";  //單據別
                        string tmpsno = "";  //請款單號
                        string tmpdate = "";//申請日期        
                        string tmpnote = "";//摘要說明
                        string tmpownman = "";//申請人                     
                        #endregion 

                        int vcinvid = int.Parse(condtionArr[i].ToString());

                        #region 找值並異動資料庫
                       
                        using (AitagBill_DBContext con = new AitagBill_DBContext())
                        {
                            vend_contractinv modobj = con.vend_contractinv.Where(r => r.vcinvid == vcinvid).FirstOrDefault();
                            string tmpmoney = modobj.totalmoney.ToString();

                            //找出下一個角色是誰               
                            string impallstring = dbobj.getnewcheck1("P", tmparolestampid, tmparolestampid, tmpmoney, "", "", modobj.vcsubtype);

                            string[] tmpstrarr = impallstring.Split(';');
                            tmprole = tmpstrarr[0].ToString();
                            tmpbillid = tmpstrarr[1].ToString();
                            if (tmprole != "")
                            {
                                #region 寄信參數值
                                //===============               
                                System.Data.SqlClient.SqlConnection tmpconn1 = dbobj.get_conn("Aitag_DBContext");
                                tmpstype = dbobj.get_billname("P", modobj.vcsubtype);
                                tmpsno = modobj.vcno;
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

                                mailtitle = tmpstype + "申請要求審核通知";
                                txt_comment = "有一筆申請單提出，請至系統進行審核作業。<br>資料如下：";

                                mailcontext += "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=450 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                                mailcontext += "<tr>";
                                mailcontext += "<td colspan=2>" + txt_comment + "</td>";
                                mailcontext += "</tr>";

                                mailcontext += "<tr>";
                                mailcontext += "<td align=right width='90'>單據別：</td>";
                                mailcontext += "<td>" + tmpstype + "</td>";
                                mailcontext += "</tr>";

                                mailcontext += "<tr>";
                                mailcontext += "<td align=right>請款單號：</td>";
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
                                string tomail = "";
                                /*while (dr.Read())
                                {
                                    dbobj.send_mailfile("", dr["enemail"].ToString(), mailtitle, mailcontext, null, null);
                                }*/

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
                            { errmsg += "【"+modobj.vcno +"】、"; }

     
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
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2") +"(群送)";
                        string  sysflag = "E";
                        string sysnote = "單據別：" + tmpstype + "<br>單號：" + allsno;
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
                    tmpform += "<form name='qfr1' action='/paybill/" + actionlink+ "' method='post'>";                  
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";         
                    tmpform += "</form>";
                    tmpform += "</body>";
            }

            return new ContentResult() { Content = @"" + tmpform };
        }


         

       /* public ActionResult expense2repdo(string sysflag, int? page, string orderdata, string orderdata1)
        {
            //Employee col = new Employee();
            int vcinvid = int.Parse(Request["vcinvid"].ToString());

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                vend_contractinv modobj = con.vend_contractinv.Where(r => r.vcinvid == vcinvid).FirstOrDefault();
                // = con.User.Find(Uid);
                string vcno = "1";
                //UserId是key值
                //  Userobj.UserId = Uid;
                modobj.vadate = DateTime.Parse(Request["vadate"].ToString());
                modobj.comid = Request["comid"];
                modobj.vendno = Request["vendno"];
                //modobj.vcno = vcno;
                modobj.vserno = 1;
                modobj.vstatus = "0";
                modobj.vctype = "3";
                modobj.ownman = Session["empid"].ToString();
                modobj.owndptid = Session["dptid"].ToString();
                modobj.costtype = Request["costtype"];
                modobj.currency = Request["currency"];
                modobj.totalmoney = int.Parse(Request["totalmoney"]);
                modobj.vcomment = Request["vcomment"];
                modobj.othercomment = Request["othercomment"];
                modobj.bmodid = Session["empid"].ToString();
                modobj.bmoddate = DateTime.Now;
                //Userobj.AddDate = DateTime.Today;
                //Userobj.Enable = Guid.Parse("1edfb680-4e9f-4f05-85fd-3ce460fffdb2");
                //Userobj.CreateDate = DateTime.Today;
                //con.User.Add(Userobj);
                //con.UserInRole.Add(UserInRoleobj);
                con.Entry(modobj).State = EntityState.Modified;
                con.SaveChanges();
                con.Dispose();
            }
            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/paybill/expense2list' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            // tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
            tmpform += "</form>";
            tmpform += "</body>";


            return new ContentResult() { Content = @"" + tmpform };
        }*/

        public ActionResult expense3list(int? page, string orderdata, string orderdata1)
        {
            IPagedList<vend_contractinv> result;
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
                sqlstr = "select * from vend_contractinv where  vctype = 'P' and vcsubtype='5' and comid = '" + Session["comid"].ToString() + "' and ownman = '" + Session["empid"].ToString() + "'   and";
                if (qvcno != "")
                    sqlstr += " vcno like '%" + qvcno + "%'  and";
                if (qvendno != "")
                    sqlstr += " vendno in (select comid from allcompany where comid like '%" + qvendno + "%' or comtitle like '%" + qvendno + "%')   and";
                if (qvcomment != "")
                    sqlstr += " vcomment like '%" + qvcomment + "%'  and";
                if (qsvadate != "")
                    sqlstr += " vadate >= '" + qsvadate + "'  and";
                if (qevadate != "")
                    sqlstr += " vadate <= '" + qevadate + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.vend_contractinv.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<vend_contractinv>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);
        }


        public ActionResult expense4list(int? page, string orderdata, string orderdata1)
        {
            IPagedList<vend_contractinv> result;
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
                sqlstr = "select * from vend_contractinv where  vctype = 'P' and vcsubtype='4'  and comid = '" + Session["comid"].ToString() + "' and ownman = '" + Session["empid"].ToString() + "'   and";
                if (qvcno != "")
                    sqlstr += " vcno like '%" + qvcno + "%'  and";
                if (qvendno != "")
                    sqlstr += " vendno in (select comid from allcompany where comid like '%" + qvendno + "%' or comtitle like '%" + qvendno + "%')   and";
                if (qvcomment != "")
                    sqlstr += " vcomment like '%" + qvcomment + "%'  and";
                if (qsvadate != "")
                    sqlstr += " vadate >= '" + qsvadate + "'  and";
                if (qevadate != "")
                    sqlstr += " vadate <= '" + qevadate + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.vend_contractinv.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<vend_contractinv>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);
        }

      

        public ActionResult expense5list(int? page, string orderdata, string orderdata1)
        {
            IPagedList<vend_contractinv> result;
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
                sqlstr = "select * from vend_contractinv where  vctype = 'P' and vcsubtype='3'  and comid = '" + Session["comid"].ToString() + "' and ownman = '" + Session["empid"].ToString() + "'   and";
                if (qvcno != "")
                    sqlstr += " vcno like '%" + qvcno + "%'  and";
                if (qvendno != "")
                    sqlstr += " vendno in (select comid from allcompany where comid like '%" + qvendno + "%' or comtitle like '%" + qvendno + "%')   and";
                if (qvcomment != "")
                    sqlstr += " vcomment like '%" + qvcomment + "%'  and";
                if (qsvadate != "")
                    sqlstr += " vadate >= '" + qsvadate + "'  and";
                if (qevadate != "")
                    sqlstr += " vadate <= '" + qevadate + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.vend_contractinv.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<vend_contractinv>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);
        }

       
        public ActionResult expense6list(int? page, string orderdata, string orderdata1)
        {
            IPagedList<vend_contractinv> result;
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
                sqlstr = "select * from vend_contractinv where  vctype = 'P' and vcsubtype='7' and comid = '" + Session["comid"].ToString() + "' and ownman = '" + Session["empid"].ToString() + "'   and";
                if (qvcno != "")
                    sqlstr += " vcno like '%" + qvcno + "%'  and";
                if (qvendno != "")
                    sqlstr += " vendno in (select comid from allcompany where comid like '%" + qvendno + "%' or comtitle like '%" + qvendno + "%')   and";
                if (qvcomment != "")
                    sqlstr += " vcomment like '%" + qvcomment + "%'  and";
                if (qsvadate != "")
                    sqlstr += " vadate >= '" + qsvadate + "'  and";
                if (qevadate != "")
                    sqlstr += " vadate <= '" + qevadate + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.vend_contractinv.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<vend_contractinv>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);
        }

       
        public ActionResult expense7list(int? page, string orderdata, string orderdata1)
        {
            IPagedList<vend_contractinv> result;
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
                sqlstr = "select * from vend_contractinv where  vctype = 'P' and vcsubtype='8' and comid = '" + Session["comid"].ToString() + "' and ownman = '" + Session["empid"].ToString() + "'   and";
                if (qvcno != "")
                    sqlstr += " vcno like '%" + qvcno + "%'  and";
                if (qvendno != "")
                    sqlstr += " vendno in (select comid from allcompany where comid like '%" + qvendno + "%' or comtitle like '%" + qvendno + "%')   and";
                if (qvcomment != "")
                    sqlstr += " vcomment like '%" + qvcomment + "%'  and";
                if (qsvadate != "")
                    sqlstr += " vadate >= '" + qsvadate + "'  and";
                if (qevadate != "")
                    sqlstr += " vadate <= '" + qevadate + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.vend_contractinv.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<vend_contractinv>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);
        }

        public ActionResult expense8list(int? page, string orderdata, string orderdata1)
        {
            IPagedList<vend_contractinv> result;
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
                sqlstr = "select * from vend_contractinv where  vctype = 'P' and vcsubtype='6' and comid = '" + Session["comid"].ToString() + "' and ownman = '" + Session["empid"].ToString() + "'   and";
                if (qvcno != "")
                    sqlstr += " vcno like '%" + qvcno + "%'  and";
                if (qvendno != "")
                    sqlstr += " vendno in (select comid from allcompany where comid like '%" + qvendno + "%' or comtitle like '%" + qvendno + "%')   and";
                if (qvcomment != "")
                    sqlstr += " vcomment like '%" + qvcomment + "%'  and";
                if (qsvadate != "")
                    sqlstr += " vadate >= '" + qsvadate + "'  and";
                if (qevadate != "")
                    sqlstr += " vadate <= '" + qevadate + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.vend_contractinv.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<vend_contractinv>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);
        }

        public ActionResult expensechkmod()
        {
            //Employee col = new Employee();
            ViewBag.id = Request["vcinvid"].ToString();
            return View();
        }

        public ActionResult expensechkmoddo(string sysflag, int? page, string orderdata, string orderdata1)
        {
            NDcommon dbobj = new NDcommon();
            SqlConnection conn0 = dbobj.get_conn("AitagBill_DBContext");
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn0;
            int vcinvid = int.Parse(Request["vcinvid"].ToString());
            string sqlstr = "select * from vend_contractinv_det where vcinvid in (" + vcinvid + ") and (itemcode = '' or itemcode is null)";
            cmd.CommandText = sqlstr;
            SqlDataReader alldr = cmd.ExecuteReader();
            if (Session["rid"].ToString() == "G010" || Session["rid"].ToString() == "A01511")
            { 
                if (alldr.HasRows)
                {
                    return new ContentResult() { Content = @"<body onload=javascript:alert('表單內分攤明細之歸帳代號並未設定!!');window.history.go(-1);>" };
                }
            }
            alldr.Close();
            alldr.Dispose();

            string vstatus = Request["vstatus"].ToString();
            string tmpstatus = "";
            string tmprback = Request["rback"].ToString();
            
            string tmprolestampid = Request["rolestampid"].ToString(); //目前簽核角色(一個)           
            string billflowid = Request["billflowid"].ToString();
            string rolea_1 = Request["rolestampidall"].ToString();
            
            string roleall = "";
            roleall = rolea_1 + "," + tmprolestampid; //簽核過角色(多個)
            string tmprole = "";

            #region 寄信參數    
                string tmpvcsubtype = "";
                string tmpowndptid = "";
                string bccemail = "";
                string tmpstype = "";  //單據別
                string tmpsno = "";  //請款單號
                string tmpdate = "";//申請日期
               // string tmpvendno = "";//廠商
               // string tmpamoney = "";//請款金額
                string tmpnote = "";//摘要說明
                string tmpownman = "";//申請人
            #endregion 
            if (vstatus == "1")
            {
                #region  通過時

                //找出下一個角色是誰

                tmprole = dbobj.getnewcheck1("P", tmprolestampid, roleall, "0", "", billflowid);

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
                tmpstatus = vstatus;
            }
                using (AitagBill_DBContext con = new AitagBill_DBContext())
                {
                    #region 寄信參數值                    
                        //===============
                        vend_contractinv modobj = con.vend_contractinv.Where(r => r.vcinvid == vcinvid).FirstOrDefault();
                        System.Data.SqlClient.SqlConnection tmpconn1 = dbobj.get_conn("Aitag_DBContext");
                        tmpstype = dbobj.get_billname("P", modobj.vcsubtype);
                        tmpsno = modobj.vcno;
                        tmpdate = dbobj.get_date(modobj.vadate.ToString(), "1");
                        //tmpvendno = modobj.vendno +"－" + dbobj.get_dbvalue(tmpconn1, "select comsttitle from allcompany where comid = '" + modobj.vendno + "'");
                        //tmpamoney = modobj.totalmoney.ToString();
                        tmpnote = modobj.vcomment;
                        if (modobj.paytype == "3" || modobj.paytype == "4")
                        tmpownman = modobj.ownman;
                        else
                        tmpownman = modobj.vendno;

                        tmpvcsubtype = modobj.vcsubtype;
                        tmpowndptid = modobj.owndptid;
                        bccemail = dbobj.get_dbvalue(tmpconn1, "select enemail from employee where empid = '" + tmpownman + "'");
                        tmpconn1.Close();
                        tmpconn1.Dispose();
                        //===============
                    #endregion
                    //呈核人員
                    //======================             
                    modobj.vstatus = tmpstatus;
                    if (vstatus == "1")
                    { modobj.rolestampid = tmprole;  //下個呈核角色

                    modobj.rolestampidall = roleall; //所有呈核角色
                    modobj.empstampidall = modobj.empstampidall + ",'" + Session["empid"].ToString() + "'"; //所有人員帳號
                    modobj.billtime = modobj.billtime + "," + DateTime.Now.ToString(); //所有時間
                    if (Request["exppaydate"] != null)
                    {
                        if (Request["exppaydate"].ToString()!="")
                        { 
                        modobj.exppaydate = DateTime.Parse(Request["exppaydate"].ToString());
                        }
                    }
                    modobj.bmodid = Session["empid"].ToString();
                    modobj.bmoddate = DateTime.Now;
                    }

                    if (vstatus == "D")
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
                        if (vstatus == "3")
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

                #region 審核通過時，產生傳票
                if (tmpstatus == "2")
                {
                    SqlConnection conn = dbobj.get_conn("AitagBill_DBContext");
                    SqlConnection conn1 = dbobj.get_conn("AitagBill_DBContext");
                    string paytype = "" ,costtype = "" , ownman="" , paydate="";
                    string subjectcode = "";
                    string subjectname = "";
                    string vcsubtype = "";
                    string delvcno = "";
                    decimal allmoney = 0;

                        using (AitagBill_DBContext con = new AitagBill_DBContext())
                        {
                            #region 多筆分擹借項
                                 SqlCommand sqlsmd = new SqlCommand();
                                sqlsmd.Connection = conn;
                                accountvoucher col;
                                sqlsmd.CommandText = "select * from  view_vend_contractinv_det where vcno = '" + tmpsno + "' and comid = '" + Session["comid"] + "' and itemcode <>'' and itemcode IS NOT NULL";
                                SqlDataReader dr = sqlsmd.ExecuteReader();
                                int itemno = 10;
                                
                                if (tmpvcsubtype == "3")
                                {
                                    dr.Close();
                                    dr.Dispose();
                                    sqlsmd.CommandText = "select * from  vend_contractinv where vcno = '" + tmpsno + "' and comid = '" + Session["comid"] + "'";
                                    dr = sqlsmd.ExecuteReader();
                                    if(dr.Read())
                                    {
                                    costtype = dr["costtype"].ToString();
                                    ownman = dr["ownman"].ToString();
                                    subjectcode = "1470073";
                                    subjectname = "暫付款-員工";
                                    col = new accountvoucher();
                                    col.billtype = "P";
                                    col.costtype = costtype;
                                    col.ownman = ownman;
                                    col.vcsubtype = tmpvcsubtype;
                                    col.vcsno = tmpsno;
                                    col.deliveryno = 1;
                                    col.pinvdate = DateTime.Parse(tmpdate);
                                    col.itemno = itemno;
                                    col.subjectcode = subjectcode;
                                    col.subjectname = subjectname;
                                    col.cedeptcode = tmpowndptid;
                                    col.objno = tmpownman;
                                    col.debitsum = decimal.Parse(dr["totalmoney"].ToString());
                                    col.creditsum = 0;

                                    paydate = dbobj.get_date(DateTime.Parse(dr["vpdate"].ToString()).AddDays(15).ToString(), "1");
                                    //    dbobj.get_date(DateTime.Parse(dr["vpdate"].ToString()).AddDays(15).ToString(),"1")
                                    //預計付款日
                                    col.ceenddate = DateTime.Parse(dr["vpdate"].ToString()).AddDays(15) ;
                                    col.cebillcode = "";
                                    col.vouabstract = dr["vcomment"].ToString();
                                    //col.baddid = Session["tempid"].ToString();
                                    col.comid = Session["comid"].ToString();
                                    col.bmodid = Session["empid"].ToString();
                                    //col.badddate = DateTime.Now;
                                    col.bmoddate = DateTime.Now;

                                    allmoney += decimal.Parse(dr["totalmoney"].ToString());

                                    itemno = itemno + 10;
                                    con.accountvoucher.Add(col);
                                    con.SaveChanges();
                                    }
                                }
                                else
                                { 
                                        while (dr.Read())
                                        {
                                            //借方金額
                                            paytype = dr["paytype"].ToString();
                                            costtype =  dr["costtype"].ToString();
                                            ownman = dr["ownman"].ToString();
                                            vcsubtype = dr["vcsubtype"].ToString();
                                            delvcno = dr["delvcno"].ToString();
                                           
                                                subjectcode = dbobj.get_dbvalue(conn1, "select  subjectcode from billsubject where itemcode = '" + dr["itemcode"].ToString() + "' and comid = '" + dr["comid"].ToString() + "'");
                                                subjectname = dbobj.get_dbvalue(conn1, "select subjecttitle from billsubject where itemcode = '" + dr["itemcode"].ToString() + "' and comid = '" + dr["comid"].ToString() + "'");
                                                col = new accountvoucher();
                                                col.costtype = costtype;
                                                col.ownman = ownman;
                                                col.billtype = "P";
                                                col.vcsubtype = tmpvcsubtype;
                                                col.vcsno = tmpsno;
                                                col.deliveryno = 1;
                                                col.pinvdate = DateTime.Parse(tmpdate);
                                                col.itemno = itemno;
                                                col.subjectcode = subjectcode;
                                                col.subjectname = subjectname;
                                                col.cedeptcode = dr["dptid"].ToString();
                                               // col.objno = tmpownman;
                                                col.debitsum = decimal.Parse(dr["pmoney"].ToString());
                                                col.creditsum = 0;
                                    
                                                //卡號
                                                col.projno = dr["wcardno"].ToString();
                                                //客戶
                                                if (paytype == "1"){
                                                col.objno = dr["allcomid"].ToString();
                                                }
                                                else if (paytype == "2")
                                                {
                                                col.objno = dr["ownman"].ToString();
                                                }
                                                else if (paytype == "3")
                                                {
                                                col.objno = dr["allcomid"].ToString();
                                                }
                                                else if (paytype == "4")
                                                {
                                                    col.objno = dr["ownman"].ToString();
                                                }
                                                else
                                                {
                                                    col.objno = dr["ownman"].ToString();
                                                }
                                                //預計付款日
                                                paydate = dbobj.get_date(dr["exppaydate"].ToString(), "1");
                                                col.ceenddate = DateTime.Parse(dr["exppaydate"].ToString());
                                                col.cebillcode = dr["delvcno"].ToString();
                                                col.vouabstract = dr["vcomment"].ToString();
                                                //col.baddid = Session["tempid"].ToString();
                                                col.comid = Session["comid"].ToString();
                                                col.bmodid = Session["empid"].ToString();
                                                //col.badddate = DateTime.Now;
                                                col.bmoddate = DateTime.Now;

                                                allmoney += decimal.Parse(dr["pmoney"].ToString());

                                                itemno = itemno + 10;
                                                con.accountvoucher.Add(col);
                                                con.SaveChanges();
                                            }
                                }
                                dr.Close();
                                dr.Dispose();
                           #endregion

                                string taxmoney = "" , invno = "";
                                taxmoney = dbobj.get_dbvalue(conn1, "select isnull(sum(taxmoney),0) as taxmoney from incomeaccounts where vcno = '" + tmpsno + "' and comid = '" + Session["comid"] + "'") + "";
                                dr = dbobj.dbselect(conn1, "select invno from incomeaccounts where vcno = '" + tmpsno + "' and comid = '" + Session["comid"] + "'");
                                while(dr.Read())
                                {
                                    invno += dr["invno"].ToString() + ",";
                                }
                                dr.Close();
                                dr.Dispose();
                                    //taxmoney = dbobj.get_dbvalue(conn1, "select invno from incomeaccounts where vcno = '" + tmpsno + "'") + "";
                                if (taxmoney == "")
                                { taxmoney = "0"; }

                                if (allmoney > 0)
                                {
                                    
                                    #region 一筆借進項稅額  
                                       
                                        if (decimal.Parse(taxmoney) > 0)
                                        {
                                            itemno = itemno + 10;
                                            col = new accountvoucher();
                                            col.cebillcode = invno;
                                            col.costtype = costtype;
                                            col.ownman = ownman;
                                            col.billtype = "P";
                                            col.vcsubtype = tmpvcsubtype;
                                            col.vcsno = tmpsno;
                                            col.deliveryno = 1;
                                            col.pinvdate = DateTime.Parse(tmpdate);
                                            col.itemno = itemno;
                                            col.subjectcode = "1410023";
                                            col.subjectname = "進項稅額";
                                            col.cedeptcode = tmpowndptid;
                                            col.objno = tmpownman;
                                            col.debitsum = decimal.Parse(taxmoney);
                                            col.creditsum = 0;
                                            col.vouabstract = "進項稅額";
                                            //col.baddid = Session["tempid"].ToString();
                                            col.comid = Session["comid"].ToString();
                                            col.bmodid = Session["empid"].ToString();
                                            //col.badddate = DateTime.Now;
                                            col.bmoddate = DateTime.Now;

                                            allmoney += int.Parse(taxmoney);

                                            con.accountvoucher.Add(col);
                                            con.SaveChanges();
                                        }
                      
                                    #endregion


                                    #region 一筆貸方金額
                                       

                                            itemno = itemno + 10;
                                            col = new accountvoucher();
                                            col.costtype = costtype;
                                            col.ownman = ownman;
                                            col.billtype = "P";
                                            col.vcsubtype = tmpvcsubtype;
                                            col.vcsno = tmpsno;
                                            col.deliveryno = 1;
                                            col.pinvdate = DateTime.Parse(tmpdate);
                                            col.itemno = itemno;
                                            if (tmpvcsubtype != "3")
                                            { 
                                                    if (paytype=="4")
                                                    {
                                                        col.subjectcode = "1470073";
                                                        col.subjectname = "暫付款-員工";
                                                    }
                                                    else if (paytype == "3")
                                                    {
                                                            col.subjectcode = "2191020";
                                                            col.subjectname = "應付電匯款-員工墊款";
                                                    }
                                                    else
                                                    {
                                                        if (subjectcode == "1470073")
                                                        {
                                                            col.subjectcode = "2191020";
                                                            col.subjectname = "應付電匯款-員工墊款";
                                                        }
                                                        else
                                                        {
                                                            if (costtype == "1") { 
                                                                col.subjectcode = "2170071";
                                                                col.subjectname = "應付成本";
                                                            }
                                                            else
                                                            {
                                                                col.subjectcode = "2200019";
                                                                col.subjectname = "其他應付款-其他";
                                                            }
                                                        }
                                                    }
                                            }
                                            else
                                            {
                                                col.subjectcode = "2191020";
                                                col.subjectname = "應付電匯款-員工墊款";
                                            }

                                            col.ceenddate = DateTime.Parse(paydate);
                                            col.cedeptcode = tmpowndptid;
                                            col.objno = tmpownman;
                                            col.debitsum = 0;
                                            col.creditsum = decimal.Parse(allmoney.ToString());
                                            col.vouabstract = col.subjectname;
                                            col.comid = Session["comid"].ToString();
                                            //col.baddid = Session["tempid"].ToString();
                                            col.bmodid = Session["empid"].ToString();
                                            //col.badddate = DateTime.Now;
                                            col.bmoddate = DateTime.Now;

                                            con.accountvoucher.Add(col);
                                            con.SaveChanges();
                                    
                                        #endregion
                                }
                       
                        }
                conn.Close();
                conn.Dispose();

                conn1.Close();
                conn1.Dispose();

                
                }
           　#endregion
        
           

            #region  寄信
                string msgerr = "";           
                string txt_comment = "";
                string mailtitle = "";
                string mailcontext = "";

                    switch (tmpstatus)
                    {
                        case "1"://審核中      
                            mailtitle = tmpstype + "申請要求審核通知";
                            txt_comment = "有一筆申請單提出，請至系統進行審核作業。<br>資料如下：";
                            break;
                        case "2"://已審核        
                            mailtitle = tmpstype + "申請完成審核通知";
                            txt_comment = "您的申請單已通過審核。<br>資料如下：";
                            break;
                        case "3"://退回補正   
                            mailtitle = tmpstype + "申請退回補正通知";
                            txt_comment = "您的申請單有問題，審核者要求您修正。<br>資料如下：";
                            break;
                        case "D"://退回      
                            mailtitle = tmpstype + "申請退回通知";
                            txt_comment = "您的申請單被退回。<br>資料如下：";
                            break;
                    }
   
                        mailcontext += "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=450 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                        mailcontext += "<tr>";
                        mailcontext += "<td colspan=2>" + txt_comment + "</td>";
                        mailcontext += "</tr>";

                        mailcontext += "<tr>";
                        mailcontext += "<td align=right width='90'>單據別：</td>";
                        mailcontext += "<td>" + tmpstype + "</td>";
                        mailcontext += "</tr>";

                        mailcontext += "<tr>";
                        mailcontext += "<td align=right>請款單號：</td>";
                        mailcontext += "<td>" + tmpsno +"</td>";
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
                            tmprback = tmprback.Replace("\r\n","<br>");
                            mailcontext += "<tr>";
                            mailcontext += "<td align=right>原因：</td>";
                            mailcontext += "<td>" + tmprback + "</td>";
                            mailcontext += "</tr>";
                        }
            
                        mailcontext += "</table>";
                        mailcontext += "<br><br><font size='9pt' color=#404040>此為系統自動發信，請勿直接回覆！</font>";      


                 if (tmpstatus=="1")
                 {  //寄給下一個審核者
                     System.Data.SqlClient.SqlConnection tmpconn2 = dbobj.get_conn("Aitag_DBContext");      
                     SqlDataReader dr;
                     SqlCommand sqlsmd = new SqlCommand();
                     sqlsmd.Connection = tmpconn2;
                     sqlstr = "select employee.enemail FROM emprole INNER JOIN employee ON emprole.empid =employee.empid where emprole.rid =" + tmprole + " and employee.empstatus <> '4' and employee.enemail <>''";
                     sqlsmd.CommandText = sqlstr;
                     dr = sqlsmd.ExecuteReader();
                     string tomail = "";
                     while (dr.Read())
                     {
                         tomail += dr["enemail"].ToString() + ",";
                         //dbobj.send_mailfile("", dr["enemail"].ToString(), mailtitle, mailcontext, null, null);
                     }
                     dbobj.send_mailfile("", tomail , mailtitle, mailcontext, null, null);
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
              tmpform += "<form name='qfr1' action='/paybill/expensechk' method='post'>";
              tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
              tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
              tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
              tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
              tmpform += "</form>";
              tmpform += "</body>";
            }
            
          


            return new ContentResult() { Content = @"" + tmpform };
        }

        public ActionResult expensechkall(string sysflag, int? page, string orderdata, string orderdata1)
        {
            NDcommon dbobj = new NDcommon();
            string cdel = Request["cdel"];
           
            SqlConnection conn0 = dbobj.get_conn("AitagBill_DBContext");
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn0;
            string sqlstr = "select * from vend_contractinv_det where vcinvid in (" + cdel + ") and (itemcode = '' or itemcode is null)";
            cmd.CommandText = sqlstr;
            SqlDataReader alldr = cmd.ExecuteReader();
            if (Session["rid"].ToString() == "G010" || Session["rid"].ToString() == "A01511")
            {
                if (alldr.HasRows)
                {
                    return new ContentResult() { Content = @"<body onload=javascript:alert('表單內分攤明細之歸帳代號並未設定!!');window.history.go(-1);>" };
                }
            }
            alldr.Close();
            alldr.Dispose();
            sqlstr = "select * from vend_contractinv where vcinvid in (" + cdel + ")";
            cmd.CommandText = sqlstr;
            alldr = cmd.ExecuteReader();
            while(alldr.Read())
            {
                    //預計付款日取得
                     string date1 = "";
                     string yy  = "" , mm = "";
                     yy = DateTime.Now.Year.ToString();
                     mm = DateTime.Now.Month.ToString();
                     if (alldr["paytype"].ToString() == "1")
                     {
                         if (Decimal.Parse(alldr["totalmoney"].ToString()) <= 10000)
                        {
                           
                            mm = (int.Parse(mm) + 1).ToString();
                            if(int.Parse(mm)>12)
                            {
                                yy = (int.Parse(yy) + 1).ToString();
                                mm = (int.Parse(mm) - 12).ToString();
                            }

                           
                        }
                         else if (Decimal.Parse(alldr["totalmoney"].ToString()) > 10000 && Decimal.Parse(alldr["totalmoney"].ToString()) <= 100000)
                        {
                            mm = (int.Parse(mm) + 2).ToString();
                            if (int.Parse(mm) > 12)
                            {
                                yy = (int.Parse(yy) + 1).ToString();
                                mm = (int.Parse(mm) - 12).ToString();
                            }

                        }
                         else if (Decimal.Parse(alldr["totalmoney"].ToString()) > 100000)
                        {
                            mm = (int.Parse(mm) + 5).ToString();
                            if (int.Parse(mm) > 12)
                            {
                                yy = (int.Parse(yy) + 1).ToString();
                                mm = (int.Parse(mm) - 12).ToString();
                            }
                        }
                         date1 = yy + "/" + mm + "/28";
                    }
                     else if (alldr["paytype"].ToString() == "2")
                     {
                        // if (!string.IsNullOrWhiteSpace(Model.spdate.ToString()))
                        // { 
                        // if (Model != null) { 
                         date1 = dbobj.get_date(alldr["spdate"].ToString(),"1");
                        // }
                    }
                     else if (alldr["paytype"].ToString() == "3")
                    {
                        date1 = dbobj.get_date(DateTime.Now.ToString(),"1");
                    }
                    else
                    {
                        date1 = dbobj.get_date(DateTime.Now.ToString(), "1");
                    }
          
                    int vcinvid = int.Parse(alldr["vcinvid"].ToString());
                    string vstatus = Request["vstatus"].ToString();
                    string tmpstatus = "";
                    //string tmprback = Request["rback"].ToString();

                    string tmprolestampid = Request["rolestampid"].ToString(); //目前簽核角色(一個)           
                    string billflowid = alldr["billflowid"].ToString();
                    string rolea_1 = alldr["rolestampidall"].ToString();
                    string roleall = "";
                    roleall = rolea_1 + "," + tmprolestampid; //簽核過角色(多個)
                    string tmprole = "";

                    #region 寄信參數
                    string tmpvcsubtype = "";
                    string tmpowndptid = "";
                    string bccemail = "";
                    string tmpstype = "";  //單據別
                    string tmpsno = "";  //請款單號
                    string tmpdate = "";//申請日期
                    // string tmpvendno = "";//廠商
                    // string tmpamoney = "";//請款金額
                    string tmpnote = "";//摘要說明
                    string tmpownman = "";//申請人
                    #endregion
                    if (vstatus == "1")
                    {
                        #region  通過時

                        //找出下一個角色是誰

                        tmprole = dbobj.getnewcheck1("P", tmprolestampid, roleall, "0", "", billflowid);

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
                        tmpstatus = vstatus;
                    }
                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {
                        #region 寄信參數值
                        //===============
                        vend_contractinv modobj = con.vend_contractinv.Where(r => r.vcinvid == vcinvid).FirstOrDefault();
                        System.Data.SqlClient.SqlConnection tmpconn1 = dbobj.get_conn("Aitag_DBContext");
                        tmpstype = dbobj.get_billname("P", modobj.vcsubtype);
                        tmpsno = modobj.vcno;
                        tmpdate = dbobj.get_date(modobj.vadate.ToString(), "1");
                        //tmpvendno = modobj.vendno +"－" + dbobj.get_dbvalue(tmpconn1, "select comsttitle from allcompany where comid = '" + modobj.vendno + "'");
                        //tmpamoney = modobj.totalmoney.ToString();
                        tmpnote = modobj.vcomment;
                        tmpownman = modobj.ownman;

                        tmpvcsubtype = modobj.vcsubtype;
                        tmpowndptid = modobj.owndptid;
                        bccemail = dbobj.get_dbvalue(tmpconn1, "select enemail from employee where empid = '" + tmpownman + "'");
                        tmpconn1.Close();
                        tmpconn1.Dispose();
                        //===============
                        #endregion
                        //呈核人員
                        //======================             
                        modobj.vstatus = tmpstatus;
                        if (vstatus == "1")
                        { modobj.rolestampid = tmprole; } //下個呈核角色

                        modobj.rolestampidall = roleall; //所有呈核角色
                        modobj.empstampidall = modobj.empstampidall + ",'" + Session["empid"].ToString() + "'"; //所有人員帳號
                        modobj.billtime = modobj.billtime + "," + DateTime.Now.ToString(); //所有時間
                        /*if (Request["exppaydate"] != null)
                        {
                            modobj.exppaydate = DateTime.Parse(Request["exppaydate"].ToString());
                        }*/
                        modobj.exppaydate = DateTime.Parse(date1);
                        modobj.bmodid = Session["empid"].ToString();
                        modobj.bmoddate = DateTime.Now;

                        con.Entry(modobj).State = EntityState.Modified;
                        con.SaveChanges();
                        con.Dispose();
                    }

                    #region 審核通過時，產生傳票
                    if (tmpstatus == "2")
                    {
                        SqlConnection conn = dbobj.get_conn("AitagBill_DBContext");
                        SqlConnection conn1 = dbobj.get_conn("AitagBill_DBContext");

                        decimal allmoney = 0;

                        using (AitagBill_DBContext con = new AitagBill_DBContext())
                        {
                            #region 多筆分擹借項
                                    SqlCommand sqlsmd = new SqlCommand();
                                    sqlsmd.Connection = conn;
                                    accountvoucher col;
                                    sqlsmd.CommandText = "select * from  view_vend_contractinv_det where vcno = '" + tmpsno + "' and comid = '" + Session["comid"] + "' and itemcode <>'' and itemcode IS NOT NULL";
                                    SqlDataReader dr = sqlsmd.ExecuteReader();
                                    int itemno = 10;
                                    string costtype = "", ownman = "", subjectcode = "", subjectname = "", paydate = "", paytype = "", vcsubtype = "" ,delvcno="";
                                    if (tmpvcsubtype == "3")
                                    {
                                        dr.Close();
                                        dr.Dispose();
                                        sqlsmd.CommandText = "select * from  vend_contractinv where vcno = '" + tmpsno + "' and comid = '" + Session["comid"] + "'";
                                        dr = sqlsmd.ExecuteReader();
                                        if (dr.Read())
                                        {
                                            costtype = dr["costtype"].ToString();
                                            ownman = dr["ownman"].ToString();
                                            subjectcode = "1470073";
                                            subjectname = "暫付款-員工";
                                            col = new accountvoucher();
                                            col.billtype = "P";
                                            col.costtype = costtype;
                                            col.ownman = ownman;
                                            col.vcsubtype = tmpvcsubtype;
                                            col.vcsno = tmpsno;
                                            col.deliveryno = 1;
                                            col.pinvdate = DateTime.Parse(tmpdate);
                                            col.itemno = itemno;
                                            col.subjectcode = subjectcode;
                                            col.subjectname = subjectname;
                                            col.cedeptcode = tmpowndptid;
                                            col.objno = tmpownman;
                                            col.debitsum = decimal.Parse(dr["totalmoney"].ToString());
                                            col.creditsum = 0;

                                            paydate = dbobj.get_date(DateTime.Parse(dr["vpdate"].ToString()).AddDays(15).ToString(), "1");
                                            //    dbobj.get_date(DateTime.Parse(dr["vpdate"].ToString()).AddDays(15).ToString(),"1")
                                            //預計付款日
                                            col.ceenddate = DateTime.Parse(dr["vpdate"].ToString()).AddDays(15);
                                            col.cebillcode = "";
                                            col.vouabstract = dr["vcomment"].ToString();
                                            //col.baddid = Session["tempid"].ToString();
                                            col.comid = Session["comid"].ToString();
                                            col.bmodid = Session["empid"].ToString();
                                            //col.badddate = DateTime.Now;
                                            col.bmoddate = DateTime.Now;

                                            allmoney += decimal.Parse(dr["totalmoney"].ToString());

                                            itemno = itemno + 10;
                                            con.accountvoucher.Add(col);
                                            con.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        while (dr.Read())
                                        {
                                            //借方金額
                                            paytype = dr["paytype"].ToString();
                                            costtype = dr["costtype"].ToString();
                                            ownman = dr["ownman"].ToString();
                                            vcsubtype = dr["vcsubtype"].ToString();
                                            delvcno = dr["delvcno"].ToString();

                                            subjectcode = dbobj.get_dbvalue(conn1, "select  subjectcode from billsubject where itemcode = '" + dr["itemcode"].ToString() + "' and comid = '" + dr["comid"].ToString() + "'");
                                            subjectname = dbobj.get_dbvalue(conn1, "select subjecttitle from billsubject where itemcode = '" + dr["itemcode"].ToString() + "' and comid = '" + dr["comid"].ToString() + "'");
                                            col = new accountvoucher();
                                            col.costtype = costtype;
                                            col.ownman = ownman;
                                            col.billtype = "P";
                                            col.vcsubtype = tmpvcsubtype;
                                            col.vcsno = tmpsno;
                                            col.deliveryno = 1;
                                            col.pinvdate = DateTime.Parse(tmpdate);
                                            col.itemno = itemno;
                                            col.subjectcode = subjectcode;
                                            col.subjectname = subjectname;
                                            col.cedeptcode = dr["dptid"].ToString();
                                            // col.objno = tmpownman;
                                            col.debitsum = decimal.Parse(dr["pmoney"].ToString());
                                            col.creditsum = 0;

                                            //卡號
                                            col.projno = dr["wcardno"].ToString();
                                            //客戶
                                            if (paytype == "1")
                                            {
                                                col.objno = dr["allcomid"].ToString();
                                            }
                                            else if (paytype == "2")
                                            {
                                                col.objno = dr["ownman"].ToString();
                                            }
                                            else if (paytype == "3")
                                            {
                                                col.objno = dr["allcomid"].ToString();
                                            }
                                            else if (paytype == "4")
                                            {
                                                col.objno = dr["ownman"].ToString();
                                            }
                                            else
                                            {
                                                col.objno = dr["ownman"].ToString();
                                            }
                                            //預計付款日
                                            paydate = dbobj.get_date(dr["exppaydate"].ToString(), "1");
                                            col.ceenddate = DateTime.Parse(dr["exppaydate"].ToString());
                                            col.cebillcode = dr["delvcno"].ToString();
                                            col.vouabstract = dr["vcomment"].ToString();
                                            //col.baddid = Session["tempid"].ToString();
                                            col.comid = Session["comid"].ToString();
                                            col.bmodid = Session["empid"].ToString();
                                            //col.badddate = DateTime.Now;
                                            col.bmoddate = DateTime.Now;

                                            allmoney += decimal.Parse(dr["pmoney"].ToString());

                                            itemno = itemno + 10;
                                            con.accountvoucher.Add(col);
                                            con.SaveChanges();
                                        }
                                    }
                                    dr.Close();
                                    dr.Dispose();
                            #endregion

                                    string taxmoney = "", invno = "";
                                    taxmoney = dbobj.get_dbvalue(conn1, "select isnull(sum(taxmoney),0) as taxmoney from incomeaccounts where vcno = '" + tmpsno + "' and comid = '" + Session["comid"] + "'") + "";
                                    dr = dbobj.dbselect(conn1, "select invno from incomeaccounts where vcno = '" + tmpsno + "' and comid = '" + Session["comid"] + "'");
                                    while (dr.Read())
                                    {
                                        invno += dr["invno"].ToString() + ",";
                                    }
                                    dr.Close();
                                    dr.Dispose();
                                    //taxmoney = dbobj.get_dbvalue(conn1, "select invno from incomeaccounts where vcno = '" + tmpsno + "'") + "";
                                    if (taxmoney == "")
                                    { taxmoney = "0"; }

                                    if (allmoney > 0)
                                    {

                                        #region 一筆借進項稅額

                                        if (decimal.Parse(taxmoney) > 0)
                                        {
                                            itemno = itemno + 10;
                                            col = new accountvoucher();
                                            col.cebillcode = invno;
                                            col.costtype = costtype;
                                            col.ownman = ownman;
                                            col.billtype = "P";
                                            col.vcsubtype = tmpvcsubtype;
                                            col.vcsno = tmpsno;
                                            col.deliveryno = 1;
                                            col.pinvdate = DateTime.Parse(tmpdate);
                                            col.itemno = itemno;
                                            col.subjectcode = "1410023";
                                            col.subjectname = "進項稅額";
                                            col.cedeptcode = tmpowndptid;
                                            col.objno = tmpownman;
                                            col.debitsum = decimal.Parse(taxmoney);
                                            col.creditsum = 0;
                                            col.vouabstract = "進項稅額";
                                            //col.baddid = Session["tempid"].ToString();
                                            col.comid = Session["comid"].ToString();
                                            col.bmodid = Session["empid"].ToString();
                                            //col.badddate = DateTime.Now;
                                            col.bmoddate = DateTime.Now;

                                            allmoney += int.Parse(taxmoney);

                                            con.accountvoucher.Add(col);
                                            con.SaveChanges();
                                        }

                                        #endregion


                                        #region 一筆貸方金額


                                        itemno = itemno + 10;
                                        col = new accountvoucher();
                                        col.costtype = costtype;
                                        col.ownman = ownman;
                                        col.billtype = "P";
                                        col.vcsubtype = tmpvcsubtype;
                                        col.vcsno = tmpsno;
                                        col.deliveryno = 1;
                                        col.pinvdate = DateTime.Parse(tmpdate);
                                        col.itemno = itemno;
                                        if (tmpvcsubtype != "3")
                                        {
                                            if (paytype == "4")
                                            {
                                                col.subjectcode = "1470073";
                                                col.subjectname = "暫付款-員工";
                                            }
                                            else if (paytype == "3")
                                            {
                                                col.subjectcode = "2191020";
                                                col.subjectname = "應付電匯款-員工墊款";
                                            }
                                            else
                                            {
                                                if (subjectcode == "1470073")
                                                {
                                                    col.subjectcode = "2191020";
                                                    col.subjectname = "應付電匯款-員工墊款";
                                                }
                                                else
                                                {
                                                    if (costtype == "1")
                                                    {
                                                        col.subjectcode = "2170071";
                                                        col.subjectname = "應付成本";
                                                    }
                                                    else
                                                    {
                                                        col.subjectcode = "2200019";
                                                        col.subjectname = "其他應付款-其他";
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            col.subjectcode = "2191020";
                                            col.subjectname = "應付電匯款-員工墊款";
                                        }

                                        col.ceenddate = DateTime.Parse(paydate);
                                        col.cedeptcode = tmpowndptid;
                                        col.objno = tmpownman;
                                        col.debitsum = 0;
                                        col.creditsum = decimal.Parse(allmoney.ToString());
                                        col.vouabstract = col.subjectname;
                                        col.comid = Session["comid"].ToString();
                                        //col.baddid = Session["tempid"].ToString();
                                        col.bmodid = Session["empid"].ToString();
                                        //col.badddate = DateTime.Now;
                                        col.bmoddate = DateTime.Now;

                                        con.accountvoucher.Add(col);
                                        con.SaveChanges();

                                        #endregion
                                    }

                        }
                        conn.Close();
                        conn.Dispose();

                        conn1.Close();
                        conn1.Dispose();
                  

            }
            #endregion



            #region  寄信
            string msgerr = "";
            string txt_comment = "";
            string mailtitle = "";
            string mailcontext = "";

            switch (tmpstatus)
            {
                case "1"://審核中      
                    mailtitle = tmpstype + "申請要求審核通知";
                    txt_comment = "有一筆申請單提出，請至系統進行審核作業。<br>資料如下：";
                    break;
                case "2"://已審核        
                    mailtitle = tmpstype + "申請完成審核通知";
                    txt_comment = "您的申請單已通過審核。<br>資料如下：";
                    break;
               
            }

            mailcontext += "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=450 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
            mailcontext += "<tr>";
            mailcontext += "<td colspan=2>" + txt_comment + "</td>";
            mailcontext += "</tr>";

            mailcontext += "<tr>";
            mailcontext += "<td align=right width='90'>單據別：</td>";
            mailcontext += "<td>" + tmpstype + "</td>";
            mailcontext += "</tr>";

            mailcontext += "<tr>";
            mailcontext += "<td align=right>請款單號：</td>";
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
                string tomail = "";
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


            string tmpform = "";
          
                tmpform = "<body onload=javascript:alert('送出審核成功!!');qfr1.submit();>";
                tmpform += "<form name='qfr1' action='/paybill/expensechk' method='post'>";
                tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                tmpform += "</form>";
                tmpform += "</body>";


            return new ContentResult() { Content = @"" + tmpform };
        }

        public ActionResult expensechk(int? page, string orderdata, string orderdata1)
        {
            IPagedList<vend_contractinv> result;
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;

            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "vcinvid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qvcno = "", qvendno = "", qvcomment = "", qsvadate = "", qevadate = "", qvcsubtype = "";

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
                ViewBag.qevadate = qevadate;
            }

            if (!string.IsNullOrWhiteSpace(Request["qvcsubtype"]))
            {
                qvcsubtype = Request["qvcsubtype"].Trim();
                ViewBag.qvcsubtype = qvcsubtype;
            }

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "";
                sqlstr = "select * from vend_contractinv where vctype = 'P' and vstatus='1' and replace(rolestampid,'''','')='" + Session["rid"].ToString() + "'  and";
                if (qvcno != "")
                    sqlstr += " vcno like '%" + qvcno + "%'  and";
                if (qvendno != "")
                    sqlstr += " vendno in (select comid from allcompany where comid like '%" + qvendno + "%' or comtitle like '%" + qvendno + "%')   and";
                if (qvcomment != "")
                    sqlstr += " vcomment like '%" + qvcomment + "%'  and";
                if (qsvadate != "")
                    sqlstr += " vadate >= '" + qsvadate + "'  and";
                if (qevadate != "")
                    sqlstr += " vadate <= '" + qevadate + "'  and";
                if (qvcsubtype != "")
                    sqlstr += " vcsubtype = '" + qvcsubtype + "'  and";
                

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.vend_contractinv.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<vend_contractinv>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);
           
        }

        public ActionResult expenseacc(int? page, string orderdata, string orderdata1)
        {
            IPagedList<vend_contractinv> result;
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            orderdata = "vadate";
            orderdata1 = "asc";
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
                ViewBag.qevadate = qevadate;
            }

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "";
                sqlstr = "select * from vend_contractinv where  vctype = 'P' and vcsubtype='6'   and";
                if (qvcno != "")
                    sqlstr += " vcno like '%" + qvcno + "%'  and";
                if (qvendno != "")
                    sqlstr += " vendno in (select comid from allcompany where comid like '%" + qvendno + "%' or comtitle like '%" + qvendno + "%')   and";
                if (qvcomment != "")
                    sqlstr += " vcomment like '%" + qvcomment + "%'  and";
                if (qsvadate != "")
                    sqlstr += " vadate >= '" + qsvadate + "'  and";
                if (qevadate != "")
                    sqlstr += " vadate <= '" + qevadate + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.vend_contractinv.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<vend_contractinv>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);

        }

        public ActionResult expensemain(int? page, string orderdata, string orderdata1)
        {
            IPagedList<vend_contractinv> result;
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;

            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "vcinvid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qvcno = "", qvendno = "", qvcomment = "", qsvadate = "", qevadate = "", qvstatus = "", qvcsubtype = "";

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
                ViewBag.qevadate = qevadate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvstatus"]))
            {
                qvstatus = Request["qvstatus"].Trim();
                ViewBag.qvstatus = qvstatus;
            }

            if (!string.IsNullOrWhiteSpace(Request["qvcsubtype"]))
            {
                qvcsubtype = Request["qvcsubtype"].Trim();
                ViewBag.qvcsubtype = qvcsubtype;
            }

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "";
                sqlstr = "select * from vend_contractinv where vctype = 'P'   and";
                if (qvcno != "")
                    sqlstr += " vcno like '%" + qvcno + "%'  and";
                if (qvendno != "")
                    sqlstr += " vendno in (select comid from allcompany where comid like '%" + qvendno + "%' or comtitle like '%" + qvendno + "%')   and";
                if (qvcomment != "")
                    sqlstr += " vcomment like '%" + qvcomment + "%'  and";
                if (qsvadate != "")
                    sqlstr += " vadate >= '" + qsvadate + "'  and";
                if (qevadate != "")
                    sqlstr += " vadate <= '" + qevadate + "'  and";
                if (qvstatus != "")
                    sqlstr += " vstatus = '" + qvstatus + "'  and";
                if (qvcsubtype != "")
                    sqlstr += " vcsubtype = '" + qvcsubtype + "'  and";


                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.vend_contractinv.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<vend_contractinv>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);

        }

        public ActionResult accexpensemain(int? page, string orderdata, string orderdata1)
        {
            IPagedList<vend_contractinv> result;
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;

            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "vcinvid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qvcno = "", qvendno = "", qvcomment = "", qsvadate = "", qevadate = "", qvstatus = "", qvcsubtype = "" , qvouno = "";

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
                ViewBag.qevadate = qevadate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvstatus"]))
            {
                qvstatus = Request["qvstatus"].Trim();
                ViewBag.qvstatus = qvstatus;
            }

            if (!string.IsNullOrWhiteSpace(Request["qvcsubtype"]))
            {
                qvcsubtype = Request["qvcsubtype"].Trim();
                ViewBag.qvcsubtype = qvcsubtype;
            }

            if (!string.IsNullOrWhiteSpace(Request["qvouno"]))
            {
                qvouno = Request["qvouno"].Trim();
                ViewBag.qvouno = qvouno;
            }

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "";
                sqlstr = "select * from vend_contractinv where vctype = 'P' and vstatus = '2'   and";
                if (qvcno != "")
                    sqlstr += " vcno like '%" + qvcno + "%'  and";
                if (qvendno != "")
                    sqlstr += " vendno in (select comid from allcompany where comid like '%" + qvendno + "%' or comtitle like '%" + qvendno + "%')   and";
                if (qvcomment != "")
                    sqlstr += " vcomment like '%" + qvcomment + "%'  and";
                if (qsvadate != "")
                    sqlstr += " vadate >= '" + qsvadate + "'  and";
                if (qevadate != "")
                    sqlstr += " vadate <= '" + qevadate + "'  and";
                if (qvstatus != "")
                    sqlstr += " vstatus = '" + qvstatus + "'  and";
                if (qvcsubtype != "")
                    sqlstr += " vcsubtype = '" + qvcsubtype + "'  and";
                if (qvouno != "")
                    sqlstr += " vcno in (select distinct vcsno from accountvoucher where vouno = '" + qvouno + "')   and";


                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.vend_contractinv.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<vend_contractinv>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);

        }

        public ActionResult expenseperqry(int? page, string orderdata, string orderdata1)
        {
            IPagedList<vend_contractinv> result;
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;

            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "vcinvid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qvcno = "", qvendno = "", qvcomment = "", qsvadate = "", qevadate = "", qvstatus = "", qvcsubtype = "";

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
                ViewBag.qevadate = qevadate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvstatus"]))
            {
                qvstatus = Request["qvstatus"].Trim();
                ViewBag.qvstatus = qvstatus;
            }

            if (!string.IsNullOrWhiteSpace(Request["qvcsubtype"]))
            {
                qvcsubtype = Request["qvcsubtype"].Trim();
                ViewBag.qvcsubtype = qvcsubtype;
            }

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "";
                sqlstr = "select * from vend_contractinv where vctype = 'P' and ownman = '" + Session["empid"] + "' and comid = '" + Session["comid"] + "'   and";
                if (qvcno != "")
                    sqlstr += " vcno like '%" + qvcno + "%'  and";
                if (qvendno != "")
                    sqlstr += " vendno in (select comid from allcompany where comid like '%" + qvendno + "%' or comtitle like '%" + qvendno + "%')   and";
                if (qvcomment != "")
                    sqlstr += " vcomment like '%" + qvcomment + "%'  and";
                if (qsvadate != "")
                    sqlstr += " vadate >= '" + qsvadate + "'  and";
                if (qevadate != "")
                    sqlstr += " vadate <= '" + qevadate + "'  and";
                if (qvstatus != "")
                    sqlstr += " vstatus = '" + qvstatus + "'  and";
                if (qvcsubtype != "")
                    sqlstr += " vcsubtype = '" + qvcsubtype + "'  and";


                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.vend_contractinv.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<vend_contractinv>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);

        }

        public ActionResult expense2account(int? page, string orderdata, string orderdata1)
        {
            IPagedList<vend_contractinv> result;
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            orderdata = "vadate";
            orderdata1 = "asc";
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
                ViewBag.qevadate = qevadate;
            }

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "";
                sqlstr = "select * from vend_contractinv where  vctype = 'P' and vcsubtype='6'   and";
                if (qvcno != "")
                    sqlstr += " vcno like '%" + qvcno + "%'  and";
                if (qvendno != "")
                    sqlstr += " vendno in (select comid from allcompany where comid like '%" + qvendno + "%' or comtitle like '%" + qvendno + "%')   and";
                if (qvcomment != "")
                    sqlstr += " vcomment like '%" + qvcomment + "%'  and";
                if (qsvadate != "")
                    sqlstr += " vadate >= '" + qsvadate + "'  and";
                if (qevadate != "")
                    sqlstr += " vadate <= '" + qevadate + "'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.vend_contractinv.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<vend_contractinv>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);

        }

        public ActionResult expense2del(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qempname = Request["qempname"];
            ViewBag.qempname = qempname;
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

                        string vcno = dbobj.get_dbvalue(conn1, "select ('單號：' + vcno + ',申請人：' + ownman + ',申請日期：' + CONVERT(varchar(12), vadate, 111)) as st1 from vend_contractinv where vcinvid=" + condtionArr[i].ToString());

                        sysnote += vcno + "<br>";
                        //刪除付款憑單
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM vend_contractinv where vcinvid = '" + condtionArr[i].ToString() + "'");
                        //刪除付款憑單明細
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM vend_contractinv_det where vcinvid = '" + condtionArr[i].ToString() + "'");
                        //刪除發票
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM incomeaccounts where vcinvid = '" + condtionArr[i].ToString() + "'");
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
                    if (Request["vcsubtype"] == "1")
                     tgourl = "/paybill/expense2list?page=" + page + "&qempname=" + qempname;
                    else if (Request["vcsubtype"] == "2")
                     tgourl = "/paybill/expense1list?page=" + page + "&qempname=" + qempname;
                    else if (Request["vcsubtype"] == "3")
                     tgourl = "/paybill/expense5list?page=" + page + "&qempname=" + qempname;
                    else if (Request["vcsubtype"] == "4")
                     tgourl = "/paybill/expense4list?page=" + page + "&qempname=" + qempname;
                    else if (Request["vcsubtype"] == "5")
                        tgourl = "/paybill/expense3list?page=" + page + "&qempname=" + qempname;
                    else if (Request["vcsubtype"] == "6")
                        tgourl = "/paybill/expense8list?page=" + page + "&qempname=" + qempname;
                    else if (Request["vcsubtype"] == "7")
                        tgourl = "/paybill/expense6list?page=" + page + "&qempname=" + qempname;
                    else if (Request["vcsubtype"] == "8")
                        tgourl = "/paybill/expense7list?page=" + page + "&qempname=" + qempname;
                    else
                        tgourl = "/paybill/expensemain?page=" + page + "&qempname=" + qempname;
                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                    //return RedirectToAction("List");
                }
            }
        }

        public ActionResult expensedetdel(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
           // string qempname = Request["qempname"];
           // ViewBag.qempname = qempname;
            string cdel = Request["cdel"];
            string vcno = Request["vcno"];

            if (string.IsNullOrWhiteSpace(cdel))
            {

                string tgourl = "/paybill/expense2dp?vcno=" + vcno;
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
                        string money1 = dbobj.get_dbvalue(conn1, "select ('卡號' + wcardno + ',部門' + dptid + ',金額' + convert(char,totalmoney)) as st1  from vend_contractinv where vctid = '" + condtionArr[i].ToString() + "'");

                        sysnote += money1 + "<br>";

                      //  dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM vend_contractinv where vcinvid = '" + condtionArr[i].ToString() + "'");
                        //刪除付款憑單明細資料
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM vend_contractinv_det where vctid = '" + condtionArr[i].ToString() + "'");
                      
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
                    string tgourl = "/paybill/expense2dp?vcno=" + vcno ;
                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                    //return RedirectToAction("List");
                }
            }
        }


        public ActionResult expenseinvdel(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            // string qempname = Request["qempname"];
            // ViewBag.qempname = qempname;
            string cdel = Request["cdel"];
            string vcno = Request["vcno"];

            if (string.IsNullOrWhiteSpace(cdel))
            {

                string tgourl = "/paybill/expenseinv?vcno=" + vcno;
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
                        string money1 = dbobj.get_dbvalue(conn1, "select ('發票號碼' + invno + ',金額' + convert(char,invmoney)) as st1  from vend_contractinv where vctid = '" + condtionArr[i].ToString() + "'");

                        sysnote += money1 + "<br>";

                        //  dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM vend_contractinv where vcinvid = '" + condtionArr[i].ToString() + "'");
                        //刪除付款憑單明細資料
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM incomeaccounts where icid = '" + condtionArr[i].ToString() + "'");

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
                    string tgourl = "/paybill/expenseinv?vcno=" + vcno;
                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                    //return RedirectToAction("List");
                }
            }
        }

        public ActionResult expenserepdo(string sysflag, int? page, string orderdata, string orderdata1)
        {
           
             //string uaccount = Request["UserAccount"];
            NDcommon dbobj = new NDcommon();
            SqlConnection conn = dbobj.get_conn("AitagBill_DBContext");
            SqlConnection tmpconn = dbobj.get_conn("AitagBill_DBContext");

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = tmpconn;

            cmd.CommandText = "select * from vend_contractinv where vcinvid = '" + Request["vcinvid"] + "'";
            
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                ;
            }


            string vcno = "";
            string sqlstr = "";
            string tmpvid = "";

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {

                page = ((!page.HasValue || page < 1) ? 1 : page);
                ViewBag.page = page;
                ViewBag.orderdata = orderdata;
                ViewBag.orderdata1 = orderdata1;

                string qempname = Request["qempname"];
                ViewBag.qempname = qempname;
                // vend_contractinv Userobj = con.vend_contractinv.Where(r => r.UserAccount == uaccount).FirstOrDefault();
                vend_contractinv addobj = new vend_contractinv();
               // if (Userobj == null)
               // {
                vcno = dbobj.get_billno(conn, "P", Request["vcsubtype"].ToString() , Request["comid"].ToString(), "", Request["vadate"].ToString());
               //     Userobj = new User();
                    //Guid 產生的方式
                    //Guid Uid = Guid.NewGuid();
                    addobj.vadate = DateTime.Parse(Request["vadate"].ToString());
                   
                    addobj.comid = Request["comid"];
                    addobj.vendno = dr["vendno"].ToString();
                    addobj.vcno = vcno;
                    addobj.vserno = 1;
                    addobj.vstatus = "0";
                    addobj.vctype = "P";
                    addobj.vcsubtype = Request["vcsubtype"];
                    addobj.costtype = dr["costtype"].ToString();
                    addobj.paytype = dr["paytype"].ToString();
                   // if (!string.IsNullOrWhiteSpace(Request["qvcno"]))
                    if (!string.IsNullOrWhiteSpace(dr["spdate"].ToString()))
                    {
                        addobj.spdate = DateTime.Parse(dr["spdate"].ToString());
                    }
                    else
                    {
                        addobj.spdate = null;
                    }
                  
                    if (!string.IsNullOrWhiteSpace(dr["vpdate"].ToString()))
                    {
                        addobj.vpdate = DateTime.Parse(dr["vpdate"].ToString());
                    }
                    else
                    {
                        addobj.vpdate = null;
                    }
                    addobj.ownman = Session["empid"].ToString();
                    addobj.payman = Session["empid"].ToString();
                    addobj.owndptid = Session["dptid"].ToString();
                    addobj.paycomment = dr["paycomment"].ToString();
                    addobj.delvcno = dr["delvcno"].ToString();
                    addobj.currency = dr["currency"].ToString();
                    addobj.totalmoney = decimal.Parse(dr["totalmoney"].ToString());
                    addobj.vcomment = dr["vcomment"].ToString();
                    addobj.othercomment = dr["othercomment"].ToString();
                    addobj.bmodid = Session["empid"].ToString();
                    addobj.bmoddate = DateTime.Now;

                    dr.Close();
                    dr.Dispose();
                    con.vend_contractinv.Add(addobj);
                    
                    con.SaveChanges();
                    con.Dispose();

                    //複製加入分攤資料;
                    tmpvid = dbobj.get_vcinvid(conn,Request["comid"].ToString(),vcno);
                    sqlstr = "insert into vend_contractinv_det (vcinvid , vcno , wcardno , dptid , pmoney , pcount , punit , psummoney , pcomment , projno , comid , bmodid , bmoddate) ";
                    sqlstr += " select (" + tmpvid + ") as vcinvid , ('" + vcno + "') as vcno , wcardno , dptid , pmoney , pcount , punit , psummoney , pcomment , projno , comid , bmodid , bmoddate from vend_contractinv_det  where vcinvid = " + Request["vcinvid"].ToString();

                    cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = sqlstr;
                    cmd.ExecuteNonQuery();

                    //加入發票資料
                    //sqlstr = "insert into incomeaccounts (vcinvid , vcno , invno , invdate , pmoney , pcount , punit , psummoney , pcomment , projno , comid , bmodid , bmoddate) ";
                    //sqlstr += " select (" + tmpvid + ") as vcinvid , (" + vcno + ") as vcno , wcardno , dptid , pmoney , pcount , punit , psummoney , pcomment , projno , comid , bmodid , bmoddate  where vcinvid = " + Request["vcinvid"].ToString();

            }

            string tmpform = "";
           
            tmpform = "<body onload=qfr1.submit();>";

            tmpform += "<form name='qfr1' action='/paybill/expenserepshow'  method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden name='vcno' id='vcno' value='" + vcno + "'>";
            tmpform += "<input type=hidden name='vcsubtype' id='vcsubtype' value='" + Request["vcsubtype"].ToString() + "'>";
            // tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
            tmpform += "</form>";
            tmpform += "</body>";

           
            conn.Close();
            conn.Dispose();
            tmpconn.Close();
            tmpconn.Dispose();

            return new ContentResult() { Content = @"" + tmpform };
        }

        public ActionResult expenserepshow()
        {
            return View();
        }


        public ActionResult expenseaccount()
        {
            string tvcno = "";
            tvcno = Request["vcno"].ToString();
            List<accountvoucher> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                var query = con.accountvoucher
                  .AsQueryable();
                result = query.Where(r => r.vcsno == tvcno).AsQueryable().ToList();

            }
            return View(result);
        }


        public ActionResult expenseaccountdo(string sysflag, int? page, string orderdata, string orderdata1)
        {
          
            NDcommon dbobj = new NDcommon();
            SqlConnection erpconn = dbobj.get_conn("AitagBill_DBContext");
            SqlCommand cmd = new SqlCommand();
            string vcno = "";

            vcno = Request["vcno"].ToString();
           
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {

                page = ((!page.HasValue || page < 1) ? 1 : page);
                ViewBag.page = page;
                ViewBag.orderdata = orderdata;
                ViewBag.orderdata1 = orderdata1;           
                string cdel1 = Request["voudid"];
                string pinvdate1 = Request["pinvdate"];
                string itemno1 = Request["itemno"];
                string subjectcode1 = Request["subjectcode"];
                string subjectname1 = Request["subjectname"];
                string cedeptcode1 = Request["cedeptcode"];
                string projno1 = Request["projno"];
                string objno1 = Request["objno"];
                string ceenddate1 = Request["ceenddate"];
                string vouabstract1 = Request["vouabstract"];
                string debitsum1 = Request["debitsum"];
                string creditsum1 = Request["creditsum"];
                string[] cdelarr = cdel1.Split(',');

                string[] pinvdatearr = pinvdate1.Split(',');
                string[] itemnoarr = itemno1.Split(',');
                string[] subjectcodearr = subjectcode1.Split(',');
                string[] subjectnamearr = subjectname1.Split(',');
                string[] cedeptcodearr = cedeptcode1.Split(',');
                string[] projnoarr = projno1.Split(',');
                string[] objnoarr = objno1.Split(',');
                string[] ceenddatearr = ceenddate1.Split(',');
                string[] vouabstractarr = vouabstract1.Split(',');
                string[] debitsumarr = debitsum1.Split(',');
                string[] creditsumarr = creditsum1.Split(',');
              
                for (int i = 0; i < cdelarr.Length; i++)
                {
                    if (cdelarr[i].Trim() == "")
                    {
                        if (!(pinvdatearr[i].Trim() == "" && subjectcodearr[i].Trim() == "" && subjectnamearr[i].Trim() == ""))
                        {
                            accountvoucher addobj = new accountvoucher();
                            addobj.billtype = "P";
                            addobj.vcsubtype = Request["vcsubtype"].ToString();
                            addobj.vcsno = vcno;
                            addobj.deliveryno = 1;
                            addobj.pinvdate = DateTime.Parse(pinvdatearr[i].Trim());
                            addobj.itemno = int.Parse(itemnoarr[i].Trim());
                            addobj.subjectcode = subjectcodearr[i].Trim();
                            addobj.subjectname = subjectnamearr[i].Trim();
                            addobj.cedeptcode = cedeptcodearr[i].Trim();
                            addobj.objno = objnoarr[i].Trim();

                            if (ceenddatearr[i] == null || ceenddatearr[i] == "")
                            { addobj.ceenddate = null; }
                            else
                            { addobj.ceenddate = DateTime.Parse(ceenddatearr[i].Trim()); }

                            addobj.projno = projnoarr[i].Trim();
                            addobj.vouabstract = vouabstractarr[i].Trim();

                            if (debitsumarr[i] == null || debitsumarr[i] == "")
                            { addobj.debitsum =0; }
                            else
                            { addobj.debitsum = int.Parse(debitsumarr[i].Trim()); }

                            if (creditsumarr[i] == null || creditsumarr[i] == "")
                            { addobj.creditsum = 0; }
                            else
                            { addobj.creditsum = decimal.Parse(creditsumarr[i].Trim()); }     
                           
                            addobj.bmodid = Session["empid"].ToString();                          
                            addobj.bmoddate = DateTime.Now;

                            con.accountvoucher.Add(addobj);
                            con.SaveChanges();

                        }
                    }
                    else
                    {

                        int voudid = int.Parse(cdelarr[i].Trim());
                        accountvoucher modobj = con.accountvoucher.Where(r => r.voudid ==voudid ).FirstOrDefault();

                        modobj.billtype = "P";
                        modobj.vcsubtype = Request["vcsubtype"].ToString();
                        modobj.vcsno = vcno;
                        modobj.deliveryno = 1;
                        modobj.pinvdate = DateTime.Parse(pinvdatearr[i].Trim());
                        modobj.itemno = int.Parse(itemnoarr[i].Trim());
                        modobj.subjectcode = subjectcodearr[i].Trim();
                        modobj.subjectname = subjectnamearr[i].Trim();
                        modobj.cedeptcode = cedeptcodearr[i].Trim();
                        modobj.objno = objnoarr[i].Trim();

                        if (ceenddatearr[i] == null || ceenddatearr[i] == "")
                        { modobj.ceenddate = null; }
                        else
                        { modobj.ceenddate = DateTime.Parse(ceenddatearr[i].Trim()); }

                        modobj.projno = projnoarr[i].Trim();
                        modobj.vouabstract = vouabstractarr[i].Trim();

                        if (debitsumarr[i] == null || debitsumarr[i] == "")
                        { modobj.debitsum = 0; }
                        else
                        { modobj.debitsum = decimal.Parse(debitsumarr[i].Trim()); }

                        if (creditsumarr[i] == null || creditsumarr[i] == "")
                        { modobj.creditsum = 0; }
                        else
                        { modobj.creditsum = decimal.Parse(creditsumarr[i].Trim()); }

                        modobj.bmodid = Session["empid"].ToString();
                        modobj.bmoddate = DateTime.Now;
                        modobj.comid = modobj.comid;
                     

                        con.Entry(modobj).State = EntityState.Modified;
                        con.SaveChanges();
                     
                           
                       
                    }
                }
                con.Dispose();


            }

            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/paybill/expenseaccount' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden name='vcno' id='vcno' value='" + vcno + "'>";
            // tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
            tmpform += "</form>";
            tmpform += "</body>";

            erpconn.Close();
            erpconn.Dispose();

            return new ContentResult() { Content = @"" + tmpform };
        }


        public ActionResult expenseaccountdel(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            // string qempname = Request["qempname"];
            // ViewBag.qempname = qempname;
            string cdel = Request["cdel"];
            string vcno = Request["vcno"];

            if (string.IsNullOrWhiteSpace(cdel))
            {

                string tgourl = "/paybill/expenseaccount?vcno=" + vcno;
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
                        string money1 = dbobj.get_dbvalue(conn1, "select ('請款單號' + vcsno + ',科目編號' + subjectcode) as st1  from accountvoucher where voudid = '" + condtionArr[i].ToString() + "'");

                        sysnote += money1 + "<br>";
                     
                        //刪除
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM accountvoucher where voudid = '" + condtionArr[i].ToString() + "'");

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
                  //  string tgourl = "/paybill/expenseaccount?vcno=" + vcno;
                  //  return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                    string tmpform = "";
                    tmpform = "<body onload=alert('刪除成功!!');qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/paybill/expenseaccount' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    //tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                   // tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden name='vcno' id='vcno' value='" + vcno + "'>";
               
                    tmpform += "</form>";
                    tmpform += "</body>";

                   

                    return new ContentResult() { Content = @"" + tmpform };

               
                }
            }
        }
    }
}
