using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BD.Inventory.WebApi.WLNoperation.Models
{
    /// <summary>
    /// 商品请求参数
    /// </summary>
    public class Param_Goods
    {
        
        /// <summary>
        /// 每页大小
        /// </summary>
        public int limit { get; set; } = 10;

        /// <summary>
        /// 当前页
        /// </summary>
        public int page { get; set; } = 1;

        /// <summary>
        /// 是否查询所有状态（启用、停用）的商品
        /// </summary>
        public bool all_status { get; set; } = false;

        /// <summary>
        /// 条码
        /// </summary>
        public string bar_code { get; set; } = "";

        /// <summary>
        /// 修改结束时间
        /// </summary>
        public string end_time { get; set; } = "";

        public GoodsQueryExtend goods_query_extend { get; set; } = new GoodsQueryExtend();

        /// <summary>
        /// 商品编码
        /// </summary>
        public string item_code { get; set; } = "";

        /// <summary>
        /// 修改时间
        /// </summary>
        public string modify_time { get; set; } = "";

        /// <summary>
        /// 是否需要查询商品自定义属性(默认不传为false)
        /// </summary>
        public bool need_properties { get; set; } = false;

        /// <summary>
        /// 规格编码
        /// </summary>
        public string spec_code { get; set; } = "";


    }

    public class GoodsQueryExtend
    {
        public string brand_id { get; set; } = "";

        public string goods_marks { get; set; } = "";
    }
    
}