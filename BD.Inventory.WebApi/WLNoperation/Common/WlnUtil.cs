using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BD.Inventory.WebApi.WLNoperation.Common
{
    /// <summary>
    /// 万里牛工具类
    /// </summary>
    public static class WlnUtil
    {
        const string BaseUrl = "https://open-api.hupun.com/api";

        /// <summary>
        /// 获取根地址
        /// </summary>
        /// <returns></returns>
        public static string GetBase_Url()
        {
            return BaseUrl;
        }

        /// <summary>
        /// 将输入的字符串转换成适合在URL中传输的格式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string NetURLEncode(string str)
        {
            string encodedStr = Uri.EscapeDataString(str)
                .Replace("!", "%21")
                .Replace("'", "%27")
                .Replace("(", "%28")
                .Replace(")", "%29")
                .Replace("~", "%7E")
                .Replace("%20", "+");
            return encodedStr;
        }

        /// <summary>
        /// 签名算法
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="appKey"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static Dictionary<string, string> SignParameters(Dictionary<string, string> parameters, string appKey, string secret)
        {
            var sortedParams = new SortedDictionary<string, string>(parameters)
        {
            { "_app", appKey },
            { "_s", "" },
            { "_t", ToUnixTimeMilliseconds(DateTimeOffset.UtcNow) }
        };

            var queryBuilder = new StringBuilder();
            foreach (var pair in sortedParams)
            {
                queryBuilder.Append(pair.Key)
                            .Append("=")
                            .Append(NetURLEncode(pair.Value))
                            .Append("&");
            }

            queryBuilder.Length--; // Remove the last '&'

            var content = secret + queryBuilder + secret;
            var sign = Md5Hash(content);
            sortedParams.Add("_sign", sign);

            return new Dictionary<string, string>(sortedParams);
        }

        /// <summary>
        /// 获取时间戳，自1970年以来的秒数
        /// </summary>
        /// <param name="dateTimeOffset"></param>
        /// <returns></returns>
        public static string ToUnixTimeMilliseconds(this DateTimeOffset dateTimeOffset)
        {
            DateTimeOffset epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            return (dateTimeOffset - epoch).TotalMilliseconds.ToString();
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string Md5Hash(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                foreach (var t in hashBytes)
                {
                    sb.Append(t.ToString("x2"));
                }
                return sb.ToString();
            }
        }

    }
}