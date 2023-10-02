using System;
using System.Collections.Generic;
using TextProcessor.Logics.Operations.Conditions;

namespace TextProcessor.Logics.Operations.Conversions
{
    /// <summary>
    /// 値の変換を表します。
    /// </summary>
    [Serializable]
    public abstract class ValueConversion : ICloneable, IHasDefinedSet<ValueConversion>, IHasArguments
    {
        /// <summary>
        /// 値を素通しするインスタンスを取得します。
        /// </summary>
        public static ValueConversion Through { get; } = new ThroughValueConversion();

        /// <summary>
        /// 引数一覧を取得します。
        /// </summary>
        public IList<ArgumentInfo> Arguments => _arguments ??= GenerateArguments();

        [NonSerialized]
        private IList<ArgumentInfo>? _arguments;

        /// <inheritdoc/>
        public abstract string? Title { get; }

        /// <summary>
        /// <see cref="ValueConversion"/>の新しいインスタンスを初期化します。
        /// </summary>
        protected ValueConversion()
        {
        }

        /// <summary>
        /// <see cref="ValueConversion"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected ValueConversion(ValueConversion cloned)
        {
            ArgumentNullException.ThrowIfNull(cloned);
        }

        /// <inheritdoc cref="ICloneable.Clone"/>
        public ValueConversion Clone() => CloneCore();

        /// <summary>
        /// 複製処理を行います。
        /// </summary>
        /// <returns>複製されたインスタンス</returns>
        protected virtual ValueConversion CloneCore() => (ValueConversion)MemberwiseClone();

        object ICloneable.Clone() => Clone();

        /// <summary>
        /// 引数情報を生成します。
        /// </summary>
        /// <returns>引数情報一覧</returns>
        protected virtual IList<ArgumentInfo> GenerateArguments() => Array.Empty<ArgumentInfo>();

        /// <inheritdoc/>
        public static ValueConversion[] GetDefinedSet()
        {
            return new ValueConversion[]
            {
                Through,
                new SequentialValueConversion(),
                new IfValueConversion(),
                new PrependValueConversion(),
                new AppendValueConversion(),
                new InsertValueConversion(),
                new OverwriteValueConversion(),
                new RangeValueConversion(),
                new ToUpperValueConversion(),
                new ToLowerValueConversion(),
                new RegexMatchValueConversion(),
                ValueConversionFactory.AddAsInteger(),
                ValueConversionFactory.SubtractAsInteger(),
                ValueConversionFactory.MultiplyAsInteger(),
                ValueConversionFactory.DivideAsInteger(),
                ValueConversionFactory.ModuloAsInteger(),
                ValueConversionFactory.AddAsDecimal(),
                ValueConversionFactory.SubtractAsDecimal(),
                ValueConversionFactory.MultiplyAsDecimal(),
                ValueConversionFactory.DivideAsDecimal(),
                ValueConversionFactory.ModuloAsDecimal(),
                new RoundValueConversion(),
                new TruncateValueConversion(),
                new CeilingValueConversion(),
                new Condition2BooleanValueConversion(),
            };
        }

        /// <inheritdoc cref="IHasArguments.VerifyArguments"/>
        public ProcessStatus VerifyArguments()
        {
            var result = new ProcessStatus();
            VerifyArgumentsCore(result);
            return result;
        }

        /// <summary>
        /// <inheritdoc cref="IHasArguments.VerifyArguments"/>
        /// </summary>
        /// <param name="status">チェック結果</param>
        protected virtual void VerifyArgumentsCore(ProcessStatus status)
        {
        }

        /// <summary>
        /// 値の変換を行います。
        /// </summary>
        /// <param name="value">変換する値</param>
        /// <param name="result">変換後の値。変換に失敗したら<see langword="null"/></param>
        /// <returns>変換処理の結果</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/>が<see langword="null"/></exception>
        public ProcessStatus Convert(string value, out string? result)
        {
            ArgumentNullException.ThrowIfNull(value);

            var status = new ProcessStatus();
            result = ConvertCore(value, status);
            return status;
        }

