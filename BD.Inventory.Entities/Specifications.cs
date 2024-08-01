namespace BD.Inventory.Entities
{
    public class Specifications
    {
        public Specifications()
        {
            spec1 = "";
            spec2 = "";
            barcode = "";
            barcodes = "";
            pic = "";
            height = 0;
            length = 0;
            width = 0;
            weight = 0;
            sale_price = 0;
            wholesale_price = 0;
            prime_price = 0;
            status = 0;
        }
        /// <summary>
        /// Desc:系统规格uid
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string sys_spec_uid { get; set; }

        /// <summary>
        /// Desc:商品编码
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string goods_code { get; set; }

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

        /// <summary>
        /// Desc:规格图片
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string pic { get; set; }

        /// <summary>
        /// Desc:高度
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? height { get; set; }

        /// <summary>
        /// Desc:长度
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? length { get; set; }

        /// <summary>
        /// Desc:宽度
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? width { get; set; }

        /// <summary>
        /// Desc:重量
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? weight { get; set; }

        /// <summary>
        /// Desc:标准售价
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? sale_price { get; set; }

        /// <summary>
        /// Desc:批发价
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? wholesale_price { get; set; }

        /// <summary>
        /// Desc:参考进价
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? prime_price { get; set; }

        /// <summary>
        /// Desc:商品规格状态 0 停用 1启用
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? status { get; set; }

    }
}
