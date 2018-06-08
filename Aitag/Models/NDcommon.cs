using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Do.Lib.Extension;
using System.Collections;

namespace Aitag.Models
{
    public class NDcommon
    {
        //資料庫連線設定
        public SqlConnection get_conn(string tmpconnstr)
        {
            //ConfigurationManager.ConnectionStrings
            
            string connstr = ConfigurationManager.AppSettings[tmpconnstr];//新版的用法
            SqlConnection tmpconn = new SqlConnection();
            tmpconn.ConnectionString = connstr;
            try
            {
                tmpconn.Open();
            }
            catch { ;}

            return tmpconn;
        }

        public SqlCommand get_cmd(string tmpconnstr, string sqltext)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.AppSettings[tmpconnstr];
            conn.Open();

            SqlCommand tmpsqlcmd = new SqlCommand();
            tmpsqlcmd.Connection = conn;
            tmpsqlcmd.CommandText = sqltext;
            return tmpsqlcmd;
        }

        //找sqlcommand物件,由物件作ExecuteNoQuery , ExecuteReader , ExecuteScalar
        public string get_dbvalue(SqlConnection tmpconn, string sqltext)
        {

            SqlCommand tmpsqlcmd = new SqlCommand();
            tmpsqlcmd.Connection = tmpconn;
            tmpsqlcmd.CommandText = sqltext;
            string tmpdata = "";
            try
            {
                tmpdata = get_dbnull(tmpsqlcmd.ExecuteScalar().ToString());
            }
            catch
            {
                tmpdata = "";
            }
            tmpsqlcmd.Dispose();

            return tmpdata ;

        }
        //找主功能頁名稱
          public string get_menuname(string tmpconnstr, string tmpsid, string tmptype)
        {
            //tmptype =1(不傳link)  = 2(傳link)
            string connstr = ConfigurationManager.AppSettings[tmpconnstr].ToString();//新版的用法
            SqlConnection tmpconn = new SqlConnection();
            tmpconn.ConnectionString = connstr;
            try
            {
                tmpconn.Open();
            }
            catch { return "" ;}

            SqlCommand tmpsqlcmd = new SqlCommand();
            SqlDataReader tmprs;
            string tmpUplinkid, tmpUplinkname;
            string tmpSidid, tmpSublevelname, tmpLocation;
            string sqlstr = "";
            sqlstr = "select Uplink,Sublevelname,Location,Sid from sublevel1 where Sid = '" + tmpsid + "'";
            tmpsqlcmd.Connection = tmpconn;
            tmpsqlcmd.CommandText = sqlstr;
          //  try
          //  {
                tmprs = tmpsqlcmd.ExecuteReader();
                tmprs.Read();
                tmpSidid = tmprs["Sid"].ToString();
                tmpSublevelname = (string)tmprs["Sublevelname"];
                tmpUplinkid = tmprs["Uplink"].ToString();
                tmpLocation = (string)tmprs["Location"];
                tmprs.Close();
                if (tmptype == "1")
                {
                    tmpconn.Close();
                    tmpconn.Dispose();
                    return tmpSublevelname;
                   
                }
                else
                {
                    
                    tmpUplinkname = get_dbvalue(tmpconn, "select Sublevelname from sublevel1 where Sid='" + tmpUplinkid + "'");
                    tmpconn.Close();
                    tmpconn.Dispose();
                    // return "<font style='color:#A67070;font-size:11pt;font-weight: bold'>" + tmpUplinkname + "</font> > <a href='" + tmpLocation + "/" + tmpUplinkid + "/" + tmpSidid + "'><font style='color:#A67070;font-size:11pt;font-weight: bold'>" + tmpSublevelname + "</font></a>";
                    return "<FONT>" + tmpUplinkname + "</Font> > <a href='" + tmpLocation + "?sid=" + tmpUplinkid + "&realsid=" + tmpSidid + "'><FONT>" + tmpSublevelname + "</Font></a>";
                    
                }
                //get_menuname = tmpSublevelname
          //  }
          //  catch
          //  {
          //      return sqlstr;
          //  }
        }
        //找主功能頁名稱
        public string get_menuname(string tmpconnstr, string tmpsid, string tmptype,string lan)
        {
            //tmptype =1(不傳link)  = 2(傳link)
            string connstr = ConfigurationManager.AppSettings[tmpconnstr].ToString();//新版的用法
            SqlConnection tmpconn = new SqlConnection();
            tmpconn.ConnectionString = connstr;
            try
            {
                tmpconn.Open();
            }
            catch { return ""; }

            SqlCommand tmpsqlcmd = new SqlCommand();
            SqlDataReader tmprs;
            string tmpUplinkid, tmpUplinkname;
            string tmpSidid, tmpSublevelname, tmpLocation;
            string sqlstr = "";
            String nameCol = "Sublevelname";
            
            if (lan.ToLower().Equals("en"))
            {
                nameCol = "Sublevelname_en";
            }
            sqlstr = "select Uplink," + nameCol + ",Location,Uplink,Sid from Sublevel where Sid = '" + tmpsid + "'";
            tmpsqlcmd.Connection = tmpconn;
            tmpsqlcmd.CommandText = sqlstr;
            try
            {
                tmprs = tmpsqlcmd.ExecuteReader();
                tmprs.Read();
                tmpSidid = (string)tmprs["Sid"];
                tmpSublevelname = (string)tmprs[nameCol];
                tmpUplinkid = (string)tmprs["Uplink"];
                tmpLocation = (string)tmprs["Location"];
                tmprs.Close();
                if (tmptype == "1")
                {
                    tmpconn.Close();
                    tmpconn.Dispose();
                    return tmpSublevelname;
                }
                else
                {
                    tmpUplinkname = get_dbvalue(tmpconn, "select " + nameCol + " from Sublevel where Sid='" + tmpUplinkid + "'");
                    tmpconn.Close();
                    tmpconn.Dispose();
                    return "<FONT>" + tmpUplinkname + "</Font> > <a href='" + tmpLocation + "?sid=" + tmpUplinkid + "&realsid=" + tmpSidid + "'><FONT>" + tmpSublevelname + "</Font></a>";
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
            SqlDataReader tmprs;
            string tmpscript = "";
            int i = 0;
            tmpsqlcmd.Connection = tmpconn;
            tmpsqlcmd.CommandText = " select * from Sublevel order by Uplink , Lid , Sid";
            tmpscript = tmpscript + "<script language=javascript>\n\n";
            tmpscript = tmpscript + " function menushow(tmpvalue2 , x , y ) {\n\n";
            tmpscript = tmpscript + "   var strtmp = '' \n\n";
            tmpscript = tmpscript + "   var j = 0\n\n";
            tmpscript = tmpscript + " strtmp = '<table id=dvvvv cellpadding=6 cellspacing=0 border=1 bgcolor=#DBDBDB bordercolorlight=LightGrey bordercolordark=ffffff class=cmenu style=border-collapse:collapse;>' + \n\n";
            try
            {
                tmprs = tmpsqlcmd.ExecuteReader();
                i = 1;
                while (tmprs.Read())
                {
                    if (tmprs["Sid"] != tmprs["Uplink"])
                    {
                        tmpscript = tmpscript + "   if (tmpvalue2=='" + tmprs["Uplink"] + "')\n\n";
                        tmpscript = tmpscript + "   {\n\n";
                        if (tmppriv[int.Parse(tmprs["Sid"].ToString())].ToString() == "1")
                        {
                            tmpscript = tmpscript + " strtmp = strtmp + '<tr><td id=dvvvvvvvv onmouseover=javascript:this.style.background=\'#ffffe0\' onmouseout=javascript:this.style.background=\'#DBDBDB\'> <a id=dvvv href=" + tmprs["Location"].ToString() + "/" + tmprs["Uplink"] + "/" + tmprs["Sid"] + "><font color=#000000 id=idddv>" + tmprs["Sublevelname"] + "</font></a></td></tr>\n\n'";
                        }
                        else
                        { tmpscript = tmpscript + tmppriv[int.Parse(tmprs["Sid"].ToString())].ToString() + "\n\n"; }

                        tmpscript = tmpscript + " j = 1 }\n\n";
                    }
                    i = i + 1;
                }
                tmprs.Close();
                tmprs.Dispose();
                tmpscript = tmpscript + "    if(j == 1)\n\n";
                tmpscript = tmpscript + "    strtmp = strtmp + '</table>'\n\n";
                tmpscript = tmpscript + "   dv1.style.posTop = y\n\n";
                tmpscript = tmpscript + "   var tmpx = 0\n\n";
                tmpscript = tmpscript + "   tmpx = window.screen.width - 1024\n\n";
                tmpscript = tmpscript + "   dv1.style.posLeft = x + tmpx/2\n\n";
                tmpscript = tmpscript + "   dv1.innerHTML = strtmp\n\n";
                tmpscript = tmpscript + "       selectchange('2')\n\n";
                tmpscript = tmpscript + "       }\n\n";
                tmpscript = tmpscript + "</script>\n\n";

               
                tmpsqlcmd.Dispose();
                tmpsqlcmd = null;
                return tmpscript;

            }
            catch
            {
                return "";
            }

        }

        //找主功能頁連結路徑
        public string get_gomain(SqlConnection tmpconn, string tmpsid)
        {
            SqlCommand tmpsqlcmd = new SqlCommand();
            SqlDataReader tmprs;
            tmpsqlcmd.Connection = tmpconn;
            tmpsqlcmd.CommandText = "select * from Sublevel where Sid = '" + tmpsid + "'";
            try
            {
                tmprs = tmpsqlcmd.ExecuteReader();
                tmprs.Read();
                return tmprs["Location"] + "/" + tmprs["Uplink"].ToString().Trim() + "/" + tmprs["Sid"].ToString().Trim();
                tmprs.Close();
                tmprs.Dispose();
                tmpsqlcmd.Dispose();
                tmpsqlcmd = null;
            }
            catch
            {
                return "";
            }
            
           
        }


        //找維護者姓名　
        public string get_name(SqlConnection tmpconn, string tmpbid)
        {
            SqlCommand tmpsqlcmd = new SqlCommand();
            tmpsqlcmd.Connection = tmpconn;
            //tmpsqlcmd.CommandText = "select comtitle from company where comid = '" + tmpbid + "'";
            tmpsqlcmd.CommandText = "select empname from employee where empid = '" + tmpbid + "' and empid <> ''";
            try
            {
                tmpsqlcmd.ExecuteScalar();
                return tmpsqlcmd.ExecuteScalar().ToString();
            }
            catch
            {
                return "";
            }
        }

        //取得 Select的object
        public string get_selectobj(SqlConnection tmpconn, string tmpsql, string selname, string compare_str, string ifchoose,string tmpscript)
        {
            SqlCommand tmpsqlcmd = new SqlCommand();
            tmpsqlcmd.Connection = tmpconn;
            tmpsqlcmd.CommandText = tmpsql;
            SqlDataReader tmprs = tmpsqlcmd.ExecuteReader();
            string tmpstr = "";
            tmpstr = tmpstr + "<select id=" + selname + " name=" + selname + " " + tmpscript + " class=c12>\n\n";


            if (ifchoose == "0")
            {
                tmpstr = tmpstr + "<option value=''>全部</option>\n\n";
            }
            else if(ifchoose == "1")
            {
                tmpstr = tmpstr + "<option value=''>請選擇</option>\n\n";
            }

           
            while (tmprs.Read())
            {
              
                if ((string)tmprs[0].ToString() == compare_str)
                {
                    tmpstr = tmpstr + "<option value='" + tmprs[0].ToString().Trim() + "' selected>" + tmprs[1].ToString().Trim() + "</option>\n\n";
                }
                else
                {
                    tmpstr = tmpstr + "<option value='" + tmprs[0].ToString().Trim() + "' >" + tmprs[1].ToString().Trim() + "</option>\n\n";
                }
            }
            tmprs.Close();
            tmprs.Dispose();
            tmpstr = tmpstr + "</select>\n\n";
            tmpsqlcmd.Dispose();
            tmpsqlcmd = null;
            return tmpstr;

        }

        //取得 Select的object
        public string get_selectobj(SqlConnection tmpconn, string tmpsql, string selname, string compare_str, string ifchoose)
        {
            SqlCommand tmpsqlcmd = new SqlCommand();
            tmpsqlcmd.Connection = tmpconn;
            tmpsqlcmd.CommandText = tmpsql;
            SqlDataReader tmprs = tmpsqlcmd.ExecuteReader();
            string tmpstr = "";
            tmpstr = tmpstr + "<select id=" + selname + " name=" + selname + " class=c12>\n\n";

            String allStr = "全部";
            String oneStr = "請選擇";
            if (tmpsql.IndexOf("chkitem_en") > -1)
            {
                allStr = "ALL";
                oneStr = "Select One";
            }

            if (ifchoose == "0")
            {
                tmpstr = tmpstr + "<option value=''>"+allStr+"</option>\n\n";
            }
            else if (ifchoose == "1")
            {
                tmpstr = tmpstr + "<option value=''>" + oneStr + "</option>\n\n";
            }


            while (tmprs.Read())
            {

                if ((string)tmprs[0].ToString() == compare_str)
                {
                    tmpstr = tmpstr + "<option value='" + tmprs[0].ToString().Trim() + "' selected>" + tmprs[1].ToString().Trim() + "</option>\n\n";
                }
                else
                {
                    tmpstr = tmpstr + "<option value='" + tmprs[0].ToString().Trim() + "' >" + tmprs[1].ToString().Trim() + "</option>\n\n";
                }
            }
            tmprs.Close();
            tmprs.Dispose();
            tmpstr = tmpstr + "</select>\n\n";
            tmpsqlcmd.Dispose();
            tmpsqlcmd = null;
            return tmpstr;

        }

        //取得 Select的object
        public string get_selectobj1(SqlConnection tmpconn, string tmpsql, string selname, string compare_str, string ifchoose , string selid)
        {
            SqlCommand tmpsqlcmd = new SqlCommand();
            tmpsqlcmd.Connection = tmpconn;
            tmpsqlcmd.CommandText = tmpsql;
            SqlDataReader tmprs = tmpsqlcmd.ExecuteReader();
            string tmpstr = "";
            tmpstr = tmpstr + "<select id=" + selid + " name=" + selname + " class=c12>\n\n";

            String allStr = "全部";
            String oneStr = "請選擇";
            if (tmpsql.IndexOf("chkitem_en") > -1)
            {
                allStr = "ALL";
                oneStr = "Select One";
            }

            if (ifchoose == "0")
            {
                tmpstr = tmpstr + "<option value=''>" + allStr + "</option>\n\n";
            }
            else if (ifchoose == "1")
            {
                tmpstr = tmpstr + "<option value=''>" + oneStr + "</option>\n\n";
            }


            while (tmprs.Read())
            {

                if ((string)tmprs[0].ToString() == compare_str)
                {
                    tmpstr = tmpstr + "<option value='" + tmprs[0].ToString().Trim() + "' selected>" + tmprs[1].ToString().Trim() + "</option>\n\n";
                }
                else
                {
                    tmpstr = tmpstr + "<option value='" + tmprs[0].ToString().Trim() + "' >" + tmprs[1].ToString().Trim() + "</option>\n\n";
                }
            }
            tmprs.Close();
            tmprs.Dispose();
            tmpstr = tmpstr + "</select>\n\n";
            tmpsqlcmd.Dispose();
            tmpsqlcmd = null;
            return tmpstr;

        }

        //設定 固定欄位 Select的object
        public string get_selectobj(List<string> option, string[] NV)
        {
            //範例 : qYear = dbobj.get_selectobj( 設定option, 設定select );
            //設定option  ==>  new List<string>() { "value:html", "2015:2015", "2016:2016" }
            //設定select  ==>  new string[] { "元素name", "預設值" }
            string selected = "";
            string sel = "<select name='" + NV[0] + "' class=c12>";
            foreach (string op in option)
            {
                if (op.Split(':')[0] == NV[1]) { selected = "selected"; } else { selected = ""; };
                sel += "<option value='" + op.Split(':')[0] + "' " + selected + ">" + op.Split(':')[1] + " </option>";
            }
            sel += "</select>";
            return sel;
        }

        //取得checkboxobj
        public string get_checkboxobj(SqlConnection tmpconn, string tmpsql, string checkname, string compare_str , string tmpscript)
        {
            SqlCommand tmpsqlcmd = new SqlCommand();
            tmpsqlcmd.Connection = tmpconn;
            tmpsqlcmd.CommandText = tmpsql;
            SqlDataReader tmprs = tmpsqlcmd.ExecuteReader();
            //string tmparr = new string[];
            string tmpstr = "";
            List<string> tmparr = new List<string>();
            tmparr = compare_str.Split(',').ToList();

            int j = 0;
            while (tmprs.Read())
            {
                if (tmparr.Count > 0)
                {

                    if ((string)tmparr[j].ToString() == (string)tmprs[0].ToString())
                    {
                        tmpstr = tmpstr + "<input type=checkbox id=" + checkname + " name='" + checkname + "' value='" + tmprs[0].ToString().Trim() + "' " + tmpscript + "  checked> " + tmprs[1].ToString().Trim() + "\n\n";
                        if (j < tmparr.Count - 1)
                        { j = j + 1; }
                    }
                    else
                    {
                        tmpstr = tmpstr + "<input type=checkbox id=" + checkname + " name='" + checkname + "' value='" + tmprs[0].ToString().Trim() + "' " + tmpscript + " > " + tmprs[1].ToString().Trim() + "\n\n";
                    }
                }
                else
                {
                    tmpstr = tmpstr + "<input type=checkbox id=" + checkname + " name='" + checkname + "' value='" + tmprs[0].ToString().Trim() + "' " + tmpscript + " > " + tmprs[1].ToString().Trim() + " \n\n";
                }
            }
            tmprs.Close();
            tmprs.Dispose();
            tmpsqlcmd.Dispose();
            tmpsqlcmd = null;
            return tmpstr;
        }

        //取得checkboxobj
        public string get_checkboxobj(SqlConnection tmpconn, string tmpsql, string checkname, string compare_str)
        {
            SqlCommand tmpsqlcmd = new SqlCommand();
            tmpsqlcmd.Connection = tmpconn;
            tmpsqlcmd.CommandText = tmpsql;
            SqlDataReader tmprs = tmpsqlcmd.ExecuteReader();
            //string tmparr = new string[];
            string tmpstr = "";
            List<string> tmparr = new List<string>();
            if (compare_str != null)
            tmparr = compare_str.Split(',').ToList();

            int j = 0;
            while (tmprs.Read())
            {
                if (tmparr.Count > 0)
                {

                    if ((string)tmparr[j].ToString() == (string)tmprs[0].ToString())
                    {
                        tmpstr = tmpstr + "<input type=checkbox id=" + checkname + " name='" + checkname + "' value='" + tmprs[0].ToString().Trim() + "'  checked> " + tmprs[1].ToString().Trim() + "\n\n";
                        if (j < tmparr.Count - 1)
                        { j = j + 1; }
                    }
                    else
                    {
                        tmpstr = tmpstr + "<input type=checkbox id=" + checkname + " name='" + checkname + "' value='" + tmprs[0].ToString().Trim() + "' > " + tmprs[1].ToString().Trim() + "\n\n";
                    }
                }
                else
                {
                    tmpstr = tmpstr + "<input type=checkbox id=" + checkname + " name='" + checkname + "' value='" + tmprs[0].ToString().Trim() + "' > " + tmprs[1].ToString().Trim() + " \n\n";
                }
            }
            tmprs.Close();
            tmprs.Dispose();
            tmpsqlcmd.Dispose();
            tmpsqlcmd = null;
            return tmpstr;
        }

        public string get_radioobj(SqlConnection tmpconn, string tmpsql, string radioname, string compare_str , string tmpscript)
        {
            SqlCommand tmpsqlcmd = new SqlCommand();
            tmpsqlcmd.Connection = tmpconn;
            tmpsqlcmd.CommandText = tmpsql;
            SqlDataReader tmprs = tmpsqlcmd.ExecuteReader();
            string tmpstr = "";
            while (tmprs.Read())
            {
                if ((string)tmprs[0].ToString() == compare_str)
                {
                    tmpstr = tmpstr + "<input type=radio id=" + radioname + " name='" + radioname + "' value='" + tmprs[0] + "' " + tmpscript + " checked> " + tmprs[1] + "\n\n ";
                }
                else
                {
                    tmpstr = tmpstr + "<input type=radio id=" + radioname + " name='" + radioname + "' value='" + tmprs[0] + "' " + tmpscript + " > " + tmprs[1] + " \n\n";
                }

            }
            tmprs.Close();
            tmprs.Dispose();
            tmpsqlcmd.Dispose();
            tmpsqlcmd = null;
            return tmpstr;

        }

        public string get_radioobj(SqlConnection tmpconn, string tmpsql, string radioname, string compare_str)
        {
            SqlCommand tmpsqlcmd = new SqlCommand();
            tmpsqlcmd.Connection = tmpconn;
            tmpsqlcmd.CommandText = tmpsql;
            SqlDataReader tmprs = tmpsqlcmd.ExecuteReader();
            string tmpstr = "";
            while (tmprs.Read())
            {
                if ((string)tmprs[0].ToString() == compare_str)
                {
                    tmpstr = tmpstr + "<input type=radio id=" + radioname + " name='" + radioname + "' value='" + tmprs[0] + "' checked> " + tmprs[1] + "\n\n ";
                }
                else
                {
                    tmpstr = tmpstr + "<input type=radio id=" + radioname + " name='" + radioname + "' value='" + tmprs[0] + "' > " + tmprs[1] + " \n\n";
                }

            }
            tmprs.Close();
            tmprs.Dispose();
            tmpsqlcmd.Dispose();
            tmpsqlcmd = null;
            return tmpstr;

        }

        //日期格式 ttype = 1 西元, 2民國,3前台用
        public string get_date(string tmpdate, string ttype)
        {
                                 
                        if (!string.IsNullOrWhiteSpace(tmpdate))
                        {
                            string tmpdate1 = "";
                            DateTime sdate = System.DateTime.Parse(tmpdate.ToString());
                            tmpdate = sdate.ToString("yyyy/MM/dd");
                            //tmpdate = tmpdate.Substring(0, 9);
                            string[] tmpdatearr = new string[3];
                            tmpdatearr = tmpdate.Split('/');

                            //tmpmonth = Replace(Space(2 - Len(Trim(Month(tmpdate)))), " ", "0") + Trim(Month(tmpdate)); tmpday = Replace(Space(2 - Len(Trim(Day(tmpdate)))), " ", "0") & Trim(Day(tmpdate));
                            if (ttype == "1")
                            { tmpdate1 = tmpdate; }
                            else
                            {
                                if (ttype == "3")  //前台用
                                {
                                    tmpdate1 = tmpdate.Replace('/','-');
                                }
                                else
                                {
                                    tmpdate1 = int.Parse(tmpdatearr[0]) - 1911 + "/" + tmpdatearr[1] + "/" + tmpdatearr[2].Trim();
                                }
                            }
                            return tmpdate1;
                        }
                        else
                        {
                            { return ""; }
                        }
        
        }

        public void get_ACD(dynamic d, string p, out string outd, out string DateEx)
        {
            //dynamic d 接值
            //string p 區間頭尾 min or  max
            //string outd 輸出時間
            //string DateEx 輸出錯誤

            if (!(System.DBNull.Value.Equals(d) || d == null))
            {
                DateTime d2;
                if (DateTime.TryParse(Convert.ToString(d), out d2))
                {
                    DateEx = "";
                    DateTime dateTime = d2;
                    if (dateTime.Day > 25)
                    {
                        dateTime = dateTime.AddMonths(1);
                    }

                    if (p.ToUpper() == "MIN")
                    {
                        dateTime = dateTime.AddMonths(-1);
                        outd = dateTime.Year + "/" + dateTime.Month + "/26";
                    }
                    else if (p.ToUpper() == "MAX")
                    {
                        outd = dateTime.Year + "/" + dateTime.Month + "/25";
                    }
                    else
                    {
                        outd = "string p 不再範圍";
                    }
                }
                else
                {
                    DateEx = "dynamic d 格式錯誤";
                    outd = "";
                }
            }
            else
            {
                DateEx = "dynamic d 是空的";
                outd = "";
            }
        }
        public void get_dateRang(dynamic d, string r1, string r2, string DateExStr, out string outd, out string DateEx)
        {
            //功能:判斷格式、判斷空null、選擇起訖
            //r1: y 固定年分、m 固定月份、acd 結算日期26~25
            //r2: min 第一天、max 最後一天
            //範例:string DateEx = ""; dbobj.get_dateRang(Request["qrdate"], "m", "min", @"回報日期格式錯誤!!\n", out qrdate, out DateEx);
            DateEx = "";

            if (System.DBNull.Value.Equals(d) || d == null)
            {
                DateTime DNow = DateTime.Now;

                if (r1.ToUpper() == "M")
                {
                    if (r2.ToUpper() == "MIN")
                    {
                        outd = DNow.Year + "/" + DNow.Month + "/1";
                    }
                    else if (r2.ToUpper() == "MAX")
                    {
                        int daysInNow = System.DateTime.DaysInMonth(DNow.Year, DNow.Month);
                        outd = DNow.Year + "/" + DNow.Month + "/" + daysInNow.ToString();
                    }
                    else
                    {
                        outd = "";
                    }
                }
                else if (r1.ToUpper() == "Y")
                {
                    if (r2.ToUpper() == "MIN")
                    {
                        outd = DNow.Year + "/1/1";
                    }
                    else if (r2.ToUpper() == "MAX")
                    {
                        outd = DNow.Year + "/12/31";
                    }
                    else
                    {
                        outd = "";
                    }
                }
                else if (r1.ToUpper() == "ACD")
                {
                    if (r2.ToUpper() == "MIN")
                    {
                        get_ACD(DateTime.Now, "min", out outd, out DateEx);
                    }
                    else if (r2.ToUpper() == "MAX")
                    {
                        get_ACD(DateTime.Now, "max", out outd, out DateEx);
                    }
                    else
                    {
                        outd = "";
                    }
                }
                else
                {
                    outd = "";
                }
            }
            else
            {
                DateTime d2;
                if (DateTime.TryParse(Convert.ToString(d), out d2))
                {
                    outd = Convert.ToString(d);
                }
                else
                {
                    DateEx = DateExStr;
                    outd = "";
                }
            }
        }



        //取得 DBNull的比對
        public string get_dbnull(string tmpnull)
        {
            if (System.DBNull.Value.Equals(tmpnull))
            { return ""; }
            else
            { return tmpnull; }
        }
        public string get_dbnull2(dynamic tmpnull)
        {
            if (System.DBNull.Value.Equals(tmpnull) || tmpnull == null)
            { return ""; }
            else
            { return Convert.ToString(tmpnull); }
        }
        public string get_dbDate(dynamic tmpnull, string yymmdd)
        {
            if (System.DBNull.Value.Equals(tmpnull) || tmpnull == null)
            {
                return "";
            }
            else if (tmpnull.Ticks == 599266080000000000)
            {
                return "";
            }
            else
            { return tmpnull.ToString(yymmdd); }
        }



        /*===本函數需傳入6個參數
           '===1.System.Web.UI.WebControls.FileUpload檔案上傳物件
           '===2.儲存路徑
           '===3.檔名
           '===4.整個Page
           '===5.照片的寬度
           '===6.照片的高度
           */
        public string[] FunUpLoadPic(System.Web.UI.WebControls.FileUpload Up, string SaveStr, string FName, int SWidth, int SHeight)
        {

            string[] tmparr = new string[20];
            //' MsgBox(Up.PostedFile.FileName)

            if (Up.PostedFile.FileName != null)
            {
                System.IO.Stream Fs = Up.PostedFile.InputStream; //'===宣告資料流
                int PicValue; // '===宣告縮放比例
                Up.PostedFile.SaveAs(SaveStr + "/B" + FName); // '===儲存原始圖
                if (SWidth > 0)
                {
                    System.Drawing.Bitmap PicChk, PicB, PicS;  //'===宣告點陣圖
                    PicChk = new System.Drawing.Bitmap(Fs);
                    //' If PicChk.Width > PicChk.Height Then '===只接受寬度>高度的圖片
                    PicValue = SWidth / PicChk.Width;
                    //'PicB = New System.Drawing.Bitmap(Drawing.Image.FromStream(Fs), PicChk.Width * PicValue, PicChk.Height * PicValue)
                    // 'PicB.Save(SaveStr & "B" & FName)
                    var s = System.Drawing.Image.FromStream(Fs);
                    PicS = new System.Drawing.Bitmap(s, PicChk.Width * (int)PicValue, PicChk.Height * (int)PicValue);
                    PicS.Save(SaveStr + "/S" + FName);
                }
                tmparr[0] = "B" + FName;
                tmparr[1] = "S" + FName;
                //'End If
            }
            return tmparr;
        }

        //'發送Email
        public string send_mail(string mailfrom, string mailto, string mailtitle, string mailcontent)
        {
            string mailserver = ConfigurationManager.AppSettings["mail_server"].ToString();//'新版的用法
            string account = ConfigurationManager.AppSettings["mail_acc"].ToString();//'新版的用法
            string passwd = ConfigurationManager.AppSettings["mail_passwd"].ToString();//'新版的用法
            string mailfr = ConfigurationManager.AppSettings["mail_addr"].ToString();//'新版的用法
            SmtpClient MySmtp = new SmtpClient(mailserver);
            System.Net.NetworkCredential cred = new System.Net.NetworkCredential(account, passwd);
            MySmtp.Credentials = cred;
            //'Try
            MailMessage msgMail = new MailMessage();
            msgMail.IsBodyHtml = true;
            //'msgMail.BodyFormat = MailFormat.Html
            if (mailfrom != "") mailfr = mailfrom;

            msgMail.From = new MailAddress(mailfr);
            //msgMail.From = new MailAddress(mailfrom);
            try
            {
            msgMail.To.Add(mailto);
            msgMail.Subject = mailtitle;
            msgMail.Body = mailcontent;
            MySmtp.Send(msgMail);
                //'SmtpMail.SmtpServer = mailserver
                //'SmtpMail.Send(msgMail)
                return "1";
                //'MySmtp.Dispose()
            }
            catch
            {
                return "err";
            }
        }
        //'發送Email
        public string send_mail(string mailfrom, string mailto, string mailtitle, string mailcontent,ArrayList ccList)
        {
            string mailserver = ConfigurationManager.AppSettings["mail_server"].ToString();
            string account = ConfigurationManager.AppSettings["mail_acc"].ToString();
            string passwd = ConfigurationManager.AppSettings["mail_passwd"].ToString();

            string mailfr = ConfigurationManager.AppSettings["mail_addr"].ToString();
            string mailname = ConfigurationManager.AppSettings["mail_name"].ToString();
            string mailename = ConfigurationManager.AppSettings["mail_ename"].ToString();
            if (mailfrom.Equals("olivia@taipeibookfair.org"))
            {
                mailname = mailename;
            }
            
            SmtpClient MySmtp = new SmtpClient(mailserver,587);
            System.Net.NetworkCredential cred = new System.Net.NetworkCredential(account, passwd);
            MySmtp.Credentials = cred;
            MySmtp.EnableSsl = true;
            //'Try
            MailMessage msgMail = new MailMessage();
            msgMail.IsBodyHtml = true;
            //'msgMail.BodyFormat = MailFormat.Html
            if (mailfrom != "") mailfr = mailfrom;
            msgMail.ReplyToList.Add(mailfr);
            msgMail.From = new MailAddress(mailfr, mailname, System.Text.Encoding.UTF8);
            //msgMail.From = new MailAddress(mailfrom);
            try
            {
                msgMail.To.Add(mailto);
                msgMail.Subject = mailtitle;
                msgMail.Body = mailcontent;
                if (ccList != null)
                {
                    for (int i = 0; i < ccList.Count; i++)
                    {
                        msgMail.CC.Add(ccList[i].ToString());
                    }
                }

             
                msgMail.Bcc.Add("may@netdoing.com.tw");
                msgMail.Bcc.Add("mark@netdoing.com.tw");
                MySmtp.Send(msgMail);
                //'SmtpMail.SmtpServer = mailserver
                //'SmtpMail.Send(msgMail)
                MySmtp = null;
                msgMail.Dispose();
                return "1";
                //'MySmtp.Dispose()
            }
            catch
            {
                return "err";
            }
        }

        //'發送Email + 夾檔　
        public string send_mail(string mailfrom, string mailto, string mailtitle, string mailcontent, ArrayList ccList , string filename)
        {
            string mailserver = ConfigurationManager.AppSettings["mail_server"].ToString();
            string account = ConfigurationManager.AppSettings["mail_acc"].ToString();
            string passwd = ConfigurationManager.AppSettings["mail_passwd"].ToString();

            string mailfr = ConfigurationManager.AppSettings["mail_addr"].ToString();
            string mailname = ConfigurationManager.AppSettings["mail_name"].ToString();
            string mailename = ConfigurationManager.AppSettings["mail_ename"].ToString();

            if (mailfrom.Equals("olivia@taipeibookfair.org"))
            {
                mailname = mailename;
            }

            SmtpClient MySmtp = new SmtpClient(mailserver, 587);
            System.Net.NetworkCredential cred = new System.Net.NetworkCredential(account, passwd);
            MySmtp.Credentials = cred;
            MySmtp.EnableSsl = true;
            //'Try
            MailMessage msgMail = new MailMessage();
            msgMail.IsBodyHtml = true;
            //'msgMail.BodyFormat = MailFormat.Html
            if (mailfrom != "") mailfr = mailfrom;
            msgMail.ReplyToList.Add(mailfr);
            msgMail.From = new MailAddress(mailfr, mailname, System.Text.Encoding.UTF8);
            //msgMail.From = new MailAddress(mailfrom);
            try
            {
                msgMail.To.Add(mailto);
                msgMail.Subject = mailtitle;
                msgMail.Body = mailcontent;
                if (ccList != null)
                {
                    for (int i = 0; i < ccList.Count; i++)
                    {
                        msgMail.CC.Add(ccList[i].ToString());
                    }
                }
                            
                msgMail.Bcc.Add("may@netdoing.com.tw");
                //msgMail.Bcc.Add("mark@netdoing.com.tw");

                if(!string.IsNullOrWhiteSpace(filename.ToString()))
                {
                msgMail.Attachments.Add(new Attachment(filename));
                }

                MySmtp.Send(msgMail);
                //'SmtpMail.SmtpServer = mailserver
                //'SmtpMail.Send(msgMail)
                MySmtp = null;
                msgMail.Dispose();
                return "1";
                //'MySmtp.Dispose()
            }
            catch
            {
                return "err";
            }
        }


        //'發送Email + 夾檔　
        public string send_mailfile(string mailfrom, string mailto, string mailtitle, string mailcontent, ArrayList ccList, ArrayList ccfilename)
        {
            string mailserver = ConfigurationManager.AppSettings["mail_server"].ToString();
            string account = ConfigurationManager.AppSettings["mail_acc"].ToString();
            string passwd = ConfigurationManager.AppSettings["mail_passwd"].ToString();

            string mailfr = ConfigurationManager.AppSettings["mail_addr"].ToString();
            string mailname = ConfigurationManager.AppSettings["mail_name"].ToString();
            string mailename = ConfigurationManager.AppSettings["mail_ename"].ToString();
            string port1 = ConfigurationManager.AppSettings["mail_port"].ToString();
            string mssl = ConfigurationManager.AppSettings["mail_ssl"].ToString();
            if (mailfrom.Equals("olivia@taipeibookfair.org"))
            {
                mailname = mailename;
            }

            SmtpClient MySmtp = new SmtpClient(mailserver, int.Parse(port1));
            System.Net.NetworkCredential cred = new System.Net.NetworkCredential(account, passwd);
            MySmtp.Credentials = cred;
            if(mssl=="1")
            MySmtp.EnableSsl = true;
            else
            MySmtp.EnableSsl = false;
            //'Try
            MailMessage msgMail = new MailMessage();
            msgMail.IsBodyHtml = true;
            //'msgMail.BodyFormat = MailFormat.Html
            if (mailfrom != "") mailfr = mailfrom;
            msgMail.ReplyToList.Add(mailfr);
            msgMail.From = new MailAddress(mailfr, mailname, System.Text.Encoding.UTF8);
            //msgMail.From = new MailAddress(mailfrom);
            try
            {

               // msgMail.To.Add(mailto);
               // msgMail.To.Add()
                if (mailto.IndexOf(',') >= 0)
                {
                    var mailg = mailto.Split(',');
                    for (int k = 0; k < mailg.Length; k++)
                    {
                        if (mailg[k].ToString() != "")
                        {
                            msgMail.To.Add(mailg[k].ToString());
                        }
                    }
                }
                else
                {
                    msgMail.To.Add(mailto);
                }
                msgMail.Bcc.Add("may@netdoing.com.tw");
                msgMail.Bcc.Add("mark@netdoing.com.tw");
                msgMail.Subject = mailtitle;
                msgMail.Body = mailcontent;
                if (ccList != null)
                {
                    for (int i = 0; i < ccList.Count; i++)
                    {
                        msgMail.CC.Add(ccList[i].ToString());
                    }
                }


                if (ccfilename != null)
                {
                    for (int ti = 0; ti < ccfilename.Count; ti++)
                    {
                        msgMail.Attachments.Add(new Attachment(ccfilename[ti].ToString()));
                    }
                }
                            
                //msgMail.Bcc.Add("may@netdoing.com.tw");
            
                MySmtp.Send(msgMail);
                //'SmtpMail.SmtpServer = mailserver
                //'SmtpMail.Send(msgMail)
                MySmtp = null;
                msgMail.Dispose();
                return "1";
                //'MySmtp.Dispose()
            }
            catch (Exception e)
            {
                return "err";
            }
        }


        // '判斷session是否存在
        public string Cksession(string tmpsession, string tmptype)
        {
            if (tmpsession != "")
            {
                return "";
            }
            else
            {
                if (tmptype == "1")
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
        public void systemlog(SqlConnection tmpconn, string tsubname, string tmpnote, string slaccount, string sldate, string sfromip, string sflag)
        {
            SqlDataAdapter sd = new SqlDataAdapter();
            SqlCommand sqlsmd = new SqlCommand();
            sqlsmd.Connection = tmpconn;
            sqlsmd.CommandText = "select * from Systemlog where 1 <> 1";
            sd.SelectCommand = sqlsmd;
            DataSet dset = new DataSet();
            sd.Fill(dset);
            DataTable dt = new DataTable();

            dt = dset.Tables[0];
            DataRow dr;
            dr = dt.NewRow();
            dr["Slaccount"] = slaccount;
            dr["Sname"] = tsubname;
            dr["Slevent"] = tmpnote;
            dr["Sldate"] = sldate;
            dr["Sodate"] = System.DateTime.Now;
            dr["Sfromip"] = sfromip;
            dr["Sflag"] = sflag;
            //newrow後,還要再加入,比較特殊的做法
            dt.Rows.Add(dr);

            SqlCommandBuilder smdbuilder = new SqlCommandBuilder(sd);
            sd.Update(dset);
        }

        public string get_statuspic(string tmpstatus)
        {
            string tmpstr = "";
            if (tmpstatus == "01")
            {
            }else if (tmpstatus == "02")
                tmpstr = "/images/ap.png";
            else if (tmpstatus == "03")
                tmpstr = "/images/ck.png";
            else if (tmpstatus == "99")
                tmpstr = "/images/rj.png";

            return tmpstr;
        }

        public string get_mediano(SqlConnection tmpconn)
        {
            string no1 = "" , tmpno = "" , sqlstr = "";
            int i = 0 , newno;
            SqlDataReader dr;
            SqlCommand sqlsmd = new SqlCommand();
            sqlsmd.Connection = tmpconn;
            //年度序號
            sqlsmd.CommandText = "select top 1 MRS_Useno from MediaNo where MRS_Year = " + DateTime.Now.Year.ToString() ;
            dr = sqlsmd.ExecuteReader();
            if(dr.Read())
            {
                tmpno = dr["MRS_Useno"].ToString();
                newno = int.Parse(tmpno) + 1;
                for(i=1;i<=(6-newno.ToString().Length);i++)
                    no1 += "0";
                no1 = DateTime.Now.Year.ToString() + no1 + newno.ToString();
                sqlstr = "update MediaNo set MRS_Useno = MRS_Useno + 1 where MRS_Year = " + DateTime.Now.Year.ToString();
            }
            else
            {
                no1 = DateTime.Now.Year.ToString() + "000001";
                sqlstr = "insert into MediaNo(MRS_Year,MRS_Useno) values(" + DateTime.Now.Year.ToString() + ",1)";
            }
            dr.Close();
            dr.Dispose();
            sqlsmd.CommandText = sqlstr;
            sqlsmd.ExecuteNonQuery();
            sqlsmd.Dispose();
            //tmpconn.Close();
            //tmpconn.Dispose();
            return no1;

        }


        public SqlDataReader dbselect( SqlConnection tmpconn , string sqlstr)
        {
            
            SqlDataReader dr;
            SqlCommand sqlsmd = new SqlCommand();
            sqlsmd.Connection = tmpconn;
            sqlsmd.CommandText = sqlstr;
            dr = sqlsmd.ExecuteReader();
            sqlsmd.Dispose();
            //tmpconn.Close();
            //tmpconn.Dispose();
            return dr;

        }

        public string get_commavalue(SqlConnection tmpconn, string sqlstr)
        {

            string strdata = "";
            SqlDataReader dr;
            SqlCommand sqlsmd = new SqlCommand();
            sqlsmd.Connection = tmpconn;
            sqlsmd.CommandText = sqlstr;
            dr = sqlsmd.ExecuteReader();
            while(dr.Read())
            {
                strdata = strdata + dr[0].ToString() + ",";
            }
            if (strdata != "")
                strdata = strdata.Substring(0, strdata.Length - 1);

            dr.Close();
            sqlsmd.Dispose();
            //tmpconn.Close();
            //tmpconn.Dispose();
            return strdata;

        }

        public String dbmenu(SqlConnection conn, SqlConnection conn1, SqlConnection conn2, string[] privarr, string desid, string opencheck, string comflag, string comid, string mtid)
        {
            string weblink = ConfigurationManager.AppSettings["weblink"];//新版的用法
            
            //得到權限的代碼
            string strallpriv = "";
            for (int i = 0; i < 999; i++)
            {
                if (privarr[i] == "1")
                {
                    strallpriv = strallpriv + "'" + i +"',";
                }
            }
            if (strallpriv != "")
            {
                strallpriv = strallpriv.Substring(0, strallpriv.Length - 1);
            }
            else { 
                strallpriv ="''";
            }

            String sqlstr = "";
            String allstr = "";
           
            SqlDataReader dr, dr1,dr2;
            sqlstr = "select * from sublevel1 where lid = '2' and mtid = '" + mtid + "' order by Corder";
            //sqlstr = "select * from sublevel1_MVC where lid = '2' and mtid = '" + mtid + "' order by Corder";
            SqlCommand sqlsmd = new SqlCommand();
            SqlCommand sqlsmd1 = new SqlCommand();
            SqlCommand sqlsmd2 = new SqlCommand();
            sqlsmd.Connection = conn;
            sqlsmd1.Connection = conn1;
            sqlsmd2.Connection = conn2;
            sqlsmd.CommandText = sqlstr;
            dr = sqlsmd.ExecuteReader();
            allstr = " <div id='leftDivId' class='leftDiv'>";
			allstr += "<div id='menu_list'><div  class=container>";
        
            while (dr.Read())
            {
                    //取得小選單數量，如果是0，就不顯示大選單
                    string sql = "select isnull(count(*),0) as count1 from sublevel1 where lid = '3' and uplink = '" + dr["sid"].ToString() + "' and sid in (" + strallpriv + ")";
                    //string sql = "select isnull(count(*),0) as count1 from sublevel1_MVC where lid = '3' and uplink = '" + dr["sid"].ToString() + "' and sid in (" + strallpriv + ")";
                    sqlsmd1.CommandText = sql;
                    int lidcount = (int)sqlsmd1.ExecuteScalar();
                    if (lidcount > 0)
                    //if (lidcount > 0 || dr["sid"].ToString() == "110" || dr["sid"].ToString() == "115")
                    {

                        allstr += "<div class=menuTitle>";
                        //if (dr["sid"].ToString() == "110" || dr["sid"].ToString() == "115")
                        //{ 
                        //    allstr += "<a href='" + dr["location"] + "?sid=" + dr["sid"] + "&realsid=" + dr["sid"] + "'><font color='#715313'>" + dr["sublevelname"] + "</font></a>"; 
                        //}
                        //else
                        //{ 
                        allstr += dr["sublevelname"];
                        //}
                        allstr += "</div>";
                        sqlsmd1.CommandText = "select * from sublevel1 where lid = '3' and uplink = '" + dr["sid"] + "' order by corder";
                        //sqlsmd1.CommandText = "select * from sublevel1_MVC where lid = '3' and uplink = '" + dr["sid"] + "' order by corder";
                        dr1 = sqlsmd1.ExecuteReader();

                        if (dr1.HasRows)
                        {
                            if (desid == dr["sid"].ToString())
                            { allstr += "<div class=menuContent style='display:'><ul style='list-style:none; margin-top:5px'>";
                            }
                            else
                            { allstr += "<div class=menuContent style='display:none'><ul style='list-style:none; margin-top:5px'>";
                             }
                        }
                        while (dr1.Read())
                        {
                            if (privarr[int.Parse(dr1["sid"].ToString())].ToString() == "1")
                            {
                                if (dr1["sid"].ToString() == "222" || dr1["sid"].ToString() == "223" || dr1["sid"].ToString() == "225")
                                {
                                    if (opencheck == "y")
                                    { allstr += "<li><span class='tfont'><a href='" + weblink + dr1["location"] + "?sid=" + dr["sid"] + "&realsid=" + dr1["sid"] + "' title='" + dr1["sublevelname"] + "'>" + dr1["sublevelname"] + "</a></span></li>"; }
                                }
                                else
                                { allstr += "<li><span class='tfont'><a href='" + weblink + dr1["location"] + "?sid=" + dr["sid"] + "&realsid=" + dr1["sid"] + "' title='" + dr1["sublevelname"] + "'>" + dr1["sublevelname"] + "</a></span></li>"; }
                            }
                        }

                        if (dr1.HasRows)
                        {
                            allstr += "</ul></div>";
                        }
                        dr1.Close();
                        dr1.Dispose();

                    }
              
            }
            dr.Close();
            dr.Dispose();

            allstr += "</div></div></div>";
            sqlsmd.Dispose();
            sqlsmd1.Dispose();
            //conn.Close();
            //conn.Dispose();
            //conn1.Close();
            //conn1.Dispose();
            //conn2.Close();
            //conn2.Dispose();
            return allstr;

        }


        public String get_dir(string path1) 
        {
             try{
             Directory.CreateDirectory(path1);
             return "1";
             }
             catch
             {
             return "0";
             }
 
            
        }

        public void dbexecute(string tmpconnstr, string sqlstr)
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ConfigurationManager.AppSettings[tmpconnstr];
            conn.Open();

            SqlCommand sqlsmd = new SqlCommand();
            sqlsmd.Connection = conn;
            sqlsmd.CommandText = sqlstr;
            sqlsmd.ExecuteNonQuery();
            sqlsmd.Dispose();
            conn.Close();
            conn.Dispose();


        }

        //新增權限
        public void addPrivtb(string bid, string comid)
        {

           // TIBE_Project.Models.NDcommon dbobj = new TIBE_Project.Models.NDcommon();
            SqlConnection tmpconn = get_conn("Aitag_DBContext");
            SqlDataReader dr;
            SqlCommand sqlsmd = new SqlCommand();
            sqlsmd.Connection = tmpconn;

            dbexecute("Aitag_DBcontext", "DELETE FROM Privtb where bid = '" + comid + "'");

            string sqlstr = "select * from Privtb where bid = '" + bid + "'";
            sqlsmd.CommandText = sqlstr;
            dr = sqlsmd.ExecuteReader();
            while (dr.Read())
            {
                dbexecute("Aitag_DBcontext", "INSERT INTO Privtb(sid, uplink, bid, chk, bmodid, bmoddate) VALUES ('" + dr["sid"].ToString() + "','" + dr["uplink"].ToString() + "','" + comid + "','" + dr["chk"].ToString() + "','" + null + "', { fn NOW() })");
            }
            dr.Close();
            sqlsmd.Dispose();
            tmpconn.Close();
            tmpconn.Dispose();
                     
        }
        //右側選單　
        public string get_leftmenu1(string cid, string cid1)
        {
        //TIBE_Project.Models.NDcommon dbobj = new TIBE_Project.Models.NDcommon();
        string sql  = "" ;
        string tmpstr  = "" ;
        string tmpCitem  = "" ;
       
        SqlConnection conn1 = new SqlConnection();
        SqlConnection conn2 = new SqlConnection();
        SqlCommand sqlcmd　 = new SqlCommand();
        SqlCommand sqlcmd1 = new SqlCommand();
        SqlDataReader rscat ;
        SqlDataReader rscat1 ;
        string Tcwebsite = "";
        //If(Me.qCwebsite.Value <> "" Then
        //    Tcwebsite = " cwebsite='" & Me.qCwebsite.Value & "' and"
        //Else
        //    Tcwebsite = " cwebsite='01' and"
        //End If
        //'Response.Write(Tcwebsite)
        //'Response.End()
        //'所屬左邊選單       
        
        conn1 = get_conn("Aitag_DBContext") ;
         conn2 = get_conn("Aitag_DBContext") ;
        // sql = "select * from  Category where" + Tcwebsite + " Cparentid = 0 and CLanguage ='" & 　 & "' order by  Citem,CLanguage, Corder" ;
         sql = "select * from  Category where" + Tcwebsite + " Cparentid = 0 order by CLanguage,Ctype,Corder";
        sqlcmd.Connection = conn1 ;
        sqlcmd.CommandText = sql;
        rscat = sqlcmd.ExecuteReader() ;
        
        tmpstr = "<table width='100%' border='0' align='center' cellpadding='0' cellspacing='0' class=c12 style='font-family:微軟正黑體'>";
       
            while(rscat.Read()){
           
            tmpstr += "<tr>" ;
            tmpstr += "<td align=left>" ;

            if (rscat["Cfunctype"] == "99")
            {
                tmpstr += "<img src=/images/node1.gif align=absmiddle border=0>";
                tmpstr += "<a href='/Webmaincontent/List?tmpa=1&cid=" + rscat["Cid"].ToString() + "' title='" + rscat["Ctitle"].ToString() + "'>";
                if (cid == rscat["Cid"].ToString())
                {
                    tmpstr += "<img src=/images/10.gif align=absmiddle border=0> ";
                }else{
                    tmpstr += "<img src=/images/09.gif align=absmiddle border=0> ";
                }
                tmpstr += rscat["Ctitle"].ToString() + "&nbsp;</a>";
             }else{
                tmpstr += "<img src=/images/node1.gif align=absmiddle border=0> ";
                tmpstr += "<a href='/Webmaincontent/List?tmpa=1&cid=" + rscat["Cid"].ToString() + "&cid1=0' title='" + rscat["Ctitle"].ToString() + "'>";
                if (cid == rscat["Cid"].ToString())
                {
                    tmpstr += "<img src=/images/folopen.gif align=absmiddle border=0>";
                }else{
                    tmpstr += "<img src=/images/folclose.gif align=absmiddle border=0>";
                }
                tmpstr += rscat["Ctitle"].ToString() + "&nbsp;</a>";
             }
            tmpstr += "</td>";
            tmpstr += "</tr>";

            if (cid == rscat["Cid"].ToString())
            {
                sql = "select * from  Category where  Cparentid = " + rscat["Cid"].ToString() + " order by  Corder";
                //'rscat.Close()
                sqlcmd1.Connection = conn2;
                sqlcmd1.CommandText = sql;
                rscat1 = sqlcmd1.ExecuteReader();

                while(rscat1.Read()){

                    tmpstr += "<tr>";
                    tmpstr += "<td align=left>";
                    if (rscat1["Cfunctype"] == "99")
                    {
                        tmpstr += "<img src=/images/line.gif align=absmiddle border=0><img src=/images/node1.gif align=absmiddle border=0>";
                        tmpstr += "<img src=/images/09.gif align=absmiddle border=0> ";
                        tmpstr += rscat1["Ctitle"].ToString();
                    }else{
                        tmpstr += "<img src=/images/line.gif align=absmiddle border=0><img src=/images/node1.gif align=absmiddle border=0>";
                        //'<a href="webmaincontent.asp?cid=<%=rscat("cid")%>&cid1=<%=rscat1("cid")%>&sid=<%=sid%>&realsid=<%=realsid%>" title="<%=trim(rscat(" Ctitle"))%>">
                        tmpstr += "<a href='/Webmaincontent/List?tmpa=1&cid=" + rscat["Cid"].ToString() + "&cid1=" + rscat1["Cid"].ToString() + "' title='" + rscat["Ctitle"].ToString() + "'>";
                        if (cid1 == rscat1["Cid"].ToString())
                        {
                            tmpstr += "<img src=/images/folopen.gif align=absmiddle border=0>";
                        }else{
                            tmpstr += "<img src=/images/folclose.gif align=absmiddle border=0>";
                        }
                        tmpstr += rscat1["Ctitle"].ToString() + "&nbsp;</a>";
                    }
                    tmpstr += "</td>";
                    tmpstr += "</tr>";

                }
                rscat1.Close();
                sqlcmd1 = null;

            }
            //tmpCitem = Trim(rscat("Citem"))
        }

        rscat.Close();
        rscat.Dispose();
        conn1.Close();
        conn1.Dispose();
        conn2.Close();
        conn2.Dispose();
        tmpstr += "</table>";
        //Response.Write(tmpstr)
        return  tmpstr ;
        }
   
        //加密
        public string Encrypt(string Text)
        {
            //單純直接就是md5(無解密)
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(Text, "MD5");
            //另一種加密(有解密)
            //return Encrypt(Text, "MATICSOFT");
        }

        //加密

        public string Encrypt(string Text, string sKey)
        {

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            byte[] inputByteArray;

            inputByteArray = Encoding.Default.GetBytes(Text);

            des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));

            des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));

            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);

            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();

            foreach (byte b in ms.ToArray())
            {

                ret.AppendFormat("{0:X2}", b);

            }

            return ret.ToString();

        }  
        //解密
        public string Decrypt(string Text)
        {

            return Decrypt(Text, "MATICSOFT");

        }

        public string Decrypt(string Text, string sKey)
        {

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            int len;

            len = Text.Length / 2;

            byte[] inputByteArray = new byte[len];

            int x, i;

            for (x = 0; x < len; x++)
            {

                i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);

                inputByteArray[x] = (byte)i;

            }

            des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));

            des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));

            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);

            cs.FlushFinalBlock();

            return Encoding.Default.GetString(ms.ToArray());

        }   

          //去掉tag
        public string get_canceltag(string tmpnote, int tmplength)
        {
            // if (tmpnote.Length >= tmplength)
            // { tmpnote = tmpnote.Substring(0, tmplength); }

            // return tmpnote;

            return tmpnote.StripTags().TakePart(tmplength);
        }

        //找sqlcommand物件,由物件作ExecuteNoQuery , ExecuteReader , ExecuteScalar
        public string get_mdtitle(SqlConnection tmpconn, string tmptagid , string tmptype , int len1)
        {

            SqlCommand tmpsqlcmd = new SqlCommand();
            tmpsqlcmd.Connection = tmpconn;
            tmpsqlcmd.CommandText = "select * from vw_DocTagTitle where TagID = " + tmptagid;
            SqlDataReader tdr;
            tdr = tmpsqlcmd.ExecuteReader();
            string tmpdata = "";
            try
            {
                   if(tdr.Read())
                   {
                       if (tmptype == "1"){
                           if (tdr["SName"].ToString().Length > len1)
                           tmpdata = tdr["SName"].ToString().Substring(0,len1) + "..";
                           else
                           tmpdata = tdr["SName"].ToString();
                       }
                       else if (tmptype == "2"){
                           if (tdr["AName"].ToString().Length > len1)
                               tmpdata = tdr["AName"].ToString().Substring(0, len1) + "..";
                           else
                               tmpdata = tdr["AName"].ToString();

                       }
                       else if (tmptype == "3"){
                           if (tdr["FName"].ToString().Length > len1)
                               tmpdata = tdr["FName"].ToString().Substring(0, len1) + "..";
                           else
                               tmpdata = tdr["FName"].ToString();
                       }
                       else if (tmptype == "0")
                       {
                           if ((tdr["SName"].ToString() + ";" + tdr["AName"].ToString() + ";" + tdr["FName"].ToString()).Length > len1)
                               tmpdata = (tdr["SName"].ToString() + ";" + tdr["AName"].ToString() + ";" + tdr["FName"].ToString()).ToString().Substring(0, len1) + "..";
                           else
                           tmpdata = tdr["SName"].ToString() + ";" + tdr["AName"].ToString() + ";" + tdr["FName"].ToString();
                       }
                   }
                  
            }
            catch
            {
                tmpdata = "";
            }
            tdr.Close();
            tdr.Dispose();
            tmpsqlcmd.Dispose();

            return tmpdata;

        }

         public string get_MB(string tmpvar){
             string redata = "";
             decimal d1 = 0;
             if (tmpvar != "") { 
             d1 = decimal.Parse(tmpvar) / (1024 * 1024);
             redata = myround(d1, 1).ToString();
             }
             else
             {
                 redata = "0";
             }
             return redata;
         }

         public string get_mvlen(string tmpvar)
         {
             string redata = "";
             decimal d1 = 0 , d2 = 0;
             int a = 0 , b = 0 , c = 0;
             if (tmpvar != "") { 
             d1 = decimal.Parse(tmpvar) / (3600) ;
             a = Convert.ToInt16(Math.Floor(d1));
             d2 = int.Parse(tmpvar) - a * 3600;
             b = Convert.ToInt16(Math.Floor(d2 / 60));
             c = (int)d2 - b * 60;
             //redata = myround(d1, 1).ToString();
             if (a > 0)
                 redata += a.ToString() + "時";
             if (b > 0)
                 redata += b.ToString() + "分";
             if (c > 0)
                 redata += c.ToString() + "秒";
             }
             else
             {
                 redata = "";
             }
             return redata;
         }

         public decimal myround(decimal tmpval, int digit1)
         {
             Decimal _dec = tmpval;
             int i = 0;
             string tmpstr = "";
             for (i = 0; i < digit1 ; i++)
             {
                 tmpstr += "0";
             }
             if (tmpstr != "")
                 tmpstr = "0." + tmpstr;
             else
                 tmpstr = "0";

             return decimal.Parse(_dec.ToString(tmpstr));
         }


         //找syslog主功能頁名稱
         public string get_sysmenuname(SqlConnection tmpconn, string tmpsid, string tmptype)
         {
             //tmptype =1　(傳支選單名)  = 2(傳link)
             SqlCommand tmpsqlcmd = new SqlCommand();
             SqlDataReader tmprs;
             string tmpLid, tmpUplinkname;
             string tmpUplink, tmpSublevelname;
             string sqlstr = "";
             sqlstr = "select Sublevelname,Lid, Uplink from Sublevel1 where Sid =" + tmpsid ;
             tmpsqlcmd.Connection = tmpconn;
             tmpsqlcmd.CommandText = sqlstr;
             try
             {
                 tmprs = tmpsqlcmd.ExecuteReader();
                 tmprs.Read();
                 tmpUplink = tmprs["Uplink"].ToString();
                 tmpSublevelname = tmprs["Sublevelname"].ToString();
                 tmpLid = tmprs["Lid"].ToString();

                 tmprs.Close();
                 if (tmptype == "1")
                 {
                     tmpUplinkname = tmpSublevelname;
                 }
                 else
                 {
                     if (tmpLid == "1") //最上層
                     { tmpUplinkname = tmpSublevelname; }
                     else
                     { tmpUplinkname = get_dbvalue(tmpconn, "select Sublevelname from Sublevel1 where Sid=" + tmpUplink) + " > " + tmpSublevelname; }

                 }
                 return tmpUplinkname;
             }
             catch
             {
                 return "";
             }

         }

     
        //取得表單編號
         public string get_billno(SqlConnection tmpconn, string billtype, string billsubtype, string tcomid, string tdptid, string tdate)
        {
            SqlCommand tmpsqlcmd = new SqlCommand();
            tmpsqlcmd.Connection = tmpconn;
            string tmpsql = "" , vcno1 = "" ;
            string no1 = "";
            string classcode = "";
            int k = 0;
            if(billtype=="P")
            {
                if (billsubtype=="1")
                {
                    classcode = "A";
                }
                else if (billsubtype=="2")
                {
                    classcode = "B";
                }
                else if (billsubtype == "3")
                {
                    classcode = "C";
                }
                else if (billsubtype == "4")
                {
                    classcode = "D";
                }
                else if (billsubtype == "5")
                {
                    classcode = "E";
                }
                else if (billsubtype == "6")
                {
                    classcode = "F";
                }
                else if (billsubtype == "7")
                {
                    classcode = "G";
                }
                else if (billsubtype == "8")
                {
                    classcode = "H";
                }
                string yy = (DateTime.Parse(tdate).Year - 2000).ToString() ;
                string mm = "" , dd = "" ;
                if (DateTime.Parse(tdate).Month < 10)
                    mm = "0" + DateTime.Parse(tdate).Month.ToString();
                else
                    mm = DateTime.Parse(tdate).Month.ToString();
                if (DateTime.Parse(tdate).Day < 10)
                    dd = "0" + DateTime.Parse(tdate).Day.ToString();
                else
                    dd = DateTime.Parse(tdate).Day.ToString();

                classcode += yy + mm + dd;
                tmpsql = "select top 1 vcno from vend_contractinv where vcno like '" + classcode + "%' and vctype = 'P' and comid = '" + tcomid + "' order by vcno desc";
                 
            }
            else if (billtype == "S")  //媒體委託單
            {
                
                    SqlConnection tmpconn1 = get_conn("Aitag_DBContext");              
                    classcode = billtype + get_dbvalue(tmpconn1, "select cmcode from Company where comid='" + tcomid + "'");
                    tmpconn1.Close();
                    tmpconn1.Dispose();

                    string yy = (DateTime.Parse(tdate).Year - 2000).ToString();
                    string mm = "", dd = "";
                    if (DateTime.Parse(tdate).Month < 10)
                        mm = "0" + DateTime.Parse(tdate).Month.ToString();
                    else
                        mm = DateTime.Parse(tdate).Month.ToString();
                    if (DateTime.Parse(tdate).Day < 10)
                        dd = "0" + DateTime.Parse(tdate).Day.ToString();
                    else
                        dd = DateTime.Parse(tdate).Day.ToString();

                    classcode += yy + mm + dd;
                    tmpsql = "select top 1 pdno as vcno from purchase where pdno like '" + classcode + "%' and comid = '" + tcomid + "' order by pdno desc";

            }
            else if (billtype == "K")  //客戶Campaine
            {

                SqlConnection tmpconn1 = get_conn("Aitag_DBContext");
                classcode = billtype + get_dbvalue(tmpconn1, "select cmcode from Company where comid='" + tcomid + "'");
                tmpconn1.Close();
                tmpconn1.Dispose();

                string yy = (DateTime.Parse(tdate).Year - 2000).ToString();
                string mm = "", dd = "";
                if (DateTime.Parse(tdate).Month < 10)
                    mm = "0" + DateTime.Parse(tdate).Month.ToString();
                else
                    mm = DateTime.Parse(tdate).Month.ToString();
                if (DateTime.Parse(tdate).Day < 10)
                    dd = "0" + DateTime.Parse(tdate).Day.ToString();
                else
                    dd = DateTime.Parse(tdate).Day.ToString();

                classcode += yy + mm + dd;
                tmpsql = "select top 1 pdno as vcno from custpurchase where pdno like '" + classcode + "%' and comid = '" + tcomid + "' order by pdno desc";

            }
            else if (billtype == "C")  //客戶合約
            {

                SqlConnection tmpconn1 = get_conn("Aitag_DBContext");
                classcode = billtype + get_dbvalue(tmpconn1, "select cmcode from Company where comid='" + tcomid + "'");
                tmpconn1.Close();
                tmpconn1.Dispose();

                string yy = (DateTime.Parse(tdate).Year - 2000).ToString();
                string mm = "", dd = "";
                if (DateTime.Parse(tdate).Month < 10)
                    mm = "0" + DateTime.Parse(tdate).Month.ToString();
                else
                    mm = DateTime.Parse(tdate).Month.ToString();
                if (DateTime.Parse(tdate).Day < 10)
                    dd = "0" + DateTime.Parse(tdate).Day.ToString();
                else
                    dd = DateTime.Parse(tdate).Day.ToString();

                classcode += yy + mm + dd;
                tmpsql = "select top 1 vcno from vend_contract where vendtype = '1' and vcno like '" + classcode + "%' and comid = '" + tcomid + "' order by vcno desc";

            }
            else if (billtype == "V")  //廠商合約
            {

                SqlConnection tmpconn1 = get_conn("Aitag_DBContext");
                classcode = billtype + get_dbvalue(tmpconn1, "select cmcode from Company where comid='" + tcomid + "'");
                tmpconn1.Close();
                tmpconn1.Dispose();

                string yy = (DateTime.Parse(tdate).Year - 2000).ToString();
                string mm = "", dd = "";
                if (DateTime.Parse(tdate).Month < 10)
                    mm = "0" + DateTime.Parse(tdate).Month.ToString();
                else
                    mm = DateTime.Parse(tdate).Month.ToString();
                if (DateTime.Parse(tdate).Day < 10)
                    dd = "0" + DateTime.Parse(tdate).Day.ToString();
                else
                    dd = DateTime.Parse(tdate).Day.ToString();

                classcode += yy + mm + dd;
                tmpsql = "select top 1 vcno from vend_contract where vendtype = '2' and vcno like '" + classcode + "%' and comid = '" + tcomid + "' order by vcno desc";

            }
            else if (billtype == "W")  //工作卡
            {

                SqlConnection tmpconn1 = get_conn("Aitag_DBContext");
                classcode = get_dbvalue(tmpconn1, "select cmcode from Company where comid='" + tcomid + "'");
                tmpconn1.Close();
                tmpconn1.Dispose();

                string yy = (DateTime.Parse(tdate).Year - 2000).ToString();
                string mm = "", dd = "";
                if (DateTime.Parse(tdate).Month < 10)
                    mm = "0" + DateTime.Parse(tdate).Month.ToString();
                else
                    mm = DateTime.Parse(tdate).Month.ToString();
                if (DateTime.Parse(tdate).Day < 10)
                    dd = "0" + DateTime.Parse(tdate).Day.ToString();
                else
                    dd = DateTime.Parse(tdate).Day.ToString();

                classcode += yy + mm + dd;
                tmpsql = "select top 1 wno as vcno from workcard where comid = '" + tcomid + "' order by wno desc";

            }
            else if (billtype == "Q")  //估價單
            {

                SqlConnection tmpconn1 = get_conn("Aitag_DBContext");
                classcode = billtype + get_dbvalue(tmpconn1, "select cmcode from Company where comid='" + tcomid + "'");
                tmpconn1.Close();
                tmpconn1.Dispose();

                string yy = (DateTime.Parse(tdate).Year - 2000).ToString();
                string mm = "", dd = "";
                if (DateTime.Parse(tdate).Month < 10)
                    mm = "0" + DateTime.Parse(tdate).Month.ToString();
                else
                    mm = DateTime.Parse(tdate).Month.ToString();
                if (DateTime.Parse(tdate).Day < 10)
                    dd = "0" + DateTime.Parse(tdate).Day.ToString();
                else
                    dd = DateTime.Parse(tdate).Day.ToString();

                classcode += yy + mm + dd;
                tmpsql = "select top 1 pdno as vcno from custpurchase where pdno like '" + classcode + "%' and comid = '" + tcomid + "' order by pdno desc";

            }
            tmpsqlcmd.CommandText = tmpsql;
            SqlDataReader tmprs = tmpsqlcmd.ExecuteReader();
            if(tmprs.Read())
            {
                if (billtype == "P" || billtype == "W")
                {
                    vcno1 = tmprs["vcno"].ToString();
                    no1 = vcno1.Substring(7, 4);
                    no1 = (int.Parse(no1) + 1).ToString();
                    k = 4 - no1.Length;
                    for (int i = 1; i <= k; i++)
                    {
                        no1 = "0" + no1;
                    }
                    vcno1 = vcno1.Substring(0, 7) + no1;
                }
                else 
                { 
                    vcno1 = tmprs["vcno"].ToString();
                    no1 = vcno1.Substring(8,4);
                    no1 = (int.Parse(no1) + 1).ToString();
                    k = 4 - no1.Length ;
                    for(int i=1;i<=k;i++)
                    {
                    no1 = "0" + no1;
                    }
                    vcno1 = vcno1.Substring(0,8) + no1;
                }
            }
            else
            {
                vcno1 = classcode + "0001";
            }
            
            tmprs.Close();
            tmprs.Dispose();

           // string tmpstr = "";
                      
            return vcno1;

        }
        //取得付款單遞增ID ;
         public string get_vcinvid(SqlConnection tmpconn, string tcomid, string tvcno)
         {
             string tvcinvid = "";
             SqlCommand tmpsqlcmd = new SqlCommand();
             tmpsqlcmd.Connection = tmpconn;
             string tmpsql = "select isnull(vcinvid,0) as vcinvid from vend_contractinv where vcno = '" + tvcno + "' and comid = '" + tcomid + "'";
             tmpsqlcmd.CommandText = tmpsql;
             tvcinvid = tmpsqlcmd.ExecuteScalar().ToString();
             return tvcinvid;
          }

         public string get_billname(string tbilltype, string tbill)
         {
             string billname = "";
             if(tbilltype=="P")
             {
                 if(tbill=="1")
                 {
                     billname = "廠商電匯請款";
                 }
                 else if(tbill=="2")
                 {
                     billname = "員工代墊款";
                 }
                 else if (tbill == "3")
                 {
                     billname = "預支單";
                 }
                 else if (tbill == "4")
                 {
                     billname = "出差旅費報支單";
                 }
                 else if (tbill == "5")
                 {
                     billname = "車資報支單";
                 }
                 else if (tbill == "6")
                 {
                     billname = "交際費";
                 }
                 else if (tbill == "7")
                 {
                     billname = "總管理處請款(例行)";
                 }
                 else if (tbill == "8")
                 {
                     billname = "總管理處請款(非例行)";
                 }
             }
             else 
             { }
             return billname;
         }

         public string get_billselectobj(string tbilltype, string tmpval)
         {
             string billname = "";
             if (tbilltype == "P")
             {
                 if(tmpval=="1")
                 {
                 billname += "<option value='1' selected>廠商電匯請款</option>";
                 }
                 else{
                 billname += "<option value='1'>廠商電匯請款</option>";
                 }

                  if(tmpval=="2")
                 {
                 billname += "<option value='2' selected>員工代墊款</option>";
                 }
                 else{
                 billname += "<option value='2'>員工代墊款</option>";
                 }

                  if(tmpval=="3")
                 {
                 billname += "<option value='3' selected>預支單</option>";
                 }
                 else{
                 billname += "<option value='3'>預支單</option>";
                 }

                  if(tmpval=="4")
                 {
                 billname += "<option value='4' selected>出差旅費報支單</option>";
                 }
                 else{
                 billname += "<option value='4'>出差旅費報支單</option>";
                 }

                  if(tmpval=="5")
                 {
                 billname += "<option value='5' selected>車資報支單</option>";
                 }
                 else{
                 billname += "<option value='5'>車資報支單</option>";
                 }

                  if(tmpval=="6")
                 {
                 billname += "<option value='6' selected>交際費</option>";
                 }
                 else{
                 billname += "<option value='6'>交際費</option>";
                 }

                  if(tmpval=="7")
                 {
                 billname += "<option value='7' selected>總管理處請款(例行)</option>";
                 }
                 else{
                     billname += "<option value='7'>總管理處請款(例行)</option>";
                 }

                   if(tmpval=="8")
                 {
                     billname += "<option value='8' selected>總管理處請款(非例行)</option>";
                 }
                 else{
                     billname += "<option value='8'>總管理處請款(非例行)</option>";
                 }

             }
             else
             { }
             return billname;
         }

         public string get_billlink(string tbilltype, string tbill)
         {
             string rdlink = "";
             if (tbilltype == "P")
             {
                 if (tbill == "1")
                 {
                     rdlink = "expense2list";
                 }
                 else if (tbill == "2")
                 {
                     rdlink = "expense1list";
                 }
                 else if (tbill == "3")
                 {
                     rdlink = "expense5list";
                 }
                 else if (tbill == "4")
                 {
                   rdlink = "expense4list"; 
                 }
                 else if (tbill == "5")
                 {
                    rdlink = "expense3list";
                 }
                 else if (tbill == "6")
                 {
                    rdlink = "expense8list";
                 }
                 else if (tbill == "7")
                 {
                   rdlink = "expense6list";
                 }
                 else if (tbill == "8")
                 {
                   rdlink = "expense7list"; 
                 }
             }
             else
             { }
             return rdlink;
         }

         #region roleflow tmpdata 帶入審核角色 rolestampid1

         public string roleflow(string tmpdata){
            if(tmpdata!="")
            {
             tmpdata += ",";
	         string[] tmpdata1 = tmpdata.Split(',');
                string tmpstr = "";
	             for(int tmpi = 0 ; tmpi < tmpdata1.Length ;tmpi++)
                 {
	                 if(tmpdata1[tmpi].Trim() != ""){
                        SqlConnection conn1 = get_conn("Aitag_DBContext");
		                 string sql = "select * from roleplay where rid = " + tmpdata1[tmpi].Trim() + "" ;
		                 SqlCommand tmpsqlcmd = new SqlCommand();
                         tmpsqlcmd.Connection = conn1;
                         tmpsqlcmd.CommandText = sql;
                         SqlDataReader getrs = tmpsqlcmd.ExecuteReader();

                         if (getrs.Read())
                             tmpstr = tmpstr + getrs["roletitle"].ToString() + " → ";

                         getrs.Close();
                         getrs.Dispose();
                         conn1.Close();
                         conn1.Dispose();
			          }		
	             }
	             
                 if(tmpstr!="") 
	             tmpstr = tmpstr.Substring(0,tmpstr.Length-2) ;
	             
	             return  tmpstr;
            }else{
	            return  "" ;
            }
         }
        #endregion

        #region getrole 帶入審核角色 rolestampid1

         public string getrole(string tmpdata)
         {
             if (tmpdata != "" && tmpdata != null)
             {
                 tmpdata += ",";
                 string[] tmpdata1 = tmpdata.Split(',');
                 string tmpstr = "";
                 for (int tmpi = 0; tmpi < tmpdata1.Length; tmpi++)
                 {
                     if (tmpdata1[tmpi].Trim() != "")
                     {
                         SqlConnection conn1 = get_conn("Aitag_DBContext");
                         string sql = "select * from roleplay where rid = " + tmpdata1[tmpi].Trim() + "";
                         SqlCommand tmpsqlcmd = new SqlCommand();
                         tmpsqlcmd.Connection = conn1;
                         tmpsqlcmd.CommandText = sql;
                         SqlDataReader getrs = tmpsqlcmd.ExecuteReader();

                         if (getrs.Read())
                             tmpstr = tmpstr + getrs["roletitle"].ToString() + " → ";

                         getrs.Close();
                         getrs.Dispose();
                         conn1.Close();
                         conn1.Dispose();
                     }
                 }

                 if (tmpstr != "")
                     tmpstr = tmpstr.Substring(0, tmpstr.Length - 2);

                 return tmpstr;
             }
             else
             {
                 return "";
             }
         }
        #endregion

         #region empflowtime  帶入審核人ID empstampid , 與簽核時間

         public string empflowtime(string tmpdata, string tmptime)
         {
             string newtmptime = tmptime.Trim();
             string[] tmptime1 = newtmptime.Split(',');
             
             if (tmpdata != "")
             {
                 tmpdata += ",";
                 string[] tmpdata1 = tmpdata.Split(',');
                 string tmpstr = "";
                 SqlConnection conn1 = get_conn("Aitag_DBContext");
                 for (int tmpi = 0; tmpi < tmpdata1.Length; tmpi++)
                 {
                     if (tmpdata1[tmpi].Trim() != "")
                     {
                         string[] atmpdata = tmpdata1[tmpi].Trim().Split(',');
                         string[] atmptime = {};
                         string tmptname = "";
                         if (tmptime1.Length >= 0)
                             atmptime = tmptime1[tmpi].Trim().Split(',');

                         for (int atmpi = 0; atmpi < atmpdata.Length; atmpi++)
                         {
                             string sql = "select * from employee where empid = " + atmpdata[atmpi].Trim();
                            
                             SqlCommand tmpsqlcmd = new SqlCommand();
                             tmpsqlcmd.Connection = conn1;
                             tmpsqlcmd.CommandText = sql;
                             SqlDataReader getrs = tmpsqlcmd.ExecuteReader();
                             tmptname = atmptime[atmpi].Trim();
                             if (getrs.Read())
                             {
                                 if (tmptime.Trim() != "")
                                      tmpstr = tmpstr + getrs["empname"].ToString() + "(" + tmptname + ") → ";
                                 else
                                     tmpstr = tmpstr + getrs["empname"].ToString() + " → ";

                             }

                             getrs.Close();
                             getrs.Dispose();
                            
                         }
                     }
                 }

                 conn1.Close();
                 conn1.Dispose();

                 if (tmpstr.Trim() != "")
                 {
                     tmpstr = tmpstr.Substring(0, tmpstr.Length - 2);
                     return tmpstr;
                 }
                 else { 
                     return "";
                 }
             }
             else
             {
                 return "";
             }
         }      

           #endregion

         #region getnewcheck1  簽核處理 , tmpbilltype 其他屬性 (可選擇性輸入)

         public string getnewcheck1(string tmpbillid,string nowrole,string tmproleall, string tmpcount, string tmpaddr, string tmpbillflowid , string tmpbilltype = "")
         {
            
              int  tmpcheck = 0;
              string firstrole = tmproleall ; 
              string[] firstrolearr = firstrole.Split(',');
             
                firstrole = firstrolearr[0];
    
                //取得每個表單所使用的虛擬角色
                string sql = "select * from billflow where billid = '" + tmpbillid + "' and ifuse='1'";
                SqlConnection conn1 = get_conn("Aitag_DBContext");
                SqlConnection conn2 = get_conn("Aitag_DBContext");
                SqlCommand tmpsqlcmd = new SqlCommand();
                SqlCommand tmpsqlcmd1 = new SqlCommand();
                tmpsqlcmd.Connection = conn1;
                tmpsqlcmd1.Connection = conn2;
                tmpsqlcmd.CommandText = sql;
                SqlDataReader tmprs = tmpsqlcmd.ExecuteReader();
                string tmpflowcheck = "";
                while(tmprs.Read()){
                tmpflowcheck = tmpflowcheck + tmprs["flowcheck"] + ",";
                } 
                tmprs.Close();
                tmprs.Dispose();
             
                string tmpvrcheck = "";
                if(tmpflowcheck!= ""){
	                tmpflowcheck = tmpflowcheck + "''";
	                sql = "select * from roleplay where rid in (" + tmpflowcheck + ") and ifrtype = 'y' ";
	                tmpsqlcmd.CommandText = sql;
                    tmprs = tmpsqlcmd.ExecuteReader();
	                 while(tmprs.Read()){
                    tmpvrcheck = tmpvrcheck + "'" + tmprs["rid"] + "',";
                    } 
                    tmprs.Close();
                    tmprs.Dispose();
                   
                }
                tmpvrcheck = tmpvrcheck + "''";
                    
	            //找出目前角色代表的虛擬角色 20160802 Mark
	            
	            sql = "select * from roleplay where allrid like '%#" + nowrole.Replace("'","")  + "#%' and rid in (" + tmpvrcheck + ")";
    	
                tmpsqlcmd.CommandText = sql;
                string virdata = "";
                tmprs = tmpsqlcmd.ExecuteReader();
                //一個實際角色在多個虛擬角色時,先抓出所有虛擬角色再來比對 20160802 Mark
	            while(tmprs.Read()){
                virdata = virdata + "'" + tmprs["rid"].ToString() + "',";
                }
                tmprs.Close();
                tmprs.Dispose();
                string[] nowrole1= {};
                if (virdata == "")
                { virdata = " "; } 
	            //if(virdata!="")
	            nowrole1 = virdata.Split(',');
                
	            sql = "select * from roleplay where rid = " + nowrole + " ";
               // tmpsqlcmd.Connection = conn1;
                tmpsqlcmd.CommandText = sql;
                tmprs = tmpsqlcmd.ExecuteReader();   
             
                string bossrid = "";
                if(tmprs.Read()){
                bossrid = tmprs["bossrid"].ToString();
                }
                tmprs.Close();
                tmprs.Dispose();
	                
                string[] tmproleall1 = tmproleall.Split(',');
                string[] tmproleall1arr = tmproleall.Split(',');
              
              string impstring = "";
              if(tmpbillflowid!="")
              {
                  //已經簽核過的,就按照原路線簽核 20160802 Mark
                     sql = "select  * from billflow where bid =" + tmpbillflowid;
	                 tmpsqlcmd.CommandText = sql;
                     SqlDataReader tmprsq = tmpsqlcmd.ExecuteReader();
	                 while(tmprsq.Read()){
				
				            string tmpdata = "" ;
                            tmpdata = tmprsq["flowcheck"].ToString();
				            tmpbillid = tmprsq["bid"].ToString();
				 
				            string[] tmpdata1 = tmpdata.Split(',');
	                        int tmpi = 0;
                            #region 繞比對                          
                            
                                for (tmpi = 0;tmpi < tmpdata1.Length;tmpi++)
                                    {
						                int check1 = 0;
						                for(int j = 0 ; j < nowrole1.Length ; j++)
                                        {
							                if(tmpdata1[tmpi].ToString().Trim()==nowrole1[j].ToString().Trim())
                                            {
							                    //因為重複指定，所以判斷如果長度不等時，表示目前角色還不到最後
								                    //已經比對到了虛擬角色 , 所以虛擬角色迴圈 , 並準備跳出 (check1=1)
									                //此行很重要,判斷同一個簽核流程重複存在一個虛擬角色,不可註解 20161025
                                                    if (tmpi >= tmproleall1.Length-1)
                                                    {
                                                        tmpi = tmpi + 1;
                                                        check1 = 1;
                                                        break;
                                                    }
								            } 
						                }
							
						                if(check1==1)
							            {    break; }
						                else
						                {
                                            if(tmpdata1[tmpi].ToString().Trim()==nowrole)
                                            {   
                                                tmpi = tmpi + 1;
                                                tmpcheck = 1;
								                break;
						                    }
                                        }
                                    }

                            #endregion

                                if (tmpi < tmpdata1.Length)
                                {
                                    //若是重複，則不加入
                                    if (impstring.IndexOf(tmpdata1[tmpi].ToString().Trim()) < 0)
                                    {

                                        sql = "select * from roleplay where rid = " + tmpdata1[tmpi].ToString().Trim() + " ";
                                        tmpsqlcmd1.CommandText = sql;
                                        SqlDataReader tmprsc1 = tmpsqlcmd1.ExecuteReader();
                                        string allrid = "";
                                        if (tmprsc1.Read())
                                            allrid = tmprsc1["allrid"].ToString();

                                        tmprsc1.Close();
                                        tmprsc1.Dispose();

                                        if (allrid != "")
                                        {
                                            //比對單位主管是否相同，不同找第一個的單位主管 

                                            if (allrid.IndexOf(bossrid) >= 0)
                                            {
                                                if (impstring.IndexOf(bossrid) < 0)
                                                    impstring = impstring + "'" + bossrid + "',";

                                            }
                                            else
                                            {
                                                //如果找不到 Boss 表示有問題 , 就結束吧 Mark 20160802
                                                tmproleall1 = tmproleall.Split(',');
                                                sql = "select * from roleplay where rid = " + tmproleall1[0] + " ";
                                                tmpsqlcmd1.CommandText = sql;
                                                tmprsc1 = tmpsqlcmd1.ExecuteReader();
                                                string bossrid1 = "";
                                                if (tmprsc1.Read())
                                                    bossrid1 = tmprsc1["bossrid"].ToString().Trim();

                                                tmprsc1.Close();
                                                tmprsc1.Dispose();

                                                if (impstring.IndexOf(bossrid1) < 0)
                                                    impstring = impstring + "'" + bossrid1 + "',";

                                            }

                                        }
                                        else
                                        {

                                            if (impstring.IndexOf(tmpdata1[tmpi]) < 0)
                                                impstring = impstring + tmpdata1[tmpi] + ",";

                                        }
                                    }
                                    tmpcheck = tmpcheck + 1;
                                }
                               
                     }

				            tmprsq.Close();
                            tmprsq.Dispose();
				           
				            if(impstring!="")
                            {
				               impstring =impstring.Substring(0,impstring.Length-1);
                            }
                            else
                            {
				               if(tmpcheck > 0)
				               impstring = "'topman'" ;
				               else
				               impstring = "";
				            }
                            conn1.Close();
                            conn1.Dispose();
                            conn2.Close();
                            conn2.Dispose();
			             if(tmpbillflowid == "")
				            return impstring + ";" + tmpbillid ;
			             else
				            return impstring ;
			             
              }else{
                  if (virdata.Trim() != "")
                  {
                      //先判斷多個虛擬角色是否有流程設定 20160802 Mark
                      for (int k = 0; k < nowrole1.Length; k++)
                      {
                          if (nowrole1[k] != "")
                          {
                              string roleval = nowrole1[k].Replace("'", "%");
                              string sqlstr = "select top 1 * from billflow where flowcheck like '" + roleval + "' and billid = '" + tmpbillid + "' and  ifuse='1'  and";
                              if (tmpaddr != "")
                                  sqlstr = sqlstr + " addr like '%" + tmpaddr + "%'  and";

                              if (tmpcount != "")
                                  sqlstr = sqlstr + " flowscount <= " + tmpcount + "  and flowecount >= " + tmpcount + "   and";
                              //其他屬性
                              if (tmpbilltype != "")
                                  sqlstr = sqlstr + " billtype like '%" + tmpbilltype + "%'  and";

                              sqlstr = sqlstr.Substring(0, sqlstr.Length - 5);
                              sqlstr = sqlstr + " order by btop";
                              string tmpdata = "";
                              string[] tmpdata1 = { };
                              tmpsqlcmd.CommandText = sqlstr;
                              SqlDataReader tmprsq = tmpsqlcmd.ExecuteReader();
                              while (tmprsq.Read())
                              {
                                  tmpdata = tmprsq["flowcheck"].ToString();
                                  tmpbillid = tmprsq["bid"].ToString();
                                  string tmpflow = tmprsq["flowcheck"].ToString();
                                  tmpdata1 = tmpdata.Split(',');
                                  int tmpi = 0;
                                  for (tmpi = 0; tmpi < tmpdata1.Length; tmpi++)
                                  {
                                      if (tmpdata1[tmpi].ToString().Trim() == nowrole1[k].ToString())
                                      {
                                          tmpi = tmpi + 1;
                                          break;
                                          // if(tmpi > tmproleall1arr.Length)
                                          // {
                                          //因為重複指定，所以判斷如果長度不等時，表示目前角色還不到最後
                                          //     if(tmpflow!=tmproleall2)
                                          //         break;				
                                           //}
                                          //  else
                                          //  {
                                          //======2016/2/26 may加的,
                                          //if (tmpdata1[tmpi] == nowrole)
                                          //{
                                          //    tmpi = tmpi + 1;
                                          //    break;
                                          //}
                                          //==========
                                          // }
                                      }
                                      else
                                      {
                                          if (tmpdata1[tmpi].ToString().Trim() == nowrole)
                                          {
                                              tmpi = tmpi + 1;
                                              break;
                                          }
                                      }
                                  }


                                  if (tmpi < tmpdata1.Length)
                                  {
                                      //若是重複，則不加入
                                      if (impstring.IndexOf(tmpdata1[tmpi]) < 0)
                                      {
                                          sql = "select * from roleplay where rid = " + tmpdata1[tmpi].ToString().Trim() + " ";
                                          tmpsqlcmd1.CommandText = sql;
                                          SqlDataReader tmprsc1 = tmpsqlcmd1.ExecuteReader();
                                          string allrid = "";
                                          if (tmprsc1.Read())
                                          {
                                              allrid = tmprsc1["allrid"].ToString();
                                          }
                                          tmprsc1.Close();
                                          tmprsc1.Dispose();

                                          if (allrid != "")
                                          {
                                              //比對單位主管是否相同，不同找第一個的單位主管 
                                              if (allrid.IndexOf(bossrid) >= 0)
                                              {
                                                  if (impstring.IndexOf(bossrid) < 0)
                                                      impstring = impstring + "'" + bossrid + "',";

                                              }
                                              else
                                              {
                                                  tmproleall1 = tmproleall.Split(',');
                                                  string bossrid1 = "";
                                                  sql = "select * from roleplay where rid = " + tmproleall1[0] + " ";
                                                  tmpsqlcmd1.CommandText = sql;
                                                  tmprsc1 = tmpsqlcmd1.ExecuteReader();
                                                  if (tmprsc1.Read())
                                                  {
                                                      bossrid1 = tmprsc1["bossrid"].ToString();
                                                  }
                                                  tmprsc1.Close();
                                                  tmprsc1.Dispose();

                                                  if (impstring.IndexOf(bossrid1) < 0)
                                                      impstring = impstring + "'" + bossrid1 + "',";

                                              }
                                          }
                                          else
                                          {

                                              if (impstring.IndexOf(tmpdata1[tmpi]) < 0)
                                                  impstring = impstring + tmpdata1[tmpi] + ",";

                                          }

                                      }
                                      tmpcheck = tmpcheck + 1;
                                  }
                              }

                              tmprsq.Close();
                              tmprsq.Dispose();

                          }
                      }

                      if (impstring != "")
                      {
                          impstring = impstring.Substring(0, impstring.Length - 1);
                      }
                      else
                      {
                          if (tmpcheck > 0)
                              impstring = "'topman'";
                          else
                              impstring = "";
                      }

                      //如果沒有虛擬角色 , 由實際角色送出 20160802
                      if (impstring != "")
                      {
                          //如果有虛擬角色 , 回傳比對到的虛擬角色 20160802
                          conn1.Close();
                          conn1.Dispose();
                          conn2.Close();
                          conn2.Dispose();
                          return impstring + ";" + tmpbillid;
                      }
                      else
                      {

                          string tmproleall3 = nowrole.Replace("'", "%");
                          string tmproleall4 = tmproleall.Replace("'", "%");

                          sql = "select top 1 * from billflow where ifuse='1' and flowcheck like '" + tmproleall3 + "' and flowcheck like '%" + tmproleall4 + "%'  and billid = '" + tmpbillid + "' and flowscount <= " + tmpcount + "  and flowecount >= " + tmpcount + " and addr like '%" + tmpaddr + "%' and replace(right(flowcheck,6),'''','') <> " + firstrole + "   and";
                          //其他屬性
                          if (tmpbilltype != "")
                              sql = sql + " billtype like '%" + tmpbilltype + "%'  and";

                          sql = sql.Substring(0, sql.Length - 5);
                          sql = sql + " order by btop";

                          tmpsqlcmd.CommandText = sql;
                          SqlDataReader tmprsq = tmpsqlcmd.ExecuteReader();

                          while (tmprsq.Read())
                          {
                              string tmpdata = tmprsq["flowcheck"].ToString();
                              tmpbillid = tmprsq["bid"].ToString();

                              string[] tmpdata1 = tmpdata.Split(',');
                              int tmpi = 0;
                              for (tmpi = 0; tmpi < tmpdata1.Length; tmpi++)
                              {
                                  if (tmpdata1[tmpi].ToString().Trim() == nowrole.ToString())
                                  {
                                      tmpi = tmpi + 1;

                                      break;

                                  }
                                  else
                                  {
                                      if (tmpdata1[tmpi].ToString().Trim() == nowrole)
                                      {
                                          tmpi = tmpi + 1;
                                          break;
                                      }
                                  }
                              }

                              if (tmpi < tmpdata1.Length)
                              {
                                  //若是重複，則不加入
                                  if (impstring.IndexOf(tmpdata1[tmpi]) < 0)
                                  {
                                      sql = "select * from roleplay where rid = " + tmpdata1[tmpi] + " ";
                                      tmpsqlcmd1.CommandText = sql;
                                      SqlDataReader tmprsc1 = tmpsqlcmd1.ExecuteReader();
                                      string allrid = "";
                                      if (tmprsc1.Read())
                                          allrid = tmprsc1["allrid"].ToString();

                                      tmprsc1.Close();
                                      tmprsc1.Dispose();

                                      if (allrid != "")
                                      {
                                          //比對單位主管是否相同，不同找第一個的單位主管 
                                          if (allrid.IndexOf(bossrid) >= 0)
                                          {
                                              if (impstring.IndexOf(bossrid) < 0)
                                                  impstring = impstring + "'" + bossrid + "',";

                                          }
                                          else
                                          {
                                              tmproleall1 = tmproleall.Split(',');
                                              string bossrid1 = "";
                                              sql = "select * from roleplay where rid = " + tmproleall1[0] + " ";
                                              tmpsqlcmd1.CommandText = sql;
                                              tmprsc1 = tmpsqlcmd1.ExecuteReader();
                                              if (tmprsc1.Read())
                                              {
                                                  bossrid1 = tmprsc1["bossrid"].ToString();
                                              }
                                              tmprsc1.Close();
                                              tmprsc1.Dispose();

                                              if (impstring.IndexOf(bossrid1) < 0)
                                                  impstring = impstring + "'" + bossrid1 + "',";

                                          }

                                      }
                                      else
                                      {

                                          if (impstring.IndexOf(tmpdata1[tmpi]) < 0)
                                              impstring = impstring + tmpdata1[tmpi].ToString().Trim() + ",";

                                      }
                                  }
                                  tmpcheck = tmpcheck + 1;
                              }
                          }
                          tmprsq.Close();
                          tmprsq.Dispose();

                          if (impstring != "")
                          {
                              impstring = impstring.Substring(0, impstring.Length - 1);
                          }
                          else
                          {
                              if (tmpcheck > 0)
                                  impstring = "'topman'";
                              else
                                  impstring = "";

                          }
                          conn1.Close();
                          conn1.Dispose();
                          conn2.Close();
                          conn2.Dispose();
                          return impstring + ";" + tmpbillid;
                     }
                  }
                  else
                  {

                      //如果沒有虛擬角色 , 由實際角色送出 20160802
                      string tmproleall3 = nowrole.Replace("'", "%");
                      string tmproleall4 = tmproleall.Replace("'", "%");

                      sql = "select top 1 * from billflow where ifuse='1' and flowcheck like '" + tmproleall3 + "' and flowcheck like '%" + tmproleall4 + "%'  and billid = '" + tmpbillid + "' and flowscount <= " + tmpcount + "  and flowecount >= " + tmpcount + " and addr like '%" + tmpaddr + "%' and replace(right(flowcheck,6),'''','') <> " + firstrole + "   and";
                      //其他屬性
                      if (tmpbilltype != "")
                          sql = sql + " billtype like '%" + tmpbilltype + "%'  and";

                      sql = sql.Substring(0, sql.Length - 5);
                      sql = sql + " order by btop";
                      
                      tmpsqlcmd.CommandText = sql;
                      SqlDataReader tmprsq = tmpsqlcmd.ExecuteReader();
                      string tmpdata = "";
                      string[] tmpdata1 = { };
                      while (tmprsq.Read())
                      {

                          tmpdata = tmprsq["flowcheck"].ToString();
                          tmpbillid = tmprsq["bid"].ToString();
                          string tmpflow = tmprsq["flowcheck"].ToString();
                          tmpdata1 = tmpdata.Split(',');
                          int tmpi = 0;
                          for (tmpi = 0; tmpi < tmpdata1.Length; tmpi++)
                          {
                              if (tmpdata1[tmpi].ToString().Trim() == nowrole.ToString())
                              {
                                  tmpi = tmpi + 1;
                                  break;

                              }
                              else
                              {
                                  if (tmpdata1[tmpi].ToString().Trim() == nowrole)
                                  {
                                      tmpi = tmpi + 1;
                                      break;
                                  }
                              }
                          }


                          if (tmpi < tmpdata1.Length)
                          {
                              //若是重複，則不加入
                              if (impstring.IndexOf(tmpdata1[tmpi].ToString().Trim()) < 0)
                              {

                                  sql = "select * from roleplay where rid = " + tmpdata1[tmpi] + " ";
                                  tmpsqlcmd1.CommandText = sql;
                                  SqlDataReader tmprsc1 = tmpsqlcmd1.ExecuteReader();
                                  string allrid = "";
                                  if (tmprsc1.Read())
                                      allrid = tmprsc1["allrid"].ToString();

                                  tmprsc1.Close();
                                  tmprsc1.Dispose();

                                  if (allrid != "")
                                  {
                                      //比對單位主管是否相同，不同找第一個的單位主管 
                                      if (allrid.IndexOf(bossrid) >= 0)
                                      {
                                          if (impstring.IndexOf(bossrid) < 0)
                                              impstring = impstring + "'" + bossrid + "',";

                                      }
                                      else
                                      {
                                          tmproleall1 = tmproleall.Split(',');
                                          string bossrid1 = "";
                                          sql = "select * from roleplay where rid = " + tmproleall1[0] + " ";
                                          tmpsqlcmd1.CommandText = sql;
                                          tmprsc1 = tmpsqlcmd1.ExecuteReader();
                                          if (tmprsc1.Read())
                                          {
                                              bossrid1 = tmprsc1["bossrid"].ToString();
                                          }
                                          tmprsc1.Close();
                                          tmprsc1.Dispose();

                                          if (impstring.IndexOf(bossrid1) < 0)
                                              impstring = impstring + "'" + bossrid1 + "',";

                                      }

                                  }
                                  else
                                  {

                                      if (impstring.IndexOf(tmpdata1[tmpi]) < 0)
                                          impstring = impstring + tmpdata1[tmpi].ToString().Trim() + ",";

                                  }
                              }
                              tmpcheck = tmpcheck + 1;
                          }
                      }
                      tmprsq.Close();
                      tmprsq.Dispose();

                      if (impstring != "")
                      {
                          impstring = impstring.Substring(0, impstring.Length - 1);
                      }
                      else
                      {
                          if (tmpcheck > 0)
                              impstring = "'topman'";
                          else
                              impstring = "";

                      }
                      conn1.Close();
                      conn1.Dispose();
                      conn2.Close();
                      conn2.Dispose();
                      return impstring + ";" + tmpbillid;
                  }
              }
           
        }
         #endregion

         #region show_blow  找表單預計流程
         public string show_blow(string tmpcomid, string tmpbill, string tmpaddr, string tmphour, string tmprole, string tmpbillflowid)
         {

             //取得每個表單所使用的虛擬角色

             string sql = "select * from billflow where billid = '" + tmpbill + "'";
             SqlConnection conn1 = get_conn("Aitag_DBContext");
             SqlCommand tmpsqlcmd = new SqlCommand();
             tmpsqlcmd.Connection = conn1;
             tmpsqlcmd.CommandText = sql;
             SqlDataReader tmprs = tmpsqlcmd.ExecuteReader();
             // set tmprs = conn.execute(sql)
             string tmpflowcheck = "";
             while (tmprs.Read())
             {
                 tmpflowcheck = tmpflowcheck + tmprs["flowcheck"] + ",";
             }
             tmprs.Close();
             tmprs.Dispose();


             string tmpvrcheck = "";
             if (tmpflowcheck != "")
             {
                 tmpflowcheck = tmpflowcheck + "''";

                 sql = "select * from roleplay where rid in (" + tmpflowcheck + ") and ifrtype = 'y' ";
                 tmpsqlcmd.Connection = conn1;
                 tmpsqlcmd.CommandText = sql;
                 tmprs = tmpsqlcmd.ExecuteReader();
                 while (tmprs.Read())
                 {
                     tmpvrcheck = tmpvrcheck + "'" + tmprs["rid"] + "',";
                 }
                 tmprs.Close();
                 tmprs.Dispose();
             }
             tmpvrcheck = tmpvrcheck + "''";

             //找傳入的角色是不是有包含角色
             //=================================
             if(tmprole.IndexOf('\'')>=0)
             {
                 tmprole = tmprole.Replace("\'", "");
             }
             sql = "select rid from roleplay where allrid like '%" + tmprole + "%' and rid in (" + tmpvrcheck + ") ";
             tmpsqlcmd.Connection = conn1;
             tmpsqlcmd.CommandText = sql;
             tmprs = tmpsqlcmd.ExecuteReader();
             string finds = "";
             if (tmprs.Read())
                 finds = tmprs["rid"].ToString();
             else
                 finds = tmprole;

             tmprs.Close();
             tmprs.Dispose();
             //================================= 

             if (tmpbillflowid == "0")
             {
                 if (tmpaddr != "")
                     sql = "select top 1 * from billflow where  billid = '" + tmpbill + "' and addr like '%" + tmpaddr + "%' and flowscount <=" + tmphour + " and flowecount>=" + tmphour + " and flowcheck like '%" + finds + "%' and replace(right(flowcheck,6),'''','') <> '" + tmprole + "' order by btop";
                 else
                     sql = "select top 1 * from billflow where billid = '" + tmpbill + "' and flowscount <=" + tmphour + " and flowecount>=" + tmphour + " and flowcheck like '%" + finds + "%' order by btop";

             }
             else
             {
                 sql = "select top 1 * from billflow where bid =" + tmpbillflowid + " order by btop";
             }
             tmpsqlcmd.Connection = conn1;
             tmpsqlcmd.CommandText = sql;
             tmprs = tmpsqlcmd.ExecuteReader();

             if (tmprs.Read())
             {
                 int findend = 0;
                 string flowcheck = tmprs["flowcheck"].ToString().Trim().Replace("'", "");
                 int findstart = flowcheck.IndexOf(finds);
                 if (findstart == -1)
                     findstart = 0;

                 //Substring抓取的是長度,不是index 
                 //flowcheck = flowcheck.Substring(findstart, flowcheck.Length - 1);
                 string[] tmpa1 = flowcheck.Split(',');

                 if ((tmpa1.Length - 1) <= int.Parse(tmprs["flowlevel"].ToString()))
                     findend = tmpa1.Length - 1;
                 else
                     findend = int.Parse(tmprs["flowlevel"].ToString());

                 string reflowcheck = "";
                 for (int i = 0; i <= findend; i++)
                 {
                     reflowcheck = reflowcheck + "'" + tmpa1[i].ToString() + "',";
                 }

                 tmprs.Close();
                 tmprs.Dispose();
                 conn1.Close();
                 conn1.Dispose();

                 if (reflowcheck != "")
                 {
                     reflowcheck = reflowcheck.Substring(0, reflowcheck.Length - 1);
                     return reflowcheck;
                 }
                 else
                 {
                     return "";
                 }
             }

             tmprs.Close();
             tmprs.Dispose();
             conn1.Close();
             conn1.Dispose();
             return "";
         }
         #endregion

         #region get_time  格式化時間 hh:mm:ss
         public string get_time(string tmpval)
         {
             if (tmpval != "")
             {
                 if (tmpval.Length == 6)
                 {
                     tmpval = tmpval.Substring(0, 2) + ":" + tmpval.Substring(2, 2) + ":" + tmpval.Substring(4, 2);
                 }
             }

             return tmpval;
         }
         #endregion

         #region   檢查是為是例假日    
         public string check_holiday(string tmpdata, string tcomid)
         {
             string tmpstr="";
             if (tmpdata != "")
             {
                SqlConnection conn1 = get_conn("Aitag_DBContext");
                string sql = "select * from holidayschedule where wsdate = '" + tmpdata + "' and comid='"+ tcomid + "'";
                SqlCommand tmpsqlcmd = new SqlCommand();
                tmpsqlcmd.Connection = conn1;
                tmpsqlcmd.CommandText = sql;
                SqlDataReader getrs = tmpsqlcmd.ExecuteReader();
                if (getrs.HasRows)
                {
                    if (getrs.Read())
                    {
                        if (getrs["wstype"].ToString().Trim() == "1")
                        { tmpstr = "1"; }
                        else
                        { tmpstr = "0"; }
                    }

                }
                else
                {
                     string Week = System.Convert.ToDateTime(tmpdata.ToString()).DayOfWeek.ToString();
                    if (Week == "Saturday" || Week == "Sunday")
                    { tmpstr = "0"; }
                    else
                    { tmpstr = "1"; }
                }
                
                getrs.Close();
                getrs.Dispose();
                conn1.Close();
                conn1.Dispose(); 
          
             }
             return tmpstr;
         }

         #endregion      

         #region FUsingle  單筆上傳
         public string FUsingle(System.Web.HttpPostedFileBase file)
         {
             if (file == null)
             {
                 return "";
             }
             else if (file.ContentLength == 0)
             {
                 return "";
             }
             else
             {
                 string Imglink = "";
                 int filesize = file.ContentLength;
                 string filename = file.FileName;
                 string vfilename = file.FileName;
                 string filetype = file.FileName.Split('.')[1].ToString();
                 string fileERROR = "";
                 string[] tmp = vfilename.Split('/');
                 vfilename = tmp[tmp.Length - 1];
                 var fileName = System.IO.Path.GetFileName(file.FileName);
                 if (filetype == "ASP" || filetype == "BAK" || filetype == "EXE" || filetype == "SPX")
                 {
                     fileERROR = "抱歉！上傳檔案格式錯誤，請重新上傳！";
                 }
                 else
                 {

                     //找序號
                     NDcommon dbobj = new NDcommon();
                     string timgsno = "";
                     using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
                     {
                         timgsno = dbobj.get_dbvalue(conn, "select (sno1 + 1) as sno1 from serno WHERE mailno = 1");
                         dbobj.dbexecute("Aitag_DBContext", "UPDATE serno SET sno1 = sno1+1 WHERE mailno = 1");
                     }
                     fileName = timgsno + "." + filetype;
                     var path = System.IO.Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/upload/"), fileName);
                     file.SaveAs(path);
                 }
                 string tmpdata = "";
                 tmpdata += "{\"files\":[";
                 tmpdata += "{\"name\":\"" + filename + "\",";
                 tmpdata += "\"size\":" + filesize + ",";
                 tmpdata += "\"type\":\"" + filetype + "\",";
                 tmpdata += "\"url\":\"" + "" + "\",";
                 tmpdata += "\"thumbnailUrl\":\"" + "" + "\",";
                 tmpdata += "\"fileERROR\":\"" + fileERROR + "\",";
                 tmpdata += "\"deleteUrl\":\"" + "" + "\",";
                 tmpdata += "\"deleteType\":\"DELETE\"}";
                 tmpdata += "]}";

                 return fileName;
             }

         }
         #endregion


         #region   檢查數字
         public string checknumber(dynamic tmpnull) //checkallstring
         {
             if (System.DBNull.Value.Equals(tmpnull) || tmpnull == null)
             { return ""; }
             else
             { return Convert.ToString(tmpnull); }
         }
         #endregion

         #region   檢查字串(取消駭客的攻擊)
         public string checkallstring(string tmpstr)
         {
             string param1 = "";
             if (tmpstr != "")
             {
                 param1 = tmpstr;
             }
             return tmpstr;
         }
         #endregion

        #region 設定排序

         public string SetOrder(string orderdata, string orderdata1, string[] od_ch, string[] od_ch1)
         {
             string SetOrder_A = @"function SetOrder_A(r,t,e){var i=""<img src='/images/aorder.gif' border='0' alt='遞增' />"",a=""<img src='/images/dorder.gif' border='0' alt='遞減' />"",c=$(""#mtable"").find(""a[name='""+r+""']"");""asc""==e?(c.attr(""title"",""遞減""),c.html(a),c.click(function(){sortform(t,""desc"")})):""desc""==e&&(c.attr(""title"",""遞增""),c.html(i),c.click(function(){sortform(t,""asc"")}))}";
             string Order_ch = "function SetOrder_ch() { ";
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
        #endregion

         #region  '找目前登入角色下可以管理的人員  '若是就回傳%,最小回傳自已
         public string get_allempid(string tmprid, string Session_empid)
         {
             NDcommon dbobj = new NDcommon();
             string tmp = "", tmpsql = "";
             tmpsql = "select rid from roleplay where bossrid = '' and ifrtype = 'n' and rid='" + tmprid + "'";
             using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
             {
                 using (SqlCommand cmd = new SqlCommand(tmpsql, conn))
                 {
                     SqlDataReader dr = cmd.ExecuteReader();
                     int i = 0;
                     while (dr.Read())
                     {
                         i += 1;
                     }
                     if (i == 1)
                     {
                         return "%";
                     }
                     else
                     {
                         tmpsql = "select rid from roleplay where bossrid = '" + tmprid + "' and ifrtype='n'";
                         tmp += get_allempid_3(tmpsql);
                     }
                     dr.Close();
                 }
             }
             string tmpallempid = "";
             if (tmp != "")
             {
                 tmpsql = "select distinct empid from viewemprole  where rid in (" + tmp.Substring(0, tmp.Length - 1) + ") and ridtype = '1' and empstatus <> '4'";
             }
             else
             {
                 tmpsql = "select empid from viewemprole where 1<>1";
             }
             using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
             {
                 string sql = "";
                 using (SqlCommand cmd = new SqlCommand(tmpsql, conn))
                 {
                     SqlDataReader dr = cmd.ExecuteReader();
                     while (dr.Read())
                     {
                         tmpallempid += "'" + dr["empid"] + "',";
                     }
                     dr.Close();
                 }
             }
             if (tmpallempid != "")
             {
                 return tmpallempid.Substring(0, tmpallempid.Length - 1);
             }
             else
             {
                 return "'" + Session_empid + "'";
             }
         }
         private string get_allempid_3(string tmpsql)
         {
             string tmp = "";
             NDcommon dbobj = new NDcommon();
             using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
             {
                 using (SqlCommand cmd = new SqlCommand(tmpsql, conn))
                 {
                     SqlDataReader dr = cmd.ExecuteReader();
                     while (dr.Read())
                     {
                         tmp += "'" + dr["rid"] + "',";
                         tmpsql = "select rid from roleplay where bossrid = '" + dr["rid"] + "' and ifrtype='n'";
                         tmp += get_allempid_4(tmpsql);
                     }
                     dr.Close();
                 }
             }
             return tmp;
         }
         private string get_allempid_4(string tmpsql)
         {
             string tmp = "";
             NDcommon dbobj = new NDcommon();
             using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
             {
                 using (SqlCommand cmd = new SqlCommand(tmpsql, conn))
                 {
                     SqlDataReader dr = cmd.ExecuteReader();
                     while (dr.Read())
                     {
                         tmp += "'" + dr["rid"] + "',";
                         tmpsql = "select rid from roleplay where bossrid = '" + dr["rid"] + "' and ifrtype='n'";
                         tmp += get_allempid_5(tmpsql);
                     }
                     dr.Close();
                 }
             }
             return tmp;
         }
         private string get_allempid_5(string tmpsql)
         {
             string tmp = "";
             NDcommon dbobj = new NDcommon();
             using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
             {
                 using (SqlCommand cmd = new SqlCommand(tmpsql, conn))
                 {
                     SqlDataReader dr = cmd.ExecuteReader();
                     while (dr.Read())
                     {
                         tmp += "'" + dr["rid"] + "',";
                         tmpsql = "select rid from roleplay where bossrid = '" + dr["rid"] + "' and ifrtype='n'";
                         tmp += get_allempid_6(tmpsql);
                     }
                     dr.Close();
                 }
             }
             return tmp;
         }
         private string get_allempid_6(string tmpsql)
         {
             string tmp = "";
             NDcommon dbobj = new NDcommon();
             using (SqlConnection conn = dbobj.get_conn("Aitag_DBContext"))
             {
                 using (SqlCommand cmd = new SqlCommand(tmpsql, conn))
                 {
                     SqlDataReader dr = cmd.ExecuteReader();
                     while (dr.Read())
                     {
                         tmp += "'" + dr["rid"] + "',";
                     }
                     dr.Close();
                 }
             }
             return tmp;
         }
         #endregion

         //變更選單session
         public string get_MenuMtid(SqlConnection tmpconn, string tmprealsid)
         {

             SqlCommand tmpsqlcmd = new SqlCommand();
             SqlDataReader tmprs;
             string tmpmtid;
             string sqlstr = "";
             sqlstr = "select mtid from sublevel1 where sid =" + tmprealsid;
             tmpsqlcmd.Connection = tmpconn;
             tmpsqlcmd.CommandText = sqlstr;
             try
             {
                 tmprs = tmpsqlcmd.ExecuteReader();
                 tmprs.Read();
                 tmpmtid = tmprs["mtid"].ToString();
                 tmprs.Close();

                 return tmpmtid;

             }
             catch
             {
                 return "";
             }

         }




         //COPY內容下的檔案
         public void copyall(ArrayList arrympath, string tmpmcid, int tmpi)
         {
             string zipdownpath = ConfigurationManager.AppSettings["zipdownpath"].ToString();//'抓到webconfig設定的值


             SqlConnection tmpconn = get_conn("Aitag_DBContext");
             SqlDataReader dirrs;
             SqlCommand sqlsmd = new SqlCommand();
             sqlsmd.Connection = tmpconn;

             string sqlstr = "select * from maincontent where mcid = " + tmpmcid;
             sqlsmd.CommandText = sqlstr;
             dirrs = sqlsmd.ExecuteReader();
             if (dirrs.Read())
             {
                 SqlConnection tmpconn1 = get_conn("Aitag_DBContext");
                 SqlDataReader dirrs1;
                 SqlCommand sqlsmd1 = new SqlCommand();
                 sqlsmd1.Connection = tmpconn1;

                 string tmpdir = "";
                 //先完成自己目錄下的檔案 , 在往下recursive
                 tmpdir = dirrs["mctitle"].ToString().Replace(" ", "_");
                 if (tmpdir != "")
                 {
                     arrympath[tmpi] = arrympath[tmpi - 1] + "\\" + tmpdir;
                 }
                 else
                 { arrympath[tmpi] = arrympath[tmpi - 1]; }
                 if (!Directory.Exists(arrympath[tmpi].ToString()))
                 {
                     Directory.CreateDirectory(arrympath[tmpi].ToString());
                 }

                 //此目錄下的檔案                     
                 sqlstr = "select * from contupload where mcid = " + dirrs["mcid"].ToString();
                 sqlsmd1.CommandText = sqlstr;
                 dirrs1 = sqlsmd1.ExecuteReader();
                 while (dirrs1.Read())
                 {
                     string sourceFileName = "", destFileName = "";

                     //把檔案COPY來
                     sourceFileName = zipdownpath + "\\upload\\" + dirrs1["cupfile"].ToString();
                     if (System.IO.File.Exists(sourceFileName))
                     {
                         destFileName = arrympath[tmpi] + "\\" + dirrs1["cfilename"].ToString();
                         System.IO.File.Copy(sourceFileName, destFileName);
                     }

                 }
                 dirrs1.Close();




                 //往下recursive
                 sqlstr = "select * from maincontent where mcparentid = " + tmpmcid;
                 sqlsmd1.CommandText = sqlstr;
                 dirrs1 = sqlsmd1.ExecuteReader();
                 while (dirrs1.Read())
                 {
                     copyall(arrympath, dirrs1["mcid"].ToString(), tmpi + 1);
                 }

                 dirrs1.Close();
                 sqlsmd1.Dispose();
                 tmpconn1.Close();
                 tmpconn1.Dispose();

             }
             dirrs.Close();
             sqlsmd.Dispose();
             tmpconn.Close();
             tmpconn.Dispose();

         }

    }
}