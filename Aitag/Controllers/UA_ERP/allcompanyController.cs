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
    public class allcompanyController : BaseController
    {

        //private Aitag_DBContext db = new Aitag_DBContext();
        //
        // GET: /Company/

        public ActionResult Index()
        {
            
            return RedirectToAction("List");
        }

        //[HttpGet]
        //public ActionResult Add()
        //{
        //    ViewBag.Ifboss = Session["Ifboss"].ToString();
        //    ViewBag.comid = Session["comid"].ToString();
        //    Company col = new Company();
        //    return View(col);
        //}

        //[HttpPost]
         [ValidateInput(false)]
        public ActionResult add(allcompany col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "comid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcomid = "", qcomsno = "", qcomtitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qcomid"]))
            {
                qcomid = Request["qcomid"].Trim();
                ViewBag.qcomid = qcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomsno"]))
            {
                qcomsno = Request["qcomsno"].Trim();
                ViewBag.qcomsno = qcomsno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomtitle"]))
            {

                qcomtitle = Request["qcomtitle"].Trim();
                ViewBag.qcomtitle = qcomtitle;
            }

            if (sysflag != "A")
            {
                allcompany newcol = new allcompany();
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
                    string sqlstr = "select comid from allcompany where comid = '" + col.comid + "'";
                    sqlsmd.CommandText = sqlstr;
                    dr = sqlsmd.ExecuteReader();

                    if (dr.Read())
                    {

                        ModelState.AddModelError("", "公司代碼代碼重複!");
                        return View(col);
                    }
                    dr.Close();
                    dr.Dispose();
                    sqlsmd.Dispose();
                    conn.Close();
                    conn.Dispose();

                    //密碼加密
                    //col.emppasswd = dbobj.Encrypt(col.emppasswd);
                    //col.comid = col.emppasswd;
                    //col.baddid = Session["tempid"].ToString();
                    col.comtype = Request["comtype"];
                    col.bmodid = Session["tempid"].ToString();
                    //col.badddate = DateTime.Now;
                    col.bmoddate = DateTime.Now;
                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {
                        con.allcompany.Add(col);
                       
                        try
                        {
                          
                            con.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }

                        

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "代碼:" + col.comid + "名稱:" + col.comtitle;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/allcompany/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qcomid' name='qcomid' value='" + qcomid + "'>";
                    tmpform += "<input type=hidden id='qcomsno' name='qcomsno' value='" + qcomsno + "'>";
                    tmpform += "<input type=hidden id='qcomtitle' name='qcomtitle' value='" + qcomtitle + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                    // return RedirectToAction("List");

                }
            }


        }


        [ValidateInput(false)]
        public ActionResult Edit(allcompany chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "comid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcomid = "", qcomsno = "", qcomtitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qcomid"]))
            {
                qcomid = Request["qcomid"].Trim();
                ViewBag.qcomid = qcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomsno"]))
            {
                qcomsno = Request["qcomsno"].Trim();
                ViewBag.qcomsno = qcomsno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomtitle"]))
            {

                qcomtitle = Request["qcomtitle"].Trim();
                ViewBag.qcomtitle = qcomtitle;
            }

            if (sysflag != "E")
            {
                using (AitagBill_DBContext con = new AitagBill_DBContext())
                {
                    var data = con.allcompany.Where(r => r.comid == chks.comid).FirstOrDefault();
                    allcompany eCompanys = con.allcompany.Find(chks.comid);
                    if (eCompanys == null)
                    {
                        return HttpNotFound();
                    }
                    return View(eCompanys);
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

                    //string oldcomid = Request["oldcomid"];                 

                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {
                        NDcommon dbobj = new NDcommon();
                        chks.comtype = Request["comtype"];
                        chks.comid = Request["comid"].Trim();
                        chks.bmodid = Session["tempid"].ToString();
                        chks.bmoddate = DateTime.Now;
                        con.Entry(chks).State = EntityState.Modified;
                        con.SaveChanges();


                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "代碼:" + chks.comid + "名稱:" + chks.comtitle;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/allcompany/List' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                        tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                        tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                        tmpform += "<input type=hidden id='qcomid' name='qcomid' value='" + qcomid + "'>";
                        tmpform += "<input type=hidden id='qcomsno' name='qcomsno' value='" + qcomsno + "'>";
                        tmpform += "<input type=hidden id='qcomtitle' name='qcomtitle' value='" + qcomtitle + "'>";
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
            { orderdata = "comid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcomid = "", qcsno = "", qcomtitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qcomid"]))
            {
                qcomid = Request["qcomid"].Trim();
                ViewBag.qcomid = qcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcsno"]))
            {
                qcsno = Request["qcsno"].Trim();
                ViewBag.qcsno = qcsno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomtitle"]))
            {

                qcomtitle = Request["qcomtitle"].Trim();
                ViewBag.qcomtitle = qcomtitle;
            }

            IPagedList<allcompany> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "select * from allCompany where comtype like '%1%'  and";
                if (qcomid != "")
                    sqlstr += " comid like '%" + qcomid + "%'  and";
                if (qcsno != "")
                    sqlstr += " csno like '%" + qcsno + "%'  and";
                if (qcomtitle != "")
                    sqlstr += " (comtitle like '%" + qcomtitle + "%' or comsttitle like '%" + qcomtitle + "%')    and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.allcompany.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<allcompany>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch(orderdata, orderdata1);
            return View(result);
        }

        public ActionResult vendlist(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "comid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcomid = "", qcsno = "", qcomtitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qcomid"]))
            {
                qcomid = Request["qcomid"].Trim();
                ViewBag.qcomid = qcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcsno"]))
            {
                qcsno = Request["qcsno"].Trim();
                ViewBag.qcsno = qcsno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomtitle"]))
            {

                qcomtitle = Request["qcomtitle"].Trim();
                ViewBag.qcomtitle = qcomtitle;
            }

            IPagedList<allcompany> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "select * from allCompany where (comtype like '%2%' or comtype like '%3%' or comtype like '%4%')   and";
                if (qcomid != "")
                    sqlstr += " comid like '%" + qcomid + "%'  and";
                if (qcsno != "")
                    sqlstr += " csno like '%" + qcsno + "%'  and";
                if (qcomtitle != "")
                    sqlstr += " (comtitle like '%" + qcomtitle + "%' or comsttitle like '%" + qcomtitle + "%')    and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.allcompany.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<allcompany>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch(orderdata, orderdata1);
            return View(result);
        }
        private string SetOrder_ch(string orderdata, string orderdata1)
        {
            string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
            string Order_ch = "function SetOrder_ch() { ";
            string[] od_ch = { "comid", "comtitle" };
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

       
        public ActionResult privcustqry(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "comid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcomid = "", qcsno = "", qcomtitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qcomid"]))
            {
                qcomid = Request["qcomid"].Trim();
                ViewBag.qcomid = qcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcsno"]))
            {
                qcsno = Request["qcsno"].Trim();
                ViewBag.qcsno = qcsno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomtitle"]))
            {

                qcomtitle = Request["qcomtitle"].Trim();
                ViewBag.qcomtitle = qcomtitle;
            }

            IPagedList<allcompany> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "select * from allCompany where comtype not like '%1%'   and";
                if (qcomid != "")
                    sqlstr += " comid like '%" + qcomid + "%'  and";
                if (qcsno != "")
                    sqlstr += " csno like '%" + qcsno + "%'  and";
                if (qcomtitle != "")
                    sqlstr += " (comtitle like '%" + qcomtitle + "%' or comsttitle like '%" + qcomtitle + "%')    and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.allcompany.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<allcompany>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch(orderdata, orderdata1);
            return View(result);
        }
        public ActionResult privvendqry(int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "comid"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "asc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qcomid = "", qcsno = "", qcomtitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qcomid"]))
            {
                qcomid = Request["qcomid"].Trim();
                ViewBag.qcomid = qcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcsno"]))
            {
                qcsno = Request["qcsno"].Trim();
                ViewBag.qcsno = qcsno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomtitle"]))
            {

                qcomtitle = Request["qcomtitle"].Trim();
                ViewBag.qcomtitle = qcomtitle;
            }

            IPagedList<allcompany> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "select * from allCompany where comtype not like '%1%'   and";
                if (qcomid != "")
                    sqlstr += " comid like '%" + qcomid + "%'  and";
                if (qcsno != "")
                    sqlstr += " csno like '%" + qcsno + "%'  and";
                if (qcomtitle != "")
                    sqlstr += " (comtitle like '%" + qcomtitle + "%' or comsttitle like '%" + qcomtitle + "%')    and";

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.allcompany.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<allcompany>(page.Value - 1, (int)Session["pagesize"]);

            }
            ViewBag.SetOrder_ch = SetOrder_ch(orderdata, orderdata1);
            return View(result);
        }


        public ActionResult prodlist()
        {
            
            string tmppid = "";
            tmppid = Request["allcomid"].ToString();
            ViewBag.allcomid = tmppid.ToString();
            string fprodid = "";
            fprodid = Request["fprodid"].ToString().Trim();
           
            ViewBag.fprodid = fprodid.ToString();

            List<custproduct> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                var query = con.custproduct.AsQueryable();
                result = query.Where(r => r.allcomid == tmppid && r.fprodid == fprodid).AsQueryable().ToList();

            }
            return View(result);

        }

         [ValidateInput(false)]
        public ActionResult prodlistdo(string sysflag, int? page, string orderdata, string orderdata1)
        {

            string allcomid = "";
            string fprodid = "";
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {

                page = ((!page.HasValue || page < 1) ? 1 : page);
                ViewBag.page = page;
                ViewBag.orderdata = orderdata;
                ViewBag.orderdata1 = orderdata1;
                string sqlstr = "";
                string cdel1 = Request["prodid"];

                string prodtitle1 = Request["prodtitle"];
               

                string[] cdelarr = cdel1.Split(',');
                
                string[] prodtitlearr = prodtitle1.Split(',');


                allcomid = Request["allcomid"].ToString();
                fprodid = Request["fprodid"].ToString();
                int pitemno = 10;
                for (int i = 0; i < cdelarr.Length; i++)
                {
                    if (cdelarr[i].Trim() == "")
                    {
                        if (!(prodtitlearr[i].Trim() == ""))
                        {
                            custproduct addobj = new custproduct();
                            addobj.prodid = DateTime.Now.ToString("yyyyMMddhhmmssfffff");
                            addobj.allcomid = allcomid;
                            addobj.prodtitle = prodtitlearr[i].Trim();
                            // addobj.mdno = mdnoarr[i].Trim();
                            // addobj.mdcomment = mdcommentarr[i].Trim();
                            addobj.fprodid = fprodid;

                            addobj.comid = Session["comid"].ToString();
                            addobj.bmodid = Session["empid"].ToString();
                            addobj.bmoddate = DateTime.Now;

                            con.custproduct.Add(addobj);
                            con.SaveChanges();
                            pitemno = pitemno + 10;
                        }
                    }
                    else
                    {
                        //修改
                        string prodid = cdelarr[i].Trim();
                        custproduct modobj = con.custproduct.Where(r => r.prodid == prodid).FirstOrDefault();

                        modobj.prodtitle = prodtitlearr[i].Trim();

                        modobj.comid = Session["comid"].ToString();
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
            tmpform += "<form name='qfr1' action='/allcompany/prodlist' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden name='allcomid' id='allcomid' value='" + allcomid + "'>";
            tmpform += "<input type=hidden name='fprodid' id='fprodid' value='" + fprodid + "'>";
            tmpform += "</form>";
            tmpform += "</body>";



            return new ContentResult() { Content = @"" + tmpform };
        }

        public ActionResult proddel(string id, int? page)
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

            string qcomid = "", qcomsno = "", qcomtitle = "" ,allcomid = "" , fprodid = "";
            if (!string.IsNullOrWhiteSpace(Request["qcomid"]))
            {
                qcomid = Request["qcomid"].Trim();
                ViewBag.qcomid = qcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomsno"]))
            {
                qcomsno = Request["qcomsno"].Trim();
                ViewBag.qcomsno = qcomsno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomtitle"]))
            {

                qcomtitle = Request["qcomtitle"].Trim();
                ViewBag.qcomtitle = qcomtitle;
            }

            if (!string.IsNullOrWhiteSpace(Request["allcomid"]))
            {

                allcomid = Request["allcomid"].Trim();
                ViewBag.allcomid = allcomid;
            }


            if (!string.IsNullOrWhiteSpace(Request["fprodid"]))
            {

                fprodid = Request["fprodid"].Trim();
                ViewBag.fprodid = fprodid;
            }


            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/allcompany/prodlist' method='post'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

            tmpform += "<input type=hidden id='allcomid' name='allcomid' value='" + allcomid + "'>";
            tmpform += "<input type=hidden id='qcomid' name='qcomid' value='" + qcomid + "'>";
            tmpform += "<input type=hidden id='fprodid' name='fprodid' value='" + fprodid + "'>";
            //tmpform += "<input type=hidden id='qcomsno' name='qcomsno' value='" + qcomsno + "'>";
            tmpform += "<input type=hidden id='qcomtitle' name='qcomtitle' value='" + qcomtitle + "'>";

            tmpform += "</form>";
            tmpform += "</body>";

            string cdel = Request["cdel"];
            if (string.IsNullOrWhiteSpace(cdel))
            {
                return new ContentResult() { Content = @"<script>alert('請勾選要刪除的資料!!');</script>" + tmpform };
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
                        if (condtionArr[i] != "")
                        {
                            string eCompanys = dbobj.get_dbvalue(conn1, "select prodtitle from custproduct where prodid ='" + condtionArr[i].ToString() + "'");

                            sysnote += "產品名稱:" + eCompanys + "，序號:" + condtionArr[i].ToString() + "<br>";

                            dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM custproduct where prodid = '" + condtionArr[i].ToString() + "'");
                        }
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

        public ActionResult medialist()
        {

            string tmppid = "" , mcno="";
            tmppid = Request["allcomid"].ToString();
            ViewBag.allcomid = tmppid.ToString();
            mcno = Request["mcno"].ToString();
            ViewBag.mcno = mcno.ToString();
            //decimal fprodid = 0;
            //fprodid = Decimal.Parse(Request["fprodid"].ToString());

            //ViewBag.fprodid = fprodid.ToString();

            List<view_mcno_allcompany_media> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                var query = con.view_mcno_allcompany_media.AsQueryable();
                if (mcno!="")
                    result = query.Where(r => r.allcomid == tmppid && r.mcno == mcno).AsQueryable().ToList();
                else
                    result = query.Where(r => r.allcomid == tmppid).AsQueryable().ToList();

            }
            return View(result);

        }

        [ValidateInput(false)]
        public ActionResult medialistdo(string sysflag, int? page, string orderdata, string orderdata1)
        {

            string allcomid = "" , mcno = "";
          //  string fprodid = "";
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {

                page = ((!page.HasValue || page < 1) ? 1 : page);
                ViewBag.page = page;
                ViewBag.orderdata = orderdata;
                ViewBag.orderdata1 = orderdata1;
                string sqlstr = "";
                string cdel1 = Request["cid"];
                string mdno1 = Request["mdno"];
                string contactman1 = Request["contactman"];
                string ctel1 = Request["ctel"];
                string cmob1 = Request["cmob"];
                string cfax1 = Request["cfax"];
                string ctsdate1 = Request["ctsdate"];
                string ctedate1 = Request["ctedate"];
              //  string cmob1 = Request["cmob"];
              //  string mdno1 = Request["mdno"];


                string[] cdelarr = cdel1.Split(',');
                string[] mdnoarr = mdno1.Split(',');
                string[] contactmanarr = contactman1.Split(',');
                string[] ctelarr = ctel1.Split(',');
                string[] cmobarr = cmob1.Split(',');
                string[] cfaxarr = cfax1.Split(',');
                string[] ctsdatearr = ctsdate1.Split(',');
                string[] ctedatearr = ctedate1.Split(',');


                allcomid = Request["allcomid"].ToString();
                mcno = Request["mcno"].ToString();
         //       fprodid = Request["fprodid"].ToString();
                int pitemno = 10;
                for (int i = 0; i < cdelarr.Length; i++)
                {
                    if (cdelarr[i].Trim() == "")
                    {
                        if (!(mdnoarr[i].Trim() == ""))
                        {
                            allcompany_media addobj = new allcompany_media();
                            addobj.mdno = mdnoarr[i].Trim();
                            addobj.allcomid = allcomid;
                            addobj.contactman = contactmanarr[i].Trim();
                            addobj.ctel = ctelarr[i].Trim();
                            addobj.cmob = cmobarr[i].Trim();
                            addobj.cfax = cfaxarr[i].Trim();
                           
                            addobj.comid = Session["comid"].ToString();
                            addobj.bmodid = Session["empid"].ToString();
                            addobj.bmoddate = DateTime.Now;
                            addobj.ctsdate = DateTime.Parse(ctsdatearr[i].Trim());
                            addobj.ctedate = DateTime.Parse(ctedatearr[i].Trim());

                            con.allcompany_media.Add(addobj);
                            con.SaveChanges();
                            pitemno = pitemno + 10;
                        }
                    }
                    else
                    {
                        //修改
                        int cid = int.Parse(cdelarr[i].Trim());
                        allcompany_media modobj = con.allcompany_media.Where(r => r.cid == cid).FirstOrDefault();

                        modobj.contactman = contactmanarr[i].Trim();
                        modobj.ctel = ctelarr[i].Trim();
                        modobj.cmob = cmobarr[i].Trim();
                        modobj.cfax = cfaxarr[i].Trim();
                        
                        modobj.comid = Session["comid"].ToString();
                        modobj.bmodid = Session["empid"].ToString();
                        modobj.bmoddate = DateTime.Now;
                        modobj.ctsdate = DateTime.Parse(ctsdatearr[i].Trim());
                        modobj.ctedate = DateTime.Parse(ctedatearr[i].Trim());
                        
                        con.Entry(modobj).State = EntityState.Modified;
                        con.SaveChanges();
                    }
                }
                con.Dispose();


            }

            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/allcompany/medialist' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden name='allcomid' id='allcomid' value='" + allcomid + "'>";
            tmpform += "<input type=hidden name='mcno' id='mcno' value='" + mcno + "'>";
            tmpform += "</form>";
            tmpform += "</body>";



            return new ContentResult() { Content = @"" + tmpform };
        }

        public ActionResult medialistdel(string id, int? page)
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

            string qcomid = "", qcomsno = "", qcomtitle = "", allcomid = "";
            if (!string.IsNullOrWhiteSpace(Request["qcomid"]))
            {
                qcomid = Request["qcomid"].Trim();
                ViewBag.qcomid = qcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomsno"]))
            {
                qcomsno = Request["qcomsno"].Trim();
                ViewBag.qcomsno = qcomsno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomtitle"]))
            {

                qcomtitle = Request["qcomtitle"].Trim();
                ViewBag.qcomtitle = qcomtitle;
            }

            if (!string.IsNullOrWhiteSpace(Request["allcomid"]))
            {

                allcomid = Request["allcomid"].Trim();
                ViewBag.allcomid = allcomid;
            }


            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/allcompany/medialist' method='post'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

            tmpform += "<input type=hidden id='allcomid' name='allcomid' value='" + allcomid + "'>";
            tmpform += "<input type=hidden id='qcomid' name='qcomid' value='" + qcomid + "'>";
            //tmpform += "<input type=hidden id='qcomsno' name='qcomsno' value='" + qcomsno + "'>";
            tmpform += "<input type=hidden id='qcomtitle' name='qcomtitle' value='" + qcomtitle + "'>";

            tmpform += "</form>";
            tmpform += "</body>";

            string cdel = Request["cdel"];
            if (string.IsNullOrWhiteSpace(cdel))
            {
                return new ContentResult() { Content = @"<script>alert('請勾選要刪除的資料!!');</script>" + tmpform };
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
                        if (condtionArr[i] != "")
                        {
                            string eCompanys = dbobj.get_dbvalue(conn1, "select contactman from allcompany_media where cid ='" + condtionArr[i].ToString() + "'");

                            sysnote += "聯絡人:" + eCompanys + "，序號:" + condtionArr[i].ToString() + "<br>";

                            dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM allcompany_media where cid = '" + condtionArr[i].ToString() + "'");
                        }
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


        public ActionResult brlist()
        {
            //string allcomid = "";
            //allcomid = decimal.Parse(Request["vcid"].ToString());
            string allcomid = "" , bseason = "";
            allcomid = Request["allcomid"].ToString();
            ViewBag.allcomid = allcomid.ToString();
             bseason = Request["bseason"].ToString();
             if (string.IsNullOrWhiteSpace(bseason))
                 bseason = "00";
            ViewBag.bseason = bseason.ToString();

            List<allcompany_rate> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                var query = con.allcompany_rate.AsQueryable();
                result = query.Where(r => r.allcomid == allcomid && r.bseason == bseason).AsQueryable().ToList();

            }
            return View(result);

        }

        public ActionResult brlistdo(string sysflag, int? page, string orderdata, string orderdata1)
        {

            string allcomid = "" , bseason = "";
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {

                page = ((!page.HasValue || page < 1) ? 1 : page);
                ViewBag.page = page;
                ViewBag.orderdata = orderdata;
                ViewBag.orderdata1 = orderdata1;
                string sqlstr = "";
                string cdel1 = Request["bid"];

              //  string bseason1 = Request["bseason"];
                string btype1 = Request["btype"];
                string mcno1 = Request["mcno"];
                string mdno1 = Request["Mmdno"];
                string nonmdno1 = Request["Mnonmdno"];
                string nonclosetype1 = Request["Mmino"];
                string bsmoney1 = Request["bsmoney"];
                string bemoney1 = Request["bemoney"];
                string brate1 = Request["brate"];
                //string bcomment1 = Request["bcomment"];
                string ifzero1 = Request["ifzero"];
                string bfday_discount1 = Request["bfday_discount"];
                string iftax1 = Request["iftax"];

                string[] cdelarr = cdel1.Split(',');
               // string[] bseasonarr = bseason1.Split(',');
                string[] btypearr = btype1.Split(',');
                string[] mcnoarr = mcno1.Split(',');
                string[] mdnoarr = mdno1.Split(',');
                string[] nonmdnoarr = nonmdno1.Split(',');
                string[] nonclosetypearr = nonclosetype1.Split(',');
                // string[] mdnoarr = mdno1.Split(',');
                // string[] mdcommentarr = mdcomment1.Split(',');
                string[] bsmoneyarr = bsmoney1.Split(',');
                string[] bemoneyarr = bemoney1.Split(',');
                string[] bratearr = brate1.Split(',');
                string[] ifzeroarr = ifzero1.Split(',');
                string[] bfday_discountarr = bfday_discount1.Split(',');
                string[] iftaxarr = iftax1.Split(',');

            //    string[] bcommentarr = bcomment1.Split(',');


                allcomid = Request["allcomid"].ToString();
                bseason = Request["bseason"].ToString();
                int pitemno = 10;
                for (int i = 0; i < cdelarr.Length; i++)
                {
                    if (cdelarr[i].Trim() == "")
                    {
                        if (!(btypearr[i].Trim() == ""))
                        {
                            allcompany_rate addobj = new allcompany_rate();
                            addobj.allcomid = allcomid;
                           
                            //addobj.vcno = Request["vcno"].ToString();
                            addobj.comid = Session["comid"].ToString();

                            addobj.bseason = bseason.Trim();
                            addobj.btype = btypearr[i].Trim();
                            addobj.mcno = mcnoarr[i].Trim();
                            if (mdnoarr[i].Trim() != "")
                                addobj.mdno = mdnoarr[i].Trim();
                            else
                                addobj.mdno = "";

                            if (nonmdnoarr[i].Trim() != "")
                                addobj.nonmdno = nonmdnoarr[i].Trim();
                            else
                                addobj.nonmdno = "";

                            if (nonclosetypearr[i].Trim() != "")
                                addobj.nonclosetype = nonclosetypearr[i].Trim();
                            else
                                addobj.nonclosetype = "";


                            // addobj.mdno = mdnoarr[i].Trim();
                            // addobj.mdcomment = mdcommentarr[i].Trim();

                            // addobj.vitemno = pitemno;
                            addobj.bsmoney = decimal.Parse("0" + bsmoneyarr[i].ToString());
                            addobj.bemoney = decimal.Parse("0" + bemoneyarr[i].ToString());
                            addobj.brate = decimal.Parse("0" + bratearr[i].ToString());
                            //addobj.vcallmoney = Decimal.Parse(vcallmoneyarr[i].ToString());

                   //         addobj.bcomment = bcommentarr[i].Trim();
                            // addobj.projno = Request["projno"].ToString();
                            addobj.ifzero = ifzeroarr[i].Trim();
                            addobj.bfday_discount = bfday_discountarr[i].Trim();
                            addobj.iftax = iftaxarr[i].Trim();

                            addobj.bmodid = Session["empid"].ToString();
                            addobj.bmoddate = DateTime.Now;

                            con.allcompany_rate.Add(addobj);
                            con.SaveChanges();
                            pitemno = pitemno + 10;
                        }
                    }
                    else
                    {
                        //修改
                        int bid = int.Parse(cdelarr[i].Trim());
                        allcompany_rate modobj = con.allcompany_rate.Where(r => r.bid == bid).FirstOrDefault();


                   //   modobj.comid = Request["comid"].ToString();
                        modobj.mcno = mcnoarr[i].Trim();
                        if (mdnoarr[i].Trim() != "")
                            modobj.mdno = mdnoarr[i].Trim();
                        else
                            modobj.mdno = "";

                        if (nonmdnoarr[i].Trim() != "")
                            modobj.nonmdno = nonmdnoarr[i].Trim();
                        else
                            modobj.nonmdno = "";

                        if (nonclosetypearr[i].Trim() != "")
                            modobj.nonclosetype = nonclosetypearr[i].Trim();
                        else
                            modobj.nonclosetype = "";
                       
                        modobj.btype = btypearr[i].Trim();
                        // addobj.mdno = mdnoarr[i].Trim();
                        // addobj.mdcomment = mdcommentarr[i].Trim();

                        // addobj.vitemno = pitemno;
                        modobj.bsmoney = decimal.Parse(bsmoneyarr[i].ToString());
                        modobj.bemoney = decimal.Parse(bemoneyarr[i].ToString());
                        modobj.brate = decimal.Parse(bratearr[i].ToString());
                        //addobj.vcallmoney = Decimal.Parse(vcallmoneyarr[i].ToString());

               //         modobj.bcomment = bcommentarr[i].Trim();
                        // addobj.projno = Request["projno"].ToString();
                        modobj.ifzero = ifzeroarr[i].Trim();
                        modobj.bfday_discount = bfday_discountarr[i].Trim();
                        modobj.iftax = iftaxarr[i].Trim();
                        modobj.bmodid = Session["empid"].ToString();
                        modobj.bmoddate = DateTime.Now;
                        modobj.bmodid = Session["empid"].ToString();
                        modobj.bmoddate = DateTime.Now;

                        con.Entry(modobj).State = EntityState.Modified;
                        try
                        {
                            con.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }
                }
                con.Dispose();


            }

            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/allcompany/brlist' method='post'>";
            tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
            tmpform += "<input type=hidden name='allcomid' id='allcomid' value='" + allcomid + "'>";
            tmpform += "<input type=hidden name='bseason' id='bseason' value='" + bseason + "'>";
            tmpform += "</form>";
            tmpform += "</body>";



            return new ContentResult() { Content = @"" + tmpform };
        }


        public ActionResult brdel(string id, int? page)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            string cdel = Request["cdel"];
            string allcomid = Request["allcomid"];
            string bseason = Request["bseason"];
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
                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM allcompany_rate where bid = '" + condtionArr[i].ToString() + "'");

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
                    tmpform += "<form name='qfr1' action='/allcompany/brlist' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden id='allcomid' name='allcomid' value='" + allcomid + "'>";
                    tmpform += "<input type=hidden id='bseason' name='bseason' value='" + bseason + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";

                    return new ContentResult() { Content = @"<script>alert('刪除成功!!');</script>" + tmpform };


                }
            }
        }


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

            string qcomid = "", qcomsno = "", qcomtitle = "";
            if (!string.IsNullOrWhiteSpace(Request["qcomid"]))
            {
                qcomid = Request["qcomid"].Trim();
                ViewBag.qcomid = qcomid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomsno"]))
            {
                qcomsno = Request["qcomsno"].Trim();
                ViewBag.qcomsno = qcomsno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qcomtitle"]))
            {

                qcomtitle = Request["qcomtitle"].Trim();
                ViewBag.qcomtitle = qcomtitle;
            }


            string tmpform = "";
            tmpform = "<body onload=qfr1.submit();>";
            tmpform += "<form name='qfr1' action='/allcompany/List' method='post'>";
            tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
            tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
            tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";

            tmpform += "<input type=hidden id='qcomid' name='qcomid' value='" + qcomid + "'>";
            tmpform += "<input type=hidden id='qcsno' name='qcomsno' value='" + qcomsno + "'>";
            tmpform += "<input type=hidden id='qcomtitle' name='qcomtitle' value='" + qcomtitle + "'>";

            tmpform += "</form>";
            tmpform += "</body>";

            string cdel = Request["cdel"];
            if (string.IsNullOrWhiteSpace(cdel))
            {
                return new ContentResult() { Content = @"<script>alert('請勾選要刪除的資料!!');</script>" + tmpform }; 
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
                        string eCompanys = dbobj.get_dbvalue(conn1, "select comtitle from allCompany where comid ='" + condtionArr[i].ToString() + "'");

                        sysnote += "代碼名稱:" + eCompanys + "，序號:" + condtionArr[i].ToString() + "<br>";

                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM allCompany where comid = '" + condtionArr[i].ToString() + "'");
                     
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



    }
}
