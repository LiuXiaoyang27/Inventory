using BD.Inventory.Common;
using BD.Inventory.DBUtility;
using BD.Inventory.Entities;
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
    public class InvCheckDal
    {
        // 使用 Lazy<T> 实现单例模式，确保实例的惰性初始化和线程安全。
        private static readonly Lazy<InvCheckDal> _instance = new Lazy<InvCheckDal>(() => new InvCheckDal());

        /// <summary>
        /// 获取类的单例实例。
        /// </summary>
        public static InvCheckDal Instance { get; } = _instance.Value;

        // 私有构造函数，防止外部实例化该类。
        private InvCheckDal()
        {
        }

        private const string table1 = "InvCheckBillHead";
        private const string table2 = "InvCheckBillBody";
        private const string table3 = "Storage";

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


        // ===========================以下为PC端方法====================================

        #region 生成盘点单号
        /// <summary>
        /// 生成盘点单号
        /// </summary>
        /// <returns></returns>
        public string InventoryNumberGenerator()
        {
            string prefix = "PD";
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            int counterValue = 0;
            DateTime currentDate = DateTime.Today;

            string connectionString = SqlHelper.connectionString;
            Action<SqlConnection, SqlTransaction> sqlAction = (connection, transaction) =>
            {
                // 通过单据编码查询表头仓库
                SqlCommand selectCommand = new SqlCommand(
                        "SELECT CurrentCounter, LastGeneratedDate FROM InventoryCounter WHERE Id = 1",
                        connection,
                        transaction);
                using (SqlDataReader reader = selectCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        counterValue = Convert.ToInt32(reader["CurrentCounter"]);
                        DateTime lastGeneratedTimestamp = (DateTime)reader["LastGeneratedDate"];

                        // 如果新生成的日期不同或者上次时间戳晚于当前时间
                        if (lastGeneratedTimestamp < currentDate)
                        {
                            counterValue = 1; // 仅在日期变化或时间戳异常的情况下重置计数器
                        }
                    }
                    else
                    {
                        throw new Exception("InventoryCounter record not found.");
                    }
                }
                SqlCommand updateCommand = new SqlCommand(
                        "UPDATE InventoryCounter SET CurrentCounter = @counter, LastGeneratedDate = @currentDate WHERE Id = 1",
                        connection,
                        transaction);

                updateCommand.Parameters.AddWithValue("@counter", counterValue + 1);
                updateCommand.Parameters.AddWithValue("@currentDate", currentDate);
                updateCommand.ExecuteNonQuery();

            };
            SqlHelper.ExecuteTransaction(sqlAction, connectionString);

            string counterPart = counterValue.ToString("D4");
            string inventoryNumber = $"{prefix}{datePart}{counterPart}";

            return inventoryNumber;

        }
        #endregion

        /// <summary>
        /// 查询盘点单头（分页）
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="strWhere"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataTable SelInvCheckHead(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            string query = @"select t1.id,t1.bill_code,t1.bill_creater,t1.bill_date,t1.create_time,t1.remark,t1.state,
                t1.storage_code,t1.storage_name from InvCheckBillHead t1 Inner Join (
                select DISTINCT bill_code from UHFInvCheck) t2 ON t1.bill_code=t2.bill_code where t1.is_delete_tag = 0 ";
            if (!string.IsNullOrEmpty(strWhere))
            {
                query += strWhere;
            }
            string countSql = PageHelper.CreateCountingSql(query);
            recordCount = Convert.ToInt32(SqlHelper.GetSingle(countSql));
            string pageSql = PageHelper.CreatePageSql(recordCount, pageSize, pageIndex, query, filedOrder);
            DataSet ds = SqlHelper.Query(pageSql);
            return ds.Tables[0];
        }

        /// <summary>
        /// 根据单号查询单头
        /// </summary>
        /// <param name="bill_code"></param>
        /// <returns></returns>
        public InvCheckDTO GetModelByBillCode(string bill_code)
        {
            string query = @"select t1.bill_code,t1.storage_code from InvCheckBillHead t1 Inner Join (
                select DISTINCT bill_code from UHFInvCheck) t2 ON t1.bill_code=t2.bill_code where t1.bill_code=@bill_code ";
            SqlParameter[] parameters =
           {
                new SqlParameter("@bill_code",bill_code)
            };
            DataSet ds = SqlHelper.Query(query, parameters);
            InvCheckDTO model = CommonOperation.DataTableToModel<InvCheckDTO>(ds.Tables[0]);
            return model;
        }

        /// <summary>
        /// 查询详情
        /// </summary>
        /// <param name="spec_code"></param>
        /// <returns></returns>
        public DataTable GetDetail(string bill_code, int pageSize, int pageIndex, string filedOrder, out int recordCount)
        {
            string query = @"SELECT 
                                b.goods_code,
                                b.goods_name,
                                b.spec_code,
                                b.spec_name,
                                b.batch_code,
                                b.batch_date,
                                b.expiry_date,
                                b.nums,
                                b.stock_type,
                                SUM(CASE WHEN b.has_check = 1 THEN 1 ELSE 0 END) AS isChecked,
                                SUM(CASE WHEN b.has_check = 0 THEN 1 ELSE 0 END) AS unChecked,
                                COUNT(*) AS total_Inventory
                            FROM 
                                InventoryDB.dbo.UHFInvCheck AS b
                            WHERE 
                                b.bill_code = @bill_code
                            GROUP BY 
                                b.goods_code, b.goods_name, b.spec_code, b.spec_name, b.batch_code, b.batch_date, b.expiry_date, b.nums, b.stock_type";

            SqlParameter[] parameters =
            {
                new SqlParameter("@bill_code",bill_code)
            };
            string countSql = PageHelper.CreateCountingSql(query);
            recordCount = Convert.ToInt32(SqlHelper.GetSingle(countSql, parameters));
            string pageSql = PageHelper.CreatePageSql(recordCount, pageSize, pageIndex, query, filedOrder);
            SqlParameter[] parameters1 =
            {
                new SqlParameter("@bill_code",bill_code)
            };
            DataSet ds = SqlHelper.Query(pageSql, parameters1);
            return ds.Tables[0];
        }

        /// <summary>
        /// 获取同步日期
        /// </summary>
        /// <returns></returns>
        public SyncDate GetSyncDate()
        {
            string query = @"select ID,Year,Month,Day,Hour,Minute,Second from SyncDate";
            var ds = SqlHelper.Query(query);
            SyncDate model = CommonOperation.DataTableToModel<SyncDate>(ds.Tables[0]);
            return model;
        }

        /// <summary>
        /// 设置同步时间
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public bool SetSyncTime(SyncDate model)
        {
            string nextId = Utils.GetNextID();
            string sql = @"insert into SyncDate(Year,Month,Day,Hour,Minute,Second,ID) 
                        values(@year,@month,@day,@hour,@minute,@second,@id)";
            SqlParameter[] parameters =
            {
                new SqlParameter("@year",model.Year),
                new SqlParameter("@month",model.Month),
                new SqlParameter("@day",model.Day),
                new SqlParameter("@hour",model.Hour),
                new SqlParameter("@minute",model.Minute),
                new SqlParameter("@second",model.Second),
                new SqlParameter("@id",nextId)
        };

            int res = SqlHelper.ExecuteSql(sql, parameters);

            return res > 0;

        }

        /// <summary>
        /// 修改同步时间
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public bool UpdateSyncTime(SyncDate model)
        {
            string sql = @"update SyncDate set Year=@year,Month=@month,Day=@day,Hour=@hour,
                            Minute=@minute,Second=@second where ID=@id";
            SqlParameter[] parameters =
           {
                new SqlParameter("@year",model.Year),
                new SqlParameter("@month",model.Month),
                new SqlParameter("@day",model.Day),
                new SqlParameter("@hour",model.Hour),
                new SqlParameter("@minute",model.Minute),
                new SqlParameter("@second",model.Second),
                new SqlParameter("@id",model.ID)
            };

            int res = SqlHelper.ExecuteSql(sql, parameters);

            return res > 0;
        }


        // ===========================以下为手持端方法===================================

        /// <summary>
        /// 查询单号
        /// </summary>
        /// <returns></returns>
        public DataTable GetBillCode(string strWhere)
        {
            var sb = new StringBuilder();
            sb.Append($"select id, bill_code from { table1} where is_delete_tag = 0 ");
            if (!string.IsNullOrEmpty(strWhere))
            {
                sb.Append(strWhere);
            }

            var ds = SqlHelper.Query(sb.ToString());
            return ds.Tables[0];
        }

        /// <summary>
        /// 查询仓库
        /// </summary>
        /// <returns></returns>
        public DataTable GetStorage(string strWhere)
        {
            var sb = new StringBuilder();
            sb.Append($"select storage_code,storage_name from { table3} where is_delete_tag = 0 ");
            if (!string.IsNullOrEmpty(strWhere))
            {
                sb.Append(strWhere);
            }

            var ds = SqlHelper.Query(sb.ToString());
            return ds.Tables[0];
        }


        /// <summary>
        /// 选择单号查询数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ChooseBillCodeDTO SelDataByBillCode(string bill_code, int pageIndex, int pageSize)
        {
            ChooseBillCodeDTO result = new ChooseBillCodeDTO();
            string connectionString = SqlHelper.connectionString;

            Action<SqlConnection, SqlTransaction> sqlAction = (connection, transaction) =>
            {
                // 通过单据编码查询表头仓库
                Storage storageModel = GetStorageByBillCode(bill_code, connection, transaction);
                result.storage_code = storageModel.storage_code;
                result.storage_name = storageModel.storage_name;
                // 通过单据编码查询所有待盘点数据
                List<UHFInvCheck> toCheckList = GetBillCheckBody(bill_code, connection, transaction);
                if (toCheckList != null && toCheckList.Count > 0)
                {
                    // 查询需要盘点总数
                    result.total_num = toCheckList.Count;  //  GetTotalNum(bodyList, connection, transaction);
                                                           // 将所有待盘点数据插入数据库
                    foreach (var item in toCheckList)
                    {
                        if (!UHFInvExists(item.bill_code, item.RFID, connection, transaction))
                        {
                            InsertUHFInv(item, storageModel.storage_code, connection, transaction);
                        }
                    }
                }

                // 查询已盘点数量
                result.has_checked_num = GetCheckedNum(bill_code, connection, transaction);

                result.currentPage = pageIndex;

                // 查询未盘点数据集
                //result.un_check_items = GetUnCheckListOfPage(bill_code, pageSize, pageIndex, connection, transaction);
                GetUnCheckListOfPage(bill_code, result, pageSize, connection, transaction);
            };

            SqlHelper.ExecuteTransaction(sqlAction, connectionString);

            return result;
        }

        /// <summary>
        /// 通过单据编码查询未盘点数据集(分页)
        /// </summary>
        /// <param name="bill_code">单据编码</param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private void GetUnCheckListOfPage(string bill_code, ChooseBillCodeDTO result, int pageSize, SqlConnection connection, SqlTransaction transaction)
        {
            List<UHFInvCheck> list = new List<UHFInvCheck>();
            string query = SelUHFInvCheck_Sql("has_check = 0 and bill_code=@bill_code");



            // 分页操作
            string countSql = PageHelper.CreateCountingSql(query);
            int recordCount = 0;
            using (SqlCommand command1 = new SqlCommand(countSql, connection, transaction))
            {
                command1.Parameters.AddWithValue("@bill_code", bill_code);
                recordCount = (int)command1.ExecuteScalar();
                result.totalCount = recordCount;
            }

            string pageSql = PageHelper.CreatePageSql(recordCount, pageSize, result.currentPage, query, "create_time");

            using (SqlCommand command = new SqlCommand(pageSql, connection, transaction))
            {
                command.Parameters.AddWithValue("@bill_code", bill_code);

                // 创建DataSet对象
                DataSet ds = new DataSet();

                // 执行查询并填充DataSet
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(ds);
                }
                result.totalPage = PageHelper.GetPageCount(pageSize, recordCount);
                list = CommonOperation.ConvertDataTableToModelList<UHFInvCheck>(ds.Tables[0]);

                result.un_check_items = list;
            }






        }


        /// <summary>
        /// 通过单据编码查询表头仓库编号
        /// </summary>
        /// <param name="bill_code">单据编码</param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private Storage GetStorageByBillCode(string bill_code, SqlConnection connection, SqlTransaction transaction)
        {
            // 查询表体数据
            string query = @"select  storage_code,storage_name from " + table1 + " where bill_code=@bill_code";
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@bill_code", bill_code);
                // 创建DataSet对象
                DataSet ds = new DataSet();

                // 执行查询并填充DataSet
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(ds, "InvCheckBillHead");
                }

                Storage model = CommonOperation.DataTableToModel<Storage>(ds.Tables[0]);

                // 检查DataSet中是否有数据
                //if (ds.Tables["InvCheckBillHead"].Rows.Count > 0)
                //{
                //    // 假设查询结果中有且仅有一条记录，取第一条记录的storage_code字段
                //    string storage_code = ds.Tables["InvCheckBillHead"].Rows[0]["storage_code"].ToString();
                //    return storage_code;
                //}
                //else
                //{
                //    // 如果没有找到记录，可以选择返回null或者抛出异常
                //    // return null;
                //    return "";
                //}
                return model;

            }

        }

        /// <summary>
        /// 通过单据编码查询所有待盘点数据
        /// </summary>
        /// <param name="bill_code">单据编码</param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private List<UHFInvCheck> GetBillCheckBody(string bill_code, SqlConnection connection, SqlTransaction transaction)
        {
            List<UHFInvCheck> list = new List<UHFInvCheck>();
            // 查询表体数据
            string query = @"select  t1.RFID,t1.barcode,t2.goods_code, t2.goods_name, t2.bill_code, t2.batch_code, t2.batch_date, t2.expiry_date,
                t2.nums, t2.quantity, t2.quantity_start, t2.spec_code, t2.spec_name,
                t2.stock_type from BindRFID t1 Left Join " + table2 + " t2 ON t1.spec_code=t2.spec_code  where t2.bill_code=@bill_code";
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@bill_code", bill_code);
                // 创建DataSet对象
                DataSet ds = new DataSet();

                // 执行查询并填充DataSet
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(ds, "InvCheckBillBody");
                }

                list = CommonOperation.ConvertDataTableToModelList<UHFInvCheck>(ds.Tables[0]);

                return list;
            }

        }

        /// <summary>
        /// 查询待盘点数据是否存在
        /// </summary>
        /// <param name="sys_goods_uid"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private bool UHFInvExists(string bill_code, string RFID, SqlConnection connection, SqlTransaction transaction)
        {
            string query = "SELECT COUNT(1) FROM UHFInvCheck WHERE bill_code = @bill_code AND RFID = @RFID";
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@bill_code", bill_code);
                command.Parameters.AddWithValue("@RFID", RFID);
                return (int)command.ExecuteScalar() > 0;
            }
        }

        /// <summary>
        /// 插入待盘点数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="storage_code"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        private void InsertUHFInv(UHFInvCheck model, string storage_code, SqlConnection connection, SqlTransaction transaction)
        {
            string nextId = Utils.GetNextID();
            string query = @"
        INSERT INTO UHFInvCheck (id, goods_code, goods_name, bill_code, batch_code,batch_date, expiry_date, nums, quantity, quantity_start, 
        spec_code, spec_name, stock_type, storage_code, has_check, create_time, RFID, barcode) 
        VALUES (@id, @goods_code, @goods_name, @bill_code, @batch_code, @batch_date, @expiry_date, @nums, @quantity, @quantity_start, 
        @spec_code, @spec_name, @stock_type, @storage_code, -1, GETDATE(), @RFID, @barcode)";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@id", nextId);
                command.Parameters.AddWithValue("@goods_code", model.goods_code);
                command.Parameters.AddWithValue("@goods_name", model.goods_name);
                command.Parameters.AddWithValue("@bill_code", model.bill_code);
                command.Parameters.AddWithValue("@batch_code", model.batch_code);
                command.Parameters.AddWithValue("@batch_date", model.batch_date.HasValue ? (object)model.batch_date.Value : DBNull.Value);
                command.Parameters.AddWithValue("@expiry_date", model.expiry_date.HasValue ? (object)model.expiry_date.Value : DBNull.Value);
                command.Parameters.AddWithValue("@nums", model.nums.HasValue ? (object)model.nums.Value : DBNull.Value);
                command.Parameters.AddWithValue("@quantity", model.quantity.HasValue ? (object)model.quantity.Value : DBNull.Value);
                command.Parameters.AddWithValue("@quantity_start", model.quantity_start.HasValue ? (object)model.quantity_start.Value : DBNull.Value);
                command.Parameters.AddWithValue("@spec_code", model.spec_code);
                command.Parameters.AddWithValue("@spec_name", model.spec_name);
                command.Parameters.AddWithValue("@stock_type", model.stock_type.HasValue ? (object)model.stock_type.Value : DBNull.Value);
                command.Parameters.AddWithValue("@storage_code", storage_code);
                command.Parameters.AddWithValue("@RFID", model.RFID);
                command.Parameters.AddWithValue("@barcode", model.barcode);

                command.ExecuteNonQuery();
            }
        }

        ///// <summary>
        ///// 通过单据编码查询需要盘点的总数量
        ///// </summary>
        ///// <param name="sys_goods_uid"></param>
        ///// <param name="connection"></param>
        ///// <param name="transaction"></param>
        ///// <returns></returns>
        //private int GetTotalNum(List<InvCheckBillBody> bodyList, SqlConnection connection, SqlTransaction transaction)
        //{
        //    int totalCount = 0;
        //    if (bodyList != null && bodyList.Count > 0)
        //    {
        //        foreach (var body in bodyList)
        //        {
        //            // 查询每个规格商品的数量
        //            string selcount_sql = @"select COUNT(1) from BindRFID where goods_code=@goods_code and spec_code=@spec_code";
        //            using (SqlCommand command = new SqlCommand(selcount_sql, connection, transaction))
        //            {
        //                command.Parameters.AddWithValue("@goods_code", body.goods_code);
        //                command.Parameters.AddWithValue("@spec_code", body.spec_code);
        //                int count = (int)command.ExecuteScalar();
        //                totalCount += count;
        //            }

        //        }
        //    }

        //    return totalCount;

        //}

        /// <summary>
        /// 通过单据编码查询已盘点数量
        /// </summary>
        /// <param name="bill_code"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private int GetCheckedNum(string bill_code, SqlConnection connection, SqlTransaction transaction)
        {
            int checkedCount = 0;

            // 查询每个规格商品的数量
            string selcount_sql = @"select COUNT(1) from UHFInvCheck where has_check = 1 and bill_code=@bill_code";
            using (SqlCommand command = new SqlCommand(selcount_sql, connection, transaction))
            {
                command.Parameters.AddWithValue("@bill_code", bill_code);
                checkedCount = (int)command.ExecuteScalar();
            }

            return checkedCount;

        }

        private string SelUHFInvCheck_Sql(string strWhere)
        {
            string query = @"select id, goods_code, goods_name, bill_code, batch_code, expiry_date,
                nums, quantity, quantity_start, spec_code, spec_name,
                stock_type, storage_code, has_check, create_time,RFID,barcode from UHFInvCheck ";
            if (!string.IsNullOrEmpty(strWhere))
            {
                query += "where " + strWhere;
            }
            return query;
        }

        /// <summary>
        /// 通过单据编码查询未盘点数据集
        /// </summary>
        /// <param name="bill_code">单据编码</param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private List<UHFInvCheck> GetUnCheckList(string bill_code, SqlConnection connection, SqlTransaction transaction)
        {
            List<UHFInvCheck> list = new List<UHFInvCheck>();
            // 查询表体数据
            //string query = @"select id, goods_code, goods_name, bill_code, batch_code, expiry_date,
            //    nums, quantity, quantity_start, spec_code, spec_name,
            //    stock_type, storage_code, has_check, create_time from UHFInvCheck where has_check = 0 and bill_code=@bill_code";
            string query = SelUHFInvCheck_Sql("has_check = 0 and bill_code=@bill_code");
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@bill_code", bill_code);
                // 创建DataSet对象
                DataSet ds = new DataSet();

                // 执行查询并填充DataSet
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(ds);
                }

                list = CommonOperation.ConvertDataTableToModelList<UHFInvCheck>(ds.Tables[0]);

                return list;
            }

        }

        /// <summary>
        /// 通过单据编码查询所有待盘点数据集
        /// </summary>
        /// <param name="bill_code">单据编码</param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private List<UHFInvCheck> GetAllCheckList(string bill_code, SqlConnection connection, SqlTransaction transaction)
        {
            List<UHFInvCheck> list = new List<UHFInvCheck>();
            string query = SelUHFInvCheck_Sql("has_check <> 1 and bill_code=@bill_code");
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@bill_code", bill_code);
                // 创建DataSet对象
                DataSet ds = new DataSet();

                // 执行查询并填充DataSet
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(ds);
                }

                list = CommonOperation.ConvertDataTableToModelList<UHFInvCheck>(ds.Tables[0]);

                return list;
            }

        }


        /// <summary>
        /// 盘点提交
        /// </summary>
        /// <param name="scannedRFIDsSet">扫描的RFID</param>
        /// <param name="bill_code"></param>
        /// <param name="isRepeat"></param>
        /// <returns></returns>
        public bool InvSubmit(HashSet<string> scannedRFIDsSet, string bill_code, int isRepeat)
        {
            string connectionString = SqlHelper.connectionString;

            Action<SqlConnection, SqlTransaction> sqlAction = (connection, transaction) =>
            {
                // 根据isRepeat查询所有RFID
                List<UHFInvCheck> allList = new List<UHFInvCheck>();
                if (isRepeat == 0)
                {
                    // 查询所有数据
                    allList = GetAllCheckList(bill_code, connection, transaction);
                }
                if (isRepeat == 1)
                {
                    // 只查询未盘点数据
                    allList = GetUnCheckList(bill_code, connection, transaction);
                }
                if (allList != null && allList.Count > 0)
                {
                    HashSet<string> allRFIDsSet = new HashSet<string>();
                    foreach (var item in allList)
                    {
                        allRFIDsSet.Add(item.RFID);
                    }
                    // 将扫描的RFIDs存储在集合中
                    //HashSet<string> scannedRFIDsSet = new HashSet<string>(RFIDList);

                    // 找出未扫描的RFIDs
                    var missingRFIDs = allRFIDsSet.Except(scannedRFIDsSet);

                    // 修改已扫描的数据，将has_check改为1
                    UpdateHasCheck(scannedRFIDsSet, bill_code, connection, transaction);

                    // 修改未扫描的数据，将has_check改为0
                    UpdateUnCheck(missingRFIDs, bill_code, connection, transaction);

                }
            };

            SqlHelper.ExecuteTransaction(sqlAction, connectionString);

            return true;
        }

        /// <summary>
        /// 修改已扫描的数据，将has_check改为1
        /// </summary>
        /// <param name="scannedRFIDsSet"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        private void UpdateHasCheck(HashSet<string> scannedRFIDsSet, string bill_code, SqlConnection connection, SqlTransaction transaction)
        {
            foreach (var item in scannedRFIDsSet)
            {
                string sql = @"update UHFInvCheck set has_check = 1 where bill_code=@bill_code and RFID=@RFID";
                using (SqlCommand command = new SqlCommand(sql, connection, transaction))
                {
                    command.Parameters.AddWithValue("@bill_code", bill_code);
                    command.Parameters.AddWithValue("@RFID", item);
                    command.ExecuteNonQuery();
                }
            }

        }

        /// <summary>
        /// 修改未扫描的数据，将has_check改为0
        /// </summary>
        /// <param name="scannedRFIDsSet"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        private void UpdateUnCheck(IEnumerable<string> missingRFIDs, string bill_code, SqlConnection connection, SqlTransaction transaction)
        {
            foreach (var item in missingRFIDs)
            {
                string sql = @"update UHFInvCheck set has_check = 0 where bill_code=@bill_code and RFID=@RFID";
                using (SqlCommand command = new SqlCommand(sql, connection, transaction))
                {
                    command.Parameters.AddWithValue("@bill_code", bill_code);
                    command.Parameters.AddWithValue("@RFID", item);
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
