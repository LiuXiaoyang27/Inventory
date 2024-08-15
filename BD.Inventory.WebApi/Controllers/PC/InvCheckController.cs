using BD.Inventory.Bll;
using BD.Inventory.Common;
using BD.Inventory.Entities;
using BD.Inventory.Entities.DTO;
using BD.Inventory.WebApi.App_Start;
using BD.Inventory.WebApi.Common;
using BD.Inventory.WebApi.QuartzJob;
using BD.Inventory.WebApi.WLNoperation;
using BD.Inventory.WebApi.WLNoperation.Common;
using BD.Inventory.WebApi.WLNoperation.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace BD.Inventory.WebApi.Controllers.PC
{
    /// <summary>
    /// PC盘点作业
    /// </summary>
    [ControllerGroup("InvCheck", "PC-盘点作业")]
    public class InvCheckController : BaseController
    {
        private JWTPlayloadInfo playload;

        private readonly InvCheckBll _instance;
        private readonly WlnPublic wlnpu;
        /// <summary>
        /// 构造方法
        /// </summary>
        public InvCheckController()
        {
            _instance = InvCheckBll.Instance;
            wlnpu = new WlnPublic();
        }

        ///// <summary>
        ///// 生成盘点单号 测试接口用，方法是好用的，不过不需要在前端调用
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //public HttpResponseMessage InventoryNumberGenerator()
        //{
        //    playload = (JWTPlayloadInfo)Request.Properties["playload"];
        //    try
        //    {


        //        var number = _instance.InventoryNumberGenerator();

        //        return JsonHelper.SuccessJson("Success", number);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.LogError(playload, ex, Constant.ActionEnum.Show, "生成盘点单号");
        //        return JsonHelper.ErrorJson(ex.Message);
        //    }
        //}

        /// <summary>
        /// 分页查询盘点单头
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页尺寸</param>
        /// <param name="bill_code">盘点单号</param>
        /// <param name="status">状态 1:已提交(完成) 2：待审核 </param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage SelInvCheckHead(int pageIndex = 1, int pageSize = 10, string bill_code = "", int status = -1)
        {
            playload = (JWTPlayloadInfo)Request.Properties["playload"];
            try
            {
                StringBuilder strWhere = new StringBuilder("");
                if (!string.IsNullOrEmpty(bill_code))
                {
                    strWhere.Append(" and bill_code like '%" + bill_code + "%'");
                }
                if (status != -1)
                {
                    if (status != 1 && status != 2)
                    {
                        return JsonHelper.ErrorJson("状态参数错误！");
                    }
                    strWhere.Append($" and state={status}");
                }

                var dt = _instance.SelInvCheckHead(pageSize, pageIndex, strWhere.ToString(), "create_time DESC", out int records);
                int totalPages = PageHelper.GetPageCount(pageSize, records);
                return JsonHelper.SuccessJson(CommonOperation.DataTable2JArray(dt), pageIndex, totalPages, records);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Show, "分页查询盘点单头");
                return JsonHelper.ErrorJson(ex.Message);
            }
        }

        /// <summary>
        /// 查询详情
        /// </summary>
        /// <param name="bill_code">盘点单号</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="goods_code">商品编码</param>
        /// <param name="goods_name">商品名称</param>
        /// <param name="spec_name">规格名称</param>
        /// <param name="barcode">商品条码</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage SelDetail(string bill_code, int pageIndex = 1, int pageSize = 10, string goods_code = "", string goods_name = "", string spec_name = "", string barcode = "")
        {
            playload = (JWTPlayloadInfo)Request.Properties["playload"];
            if (string.IsNullOrEmpty(bill_code))
            {
                return JsonHelper.FailJson("参数不能为空！");
            }
            try
            {
                // 查询单号是否存在
                bool isEx = BaseBll.Instance.IsExist("InvCheckBillHead", $"bill_code='{bill_code}'");
                if (!isEx)
                {
                    return JsonHelper.FailJson("单号不存在");
                }
                StringBuilder strWhere = new StringBuilder($" b.bill_code = '{bill_code}'");
                if (!string.IsNullOrEmpty(goods_code))
                {
                    strWhere.Append(" and b.goods_code like '%" + goods_code + "%'");
                }
                if (!string.IsNullOrEmpty(goods_name))
                {
                    strWhere.Append(" and b.goods_name like '%" + goods_name + "%'");
                }
                if (!string.IsNullOrEmpty(spec_name))
                {
                    strWhere.Append(" and b.spec_name like '%" + spec_name + "%'");
                }
                if (!string.IsNullOrEmpty(barcode))
                {
                    strWhere.Append(" and b.bar_code like '%" + barcode + "%'");
                }
                // 旧逻辑 ，弃用
                //var dt = _instance.GetDetail1(bill_code, pageSize, pageIndex, "b.goods_code", out int records);

                // 新逻辑
                var dt = _instance.GetDetail(strWhere.ToString(), pageSize, pageIndex, "bill_code", out int records);
                int totalPages = PageHelper.GetPageCount(pageSize, records);
                return JsonHelper.SuccessJson(CommonOperation.DataTable2JArray(dt), pageIndex, totalPages, records);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Show, "查询详情");
                return JsonHelper.ErrorJson(ex.Message);
            }

        }

        /// <summary>
        /// 新增盘点单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> CreateCheckBill([FromBody] Param_CreateCheckBillDTO param)
        {
            playload = (JWTPlayloadInfo)Request.Properties["playload"];
            try
            {
                if (param == null)
                {
                    return JsonHelper.ErrorJson("参数错误");
                }
                if (string.IsNullOrEmpty(param.storage_code))
                {
                    return JsonHelper.ErrorJson("仓库编码不能为空！");
                }
                if (string.IsNullOrEmpty(param.storage_name))
                {
                    return JsonHelper.ErrorJson("仓库名称不能为空！");
                }

                // 万里牛所需参数
                Param_InvInfo p_wln = new Param_InvInfo
                {
                    article_number = "",
                    bar_code = param.bar_code,
                    sku_code = param.sku_code,
                    storage_code = param.storage_code,
                    storage_name = param.storage_name
                };

                string msg = "";
                bool r = false;

                if (string.IsNullOrEmpty(p_wln.article_number) && string.IsNullOrEmpty(p_wln.bar_code) && string.IsNullOrEmpty(p_wln.sku_code))
                {
                    // 根据modify_time 查询
                    p_wln.modify_time = "2003-12-10 13:45:17";
                    List<InvInfo> res = await wlnpu.GetGoodsInvInfobyModifyTime(p_wln);
                    r = _instance.CreateCheckBill(res, playload.UserName, ref msg);
                }
                else
                {
                    InvInfo res = await wlnpu.GetGoodsInvInfobySku(p_wln);
                    List<InvInfo> list = new List<InvInfo>
                    {
                        res
                    };
                    r = _instance.CreateCheckBill(list, playload.UserName, ref msg);
                }
                if (r)
                {
                    LogHelper.LogAction(playload, Constant.ActionEnum.Add, "新增盘点单");
                    return JsonHelper.SuccessJson();
                }
                else
                {
                    LogHelper.LogWarn(playload, Constant.ActionEnum.Add, $"新增盘点单:{msg}");
                    return JsonHelper.FailJson(msg);
                }

            }
            catch (Exception ex)
            {
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Add, "新增盘点单");
                return JsonHelper.ErrorJson(ex.Message);
            }

        }

        /// <summary>
        /// 删除盘点单
        /// </summary>
        /// <param name="bill_codes"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage DeleteBatch([FromBody] List<string> bill_codes)
        {
            playload = (JWTPlayloadInfo)Request.Properties["playload"];
            if (bill_codes == null || bill_codes.Count == 0)
            {
                JsonHelper.ErrorJson("请选择单号");
            }
            try
            {
                int res = _instance.DeleteBatch(bill_codes, playload.UserName);
                if (res > 0)
                {
                    LogHelper.LogAction(playload, Constant.ActionEnum.Delete, "删除盘点单");
                    return JsonHelper.SuccessJson("删除成功");
                }
                else
                {
                    LogHelper.LogWarn(playload, Constant.ActionEnum.Delete, "删除盘点单失败");
                    return JsonHelper.FailJson("删除失败");
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Delete, "删除盘点单");
                return JsonHelper.ErrorJson(ex.Message);
            }
        }

        /// <summary>
        /// 完成盘点单
        /// </summary>
        /// <param name="bill_codes"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Complete([FromBody] List<string> bill_codes)
        {
            playload = (JWTPlayloadInfo)Request.Properties["playload"];
            if (bill_codes == null || bill_codes.Count == 0)
            {
                JsonHelper.ErrorJson("请选择单号");
            }
            try
            {
                int res = _instance.Complete(bill_codes);
                if (res > 0)
                {
                    LogHelper.LogAction(playload, Constant.ActionEnum.Confirm, "完成盘点单");
                    return JsonHelper.SuccessJson("状态修改成功");
                }
                else
                {
                    LogHelper.LogWarn(playload, Constant.ActionEnum.Confirm, "完成盘点单失败");
                    return JsonHelper.FailJson("状态修改失败");
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Confirm, "完成盘点单");
                return JsonHelper.ErrorJson(ex.Message);
            }
        }



        #region 添加增量盘点单 (此逻辑暂时弃用)
        /// <summary>
        /// 同步盘点单数据
        /// </summary>
        /// <param name="bill_code"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> SyncData(string bill_code)
        {
            playload = (JWTPlayloadInfo)Request.Properties["playload"];
            if (string.IsNullOrEmpty(bill_code))
            {
                return JsonHelper.FailJson("参数不能为空！");
            }

            try
            {
                InvCheckDTO model = _instance.GetModelByBillCode(bill_code);
                if (model == null)
                {
                    return JsonHelper.FailJson("不存在该单据信息");
                }

                var dt = _instance.GetDetail(bill_code, 0, 1, "b.goods_code", out int records);

                if (records > 0)
                {
                    model.details = CommonOperation.ConvertDataTableToModelList<InvCheckDetailDTO>(dt);
                    WlnPublic wlnpu = new WlnPublic();
                    Param_InvCheckBill_Add i = ParamConversion.DTO2WNLparam(model);

                    CheckBill_AddApiResponse res = await wlnpu.AddCheckBill(i);
                    if (res.code == 0)
                    {
                        return JsonHelper.SuccessJson();
                    }
                    else
                    {
                        return JsonHelper.SuccessJson(res.message);
                    }

                    //return await wlnpu.AddCheckBill(i);
                }
                else
                {
                    return JsonHelper.FailJson("没有要同步的数据");
                }

            }
            catch (Exception ex)
            {
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Show, "同步数据");
                return JsonHelper.ErrorJson(ex.Message);
            }

        }

        /// <summary>
        /// 获取系统同步时间
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetSyncTime()
        {
            playload = (JWTPlayloadInfo)Request.Properties["playload"];
            try
            {
                string time = "";
                SyncDate model = _instance.GetSyncDate();
                if (model != null)
                {
                    DateTime date = new DateTime(model.Year, model.Month, model.Day, model.Hour, model.Minute, model.Second);
                    time = date.ToString("yyyy-MM-dd HH:mm:ss");
                    return JsonHelper.SuccessJson("Success", time);
                }
                else
                {
                    return JsonHelper.FailJson("还未设置系统时间");
                }

            }
            catch (Exception ex)
            {
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Add, "获取同步时间");
                return JsonHelper.ErrorJson(ex.Message);
            }
        }

        /// <summary>
        /// 设置系统同步时间
        /// </summary>
        /// <param name="datetime">日期时间字符串（"yyyy-MM-dd HH:mm:ss" 格式）</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage SetSyncTime(string datetime)
        {
            playload = (JWTPlayloadInfo)Request.Properties["playload"];
            if (string.IsNullOrEmpty(datetime))
            {
                return JsonHelper.ErrorJson("参数不能为空");
            }

            try
            {
                string format = "yyyy-MM-dd HH:mm:ss";
                bool isValid = DateTime.TryParseExact(
                                    datetime,
                                    format,
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None,
                                    out DateTime parsedDate);
                if (!isValid)
                {
                    return JsonHelper.ErrorJson("参数格式错误");
                }

                SyncDate model = new SyncDate();
                model.Year = parsedDate.Year;
                model.Month = parsedDate.Month;
                model.Day = parsedDate.Day;
                model.Hour = parsedDate.Hour;
                model.Minute = parsedDate.Minute;
                model.Second = parsedDate.Second;

                //查询表中是否有数据，有则修改，没有添加

                SyncDate syncDay = _instance.GetSyncDate();

                bool res = false;

                if (syncDay == null)
                {
                    res = _instance.SetSyncTime(model);

                }
                else
                {
                    model.ID = syncDay.ID;
                    res = _instance.UpdateSyncTime(model);
                }

                if (res)
                {
                    // 重新调用定时任务

                    SchedulerServer.SynChronizeStart();

                    LogHelper.LogAction(playload, Constant.ActionEnum.Add, "设置同步时间");
                    return JsonHelper.SuccessJson("设置成功！");
                }
                else
                {
                    return JsonHelper.FailJson("设置失败！");
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Add, "设置同步时间");
                return JsonHelper.ErrorJson(ex.Message);
            }



        }

        #endregion



    }
}
