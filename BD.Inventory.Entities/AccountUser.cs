using System;

namespace BD.Inventory.Entities
{
    /// <summary>
    /// 用户表
    /// </summary>
    public class AccountUser
    {
        /// <summary>
        /// 主键
        /// </summary>
        public decimal ID { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string UserNo { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户类型
        /// </summary>
        public string UserType { get; set; }

        /// <summary>
        /// 用户组ID
        /// </summary>
        public string GroupNo { get; set; }

        /// <summary>
        /// 用户组编号
        /// </summary>
        public string GroupNo1 { get; set; }

        /// <summary>
        /// Desc:用户组名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string GroupName { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        public int UserState { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateName { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string UpdName { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime? UpdDate { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public int IsDelete { get; set; }

        public AccountUser()
        {
            PassWord = "123";
            UserType = "";
            GroupNo = "";
            UserState = 1;
            IsDelete = 0;
            CreateDate = null;
            UpdDate = null;
        }
    }
}
