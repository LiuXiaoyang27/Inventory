using BD.Inventory.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BD.Inventory.Entities
{
    ///<summary>
    ///库存信息
    ///</summary>
    public partial class InvInfo
    {
        public InvInfo()
        {


        }
        /// <summary>
        /// Desc:主键
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string id { get; set; }

        /// <summary>
        /// Desc:货号
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string article_number { get; set; }

        /// <summary>
        /// Desc:商品条码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string bar_code { get; set; }

        /// <summary>
        /// Desc:总价
        /// Default:
        /// Nullable:True
        /// </summary>           
        public decimal? cost { get; set; }

        /// <summary>
        /// Desc:商品编码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string goods_code { get; set; }

        /// <summary>
        /// Desc:最后采购金额
        /// Default:
        /// Nullable:True
        /// </summary>           
        public decimal? last_stock { get; set; }

        /// <summary>
        /// Desc:锁定库存
        /// Default:
        /// Nullable:True
        /// </summary>           
        public decimal? lock_size { get; set; }

        /// <summary>
        /// Desc:数量
        /// Default:
        /// Nullable:True
        /// </summary>           
        public decimal? quantity { get; set; }

        /// <summary>
        /// Desc:规格编码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string sku_code { get; set; }

        /// <summary>
        /// Desc:规格名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string spec_name { get; set; }

        /// <summary>
        /// Desc:在途库存
        /// Default:
        /// Nullable:True
        /// </summary>           
        public decimal? underway { get; set; }

        /// <summary>
        /// Desc:仓库编码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string storage_code { get; set; }

        /// <summary>
        /// Desc:仓库编码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string storage_name { get; set; }

        /// <summary>
        /// Desc:拉取时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime? syn_date { get; set; }

        /// <summary>
        /// 批次信息
        /// </summary>
        public List<InvBatchInfo> batchs { get; set; }

    }
}
