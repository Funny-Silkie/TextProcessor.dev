using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace TextProcessor.Logics.Operations.Conditions
{
    /// <summary>
    /// 指定した文字列から始まるかどうかを検証します。
    /// </summary>
    [Serializable]
    internal sealed class StartsWithValueCondition : ValueCondition
    {
        /// <summary>
        /// 検索文字列を取得または設定します。
        /// </summary>
        public string Comparison { get; set; } = string.Empty;

        /// <summary>
        /// 大文字小文字を区別するかどうかを表す値を取得または設定します。
        /// </summary>
        public bool CaseSensitive { get; set; } = true;

        /// <inheritdoc/>
        public override string? Title => "指定した文字列で始まる";

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.String, "検索文字列", () => Comparison, x => Comparison = x),
                new ArgumentInfo(ArgumentType.Boolean, "大文字小文字の区別", () => CaseSensitive, x => CaseSensitive = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (string.IsNullOrEmpty(Comparison)) status.Errors.Add(new StatusEntry(Title, Arguments[0], "検索文字列が指定されていません"));
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            return target.StartsWith(Comparison, CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) ? MatchResult.Matched : MatchResult.NotMatched;
        }
    }

    /// <summary>
    /// 指定した文字列から終わるかどうかを検証します。
    /// </summary>
    [Serializable]
    internal sealed class EndsWithValueCondition : ValueCondition
    {
        /// <summary>
        /// 検索文字列を取得または設定します。
        /// </summary>
        public string Comparison { get; set; } = string.Empty;

        /// <summary>
        /// 大文字小文字を区別するかどうかを表す値を取得または設定します。
        /// </summary>
        public bool CaseSensitive { get; set; } = true;

        /// <inheritdoc/>
        public override string? Title => "指定した文字列で終わる";

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.String, "検索文字列", () => Comparison, x => Comparison = x),
                new ArgumentInfo(ArgumentType.Boolean, "大文字小文字の区別", () => CaseSensitive, x => CaseSensitive = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (string.IsNullOrEmpty(Comparison)) status.Errors.Add(new StatusEntry(Title, Arguments[0], "検索文字列が指定されていません"));
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            return target.EndsWith(Comparison, CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) ? MatchResult.Matched : MatchResult.NotMatched;
        }
    }

    /// <summary>
    /// 指定した文字列を含むかどうかを検証します。
    /// </summary>
    [Serializable]
    internal sealed class ContainsValueCondition : ValueCondition
    {
        /// <summary>
        /// 検索文字列を取得または設定します。
        /// </summary>
        public string Comparison { get; set; } = string.Empty;

        /// <summary>
        /// 大文字小文字を区別するかどうかを表す値を取得または設定します。
        /// </summary>
        public bool CaseSensitive { get; set; } = true;

        /// <inheritdoc/>
        public override string? Title => "指定した文字列を含む";

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.String, "検索文字列", () => Comparison, x => Comparison = x),
                new ArgumentInfo(ArgumentType.Boolean, "大文字小文字の区別", () => CaseSensitive, x => CaseSensitive = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (string.IsNullOrEmpty(Comparison)) status.Errors.Add(new StatusEntry(Title, Arguments[0], "検索文字列が指定されていません"));
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            return target.Contains(Comparison, CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) ? MatchResult.Matched : MatchResult.NotMatched;
        }
    }

    /// <summary>
    /// 指定した文字列と完全一致するどうかを検証します。
    /// </summary>
    [Serializable]
    internal sealed class MatchValueCondition : ValueCondition
    {
        /// <summary>
        /// 検索文字列を取得または設定します。
        /// </summary>
        public string Comparison { get; set; } = string.Empty;

        /// <summary>
        /// 大文字小文字を区別するかどうかを表す値を取得または設定します。
        /// </summary>
        public bool CaseSensitive { get; set; } = true;

        /// <inheritdoc/>
        public override string? Title => "指定した文字列と一致";

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.String, "文字列", () => Comparison, x => Comparison = x),
                new ArgumentInfo(ArgumentType.Boolean, "大文字小文字の区別", () => CaseSensitive, x => CaseSensitive = x),
            };
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            return string.Equals(target, Comparison, CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) ? MatchResult.Matched : MatchResult.NotMatched;
        }
    }

    /// <summary>
    /// 正規表現にマッチするかどうかを検証します。
    /// </summary>
    [Serializable]
    internal sealed class RegexMatchValueCondition : ValueCondition
    {
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
        /// 大文字小文字を区別するかどうかを表す値を取得または設定します。
        /// </summary>
        public bool CaseSensitive { get; set; } = true;

        /// <summary>
        /// 使用する正規表現オブジェクトを取得します。
        /// </summary>
        private Regex? Regex
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

        /// <inheritdoc/>
        public override string? Title => "正規表現に一致";

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
        public override MatchResult Match(string target)
        {
            Regex? regex = Regex;
            if (regex is null) return MatchResult.Error;
            return regex.IsMatch(target) ? MatchResult.Matched : MatchResult.NotMatched;
        }
    }
}
