using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniteErp
{
    public abstract class SessionUtil
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const String SESSION_AC_EMPID = @"tempid";
        private const String SESSION_AC_DPTID = @"Dptid";
        private const String SESSION_AC_MSID = @"AC_Msid";
        private const String SESSION_LOGIN_TIME = @"sldate";
        private const String SESSION_LOGIN_IP = @"sfip";
        private const String SESSION_PAGE_NUM = @"pagesize";
        private const String SESSION_REAL_SID = @"realsid";
        private const String SESSION_SID = @"sid";
        private const String SESSION_DOC = @"doc";
        private const String SESSION_DOC_FILES = @"doc_files";

        public static String ac_empid
        {
            get { return String.Format("{0}", HttpContext.Current.Session[SESSION_AC_EMPID]); }
        }

        public static Int32 ac_dptid
        {
            get { return HttpContext.Current.Session[SESSION_AC_DPTID] == null ? 0 : Convert.ToInt32(HttpContext.Current.Session[SESSION_AC_DPTID]); }
        }

        public static String ac_msid
        {
            get { return String.Format("{0}", HttpContext.Current.Session[SESSION_AC_MSID]); }
        }

        public static DateTime login_time
        {
            get { return HttpContext.Current.Session[SESSION_LOGIN_TIME] == null ? DateTime.Now : Convert.ToDateTime(HttpContext.Current.Session[SESSION_LOGIN_TIME]); }
        }

        public static String login_ip
        {
            get { return String.Format("{0}", HttpContext.Current.Session[SESSION_LOGIN_IP]); }
        }

        public static Int32 page_num
        {
            get { return HttpContext.Current.Session[SESSION_PAGE_NUM] == null ? 20 : Convert.ToInt32(HttpContext.Current.Session[SESSION_PAGE_NUM]); }
        }

        public static String realsid
        {
            get
            {
                String result = HttpContext.Current.Request.QueryString[SESSION_REAL_SID];
                if (String.IsNullOrEmpty(result))
                    result = String.Format("{0}", HttpContext.Current.Session[SESSION_REAL_SID]);
                else
                    HttpContext.Current.Session[SESSION_REAL_SID] = result;
                return result;
            }
        }

        public static String sid
        {
            get
            {
                String result = HttpContext.Current.Request.QueryString[SESSION_SID];
                if (String.IsNullOrEmpty(result))
                    result = String.Format("{0}", HttpContext.Current.Session[SESSION_SID]);
                else
                    HttpContext.Current.Session[SESSION_SID] = result;
                return result;
            }
        }

       
    }
}
