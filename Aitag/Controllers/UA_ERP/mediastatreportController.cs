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
    public class mediastatreportController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /cardlog/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }


        #region  客戶業績排行榜 mediastatreport_m1
        public ActionResult mediastatreport_m1()
        {
           
            return View();
        }

        public ActionResult mediastatreport_m1rpt()
        {
            string qslyear = "", qslmonth = "";
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
            int month1 = 1, month2 = 6, m1year = 0;
            int beformonth1 = 7, beformonth2 = 12, beform1year = 0;
            if (qslmonth == "1")
            {
                month1 = 1; month2 = 6; m1year = int.Parse(qslyear);
                beformonth1 = 7; beformonth2 = 12; beform1year = int.Parse(qslyear)-1;
                
            }
            else if (qslmonth == "7")
            {
                month1 = 7; month2 = 12; m1year = int.Parse(qslyear);
                beformonth1 = 1; beformonth2 = 6; beform1year = int.Parse(qslyear);
            }
           
            //組title
            string title = "104.01- 06";
            title = "{0}.{1}- {2}";
            title = string.Format(title, m1year - 1911, month1.ToString("00"), month2.ToString("00"));
            string sqlstr = "";
            string Excel1 = "";
            string Excel2 = "";/*繞第二層 table*/
            Excel1 += "<HTML>";
            Excel1 += "<HEAD>";
            Excel1 += @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">";
            Excel1 += "</HEAD>";
            Excel1 += "<body>";

            Excel1 += "<center><b style='font-size:17pt'>2008傳媒行銷股份有限公司<BR>";
            Excel1 += "2008 Media Marketing Company Limited</b><p><p>";
            Excel1 += " <b style='font-size:17pt'>" + title + "前十大客戶同期比較營收</b></center>";

            Excel1 += @"<table border=0 borderColorDark=#ffffff borderColorLight=#000000 cellpadding=5 cellspacing=0 width=100% style='font-size:12pt'>";
            
            //Excel1 += @"<tr align=center style='font-weight: bold;'>";
            //Excel1 += @"<td colspan=2>" + title + "前十大客戶同期比較營收</td>";
            //Excel1 += @"</tr>";
            

            Excel1 += @"<tr>";
            Excel1 += @"<td colspan=2>";


            Excel2 = m1rptExcel2(month1, month2, m1year, beformonth1, beformonth2, beform1year);

            Excel1 += Excel2;
            Excel1 += @"</td>";
            Excel1 += @"<tr>";
            Excel1 += @"</table>";

            Excel1 += "</body>";
            Excel1 += "</HTML>";


            ViewBag.Excel = Excel1;
            return View();
        }


        private string m1rptExcel2(int month1, int month2, int m1year, int beformonth1, int beformonth2, int beform1year)
        {
            NDcommon dbobj = new NDcommon();

            string sql2 = " (SELECT vsid FROM view_vend_monthmoney WHERE (vtype = '{0}') AND (slyear = '{1}') AND (slmonth >= '{2}') AND (slmonth <= '{3}') and (custcomid = cus.custcomid))";
            string sql = "select custcomid, ";
            sql += " (select sum(pallmoney) from view_vend_monthmoney where vsid in ";/*本期收入*/
            sql += string.Format(sql2, "1", m1year, month1, month2);
            sql += " group by custcomid ) as money1,";

            sql += " (select sum(pallmoney) from view_vend_monthmoney where vsid in ";/*前期收入*/
            sql += string.Format(sql2, "1", beform1year, beformonth1, beformonth2);
            sql += " group by custcomid ) as money2,";

            sql += " (select sum(pallmoney) from view_vend_monthmoney where vsid in ";/*本期支出*/
            sql += string.Format(sql2, "2", m1year, month1, month2);
            sql += " group by custcomid ) as money3,";

            sql += " (select sum(pallmoney) from view_vend_monthmoney where vsid in";/*前期支出*/
            sql += string.Format(sql2, "2", beform1year, beformonth1, beformonth2);
            sql += " group by custcomid) as money4";

            string sql3 = " from view_vend_monthmoney as cus where vsid in ( SELECT vsid FROM view_vend_monthmoney WHERE (slyear = '{0}') AND (slmonth >= '{1}') AND (slmonth <= '{2}')) group by custcomid";
            sql += string.Format(sql3, m1year, month1, month2);


            List<m1rptCol> m1rptColList = new List<m1rptCol>();

            using (SqlConnection conn = dbobj.get_conn("AitagBill_DBContext"))
            {
                using (SqlConnection conn1 = dbobj.get_conn("AitagBill_DBContext"))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        SqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            /*營業收入*/
                            //本期
                            double money1 = double.Parse("0" + dbobj.get_dbnull2(dr["money1"]));
                            //前期
                            double money2 = double.Parse("0" + dbobj.get_dbnull2(dr["money2"]));
                            //差異金額
                            double money1_2 = money1 - money2;/*本期 減去 前期*/
                            //%
                            double moneypct = funDivision(money1, money2);

                            /*營業毛利*/
                            //本期
                            double money3 = money1 - double.Parse("0" + dbobj.get_dbnull2(dr["money3"]));
                            //前期
                            double money4 = money2 - double.Parse("0" + dbobj.get_dbnull2(dr["money4"]));
                            //差異金額
                            double money3_4 = money3 - money4;/*本期 減去 前期*/
                            //%
                            double moneypct2 = funDivision(money3, money4);
                            
                            m1rptColList.Add(new m1rptCol()
                            {
                                custcomid = dbobj.get_dbvalue(conn1, "select comtitle from allCompany where comsno = '" + dr["custcomid"].ToString() + "'"),
                                numA = money1,
                                numB = money2,
                                numC = money1_2,
                                numD = moneypct,
                                numE = money3,
                                numF = money4,
                                numG = money3_4,
                                numH = moneypct2
                            });

                        }
                        dr.Close();
                    }
                }
            }


            IEnumerable<m1rptCol> query = null;
            query = from items in m1rptColList orderby items.numE descending select items;
            //query = from items in m1rptColList orderby items.numD, items.numA descending select items;

           

            string Excel2 = "";
            Excel2 += "<table border=1 borderColorDark=#ffffff borderColorLight=#000000 cellpadding=5 cellspacing=0 width=100% style='font-size:10pt'>";
            Excel2 += "<tr>";
            Excel2 += "<th rowspan=2>排行</th>";
            Excel2 += "<th rowspan=2>客戶別</th>";
            Excel2 += "<th colspan=4>營業收入</th>";
            Excel2 += "<th colspan=4>營業毛利</th>";
            Excel2 += "</tr><tr><th>本期</th><th>前期(去年)</th><th>差異金額</th><th>%</th><th>本期</th><th>前期</th><th>差異金額</th><th>%</th></tr>";

            List<m1rptCol> m1rptColSUM = new List<m1rptCol>();/*準備塞合計*/
            m1rptColSUM.Add(new m1rptCol() { });

            int Ranking = 0;/*前十大  超過會跳出*/
            foreach (m1rptCol v in query)
            {
                /*塞合計*/
                m1rptColSUM[0].numA += v.numA;
                m1rptColSUM[0].numB += v.numB;
                m1rptColSUM[0].numC += v.numC;

                m1rptColSUM[0].numE += v.numE;
                m1rptColSUM[0].numF += v.numF;
                m1rptColSUM[0].numG += v.numG;
                

                Ranking++;
                //營業收入
                Excel2 += "<tr align=center>";
                Excel2 += "<th>" + Ranking + "</th>";
                Excel2 += "<td>" + v.custcomid + "</td>";
                Excel2 += "<td>" + v.numA + "</td>";
                Excel2 += "<td>" + v.numB + "</td>";
                Excel2 += "<td>" + v.numC + "</td>";
                Excel2 += "<td>" + v.numD.ToString("##0") + "%</td>";
                //營業毛利
                Excel2 += "<td>" + v.numE + "</td>";
                Excel2 += "<td>" + v.numF + "</td>";
                Excel2 += "<td>" + v.numG + "</td>";
                Excel2 += "<td>" + v.numH.ToString("##0") + "%</td>";
                Excel2 += "</tr>";
                if (Ranking >= 3)
                {
                    break;
                }
            }

            

            m1rptColSUM[0].numD = funDivision(m1rptColSUM[0].numA, m1rptColSUM[0].numB);
            m1rptColSUM[0].numH = funDivision(m1rptColSUM[0].numE, m1rptColSUM[0].numF);
            //合計
            Excel2 += "<tr align=center>";
            Excel2 += "<th>q</th>";
            Excel2 += "<td>差異金額合計</td>";
            Excel2 += "<td>" + m1rptColSUM[0].numA + "</td>";
            Excel2 += "<td>" + m1rptColSUM[0].numB + "</td>";
            Excel2 += "<td>" + m1rptColSUM[0].numC + "</td>";
            Excel2 += "<td>" + m1rptColSUM[0].numD.ToString("##0") + "%</td>";
            //營業毛利
            Excel2 += "<td>" + m1rptColSUM[0].numE + "</td>";
            Excel2 += "<td>" + m1rptColSUM[0].numF + "</td>";
            Excel2 += "<td>" + m1rptColSUM[0].numG + "</td>";
            Excel2 += "<td>" + m1rptColSUM[0].numH.ToString("##0") + "%</td>";
            Excel2 += "</tr>";
            Excel2 += "</table>";


            return Excel2;
        }

        private double funDivision(double money1, double money2)
        {
            double moneypct = 0;
            double money1_2 = money1 - money2;

            if (money1 > money2)
            {
                moneypct = (money1_2 / money1) * 100;
            }
            else if (money2 > money1)
            {
                if (money2 == 0)
                {
                    moneypct = -100;
                }
                else
                {
                    moneypct = (money1_2 / money2) * 100;
                }
            }
            else
            {
                moneypct = 0;
            }
            return moneypct;
        }


        #endregion

        #region  營業收入發稿統計表 mediastatreport_m2
        public ActionResult mediastatreport_m2()
        {


            
            return View();
        }

        public ActionResult mediastatreport_m2rpt()
        {
            string qslyear = "", qslmonth = "";
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
            
            string sqlstr = "";
            string Excel1 = "";
            string Excel2 = "";/*繞第二層 table*/
            Excel1 += "<HTML>";
            Excel1 += "<HEAD>";
            Excel1 += @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">";
            Excel1 += "</HEAD>";
            Excel1 += "<body>";

            Excel1 += "<center><b style='font-size:17pt'>2008傳媒行銷股份有限公司<BR>";
            Excel1 += "2008 Media Marketing Company Limited</b><p><p>";
            Excel1 += " <b style='font-size:17pt'>營業收入統計表</b></center>";

            Excel1 += @"<table border=0 borderColorDark=#ffffff borderColorLight=#000000 cellpadding=5 cellspacing=0 width=100% style='font-size:12pt'>";
            //Excel1 += @"<tr align=center style='font-weight: bold;'>";
            //Excel1 += @"<td colspan=2>營業收入統計表</td>";
            //Excel1 += @"</tr>";
            Excel1 += @"<tr align=center>";
            int year = int.Parse(qslyear) - 1911;
            Excel1 += @"<td colspan=2>" + year + " 年 " + qslmonth + " 月</td>";
            Excel1 += @"</tr>";
            Excel1 += @"<tr>";
            Excel1 += @"<td>M-104.9-09</td>";
            Excel1 += @"<td align=right>單位:元</td>";
            Excel1 += @"</tr>";
            Excel1 += @"<tr>";
            Excel1 += @"<td colspan=2>";

            Excel2 = m2rptExcel2(qslyear, qslmonth);

            Excel1 += Excel2;
            Excel1 += @"</td>";
            Excel1 += @"<tr>";
            Excel1 += @"</table>";

            Excel1 += "</body>";
            Excel1 += "</HTML>";
            ViewBag.Excel = Excel1;
            return View();
        }

        private string m2rptExcel2(string qslyear, string qslmonth)
        {
            NDcommon dbobj = new NDcommon();
            string sql = "";
            List<string> msctitleList = new List<string>();
            using (SqlConnection conn = dbobj.get_conn("AitagBill_DBContext"))
            {
                sql = "select msctitle from mediaclass group by msctitle";/*媒體類別*/
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        msctitleList.Add(dbobj.get_dbnull2(dr["msctitle"]));
                    }
                    dr.Close();
                }
            }
            List<m2rptCol> m2rptColList = new List<m2rptCol>();
            using (SqlConnection conn = dbobj.get_conn("AitagBill_DBContext"))
            {
                sql = "select ";
                string sql2 = " (select SUM(pallmoney) from vend_monthmoney where vtype = '1' and slyear = '{0}' and slmonth = '{1}' and custcomid = v_mt.custcomid and mcno in (select mcno from mediaclass where msctitle = '{2}')) as {2},";
                foreach (string v in msctitleList)
                {
                    sql += string.Format(sql2, qslyear, qslmonth, v);/*繞 媒體類別 個別本期金額*/
                }
                sql2 = " (select SUM(paccmoney) from vend_monthmoney where vtype = '1' and slyear = '{0}' and slmonth = '{1}' and custcomid = v_mt.custcomid and mcno in (select mcno from mediaclass where msctitle = '{2}')) as {2}2,";
                foreach (string v in msctitleList)
                {
                    sql += string.Format(sql2, qslyear, qslmonth, v);/*繞 媒體類別 個別累計金額*/
                }
                sql += " custcomid";

                string sql1 = " from vend_monthmoney as v_mt where vtype = '1' and slyear = '{0}' and slmonth = '{1}'";
                sql += string.Format(sql1, qslyear, qslmonth);
                sql += " group by custcomid";


                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlConnection conn1 = dbobj.get_conn("AitagBill_DBContext"))
                    {
                        SqlDataReader dr = cmd.ExecuteReader();
                        int i;
                        double sumPallmoney = 0, Pallmoney;
                        double sumPaccmoney = 0, Paccmoney;
                        while (dr.Read())
                        {
                            m2rptColList.Add(new m2rptCol() { });
                            i = m2rptColList.Count - 1;
                            sumPallmoney = 0;
                            sumPaccmoney = 0;

                            m2rptColList[i].custcomid = dbobj.get_dbvalue(conn1, "select comtitle from allCompany where comsno = '" + dr["custcomid"].ToString() + "'");
                            foreach (string v in msctitleList)
                            {
                                Pallmoney = double.Parse("0" + dbobj.get_dbnull2(dr[v]));
                                m2rptColList[i].pallmoney.Add(Pallmoney);
                                sumPallmoney += Pallmoney;
                            }
                            m2rptColList[i].sumPallmoney = sumPallmoney;
                            foreach (string v in msctitleList)
                            {
                                string vv = v + "2";
                                Paccmoney = double.Parse("0" + dbobj.get_dbnull2(dr[vv]));
                                m2rptColList[i].paccmoney.Add(Paccmoney);
                                sumPaccmoney += Paccmoney;
                            }
                            m2rptColList[i].sumPaccmoney = sumPaccmoney;

                        }
                        dr.Close();
                    }
                }
            }


            int colspan = msctitleList.Count;

            string Excel2 = "";
            Excel2 += @"<table border=1 borderColorDark=#ffffff borderColorLight=#000000 cellpadding=5 cellspacing=0 width=100% style='font-size:10pt'>";
            Excel2 += @"<tr>";
            Excel2 += @"<th rowspan=3 width=15%>客戶</th>";
            colspan++;
            Excel2 += @"<th colspan=" + colspan + ">當月</th>";
            Excel2 += @"<th colspan=" + colspan + ">累計</th>";
            Excel2 += @"</tr>";
            Excel2 += @"<tr align=center>";
            colspan--;
            Excel2 += @"<th colspan=" + colspan + ">廣告費收入</th>";
            Excel2 += @"<th>營業收入</th>";
            Excel2 += @"<th colspan=" + colspan + ">廣告費收入</th>";
            Excel2 += @"<th>營業收入</th>";
            Excel2 += @"</tr>";


            Excel2 += @"<tr align=center>";
            foreach (string v in msctitleList)
            {
                Excel2 += @"<th>" + v + "</th>";
            }
            Excel2 += @"<td>合計</td>";
            foreach (string v in msctitleList)
            {
                Excel2 += @"<th>" + v + "</th>";
            }
            Excel2 += @"<td>總計</td>";
            Excel2 += @"</tr>";


            foreach (m2rptCol v in m2rptColList)
            {
                Excel2 += @"<tr align=center>";
                Excel2 += @"<th>" + v.custcomid + "</th>";
                foreach (double num in v.pallmoney)
                {
                    Excel2 += @"<td>" + num + "</td>";
                }
                Excel2 += @"<td>"+v.sumPallmoney+"</td>";

                foreach (double num in v.paccmoney)
                {
                    Excel2 += @"<td>" + num + "</td>";
                }
                Excel2 += @"<td>" + v.sumPaccmoney + "</td>";
                Excel2 += @"</tr>";
            }
            
            Excel2 += @"<tr align=center>";
            Excel2 += @"<th>合計</th>";
            
            double totalsumPallmoney = 0;
            double totalsumPaccmoney = 0;
            foreach (m2rptCol v in m2rptColList)
            {
                totalsumPallmoney += v.sumPallmoney;
                totalsumPaccmoney += v.sumPaccmoney;
            }
            double totalpallmoney = 0;
            for (int i = 0; i < msctitleList.Count; i++)
            {
                totalpallmoney = 0;
                foreach (m2rptCol v in m2rptColList)
                {
                    totalpallmoney += v.pallmoney[i];
                }
                Excel2 += @"<td>" + totalpallmoney + "</td>";
            }
            Excel2 += @"<td>" + totalsumPallmoney + "</td>";
            double totalpaccmoney = 0;
            for (int i = 0; i < msctitleList.Count; i++)
            {
                totalpaccmoney = 0;
                foreach (m2rptCol v in m2rptColList)
                {
                    totalpaccmoney += v.paccmoney[i];
                }
                Excel2 += @"<td>" + totalpaccmoney + "</td>";
            }
            Excel2 += @"<td>" + totalsumPaccmoney + "</td>";

            Excel2 += @"</tr>";
            Excel2 += @"</table>";

            return Excel2;
        }

        #endregion

        #region  各類營業收入統計表 mediastatreport_m3
        public ActionResult mediastatreport_m3()
        {
            
            return View();
        }

        public ActionResult mediastatreport_m3rpt()
        {
            string qslyear = "", qslmonth = "", msctitle = "";
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
            if (!string.IsNullOrWhiteSpace(Request["msctitle"]))
            {
                msctitle = Request["msctitle"].Trim();
                ViewBag.msctitle = msctitle;
            }
            //NDcommon dbobj = new NDcommon();
            //using (SqlConnection conn = dbobj.get_conn("AitagBill_DBContext"))
            //{

            //}
            string sqlstr = "";
            
            string Excel1 = "";
            string Excel2 = "";/*繞第二層 table*/

            Excel1 += "<HTML>";
            Excel1 += "<HEAD>";
            Excel1 += @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">";
            Excel1 += "</HEAD>";
            Excel1 += "<body>";

            Excel1 += "<center><b style='font-size:17pt'>2008傳媒行銷股份有限公司<BR>";
            Excel1 += "2008 Media Marketing Company Limited</b><p><p>";
            Excel1 += " <b style='font-size:17pt'>" + msctitle + "統計表</b></center>";

            Excel1 += @"<table border=0 borderColorDark=#ffffff borderColorLight=#000000 cellpadding=5 cellspacing=0 width=100% style='font-size:12pt'>";
            //Excel1 += @"<tr align=center style='font-weight: bold;'>";
            //Excel1 += @"<td colspan=2>" + msctitle + "統計表</td>";
            //Excel1 += @"</tr>";
            Excel1 += @"<tr align=center>";
            int year = int.Parse(qslyear) - 1911;
            Excel1 += @"<td colspan=2>" + year + " 年 " + qslmonth + " 月</td>";
            Excel1 += @"</tr>";
            Excel1 += @"<tr>";
            Excel1 += @"<td>M-104.9-09</td>";
            Excel1 += @"<td align=right>單位:元</td>";
            Excel1 += @"</tr>";
            Excel1 += @"<tr>";
            Excel1 += @"<td colspan=2>";


            Excel2 = m3rptExcel2(qslyear, qslmonth, msctitle);

            Excel1 += Excel2;
            Excel1 += @"</td>";
            Excel1 += @"<tr>";
            Excel1 += @"</table>";

            Excel1 += "</body>";
            Excel1 += "</HTML>";


            ViewBag.Excel = Excel1;
            return View();
        }

        private string m3rptExcel2(string qslyear, string qslmonth, string msctitle)
        {
            List<mcnoCol> mcnoColList = new List<mcnoCol>();
            List<Excel4Raw> Excel4RawList = new List<Excel4Raw>();
            List<double> mcnoColList3 = new List<double>();/*準備放 最後合計*/
            List<string> mcnoColList4 = new List<string>();
            NDcommon dbobj = new NDcommon();

            string Excel3 = "";
            string Excel4 = "";
            string sql = "";
            using (SqlConnection conn = dbobj.get_conn("AitagBill_DBContext"))
            {
                sql = "select * from mediachannel where mcno in (select mcno from mediaclass where msctitle = '" + msctitle + "')";/*組欄位*/
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        if (dbobj.get_dbnull2(dr["mdtitle"]) != "")
                        {
                            mcnoColList4.Add(dbobj.get_dbnull2(dr["mdtitle"]));
                        }
                        
                       
                        mcnoColList.Add(new mcnoCol() { mdno = dbobj.get_dbnull2(dr["mdno"]), money = "0", pallmoney = "0" });
                        mcnoColList3.Add(0);
                    }
                    mcnoColList3.Add(0);/*準備放總計*/
                    mcnoColList3.AddRange(mcnoColList3);
                    dr.Close();
                }
            }
            using (SqlConnection conn = dbobj.get_conn("AitagBill_DBContext"))
            {
                sql = "select DISTINCT custcomid from view_vend_monthmoney where vsid in ( SELECT vsid FROM view_vend_monthmoney WHERE (vtype = '1') AND (slyear = '" + qslyear + "') AND (slmonth = '" + qslmonth + "') AND (mcno in (select mcno from mediaclass where msctitle = '" + msctitle + "')) )";/*客戶*/
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    string custcomid = "", comtitle = "";
                    List<mcnoCol> mcnoColList2 = new List<mcnoCol>();
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        custcomid = dbobj.get_dbnull2(dr["custcomid"]);
                        mcnoColList2.Clear();
                        mcnoColList.ForEach(i => mcnoColList2.Add(i));
                        Excel4RawList.Add(new Excel4Raw() { });
                        int dataRaw = Excel4RawList.Count - 1;

                        using (SqlConnection conn1 = dbobj.get_conn("AitagBill_DBContext"))
                        {
                            comtitle = dbobj.get_dbvalue(conn1, "select comtitle from allCompany where comsno = '" + custcomid + "'");
                            Excel4RawList[dataRaw].Excel4Col.Add(comtitle);

                            sql = "select mdno, sum(pallmoney) as pallmoney, sum(paccmoney) as paccmoney from view_vend_monthmoney where"+
                                " vsid in ( SELECT vsid FROM view_vend_monthmoney WHERE (vtype = '1') AND (slyear = '" + qslyear + "') AND (slmonth = '" + qslmonth + "') AND (mcno in (select mcno from mediaclass where msctitle = '" + msctitle + "')) AND custcomid = '" + custcomid + "')" +
                                " group by mdno";
                            using (SqlCommand cmd1 = new SqlCommand(sql, conn1))
                            {
                                SqlDataReader dr1 = cmd1.ExecuteReader();
                                while (dr1.Read())
                                {
                                    foreach (mcnoCol v in mcnoColList2)
                                    {
                                        if (v.mdno == dbobj.get_dbnull2(dr1["mdno"]))
                                        {
                                            v.money = dbobj.get_dbnull2(dr1["pallmoney"]);
                                            v.pallmoney = dbobj.get_dbnull2(dr1["paccmoney"]);
                                        }
                                    }
                                }
                                dr1.Close();
                            }
                        }


                        
                        //收入
                        double sum_money = 0;
                        int j = 0;
                        
                        foreach (mcnoCol v in mcnoColList2)
                        {
                            mcnoColList3[j] += double.Parse(v.money); j++;
                            Excel4RawList[dataRaw].Excel4Col.Add(v.money);
                            sum_money += double.Parse(v.money);
                        }
                        mcnoColList3[j] += double.Parse(sum_money.ToString()); j++;
                        Excel4RawList[dataRaw].Excel4Col.Add(sum_money.ToString());

                        //累計收入
                        double sum_pallmoney = 0;
                        foreach (mcnoCol v in mcnoColList2)
                        {
                            mcnoColList3[j] += double.Parse(v.pallmoney); j++;
                            Excel4RawList[dataRaw].Excel4Col.Add(v.pallmoney);
                            sum_pallmoney += double.Parse(v.pallmoney);
                        }
                        mcnoColList3[j] += sum_pallmoney; j++;
                        Excel4RawList[dataRaw].Excel4Col.Add(sum_pallmoney.ToString());

                    }
                    dr.Close();
                }
            }
            #region 組欄位
            int colnum1 = 1;/*欄位數量*/
            int colnum2 = 1;/*欄位數量*/
            int jj = 0;
            foreach (string v in mcnoColList4)
            {
                if (mcnoColList3[jj] != 0)
                {
                    Excel3 += "<th>" + v + "</th>";
                    colnum1++;
                }
                jj++;
            }
            Excel3 += @"<th>總計</th>";
            jj++;
            foreach (string v in mcnoColList4)
            {
                if (mcnoColList3[jj] != 0)
                {
                    Excel3 += "<th>" + v + "</th>";
                    colnum2++;
                }
                jj++;
            }
            Excel3 += @"<th>總計</th>";


            #endregion

            #region 組資料

            foreach (Excel4Raw v in Excel4RawList)
            {
                jj = 0;

                Excel4 += @"<tr align=center>";
                Excel4 += @"<th>" + v.Excel4Col[0] + "</th>";

                for (int i = 1; i < v.Excel4Col.Count; i++)
                {
                    if (mcnoColList3[jj] != 0)
                    {
                        Excel4 += @"<td>" + v.Excel4Col[i] + "</td>";
                    }
                    jj++;
                }

                Excel4 += @"</tr>";
            }
            




            #endregion

            string Excel2 = "";/*繞第二層 table*/
            Excel2 += @"<table border=1 borderColorDark=#ffffff borderColorLight=#000000 cellpadding=5 cellspacing=0 width=100% style='font-size:10pt'>";
            Excel2 += @"<tr>";
            Excel2 += @"<th rowspan=2 width=15%>客戶</th>";
            Excel2 += @"<th colspan='" + colnum1 + "'>當月</th>";
            Excel2 += @"<th colspan='" + colnum2 + "'>累計</th>";
            Excel2 += @"</tr>";
            Excel2 += @"<tr align=center>";

            Excel2 += Excel3;
           
            Excel2 += @"</tr>";

            //----
            Excel2 += Excel4;

            //最下面 合計
            Excel2 += @"<tr align=center>";
            Excel2 += @"<th>合計</th>";
            foreach (double v in mcnoColList3)
            {
                if (v != 0)
                {
                    Excel2 += @"<td>" + v + "</td>";
                }
            }
            Excel2 += @"</tr>";
            Excel2 += @"</table>";


            return Excel2;
        }

        #endregion

        #region  年度損益管報 mediastatreport_m4
        public ActionResult mediastatreport_m4()
        {

            return View();
        }
        public ActionResult mediastatreport_m4rpt()
        {
            string qslyear = "", qslmonth = "";
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

            string sqlstr = "";
            string Excel1 = "";
            string Excel2 = "";/*繞第二層 table*/
            Excel1 += "<HTML>";
            Excel1 += "<HEAD>";
            Excel1 += @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">";
            Excel1 += @"<style type=""text/css"">";
            Excel1 += @".xl66{text-align:center;border:.5pt solid black;font-weight:700;}";
            Excel1 += @".xl67{text-align:left;border:.5pt solid black;font-weight:700;}";
            Excel1 += @"</style>";
            Excel1 += "</HEAD>";
            Excel1 += "<body>";

            Excel1 += "<center><b style='font-size:17pt'>2008傳媒行銷股份有限公司<BR>";
            Excel1 += "2008 Media Marketing Company Limited</b><p><p>";
            Excel1 += " <b style='font-size:17pt'>年度損益管報</b></center>";

            Excel2 = m4rptExcel2(qslyear, qslmonth);
            Excel1 += Excel2;

            Excel1 += "</body>";
            Excel1 += "</HTML>";
            ViewBag.Excel = Excel1;
            return View();
        }
        private string m4rptExcel2(string qslyear, string qslmonth)
        {
            NDcommon dbobj = new NDcommon();
            string sql = "";
            List<double> income = new List<double>();
            List<double> cost = new List<double>();
            List<double> gross = new List<double>();
            using (SqlConnection conn = dbobj.get_conn("AitagBill_DBContext"))
            {
                string sql2 = " (select SUM(pallmoney) from vend_monthmoney as v_mt where vtype = '{0}' and slyear = '{1}' and slmonth = '{2}') as num{0}_{2},";
                sql = "select top(1) ";
                for (int i = 1; i <= 12; i++)
                {
                    sql += string.Format(sql2, "1", qslyear, i.ToString());
                    sql += string.Format(sql2, "2", qslyear, i.ToString());
                }
                sql += " '' as nn";
                sql += " from vend_monthmoney as v_mt";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        double num1 = 0, num2 = 0;
                        for (int i = 1; i <= 12; i++)
                        {
                            num1 = double.Parse("0" + dbobj.get_dbnull2(dr["num1_" + i]));
                            num2 = double.Parse("0" + dbobj.get_dbnull2(dr["num2_" + i]));
                            income.Add(num1);
                            cost.Add(num2);
                            gross.Add(num1 - num2);
                        }
                    }
                    dr.Close();
                }
            }





            #region 整理要輸出的資料
            string[] glClassarr = { "營業收入", "媒體-TV", "媒體-NONTV", "製作收入-AC", "現金折扣收入", "獎金", "減:營業退回/折讓", "收入小計", "營業成本", "媒體-TV", "媒體-NONTV", "媒體-獎金成本", "營業成本-其他", "營業成本-市調", "營業成本-AC", "減:進貨退回/折讓", "成本小計", "營業毛利" };
            List<m4rptCol> m4rptColList = new List<m4rptCol>();
            foreach (string s in glClassarr)
            {
                m4rptColList.Add(new m4rptCol() { glClass = s });
            }
            //營業收入
            m4rptColList[0].money.Add(new m4rptCol2() { numA = sumDouble1_12(income), numB = 222, numC = 333 });
            for (int i = 1; i <= 12; i++)
            {
                m4rptColList[0].money.Add(new m4rptCol2() { numA = income[i - 1], numB = 0, numC = 0 });
            }

            //媒體-TV
            m4rptColList[1].money.Add(new m4rptCol2() { numA = 111, numB = 222, numC = 333 });
            for (int i = 1; i <= 12; i++)
            {
                m4rptColList[1].money.Add(new m4rptCol2() { numA = 1, numB = 2, numC = 3 });
            }

            //媒體-NONTV
            m4rptColList[2].money.Add(new m4rptCol2() { numA = 111, numB = 222, numC = 333 });
            for (int i = 1; i <= 12; i++)
            {
                m4rptColList[2].money.Add(new m4rptCol2() { numA = 1, numB = 2, numC = 3 });
            }

            //製作收入-AC
            m4rptColList[3].money.Add(new m4rptCol2() { numA = 111, numB = 222, numC = 333 });
            for (int i = 1; i <= 12; i++)
            {
                m4rptColList[3].money.Add(new m4rptCol2() { numA = 1, numB = 2, numC = 3 });
            }

            //現金折扣收入
            m4rptColList[4].money.Add(new m4rptCol2() { numA = 111, numB = 222, numC = 333 });
            for (int i = 1; i <= 12; i++)
            {
                m4rptColList[4].money.Add(new m4rptCol2() { numA = 1, numB = 2, numC = 3 });
            }

            //獎金
            m4rptColList[5].money.Add(new m4rptCol2() { numA = 111, numB = 222, numC = 333 });
            for (int i = 1; i <= 12; i++)
            {
                m4rptColList[5].money.Add(new m4rptCol2() { numA = 1, numB = 2, numC = 3 });
            }

            //減:營業退回/折讓
            m4rptColList[6].money.Add(new m4rptCol2() { numA = 111, numB = 222, numC = 333 });
            for (int i = 1; i <= 12; i++)
            {
                m4rptColList[6].money.Add(new m4rptCol2() { numA = 1, numB = 2, numC = 3 });
            }

            //收入小計
            m4rptColList[7].money.Add(new m4rptCol2() { numA = 111, numB = 222, numC = 333 });
            for (int i = 1; i <= 12; i++)
            {
                m4rptColList[7].money.Add(new m4rptCol2() { numA = 1, numB = 2, numC = 3 });
            }

            //營業成本
            m4rptColList[8].money.Add(new m4rptCol2() { numA = sumDouble1_12(cost), numB = 222, numC = 333 });
            for (int i = 1; i <= 12; i++)
            {
                m4rptColList[8].money.Add(new m4rptCol2() { numA = cost[i - 1], numB = 0, numC = 0 });
            }

            //媒體-TV
            m4rptColList[9].money.Add(new m4rptCol2() { numA = 111, numB = 222, numC = 333 });
            for (int i = 1; i <= 12; i++)
            {
                m4rptColList[9].money.Add(new m4rptCol2() { numA = 1, numB = 2, numC = 3 });
            }

            //媒體-NONTV
            m4rptColList[10].money.Add(new m4rptCol2() { numA = 111, numB = 222, numC = 333 });
            for (int i = 1; i <= 12; i++)
            {
                m4rptColList[10].money.Add(new m4rptCol2() { numA = 1, numB = 2, numC = 3 });
            }

            //媒體-獎金成本
            m4rptColList[11].money.Add(new m4rptCol2() { numA = 111, numB = 222, numC = 333 });
            for (int i = 1; i <= 12; i++)
            {
                m4rptColList[11].money.Add(new m4rptCol2() { numA = 1, numB = 2, numC = 3 });
            }

            //營業成本-其他
            m4rptColList[12].money.Add(new m4rptCol2() { numA = 111, numB = 222, numC = 333 });
            for (int i = 1; i <= 12; i++)
            {
                m4rptColList[12].money.Add(new m4rptCol2() { numA = 1, numB = 2, numC = 3 });
            }

            //營業成本-市調
            m4rptColList[13].money.Add(new m4rptCol2() { numA = 111, numB = 222, numC = 333 });
            for (int i = 1; i <= 12; i++)
            {
                m4rptColList[13].money.Add(new m4rptCol2() { numA = 1, numB = 2, numC = 3 });
            }

            //營業成本-AC
            m4rptColList[14].money.Add(new m4rptCol2() { numA = 111, numB = 222, numC = 333 });
            for (int i = 1; i <= 12; i++)
            {
                m4rptColList[14].money.Add(new m4rptCol2() { numA = 1, numB = 2, numC = 3 });
            }

            //減:進貨退回/折讓
            m4rptColList[15].money.Add(new m4rptCol2() { numA = 111, numB = 222, numC = 333 });
            for (int i = 1; i <= 12; i++)
            {
                m4rptColList[15].money.Add(new m4rptCol2() { numA = 1, numB = 2, numC = 3 });
            }

            //成本小計
            m4rptColList[16].money.Add(new m4rptCol2() { numA = 111, numB = 222, numC = 333 });
            for (int i = 1; i <= 12; i++)
            {
                m4rptColList[16].money.Add(new m4rptCol2() { numA = 1, numB = 2, numC = 3 });
            }

            //營業毛利
            m4rptColList[17].money.Add(new m4rptCol2() { numA = sumDouble1_12(gross), numB = 222, numC = 333 });
            for (int i = 1; i <= 12; i++)
            {
                m4rptColList[17].money.Add(new m4rptCol2() { numA = gross[i - 1], numB = 0, numC = 0 });
            }
            #endregion

            string Excel2 = "";
            Excel2 += @"<table border=1 borderColorDark=#ffffff borderColorLight=#000000 cellpadding=5 cellspacing=0 width=100% style='font-size:10pt;border-style: none none none none;'>";
            Excel2 += @"<tr>";
            Excel2 += @"<td style='border-style: none none none none;' colspan=15 align=left >2015年 公司 當月損益比較總表-實際vs預算vs去年同期</td>";
            Excel2 += @"<td style='border-style: none none none none;' colspan=18 align=center ></td>";
            Excel2 += @"<td style='border-style: none none none none;' colspan=7  align=right >單位:元</td>";
            Excel2 += @"</tr>";
            #region 欄位
            
            Excel2 += @"<tr>";
            Excel2 += @"<th rowspan=2 style='width:110pt'>損益科目</th>";
            int TWyear = int.Parse(qslyear) - 1911;
            for (int i = 1; i <= 12; i++)
            {
                Excel2 += @"<th colspan=3>" + TWyear + "." + i + "月</th>";
            }
            Excel2 += @"<th colspan=3>二零零傳媒合計</th>";
            Excel2 += @"</tr>";
            Excel2 += @"<tr align=center height=92>";
            int wid1 = 28;
            int wid2 = 28;
            TWyear--;
            for (int i = 1; i <= 12; i++)
            {
                Excel2 += @"<th style='width:" + wid1 + "pt'>" + i + "月實際</th>";
                Excel2 += @"<th style='width:" + wid1 + "pt'>" + i + "月預算</th>";
                Excel2 += @"<th style='width:" + wid2 + "pt'>去年(" + TWyear + ")" + i + "月</th>";
            }
            wid1 += 16;
            wid2 += 16;
            TWyear++;
            Excel2 += @"<th style='width:" + wid1 + "pt'>" + TWyear + "  (1-12)月實際</th>";
            Excel2 += @"<th style='width:" + wid1 + "pt'>" + TWyear + "  (1-12)月預算</th>";
            TWyear--;
            Excel2 += @"<th style='width:" + wid1 + "pt'>去年  " + TWyear + "  (1-12)月</th>";
            Excel2 += @"</tr>";
            
            #endregion
            #region 值
            foreach (m4rptCol v in m4rptColList)
            {
                Excel2 += @"<tr>";
                string tdcl = "";
                if (v.glClass == "營業收入" || v.glClass == "營業成本" || v.glClass == "營業毛利")
                {
                    tdcl = "xl67";
                }
                if (v.glClass == "收入小計" || v.glClass == "成本小計")
                {
                    tdcl = "xl66";
                }
                Excel2 += @"<td class='" + tdcl + "'>" + v.glClass + "</td>";
                for (int i = 1; i <= 12; i++)
                {
                    Excel2 += @"<td>" + v.money[i].numA + "</td>";
                    Excel2 += @"<td>" + v.money[i].numB + "</td>";
                    Excel2 += @"<td>" + v.money[i].numC + "</td>";
                }

                Excel2 += @"<td>" + v.money[0].numA + "</td>";
                Excel2 += @"<td>" + v.money[0].numB + "</td>";
                Excel2 += @"<td>" + v.money[0].numC + "</td>";

                Excel2 += @"</tr>";

            }


            #endregion
            Excel2 += @"</table>";

            return Excel2;
        }

        private double sumDouble1_12(List<double> DoubleList)
        {
            double sumDouble1_12 = 0;
            foreach (double n in DoubleList)
            {
                sumDouble1_12 += n;
            }
            return sumDouble1_12;
        }
        #endregion

        #region  媒體彙總表 mediastatreport_m5
        public ActionResult mediastatreport_m5()
        {

            return View();
        }

        public ActionResult mediastatreport_m5rpt()
        {
            string qslyear = "", qslmonth = "";
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

            string sqlstr = "";
            string Excel1 = "";
            string Excel2 = "";/*繞第二層 table*/
            Excel1 += "<HTML>";
            Excel1 += "<HEAD>";
            Excel1 += @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">";
            Excel1 += @"<style type=""text/css"">";
            Excel1 += @".xl66{text-align:center;border:.5pt solid black;font-weight:700;}";
            Excel1 += @".xl67{text-align:left;border:.5pt solid black;font-weight:700;}";
            Excel1 += @"</style>";
            Excel1 += "</HEAD>";
            Excel1 += "<body>";

            Excel1 += "<center><b style='font-size:17pt'>二零零八傳媒行銷股份有限公司<BR>";
            Excel1 += "2008 Media Marketing Company Limited</b><p><p>";
            Excel1 += " <b style='font-size:17pt'>媒體彙總表- BY 客戶別</b>";
            int year = int.Parse(qslyear) - 1911;
            Excel1 += " <br>" + year + "年" + int.Parse(qslmonth).ToString("00") + "月</center>";

            Excel2 = m5rptExcel2(qslyear, qslmonth);
            Excel1 += Excel2;


            Excel1 += "</body>";
            Excel1 += "</HTML>";
            ViewBag.Excel = Excel1;
            return View();
        }

        private string m5rptExcel2(string qslyear, string qslmonth)
        {
            List<m5rptCol> m5rptColList = new List<m5rptCol>();

            for (int i = 0; i < 5; i++)
            {
                m5rptColList.Add(new m5rptCol()
                {
                    month = 1,
                    media = "MG",
                    comas = "傳立",
                    customer = "永勝光學",
                    product = @"永勝光學 產品" + i,

                    ACnum1 = 1,
                    adcfee = 2,
                    cashDOS = 3,
                    ACnum2 = 4,
                    BonusA = 5,
                    BonusB = 6,
                    BonusC = 7,
                    BonusD = 8,
                    BonusE = 9,
                    incomeA = 10,
                    BonusF = 11,
                    incomeB = 12,
                    cost = 13,
                    ServiceCharge = 14,
                    cashDOM = 15,
                    GProfit = 16,
                    GMargin = 17,
                    GMnobonus = 18,
                    diffMoney = 19,

                    Note = "備註"

                });
            }
            for (int i = 0; i < 5; i++)
            {
                m5rptColList.Add(new m5rptCol()
                {
                    month = 1,
                    media = "TPD",
                    comas = "傳立",
                    customer = "鈊象電子",
                    product = @"鈊象電子 產品" + i,

                    ACnum1 = 1,
                    adcfee = 2,
                    cashDOS = 3,
                    ACnum2 = 4,
                    BonusA = 5,
                    BonusB = 6,
                    BonusC = 7,
                    BonusD = 8,
                    BonusE = 9,
                    incomeA = 10,
                    BonusF = 11,
                    incomeB = 12,
                    cost = 13,
                    ServiceCharge = 14,
                    cashDOM = 15,
                    GProfit = 16,
                    GMargin = 17,
                    GMnobonus = 18,
                    diffMoney = 19,

                    Note = "備註"

                });
            }


            int wid1 = 20;
            int wid2 = 32;
            string Excel2 = "";
            Excel2 += @"<table border=1 borderColorDark=#ffffff borderColorLight=#000000 cellpadding=5 cellspacing=0 width=100% style='font-size:10pt;border-style: none none none none;'>";
            Excel2 += @"<tr>";
            Excel2 += @"<td style='border-style: none none none none;' colspan=10 align=left ></td>";
            Excel2 += @"<td style='border-style: none none none none;' colspan=10 align=center ></td>";
            Excel2 += @"<td style='border-style: none none none none;' colspan=5  align=right >M104.01-A</td>";
            Excel2 += @"</tr>";
            #region 欄位
            Excel2 += @"<tr>";
            Excel2 += @"<th style='width:" + wid1 + "pt'>月份</th>";
            Excel2 += @"<th>媒體</th>";
            Excel2 += @"<th>相關公司</th>";
            Excel2 += @"<th>客戶</th>";
            Excel2 += @"<th>產品</th>";
            Excel2 += @"<th>A/C%</th>";
            Excel2 += @"<th style='width:" + wid2 + "pt'>廣告費</th>";
            Excel2 += @"<th style='width:" + wid2 + "pt'>銷貨現金折扣</th>";
            Excel2 += @"<th>A/C</th>";
            Excel2 += @"<th style='width:" + wid1 + "pt'>應收獎金</th>";
            Excel2 += @"<th style='width:" + wid1 + "pt'>實退獎金</th>";
            Excel2 += @"<th style='width:" + wid1 + "pt'>獎金餘額</th>";
            Excel2 += @"<th style='width:" + wid2 + "pt'>當月獎金收入</th>";
            Excel2 += @"<th style='width:" + wid1 + "pt'>保留獎金</th>";
            Excel2 += @"<th style='width:" + wid2 + "pt'>前期轉收入</th>";
            Excel2 += @"<th style='width:" + wid2 + "pt'>獎金收入小計</th>";
            Excel2 += @"<th style='width:" + wid1 + "pt'>收入合計</th>";
            Excel2 += @"<th style='width:" + wid1 + "pt'>成本</th>";
            int wid3 = wid2 + 22;
            Excel2 += @"<th style='width:" + wid3 + "pt'>代理服務費2% or 2.5%</th>";
            Excel2 += @"<th style='width:" + wid2 + "pt'>媒體現金折扣</th>";
            Excel2 += @"<th style='width:" + wid1 + "pt'>毛利</th>";
            Excel2 += @"<th style='width:" + wid2 + "pt'>毛利率</th>";
            wid3 = wid2 + 12;
            Excel2 += @"<th style='width:" + wid3 + "pt'>毛利率(不含獎金)</th>";
            Excel2 += @"<th style='width:" + wid1 + "pt'>價差</th>";
            Excel2 += @"<th style='width:" + wid1 + "pt'>備註</th>";
            Excel2 += @"</tr>";
            #endregion
            #region 值
            m5rptCol SUMm5rptCol = new m5rptCol();/*客戶 小計*/
            m5rptCol totalSUM = new m5rptCol();/*總計*/
            string oldcustomer = "";
            int ii = 0;
            foreach (m5rptCol v in m5rptColList)
            {
                if (ii != 0)
                {
                    if (oldcustomer == v.customer)
                    {
                        Excel2 += OutRaw(v, SUMm5rptCol, out SUMm5rptCol);/*細項輸出(累加)*/
                    }
                    else
                    {
                        oldcustomer = v.customer;
                        Excel2 += OutRaw2(SUMm5rptCol);/*客戶小計輸出*/
                        funtotalSUM(SUMm5rptCol, totalSUM, out totalSUM);
                        SUMm5rptCol = v;
                        Excel2 += OutRaw3(v);/*細項輸出(沒有累加)*/
                    }
                }
                else
                {
                    ii++;
                    SUMm5rptCol = v;
                    oldcustomer = v.customer;
                    Excel2 += OutRaw3(v);/*細項輸出(沒有累加)*/
                }
            }
            Excel2 += OutRaw2(SUMm5rptCol);/*客戶小計輸出*/
            funtotalSUM(SUMm5rptCol, totalSUM, out totalSUM);
            SUMm5rptCol = null;

            Excel2 += OutRaw4(totalSUM);/* 總計 輸出*/
            #endregion
            Excel2 += @"</table>";

            return Excel2;
        }

        private string OutRaw4(m5rptCol v)
        {
            string Excel2 = "";
            Excel2 += @"<tr>";
            Excel2 += @"<td>" + v.month + "</td>";
            Excel2 += @"<td>" + v.media + "</td>";
            Excel2 += @"<td>" + v.comas + "</td>";
            Excel2 += @"<td colspan=2 class='xl67'>" + v.customer + "總計</td>";
            Excel2 += @"<td>" + v.ACnum1 + "%</td>";
            Excel2 += @"<td>" + v.adcfee + "</td>";
            Excel2 += @"<td>" + v.cashDOS + "</td>";
            Excel2 += @"<td>" + v.ACnum2 + "</td>";
            Excel2 += @"<td>" + v.BonusA + "</td>";
            Excel2 += @"<td>" + v.BonusB + "</td>";
            Excel2 += @"<td>" + v.BonusC + "</td>";
            Excel2 += @"<td>" + v.BonusD + "</td>";
            Excel2 += @"<td>" + v.BonusE + "</td>";
            Excel2 += @"<td>" + v.incomeA + "</td>";
            Excel2 += @"<td>" + v.BonusF + "</td>";
            Excel2 += @"<td>" + v.incomeB + "</td>";
            Excel2 += @"<td>" + v.cost + "</td>";
            Excel2 += @"<td>" + v.ServiceCharge + "</td>";
            Excel2 += @"<td>" + v.cashDOM + "</td>";
            Excel2 += @"<td>" + v.GProfit + "</td>";
            Excel2 += @"<td>" + v.GMargin + "</td>";
            Excel2 += @"<td>" + v.GMnobonus + "</td>";
            Excel2 += @"<td>" + v.diffMoney + "</td>";
            Excel2 += @"<td>" + v.Note + "</td>";
            Excel2 += @"</tr>";
            return Excel2;
        }/* 總計 輸出*/

        private void funtotalSUM(m5rptCol SUMm5rptCol, m5rptCol totalSUM1, out m5rptCol totalSUM2)
        {
            totalSUM1.BonusA += SUMm5rptCol.BonusA;
            totalSUM1.BonusB += SUMm5rptCol.BonusB;
            totalSUM1.BonusC += SUMm5rptCol.BonusC;
            totalSUM1.BonusD += SUMm5rptCol.BonusD;
            totalSUM1.BonusE += SUMm5rptCol.BonusE;


            totalSUM2 = totalSUM1;
        }/*客戶小計(累加)*/

        private string OutRaw3(m5rptCol v)
        {
            string Excel2 = "";
            Excel2 += @"<tr>";
            Excel2 += @"<td>" + v.month + "</td>";
            Excel2 += @"<td>" + v.media + "</td>";
            Excel2 += @"<td>" + v.comas + "</td>";
            Excel2 += @"<td>" + v.customer + "</td>";
            Excel2 += @"<td>" + v.product + "</td>";
            Excel2 += @"<td>" + v.ACnum1 + "%</td>";
            Excel2 += @"<td>" + v.adcfee + "</td>";
            Excel2 += @"<td>" + v.cashDOS + "</td>";
            Excel2 += @"<td>" + v.ACnum2 + "</td>";
            Excel2 += @"<td>" + v.BonusA + "</td>";
            Excel2 += @"<td>" + v.BonusB + "</td>";
            Excel2 += @"<td>" + v.BonusC + "</td>";
            Excel2 += @"<td>" + v.BonusD + "</td>";
            Excel2 += @"<td>" + v.BonusE + "</td>";
            Excel2 += @"<td>" + v.incomeA + "</td>";
            Excel2 += @"<td>" + v.BonusF + "</td>";
            Excel2 += @"<td>" + v.incomeB + "</td>";
            Excel2 += @"<td>" + v.cost + "</td>";
            Excel2 += @"<td>" + v.ServiceCharge + "</td>";
            Excel2 += @"<td>" + v.cashDOM + "</td>";
            Excel2 += @"<td>" + v.GProfit + "</td>";
            Excel2 += @"<td>" + v.GMargin + "</td>";
            Excel2 += @"<td>" + v.GMnobonus + "</td>";
            Excel2 += @"<td>" + v.diffMoney + "</td>";
            Excel2 += @"<td>" + v.Note + "</td>";
            Excel2 += @"</tr>";
            return Excel2;
        }/*細項輸出(沒有累加)*/

        private string OutRaw2(m5rptCol v)
        {
            string Excel2 = "";
            Excel2 += @"<tr>";
            Excel2 += @"<td>" + v.month + "</td>";
            Excel2 += @"<td>" + v.media + "</td>";
            Excel2 += @"<td>" + v.comas + "</td>";
            Excel2 += @"<td colspan=2 class='xl67'>" + v.customer + "合計</td>";
            Excel2 += @"<td>" + v.ACnum1 + "%</td>";
            Excel2 += @"<td>" + v.adcfee + "</td>";
            Excel2 += @"<td>" + v.cashDOS + "</td>";
            Excel2 += @"<td>" + v.ACnum2 + "</td>";
            Excel2 += @"<td>" + v.BonusA + "</td>";
            Excel2 += @"<td>" + v.BonusB + "</td>";
            Excel2 += @"<td>" + v.BonusC + "</td>";
            Excel2 += @"<td>" + v.BonusD + "</td>";
            Excel2 += @"<td>" + v.BonusE + "</td>";
            Excel2 += @"<td>" + v.incomeA + "</td>";
            Excel2 += @"<td>" + v.BonusF + "</td>";
            Excel2 += @"<td>" + v.incomeB + "</td>";
            Excel2 += @"<td>" + v.cost + "</td>";
            Excel2 += @"<td>" + v.ServiceCharge + "</td>";
            Excel2 += @"<td>" + v.cashDOM + "</td>";
            Excel2 += @"<td>" + v.GProfit + "</td>";
            Excel2 += @"<td>" + v.GMargin + "</td>";
            Excel2 += @"<td>" + v.GMnobonus + "</td>";
            Excel2 += @"<td>" + v.diffMoney + "</td>";
            Excel2 += @"<td>" + v.Note + "</td>";
            Excel2 += @"</tr>";
            return Excel2;
        }/*客戶小計輸出(沒有累加)*/

        private string OutRaw(m5rptCol v, m5rptCol v1, out m5rptCol v2)
        {
            string Excel2 = "";
            Excel2 += @"<tr>";
            Excel2 += @"<td>" + v.month + "</td>"; v1.month = v.month;
            Excel2 += @"<td>" + v.media + "</td>"; v1.media = v.media;
            Excel2 += @"<td>" + v.comas + "</td>"; v1.comas = v.comas;
            Excel2 += @"<td>" + v.customer + "</td>"; v1.customer = v.customer;
            Excel2 += @"<td>" + v.product + "</td>";
            Excel2 += @"<td>" + v.ACnum1 + "%</td>";
            Excel2 += @"<td>" + v.adcfee + "</td>";
            Excel2 += @"<td>" + v.cashDOS + "</td>";
            Excel2 += @"<td>" + v.ACnum2 + "</td>";
            Excel2 += @"<td>" + v.BonusA + "</td>"; v1.BonusA += v.BonusA;
            Excel2 += @"<td>" + v.BonusB + "</td>"; v1.BonusB += v.BonusB;
            Excel2 += @"<td>" + v.BonusC + "</td>"; v1.BonusC += v.BonusC;
            Excel2 += @"<td>" + v.BonusD + "</td>"; v1.BonusD += v.BonusD;
            Excel2 += @"<td>" + v.BonusE + "</td>"; v1.BonusE += v.BonusE;
            Excel2 += @"<td>" + v.incomeA + "</td>";
            Excel2 += @"<td>" + v.BonusF + "</td>";
            Excel2 += @"<td>" + v.incomeB + "</td>";
            Excel2 += @"<td>" + v.cost + "</td>";
            Excel2 += @"<td>" + v.ServiceCharge + "</td>";
            Excel2 += @"<td>" + v.cashDOM + "</td>";
            Excel2 += @"<td>" + v.GProfit + "</td>";
            Excel2 += @"<td>" + v.GMargin + "</td>";
            Excel2 += @"<td>" + v.GMnobonus + "</td>";
            Excel2 += @"<td>" + v.diffMoney + "</td>";
            Excel2 += @"<td>" + v.Note + "</td>";
            Excel2 += @"</tr>";

            v2 = v1;
            return Excel2;
        }/*細項輸出(累加)*/
        #endregion

        #region 媒體系統年度報表 mediastatreport_m6
        public ActionResult mediastatreport_m6()
        {
            return View();
        }

        public ActionResult mediastatreport_m6rpt()
        {
            string qslyear = "", qtype = "", allcomid = "";
            if (!string.IsNullOrWhiteSpace(Request["qslyear"]))
            {
                qslyear = Request["qslyear"].Trim();
                ViewBag.qslyear = qslyear;
            }
           

            if (!string.IsNullOrWhiteSpace(Request["qtype"]))
            {
                qtype = Request["qtype"].Trim();
                ViewBag.qtype = qtype;
            }


            if (!string.IsNullOrWhiteSpace(Request["allcomid"]))
            {
                allcomid = Request["allcomid"].Trim();
                ViewBag.allcomid = allcomid;
            }

            string Excel1 = "";
            string Excel2 = "";/*繞第二層 table*/
            Excel1 += "<HTML>";
            Excel1 += "<HEAD>";
            Excel1 += @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">";
            Excel1 += @"<style type=""text/css"">";
            Excel1 += @".xl66{text-align:center;border:.5pt solid black;font-weight:700;}";
            Excel1 += @".xl67{text-align:left;border:.5pt solid black;font-weight:700;}";
            Excel1 += @"</style>";
            Excel1 += "</HEAD>";
            Excel1 += "<body>";

            Excel1 += "<center><b style='font-size:17pt'>二零零八傳媒行銷股份有限公司<BR>";
            Excel1 += "2008 Media Marketing Company Limited</b><p><p>";
            Excel1 += " <b style='font-size:17pt'>媒體系統年度報表</b>";
            int year = int.Parse(qslyear) - 1911;
            Excel1 += " <br>" + year + "年</center>";

            Excel2 = m6rptExcel2(qslyear, qtype, allcomid);
            Excel1 += Excel2;


            Excel1 += "</body>";
            Excel1 += "</HTML>";
            ViewBag.Excel = Excel1;
            return View();
        }


        private string m6rptExcel2(string qslyear, string qtype, string allcomid)
        {
            NDcommon dbobj = new NDcommon();

            List<mcnoColrpt6> mcnoList = new List<mcnoColrpt6>();        

            #region /*媒體類別*/
            using (SqlConnection conn = dbobj.get_conn("AitagBill_DBContext"))
            {
                string sql = "select mcno,mctitle from mediaclass order by mcno";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        mcnoList.Add(new mcnoColrpt6() { mcno = dbobj.get_dbnull2(dr["mcno"]), mctitle = dbobj.get_dbnull2(dr["mctitle"]) });
                      
                    }
                    dr.Close();

                }
            }
            #endregion

       
            List<itemCol> itemList = new List<itemCol>();
            List<prodCol> prodList = new List<prodCol>();         
            using (SqlConnection conn = dbobj.get_conn("AitagBill_DBContext"))
            {
                //1:收入 2:支出
                string sqlmain = "";//找客戶
                string sqlmain1 = "";//找產品名稱或媒體名稱
             
                if (qtype == "1")
                {
                    if (allcomid != "")
                    { sqlmain = " select custcomid as allcomid,(select distinct comsttitle from allcompany where comid = custcomid) as comname  from vend_monthmoney where vtype = '1' and slyear = " + qslyear + " and custcomid='" + allcomid + "' group by custcomid order by custcomid"; }
                    else
                    { sqlmain = " select custcomid as allcomid,(select distinct comsttitle from allcompany where comid = custcomid) as comname  from vend_monthmoney where vtype = '1' and slyear = " + qslyear + " group by custcomid order by custcomid"; }
                    
                    sqlmain1 = " select prodno as prid,(select distinct prodtitle from custproduct where prodid = prodno) as pname from vend_monthmoney where vtype = '1' and slyear = " + qslyear;
                    
                }
                else
                {
                    if (allcomid != "")
                    { sqlmain = " select vendcomid as allcomid,(select distinct comsttitle from allcompany where comid = vendcomid) as comname from vend_monthmoney where vtype = '2' and slyear = " + qslyear + " and vendcomid='" + allcomid + "'  group by vendcomid order by vendcomid"; }
                    else
                    { sqlmain = " select vendcomid as allcomid,(select distinct comsttitle from allcompany where comid = vendcomid) as comname from vend_monthmoney where vtype = '2' and slyear = " + qslyear + "  group by vendcomid order by vendcomid"; }
                    sqlmain1 = " select mdno as prid,(select distinct mdtitle from mediachannel where mdno = vend_monthmoney.mdno) as pname from vend_monthmoney where vtype = '2' and slyear = " + qslyear;
                   
                }


                using (SqlCommand cmd = new SqlCommand(sqlmain, conn))
                {
                    using (SqlConnection conn1 = dbobj.get_conn("AitagBill_DBContext"))
                    {
                        SqlDataReader dr = cmd.ExecuteReader();     
                        while (dr.Read())
                        {
                            int i = 0;
                        
                           
                            string sql="";
                            if (qtype == "1")
                            {sql = sqlmain1 + " and custcomid='" + dr["allcomid"]+ "' group by prodno order by prodno";}
                            else
                            { sql = sqlmain1 + " and vendcomid='" + dr["allcomid"]+ "' group by mdno order by mdno";}
                            
                            SqlDataReader dr1 = dbobj.dbselect(conn1, sql);

                            while (dr1.Read())
                            {
                               int j = mcnoList.Count;
                              
                            
                                foreach (mcnoColrpt6 v in mcnoList)    
                                  {
                                      i++;                                     
                                  }
                              
                                prodList.Add(new prodCol() { cid = dbobj.get_dbnull2(dr["allcomid"]), prid = dbobj.get_dbnull2(dr1["prid"]), pname = dbobj.get_dbnull2(dr1["pname"]), item = j }); //找產品名稱或媒體名稱
                               // prodList.Add(dbobj.get_dbnull2(dr1["prid"]));  //找產品名稱或媒體名稱
                            }
                           
                   
                            itemList.Add(new itemCol() { cid = dbobj.get_dbnull2(dr["allcomid"]), cname = dbobj.get_dbnull2(dr["comname"]), item = i});

                            dr1.Close();
                            dr1.Dispose();                           
                        }

                        dr.Close();
                      dr.Dispose();
                    }
                }
            }

        
            string Excel2 = "";
            Excel2 += @"<table border=1 borderColorDark=#ffffff borderColorLight=#000000 cellpadding=5 cellspacing=0 width=100% style='font-size:10pt'>";
            Excel2 += @"<tr>";
            if (qtype == "1")
            {
                Excel2 += @"<th>客戶名稱</th>";
                Excel2 += @"<th>產品名稱</th>";
            }
            else
            {
                Excel2 += @"<th>廠商名稱</th>";
                Excel2 += @"<th>媒體名稱</th>";
            }
            Excel2 += @"<th>媒體類別/結帳年月</th>";
            for (int i = 1; i <= 12; i++)
            {
                Excel2 += @"<th>"+i.ToString()+"</th>";
            }
            Excel2 += @"<th>總金額</th>";
            Excel2 += @"</tr>";

         
            foreach (itemCol v in itemList)        
            {
                int tmp = 0;
                Excel2 += @"<tr align=center>";
                Excel2 += @"<td rowspan=" + v.item + ">" + v.cname + "</td>";

                foreach (prodCol v1 in prodList)
                {
                 
                    if (v.cid == v1.cid)
                    { 
                        tmp++;
                        if (tmp != 1)
                        { Excel2 += @"<tr>"; }

                        Excel2 += @"<td rowspan=" + v1.item + " align=left>" + v1.pname + "</td>";                                      
                        int tmp1 = 0;
                        foreach (mcnoColrpt6 v2 in mcnoList)
                        {
                            tmp1++;
                            if (tmp1 != 1)
                            { Excel2 += @"<tr>"; }

                            Excel2 += @"<td align=left>" + v2.mctitle + "</td>";

                            decimal summonthpallmoney = 0;
                            //找值
                            using (SqlConnection conn2 = dbobj.get_conn("AitagBill_DBContext"))
                            {
                                for (int tmpi = 1; tmpi <= 12; tmpi++)
                                {
                                    string sqlmain2 = "";//錢
                                    if (qtype == "1")
                                    { sqlmain2 = " select isnull(sum(pallmoney),0) as pallmoney from vend_monthmoney where vtype = '1' and slyear = " + qslyear + " and custcomid='" + v.cid + "' and prodno='" + v1.prid + "'"; }
                                    else
                                    { sqlmain2 = " select isnull(sum(pallmoney),0) as pallmoney from vend_monthmoney where vtype = '2' and slyear = " + qslyear + " and vendcomid='" + v.cid + "' and mdno='" + v1.prid + "'"; }

                                    sqlmain2 = sqlmain2 + " and mcno='" + v2.mcno + "' and slmonth=" + tmpi;

                                    string monthpallmoney = "";
                                    monthpallmoney = dbobj.get_dbvalue(conn2, sqlmain2) + "";
                                    if (monthpallmoney == "")
                                    { monthpallmoney = "0"; }
                                    
                                    monthpallmoney = int.Parse(monthpallmoney).ToString("###,###,##0");

                                    Excel2 += @"<td align=right>" + monthpallmoney + "</td>";

                                    summonthpallmoney += decimal.Parse(monthpallmoney);
                                   
                                }

                                Excel2 += @"<td align=right>" + summonthpallmoney.ToString("###,###,##0") + "</td>";

                            }
                                  

                        
                        }
                        Excel2 += @"</tr>";
                    }
                }

             
            }
           
          
            
            Excel2 += @"</table>";

            return Excel2;
        }

        #endregion
    }

    
    #region  客戶業績排行榜 mediastatreport_m1
    public class m1rptCol
    {
        public string custcomid = "";
        public double numA = 0;
        public double numB = 0;
        public double numC = 0;
        public double numD = 0;
        public double numE = 0;
        public double numF = 0;
        public double numG = 0;
        public double numH = 0;
    }
    #endregion
    #region  營業收入發稿統計表 mediastatreport_m2
    public class m2rptCol
    {
        public string custcomid = "";
        public List<double> pallmoney = new List<double>();
        public List<double> paccmoney = new List<double>();
        public double sumPallmoney = 0;
        public double sumPaccmoney = 0;
    }

    #endregion
    #region  各類營業收入統計表 mediastatreport_m3
    public class mcnoCol
    {
        public string mdno = "";
        public string money = "";
        public string pallmoney = "";
    }
    public class Excel4Raw
    {
        public List<string> Excel4Col = new List<string>();
    }

    #endregion
    #region  年度損益管報 mediastatreport_m4
    public class m4rptCol
    {
        public string glClass = "";/*損益科目*/
        public List<m4rptCol2> money = new List<m4rptCol2>();
    }
    public class m4rptCol2
    {
        public double numA = 0;
        public double numB = 0;
        public double numC = 0;
    }
    #endregion
    #region  媒體彙總表 mediastatreport_m5
    public class m5rptCol
    {
        
        public int month = 0;/*月份month*/
        
        public string media = "";/*媒體media*/
        
        public string comas = "";/*相關公司comas*/
       
        public string customer = "";/*客戶customer*/
        
        public string product = "";/*產品product*/
        
        public double ACnum1 = 0;/*  A/C%  */
        
        public double adcfee = 0;/*廣告費adcfee*/
        
        public double cashDOS = 0;/*銷貨現金折扣cashDOS*/
        
        public double ACnum2 = 0;/*  A/C  */
        
        public double BonusA = 0;/*應收獎金BonusA*/
        
        public double BonusB = 0;/*實退獎金BonusB*/
        
        public double BonusC = 0;/*獎金餘額BonusC*/
        
        public double BonusD = 0;/*當月獎金收入BonusD*/
        
        public double BonusE = 0;/*保留獎金BonusE*/
        
        public double incomeA = 0;/*前期轉收入incomeA*/
        
        public double BonusF = 0;/*獎金收入小計BonusF*/
        
        public double incomeB = 0;/*收入合計incomeB*/
        
        public double cost = 0;/*成本cost*/
        
        public double ServiceCharge = 0;/* 代理服務費2% or 2.5%  ServiceCharge */
        
        public double cashDOM = 0;/*媒體現金折扣cashDOM*/
        
        public double GProfit = 0;/*毛利GProfit*/
        
        public double GMargin = 0;/*毛利率GMargin*/
        
        public double GMnobonus = 0;/*毛利率(不含獎金)GMnobonus*/
        
        public double diffMoney = 0;/*價差diffMoney*/
        
        public string Note = "";/*備註Note*/

    }
    #endregion    
    #region  媒體系統年度報表 mediastatreport_m6

    public class itemCol
    {
        public string cid = "";
        public string cname = "";
        public int item = 0;
    }

    public class prodCol
    {
        public string cid = "";
        public string prid = "";
        public string pname = "";
        public int item = 0;
    }

     public class mcnoColrpt6
    {
         public string mcno = "";
         public string mctitle = "";
      
    }

   
    #endregion

}
