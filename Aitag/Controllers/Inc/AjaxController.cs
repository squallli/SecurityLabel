using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aitag.Models;
using System.Data;
using System.Data.SqlClient;
using Aitag.Filters;
using MvcPaging;


namespace Aitag.Controllers
{
     public class AjaxController : BaseController
    {
        //
        // GET: /common/

        public ActionResult Index()
        {

            
            return View();
        }



        #region postcode 縣市取得郵區;
        public ActionResult PostCode()
        {
            
            NDcommon dbobj = new NDcommon();
            SqlConnection tmpconn = dbobj.get_conn("Aitag_DBContext");
            //  tmpconn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = tmpconn;
            //抓出郵區資料
            cmd.CommandText = "select * from CodeMap where SubCode = '" + Request["Code"] + "' order by weight";
            SqlDataReader dr = cmd.ExecuteReader();
            string mcontent = "";
            while(dr.Read())
            {
                if (Request["Code1"] != null) {
                    if (Request["Code1"].ToString() == dr["CodeMapId"].ToString()) { 
                    mcontent = mcontent + "<option value='" + dr["CodeMapId"].ToString() + "' selected>" + dr["Description"].ToString() + "</option>";
                    }
                    else
                    {
                    mcontent = mcontent + "<option value='" + dr["CodeMapId"].ToString() + "'>" + dr["Description"].ToString() + "</option>";
                    }
                }
                else
                {
                    mcontent = mcontent + "<option value='" + dr["CodeMapId"].ToString() + "'>" + dr["Description"].ToString() + "</option>";
                }
            }
            ViewBag.mcontent = mcontent;
            dr.Close();
            dr.Dispose();
            cmd = null;
            tmpconn.Close();
            tmpconn.Dispose();
            return View();
        }
        #endregion

        #region getdpt 由公司碼取得部門;
        public ActionResult getdpt()
        {

            NDcommon dbobj = new NDcommon();
            SqlConnection tmpconn = dbobj.get_conn("Aitag_DBContext");
            //  tmpconn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = tmpconn;
            //抓出郵區資料
            cmd.CommandText = "select * from department where comid = '" + Request["comid"] + "' order by dptid";
            SqlDataReader dr = cmd.ExecuteReader();
            string mcontent = "";
            while (dr.Read())
            {
                if (Request["dptval"] != null)
                {
                    if (Request["dptval"].ToString() == dr["dptid"].ToString())
                    {
                        mcontent = mcontent + "<option value='" + dr["dptid"].ToString() + "' selected>" + dr["dpttitle"].ToString() + "</option>";
                    }
                    else
                    {
                        mcontent = mcontent + "<option value='" + dr["dptid"].ToString() + "'>" + dr["dpttitle"].ToString() + "</option>";
                    }
                }
                else
                {
                    mcontent = mcontent + "<option value='" + dr["dptid"].ToString() + "'>" + dr["dpttitle"].ToString() + "</option>";
                }
            }
            ViewBag.mcontent = mcontent;
            dr.Close();
            dr.Dispose();
            cmd = null;
            tmpconn.Close();
            tmpconn.Dispose();
            return View();
        }
        #endregion

