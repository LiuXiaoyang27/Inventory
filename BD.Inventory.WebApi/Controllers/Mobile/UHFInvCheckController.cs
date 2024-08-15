using BD.Inventory.Bll;
using BD.Inventory.Common;
using BD.Inventory.Entities;
using BD.Inventory.Entities.DTO;
using BD.Inventory.WebApi.App_Start;
using BD.Inventory.WebApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BD.Inventory.WebApi.Controllers.Mobile
{
    /// <summary>
    /// 手持盘点
    /// </summary>
    [ControllerGroup("UHFInvCheck", "手持-盘点作业")]
    public class UHFInvCheckController : ApiController
    {
        private const string name = "手持";
        private const string IP = "1.1.1.1";

        private readonly JWTPlayloadInfo playload = new JWTPlayloadInfo { LoginIP = IP, UserName = name };

        private readonly InvCheckBll _instance;
        /// <summary>
        /// 构造方法
        /// </summary>
        public UHFInvCheckController()
        {
            _instance = InvCheckBll.Instance;
        }

        /// <summary>
        /// 查询单号和仓库
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage SelBillAndStorage()
        {
            try
            {
                // 查询单号
                var bill_code_dt = _instance.GetBillCode("");

                // 查询仓库 目前用不到
                //var storage_dt = _instance.GetStorage("");

                return JsonHelper.SuccessJson(CommonOperation.DataTable2JArray(bill_code_dt));

            }
            catch (Exception ex)
            {

                LogHelper.LogError(playload, ex, Constant.ActionEnum.Show, "查询所有单号");
                return JsonHelper.ErrorJson(ex.Message);
            }

        }

        /// <summary>
        /// 选择单号查询数据
        /// </summary>
        /// <param name="bill_code">盘点单号</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage SelDataByBillCode(string bill_code, int pageIndex = 1, int pageSize = 10)
        {
            if (string.IsNullOrEmpty(bill_code))
            {
                return JsonHelper.ErrorJson("参数错误");
            }
            try
            {
                bool IsExist = BaseBll.Instance.IsExist("InvCheckBillBody", $"bill_code='{bill_code}'");
                if (!IsExist)
                {
                    return JsonHelper.ErrorJson("单号不存在");
                }

                ChooseBillCodeDTO model = _instance.SelDataByBillCode(bill_code, pageIndex, pageSize);

                return JsonHelper.SuccessJson(JsonHelper.ModelToJObject(model));

            }
            catch (Exception ex)
            {

                LogHelper.LogError(playload, ex, Constant.ActionEnum.Show, "选择单号查询数据");
                return JsonHelper.ErrorJson(ex.Message);
            }

        }

        /// <summary>
        /// 盘点提交
        /// </summary>
        /// <param name="RFIDs">RFID集合</param>
        /// <param name="bill_code">盘点单号</param>
        /// <param name="isRepeat">是否补盘提交 默认0：否，1:是</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage InvSubmit([FromBody] List<string> RFIDs, string bill_code, int isRepeat = 0, int pageIndex = 1, int pageSize = 10)
        {
            if (RFIDs == null || RFIDs.Count == 0)
            {
                return JsonHelper.ErrorJson("参数错误");
            }

            try
            {
                // 去重
                //List<string> RFIDList = RFIDs.Distinct().ToList();
                HashSet<string> scannedRFIDsSet = new HashSet<string>(RFIDs);

                bool result = _instance.InvSubmit(scannedRFIDsSet, bill_code, isRepeat);

                ChooseBillCodeDTO model = _instance.SelDataByBillCode(bill_code, pageIndex, pageSize);
                string msg = "";
                if (isRepeat == 0)
                {
                    msg = "盘点提交（初盘）";
                }
                if (isRepeat == 1)
                {
                    msg = "盘点提交（复盘）";
                }
                LogHelper.LogAction(playload, Constant.ActionEnum.Build, msg);

                return JsonHelper.SuccessJson(JsonHelper.ModelToJObject(model));

            }
            catch (Exception ex)
            {
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Build, "盘点提交");
                return JsonHelper.ErrorJson(ex.Message);
            }


        }

    }
}
