using BD.Inventory.Bll;
using BD.Inventory.Common;
using BD.Inventory.Entities;
using BD.Inventory.WebApi.App_Start;
using BD.Inventory.WebApi.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace BD.Inventory.WebApi.Controllers.PC
{
    /// <summary>
    /// 用户组
    /// </summary>
    [ControllerGroup("AccountGroup", "PC端-权限管理")]
    public class AccountGroupController : BaseController
    {
        private JWTPlayloadInfo playload;
        private readonly AccountGroupBll _instance;
        private readonly MenuBll menu_instance;
        /// <summary>
        /// 构造方法
        /// </summary>
        public AccountGroupController()
        {
            _instance = AccountGroupBll.Instance;
            menu_instance = MenuBll.Instance;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页尺寸</param>
        /// <param name="groupNo">用户组编号</param>
        /// <param name="groupName">用户组名称</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage SelAccountGroup(int pageIndex = 1, int pageSize = 10, string groupNo = "", string groupName = "")
        {
            playload = (JWTPlayloadInfo)Request.Properties["playload"];
            try
            {
                StringBuilder strWhere = new StringBuilder("");
                if (!string.IsNullOrEmpty(groupNo))
                {
                    strWhere.Append(" and GroupNo like '%" + groupNo + "%'");
                }
                if (!string.IsNullOrEmpty(groupName))
                {
                    strWhere.Append(" and GroupName like '%" + groupName + "%'");
                }

                var dt = _instance.GetPageList(pageSize, pageIndex, strWhere.ToString(), "CreateDate DESC", out int records);
                int totalPages = PageHelper.GetPageCount(pageSize, records);
                return JsonHelper.SuccessJson(CommonOperation.DataTable2JArray(dt), pageIndex, totalPages, records);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Show, "分页查询");
                return JsonHelper.ErrorJson(ex.Message);
            }
        }

        /// <summary>
        /// 新增时查询所有的菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage SelSys_Menu()
        {
            // PC菜单
            List<Menu> listPC = menu_instance.GetList(" and MenuType=1 and MenuState=1 ");

            // SC菜单
            List<Menu> listSC = menu_instance.GetList(" and MenuType=2 and MenuState=1 ");

            return JsonHelper.SuccessJson(menu_instance.GetTreeJson(listPC), menu_instance.GetTreeJson(listSC));
        }

        /// <summary>
        /// 根据ID获取实体对象
        /// </summary>
        /// <param name="id">数据表ID</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage SelAccountGroupBYID(string id)
        {
            playload = (JWTPlayloadInfo)Request.Properties["playload"];
            if (string.IsNullOrEmpty(id))
            {
                return JsonHelper.FailJson("参数不能为空！");
            }
            try
            {
                // 判断格式
                if (!decimal.TryParse(id, out decimal ID))
                {
                    return JsonHelper.FailJson("参数错误！");
                }
                AccountGroup model = _instance.GetModelByID(ID);
                if (model != null)
                {
                    return JsonHelper.SuccessJson(JsonHelper.ModelToJObject(model));
                }
                else
                {
                    return JsonHelper.NullJson("无数据");
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Show, "根据ID获取实体对象");
                return JsonHelper.ErrorJson(ex.Message);
            }

        }

        /// <summary>
        /// 新增权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage InsAccountGroup([FromBody] AccountGroup model)
        {
            playload = (JWTPlayloadInfo)Request.Properties["playload"];
            try
            {
                //验证各字段合法性
                string msg = "";
                bool isOk = _instance.VerifyModel(model, ref msg);
                if (isOk)
                {
                    model.CreateName = playload.UserName;
                    model.CreateDate = DateTime.Now;
                    int res = _instance.AddGroup(model);
                    if (res > 0)
                    {
                        LogHelper.LogAction(playload, Constant.ActionEnum.Add, "新增用户组");
                        return JsonHelper.SuccessJson("新增成功");
                    }
                    else
                    {
                        LogHelper.LogWarn(playload, Constant.ActionEnum.Add, "新增用户组");
                        return JsonHelper.FailJson("新增失败");
                    }
                }
                else
                {
                    return JsonHelper.FailJson(msg);
                }

            }
            catch (Exception ex)
            {
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Add, "新增用户组");
                return JsonHelper.ErrorJson(ex.Message);
            }
        }

        /// <summary>
        /// 修改权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage UpdAccountGroup([FromBody] AccountGroup model)
        {
            playload = (JWTPlayloadInfo)Request.Properties["playload"];
            try
            {
                if (model.ID <= 0)
                {
                    return JsonHelper.ErrorJson("参数错误");
                }
                bool isExist = _instance.IsExist(" and ID=" + model.ID);
                if (!isExist)
                {
                    return JsonHelper.ErrorJson("不存在该数据");
                }
                //验证各字段合法性
                string msg = "";
                bool isOk = _instance.VerifyModel(model, ref msg);
                if (isOk)
                {
                    model.UpdName = playload.UserName;
                    model.UpdDate = DateTime.Now;
                    int res = _instance.UpdateGroup(model);
                    if (res > 0)
                    {
                        LogHelper.LogAction(playload, Constant.ActionEnum.Edit, "修改用户组");
                        return JsonHelper.SuccessJson("修改成功");
                    }
                    else
                    {
                        LogHelper.LogWarn(playload, Constant.ActionEnum.Edit, "修改用户组");
                        return JsonHelper.FailJson("修改失败");

                    }
                }
                else
                {
                    return JsonHelper.FailJson(msg);
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Edit, "修改用户组");
                return JsonHelper.ErrorJson(ex.Message);
            }
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage DelAccountGroup(string id)
        {
            playload = (JWTPlayloadInfo)Request.Properties["playload"];
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return JsonHelper.FailJson("参数不能为空！");
                }
                string[] ids = id.Split(',');

                decimal[] numbersArray = new decimal[ids.Length];
                for (int i = 0; i < ids.Length; i++)
                {
                    bool success = decimal.TryParse(ids[i], out decimal number); // 尝试将字符串转换为整数  
                    if (!success)
                    {
                        return JsonHelper.FailJson("参数错误！");
                    }
                }

                List<string> delId = new List<string>(ids);

                int result = _instance.DeleteGroups(delId, playload.UserName);
                if (result > 0)
                {
                    LogHelper.LogAction(playload, Constant.ActionEnum.Delete, "删除用户组");
                    return JsonHelper.SuccessJson("删除成功");
                }
                else
                {
                    LogHelper.LogWarn(playload, Constant.ActionEnum.Delete, "删除用户组");
                    return JsonHelper.SuccessJson("删除失败");
                }


            }
            catch (Exception ex)
            {
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Delete, "删除用户组");
                return JsonHelper.ErrorJson(ex.Message);
            }
        }
    }
}
