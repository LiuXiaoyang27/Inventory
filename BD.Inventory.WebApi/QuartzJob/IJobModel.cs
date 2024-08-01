using Quartz;

namespace BD.Inventory.WebApi.QuartzJob
{
    public interface IJobModel : IJob
    {
        /// <summary>
        /// 任务时间
        /// </summary>
        string JobTime { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        string JobName { get; set; }
    }
}