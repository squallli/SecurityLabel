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
    public class otworklogController : BaseController
    {
        public string mfrom = System.Configuration.ConfigurationManager.AppSettings["mail_from"].ToString();
        string DateEx = "";

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /otworklog/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "otlogsdate"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qotstatus = "", qempname = "", qdptid = "", otlogsdate = "", otlogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qotstatus"]))
            {
                qotstatus = Request["qotstatus"].Trim();
                ViewBag.qotstatus = qotstatus;
            }
            else
            {
                qotstatus = "1";
                ViewBag.qotstatus = qotstatus;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempname"]))
            {
                qempname = Request["qempname"].Trim();
                ViewBag.qempname = qempname;
            }
            if (!string.IsNullOrWhiteSpace(Request["qdptid"]))
            {
                qdptid = Request["qdptid"].Trim();
                ViewBag.qdptid = qdptid;
            }
            otlogsdate = NullStDate(Request["otlogsdate"]);
            ViewBag.otlogsdate = otlogsdate;
            otlogedate = NullTeDate(Request["otlogedate"]);
            ViewBag.otlogedate = otlogedate;
            //NullStDate 跟 NullTeDate 會判斷格式，有錯誤就 修改全域的DateEx
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }
            NDcommon dbobj = new NDcommon();
            IPagedList<otworklog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string viewid = "";
                string[] mpriv = (string[])Session["priv"];
                //viewid = get_viewpriv(int.Parse(funcpriv(2)), int.Parse(mpriv(realsid, 2)));
                string tmpall = "";
                //tmpall = dbobj.get_allempid((string)Session["rid"]);
                tmpall = "%";


                string sqlstr = "", sqlstr_1 = "";
                if(tmpall == "%")
                {
                    sqlstr = "SELECT * FROM otworklog where comid='" + (string)Session["comid"] + "'  and";
                    sqlstr_1 = "SELECT isnull(sum(otloghour),0) as otloghour FROM otworklog where comid='" + (string)Session["comid"] + "'  and";
                }
                else
                {
                    sqlstr = "SELECT * FROM otworklog where empid in(" + tmpall + ") and comid='" + (string)Session["comid"] + "'  and";
                    sqlstr_1 = "SELECT isnull(sum(otloghour),0) as otloghour FROM otworklog where empid in(" + tmpall + ") and comid='" + (string)Session["comid"] + "'  and";
                }

                if (qotstatus != "all")
                {
                    string sql_otstatus = " otstatus like '" + qotstatus + "'  and";
                     sqlstr = sqlstr + sql_otstatus;
                     sqlstr_1 = sqlstr_1 + sql_otstatus;
                }
                if (otlogsdate != "" && otlogedate != "")
                {
                    string sql_date = " (( '" + otlogsdate + "' <= otlogsdate and otlogsdate <= '" + otlogedate + "' ) or " +
                                        "( '" + otlogsdate + "' <= otlogedate and otlogedate <= '" + otlogedate + "'  ))  and";
                    sqlstr = sqlstr + sql_date;
                    sqlstr_1 = sqlstr_1 + sql_date;
                }
                if (qempname != "")
                {
                    string sql_empname = " empname like N'%" + qempname + "%'  and";
                    sqlstr = sqlstr + sql_empname;
                    sqlstr_1 = sqlstr_1 + sql_empname;
                }
                if (qdptid != "")
                {
                    string sql_dptid = " dptid='" + qdptid + "'  and";
                    sqlstr = sqlstr + sql_dptid;
                    sqlstr_1 = sqlstr_1 + sql_dptid;
                }


                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr_1 = sqlstr_1.Substring(0, sqlstr_1.Length - 5);
                getSUMhour(sqlstr, sqlstr_1);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.otworklog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<otworklog>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch(orderdata, orderdata1);
            return View(result);
        }

        private void getSUMhour(string sqlstr, string sqlstr_1)
        {
            NDcommon dbobj = new NDcommon();
            using (SqlConnection comconn = dbobj.get_conn("Aitag_DBContext"))
            {
                ViewBag.otloghour = dbobj.get_dbvalue(comconn, sqlstr_1);
                using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                string txtosno = "", sqlstr_2 = "";
                using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            txtosno += "'" + dbobj.get_dbnull2(dr["osno"]) + "',";
                        }
                        sqlstr_2 = "select isnull(sum(resthour),0) as resthour,isnull(sum(moneyhour),0) as moneyhour from resthourlog where osno in (" + txtosno.Substring(0, txtosno.Length -1) + ") and comid='" + (string)Session["comid"] + "'";
                    }
                    else
                    {
                        sqlstr_2 = "select isnull(sum(resthour),0) as resthour,isnull(sum(moneyhour),0) as moneyhour from resthourlog where 1<>1";
                    }
                    dr.Close();


                }
                using (SqlCommand cmd = new SqlCommand(sqlstr_2, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        dr.Read();
                        ViewBag.sumresthour = dbobj.get_dbnull2(dr["resthour"]);
                        ViewBag.summoneyhour = dbobj.get_dbnull2(dr["moneyhour"]);
                    }
                    else
                    {
                        ViewBag.sumresthour = "0";
                        ViewBag.summoneyhour = "0";
                    }
                }
            }
            }
        }
        private string SetOrder_ch(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "otype", "otstatus", "ifotdell", "empid", "empname", "dptid", "otlogsdate" };
            string[] od_ch1 = { "asc", "asc", "asc", "asc", "asc", "asc", "asc" };
            Order_ch += @"var orderdata = '" + orderdata + "';";
            Order_ch += @"var orderdata1 = '" + orderdata1 + "';";

            Order_ch += @"var od_ch = new Array(""""";
            foreach (string i in od_ch) { Order_ch += @", '" + i + "'"; }
            Order_ch += @");";

            Order_ch += @"var od_ch1 = new Array(""""";
            foreach (string i in od_ch1) { Order_ch += @", '" + i + "'"; }
            Order_ch += @");";

            Order_ch += @"switch(orderdata){ ";
            int ii = 0;
            foreach (string i in od_ch)
            {
                ii += 1;
                Order_ch += @"case""" + i + @""":od_ch1[" + ii + "]=orderdata1;break;";
            }
            Order_ch += @"};";

            ii = 0;
            foreach (string i in od_ch)
            {
                ii += 1;
                Order_ch += @"SetOrder_A('order" + ii + "', od_ch[" + ii + "], od_ch1[" + ii + "]);";
            }

            //Order_ch += @"";
            Order_ch += "  }  ";
            return SetOrder_A + Order_ch;
        }
        private string NullStDate(string stdate)
        {
            if (string.IsNullOrWhiteSpace(stdate))
            {
                stdate = DateTime.Now.ToString("yyyy/MM") + "/1";
            }
            try
            {
                DateTime.Parse(stdate);
            }
            catch
            {
                DateEx += @"出差日期起格式錯誤!!\n";
                stdate = "";
            }
            return stdate;
        }
        private string NullTeDate(string tedate)
        {
            if (string.IsNullOrWhiteSpace(tedate))
            {
                var dat = new DateTime(DateTime.Now.Year - 1, 12, 31);
                tedate = dat.AddMonths(DateTime.Now.Month).ToString("yyyy/MM/dd");
            }
            try
            {
                DateTime.Parse(tedate);
            }
            catch
            {
                DateEx += @"出差日期訖格式錯誤!!\n";
                tedate = "";
            }

            return tedate;
        }

        #region   getviewpriv  '取得觀看權限

        public string get_viewpriv(int dptval, int perval)
        {
            if (dptval == 0)
            {
                return "9999999999";//無
            }
            else if (dptval == 1)
            {
                //個人
                if (dptval > perval)
                {
                    return "9999999999";
                }
                else
                {
                    return (string)Session["empid"];
                }

            }
            else if (dptval == 2)
            {
                //全部
                if (dptval > perval)
                {
                    if (perval == 1)
                    {
                        return (string)Session["empid"];
                    }
                    else
                    {
                        return "9999999999";
                    }
                }
                else if (dptval == perval)
                {
                    return "";
                }
            }
            return "";
        }

        #endregion


        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string orderdata = "", orderdata1 = "";
            if (!string.IsNullOrWhiteSpace(Request["orderdata"]))
            {
                orderdata = Request["orderdata"].Trim();
            }
            if (!string.IsNullOrWhiteSpace(Request["orderdata1"]))
            {
                orderdata1 = Request["orderdata1"].Trim();
            }

            string qotstatus = "", qempname = "", qdptid = "", otlogsdate = "", otlogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qotstatus"]))
            {
                qotstatus = Request["qotstatus"].Trim();
                ViewBag.qotstatus = qotstatus;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempname"]))
            {
                qempname = Request["qempname"].Trim();
                ViewBag.qempname = qempname;
            }
            if (!string.IsNullOrWhiteSpace(Request["qdptid"]))
            {
                qdptid = Request["qdptid"].Trim();
                ViewBag.qdptid = qdptid;
            }
            if (!string.IsNullOrWhiteSpace(Request["otlogsdate"]))
            {
                otlogsdate = Request["otlogsdate"].Trim();
                ViewBag.otlogsdate = otlogsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["otlogedate"]))
            {
                otlogedate = Request["otlogedate"].Trim();
                ViewBag.otlogedate = otlogedate;
            }


            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/otworklog/List' method='post'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";


            tmpform += "<input type=hidden id='qotstatus' name='qotstatus' value='" + qotstatus + "'>";
            tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
            tmpform += "<input type=hidden id='qdptid' name='qdptid' value='" + qdptid + "'>";
            tmpform += "<input type=hidden id='otlogsdate' name='otlogsdate' value='" + otlogsdate + "'>";
            tmpform += "<input type=hidden id='otlogedate' name='otlogedate' value='" + otlogedate + "'>";

            tmpform += "</form>";
            tmpform += "</body>";

            string cdel = Request["cdel"];
            if (string.IsNullOrWhiteSpace(cdel))
            {
                return new ContentResult() { Content = @"<script>alert('請勾選要刪除的資料!!');</script>" + tmpform };
            }
            else
            {
                Int16 tmpcount = 0;
                string tmpcomment = "";
                string sql = "select * from otworklog";
                string sqlwhere = " where otlogid in (" + cdel + ") and comid='" + (string)Session["comid"] + "'";
                sql += sqlwhere;
                NDcommon dbobj = new NDcommon();
                using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                {
                    using (SqlConnection comconn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            string xotlogstime = "", xotlogetime = "";
                            string otcomment = "", mailtitle = "", MailContext = "";
                            string fromadd = "", fromaddname = "", toadd = "";
                            string rolestampid = "", sql_m = "";
                            int bloghour = 0;


                            SqlDataReader dr = cmd.ExecuteReader();
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    xotlogstime = "（" + dr["otlogstime"] + "）";
                                    xotlogetime = "（" + dr["otlogetime"] + "）";

                                    #region  寄信(通知給目前簽核角色)
                                    if (dbobj.get_dbnull2(dr["otcomment"]) != "")
                                    {
                                        otcomment = dbobj.get_dbnull2(dr["otcomment"]).Replace(Environment.NewLine, "<br>");
                                    }
                                    else{
                                        otcomment = "&nbsp;";
                                    }
                                    mailtitle = "加班確認單撤回通知";

                                    MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=BIG5'></HEAD><body>";
                                    MailContext = MailContext + "以下為明細資料：<BR>";
                                    MailContext = MailContext + "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                                    MailContext = MailContext + "<tr><td align=right width=130>申請人：</td><td>" + dbobj.get_dbnull2(dr["empname"]) + "</td></tr>";
                                    MailContext = MailContext + "<tr><td align=right>起迄日期：</td><td>自 " + dbobj.get_dbnull2(dr["otlogsdate"]) + xotlogstime + "<BR>至 " + dbobj.get_dbnull2(dr["otlogedate"]) + xotlogetime + "</td></tr>";
                                    MailContext = MailContext + "<tr><td align=right>共計小時：</td><td>" + dbobj.get_dbnull2(dr["otloghour"]) + "時</td></tr>";
                                    MailContext = MailContext + "<tr><td align=right>事由：</td><td>" + otcomment + "+nbsp;</td></tr>";
                                    MailContext = MailContext + "</table>";
                                    MailContext = MailContext + "</body></HTML>";

                                    //寄件者
                                    fromadd = dbobj.get_dbvalue(comconn, "select enemail from employee where empid='" + (string)Session["empid"] + "'");
                                    fromaddname = (string)Session["empname"];

                                    //'寄給申請人
                                    toadd = dbobj.get_dbvalue(comconn, "select enemail from employee where empid='" + dbobj.get_dbnull2(dr["empid"]) + "'");

                                    if (toadd != "")
                                    {
                                        //#include file=../inc/mail.asp
                                        dbobj.send_mail(mfrom, toadd, mailtitle, MailContext);
                                    }
                                    //收件者
                                    if (dbobj.get_dbnull2(dr["rolestampid"]) != "")
                                    {
                                        rolestampid = dbobj.get_dbnull2(dr["rolestampid"]);
                                        sql_m = "select enemail from viewemprole where rid in (" + rolestampid + ") and empstatus <> '4' and enemail<>'' and comid='" + (string)Session["comid"] + "'";
                                        using (SqlCommand cmd2 = new SqlCommand(sql, conn))
                                        {
                                            SqlDataReader dr2 = cmd2.ExecuteReader();
                                            while (dr.Read())
                                            {
                                                toadd = dbobj.get_dbnull2(dr2["enemail"]);
                                                //#include file=../inc/mail.asp
                                                dbobj.send_mail(mfrom, toadd, mailtitle, MailContext);
                                            }
                                            dr.Close();
                                        }
                                    }
                                    #endregion

                                    sql = "UPDATE otworklog SET otstatus = 'D'";
                                    sql += sqlwhere;
                                    sql += ";delete from resthourlog";
                                    sql += sqlwhere;
                                    dbobj.dbexecute("Aitag_DBContext", sql);


                                    tmpcount++;
                                    tmpcomment += "姓名：" + dbobj.get_dbnull2(dr["empname"]) + "申請單號：" + dbobj.get_dbnull2(dr["osno"]) + ",";
                                }
                                tmpcomment = tmpcomment.Substring(0, tmpcomment.Length - 1);
                            }


                            //系統LOG檔
                            string sysnote = tmpcomment + "的資料" + tmpcount + "筆";
                            if(sysnote.Length >4000)
                            {
                                sysnote = sysnote.Substring(0, 4000);
                            }
                            //================================================= //                  
                            string sysrealsid = Request["sysrealsid"].ToString();
                            SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                            string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2") + "(撤回)";
                            string sysflag = "D";
                            dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                            sysconn.Close();
                            sysconn.Dispose();
                            //====================================================== 
                            dr.Close();
                        }
                    }

                }

                return new ContentResult() { Content = @"<script>alert('刪除成功!!');</script>" + tmpform };
            }
        }

    }
}
