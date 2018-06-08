using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace UniteErp
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MultipleSubmitAttribute : ActionNameSelectorAttribute
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public String Name { get; set; }
        public String Value { get; set; }

        public override Boolean IsValidName(ControllerContext controllerContext, String actionName, MethodInfo methodInfo)
        {
            Boolean result = false;
            try
            {
                String key = String.Format("{0}:{1}", Name, Value);
                ValueProviderResult value = controllerContext.Controller.ValueProvider.GetValue(key);
                if (value == null) return result;
                controllerContext.Controller.ControllerContext.RouteData.Values[Name] = Value;
                result = true;
            }
            catch (Exception ex)
            {
                logger.Info(String.Format("actionName={0}, methodInfo.Name={1}", actionName, methodInfo.Name));
                logger.Info(ex.Message);
                logger.Info(ex.StackTrace);
            }
            return result;
        }

        //public override Boolean IsValidName(ControllerContext controllerContext, String actionName, MethodInfo methodInfo)
        //{
        //    Boolean result = false;
        //    try
        //    {
        //        if (actionName.Equals(methodInfo.Name, StringComparison.InvariantCultureIgnoreCase))
        //            return true;

        //        if (!actionName.Equals("Action", StringComparison.InvariantCultureIgnoreCase))
        //            return false;

        //        var request = controllerContext.RequestContext.HttpContext.Request;
        //        result = (request[methodInfo.Name] != null);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Info(String.Format("actionName={0}, methodInfo.Name={1}", actionName, methodInfo.Name));
        //        logger.Info(ex.Message);
        //        logger.Info(ex.StackTrace);
        //    }
        //    return result;
        //}
    }
}