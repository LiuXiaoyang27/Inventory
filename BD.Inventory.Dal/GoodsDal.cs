using BD.Inventory.Common;
using BD.Inventory.DBUtility;
using BD.Inventory.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD.Inventory.Dal
{
    public class GoodsDal
    {
        // 使用 Lazy<T> 实现单例模式，确保实例的惰性初始化和线程安全。
        private static readonly Lazy<GoodsDal> _instance = new Lazy<GoodsDal>(() => new GoodsDal());

        /// <summary>
        /// 获取类的单例实例。
        /// </summary>
        public static GoodsDal Instance { get; } = _instance.Value;

        // 私有构造函数，防止外部实例化该类。
        private GoodsDal()
        {
        }
        private const string table1 = "Goods";
        private const string table2 = "Specifications";
        private const string table3 = "BindRFID";


        /// <summary>
        /// 查询数据是否存在
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public bool IsExist(string tableName, string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + tableName);
            strSql.Append(" where " + strWhere);
            return SqlHelper.Exists(strSql.ToString());
        }

        /// <summary>
        /// 查询sql
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <returns></returns>
        private StringBuilder GetSelectSql(string strWhere = "")
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append("select t1.sys_goods_uid,t1.goods_code,t1.goods_name,t2.sys_spec_uid,t2.spec_code,t2.spec1,t2.spec2,t2.barcode,");
            strSql.Append("t2.pic,t2.barcodes ");
            strSql.Append($"From {table1} t1 left join {table2} t2 ON t1.goods_code=t2.goods_code");
            strSql.Append(" where t1.is_delete_tag = 0 ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(strWhere);
            }

            return strSql;
        }

        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public DataTable GetPageList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder sb = GetSelectSql(strWhere);
            string countSql = PageHelper.CreateCountingSql(sb.ToString());
            recordCount = Convert.ToInt32(SqlHelper.GetSingle(countSql));
            string pageSql = PageHelper.CreatePageSql(recordCount, pageSize, pageIndex, sb.ToString(), filedOrder);
            DataSet ds = SqlHelper.Query(pageSql);
            return ds.Tables[0];
        }

        /// <summary>
        /// 查询中间表
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public DataTable GetDetail(string barcode, int pageSize, int pageIndex, string filedOrder, out int recordCount)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"select id,goods_code,spec_code,RFID,modify_time,barcode ");
            sb.Append($"from {table3} where barcode='{barcode}'");

            string countSql = PageHelper.CreateCountingSql(sb.ToString());
            recordCount = Convert.ToInt32(SqlHelper.GetSingle(countSql));
            string pageSql = PageHelper.CreatePageSql(recordCount, pageSize, pageIndex, sb.ToString(), filedOrder);
            DataSet ds = SqlHelper.Query(pageSql);
            return ds.Tables[0];
        }

        /// <summary>
        /// 条码绑定
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int BindingCode(BindRFIDDTO model)
        {

            string connectionString = SqlHelper.connectionString;
            int result = 0;

            Action<SqlConnection, SqlTransaction> sqlAction = (connection, transaction) =>
            {
                //if (RFIDExists(model.goods_code, model.spec_code, connection, transaction))
                //{
                //    result = UpdateRFID(model, connection, transaction);
                //}
                //else
                //{
                //    result = InsertRFID(model, connection, transaction);
                //}
                result = InsertRFID(model, connection, transaction);
            };

            SqlHelper.ExecuteTransaction(sqlAction, connectionString);

            return result;
        }

        /// <summary>
        /// 条码绑定(批量绑定)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int BindingCodeBatch(BindRFIDDTO model)
        {

            string connectionString = SqlHelper.connectionString;
            int result = 0;

            Action<SqlConnection, SqlTransaction> sqlAction = (connection, transaction) =>
            {
                foreach (var RFID in model.RFIDs)
                {
                    
                    if (!RFIDExists(RFID, connection, transaction))
                    {
                        model.RFID = RFID;
                        result += InsertRFID(model, connection, transaction);
                    }

                }

            };

            SqlHelper.ExecuteTransaction(sqlAction, connectionString);

            return result;
        }

        /// <summary>
        /// 查询数据是否存在
        /// </summary>
        /// <param name="sys_goods_uid"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private bool RFIDExists(string RFID, SqlConnection connection, SqlTransaction transaction)
        {
            string query = $"SELECT COUNT(1) FROM {table3} WHERE RFID = @RFID";
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@RFID", RFID);
                return (int)command.ExecuteScalar() > 0;
            }
        }

        /// <summary>
        /// 插入RFID数据
        /// </summary>
        /// <param name="good"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        private int InsertRFID(BindRFIDDTO model, SqlConnection connection, SqlTransaction transaction)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Insert into {table3} (");
            sb.Append("id,goods_code,spec_code,RFID,modify_time,barcode)");
            sb.Append(" VALUES(@id,@goods_code,@spec_code,@rfid,GETDATE(),@barcode)");
            string nextID = Utils.GetNextID();
            int res = 0;
            using (SqlCommand command = new SqlCommand(sb.ToString(), connection, transaction))
            {
                command.Parameters.AddWithValue("@id", nextID);
                command.Parameters.AddWithValue("@goods_code", model.goods_code);
                command.Parameters.AddWithValue("@spec_code", model.spec_code);
                command.Parameters.AddWithValue("@rfid", model.RFID);
                command.Parameters.AddWithValue("@barcode", model.barcode);

                res = command.ExecuteNonQuery();
            }
            return res;
        }

        ///// <summary>
        ///// 修改RFID数据
        ///// </summary>
        ///// <param name="good"></param>
        ///// <param name="connection"></param>
        ///// <param name="transaction"></param>
        //private int UpdateRFID(BindRFIDDTO model, SqlConnection connection, SqlTransaction transaction)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append($"Update {table3} SET ");
        //    sb.Append("RFID=@rfid,modify_time=GETDATE()");
        //    sb.Append(" Where goods_code=@goods_code and spec_code=@spec_code");
        //    int res = 0;
        //    using (SqlCommand command = new SqlCommand(sb.ToString(), connection, transaction))
        //    {
        //        command.Parameters.AddWithValue("@rfid", model.RFID);
        //        command.Parameters.AddWithValue("@goods_code", model.goods_code);
        //        command.Parameters.AddWithValue("@spec_code", model.spec_code);

        //        res = command.ExecuteNonQuery();
        //    }
        //    return res;
        //}

        /// <summary>
        /// 删除详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DelDetail(string id)
        {
            string sql = "delete from " + table3 + " where id=@id";
            SqlParameter[] parameters =
            {
                new SqlParameter("@id",id)
            };

            int result = SqlHelper.ExecuteSql(sql, parameters);

            return result > 0;
        }

    }
}
