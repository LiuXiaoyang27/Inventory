using System.Collections.Generic;

namespace BD.Inventory.Entities.DTO
{
    public class ChooseBillCodeDTO
    {
        /// <summary>
        /// 库存数量（需盘点数量）
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

    }
}