        /// <summary>
        /// 値の変換を行います。
        /// </summary>
        /// <param name="value">変換する値</param>
        /// <param name="status">変換処理の結果</param>
        /// <returns>変換後の値。変換に失敗したら<see langword="null"/></returns>
        protected abstract string? ConvertCore(string value, ProcessStatus status);

        /// <summary>
        /// 値を素通しする変換を表します。
        /// </summary>
        [Serializable]
        private sealed class ThroughValueConversion : ValueConversion
        {
            /// <inheritdoc/>
            public override string? Title => "そのまま";

            /// <inheritdoc/>
            public override bool Equals(object? obj) => obj is ThroughValueConversion;

            /// <inheritdoc/>
            public override int GetHashCode() => GetType().Name.GetHashCode();

            /// <inheritdoc/>
            protected override string? ConvertCore(string value, ProcessStatus status)
            {
                return value;
            }
        }
    }

    /// <summary>
    /// <see cref="ValueConversion"/>の生成を行います。
    /// </summary>
    internal static class ValueConversionFactory
    {
        /// <summary>
        /// <see cref="long"/>に対する<see cref="AddValueConversion{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="AddValueConversion{T}"/>の新しいインスタンス</returns>
        public static AddValueConversion<long> AddAsInteger()
        {
            return new AddValueConversion<long>(ArgumentType.Integer64, "整数の和", 0, x => (long.TryParse(x, out long result), result));
        }

        /// <summary>
        /// <see cref="double"/>に対する<see cref="AddValueConversion{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="AddValueConversion{T}"/>の新しいインスタンス</returns>
        public static AddValueConversion<double> AddAsDecimal()
        {
            return new AddValueConversion<double>(ArgumentType.Decimal, "数値の和", 0, x => (double.TryParse(x, out double result), result));
        }

        /// <summary>
        /// <see cref="long"/>に対する<see cref="SubtractValueConversion{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="SubtractValueConversion{T}"/>の新しいインスタンス</returns>
        public static SubtractValueConversion<long> SubtractAsInteger()
        {
            return new SubtractValueConversion<long>(ArgumentType.Integer64, "整数の差", 0, x => (long.TryParse(x, out long result), result));
        }

        /// <summary>
        /// <see cref="double"/>に対する<see cref="SubtractValueConversion{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="SubtractValueConversion{T}"/>の新しいインスタンス</returns>
        public static SubtractValueConversion<double> SubtractAsDecimal()
        {
            return new SubtractValueConversion<double>(ArgumentType.Decimal, "数値の差", 0, x => (double.TryParse(x, out double result), result));
        }

        /// <summary>
        /// <see cref="long"/>に対する<see cref="MultiplyValueConversion{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="MultiplyValueConversion{T}"/>の新しいインスタンス</returns>
        public static MultiplyValueConversion<long> MultiplyAsInteger()
        {
            return new MultiplyValueConversion<long>(ArgumentType.Integer64, "整数の積", 1, x => (long.TryParse(x, out long result), result));
        }

        /// <summary>
        /// <see cref="double"/>に対する<see cref="MultiplyValueConversion{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="MultiplyValueConversion{T}"/>の新しいインスタンス</returns>
        public static MultiplyValueConversion<double> MultiplyAsDecimal()
        {
            return new MultiplyValueConversion<double>(ArgumentType.Decimal, "数値の積", 1, x => (double.TryParse(x, out double result), result));
        }

        /// <summary>
        /// <see cref="long"/>に対する<see cref="DivideValueConversion{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="DivideValueConversion{T}"/>の新しいインスタンス</returns>
        public static DivideValueConversion<long> DivideAsInteger()
        {
            return new DivideValueConversion<long>(ArgumentType.Integer64, "整数の商", 1, x => (long.TryParse(x, out long result), result), false);
        }

