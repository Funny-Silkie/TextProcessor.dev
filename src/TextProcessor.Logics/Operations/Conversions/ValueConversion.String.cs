using System;
using System.Collections.Generic;

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
}
