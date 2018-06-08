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
    public class mediaitemtypeController : BaseController
    {

        
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult add(mediaitemtype col, string sysflag, int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "mino"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qmitype = "", qmcno = "", qmititle = "";
            if (!string.IsNullOrWhiteSpace(Request["qmitype"]))
            {
                qmitype = Request["qmitype"].Trim();
                ViewBag.qmitype = qmitype;
            }
            if (!string.IsNullOrWhiteSpace(Request["qmcno"]))
            {
                qmcno = Request["qmcno"].Trim();
                ViewBag.qmcno = qmcno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qmititle"]))
            {

                qmititle = Request["qmititle"].Trim();
                ViewBag.qmititle = qmititle;
            }

            if (sysflag != "A")
            {
                mediaitemtype newcol = new mediaitemtype();
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
                    string sqlstr = "select mcno from mediaitemtype where mcno = '" + col.mcno + "'";
                    sqlsmd.CommandText = sqlstr;
                    dr = sqlsmd.ExecuteReader();

                    if (dr.Read())
                    {

                        ModelState.AddModelError("", "類別編號重複!");
                        return View(col);
                    }
                    dr.Close();
                    dr.Dispose();
                    sqlsmd.Dispose();
                    conn.Close();
                    conn.Dispose();

                    col.comid = Session["comid"].ToString();
                    col.bmodid = Session["tempid"].ToString();
                    col.bmoddate = DateTime.Now;
                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {
                        con.mediaitemtype.Add(col);
                        con.SaveChanges();

                        //系統LOG檔 //================================================= //
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "類別編號:" + col.mcno + "類別名稱:" + col.mititle;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                    }
                    string tmpform = "";
                    tmpform = "<body onload=qfr1.submit();>";
                    tmpform += "<form name='qfr1' action='/mediaitemtype/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qmcno' name='qmcno' value='" + qmcno + "'>";
                    tmpform += "<input type=hidden id='qmititle' name='qmititle' value='" + qmititle + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };

                }
            }


        }


        public ActionResult Edit(mediaitemtype chks, string sysflag, int? page, string orderdata, string orderdata1)
        {
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "mino"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qmitype = "", qmcno = "", qmititle = "";
            if (!string.IsNullOrWhiteSpace(Request["qmitype"]))
            {
                qmitype = Request["qmitype"].Trim();
                ViewBag.qmitype = qmitype;
            }
            if (!string.IsNullOrWhiteSpace(Request["qmcno"]))
            {
                qmcno = Request["qmcno"].Trim();
                ViewBag.qmcno = qmcno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qmititle"]))
            {

                qmititle = Request["qmititle"].Trim();
                ViewBag.qmititle = qmititle;
            }

            if (sysflag != "E")
            {
                using (AitagBill_DBContext con = new AitagBill_DBContext())
                {
                    var data = con.mediaitemtype.Where(r => r.mino == chks.mino).FirstOrDefault();
                    mediaitemtype ebillsubjects = con.mediaitemtype.Find(chks.mino);
                    if (ebillsubjects == null)
                    {
                        return HttpNotFound();
                    }
                    return View(ebillsubjects);
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

                    using (AitagBill_DBContext con = new AitagBill_DBContext())
                    {

                        NDcommon dbobj = new NDcommon();
                        chks.mino = Request["mino"].ToString();
                        chks.bmodid = Session["tempid"].ToString();
                        chks.bmoddate = DateTime.Now;
                        con.Entry(chks).State = EntityState.Modified;
                        con.SaveChanges();


                        //系統LOG檔
                        //================================================= //                     
                        SqlConnection sysconn = dbobj.get_conn("Aitag_DBContext");
                        string sysrealsid = Request["sysrealsid"].ToString();
                        string syssubname = dbobj.get_sysmenuname(sysconn, sysrealsid, "2");
                        string sysnote = "編號:" + chks.mcno + "名稱:" + chks.mititle;
                        dbobj.systemlog(sysconn, syssubname, sysnote, Session["tempid"].ToString(), Session["sldate"].ToString(), Session["sfip"].ToString(), sysflag);
                        sysconn.Close();
                        sysconn.Dispose();
                        //=================================================

                        string tmpform = "";
                        tmpform = "<body onload=qfr1.submit();>";
                        tmpform += "<form name='qfr1' action='/mediaitemtype/List' method='post'>";
                        tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                        tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                        tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                        tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                        tmpform += "<input type=hidden id='qmcno' name='qmcno' value='" + qmcno + "'>";
                        tmpform += "<input type=hidden id='qmititle' name='qmititle' value='" + qmititle + "'>";
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
            { orderdata = "mino"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qmitype = "", qmcno = "", qmititle = "";
            if (!string.IsNullOrWhiteSpace(Request["qmitype"]))
            {
                qmitype = Request["qmitype"].Trim();
                ViewBag.qmitype = qmitype;
            }
            if (!string.IsNullOrWhiteSpace(Request["qmcno"]))
            {
                qmcno = Request["qmcno"].Trim();
                ViewBag.qmcno = qmcno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qmititle"]))
            {

                qmititle = Request["qmititle"].Trim();
                ViewBag.qmititle = qmititle;
            }

            IPagedList<mediaitemtype> result;
            using (AitagBill_DBContext con = new AitagBill_DBContext())
            {
                string sqlstr = "select * from mediaitemtype where";
                if (qmitype != "")
                    sqlstr += " mitype= '" + qmitype + "'  and";
                if (qmcno != "")
                    sqlstr += " mcno like '%" + qmcno + "%'  and";
                if (qmititle != "")
                    sqlstr += " mititle like '%" + qmititle + "%'  and";
            

                sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                sqlstr += " order by " + orderdata + " " + orderdata1;

                var query = con.mediaitemtype.SqlQuery(sqlstr).AsQueryable();

                result = query.ToPagedList<mediaitemtype>(page.Value - 1, (int)Session["pagesize"]);
            
            }
            return View(result);
        }


        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string id, int? page, string orderdata, string orderdata1)
        {
            ModelState.Clear();
            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "mino"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qmcno = "", qmititle = "";
            if (!string.IsNullOrWhiteSpace(Request["qmcno"]))
            {
                qmcno = Request["qmcno"].Trim();
                ViewBag.qmcno = qmcno;
            }
            if (!string.IsNullOrWhiteSpace(Request["qmititle"]))
            {

                qmititle = Request["qmititle"].Trim();
                ViewBag.qmititle = qmititle;
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
                        string ebillsubjects = dbobj.get_dbvalue(conn1, "select mino from mediaitemtype where mino = '" + condtionArr[i].ToString() + "'");

                        sysnote += "代碼:" + ebillsubjects + "<br>";

                        dbobj.dbexecute("AitagBill_DBContext", "DELETE FROM mediaitemtype where mino = '" + condtionArr[i].ToString() + "'");

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
                    tmpform += "<form name='qfr1' action='/mediaitemtype/List' method='post'>";
                    tmpform += "<input type=hidden name='sysflag' id='sysflag' value='" + sysflag + "'>";
                    tmpform += "<input type=hidden name='page' id='page' value='" + page + "'>";
                    tmpform += "<input type=hidden name='orderdata' id='orderdata' value='" + orderdata + "'>";
                    tmpform += "<input type=hidden name='orderdata1' id='orderdata1' value='" + orderdata1 + "'>";
                    tmpform += "<input type=hidden id='qmcno' name='qmcno' value='" + qmcno + "'>";
                    tmpform += "<input type=hidden id='qmititle' name='qmititle' value='" + qmititle + "'>";
                    tmpform += "</form>";
                    tmpform += "</body>";


                    return new ContentResult() { Content = @"" + tmpform };
                }
            }
        }



    }
}
