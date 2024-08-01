using BD.Inventory.Dal;
using BD.Inventory.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace BD.Inventory.Bll
{
    public class MenuBll
    {
        private static readonly Lazy<MenuBll> _instance = new Lazy<MenuBll>(() => new MenuBll());

        public static MenuBll Instance { get; } = _instance.Value;

        // 获取单例实例
        private readonly MenuDal DalInstance;

        private MenuBll()
        {
            DalInstance = MenuDal.Instance;
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
        /// 查询菜单集合
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public List<Menu> GetList(string strWhere)
        {
            return DalInstance.GetList(strWhere);
        }

        /// <summary>
        /// 查询PC菜单集合
        /// </summary>
        /// <param name="menuName"></param>
        /// <param name="menuZSGC"></param>
        /// <returns></returns>
        public List<Menu> GetListByMenuName(string menuName, string menuZSGC)
        {
            return DalInstance.GetListByMenuName(menuName, menuZSGC);
        }

        /// <summary>
        /// 查询手持菜单集合
        /// </summary>
        /// <param name="menuName"></param>
        /// <param name="menuZSGC"></param>
        /// <returns></returns>
        public List<Menu> GetListSCByMenuName(string menuName)
        {
            return DalInstance.GetListSCByMenuName(menuName);
        }

        

        /// <summary>
        /// 将list转为树形json结构
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public JArray GetTreeJson(List<Menu> list)
        {
            JArray result = new JArray();
            List<Menu> parentList = GetParentNodes(list);
            JArray children = new JArray();
            JObject parent;

            foreach (Menu model in parentList)
            {
                children = GetChilds(list, model.ID);
                parent = new JObject();
                parent["ID"] = model.ID;
                parent["ParentMenu"] = model.ParentMenu;
                parent["MenuNo"] = model.MenuNo;
                parent["MenuName"] = model.MenuName;
                parent["MenuLv"] = model.MenuLv;
                parent["MenuUrl"] = model.MenuUrl;
                parent["MenuIcon"] = model.MenuIcon;
                parent["MenuState"] = model.MenuState;
                parent["NavType"] = model.NavType;
                parent["MenuSort"] = model.MenuSort;
                parent["StandBy1"] = model.StandBy1;
                parent["StandBy2"] = model.StandBy2;
                parent["StandBy3"] = model.StandBy3;
                parent["hasList"] = children.Count > 0 ? true : false;
                parent["list"] = children;
                result.Add(parent);
            }
            return result;
        }

        /// <summary>
        /// 获取父节点信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<Menu> GetParentNodes(List<Menu> list)
        {
            List<Menu> result = new List<Menu>();
            Menu menu = new Menu();
            result.AddRange(list.FindAll(a => a.ParentMenu == 0));
            return result;
        }

        /// <summary>
        /// 子节点数据
        /// </summary>
        /// <param name="list">列表数据</param>
        /// <param name="id">id</param>
        /// <returns></returns>
        public JArray GetChilds(List<Menu> list, int parentId)
        {
            JArray result = new JArray();
            foreach (Menu model in list)
            {
                if (model.ParentMenu == parentId)
                {
                    JObject obj = new JObject();
                    JArray children = GetChilds(list, model.ID);
                    obj["ID"] = model.ID;
                    obj["ParentMenu"] = model.ParentMenu;
                    obj["MenuNo"] = model.MenuNo;
                    obj["MenuName"] = model.MenuName;
                    obj["MenuLv"] = model.MenuLv;
                    obj["MenuUrl"] = model.MenuUrl;
                    obj["MenuIcon"] = model.MenuIcon;
                    obj["MenuState"] = model.MenuState;
                    obj["NavType"] = model.NavType;
                    obj["MenuSort"] = model.MenuSort;
                    obj["StandBy1"] = model.StandBy1;
                    obj["StandBy2"] = model.StandBy2;
                    obj["StandBy3"] = model.StandBy3;
                    obj["hasList"] = children.Count > 0 ? true : false;
                    obj["list"] = children;
                    result.Add(obj);
                }
            }
            return result;
        }
    }
}
