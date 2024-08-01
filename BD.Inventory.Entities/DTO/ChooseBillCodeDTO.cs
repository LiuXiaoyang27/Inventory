using System.Collections.Generic;

namespace BD.Inventory.Entities.DTO
{
    public class ChooseBillCodeDTO
    {
        public int total_num { get; set; }

        public int has_checked_num { get; set; }

        public string storage_code { get; set; }

        public string storage_name { get; set; }

        public List<UHFInvCheck> un_check_items { get; set; }

        public int currentPage { get; set; }

        public int totalPage { get; set; }

        public int totalCount { get; set; }

    }
}
