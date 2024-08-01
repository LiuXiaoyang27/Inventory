using BD.Inventory.Entities;
using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using JWT.Serializers;
using System;

namespace BD.Inventory.WebApi.Common
{
    /// <summary>
    /// JWT操作帮助类
    /// </summary>
    public class JWTHelper
    {
        /// <summary>
        /// 签发token
        /// </summary>
        /// <param name="playload">载荷</param>
        /// <returns></returns>
        public static string GetToken(JWTPlayloadInfo playload)
        {

            string token = string.Empty;
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            //获取私钥
            string secret = GetSecret();
            if (playload.UserNo != "" && playload.UserNo != null)
            {
                token = encoder.Encode(playload, secret);
            }

            return token;
        }

        /// <summary>
        /// token校验
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static JWTPlayloadInfo CheckToken(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token)) return null;

                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                var algorithm = new HMACSHA256Algorithm();

                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);

                //获取私钥
                string secret = GetSecret();
                JWTPlayloadInfo playloadInfo = decoder.DecodeToObject<JWTPlayloadInfo>(token, secret, true);
                // 获取当前用户
                //Users user = CommonDB.GetLoginUser(playloadInfo);
                //if (playloadInfo.iat < user.UpdatePwdTime)
                //{
                //    throw new TokenExpiredException("token失效");
                //}
                return playloadInfo;
            }
            catch (TokenExpiredException ex)
            {
                string msg = ex.Message;
                if (!msg.Equals("token失效"))
                {
                    msg = "token过期";
                }
                JWTPlayloadInfo playloadInfo = new JWTPlayloadInfo { Status = "ERROR", Message = msg };
                return playloadInfo;
            }
            catch (SignatureVerificationException)
            {
                JWTPlayloadInfo playloadInfo = new JWTPlayloadInfo { Status = "ERROR", Message = "token被篡改" };
                return playloadInfo;
            }
            catch (Exception ex)
            {
                JWTPlayloadInfo playloadInfo = new JWTPlayloadInfo { Status = "ERROR", Message = ex.Message };
                return playloadInfo;
            }

        }

        /// <summary>
        /// 获取私钥
        /// </summary>
        /// <returns></returns>
        private static string GetSecret()
        {
            //TODO 从文件中去读真正的私钥
            return "BDSPEECH05LianBoDi24570Hui2She3Qu";
        }
    }
}