using BD.Inventory.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BD.Inventory.Entities
{
    public class InvCheckBillHead
    {
        public InvCheckBillHead()
        {
            bill_code = "";
            bill_creater = "";
            bill_date = null;
            create_time = null;
            remark = "";
            storage_code = "";
            storage_name = "";
        }
        /// <summary>
        /// Desc:主键
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string id { get; set; }

        /// <summary>
        /// Desc:单据编码
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string bill_code { get; set; }

        /// <summary>
        /// Desc:创建者
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string bill_creater { get; set; }

        /// <summary>
        /// Desc:业务日期
        /// Default:
        /// Nullable:True
        /// </summary>           
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime? bill_date { get; set; }

        /// <summary>
        /// Desc:创建时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime? create_time { get; set; }

        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string remark { get; set; }

        /// <summary>
        /// Desc:状态 1:已提交(完成) 2：待审核 0：关闭
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? state { get; set; }

        /// <summary>
        /// Desc:仓库编号
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string storage_code { get; set; }

        /// <summary>
        /// Desc:仓库名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string storage_name { get; set; }

        /// <summary>
        /// Desc:
        /// Default:0
        /// Nullable:True
        /// </summary>           
        public int? is_delete_tag { get; set; }

        /// <summary>
        /// 盘点单表体
        /// </summary>
        public List<InvCheckBillBody> details { get; set; }
    }
}
