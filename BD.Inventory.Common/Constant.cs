using System;
using System.ComponentModel;
using System.Reflection;

namespace BD.Inventory.Common
{
    /// <summary>
    /// 常数类
    /// </summary>
    public class Constant
    {
        /// <summary>
        /// 统一管理操作枚举
        /// </summary>
        public enum ActionEnum
        {
            /// <summary>
            /// 显示
            /// </summary>
            [Description("Show")]
            Show,

            /// <summary>
            /// 查看
            /// </summary>
            [Description("View")]
            View,

            /// <summary>
            /// 添加
            /// </summary>
            [Description("Add")]
            Add,

            /// <summary>
            /// 修改
            /// </summary>
            [Description("Edit")]
            Edit,

            /// <summary>
            /// 删除
            /// </summary>
            [Description("Delete")]
            Delete,

            /// <summary>
            /// 审核
            /// </summary>
            [Description("Audit")]
            Audit,

            /// <summary>
            /// 反审核
            /// </summary>
            [Description("ReAudit")]
            ReAudit,

            /// <summary>
            /// 回复
            /// </summary>
            [Description("Reply")]
            Reply,

            /// <summary>
            /// 确认
            /// </summary>
            [Description("Confirm")]
            Confirm,

            /// <summary>
            /// 取消
            /// </summary>
            [Description("Cancel")]
            Cancel,

            /// <summary>
            /// 作废
            /// </summary>
            [Description("Invalid")]
            Invalid,

            /// <summary>
            /// 生成
            /// </summary>
            [Description("Build")]
            Build,

            /// <summary>
            /// 安装
            /// </summary>
            [Description("Instal")]
            Instal,

            /// <summary>
            /// 卸载
            /// </summary>
            [Description("UnLoad")]
            UnLoad,

            /// <summary>
            /// 登录
            /// </summary>
            [Description("Login")]
            Login,

            /// <summary>
            /// 备份
            /// </summary>
            [Description("Back")]
            Back,

            /// <summary>
            /// 还原
            /// </summary>
            [Description("Restore")]
            Restore,

            /// <summary>
            /// 替换
            /// </summary>
            [Description("Replace")]
            Replace,

            /// <summary>
            /// 复制
            /// </summary>
            [Description("Copy")]
            Copy,

            /// <summary>
            /// 下载
            /// </summary>
            [Description("Download")]
            Download,

            /// <summary>
            /// 导出
            /// </summary>
            [Description("Export")]
            Export,

            /// <summary>
            /// 导入
            /// </summary>
            [Description("Import")]
            Import,

            /// <summary>
            /// 打印
            /// </summary>
            [Description("Print")]
            Print,

            /// <summary>
            /// 启用/禁用
            /// </summary>
            [Description("Enabled")]
            Enabled,

            /// <summary>
            /// 上传
            /// </summary>
            [Description("Upload")]
            Upload,

            /// <summary>
            /// 定时任务
            /// </summary>
            [Description("Upload")]
            Quartz,
        }

        /// <summary>
        /// 操作状态枚举
        /// </summary>
        public enum StatusEnum
        {
            /// <summary>
            /// 操作成功
            /// </summary>
            [Description("Success")]
            Success,

            /// <summary>
            /// 操作失败
            /// </summary>
            [Description("Fail")]
            Fail,

            /// <summary>
            /// 操作异常
            /// </summary>
            [Description("Exception")]
            Exception
        }

        /// <summary>
        /// 获取枚举类型的描述信息
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static string GetActionDescription(Enum action)
        {
            FieldInfo field = action.GetType().GetField(action.ToString());

            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attribute == null ? action.ToString() : attribute.Description;
        }
    }
}