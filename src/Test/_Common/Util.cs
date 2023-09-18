using System.Text;
using TextProcessor.Logics.Data;
using TextProcessor.Logics.Data.Options;

namespace Test
{
    /// <summary>
    /// 共通処理を記述します。
    /// </summary>
    internal static class Util
    {
        /// <summary>
        /// ファイルを読み込みます。
        /// </summary>
        /// <param name="fileName">読み込むファイル名</param>
        /// <param name="options">読み込み時のオプション</param>
        /// <returns><paramref name="fileName"/>に応じた<see cref="TextData"/>のインスタンス</returns>
        public static TextData LoadFile(string fileName, TextLoadOptions? options = null)
        {
            return TextData.CreateFromDsv(fileName, options ?? new TextLoadOptions(true, "\t", new UTF8Encoding(false)));
        }
    }
}
