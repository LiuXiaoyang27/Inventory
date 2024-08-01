using System;

namespace BD.Inventory.Entities
{
    ///<summary>
    ///菜单表
    ///</summary>
    public partial class Menu
    {

        /// <summary>
        /// Desc:自增主键ID
        /// </summary>       
        public int ID { get; set; }

        /// <summary>
        /// Desc:编号
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string MenuNo { get; set; }

        /// <summary>
        /// Desc:菜单名称
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string MenuName { get; set; }

        /// <summary>
        /// Desc:菜单级别
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int MenuLv { get; set; }

        /// <summary>
        /// Desc:上级菜单
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int ParentMenu { get; set; }

        /// <summary>
        /// Desc:菜单图标
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string MenuIcon { get; set; }

        /// <summary>
        /// Desc:链接地址
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string MenuUrl { get; set; }

        /// <summary>
        /// Desc:菜单状态（1：启用；2：禁用）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int MenuState { get; set; }

        /// <summary>
        /// Desc:排序码
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int MenuSort { get; set; }

        /// <summary>
        /// Desc:导航类型（1：目录；2：页面）
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int NavType { get; set; }

        /// <summary>
        /// Desc:是否删除
        /// Default:0
        /// Nullable:True
        /// </summary>           
        public int IsDelete { get; set; }

        /// <summary>
        /// Desc:操作时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 菜单类别 1：PC;2:手机
        /// </summary>
        public int MenuType { get; set; }

        /// <summary>
        /// Desc:备注
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string StandBy1 { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string StandBy2 { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string StandBy3 { get; set; }

        public Menu()
        {
            MenuNo = "";
            MenuIcon = "";
            MenuUrl = "/";
            MenuState = 1;
            NavType = 1;
            IsDelete = 0;
            ModifyTime = DateTime.Now;
            StandBy1 = "";
            StandBy2 = "";
            StandBy3 = "";
            MenuType = 1;
        }

    }
}
