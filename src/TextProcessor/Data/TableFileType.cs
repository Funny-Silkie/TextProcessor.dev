using System;

namespace TextProcessor.Data
{
    /// <summary>
    /// ファイルの種類を表します。
    /// </summary>
    [Serializable]
    public enum TableFileType
    {
        /// <summary>
        /// DSVファイルを表します。
        /// </summary>
        Dsv,

        /// <summary>
        /// EXCELのファイルを表します。
        /// </summary>
        Excel,
    }
}
