using BD.Inventory.Entities.DTO;
using BD.Inventory.WebApi.WLNoperation.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BD.Inventory.WebApi.WLNoperation.Common
{
    /// <summary>
    /// 参数转换
    /// </summary>
    public class ParamConversion
    {
        /// <summary>
        /// 将商品类参数放到字典中
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ConvertParam_GoodsToDictionary(Param_Goods param)
        {
            var parameters = new Dictionary<string, string>();

            // 添加基本属性
            parameters.Add("limit", param.limit.ToString());
            parameters.Add("page", param.page.ToString());
            parameters.Add("all_status", param.all_status.ToString().ToLower());
            if (!string.IsNullOrEmpty(param.bar_code))
            {
                parameters.Add("bar_code", HttpUtility.UrlEncode(param.bar_code));
            }
            if (!string.IsNullOrEmpty(param.end_time))
            {
                parameters.Add("end_time", param.end_time);
            }
            if (!string.IsNullOrEmpty(param.item_code))
            {
                parameters.Add("item_code", param.item_code);
            }
            if (!string.IsNullOrEmpty(param.modify_time))
            {
                parameters.Add("modify_time", param.modify_time);
            }

            parameters.Add("need_properties", param.need_properties.ToString().ToLower());

            if (!string.IsNullOrEmpty(param.spec_code))
            {
                parameters.Add("spec_code", param.spec_code);
            }

            // 处理复杂对象 goods_query_extend
            if (param.goods_query_extend != null)
            {
                string serializedExtend = JsonConvert.SerializeObject(param.goods_query_extend);
                parameters.Add("goods_query_extend", serializedExtend);
            }

            return parameters;
        }

        /// <summary>
        /// 将盘点单参数放到字典中
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ConvertParam_CheckBillToDictionary(Param_InvCheckBill param)
        {
            var parameters = new Dictionary<string, string>();

            // 添加基本属性
            parameters.Add("limit", param.limit.ToString());
            parameters.Add("page", param.page.ToString());
            if (!string.IsNullOrEmpty(param.bill_code))
            {
                parameters.Add("bill_code", param.bill_code);
            }
            if (param.bill_status != -1)
            {
                parameters.Add("bill_status", param.bill_status.ToString());
            }
            if (!string.IsNullOrEmpty(param.create_end_time))
            {
                parameters.Add("create_end_time", param.create_end_time);
            }
            if (!string.IsNullOrEmpty(param.create_time))
            {
                parameters.Add("create_time", param.create_time);
            }
            if (!string.IsNullOrEmpty(param.storage_code))
            {
                parameters.Add("storage_code", param.storage_code);
            }

            return parameters;
        }

        /// <summary>
        /// 将增量盘点单参数放到字典中
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ConvertParam_CheckBill_AddToDictionary(Param_InvCheckBill_Add param)
        {
            var parameters = new Dictionary<string, string>
                {
                    { "open_inventory_check_bill_request", string.Empty }
                };

            if (param != null)
            {
                string serializedExtend = JsonConvert.SerializeObject(param,Formatting.None, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy() // 如果API期望camelCase命名，使用这个
                    }
                });
                parameters["open_inventory_check_bill_request"] = serializedExtend;
            }

            return parameters;
        }

        /// <summary>
        /// 将数据转换为万里牛平台参数
        /// </summary>
        /// <param name="DtoModel"></param>
        /// <returns></returns>
        public static Param_InvCheckBill_Add DTO2WNLparam(InvCheckDTO DtoModel)
        {
            Param_InvCheckBill_Add param = new Param_InvCheckBill_Add();
            param.bill_code = DtoModel.bill_code;
            param.storage_code = DtoModel.storage_code;

            List<Param_InvCheckBill_Add_Details> details = new List<Param_InvCheckBill_Add_Details>();
            foreach (var item in DtoModel.details)
            {
                //public string batch_no { get; set; } = "";

                //public string expired_date { get; set; } = "";

                //public double nums { get; set; } = 0;
                //public string spec_code { get; set; } = "";
                //public int stock_type { get; set; } = 0;
                Param_InvCheckBill_Add_Details d_model = new Param_InvCheckBill_Add_Details();
                d_model.batch_date = item.batch_date == DateTime.MinValue ? "" : item.batch_date.ToString("yyyy-MM-dd HH:mm:ss");
                d_model.batch_no = item.batch_code;
                d_model.expired_date = item.expiry_date == DateTime.MinValue ? "" : item.expiry_date.ToString("yyyy-MM-dd HH:mm:ss");
                d_model.nums = item.isChecked;
                d_model.spec_code = item.spec_code;
                d_model.stock_type = item.stock_type;

                details.Add(d_model);
            }
            param.details = details;

            return param;

        }

        /// <summary>
        /// 将库存信息参数放到字典中
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ConvertParam_InvInfoToDictionary(Param_InvInfo param)
        {
            var parameters = new Dictionary<string, string>();

            // 添加基本属性
            parameters.Add("page_no", param.page_no.ToString());
            parameters.Add("page_size", param.page_size.ToString());            
            if (!string.IsNullOrEmpty(param.article_number))
            {
                parameters.Add("article_number", param.article_number.ToString());
            }
            if (!string.IsNullOrEmpty(param.bar_code))
            {
                parameters.Add("bar_code", param.bar_code);
            }
            if (!string.IsNullOrEmpty(param.modify_time))
            {
                parameters.Add("modify_time", param.modify_time);
            }

            if (!string.IsNullOrEmpty(param.sku_code))
            {
                parameters.Add("sku_code", param.sku_code);
            }

            if (!string.IsNullOrEmpty(param.storage_code))
            {
                parameters.Add("storage", param.storage_code);
            }



            return parameters;
        }

    }
}