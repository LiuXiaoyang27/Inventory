using BD.Inventory.Common;
using BD.Inventory.DBUtility;
using BD.Inventory.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BD.Inventory.Dal
{
    public class MenuDal
    {
        // 使用 Lazy<T> 实现单例模式，确保实例的惰性初始化和线程安全。
        private static readonly Lazy<MenuDal> _instance = new Lazy<MenuDal>(() => new MenuDal());

        /// <summary>
        /// 获取类的单例实例。
        /// </summary>
        public static MenuDal Instance { get; } = _instance.Value;

        // 私有构造函数，防止外部实例化该类。
        private MenuDal()
        {
        }

        private const string tableName = "Menu";

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

            strSql.Append("select ID,MenuNo,MenuName,MenuLv,ParentMenu,MenuIcon,MenuUrl,MenuState,");
            strSql.Append("MenuSort,NavType,ModifyTime,MenuType,StandBy1,StandBy2,StandBy3 ");
            strSql.Append("from " + tableName);
            strSql.Append(" where IsDelete = 0 ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(strWhere);
            }

            strSql.Append(" Order by MenuSort ");

            return strSql;
        }

        /// <summary>
        /// 查询菜单集合
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<Menu> GetList(string strWhere)
        {
            StringBuilder sb = GetSelectSql(strWhere);
            DataSet ds = SqlHelper.Query(sb.ToString());
            return CommonOperation.ConvertDataTableToModelList<Menu>(ds.Tables[0]);
        }


        /// <summary>
        /// 查询菜单集合
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<Menu> GetListByMenuName(string menuName, string menuZSGC)
        {
            List<Menu> resList = new List<Menu>();
            StringBuilder sb = GetSelectSql(" and MenuType=1 and MenuName in(" + menuName + ")");

            DataSet ds = SqlHelper.Query(sb.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                resList = CommonOperation.ConvertDataTableToModelList<Menu>(ds.Tables[0]);

                // 查询三级菜单
                StringBuilder sb3 = GetSelectSql(" and MenuType=1 and ID in(" + menuZSGC + ")");
                DataSet ds3 = SqlHelper.Query(sb3.ToString());
                if (ds3.Tables[0].Rows.Count > 0)
                {
                    List<Menu> lv3List = CommonOperation.ConvertDataTableToModelList<Menu>(ds3.Tables[0]);
                    resList.AddRange(lv3List);
                }
            }

            return resList;
        }

        /// <summary>
        /// 查询手持菜单集合
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<Menu> GetListSCByMenuName(string menuName)
        {
            List<Menu> resList = new List<Menu>();
            StringBuilder sb = GetSelectSql(" and MenuType=2 and MenuName in(" + menuName + ")");

            DataSet ds = SqlHelper.Query(sb.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                resList = CommonOperation.ConvertDataTableToModelList<Menu>(ds.Tables[0]);              
            }

            return resList;
        }

    }
}
