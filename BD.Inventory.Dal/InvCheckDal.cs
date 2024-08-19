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

        #region 创建盘点单

        /// <summary>
        /// 创建盘点单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool CreateCheckBill(InvCheckBillHead model)
        {
            // 主表id
            string h_next_id = Utils.GetNextID();
            // 定义主表SQL语句和参数
            StringBuilder hSql = new StringBuilder();
            hSql.Append("insert into " + table1 + " (");
            hSql.Append("id,bill_code,bill_creater,bill_date,create_time,remark,state,storage_code,");
            hSql.Append("storage_name)");
            hSql.Append(" OUTPUT inserted.ID values (");
            hSql.Append("@id,@bill_code,@bill_creater,@bill_date,@create_time,@remark,@state,@storage_code,");
            hSql.Append("@storage_name)");

            string connectionString = SqlHelper.connectionString;
            Action<SqlConnection, SqlTransaction> sqlAction = (connection, transaction) =>
            {
                using (SqlCommand command1 = new SqlCommand(hSql.ToString(), connection, transaction))
                {
                    command1.CommandType = CommandType.Text;
                    command1.Parameters.AddWithValue("@id", h_next_id);
                    command1.Parameters.AddWithValue("@bill_code", model.bill_code);
                    command1.Parameters.AddWithValue("@bill_creater", model.bill_creater);
                    command1.Parameters.AddWithValue("@bill_date", model.bill_date.HasValue ? (object)model.bill_date.Value : DBNull.Value);
                    command1.Parameters.AddWithValue("@create_time", model.create_time.HasValue ? (object)model.create_time.Value : DBNull.Value);
                    command1.Parameters.AddWithValue("@remark", model.remark);
                    command1.Parameters.AddWithValue("@state", model.state.HasValue ? (object)model.state.Value : DBNull.Value);
                    command1.Parameters.AddWithValue("@storage_code", model.storage_code);
                    command1.Parameters.AddWithValue("@storage_name", model.storage_name);

                    command1.Connection = connection; // 确保连接已设置

                    // 执行命令
                    int result = command1.ExecuteNonQuery();
                    if (result > 0)
                    {
                        model.id = h_next_id;
                    }

                    // 子表数据
                    List<InvCheckBillBody> details = model.details;
                    if (details != null && details.Count > 0)
                    {
                        SqlCommand command2;

                        StringBuilder itemSql;
                        foreach (InvCheckBillBody detail in details)
                        {
                            detail.id = Utils.GetNextID();
                            itemSql = new StringBuilder();
                            itemSql.Append("insert into " + table2 + "(");
                            itemSql.Append("id,goods_code,goods_name,bill_code,bar_code,batch_code,batch_date,change_size,expiry_date,[index],inventory_type,nums,price,");
                            itemSql.Append("product_batch_code,quantity,quantity_start,remark,spec_code,spec_name,stock_type,total_money,unit)");
                            itemSql.Append(" values (");
                            itemSql.Append("@id,@goods_code,@goods_name,@bill_code,@bar_code,@batch_code,@batch_date,@change_size,@expiry_date,@index,@inventory_type,@nums,@price,");
                            itemSql.Append("@product_batch_code,@quantity,@quantity_start,@remark,@spec_code,@spec_name,@stock_type,@total_money,@unit)");

                            using (command2 = new SqlCommand(itemSql.ToString(), connection, transaction))
                            {
                                command2.CommandType = CommandType.Text;
                                command2.Parameters.AddWithValue("@id", detail.id);
                                command2.Parameters.AddWithValue("@goods_code", detail.goods_code);
                                command2.Parameters.AddWithValue("@goods_name", detail.goods_name);
                                command2.Parameters.AddWithValue("@bill_code", detail.bill_code);
                                command2.Parameters.AddWithValue("@bar_code", detail.bar_code);
                                command2.Parameters.AddWithValue("@batch_code", detail.batch_code);
                                command2.Parameters.AddWithValue("@batch_date", detail.batch_date.HasValue ? (object)detail.batch_date.Value : DBNull.Value);
                                command2.Parameters.AddWithValue("@change_size", detail.change_size.HasValue ? (object)detail.change_size.Value : DBNull.Value);
                                command2.Parameters.AddWithValue("@expiry_date", detail.expiry_date.HasValue ? (object)detail.expiry_date.Value : DBNull.Value);
                                command2.Parameters.AddWithValue("@index", detail.index.HasValue ? (object)detail.index.Value : DBNull.Value);
                                command2.Parameters.AddWithValue("@inventory_type", detail.inventory_type);
                                command2.Parameters.AddWithValue("@nums", detail.nums.HasValue ? (object)detail.nums.Value : DBNull.Value);
                                command2.Parameters.AddWithValue("@price", detail.price.HasValue ? (object)detail.price.Value : DBNull.Value);
                                command2.Parameters.AddWithValue("@product_batch_code", detail.product_batch_code);
                                command2.Parameters.AddWithValue("@quantity", detail.quantity.HasValue ? (object)detail.quantity.Value : DBNull.Value);
                                command2.Parameters.AddWithValue("@quantity_start", detail.quantity_start.HasValue ? (object)detail.quantity_start.Value : DBNull.Value);
                                command2.Parameters.AddWithValue("@remark", detail.remark);
                                command2.Parameters.AddWithValue("@spec_code", detail.spec_code);
                                command2.Parameters.AddWithValue("@spec_name", detail.spec_name);
                                command2.Parameters.AddWithValue("@stock_type", detail.stock_type.HasValue ? (object)detail.stock_type.Value : DBNull.Value);
                                command2.Parameters.AddWithValue("@total_money", detail.total_money.HasValue ? (object)detail.total_money.Value : DBNull.Value);
                                command2.Parameters.AddWithValue("@unit", detail.unit);

                                command2.ExecuteNonQuery();
                            }
                        }
                    }
                }
            };

            // 调用DatabaseHelper中的静态方法ExecuteTransaction
            SqlHelper.ExecuteTransaction(sqlAction, connectionString);

            return true;
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
            string query = @"select id,bill_code,bill_creater,bill_date,create_time,remark,state,
                storage_code,storage_name from InvCheckBillHead where is_delete_tag = 0 ";
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
        /// 查询详情（分页）
        /// </summary>
        /// <param name="spec_code"></param>
        /// <returns></returns>
        public DataTable GetDetail(string strWhere, int pageSize, int pageIndex, string filedOrder, out int recordCount)
        {
            string query = @"SELECT 
                                b.id,
                                b.goods_code,
                                b.goods_name,
                                b.bill_code,
                                b.spec_code,
                                b.spec_name,
                                b.change_size,
                                b.price,
                                b.quantity,
                                b.quantity_start,
                                b.stock_type,
                                b.remark,
                                b.total_money,
                                b.unit,
                                b.bar_code
                            FROM 
                                InventoryDB.dbo.InvCheckBillBody AS b
                            WHERE ";

            query += strWhere;
            string countSql = PageHelper.CreateCountingSql(query);
            recordCount = Convert.ToInt32(SqlHelper.GetSingle(countSql));
            string pageSql = PageHelper.CreatePageSql(recordCount, pageSize, pageIndex, query, filedOrder);

            DataSet ds = SqlHelper.Query(pageSql);
            return ds.Tables[0];
        }

        /// <summary>
        /// 批量删除盘点单
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public int DeleteBatch(List<string> bill_code_list, string userName)
        {

            List<CommandInfo> sqlList = new List<CommandInfo>();

            foreach (string bill_code in bill_code_list)
            {
                StringBuilder strSql_b = new StringBuilder();
                strSql_b.Append(" update " + table1 + " set ");
                strSql_b.Append("is_delete_tag=1,");
                strSql_b.Append("delete_user=@user,");
                strSql_b.Append("delete_time=GETDATE()");
                strSql_b.Append(" Where bill_code =@bill_code");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@user",userName),
                    new SqlParameter("@bill_code",bill_code)
                };
                CommandInfo cmd = new CommandInfo(strSql_b.ToString(), parameters);
                sqlList.Add(cmd);

            }
            int result = SqlHelper.ExecuteSqlTran(sqlList);
            return result;
        }

        /// <summary>
        /// 批量完成盘点单
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public int Complete(List<string> bill_code_list)
        {
            List<CommandInfo> sqlList = new List<CommandInfo>();

            foreach (string bill_code in bill_code_list)
            {
                StringBuilder strSql_b = new StringBuilder();
                strSql_b.Append(" update " + table1 + " set ");
                strSql_b.Append("state=1");
                strSql_b.Append(" Where bill_code =@bill_code");
                SqlParameter[] parameters =
                {
                    new SqlParameter("@bill_code",bill_code)
                };
                CommandInfo cmd = new CommandInfo(strSql_b.ToString(), parameters);
                sqlList.Add(cmd);

            }
            int result = SqlHelper.ExecuteSqlTran(sqlList);
            return result;
        }



        #region 添加增量盘点单相关逻辑（弃用）

        /// <summary>
        /// 查询盘点单头（分页）
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="strWhere"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataTable SelInvCheckHead1(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
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
        public DataTable GetDetail1(string bill_code, int pageSize, int pageIndex, string filedOrder, out int recordCount)
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


        #endregion


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

        #region 查询仓库表 （目前用不到）
        /// <summary>
        /// 查询仓库表 （目前用不到）
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
        #endregion

        #region 选择单号查询数据      

        /// <summary>
        /// 选择单号查询数据 (新逻辑)
        /// </summary>
        /// <param name="bill_code"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="after">是否盘点结束</param>
        /// <returns></returns>
        public ChooseBillCodeDTO SelDataByBillCode(string bill_code, int pageIndex, int pageSize, bool after)
        {
            ChooseBillCodeDTO result = new ChooseBillCodeDTO();
            string connectionString = SqlHelper.connectionString;

            Action<SqlConnection, SqlTransaction> sqlAction = (connection, transaction) =>
            {
                if (!after)
                {
                    // 如果是盘点前

                    // 1.通过单据编码查询表头仓库
                    Storage storageModel = GetStorageByBillCode(bill_code, connection, transaction);
                    result.storage_code = storageModel.storage_code;
                    result.storage_name = storageModel.storage_name;

                    // 2.通过单据编码查询所有待盘点数据
                    List<UHFInvCheck> toCheckList = GetBillCheckBody(bill_code, connection, transaction);
                    if (toCheckList != null && toCheckList.Count > 0)
                    {
                        // 万里牛中的库存数量  PS:新增字段 20240816
                        // 按spec_code去重
                        var distincttoCheckList = toCheckList.GroupBy(g => g.spec_code)
                                                          .Select(group => group.First())
                                                          .ToList();

                        // 计算去重后的num之和
                        result.wln_inv_num = (int)distincttoCheckList.Sum(m => m.quantity_start);
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

                }
                else
                {
                    // 如果是盘点后
                    // 1.查询已盘点数量
                    result.has_checked_num = GetCheckedNum(bill_code, connection, transaction);

                    result.currentPage = pageIndex;

                    // 2.查询未盘点数据集
                    GetUnCheckListOfPage(bill_code, result, pageSize, connection, transaction);
                }

            };

            SqlHelper.ExecuteTransaction(sqlAction, connectionString);

            return result;
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

        #endregion

        /// <summary>
        /// 通过单据编码查询未盘点数据集(分页)
        /// </summary>
        /// <param name="bill_code">单据编码</param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private void GetUnCheckListOfPage(string bill_code, ChooseBillCodeDTO result, int pageSize, SqlConnection connection, SqlTransaction transaction)
        {
            List<UHFInvCheck> list;
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
        INSERT INTO UHFInvCheck (id, goods_code, goods_name, bill_code, nums, quantity, quantity_start, 
        spec_code, spec_name, storage_code, has_check, create_time, RFID, barcode) 
        VALUES (@id, @goods_code, @goods_name, @bill_code, @nums, @quantity, @quantity_start, 
        @spec_code, @spec_name, @storage_code, -1, GETDATE(), @RFID, @barcode)";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@id", nextId);
                command.Parameters.AddWithValue("@goods_code", model.goods_code);
                command.Parameters.AddWithValue("@goods_name", model.goods_name);
                command.Parameters.AddWithValue("@bill_code", model.bill_code);
                command.Parameters.AddWithValue("@nums", model.nums.HasValue ? (object)model.nums.Value : DBNull.Value);
                command.Parameters.AddWithValue("@quantity", model.quantity.HasValue ? (object)model.quantity.Value : DBNull.Value);
                command.Parameters.AddWithValue("@quantity_start", model.quantity_start.HasValue ? (object)model.quantity_start.Value : DBNull.Value);
                command.Parameters.AddWithValue("@spec_code", model.spec_code);
                command.Parameters.AddWithValue("@spec_name", model.spec_name);
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
            string query = @"select id, goods_code, goods_name, bill_code,
                nums, quantity, quantity_start, spec_code, spec_name, 
                storage_code, has_check, create_time,RFID,barcode from UHFInvCheck ";
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
        /// <param name="bill_code">单号</param>
        /// <param name="isRepeat">0：初盘，1：复盘</param>
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
                    // 1. 过滤掉scannedRFIDsSet中不属于此单号下的RFID
                    //scannedRFIDsSet.RemoveWhere(rfid => !IsValidRFIDForBill(rfid, bill_code, connection, transaction));

                    // 找出未扫描的RFIDs
                    var missingRFIDs = allRFIDsSet.Except(scannedRFIDsSet);

                    // 修改已扫描的数据，将has_check改为1
                    UpdateHasCheck(scannedRFIDsSet, bill_code, allList, connection, transaction);

                    // 修改未扫描的数据，将has_check改为0
                    UpdateUnCheck(missingRFIDs, bill_code, connection, transaction);

                }
            };

            SqlHelper.ExecuteTransaction(sqlAction, connectionString);

            return true;
        }

        ///// <summary>
        ///// 验证rfid是否输入指定单号
        ///// </summary>
        ///// <param name="rfid"></param>
        ///// <param name="bill_code"></param>
        ///// <param name="connection"></param>
        ///// <param name="transaction"></param>
        ///// <returns></returns>
        //private bool IsValidRFIDForBill(string rfid, string bill_code, SqlConnection connection, SqlTransaction transaction)
        //{
        //    bool res = false;
        //    string sql = @"select count(1) from UHFInvCheck where bill_code=@bill_code and RFID=@RFID";
        //    using (SqlCommand command = new SqlCommand(sql, connection, transaction))
        //    {
        //        command.Parameters.AddWithValue("@bill_code", bill_code);
        //        command.Parameters.AddWithValue("@RFID", rfid);
        //        int i = (int)command.ExecuteScalar();
        //        if (i > 0)
        //        {
        //            res = true;
        //        }
        //        else
        //        {
        //            res = false;
        //        }
        //    }
        //    return res;
        //}

        /// <summary>
        /// 修改已扫描的数据，将has_check改为1；PS:新增逻辑，同时修改盘点单表体中的已盘数量和差异数量
        /// 如果表体中所有数据的差异数为0，这修改表头状态为 1：已完成
        /// </summary>
        /// <param name="scannedRFIDsSet"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        private void UpdateHasCheck(HashSet<string> scannedRFIDsSet, string bill_code, List<UHFInvCheck> allList, SqlConnection connection, SqlTransaction transaction)
        {
            List<InvCheckBillBody> b_list = new List<InvCheckBillBody>();
            foreach (var item in scannedRFIDsSet)
            {
                // 根据RFID和单号找到实体
                UHFInvCheck model = allList.FirstOrDefault(m => m.RFID == item);
                if (model != null)
                {
                    // 找到对象,创建表体
                    InvCheckBillBody b_modle = new InvCheckBillBody
                    {
                        spec_code = model.spec_code,
                        quantity_start = model.quantity_start,
                    };
                    b_list.Add(b_modle);
                    // 修改已扫描的数据，将has_check改为1
                    string sql = @"update UHFInvCheck set has_check = 1 where bill_code=@bill_code and RFID=@RFID";
                    using (SqlCommand command = new SqlCommand(sql, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@bill_code", bill_code);
                        command.Parameters.AddWithValue("@RFID", item);
                        command.ExecuteNonQuery();
                    }
                }

            }

            // 使用LINQ对集合中的barcode字段进行分组，并计算每个组的元素数量
            var groupedByCount = b_list.GroupBy(b => b.spec_code)
                                      .Select(group => new { specCode = group.Key, Count = group.Count(), startCount = group.First().quantity_start });
            // 修改盘点单体实盘数量与差异数量
            foreach (var item in groupedByCount)
            {
                int count = item.Count;
                int startCount = (int)item.startCount;
                int change_size = startCount - count;
                string sql = @"update InvCheckBillBody set quantity = @count,change_size=@change_size  where spec_code=@spec_code";
                using (SqlCommand command = new SqlCommand(sql, connection, transaction))
                {
                    command.Parameters.AddWithValue("@count", count);
                    command.Parameters.AddWithValue("@change_size", change_size);
                    // command.Parameters.AddWithValue("@spec_code", $"[{item.specCode}]"); // 仅当spec_code可能包含特殊字符时使用
                    command.Parameters.AddWithValue("@spec_code", item.specCode);
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
