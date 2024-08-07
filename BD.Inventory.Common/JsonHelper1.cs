using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Script.Serialization;

namespace BD.Inventory.Common
{
    public static class JsonHelper1
    {
        /// <summary>
        /// 统一返回结果
        /// </summary>
        /// <param name="code"></param>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static IHttpActionResult CreateResponse(ApiController controller, HttpStatusCode code, string msg, object data = null)
        {
            var result = new JObject
            {
                ["status"] = (int)code,
                ["msg"] = msg
            };

            if (data != null)
            {
                result["data"] = JToken.FromObject(data);
            }

            var response = controller.Request.CreateResponse(code);
            response.Content = new StringContent(result.ToString(), Encoding.UTF8, "application/json");

            return new ResponseMessageResult(response);
        }

        /// <summary>
        /// 登录成功返回数据
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="user"></param>
        /// <param name="items"></param>
        /// <param name="token"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static IHttpActionResult SuccessLogin(ApiController controller, string msg, JObject user, JArray items, JArray itemsSC, string token, HttpStatusCode code = HttpStatusCode.OK)
        {
            var data = new JObject
            {
                ["user"] = user,
                ["items"] = items,
                ["itemsSC"] = itemsSC
            };
            data["token"] = token;

            return CreateResponse(controller, code, msg, data);
        }

        /// <summary>
        /// 分页查询返回数据
        /// </summary>
        /// <param name="items"></param>
        /// <param name="currentPage"></param>
        /// <param name="totalPage"></param>
        /// <param name="totalCount"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static IHttpActionResult SuccessJson(ApiController controller, JArray items, int currentPage, int totalPage = 0, int totalCount = 0, HttpStatusCode code = HttpStatusCode.OK)
        {
            var data = new
            {
                items,
                currentPage,
                totalPage,
                totalCount
            };
            return CreateResponse(controller, code, "Success", data);
        }

        /// <summary>
        /// 返回查询结果（单条）
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static IHttpActionResult SuccessJson(ApiController controller, JObject obj, HttpStatusCode code = HttpStatusCode.OK)
        {
            return CreateResponse(controller, code, "Success", obj);
        }

        /// <summary>
        /// 返回查询结果（多条）
        /// </summary>
        /// <param name="items"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static IHttpActionResult SuccessJson(ApiController controller, JArray items, HttpStatusCode code = HttpStatusCode.OK)
        {
            var data = new
            {
                items
            };
            return CreateResponse(controller, code, "Success", data);
        }

        /// <summary>
        /// 返回查询结果（多条）
        /// </summary>
        /// <param name="items"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static IHttpActionResult SuccessJson(ApiController controller, JArray items, JArray items1, HttpStatusCode code = HttpStatusCode.OK)
        {
            var data = new
            {
                items,
                items1
            };
            return CreateResponse(controller, code, "Success", data);
        }

        /// <summary>
        /// 请求成功状态
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="data"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static IHttpActionResult SuccessJson(ApiController controller, string msg = "Success", string data = "", HttpStatusCode code = HttpStatusCode.OK)
        {
            return CreateResponse(controller, code, msg, string.IsNullOrEmpty(data) ? null : data);
        }

        /// <summary>
        /// 失败状态返回结果
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static IHttpActionResult FailJson(ApiController controller, string msg, HttpStatusCode code = HttpStatusCode.Accepted)
        {
            return CreateResponse(controller, code, msg);
        }

        /// <summary>
        /// 空数据返回结果
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static IHttpActionResult NullJson(ApiController controller, string msg, HttpStatusCode code = HttpStatusCode.NoContent)
        {
            return CreateResponse(controller, code, msg);
        }

        /// <summary>
        /// 异常状态返回结果
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static IHttpActionResult ErrorJson(ApiController controller, string msg, HttpStatusCode code = HttpStatusCode.Accepted)
        {
            return CreateResponse(controller, code, msg);
        }

        #region json 操作
        /// <summary> 
        /// JSON文本转对象,泛型方法 
        /// </summary> 
        /// <typeparam name="T">类型</typeparam> 
        /// <param name="jsonText">JSON文本</param> 
        /// <returns>指定类型的对象</returns> 
        public static T JSONToObject<T>(string jsonText)
        {
            var jss = new JavaScriptSerializer();
            return jss.Deserialize<T>(jsonText);
        }
        /// <summary> 
        /// 将JSON文本转换为数据表数据 
        /// </summary> 
        /// <param name="jsonText">JSON文本</param> 
        /// <returns>数据表字典</returns> 
        public static Dictionary<string, List<Dictionary<string, object>>> TablesDataFromJSON(string jsonText)
        {
            return JSONToObject<Dictionary<string, List<Dictionary<string, object>>>>(jsonText);
        }
        /// <summary> 
        /// 将JSON文本转换成数据行 
        /// </summary> 
        /// <param name="jsonText">JSON文本</param> 
        /// <returns>数据行的字典</returns>
        public static Dictionary<string, object> DataRowFromJSON(string jsonText)
        {
            return JSONToObject<Dictionary<string, object>>(jsonText);
        }
        #endregion

        #region List/Model 转 JArray/JObject

        /// <summary>
        /// list转JArray
        /// </summary>
        /// <typeparam name="T">泛型对象</typeparam>
        /// <param name="list">对象集合</param>
        /// <returns></returns>
        public static JArray ListToJArray<T>(List<T> list, string control = "default", bool isTree = false) where T : class, new()
        {
            if (list == null)
            {
                return null;
            }

            var settings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            };

            // 将每个对象序列化为 JSON 字符串并添加到 JArray 中
            var array = new JArray();
            foreach (var item in list)
            {
                string jsonString = JsonConvert.SerializeObject(item, settings);
                array.Add(JObject.Parse(jsonString));
            }

            return array;

        }

        /// <summary>
        /// 对象转JObject
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="model">对象</param>
        /// <returns></returns>
        public static JObject ModelToJObject<T>(T model, string control = "default", bool isTree = false) where T : class, new()
        {
            //return JsonConvert.DeserializeObject<JObject>(SerializeToJson(model));//JObject.FromObject(model);
            if (model == null)
            {
                return null;
            }

            // 自定义日期格式
            var settings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            };

            // 序列化对象到 JSON 字符串
            string jsonString = JsonConvert.SerializeObject(model, settings);

            // 将 JSON 字符串反序列化为 JObject
            return JObject.Parse(jsonString);

        }

        #endregion

    }
}
