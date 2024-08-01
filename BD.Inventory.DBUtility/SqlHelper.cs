using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace BD.Inventory.DBUtility
{
    public static class SqlHelper
    {
        // 数据库连接字符串
        public static string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        // 执行查询，返回 DataSet
        public static DataSet Query(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, connection);
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                try
                {
                    adapter.Fill(ds);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return ds;
            }
        }

        // 异步查询方法，返回 DataSet
        public static async Task<DataSet> QueryAsync(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, connection);
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                try
                {
                    await connection.OpenAsync();
                    adapter.Fill(ds);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return ds;
            }
        }

        // 执行查询，返回首行首列
        public static object GetSingle(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, connection);
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                try
                {
                    connection.Open();
                    object obj = cmd.ExecuteScalar();
                    return obj;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        // 执行查询，返回首行首列
        public static async Task<object> GetSingleAsync(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, connection);
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                try
                {
                    await connection.OpenAsync();
                    object obj = await cmd.ExecuteScalarAsync();
                    return obj;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 执行一条插入语句，返回插入后的id（object）。
        /// </summary>
        /// <param name="strSql">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object ExecuteReturnIdentity(string strSql, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(strSql + ";SELECT SCOPE_IDENTITY();", connection)) // 添加 SELECT SCOPE_IDENTITY() 语句
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, strSql, cmdParms);
                        connection.Open(); // 打开连接
                        object obj = cmd.ExecuteScalar();
                        if ((Equals(obj, null)) || (Equals(obj, DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// 执行一条插入语句，返回插入后的id（object）异步
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static async Task<object> ExecuteReturnIdentityAsync(string strSql, params SqlParameter[] cmdParms)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(strSql, connection)) // 添加 SELECT SCOPE_IDENTITY() 语句
                {
                    try
                    {
                        await connection.OpenAsync(); // 异步打开连接
                        PrepareCommand(cmd, connection, null, strSql, cmdParms);
                        object obj = await cmd.ExecuteScalarAsync(); // 异步执行查询
                        return obj;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        // 执行命令，返回受影响的行数
        public static int ExecuteSql(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, sql, parameters);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                //SqlCommand cmd = new SqlCommand(sql, connection);
                //if (parameters != null)
                //{
                //    cmd.Parameters.AddRange(parameters);
                //}
                //try
                //{
                //    connection.Open();
                //    int rows = cmd.ExecuteNonQuery();
                //    return rows;
                //}
                //catch (SqlException ex)
                //{
                //    throw new Exception(ex.Message);
                //}
            }
        }

        // 执行命令，返回受影响的行数
        public static async Task<int> ExecuteSqlAsync(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sql, connection);
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }
                try
                {
                    await connection.OpenAsync();
                    int rows = await cmd.ExecuteNonQueryAsync();
                    return rows;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        // 判断记录是否存在
        public static bool Exists(string sql, params SqlParameter[] parameters)
        {
            object obj = GetSingle(sql, parameters);
            int cmdResult;
            if (Equals(obj, null) || Equals(obj, DBNull.Value))
            {
                cmdResult = 0;
            }
            else
            {
                cmdResult = int.Parse(obj.ToString());
            }
            return cmdResult != 0;
        }

        // 判断记录是否存在
        public static async Task<bool> ExistsAsync(string sql, params SqlParameter[] parameters)
        {
            object obj = await GetSingleAsync(sql, parameters);
            int cmdResult;
            if (Equals(obj, null) || Equals(obj, DBNull.Value))
            {
                cmdResult = 0;
            }
            else
            {
                cmdResult = int.Parse(obj.ToString());
            }
            return cmdResult != 0;
        }

        // 执行事务
        public static void ExecuteTransaction(Action<SqlConnection, SqlTransaction> sqlAction, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    sqlAction(connection, transaction);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 异步
        /// </summary>
        /// <param name="sqlActionAsync"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static async Task ExecuteTransactionAsync(Func<SqlConnection, SqlTransaction, Task> sqlActionAsync, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync(); // 使用异步打开连接  
                SqlTransaction transaction = connection.BeginTransaction(); // 使用异步开始事务  
                try
                {
                    await sqlActionAsync(connection, transaction); // 调用异步的sql操作  
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        // 执行事务，返回受影响的行数
        public static int ExecuteSqlTran(List<CommandInfo> cmdList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.Transaction = trans;
                    try
                    {
                        int count = 0;
                        foreach (CommandInfo myDE in cmdList)
                        {
                            string cmdText = myDE.CommandText;
                            SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            count += val;
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                        return count;
                    }
                    catch
                    {
                        trans.Rollback();
                        return 0;
                    }
                }
            }
        }

        /// <summary>
        /// 异步执行事务，返回受影响的行数
        /// </summary>
        /// <param name="cmdList"></param>
        /// <returns></returns>
        public static async Task<int> ExecuteSqlTranAsync(List<CommandInfo> cmdList)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.Transaction = trans;
                    try
                    {
                        int count = 0;
                        foreach (CommandInfo myDE in cmdList)
                        {
                            string cmdText = myDE.CommandText;
                            SqlParameter[] cmdParms = (SqlParameter[])myDE.Parameters;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = await cmd.ExecuteNonQueryAsync();
                            count += val;
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                        return count;
                    }
                    catch
                    {
                        trans.Rollback();
                        return 0;
                    }
                }
            }
        }

        // 准备 SqlCommand 对象
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
    }

}
