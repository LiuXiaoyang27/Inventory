using BD.Inventory.Bll;
using BD.Inventory.Common;
using BD.Inventory.Entities;
using BD.Inventory.Entities.DTO;
using BD.Inventory.WebApi.Common;
using BD.Inventory.WebApi.WLNoperation;
using BD.Inventory.WebApi.WLNoperation.Common;
using BD.Inventory.WebApi.WLNoperation.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BD.Inventory.WebApi.QuartzJob
{
    /// <summary>
    /// 定时任务，同步盘点数据
    /// </summary>
    public class SyncDataJob : IJobModel
    {
        /// <summary>
        /// 触发时间
        /// </summary>
        public string JobTime { get { return GetJobTime(); } set { JobTime = value; } }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get { return "SyncData"; } set { JobName = value; } }

        /// <summary>
        /// 任务具体逻辑
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            var playload = new JWTPlayloadInfo { LoginIP = "0.0.0.0", UserName = "主机" };
            try
            {

                Task.Run(() => SyncDataJobExecute());
                //Task.WaitAll(syncDataTask);               

                string content = $"定时任务-同步盘点单数据。";
                LogHelper.LogAction(playload, Constant.ActionEnum.Quartz, content);


            }
            catch (Exception ex)
            {
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Quartz, "定时任务-同步盘点单数据");
            }
        }

        // =============================定时任务 ====================================================

        /// <summary>
        /// 同步数据
        /// </summary>
        /// <returns></returns>
        public async Task SyncDataJobExecute()
        {
            WlnPublic wlnp = new WlnPublic();
            InvCheckBll _instance = InvCheckBll.Instance;
            var dt = _instance.SelInvCheckHead(0, 1, "", "t1.create_time", out int records);
            List<InvCheckDTO> headList = CommonOperation.ConvertDataTableToModelList<InvCheckDTO>(dt);
            if (headList != null && headList.Count > 0)
            {
                foreach (var head in headList)
                {
                    var dt1 = _instance.GetDetail(head.bill_code, 0, 1, "b.goods_code", out int records1);
                    if (records1 > 0)
                    {
                        head.details = CommonOperation.ConvertDataTableToModelList<InvCheckDetailDTO>(dt1);
                        Param_InvCheckBill_Add i = ParamConversion.DTO2WNLparam(head);

                        CheckBill_AddApiResponse res = await wlnp.AddCheckBill(i);
                        //return JsonHelper.SuccessJson(JsonHelper.ModelToJObject(res));
                        //return await wlnpu.AddCheckBill(i);
                    }
                    
                }

            }


        }

        public static string GetJobTime()
        {
            string jobTime;

            SyncDate model = InvCheckBll.Instance.GetSyncDate();

            if (model == null)
            {
                model = new SyncDate
                {
                    Year = 2000,
                    Month = 1,
                    Day = 1,
                    Hour = 0,
                    Minute = 0,
                    Second = 0
                };

            }

            jobTime = string.Format("{0} {1} {2} {3} {4} ? {5}", model.Second, model.Minute, model.Hour, model.Day, model.Month, model.Year);

            return jobTime;//"59 30 17 20 * ?";
        }
    }
}