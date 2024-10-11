namespace BD.Inventory.Entities.DTO
{
    /// <summary>
    /// 创建盘点单参数
    /// </summary>
    public class Param_CreateCheckBillDTO
    {
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string storage_code { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string storage_name { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        public string bar_code { get; set; }

        /// <summary>
        /// 规格编码
        /// </summary>
        public string sku_code { get; set; }

        /// <summary>
        /// 商品分类id
        /// </summary>
        public string catagoryid { get; set; }
    }
}
