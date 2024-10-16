using System.Collections.Generic;

namespace BD.Inventory.Entities.DTO
{
    public class ChooseBillCodeDTO
    {

        /// <summary>
        /// 万里牛中库存数
        /// </summary>
        public int wln_inv_num { get; set; }

        /// <summary>
        /// 需盘点数量，绑定了RFID的数量
        /// </summary>
        public int total_num { get; set; }

        /// <summary>
        /// 本次盘点数量
        /// </summary>
        public int has_checked_num { get; set; }

        /// <summary>
        ///  仓库编码
        /// </summary>
        public string storage_code { get; set; }

        /// <summary>
        ///  仓库名称
        /// </summary>
        public string storage_name { get; set; }

        /// <summary>
        /// 未盘点数据明细
        /// </summary>
        public List<UHFInvCheck> un_check_items { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int currentPage { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int totalPage { get; set; }

        /// <summary>
        /// 总数居条数
        /// </summary>
        public int totalCount { get; set; }

        /// <summary>
        /// 未绑定条码集合
        /// </summary>
        public List<string> unBindList { get; set; }

    }
}
