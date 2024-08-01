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
    public class ImportGoodsJob : IJobModel
    {
        /// <summary>
        /// 触发时间
        /// </summary>
        public string JobTime { get { return GetJobTime(); } set { JobTime = value; } }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get { return "ImportGoods"; } set { JobName = value; } }

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
                // 拉取商品，规格
                WlnPublic wlnp = new WlnPublic();
                var importGoodsTask = Task.Run(() => wlnp.ImportGoods(res));
                // 拉取仓库，盘点单
                //var importCheckBillAndStorageTask = Task.Run(() => wlnp.ImportCheckBillAndStorage(res1));
                // 等待所有任务完成
                //Task.WaitAll(importGoodsTask, importCheckBillAndStorageTask);
                Task.WaitAll(importGoodsTask);

                // 获取插入计数
                int goodsCount = res.goods_insert_count;
                int specCount = res.spec_insert_count;

                string content = $"定时任务-拉取万里牛数据:商品{goodsCount}条，规格{specCount}条。";
                LogHelper.LogAction(playload, Constant.ActionEnum.Quartz, content);


            }
            catch (Exception ex)
            {
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Quartz, "定时任务-拉取万里牛数据");
            }
        }

        public static string GetJobTime()
        {
            string jobTime = ConfigurationManager.AppSettings["importGoods"];// string.Format("{0} {1} {2} {3} * ?", 15, 33, 10, "*");

            return jobTime;//"00 30 17 * * ?";
        }
    }
}