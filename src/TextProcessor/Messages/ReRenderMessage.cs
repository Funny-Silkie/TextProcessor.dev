namespace TextProcessor.Messages
{
    /// <summary>
    /// 再描画のメッセージを表すクラスです。
    /// </summary>
    public class ReRenderMessage
    {
        /// <summary>
        /// 再描画対象を取得します。
        /// </summary>
        public string? Target { get; init; }
    }
}
