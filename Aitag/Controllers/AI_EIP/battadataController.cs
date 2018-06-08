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
    public class battadataController : BaseController
    {
        public string mfrom = System.Configuration.ConfigurationManager.AppSettings["mail_from"].ToString();
        string DateEx = "";
        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /battalog/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        [ValidateInput(false)]
        public ActionResult Edit(battalog chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "blogid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qblogstatus = "", qempname = "", qdptid = "", qblogsdate = "", qblogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qblogstatus"]))
            {
                qblogstatus = Request["qblogstatus"].Trim();
                ViewBag.qblogstatus = qblogstatus;
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
            if (!string.IsNullOrWhiteSpace(Request["qblogsdate"]))
            {
                qblogsdate = Request["qblogsdate"].Trim();
                ViewBag.qblogsdate = qblogsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qblogedate"]))
            {
                qblogedate = Request["qblogedate"].Trim();
                ViewBag.qblogedate = qblogedate;
            }

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.battalog.Where(r => r.blogid == chks.blogid).FirstOrDefault();
                    battalog ebattalogs = con.battalog.Find(chks.blogid);
                    if (ebattalogs == null)
                    {
                        return HttpNotFound();
                    }
                    return View(ebattalogs);
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

                    //string oldblogid = Request["oldblogid"];                 

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        NDcommon dbobj = new NDcommon();
                        chks.blogid = int.Parse(Request["blogid"].Trim()) ;
                        chks.bmodid = Session["tempid"].ToString();
                        chks.bmoddate = DateTime.Now;
                        con.Entry(chks).State = EntityState.Modified;
                        //con.SaveChanges();


                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "代碼:" + chks.blogid + "名稱:" + chks.empname;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/battadata/List' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                        tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                        tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                        tmpform += "<input type=hidden id='qblogstatus' name='qblogstatus' value='" + qblogstatus + "'>";
                        tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
                        tmpform += "<input type=hidden id='qdptid' name='qdptid' value='" + qdptid + "'>";
                        tmpform += "<input type=hidden id='qblogsdate' name='qblogsdate' value='" + qblogsdate + "'>";
                        tmpform += "<input type=hidden id='qblogedate' name='qblogedate' value='" + qblogedate + "'>";
                        tmpform += "</form>";
                        tmpform += "</body>";


                        return new ContentResult() { Content = @"" + tmpform };
                        //return RedirectToAction("List");
                    }
                }
            }

        }

        public ActionResult battadatarpt(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "bsno"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qblogstatus = "", qempname = "", qdptid = "", qblogsdate = "", qblogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qblogstatus"]))
            {
                qblogstatus = Request["qblogstatus"].Trim();
                ViewBag.qblogstatus = qblogstatus;
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
            qblogsdate = NullStDate(Request["qblogsdate"]);
            ViewBag.qblogsdate = qblogsdate;
            qblogedate = NullTeDate(Request["qblogedate"]);
            ViewBag.qblogedate = qblogedate;
            //NullStDate 跟 NullTeDate 會判斷格式，有錯誤就 修改全域的DateEx
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }

            string Excel = "", Excel2 = "";
            string sqlstr = "";
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string viewid = "";
                string[] mpriv = (string[])Session["priv"];
                //viewid = get_viewpriv(int.Parse(funcpriv(2)), int.Parse(mpriv(realsid, 2)));

                sqlstr = "select * from battalog where (blogtype='1' or (blogtype='2' and (pbsno='' or pbsno is null )) ) and comid='" + (string)Session["comid"] + "'";

                if (viewid != "")
                {
                    sqlstr += " and bmodid = '" + viewid + "'";
                }
                if (qblogstatus != "" && qblogstatus != "all")
                {
                    sqlstr += " and blogstatus = '" + qblogstatus + "'";
                }
                else if (qblogstatus == "")
                {
                    sqlstr += " and blogstatus = '1'";
                    ViewBag.qblogstatus = "1";

                }
                if (qempname != "")
                {
                    sqlstr += " and empname like N'%" + qempname + "%'";
                }
                if (qdptid != "")
                {
                    sqlstr += " and dptid='" + qdptid + "'";
                }

                sqlstr += " and (( blogsdate >= '" + qblogsdate + "' and blogsdate <= '" + qblogedate + "' ) or ";
                sqlstr += "( blogedate >= '" + qblogsdate + "' and blogedate <= '" + qblogedate + "'))";

                sqlstr += " order by " + orderdata + " " + orderdata1;

            }
            #region 組 Excel 格式
            Excel += "<HTML>";
            Excel += "<HEAD>";
            Excel += @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">";
            Excel += "</HEAD>";
            Excel += "<body>";
            Excel += "<table  border=1  cellpadding=0 cellspacing=0 bordercolor=#000000 bordercolordark=#ffffff width=900 >";
            Excel += "<tr align=center>";
            Excel += @"<td colspan=""8"" style=""font-size:14pt"">出差明細表";
            Excel += "</td>";
            Excel += "</tr>";
            Excel += "<tr align=center>";
            int count = 7;
            Excel += "<td colspan='" + count + "' ></td><td>列印日期：" + DateTime.Now.ToString("yyyy/MM/dd") + "</td>";
            Excel += "</tr>";
            Excel += "<tr align=center>";
            Excel += "<td>狀態</td>";
            Excel += "<td>核銷</td>";
            Excel += "<td>員工編號</td>";
            Excel += "<td>姓名</td>";
            Excel += "<td>部門</td>";
            Excel += "<td>出差起迄日</td>";
            Excel += "<td>出差天數</td>";
            Excel += "<td>地點</td>";
            Excel += "</tr>";
            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();

                    string dpttitle = "";
                    string empno = "", blogstatus = "", ifhdell = "";
                    string SEtime = "自{0}({1}時)<br>至{2}({3}時)";
                    string chkitem = "", blogsdate = "", blogstime = "", blogedate = "", blogetime = "";
                    int bloghour = 0;
                    while (dr.Read())
                    {
                        blogstatus = dbobj.get_dbnull2(dr["blogstatus"]);
                        switch (blogstatus)
                        {
                            case "0":
                                blogstatus = "簽核中";
                                break;
                            case "1":
                                blogstatus = "已核准";
                                break;
                            case "2":
                                blogstatus = "退回";
                                break;
                            case "D":
                                blogstatus = "撤回";
                                break;
                            default:
                                break;
                        }
                        ifhdell = dbobj.get_dbnull2(dr["ifhdell"]);
                        if (ifhdell == "y")
                        {
                            ifhdell = "是";
                        }
                        else if (ifhdell == "n")
                        {
                            ifhdell = "否";
                        }
                        using (SqlConnection comconn = dbobj.get_conn("Aitag_DBContext"))
                        {

                            empno = "select empno from employee where empid='" + dbobj.get_dbnull2(dr["empid"]) + "'"; empno = dbobj.get_dbvalue(comconn, empno);
                            dpttitle = "select dpttitle from Department where dptid='" + dbobj.get_dbnull2(dr["dptid"]) + "' and comid='" + (string)Session["comid"] + "'"; dpttitle = dbobj.get_dbvalue(comconn, dpttitle);
                            
                            blogsdate = Convert.ToDateTime(dbobj.get_dbnull2(dr["blogsdate"])).ToString("yyyy/MM/dd");
                            blogstime = int.Parse(dbobj.get_dbnull2(dr["blogstime"])).ToString("00");
                            blogedate = Convert.ToDateTime(dbobj.get_dbnull2(dr["blogedate"])).ToString("yyyy/MM/dd");
                            blogetime = int.Parse(dbobj.get_dbnull2(dr["blogetime"])).ToString("00");
                            bloghour = int.Parse("0" + dbobj.get_dbnull2(dr["bloghour"]));

                            if (bloghour > 0){ bloghour = bloghour / 8; }else{ bloghour = 0; }
                            chkitem = "select chkitem from billsubject where chkclass = '90' and chkcode = '" + dbobj.get_dbnull2(dr["blogaddr"]) + "'"; chkitem = dbobj.get_dbvalue(comconn, chkitem);
                        }


                        Excel2 += "<tr>";
                        Excel2 += "<td>" + blogstatus + "</td>";
                        Excel2 += "<td>" + ifhdell + "</td>";
                        Excel2 += "<td>" + empno + "</td>";
                        Excel2 += "<td>" + dbobj.get_dbnull2(dr["empname"]) + "</td>";
                        Excel2 += "<td>" + dpttitle + "</td>";
                        Excel2 += "<td>" + String.Format(SEtime, blogsdate, blogstime, blogedate, blogetime) + "</td>";
                        Excel2 += "<td>" + bloghour + "</td>";
                        Excel2 += "<td>" + chkitem;
                        if (dbobj.get_dbnull2(dr["blogplace"]) != "")
                        {
                            Excel2 += dbobj.get_dbnull2(dr["blogplace"]);
                        }
                        Excel2 += "</td>";
                        Excel2 += "</tr>";
                    }
                    if (Excel2 == "")
                    {
                        Excel += "<tr align=left><td colspan=6>目前沒有資料</td></tr>";
                    }
                    else
                    {
                        Excel += Excel2;
                    }
                    dr.Close();
                }
            }


            #endregion
            Excel += "</table>";
            Excel += "</body>";
            Excel += "</HTML>";

            ViewBag.Excel = Excel;
            return View();
        }

        public ActionResult List(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "bsno"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qblogstatus = "", qempname = "", qdptid = "", qblogsdate = "", qblogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qblogstatus"]))
            {
                qblogstatus = Request["qblogstatus"].Trim();
                ViewBag.qblogstatus = qblogstatus;
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
            qblogsdate = NullStDate(Request["qblogsdate"]);
            ViewBag.qblogsdate = qblogsdate;
            qblogedate = NullTeDate(Request["qblogedate"]);
            ViewBag.qblogedate = qblogedate;
            //NullStDate 跟 NullTeDate 會判斷格式，有錯誤就 修改全域的DateEx
            if (DateEx != "") { ViewBag.DateEx = @"<script>alert(""" + DateEx + @""");</script>"; }

            IPagedList<battalog> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string viewid = "";
                string[] mpriv = (string[])Session["priv"];
                //viewid = get_viewpriv(int.Parse(funcpriv(2)), int.Parse(mpriv(realsid, 2)));

                string sqlstr = "select * from battalog where (blogtype='1' or (blogtype='2' and (pbsno='' or pbsno is null )) ) and comid='" + (string)Session["comid"] + "'";

                if (viewid != "")
                {
                    sqlstr += " and bmodid = '" + viewid + "'";
                }
                if (qblogstatus != "" && qblogstatus != "all")
                {
                    sqlstr += " and blogstatus = '" + qblogstatus + "'";
                }
                else if (qblogstatus == "")
                {
                    sqlstr += " and blogstatus = '1'";
                    ViewBag.qblogstatus = "1";

                }
                if (qempname != "")
                {
                    sqlstr += " and empname like N'%" + qempname + "%'";
                }
                if (qdptid != "")
                {
                    sqlstr += " and dptid='" + qdptid + "'";
                }

                sqlstr += " and (( blogsdate >= '" + qblogsdate + "' and blogsdate <= '" + qblogedate + "' ) or ";
                sqlstr += "( blogedate >= '" + qblogsdate + "' and blogedate <= '" + qblogedate + "'))";

                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.battalog.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<battalog>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch(orderdata, orderdata1);
            return View(result);
        }
        private string SetOrder_ch(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "blogstatus", "ifhdell", "empid", "empname", "dptid", "blogsdate" };
            string[] od_ch1 = { "asc", "asc", "asc", "asc", "asc", "asc" };
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
                    if(perval == 1)
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

        public ActionResult battadatdo(string id, int? page)
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

            string qblogstatus = "", qempname = "", qdptid = "", qblogsdate = "", qblogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qblogstatus"]))
            {
                qblogstatus = Request["qblogstatus"].Trim();
                ViewBag.qblogstatus = qblogstatus;
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
            if (!string.IsNullOrWhiteSpace(Request["qblogsdate"]))
            {
                qblogsdate = Request["qblogsdate"].Trim();
                ViewBag.qblogsdate = qblogsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qblogedate"]))
            {
                qblogedate = Request["qblogedate"].Trim();
                ViewBag.qblogedate = qblogedate;
            }


            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/battadata/List' method='post'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

            tmpform += "<input type=hidden id='qblogstatus' name='qblogstatus' value='" + qblogstatus + "'>";
            tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
            tmpform += "<input type=hidden id='qdptid' name='qdptid' value='" + qdptid + "'>";
            tmpform += "<input type=hidden id='qblogsdate' name='qblogsdate' value='" + qblogsdate + "'>";
            tmpform += "<input type=hidden id='qblogedate' name='qblogedate' value='" + qblogedate + "'>";

            tmpform += "</form>";
            tmpform += "</body>";
            NDcommon dbobj = new NDcommon();

            string cdel = Request["cdel"];

            string sql = "", sqlwhere = "", tmpcomment = "";
            if (string.IsNullOrWhiteSpace(cdel))
            {
                if (dbobj.get_dbnull2(Request["blogid"]) != "")
                {
                    sql = "select * from battalog";
                    sqlwhere = " where blogid=" + dbobj.get_dbnull2(Request["blogid"]) + " and comid='" + (string)Session["comid"] + "'";
                    sql += sqlwhere;
                    dbobj.dbexecute("Aitag_DBContext", "UPDATE battalog ifhdell='y' " + sqlwhere);
                    using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            SqlDataReader dr = cmd.ExecuteReader();
                            if (dr.HasRows)
                            {
                                dr.Read();
                                //找是否己全部核銷了
                                sql = "select * from battalog where pbsno='" + dbobj.get_dbnull2(dr["pbsno"]) + "' and comid='" + (string)Session["comid"] + "' and ifhdell='n'";
                                using (SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext"))
                                {
                                    using (SqlCommand cmd1 = new SqlCommand(sql, conn1))
                                    {
                                        SqlDataReader dr1 = cmd1.ExecuteReader();
                                        if (dr1.HasRows)
                                        {
                                            while (dr1.Read())
                                            {
                                                sql = "UPDATE battalog SET ifhdell='y' where bsno='" + dbobj.get_dbnull2(dr["pbsno"]) + "' and comid='" + (string)Session["comid"] + "'";
                                                dbobj.dbexecute("Aitag_DBContext", sql);
                                            }
                                        }
                                        dr1.Close();
                                    }
                                } 

                                tmpcomment = "申請人：" + dbobj.get_dbnull2(dr["empname"]) + "<br>申請單號：" + dbobj.get_dbnull2(dr["bsno"]) + "的資料";
                            }
                            dr.Close();
                        }
                    } 





                    return new ContentResult() { Content = @"<script>alert('核銷成功!!');</script>" + tmpform };
                }
                else
                {
                    return new ContentResult() { Content = @"<script>alert('請選擇要核銷的資料。');</script>" + tmpform };
                }
            }
            else
            {
                string chkdel = cdel;
                Int16 mpcount = 0;
                tmpcomment = "";
                sql = "select * from battalog";
                sqlwhere = " where blogid in (" + chkdel + ") and comid='" + (string)Session["comid"] + "'";
                sql += sqlwhere;
                using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                {
                    using (SqlConnection comconn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {

                            SqlDataReader dr = cmd.ExecuteReader();
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    sql = "UPDATE battalog SET ifhdell='y' where blogid=" + dbobj.get_dbnull2(dr["blogid"]) + " and comid='" + (string)Session["comid"] + "'";
                                    dbobj.dbexecute("Aitag_DBContext", sql);
                                    
                                    //找是否己全部核銷了
                                    sql = "select * from battalog where pbsno='" + dbobj.get_dbnull2(dr["pbsno"]) + "' and comid='" + (string)Session["comid"] + "' and ifhdell='n'";
                                    using (SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext"))
                                    {
                                        using (SqlCommand cmd1 = new SqlCommand(sql, conn1))
                                        {
                                            SqlDataReader dr1 = cmd1.ExecuteReader();
                                            if (dr1.HasRows)
                                            {
                                                while (dr1.Read())
                                                {
                                                    sql = "UPDATE battalog SET ifhdell='y' where bsno='" + dbobj.get_dbnull2(dr["pbsno"]) + "' and comid='" + (string)Session["comid"] + "'";
                                                    dbobj.dbexecute("Aitag_DBContext", sql);
                                                }
                                            }
                                            dr1.Close();
                                        }
                                    }   



                                    mpcount++;
                                    tmpcomment += "姓名：" + dbobj.get_dbnull2(dr["empname"]) + ",申請單號：" + dbobj.get_dbnull2(dr["bsno"]) + "<br>";
                                }
                                tmpcomment = tmpcomment.Substring(0, tmpcomment.Length - 4);
                                tmpcomment += "的資料" + mpcount + "筆";
                            }
                            

                            //系統LOG檔
                            string sysnote = tmpcomment;
                            if (sysnote.Length > 4000) { sysnote.Substring(0, 4000); }
                            //================================================= //                  
                            string sysrealsid = Request["sysrealsid"].ToString();
                            SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                            string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2") + "(核銷)";
                            string sysflag = "M";
                            dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                            sysconn.Close();
                            sysconn.Dispose();
                            //====================================================== 
                            dr.Close();
                        }
                    }

                }

                return new ContentResult() { Content = @"<script>alert('核銷成功!!');</script>" + tmpform };
            }
        }

        [ActionName("Delete")]   //刪除  改成撤回
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

            string qblogstatus = "", qempname = "", qdptid = "", qblogsdate = "", qblogedate = "";
            if (!string.IsNullOrWhiteSpace(Request["qblogstatus"]))
            {
                qblogstatus = Request["qblogstatus"].Trim();
                ViewBag.qblogstatus = qblogstatus;
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
            if (!string.IsNullOrWhiteSpace(Request["qblogsdate"]))
            {
                qblogsdate = Request["qblogsdate"].Trim();
                ViewBag.qblogsdate = qblogsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["qblogedate"]))
            {
                qblogedate = Request["qblogedate"].Trim();
                ViewBag.qblogedate = qblogedate;
            }


            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/battadata/List' method='post'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

            tmpform += "<input type=hidden id='qblogstatus' name='qblogstatus' value='" + qblogstatus + "'>";
            tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
            tmpform += "<input type=hidden id='qdptid' name='qdptid' value='" + qdptid + "'>";
            tmpform += "<input type=hidden id='qblogsdate' name='qblogsdate' value='" + qblogsdate + "'>";
            tmpform += "<input type=hidden id='qblogedate' name='qblogedate' value='" + qblogedate + "'>";
            
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
                string sql = "select * from battalog";
                string sqlwhere = " where blogid in (" + cdel + ")";
                sql += sqlwhere;
                NDcommon dbobj = new NDcommon();
                using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                {
                    using (SqlConnection comconn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            string xblogstime = "", xblogetime = "";
                            string blogcomment = "", mailtitle = "", MailContext = "";
                            string fromadd = "", fromaddname = "", toadd = "";
                            string rolestampid = "", sql_m = "";
                            int bloghour = 0;


                            SqlDataReader dr = cmd.ExecuteReader();
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    xblogstime = "（" + dr["blogstime"] + "時）";
                                    xblogetime = "（" + dr["blogetime"] + "時）";

                                    #region  寄信(通知給目前簽核角色)

                                    if (dbobj.get_dbnull2(dr["blogcomment"]) != "")
                                    {
                                        blogcomment = dbobj.get_dbnull2(dr["blogcomment"]).Replace(Environment.NewLine, "<br>");
                                    }
                                    if (dbobj.get_dbnull2(dr["blogtype"]) == "1")
                                    {
                                        mailtitle = "出差單撤回通知";
                                    }
                                    else
                                    {
                                        mailtitle = "集體出差單撤回通知";
                                    }

                                    MailContext = "<HTML><HEAD><meta http-equiv='Content-Type' content='text/html; charset=UTF-8'></HEAD><body>";
                                    MailContext = MailContext + "以下為明細資料：<BR>";
                                    MailContext = MailContext + "<table cellpadding=3 cellspacing=0 bordercolorlight=#000000 bordercolordark=ffffff border=1 width=400 bgcolor=ffffff style='FONT-SIZE: 11pt;FONT-FAMILY:Tahoma,Arial'>";
                                    MailContext = MailContext + "<tr><td align=right width=130>申請人：</td><td>" + dbobj.get_dbnull2(dr["empname"]) + "</td></tr>";
                                    MailContext = MailContext + "<tr><td align=right>起迄日期：</td><td>自 " + dbobj.get_dbnull2(dr["blogsdate"]) + xblogstime + "<BR>至 " + dbobj.get_dbnull2(dr["blogedate"]) + xblogetime + "</td></tr>";

                                    bloghour = int.Parse("0" + dbobj.get_dbnull2(dr["bloghour"]));
                                    if (bloghour > 0) { bloghour = bloghour / 8; } else { bloghour = 0; }
                                    MailContext = MailContext + "<tr><td align=right>共計天數：</td><td>" + bloghour + "天</td></tr>";

                                    MailContext = MailContext + "<tr><td align=right>出差事由：</td><td>" + blogcomment + "+nbsp;</td></tr>";
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
                                        sql_m = "select enemail from viewemprole where rid in (" + rolestampid + ") and empstatus <> '4' and enemail<>''";
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

                                    sql = "UPDATE battalog SET blogstatus = 'D'";
                                    sql += sqlwhere;
                                    dbobj.dbexecute("Aitag_DBContext", sql);

                                    //'如果是集體的，細項狀態也update
                                    if(dbobj.get_dbnull2(dr["blogtype"]) == "2")
                                    {
                                        sql = "update battalog set blogstatus='D' where pbsno ='" + dbobj.get_dbnull2(dr["bsno"]) + "' and comid='" + (string)Session["comid"] + "'";
                                        dbobj.dbexecute("Aitag_DBContext", sql);
                                    }

                                    tmpcount++;
                                    tmpcomment += "姓名：" + dbobj.get_dbnull2(dr["empname"]) + "申請單號：" + dbobj.get_dbnull2(dr["bsno"]) + ",";
                                }
                                tmpcomment = tmpcomment.Substring(0, tmpcomment.Length -1);
                            }


                            //系統LOG檔
                            string sysnote = tmpcomment + "的資料" + tmpcount + "筆";
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