        #region gschowner 公司 > 部門 > 員工;
        public ActionResult gschowner()
        {
            #region 準備中
            string selected = "";//是否選擇
            string sql = "", optiontxt = "", optionVal = "", ModelConn = "", ViewBag_outOption = "";
            string[] Codeandpreset;
            string[,] Code = { { "", "" }, { "", "" } };

            Codeandpreset = Request["args"].Split(',');
            List<args> argsList = new List<args>();
            foreach (string i in Codeandpreset)
            {
                argsList.Add(new args() { col = i.Split(':')[0], val = i.Split(':')[1] });
            }
            #endregion


            #region 連動下拉  要改的只有這裡
            switch (argsList.Count)
            {
                case 1:
                    sql += "select comid,comtitle from Company order by comid";
                    optiontxt = "comtitle";
                    optionVal = "comid";
                    ModelConn = "Aitag_DBContext";
                    break;
                case 2:
                    sql += "select dptid, dpttitle from Department where comid='" + argsList[1].val + "'order by dptid";
                    optiontxt = "dpttitle";
                    optionVal = "dptid";
                    ModelConn = "Aitag_DBContext";
                    break;
                case 3:
                    sql += "select * from employee where empworkcomp='" + argsList[1].val + "' and empworkdepid='" + argsList[2].val + "' order by emid";
                    optiontxt = "empname";
                    optionVal = "empid";
                    ModelConn = "Aitag_DBContext";
                    break;
                default:
                    /*傳入參數超過設定值*/
                    break;
            }

            #endregion
            

            NDcommon dbobj = new NDcommon();
            #region 取得option
            ViewBag_outOption = "";
            using (SqlConnection conn = dbobj.get_conn(ModelConn))
            {
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        if (argsList[0].val == dr[optionVal].ToString())
                        {
                            selected = "selected='selected'";
                        }
                        else
                        {
                            selected = "";
                        }
                        ViewBag_outOption += "<option " + selected + " value='" + dr[optionVal] + "' >" + dr[optiontxt] + "</option>";
                    }
                    dr.Close();
                }
            }
            //在前面加一個option 
            if (!string.IsNullOrWhiteSpace(Request["str"]))
            {
                ViewBag_outOption = "<option  value='' selected>" + Request["str"] + "</option>" + ViewBag_outOption;
            }
            #endregion
            ViewBag.option = ViewBag_outOption;

