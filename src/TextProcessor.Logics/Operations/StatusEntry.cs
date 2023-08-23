namespace TextProcessor.Logics.Operations
{
    /// <summary>
    /// 処理ステータスを表します。
    /// </summary>
    /// <param name="TargetName">引数を持つ対象名</param>
    /// <param name="Arg">検証した引数情報</param>
    /// <param name="Message">メッセージ</param>
    public record class StatusEntry(string? TargetName, ArgumentInfo? Arg, string Message)
    {
        private const string Delimiter = "/";

        /// <summary>
        /// 子の引数としてインスタンスを生成します。
        /// </summary>
        /// <param name="parentName">親プロセス名</param>
        /// <returns>この引数としてのインスタンス</returns>
        public StatusEntry CreateAsChildren(string? parentName)
        {
            return new StatusEntry($"{parentName}{Delimiter}{TargetName}", Arg, Message);
        }
    }
}
