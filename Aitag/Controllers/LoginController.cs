using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Aitag.Models;
using WebMatrix.WebData;


namespace Aitag.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Checkcode/

        // GET: /Login/
        [HttpGet]
        public ActionResult Index(string returnUrl)
        {
                       
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        
        [HttpGet]
        public ActionResult goout()
        {
            return new ContentResult() { Content = @"<script>alert('作業已逾時，請重新登入!!');location.href='/Login'</script>" };
        }

       
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(employee col,Logoin model, string UserName, string Password)
        {

            ModelState.Clear();      

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {

                NDcommon dbobj = new NDcommon();

                SqlConnection conn = dbobj.get_conn("Aitag_DBContext");
                SqlConnection tmpconn = dbobj.get_conn("Aitag_DBContext");

                SqlDataReader dr;
                SqlCommand sqlsmd = new SqlCommand();
                sqlsmd.Connection = conn;
                //加密比對登入
                //string sqlstr = "select * from Company where comid = '" + UserName.Trim() + "'  and ifstop='y'";
                //一般沒加密登入
                //string sqlstr = "select * from Company where comid = '" + UserName.Trim() + "' and compwd='" + Password.Trim() + "' and ifstop='y'";
                string sqlstr = "select * from Employee where empid = '" + UserName.Trim() + "' and emppasswd ='" + Password.Trim() + "' and empstatus<>'4'and ifuse = 'y'";
                sqlsmd.CommandText = sqlstr;
                dr = sqlsmd.ExecuteReader();

                if (dr.Read())
                {
                    Session["comcon"] = dr["comcon"].ToString().Replace("#","\'");
                    Session["tempid"] = dr["empid"].ToString();
                    Session["empid"] = dr["empid"].ToString();
                    Session["tempname"] = dr["empname"].ToString();
                    Session["empname"] = dr["empname"].ToString();
                    Session["Dptid"] = dr["empworkdepid"].ToString();
                    Session["comid"] = dr["empworkcomp"].ToString();
                    Session["Msid"] = dr["Msid"].ToString();                   
                    Session["Dpttitle"] = dbobj.get_dbvalue(tmpconn, "select dpttitle from department where dptid = '" + dr["empworkdepid"].ToString() + "'");                
                    Session["sldate"] = DateTime.Now;
                    Session["sfip"] = Request.ServerVariables["REMOTE_ADDR"].ToString();
                    Session["pagesize"] = 20;
                    Session["epagesize"] = 100;
                    Session["mcid"] = "";
                    Session["mtid"] = dr["etab"].ToString();
                    Session["logopic"] = dbobj.get_dbvalue(tmpconn, "select logopic from company where comid = '" + dr["empworkcomp"].ToString() + "'");                
                    dr.Close();

                    //抓最大角色
                    sqlstr = "select  TOP (1) rid from emprole where empid = '" + Session["tempid"].ToString().Trim() + "' order by rid";
                    sqlsmd.CommandText = sqlstr;
                    dr = sqlsmd.ExecuteReader();
                    while (dr.Read())
                    {
                         Session["rid"] = dr["rid"].ToString();
                         Session["mplayrole"] = "'" + dr["rid"].ToString() + "'";
                    }
                    dr.Close();
                    string[] privtb = new string[999];
                    for (int i = 0; i < 999; i++)
                    {
                        privtb[i] = "0";
                    }
                    sqlstr = "select * from privtb where bid = '" + UserName.Trim() + "'";
                    sqlsmd.CommandText = sqlstr;
                    dr = sqlsmd.ExecuteReader();
                    while (dr.Read())
                    {
                        privtb[int.Parse(dr["sid"].ToString())] = "1";
                    }
                    Session["privtb"] = privtb;
                    dr.Close();
                    dr.Dispose();
                    return RedirectToAction("Index", "main");
                }
                else
                {
                    String wrongID = "查無此帳號資訊!!";
                    //if (Session["comclass"] == "2")//國外
                    //{
                    //    wrongID = "Please check your userid and password";
                    //}
                    //ModelState.AddModelError("", wrongID);
                    dr.Close();
                    dr.Dispose();
                }

                sqlsmd.Dispose();
                conn.Close();
                conn.Dispose();
                tmpconn.Close();
                tmpconn.Dispose();
                dbobj = null;



            }


            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            // ModelState.AddModelError("", "所提供的使用者名稱或密碼不正確。");
           // return View(model);
            return new ContentResult() { Content = @"<script>alert('使用者帳號密碼不正確或停止使用!!');location.href='/'</script>" };
        }

        
         public ActionResult mvclogin()
        {
                string mtid = Request["mtid"];
                string empid = Request["empid"];
                NDcommon dbobj = new NDcommon();

                SqlConnection conn = dbobj.get_conn("Aitag_DBContext");
                SqlConnection tmpconn = dbobj.get_conn("Aitag_DBContext");

                SqlDataReader dr;
                SqlCommand sqlsmd = new SqlCommand();
                sqlsmd.Connection = conn;
                //加密比對登入
                //string sqlstr = "select * from Company where comid = '" + UserName.Trim() + "'  and ifstop='y'";
                //一般沒加密登入
                //string sqlstr = "select * from Company where comid = '" + UserName.Trim() + "' and compwd='" + Password.Trim() + "' and ifstop='y'";
                string sqlstr = "select * from Employee where empid = '" + empid.Trim() + "' and empstatus<>'4'and ifuse = 'y'";
                sqlsmd.CommandText = sqlstr;
                dr = sqlsmd.ExecuteReader();

                if (dr.Read())
                {
                    Session["tempid"] = dr["empid"].ToString();
                    Session["empid"] = dr["empid"].ToString();
                    Session["tempname"] = dr["empname"].ToString();
                    Session["empname"] = dr["empname"].ToString();
                    Session["Dptid"] = dr["empworkdepid"].ToString();
                    Session["comid"] = dr["empworkcomp"].ToString();
                    Session["Msid"] = dr["Msid"].ToString();
                    Session["Dpttitle"] = dbobj.get_dbvalue(tmpconn, "select dpttitle from department where ID = '" + dr["empworkdepid"].ToString() + "'");
                    Session["sldate"] = DateTime.Now;
                    Session["sfip"] = Request.ServerVariables["REMOTE_ADDR"].ToString();
                    Session["pagesize"] = 20;
                    Session["mtid"] = dr["etab"].ToString();
                    Session["logopic"] = dbobj.get_dbvalue(tmpconn, "select logopic from company where comid = '" + dr["empworkcomp"].ToString() + "'");                
                    dr.Close();

                    //抓最大角色
                    sqlstr = "select TOP (1) rid from emprole where empid = '" + Session["tempid"].ToString().Trim() + "' order by rid";
                    sqlsmd.CommandText = sqlstr;
                    dr = sqlsmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Session["rid"] = dr["rid"].ToString();
                        Session["mplayrole"] = "'" + dr["rid"].ToString() +"'";
                    }
                    dr.Close();

                    string[] privtb = new string[999];
                    for (int i = 0; i < 999; i++)
                    {
                        privtb[i] = "0";
                    }
                    sqlstr = "select * from privtb where bid = '" + empid.Trim() + "'";
                    sqlsmd.CommandText = sqlstr;
                    dr = sqlsmd.ExecuteReader();
                    while (dr.Read())
                    {
                        privtb[int.Parse(dr["sid"].ToString())] = "1";
                    }
                    Session["privtb"] = privtb;
                    dr.Close();
                    dr.Dispose();
                    return new ContentResult() { Content = @"<script>location.href='/main'</script>" };
                }
               
                sqlsmd.Dispose();
                conn.Close();
                conn.Dispose();
                tmpconn.Close();
                tmpconn.Dispose();
                dbobj = null;



                 // 如果執行到這裡，發生某項失敗，則重新顯示表單
            // ModelState.AddModelError("", "所提供的使用者名稱或密碼不正確。");
            // return View(model);
            return new ContentResult() { Content = @"<script>alert('使用者帳號密碼不正確或停止使用!!');location.href='/'</script>" };
        }


       

        [HttpGet]
        public ActionResult logout()
        {
            string strlink = "/login";

            String logoutStr = "登出聯廣集團營運管理系統!!";
            
            Session["tempid"] = null;
            Session["Dptid"] = null;
            Session["Msid"] = null;
            Session["Dpttitle"] = null;
            Session["sldate"] = null;
            Session["sfip"] = null;
            Session["pagesize"] = null;
            Session["tempname"] = null;
            Session["rid"] = null;
            Session["mplayrole"] = null;

            return new ContentResult() { Content = @"<script>alert('" + logoutStr + "!!');location.href='" + strlink + "'</script>" };
        }
    }
}
