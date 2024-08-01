using BD.Inventory.Common;
using BD.Inventory.Entities;
using BD.Inventory.WebApi.Common;
using BD.Inventory.WebApi.WLNoperation;
using Quartz;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace BD.Inventory.WebApi.QuartzJob
{
    /// <summary>
    /// 定时任务，导入万里牛基础数据
    /// </summary>
    public class ImportCheckBillJob : IJobModel
    {
        /// <summary>
        /// 触发时间
        /// </summary>
        public string JobTime { get { return GetJobTime(); } set { JobTime = value; } }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get { return "ImportCheckBill"; } set { JobName = value; } }

        /// <summary>
        /// 任务具体逻辑
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            var playload = new JWTPlayloadInfo { LoginIP = "0.0.0.0", UserName = "主机" };
            try
            {
                InsertGoodsResult res = new InsertGoodsResult
                {
                    goods_insert_count = 0,
                    spec_insert_count = 0
                };
                InsertCheckBillResult res1 = new InsertCheckBillResult
                {
                    check_bill_body_insert_count = 0,
                    check_bill_head_insert_count = 0,
                    storage_insert_count = 0
                };
                // 拉取商品，规格
                WlnPublic wlnp = new WlnPublic();
                //var importGoodsTask = Task.Run(() => wlnp.ImportGoods(res));
                // 拉取仓库，盘点单
                var importCheckBillAndStorageTask = Task.Run(() => wlnp.ImportCheckBillAndStorage(res1));
                // 等待所有任务完成
                //Task.WaitAll(importGoodsTask, importCheckBillAndStorageTask);
                Task.WaitAll(importCheckBillAndStorageTask);

                // 获取插入计数
                int storageCount = res1.storage_insert_count;
                int billHeadCount = res1.check_bill_head_insert_count;
                int billBodyCount = res1.check_bill_body_insert_count;

                string content = $"定时任务-拉取万里牛数据:仓库{storageCount}条，盘点单头{billHeadCount}条，盘点单体{billBodyCount}条。";
                LogHelper.LogAction(playload, Constant.ActionEnum.Quartz, content);


            }
            catch (Exception ex)
            {
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Quartz, "定时任务-拉取万里牛数据");
            }
        }

        public static string GetJobTime()
        {
            string jobTime = ConfigurationManager.AppSettings["importGoods"];// string.Format("{0} {1} {2} {3} * ?", 30, 27, 14, "*");

            return jobTime;//"59 30 17 20 * ?";
        }
    }
}