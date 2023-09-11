using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TextProcessor.Logics.Data;

namespace TextProcessor.Logics.Operations.OperationImpl
{
    /// <summary>
    /// 置換処理を行う処理のクラスです。
    /// </summary>
    [Serializable]
    internal class ReplaceOperation : Operation
    {
        /// <summary>
        /// 検索文字列を取得または設定します。
        /// </summary>
        public string QueryText
        {
            get => _queryText;
            set
            {
                if (string.Equals(_queryText, value, StringComparison.Ordinal)) return;
                _queryText = value;
                _regex = null;
            }
        }

        private string _queryText = string.Empty;

        /// <summary>
        /// 置換後の文字列を取得または設定します。
        /// </summary>
        public string ReplacerText { get; set; } = string.Empty;

        /// <summary>
        /// 処理対象の列インデックスを取得または設定します。
        /// </summary>
        public int TargetColumnIndex { get; set; }

        /// <summary>
        /// 全ての列のデータを検証するかどうかを表す値を取得または設定します。
        /// </summary>
        public bool IsAll { get; set; }

        /// <summary>
        /// 大文字小文字を区別するかどうかを表す値を取得または設定します。
        /// </summary>
        public bool CaseSensitive { get; set; } = true;

        /// <summary>
        /// 正規表現を使用するかどうかを表す値を取得または設定します。
        /// </summary>
        public bool UseRegex { get; set; }

        /// <summary>
        /// 使用する正規表現オブジェクトを取得します。
        /// </summary>
        private Regex? Regex
        {
            get
            {
                if (!UseRegex) return null;
                if (_regex is null)
                {
                    var options = RegexOptions.Compiled;
                    if (!CaseSensitive) options |= RegexOptions.IgnoreCase;
                    try
                    {
                        _regex = new Regex(QueryText, options);
                    }
                    catch
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
        public override string Title => "値を置換";

        /// <summary>
        /// <see cref="ReplaceOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        public ReplaceOperation()
        {
        }

        /// <summary>
        /// <see cref="ReplaceOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        private ReplaceOperation(ReplaceOperation cloned)
            : base(cloned)
        {
            QueryText = cloned.QueryText;
            ReplacerText = cloned.ReplacerText;
            TargetColumnIndex = cloned.TargetColumnIndex;
            IsAll = cloned.IsAll;
            CaseSensitive = cloned.CaseSensitive;
            UseRegex = cloned.UseRegex;
        }

        /// <inheritdoc/>
        protected override Operation CloneCore() => new ReplaceOperation(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.String, "置換前", () => QueryText, x => QueryText = x),
                new ArgumentInfo(ArgumentType.String, "置換後", () => ReplacerText, x => ReplacerText = x),
                new ArgumentInfo(ArgumentType.Index, "対象列番号", () => TargetColumnIndex, x => TargetColumnIndex = x),
                new ArgumentInfo(ArgumentType.Boolean, "全体を対象にする", () => IsAll, x => IsAll = x),
                new ArgumentInfo(ArgumentType.Boolean, "大文字小文字の区別", () => CaseSensitive, x => CaseSensitive = x),
                new ArgumentInfo(ArgumentType.Boolean, "正規表現を使用", () => UseRegex, x => UseRegex = x),
            };
        }

        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (string.IsNullOrEmpty(QueryText)) status.Errors.Add(new StatusEntry(Title, Arguments[0], "検索文字列が指定されていません"));
            if (UseRegex && Regex is null) status.Errors.Add(new StatusEntry(Title, Arguments[0], "正規表現が無効です"));
        }

        /// <inheritdoc/>
        protected override void OperateCore(TextData data, ProcessStatus status)
        {
            List<List<string>> list = data.GetSourceData();
            if (UseRegex)
            {
                Regex? regex = Regex;
                if (regex is null) return;

                if (IsAll)
                {
                    for (int rowIndex = data.HasHeader ? 1 : 0; rowIndex < list.Count; rowIndex++)
                    {
                        List<string> row = list[rowIndex];
                        for (int columnIndex = 0; columnIndex < row.Count; columnIndex++)
                        {
                            string value = row[columnIndex];
                            if (string.IsNullOrEmpty(value)) continue;
                            row[columnIndex] = regex.Replace(value, ReplacerText);
                        }
                    }
                }
                else
                {
                    for (int rowIndex = data.HasHeader ? 1 : 0; rowIndex < list.Count; rowIndex++)
                    {
                        List<string> row = list[rowIndex];
                        if (TargetColumnIndex >= row.Count) continue;

                        string value = row[TargetColumnIndex];
                        if (string.IsNullOrEmpty(value)) continue;
                        row[TargetColumnIndex] = regex.Replace(value, ReplacerText);
                    }
                }
            }
            else
            {
                StringComparison stringComparison = CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

                if (IsAll)
                {
                    for (int rowIndex = data.HasHeader ? 1 : 0; rowIndex < list.Count; rowIndex++)
                    {
                        List<string> row = list[rowIndex];
                        for (int columnIndex = 0; columnIndex < row.Count; columnIndex++)
                        {
                            string value = row[columnIndex];
                            if (string.IsNullOrEmpty(value)) continue;
                            row[columnIndex] = value.Replace(QueryText, ReplacerText, stringComparison);
                        }
                    }
                }
                else
                {
                    for (int rowIndex = data.HasHeader ? 1 : 0; rowIndex < list.Count; rowIndex++)
                    {
                        List<string> row = list[rowIndex];
                        if (TargetColumnIndex >= row.Count) continue;

                        string value = row[TargetColumnIndex];
                        if (string.IsNullOrEmpty(value)) continue;
                        row[TargetColumnIndex] = value.Replace(QueryText, ReplacerText, stringComparison);
                    }
                }
            }
        }
    }
}
