using BD.Inventory.Common;
using log4net.Layout;
using log4net.Layout.Pattern;
using log4net.Repository;
using log4net.Util;
using System;
using System.IO;
using System.Reflection;

namespace BD.Inventory.WebApi.Common
{
    /// <summary>
    /// 日志内容
    /// </summary>
    public class LogContent
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="loginIP">登录ip</param>
        /// <param name="userName">用户名</param>
        /// <param name="description">描述</param>
        /// <param name="action">动作事件</param>
        /// <param name="status">操作状态</param>
        public LogContent(string id, string loginIP, string userName, string description, Constant.ActionEnum action, Constant.StatusEnum status)
        {
            this.ID = id;
            this.LoginIP = loginIP;
            this.UserName = userName;
            this.Method = Constant.GetActionDescription(action);
            this.Message = description;
            this.Status = Constant.GetActionDescription(status);
        }

        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 访问IP
        /// </summary>
        public string LoginIP { get; set; }

        /// <summary>
        /// 系统登陆用户
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 日志描述信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 操作状态
        /// </summary>
        public string Status { get; set; }

    }
    public class CustomLayout : PatternLayout
    {
        public CustomLayout()
        {
            this.AddConverter("property", typeof(LogInfoPatternConverter));
        }
    }
    public class LogInfoPatternConverter : PatternLayoutConverter
    {

        protected override void Convert(System.IO.TextWriter writer, log4net.Core.LoggingEvent loggingEvent)
        {
            if (Option != null)
            {
                // Write the value for the specified key
                WriteObject(writer, loggingEvent.Repository, LookupProperty(Option, loggingEvent));
            }
            else
            {
                // Write all the key value pairs
                WriteDictionary(writer, loggingEvent.Repository, loggingEvent.GetProperties());
            }
        }

        private void WriteDictionary(TextWriter writer, ILoggerRepository repository, PropertiesDictionary propertiesDictionary)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 通过反射获取传入的日志对象的某个属性的值
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private object LookupProperty(string property, log4net.Core.LoggingEvent loggingEvent)
        {
            object propertyValue = string.Empty;
            PropertyInfo propertyInfo = loggingEvent.MessageObject.GetType().GetProperty(property);
            if (propertyInfo != null)
                propertyValue = propertyInfo.GetValue(loggingEvent.MessageObject, null);
            return propertyValue;
        }
    }

}