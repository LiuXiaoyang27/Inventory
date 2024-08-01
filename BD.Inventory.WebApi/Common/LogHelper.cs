using BD.Inventory.Common;
using BD.Inventory.Entities;
using log4net;
using System;
using System.Diagnostics;

namespace BD.Inventory.WebApi.Common
{
    /// <summary>
    /// 日志帮助类
    /// </summary>
    public class LogHelper : ILogHelper
    {
        public void Debug(object message)
        {
            LogManager.GetLogger(GetCurrentMethodFullName()).Debug(message);
        }

        public void Debug(object message, Exception ex)
        {
            LogManager.GetLogger(GetCurrentMethodFullName()).Debug(message, ex);
        }

        public void Error(object message)
        {
            LogManager.GetLogger(GetCurrentMethodFullName()).Error(message);
        }

        public void Error(object message, Exception ex)
        {
            LogManager.GetLogger(GetCurrentMethodFullName()).Error(message, ex);
        }

        public void Info(object message)
        {
            LogManager.GetLogger(GetCurrentMethodFullName()).Info(message);
        }

        public void Info(object message, Exception ex)
        {
            LogManager.GetLogger(GetCurrentMethodFullName()).Info(message, ex);
        }

        public void Warn(object message)
        {
            LogManager.GetLogger(GetCurrentMethodFullName()).Warn(message);
        }

        public void Warn(object message, Exception ex)
        {
            LogManager.GetLogger(GetCurrentMethodFullName()).Warn(message, ex);
        }

        private string GetCurrentMethodFullName()
        {
            try
            {
                StackFrame frame;
                string str2;
                int num = 2;
                StackTrace trace = new StackTrace();
                int length = trace.GetFrames().Length;
                do
                {
                    frame = trace.GetFrame(num++);
                    str2 = frame.GetMethod().DeclaringType.ToString();
                }
                while (str2.EndsWith("Exception") && (num < length));
                string name = frame.GetMethod().Name;
                return (str2 + "." + name);
            }
            catch
            {
                return null;
            }
        }

        #region 日志

        public static void LogAction(JWTPlayloadInfo playload, Constant.ActionEnum method, string action)
        {
            new LogHelper().Info(new LogContent(Utils.GetNextID(), playload.LoginIP, playload.UserName, action, method, Constant.StatusEnum.Success));
        }

        public static void LogWarn(JWTPlayloadInfo playload, Constant.ActionEnum method, string action)
        {
            new LogHelper().Warn(new LogContent(Utils.GetNextID(), playload.LoginIP, playload.UserName, action, method, Constant.StatusEnum.Fail));
        }

        public static void LogError(JWTPlayloadInfo playload, Exception ex, Constant.ActionEnum method, string action)
        {
            new LogHelper().Error(new LogContent(Utils.GetNextID(), playload.LoginIP, playload.UserName, $"{action}: {ex}", method, Constant.StatusEnum.Exception), ex);
        }

        #endregion 日志
    }

    public interface ILogHelper
    {
        void Debug(object message);

        void Debug(object message, Exception ex);

        void Error(object message);

        void Error(object message, Exception ex);

        void Info(object message);

        void Info(object message, Exception ex);

        void Warn(object message);

        void Warn(object message, Exception ex);
    }
}