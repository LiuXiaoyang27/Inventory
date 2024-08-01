using BD.Inventory.DBUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD.Inventory.Dal
{
    public class SpecificationsDal
    {
        // 使用 Lazy<T> 实现单例模式，确保实例的惰性初始化和线程安全。
        private static readonly Lazy<SpecificationsDal> _instance = new Lazy<SpecificationsDal>(() => new SpecificationsDal());

        /// <summary>
        /// 获取类的单例实例。
        /// </summary>
        public static SpecificationsDal Instance { get; } = _instance.Value;

        // 私有构造函数，防止外部实例化该类。
        private SpecificationsDal()
        {
        }
        private const string tableName = "Specifications";


        /// <summary>
        /// 查询数据是否存在
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public bool IsExist(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + tableName);
            strSql.Append(" where " + strWhere);
            return SqlHelper.Exists(strSql.ToString());
        }
    }
}
