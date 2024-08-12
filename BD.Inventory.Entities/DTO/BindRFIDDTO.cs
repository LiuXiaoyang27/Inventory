using System.Collections.Generic;

namespace BD.Inventory.Entities.DTO
{
    public class BindRFIDDTO
    {

        /// <summary>
        /// Desc:商品编码
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string goods_code { get; set; }

        /// <summary>
        /// Desc:规格编码
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string spec_code { get; set; }

        /// <summary>
        /// 条码
        /// </summary>
        public string barcode { get; set; }

        /// <summary>
        /// Desc:RFID
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string RFID { get; set; }

        /// <summary>
        /// RFID集合
        /// </summary>
        public List<string> RFIDs { get; set; }


    }
}
