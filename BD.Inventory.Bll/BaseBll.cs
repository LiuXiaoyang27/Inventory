using BD.Inventory.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD.Inventory.Bll
{
    public class BaseBll
    {
        private static readonly Lazy<BaseBll> _instance = new Lazy<BaseBll>(() => new BaseBll());

        public static BaseBll Instance { get; } = _instance.Value;

        // 获取单例实例
        private readonly BaseDal DalInstance;

        private BaseBll()
        {
            DalInstance = BaseDal.Instance;
        }

        /// <summary>
        /// 查询数据是否存在
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public bool IsExist(string tableName, string strWhere)
        {
            return DalInstance.IsExist(tableName, strWhere);
        }
    }
}
