using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD.Inventory.Entities
{
    public partial class UHFInvCheck
    {
        public UHFInvCheck()
        {


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
        public DateTime? batch_date { get; set; }

        /// <summary>
        /// Desc:有效期
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? expiry_date { get; set; }

        /// <summary>
        /// Desc:数量
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? nums { get; set; }

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
        /// Desc:仓库编号
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string storage_code { get; set; }

        /// <summary>
        /// Desc:是否已盘点（0：未盘点，1：已盘点）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? has_check { get; set; }

        /// <summary>
        /// Desc:创建时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? create_time { get; set; }

        /// <summary>
        /// RFID
        /// </summary>
        public string RFID { get; set; }

        /// <summary>
        /// barcode
        /// </summary>
        public string barcode { get; set; }

    }
}