        /// <summary>
        /// <see cref="double"/>に対する<see cref="DivideValueConversion{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="DivideValueConversion{T}"/>の新しいインスタンス</returns>
        public static DivideValueConversion<double> DivideAsDecimal()
        {
            return new DivideValueConversion<double>(ArgumentType.Decimal, "数値の商", 1, x => (double.TryParse(x, out double result), result), true);
        }

        /// <summary>
        /// <see cref="long"/>に対する<see cref="ModuloValueConversion{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="ModuloValueConversion{T}"/>の新しいインスタンス</returns>
        public static ModuloValueConversion<long> ModuloAsInteger()
        {
            return new ModuloValueConversion<long>(ArgumentType.Integer64, "整数の剰余", 1, x => (long.TryParse(x, out long result), result), false);
        }

        /// <summary>
        /// <see cref="double"/>に対する<see cref="ModuloValueConversion{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="ModuloValueConversion{T}"/>の新しいインスタンス</returns>
        public static ModuloValueConversion<double> ModuloAsDecimal()
        {
            return new ModuloValueConversion<double>(ArgumentType.Decimal, "数値の剰余", 1, x => (double.TryParse(x, out double result), result), true);
        }
    }

    /// <summary>
    /// 複数処理を行う変換を表します。
    /// </summary>
    [Serializable]
    internal sealed class SequentialValueConversion : ValueConversion
    {
        /// <inheritdoc/>
        public override string? Title => "複数処理";

        /// <summary>
        /// 処理一覧を取得または設定します。
        /// </summary>
        public List<ValueConversion> Conversions { get; set; }

        /// <summary>
        /// <see cref="SequentialValueConversion"/>の新しいインスタンスを初期化します。
        /// </summary>
        public SequentialValueConversion()
        {
            Conversions = new List<ValueConversion>() { Through };
        }

        /// <summary>
        /// <see cref="SequentialValueConversion"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        private SequentialValueConversion(SequentialValueConversion cloned)
            : base(cloned)
        {
            Conversions = cloned.Conversions.CloneAll();
        }

        /// <inheritdoc/>
        protected override ValueConversion CloneCore() => new SequentialValueConversion(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.ValueConversion | ArgumentType.List, "処理一覧", () => Conversions, x => Conversions = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            for (int i = 0; i < Conversions.Count; i++) StatusHelper.VerifyValueConversion(Title, status, Arguments[0], Conversions[i]);
        }

        /// <inheritdoc/>
        protected override string? ConvertCore(string value, ProcessStatus status)
        {
            string? result = value;
            for (int i = 0; i < Conversions.Count; i++)
            {
                ProcessStatus currentStatus = Conversions[i].Convert(result!, out result);
                StatusHelper.MergeAsChild(Title, status, currentStatus);
                if (!currentStatus.Success) return null;
            }
            return result;
        }
    }

    /// <summary>
    /// 条件分岐を伴う値の変換を表します。
    /// </summary>
    [Serializable]
    internal sealed class IfValueConversion : ValueConversion
    {
        /// <inheritdoc/>
        public override string? Title => "条件分岐";

        /// <summary>
        /// 分岐に用いる条件を取得または設定します。
        /// </summary>
        public ValueCondition Condition { get; set; }

        /// <summary>
        /// 条件を満たす際の変換を取得または設定します。
        /// </summary>
        public ValueConversion TrueConversion { get; set; }

        /// <summary>
        /// 条件を満たさない際の変換を取得または設定します。
        /// </summary>
        public ValueConversion FalseConversion { get; set; }

        /// <summary>
        /// <see cref="IfValueConversion"/>の新しいインスタンスを初期化します。
        /// </summary>
        public IfValueConversion()
        {
            Condition = ValueCondition.Null;
            TrueConversion = Through;
            FalseConversion = Through;
        }

