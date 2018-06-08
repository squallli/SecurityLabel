using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aitag.Models;
using System.Data;
using System.Data.SqlClient;
using Aitag.Filters;


namespace Aitag.Controllers
{
     [DoAuthorizeFilter]
     public class commonController : BaseController
    {
        //
        // GET: /common/

        public ActionResult Index()
        {

            
            return View();
        }

        public ActionResult menugo(string id)
        {
            Aitag.Models.NDcommon dbobj = new Aitag.Models.NDcommon();
            SqlConnection comconn = dbobj.get_conn("Aitag_DBContext");
            string prog = dbobj.get_dbvalue(comconn, "select location from menutab where mtid = '" + id + "'");
            Session["mtid"] = id;
            string sql = "SELECT sublevel1.* FROM privtb LEFT OUTER JOIN sublevel1 ON privtb.sid = sublevel1.sid where sublevel1.mtid = '" + id + "' and privtb.bid='" + Session["empid"] + "' and privtb.chk='1' order by sublevel1.lid , sublevel1.corder , sublevel1.sid";
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = comconn;
            cmd.CommandText = sql;
            SqlDataReader tmprs = cmd.ExecuteReader();
            if(tmprs.Read())
            {
               string location = tmprs["location"].ToString().Trim();
               string p1 = tmprs["uplink"].ToString().Trim();
               string p2 = tmprs["sid"].ToString().Trim();
               if(location.IndexOf('#') >= 0)
               {
                   sql = "SELECT sublevel1.location,sublevel1.sid FROM privtb LEFT OUTER JOIN sublevel1 ON privtb.sid = sublevel1.sid where sublevel1.uplink = '" + tmprs["sid"].ToString() + "' and sublevel1.lid='3' and privtb.bid='" + Session["empid"].ToString() + "' and privtb.chk='1' order by sublevel1.corder , sublevel1.sid" ;
                   tmprs.Close();
                   tmprs.Dispose();
                   cmd.CommandText = sql;
                   tmprs = cmd.ExecuteReader();
                   if(tmprs.Read())
                   {
                     location = tmprs["location"].ToString().Trim();
                     p1 = p2;
                     p2 = tmprs["sid"].ToString().Trim();
                   }
                   tmprs.Close();
                   tmprs.Dispose();
               }
               else
               {
                   tmprs.Close();
                   tmprs.Dispose();
               }

               if (location.IndexOf('?') >= 0)
               {
                   ViewBag.js = @"location.href='" + location + "';";
                   return View();
               }
               else
               {
                   ViewBag.js = @"location.href='" + location + "?sid=" + p1 + "&realsid=" + p2 + "';";
                   return View();
               }
 
            }
            else
            {
                return new ContentResult() { Content = @"<script>alert('您並無此選單的使用權限!!');window.history.go(-1);</script>" };
            }
            comconn.Close();
            comconn.Dispose();
          
        }

        public ActionResult newlogin(string comidrid, string comid)
        {
            string tmppath1="";
            string tmpsid="";
            string tmprealsid="";
            
            tmppath1 = Request["path1"];
            tmpsid = Request["sid"];
            tmprealsid = Request["realsid"];

            if (!string.IsNullOrWhiteSpace(comidrid))
            {
              Session["rid"]=comidrid;
              Session["mplayrole"]= "'" + comidrid +"'";
            }
             
            if (!string.IsNullOrWhiteSpace(comid))
            {
              Session["comid"]=comid;
              NDcommon dbobj = new NDcommon();

              SqlConnection tmpconn = dbobj.get_conn("Aitag_DBContext");
              Session["logopic"] = dbobj.get_dbvalue(tmpconn, "select logopic from company where comid = '" + comid + "'");
              tmpconn.Close();
              tmpconn.Dispose();
            }

            if (tmppath1 == "" || tmppath1==null)
            {
                if (Session["mtid"] == "A0032")
                { return new ContentResult() { Content = @"<script>location.href='/paybill'</script>" }; }
                else
                { return new ContentResult() { Content = @"<script>location.href='/main'</script>" }; }
            }
            else
            {
                if (tmpsid == "510")
                { Session["mtid"] = "A0032";}
                else
                { Session["mtid"] = "A004";}
               
                Session["sid"] = tmpsid;
                Session["realsid"] = tmprealsid;
                
                return new ContentResult() { Content = @"<script>location.href='/" + tmppath1 + "'</script>" };
            }
            //return View();
        }

         

    }
}
