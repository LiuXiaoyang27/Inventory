using System;

namespace BD.Inventory.Entities
{
    /// <summary>
    /// JWT载荷实体
    /// </summary>
    public class JWTPlayloadInfo
    {
        /// <summary>
        /// jwt签发者
        /// </summary>
        public string iss { get; set; } = "BD.Inventory.WebApi";

        /// <summary>
        /// 接收jwt的一方 
        /// </summary>
        public string aud { get; set; } = "";

        /// <summary>
        /// jwt的签发时间
        /// </summary>
        public DateTime iat { get; set; } = DateTime.Now;

        /// <summary>
        /// jwt过期时间
        /// </summary>
        public double exp { get; set; }

        /// <summary>
        /// 用户账户
        /// </summary>
        public string UserNo { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户组
        /// </summary>
        public string GroupNo { get; set; }

        ///// <summary>
        ///// 公司id
        ///// </summary>
        //public int CompanyId { get; set; }

        ///// <summary>
        ///// 公司名称
        ///// </summary>
        //public string CompanyName { get; set; }

        ///// <summary>
        ///// 用户部门ID
        ///// </summary>
        //public int DeptId { get; set; }

        ///// <summary>
        ///// 部门名称
        ///// </summary>
        //public string DeptName { get; set; }

        ///// <summary>
        ///// 角色id
        ///// </summary>
        //public int RoleId { get; set; }

        ///// <summary>
        ///// 角色名
        ///// </summary>
        //public string RoleName { get; set; }

        ///// <summary>
        ///// 岗位ID
        ///// </summary>
        //public int PostId { get; set; }

        ///// <summary>
        ///// 岗位名
        ///// </summary>
        //public string PostName { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginTime { get; set; } = DateTime.Now;

        ///// <summary>
        ///// 上次登录时间
        ///// </summary>
        //public DateTime prevLoginTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 登录IP
        /// </summary>
        public string LoginIP { get; set; }

        ///// <summary>
        ///// 上次登录IP
        ///// </summary>
        //public string prevLoginIP { get; set; }

        ///// <summary>
        ///// 1：超级管理员
        ///// </summary>
        //public int IsAdmain { get; set; }

        /// <summary>
        /// 状态（是否登录成功）
        /// </summary>
        public string Status { get; set; } = "OK";

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }
    }
}