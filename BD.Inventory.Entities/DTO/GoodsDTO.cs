namespace BD.Inventory.Entities.DTO
{
    public partial class GoodsDTO
    {

        /// <summary>
        /// Desc:商品编码
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string goods_code { get; set; }

        /// <summary>
        /// Desc:商品名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string goods_name { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string unit_name { get; set; }

        /// <summary>
        /// 规格编码
        /// </summary>
        public string spec_code { get; set; }

        /// <summary>
        /// 条码
        /// </summary>
        public string barcode { get; set; }

        /// <summary>
        /// 标准售价
        /// </summary>
        public double sale_price { get; set; }

        /// <summary>
        /// 批发价
        /// </summary>
        public double wholesale_price { get; set; }

        /// <summary>
        /// 参考进价
        /// </summary>
        public double prime_price { get; set; }
    }
}
