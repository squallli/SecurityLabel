using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Linq;
using System.Collections.Generic;

public class funGetData{
    //資料庫連線設定
    public SqlConnection get_conn(string tmpconnstr) {
       
        string connstr = ConfigurationManager.AppSettings[tmpconnstr] ;//新版的用法
        SqlConnection tmpconn = new SqlConnection() ;
        tmpconn.ConnectionString = connstr;
        try{
            tmpconn.Open();
        }
        catch{;}
        
        return  tmpconn ;
     }

     public SqlCommand get_cmd(string tmpconnstr,string sqltext) 
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.AppSettings[tmpconnstr] ;
            conn.Open();

            SqlCommand tmpsqlcmd = new SqlCommand();
            tmpsqlcmd.Connection = conn;
            tmpsqlcmd.CommandText = sqltext;
            return  tmpsqlcmd;
         }

    //找sqlcommand物件,由物件作ExecuteNoQuery , ExecuteReader , ExecuteScalar
    public string get_dbvalue(SqlConnection tmpconn , string sqltext) 
    {
        
        SqlCommand tmpsqlcmd = new SqlCommand();
        tmpsqlcmd.Connection = tmpconn;
        tmpsqlcmd.CommandText = sqltext;
        string tmpdata = (string)tmpsqlcmd.ExecuteScalar();
        string tmpcol = (string)get_dbnull(tmpdata) ;
        tmpsqlcmd.Dispose();

        return tmpcol;
        
    }
    //找主功能頁名稱
    public string get_menuname(SqlConnection tmpconn , string tmpsid , string tmptype )
    {
        //tmptype =1(不傳link)  = 2(傳link)
        SqlCommand tmpsqlcmd = new SqlCommand();
        SqlDataReader tmprs ;
        string tmpUplinkid, tmpUplinkname ;
        string tmpSidid, tmpSublevelname, tmpLocation;
        string sqlstr = "";
        sqlstr =  "select Uplink,Sublevelname,Location,Uplink,Sid from Sublevel where Sid = '" + tmpsid + "'";
        tmpsqlcmd.Connection = tmpconn ;
        tmpsqlcmd.CommandText = sqlstr ;
        try{
            tmprs = tmpsqlcmd.ExecuteReader();
            tmprs.Read();
            tmpSidid = (string)tmprs["Sid"] ;
            tmpSublevelname = (string)tmprs["Sublevelname"];
            tmpUplinkid = (string)tmprs["Uplink"] ;
            tmpLocation = (string)tmprs["Location"];
            tmprs.Close();
            if(tmptype == "1")
            {
                return tmpSublevelname;
            }
            else
            { 
                tmpUplinkname = get_dbvalue(tmpconn, "select Sublevelname from Sublevel where Sid='" + tmpUplinkid + "'") ;
                return "<font style='color:#A67070;font-size:11pt;font-weight: bold'>" + tmpUplinkname + "</font> > <a href='" + tmpLocation + "?sid=" + tmpUplinkid + "&realsid=" + tmpSidid + "'><font style='color:#A67070;font-size:11pt;font-weight: bold'>" + tmpSublevelname + "</font></a>";
            }   
                //get_menuname = tmpSublevelname
           
        }
        catch
        {
            return "";
        }
        
    }

    //找選單Javascript
    public string get_menuscript(SqlConnection tmpconn, string[] tmppriv) 
    {
        SqlCommand tmpsqlcmd = new SqlCommand();
        SqlDataReader tmprs ;
        string tmpscript  = "";
        int i = 0;
        tmpsqlcmd.Connection = tmpconn;
        tmpsqlcmd.CommandText = " select * from Sublevel order by Uplink , Lid , Sid";
        tmpscript = tmpscript + "<script language=javascript>\n\n"  ;
        tmpscript = tmpscript + " function menushow(tmpvalue2 , x , y ) {\n\n"; 
        tmpscript = tmpscript + "   var strtmp = '' \n\n" ;
        tmpscript = tmpscript + "   var j = 0\n\n";
        tmpscript = tmpscript + " strtmp = '<table id=dvvvv cellpadding=6 cellspacing=0 border=1 bgcolor=#DBDBDB bordercolorlight=LightGrey bordercolordark=ffffff class=cmenu style=border-collapse:collapse;>' + \n\n" ;
        try{
            tmprs = tmpsqlcmd.ExecuteReader();
            i = 1;
            while(tmprs.Read())
            {
                if(tmprs["Sid"] != tmprs["Uplink"])
                {
                    tmpscript = tmpscript + "   if (tmpvalue2=='" + tmprs["Uplink"] + "')\n\n";
                    tmpscript = tmpscript + "   {\n\n"; 
                    if(tmppriv[int.Parse(tmprs["Sid"].ToString())].ToString() == "1")
                    {
                        tmpscript = tmpscript + " strtmp = strtmp + '<tr><td id=dvvvvvvvv onmouseover=javascript:this.style.background=\'#ffffe0\' onmouseout=javascript:this.style.background=\'#DBDBDB\'> <a id=dvvv href=" + tmprs["Location"].ToString() + "?sid=" + tmprs["Uplink"] + "&realsid=" + tmprs["Sid"] + "><font color=#000000 id=idddv>" + tmprs["Sublevelname"] + "</font></a></td></tr>\n\n'" ;
                    }
                    else
                    { tmpscript = tmpscript + tmppriv[int.Parse(tmprs["Sid"].ToString())].ToString() + "\n\n";}
                    
                    tmpscript = tmpscript + " j = 1 }\n\n" ;
                }
                i = i + 1;
            }
            tmprs.Close();
            tmpscript = tmpscript + "    if(j == 1)\n\n";
            tmpscript = tmpscript + "    strtmp = strtmp + '</table>'\n\n"; 
            tmpscript = tmpscript + "   dv1.style.posTop = y\n\n";  
            tmpscript = tmpscript + "   var tmpx = 0\n\n"; 
            tmpscript = tmpscript + "   tmpx = window.screen.width - 1024\n\n" ;
            tmpscript = tmpscript + "   dv1.style.posLeft = x + tmpx/2\n\n";
            tmpscript = tmpscript + "   dv1.innerHTML = strtmp\n\n" ;
            tmpscript = tmpscript + "       selectchange('2')\n\n" ;
            tmpscript = tmpscript + "       }\n\n"; 
            tmpscript = tmpscript + "</script>\n\n" ;
            return tmpscript;
            
        }
        catch
        { 
            return "";
        }
        
    }
    
    //找主功能頁連結路徑
    public string get_gomain(SqlConnection tmpconn , string tmpsid ) 
    {
        SqlCommand tmpsqlcmd =  new SqlCommand();
        SqlDataReader tmprs ;
        tmpsqlcmd.Connection = tmpconn ;
        tmpsqlcmd.CommandText = "select * from Sublevel where Sid = '" + tmpsid + "'" ;
        try{
            tmprs = tmpsqlcmd.ExecuteReader() ;
            tmprs.Read();
            return tmprs["Location"] + "?sid=" + tmprs["Uplink"].ToString().Trim() + "&realsid=" + tmprs["Sid"].ToString().Trim() ;
        }catch{
            return "";
        }
    }

    //找維護者姓名　
    public string get_name(SqlConnection tmpconn , string tmpbid )
    {
        SqlCommand tmpsqlcmd = new SqlCommand();
        tmpsqlcmd.Connection = tmpconn ;
        tmpsqlcmd.CommandText = "select empname from employee where empid = '" + tmpbid + "'";
       try{
            tmpsqlcmd.ExecuteScalar() ;
            return tmpsqlcmd.ExecuteScalar().ToString() ;
       }catch{
            return "" ;
       }
    }

    //取得 Select的object
    public string get_selectobj(SqlConnection tmpconn, string tmpsql, string selname, string compare_str, string ifchoose) 
    {
        SqlCommand tmpsqlcmd = new SqlCommand();
        tmpsqlcmd.Connection = tmpconn;
        tmpsqlcmd.CommandText = tmpsql ;
         SqlDataReader tmprs  = tmpsqlcmd.ExecuteReader();
        string tmpstr = "";
        tmpstr = tmpstr + "<select name=" + selname + " class=c12>\n\n" ;
        
        if(ifchoose== "1")
        {
        tmpstr = tmpstr + "<option value=''>請選擇</option>\n\n"; 
        }
        
        while(tmprs.Read())
        {
            if(tmprs[0] == compare_str)
            {
                 tmpstr = tmpstr + "<option value='" + tmprs[0].ToString().Trim() + "' selected>" + tmprs[1].ToString().Trim() + "</option>\n\n" ;
            }
            else
            {
                 tmpstr = tmpstr + "<option value='" + tmprs[0].ToString().Trim() + "' >" + tmprs[1].ToString().Trim() + "</option>\n\n" ;
            }
        }
        　　tmpstr = tmpstr + "</select>\n\n" ;
        
        return tmpstr;
        
    }

    //取得checkboxobj
    public string get_checkboxobj(SqlConnection tmpconn　, string tmpsql　, string checkname, string compare_str)
    {
        SqlCommand tmpsqlcmd = new SqlCommand();
        tmpsqlcmd.Connection = tmpconn;
        tmpsqlcmd.CommandText = tmpsql ;
        SqlDataReader tmprs  = tmpsqlcmd.ExecuteReader();
        //string tmparr = new string[];
        string tmpstr = "";
        List<string> tmparr = new List<string>();
        tmparr = compare_str.Split(',').ToList();
        
        int j = 0 ;
        while(tmprs.Read())
        {
            if(tmparr.Count > 0)
            {
                if (tmparr[j] == tmprs[0])
                { 
                    tmpstr = tmpstr + "<input type=checkbox name='" + checkname + "' value='" + tmprs[0] + "' checked> " + tmprs[1] + "\n\n";
                    if(j < tmparr.Count - 1 )
                    {j = j + 1;}
                }
                else
                {
                tmpstr = tmpstr + "<input type=checkbox name='" + checkname + "' value='" + tmprs[0] + "' > " + tmprs[1] + "\n\n";
                }
            }
            else
            {
            tmpstr = tmpstr + "<input type=checkbox name='" + checkname + "' value='" + tmprs[0] + "' > " + tmprs[1] + " \n\n" ;
            }
        }
         return tmpstr;
    }  

    public string get_radioobj(SqlConnection tmpconn　, string tmpsql, string radioname, string compare_str) 
        {
             SqlCommand tmpsqlcmd = new SqlCommand();
            tmpsqlcmd.Connection = tmpconn;
            tmpsqlcmd.CommandText = tmpsql ;
            SqlDataReader tmprs  = tmpsqlcmd.ExecuteReader();
            string tmpstr = "";
            while(tmprs.Read())
            {
             if(tmprs[0] == compare_str)
               {
                tmpstr = tmpstr + "<input type=radio name='" + radioname + "' value='" + tmprs[0] + "' checked> " + tmprs[1] + "\n\n " ;
                }else{
                tmpstr = tmpstr + "<input type=radio name='" + radioname + "' value='" + tmprs[0] + "' > " + tmprs[1] + " \n\n" ;
             }
           
            }

            return tmpstr;

        }

    
    //日期格式 ttype = 1 西元, 2民國,3前台用
    //public string get_date(string tmpdate , string ttype )
    //{
    //    string tmpdate1  = "" ;
    //    string tmpmonth  = "" ;
    //    string tmpday  = "";
    //    if(get_dbnull(tmpdate)!="")
    //    {
    //        tmpmonth = Replace(Space(2 - Len(Trim(Month(tmpdate)))), " ", "0") + Trim(Month(tmpdate)) ;            tmpday = Replace(Space(2 - Len(Trim(Day(tmpdate)))), " ", "0") & Trim(Day(tmpdate)) ;
    //        if(ttype == "1")
    //        {tmpdate1 = Year(tmpdate) + "/" + tmpmonth + "/" + tmpday ;}
    //        else
    //        {
    //            if(ttype=="3")  //前台用
    //            {
    //                tmpdate1 = Year(tmpdate) + "-" + tmpmonth + "-" + tmpday;
    //            }
    //            else
    //            {
    //                tmpdate1 = Year(tmpdate) - 1911 + "/" + tmpmonth + "/" + tmpday;
    //            }
    //        }
    //        return tmpdate1;
    //    }else{
    //        {return "";}
    //    }
    //}
    
        //取得 DBNull的比對
    public string get_dbnull(string tmpnull){
        if(System.DBNull.Value.Equals(tmpnull))
        {return "" ;}
        else
        {return tmpnull ;}
     }

     /*===本函數需傳入6個參數
        '===1.System.Web.UI.WebControls.FileUpload檔案上傳物件
        '===2.儲存路徑
        '===3.檔名
        '===4.整個Page
        '===5.照片的寬度
        '===6.照片的高度
        */
    public string[] FunUpLoadPic(System.Web.UI.WebControls.FileUpload Up  , string SaveStr , string FName , int SWidth , int SHeight ) 
    {
       
          string[] tmparr = new string[20] ;
        //' MsgBox(Up.PostedFile.FileName)

        if(Up.PostedFile.FileName!= null)
        {
            System.IO.Stream Fs  = Up.PostedFile.InputStream ; //'===宣告資料流
            int PicValue ; // '===宣告縮放比例
            Up.PostedFile.SaveAs(SaveStr + "/B" + FName) ; // '===儲存原始圖
            if(SWidth > 0)
            {
                System.Drawing.Bitmap PicChk, PicB, PicS ;  //'===宣告點陣圖
                PicChk = new System.Drawing.Bitmap(Fs) ;
                //' If PicChk.Width > PicChk.Height Then '===只接受寬度>高度的圖片
                PicValue = SWidth / PicChk.Width ;
                //'PicB = New System.Drawing.Bitmap(Drawing.Image.FromStream(Fs), PicChk.Width * PicValue, PicChk.Height * PicValue)
               // 'PicB.Save(SaveStr & "B" & FName)
                var s = System.Drawing.Image.FromStream(Fs);
                PicS = new System.Drawing.Bitmap(s,PicChk.Width * (int) PicValue, PicChk.Height * (int)PicValue);
                PicS.Save(SaveStr + "/S" + FName);
            }
            tmparr[0] = "B" + FName ;
            tmparr[1] = "S" + FName ;
            //'End If
        }
        return tmparr ;
     } 

    //'資料庫連線設定
    public string send_mail(string mailfrom , string mailto , string mailtitle , string mailcontent ) 
    {

        string mailserver = ConfigurationManager.AppSettings["mail_server"].ToString() ;//'新版的用法
		SmtpClient MySmtp = new SmtpClient(mailserver) ;
		System.Net.NetworkCredential cred  = new System.Net.NetworkCredential("press", "pressPRESS") ;
        MySmtp.Credentials = cred ;
		//'Try
            MailMessage msgMail = new MailMessage();
            msgMail.IsBodyHtml = true;
			//'msgMail.BodyFormat = MailFormat.Html
            msgMail.From = new MailAddress(mailfrom);
        
            msgMail.To.Add(mailto);
          
            msgMail.Subject = mailtitle;
            msgMail.Body = mailcontent;

            try
            {
				MySmtp.Send(msgMail);
                //'SmtpMail.SmtpServer = mailserver
                //'SmtpMail.Send(msgMail)
                return "1";
				//'MySmtp.Dispose()
            }
            catch{
              return "err";
            }
        

    }

   // '判斷session是否存在
    public string Cksession(string tmpsession , string tmptype) 
    {
        if(tmpsession != "")
        {
            return "";
        }
        else
        {
            if(tmptype=="1")
            {
                   return "Default.aspx";
            }
            else
            {
                return "";
            }
        }
    }
    //進系統log檔
    public void systemlog(SqlConnection tmpconn , string tsubname , string tmpnote , string tmpsqlnote , string slaccount , string sldate , string sfromip , string sflag )
    {
        SqlDataAdapter sd  = new SqlDataAdapter();
        SqlCommand sqlsmd = new SqlCommand();
        sqlsmd.Connection = tmpconn;
        sqlsmd.CommandText = "select * from Systemlog where 1 <> 1";
        sd.SelectCommand = sqlsmd ;
        DataSet dset  = new DataSet();
        sd.Fill(dset);
        DataTable dt = new DataTable();
       
        dt = dset.Tables[0] ;
        DataRow dr ;
        dr = dt.NewRow() ;
        dr["Slaccount"] = slaccount;
        dr["Sname"] = tsubname;
        dr["Slevent"] = tmpnote;
        dr["Ssqlog"] = tmpsqlnote;
        dr["Sldate"] = sldate;
        dr["Sodate"] = System.DateTime.Now;
        dr["Sfromip"] = sfromip;
        dr["Sflag"] = sflag;
        //newrow後,還要再加入,比較特殊的做法
        dt.Rows.Add(dr);

        SqlCommandBuilder smdbuilder = new SqlCommandBuilder(sd);
        sd.Update(dset);
    }

	
  }
