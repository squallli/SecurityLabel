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
    public class workhourController : BaseController
    {

        //private AitagBill_DBContext db = new AitagBill_DBContext();
        //
        // GET: /workhour/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.Msid = Session["Msid"].ToString();
        //    workhour col = new workhour();
        //    return View(col);
        //}

        //[HttpPost]
        public ActionResult add(workhour col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "whid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            NDcommon dbobj = new NDcommon();

            string qcustno = "", qwhno = "", qempid = "";


            int count1 = int.Parse("0"+ dbobj.get_dbnull2(Request["count1"]));
            if (!string.IsNullOrWhiteSpace(Request["qcustno"]))
            {
                qcustno = Request["qcustno"].Trim();
                ViewBag.qcustno = qcustno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qwhno"]))
            {
                qwhno = Request["qwhno"].Trim();
                ViewBag.qwhno = qwhno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {
                qempid = Request["qempid"].Trim();
                ViewBag.qempid = qempid;
            }

            if (sysflag != "A")
            {
                workhour newcol = new workhour();
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

                    col.wstatus = "0";
                    col.arolestampid = "'" + Session["rid"].ToString() + "'";
                    col.bmodid = Session["tempid"].ToString();
                    col.whid = Decimal.Parse(DateTime.Now.ToString("yyyyMMddhhmmssff"));
                    col.wadddate = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                    col.dptid = Session["Dptid"].ToString();
                    col.empid = Session["tempid"].ToString();
                    col.bmoddate = DateTime.Now;
                    col.comid = Session["comid"].ToString();
                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {
                        string  put_in_time = "";
                        string  work_no_code = "";
                        for (int p = 0; p <= count1; p++)
                        {
                            put_in_time = "put_in_time" +p;
                            work_no_code = "work_no_code" +p;
                            if (dbobj.get_dbnull2(Request[put_in_time]) != "")
                            {
                            
                                col.put_in_times = int.Parse("0"+ Request[put_in_time])*30;
                                col.work_no_code = Request[work_no_code];
                                col.arolestampid = "'" + Session["rid"].ToString() + "'";
                                con.workhour.Add(col);
                                con.SaveChanges();
                            }
                        }


                    }

                    //系統LOG檔 //================================================= //
                    SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    string sysrealsid = Request["sysrealsid"].ToString();
                    string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    string sysnote = "客戶別:" + col.custno + "工作卡號:" + col.whno;
                    dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    sysconn.Close();
                    sysconn.Dispose();
                    //=================================================

                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/workhour/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qcustno' name='qcustno' value='" + qcustno + "'>";
                    tmpform += "<input type=hidden id='qwhno' name='qwhno' value='" + qwhno + "'>";
                    tmpform += "<input type=hidden id='qempid' name='qempid' value='" + qempid + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"" + tmpform };                 

                }
            }


        }


        [ValidateInput(false)]
        public ActionResult Edit(workhour chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            NDcommon dbobj = new NDcommon();
            int count1 = int.Parse("0" + dbobj.get_dbnull2(Request["count1"])); 
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "whid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcustno = "", qwhno = "", qempid = "";
            if (!string.IsNullOrWhiteSpace(Request["qcustno"]))
            {
                qcustno = Request["qcustno"].Trim();
                ViewBag.qcustno = qcustno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qwhno"]))
            {
                qwhno = Request["qwhno"].Trim();
                ViewBag.qwhno = qwhno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {
                qempid = Request["qempid"].Trim();
                ViewBag.qempid = qempid;
            }

            if (sysflag != "E")
            {
                using (AitagBill_DBContext con = new AitagBill_DBContext())
                {
                    var data = con.workhour.Where(r => r.whid == chks.whid).FirstOrDefault();
                    
                    
                    if (data == null)
                    {
                        return HttpNotFound();
                    }
                    return View(data);
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

                    dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM workhour where whid = '" + Request["whid"].ToString() + "'");

                    using (AitagBill_DBContext con = new AitagBill_DBContext()) 
                    {

                        string put_in_time = "";
                        string work_no_code = "";
                        for (int p = 0; p <= count1; p++)
                        {
                            //整理欄位
                            chks.bmodid = Session["tempid"].ToString();
                            chks.bmoddate = DateTime.Now;
                            chks.dptid = Session["Dptid"].ToString();
                            chks.empid = Session["tempid"].ToString();

                            //==
                            put_in_time = "put_in_time" + p;
                            work_no_code = "work_no_code" + p;
                            if (dbobj.get_dbnull2(Request[put_in_time]) != "")
                            {
                                chks.put_in_times = int.Parse("0" + Request[put_in_time]) * 30;
                                chks.work_no_code = Request[work_no_code];
                                con.workhour.Add(chks);
                                con.SaveChanges();
                            }
                        }
                    }


                    //系統LOG檔
                    //================================================= //                     
                    SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    string sysrealsid = Request["sysrealsid"].ToString();
                    string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    string sysnote = "客戶別:" + chks.custno + "工作卡號:" + chks.whno;
                    dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    sysconn.Close();
                    sysconn.Dispose();
                    //=================================================

                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/workhour/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qcustno' name='qcustno' value='" + qcustno + "'>";
                    tmpform += "<input type=hidden id='qwhno' name='qwhno' value='" + qwhno + "'>";
                    tmpform += "<input type=hidden id='qempid' name='qempid' value='" + qempid + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    //return RedirectToAction("List");
                }
            }

        }

        public ActionResult List(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = " whid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = " asc"; }

            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcustno = "", qwhno = "", qempid = "";
            if (!string.IsNullOrWhiteSpace(Request["qcustno"]))
            {
                qcustno = Request["qcustno"].Trim();
                ViewBag.qcustno = qcustno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qwhno"]))
            {
                qwhno = Request["qwhno"].Trim();
                ViewBag.qwhno = qwhno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {

                qempid = Request["qempid"].Trim();
                ViewBag.qempid = qempid;
            }

            IPagedList<workhour> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "select distinct (select top(1) wid from workhour as a1 where a1.whid= a2.whid )  as wid, whid,wstatus,'' as corp_no, '' as voucher_no, 0 as itemno, '' as empno, 0 as put_in_times, '' as work_no_code, '' as whdptid, '' as comid, '' as bmodid , null as bmoddate, '' as billno, '' as arolestampid, rolestampid, '' as rolestampidall, '' as empstampidall, '' as backrolestampid, '' as backrolestampidall, '' as backempstampidall, '' as rback, '' as billtime, '' as backbilltime, 0 as billflowid, '' as modbackrolestampidall, '' as modbackempstampidall, '' as modback, '' as modbackbilltime, custno,whno,whcomment,wadddate,dptid,empid from workhour as a2 where";
                if (qcustno != "")            
                   sqlstr += " custno in (select comid from allcompany where comid like '%" + qcustno + "%' or comtitle like '%" + qcustno + "%')   and";
                if (qwhno != "")
                    sqlstr += " whno like '%" + qwhno + "%'  and";
                if (qempid != "")
                    sqlstr += " empid like '%" + qempid + "%'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += "group by wid, whid,wstatus,corp_no,whno,voucher_no,itemno,wadddate,dptid,empid,empno,put_in_times,work_no_code,custno,whdptid,whcomment,comid,bmodid,bmoddate,billno,arolestampid,rolestampid,rolestampidall,empstampidall,backrolestampid,backrolestampidall,backempstampidall,rback,billtime,backbilltime,billflowid,modbackrolestampidall,modbackempstampidall,modback,modbackbilltime order by " + orderdata + " " + orderdata1;


                var query = con.workhour.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<workhour>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
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
                    string tmpwhno = "";//工作卡號
                    string tomail = "";
                    #endregion

                    Int64 whid = Int64.Parse(condtionArr[i].Trim());

                    #region 找值並異動資料庫

                    System.Data.SqlClient.SqlConnection connlist = dbobj.get_conn("AitagBill_DBContext");

                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {
                        workhour modobj = con.workhour.Where(r => r.whid == whid).FirstOrDefault();
                        //string tmpmoney = modobj.pallmoney.ToString();

                        //找出下一個角色是誰
                        string impallstring = dbobj.getnewcheck1("T", tmparolestampid, tmparolestampid, "0", "", "", "");
                        string[] tmpstrarr = impallstring.Split(';');
                        tmprole = tmpstrarr[0].ToString();
                        tmpbillid = tmpstrarr[1].ToString();
                        if (tmprole != "")
                        {
                            #region 寄信參數值
                            //===============               
                            System.Data.SqlClient.SqlConnection tmpconn1 = dbobj.get_conn("Aitag_DBContext");
                            tmpsno = modobj.custno;
                            tmpwhno = modobj.whno;
                            tmpdate = dbobj.get_date(modobj.wadddate.ToString(), "1");
                            // tmpvendno = modobj.vendno + "－" + dbobj.get_dbvalue(tmpconn1, "select comsttitle from allcompany where comid = '" + modobj.vendno + "'");
                            // tmpamoney = modobj.totalmoney.ToString();
                            //tmpnote = modobj.pcomment;
                            tmpownman = modobj.empid;
                            bccemail = dbobj.get_dbvalue(tmpconn1, "select enemail from employee where empid = '" + tmpownman + "'");
                            tmpconn1.Close();
                            tmpconn1.Dispose();
                            //===============
                            #endregion

                            //呈核人員
                            //======================
                            //當不是代申請且未選擇(就是沒有下拉)申請呈核角色時，tmparolestampid就接arolestampid1的值
                            string listsql = "select wid from workhour where whid=" + whid;
                            SqlDataReader drlist = dbobj.dbselect(connlist, listsql);


                            while (drlist.Read())
                            {
                                int wid = int.Parse(drlist["wid"].ToString());

                                using (AitagBill_DBContext con1 = new AitagBill_DBContext())
                                {
                                    workhour nmodobj = con1.workhour.Where(r => r.wid == wid).FirstOrDefault();
                                    nmodobj.wstatus = "1";
                                    nmodobj.arolestampid = tmparolestampid; //申請角色
                                    nmodobj.rolestampid = tmprole; //下個呈核角色
                                    nmodobj.rolestampidall = tmparolestampid; //呈核角色
                                    nmodobj.empstampidall = "'" + Session["empid"].ToString() + "'"; //所有人員帳號
                                    nmodobj.billtime = DateTime.Now.ToString(); //所有時間
                                    nmodobj.billflowid = int.Parse(tmpbillid);
                                    nmodobj.bmodid = Session["empid"].ToString();
                                    nmodobj.bmoddate = DateTime.Now;
                                    con1.Entry(nmodobj).State = EntityState.Modified;
                                    con1.SaveChanges();
                                    con1.Dispose();
                                }

                            }

                            drlist.Close();
                            drlist.Dispose();
                    #endregion

                            //con.Entry(modobj).State = EntityState.Modified;
                            //con.SaveChanges();
                            con.Dispose();


                            connlist.Close();
                            connlist.Dispose();


                            #region  寄信
                            string txt_comment = "";
                            string mailtitle = "";
                            string mailcontext = "";

                            mailtitle = "工時填寫作業申請要求審核通知";
                            txt_comment = "有一筆申請單提出，請至系統進行審核作業。<br>資料如下：";

                            mailcontext += "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=450 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                            mailcontext += "<tr>";
                            mailcontext += "<td colspan=2>" + txt_comment + "</td>";
                            mailcontext += "</tr>";

                            mailcontext += "<tr>";
                            mailcontext += "<td align=right width='90'>客戶編號：</td>";
                            mailcontext += "<td>" + tmpsno + "</td>";
                            mailcontext += "</tr>";

                            mailcontext += "<tr>";
                            mailcontext += "<td align=right>工作卡號：</td>";
                            mailcontext += "<td>" + tmpwhno + "</td>";
                            mailcontext += "</tr>";

                            mailcontext += "<tr>";
                            mailcontext += "<td align=right>員工帳號：</td>";
                            mailcontext += "<td>" + tmpownman + "</td>";
                            mailcontext += "</tr>";

                            mailcontext += "<tr>";
                            mailcontext += "<td align=right>工作登入日：</td>";
                            mailcontext += "<td>" + tmpdate + "</td>";
                            mailcontext += "</tr>";


                            //mailcontext += "<tr>";
                            //mailcontext += "<td align=right>摘要說明：</td>";
                            //mailcontext += "<td>" + tmpnote + "</td>";
                            //mailcontext += "</tr>";

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

                            allsno += "【" + modobj.custno + "】、";
                        }
                        else
                        { errmsg += "【" + modobj.custno + "】、"; }


                    }


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
                tmpform += "<form name='qfr1' action='/workhour/list' method='post'>";
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
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = " whid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = " asc"; }

            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcustno = "", qwhno = "", qempid = "";
            if (!string.IsNullOrWhiteSpace(Request["qcustno"]))
            {
                qcustno = Request["qcustno"].Trim();
                ViewBag.qcustno = qcustno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qwhno"]))
            {
                qwhno = Request["qwhno"].Trim();
                ViewBag.qwhno = qwhno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {

                qempid = Request["qempid"].Trim();
                ViewBag.qempid = qempid;
            }

            IPagedList<workhour> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "select distinct (select top(1) wid from workhour as a1 where  a1.whid= a2.whid )  as wid, whid,wstatus,'' as corp_no, '' as voucher_no, 0 as itemno, '' as empno, 0 as put_in_times, '' as work_no_code, '' as whdptid, '' as comid, '' as bmodid , null as bmoddate, '' as billno,arolestampid,rolestampid,rolestampidall, '' as empstampidall, '' as backrolestampid, '' as backrolestampidall, '' as backempstampidall, '' as rback, '' as billtime, '' as backbilltime, 0 as billflowid, '' as modbackrolestampidall, '' as modbackempstampidall, '' as modback, '' as modbackbilltime, custno,whno,whcomment,wadddate,dptid,empid from workhour as a2 where wstatus = '1' and replace(rolestampid,'''','')='" + Session["rid"].ToString() + "'     ";
                if (qcustno != "")
                    sqlstr += " custno in (select comid from allcompany where comid like '%" + qcustno + "%' or comtitle like '%" + qcustno + "%')   and";
                if (qwhno != "")
                    sqlstr += " whno like '%" + qwhno + "%'  and";
                if (qempid != "")
                    sqlstr += " empid like '%" + qempid + "%'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += "group by wid, whid,wstatus,corp_no,whno,voucher_no,itemno,wadddate,dptid,empid,empno,put_in_times,work_no_code,custno,whdptid,whcomment,comid,bmodid,bmoddate,billno,arolestampid,rolestampid,rolestampidall,empstampidall,backrolestampid,backrolestampidall,backempstampidall,rback,billtime,backbilltime,billflowid,modbackrolestampidall,modbackempstampidall,modback,modbackbilltime order by " + orderdata + " " + orderdata1;


                var query = con.workhour.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<workhour>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }

        public ActionResult chkEdit(workhour chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            NDcommon dbobj = new NDcommon();
            int count1 = int.Parse("0" + dbobj.get_dbnull2(Request["count1"]));
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "whid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcustno = "", qwhno = "", qempid = "";
            if (!string.IsNullOrWhiteSpace(Request["qcustno"]))
            {
                qcustno = Request["qcustno"].Trim();
                ViewBag.qcustno = qcustno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qwhno"]))
            {
                qwhno = Request["qwhno"].Trim();
                ViewBag.qwhno = qwhno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {
                qempid = Request["qempid"].Trim();
                ViewBag.qempid = qempid;
            }

            if (sysflag != "E")
            {
                using (AitagBill_DBContext con = new AitagBill_DBContext())
                {
                    var data = con.workhour.Where(r => r.whid == chks.whid).FirstOrDefault();


                    if (data == null)
                    {
                        return HttpNotFound();
                    }
                    return View(data);
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

                    dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM workhour where whid = '" + Request["whid"].ToString() + "'");

                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {

                        string put_in_time = "";
                        string work_no_code = "";
                        for (int p = 0; p <= count1; p++)
                        {
                            //整理欄位
                            chks.bmodid = Session["tempid"].ToString();
                            chks.bmoddate = DateTime.Now;
                            chks.dptid = Session["Dptid"].ToString();
                            chks.empid = Session["tempid"].ToString();

                            //==
                            put_in_time = "put_in_time" + p;
                            work_no_code = "work_no_code" + p;
                            if (dbobj.get_dbnull2(Request[put_in_time]) != "")
                            {
                                chks.put_in_times = int.Parse("0" + Request[put_in_time]) * 30;
                                chks.work_no_code = Request[work_no_code];
                                con.workhour.Add(chks);
                                con.SaveChanges();
                            }
                        }
                    }


                    //系統LOG檔
                    //================================================= //                     
                    SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    string sysrealsid = Request["sysrealsid"].ToString();
                    string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    string sysnote = "客戶別:" + chks.custno + "工作卡號:" + chks.whno;
                    dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    sysconn.Close();
                    sysconn.Dispose();
                    //=================================================

                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/workhour/chkEdit' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qcustno' name='qcustno' value='" + qcustno + "'>";
                    tmpform += "<input type=hidden id='qwhno' name='qwhno' value='" + qwhno + "'>";
                    tmpform += "<input type=hidden id='qempid' name='qempid' value='" + qempid + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    //return RedirectToAction("List");
                }
            }

        }

        public ActionResult chkEditdo(string sysflag, int? page, string orderdata, string orderdata1)
        {

            Int64 whid = Int64.Parse(Request["whid"].ToString().Trim());

            NDcommon dbobj = new NDcommon();

            string wstatus = Request["wstatus"].ToString();
            string tmwstatus = "";
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
            string tmpwhno = "";//工作卡號
            string tomail = "";
            #endregion
            if (wstatus == "1")
            {
                #region  通過時

                //找出下一個角色是誰

                tmprole = dbobj.getnewcheck1("T", tmprolestampid, roleall, "0", "", billflowid);

                if (tmprole == "")
                {
                    return new ContentResult() { Content = @"<body onload=javascript:alert('請先至表單流程設定中設定呈核單據的呈核流程!');window.history.go(-1);>" };
                }

                if (tmprole == "'topman'")
                {
                    tmwstatus = "2";//'己簽核
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
                        tmwstatus = "2";//'己簽核
                    }
                    else
                    { tmwstatus = "1"; }

                    //==========================                
                    #endregion

                }
                #endregion
            }
            else
            {
                tmwstatus = wstatus;
            }

            System.Data.SqlClient.SqlConnection connlist = dbobj.get_conn("AitagBill_DBContext");

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                #region 寄信參數值
                //===============
                workhour modobj = con.workhour.Where(r => r.whid == whid).FirstOrDefault();
                System.Data.SqlClient.SqlConnection tmpconn1 = dbobj.get_conn("Aitag_DBContext");

                tmpsno = modobj.custno;
                tmpwhno = modobj.whno;
                tmpdate = dbobj.get_date(modobj.wadddate.ToString(), "1");
                // tmpvendno = modobj.vendno + "－" + dbobj.get_dbvalue(tmpconn1, "select comsttitle from allcompany where comid = '" + modobj.vendno + "'");
                // tmpamoney = modobj.totalmoney.ToString();
                //tmpnote = modobj.pcomment;
                tmpownman = modobj.empid;
                bccemail = dbobj.get_dbvalue(tmpconn1, "select enemail from employee where empid = '" + tmpownman + "'");
                tmpconn1.Close();
                tmpconn1.Dispose();
                //===============
                #endregion
                //呈核人員
                //======================          

                string listsql = "select wid from workhour where whid=" + whid;
                SqlDataReader drlist = dbobj.dbselect(connlist, listsql);

                while (drlist.Read())
                {
                    int wid = int.Parse(drlist["wid"].ToString());

                    using (AitagBill_DBContext con1 = new AitagBill_DBContext())
                    {
                        workhour nmodobj = con1.workhour.Where(r => r.wid == wid).FirstOrDefault();

                        nmodobj.wstatus = tmwstatus;
                        if (wstatus == "1")
                        {
                            nmodobj.rolestampid = tmprole;  //下個呈核角色

                            nmodobj.rolestampidall = roleall; //所有呈核角色
                            nmodobj.empstampidall = nmodobj.empstampidall + ",'" + Session["empid"].ToString() + "'"; //所有人員帳號
                            nmodobj.billtime = nmodobj.billtime + "," + DateTime.Now.ToString(); //所有時間               
                            nmodobj.bmodid = Session["empid"].ToString();
                            nmodobj.bmoddate = DateTime.Now;
                        }

                        if (wstatus == "D")
                        {
                            #region  退件時
                            if (nmodobj.backrolestampidall != null && nmodobj.backrolestampidall != "")
                            {
                                nmodobj.backrolestampidall = nmodobj.backrolestampidall + "," + tmprolestampid;
                                nmodobj.backempstampidall = nmodobj.backempstampidall + ",'" + Session["empid"].ToString() + "'"; //所有人員帳號
                                nmodobj.backbilltime = nmodobj.backbilltime + "," + DateTime.Now.ToString();
                            }
                            else
                            {
                                nmodobj.backrolestampidall = tmprolestampid;
                                nmodobj.backempstampidall = Session["empid"].ToString() + "'"; //所有人員帳號
                                nmodobj.backbilltime = DateTime.Now.ToString();
                            }


                            //原因
                            if (tmprback != "")
                            {
                                if (nmodobj.rback != null && nmodobj.rback != "")
                                {
                                    nmodobj.rback = nmodobj.rback + "," + tmprback;
                                }
                                else
                                { nmodobj.rback = tmprback; }
                            }

                            #endregion
                        }
                        else
                        {
                            if (wstatus == "3")
                            {
                                #region 3 退回補正

                                if (nmodobj.modbackrolestampidall != null && nmodobj.modbackrolestampidall != "")
                                {
                                    nmodobj.modbackrolestampidall = nmodobj.modbackrolestampidall + "," + tmprolestampid;
                                    nmodobj.modbackempstampidall = nmodobj.modbackempstampidall + ",'" + Session["empid"].ToString() + "'"; //所有人員帳號
                                    nmodobj.modbackbilltime = nmodobj.modbackbilltime + "," + DateTime.Now.ToString();
                                }
                                else
                                {
                                    nmodobj.modbackrolestampidall = tmprolestampid;
                                    nmodobj.modbackempstampidall = Session["empid"].ToString() + "'"; //所有人員帳號
                                    nmodobj.modbackbilltime = DateTime.Now.ToString();
                                }


                                //原因
                                if (tmprback != "")
                                {
                                    if (nmodobj.modback != null && nmodobj.modback != "")
                                    {
                                        nmodobj.modback = nmodobj.modback + "," + tmprback;
                                    }
                                    else
                                    { nmodobj.modback = tmprback; }
                                }
                                #endregion
                            }
                        }
                        con1.Entry(nmodobj).State = EntityState.Modified;
                        con1.SaveChanges();
                        con1.Dispose();
                    }

                }

                drlist.Close();
                drlist.Dispose();

                //con.Entry(modobj).State = EntityState.Modified;
                //con.SaveChanges();
                con.Dispose();
            }


            #region  寄信
            string msgerr = "";
            string txt_comment = "";
            string mailtitle = "";
            string mailcontext = "";

            switch (tmwstatus)
            {
                case "1"://審核中      
                    mailtitle = "工時填寫作業申請要求審核通知";
                    txt_comment = "有一筆申請單提出，請至系統進行審核作業。<br>資料如下：";
                    break;
                case "2"://已審核        
                    mailtitle = "工時填寫作業申請完成審核通知";
                    txt_comment = "您的申請單已通過審核。<br>資料如下：";
                    break;
                case "3"://退回補正   
                    mailtitle = "工時填寫作業申請退回補正通知";
                    txt_comment = "您的申請單有問題，審核者要求您修正。<br>資料如下：";
                    break;
                case "D"://退回      
                    mailtitle = "媒工時填寫作業申請退回通知";
                    txt_comment = "您的申請單被退回。<br>資料如下：";
                    break;
            }

            mailcontext += "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=450 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
            mailcontext += "<tr>";
            mailcontext += "<td colspan=2>" + txt_comment + "</td>";
            mailcontext += "</tr>";


            mailcontext += "<tr>";
            mailcontext += "<td align=right width='90'>客戶編號：</td>";
            mailcontext += "<td>" + tmpsno + "</td>";
            mailcontext += "</tr>";

            mailcontext += "<tr>";
            mailcontext += "<td align=right>工作卡號：</td>";
            mailcontext += "<td>" + tmpwhno + "</td>";
            mailcontext += "</tr>";

            mailcontext += "<tr>";
            mailcontext += "<td align=right>員工帳號：</td>";
            mailcontext += "<td>" + tmpownman + "</td>";
            mailcontext += "</tr>";

            mailcontext += "<tr>";
            mailcontext += "<td align=right>工作登入日：</td>";
            mailcontext += "<td>" + tmpdate + "</td>";
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


            if (tmwstatus == "1")
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
                tmpform += "<form name='qfr1' action='/workhour/chk' method='post'>";
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

                sqlstr = "select  distinct (select top(1) wid from workhour as a1 where a1.whid= a2.whid )  as wid, whid,wstatus,rolestampid,billflowid,rolestampidall from workhour as a2 where whid in (" + cdel + ")";
                cmd.CommandText = sqlstr;
                SqlDataReader alldr = cmd.ExecuteReader();
                while (alldr.Read())
                {
                    Int64 whid = Int64.Parse(alldr["whid"].ToString().Trim());
                    string wstatus = Request["wstatus"].ToString();
                    string tmwstatus = "";

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
                    string tmpwhno = "";//工作卡號
                    string tomail = "";
                    #endregion
                    if (wstatus == "1")
                    {
                        #region  通過時

                        //找出下一個角色是誰

                        tmprole = dbobj.getnewcheck1("T", tmprolestampid, roleall, "0", "", billflowid);

                        if (tmprole == "")
                        {
                            return new ContentResult() { Content = @"<body onload=javascript:alert('請先至表單流程設定中設定呈核單據的呈核流程!');window.history.go(-1);>" };
                        }

                        if (tmprole == "'topman'")
                        {
                            tmwstatus = "2";//'己簽核
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
                                tmwstatus = "2";//'己簽核
                            }
                            else
                            { tmwstatus = "1"; }

                            //==========================                
                            #endregion

                        }
                        #endregion
                    }
                    else
                    {
                        tmwstatus = wstatus;
                    }

                    System.Data.SqlClient.SqlConnection connlist = dbobj.get_conn("AitagBill_DBContext");

                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {
                        #region 寄信參數值
                        //===============
                        workhour modobj = con.workhour.Where(r => r.whid == whid).FirstOrDefault();
                        System.Data.SqlClient.SqlConnection tmpconn1 = dbobj.get_conn("Aitag_DBContext");

                        tmpsno = modobj.custno;
                        tmpwhno = modobj.whno;
                        tmpdate = dbobj.get_date(modobj.wadddate.ToString(), "1");
                        // tmpvendno = modobj.vendno + "－" + dbobj.get_dbvalue(tmpconn1, "select comsttitle from allcompany where comid = '" + modobj.vendno + "'");
                        // tmpamoney = modobj.totalmoney.ToString();
                        //tmpnote = modobj.pcomment;
                        tmpownman = modobj.empid;
                        bccemail = dbobj.get_dbvalue(tmpconn1, "select enemail from employee where empid = '" + tmpownman + "'");
                        tmpconn1.Close();
                        tmpconn1.Dispose();
                        //===============
                        #endregion
                        //呈核人員
                        //======================  
                        #region 存檔
                        string listsql = "select wid from workhour where whid=" + whid;
                        SqlDataReader drlist = dbobj.dbselect(connlist, listsql);


                        while (drlist.Read())
                        {
                            int wid = int.Parse(drlist["wid"].ToString());

                            using (AitagBill_DBContext con1 = new AitagBill_DBContext())
                            {
                                workhour nmodobj = con1.workhour.Where(r => r.wid == wid).FirstOrDefault();

                                nmodobj.wstatus = tmwstatus;
                                if (wstatus == "1")
                                { nmodobj.rolestampid = tmprole; } //下個呈核角色

                                nmodobj.rolestampidall = roleall; //所有呈核角色
                                nmodobj.empstampidall = nmodobj.empstampidall + ",'" + Session["empid"].ToString() + "'"; //所有人員帳號
                                nmodobj.billtime = nmodobj.billtime + "," + DateTime.Now.ToString(); //所有時間                  
                                nmodobj.bmodid = Session["empid"].ToString();
                                nmodobj.bmoddate = DateTime.Now;

                                con1.Entry(nmodobj).State = EntityState.Modified;
                                con1.SaveChanges();
                                con1.Dispose();
                            }
                        }

                        drlist.Close();
                        drlist.Dispose();
                        #endregion

                        //con.Entry(modobj).State = EntityState.Modified;
                        //con.SaveChanges();
                        con.Dispose();
                    }

                    connlist.Close();
                    connlist.Dispose();

                    #region  寄信
                    string msgerr = "";
                    string txt_comment = "";
                    string mailtitle = "";
                    string mailcontext = "";

                    switch (tmwstatus)
                    {
                        case "1"://審核中      
                            mailtitle = "工時審核作業申請要求審核通知";
                            txt_comment = "有一筆申請單提出，請至系統進行審核作業。<br>資料如下：";
                            break;
                        case "2"://已審核        
                            mailtitle = "工時審核作業申請完成審核通知";
                            txt_comment = "您的申請單已通過審核。<br>資料如下：";
                            break;

                    }

                    mailcontext += "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=450 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                    mailcontext += "<tr>";
                    mailcontext += "<td colspan=2>" + txt_comment + "</td>";
                    mailcontext += "</tr>";

                    mailcontext += "<tr>";
                    mailcontext += "<td align=right width='90'>客戶編號：</td>";
                    mailcontext += "<td>" + tmpsno + "</td>";
                    mailcontext += "</tr>";

                    mailcontext += "<tr>";
                    mailcontext += "<td align=right>工作卡號：</td>";
                    mailcontext += "<td>" + tmpwhno + "</td>";
                    mailcontext += "</tr>";

                    mailcontext += "<tr>";
                    mailcontext += "<td align=right>員工帳號：</td>";
                    mailcontext += "<td>" + tmpownman + "</td>";
                    mailcontext += "</tr>";

                    mailcontext += "<tr>";
                    mailcontext += "<td align=right>工作登入日：</td>";
                    mailcontext += "<td>" + tmpdate + "</td>";
                    mailcontext += "</tr>";


                    mailcontext += "</table>";
                    mailcontext += "<br><br><font size='9pt' color=#404040>此為系統自動發信，請勿直接回覆！</font>";


                    if (tmwstatus == "1")
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
                tmpform += "<form name='qfr1' action='/workhour/chk' method='post'>";
                tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                tmpform += "</form>";
                tmpform += "</body>";
            }

            return new ContentResult() { Content = @"" + tmpform };
        }

        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qcustno = "", qwhno = "", qempid = "";
            if (!string.IsNullOrWhiteSpace(Request["qcustno"]))
            {
                qcustno = Request["qcustno"].Trim();
                ViewBag.qcustno = qcustno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qwhno"]))
            {
                qwhno = Request["qwhno"].Trim();
                ViewBag.qwhno = qwhno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {
                qempid = Request["qempid"].Trim();
                ViewBag.qempid = qempid;
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
                        
                        string work_no_code = dbobj.get_dbvalue(conn1, "select work_no_code from workhour where whid='" + condtionArr[i].ToString() + "'");
                        string comid1 = dbobj.get_dbvalue(conn1, "select custno from workhour where whid='" + condtionArr[i].ToString() + "'");
                        string custname = dbobj.get_dbvalue(conn1, "select comtitle from allcompany where comtype like'%1%' and comid='" + comid1 + "'");

                        sysnote += "客戶別:" + custname + "，工作卡號:" + work_no_code + "<br>";

                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM workhour where whid = '" + condtionArr[i].ToString() + "'");

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
                    string tgourl = "/workhour/List?page=" + page + "&qcustno=" + qcustno + "&qwhno=" + qwhno + "&qempid=" + qempid;
                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                    //return RedirectToAction("List");
                }
            }
        }

        public ActionResult workflowdo(string sysflag, int? page, string orderdata, string orderdata1)
        {
            string tmparolestampid = "";
            string tmprole = "";
            string tmpbillid = "";
            NDcommon dbobj = new NDcommon();

            Int64 whid = Int64.Parse(Request["whid"].ToString().Trim());

            #region 寄信參數
            string bccemail = "";
            string tmpsno = "";  //客戶編號
            string tmpdate = "";//工作登入日        
            string tmpnote = "";//摘要說明
            string tmpownman = "";//員工帳號
            string tmpwhno = "";//工作卡號
            string tmpmtitle = "";
            string tomail = "";
            #endregion

            System.Data.SqlClient.SqlConnection connlist = dbobj.get_conn("AitagBill_DBContext");

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                workhour modobj = con.workhour.Where(r => r.whid == whid).FirstOrDefault();

                if (Request["wstatus"].ToString() == "0")  //第一次送時,補件修正不用
                {
                    if (Request["arolestampid"].ToString() != "")
                        tmparolestampid = "'" + Request["arolestampid"].ToString() + "'";
                    else
                        tmparolestampid = "'" + Request["arolestampid1"].ToString() + "'";

                    string tmpmoney = "0";
                    //找出下一個角色是誰               
                    string impallstring = dbobj.getnewcheck1("T", tmparolestampid, tmparolestampid, tmpmoney, "", "", "");

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

                tmpsno = modobj.custno;
                tmpwhno = modobj.whno;
                tmpdate = dbobj.get_date(modobj.wadddate.ToString(), "1");
                // tmpvendno = modobj.vendno + "－" + dbobj.get_dbvalue(tmpconn1, "select comsttitle from allcompany where comid = '" + modobj.vendno + "'");
                // tmpamoney = modobj.totalmoney.ToString();
                //tmpnote = modobj.pcomment;
                tmpownman = modobj.empid;
                //bccemail = dbobj.get_dbvalue(tmpconn1, "select enemail from employee where empid = '" + tmpownman + "'");
                tmpconn1.Close();
                tmpconn1.Dispose();
                //===============
                #endregion

                //呈核人員
                //======================
                //當不是代申請且未選擇(就是沒有下拉)申請呈核角色時，tmparolestampid就接arolestampid1的值

                #region 存檔
            
                string listsql = "select wid from workhour where whid=" + whid;
                SqlDataReader drlist = dbobj.dbselect(connlist, listsql);

              
                    while (drlist.Read())
                    {
                        int wid = int.Parse(drlist["wid"].ToString());

                        using (AitagBill_DBContext con1 = new AitagBill_DBContext())
                        {
                            workhour nmodobj = con1.workhour.Where(r => r.wid == wid).FirstOrDefault();

                            if (Request["wstatus"].ToString() == "0")  //第一次送時,補件修正不用
                            {
                                    nmodobj.wstatus = "1";
                                    nmodobj.arolestampid = tmparolestampid; //申請角色
                                    nmodobj.rolestampid = tmprole; //下個呈核角色
                                    nmodobj.rolestampidall = tmparolestampid; //呈核角色
                                    nmodobj.empstampidall = "'" + Session["empid"].ToString() + "'"; //所有人員帳號
                                    nmodobj.billtime = DateTime.Now.ToString(); //所有時間
                                    nmodobj.billflowid = int.Parse(tmpbillid);
                                    nmodobj.bmodid = Session["empid"].ToString();
                                    nmodobj.bmoddate = DateTime.Now;
                            }
                            else
                            {
                                tmpmtitle = "(補件修正)";
                                nmodobj.wstatus = "1";
                                nmodobj.bmodid = Session["empid"].ToString();
                                nmodobj.bmoddate = DateTime.Now;
                            }
                            con1.Entry(nmodobj).State = EntityState.Modified;
                            con1.SaveChanges();
                            con1.Dispose();
                        }
                               
                    }

                    drlist.Close();
                    drlist.Dispose();
                #endregion

               //con.Entry(modobj).State = EntityState.Modified;
                //con.SaveChanges();
                con.Dispose();
            }

            connlist.Close();
            connlist.Dispose();

            #region  寄信
            string txt_comment = "";
            string mailtitle = "";
            string mailcontext = "";

            mailtitle = "工時填寫作業申請要求審核通知" + tmpmtitle;
            txt_comment = "有一筆申請單提出，請至系統進行審核作業。<br>資料如下：";

            mailcontext += "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=450 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
            mailcontext += "<tr>";
            mailcontext += "<td colspan=2>" + txt_comment + "</td>";
            mailcontext += "</tr>";
            

            mailcontext += "<tr>";
            mailcontext += "<td align=right width='90'>客戶編號：</td>";
            mailcontext += "<td>" + tmpsno + "</td>";
            mailcontext += "</tr>";

            mailcontext += "<tr>";
            mailcontext += "<td align=right>工作卡號：</td>";
            mailcontext += "<td>" + tmpwhno + "</td>";
            mailcontext += "</tr>";

            mailcontext += "<tr>";
            mailcontext += "<td align=right>員工帳號：</td>";
            mailcontext += "<td>" + tmpownman + "</td>";
            mailcontext += "</tr>";
            
            mailcontext += "<tr>";
            mailcontext += "<td align=right>工作登入日：</td>";
            mailcontext += "<td>" + tmpdate + "</td>";
            mailcontext += "</tr>";


            //mailcontext += "<tr>";
            //mailcontext += "<td align=right>摘要說明：</td>";
            //mailcontext += "<td>" + tmpnote + "</td>";
            //mailcontext += "</tr>";

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
            tmpform += "<form name='qfr1' action='/workhour/list' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "</form>";
            tmpform += "</body>";


            return new ContentResult() { Content = @"" + tmpform };
        }



    }
}
