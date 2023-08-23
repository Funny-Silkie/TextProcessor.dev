namespace TextProcessor.Logics.Operations
{
    /// <summary>
    /// 定義済みのセットを提供するインターフェイスです。
    /// </summary>
    /// <typeparam name="TSelf">自身の型</typeparam>
    public interface IHasDefinedSet<TSelf>
        where TSelf : IHasDefinedSet<TSelf>
    {
        /// <summary>
        /// 表示名を取得します。
        /// </summary>
        string? Title { get; }

        /// <summary>
        /// 定義されているインスタンスを取得します。
        /// </summary>
        /// <returns>定義されているインスタンス一覧</returns>
        abstract static TSelf[] GetDefinedSet();
    }
}
