using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aitag.Filters
{
    public class ParameterFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if(filterContext.HttpContext.Request.QueryString["sid"] != null)
                filterContext.Controller.ViewBag.sid =filterContext.HttpContext.Request.QueryString["sid"].ToString();

            if (filterContext.HttpContext.Request.QueryString["realsid"] != null)
                filterContext.Controller.ViewBag.realsid = filterContext.HttpContext.Request.QueryString["realsid"].ToString();

        }
    }

    public class DoAuthorizeFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["tempid"] == null || filterContext.HttpContext.Session["tempid"] == "")
                filterContext.Result = new RedirectResult("~/login/goout");

        }
    }
}