namespace TextProcessor.Data
{
    /// <summary>
    /// ログ情報を表します。
    /// </summary>
    /// <param name="Type">ログの種類</param>
    /// <param name="Message">メッセージ</param>
    /// <param name="Target">ターゲット</param>
    public sealed record class LogInfo(LogType Type, string? Message, string? Target);
}
