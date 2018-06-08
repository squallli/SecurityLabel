using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Aitag
{
    // 注意: 如需啟用 IIS6 或 IIS7 傳統模式的說明，
    // 請造訪 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Application_Start()
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
                logger.Info("iMedia: application start");

                AreaRegistration.RegisterAllAreas();

                WebApiConfig.Register(GlobalConfiguration.Configuration);
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);
                AuthConfig.RegisterAuth();

             //   AutoMapperConfig.Execute();
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
                logger.Info(ex.StackTrace);
            }
        }

        protected void Application_End(object sender, EventArgs e)
        {
            try
            {
                logger.Info("iMedia: application end");
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
                logger.Info(ex.StackTrace);
            }
        }


        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            HttpRuntimeSection runTime = (HttpRuntimeSection)WebConfigurationManager.GetSection("system.web/httpRuntime");
            //Approx 100 Kb(for page content) size has been deducted because the maxRequestLength proprty is the page size, not only the file upload size
            int maxRequestLength = (runTime.MaxRequestLength - 100) * 1024;

            //This code is used to check the request length of the page and if the request length is greater than 
            //MaxRequestLength then retrun to the same page with extra query string value action=exception

            HttpContext context = ((HttpApplication)sender).Context;
            if (context.Request.ContentLength > maxRequestLength)
            {
                IServiceProvider provider = (IServiceProvider)context;
                HttpWorkerRequest workerRequest = (HttpWorkerRequest)provider.GetService(typeof(HttpWorkerRequest));

                // Check if body contains data
                if (workerRequest.HasEntityBody())
                {
                    // get the total body length
                    int requestLength = workerRequest.GetTotalEntityBodyLength();
                    // Get the initial bytes loaded
                    int initialBytes = 0;
                    if (workerRequest.GetPreloadedEntityBody() != null)
                        initialBytes = workerRequest.GetPreloadedEntityBody().Length;
                    if (!workerRequest.IsEntireEntityBodyIsPreloaded())
                    {
                        byte[] buffer = new byte[512000];
                        // Set the received bytes to initial bytes before start reading
                        int receivedBytes = initialBytes;
                        while (requestLength - receivedBytes >= initialBytes)
                        {
                            // Read another set of bytes
                            initialBytes = workerRequest.ReadEntityBody(buffer, buffer.Length);

                            // Update the received bytes
                            receivedBytes += initialBytes;
                        }
                        initialBytes = workerRequest.ReadEntityBody(buffer, requestLength - receivedBytes);
                    }
                }
                // Redirect the user to the same page with querystring action=exception. 
                context.Response.Redirect(this.Request.Url.LocalPath + "?action=exception");
            }
        }

    }
}