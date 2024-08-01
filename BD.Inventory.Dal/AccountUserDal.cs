using BD.Inventory.Common;
using BD.Inventory.DBUtility;
using BD.Inventory.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace BD.Inventory.Dal
{
    public class AccountUserDal
    {
        // 使用 Lazy<T> 实现单例模式，确保实例的惰性初始化和线程安全。
        private static readonly Lazy<AccountUserDal> _instance = new Lazy<AccountUserDal>(() => new AccountUserDal());

        /// <summary>
        /// 获取类的单例实例。
        /// </summary>
        public static AccountUserDal Instance { get; } = _instance.Value;

        // 私有构造函数，防止外部实例化该类。
        private AccountUserDal()
        {
        }

        private const string tableName = "AccountUser";

        /// <summary>
        /// 查询数据是否存在
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public bool IsExist(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from " + tableName);
            strSql.Append(" where IsDelete=0 " + strWhere);
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

            strSql.Append("select t1.ID,t1.UserNo,t1.PassWord,t1.UserName,t1.UserType,t1.GroupNo,t2.GroupNo as GroupNo1,t2.GroupName,t1.UserState,t1.Type,");
            strSql.Append("t1.CreateName,t1.CreateDate,t1.UpdName,t1.UpdDate ");
            strSql.Append("From " + tableName + " t1 Left Join AccountGroup t2 ");
            strSql.Append("ON t1.GroupNo=t2.ID");
            strSql.Append(" where t1.IsDelete = 0 ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(strWhere);
            }

            return strSql;
        }

        /// <summary>
        /// 根据账号获取实体信息（登录用）
        /// </summary>
        /// <param name="userNo">账号</param>
        /// <returns></returns>
        public AccountUser GetLoginModel(string userNo)
        {

            StringBuilder sb = GetSelectSql(" and t1.UserState=1 and t1.UserNo=@userNo");

            SqlParameter[] parameters =
            {
                new SqlParameter("@userNo",userNo)
            };
            DataSet ds = SqlHelper.Query(sb.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return CommonOperation.DataTableToModel<AccountUser>(ds.Tables[0]);
            }
            else
            {
                return null;
            }
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
        /// 查询数据
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<AccountUser> GetList(string strWhere)
        {
            StringBuilder sb = GetSelectSql(strWhere);
            DataSet ds = SqlHelper.Query(sb.ToString());
            return CommonOperation.ConvertDataTableToModelList<AccountUser>(ds.Tables[0]);
        }

        /// <summary>
        /// 根据ID查询实体对象
        /// </summary>
        /// <param name="groupNo"></param>
        /// <returns></returns>
        public AccountUser GetModelByID(decimal ID)
        {
            StringBuilder sb = GetSelectSql(" and t1.UserState=1 and t1.ID=@id");

            SqlParameter[] parameters =
            {
                new SqlParameter("@id",ID)
            };
            DataSet ds = SqlHelper.Query(sb.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return CommonOperation.DataTableToModel<AccountUser>(ds.Tables[0]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddUser(AccountUser model)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO " + tableName + "(");
            sb.Append("UserNo,PassWord,UserName,UserType,GroupNo,UserState,Type,");
            sb.Append("CreateName,CreateDate)");
            sb.Append(" VALUES(");
            sb.Append("@userNo,@passWord,@userName,@userType,@groupNo,@userState,@type,");
            sb.Append("@createName,@createDate)");
            SqlParameter[] parameters =
           {
                new SqlParameter("@userNo",model.UserNo),
                new SqlParameter("@passWord",model.PassWord),
                new SqlParameter("@userName",model.UserName),
                new SqlParameter("@userType",model.UserType),
                new SqlParameter("@groupNo",model.GroupNo),
                new SqlParameter("@userState",model.UserState),
                new SqlParameter("@type",model.Type),
                new SqlParameter("@createName",model.CreateName),
                new SqlParameter("@createDate",model.CreateDate.HasValue ? (object)model.CreateDate.Value : DBNull.Value)
            };
            int res = SqlHelper.ExecuteSql(sb.ToString(), parameters);
            return res;
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateUser(AccountUser model)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Update " + tableName + " SET ");
            sb.Append("UserName=@userName,");
            sb.Append("UserType=@userType,");
            sb.Append("GroupNo=@groupNo,");
            sb.Append("UserState=@userState,");
            sb.Append("Type=@type,");
            sb.Append("UpdName=@updName,");
            sb.Append("UpdDate=@updDate ");
            sb.Append("WHERE ID=@id");
            SqlParameter[] parameters = {
                new SqlParameter("@userName",model.UserName),
                new SqlParameter("@userType",model.UserType),
                new SqlParameter("@groupNo",model.GroupNo),
                new SqlParameter("@userState",model.UserState),
                new SqlParameter("@type",model.Type),
                new SqlParameter("@updName",model.UpdName),
                new SqlParameter("@updDate",model.UpdDate.HasValue ? (object)model.UpdDate.Value : DBNull.Value),
                new SqlParameter("@id",model.ID),
            };
            int result = SqlHelper.ExecuteSql(sb.ToString(), parameters);
            return result;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteUser(decimal id)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append(" update " + tableName + " set ");
            strSql.Append(" IsDelete=1");
            strSql.Append(" Where ID =" + id);
            int result = SqlHelper.ExecuteSql(strSql.ToString());
            return result > 0;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public int DeleteUsers(List<string> ids, string userName)
        {
            StringBuilder strSql;
            List<CommandInfo> sqlList = new List<CommandInfo>();
            CommandInfo cmd;
            foreach (string id in ids)
            {
                strSql = new StringBuilder();
                strSql.Append(" update " + tableName + " set ");
                strSql.Append(" IsDelete=1");
                strSql.Append(" Where ID =" + id);
                cmd = new CommandInfo(strSql.ToString());
                sqlList.Add(cmd);
            }
            int result = SqlHelper.ExecuteSqlTran(sqlList);
            return result;
        }
    }
}
