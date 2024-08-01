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
    public class AccountGroupDal
    {
        // 使用 Lazy<T> 实现单例模式，确保实例的惰性初始化和线程安全。
        private static readonly Lazy<AccountGroupDal> _instance = new Lazy<AccountGroupDal>(() => new AccountGroupDal());

        /// <summary>
        /// 获取类的单例实例。
        /// </summary>
        public static AccountGroupDal Instance { get; } = _instance.Value;

        // 私有构造函数，防止外部实例化该类。
        private AccountGroupDal()
        {
        }

        private const string tableName = "AccountGroup";

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

            strSql.Append("select ID,GroupNo,GroupName,GroupState,GroupMeno,GroupType,GroupMenuPC,GroupMenuSC,");
            strSql.Append("CreateName,CreateDate,UpdName,UpdDate,GroupMenuSJ,GroupZSGC,GroupSCID ");
            strSql.Append("From " + tableName);
            strSql.Append(" where IsDelete = 0 ");
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
        /// 查询数据
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<AccountGroup> GetList(string strWhere)
        {
            StringBuilder sb = GetSelectSql(strWhere);
            DataSet ds = SqlHelper.Query(sb.ToString());
            return CommonOperation.ConvertDataTableToModelList<AccountGroup>(ds.Tables[0]);
        }

        /// <summary>
        /// 根据用户组ID查询实体对象
        /// </summary>
        /// <param name="groupNo"></param>
        /// <returns></returns>
        public AccountGroup GetModelByID(decimal groupID)
        {
            StringBuilder sb = GetSelectSql(" and GroupState=1 and ID=@groupID");

            SqlParameter[] parameters =
            {
                new SqlParameter("@groupID",groupID)
            };
            DataSet ds = SqlHelper.Query(sb.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return CommonOperation.DataTableToModel<AccountGroup>(ds.Tables[0]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 新增权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddGroup(AccountGroup model)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO " + tableName + "(");
            sb.Append("GroupNo,GroupName,GroupState,GroupMeno,GroupType,GroupMenuPC,GroupMenuSC,");
            sb.Append("CreateName,CreateDate,GroupMenuSJ,GroupZSGC,GroupSCID)");
            sb.Append(" VALUES(");
            sb.Append("@groupNo,@groupName,@groupState,@groupMeno,@groupType,@groupMenuPC,@groupMenuSC,");
            sb.Append("@createName,@createDate,@groupMenuSJ,@groupZSGC,@groupSCID)");
            SqlParameter[] parameters =
           {
                new SqlParameter("@groupNo",model.GroupNo),
                new SqlParameter("@groupName",model.GroupName),
                new SqlParameter("@groupState",model.GroupState),
                new SqlParameter("@groupMeno",model.GroupMeno),
                new SqlParameter("@groupType",model.GroupType),
                new SqlParameter("@groupMenuPC",model.GroupMenuPC),
                new SqlParameter("@groupMenuSC",model.GroupMenuSC),
                new SqlParameter("@createName",model.CreateName),
                new SqlParameter("@createDate",model.CreateDate.HasValue ? (object)model.CreateDate.Value : DBNull.Value),
                new SqlParameter("@groupMenuSJ",model.GroupMenuSJ),
                new SqlParameter("@groupZSGC",model.GroupZSGC),
                new SqlParameter("@groupSCID",model.GroupSCID)
            };
            int res = SqlHelper.ExecuteSql(sb.ToString(), parameters);
            return res;
        }

        /// <summary>
        /// 修改权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateGroup(AccountGroup model)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Update " + tableName + " SET ");
            sb.Append("GroupNo=@groupNo,");
            sb.Append("GroupName=@groupName,");
            sb.Append("GroupState=@groupState,");
            sb.Append("GroupMeno=@groupMeno,");
            sb.Append("GroupType=@groupType,");
            sb.Append("GroupMenuPC=@groupMenuPC,");
            sb.Append("GroupMenuSC=@groupMenuSC,");
            sb.Append("UpdName=@updName,");
            sb.Append("UpdDate=@updDate,");
            sb.Append("GroupMenuSJ=@groupMenuSJ,");
            sb.Append("GroupZSGC=@groupZSGC,");
            sb.Append("GroupSCID=@groupSCID ");
            sb.Append("WHERE ID=@id");
            SqlParameter[] parameters = {
                new SqlParameter("@groupNo",model.GroupNo),
                new SqlParameter("@groupName",model.GroupName),
                new SqlParameter("@groupState",model.GroupState),
                new SqlParameter("@groupMeno",model.GroupMeno),
                new SqlParameter("@groupType",model.GroupType),
                new SqlParameter("@groupMenuPC",model.GroupMenuPC),
                new SqlParameter("@groupMenuSC",model.GroupMenuSC),
                new SqlParameter("@updName",model.UpdName),
                new SqlParameter("@updDate",model.UpdDate.HasValue ? (object)model.UpdDate.Value : DBNull.Value),
                new SqlParameter("@groupMenuSJ",model.GroupMenuSJ),
                new SqlParameter("@groupZSGC",model.GroupZSGC),
                new SqlParameter("@groupSCID",model.GroupSCID),
                new SqlParameter("@id",model.ID),
            };
            int result = SqlHelper.ExecuteSql(sb.ToString(), parameters);
            return result;
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteGroup(decimal id)
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
        public int DeleteGroups(List<string> ids, string userName)
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
