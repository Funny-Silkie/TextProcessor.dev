using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextProcessor.Logics
{
    internal partial class DsvLogic
    {
        private static readonly Dictionary<string, Regex> SplitRegexCache = new Dictionary<string, Regex>(StringComparer.Ordinal);

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private const string TsvSplitPattern = "(((?<=(^\")|(\\t\"))[^\"]*?(?=(\"\\t)|(\"$)))|((?<=(^')|(\\t'))[^']*?(?=('\\t)|('$)))|((?<=^|\\t)[^\\t'\"]*?['\"]?(?=\\t|$)))";

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private const string CsvSplitPattern = "(((?<=(^\")|(,\"))[^\"]*?(?=(\",)|(\"$)))|((?<=(^')|(,'))[^']*?(?=(',)|('$)))|((?<=^|,)[^,'\"]*?['\"]?(?=,|$)))";

        /// <summary>
        /// 行区切り文字に応じた正規表現オブジェクトを取得します。
        /// </summary>
        /// <param name="separator">行区切り文字</param>
        /// <returns><paramref name="separator"/>に応じた正規表現オブジェクト</returns>
        private Regex GetSplitRegex(string separator)
        {
            switch (separator)
            {
                case "\t": return GetTsvSplitRegex();
                case ",": return GetCsvSplitRegex();
                default:
                    if (!SplitRegexCache.TryGetValue(separator, out Regex? result))
                    {
                        result = new Regex(TsvSplitPattern.Replace("\\t", separator));
                        SplitRegexCache[separator] = result;
                    }
                    return result;
            }
        }

        [GeneratedRegex(TsvSplitPattern, RegexOptions.Compiled)]
        private static partial Regex GetTsvSplitRegex();

        [GeneratedRegex(CsvSplitPattern, RegexOptions.Compiled)]
        private static partial Regex GetCsvSplitRegex();

        /// <summary>
        /// 文字列を分割します。
        /// </summary>
        /// <param name="value">分割する文字列</param>
        /// <param name="separator">行区切り文字</param>
        /// <param name="capacity">リストの初期容量</param>
        /// <returns>分割後の文字列一覧</returns>
        /// <exception cref="ArgumentNullException"><paramref name="separator"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="separator"/>が空文字</exception>
        /// <exception cref="RegexMatchTimeoutException">正規表現検索時にタイムアウトが発生</exception>
        public List<string> Split(string value, string separator, int capacity = 0)
        {
            ArgumentException.ThrowIfNullOrEmpty(separator);

            Regex regex = GetSplitRegex(separator);
            return regex.Matches(value).Select(x => x.Value).ToList(capacity);
        }

        /// <summary>
        /// 行を出力します。
        /// </summary>
        /// <param name="writer">使用するライター</param>
        /// <param name="row">行を表すリスト</param>
        /// <param name="separator">行区切り文字</param>
        /// <exception cref="ArgumentNullException"><paramref name="separator"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="separator"/>が空文字</exception>
        public void WriteRow(TextWriter writer, List<string> row, string separator)
        {
            ArgumentException.ThrowIfNullOrEmpty(separator);

            int lastIndex = row.FindLastIndex(x => !string.Equals(x, separator, StringComparison.Ordinal));
            if (lastIndex >= 0)
            {
                WriteValue(writer, row[0], separator);
                for (int i = 1; i <= lastIndex; i++)
                {
                    writer.Write(separator);
                    WriteValue(writer, row[i], separator);
                }
            }
            writer.WriteLine();
        }

        /// <summary>
        /// 値を出力します。
        /// </summary>
        /// <param name="writer">使用するライター</param>
        /// <param name="value">出力する値</param>
        /// <param name="separator">行区切り文字</param>
        private void WriteValue(TextWriter writer, string value, string separator)
        {
            if (!value.Contains(separator, StringComparison.Ordinal))
            {
                writer.Write(value);
                return;
            }

            if (separator.Length == 1 && separator[0] == '"')
            {
                writer.Write('\'');
                writer.Write(value);
                writer.Write('\'');
            }
            else
            {
                writer.Write('"');
                writer.Write(value);
                writer.Write('"');
            }
        }

        /// <summary>
        /// 行を出力します。
        /// </summary>
        /// <param name="writer">使用するライター</param>
        /// <param name="row">行を表すリスト</param>
        /// <param name="separator">行区切り文字</param>
        /// <exception cref="ArgumentNullException"><paramref name="separator"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="separator"/>が空文字</exception>
        public async Task WriteRowAsync(TextWriter writer, List<string> row, string separator)
        {
            ArgumentException.ThrowIfNullOrEmpty(separator);

            int lastIndex = row.FindLastIndex(x => !string.Equals(x, separator, StringComparison.Ordinal));
            if (lastIndex >= 0)
            {
                await WriteValueAsync(writer, row[0], separator);
                for (int i = 1; i <= lastIndex; i++)
                {
                    await writer.WriteAsync(separator);
                    await WriteValueAsync(writer, row[i], separator);
                }
            }
            await writer.WriteLineAsync();
        }

        /// <summary>
        /// 値を出力します。
        /// </summary>
        /// <param name="writer">使用するライター</param>
        /// <param name="value">出力する値</param>
        /// <param name="separator">行区切り文字</param>
        private async Task WriteValueAsync(TextWriter writer, string value, string separator)
        {
            if (!value.Contains(separator, StringComparison.Ordinal))
            {
                await writer.WriteAsync(value);
                return;
            }

            if (separator.Length == 1 && separator[0] == '"')
            {
                await writer.WriteAsync('\'');
                await writer.WriteAsync(value);
                await writer.WriteAsync('\'');
            }
            else
            {
                await writer.WriteAsync('"');
                await writer.WriteAsync(value);
                await writer.WriteAsync('"');
            }
        }
    }
}
