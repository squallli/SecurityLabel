using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Aitag.Models.ViewModels;

namespace Aitag.Controllers
{
    public abstract class BaseController : Controller
    {
        public NetDoViewModel NetDo { get; set; }
        public BaseController():base()
        {   

        }
                
        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (context != null)
            {
                #region Action傳入Model字串欄位如果是null，預設為空字串string.Empty;
                ///Doko :20130716
                //if (context.ActionParameters != null && context.ActionParameters.Count > 0)
                //{
                //    foreach (var o in context.ActionParameters)
                //    {
                //        foreach (PropertyInfo pro in o.Value.GetType().GetMembers(System.Reflection.BindingFlags.GetProperty))
                //        {
                //            if ((pro.GetType() == typeof(string) || pro.GetType() == typeof(String))
                //                && pro.GetValue(o, null) == null)
                //                pro.SetValue(o, string.Empty, null);
                //        }
                //    }
                //}
                #endregion Action傳入Model字串欄位如果是null，預設為空字串string.Empty;


                this.NetDo = new NetDoViewModel();
                ParameterHiddenViewModel parameterHidden =
                 new ParameterHiddenViewModel();

                if (context.HttpContext.Request.QueryString.AllKeys.Contains("sid"))
                {
                    context.HttpContext.Session["sid"] = context.HttpContext.Request.QueryString["sid"].ToString();
                    parameterHidden.sid = context.HttpContext.Request.QueryString["sid"].ToString();
                }
                else if (context.HttpContext.Request.Form.AllKeys.Contains("sid"))
                {
                    context.HttpContext.Session["sid"] = context.HttpContext.Request.Form["sid"].ToString();
                    parameterHidden.sid = context.HttpContext.Request.Form["sid"].ToString();
                }



                if (context.HttpContext.Request.QueryString.AllKeys.Contains("realsid"))
                {
                    context.HttpContext.Session["realsid"] = context.HttpContext.Request.QueryString["realsid"].ToString();
                    parameterHidden.realsid = context.HttpContext.Request.QueryString["realsid"].ToString();
                }
                else if (context.HttpContext.Request.Form.AllKeys.Contains("realsid"))
                {
                    context.HttpContext.Session["realsid"] = context.HttpContext.Request.Form["realsid"].ToString();
                    parameterHidden.realsid = context.HttpContext.Request.Form["realsid"].ToString();
                }


                NetDo.PHV = parameterHidden;
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            if (this.NetDo != null && this.NetDo.PHV!=null)
            {

                context.Controller.ViewBag.sid = (string.IsNullOrWhiteSpace(this.NetDo.PHV.sid) ? string.Empty : this.NetDo.PHV.sid);

                context.Controller.ViewBag.realsid = (string.IsNullOrWhiteSpace(this.NetDo.PHV.realsid) ? string.Empty : this.NetDo.PHV.realsid);
            }
        }
    }
}