            return View();
        }
        #endregion

        #region ComDepartmentSelect
        //ComDepartmentSelect
        public ActionResult ComDepartmentSelect()
        {
            NDcommon dbobj = new NDcommon();
            string sql = "", optiontxt = "", optionVal = "", ModelConn = "", ViewBag_outOption = "";
            string[] Codeandpreset = { "", "" };
            //string[,] Code = { {"SelectID", "SelectValue"}, {"conditionID", "conditionValue"} };
            string[,] Code = { { "", "" }, { "", "" } };

            Codeandpreset[0] = Request["Code"].Split(',')[0];
            Codeandpreset[1] = Request["Code"].Split(',')[1];
            int n = 0;
            foreach (string i in Codeandpreset)
            {
                Code[n, 0] = i.Split(':')[0];
                Code[n, 1] = i.Split(':')[1];
                n++;
            }
            Console.WriteLine("NewLine: {0}  first line{0}  second line{0}  third line", Environment.NewLine);

            string selected = "";//是否選擇
            //qempworkcomp查詢用 empworkcomp新增修改用
            if ((Code[0, 0]) == "qempworkcomp" || (Code[0, 0]) == "empworkcomp")
            {
                #region 公司 第一層
                sql += "select comid,comtitle from Company order by comid";
                optiontxt = "comtitle";
                optionVal = "comid";
                ModelConn = "Aitag_DBContext";

                #region 取得option
                ViewBag_outOption = "";
                using (SqlConnection conn = dbobj.get_conn(ModelConn))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        SqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            if (Code[0, 1] == dr[optionVal].ToString())
                            {
                                selected = "selected='selected'";
                            }
                            else
                            {
                                selected = "";
                            }
                            ViewBag_outOption += "<option " + selected + " value='" + dr[optionVal] + "' >" + dr[optiontxt] + "</option>";
                        }
                        dr.Close();
                    }
                }
                #endregion

                //在前面加一個option 請選擇
                ViewBag.option = "<option  value='' selected>請選擇</option>" + ViewBag_outOption;

                #endregion
            }
            else if ((Code[0, 0]) == "qempworkdepid" || (Code[0, 0]) == "empworkdepid")
            {
                #region 部門 第二層
                sql += "select dptid, dpttitle from Department where comid='" + Code[1, 1] + "' order by dptid";
                optiontxt = "dpttitle";
                optionVal = "dptid";
                ModelConn = "Aitag_DBContext";

                #region 取得option
                ViewBag_outOption = "";
                using (SqlConnection conn = dbobj.get_conn(ModelConn))
                {
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        SqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            if (Code[0, 1] == dr[optionVal].ToString())
                            {
                                selected = "selected='selected'";
                            }
                            else
                            {
                                selected = "";
                            }
                            ViewBag_outOption += "<option " + selected + " value='" + dr[optionVal] + "' >" + dr[optiontxt] + "</option>";
                        }
                        dr.Close();
                    }
                }
                #endregion

                //在前面加一個option 請選擇
                ViewBag.option = "<option  value='' selected>請選擇</option>" + ViewBag_outOption;
                #endregion
            }
            else
            {
                ViewBag.option = "<option  value='' selected>請選擇</option>";
            }


            return View();
        }
        #endregion
        
     
        #region chooseempno //職務代理人;

        public ActionResult chooseempno(int? page, string orderdata, string orderdata1)
        {

            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "hlogsdate"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string fname = "", tempid = "", tmp1 = "", tmp2 = "", tmp3 = "", tmpsdate = "", tmpedate = "", tmpstime = "", tmpetime = "", qempworkdepid = "", qempname = "";
            if (!string.IsNullOrWhiteSpace(Request["fname"]))
            {
                fname = Request["fname"].Trim();
                ViewBag.fname = fname;
            }
            if (!string.IsNullOrWhiteSpace(Request["tempid"]))
            {
                tempid = Request["tempid"].Trim();
                ViewBag.tempid = tempid;
            }
            if (!string.IsNullOrWhiteSpace(Request["tmp1"]))
            {
                tmp1 = Request["tmp1"].Trim();
                ViewBag.tmp1 = tmp1;
            }
            if (!string.IsNullOrWhiteSpace(Request["tmp2"]))
            {
                tmp2 = Request["tmp2"].Trim();
                ViewBag.tmp2 = tmp2;
            }
            if (!string.IsNullOrWhiteSpace(Request["tmp3"]))
            {
                tmp3 = Request["tmp3"].Trim();
                ViewBag.tmp3 = tmp3;
            }
            if (!string.IsNullOrWhiteSpace(Request["tmpsdate"]))
            {
                tmpsdate = Request["tmpsdate"].Trim();
                ViewBag.tmpsdate = tmpsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["tmpedate"]))
            {
                tmpedate = Request["tmpedate"].Trim();
                ViewBag.tmpedate = tmpedate;
            }
            if (!string.IsNullOrWhiteSpace(Request["tmpstime"]))
            {
                tmpstime = Request["tmpstime"].Trim();
                ViewBag.tmpstime = tmpstime;
            }
            if (!string.IsNullOrWhiteSpace(Request["tmpetime"]))
            {
                tmpetime = Request["tmpetime"].Trim();
                ViewBag.tmpetime = tmpetime;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempworkdepid"]))
            {
                qempworkdepid = Request["qempworkdepid"].Trim();
                ViewBag.qempworkdepid = qempworkdepid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempname"]))
            {
                qempname = Request["qempname"].Trim();
                ViewBag.qempname = qempname;
            }




            IPagedList<employee> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqln = "select * from employee where not empstatus in ('3','4')  and empid <> '" + ViewBag.tempid + "' ";
                sqln += " and empworkcomp='" + (string)Session["comid"] + "'";

                if (qempworkdepid != "")
                {
                    sqln += " and empworkdepid = '" + qempworkdepid + "'";
                }
                if (qempname != "")
                {
                    sqln += " and empname like '%" + qempname + "%'";
                }
                sqln += " order by emid";


                var query = con.employee.SqlQuery(sqln).AsQueryable();

                result = query.ToPagedList<employee>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }

        #endregion

        #region chooseempno1 //挑選申請人;

        public ActionResult chooseempno1(int? page, string orderdata, string orderdata1)
        {

            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "hlogsdate"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string fname = "", tempid = "", tmp1 = "", tmp2 = "", tmp3 = "", tmpsdate = "", tmpedate = "", tmpstime = "", tmpetime = "", qempworkdepid = "", qempname = "";
            if (!string.IsNullOrWhiteSpace(Request["fname"]))
            {
                fname = Request["fname"].Trim();
                ViewBag.fname = fname;
            }
            if (!string.IsNullOrWhiteSpace(Request["tempid"]))
            {
                tempid = Request["tempid"].Trim();
                ViewBag.tempid = tempid;
            }
            if (!string.IsNullOrWhiteSpace(Request["tmp1"]))
            {
                tmp1 = Request["tmp1"].Trim();
                ViewBag.tmp1 = tmp1;
            }
            if (!string.IsNullOrWhiteSpace(Request["tmp2"]))
            {
                tmp2 = Request["tmp2"].Trim();
                ViewBag.tmp2 = tmp2;
            }
             if (!string.IsNullOrWhiteSpace(Request["tmp3"]))
              {
                  tmp3 = Request["tmp3"].Trim();
                  ViewBag.tmp3 = tmp3;
              }
            if (!string.IsNullOrWhiteSpace(Request["tmpsdate"]))
            {
                tmpsdate = Request["tmpsdate"].Trim();
                ViewBag.tmpsdate = tmpsdate;
            }
            if (!string.IsNullOrWhiteSpace(Request["tmpedate"]))
            {
                tmpedate = Request["tmpedate"].Trim();
                ViewBag.tmpedate = tmpedate;
            }
            if (!string.IsNullOrWhiteSpace(Request["tmpstime"]))
            {
                tmpstime = Request["tmpstime"].Trim();
                ViewBag.tmpstime = tmpstime;
            }
            if (!string.IsNullOrWhiteSpace(Request["tmpetime"]))
            {
                tmpetime = Request["tmpetime"].Trim();
                ViewBag.tmpetime = tmpetime;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempworkdepid"]))
            {
                qempworkdepid = Request["qempworkdepid"].Trim();
                ViewBag.qempworkdepid = qempworkdepid;
            }
            if (!string.IsNullOrWhiteSpace(Request["qempname"]))
            {
                qempname = Request["qempname"].Trim();
                ViewBag.qempname = qempname;
            }




            IPagedList<employee> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqln = "select * from employee where not empstatus in ('3','4') and empid <> '" + ViewBag.empid + "' ";
                sqln += " and empworkcomp='" + (string)Session["comid"] + "'";

                if (qempworkdepid != "")
                {
                    sqln += " and empworkdepid = '" + qempworkdepid + "'";
                }
                if (qempname != "")
                {
                    sqln += " and empname like '%" + qempname + "%'";
                }
                sqln += " order by emid";


                var query = con.employee.SqlQuery(sqln).AsQueryable();

                result = query.ToPagedList<employee>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }

        #endregion

        #region chooseemployee    挑選角色  (知會人員)

        public ActionResult chooseemployee(int? page, string orderdata, string orderdata1)
        {

            page = ((!page.HasValue || page < 1) ? 1 : page);
            ViewBag.page = page;
            if (string.IsNullOrWhiteSpace(orderdata))
            { orderdata = "hlogsdate"; }

            if (string.IsNullOrWhiteSpace(orderdata1))
            { orderdata1 = "desc"; }
            ViewBag.orderdata = orderdata;
            ViewBag.orderdata1 = orderdata1;
            string qempworkcomp = "", qempworkdepid = "", qempname = "";
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
            if (!string.IsNullOrWhiteSpace(Request["qempname"]))
            {
                qempname = Request["qempname"].Trim();
                ViewBag.qempname = qempname;
            }

            string backlman = "", backotherman = "", sysflag = "";
            if (!string.IsNullOrWhiteSpace(Request["backlman"]))
            {
                backlman = Request["backlman"].Trim();
                ViewBag.backlman = backlman;
            }
            if (!string.IsNullOrWhiteSpace(Request["backotherman"]))
            {
                backotherman = Request["backotherman"].Trim();
                ViewBag.backotherman = backotherman;
            }
            if (!string.IsNullOrWhiteSpace(Request["sysflag"]))
            {
                sysflag = Request["sysflag"].Trim();
                ViewBag.sysflag = sysflag;
            }


            IPagedList<employee> result;
            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                string sqln = "select * from employee where empstatus<>'4'";
                //sqln += " and empworkcomp='" + (string)Session["comid"] + "'";

                if (qempworkcomp != "")
                {
                    sqln += " and empworkcomp = '" + qempworkcomp + "'";
                }
                if (qempworkdepid != "")
                {
                    sqln += " and empworkdepid = '" + qempworkdepid + "'";
                }
                if (qempname != "")
                {
                    sqln += " and empname like '%" + qempname + "%'";
                }
                sqln += " order by emid";


                var query = con.employee.SqlQuery(sqln).AsQueryable();

                result = query.ToPagedList<employee>(page.Value - 1, (int)Session["pagesize"]);

            }
            return View(result);
        }

        #endregion


        #region ajaxdropbox 共用ajaxdropbox ;
        public ActionResult ajaxdropbox()
        {
            string sqldata = Request["sqldata"].ToString();
            string valname = Request["valname"].ToString();
            string showname = Request["showname"].ToString();
            string ajaxdiv = Request["ajaxdiv"].ToString();
            NDcommon dbobj = new NDcommon();
            SqlConnection tmpconn = dbobj.get_conn("AitagBill_DBContext");
            //  tmpconn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = tmpconn;

            cmd.CommandText += sqldata ;
            SqlDataReader dr = cmd.ExecuteReader();
            string mcontent = "<table border=0 cellpadding=5 class=divdd width=90% id=mtb>";
          //  mcontent = mcontent + "<tr><td>ID</td><td>媒體名稱</td></tr>";
            int j = 1;
            while (dr.Read())
            {
                if(j==1)
                {
                    mcontent = mcontent + "<tr bgcolor='#ddeeff' style='cursor:pointer' id='idarray" + valname + j + "' name='" + dr[0].ToString().Trim() + "," + dr[1].ToString().Trim().Replace("\r", "") + "' onclick=\"$('#" + valname + "').val('" + dr[0].ToString().Trim() + "');$('#" + showname + "').val('" + dr[1].ToString().Trim().Replace("\r","") + "');$('#" + ajaxdiv + "').css('display','none');\"><td>" + dr[0].ToString().Trim() + "</td><td>" + dr[1].ToString().Trim().Replace("\r", "") + "</td></tr>";
                }
                else {
                    mcontent = mcontent + "<tr bgcolor='#ffffff' style='cursor:pointer' id='idarray" + valname + j + "' name='" + dr[0].ToString().Trim() + "," + dr[1].ToString().Trim().Replace("\r", "") + "' onclick=\"$('#" + valname + "').val('" + dr[0].ToString().Trim() + "');$('#" + showname + "').val('" + dr[1].ToString().Trim().Replace("\r", "") + "');$('#" + ajaxdiv + "').css('display','none');\"><td>" + dr[0].ToString().Trim() + "</td><td>" + dr[1].ToString().Trim().Replace("\r", "") + "</td></tr>";
                }
                    //mcontent = mcontent + "<li>" + dr["mdno"].ToString().Trim() + "-" + dr["mdtitle"].ToString().Trim() + "</li>";
                j++;
            }
            mcontent += "</table>";
            ViewBag.mcontent = mcontent;
            dr.Close();
            dr.Dispose();
            cmd = null;
            tmpconn.Close();
            tmpconn.Dispose();
            return View();

        }
        #endregion



    }
     public class args
     {
         public string col= "";
         public string val = "";
     }

     

}
