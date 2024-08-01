using BD.Inventory.WebApi.App_Start;
using System.Web.Http;

namespace BD.Inventory.WebApi.Controllers.PC
{
    /// <summary>
    /// 父控制器
    /// </summary>
    [UserAuthorize]
    public class BaseController : ApiController
    {
    }
}
