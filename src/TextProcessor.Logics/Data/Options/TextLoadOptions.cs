using System;
using System.Text;

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
        /// 文字エンコードを取得します。
        /// </summary>
        public Encoding Encoding { get; }

        /// <summary>
        /// <see cref="TextLoadOptions"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="hasHeader"><see cref="HasHeader"/></param>
        /// <param name="separator"><see cref="Separator"/></param>
        /// <param name="encoding"><see cref="Encoding"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="separator"/>または<paramref name="encoding"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="separator"/>が空文字</exception>
        public TextLoadOptions(bool hasHeader, string separator, Encoding encoding)
        {
            ArgumentException.ThrowIfNullOrEmpty(separator);
            ArgumentNullException.ThrowIfNull(encoding);

            HasHeader = hasHeader;
            Separator = separator;
            Encoding = encoding;
        }
    }
}
