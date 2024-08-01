using BD.Inventory.WebApi.App_Start;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace BD.Inventory.WebApi.App_Start
{
    public class SwaggerConfig
    {
        /// <summary>
        /// 注册
        /// </summary>
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {

                    c.SingleApiVersion("v1", "BD.Inventory.WebApi");

                    //包含XML文件，只要包含了这个文件才能正常使用swagger在线文档
                    var xmlFile = string.Format(@"{0}/bin/BD.Inventory.WebApi.xml", AppDomain.CurrentDomain.BaseDirectory);
                    c.IncludeXmlComments(xmlFile);

                    c.CustomProvider((defaultProvider) => new SwaggerControllerDescProvider(defaultProvider, xmlFile));

                    //解决同样的接口名 传递不同参数（多个HttpGet请求只显示第一个）
                    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                    //这个是自定义的filter，判断方法是否添加头部token使用
                    c.OperationFilter<HttpAuthHeaderFilter>();

                    //设置分组名字
                    c.GroupActionsBy(apiDesc =>
                    apiDesc.GetControllerAndActionAttributes<ControllerGroupAttribute>().Any() ?
                    apiDesc.GetControllerAndActionAttributes<ControllerGroupAttribute>().First().GroupName + " - " +
                    apiDesc.GetControllerAndActionAttributes<ControllerGroupAttribute>().First().Useage : "暂未设置ControllGroup");



                })
                .EnableSwaggerUi(c =>
                {
                    c.DocumentTitle("Swagger UI");
                    //加载汉化的js文件
                    c.InjectJavaScript(System.Reflection.Assembly.GetExecutingAssembly(), "BD.Inventory.WebApi.Script.swagger.js");

                });
        }

        /// <summary>
        /// 自定义的filter，判断方法是否添加头部token使用
        /// </summary>
        public class HttpAuthHeaderFilter : IOperationFilter
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="operation"></param>
            /// <param name="schemaRegistry"></param>
            /// <param name="apiDescription"></param>
            public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
            {
                if (operation.parameters == null)
                    operation.parameters = new List<Parameter>();

                operation.parameters.Add(new Parameter { name = "token", @in = "header", description = "token", required = false, type = "string" });

            }

        }

        //添加XML解析
        //private static string GetXmlCommentsPath()
        //{
        //    return string.Format("{0}/bin/BaseWebApi.XML", AppDomain.CurrentDomain.BaseDirectory);
        //}

    }
}