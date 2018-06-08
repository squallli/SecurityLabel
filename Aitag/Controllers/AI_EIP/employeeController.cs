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
    public class employeeController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /employee/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.emid = Session["emid"].ToString();
        //    employee col = new employee();
        //    return View(col);
        //}

        //[HttpPost]
        public ActionResult add(employee col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "emid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qempworkcomp = "", qempworkdepid = "", qempstatus = "", qempno = "", qempid = "", qempname = "";


            if (!string.IsNullOrWhiteSpace(Request["qempworkcomp"]))
            {
                qempworkcomp = Request["qempworkcomp"].Trim();
                ViewBag.qempworkcomp = qempworkcomp;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempworkdepid"]))
            {
                qempworkdepid = Request["qempworkdepid"].Trim();
                ViewBag.qempworkdepid = qempworkdepid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempstatus"]))
            {
                qempstatus = Request["qempstatus"].Trim();
                ViewBag.qempstatus = qempstatus;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempno"]))
            {
                qempno = Request["qempno"].Trim();
                ViewBag.qempno = qempno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {
                qempid = Request["qempid"].Trim();
                ViewBag.qempid = qempid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempname"]))
            {
                qempname = Request["qempname"].Trim();
                ViewBag.qempname = qempname;
            }



            if (sysflag != "A")
            {
                employee newcol = new employee();
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
                    SqlConnection conn = dbobj.get_conn("Aitag_DBContext");
                    SqlDataReader dr;
                    SqlCommand sqlsmd = new SqlCommand();
                    sqlsmd.Connection = conn;
                    string sqlstr = "select emid from employee where emid = '" + col.emid + "'";
                    sqlsmd.CommandText = sqlstr;
                    dr = sqlsmd.ExecuteReader();

                    if (dr.Read())
                    {

                        ModelState.AddModelError("", "權限代碼重複!");
                        return View(col);
                    }
                    dr.Close();
                    dr.Dispose();
                    sqlsmd.Dispose();
                    conn.Close();
                    conn.Dispose();
                    col.comcon = Request["comcon"];
                    col.hsalary = 0;
                    col.bmodid = Session["tempid"].ToString();
                    col.bmoddate = DateTime.Now;
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.employee.Add(col);
                        con.SaveChanges();
                    
                        

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "代碼:" + col.emid + "名稱:" + col.empname;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/employee/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

                    tmpform += "<input type=hidden id='qempworkcomp' name='qempworkcomp' value='" + qempworkcomp + "'>";
                    tmpform += "<input type=hidden id='qempworkdepid' name='qempworkdepid' value='" + qempworkdepid + "'>";
                    tmpform += "<input type=hidden id='qempstatus' name='qempstatus' value='" + qempstatus + "'>";
                    tmpform += "<input type=hidden id='qempno' name='qempno' value='" + qempno + "'>";
                    tmpform += "<input type=hidden id='qempid' name='qempid' value='" + qempid + "'>";
                    tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";

                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    // return RedirectToAction("List");

                }
            }


        }


        [ValidateInput(false)]
        public ActionResult Edit(employee chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "emid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qempworkcomp = "", qempworkdepid = "", qempstatus = "", qempno = "", qempid = "", qempname = "";


            if (!string.IsNullOrWhiteSpace(Request["qempworkcomp"]))
            {
                qempworkcomp = Request["qempworkcomp"].Trim();
                ViewBag.qempworkcomp = qempworkcomp;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempworkdepid"]))
            {
                qempworkdepid = Request["qempworkdepid"].Trim();
                ViewBag.qempworkdepid = qempworkdepid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempstatus"]))
            {
                qempstatus = Request["qempstatus"].Trim();
                ViewBag.qempstatus = qempstatus;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempno"]))
            {
                qempno = Request["qempno"].Trim();
                ViewBag.qempno = qempno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {
                qempid = Request["qempid"].Trim();
                ViewBag.qempid = qempid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempname"]))
            {
                qempname = Request["qempname"].Trim();
                ViewBag.qempname = qempname;
            }

            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.employee.Where(r => r.emid == chks.emid).FirstOrDefault();
                    employee eemployees = con.employee.Find(chks.emid);
                    if (eemployees == null)
                    {
                        return HttpNotFound();
                    }
                    //組生日下拉
                    string datetime1 = "";
                    if (eemployees.empbirth==null)
                    {
                        datetime1 = "1900/1/1";
                    }
                    else
                    {
                        datetime1 = eemployees.empbirth.ToString();
                    }
                    ViewBag.BirthSelect = BirthSelect(datetime1);

                    return View(eemployees);
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

                    //string oldemid = Request["oldemid"];                 

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        NDcommon dbobj = new NDcommon();
                        chks.comcon = Request["comcon"];
                        chks.emid = int.Parse(Request["emid"]);
                        chks.bmodid = Session["tempid"].ToString();
                        chks.bmoddate = DateTime.Now;
                        con.Entry(chks).State = EntityState.Modified;
                        con.SaveChanges();


                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "代碼:" + chks.emid + "名稱:" + chks.empname;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/employee/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

                    tmpform += "<input type=hidden id='qempworkcomp' name='qempworkcomp' value='" + qempworkcomp + "'>";
                    tmpform += "<input type=hidden id='qempworkdepid' name='qempworkdepid' value='" + qempworkdepid + "'>";
                    tmpform += "<input type=hidden id='qempstatus' name='qempstatus' value='" + qempstatus + "'>";
                    tmpform += "<input type=hidden id='qempno' name='qempno' value='" + qempno + "'>";
                    tmpform += "<input type=hidden id='qempid' name='qempid' value='" + qempid + "'>";
                    tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";

                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    //return RedirectToAction("List");
                }
            }

        }
        private string BirthSelect(string Preset)
        {
            //已使用 byear bmonth bday ，其他元件不能重複!!
            Preset += ' ';
            Preset = Preset.Split(' ')[0];
            int byear = int.Parse(Preset.Split('/')[0]);
            int bmonth = int.Parse(Preset.Split('/')[1]);
            int bday = int.Parse(Preset.Split('/')[2]);

            string selected = "";
            //組下拉 年分
            string BirthSelect = @"<select name=""byear""><option value =''>----</option>";
            DateTime localDate = DateTime.Now;
            for (var i = localDate.Year; i >= 1920; i--)
            {
                if (i == byear) { selected = "selected='selected'"; } else { selected = ""; }
                BirthSelect += "<option value ='" + i + "' " + selected + " >" + i + "</option>";
            }
            BirthSelect += "</select>";
            //組下拉 月份
            BirthSelect += @"<select name=""bmonth""><option value =''>--</option>";
            for (var i = 1; i <= 12; i++)
            {
                if (i == bmonth) { selected = "selected='selected'"; } else { selected = ""; }
                BirthSelect += "<option value ='" + i + "' " + selected + " >" + i + "</option>";
            }
            BirthSelect += "</select>";
            //組下拉 日期
            bmonth += 1;
            if (bmonth == 13)
            {
                byear += 1;
                bmonth = 1;
            }
            DateTime d = new DateTime(byear, bmonth, 1);
            long t = d.Ticks - 1000 * 60 * 60 * 24;
            DateTime d2 = new DateTime(t);
            int maxDay = d2.Day;
    
            BirthSelect += @"<select name=""bday""><option value =''>--</option>";
            for (var i = 1; i <= maxDay; i++)
            {
                if (i == bday) { selected = "selected='selected'"; } else { selected = ""; }
                BirthSelect += "<option value ='" + i + "' " + selected + " >" + i + "</option>";
            }
            BirthSelect += "</select>";


            BirthSelect += @"<script>";
            BirthSelect += @"function SelectPreset(e) { if ("""" != e && ""undefined"" != typeof e) { e += "" "", e = e.split("" "")[0]; var t = e.split(""/"")[0], a = e.split(""/"")[1], n = e.split(""/"")[2]; $(""select[name='byear']"").val(t), $(""select[name='bmonth']"").val(a), Setbday(""bday""), $(""select[name='bday']"").val(n) } else $(""select[name='byear']"")[0].selectedIndex = 0, $(""select[name='bmonth']"")[0].selectedIndex = 0, Setbday(""bday""), $(""select[name='bday']"")[0].selectedIndex = 0 }";
            BirthSelect += @"function SetbirthDay(e) { var t = $(""select[name='byear']"").find("":selected"").val(), a = $(""select[name='bmonth']"").find("":selected"").val(), n = $(""select[name='bday']"").find("":selected"").val(); $(""input[name='"" + e + ""']"").val(t + ""/"" + a + ""/"" + n) }";
            BirthSelect += @"function Setbday(e) { var t = 0, a = $(""select[name='byear']"").find("":selected"").val(), n = 1 + parseInt($(""select[name='bmonth']"").find("":selected"").val()); 13 == n && (a = parseInt(a) + 1, n = 1); var l = new Date(a + ""/"" + n + ""/1""), d = l.getTime() - 864e5, t = new Date(d).getDate(); 0 == t && (t = 31), $(""select[name='"" + e + ""'] option"").remove(), optionAdd(""bday"", """", ""--""); for (var c = 1; t >= c; c++) optionAdd(e, c, c) }";
            BirthSelect += @"function optionAdd(e, t, a) { $(""select[name='"" + e + ""']"").append($(""<option></option>"").attr(""value"", t).text(a)) }";
            BirthSelect += @"function SetempbirthSelect() { $(""select[name='byear']"").change(function () { Setbday(""bday""), SetbirthDay(""empbirth"") }), $(""select[name='bmonth']"").change(function () { Setbday(""bday""), SetbirthDay(""empbirth"") }), $(""select[name='bday']"").change(function () { SetbirthDay(""empbirth"") }) }";
            BirthSelect += @"</script>";
            return BirthSelect;
        }


        public ActionResult List(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "emid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qempworkcomp = "", qempworkdepid = "", qempstatus = "", qempno = "", qempid = "", qempname = "", qrid = "";


            if (!string.IsNullOrWhiteSpace(Request["qempworkcomp"]))
            {
                qempworkcomp = Request["qempworkcomp"].Trim();
                ViewBag.qempworkcomp = qempworkcomp;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempworkdepid"]))
            {
                qempworkdepid = Request["qempworkdepid"].Trim();
                ViewBag.qempworkdepid = qempworkdepid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempstatus"]))
            {
                qempstatus = Request["qempstatus"].Trim();
                ViewBag.qempstatus = qempstatus;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempno"]))
            {
                qempno = Request["qempno"].Trim();
                ViewBag.qempno = qempno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {
                qempid = Request["qempid"].Trim();
                ViewBag.qempid = qempid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempname"]))
            {
                qempname = Request["qempname"].Trim();
                ViewBag.qempname = qempname;
            }

            if (!string.IsNullOrWhiteSpace(Request["qrid"]))
            {
                qrid = Request["qrid"].Trim();
                ViewBag.qrid = qrid;
            }

            IPagedList<employee> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqlstr = "select * from employee where";


                if (qempworkcomp != "")
                    sqlstr += " empworkcomp = '" + qempworkcomp + "'  and";
                if (qempworkdepid != "")
                    sqlstr += " empworkdepid = '" + qempworkdepid + "'  and";
                if (qempstatus != "")
                    sqlstr += " empstatus = '" + qempstatus + "'  and";
                if (qempno != "")
                    sqlstr += " empno like '%" + qempno + "%'  and";
                if (qempid != "")
                    sqlstr += " empid like '%" + qempid + "%'  and";
                if (qempname != "")
                    sqlstr += " empname like N'%" + qempname + "%'  and";
                if (qrid != "")
                    sqlstr += " empid in (select distinct empid from emprole where rid = '" + qrid + "')  and";

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
            string[] od_ch = { "empno", "empid" };
            string[] od_ch1 = { "asc", "asc" };
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





        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string orderdata = "", orderdata1 = "";
            //if (!string.IsNullOrWhiteSpace(Request["sysflag"]))
            //{
            //    sysflag = Request["sysflag"].Trim();
            //}
            if (!string.IsNullOrWhiteSpace(Request["orderdata"]))
            {
                orderdata = Request["orderdata"].Trim();
            }
            if (!string.IsNullOrWhiteSpace(Request["orderdata1"]))
            {
                orderdata1 = Request["orderdata1"].Trim();
            }


            string qempworkcomp = "", qempworkdepid = "", qempstatus = "", qempno = "", qempid = "", qempname = "";
            if (!string.IsNullOrWhiteSpace(Request["qempworkcomp"]))
            {
                qempworkcomp = Request["qempworkcomp"].Trim();
                //ViewBag.qempworkcomp = qempworkcomp;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempworkdepid"]))
            {
                qempworkdepid = Request["qempworkdepid"].Trim();
                //ViewBag.qempworkdepid = qempworkdepid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempstatus"]))
            {
                qempstatus = Request["qempstatus"].Trim();
                //ViewBag.qempstatus = qempstatus;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempno"]))
            {
                qempno = Request["qempno"].Trim();
                //ViewBag.qempno = qempno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {
                qempid = Request["qempid"].Trim();
                //ViewBag.qempid = qempid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempname"]))
            {
                qempname = Request["qempname"].Trim();
                //ViewBag.qempname = qempname;
            }
            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/employee/List' method='post'>";
            //tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

            tmpform += "<input type=hidden id='qempworkcomp' name='qempworkcomp' value='" + qempworkcomp + "'>";
            tmpform += "<input type=hidden id='qempworkdepid' name='qempworkdepid' value='" + qempworkdepid + "'>";
            tmpform += "<input type=hidden id='qempstatus' name='qempstatus' value='" + qempstatus + "'>";
            tmpform += "<input type=hidden id='qempno' name='qempno' value='" + qempno + "'>";
            tmpform += "<input type=hidden id='qempid' name='qempid' value='" + qempid + "'>";
            tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";

            tmpform += "</form>";
            tmpform += "</body>";


            string cdel = Request["cdel"];
            if (string.IsNullOrWhiteSpace(cdel))
            {
                return new ContentResult() { Content = @"<script>alert('請勾選要刪除的資料!!');</script>" + tmpform };
            }
            else
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {

                    NDcommon dbobj = new NDcommon();
                    SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext");
                    string sysnote = "";
                    string[] condtionArr = cdel.Split(',');
                    int condtionLen = condtionArr.Length;
                    for (int i = 0; i < condtionLen; i++)
                    {
                        string eemployees = dbobj.get_dbvalue(conn1, "select empname from employee where emid ='" + condtionArr[i].ToString() + "'");

                        sysnote += "代碼名稱:" + eemployees + "，序號:" + condtionArr[i].ToString() + "<br>";

                        dbobj.dbexecute("Aitag_DBContext", "DELETE FROM employee where emid = '" + condtionArr[i].ToString() + "'");
                     
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



                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');</script>" + tmpform };
                    //return RedirectToAction("List");
                }
            }
        }


        public ActionResult perservice(employee chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "emid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            List<employee> result;
            if (sysflag != "E")
            {
                
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    string sqlstr = "select * from employee where empid = '" + (string)Session["empid"] + "'";
                    var query = con.employee.SqlQuery(sqlstr).AsQueryable();
                    result = query.ToList();
                    
                    /*var data = con.employee.Where(r => r.empid == (string)Session["empid"]).FirstOrDefault();
                    employee eemployees = con.employee.Find((string)Session["empid"]);
                    if (eemployees == null)
                    {
                      return HttpNotFound();
                    }*/

                    return View(result);
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

                    //string oldemid = Request["oldemid"];

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();

                        string sql = "update employee set ";
                        sql += "epagesize = '" + Request["epagesize"] + "', ";
                        sql += "emppasswd = '" + Request["emppasswd"] + "', ";
                        sql += "eicon = '" + Request["eicon"] + "', ";
                        sql += "etab = '" + Request["etab"] + "', ";
                        sql += "bmodid = '" + (string)Session["empid"] + "', ";
                        sql += "bmoddate = '" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "' ";
                        sql += "where empid = '" + (string)Session["empid"] + "' ";

                        dbobj.dbexecute("Aitag_DBContext", sql);

                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "員工編號：" + (string)Session["empid"] + "的個人資料";
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/employee/perservice' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value=''>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    //return RedirectToAction("List");
                }
            }

        }


        public ActionResult personal(employee chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "emid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;

            List<employee> result;
            if (sysflag != "E")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    string sqlstr = "select * from employee where empid = '" + (string)Session["empid"] + "'";
                    var query = con.employee.SqlQuery(sqlstr).AsQueryable();
                    result = query.ToList();

                    return View(result);
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

                    //string oldemid = Request["oldemid"];
                    string empbirth = Request["byear"] + "/" + Request["bmonth"] + "/" + Request["bday"];
                    try
                    {
                        DateTime.Parse(empbirth);
                    }
                    catch
                    {
                        empbirth = "";
                    }

                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();

                        string sql = "update employee set ";
                        sql += "emppasswd = '" + Request["emppasswd"] + "', ";
                        sql += "empname = '" + Request["empname"] + "', ";
                        sql += "empenname = '" + Request["empenname"] + "', ";
                        if (empbirth != "") { sql += "empbirth = '" + empbirth + "', "; }
                        sql += "empsex = '" + Request["empsex"] + "', ";
                        sql += "eaddress = '" + Request["eaddress"] + "', ";
                        sql += "enaddress = '" + Request["enaddress"] + "', ";
                        sql += "entel = '" + Request["entel"] + "', ";
                        sql += "enmob = '" + Request["enmob"] + "', ";
                        sql += "enemail = '" + Request["enemail"] + "', ";
                        sql += "empcomment = '" + Request["empcomment"] + "', ";
                        sql += "bmodid = '" + (string)Session["empid"] + "', ";
                        sql += "bmoddate = '" + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + "' ";
                        sql += "where empid = '" + (string)Session["empid"] + "' ";

                        dbobj.dbexecute("Aitag_DBContext", sql);

                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "員工編號：" + (string)Session["empid"] + "的個人資料";
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/employee/personal' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value=''>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    //return RedirectToAction("List");
                }
            }

        }

        public ActionResult emproleadd(emprole col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "cid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string emid = "", qempworkcomp = "", qempworkdepid = "", qempstatus = "", qempno = "", qempid = "", qempname = "";
            if (!string.IsNullOrWhiteSpace(Request["emid"]))
            {
                emid = Request["emid"].Trim();
                ViewBag.emid = emid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempworkcomp"]))
            {
                qempworkcomp = Request["qempworkcomp"].Trim();
                ViewBag.qempworkcomp = qempworkcomp;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempworkdepid"]))
            {
                qempworkdepid = Request["qempworkdepid"].Trim();
                ViewBag.qempworkdepid = qempworkdepid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempstatus"]))
            {
                qempstatus = Request["qempstatus"].Trim();
                ViewBag.qempstatus = qempstatus;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempno"]))
            {
                qempno = Request["qempno"].Trim();
                ViewBag.qempno = qempno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {
                qempid = Request["qempid"].Trim();
                ViewBag.qempid = qempid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempname"]))
            {
                qempname = Request["qempname"].Trim();
                ViewBag.qempname = qempname;
            }

            if (sysflag != "A")
            {

                return View(col);
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
                    string empid = "", empworkcomp = "", tmpname = "", rid = "";
                    rid = Request["rid"].Trim();
                    using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        empid = dbobj.get_dbvalue(conn, "select empid from employee where emid = '" + emid + "'");
                        empworkcomp = dbobj.get_dbvalue(conn, "select empworkcomp from employee where emid = '" + emid + "'");
                        tmpname = dbobj.get_dbvalue(conn, "select roletitle from roleplay where rid = '" + rid + "' and comid = '" + empworkcomp + "'");//系統LOG 用

                        string sql = "select * from emprole where empid='"
                            + empid
                            + "' and comid='"
                            + empworkcomp
                            + "' and rid = '"
                            + rid
                            + "'";

                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            SqlDataReader dr = cmd.ExecuteReader();
                            if (dr.HasRows)
                            {
                                ModelState.AddModelError("", "此角色已經安排，請重新輸入!");
                                return View(col);
                            }
                        }

                    }


                    col.empid = empid;
                    col.rid = rid;
                    col.comid = empworkcomp;

                    col.bmodid = Session["tempid"].ToString();
                    col.bmoddate = DateTime.Now;
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {
                        con.emprole.Add(col);
                        con.SaveChanges();


                        

                        //系統LOG檔
                        string sysnote = "員工編號：" + empid + "<br>組織角色：" + tmpname + "的資料 ";
                        if (sysnote.Length > 4000) { sysnote = sysnote.Substring(0, 4000); }
                        //================================================= // 
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/employee/Edit' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='emid' name='emid' value='" + emid + "'>";
                    tmpform += "<input type=hidden id='qempworkcomp' name='qempworkcomp' value='" + qempworkcomp + "'>";
                    tmpform += "<input type=hidden id='qempworkdepid' name='qempworkdepid' value='" + qempworkdepid + "'>";
                    tmpform += "<input type=hidden id='qempstatus' name='qempstatus' value='" + qempstatus + "'>";
                    tmpform += "<input type=hidden id='qempno' name='qempno' value='" + qempno + "'>";
                    tmpform += "<input type=hidden id='qempid' name='qempid' value='" + qempid + "'>";
                    tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    // return RedirectToAction("List");

                }
            }


        }

        public ActionResult emprolemod(emprole col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "cid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string emid = "", qempworkcomp = "", qempworkdepid = "", qempstatus = "", qempno = "", qempid = "", qempname = "";
            if (!string.IsNullOrWhiteSpace(Request["emid"]))
            {
                emid = Request["emid"].Trim();
                ViewBag.emid = emid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempworkcomp"]))
            {
                qempworkcomp = Request["qempworkcomp"].Trim();
                ViewBag.qempworkcomp = qempworkcomp;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempworkdepid"]))
            {
                qempworkdepid = Request["qempworkdepid"].Trim();
                ViewBag.qempworkdepid = qempworkdepid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempstatus"]))
            {
                qempstatus = Request["qempstatus"].Trim();
                ViewBag.qempstatus = qempstatus;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempno"]))
            {
                qempno = Request["qempno"].Trim();
                ViewBag.qempno = qempno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {
                qempid = Request["qempid"].Trim();
                ViewBag.qempid = qempid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempname"]))
            {
                qempname = Request["qempname"].Trim();
                ViewBag.qempname = qempname;
            }

            ViewBag.erid = col.erid;

            if (sysflag != "M")
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    var data = con.emprole.Where(r => r.erid == col.erid).FirstOrDefault();
                    emprole eemproles = con.emprole.Find(col.erid);
                    if (eemproles == null)
                    {
                        return HttpNotFound();
                    }


                    return View(eemproles);

                }
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
                    string empid = "", empworkcomp = "", tmpname = "", rid = "";
                    rid = Request["rid"].Trim();
                    using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        empid = dbobj.get_dbvalue(conn, "select empid from employee where emid = '" + emid + "'");
                        empworkcomp = dbobj.get_dbvalue(conn, "select empworkcomp from employee where emid = '" + emid + "'");
                        tmpname = dbobj.get_dbvalue(conn, "select roletitle from roleplay where rid = '" + rid + "' and comid = '" + empworkcomp + "'");//系統LOG 用

                        string sql = "select * from emprole where empid='"+ empid
                            + "' and comid='"+ empworkcomp
                            + "' and rid = '"+ rid
                            + "' and erid <> '"+ col.erid
                            +"'";

                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            SqlDataReader dr = cmd.ExecuteReader();
                            if (dr.HasRows)
                            {
                                ModelState.AddModelError("", "此角色已經安排，請重新輸入!");
                                return View(col);
                            }
                        }

                    }


                    col.empid = empid;
                    col.rid = rid;
                    col.comid = empworkcomp;

                    col.bmodid = Session["tempid"].ToString();
                    col.bmoddate = DateTime.Now;
                    using (Aitag_DBContext con = new Aitag_DBContext())
                    {


                        con.Entry(col).State = EntityState.Modified;
                        con.SaveChanges();



                        //系統LOG檔
                        string sysnote = "員工編號：" + empid + "<br>組織角色：" + tmpname + "的資料 ";
                        if (sysnote.Length > 4000) { sysnote = sysnote.Substring(0, 4000); }
                        //================================================= // 
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/employee/Edit' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='emid' name='emid' value='" + emid + "'>";
                    tmpform += "<input type=hidden id='qempworkcomp' name='qempworkcomp' value='" + qempworkcomp + "'>";
                    tmpform += "<input type=hidden id='qempworkdepid' name='qempworkdepid' value='" + qempworkdepid + "'>";
                    tmpform += "<input type=hidden id='qempstatus' name='qempstatus' value='" + qempstatus + "'>";
                    tmpform += "<input type=hidden id='qempno' name='qempno' value='" + qempno + "'>";
                    tmpform += "<input type=hidden id='qempid' name='qempid' value='" + qempid + "'>";
                    tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    // return RedirectToAction("List");

                }
            }


        }

        public ActionResult emproledel(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string orderdata = "", orderdata1 = "";
            //if (!string.IsNullOrWhiteSpace(Request["sysflag"]))
            //{
            //    sysflag = Request["sysflag"].Trim();
            //}
            if (!string.IsNullOrWhiteSpace(Request["orderdata"]))
            {
                orderdata = Request["orderdata"].Trim();
            }
            if (!string.IsNullOrWhiteSpace(Request["orderdata1"]))
            {
                orderdata1 = Request["orderdata1"].Trim();
            }

            string emid = "", qempworkcomp = "", qempworkdepid = "", qempstatus = "", qempno = "", qempid = "", qempname = "";
            if (!string.IsNullOrWhiteSpace(Request["emid"]))
            {
                emid = Request["emid"].Trim();
            }
            if (!string.IsNullOrWhiteSpace(Request["qempworkcomp"]))
            {
                qempworkcomp = Request["qempworkcomp"].Trim();
            }
            if (!string.IsNullOrWhiteSpace(Request["qempworkdepid"]))
            {
                qempworkdepid = Request["qempworkdepid"].Trim();
            }
            if (!string.IsNullOrWhiteSpace(Request["qempstatus"]))
            {
                qempstatus = Request["qempstatus"].Trim();
            }
            if (!string.IsNullOrWhiteSpace(Request["qempno"]))
            {
                qempno = Request["qempno"].Trim();
            }
            if (!string.IsNullOrWhiteSpace(Request["qempid"]))
            {
                qempid = Request["qempid"].Trim();
            }
            if (!string.IsNullOrWhiteSpace(Request["qempname"]))
            {
                qempname = Request["qempname"].Trim();
            }
            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/Employee/Edit' method='post'>";
            //tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

            tmpform += "<input type=hidden id='emid' name='emid' value='" + emid + "'>";
            tmpform += "<input type=hidden id='qempworkcomp' name='qempworkcomp' value='" + qempworkcomp + "'>";
            tmpform += "<input type=hidden id='qempworkdepid' name='qempworkdepid' value='" + qempworkdepid + "'>";
            tmpform += "<input type=hidden id='qempstatus' name='qempstatus' value='" + qempstatus + "'>";
            tmpform += "<input type=hidden id='qempno' name='qempno' value='" + qempno + "'>";
            tmpform += "<input type=hidden id='qempid' name='qempid' value='" + qempid + "'>";
            tmpform += "<input type=hidden id='qempname' name='qempname' value='" + qempname + "'>";

            tmpform += "</form>";
            tmpform += "</body>";


            string erid = Request["erid"];
            if (string.IsNullOrWhiteSpace(erid))
            {
                return new ContentResult() { Content = @"<script>alert('');</script>" + tmpform };
            }
            else
            {
                using (Aitag_DBContext con = new Aitag_DBContext())
                {
                    string empid = "", empworkcomp = "", tmpname = "", rid = "";
                    NDcommon dbobj = new NDcommon();
                    using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                    {

                        empid = dbobj.get_dbvalue(conn, "select empid from emprole where erid = '" + erid + "'");
                        empworkcomp = dbobj.get_dbvalue(conn, "select comid from emprole where erid = '" + erid + "'");
                        rid = dbobj.get_dbvalue(conn, "select rid from emprole where erid = '" + erid + "'");
                        tmpname = dbobj.get_dbvalue(conn, "select roletitle from roleplay where rid = '" + rid + "' and comid = '" + empworkcomp + "'");//系統LOG 用

                    }

                    dbobj.dbexecute("Aitag_DBContext", "delete from emprole where erid = '" + erid + "'");

                    
                    string sysrealsid = Request["sysrealsid"].ToString();

                    //系統LOG檔
                    string sysnote = "員工編號：" + empid + "<br>組織角色：" + tmpname + "的資料 ";
                    if (sysnote.Length > 4000) { sysnote = sysnote.Substring(0, 4000); }
                    //================================================= // 
                    SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                    string sysflag = "D";
                    dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    sysconn.Close();
                    sysconn.Dispose();
                    //======================================================     



                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');</script>" + tmpform };
                    //return RedirectToAction("List");
                }
            }
        }
        
        public ActionResult PriCheck()
        {
            ViewBag.psid = Request["psid"].ToString();
            ViewBag.empid = Request["empid"].ToString();
            return View();
        }


        public ActionResult PriCheckdo()
        {
            string bid = Request["bid"];
            string psid = Request["psid"].ToString();
            //修改權限
                if (Request["privdata"] != null)
                {
                    NDcommon dbobj = new NDcommon();
                    dbobj.dbexecute("Aitag_DBContext", "DELETE FROM Privtb where bid = '" + bid + "' and psid = '" + psid + "'");
                    string privstr = Request["privdata"];
                    string[] pvarr = privstr.Split(',');

                    //NDcommon dbobj = new NDcommon();
                    SqlConnection conn = dbobj.get_conn("Aitag_DBContext");
                    SqlCommand sqlsmd = new SqlCommand();
                    sqlsmd.Connection = conn;
                    for (int i = 0; i < pvarr.Length; i++)
                    {
                        //psid = dbobj.get_dbvalue(conn, "select distinct psid from sublevel1 where sid = '" + pvarr[i].ToString().Trim() + "'");
                        if (pvarr[i].ToString().Trim() != "")
                        {
                            sqlsmd.CommandText = "insert into Privtb(sid,bid,psid,chk,subread,subadd,submod,subdel,Bmodid,Bmoddate) values('" + pvarr[i].ToString().Trim() + "','" + bid + "','" + psid + "','1','1','1','1','1','" + Session["empid"].ToString() + "',getdate())";
                            sqlsmd.ExecuteNonQuery();
                        }
                    }
                    conn.Close();
                    conn.Dispose();

                    //系統LOG檔 //================================================= //
                    // iMedia.Models.NDcommon dbobj = new iMedia.Models.NDcommon();
                    string syssubname = "系統管理作業 > 使用者管理作業(權限)";
                    string sysnote = "帳號:" + bid;
                    string sysflag = "M";
                    SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                    dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                    sysconn.Close();
                    sysconn.Dispose();
                    //=================================================
                    //var oldPolicyManagement = con.Privtbs.Where(r => r.bid == "adm" && data.PolicyManagement.Contains(r.sid)).ToList();

                    return new ContentResult() { Content = @"<script>alert('權限修改成功!!');location.href='/employee/PriCheck/?empid=" + bid + "&psid=2'</script>" };
                }
                else
                {
                    return new ContentResult() { Content = @"<script>alert('請挑選功能權限!!');location.href='/employee/PriCheck/?empid=" + bid + "&psid=2'</script>" };
                }
                //return RedirectToAction("PriCheck");
            
        }


        #region  orggraph  組織列印
        public ActionResult orggraph()
        {
            NDcommon dbobj = new NDcommon();
            
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                string sql = "select * from roleplay where bossrid = '' and ifrtype = 'n'";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    using (SqlConnection comconn = dbobj.get_conn("Aitag_DBContext"))
                    {
                        while (dr.Read())
                        {
                            string tmpwhere = " where rid = '" + dr["rid"] + "' and empstatus <> '4'";
                            string tmpchname = dbobj.get_dbvalue(comconn, "select empname from viewemprole" + tmpwhere);
                            string tmpname = dbobj.get_dbvalue(comconn, "select empenname from viewemprole" + tmpwhere);
                            ViewBag.mcontent += "<tr>";
                            ViewBag.mcontent += "<td style=color:red><img src=/images/node1.gif align=absmiddle><img src=/images/boss.gif align=absmiddle>";
                            ViewBag.mcontent += dr["roletitle"] + " (" + tmpname + " " + tmpchname + ")";
                            ViewBag.mcontent += "</td>";
                            ViewBag.mcontent += "</tr>";
                            ViewBag.mcontent += "";
                            conn1( dbobj, dr);
                        }
                    }
                    
                    dr.Close();
                }
            } 


            return View();
        }

        private void conn1( NDcommon dbobj, SqlDataReader pdr)
        {
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                string sql = "select  * from roleplay where bossrid = '" + pdr["rid"] + "' and ifrtype = 'n' order by rid";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        string tmpname = "", tmpchname = "";
                        while (dr.Read())
                        {
                            sql = "select distinct empname,empenname  from viewemprole  where rid = '" + dr["rid"] + "' and ifuse = 'y' and empstatus <> '4'";
                            using (SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext"))
                            {
                                using (SqlCommand cmd1 = new SqlCommand(sql, conn1))
                                {
                                    SqlDataReader dr1 = cmd1.ExecuteReader();
                                    if (dr1.HasRows)
                                    {
                                        while (dr1.Read())
                                        {
                                            tmpname = dr1["empenname"] + "&#160;&#160;";
                                            tmpchname = dr1["empname"] + "";
                                            ViewBag.mcontent += "<tr>";
                                            ViewBag.mcontent += "<td style=color:blue><img src=/images/line.gif align=absmiddle><img src=/images/node1.gif align=absmiddle><img src=/images/boss.gif align=absmiddle>";
                                            ViewBag.mcontent += dr["roletitle"] + " (" + tmpname + tmpchname + ")";
                                            ViewBag.mcontent += "</td>";
                                            ViewBag.mcontent += "</tr>";
                                            ViewBag.mcontent += "";
                                        }
                                    }
                                    else
                                    {
                                        ViewBag.mcontent += "<tr>";
                                        ViewBag.mcontent += "<td style=color:blue><img src=/images/line.gif align=absmiddle><img src=/images/node1.gif align=absmiddle><img src=/images/boss.gif align=absmiddle>";
                                        ViewBag.mcontent += dr["roletitle"] + " (無)";
                                        ViewBag.mcontent += "</td>";
                                        ViewBag.mcontent += "</tr>";
                                        ViewBag.mcontent += "";
                                    }
                                    dr1.Close();
                                }
                            }
                            conn2( dbobj, dr);
                        }
                    }
                    dr.Close();
                }
            } 


        }

        private void conn2( NDcommon dbobj, SqlDataReader pdr)
        {
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                string sql = "select * from roleplay where bossrid = '" + pdr["rid"] + "' and ifrtype = 'n' order by rid";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        string tmpname = "", tmpchname = "";
                        while (dr.Read())
                        {
                            sql = "select distinct empname,empenname from viewemprole  where rid = '" + dr["rid"] + "' and ifuse = 'y'  and empstatus <> '4'";
                            using (SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext"))
                            {
                                using (SqlCommand cmd1 = new SqlCommand(sql, conn1))
                                {
                                    SqlDataReader dr1 = cmd1.ExecuteReader();
                                    if (dr1.HasRows)
                                    {
                                        while (dr1.Read())
                                        {
                                            tmpname = dr1["empenname"] + "&#160;&#160;";
                                            tmpchname = dr1["empname"] + "";
                                            ViewBag.mcontent += "<tr>";
                                            ViewBag.mcontent += "<td style=color:green><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/node1.gif align=absmiddle><img src=/images/boss.gif align=absmiddle>";
                                            ViewBag.mcontent += dr["roletitle"] + " (" + tmpname + tmpchname + ")";
                                            ViewBag.mcontent += "</td>";
                                            ViewBag.mcontent += "</tr>";
                                            ViewBag.mcontent += "";
                                        }
                                    }
                                    else
                                    {
                                        ViewBag.mcontent += "<tr>";
                                        ViewBag.mcontent += "<td style=color:green><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/node1.gif align=absmiddle><img src=/images/boss.gif align=absmiddle>";
                                        ViewBag.mcontent += dr["roletitle"] + " (無)";
                                        ViewBag.mcontent += "</td>";
                                        ViewBag.mcontent += "</tr>";
                                        ViewBag.mcontent += "";
                                    }
                                    dr1.Close();
                                }
                            }
                            conn3( dbobj, dr);
                        }
                    }
                    dr.Close();
                }
            }
        }

        private void conn3( NDcommon dbobj, SqlDataReader pdr)
        {
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                string sql = "select * from roleplay where bossrid = '" + pdr["rid"] + "' and ifrtype = 'n' order by rid";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        string tmpname = "", tmpchname = "";
                        while (dr.Read())
                        {
                            sql = "select distinct empname,empenname from viewemprole  where rid = '" + dr["rid"] + "' and ifuse = 'y'  and empstatus <> '4'";
                            using (SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext"))
                            {
                                using (SqlCommand cmd1 = new SqlCommand(sql, conn1))
                                {
                                    SqlDataReader dr1 = cmd1.ExecuteReader();
                                    if (dr1.HasRows)
                                    {
                                        while (dr1.Read())
                                        {
                                            tmpname = dr1["empenname"] + "&#160;&#160;";
                                            tmpchname = dr1["empname"] + "";
                                            ViewBag.mcontent += "<tr>";
                                            ViewBag.mcontent += "<td style=color:MediumVioletRed ><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/node1.gif align=absmiddle><img src=/images/boss.gif align=absmiddle>";
                                            ViewBag.mcontent += dr["roletitle"] + " (" + tmpname + tmpchname + ")";
                                            ViewBag.mcontent += "</td>";
                                            ViewBag.mcontent += "</tr>";
                                            ViewBag.mcontent += "";
                                        }
                                    }
                                    else
                                    {
                                        ViewBag.mcontent += "<tr>";
                                        ViewBag.mcontent += "<td style=color:MediumVioletRed ><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/node1.gif align=absmiddle><img src=/images/boss.gif align=absmiddle>";
                                        ViewBag.mcontent += dr["roletitle"] + " (無)";
                                        ViewBag.mcontent += "</td>";
                                        ViewBag.mcontent += "</tr>";
                                        ViewBag.mcontent += "";
                                    }
                                    dr1.Close();
                                }
                            }
                            conn4( dbobj, dr);
                        }
                    }
                    dr.Close();
                }
            }
        }

        private void conn4( NDcommon dbobj, SqlDataReader pdr)
        {
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                string sql = "select * from roleplay where bossrid = '" + pdr["rid"] + "' and ifrtype = 'n' order by rid";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        string tmpname = "", tmpchname = "";
                        while (dr.Read())
                        {
                            sql = "select distinct empname,empenname from viewemprole  where rid = '" + dr["rid"] + "' and ifuse = 'y' and ridtype = '1' and empstatus <> '4'";
                            using (SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext"))
                            {
                                using (SqlCommand cmd1 = new SqlCommand(sql, conn1))
                                {
                                    SqlDataReader dr1 = cmd1.ExecuteReader();
                                    if (dr1.HasRows)
                                    {
                                        while (dr1.Read())
                                        {
                                            tmpname = dr1["empenname"] + "&#160;&#160;";
                                            tmpchname = dr1["empname"] + "";
                                            ViewBag.mcontent += "<tr>";
                                            ViewBag.mcontent += "<td style=color:aa66dd ><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/node1.gif align=absmiddle><img src=/images/boss.gif align=absmiddle>";
                                            ViewBag.mcontent += dr["roletitle"] + " (" + tmpname + tmpchname + ")";
                                            ViewBag.mcontent += "</td>";
                                            ViewBag.mcontent += "</tr>";
                                            ViewBag.mcontent += "";
                                        }
                                    }
                                    else
                                    {
                                        ViewBag.mcontent += "<tr>";
                                        ViewBag.mcontent += "<td style=color:aa66dd ><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/node1.gif align=absmiddle><img src=/images/boss.gif align=absmiddle>";
                                        ViewBag.mcontent += dr["roletitle"] + " (無)";
                                        ViewBag.mcontent += "</td>";
                                        ViewBag.mcontent += "</tr>";
                                        ViewBag.mcontent += "";
                                    }
                                    dr1.Close();
                                }
                            }
                            conn5( dbobj, dr);
                        }
                    }
                    dr.Close();
                }
            }
        }

        private void conn5( NDcommon dbobj, SqlDataReader pdr)
        {
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                string sql = "select * from roleplay where bossrid = '" + pdr["rid"] + "' and ifrtype = 'n' order by rid";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        string tmpname = "", tmpchname = "";
                        while (dr.Read())
                        {
                            sql = "select distinct empname,empenname from viewemprole  where rid = '" + dr["rid"] + "' and ifuse = 'y' and empstatus <> '4'";
                            using (SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext"))
                            {
                                using (SqlCommand cmd1 = new SqlCommand(sql, conn1))
                                {
                                    SqlDataReader dr1 = cmd1.ExecuteReader();
                                    if (dr1.HasRows)
                                    {
                                        while (dr1.Read())
                                        {
                                            tmpname = dr1["empenname"] + "&#160;&#160;";
                                            tmpchname = dr1["empname"] + "";
                                            ViewBag.mcontent += "<tr>";
                                            ViewBag.mcontent += "<td style=color:6666ff ><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/node1.gif align=absmiddle><img src=/images/man.gif align=absmiddle>";
                                            ViewBag.mcontent += dr["roletitle"] + " (" + tmpname + tmpchname + ")";
                                            ViewBag.mcontent += "</td>";
                                            ViewBag.mcontent += "</tr>";
                                            ViewBag.mcontent += "";
                                        }
                                    }
                                    else
                                    {
                                        ViewBag.mcontent += "<tr>";
                                        ViewBag.mcontent += "<td style=color:6666ff ><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/node1.gif align=absmiddle><img src=/images/man.gif align=absmiddle>";
                                        ViewBag.mcontent += dr["roletitle"] + " (無)";
                                        ViewBag.mcontent += "</td>";
                                        ViewBag.mcontent += "</tr>";
                                        ViewBag.mcontent += "";
                                    }
                                    dr1.Close();
                                }
                            }
                            conn6( dbobj, dr);
                        }
                    }
                    dr.Close();
                }
            }
        }

        private void conn6( NDcommon dbobj, SqlDataReader pdr)
        {
            using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
            {
                string sql = "select * from roleplay where bossrid = '" + pdr["rid"] + "' and ifrtype = 'n' order by rid";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        string tmpname = "", tmpchname = "";
                        while (dr.Read())
                        {
                            sql = "select distinct empname,empenname from viewemprole  where rid = '" + dr["rid"] + "'  and ifuse = 'y' and empstatus <> '4'";
                            using (SqlConnection conn1 = dbobj.get_conn("Aitag_DBContext"))
                            {
                                using (SqlCommand cmd1 = new SqlCommand(sql, conn1))
                                {
                                    SqlDataReader dr1 = cmd1.ExecuteReader();
                                    if (dr1.HasRows)
                                    {
                                        while (dr1.Read())
                                        {
                                            tmpname = dr1["empenname"] + "&#160;&#160;";
                                            tmpchname = dr1["empname"] + "";
                                            ViewBag.mcontent += "<tr>";
                                            ViewBag.mcontent += "<td style=color:6666ff ><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/node1.gif align=absmiddle><img src=/images/man.gif align=absmiddle>";
                                            ViewBag.mcontent += dr["roletitle"] + " (" + tmpname + tmpchname + ")";
                                            ViewBag.mcontent += "</td>";
                                            ViewBag.mcontent += "</tr>";
                                            ViewBag.mcontent += "";
                                        }
                                    }
                                    else
                                    {
                                        ViewBag.mcontent += "<tr>";
                                        ViewBag.mcontent += "<td style=color:6666ff ><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/line.gif align=absmiddle><img src=/images/node1.gif align=absmiddle><img src=/images/man.gif align=absmiddle>";
                                        ViewBag.mcontent += dr["roletitle"] + " (無)";
                                        ViewBag.mcontent += "</td>";
                                        ViewBag.mcontent += "</tr>";
                                        ViewBag.mcontent += "";
                                    }
                                    dr1.Close();
                                }
                            }
                            conn6( dbobj, dr);
                        }
                    }
                    dr.Close();
                }
            }
        }



        #endregion

    }
}
