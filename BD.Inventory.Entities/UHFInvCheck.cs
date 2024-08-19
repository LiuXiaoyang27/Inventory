using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD.Inventory.Entities
{
    ///<summary>
    ///手持库存盘点表
    ///</summary>
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
        /// Desc:RFID
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string RFID { get; set; }

        /// <summary>
        /// Desc:条码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string barcode { get; set; }

    }

}

