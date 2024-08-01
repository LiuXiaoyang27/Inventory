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

            //日志配置
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/log4net.config")));

            // 配置和启动 Quartz
            SchedulerServer.SynChronizeStart();
        }
    }
}
