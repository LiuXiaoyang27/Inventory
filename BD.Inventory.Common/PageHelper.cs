using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD.Inventory.Common
{
    public static class PageHelper
    {
        public static string CreatePageSql(int recordCount, int pageSize, int pageIndex, string safeSql, string orderField)
        {
            //计算总页数
            pageSize = pageSize == 0 ? recordCount : pageSize;
            int pageCount = 0;
            if (recordCount == 0)
            {
                pageCount = 0;
            }
            else
            {
                pageCount = (recordCount + pageSize - 1) / pageSize;
            }
            //int pageCount = (recordCount + pageSize - 1) / pageSize;
            pageIndex = Math.Max(1, Math.Min(pageIndex, pageCount)); //确保页数在有效范围内

            //拼接SQL字符串，加上ROW_NUMBER函数进行分页
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("SELECT * FROM (");
            sbSql.AppendFormat("SELECT ROW_NUMBER() OVER(ORDER BY {0}) as row_number,", orderField);
            sbSql.Append(safeSql.Substring(safeSql.ToUpper().IndexOf("SELECT") + 6));
            sbSql.Append(") AS T");
            sbSql.AppendFormat(" WHERE row_number BETWEEN {0} AND {1}", (pageIndex - 1) * pageSize + 1, pageIndex * pageSize);

            return sbSql.ToString();
        }

        public static string CreateCountingSql(string safeSql)
        {
            return $"SELECT COUNT(1) AS RecordCount FROM ({safeSql}) AS T1";
        }

        public static int GetPageCount(int pageSize, int totalCount)
        {
            return (int)Math.Ceiling((double)totalCount / pageSize);
        }

    }
}
