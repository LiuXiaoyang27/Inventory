using BD.Inventory.Common;
using BD.Inventory.Entities;
using BD.Inventory.WebApi.App_Start;
using BD.Inventory.WebApi.WLNoperation;
using BD.Inventory.WebApi.WLNoperation.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
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
            return JsonHelper1.SuccessJson(this, "功能尚未开放");
            //try
            //{
            //    Stopwatch stopwatch = new Stopwatch();

            //    // 开始计时
            //    stopwatch.Start();
            //    InsertCheckBillResult i = new InsertCheckBillResult { check_bill_head_insert_count = 0, check_bill_body_insert_count = 0, storage_insert_count = 0 };
            //    //await wlnpu.ImportCheckBillAndStorage(i);
            //    var importCheckBillAndStorageTask = Task.Run(() => wlnpu.ImportCheckBillAndStorage(i));
            //    await importCheckBillAndStorageTask;
            //    stopwatch.Stop();
            //    // 转换为分钟
            //    double executionTimeInMinutes = stopwatch.Elapsed.TotalMinutes;
            //    //return JsonHelper.SuccessJson(JsonHelper.ModelToJObject(i));
            //    var result = new JObject
            //    {
            //        ["status"] = 200,
            //        ["msg"] = $"共用时{executionTimeInMinutes}分钟。",
            //        ["data"] = JToken.FromObject(JsonHelper.ModelToJObject(i))
            //    };

            //    return Ok(result);
            //}
            //catch (System.Exception ex)
            //{
            //    return Content(System.Net.HttpStatusCode.InternalServerError, new JObject { ["status"] = 500, ["msg"] = ex.Message });
            //}

        }

        /// <summary>
        /// 查询库存
        /// </summary>
        /// <param name="article_number">货号</param>
        /// <param name="bar_code">商品条码，不同条码英文逗号隔离 最多支持20个(编码为空时使用)</param>
        /// <param name="sku_code">规格编码，不同编码英文逗号隔离 最多支持20个</param>
        /// <param name="storage_code">目标仓库编码;使用,隔开;不传时默认查询所有可用仓库</param>
        /// <param name="storage_name">仓库名称</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetInvInfoTest(string article_number = "", string bar_code = "", string sku_code = "", string storage_code = "", string storage_name = "")
        {
            try
            {
                if (string.IsNullOrEmpty(storage_code))
                {
                    return JsonHelper.ErrorJson("仓库编码不能为空！");
                }

                Param_InvInfo param = new Param_InvInfo();
                param.article_number = article_number;
                param.bar_code = bar_code;
                param.sku_code = sku_code;
                param.storage_code = storage_code;
                param.storage_name = storage_name;

                if (string.IsNullOrEmpty(param.article_number) && string.IsNullOrEmpty(param.bar_code) && string.IsNullOrEmpty(param.sku_code))
                {
                    // 根据modify_time 查询
                    param.modify_time = "2003-12-10 13:45:17";
                    List<InvInfo> res = await wlnpu.GetGoodsInvInfobyModifyTime(param);
                    return JsonHelper.SuccessJson(JsonHelper.ListToJArray(res));
                }
                else
                {
                    InvInfo res = await wlnpu.GetGoodsInvInfobySku(param);
                    return JsonHelper.SuccessJson(JsonHelper.ModelToJObject(res));
                }

                //await wlnpu.ImportGoods(i);
                //return JsonHelper.SuccessJson(JsonHelper.ModelToJObject(i));



            }
            catch (System.Exception ex)
            {
                return JsonHelper.ErrorJson(ex.Message);
            }

        }

        /// <summary>
        /// 查询所有仓库
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetStorage()
        {
            try
            {
                return await wlnpu.GetStorageInfo();

            }
            catch (System.Exception ex)
            {
                return JsonHelper.ErrorJson(ex.Message);
            }

        }

        /// <summary>
        /// 查询商品分类
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> GetCatagorypage()
        {
            try
            {
                return await wlnpu.GetCatagorypage();

            }
            catch (System.Exception ex)
            {
                return JsonHelper.ErrorJson(ex.Message);
            }

        }
    }
}
