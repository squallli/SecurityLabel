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
    public class battamoneyController : BaseController
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
            string qblogstatus = "", qempname = "", qdptid = "", qblogsdate = "", qblogedate = "", qifhdell="";
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
            if (!string.IsNullOrWhiteSpace(Request["qblogedqifhdellate"]))
            {
                qifhdell = Request["qifhdell"].Trim();
                ViewBag.qifhdell = qifhdell;
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
                        tmpform += "<form name='qfr1' action='/battamoney/List' method='post'>";
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

        public ActionResult battamoneyrpt(int? page, string orderdata, string orderdata1)
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
            qblogsdate = "2016/03/01";
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
                            chkitem = "select chkitem from checkcode where chkclass = '90' and chkcode = '" + dbobj.get_dbnull2(dr["blogaddr"]) + "'"; chkitem = dbobj.get_dbvalue(comconn, chkitem);
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
            string qblogstatus = "", qempname = "", qdptid = "", qblogsdate = "", qblogedate = "", qifhdell="";
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
            if (!string.IsNullOrWhiteSpace(Request["qblogedqifhdellate"]))
            {
                qifhdell = Request["qifhdell"].Trim();
                ViewBag.qifhdell = qifhdell;
            }

            qblogsdate = NullStDate(Request["qblogsdate"]);
            qblogsdate = "2016/06/01";
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
                if (qifhdell != "")
                {
                    sqlstr += " and ifhdell='" + qifhdell + "'";
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

        public ActionResult battamoneyprint(int? page, string orderdata, string orderdata1)
        {
            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            

            string qblogid = "";
            if (!string.IsNullOrWhiteSpace(Request["qblogid"]))
            {
                qblogid = Request["qblogid"].Trim();
            }

            string empname = "", blogcomment = "", blogaddr = "", exrate = "", comid = "", bbillcount="";
            DateTime blogsdate , blogedate ;
            string sql = "select * from battalog where blogid='" + qblogid + "' and comid='" + (string)Session["comid"] + "'";
            
            using (SqlConnection tconn = dbobj.get_conn("Aitag_DBContext"))
            {
                using (SqlCommand cmd = new SqlCommand(sql, tconn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    dr.Read();
                    empname = dr["empname"].ToString();
                    blogcomment = dr["blogcomment"].ToString();
                    blogaddr = dr["blogaddr"].ToString();
                    exrate = dr["exrate"].ToString();
                    comid = dr["comid"].ToString();
                    bbillcount = dr["bbillcount"].ToString();
                    blogsdate = Convert.ToDateTime(dr["blogsdate"]);
                    blogedate = Convert.ToDateTime(dr["blogedate"]);
                    
                    dr.Close();
                }
                tconn.Close();
                tconn.Dispose();
            }
            //參數
            string bdmonth = "", bdday = "", bdplace = "", bdwork = "", bdplane = "";
            string bdcar = "", bdtrain = "", bdship = "", bdliving1 = "", bdliving2 = "";
            string bdother = "", bdbillno = "", bdcomment = "", strsum = "";
            double dsum = 0, ttlmoney = 0;
            string bland = "", blive = "", bvisa = "", binsurance = "", badmin = "", bgift = "";
            int rowcounter = 0;

            string listtitle = "", Excel = "", Excel2 = "",sqlstr = "";
            int diffday = 0;
            using(SqlConnection conn = dbobj.get_conn("Aitag_DBContext")){
                //天數
                TimeSpan spantime = blogedate - blogsdate;
                diffday = spantime.Days + 1;
                //表單名稱
                string dpttitle = "select comtitle from Company where comid='" + comid + "' ";
                dpttitle = dbobj.get_dbvalue(conn, dpttitle);
                if (blogaddr == "1"){listtitle = dpttitle + "國內出差旅費報告表";}
                else { listtitle = dpttitle + "國外出差旅費報告表"; }
            

                #region 組 Excel 格式
                Excel += "<HTML>";
                Excel += "<HEAD>";
                Excel += @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">";
                Excel += "</HEAD>";
                Excel += "<body>";

                //表頭(國內國外一樣)
                Excel += "<div align=center><b><font style='font-size:18pt;font-family:標楷體'>" + listtitle + "</font></b></div>";
                Excel += "<table border=1  style='border-collapse:collapse;border:none;mso-border-alt:solid' borderColorDark=#000000 borderColorLight=#000000 cellpadding=6 cellspacing=0 width=100% align=center style='font-size:12pt;font-family:標楷體'>";
                Excel += "<tr>";
                Excel += "<td width=50>姓名</td>";
                Excel += "<td colspan=5>"+empname+"&nbsp;</td>";
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td>出差事由</td>";
                Excel += "<td colspan=5 width=500>" + blogcomment + "&nbsp;</td>";
                Excel += "</tr>";
                Excel += "</table>";

                //國內(blogaddr=1)國外(blogaddr<>1)不同
                sql = "select * from battadet where blogid=" + qblogid + " order by bddate";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        rowcounter++;
                        if (blogaddr == "1")
                        {//國內
                            bdmonth += "<td>" + dr["bdmonth"] + "&nbsp;</td>";
                            bdday += "<td>" + dr["bdday"] + "&nbsp;</td>";
                            bdplace += "<td>" + dr["bdplace"] + "&nbsp;</td>";
                            bdwork += "<td>" + dr["bdwork"] + "&nbsp;</td>";
                            bdplane += "<td>" + dr["bdplane"] + "&nbsp;</td>";
                            bdcar += "<td>" + dr["bdcar"] + "&nbsp;</td>";
                            bdtrain += "<td>" + dr["bdtrain"] + "&nbsp;</td>";
                            bdship += "<td>" + dr["bdship"] + "&nbsp;</td>";
                            bdliving1 += "<td>" + dr["bdliving1"] + "&nbsp;</td>";
                            bdliving2 += "<td>" + dr["bdliving2"] + "&nbsp;</td>";
                            bdother += "<td>" + dr["bdother"] + "&nbsp;</td>";
                            bdbillno += "<td>" + dr["bdbillno"] + "&nbsp;</td>";
                            bdcomment += "<td>" + dr["bdcomment"] + "&nbsp;</td>";

                            dsum = Convert.ToDouble(dr["bdplane"]) + Convert.ToDouble(dr["bdcar"]) + Convert.ToDouble(dr["bdtrain"]) + Convert.ToDouble(dr["bdship"]) + Convert.ToDouble(dr["bdliving1"]) + Convert.ToDouble(dr["bdliving2"]) + Convert.ToDouble(dr["bdother"]);
                            strsum += "<td>" + dsum.ToString() + "&nbsp;</td>";
                            ttlmoney += dsum;
                        }
                        else
                        {//國外
                            bdmonth += "<td>" + dr["bdmonth"] + "&nbsp;</td>";
                            bdday += "<td>" + dr["bdday"] + "&nbsp;</td>";
                            bdplace += "<td>" + dr["bdplace"] + "&nbsp;</td>";
                            bdwork += "<td>" + dr["bdwork"] + "&nbsp;</td>";
                            bdplane += "<td>" + dr["bdplane"] + "&nbsp;</td>";
                            bdship += "<td>" + dr["bdship"] + "&nbsp;</td>";
                            bland += "<td>" + dr["bland"] + "&nbsp;</td>";
                            blive += "<td>" + dr["blive"] + "&nbsp;</td>";
                            bvisa += "<td>" + dr["bvisa"] + "&nbsp;</td>";
                            binsurance += "<td>" + dr["binsurance"] + "&nbsp;</td>";
                            badmin += "<td>" + dr["badmin"] + "&nbsp;</td>";
                            bgift += "<td>" + dr["bgift"] + "&nbsp;</td>";
                            bdother += "<td>" + dr["bdother"] + "&nbsp;</td>";
                            bdbillno += "<td>" + dr["bdbillno"] + "&nbsp;</td>";
                            bdcomment += "<td>" + dr["bdcomment"] + "&nbsp;</td>";

                            dsum = Convert.ToDouble(dr["bdplane"]) + Convert.ToDouble(dr["bdship"]) + Convert.ToDouble(dr["bland"]) + Convert.ToDouble(dr["blive"]) + Convert.ToDouble(exrate);
                            dsum += Convert.ToDouble(dr["bvisa"]) + Convert.ToDouble(dr["binsurance"]) + Convert.ToDouble(dr["badmin"]) + Convert.ToDouble(dr["bgift"]) + Convert.ToDouble(dr["bdother"]);
                            strsum += "<td>" + dsum.ToString() + "&nbsp;</td>";
                            ttlmoney += dsum;
                        }
                    }
                    dr.Close();
                }
                
            }
            int emptyrow = 5 - rowcounter;
            if (blogaddr == "1")
            {
                Excel += "<table border=1 style='border-collapse:collapse;border:none;mso-border-alt:solid' borderColorDark=#000000 borderColorLight=#000000 cellpadding=6 cellspacing=0 width=100% align=center style='font-size:12pt;font-family:標楷體'><tr align=center>";
                Excel += "<tr align=center>";
                Excel += "<td colspan=" + (rowcounter + 2 )+ ">中華民國&nbsp;&nbsp;" + (blogsdate.Year - 1911) + "&nbsp;&nbsp;年&nbsp;&nbsp;" + blogsdate.Month.ToString() + "&nbsp;&nbsp;月&nbsp;&nbsp;" + blogsdate.Day.ToString() + "&nbsp;&nbsp;日起中華民國&nbsp;&nbsp;" + (blogedate.Year - 1911) + "&nbsp;&nbsp;年&nbsp;&nbsp;" + blogedate.Month.ToString() + "&nbsp;&nbsp;月&nbsp;&nbsp;" + blogedate.Day.ToString() + "&nbsp;&nbsp;日止共計&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+diffday+"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;日附單據&nbsp;&nbsp;" + bbillcount + "&nbsp;&nbsp;張</td>";
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td colspan=2 align=center>月</td>";
                Excel += bdmonth + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td colspan=2 align=center>日</td>";
                Excel += bdday + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td colspan=2 align=center>起訖地點</td>";
                Excel += bdplace + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td colspan=2 align=center>工作記要</td>";
                Excel += bdwork + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td rowspan=4 width=10%>交通費</td>";
                Excel += "<td width=15%>飛機及高鐵</td>";
                Excel += bdplane + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td>汽車及捷運</td>";
                Excel += bdcar + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td>火車</td>";
                Excel += bdtrain + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td>輪船</td>";
                Excel += bdship + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td colspan=2 >住宿費</td>";
                Excel += bdliving1 + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td colspan=2 >住宿費加計交通費<br>(旅行業代收轉付)</td>";
                Excel += bdliving2 + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td colspan=2 >膳雜費</td>";
                Excel += bdother + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td colspan=2 >單據號數</td>";
                Excel += bdbillno + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td colspan=2 >小計</td>";
                Excel += strsum + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td colspan=2 >總計</td>";
                Excel += "<td colspan=5 >" + ttlmoney + "</td>";
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td colspan=2 >備註</td>";
                Excel += bdcomment + add_emptytd(emptyrow);
                Excel += "</tr>";

                Excel += "</table>";
                Excel += "<p>";

                Excel += "<table width=100% style='font-size:12pt;font-family:標楷體'>";
                Excel += "<tr>";
                Excel += "<td valign=top>出差人</td>";
                Excel += "<td>單位<br>主管</td>";
                Excel += "<td>主辦人<br>事人員</td>";
                Excel += "<td>主辦會<br>計人員</td>";
                Excel += "<td>機關首長或<br>授權代簽人</td>";
                Excel += "</tr>";
                Excel += "</table>";
            }
            else
            {
                Excel += "<table border=1 style='border-collapse:collapse;border:none;mso-border-alt:solid' borderColorDark=#000000 borderColorLight=#000000 cellpadding=6 cellspacing=0 width=100% align=center style='font-size:12pt;font-family:標楷體'><tr align=center>";
                Excel += "<tr align=center>";
                Excel += "<td colspan=" + (rowcounter + 2) + ">中華民國&nbsp;&nbsp;" + (blogsdate.Year - 1911) + "&nbsp;&nbsp;年&nbsp;&nbsp;" + blogsdate.Month.ToString() + "&nbsp;&nbsp;月&nbsp;&nbsp;" + blogsdate.Day.ToString() + "&nbsp;&nbsp;日起中華民國&nbsp;&nbsp;" + (blogedate.Year - 1911) + "&nbsp;&nbsp;年&nbsp;&nbsp;" + blogedate.Month.ToString() + "&nbsp;&nbsp;月&nbsp;&nbsp;" + blogedate.Day.ToString() + "&nbsp;&nbsp;日止共計&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + diffday + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;日附單據&nbsp;&nbsp;" + bbillcount + "&nbsp;&nbsp;張</td>";
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td colspan=2 align=center>月</td>";
                Excel += bdmonth + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td colspan=2 align=center>日</td>";
                Excel += bdday + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td colspan=2 align=center>起訖地點</td>";
                Excel += bdplace + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td colspan=2 align=center>工作記要</td>";
                Excel += bdwork + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td rowspan=3 width=10%>交通費</td>";
                Excel += "<td width=15%>飛機</td>";
                Excel += bdplane + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td>船舶</td>";
                Excel += bdship + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td>長途大眾<Br>陸運工具</td>";
                Excel += bland + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td colspan=2>生活費(US$)</td>";
                Excel += blive + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td rowspan=5 width=10%>辦公費</td>";
                Excel += "<td width=15%>手續費</td>";
                Excel += bvisa + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td>保險費</td>";
                Excel += binsurance + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td>行政費</td>";
                Excel += badmin + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td>禮品及<br>交際費</td>";
                Excel += bgift + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td>雜費</td>";
                Excel += bdother + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td colspan=2>單據號數</td>";
                Excel += bdbillno + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td colspan=2>小計</td>";
                Excel += strsum + add_emptytd(emptyrow);
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td colspan=2 >總計</td>";
                Excel += "<td colspan=5 >" + ttlmoney + "</td>";
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td colspan=2 >備註</td>";
                Excel += bdcomment + add_emptytd(emptyrow);
                Excel += "</tr>";

                Excel += "</table>";
                Excel += "<p>";

                Excel += "<table width=100% style='font-size:12pt;font-family:標楷體'>";
                Excel += "<tr>";
                Excel += "<td valign=top>出差人</td>";
                Excel += "<td>單位<br>主管</td>";
                Excel += "<td>主辦人<br>事人員</td>";
                Excel += "<td>主辦會<br>計人員</td>";
                Excel += "<td>機關首長或<br>授權代簽人</td>";
                Excel += "</tr>";
                Excel += "</table>";
                Excel += "<br>";

                Excel += "<table width=80% style='font-size:12pt;font-family:標楷體' border=0 align=center>";
                Excel += "<tr>";
                Excel += "<td valign=top width=50%>茲　　收　　到</td>";
                Excel += "<td width=50%></td>";
                Excel += "</tr>";
                Excel += "<tr>";
                Excel += "<td>出差旅費新台幣&nbsp;" + ttlmoney + "元整</td>";
                Excel += "<td>具領人　　　　　　　　　　(簽章)</td>";
                Excel += "</tr>";
                Excel += "</table>";
            }
            

            #endregion
            Excel += "</table>";
            Excel += "</body>";
            Excel += "</HTML>";

            ViewBag.Excel = Excel;
            return View();
        }

        //補空格function
        private string add_emptytd(int number)
        {
            string addtd = "";
            for (int i = number; i <= 5; i++)
            {
                addtd += "<td>&nbsp;</td>";
            }
            return addtd;
        }



    }
}
