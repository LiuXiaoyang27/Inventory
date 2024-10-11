using BD.Inventory.WebApi.WLNoperation.Common;
using BD.Inventory.WebApi.WLNoperation.Config;
using BD.Inventory.WebApi.WLNoperation.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using BD.Inventory.WebApi.Common;
using BD.Inventory.Entities;
using BD.Inventory.Common;
using BD.Inventory.Bll;
using System.Collections.Generic;
using BD.Inventory.Entities.DTO;
using System.Diagnostics;
using System.Linq;
using System;
using System.Threading;

namespace BD.Inventory.WebApi.WLNoperation
{
    /// <summary>
    /// 万里牛基本操作类
    /// </summary>
    public class WlnPublic
    {
        private static readonly HttpClient Client = new HttpClient();

        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1); // 控制并发请求数


        private readonly WlnERPBll _instance;

        JWTPlayloadInfo playload = new JWTPlayloadInfo { LoginIP = "0.0.0.0", UserName = "主机" };
        /// <summary>
        /// 构造函数
        /// </summary>
        public WlnPublic()
        {
            // 加载配置文件
            GetConfig.ResetConfig();
            _instance = WlnERPBll.Instance;
        }


        /// <summary>
        /// 查询商品
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetGoodsList(Param_Goods param)
        {
            var baseUrl = WlnUtil.GetBase_Url();
            var path = "/erp/goods/spec/open/query/goodswithspeclist";
            var url = $"{baseUrl}{path}";

            var parameters = ParamConversion.ConvertParam_GoodsToDictionary(param);

            // 包含签名的参数
            var data = WlnUtil.SignParameters(parameters, WlnConfig.appkey, WlnConfig.secret);

            var content = new FormUrlEncodedContent(data);

            try
            {
                var response = await Client.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error: {response.StatusCode}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                //dynamic deserializedContent = JsonConvert.DeserializeObject(responseBody);
                GoodsApiResponse res = JsonConvert.DeserializeObject<GoodsApiResponse>(responseBody);
                //return deserializedContent;
                return response;
            }
            catch (HttpRequestException e)
            {
                throw;
            }

        }

