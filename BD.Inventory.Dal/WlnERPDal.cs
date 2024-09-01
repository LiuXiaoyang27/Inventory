using BD.Inventory.Common;
using BD.Inventory.DBUtility;
using BD.Inventory.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD.Inventory.Dal
{
    public class WlnERPDal
    {
        // 使用 Lazy<T> 实现单例模式，确保实例的惰性初始化和线程安全。
        private static readonly Lazy<WlnERPDal> _instance = new Lazy<WlnERPDal>(() => new WlnERPDal());

        /// <summary>
        /// 获取类的单例实例。
        /// </summary>
        public static WlnERPDal Instance { get; } = _instance.Value;

        // 私有构造函数，防止外部实例化该类。
        private WlnERPDal()
        {
        }

        // 新增前删除原数据
        public int DelOldGoods()
        {
            List<CommandInfo> sqlList = new List<CommandInfo>();

            StringBuilder strSql_g = new StringBuilder();
            strSql_g.Append(" delete from Goods;");
            CommandInfo cmd = new CommandInfo(strSql_g.ToString());
            sqlList.Add(cmd);
            StringBuilder strSql_S = new StringBuilder();
            strSql_S.Append(" delete from Specifications;");
            CommandInfo cmd1 = new CommandInfo(strSql_S.ToString());
            sqlList.Add(cmd1);

            int result = SqlHelper.ExecuteSqlTran(sqlList);
            return result;
        }

        /// <summary>
        /// 批量插入商品信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public void InsertGoodsInfo(List<Goods> list, InsertGoodsResult result)
        {
            string connectionString = SqlHelper.connectionString;
            foreach (var good in list)
            {
                Action<SqlConnection, SqlTransaction> sqlAction = (connection, transaction) =>
                {

                    //if (!GoodExists(good.sys_goods_uid, connection, transaction))
                    //{
                    //    InsertGood(good, connection, transaction);
                    //    result.goods_insert_count++;
                    //}
                    //if (good.specs != null && good.specs.Count > 0)
                    //{
                    //    foreach (var spec in good.specs)
                    //    {
                    //        if (!SpecExists(spec.sys_spec_uid, connection, transaction))
                    //        {
                    //            InsertSpec(spec, connection, transaction);
                    //            result.spec_insert_count++;
                    //        }
                    //    }
                    //}
                    InsertGood(good, connection, transaction);
                    result.goods_insert_count++;
                    if (good.specs != null && good.specs.Count > 0)
                    {
                        foreach (var spec in good.specs)
                        {
                            InsertSpec(spec, connection, transaction);
                            result.spec_insert_count++;
                        }
                    }
                };

                SqlHelper.ExecuteTransaction(sqlAction, connectionString);
            }

        }

        /// <summary>
        /// 查询商品是否存在
        /// </summary>
        /// <param name="sys_goods_uid"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private bool GoodExists(string sys_goods_uid, SqlConnection connection, SqlTransaction transaction)
        {
            string query = "SELECT COUNT(1) FROM Goods WHERE is_delete_tag = 0 AND sys_goods_uid = @sys_goods_uid";
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@sys_goods_uid", sys_goods_uid);
                return (int)command.ExecuteScalar() > 0;
            }
        }

        /// <summary>
        /// 插入商品
        /// </summary>
        /// <param name="good"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        private void InsertGood(Goods good, SqlConnection connection, SqlTransaction transaction)
        {
            string query = @"
        INSERT INTO Goods (sys_goods_uid, goods_code, goods_name, catagory_id, catagory_name, brand_name, unit_name, manufacturer_name, purchase_type_name, 
        purchase_num, remark, status, modify_time) 
        VALUES (@sys_goods_uid, @goods_code, @goods_name, @catagory_id, @catagory_name, @brand_name, @unit_name, @manufacturer_name, @purchase_type_name, 
        @purchase_num, @remark, @status, @modify_time)";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@sys_goods_uid", good.sys_goods_uid);
                command.Parameters.AddWithValue("@goods_code", good.goods_code);
                command.Parameters.AddWithValue("@goods_name", good.goods_name);
                command.Parameters.AddWithValue("@catagory_id", good.catagory_id);
                command.Parameters.AddWithValue("@catagory_name", good.catagory_name);
                command.Parameters.AddWithValue("@brand_name", good.brand_name);
                command.Parameters.AddWithValue("@unit_name", good.unit_name);
                command.Parameters.AddWithValue("@manufacturer_name", good.manufacturer_name);
                command.Parameters.AddWithValue("@purchase_type_name", good.purchase_type_name);
                command.Parameters.AddWithValue("@purchase_num", good.purchase_num);
                command.Parameters.AddWithValue("@remark", good.remark);
                command.Parameters.AddWithValue("@status", good.status);
                command.Parameters.AddWithValue("@modify_time", good.modify_time);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 规格是否存在
        /// </summary>
        /// <param name="sys_spec_uid"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private bool SpecExists(string sys_spec_uid, SqlConnection connection, SqlTransaction transaction)
        {
            string query = "SELECT COUNT(1) FROM Specifications WHERE sys_spec_uid = @sys_spec_uid";
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@sys_spec_uid", sys_spec_uid);
                return (int)command.ExecuteScalar() > 0;
            }
        }

        /// <summary>
        ///  插入规格
        /// </summary>
        /// <param name="spec"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        private void InsertSpec(Specifications spec, SqlConnection connection, SqlTransaction transaction)
        {
            string query = @"
        INSERT INTO Specifications (sys_spec_uid, goods_code, spec_code, spec1, spec2, barcode, barcodes, pic, height, length, width, 
        weight, sale_price, wholesale_price, prime_price, status) 
        VALUES (@sys_spec_uid, @goods_code, @spec_code, @spec1, @spec2, @barcode, @barcodes, @pic, @height, @length, @width, 
        @weight, @sale_price, @wholesale_price, @prime_price, @status)";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@sys_spec_uid", spec.sys_spec_uid);
                command.Parameters.AddWithValue("@goods_code", spec.goods_code);
                command.Parameters.AddWithValue("@spec_code", spec.spec_code);
                command.Parameters.AddWithValue("@spec1", spec.spec1);
                command.Parameters.AddWithValue("@spec2", spec.spec2);
                command.Parameters.AddWithValue("@barcode", spec.barcode);
                command.Parameters.AddWithValue("@barcodes", spec.barcodes);
                command.Parameters.AddWithValue("@pic", spec.pic);
                command.Parameters.AddWithValue("@height", spec.height);
                command.Parameters.AddWithValue("@length", spec.length);
                command.Parameters.AddWithValue("@width", spec.width);
                command.Parameters.AddWithValue("@weight", spec.weight);
                command.Parameters.AddWithValue("@sale_price", spec.sale_price);
                command.Parameters.AddWithValue("@wholesale_price", spec.wholesale_price);
                command.Parameters.AddWithValue("@prime_price", spec.prime_price);
                command.Parameters.AddWithValue("@status", spec.status);

                command.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// 批量插入盘点单信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public void InsertCheckBillInfo(List<InvCheckBillHead> list, InsertCheckBillResult result)
        {
            string connectionString = SqlHelper.connectionString;
            foreach (var checkbill in list)
            {
                Action<SqlConnection, SqlTransaction> sqlAction = (connection, transaction) =>
                {
                    // 插入仓库信息
                    if (!StorageExists(checkbill.storage_code, connection, transaction))
                    {
                        Storage storage = new Storage { storage_code = checkbill.storage_code, storage_name = checkbill.storage_name };
                        InsertStorage(storage, connection, transaction);
                        result.storage_insert_count++;
                    }
                    // 插入盘点单信息
                    if (!CheckBillHeadExists(checkbill.bill_code, connection, transaction))
                    {
                        InsertBillHead(checkbill, connection, transaction);
                        result.check_bill_head_insert_count++;

                        if (checkbill.details != null && checkbill.details.Count > 0)
                        {
                            foreach (var detail in checkbill.details)
                            {
                                if (!CheckBillBodyExists(detail.goods_code, detail.bill_code, detail.spec_code, connection, transaction))
                                {
                                    InsertBillBody(detail, connection, transaction);
                                    result.check_bill_body_insert_count++;
                                }
                            }
                        }
                    }
                };

                SqlHelper.ExecuteTransaction(sqlAction, connectionString);
            }

            //return result;
        }

        /// <summary>
        /// 查询仓库是否存在
        /// </summary>
        /// <param name="sys_goods_uid"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private bool StorageExists(string storage_code, SqlConnection connection, SqlTransaction transaction)
        {
            string query = "SELECT COUNT(1) FROM Storage WHERE is_delete_tag = 0 AND storage_code = @storage_code";
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@storage_code", storage_code);
                return (int)command.ExecuteScalar() > 0;
            }
        }

        /// <summary>
        /// 插入仓库
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        private void InsertStorage(Storage storage, SqlConnection connection, SqlTransaction transaction)
        {
            string query = @"
        INSERT INTO Storage (id, storage_code, storage_name) 
        VALUES (@id, @storage_code, @storage_name)";

            string nextId = Utils.GetNextID();

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@id", nextId);
                command.Parameters.AddWithValue("@storage_code", storage.storage_code);
                command.Parameters.AddWithValue("@storage_name", storage.storage_name);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 查询盘点单头是否存在
        /// </summary>
        /// <param name="sys_goods_uid"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private bool CheckBillHeadExists(string bill_code, SqlConnection connection, SqlTransaction transaction)
        {
            string query = "SELECT COUNT(1) FROM InvCheckBillHead WHERE is_delete_tag = 0 AND bill_code = @bill_code";
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@bill_code", bill_code);
                return (int)command.ExecuteScalar() > 0;
            }
        }

        /// <summary>
        /// 插入盘点单头
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        private void InsertBillHead(InvCheckBillHead invCBHead, SqlConnection connection, SqlTransaction transaction)
        {
            string query = @"
        INSERT INTO InvCheckBillHead (id,bill_code,bill_creater,bill_date,create_time,remark,state,storage_code,storage_name) 
        VALUES (@id,@bill_code, @bill_creater, @bill_date,@create_time,@remark, @state, @storage_code, @storage_name)";

            string nextId = Utils.GetNextID();

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@id", nextId);
                command.Parameters.AddWithValue("@bill_code", invCBHead.bill_code);
                command.Parameters.AddWithValue("@bill_creater", invCBHead.bill_creater);
                command.Parameters.AddWithValue("@bill_date", invCBHead.bill_date.HasValue ? (object)invCBHead.bill_date.Value : DBNull.Value);
                command.Parameters.AddWithValue("@create_time", invCBHead.create_time.HasValue ? (object)invCBHead.create_time.Value : DBNull.Value);
                command.Parameters.AddWithValue("@remark", invCBHead.remark);
                command.Parameters.AddWithValue("@state", invCBHead.state.HasValue ? (object)invCBHead.state.Value : DBNull.Value);
                command.Parameters.AddWithValue("@storage_code", invCBHead.storage_code);
                command.Parameters.AddWithValue("@storage_name", invCBHead.storage_name);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 查询盘点单体是否存在
        /// </summary>
        /// <param name="goods_code"></param>
        /// <param name="bill_code"></param>
        /// <param name="spec_code"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private bool CheckBillBodyExists(string goods_code, string bill_code, string spec_code, SqlConnection connection, SqlTransaction transaction)
        {
            string query = "SELECT COUNT(1) FROM InvCheckBillBody WHERE goods_code = @goods_code AND bill_code = @bill_code AND spec_code = @spec_code";
            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@goods_code", goods_code);
                command.Parameters.AddWithValue("@bill_code", bill_code);
                command.Parameters.AddWithValue("@spec_code", spec_code);
                return (int)command.ExecuteScalar() > 0;
            }
        }

        /// <summary>
        /// 插入盘点单体
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        private void InsertBillBody(InvCheckBillBody model, SqlConnection connection, SqlTransaction transaction)
        {
            string query = @"
        INSERT INTO InvCheckBillBody (id, goods_code, goods_name, bill_code, batch_code, batch_date, change_size, expiry_date, [index], inventory_type, 
        nums, price, product_batch_code, quantity, quantity_start, remark, spec_code, spec_name, stock_type, total_money, unit) 
        VALUES (@id, @goods_code, @goods_name, @bill_code, @batch_code, @batch_date, @change_size, @expiry_date, @index, @inventory_type, 
        @nums, @price, @product_batch_code, @quantity, @quantity_start, @remark, @spec_code, @spec_name, @stock_type, @total_money, @unit)";

            string nextId = Utils.GetNextID();

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@id", nextId);
                command.Parameters.AddWithValue("@goods_code", model.goods_code);
                command.Parameters.AddWithValue("@goods_name", model.goods_name);
                command.Parameters.AddWithValue("@bill_code", model.bill_code);
                command.Parameters.AddWithValue("@batch_code", model.batch_code);
                command.Parameters.AddWithValue("@batch_date", model.batch_date.HasValue ? (object)model.batch_date.Value : DBNull.Value);
                command.Parameters.AddWithValue("@change_size", model.change_size.HasValue ? (object)model.change_size.Value : DBNull.Value);
                command.Parameters.AddWithValue("@expiry_date", model.expiry_date.HasValue ? (object)model.expiry_date.Value : DBNull.Value);
                command.Parameters.AddWithValue("@index", model.index.HasValue ? (object)model.index.Value : DBNull.Value);
                command.Parameters.AddWithValue("@inventory_type", model.inventory_type);
                command.Parameters.AddWithValue("@nums", model.nums.HasValue ? (object)model.nums.Value : DBNull.Value);
                command.Parameters.AddWithValue("@price", model.price.HasValue ? (object)model.price.Value : DBNull.Value);
                command.Parameters.AddWithValue("@product_batch_code", model.product_batch_code);
                command.Parameters.AddWithValue("@quantity", model.quantity.HasValue ? (object)model.quantity.Value : DBNull.Value);
                command.Parameters.AddWithValue("@quantity_start", model.quantity_start.HasValue ? (object)model.quantity_start.Value : DBNull.Value);
                command.Parameters.AddWithValue("@remark", model.remark);
                command.Parameters.AddWithValue("@spec_code", model.spec_code);
                command.Parameters.AddWithValue("@spec_name", model.spec_name);
                command.Parameters.AddWithValue("@stock_type", model.stock_type.HasValue ? (object)model.stock_type.Value : DBNull.Value);
                command.Parameters.AddWithValue("@total_money", model.total_money.HasValue ? (object)model.total_money.Value : DBNull.Value);
                command.Parameters.AddWithValue("@unit", model.unit);

                command.ExecuteNonQuery();
            }
        }

    }
}
