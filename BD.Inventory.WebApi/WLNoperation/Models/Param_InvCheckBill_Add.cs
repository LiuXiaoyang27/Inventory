using System.Collections.Generic;

namespace BD.Inventory.WebApi.WLNoperation.Models
{
    /// <summary>
    /// 添加增量盘点单参数
    /// </summary>
    public class Param_InvCheckBill_Add
    {
        /// <summary>
        /// 外部单号，用户校验幂等，如单号已存在，直接返回成功，此字段不会显示在页面上
        /// </summary>
        public string bill_code { get; set; } = "";

        /// <summary>
        /// 盘点单明细,实际变化的内容
        /// </summary>
        public List<Param_InvCheckBill_Add_Details> details { get; set; } = new List<Param_InvCheckBill_Add_Details>();

        /// <summary>
        /// 仓库编号
        /// </summary>
        public string storage_code { get; set; } = "";
    }

    /// <summary>
    /// 盘点单明细,实际变化的内容
    /// </summary>
    public class Param_InvCheckBill_Add_Details
    {
        /// <summary>
        /// 批次日期，仅作页面显示用，万里牛只认批次号
        /// </summary>
        public string batch_date { get; set; } = "";

        /// <summary>
        /// 批次号，万里牛中的对应商品必须开启批次保质期管理，否则不会处理此字段
        /// </summary>
        public string batch_no { get; set; } = "";

        /// <summary>
        /// 过期日期，仅作页面显示用，万里牛只认批次号
        /// </summary>
        public string expired_date { get; set; } = "";

        /// <summary>
        /// 数量,盘盈传正数，盘亏传负数
        /// </summary>
        public int nums { get; set; } = 0;

        /// <summary>
        /// 规格编码
        /// </summary>
        public string spec_code { get; set; } = "";

        /// <summary>
        /// 1 为次品 0 正品 不填默认为正品 当使用新接口保存逻辑时有效 use_new:true
        /// </summary>
        public int stock_type { get; set; } = 0;
    }

    ///// <summary>
    ///// 业务参数
    ///// </summary>
    //public class open_inventory_check_bill
    //{
    //    /// <summary>
    //    /// 盘点请求详情
    //    /// </summary>
    //    public Param_InvCheckBill_Add open_inventory_check_bill_request { get; set; } = new Param_InvCheckBill_Add();
    //}

}