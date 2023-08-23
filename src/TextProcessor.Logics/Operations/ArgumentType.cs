using System;

namespace TextProcessor.Logics.Operations
{
    /// <summary>
    /// 引数の種類を表します。
    /// </summary>
    [Serializable]
    public enum ArgumentType
    {
        /// <summary>
        /// 文字列
        /// </summary>
        String,

        /// <summary>
        /// 文字列（複数行）
        /// </summary>
        StringMultiLine,

        /// <summary>
        /// 整数
        /// </summary>
        Integer,

        /// <summary>
        /// 整数
        /// </summary>
        Integer64,

        /// <summary>
        /// 小数
        /// </summary>
        Decimal,

        /// <summary>
        /// 1-baseなインデックス
        /// </summary>
        Index,

        /// <summary>
        /// 真偽値
        /// </summary>
        Boolean,

        /// <summary>
        /// 値の条件
        /// </summary>
        ValueCondition,

        /// <summary>
        /// 行の条件
        /// </summary>
        RowCondition,

        /// <summary>
        /// 引数セット
        /// </summary>
        Arguments,

        /// <summary>
        /// テキストデータ
        /// </summary>
        TextData,
    }
}
