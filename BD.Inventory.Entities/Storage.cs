using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD.Inventory.Entities
{
    public partial class Storage
    {
        public Storage()
        {


        }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string id { get; set; }

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
    }
}
