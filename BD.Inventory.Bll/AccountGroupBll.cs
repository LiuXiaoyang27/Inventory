using BD.Inventory.Dal;
using BD.Inventory.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace BD.Inventory.Bll
{
    public class AccountGroupBll
    {
        private static readonly Lazy<AccountGroupBll> _instance = new Lazy<AccountGroupBll>(() => new AccountGroupBll());

        public static AccountGroupBll Instance { get; } = _instance.Value;

        // 获取单例实例
        private readonly AccountGroupDal DalInstance;

        private AccountGroupBll()
        {
            DalInstance = AccountGroupDal.Instance;
        }

        /// <summary>
        /// 查询数据是否存在
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public bool IsExist(string strWhere)
        {
            return DalInstance.IsExist(strWhere);
        }

        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="strWhere"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataTable GetPageList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return DalInstance.GetPageList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<AccountGroup> GetList(string strWhere)
        {
            return DalInstance.GetList(strWhere);
        }

        /// <summary>
        /// 根据用户组编号查询实体对象
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public AccountGroup GetModelByID(decimal groupID)
        {
            AccountGroup model = DalInstance.GetModelByID(groupID);
            return model;
        }

        /// <summary>
        /// 新增权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddGroup(AccountGroup model)
        {
            return DalInstance.AddGroup(model);
        }

        /// <summary>
        /// 修改权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateGroup(AccountGroup model)
        {
            return DalInstance.UpdateGroup(model);
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteGroup(decimal id)
        {
            return DalInstance.DeleteGroup(id);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public int DeleteGroups(List<string> ids, string userName)
        {
            return DalInstance.DeleteGroups(ids, userName);
        }

        /// <summary>
        /// 验证model各字段合法性
        /// </summary>
        /// <param name="model"></param>
        /// <param name="msg">错误信息</param>
        /// <returns></returns>
        public bool VerifyModel(AccountGroup model, ref string msg)
        {
            string strWhere;
            if (model.ID > 0)
            {
                strWhere = " and GroupNo='" + model.GroupNo + "' and GroupName='" + model.GroupName + "' and ID !=" + model.ID;
            }
            else
            {
                strWhere = " and GroupNo='" + model.GroupNo + "' and GroupName='" + model.GroupName + "'";
            }
            if (IsExist(strWhere))
            {
                msg = "用户组已存在";
                return false;
            }

            return true;
        }
    }
}
