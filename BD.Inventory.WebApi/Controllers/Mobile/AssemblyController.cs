using BD.Inventory.WebApi.App_Start;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;

namespace BD.Inventory.WebApi.Controllers.Mobile
{
    /// <summary>
    /// 手持-程序集信息
    /// </summary>
    [ControllerGroup("Assembly", "手持-程序集信息")]
    public class AssemblyController : ApiController
    {

        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult GetVersion()
        {
            //var version = Assembly.GetExecutingAssembly().GetName().Version;
            //return Ok(version.ToString());

            //var version = "No informational version defined";
            //var assembly = Assembly.GetExecutingAssembly();
            //var attribute = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
            //if (attribute.Length > 0)
            //{
            //    version = ((AssemblyInformationalVersionAttribute)attribute[0]).InformationalVersion;
            //}
            //return Ok(version); 
            var version = ConfigurationManager.AppSettings["AppVersion"];
            return Ok(version);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public HttpResponseMessage GetApk()
        //{
        //    string apkFilePath = "BD.Inventory.WebApi.RFID1.0.2_Release.apk"; // 设置APK文件的相对路径
        //    var assembly = Assembly.GetExecutingAssembly();
        //    using (var stream = assembly.GetManifestResourceStream(apkFilePath))
        //    {
        //        if (stream != null)
        //        {
        //            var apkBytes = new byte[stream.Length];
        //            stream.Read(apkBytes, 0, apkBytes.Length);

        //            var response = Request.CreateResponse(HttpStatusCode.OK);
        //            response.Content = new ByteArrayContent(apkBytes);
        //            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        //            {
        //                FileName = "RFID.apk" // 设置下载时显示的文件名
        //            };
        //            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.android.package-archive");

        //            return response;
        //        }
        //        else
        //        {
        //            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "APK file not found.");
        //        }
        //    }
        //}

    }
}

