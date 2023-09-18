using System;

namespace TextProcessor.Logics.Data.Options
{
    /// <summary>
    /// Excelファイル出力時のオプションを表します。
    /// </summary>
    [Serializable]
    public sealed class ExcelSaveOptions
    {
        /// <summary>
        /// 全て文字列として保存するかどうかを表す値を取得します。
        /// </summary>
        public bool SaveAsRawString { get; }

        /// <summary>
        /// <see cref="ExcelSaveOptions"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="saveAsRawString"><see cref="SaveAsRawString"/></param>
        public ExcelSaveOptions(bool saveAsRawString)
        {
            SaveAsRawString = saveAsRawString;
        }
    }
}
