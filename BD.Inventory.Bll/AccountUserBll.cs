using BD.Inventory.Dal;
using BD.Inventory.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace BD.Inventory.Bll
{
    public class AccountUserBll
    {
        private static readonly Lazy<AccountUserBll> _instance = new Lazy<AccountUserBll>(() => new AccountUserBll());

        public static AccountUserBll Instance { get; } = _instance.Value;

        // 获取单例实例
        private readonly AccountUserDal DalInstance;

        private AccountUserBll()
        {
            DalInstance = AccountUserDal.Instance;
        }

        /// <summary>
        /// 登录校验
        /// </summary>
        /// <param name="userAccount">账号</param>
        /// <param name="password">密码</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public AccountUser CheckAccount(string userAccount, string password, out string msg)
        {
            if (string.IsNullOrEmpty(userAccount))
            {
                msg = "请输入账号！";
                return null;
            }

            if (string.IsNullOrEmpty(password))
            {
                msg = "请输入密码！";
                return null;
            }

            AccountUser model = GetLoginModel(userAccount);

            if (model == null)
            {
                msg = "用户不存在！";
                return null;
            }
            else
            {
                msg = "OK！";
                return model;
            }
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
        /// 根据账号获取实体信息（登录用）
        /// </summary>
        /// <param name="userNo">账号</param>
        /// <returns></returns>
        public AccountUser GetLoginModel(string userNo)
        {
            return DalInstance.GetLoginModel(userNo);
        }

        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <param name="strWhere"></param>
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
        public List<AccountUser> GetList(string strWhere)
        {
            return DalInstance.GetList(strWhere);
        }

        /// <summary>
        /// 根据ID查询实体对象
        /// </summary>
        /// <param name="groupNo"></param>
        /// <returns></returns>
        public AccountUser GetModelByID(decimal ID)
        {
            return DalInstance.GetModelByID(ID);
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int AddUser(AccountUser model)
        {
            return DalInstance.AddUser(model);
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateUser(AccountUser model)
        {
            return DalInstance.UpdateUser(model);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteUser(decimal id)
        {
            return DalInstance.DeleteUser(id);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public int DeleteUsers(List<string> ids, string userName)
        {
            return DalInstance.DeleteUsers(ids, userName);
        }

        /// <summary>
        /// 验证model各字段合法性
        /// </summary>
        /// <param name="model"></param>
        /// <param name="msg">错误信息</param>
        /// <returns></returns>
        public bool VerifyModel(AccountUser model, ref string msg)
        {
            string strWhere;
            if (model.ID > 0)
            {
                strWhere = " and UserNo='" + model.UserNo + "' and ID !=" + model.ID;
            }
            else
            {
                strWhere = " and UserNo='" + model.UserNo + "'";
            }
            if (IsExist(strWhere))
            {
                msg = "用户账号已存在";
                return false;
            }

            return true;
        }
    }
}
