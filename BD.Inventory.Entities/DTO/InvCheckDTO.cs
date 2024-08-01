using System;
using System.Collections.Generic;

namespace BD.Inventory.Entities.DTO
{
    /// <summary>
    /// 盘点单详情DTO
    /// </summary>
    public class InvCheckDTO
    {
        public string bill_code { get; set; }

        public string storage_code { get; set; }

        public List<InvCheckDetailDTO> details { get; set; }
    }

    public class InvCheckDetailDTO
    {
        public string goods_code { get; set; }

        public string goods_name { get; set; }

        public string spec_code { get; set; }

        public string spec_name { get; set; }

        public string batch_code { get; set; }

        public DateTime batch_date { get; set; }

        public DateTime expiry_date { get; set; }

        public double nums { get; set; }

        public int stock_type { get; set; }

        public int isChecked { get; set; }

        public int unChecked { get; set; }

        public int total_Inventory { get; set; }
    }

}
