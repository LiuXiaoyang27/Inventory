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
        /// <summary>
        /// 构造方法
        /// </summary>
        public InvCheckController()
        {
            _instance = InvCheckBll.Instance;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页尺寸</param>
        /// <param name="bill_code">盘点单号</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage SelInvCheckHead(int pageIndex = 1, int pageSize = 10, string bill_code = "")
        {
            playload = (JWTPlayloadInfo)Request.Properties["playload"];
            try
            {
                StringBuilder strWhere = new StringBuilder("");
                if (!string.IsNullOrEmpty(bill_code))
                {
                    strWhere.Append(" and t1.bill_code like '%" + bill_code + "%'");
                }

                var dt = _instance.SelInvCheckHead(pageSize, pageIndex, strWhere.ToString(), "t1.create_time DESC", out int records);
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
        /// <param name="bill_code"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage SelDetail(string bill_code, int pageIndex = 1, int pageSize = 10)
        {
            playload = (JWTPlayloadInfo)Request.Properties["playload"];
            if (string.IsNullOrEmpty(bill_code))
            {
                return JsonHelper.FailJson("参数不能为空！");
            }
            try
            {
                var dt = _instance.GetDetail(bill_code, pageSize, pageIndex, "b.goods_code", out int records);
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

    }
}
