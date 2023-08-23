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
        private string queryText = string.Empty;
        private string replacerText = string.Empty;
        private int targetColumn;
        private bool isAll;
        private bool caseSensitive = true;
        private bool useRegex;

        /// <summary>
        /// 使用する正規表現オブジェクトを取得します。
        /// </summary>
        private Regex? Regex
        {
            get
            {
                if (!useRegex) return null;
                if (_regex is null)
                {
                    var options = RegexOptions.Compiled;
                    if (!caseSensitive) options |= RegexOptions.IgnoreCase;
                    try
                    {
                        _regex = new Regex(queryText, options);
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
            queryText = cloned.queryText;
            replacerText = cloned.replacerText;
            targetColumn = cloned.targetColumn;
            isAll = cloned.isAll;
            caseSensitive = cloned.caseSensitive;
            useRegex = cloned.useRegex;
        }

        /// <inheritdoc/>
        protected override Operation CloneCore() => new ReplaceOperation(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.String, "置換前", () => queryText, x => queryText = x),
                new ArgumentInfo(ArgumentType.String, "置換後", () => replacerText, x => replacerText = x),
                new ArgumentInfo(ArgumentType.Index, "対象列番号", () => targetColumn, x => targetColumn = x),
                new ArgumentInfo(ArgumentType.Boolean, "全体を対象にする", () => isAll, x => isAll = x),
                new ArgumentInfo(ArgumentType.Boolean, "大文字小文字の区別", () => caseSensitive, x => caseSensitive = x),
                new ArgumentInfo(ArgumentType.Boolean, "正規表現を使用", () => useRegex, x => useRegex = x),
            };
        }

        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (string.IsNullOrEmpty(queryText)) status.Errors.Add(new StatusEntry(Title, Arguments[0], "検索文字列が指定されていません"));
            if (useRegex && Regex is null) status.Errors.Add(new StatusEntry(Title, Arguments[0], "正規表現が無効です"));
        }

        /// <inheritdoc/>
        protected override void OperateCore(TextData data, ProcessStatus status)
        {
            List<List<string>> list = data.GetSourceData();
            if (useRegex)
            {
                Regex? regex = Regex;
                if (regex is null) return;

                if (isAll)
                {
                    for (int rowIndex = data.HasHeader ? 1 : 0; rowIndex < list.Count; rowIndex++)
                    {
                        List<string> row = list[rowIndex];
                        for (int columnIndex = 0; columnIndex < row.Count; columnIndex++)
                        {
                            string value = row[columnIndex];
                            if (string.IsNullOrEmpty(value)) continue;
                            row[columnIndex] = regex.Replace(value, replacerText);
                        }
                    }
                }
                else
                {
                    for (int rowIndex = data.HasHeader ? 1 : 0; rowIndex < list.Count; rowIndex++)
                    {
                        List<string> row = list[rowIndex];
                        if (targetColumn >= row.Count) continue;

                        string value = row[targetColumn];
                        if (string.IsNullOrEmpty(value)) continue;
                        row[targetColumn] = regex.Replace(value, replacerText);
                    }
                }
            }
            else
            {
                StringComparison stringComparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

                if (isAll)
                {
                    for (int rowIndex = data.HasHeader ? 1 : 0; rowIndex < list.Count; rowIndex++)
                    {
                        List<string> row = list[rowIndex];
                        for (int columnIndex = 0; columnIndex < row.Count; columnIndex++)
                        {
                            string value = row[columnIndex];
                            if (string.IsNullOrEmpty(value)) continue;
                            row[columnIndex] = value.Replace(queryText, replacerText, stringComparison);
                        }
                    }
                }
                else
                {
                    for (int rowIndex = data.HasHeader ? 1 : 0; rowIndex < list.Count; rowIndex++)
                    {
                        List<string> row = list[rowIndex];
                        if (targetColumn >= row.Count) continue;

                        string value = row[targetColumn];
                        if (string.IsNullOrEmpty(value)) continue;
                        row[targetColumn] = value.Replace(queryText, replacerText, stringComparison);
                    }
                }
            }
        }
    }
}
