using System;

namespace TextProcessor.Logics.Data.Options
{
    /// <summary>
    /// テキストファイル保存時のオプションを表します。
    /// </summary>
    [Serializable]
    public sealed class TextSaveOptions
    {
        /// <summary>
        /// 行区切り文字を取得します。
        /// </summary>
        public string Separator { get; }

        /// <summary>
        /// <see cref="TextSaveOptions"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="separator"><see cref="Separator"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="separator"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="separator"/>が空文字</exception>
        public TextSaveOptions(string separator)
        {
            ArgumentException.ThrowIfNullOrEmpty(separator);

            Separator = separator;
        }
    }
}
