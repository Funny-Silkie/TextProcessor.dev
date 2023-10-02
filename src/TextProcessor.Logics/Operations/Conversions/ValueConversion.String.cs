using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace TextProcessor.Logics.Operations.Conversions
{
    /// <summary>
    /// 先頭に文字列を追加する変換を表します。
    /// </summary>
    [Serializable]
    internal sealed class PrependValueConversion : ValueConversion
    {
        /// <summary>
        /// 追加する値を取得または設定します。
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <inheritdoc/>
        public override string? Title => "先頭に文字列を追加";

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.String, "追加する文字列", () => Value, x => Value = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (string.IsNullOrEmpty(Value)) status.Warnings.Add(new StatusEntry(Title, Arguments[0], "文字列が指定されていません"));
        }

        /// <inheritdoc/>
        protected override string? ConvertCore(string value, ProcessStatus status)
        {
            return Value + value;
        }
    }

    /// <summary>
    /// 末尾に文字列を追加する変換を表します。
    /// </summary>
    [Serializable]
    internal sealed class AppendValueConversion : ValueConversion
    {
        /// <summary>
        /// 追加する値を取得または設定します。
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <inheritdoc/>
        public override string? Title => "末尾に文字列を追加";

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.String, "追加する文字列", () => Value, x => Value = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (string.IsNullOrEmpty(Value)) status.Warnings.Add(new StatusEntry(Title, Arguments[0], "文字列が指定されていません"));
        }

        /// <inheritdoc/>
        protected override string? ConvertCore(string value, ProcessStatus status)
        {
            return value + Value;
        }
    }

    /// <summary>
    /// 文字列を挿入する変換を表します。
    /// </summary>
    [Serializable]
    internal sealed class InsertValueConversion : ValueConversion
    {
        /// <inheritdoc/>
        public override string? Title => "文字列を挿入";

        /// <summary>
        /// 追加する値を取得または設定します。
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// 挿入位置を取得または設定します。
        /// </summary>
        public int Position { get; set; }

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.String, "挿入する文字列", () => Value, x => Value = x),
                new ArgumentInfo(ArgumentType.Integer, "挿入位置", () => Position, x => Position = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (string.IsNullOrEmpty(Value)) status.Warnings.Add(new StatusEntry(Title, Arguments[0], "文字列が指定されていません"));
            if (Position < 0) status.Errors.Add(new StatusEntry(Title, Arguments[1], "挿入位置が負の値です"));
        }

        /// <inheritdoc/>
        protected override string? ConvertCore(string value, ProcessStatus status)
        {
            if (Position > value.Length)
            {
                status.Errors.Add(new StatusEntry(Title, null, $"挿入位置'{Position}'が範囲外です"));
                return null;
            }
            return value.Insert(Position, Value);
        }
    }

    /// <summary>
    /// 文字列を上書きする変換を表します。
    /// </summary>
    [Serializable]
    internal sealed class OverwriteValueConversion : ValueConversion
    {
        /// <summary>
        /// 上書きする値を取得または設定します。
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <inheritdoc/>
        public override string? Title => "固定値";

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.String, "上書きする文字列", () => Value, x => Value = x),
            };
        }

        /// <inheritdoc/>
        protected override string? ConvertCore(string value, ProcessStatus status)
        {
            return Value;
        }
    }

    /// <summary>
    /// 部分文字列を切り出す変換を表します。
    /// </summary>
    [Serializable]
    internal sealed class RangeValueConversion : ValueConversion
    {
        /// <inheritdoc/>
        public override string? Title => "部分文字列を切り出す";

        /// <summary>
        /// 開始インデックスを取得または設定します。
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// 切り出す長さを取得または設定します。
        /// </summary>
        public int Length { get; set; } = 1;

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.Integer, "開始インデックス", () => StartIndex, x => StartIndex = x),
                new ArgumentInfo(ArgumentType.Integer, "切り出す長さ", () =>  Length, x => Length = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (StartIndex < 0) status.Errors.Add(new StatusEntry(Title, Arguments[0], "開始インデックスが負の値です"));
            if (Length < 0) status.Errors.Add(new StatusEntry(Title, Arguments[1], "開始インデックスが負の値です"));
        }

        /// <inheritdoc/>
        protected override string? ConvertCore(string value, ProcessStatus status)
        {
            if (StartIndex + Length >= value.Length) return value;
            return value.Substring(StartIndex, Length);
        }
    }

    /// <summary>
    /// 大文字への変換を表します。
    /// </summary>
    [Serializable]
    internal sealed class ToUpperValueConversion : ValueConversion
    {
        /// <inheritdoc/>
        public override string? Title => "大文字に変換";

        /// <inheritdoc/>
        protected override string? ConvertCore(string value, ProcessStatus status)
        {
            return value.ToUpper();
        }
    }

    /// <summary>
    /// 小文字への変換を表します。
    /// </summary>
    [Serializable]
    internal sealed class ToLowerValueConversion : ValueConversion
    {
        /// <inheritdoc/>
        public override string? Title => "小文字に変換";

        /// <inheritdoc/>
        protected override string? ConvertCore(string value, ProcessStatus status)
        {
            return value.ToLower();
        }
    }

    /// <summary>
    /// 正規表現マッチ部分への変換を表します。
    /// </summary>
    [Serializable]
    internal sealed class RegexMatchValueConversion : ValueConversion
    {
        /// <inheritdoc/>
        public override string? Title => "正規表現マッチ部分の抽出";

        /// <summary>
        /// 正規表現パターンを取得または設定します。
        /// </summary>
        [StringSyntax(StringSyntaxAttribute.Regex)]
        public string Pattern
        {
            get => _pattern;
            set
            {
                if (string.Equals(_pattern, value, StringComparison.Ordinal)) return;
                _pattern = value;
                _regex = null;
            }
        }

        [StringSyntax(StringSyntaxAttribute.Regex)]
        private string _pattern = string.Empty;

        /// <summary>
        /// 正規表現オブジェクトを取得します。
        /// </summary>
        public Regex? Regex
        {
            get
            {
                if (_regex is null)
                {
                    var options = RegexOptions.Compiled;
                    if (!CaseSensitive) options |= RegexOptions.IgnoreCase;
                    try
                    {
                        _regex = new Regex(Pattern, options);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                return _regex;
            }
        }

        [NonSerialized]
        private Regex? _regex;

        /// <summary>
        /// 大文字小文字を区別するかどうかを表す値を取得または設定します。
        /// </summary>
        public bool CaseSensitive
        {
            get => _caseSensitive;
            set
            {
                if (_caseSensitive == value) return;
                _caseSensitive = value;
                _regex = null;
            }
        }

        private bool _caseSensitive = true;

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.String, "正規表現パターン", () => Pattern, x => Pattern = x),
                new ArgumentInfo(ArgumentType.Boolean, "大文字小文字の区別", () => CaseSensitive, x => CaseSensitive = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (string.IsNullOrEmpty(Pattern)) status.Errors.Add(new StatusEntry(Title, Arguments[0], "正規表現パターンが指定されていません"));
            if (Regex is null) status.Errors.Add(new StatusEntry(Title, Arguments[0], "正規表現が無効です"));
        }

        /// <inheritdoc/>
        protected override string? ConvertCore(string value, ProcessStatus status)
        {
            if (Regex is null) return null;

            Match regexResult = Regex.Match(value);
            return regexResult.Value;
        }
    }
}
