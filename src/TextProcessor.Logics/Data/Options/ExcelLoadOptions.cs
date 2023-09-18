using System;

namespace TextProcessor.Logics.Data.Options
{
    /// <summary>
    /// Excelファイル読み込み時のオプションを表します。
    /// </summary>
    [Serializable]
    public sealed class ExcelLoadOptions
    {
        /// <summary>
        /// 先頭行をヘッダーとして扱うかどうかを取得します。
        /// </summary>
        public bool HasHeader { get; }

        /// <summary>
        /// <see cref="ExcelLoadOptions"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="hasHeader"><see cref="HasHeader"/></param>
        public ExcelLoadOptions(bool hasHeader)
        {
            HasHeader = hasHeader;
        }
    }
}
