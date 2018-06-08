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
    public class vend_monthmoneyController : BaseController
    {


        public ActionResult Index()
        {
       
            return View();
        }

        public ActionResult list(int? page, string orderdata, string orderdata1)
        {
            //ViewBag.mname = Environment.MachineName;
            IPagedList<vend_monthmoney> result;
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "slyear asc ,slmonth"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qbseason = "", qslyear = "", qslmonth = "", qvtype = "", qvendcomid = "";

            if (!string.IsNullOrWhiteSpace(Request["qbseason"]))
            {
                qbseason = Request["qbseason"].Trim();
                ViewBag.qbseason = qbseason;
            }
            if (!string.IsNullOrWhiteSpace(Request["qslyear"]))
            {
                qslyear = Request["qslyear"].Trim();
                ViewBag.qslyear = qslyear;
            }
            if (!string.IsNullOrWhiteSpace(Request["qslmonth"]))
            {
                qslmonth = Request["qslmonth"].Trim();
                ViewBag.qslmonth = qslmonth;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvtype"]))
            {
                qvtype = Request["qvtype"].Trim();
                ViewBag.qvtype = qvtype;
            }

            if (!string.IsNullOrWhiteSpace(Request["qvendcomid"]))
            {
                qvendcomid = Request["qvendcomid"].Trim();
                ViewBag.qvendcomid = qvendcomid;
            }

         
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "";
                sqlstr = "select * from vend_monthmoney where comid = '" + Session["comid"].ToString() + "' and vtype='2'  and";

                if (qvendcomid != "")
                    sqlstr += " vendcomid in (select comid from allcompany where comid like '%" + qvendcomid + "%' or comtitle like '%" + qvendcomid + "%')   and";
             
                if (qbseason != "")
                    sqlstr += " bseason = '" + qbseason + "'  and";
                if (qslyear != "")
                    sqlstr += " slyear = " + qslyear + "  and";
                if (qslmonth != "")
                    sqlstr += " slmonth = " + qslmonth + "  and";
                if (qvtype != "")
                    sqlstr += " vtype = '" + qvtype + "'  and";           
         

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.vend_monthmoney.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<vend_monthmoney>(page.Value - 1, (int)Session["pagesize"]);
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

            string qbseason = "", qslyear = "", qslmonth = "", qvtype = "", qvendcomid = "", qsdate = "", qedate = "";

            if (!string.IsNullOrWhiteSpace(Request["qbseason"]))
            {
                qbseason = Request["qbseason"].Trim();
                ViewBag.qbseason = qbseason;
            }
            if (!string.IsNullOrWhiteSpace(Request["qslyear"]))
            {
                qslyear = Request["qslyear"].Trim();
                ViewBag.qslyear = qslyear;
            }
            if (!string.IsNullOrWhiteSpace(Request["qslmonth"]))
            {
                qslmonth = Request["qslmonth"].Trim();
                ViewBag.qslmonth = qslmonth;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvtype"]))
            {
                qvtype = Request["qvtype"].Trim();
                ViewBag.qvtype = qvtype;
            }

            if (!string.IsNullOrWhiteSpace(Request["qvendcomid"]))
            {
                qvendcomid = Request["qvendcomid"].Trim();
                ViewBag.qvendcomid = qvendcomid;
            }


            if (!string.IsNullOrWhiteSpace(Request["qsdate"]))
            {
                qsdate = Request["qsdate"].Trim();
                ViewBag.qsdate = qsdate;
            }


            if (!string.IsNullOrWhiteSpace(Request["qedate"]))
            {
                qedate = Request["qedate"].Trim();
                ViewBag.qedate = qedate;
            }

            
            NDcommon dbobj = new NDcommon();
         
            SqlConnection erpconn = dbobj.get_conn("AitagBill_DBContext");
            SqlConnection erpconn1 = dbobj.get_conn("AitagBill_DBContext");
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string tmpssql = "";
                int strym = 0;
                int etrym = 0;
                   //找審過過單
                tmpssql = "select (payvendcomid) as vendno, isnull(sum(psummoney),0) as psummoney FROM vend_contractinvclose INNER JOIN vend_contractinvclose_det ON vend_contractinvclose.vcinvid =vend_contractinvclose_det.vcinvid where vstatus<>'D' ";

                switch (qbseason)
                {
                    case "01"://月獎                       
                        tmpssql += " and slyear =" + qslyear + " and slmonth = " + qslmonth + "  group by payvendcomid ";
                        break;
                    case "02"://季獎 3 / 6 / 9/ 12     
                        strym = 365 * int.Parse(qslyear) + 30 * int.Parse(qslmonth);
                        //etrym = qslyear.ToString() + qslmonth.ToString().PadLeft(2, '0');
                        if (qslmonth.ToString().PadLeft(2, '0') == "03")
                        {
                            strym = 365 * int.Parse(qslyear) + 30 * 1;
                            etrym = 365 * int.Parse(qslyear) + 30 * 3;
                        }
                        else if (qslmonth.ToString().PadLeft(2, '0') == "06")
                        {
                            strym = 365 * int.Parse(qslyear) + 30 * 4;
                            etrym = 365 * int.Parse(qslyear) + 30 * 6;
                        }
                        else if (qslmonth.ToString().PadLeft(2, '0') == "09")
                        {
                            strym = 365 * int.Parse(qslyear) + 30 * 7;
                            etrym = 365 * int.Parse(qslyear) + 30 * 9;
                        }
                        else if (qslmonth.ToString().PadLeft(2, '0') == "12")
                        {
                            strym = 365 * int.Parse(qslyear) + 30 * 10;
                            etrym = 365 * int.Parse(qslyear) + 30 * 12;
                        }
                        tmpssql += " and (slyear * 365 + slmonth * 30) >= " + strym.ToString() + " and (slyear * 365 + slmonth * 30)  <= " + etrym.ToString() + " group by payvendcomid ";
                        break;
                    case "03"://半年獎 06 / 12
                       strym = 365 * int.Parse(qslyear) + 30 * int.Parse(qslmonth);
                        //etrym = qslyear.ToString() + qslmonth.ToString().PadLeft(2, '0');
                        if (qslmonth.ToString().PadLeft(2, '0') == "06")
                        {
                            strym = 365 * int.Parse(qslyear) + 30 * 1;
                            etrym = 365 * int.Parse(qslyear) + 30 * 6;
                        }
                        else if (qslmonth.ToString().PadLeft(2, '0') == "12")
                        {
                            strym = 365 * int.Parse(qslyear) + 30 * 7;
                            etrym = 365 * int.Parse(qslyear) + 30 * 12;
                        }

                        tmpssql += " and (slyear * 365 + slmonth * 30) >= " + strym.ToString() + " and (slyear * 365 + slmonth * 30)  <= " + etrym.ToString() + " group by payvendcomid ";
                        break;
                    case "04"://年獎                        
                        tmpssql += " and year(vadate) =" + qslyear + " group by payvendcomid ";
                        break;
                }

                SqlDataReader dr = dbobj.dbselect(erpconn, tmpssql);
                string iftax = "";
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {

                        decimal tmprate = 0;
                        decimal tmppsummoney = decimal.Parse(dr["psummoney"].ToString());
                        decimal pallbonusmoney = tmppsummoney;
                        // 找 廠商/客戶的 計算方式 , vendno 在客戶計算是客戶的欄位
                        //tmpssql = "select * from allcompany_rate where bseason='" + qbseason + "' and allcomid='" + dr["vendno"].ToString() + "' and (bsmoney <=" + tmppsummoney + " and  bemoney >=" + tmppsummoney + " )";
                        //單筆金額
                        tmpssql = "select * from allcompany_rate where bseason='" + qbseason + "' and allcomid='" + dr["vendno"].ToString() + "' and btype = '0'";
                        SqlDataReader dr1 = dbobj.dbselect(erpconn1, tmpssql);
                        if (dr1.Read())
                        {
                            pallbonusmoney = decimal.Parse(dr1["brate"].ToString());
                        }

                        dr1.Close();
                        dr1.Dispose();

                        //單筆比例
                        tmpssql = "select * from allcompany_rate where bseason='" + qbseason + "' and allcomid='" + dr["vendno"].ToString() + "' and btype = '1'";
                        dr1 = dbobj.dbselect(erpconn1, tmpssql);
                        if (dr1.Read())
                        {
                            iftax = dr1["brate"].ToString();
                            tmprate = decimal.Parse(dr1["brate"].ToString());
                            if (iftax == "0")
                                pallbonusmoney = pallbonusmoney * tmprate / 100;
                            else
                                pallbonusmoney = (pallbonusmoney * tmprate * decimal.Parse("1.05")) / 100;
                        }

                        dr1.Close();
                        dr1.Dispose();

                        //落點金額比例
                        tmpssql = "select * from allcompany_rate where bseason='" + qbseason + "' and allcomid='" + dr["vendno"].ToString() + "' and btype = '2' and (bsmoney <=" + tmppsummoney + " and  bemoney >=" + tmppsummoney + ")";
                        dr1 = dbobj.dbselect(erpconn1, tmpssql);
                        if (dr1.Read())
                        {
                            iftax = dr1["brate"].ToString();
                            tmprate = decimal.Parse(dr1["brate"].ToString());
                            if (iftax == "0")
                                pallbonusmoney = pallbonusmoney * tmprate / 100;
                            else
                                pallbonusmoney = (pallbonusmoney * tmprate * decimal.Parse("1.05")) / 100;
                        }

                        dr1.Close();
                        dr1.Dispose();

                        //累積金額比例
                        tmpssql = "select * from allcompany_rate where bseason='" + qbseason + "' and allcomid='" + dr["vendno"].ToString() + "' and btype = '3' and (bsmoney <=" + tmppsummoney + " and  bemoney >=" + tmppsummoney + ")";
                        dr1 = dbobj.dbselect(erpconn1, tmpssql);
                        if (dr1.Read())
                        {
                            iftax = dr1["brate"].ToString();
                            tmprate = decimal.Parse(dr1["brate"].ToString());
                            if (iftax == "0")
                                pallbonusmoney = pallbonusmoney * tmprate / 100;
                            else
                                pallbonusmoney = (pallbonusmoney * tmprate * decimal.Parse("1.05")) / 100;
                        }

                        dr1.Close();
                        dr1.Dispose();

                         vend_monthmoney addobj = new vend_monthmoney();
                         addobj.bseason = qbseason;//01:月獎 02:季獎 03:半年獎 04:年獎  05:現折
                         addobj.slyear = int.Parse(qslyear);
                         if (qbseason == "04")
                         { addobj.slmonth = 12; }
                         else
                         { addobj.slmonth = int.Parse(qslmonth); }

                         addobj.vendcomid = dr["vendno"].ToString();
                         addobj.vtype = "2";//1:收入 2:支出
                           addobj.pallbonusmoney =pallbonusmoney;
                          addobj.pallmoney = tmppsummoney;

                         addobj.bmodid = Session["empid"].ToString();
                         addobj.bmoddate = DateTime.Now;
                         addobj.comid = Session["comid"].ToString();
                         con.vend_monthmoney.Add(addobj);


                     }
                 }

                
                con.SaveChanges();
                con.Dispose();

            }

            erpconn.Close();
            erpconn.Dispose();

            erpconn1.Close();
            erpconn1.Dispose();
            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/vend_monthmoney/list' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden id='qbseason' name='qbseason' value='" + qbseason + "'>";
            tmpform += "<input type=hidden id='qslyear' name='qslyear' value='" + qslyear + "'>";
            tmpform += "<input type=hidden id='qslmonth' name='qslmonth' value='" + qslmonth + "'>";         
            tmpform += "<input type=hidden id='qvendcomid' name='qvendcomid' value='" + qvendcomid + "'>";

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

            string qbseason = "", qslyear = "", qslmonth = "", qvtype = "", qvendcomid = "";

            if (!string.IsNullOrWhiteSpace(Request["qbseason"]))
            {
                qbseason = Request["qbseason"].Trim();
                ViewBag.qbseason = qbseason;
            }
            if (!string.IsNullOrWhiteSpace(Request["qslyear"]))
            {
                qslyear = Request["qslyear"].Trim();
                ViewBag.qslyear = qslyear;
            }
            if (!string.IsNullOrWhiteSpace(Request["qslmonth"]))
            {
                qslmonth = Request["qslmonth"].Trim();
                ViewBag.qslmonth = qslmonth;
            }
            if (!string.IsNullOrWhiteSpace(Request["qvtype"]))
            {
                qvtype = Request["qvtype"].Trim();
                ViewBag.qvtype = qvtype;
            }

            if (!string.IsNullOrWhiteSpace(Request["qvendcomid"]))
            {
                qvendcomid = Request["qvendcomid"].Trim();
                ViewBag.qvendcomid = qvendcomid;
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
                        string vcno = dbobj.get_dbvalue(conn1, "select ('年/月：' + convert(char,slyear)+ '/'+convert(char,slmonth)+ ',廠商：' + vendcomid) as st1 from vend_monthmoney where vsid='" + condtionArr[i].ToString() + "'");
                       
                        sysnote += vcno + "<br>";
                        //刪除憑單
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM vend_monthmoney where vsid = '" + condtionArr[i].ToString() + "'");                   

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
                    tmpform += "<form name='qfr1' action='/vend_monthmoney/list' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qbseason' name='qbseason' value='" + qbseason + "'>";
                    tmpform += "<input type=hidden id='qslyear' name='qslyear' value='" + qslyear + "'>";
                    tmpform += "<input type=hidden id='qslmonth' name='qslmonth' value='" + qslmonth + "'>";
                    tmpform += "<input type=hidden id='qvendcomid' name='qvendcomid' value='" + qvendcomid + "'>";
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

        

    }
}
