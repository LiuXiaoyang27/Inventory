using System;
using System.Configuration;

namespace BD.Inventory.WebApi.WLNoperation.Config
{
    /// <summary>
    /// 获取配置文件配置
    /// </summary>
    public class GetConfig
    {
        private static string GetIndexConfigValue(string key)
        {
            try
            {
                string indexConfigPath = "WlnERP.config";

                ExeConfigurationFileMap ecf = new ExeConfigurationFileMap();
                ecf.ExeConfigFilename = AppDomain.CurrentDomain.BaseDirectory + indexConfigPath;
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(ecf, ConfigurationUserLevel.None);
                return config.AppSettings.Settings[key].Value;
            }
            catch (Exception)
            {
                // 没有文件，读取异常不做处理
                return null;
            }
        }

        /// <summary>
        /// 重置配置文件
        /// </summary>
        public static void ResetConfig()
        {
            string appkey = GetIndexConfigValue("AppKey");
            string secret = GetIndexConfigValue("Secret");

            if (!string.IsNullOrWhiteSpace(appkey))
            {
                WlnConfig.appkey = appkey;
            }

            if (!string.IsNullOrWhiteSpace(secret))
            {
                WlnConfig.secret = secret;
            }

        }

    }
}