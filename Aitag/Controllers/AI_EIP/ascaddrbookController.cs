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
    public class ascaddrbookController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /employee/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }
        public ActionResult ascaddrbookcsv(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "empname"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qempname = "", qdptid = "";

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

            //IPagedList<employee> result;
            string Excel = "", Excel2 = "";
            string sqlstr = "";
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                sqlstr = "select * from employee where (ifuse <> 'n' or ifuse is null)  and emptype <> '3' and empstatus <> '4' and empworkcomp = '" + (string)(Session["comid"]) + "'  and";

                if (qempname != "")
                {
                    sqlstr += " (empname like N'%" + qempname + "%' or empenname like N'%" + qempname + "%')  and";
                }
                if (qdptid != "")
                {
                    sqlstr += " empworkdepid ='" + qdptid + "'  and";
                }
                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;
                //var query = con.employee.SqlQuery(sqlstr).AsQueryable();
                //result = query.ToPagedList<employee>(page.Value - 1, (int)Session["pagesize"]);
            }
            Excel += "<HTML>";
            Excel += "<HEAD>";
            Excel += @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">";
            Excel += "</HEAD>";
            Excel += "<body>";
            Excel += "<table border=1  cellpadding=0 cellspacing=0 bordercolor=#000000 bordercolordark=#ffffff width=900>";
            Excel += "<tr align=center>";
            Excel += "<td>名字</td>";
            Excel += "<td>姓氏</td>";
            Excel += "<td>中間名</td>";
            Excel += "<td>名稱</td>";
            Excel += "<td>暱稱</td>";
            Excel += "<td>電子郵件地址</td>";
            Excel += "<td>住家所在街道</td>";
            Excel += "<td>住家所在縣/市</td>";
            Excel += "<td>住家所在郵遞區號</td>";
            Excel += "<td>住家所在省/市</td>";
            Excel += "<td>住家所在國家/地區</td>";
            Excel += "<td>住家電話</td>";
            Excel += "<td>住家傳真</td>";
            Excel += "<td>個人網頁</td>";
            Excel += "<td>公司所在街道</td>";
            Excel += "<td>公司所在縣/市</td>";
            Excel += "<td>公司所在郵遞區號</td>";
            Excel += "<td>公司所在省/市</td>";
            Excel += "<td>公司所在國家/地區</td>";
            Excel += "<td>公司網頁</td>";
            Excel += "<td>公司電話</td>";
            Excel += "<td>公司傳真</td>";
            Excel += "<td>呼叫器</td>";
            Excel += "<td>公司</td>";
            Excel += "<td>職稱</td>";
            Excel += "<td>部門</td>";
            Excel += "<td>辦公室位置</td>";
            Excel += "<td>異動時間</td>";
            Excel += "<td>備註</td>";
            Excel += "</tr>";
            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                using (SqlCommand cmd = new SqlCommand(sqlstr, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    string tmpname = "";
                        while (dr.Read())
                        {
                            tmpname = dbobj.get_dbvalue(conn, "select dpttitle from Department where dptid = '" + dr["empworkdepid"] + "' and comid='" + dr["empworkcomp"] + "'");
                            
                            Excel2 += "<tr>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "<td>" + dr["empname"] + "</td>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "<td>" + dr["enemail"] + "&nbsp;</td>";
                            Excel2 += "<td>" + dr["enaddress"] + "&nbsp;</td>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "<td>" + dr["entel"] + "&nbsp;</td>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "<td>" + dr["enmob"] + "&nbsp;</td>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "<td>" + tmpname + "&nbsp;</td>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "<td>&nbsp;</td>";
                            Excel2 += "</tr>";
                        }
                    if(Excel2 ==""){
                        Excel += "<tr align=left><td colspan=7>目前沒有資料</td></tr>";
                    }else{
                        Excel += Excel2;
                    }

                    dr.Close();
                }
            }
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
            { orderdata = "empname"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qempname = "", qdptid = "";

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

            IPagedList<employee> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from employee where (ifuse <> 'n' or ifuse is null)  and emptype <> '3' and empstatus <> '4' and empworkcomp = '" + (string)(Session["comid"]) + "'  and";

                if (qempname != "")
                {
                    sqlstr += " (empname like N'%" + qempname + "%' or empenname like N'%" + qempname + "%')  and";
                }
                if (qdptid != "")
                {
                    sqlstr += " empworkdepid ='" + qdptid + "'  and";
                }

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.employee.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<employee>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch(orderdata, orderdata1);
            return View(result);
        }
        private string SetOrder_ch(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "empid", "empname", "empenname", "empworkdepid", "enemail" };
            string[] od_ch1 = { "asc", "asc", "asc", "asc", "asc" };
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



    }
}