        /// <summary>
        /// <see cref="IfValueConversion"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        private IfValueConversion(IfValueConversion cloned)
            : base(cloned)
        {
            Condition = cloned.Condition.Clone();
            TrueConversion = cloned.TrueConversion.Clone();
            FalseConversion = cloned.FalseConversion.Clone();
        }

        /// <inheritdoc/>
        protected override ValueConversion CloneCore() => new IfValueConversion(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.ValueCondition, "条件", () => Condition, x => Condition = x),
                new ArgumentInfo(ArgumentType.ValueConversion, "処理（TRUE）", () => TrueConversion, x => TrueConversion = x),
                new ArgumentInfo(ArgumentType.ValueConversion, "処理（FALSE）", () => FalseConversion, x => FalseConversion = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            StatusHelper.VerifyValueCondition(Title, status, Arguments[0], Condition);
            StatusHelper.VerifyValueConversion(Title, status, Arguments[1], TrueConversion);
            StatusHelper.VerifyValueConversion(Title, status, Arguments[2], FalseConversion);
        }

        /// <inheritdoc/>
        protected override string? ConvertCore(string value, ProcessStatus status)
        {
            MatchResult matchResult = Condition.Match(value);
            switch (matchResult)
            {
                case MatchResult.Matched:
                    {
                        ProcessStatus conversionResult = TrueConversion.Convert(value, out string? result);
                        StatusHelper.MergeAsChild(Title, status, conversionResult);
                        return conversionResult.Success ? result : null;
                    }

                case MatchResult.NotMatched:
                    {
                        ProcessStatus conversionResult = FalseConversion.Convert(value, out string? result);
                        StatusHelper.MergeAsChild(Title, status, conversionResult);
                        return conversionResult.Success ? result : null;
                    }

                case MatchResult.Error:
                default:
                    status.Errors.Add(new StatusEntry(Title, null, "条件検証時にエラーが発生しました"));
                    return null;
            }
        }
    }

    /// <summary>
    /// 条件の真偽値への変換を表します。
    /// </summary>
    [Serializable]
    internal sealed class Condition2BooleanValueConversion : ValueConversion
    {
        internal const string TrueString = "TRUE";
        internal const string FalseString = "FALSE";
        internal const string ErrorDefaultString = "ERROR";

        /// <inheritdoc/>
        public override string? Title => "条件の真偽値";

        /// <summary>
        /// 使用する条件を取得または設定します。
        /// </summary>
        public ValueCondition Condition { get; set; }

        /// <summary>
        /// エラー時の文字列を取得または設定します。
        /// </summary>
        public string ErrorText { get; set; } = ErrorDefaultString;

        /// <summary>
        /// <see cref="Condition2BooleanValueConversion"/>の新しいインスタンスを初期化します。
        /// </summary>
        public Condition2BooleanValueConversion()
        {
            Condition = ValueCondition.Null;
        }

        /// <summary>
        /// <see cref="Condition2BooleanValueConversion"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        private Condition2BooleanValueConversion(Condition2BooleanValueConversion cloned)
            : base(cloned)
        {
            Condition = cloned.Condition.Clone();
            ErrorText = cloned.ErrorText;
        }

        /// <inheritdoc/>
        protected override ValueConversion CloneCore() => new Condition2BooleanValueConversion(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.ValueCondition, "条件", () => Condition, x => Condition = x),
                new ArgumentInfo(ArgumentType.String, "エラー時の値", () => ErrorText, x => ErrorText = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            StatusHelper.VerifyValueCondition(Title, status, Arguments[0], Condition);
        }

        /// <inheritdoc/>
        protected override string? ConvertCore(string value, ProcessStatus status)
        {
            MatchResult matchResult = Condition.Match(value);
            switch (matchResult)
            {
                case MatchResult.Matched: return TrueString;
                case MatchResult.NotMatched: return FalseString;
                case MatchResult.Error: return ErrorText;

                default:
                    status.Errors.Add(new StatusEntry(Title, null, "条件の検証に失敗しました"));
                    return null;
            }
        }
    }
}
