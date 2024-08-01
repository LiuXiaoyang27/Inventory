using BD.Inventory.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BD.Inventory.Entities
{
    ///<summary>
    ///商品表
    ///</summary>
    public partial class Goods
    {
        public Goods()
        {
            catagory_id = "";
            catagory_name = "";
            brand_name = "";
            unit_name = "";
            manufacturer_name = "";
            purchase_type_name = "";
            purchase_num = 0;
            remark = "";
            status = 0;
        }
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
        /// Desc:分类ID
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string catagory_id { get; set; }

        /// <summary>
        /// Desc:分类
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string catagory_name { get; set; }

        /// <summary>
        /// Desc:品牌
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string brand_name { get; set; }

        /// <summary>
        /// Desc:单位
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string unit_name { get; set; }

        /// <summary>
        /// Desc:生产商
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string manufacturer_name { get; set; }

        /// <summary>
        /// Desc:采购类型
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string purchase_type_name { get; set; }

        /// <summary>
        /// Desc:采购数量
        /// Default:
        /// Nullable:True
        /// </summary>           
        public double? purchase_num { get; set; }

        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string remark { get; set; }

        /// <summary>
        /// Desc:商品状态 0 停用 1启用
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? status { get; set; }

        /// <summary>
        /// Desc:修改时间
        /// Default:
        /// Nullable:True
        /// </summary>           
       [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime? modify_time { get; set; }

        /// <summary>
        /// Desc:删除标记
        /// Default:0
        /// Nullable:True
        /// </summary>           
        public int? is_delete_tag { get; set; }

        /// <summary>
        /// 规格集
        /// </summary>
        public List<Specifications> specs { get; set; }

    }
}
