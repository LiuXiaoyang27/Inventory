using BD.Inventory.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BD.Inventory.WebApi.WLNoperation.Models
{
    /// <summary>
    /// 查询商品返回对象
    /// </summary>
    public class GoodsApiResponse
    {
        public int code { get; set; }
        public List<Goods> data { get; set; }
    }

    /// <summary>
    /// 查询盘点单返回对象
    /// </summary>
    public class CheckBillApiResponse
    {
        public int code { get; set; }
        public List<InvCheckBillHead> data { get; set; }
    }

    /// <summary>
    /// 添加增量盘点单返回对象
    /// </summary>
    public class CheckBill_AddApiResponse
    {
        public int code { get; set; }
        public string data { get; set; }
        public string message { get; set; }
    }

    /// <summary>
    /// 拉取库存返回对象
    /// </summary>
    public class InvInfoApiResponse
    {
        public int code { get; set; }
        public List<InvInfo> data { get; set; }
    }
}