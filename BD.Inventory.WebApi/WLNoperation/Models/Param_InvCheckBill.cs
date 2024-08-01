namespace BD.Inventory.WebApi.WLNoperation.Models
{
    /// <summary>
    /// 查询盘点单参数
    /// </summary>
    public class Param_InvCheckBill
    {
        /// <summary>
        /// 每页大小
        /// </summary>
        public int limit { get; set; } = 10;

        /// <summary>
        /// 当前页
        /// </summary>
        public int page { get; set; } = 1;

        /// <summary>
        /// 单据编码
        /// </summary>
        public string bill_code { get; set; } = "";

        /// <summary>
        /// 单据状态: 1:已提交(完成) 2：待审核 0：关闭
        /// </summary>
        public int bill_status { get; set; } = -1;

        /// <summary>
        /// 创建时间结束时间
        /// </summary>
        public string create_end_time { get; set; } = "";

        /// <summary>
        /// 创建时间，只能查近3个月
        /// </summary>
        public string create_time { get; set; } = "";

        /// <summary>
        /// 仓库编码
        /// </summary>
        public string storage_code { get; set; } = "";

    }
}