using BD.Inventory.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD.Inventory.Bll
{
    public class SpecificationsBll
    {
        private static readonly Lazy<SpecificationsBll> _instance = new Lazy<SpecificationsBll>(() => new SpecificationsBll());

        public static SpecificationsBll Instance { get; } = _instance.Value;

        // 获取单例实例
        private readonly SpecificationsDal DalInstance;

        private SpecificationsBll()
        {
            DalInstance = SpecificationsDal.Instance;
        }

        /// <summary>
        /// 查询数据是否存在
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public bool IsExist(string strWhere)
        {
            return DalInstance.IsExist(strWhere);
        }
    }
}
