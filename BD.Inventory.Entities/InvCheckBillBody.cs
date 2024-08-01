using BD.Inventory.Common;
using Newtonsoft.Json;
using System;

namespace BD.Inventory.Entities
{
    public class InvCheckBillBody
    {
        public InvCheckBillBody()
        {
            goods_code = "";
            goods_name = "";
            bill_code = "";
            batch_code = "";
            batch_date = null;
            change_size = null;
            expiry_date = null;
            index = null;
            inventory_type = "";
            nums = null;
            price = null;
            product_batch_code = "";
            quantity = null;
            quantity_start = null;
            remark = "";
            spec_code = "";
            spec_name = "";
            stock_type = null;
            total_money = null;
            unit = "";
        }
        /// <summary>
        /// Desc:主键
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string id { get; set; }

        /// <summary>
        /// Desc:商品编码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string goods_code { get; set; }

        /// <summary>
        /// Desc:商品名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string goods_name { get; set; }

        /// <summary>
        /// Desc:单据编码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string bill_code { get; set; }

        /// <summary>
        /// Desc:批次号,不返回数据
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string batch_code { get; set; }

        /// <summary>
        /// Desc:生产日期
        /// Default:
        /// Nullable:True
        /// </summary>           
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime? batch_date { get; set; }

        /// <summary>
        /// Desc:差异数量
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? change_size { get; set; }

        /// <summary>
        /// Desc:有效期
        /// Default:
        /// Nullable:True
        /// </summary>           
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime? expiry_date { get; set; }

        /// <summary>
        /// Desc:行号
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? index { get; set; }

        /// <summary>
        /// Desc:库存类型 正品 ZP 次品 CC
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string inventory_type { get; set; }

        /// <summary>
        /// Desc:数量
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? nums { get; set; }

        /// <summary>
        /// Desc:单价
        /// Default:
        /// Nullable:True
        /// </summary>           
        public decimal? price { get; set; }

        /// <summary>
        /// Desc:生产批次
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string product_batch_code { get; set; }

        /// <summary>
        /// Desc:盘点后数量
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? quantity { get; set; }

        /// <summary>
        /// Desc:盘点前数量
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? quantity_start { get; set; }

        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string remark { get; set; }

        /// <summary>
        /// Desc:规格编码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string spec_code { get; set; }

        /// <summary>
        /// Desc:规格名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string spec_name { get; set; }

        /// <summary>
        /// Desc:库存类型 0 正品 1次品
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? stock_type { get; set; }

        /// <summary>
        /// Desc:订单详情总金额
        /// Default:
        /// Nullable:True
        /// </summary>           
        public decimal? total_money { get; set; }

        /// <summary>
        /// Desc:单位
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string unit { get; set; }
    }
}
