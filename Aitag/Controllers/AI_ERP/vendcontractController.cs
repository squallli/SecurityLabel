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
    public class vendcontractController : BaseController
    {


        public ActionResult Index()
        {
            //string ip = Request.UserHostAddress;
            //ViewBag.mname = System.Net.Dns.GetHostByAddress(ip);
            return View();
        }

        public ActionResult list(int? page, string orderdata, string orderdata1)
        {
            //ViewBag.mname = Environment.MachineName;
            IPagedList<vend_contract> result;
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "vcid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qpdno = "", qallcomid = "", qvcno = "", qvcsdate = "", qvcedate = "";

            if (!string.IsNullOrWhiteSpace(Request["qallcomid"]))
            {
                qallcomid = Request["qallcomid"].Trim();
                ViewBag.qvendno = qallcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcno"]))
            {
                qvcno = Request["qvcno"].Trim();
                ViewBag.qvcno = qvcno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcsdate"]))
            {
                qvcsdate = Request["qvcsdate"].Trim();
                ViewBag.qvcsdate = qvcsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcedate"]))
            {
                qvcedate = Request["qvcedate"].Trim();
                ViewBag.qvcedate = qvcedate;
            }

            if (!string.IsNullOrWhiteSpace(Request["qpdno"]))
            {
                qpdno = Request["qpdno"].Trim();
                ViewBag.qpdno = qpdno;
            }

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "";
                sqlstr = "select * from vend_contract where comid = '" + Session["comid"].ToString() + "' and vendtype = '2'   and";

                if (qallcomid != "")
                    sqlstr += " allcomid in (select comid from allcompany where comid like '%" + qallcomid + "%' or comtitle like '%" + qallcomid + "%')   and";
                if (qvcno != "")
                    sqlstr += " vcno like '%" + qvcno + "%'  and";
                if (qvcsdate != "")
                    sqlstr += " vcdate >= '" + qvcsdate + "'  and";
                if (qvcedate != "")
                    sqlstr += " vcdate <= '" + qvcedate + "'  and";
                if (qpdno != "")
                    sqlstr += " pdno like '%" + qpdno + "%'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.vend_contract.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<vend_contract>(page.Value - 1, (int)Session["pagesize"]);
            }
            return View(result);
        }


        public ActionResult budgetlist(int? page, string orderdata, string orderdata1)
        {
            //ViewBag.mname = Environment.MachineName;
            IPagedList<vend_contract> result;
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "vcid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qpdno = "", qallcomid = "", qvcno = "", qvcsdate = "", qvcedate = "";

            if (!string.IsNullOrWhiteSpace(Request["qallcomid"]))
            {
                qallcomid = Request["qallcomid"].Trim();
                ViewBag.qvendno = qallcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcno"]))
            {
                qvcno = Request["qvcno"].Trim();
                ViewBag.qvcno = qvcno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcsdate"]))
            {
                qvcsdate = Request["qvcsdate"].Trim();
                ViewBag.qvcsdate = qvcsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcedate"]))
            {
                qvcedate = Request["qvcedate"].Trim();
                ViewBag.qvcedate = qvcedate;
            }

            if (!string.IsNullOrWhiteSpace(Request["qpdno"]))
            {
                qpdno = Request["qpdno"].Trim();
                ViewBag.qpdno = qpdno;
            }

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "";
                sqlstr = "select * from vend_contract where comid = '" + Session["comid"].ToString() + "' and vendtype = '1'   and";

                if (qallcomid != "")
                    sqlstr += " allcomid in (select comid from allcompany where comid like '%" + qallcomid + "%' or comtitle like '%" + qallcomid + "%')   and";
                if (qvcno != "")
                    sqlstr += " vcno like '%" + qvcno + "%'  and";
                if (qvcsdate != "")
                    sqlstr += " vcdate >= '" + qvcsdate + "'  and";
                if (qvcedate != "")
                    sqlstr += " vcdate <= '" + qvcedate + "'  and";
                if (qpdno != "")
                    sqlstr += " pdno like '%" + qpdno + "%'  and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.vend_contract.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<vend_contract>(page.Value - 1, (int)Session["pagesize"]);
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

            string qpdno = "", qallcomid = "", qvcno = "", qvcsdate = "", qvcedate = "";

            if (!string.IsNullOrWhiteSpace(Request["qallcomid"]))
            {
                qallcomid = Request["qallcomid"].Trim();
                ViewBag.qvendno = qallcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcno"]))
            {
                qvcno = Request["qvcno"].Trim();
                ViewBag.qvcno = qvcno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcsdate"]))
            {
                qvcsdate = Request["qvcsdate"].Trim();
                ViewBag.qvcsdate = qvcsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcedate"]))
            {
                qvcedate = Request["qvcedate"].Trim();
                ViewBag.qvcedate = qvcedate;
            }

            if (!string.IsNullOrWhiteSpace(Request["qpdno"]))
            {
                qpdno = Request["qpdno"].Trim();
                ViewBag.qpdno = qpdno;
            }


            NDcommon dbobj = new NDcommon();
            SqlConnection conn = dbobj.get_conn("AitagBill_DBContext");
            string vcno = "";

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {

                vend_contract addobj = new vend_contract();
                vcno = dbobj.get_billno(conn, "C", "", Request["comid"].ToString(), "", Request["pdate"].ToString());
                addobj.vcid = Decimal.Parse(DateTime.Now.ToString("yyyyMMddhhmmssff"));
                addobj.vcdate = DateTime.Parse(Request["pdate"].ToString());
                addobj.comid = Request["comid"];
                addobj.vcno = vcno;
                addobj.projno = vcno;
                addobj.vcstatus = "0";
                addobj.vendtype = "2";
                addobj.taxtype = Request["taxtype"];
                addobj.ownman = Session["empid"].ToString();

                addobj.vcmoney = int.Parse(Request["vcmoney"]);
                addobj.vctaxmoney = int.Parse(Request["vctaxmoney"]);
                addobj.vcallmoney = int.Parse(Request["vcallmoney"]);

                addobj.allcomid = Request["allcomid"].Trim();
                addobj.vccomment = Request["vccomment"].Trim();

                addobj.bmodid = Session["empid"].ToString();
                addobj.bmoddate = DateTime.Now;

                con.vend_contract.Add(addobj);

                con.SaveChanges();
                con.Dispose();

            }

            conn.Close();
            conn.Dispose();

            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/custcontract/list' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden id='qpdno' name='qpdno' value='" + qpdno + "'>";
            tmpform += "<input type=hidden id='qallcomid' name='qallcomid' value='" + qallcomid + "'>";
            tmpform += "<input type=hidden id='qvcno' name='qvcno' value='" + qvcno + "'>";
            // tmpform += "<input type=hidden id='qpcomment' name='qpcomment' value='" + qpcomment + "'>";
            tmpform += "<input type=hidden id='qvcsdate' name='qvcsdate' value='" + qvcsdate + "'>";
            tmpform += "<input type=hidden id='qvcedate' name='qvcedate' value='" + qvcedate + "'>";
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

            string qpdno = "", qallcomid = "", qvcno = "", qvcsdate = "", qvcedate = "";

            if (!string.IsNullOrWhiteSpace(Request["qallcomid"]))
            {
                qallcomid = Request["qallcomid"].Trim();
                ViewBag.qvendno = qallcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcno"]))
            {
                qvcno = Request["qvcno"].Trim();
                ViewBag.qvcno = qvcno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcsdate"]))
            {
                qvcsdate = Request["qvcsdate"].Trim();
                ViewBag.qvcsdate = qvcsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcedate"]))
            {
                qvcedate = Request["qvcedate"].Trim();
                ViewBag.qvcedate = qvcedate;
            }

            if (!string.IsNullOrWhiteSpace(Request["qpdno"]))
            {
                qpdno = Request["qpdno"].Trim();
                ViewBag.qpdno = qpdno;
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
                        string vcno = dbobj.get_dbvalue(conn1, "select vcno from vend_contract where vcid='" + condtionArr[i].ToString() + "'");

                        sysnote += "單號:" + vcno + "<br>";
                        //刪除憑單
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM vend_contract where vcid = '" + condtionArr[i].ToString() + "'");
                        //刪除明細
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM vend_contractdet where vcid = '" + condtionArr[i].ToString() + "'");

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
                    tmpform += "<form name='qfr1' action='/custcontract/list' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    // tmpform += "<input type=hidden id='qpdno' name='qpdno' value='" + qpdno + "'>";
                    tmpform += "<input type=hidden id='qallcomid' name='qallcomid' value='" + qallcomid + "'>";
                    tmpform += "<input type=hidden id='qvcno' name='qvcno' value='" + qvcno + "'>";
                    // tmpform += "<input type=hidden id='qpcomment' name='qpcomment' value='" + qpcomment + "'>";
                    tmpform += "<input type=hidden id='qvcsdate' name='qvcsdate' value='" + qvcsdate + "'>";
                    tmpform += "<input type=hidden id='qvcedate' name='qvcedate' value='" + qvcedate + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');</script>" + tmpform };
                }
            }
        }

        public ActionResult Edit()
        {

            ViewBag.vcid = Request["vcid"].ToString();
            return View();
        }

        public ActionResult Editdo(string sysflag, int? page, string orderdata, string orderdata1)
        {

            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            string qpdno = "", qallcomid = "", qvcno = "", qvcsdate = "", qvcedate = "";

            if (!string.IsNullOrWhiteSpace(Request["qallcomid"]))
            {
                qallcomid = Request["qallcomid"].Trim();
                ViewBag.qvendno = qallcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcno"]))
            {
                qvcno = Request["qvcno"].Trim();
                ViewBag.qvcno = qvcno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcsdate"]))
            {
                qvcsdate = Request["qvcsdate"].Trim();
                ViewBag.qvcsdate = qvcsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvcedate"]))
            {
                qvcedate = Request["qvcedate"].Trim();
                ViewBag.qvcedate = qvcedate;
            }

            if (!string.IsNullOrWhiteSpace(Request["qpdno"]))
            {
                qpdno = Request["qpdno"].Trim();
                ViewBag.qpdno = qpdno;
            }

            int vcid = int.Parse(Request["vcid"].ToString());
            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();

            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                vend_contract modobj = con.vend_contract.Where(r => r.vcid == vcid).FirstOrDefault();

                modobj.vcdate = DateTime.Parse(Request["vcdate"].ToString());
                modobj.vcsdate = DateTime.Parse(Request["vcsdate"].ToString());
                modobj.vcedate = DateTime.Parse(Request["vcedate"].ToString());
                modobj.comid = Request["comid"];
                modobj.projno = Request["projno"];

                modobj.taxtype = Request["taxtype"];
                modobj.vcmoney = int.Parse(Request["vcmoney"]);
                modobj.vctaxmoney = int.Parse(Request["vctaxmoney"]);
                modobj.vcallmoney = int.Parse(Request["vcallmoney"]);
                modobj.vccomment = Request["vccomment"];
                modobj.allcomid = Request["allcomid"];
                modobj.bmodid = Session["empid"].ToString();
                modobj.bmoddate = DateTime.Now;
                con.Entry(modobj).State = EntityState.Modified;
                con.SaveChanges();
                con.Dispose();
            }
            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/custcontract/list' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden id='qpdno' name='qpdno' value='" + qpdno + "'>";
            tmpform += "<input type=hidden id='qallcomid' name='qallcomid' value='" + qallcomid + "'>";
            tmpform += "<input type=hidden id='qvcno' name='qvcno' value='" + qvcno + "'>";
            //tmpform += "<input type=hidden id='qpcomment' name='qpcomment' value='" + qpcomment + "'>";
            tmpform += "<input type=hidden id='qvcsdate' name='qvcsdate' value='" + qvcsdate + "'>";
            tmpform += "<input type=hidden id='qvcedate' name='qvcedate' value='" + qvcedate + "'>";
            tmpform += "</form>";
            tmpform += "</body>";


            return new ContentResult() { Content = @"" + tmpform };
        }

        public ActionResult detlist()
        {
            int tmppid = 0;
            tmppid = int.Parse(Request["vcid"].ToString());
            ViewBag.vcid = tmppid.ToString();

            List<vend_contractdet> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                var query = con.vend_contractdet.AsQueryable();
                result = query.Where(r => r.vcid == tmppid).AsQueryable().ToList();

            }
            return View(result);

        }

        public ActionResult detlistdo(string sysflag, int? page, string orderdata, string orderdata1)
        {

            string vcid = "";
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {

                page = ((!page.HasValue || page < 1) ? 1 : page);
                ViewBag.page = page;
                ViewBag.orderdata = orderdata;
                ViewBag.orderdata1 = orderdata1;
                string sqlstr = "";
                string cdel1 = Request["pdid"];

                string bdprodno1 = Request["bdprodno"];
                string bdprodtitle1 = Request["bdprodtitle"];
                //string mdno1 = Request["mdno"];
                //string mdcomment1 = Request["mdcomment"];
                string pdunit1 = Request["pdunit"];
                string vccount1 = Request["vccount"];
                string vcmoney1 = Request["vcmoney"];
                string vcallmoney1 = Request["vcallmoney"];
                string vcdcomment1 = Request["vcdcomment"];

                string[] cdelarr = cdel1.Split(',');
                string[] bdprodnoarr = bdprodno1.Split(',');
                string[] bdprodtitlearr = bdprodtitle1.Split(',');
                // string[] mdnoarr = mdno1.Split(',');
                // string[] mdcommentarr = mdcomment1.Split(',');
                string[] pdunitarr = pdunit1.Split(',');
                string[] vccountarr = vccount1.Split(',');
                string[] vcmoneyarr = vcmoney1.Split(',');
                string[] vcallmoneyarr = vcallmoney1.Split(',');
                string[] vcdcommentarr = vcdcomment1.Split(',');

                vcid = Request["vcid"].ToString();
                int pitemno = 10;
                for (int i = 0; i < cdelarr.Length; i++)
                {
                    if (cdelarr[i].Trim() == "")
                    {
                        if (!(bdprodnoarr[i].Trim() == ""))
                        {
                            vend_contractdet addobj = new vend_contractdet();
                            addobj.vcid = decimal.Parse(vcid);
                            addobj.vcno = Request["vcno"].ToString();
                            addobj.comid = Request["comid"].ToString();

                            addobj.bdprodno = bdprodnoarr[i].Trim();
                            addobj.bdprodtitle = bdprodtitlearr[i].Trim();
                            // addobj.mdno = mdnoarr[i].Trim();
                            // addobj.mdcomment = mdcommentarr[i].Trim();

                            addobj.vitemno = pitemno;
                            addobj.pdunit = pdunitarr[i].Trim();
                            addobj.vccount = Decimal.Parse(vccountarr[i].ToString());
                            addobj.vcmoney = Decimal.Parse(vcmoneyarr[i].ToString());
                            addobj.vcallmoney = Decimal.Parse(vcallmoneyarr[i].ToString());

                            addobj.vcdcomment = vcdcommentarr[i].Trim();
                            addobj.projno = Request["projno"].ToString();

                            addobj.bmodid = Session["empid"].ToString();
                            addobj.bmoddate = DateTime.Now;

                            con.vend_contractdet.Add(addobj);
                            con.SaveChanges();
                            pitemno = pitemno + 10;
                        }
                    }
                    else
                    {
                        //修改
                        int pdid = int.Parse(cdelarr[i].Trim());
                        vend_contractdet modobj = con.vend_contractdet.Where(r => r.vcdid == pdid).FirstOrDefault();


                        modobj.bdprodno = bdprodnoarr[i].Trim();
                        modobj.bdprodtitle = bdprodtitlearr[i].Trim();
                        //  modobj.mdno = mdnoarr[i].Trim();
                        //  modobj.mdcomment = mdcommentarr[i].Trim();
                        modobj.pdunit = pdunitarr[i].Trim();
                        modobj.vccount = Decimal.Parse(vccountarr[i].ToString());
                        modobj.vcmoney = Decimal.Parse(vcmoneyarr[i].ToString());
                        modobj.vcallmoney = Decimal.Parse(vcallmoneyarr[i].ToString());
                        modobj.vcdcomment = vcdcommentarr[i].Trim();
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
            tmpform += "<form name='qfr1' action='/custcontract/detlist' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden name='vcid' id='vcid' value='" + vcid + "'>";
            tmpform += "</form>";
            tmpform += "</body>";



            return new ContentResult() { Content = @"" + tmpform };
        }


        public ActionResult detdel(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string cdel = Request["cdel"];
            string vcid = Request["vcid"];
            string vcdid = Request["vcdid"];

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
                        string money1 = dbobj.get_dbvalue(conn1, "select ('專案編號' + vcno + ',品項' + bdprodno + ',金額' + convert(char,vcallmoney)) as st1  from vend_contractdet where vcdid = '" + condtionArr[i].ToString() + "'");

                        sysnote += money1 + "<br>";
                        //刪除明細資料
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM vend_contractdet where vcdid = '" + condtionArr[i].ToString() + "'");

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
                    tmpform += "<form name='qfr1' action='/custcontract/detlist' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden id='vcid' name='vcid' value='" + vcid + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');</script>" + tmpform };


                }
            }
        }

        public ActionResult brlist()
        {
            int tmppid = 0;
            tmppid = int.Parse(Request["vcid"].ToString());
            ViewBag.vcid = tmppid.ToString();

            List<bonusrate> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                var query = con.bonusrate.AsQueryable();
                result = query.Where(r => r.vcid == tmppid).AsQueryable().ToList();

            }
            return View(result);

        }

        public ActionResult brlistdo(string sysflag, int? page, string orderdata, string orderdata1)
        {

            string vcid = "";
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {

                page = ((!page.HasValue || page < 1) ? 1 : page);
                ViewBag.page = page;
                ViewBag.orderdata = orderdata;
                ViewBag.orderdata1 = orderdata1;
                string sqlstr = "";
                string cdel1 = Request["bid"];

                string bseason1 = Request["bseason"];
                string btype1 = Request["btype"];
                //string mdno1 = Request["mdno"];
                //string mdcomment1 = Request["mdcomment"];
                string bsmoney1 = Request["bsmoney"];
                string bemoney1 = Request["bemoney"];
                string brate1 = Request["brate"];
                string bcomment1 = Request["bcomment"];

                string[] cdelarr = cdel1.Split(',');
                string[] bseasonarr = bseason1.Split(',');
                string[] btypearr = btype1.Split(',');
                // string[] mdnoarr = mdno1.Split(',');
                // string[] mdcommentarr = mdcomment1.Split(',');
                string[] bsmoneyarr = bsmoney1.Split(',');
                string[] bemoneyarr = bemoney1.Split(',');
                string[] bratearr = brate1.Split(',');
                string[] bcommentarr = bcomment1.Split(',');


                vcid = Request["vcid"].ToString();
                int pitemno = 10;
                for (int i = 0; i < cdelarr.Length; i++)
                {
                    if (cdelarr[i].Trim() == "")
                    {
                        if (!(bseasonarr[i].Trim() == ""))
                        {
                            bonusrate addobj = new bonusrate();
                            addobj.vcid = int.Parse(vcid);
                            //addobj.vcno = Request["vcno"].ToString();
                            addobj.comid = Request["comid"].ToString();

                            addobj.bseason = bseasonarr[i].Trim();
                            addobj.btype = btypearr[i].Trim();
                            // addobj.mdno = mdnoarr[i].Trim();
                            // addobj.mdcomment = mdcommentarr[i].Trim();

                            // addobj.vitemno = pitemno;
                            addobj.bsmoney = int.Parse(bsmoneyarr[i].ToString());
                            addobj.bemoney = int.Parse(bemoneyarr[i].ToString());
                            addobj.brate = Decimal.Parse(bratearr[i].ToString());
                            //addobj.vcallmoney = Decimal.Parse(vcallmoneyarr[i].ToString());

                            addobj.bcomment = bcommentarr[i].Trim();
                            // addobj.projno = Request["projno"].ToString();

                            addobj.bmodid = Session["empid"].ToString();
                            addobj.bmoddate = DateTime.Now;

                            con.bonusrate.Add(addobj);
                            con.SaveChanges();
                            pitemno = pitemno + 10;
                        }
                    }
                    else
                    {
                        //修改
                        int bid = int.Parse(cdelarr[i].Trim());
                        bonusrate modobj = con.bonusrate.Where(r => r.bid == bid).FirstOrDefault();


                        modobj.comid = Request["comid"].ToString();

                        modobj.bseason = bseasonarr[i].Trim();
                        modobj.btype = btypearr[i].Trim();
                        // addobj.mdno = mdnoarr[i].Trim();
                        // addobj.mdcomment = mdcommentarr[i].Trim();

                        // addobj.vitemno = pitemno;
                        modobj.bsmoney = int.Parse(bsmoneyarr[i].ToString());
                        modobj.bemoney = int.Parse(bemoneyarr[i].ToString());
                        modobj.brate = Decimal.Parse(bratearr[i].ToString());
                        //addobj.vcallmoney = Decimal.Parse(vcallmoneyarr[i].ToString());

                        modobj.bcomment = bcommentarr[i].Trim();
                        // addobj.projno = Request["projno"].ToString();

                        modobj.bmodid = Session["empid"].ToString();
                        modobj.bmoddate = DateTime.Now;
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
            tmpform += "<form name='qfr1' action='/custcontract/brlist' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden name='vcid' id='vcid' value='" + vcid + "'>";
            tmpform += "</form>";
            tmpform += "</body>";



            return new ContentResult() { Content = @"" + tmpform };
        }


        public ActionResult brdel(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string cdel = Request["cdel"];
            string vcid = Request["vcid"];
            string bid = Request["bid"];

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
                        //string money1 = dbobj.get_dbvalue(conn1, "select ('專案編號' + vcno + ',from vend_contractdet where vcdid = '" + condtionArr[i].ToString() + "'");

                        //sysnote += money1 + "<br>";
                        //刪除明細資料
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM bonusrate where bid = '" + condtionArr[i].ToString() + "'");

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
                    tmpform += "<form name='qfr1' action='/custcontract/brlist' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden id='vcid' name='vcid' value='" + vcid + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');</script>" + tmpform };


                }
            }
        }


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

       

    }
}
