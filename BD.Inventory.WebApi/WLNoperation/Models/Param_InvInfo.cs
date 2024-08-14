namespace BD.Inventory.WebApi.WLNoperation.Models
{
    /// <summary>
    /// 库存信息请求参数
    /// </summary>
    public class Param_InvInfo
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int page_no { get; set; } = 1;

        /// <summary>
        /// 每页大小
        /// </summary>
        public int page_size { get; set; } = 200;       

        /// <summary>
        /// 货号
        /// </summary>
        public string article_number { get; set; } = "";

        /// <summary>
        /// 商品条码，不同条码英文逗号隔离 最多支持20个(编码为空时使用)
        /// </summary>
        public string bar_code { get; set; } = "";

        /// <summary>
        /// 修改时间
        /// </summary>
        public string modify_time { get; set; } = "";

        /// <summary>
        /// 规格编码，不同编码英文逗号隔离 最多支持20个
        /// </summary>
        public string sku_code { get; set; } = "";

        /// <summary>
        /// 目标仓库编码;使用,隔开;不传时默认查询所有可用仓库
        /// </summary>
        public string storage_code { get; set; } = "";

        /// <summary>
        /// 目标仓库名称
        /// </summary>
        public string storage_name { get; set; } = "";

    }
}