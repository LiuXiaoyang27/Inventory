using BD.Inventory.Entities;
using BD.Inventory.WebApi.Common;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace BD.Inventory.WebApi.App_Start
{
    /// <summary>
    /// 自定义授权
    /// </summary>
    public class UserAuthorizeAttribute: AuthorizeAttribute
    {
        /// <summary>
        /// 用户授权
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            // 获取头部token
            var httpContext = actionContext.Request.Properties["MS_HttpContext"] as HttpContextBase;

            var token = httpContext.Request.Headers["TOKEN"];

            if (token == null)
            {
                // 未携带token
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    status = HttpStatusCode.Unauthorized,
                    msg = "用户未登录，请返回登录!"
                });
            }
            else
            {
                // token校验
                JWTPlayloadInfo playload = JWTHelper.CheckToken(token);

                if (playload.Status.Equals("OK"))
                {
                    actionContext.Request.Properties.Add("playload", playload);
                    base.IsAuthorized(actionContext);
                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new
                    {
                        status = HttpStatusCode.Unauthorized,
                        msg = playload.Message
                    });
                }

            }

        }
    }
}