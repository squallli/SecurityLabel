using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aitag.Models.ViewModels;

namespace Aitag.Controllers
{
    public abstract class BasechController : Controller
    {
        public NetDoViewModelch NetDo { get; set; }
        public BasechController()
            : base()
        {

        }

        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            this.NetDo = new NetDoViewModelch();
            ParameterchHiddenViewModel parameterchHidden =
             new ParameterchHiddenViewModel();

            if (context.HttpContext.Request.QueryString.AllKeys.Contains("cid"))
            {
               // context.HttpContext.Session["sid"] = context.HttpContext.Request.QueryString["sid"].ToString();
                parameterchHidden.cid = context.HttpContext.Request.QueryString["cid"].ToString();
            }
            else if (context.HttpContext.Request.Form.AllKeys.Contains("cid"))
            {
               // context.HttpContext.Session["sid"] = context.HttpContext.Request.Form["sid"].ToString();
                parameterchHidden.cid = context.HttpContext.Request.Form["cid"].ToString();
            }
            

            NetDo.PHV = parameterchHidden;
        }

        protected override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            if (this.NetDo != null && this.NetDo.PHV != null)
            {
                context.Controller.ViewBag.cid = (string.IsNullOrWhiteSpace(this.NetDo.PHV.cid) ? string.Empty : this.NetDo.PHV.cid);
                             
            }
        }
    }
}
