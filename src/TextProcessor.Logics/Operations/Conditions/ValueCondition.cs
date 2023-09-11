using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace TextProcessor.Logics.Operations.Conditions
{
    /// <summary>
    /// 値に対する条件を表します。
    /// </summary>
    [Serializable]
    public abstract class ValueCondition : Condition<string, ValueCondition>, IHasDefinedSet<ValueCondition>, IHasArguments
    {
        /// <summary>
        /// 空の条件を取得します。
        /// </summary>
        public static ValueCondition Null { get; } = new NullConditions();

        /// <inheritdoc/>
        public abstract string? Title { get; }

        /// <summary>
        /// <see cref="ValueCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        protected ValueCondition()
        {
        }

        /// <summary>
        /// <see cref="ValueCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected ValueCondition(ValueCondition cloned)
            : base(cloned)
        {
        }

        /// <inheritdoc/>
        public static ValueCondition[] GetDefinedSet()
        {
            return new ValueCondition[]
            {
                Null,
                new NotValueCondition(),
                new OrValueCondition(),
                new AndValueCondition(),
                new IsEmptyValueCondition(),
                new IsIntegerValueCondition(),
                new IsDecimalValueCondition(),
                new MatchValueCondition(),
                new ContainsValueCondition(),
                new StartsWithValueCondition(),
                new EndsWithValueCondition(),
                new RegexMatchValueCondition(),
                ValueConditionFactory.LargerAsInteger(),
                ValueConditionFactory.LargetAsDecimal(),
                ValueConditionFactory.LowerAsInteger(),
                ValueConditionFactory.LowerAsDecimal(),
                ValueConditionFactory.LargerOrEqualAsInteger(),
                ValueConditionFactory.LargerOrEqualAsDecimal(),
                ValueConditionFactory.LowerOrEqualAsInteger(),
                ValueConditionFactory.LowerOrEqualAsDecimal(),
                ValueConditionFactory.EqualAsInteger(),
                ValueConditionFactory.EqualAsDecimal(),
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
        /// <param name="errors">エラーの登録先</param>
        protected virtual void VerifyArgumentsCore(ProcessStatus status)
        {
        }

        /// <summary>
        /// 空の条件を表します。
        /// </summary>
        [Serializable]
        private sealed class NullConditions : ValueCondition
        {
            /// <inheritdoc/>
            public override string? Title => "条件を設定してください";

            /// <inheritdoc/>
            public override bool Equals(object? obj) => obj is NullConditions;

            /// <inheritdoc/>
            public override int GetHashCode() => GetType().Name.GetHashCode();

            /// <inheritdoc/>
            public override MatchResult Match(string target) => MatchResult.Error;
        }
    }

    /// <summary>
    /// <see cref="ValueCondition"/>の生成を行います。
    /// </summary>
    internal static class ValueConditionFactory
    {
        /// <summary>
        /// <see cref="long"/>に対する<see cref="LargerValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LargerValueCondition{T}"/>の新しいインスタンス</returns>
        public static LargerValueCondition<long> LargerAsInteger()
        {
            return new LargerValueCondition<long>(ArgumentType.Integer64, "指定した整数より大きい", 0, x => (long.TryParse(x, out long result), result));
        }

        /// <summary>
        /// <see cref="double"/>に対する<see cref="LargerValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LargerValueCondition{T}"/>の新しいインスタンス</returns>
        public static LargerValueCondition<double> LargetAsDecimal()
        {
            return new LargerValueCondition<double>(ArgumentType.Decimal, "指定した数値より大きい", 0, x => (double.TryParse(x, out double result), result));
        }

        /// <summary>
        /// <see cref="long"/>に対する<see cref="LowerValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LowerValueCondition{T}"/>の新しいインスタンス</returns>
        public static LowerValueCondition<long> LowerAsInteger()
        {
            return new LowerValueCondition<long>(ArgumentType.Integer64, "指定した整数より小さい", 0, x => (long.TryParse(x, out long result), result));
        }

        /// <summary>
        /// <see cref="double"/>に対する<see cref="LowerValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LowerValueCondition{T}"/>の新しいインスタンス</returns>
        public static LowerValueCondition<double> LowerAsDecimal()
        {
            return new LowerValueCondition<double>(ArgumentType.Decimal, "指定した数値より小さい", 0, x => (double.TryParse(x, out double result), result));
        }

        /// <summary>
        /// <see cref="long"/>に対する<see cref="LargerOrEqualValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LargerValueCondition{T}"/>の新しいインスタンス</returns>
        public static LargerOrEqualValueCondition<long> LargerOrEqualAsInteger()
        {
            return new LargerOrEqualValueCondition<long>(ArgumentType.Integer64, "指定した整数以上", 0, x => (long.TryParse(x, out long result), result));
        }

        /// <summary>
        /// <see cref="double"/>に対する<see cref="LargerOrEqualValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LargerValueCondition{T}"/>の新しいインスタンス</returns>
        public static LargerOrEqualValueCondition<double> LargerOrEqualAsDecimal()
        {
            return new LargerOrEqualValueCondition<double>(ArgumentType.Decimal, "指定した数値以上", 0, x => (double.TryParse(x, out double result), result));
        }

        /// <summary>
        /// <see cref="long"/>に対する<see cref="LowerOrEqualValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LargerValueCondition{T}"/>の新しいインスタンス</returns>
        public static LowerOrEqualValueCondition<long> LowerOrEqualAsInteger()
        {
            return new LowerOrEqualValueCondition<long>(ArgumentType.Integer64, "指定した整数以下", 0, x => (long.TryParse(x, out long result), result));
        }

        /// <summary>
        /// <see cref="double"/>に対する<see cref="LowerOrEqualValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LargerValueCondition{T}"/>の新しいインスタンス</returns>
        public static LowerOrEqualValueCondition<double> LowerOrEqualAsDecimal()
        {
            return new LowerOrEqualValueCondition<double>(ArgumentType.Decimal, "指定した数値以下", 0, x => (double.TryParse(x, out double result), result));
        }

        /// <summary>
        /// <see cref="long"/>に対する<see cref="EqualValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LargerValueCondition{T}"/>の新しいインスタンス</returns>
        public static EqualValueCondition<long> EqualAsInteger()
        {
            return new EqualValueCondition<long>(ArgumentType.Integer64, "指定した整数と等しい", 0, x => (long.TryParse(x, out long result), result));
        }

        /// <summary>
        /// <see cref="double"/>に対する<see cref="EqualValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LargerValueCondition{T}"/>の新しいインスタンス</returns>
        public static EqualValueCondition<double> EqualAsDecimal()
        {
            return new EqualValueCondition<double>(ArgumentType.Decimal, "指定した数値と等しい", 0, x => (double.TryParse(x, out double result), result));
        }
    }

    /// <summary>
    /// NOT条件を表します。
    /// </summary>
    [Serializable]
    internal sealed class NotValueCondition : ValueCondition
    {
        /// <summary>
        /// 反転する条件を取得または設定します。
        /// </summary>
        public ValueCondition Condition { get; set; }

        /// <inheritdoc/>
        public override string? Title => "NOT";

        /// <summary>
        /// <see cref="NotValueCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        public NotValueCondition()
        {
            Condition = Null;
        }

        /// <summary>
        /// <see cref="NotValueCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        private NotValueCondition(NotValueCondition cloned)
            : base(cloned)
        {
            Condition = cloned.Condition.Clone();
        }

        /// <inheritdoc/>
        protected override ValueCondition CloneCore() => new NotValueCondition(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.ValueCondition, "条件", () => Condition, x => Condition = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            StatusHelper.VerifyValueCondition(Title, status, Arguments[0], Condition);
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            MatchResult match = Condition.Match(target);
            return match switch
            {
                MatchResult.Matched => MatchResult.NotMatched,
                MatchResult.NotMatched => MatchResult.Matched,
                _ => match,
            };
        }
    }

    /// <summary>
    /// AND条件を表します。
    /// </summary>
    [Serializable]
    internal sealed class AndValueCondition : ValueCondition
    {
        /// <summary>
        /// 対象の条件一覧を取得または設定します。
        /// </summary>
        public ValueCondition[] Conditions { get; set; }

        /// <inheritdoc/>
        public override string? Title => "AND";

        /// <summary>
        /// <see cref="AndValueCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        public AndValueCondition()
        {
            Conditions = new ValueCondition[2];
            Array.Fill(Conditions, Null);
        }

        /// <summary>
        /// <see cref="AndValueCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        private AndValueCondition(AndValueCondition cloned)
            : base(cloned)
        {
            Conditions = cloned.Conditions.CloneAll();
        }

        /// <inheritdoc/>
        protected override ValueCondition CloneCore() => new AndValueCondition(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.ValueCondition | ArgumentType.Array, "条件", () => Conditions, x => Conditions = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            for (int i = 0; i < Conditions.Length; i++)
            {
                StatusHelper.VerifyValueCondition(Title, status, Arguments[0], Conditions[i]);
            }
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            foreach (ValueCondition currentCondition in Conditions)
            {
                MatchResult currentResult = currentCondition.Match(target);
                if (currentResult != MatchResult.Matched) return currentResult;
            }

            return MatchResult.Matched;
        }
    }

    /// <summary>
    /// OR条件を表します。
    /// </summary>
    [Serializable]
    internal sealed class OrValueCondition : ValueCondition
    {
        /// <summary>
        /// 対象の条件一覧を取得または設定します。
        /// </summary>
        public ValueCondition[] Conditions { get; set; }

        /// <inheritdoc/>
        public override string? Title => "OR";

        /// <summary>
        /// <see cref="OrValueCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        public OrValueCondition()
        {
            Conditions = new ValueCondition[2];
            Array.Fill(Conditions, Null);
        }

        /// <summary>
        /// <see cref="OrValueCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        private OrValueCondition(OrValueCondition cloned)
            : base(cloned)
        {
            Conditions = cloned.Conditions.CloneAll();
        }

        /// <inheritdoc/>
        protected override ValueCondition CloneCore() => new OrValueCondition(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.ValueCondition | ArgumentType.Array, "条件", () => Conditions, x => Conditions = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            for (int i = 0; i < Conditions.Length; i++)
            {
                StatusHelper.VerifyValueCondition(Title, status, Arguments[0], Conditions[i]);
            }
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            foreach (ValueCondition currentCondition in Conditions)
            {
                MatchResult currentResult = currentCondition.Match(target);
                if (currentResult != MatchResult.NotMatched) return currentResult;
            }

            return MatchResult.NotMatched;
        }
    }

    /// <summary>
    /// 空文字かどうかを検証する条件のクラスです。
    /// </summary>
    [Serializable]
    internal sealed class IsEmptyValueCondition : ValueCondition
    {
        /// <inheritdoc/>
        public override string? Title => "空欄である";

        /// <inheritdoc/>
        public override MatchResult Match(string target) => string.IsNullOrEmpty(target) ? MatchResult.Matched : MatchResult.NotMatched;
    }

    /// <summary>
    /// 整数かどうかを検証します。
    /// </summary>
    [Serializable]
    internal sealed class IsIntegerValueCondition : ValueCondition
    {
        /// <inheritdoc/>
        public override string? Title => "整数である";

        /// <inheritdoc/>
        public override MatchResult Match(string target) => long.TryParse(target, out _) ? MatchResult.Matched : MatchResult.NotMatched;
    }

    /// <summary>
    /// 小数かどうかを検証します。
    /// </summary>
    [Serializable]
    internal sealed class IsDecimalValueCondition : ValueCondition
    {
        /// <inheritdoc/>
        public override string? Title => "数値である";

        /// <inheritdoc/>
        public override MatchResult Match(string target) => double.TryParse(target, out _) ? MatchResult.Matched : MatchResult.NotMatched;
    }

    /// <summary>
    /// 指定した値より大きいかどうかを検証します。
    /// </summary>
    /// <typeparam name="T">比較する型</typeparam>
    [Serializable]
    internal sealed class LargerValueCondition<T> : ValueCondition
        where T : IComparable<T>
    {
        private readonly Func<string, (bool success, T valueT)> converter;
        private readonly ArgumentType type;

        /// <summary>
        /// 比較対象の値を取得または設定します。
        /// </summary>
        public T Comparison { get; set; }

        /// <inheritdoc/>
        public override string? Title { get; }

        /// <summary>
        /// <see cref="LargerValueCondition{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="type">要素の種類</param>
        /// <param name="title">表示名</param>
        /// <param name="comparison">比較対象</param>
        /// <param name="converter">文字列からの変換関数</param>
        /// <exception cref="ArgumentNullException"><paramref name="converter"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="type"/>が未定義の値</exception>
        public LargerValueCondition(ArgumentType type, string? title, T comparison, Func<string, (bool, T)> converter)
        {
            ArgumentNullException.ThrowIfNull(converter);
            if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));

            this.type = type;
            this.Comparison = comparison;
            this.converter = converter;
            Title = title;
        }

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(type, "値", () => Comparison, x => Comparison = x),
            };
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            (bool success, T valueT) = converter.Invoke(target);
            if (!success) return MatchResult.Error;
            return valueT.CompareTo(Comparison) > 0 ? MatchResult.Matched : MatchResult.NotMatched;
        }
    }

    /// <summary>
    /// 指定した値より小さいかどうかを検証します。
    /// </summary>
    /// <typeparam name="T">比較する型</typeparam>
    [Serializable]
    internal sealed class LowerValueCondition<T> : ValueCondition
        where T : IComparable<T>
    {
        private readonly Func<string, (bool success, T valueT)> converter;
        private readonly ArgumentType type;

        /// <summary>
        /// 比較対象の値を取得または設定します。
        /// </summary>
        public T Comparison { get; set; }

        /// <inheritdoc/>
        public override string? Title { get; }

        /// <summary>
        /// <see cref="LowerValueCondition{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="type">要素の種類</param>
        /// <param name="title">表示名</param>
        /// <param name="comparison">比較対象</param>
        /// <param name="converter">文字列からの変換関数</param>
        /// <exception cref="ArgumentNullException"><paramref name="converter"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="type"/>が未定義の値</exception>
        public LowerValueCondition(ArgumentType type, string? title, T comparison, Func<string, (bool, T)> converter)
        {
            ArgumentNullException.ThrowIfNull(converter);
            if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));

            this.type = type;
            this.Comparison = comparison;
            this.converter = converter;
            Title = title;
        }

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(type, "値", () => Comparison, x => Comparison = x),
            };
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            (bool success, T valueT) = converter.Invoke(target);
            if (!success) return MatchResult.Error;
            return valueT.CompareTo(Comparison) < 0 ? MatchResult.Matched : MatchResult.NotMatched;
        }
    }

    /// <summary>
    /// 指定した値以上かどうかを検証します。
    /// </summary>
    /// <typeparam name="T">比較する型</typeparam>
    [Serializable]
    internal sealed class LargerOrEqualValueCondition<T> : ValueCondition
        where T : IComparable<T>
    {
        private readonly Func<string, (bool success, T valueT)> converter;
        private readonly ArgumentType type;

        /// <summary>
        /// 比較対象の値を取得または設定します。
        /// </summary>
        public T Comparison { get; set; }

        /// <inheritdoc/>
        public override string? Title { get; }

        /// <summary>
        /// <see cref="LargerOrEqualValueCondition{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="type">要素の種類</param>
        /// <param name="title">表示名</param>
        /// <param name="comparison">比較対象</param>
        /// <param name="converter">文字列からの変換関数</param>
        /// <exception cref="ArgumentNullException"><paramref name="converter"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="type"/>が未定義の値</exception>
        public LargerOrEqualValueCondition(ArgumentType type, string? title, T comparison, Func<string, (bool, T)> converter)
        {
            ArgumentNullException.ThrowIfNull(converter);
            if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));

            this.type = type;
            this.Comparison = comparison;
            this.converter = converter;
            Title = title;
        }

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(type, "値", () => Comparison, x => Comparison = x),
            };
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            (bool success, T valueT) = converter.Invoke(target);
            if (!success) return MatchResult.Error;
            return valueT.CompareTo(Comparison) >= 0 ? MatchResult.Matched : MatchResult.NotMatched;
        }
    }

    /// <summary>
    /// 指定した値以下かどうかを検証します。
    /// </summary>
    /// <typeparam name="T">比較する型</typeparam>
    [Serializable]
    internal sealed class LowerOrEqualValueCondition<T> : ValueCondition
        where T : IComparable<T>
    {
        private readonly Func<string, (bool success, T valueT)> converter;
        private readonly ArgumentType type;

        /// <summary>
        /// 比較対象の値を取得または設定します。
        /// </summary>
        public T Comparison { get; set; }

        /// <inheritdoc/>
        public override string? Title { get; }

        /// <summary>
        /// <see cref="LowerOrEqualValueCondition{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="type">要素の種類</param>
        /// <param name="title">表示名</param>
        /// <param name="comparison">比較対象</param>
        /// <param name="converter">文字列からの変換関数</param>
        /// <exception cref="ArgumentNullException"><paramref name="converter"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="type"/>が未定義の値</exception>
        public LowerOrEqualValueCondition(ArgumentType type, string? title, T comparison, Func<string, (bool, T)> converter)
        {
            ArgumentNullException.ThrowIfNull(converter);
            if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));

            this.type = type;
            this.Comparison = comparison;
            this.converter = converter;
            Title = title;
        }

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(type, "値", () => Comparison, x => Comparison = x),
            };
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            (bool success, T valueT) = converter.Invoke(target);
            if (!success) return MatchResult.Error;
            return valueT.CompareTo(Comparison) <= 0 ? MatchResult.Matched : MatchResult.NotMatched;
        }
    }

    /// <summary>
    /// 指定した値と等しいかどうかを検証します。
    /// </summary>
    /// <typeparam name="T">比較する型</typeparam>
    [Serializable]
    internal sealed class EqualValueCondition<T> : ValueCondition
        where T : IEquatable<T>
    {
        private readonly Func<string, (bool success, T valueT)> converter;
        private readonly ArgumentType type;

        /// <summary>
        /// 比較対象の値を取得または設定します。
        /// </summary>
        public T Comparison { get; set; }

        /// <inheritdoc/>
        public override string? Title { get; }

        /// <summary>
        /// <see cref="EqualValueCondition{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="type">要素の種類</param>
        /// <param name="title">表示名</param>
        /// <param name="comparison">比較対象</param>
        /// <param name="converter">文字列からの変換関数</param>
        /// <exception cref="ArgumentNullException"><paramref name="converter"/>が<see langword="null"/></exception>
        public EqualValueCondition(ArgumentType type, string? title, T comparison, Func<string, (bool, T)> converter)
        {
            ArgumentNullException.ThrowIfNull(converter);
            if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));

            this.type = type;
            this.Comparison = comparison;
            this.converter = converter;
            Title = title;
        }

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(type, "値", () => Comparison, x => Comparison = x),
            };
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            (bool success, T valueT) = converter.Invoke(target);
            if (!success) return MatchResult.Error;
            return valueT.Equals(Comparison) ? MatchResult.Matched : MatchResult.NotMatched;
        }
    }

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
