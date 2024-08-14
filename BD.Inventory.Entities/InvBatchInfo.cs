using BD.Inventory.Common;
using Newtonsoft.Json;
using System;

namespace BD.Inventory.Entities
{
    ///<summary>
    ///库存批次信息表
    ///</summary>
    public partial class InvBatchInfo
    {
        public InvBatchInfo()
        {


        }
        /// <summary>
        /// Desc:主键
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string id { get; set; }

        /// <summary>
        /// Desc:主表id
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string headId { get; set; }

        /// <summary>
        /// Desc:批次编码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string batch_no { get; set; }

        /// <summary>
        /// Desc:过期日期
        /// Default:
        /// Nullable:True
        /// </summary>           
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime? expired_date { get; set; }

        /// <summary>
        /// Desc:批次锁定库存
        /// Default:
        /// Nullable:True
        /// </summary>           
        public decimal? lock_size { get; set; }

        /// <summary>
        /// Desc:数量
        /// Default:
        /// Nullable:True
        /// </summary>           
        public decimal? num { get; set; }

        /// <summary>
        /// Desc:成产日期
        /// Default:
        /// Nullable:True
        /// </summary>           
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime? produce_date { get; set; }

    }
}
