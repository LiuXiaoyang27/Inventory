using BD.Inventory.Bll;
using BD.Inventory.Common;
using BD.Inventory.Entities;
using BD.Inventory.Entities.DTO;
using BD.Inventory.WebApi.App_Start;
using BD.Inventory.WebApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace BD.Inventory.WebApi.Controllers.PC
{

    /// <summary>
    /// PC-商品管理
    /// </summary>
    [ControllerGroup("Goods", "PC端-商品管理")]
    public class GoodsController : BaseController
    {
        private JWTPlayloadInfo playload;
        private readonly GoodsBll _instance;
        /// <summary>
        /// 构造方法
        /// </summary>
        public GoodsController()
        {
            _instance = GoodsBll.Instance;
        }

        /// <summary>
        /// 查询商品
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="goods_code">商品编码</param>
        /// <param name="goods_name">商品名称</param>
        /// <param name="spec2">规格</param>
        /// <param name="barcode">条码</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage SelGoods(int pageIndex = 1, int pageSize = 10, string goods_code = "", string goods_name = "", string spec2 = "", string barcode = "")
        {
            playload = (JWTPlayloadInfo)Request.Properties["playload"];
            try
            {
                var strWhere = new StringBuilder("");
                if (!string.IsNullOrEmpty(goods_code))
                {
                    strWhere.Append($" and t1.goods_code like '%{goods_code}%'");
                }
                if (!string.IsNullOrEmpty(goods_name))
                {
                    strWhere.Append($" and t1.goods_name like '%{goods_name}%'");
                }
                if (!string.IsNullOrEmpty(spec2))
                {
                    strWhere.Append($" and t2.spec2 like '%{spec2}%'");
                }
                if (!string.IsNullOrEmpty(barcode))
                {
                    strWhere.Append($" and t2.barcode like '%{barcode}%'");
                }

                var dt = _instance.GetPageList(pageSize, pageIndex, strWhere.ToString(), "t1.modify_time DESC", out int records);
                int totalPages = PageHelper.GetPageCount(pageSize, records);
                return JsonHelper.SuccessJson(CommonOperation.DataTable2JArray(dt), pageIndex, totalPages, records);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Show, "分页查询商品");
                return JsonHelper.ErrorJson(ex.Message);
            }
        }

        /// <summary>
        /// 查询详情
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="barcode">商品条码</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage SelDetail(string barcode, int pageIndex = 1, int pageSize = 10)
        {
            playload = (JWTPlayloadInfo)Request.Properties["playload"];
            if (string.IsNullOrEmpty(barcode))
            {
                return JsonHelper.FailJson("商品条码不能为空！");
            }
            try
            {
                var dt = _instance.GetDetail(barcode, pageSize, pageIndex, "modify_time", out int records);
                int totalPages = PageHelper.GetPageCount(pageSize, records);
                return JsonHelper.SuccessJson(CommonOperation.DataTable2JArray(dt), pageIndex, totalPages, records);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Show, "查询详情");
                return JsonHelper.ErrorJson(ex.Message);
            }

        }

        /// <summary>
        /// 删除详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage DelDetail(string id)
        {
            playload = (JWTPlayloadInfo)Request.Properties["playload"];
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return JsonHelper.ErrorJson("参数错误");
                }
                bool IsExist = _instance.IsExist("BindRFID", $"id='{id}'");
                if (!IsExist)
                {
                    return JsonHelper.ErrorJson("数据不存在");
                }

                bool result = _instance.DelDetail(id);
                if (result)
                {
                    LogHelper.LogAction(playload, Constant.ActionEnum.Delete, "删除详情");
                    return JsonHelper.SuccessJson("删除成功");
                }
                else
                {
                    LogHelper.LogWarn(playload, Constant.ActionEnum.Delete, "删除详情");
                    return JsonHelper.FailJson("删除失败");
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Delete, "删除详情");
                return JsonHelper.ErrorJson(ex.Message);
            }

        }

        /// <summary>
        /// 条码绑定
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage BindingCode([FromBody] BindRFIDDTO model)
        {
            playload = (JWTPlayloadInfo)Request.Properties["playload"];
            try
            {
                if (string.IsNullOrEmpty(model.goods_code))
                {
                    return JsonHelper.ErrorJson("商品编码不能为空");
                }
                if (string.IsNullOrEmpty(model.spec_code))
                {
                    return JsonHelper.ErrorJson("规格编码不能为空");
                }
                if (string.IsNullOrEmpty(model.barcode))
                {
                    return JsonHelper.ErrorJson("商品条码不能为空");
                }
                if (string.IsNullOrEmpty(model.RFID))
                {
                    return JsonHelper.ErrorJson("绑定RFID不能为空");
                }

                bool IsExist = _instance.IsExist("BindRFID", $"RFID='{model.RFID}'");

                if (IsExist)
                {
                    return JsonHelper.ErrorJson("RFID已存在");
                }
                int res = _instance.BindingCode(model);
                if (res > 0)
                {
                    LogHelper.LogAction(playload, Constant.ActionEnum.Add, "条码绑定");
                    return JsonHelper.SuccessJson("绑定成功");
                }
                else
                {
                    LogHelper.LogWarn(playload, Constant.ActionEnum.Add, "条码绑定");
                    return JsonHelper.FailJson("绑定失败");

                }

            }
            catch (Exception ex)
            {
                LogHelper.LogError(playload, ex, Constant.ActionEnum.Add, "条码绑定");
                return JsonHelper.ErrorJson(ex.Message);
            }
        }

    }
}
