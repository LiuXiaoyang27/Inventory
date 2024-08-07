using BD.Inventory.Common;
using BD.Inventory.Entities;
using BD.Inventory.WebApi.App_Start;
using BD.Inventory.WebApi.WLNoperation;
using BD.Inventory.WebApi.WLNoperation.Models;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace BD.Inventory.WebApi.Controllers.PC
{
    [ControllerGroup("WlnTest", "PC端-万里牛测试")]
    public class WlnTestController : ApiController
    {
        private readonly WlnPublic wlnpu;

        public WlnTestController()
        {
            wlnpu = new WlnPublic();
        }

        /// <summary>
        /// 查询商品
        /// </summary>
        /// <param name="pageNo">当前页面</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="spec_code">规格编码</param>
        /// <param name="item_code">商品编码</param>
        /// <param name="modify_time">修改时间</param>
        /// <param name="bar_code">条码</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetGoodsList(int pageNo = 1, int pageSize = 10, string spec_code = "", string item_code = "", string modify_time = "", string bar_code = "")
        {
            Param_Goods param = new Param_Goods();
            param.page = pageNo;
            param.limit = pageSize;
            param.spec_code = spec_code;
            param.item_code = item_code;
            param.modify_time = modify_time;
            param.bar_code = bar_code;
            return await wlnpu.GetGoodsList(param);
            //return JsonHelper.SuccessJson("");
        }

        /// <summary>
        /// 查询盘点单
        /// </summary>
        /// <param name="pageNo">当前页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="bill_code">单据编码</param>
        /// <param name="bill_status">单据状态 1:已提交(完成) 2：待审核 0：关闭</param>
        /// <param name="create_time">创建时间，只能查近3个月</param>
        /// <param name="create_end_time">创建时间结束时间</param>
        /// <param name="storage_code">仓库编码</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetCheckBillList(int pageNo = 1, int pageSize = 10, string bill_code = "", int bill_status = -1, string create_time = "", string create_end_time = "", string storage_code = "")
        {
            Param_InvCheckBill param = new Param_InvCheckBill();
            param.page = pageNo;
            param.limit = pageSize;
            param.bill_code = bill_code;
            param.bill_status = bill_status;
            param.create_time = create_time;
            param.create_end_time = create_end_time;
            param.storage_code = storage_code;
            return await wlnpu.GetCheckBillList(param);
            //return JsonHelper.SuccessJson("");
        }

        /// <summary>
        /// 手动拉取商品数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> GoodsInsert()
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();

                // 开始计时
                stopwatch.Start();
                InsertGoodsResult i = new InsertGoodsResult { goods_insert_count = 0, spec_insert_count = 0 };
                //await wlnpu.ImportGoods(i);
                //return JsonHelper.SuccessJson(JsonHelper.ModelToJObject(i));
                var importGoodsTask = Task.Run(() => wlnpu.ImportGoods(i));
                await importGoodsTask;
                stopwatch.Stop();
                // 转换为分钟
                double executionTimeInMinutes = stopwatch.Elapsed.TotalMinutes;
                var result = new JObject
                {
                    ["status"] = 200,
                    ["msg"] = $"共用时{executionTimeInMinutes}分钟。",
                    ["data"] = JToken.FromObject(JsonHelper.ModelToJObject(i))
                };

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, new JObject { ["status"] = 500, ["msg"] = ex.Message });
            }

        }

        /// <summary>
        /// 手动拉取盘点单仓库数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IHttpActionResult> InsertCheckBillInfo()
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();

                // 开始计时
                stopwatch.Start();
                InsertCheckBillResult i = new InsertCheckBillResult { check_bill_head_insert_count = 0, check_bill_body_insert_count = 0, storage_insert_count = 0 };
                //await wlnpu.ImportCheckBillAndStorage(i);
                var importCheckBillAndStorageTask = Task.Run(() => wlnpu.ImportCheckBillAndStorage(i));
                await importCheckBillAndStorageTask;
                stopwatch.Stop();
                // 转换为分钟
                double executionTimeInMinutes = stopwatch.Elapsed.TotalMinutes;
                //return JsonHelper.SuccessJson(JsonHelper.ModelToJObject(i));
                var result = new JObject
                {
                    ["status"] = 200,
                    ["msg"] = $"共用时{executionTimeInMinutes}分钟。",
                    ["data"] = JToken.FromObject(JsonHelper.ModelToJObject(i))
                };

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return Content(System.Net.HttpStatusCode.InternalServerError, new JObject { ["status"] = 500, ["msg"] = ex.Message });
            }

        }
    }
}