        /// <summary>
        /// 查询盘点单
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetCheckBillList(Param_InvCheckBill param)
        {
            var baseUrl = WlnUtil.GetBase_Url();
            var path = "/erp/open/inventory/inventorycheckbill/query";
            var url = $"{baseUrl}{path}";

            var parameters = ParamConversion.ConvertParam_CheckBillToDictionary(param);

            // 包含签名的参数
            var data = WlnUtil.SignParameters(parameters, WlnConfig.appkey, WlnConfig.secret);

            var content = new FormUrlEncodedContent(data);

            try
            {
                var response = await Client.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error: {response.StatusCode}");
                }

                //var responseBody = await response.Content.ReadAsStringAsync();
                //CheckBillApiResponse res = JsonConvert.DeserializeObject<CheckBillApiResponse>(responseBody);
                //return deserializedContent;
                return response;
            }
            catch (HttpRequestException e)
            {
                throw;
            }

        }

        /// <summary>
        /// 拉取商品信息
        /// </summary>
        /// <returns></returns>
        public async Task ImportGoods(InsertGoodsResult result)
        {
            Param_Goods param = new Param_Goods();
            var baseUrl = WlnUtil.GetBase_Url();
            var path = "/erp/goods/spec/open/query/goodswithspeclist";
            var url = $"{baseUrl}{path}";

            param.page = 0;
            param.limit = 200;
            param.all_status = true;
            param.modify_time = "2000-01-01 00:00:00";

            var parameters = ParamConversion.ConvertParam_GoodsToDictionary(param);
            try
            {
                // 删除原数据
                int old_res = _instance.DelOldGoods();
                if (old_res >= 0)
                {
                    // 当前页（循环用）
                    int pageindex = 0;
                    GoodsApiResponse res;
                    do
                    {
                        pageindex++;
                        res = new GoodsApiResponse();
                        parameters["page"] = pageindex.ToString();
                        // 包含签名的参数
                        var data = WlnUtil.SignParameters(parameters, WlnConfig.appkey, WlnConfig.secret);

                        var content = new FormUrlEncodedContent(data);

                        var response = await Client.PostAsync(url, content);
                        if (!response.IsSuccessStatusCode)
                        {
                            LogHelper.LogWarn(playload, Constant.ActionEnum.Copy, "拉取商品信息失败");
                            throw new HttpRequestException($"Error: {response.StatusCode}");
                        }

                        var responseBody = await response.Content.ReadAsStringAsync();
                        //dynamic deserializedContent = JsonConvert.DeserializeObject(responseBody);
                        res = JsonConvert.DeserializeObject<GoodsApiResponse>(responseBody);

                        if (res != null && res.data != null && res.data.Count > 0)
                        {
                            _instance.InsertGoodsInfo(res.data, result);

                        }


                    } while (res != null && res.data != null && res.data.Count > 0);
                }


                LogHelper.LogAction(playload, Constant.ActionEnum.Copy, "拉取商品信息到本地");

            }
            catch (HttpRequestException e)
            {
                LogHelper.LogError(playload, e, Constant.ActionEnum.Copy, "拉取商品信息");
                throw;
            }

        }

        /// <summary>
        /// 拉取盘点单和仓库信息
        /// </summary>
        /// <returns></returns>
        public async Task ImportCheckBillAndStorage(InsertCheckBillResult result)
        {
            Param_InvCheckBill param = new Param_InvCheckBill();
            var baseUrl = WlnUtil.GetBase_Url();
            var path = "/erp/open/inventory/inventorycheckbill/query";
            var url = $"{baseUrl}{path}";

            param.page = 0;
            param.limit = 100;
            param.create_time = "2010-01-01";
            //param.create_end_time = create_end_date;

            var parameters = ParamConversion.ConvertParam_CheckBillToDictionary(param);
            try
            {
                // 当前页（循环用）
                int pageindex = 0;
                CheckBillApiResponse res;
                do
                {
                    pageindex++;
                    res = new CheckBillApiResponse();
                    parameters["page"] = pageindex.ToString();
                    // 包含签名的参数
                    var data = WlnUtil.SignParameters(parameters, WlnConfig.appkey, WlnConfig.secret);

                    var content = new FormUrlEncodedContent(data);

                    var response = await Client.PostAsync(url, content);
                    if (!response.IsSuccessStatusCode)
                    {
                        LogHelper.LogWarn(playload, Constant.ActionEnum.Copy, "拉取盘点单信息失败");
                        throw new HttpRequestException($"Error: {response.StatusCode}");
                    }

                    var responseBody = await response.Content.ReadAsStringAsync();
                    //dynamic deserializedContent = JsonConvert.DeserializeObject(responseBody);
                    res = JsonConvert.DeserializeObject<CheckBillApiResponse>(responseBody);

                    if (res != null && res.data != null && res.data.Count > 0)
                    {
                        _instance.InsertCheckBillInfo(res.data, result);

                    }


                } while (res != null && res.data != null && res.data.Count > 0);

                LogHelper.LogAction(playload, Constant.ActionEnum.Copy, "拉取盘点单信息到本地");

                //return result;
            }
            catch (HttpRequestException e)
            {
                LogHelper.LogError(playload, e, Constant.ActionEnum.Copy, "拉取盘点单信息");
                throw;
            }

        }

        /// <summary>
        /// 添加增量盘点单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<CheckBill_AddApiResponse> AddCheckBill(Param_InvCheckBill_Add param)
        {
            var baseUrl = WlnUtil.GetBase_Url();
            var path = "/erp/open/inventory/inventorycheckbill/add";
            var url = $"{baseUrl}{path}";

            try
            {
                var parameters = ParamConversion.ConvertParam_CheckBill_AddToDictionary(param);
                CheckBill_AddApiResponse res = new CheckBill_AddApiResponse();

                // 包含签名的参数
                var data = WlnUtil.SignParameters(parameters, WlnConfig.appkey, WlnConfig.secret);

                var content = new FormUrlEncodedContent(data);

                var response = await Client.PostAsync(url, content);
                //if (!response.IsSuccessStatusCode)
                //{
                //    LogHelper.LogWarn(playload, Constant.ActionEnum.Add, "添加增量盘点单信息异常");
                //    throw new HttpRequestException($"Error: {response.StatusCode}");
                //}

                var responseBody = await response.Content.ReadAsStringAsync();
                ////dynamic deserializedContent = JsonConvert.DeserializeObject(responseBody);
                res = JsonConvert.DeserializeObject<CheckBill_AddApiResponse>(responseBody);

                LogHelper.LogAction(playload, Constant.ActionEnum.Add, "添加增量盘点单");
                return res;

            }
            catch (HttpRequestException e)
            {
                LogHelper.LogError(playload, e, Constant.ActionEnum.Copy, "添加增量盘点单");
                throw;
            }

        }        

        /// <summary>
        /// 根据商品类别查询库存
        /// </summary>
        /// <param name="goods_list"></param>
        /// <param name="storage_code"></param>
        /// <param name="storage_name"></param>
        /// <returns></returns>
        public async Task<List<InvInfo>> GetGoodsInvInfobyCategory(List<GoodsDTO> goods_list, string storage_code, string storage_name)
        {
            var baseUrl = WlnUtil.GetBase_Url();
            var path = "/erp/open/inventory/items/get/by/modifytimev2";
            var url = $"{baseUrl}{path}";

            var resList = new List<InvInfo>();

            var stopwatch = Stopwatch.StartNew(); // 开始计时

            // 手动分组，每组最多20个
            for (int i = 0; i < goods_list.Count; i += 20)
            {
                var group = goods_list.Skip(i).Take(20).Select(g => g.barcode).ToList();

                var p_wln = new Param_InvInfo
                {
                    article_number = "",
                    bar_code = string.Join(",", group),
                    sku_code = "", // 如果需要，可以设置 sku_code
                    storage_code = storage_code
                };

                var parameters = ParamConversion.ConvertParam_InvInfoToDictionary(p_wln);
                var data = WlnUtil.SignParameters(parameters, WlnConfig.appkey, WlnConfig.secret);
                var content = new FormUrlEncodedContent(data);

                try
                {
                    using (var response = await Client.PostAsync(url, content))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var responseBody = await response.Content.ReadAsStringAsync();
                            var res = JsonConvert.DeserializeObject<InvInfoApiResponse>(responseBody);

                            if (res?.data != null)
                            {
                                foreach (var model in res.data)
                                {
                                    if (model.quantity > 0) // 根据需要的条件进行筛选
                                    {
                                        model.storage_code = storage_code;
                                        model.storage_name = storage_name;
                                        resList.Add(model);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.LogError(playload, ex, Constant.ActionEnum.Copy, "请求失败(根据类别)");
                }
            }

            stopwatch.Stop(); // 停止计时
            LogHelper.LogAction(playload, Constant.ActionEnum.Copy, $"拉取商品库存信息(根据类别)-{resList.Count}条数据;【执行时间：{stopwatch.ElapsedMilliseconds}ms】");
            return resList;
        }
        
        /// <summary>
        /// 指定仓库拉取库存信息（Modifytime查询，循环page）
        /// </summary>
        /// <returns></returns>
        public async Task<List<InvInfo>> GetGoodsInvInfobyModifyTime(Param_InvInfo param)
        {
            var baseUrl = WlnUtil.GetBase_Url();
            var path = "/erp/open/inventory/items/get/by/modifytimev2";
            var url = $"{baseUrl}{path}";

            var parameters = ParamConversion.ConvertParam_InvInfoToDictionary(param);
            try
            {
                List<InvInfo> resList = new List<InvInfo>();
                // 当前页（循环用）
                int pageindex = 0;
                InvInfoApiResponse res;
                do
                {
                    pageindex++;
                    res = new InvInfoApiResponse();
                    parameters["page_no"] = pageindex.ToString();
                    // 包含签名的参数
                    var data = WlnUtil.SignParameters(parameters, WlnConfig.appkey, WlnConfig.secret);

                    var content = new FormUrlEncodedContent(data);

                    var response = await Client.PostAsync(url, content);
                    //if (!response.IsSuccessStatusCode)
                    //{
                    //    LogHelper.LogWarn(playload, Constant.ActionEnum.Copy, "拉取商品信息失败");
                    //    throw new HttpRequestException($"Error: {response.StatusCode}");
                    //}

                    var responseBody = await response.Content.ReadAsStringAsync();
                    //dynamic deserializedContent = JsonConvert.DeserializeObject(responseBody);
                    res = JsonConvert.DeserializeObject<InvInfoApiResponse>(responseBody);

                    if (res != null && res.data != null && res.data.Count > 0)
                    {
                        resList.AddRange(res.data);
                        //_instance.InsertGoodsInfo(res.data, result);

                    }


                } while (res != null && res.data != null && res.data.Count > 0);

                resList.ForEach(p => { p.storage_code = param.storage_code; p.storage_name = param.storage_name; });

                LogHelper.LogAction(playload, Constant.ActionEnum.Copy, "拉取商品库存信息(根据仓库拉取所有)");
                return resList;


            }
            catch (System.Exception e)
            {
                LogHelper.LogError(playload, e, Constant.ActionEnum.Copy, "拉取商品库存信息(根据仓库拉取所有)");
                throw;
            }

        }

        /// <summary>
        /// 指定仓库与sku查询库存信息（不需要循环page_no）
        /// </summary>
        /// <returns></returns>
        public async Task<InvInfo> GetGoodsInvInfobySku(Param_InvInfo param)
        {
            var baseUrl = WlnUtil.GetBase_Url();
            var path = "/erp/open/inventory/items/get/by/modifytimev2";
            var url = $"{baseUrl}{path}";

            var parameters = ParamConversion.ConvertParam_InvInfoToDictionary(param);
            try
            {

                InvInfoApiResponse res = new InvInfoApiResponse();
                // 包含签名的参数
                var data = WlnUtil.SignParameters(parameters, WlnConfig.appkey, WlnConfig.secret);

                var content = new FormUrlEncodedContent(data);

                var response = await Client.PostAsync(url, content);
                //if (!response.IsSuccessStatusCode)
                //{
                //    LogHelper.LogWarn(playload, Constant.ActionEnum.Copy, "拉取商品信息失败");
                //    throw new HttpRequestException($"Error: {response.StatusCode}");
                //}

                var responseBody = await response.Content.ReadAsStringAsync();
                //dynamic deserializedContent = JsonConvert.DeserializeObject(responseBody);
                res = JsonConvert.DeserializeObject<InvInfoApiResponse>(responseBody);
                InvInfo model = new InvInfo();

                if (res != null && res.data != null && res.data.Count > 0)
                {
                    model = res.data[0];
                    model.storage_code = param.storage_code;
                    model.storage_name = param.storage_name;
                }


                LogHelper.LogAction(playload, Constant.ActionEnum.Copy, "拉取商品库存信息（指定商品）");
                return model;


            }
            catch (System.Exception e)
            {
                LogHelper.LogError(playload, e, Constant.ActionEnum.Copy, "拉取商品库存信息（指定商品）");
                throw;
            }

        }

        /// <summary>
        /// 拉取仓库信息
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetStorageInfo()
        {
            var baseUrl = WlnUtil.GetBase_Url();
            var path = "/erp/base/storage/query";
            var url = $"{baseUrl}{path}";

            var parameters = new Dictionary<string, string>();

            // 添加基本属性
            parameters.Add("page_no", "1");
            parameters.Add("page_size", "100");

            try
            {

                // 包含签名的参数
                var data = WlnUtil.SignParameters(parameters, WlnConfig.appkey, WlnConfig.secret);

                var content = new FormUrlEncodedContent(data);

                var response = await Client.PostAsync(url, content);
                //if (!response.IsSuccessStatusCode)
                //{
                //    LogHelper.LogWarn(playload, Constant.ActionEnum.Copy, "拉取商品信息失败");
                //    throw new HttpRequestException($"Error: {response.StatusCode}");
                //}



                LogHelper.LogAction(playload, Constant.ActionEnum.Copy, "获取仓库信息");
                return response;


            }
            catch (System.Exception e)
            {
                LogHelper.LogError(playload, e, Constant.ActionEnum.Copy, "获取仓库信息");
                throw;
            }

        }

        /// <summary>
        /// 获取商品分类
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetCatagorypage()
        {
            var baseUrl = WlnUtil.GetBase_Url();
            var path = "/erp/goods/catagorypage/query/v2";
            var url = $"{baseUrl}{path}";

            var parameters = new Dictionary<string, string>();

            // 添加基本属性
            parameters.Add("page", "1");
            parameters.Add("limit", "50");

            try
            {

                // 包含签名的参数
                var data = WlnUtil.SignParameters(parameters, WlnConfig.appkey, WlnConfig.secret);

                var content = new FormUrlEncodedContent(data);

                var response = await Client.PostAsync(url, content);

                LogHelper.LogAction(playload, Constant.ActionEnum.Copy, "获取商品分类信息");
                return response;


            }
            catch (System.Exception e)
            {
                LogHelper.LogError(playload, e, Constant.ActionEnum.Copy, "获取商品分类信息");
                throw;
            }

        }

    }
}