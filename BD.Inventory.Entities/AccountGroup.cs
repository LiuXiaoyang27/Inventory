using System;

namespace BD.Inventory.Entities
{
    /// <summary>
    /// 用户组实体
    /// </summary>
    public class AccountGroup
    {
        public decimal ID { get; set; }

        /// <summary>
        /// 用户组编号
        /// </summary>
        public string GroupNo { get; set; }

        /// <summary>
        /// 用户组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 组状态（1：启用）
        /// </summary>
        public int GroupState { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string GroupMeno { get; set; }

        /// <summary>
        /// 组类型
        /// </summary>
        public string GroupType { get; set; }

        /// <summary>
        /// pc授权菜单
        /// </summary>
        public string GroupMenuPC { get; set; }

        /// <summary>
        /// 手持授权菜单
        /// </summary>
        public string GroupMenuSC { get; set; }

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
        /// 手机端授权菜单
        /// </summary>
        public string GroupMenuSJ { get; set; }

        /// <summary>
        /// 增删改查权限
        /// </summary>
        public string GroupZSGC { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public int IsDelete { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GroupSCID { get; set; }

        public AccountGroup()
        {
            GroupState = 1;
            GroupMeno = "";
            GroupType = "";
            GroupMenuPC = "";
            GroupMenuSC = "";
            CreateDate = null;
            UpdDate = null;
            GroupMenuSJ = "";
            GroupZSGC = "";
            IsDelete = 0;
            GroupSCID = "";
        }
    }
}
