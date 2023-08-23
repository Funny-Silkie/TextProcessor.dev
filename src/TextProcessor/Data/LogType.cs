using System;

namespace TextProcessor.Data
{
    /// <summary>
    /// ログの種類を表します。
    /// </summary>
    [Serializable]
    public enum LogType
    {
        /// <summary>
        /// 情報
        /// </summary>
        Info,

        /// <summary>
        /// 警告
        /// </summary>
        Warning,

        /// <summary>
        /// エラー
        /// </summary>
        Error,

        /// <summary>
        /// 成功
        /// </summary>
        Success,
    }
}
