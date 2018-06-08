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
    public class sales_competitionController : BaseController
    {

        //private AitagBill_DBContext db = new AitagBill_DBContext();
        //
        // GET: /sales_competition/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.Msid = Session["Msid"].ToString();
        //    sales_competition col = new sales_competition();
        //    return View(col);
        //}

        //[HttpPost]
        public ActionResult add(sales_competition col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "ccid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qprodtitle = "", qcusttitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qprodtitle"]))
            {
                qprodtitle = Request["qprodtitle"].Trim();
                ViewBag.qprodtitle = qprodtitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcusttitle"]))
            {

                qcusttitle = Request["qcusttitle"].Trim();
                ViewBag.qcusttitle = qcusttitle;
            }

            if (sysflag != "A")
            {
                sales_competition newcol = new sales_competition();
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
                    SqlConnection conn = dbobj.get_conn("AitagBill_DBContext");
                    SqlDataReader dr;
                    SqlCommand sqlsmd = new SqlCommand();
                    sqlsmd.Connection = conn;
                    string sqlstr = "select * from sales_competition where 1<>1";
                    sqlsmd.CommandText = sqlstr;
                    dr = sqlsmd.ExecuteReader();

                    if (dr.Read())
                    {

                        ModelState.AddModelError("", "簡易代碼重複!");
                        return View(col);
                    }
                    dr.Close();
                    dr.Dispose();
                    sqlsmd.Dispose();
                    conn.Close();
                    conn.Dispose();

                    //找出下一個呈核角色
                    #region 找出下一個呈核角色
                    string tmparolestampid = "";
                    string tmprole = "";
                    string tmpbillid = "";
                    if (Request["arolestampid"].ToString() != "")
                    {
                        tmparolestampid = "'" + Request["arolestampid"].ToString() + "'";
                    }

                    string impallstring = dbobj.getnewcheck1("G", tmparolestampid, tmparolestampid, "1", "1", "");
                    tmprole = impallstring.Split(';')[0].ToString();
                    tmpbillid = impallstring.Split(';')[1].ToString();
                    if (tmprole == "")
                    {
                        ViewBag.ErrMsg = @"<script>alert(""請先至表單流程設定中設定新業務及競業呈核流程!"");</script>";
                        return View(col);
                    }
                    #endregion

                    col.custlevel1 = Request["custlevel1"];
                    col.custlevel2 = Request["custlevel2"];
                    col.custlevel3 = Request["custlevel3"];
                    col.iflaw = "";
                    col.iffin = "";
                    col.corpitem = Request["corpitem"];
                    if (!string.IsNullOrWhiteSpace(Request["arolestampid"]))
                    {
                        col.arolestampid = Request["arolestampid"];
                    }
                    else
                    {
                        col.arolestampid = Request["arolestampid1"];
                    }

                    col.slogtype = "1";
                    col.slogstatus = "0"; // 己簽核:1  :0
                    col.rolestampid = tmprole;
                    col.rolestampidall = tmparolestampid;
                    col.empstampidall = "'" + col.empid + "'";
                    col.billflowid = int.Parse(tmpbillid);
                    col.billtime = DateTime.Now.ToString();
                    col.ccid = Decimal.Parse(DateTime.Now.ToString("yyyyMMddhhmmssff"));
                    col.bmodid = Session["tempid"].ToString();
                    col.bmoddate = DateTime.Now;
                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {
                        con.sales_competition.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "承辦人:" + col.empid + "產品名稱:" + col.prodtitle;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/sales_competition/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qcusttitle' name='qcusttitle' value='" + qcusttitle + "'>";
                    tmpform += "<input type=hidden id='qprodtitle' name='qprodtitle' value='" + qprodtitle + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    // return RedirectToAction("List");

                }
            }


        }


        [ValidateInput(false)]
        public ActionResult Edit(sales_competition chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "ccid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qprodtitle = "", qcusttitlename = "";
            
            if (!string.IsNullOrWhiteSpace(Request["qprodtitle"]))
            {

                qprodtitle = Request["qprodtitle"].Trim();
                ViewBag.qprodtitle = qprodtitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcusttitlename"]))
            {

                qcusttitlename = Request["qcusttitlename"].Trim();
                ViewBag.qprodtitle = qcusttitlename;
            }

            if (sysflag != "E")
            {
                using (AitagBill_DBContext con = new AitagBill_DBContext())
                {
                    var data = con.sales_competition.Where(r => r.ccid == chks.ccid).FirstOrDefault();
                    
                    
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

                    //string oldmsid = Request["oldmsid"];  
                    

                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {
                        var chdata = con.sales_competition.Where(r => r.ccid == chks.ccid).FirstOrDefault();
                       
                        NDcommon dbobj = new NDcommon();
                        chdata.indclass = Request["indclass"];
                        chdata.prodtitle = Request["prodtitle"];
                        chdata.custtitle = Request["custtitle"];
                        chdata.custlevel1 = Request["custlevel1"];
                        chdata.custlevel2 = Request["custlevel2"];
                        chdata.custlevel3 = Request["custlevel3"];
                        chdata.corpitem = Request["corpitem"];
                        chdata.getcomtitle = Request["getcomtitle"];
                        chdata.ifget = Request["ifget"];
                        chdata.bmodid = Session["tempid"].ToString();
                        chdata.bmoddate = DateTime.Now;
                        con.Entry(chdata).State = EntityState.Modified;
                        con.SaveChanges();


                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "承辦人:" + chks.empid + "產品名稱:" + chks.prodtitle;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/sales_competition/List' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                        tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                        tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                        tmpform += "<input type=hidden id='qprodtitle' name='qprodtitle' value='" + qprodtitle + "'>";
                        tmpform += "<input type=hidden id='qcusttitlename' name='qcusttitlename' value='" + qcusttitlename + "'>";
                        tmpform += "</form>";
                        tmpform += "</body>";


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
            { orderdata = " ccid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = " asc"; }

            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcusttitle = "", qprodtitle = "";
            
            if (!string.IsNullOrWhiteSpace(Request["qprodtitle"]))
            {
                qprodtitle = Request["qprodtitle"].Trim();
                ViewBag.qprodtitle = qprodtitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcusttitle"]))
            {

                qcusttitle = Request["qcusttitle"].Trim();
                ViewBag.qcusttitle = qcusttitle;
            }

            IPagedList<sales_competition> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "select * from sales_competition where empid = '" + Session["empid"] + "'  and";
               
                if (qprodtitle != "")
                    sqlstr += " prodtitle like '%" + qprodtitle + "%'  and";
                if (qcusttitle != "")
                    sqlstr += " custtitle like '%" + qcusttitle + "%'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1 ;

                var query = con.sales_competition.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<sales_competition>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }

        public ActionResult manage(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = " ccid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = " asc"; }

            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcusttitle = "", qprodtitle = "";

            if (!string.IsNullOrWhiteSpace(Request["qprodtitle"]))
            {
                qprodtitle = Request["qprodtitle"].Trim();
                ViewBag.qprodtitle = qprodtitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcusttitle"]))
            {

                qcusttitle = Request["qcusttitle"].Trim();
                ViewBag.qcusttitle = qcusttitle;
            }

            IPagedList<sales_competition> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "select * from sales_competition where";

                if (qprodtitle != "")
                    sqlstr += " prodtitle like '%" + qprodtitle + "%'  and";
                if (qcusttitle != "")
                    sqlstr += " custtitle like '%" + qcusttitle + "%'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.sales_competition.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<sales_competition>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }


        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string qcusttitle = "", qprodtitle="";
            
            if (!string.IsNullOrWhiteSpace(Request["qcusttitle"]))
            {
                qcusttitle = Request["qcusttitle"].Trim();
                ViewBag.qcusttitle = qcusttitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["qprodtitle"]))
            {
                qprodtitle = Request["qprodtitle"].Trim();
                ViewBag.qprodtitle = qprodtitle;
            }
            string cdel = Request["cdel"];

            if (string.IsNullOrWhiteSpace(cdel))
            {

                string tgourl = "/sales_competition/List?page=" + page + "&qcusttitle=" + qcusttitle + "&qprodtitle=" + qprodtitle;
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

                        sysnote += "序號:" + condtionArr[i].ToString() + "<br>";

                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM sales_competition where ccid = '" + condtionArr[i].ToString() + "'");

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
                    string tgourl = "/sales_competition/List?page=" + page + "&qcusttitle=" + qcusttitle + "&qprodtitle=" + qprodtitle;
                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');location.href='" + tgourl + "'</script>" };

                    //return RedirectToAction("List");
                }
            }
        }

        public ActionResult chk(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = " ccid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = " asc"; }

            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcusttitle = "", qprodtitle = "";

            if (!string.IsNullOrWhiteSpace(Request["qprodtitle"]))
            {
                qprodtitle = Request["qprodtitle"].Trim();
                ViewBag.qprodtitle = qprodtitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcusttitle"]))
            {

                qcusttitle = Request["qcusttitle"].Trim();
                ViewBag.qcusttitle = qcusttitle;
            }

            IPagedList<sales_competition> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "select * from sales_competition where (slogtype='1' and slogstatus = '0' and rolestampid = '''" + Session["rid"] + "''')   and";

                if (qprodtitle != "")
                    sqlstr += " prodtitle like '%" + qprodtitle + "%'  and";
                if (qcusttitle != "")
                    sqlstr += " custtitle like '%" + qcusttitle + "%'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.sales_competition.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<sales_competition>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }

        [ValidateInput(false)]
        public ActionResult chkEdit(sales_competition chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "ccid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qprodtitle = "", qcusttitlename = "";

            if (!string.IsNullOrWhiteSpace(Request["qprodtitle"]))
            {

                qprodtitle = Request["qprodtitle"].Trim();
                ViewBag.qprodtitle = qprodtitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcusttitlename"]))
            {

                qcusttitlename = Request["qcusttitlename"].Trim();
                ViewBag.qprodtitle = qcusttitlename;
            }

            if (sysflag != "E")
            {
                using (AitagBill_DBContext con = new AitagBill_DBContext())
                {
                    var data = con.sales_competition.Where(r => r.ccid == chks.ccid).FirstOrDefault();

                    sales_competition competitionlogs = con.sales_competition.Find(chks.ccid);
                    if (competitionlogs == null)
                    {
                        return HttpNotFound();
                    }
                    return View(competitionlogs);
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

                    

                    NDcommon dbobj = new NDcommon();
                    sales_competition col = new sales_competition();
                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {
                        col = con.sales_competition.Find(chks.ccid);
                    }

                    string sysnote = "";
                    if (dbobj.get_dbnull2(Request["slogstatus"]) == "1")
                    {
                        string tmprolestampid = col.rolestampid;
                        string rolea_1 = col.rolestampidall;
                        string roleall = rolea_1 + "," + tmprolestampid; //'簽核過角色(多個)
                        string billflowid = col.billflowid.ToString();

                        //找出下一個角色是誰
                        string tmprole = dbobj.getnewcheck1("G", tmprolestampid, roleall, "", "", billflowid);

                        if (tmprole == "'topman'")
                        {
                            tmprole = "";
                        }
                        string slogstatus = "";
                        if (tmprole == "")
                        {
                            slogstatus = "1";// '己簽核
                        }
                        else
                        {
                            slogstatus = "0";
                            //'找往上呈核長管級數
                            //'==========================
                            string tmpflowlevel = "";
                            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                            {
                                tmpflowlevel = dbobj.get_dbvalue(conn, "select billflow from flowlevel where bid=" + billflowid);
                            }
                            if (tmpflowlevel == "")
                            {
                                tmpflowlevel = "0";
                            }
                            string[] tmpa = rolea_1.Split(',');
                            int tmpacount = tmpa.Length;
                            if (int.Parse(tmpflowlevel) == (tmpacount + 1))
                            {
                                tmprole = "";
                                slogstatus = "1"; // '己簽核
                            }
                            //'==========================
                        }

                        col.slogstatus = slogstatus;
                        col.rolestampid = tmprole;
                        col.rolestampidall = roleall;
                        col.empstampidall = col.empstampidall + ",'" + (string)Session["empid"] + "'"; //'所有人員帳號
                        col.bmodid = (string)Session["empid"];
                        col.bmoddate = DateTime.Now;
                        col.billtime = col.billtime + "," + DateTime.Now.ToString();
                        col.lawcomment = Request["lawcomment"].ToString();
                        col.fincomment = Request["fincomment"].ToString();
                        if (tmprole != "")
                        {
                            //寄信
                            //holidaycheckmainEditMail(col, tmprole);
                        }
                        else
                        {
                            //沒有下一個承辦人  (己通過)
                            ////資料通過後 搬移到cardreallog
                            //battacheckmainEditMove(col);

                            //(己通過)  寄信
                            //holidaycheckmainEditMailPass(col);
                        }
                        sysnote = "請假單審核通過作業";
                    }
                    else
                    {
                        col.slogstatus = "2";
                        col.rback = Request["hback1"].ToString();
                        col.bmodid = (string)Session["empid"];
                        col.bmoddate = DateTime.Now;
                        col.billtime = col.billtime + "," + DateTime.Now.ToString();
                        
                        ////資料通過後 搬移到cardreallog
                        //battacheckmainEditMove(col);

                        //(己通過)  寄信
                        //holidaycheckmainEditMailBack(col);
                        sysnote = "新業務及競業退回作業";
                    }

                    col.bmodid = Session["tempid"].ToString();
                    col.bmoddate = DateTime.Now;

                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {
                        con.Entry(col).State = EntityState.Modified;
                        con.SaveChanges();


                    }
                   


                    //系統LOG檔
                    //================================================= //                     
                    SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    string sysrealsid = Request["sysrealsid"].ToString();
                    string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    //string sysnote = "承辦人:" + chks.empid + "產品名稱:" + chks.prodtitle;
                    dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    sysconn.Close();
                    sysconn.Dispose();
                    //=================================================

                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/sales_competition/chk' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qprodtitle' name='qprodtitle' value='" + qprodtitle + "'>";
                    tmpform += "<input type=hidden id='qcusttitlename' name='qcusttitlename' value='" + qcusttitlename + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    //return RedirectToAction("List");
                }
            }

        }

        public ActionResult csvsales_competition(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = " ccid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = " asc"; }

            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcusttitle = "", qprodtitle = "";

            if (!string.IsNullOrWhiteSpace(Request["qprodtitle"]))
            {
                qprodtitle = Request["qprodtitle"].Trim();
                ViewBag.qprodtitle = qprodtitle;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcusttitle"]))
            {

                qcusttitle = Request["qcusttitle"].Trim();
                ViewBag.qcusttitle = qcusttitle;
            }

            //IPagedList<systemlog> result;
            string Excel = "", Excel2 = "";
            string sqlstr = "";
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                sqlstr = "select * from sales_competition where";

                if (qprodtitle != "")
                    sqlstr += " prodtitle like '%" + qprodtitle + "%'  and";
                if (qcusttitle != "")
                    sqlstr += " custtitle like '%" + qcusttitle + "%'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                //var query = con.systemlog.SqlQuery(sqlstr).AsQueryable();
                //result = query.ToPagedList<systemlog>(0, 10000);


            }

            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            System.Data.SqlClient.SqlConnection comconn = dbobj.get_conn("Aitag_DBContext");  
            Excel += "<HTML>";
            Excel += "<HEAD>";
            Excel += @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">";
            Excel += "</HEAD>";
            Excel += "<body>";
            Excel += "<table  border=1  cellpadding=0 cellspacing=0 bordercolor=#000000 bordercolordark=#ffffff width=900 >";
            Excel += "<tr align=center><td colspan='28'>聯廣傳播集團    New Business (NewB)控管表</td></tr>";
            Excel += "<tr><td colspan='28'>資料日期：" + DateTime.Today.ToShortDateString();
            Excel += "</td></tr>";
            Excel += "<tr align=center>";
            Excel += "<td rowspan='2'>序號</td>";
            Excel += "<td rowspan='2'>公司名稱</td>";
            Excel += "<td rowspan='2'>客戶名稱</td>";
            Excel += "<td rowspan='2'>產業</td>";
            Excel += "<td rowspan='2'>產品</td>";
            Excel += "<td rowspan='2'>業務部門</td>";
            Excel += "<td rowspan='2'>申請日期</td>";
            Excel += "<td rowspan='2'>預算金額</td>";
            Excel += "<td rowspan='2'>客戶分級</td>";
            Excel += "<td colspan='2' >類型</td>";
            Excel += "<td rowspan='2'>比稿/提案時間</td>";
            Excel += "<td colspan='8'>合作項目</td>";
          //  Excel += "<td rowspan='2'>業務組</td>";
          //  Excel += "<td colspan='3'>收費方式</td>";
            Excel += "<td colspan='2' >競業條款評估</td>";
            Excel += "<td rowspan='2'>財務說明</td>";
            Excel += "<td colspan='2' >決議</td>";
            Excel += "<td colspan='2' >結果回報</td>";
            Excel += "</tr>";
         
            Excel += "<tr align=center>";
            Excel += "<td>比稿</td>";
            Excel += "<td>提案</td>";
            Excel += "<td>平面廣告</td>";
            Excel += "<td>廣告片</td>";
            Excel += "<td>公關行銷</td>";
            Excel += "<td>網路</td>";
            Excel += "<td>市場<br>調查</td>";
            Excel += "<td>活動/贈品</td>";
            Excel += "<td>媒體企劃/購買</td>";
            Excel += "<td>其他</td>";
            Excel += "<td>無</td>";
            Excel += "<td>有</td>";
            Excel += "<td>參加</td>";
            Excel += "<td>不參加</td>";
            Excel += "<td>得標</td>";
            Excel += "<td>未得標</td>";
            Excel += "<td>得標公司</td>";
            Excel += "</tr>";
           
            using (SqlConnection conn = dbobj.get_conn("AitagBill_DBContext"))
            {
                using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    string sflag = "", empname = "";
                    int j = 0;
                    while (dr.Read())
                    {
                        j = j + 1;
                        
                        Excel2 += "<tr>";
                        Excel2 += "<td>" + j + "</td>";
                        Excel2 += "<td>" + dbobj.get_dbvalue(comconn,"select comsttitle from company where comid = '" + dr["comid"].ToString() + "'") + "&nbsp;</td>";
                        Excel2 += "<td>" + dr["custtitle"].ToString() + "</td>";
                        Excel2 += "<td>" + dbobj.get_dbnull2(dr["indclass"]) + "</td>";
                        Excel2 += "<td>" + dr["prodtitle"].ToString() + "</td>";
                        Excel2 += "<td>&nbsp;</td>";
                        Excel2 += "<td>" + dr["adddate"].ToString() + "</td>";
                        Excel2 += "<td>" + decimal.Parse(dr["salespmoney"].ToString()).ToString("###,###,###") + "</td>";
                        //分級
                        string level1 = "";
                        if (dbobj.get_dbnull2(dr["custlevel1"]).ToString() != "")
                            level1 = "A,";
                        if (dbobj.get_dbnull2(dr["custlevel2"]).ToString() != "")
                            level1 += "B,";
                        if (dbobj.get_dbnull2(dr["custlevel3"]).ToString() != "")
                            level1 += "C";
                        Excel2 += "<td>" + level1 + "</td>";
                        //類型
                        if (dbobj.get_dbnull2(dr["steptype"]).ToString() == "0")
                        { Excel2 += "<td>V</td>"; }
                        else
                        {
                          Excel2 += "<td>&nbsp;</td>";
                        }

                        if (dbobj.get_dbnull2(dr["steptype"]).ToString() == "1")
                        { Excel2 += "<td>V</td>"; }
                        else
                        {
                            Excel2 += "<td>&nbsp;</td>";
                        }
                        //比稿提案時間
                        Excel2 += "<td>" + dr["exetime"].ToString()  + "</td>";
                        //合作項目
                        if (dr["corpitem"].ToString().IndexOf("01") >= 0) { 
                        Excel2 += "<td align=center>V</td>";
                        }
                        else
                        {
                            Excel2 += "<td align=center></td>";
                        }
                        if (dr["corpitem"].ToString().IndexOf("02") >= 0)
                        {
                            Excel2 += "<td align=center>V</td>";
                        }
                        else
                        {
                            Excel2 += "<td align=center></td>";
                        }
                        if (dr["corpitem"].ToString().IndexOf("03") >= 0)
                        {
                            Excel2 += "<td align=center>V</td>";
                        }
                        else
                        {
                            Excel2 += "<td align=center></td>";
                        }
                        if (dr["corpitem"].ToString().IndexOf("04") >= 0)
                        {
                            Excel2 += "<td align=center>V</td>";
                        }
                        else
                        {
                            Excel2 += "<td align=center></td>";
                        }
                        if (dr["corpitem"].ToString().IndexOf("05") >= 0)
                        {
                            Excel2 += "<td align=center>V</td>";
                        }
                        else
                        {
                            Excel2 += "<td align=center></td>";
                        }
                        if (dr["corpitem"].ToString().IndexOf("06") >= 0)
                        {
                            Excel2 += "<td align=center>V</td>";
                        }
                        else
                        {
                            Excel2 += "<td align=center></td>";
                        }
                        if (dr["corpitem"].ToString().IndexOf("07") >= 0)
                        {
                            Excel2 += "<td align=center>V</td>";
                        }
                        else
                        {
                            Excel2 += "<td align=center></td>";
                        }
                        if (dr["corpitem"].ToString().IndexOf("99") >= 0)
                        {
                            Excel2 += "<td align=center>V</td>";
                        }
                        else
                        {
                            Excel2 += "<td align=center></td>";
                        }
                        //法務
                        if (dbobj.get_dbnull2(dr["iflaw"]).ToString() == "n")
                        { Excel2 += "<td>V</td>"; }
                        else
                        {
                            Excel2 += "<td align=center></td>";
                        }
                        if (dbobj.get_dbnull2(dr["iflaw"]).ToString() == "y")
                        { Excel2 += "<td>V " + dr["lawcomment"] + "</td>"; }
                        else
                        {
                            Excel2 += "<td align=center></td>";
                        }
                        //財務
                        Excel2 += "<td>" + dr["fincomment"] + "</td>";
                        //參加不參加
                        if (dbobj.get_dbnull2(dr["slogstatus"]).ToString() == "1")
                        {
                            Excel2 += "<td>V</td>";
                        }
                        else
                        {
                            Excel2 += "<td>&nbsp;</td>";
                        }

                        if (dbobj.get_dbnull2(dr["slogstatus"]).ToString() == "D")
                        {
                            Excel2 += "<td>V</td>";
                        }
                        else
                        {
                            Excel2 += "<td>&nbsp;</td>";
                        }
                        //結果回報
                        if (dbobj.get_dbnull2(dr["ifget"]).ToString() == "y") 
                        { 
                        Excel2 += "<td>V</td>";
                        }
                        else
                        {
                            Excel2 += "<td>&nbsp;</td>";
                        }
                        if (dbobj.get_dbnull2(dr["ifget"]).ToString() == "n")
                        {
                            Excel2 += "<td>V</td>";
                        }
                        else
                        {
                            Excel2 += "<td>&nbsp;</td>";
                        }
                        Excel2 += "<td>" + dr["getcomtitle"] + "</td>";
                        Excel2 += "</tr>";
                    }
                    //Excel2 += "<tr></tr>";
                    //Excel2 += "<tr></tr>";
                    //Excel2 += "<tr>";
                    //Excel2 += "<td></td>";
                    //Excel2 += "<td>製表人：</td>";
                    //Excel2 += "<td colspan='3'>法務主管：</td>";
                    //Excel2 += "<td colspan='6'>財務長：</td>";
                    //Excel2 += "<td colspan='5'>母公司總經理：</td>";
                    //Excel2 += "<td colspan='3'>董事長：</td>";
                    Excel2 += "</tr>";
                    if (Excel2 == "")
                    {
                        Excel += "<tr align=left><td colspan=19>目前沒有資料</td></tr>";
                    }
                    else
                    {
                        Excel += Excel2;
                    }
                    dr.Close();
                }
            }


            Excel += "</table>";
            Excel += "</body>";
            Excel += "</HTML>";


            comconn.Close();
            comconn.Dispose();
            ViewBag.Excel = Excel;
            return View();
        }



    }
}
