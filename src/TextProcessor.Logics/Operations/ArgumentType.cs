using System;

namespace TextProcessor.Logics.Operations
{
    /// <summary>
    /// 引数の種類を表します。
    /// </summary>
    [Flags]
    [Serializable]
    public enum ArgumentType
    {
        /// <summary>
        /// 文字列
        /// </summary>
        String = 1 << 0,

        /// <summary>
        /// 文字列（複数行）
        /// </summary>
        StringMultiLine = 1 << 1,

        /// <summary>
        /// 整数
        /// </summary>
        Integer = 1 << 2,

        /// <summary>
        /// 整数
        /// </summary>
        Integer64 = 1 << 3,

        /// <summary>
        /// 小数
        /// </summary>
        Decimal = 1 << 4,

        /// <summary>
        /// 1-baseなインデックス
        /// </summary>
        Index = 1 << 5,

        /// <summary>
        /// 真偽値
        /// </summary>
        Boolean = 1 << 6,

        /// <summary>
        /// 値の条件
        /// </summary>
        ValueCondition = 1 << 7,

        /// <summary>
        /// 行の条件
        /// </summary>
        RowCondition = 1 << 8,

        /// <summary>
        /// 引数セット
        /// </summary>
        Arguments = 1 << 9,

        /// <summary>
        /// テキストデータ
        /// </summary>
        TextData = 1 << 10,

        /// <summary>
        /// 固定長配列（<see cref="System.Array"/>）
        /// </summary>
        Array = 1 << 11,

        /// <summary>
        /// 可変長配列（<see cref="System.Collections.IList"/>）
        /// </summary>
        List = 1 << 12,

        /// <summary>
        /// 値の変換
        /// </summary>
        ValueConversion = 1 << 13,
    }
}
