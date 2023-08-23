using System;

namespace TextProcessor.Logics.Data.Options
{
    /// <summary>
    /// テキストファイル読み込み時のオプションを表します。
    /// </summary>
    [Serializable]
    public sealed class TextLoadOptions
    {
        /// <summary>
        /// 先頭行をヘッダーとして扱うかどうかを取得します。
        /// </summary>
        public bool HasHeader { get; }

        /// <summary>
        /// 行区切り文字を取得します。
        /// </summary>
        public string Separator { get; }

        /// <summary>
        /// <see cref="TextLoadOptions"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="hasHeader"><see cref="HasHeader"/></param>
        /// <param name="separator"><see cref="Separator"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="separator"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="separator"/>が空文字</exception>
        public TextLoadOptions(bool hasHeader, string separator)
        {
            ArgumentException.ThrowIfNullOrEmpty(separator);

            HasHeader = hasHeader;
            Separator = separator;
        }
    }
}
