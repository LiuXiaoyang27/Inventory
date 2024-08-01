namespace BD.Inventory.Entities.DTO
{
    public partial class GoodsDTO
    {
        /// <summary>
        /// Desc:系统商品uid
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string sys_goods_uid { get; set; }

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
        /// Desc:系统规格uid
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string sys_spec_uid { get; set; }

        /// <summary>
        /// Desc:规格编码
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string spec_code { get; set; }

        /// <summary>
        /// Desc:规格1值
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string spec1 { get; set; }

        /// <summary>
        /// Desc:规格2值
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string spec2 { get; set; }

        /// <summary>
        /// Desc:条码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string barcode { get; set; }

        /// <summary>
        /// Desc:辅助条码 多个辅助条码以分号隔开
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string barcodes { get; set; }
    }
}
