using BD.Inventory.Bll;
using BD.Inventory.Common;
using BD.Inventory.Entities;
using BD.Inventory.WebApi.App_Start;
using BD.Inventory.WebApi.Common;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace BD.Inventory.WebApi.Controllers.PC
{
    /// <summary>
    /// 用户登录
    /// </summary>
    [ControllerGroup("Login", "PC端-用户登录")]
    public class LoginController : ApiController
    {
        private readonly AccountUserBll user_instance;
        private readonly AccountGroupBll group_instance;
        private readonly MenuBll menu_instance;
        /// <summary>
        /// 构造方法
        /// </summary>
        public LoginController()
        {
            user_instance = AccountUserBll.Instance;
            group_instance = AccountGroupBll.Instance;
            menu_instance = MenuBll.Instance;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userNo">账号</param>
        /// <param name="passWord">密码</param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Login(string userNo, string passWord)
        {
            JWTPlayloadInfo playload = new JWTPlayloadInfo();

            try
            {

                AccountUser model = user_instance.CheckAccount(userNo, passWord, out string msg);

                if (model != null)
                {
                    // 验证密码
                    bool isValid_password = BCrypt.Net.BCrypt.Verify(passWord, model.PassWord);
                    if (isValid_password)
                    {
                        if (string.IsNullOrEmpty(model.GroupNo))
                        {
                            msg = "该账户未设置权限！";
                            return JsonHelper1.FailJson(this, msg);
                        }
                        AccountGroup group = group_instance.GetModelByID(Convert.ToDecimal(model.GroupNo));
                        // 获取该用户的PC授权菜单
                        List<Menu> menuListPC = new List<Menu>();

                        // 获取该用户的SC授权菜单
                        List<Menu> menuListSC = new List<Menu>();

                        if (group != null)
                        {
                            //根据用户组菜单获取菜单集合

                            // PC授权菜单
                            string[] menuPC = group.GroupMenuPC.Trim().Split(',');
                            string strMenuPC = "";
                            // PC菜单中的增删改查
                            string[] menuZSGC = group.GroupZSGC.Trim().Split(',');
                            string strMenuZSGC = "";

                            // SC授权菜单
                            string[] menuSC = group.GroupMenuSC.Trim().Split(',');
                            string strMenuSC = "";

                            foreach (var item in menuPC)
                            {
                                if (string.IsNullOrEmpty(strMenuPC))
                                {
                                    strMenuPC = "'" + item + "'";
                                }
                                else
                                {
                                    strMenuPC += ",'" + item + "'";
                                }
                            }
                            foreach (var item in menuZSGC)
                            {
                                if (string.IsNullOrEmpty(strMenuZSGC))
                                {
                                    strMenuZSGC = "'" + item + "'";
                                }
                                else
                                {
                                    strMenuZSGC += ",'" + item + "'";
                                }
                            }

                            foreach (var item in menuSC)
                            {
                                if (string.IsNullOrEmpty(strMenuSC))
                                {
                                    strMenuSC = "'" + item + "'";
                                }
                                else
                                {
                                    strMenuSC += ",'" + item + "'";
                                }
                            }

                            menuListPC = menu_instance.GetListByMenuName(strMenuPC, strMenuZSGC);

                            menuListSC = menu_instance.GetListSCByMenuName(strMenuSC);

                        }
                        else
                        {
                            LogHelper.LogWarn(playload, Constant.ActionEnum.Login, "该用户未设置权限");
                        }
                        //生成token
                        //设置过期时间 
                        double seconds = 43200; // 过期秒数(12小时)
                        double exp = (DateTime.UtcNow.AddSeconds(seconds) - new DateTime(1970, 1, 1)).TotalSeconds;

                        playload = new JWTPlayloadInfo
                        {
                            UserNo = model.UserNo,
                            UserName = model.UserName,
                            GroupNo = model.GroupNo,
                            LoginIP = Utils.GetClientIP(),
                            exp = exp
                        };
                        var token = JWTHelper.GetToken(playload);

                        // 添加日志
                        LogHelper.LogAction(playload, Constant.ActionEnum.Login, "PC端-用户登录");
                        //return JsonHelper.SuccessLogin("登录成功！", JsonHelper.ModelToJObject(model), menu_instance.GetTreeJson(menuListPC), menu_instance.GetTreeJson(menuListSC), token);
                        return JsonHelper1.SuccessLogin(this, "登录成功！", JsonHelper.ModelToJObject(model), menu_instance.GetTreeJson(menuListPC), menu_instance.GetTreeJson(menuListSC), token);

                    }
                    else
                    {
                        msg = "密码错误！";
                        return JsonHelper1.FailJson(this, msg);
                    }


                }
                else
                {
                    return JsonHelper1.FailJson(this, msg);
                }
            }
            catch (Exception ex)
            {
                // 异常日志
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Login, "PC端-用户登录");
                return JsonHelper1.ErrorJson(this, ex.Message);
            }

        }
    }
}
