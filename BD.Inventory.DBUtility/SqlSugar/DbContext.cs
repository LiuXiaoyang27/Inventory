using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD.Inventory.DBUtility.SqlSugar
{
    class DbContext : IDisposable
    {
        /// <summary>
        /// SqlServer
        /// </summary>
        public SqlSugarClient db;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DbContext()
        {
            // SQL Server 数据库连接配置
            db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString,
                DbType = DbType.SqlServer, // 设置数据库类型
                IsAutoCloseConnection = true, // 自动关闭连接
            });
        }

        public void Dispose()
        {
            if (db != null)
            {
                db.Dispose();
            }
        }
    }
}
