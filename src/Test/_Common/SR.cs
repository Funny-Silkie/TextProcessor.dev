namespace Test
{
    /// <summary>
    /// 静的リソースを管理します。
    /// </summary>
    internal static class SR
    {
        #region Source Files

        /// <summary>
        /// 読み込むファイルのあるディレクトリパス
        /// </summary>
        internal const string Source_Dir = "TestResource/in/";

        internal const string Source_PersonalTable = Source_Dir + "PersonalTable.tsv";
        internal const string Source_PrefectureTable = Source_Dir + "PrefectureTable.tsv";
        internal const string Source_RegionTable = Source_Dir + "RegionTable.tsv";

        #endregion Source Files

        /// <summary>
        /// 結果ファイルのあるディレクトリパス
        /// </summary>
        internal const string Result_Dir = "TestResource/out/";
    }
}
