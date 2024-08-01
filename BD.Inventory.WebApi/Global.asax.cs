using BD.Inventory.WebApi.App_Start;
using BD.Inventory.WebApi.QuartzJob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace BD.Inventory.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            //��־����
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/log4net.config")));

            // ���ú����� Quartz
            SchedulerServer.SynChronizeStart();
        }
    }
}
