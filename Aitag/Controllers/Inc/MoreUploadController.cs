using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Collections;
using Aitag.Models;
using Aitag.Filters;


namespace Aitag.Controllers
{
    public class MoreUploadController : BaseController
    {
        //
        // GET: /main/
        //public string Imglink = System.Configuration.ConfigurationManager.AppSettings["Imgurl"].ToString();
        public string Imglink = "";
        public ActionResult Index()
        {
            ViewBag.realsid = Request["realsid"].ToString();
            return View();
        }

        public ActionResult billindex()
        {
           // ViewBag.realsid = Request["realsid"].ToString();
            return View();
        }

        public ActionResult fileupload(HttpPostedFileBase aimg)
        {
            int filesize = aimg.ContentLength;
            string filename = aimg.FileName;
            string vfilename = aimg.FileName;
            string filetype = aimg.FileName.Split('.')[1].ToString();
            string fileERROR = "";
            string[] tmp= vfilename.Split('/');
            vfilename = tmp[tmp.Length - 1];
            if (filetype=="ASP"||filetype=="BAK"||filetype=="EXE"||filetype=="SPX"){
                fileERROR = "抱歉！上傳檔案格式錯誤，請重新上傳！";
            }
            else{
                aimg.SaveAs(Server.MapPath(Imglink+"/upload/") + filename);
            }
            string tmpdata = "";
            tmpdata+="{\"files\":[";
            tmpdata+="{\"name\":\"" + filename + "\",";
            tmpdata+="\"size\":" + filesize + ",";
            tmpdata += "\"type\":\"" + filetype + "\",";
            tmpdata += "\"url\":\"" + "" + "\",";
            tmpdata += "\"thumbnailUrl\":\"" + "" + "\",";
            tmpdata += "\"fileERROR\":\"" + fileERROR + "\",";
            tmpdata += "\"deleteUrl\":\"" + "" + "\",";
            tmpdata += "\"deleteType\":\"DELETE\"}";
            tmpdata+="]}";
            return new ContentResult() { Content = @"" + tmpdata };
        }

        public ActionResult fileuploadDB()
        {
            string realsid = Request["realsid"].ToString();
            string mcid = Request["mcid"].ToString();
            if(mcid == "")
            {
                mcid = Session["mcid"].ToString(); 
            }
            string qmcparentid = Request["qmcparentid"].ToString();
           
            string cfilename = Request["cfilename"].ToString();
            string vfilename = Request["vfilename"].ToString();
            string cfilesize = Request["cfilesize"].ToString();
            NDcommon dbobj = new NDcommon();


            System.IO.File.Copy(Server.MapPath(Imglink + "/upload/" + cfilename), Server.MapPath(Imglink + "/upload/" + vfilename));
            System.IO.File.Delete(Server.MapPath(Imglink + "/upload/" + cfilename));

            using (Aitag_DBContext con = new Aitag_DBContext())
            {
                SqlConnection conn = dbobj.get_conn("Aitag_DBContext");
                SqlDataReader dr;
                SqlCommand sqlsmd = new SqlCommand();
                sqlsmd.Connection = conn;
                //取得功能代號
		
                string sqlstr = "select * from sublevel1 where sid = '" + realsid + "'";
                sqlsmd.CommandText = sqlstr;
                dr = sqlsmd.ExecuteReader();
                string functype = "";
                if(dr.Read())
                {
                    functype = dr["functype"].ToString();
                }
               
                dr.Close();
                dr.Dispose();

                if(mcid == ""){
                    maincontent mainobj = new maincontent();
                            
			        if(qmcparentid!= "")
			        {
                        mainobj.mcparentid = int.Parse(qmcparentid);
                    }
			        mainobj.mctitle = "";
			        mainobj.mchttp = "";
			        mainobj.mctype = functype;
			        mainobj.sid = int.Parse(realsid) ;
			        mainobj.readallman = "" ;
                    mainobj.mdate = DateTime.Today ;
                    mainobj.mclick = 0 ;
                    mainobj.ownman = Session["empid"].ToString() ;
                    mainobj.comid = Session["comid"].ToString() ;
                    mainobj.bmodid = Session["empid"].ToString() ;
			        mainobj.bmoddate = DateTime.Today ;
			       	con.maincontent.Add(mainobj);
                    con.SaveChanges();
		
			        sqlstr = "select top 1 * from maincontent where ownman = '" + Session["empid"].ToString() + "' order by mcid desc";
			        sqlsmd.CommandText = sqlstr;
                    dr = sqlsmd.ExecuteReader();
                    if(dr.Read())
                    {
                        mcid = dr["mcid"].ToString();
                    }
               
                    dr.Close();
                    dr.Dispose();
			       
			        Session["mcid"] = mcid ;
		        }

                conn.Close();
                conn.Dispose();

                contupload addobj = new contupload();
                addobj.cfilename = cfilename;
                addobj.cfilesize = int.Parse(cfilesize);
                addobj.mcid = int.Parse(mcid);
                addobj.cfiletitle = cfilename;
                addobj.cupfile = vfilename;
               
                addobj.bmodid = Session["empid"].ToString();
                addobj.bmoddate = DateTime.Now;

                con.contupload.Add(addobj);
                con.SaveChanges();
                con.Dispose();
            }
            string tmpdata = mcid;

            return new ContentResult() { Content = @"" + tmpdata };
        }


        //內容
        public ActionResult main01(string id)
        {
            ViewBag.codeid = id;
            return View();
        }
        
        
    }
}
