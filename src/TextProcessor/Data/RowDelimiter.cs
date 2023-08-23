using System;

namespace TextProcessor.Data
{
    /// <summary>
    /// 行区切り文字を表します。
    /// </summary>
    [Serializable]
    public enum RowDelimiter
    {
        /// <summary>
        /// タブ文字
        /// </summary>
        Tab,

        /// <summary>
        /// カンマ
        /// </summary>
        Comma,

        /// <summary>
        /// その他
        /// </summary>
        Others,
    }
}
