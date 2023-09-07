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
                new LargerValueCondition<long>(ArgumentType.Integer64, "指定した整数より大きい", 0, x => (long.TryParse(x, out long result), result)),
                new LargerValueCondition<double>(ArgumentType.Decimal, "指定した数値より大きい", 0, x => (double.TryParse(x, out double result), result)),
                new LowerValueCondition<long>(ArgumentType.Integer64, "指定した整数より小さい", 0, x => (long.TryParse(x, out long result), result)),
                new LowerValueCondition<double>(ArgumentType.Decimal, "指定した数値より小さい", 0, x => (double.TryParse(x, out double result), result)),
                new LargerOrEqualValueCondition<long>(ArgumentType.Integer64, "指定した整数以上", 0, x => (long.TryParse(x, out long result), result)),
                new LargerOrEqualValueCondition<double>(ArgumentType.Decimal, "指定した数値以上", 0, x => (double.TryParse(x, out double result), result)),
                new LowerOrEqualValueCondition<long>(ArgumentType.Integer64, "指定した整数以下", 0, x => (long.TryParse(x, out long result), result)),
                new LowerOrEqualValueCondition<double>(ArgumentType.Decimal, "指定した数値以下", 0, x => (double.TryParse(x, out double result), result)),
                new EqualValueCondition<long>(ArgumentType.Integer64, "指定した整数と等しい", 0, x => (long.TryParse(x, out long result), result)),
                new EqualValueCondition<double>(ArgumentType.Decimal, "指定した数値と等しい", 0, x => (double.TryParse(x, out double result), result)),
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
    /// NOT条件を表します。
    /// </summary>
    [Serializable]
    internal sealed class NotValueCondition : ValueCondition
    {
        private ValueCondition condition;

        /// <inheritdoc/>
        public override string? Title => "NOT";

        /// <summary>
        /// <see cref="NotValueCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        public NotValueCondition()
        {
            condition = Null;
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
            condition = cloned.condition.Clone();
        }

        /// <inheritdoc/>
        protected override ValueCondition CloneCore() => new NotValueCondition(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.ValueCondition, "条件", () => condition, x => condition = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            StatusHelper.VerifyValueCondition(Title, status, Arguments[0], condition);
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            MatchResult match = condition.Match(target);
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
        private ValueCondition[] conditions;

        /// <inheritdoc/>
        public override string? Title => "AND";

        /// <summary>
        /// <see cref="AndValueCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        public AndValueCondition()
        {
            conditions = new ValueCondition[2];
            Array.Fill(conditions, Null);
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
            conditions = cloned.conditions.CloneAll();
        }

        /// <inheritdoc/>
        protected override ValueCondition CloneCore() => new AndValueCondition(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.ValueCondition | ArgumentType.Array, "条件", () => conditions, x => conditions = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                StatusHelper.VerifyValueCondition(Title, status, Arguments[0], conditions[i]);
            }
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            foreach (ValueCondition currentCondition in conditions)
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
        private ValueCondition[] conditions;

        /// <inheritdoc/>
        public override string? Title => "OR";

        /// <summary>
        /// <see cref="OrValueCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        public OrValueCondition()
        {
            conditions = new ValueCondition[2];
            Array.Fill(conditions, Null);
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
            conditions = cloned.conditions.CloneAll();
        }

        /// <inheritdoc/>
        protected override ValueCondition CloneCore() => new OrValueCondition(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.ValueCondition | ArgumentType.Array, "条件", () => conditions, x => conditions = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                StatusHelper.VerifyValueCondition(Title, status, Arguments[0], conditions[i]);
            }
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            foreach (ValueCondition currentCondition in conditions)
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
        private T comparison;
        private readonly Func<string, (bool success, T valueT)> converter;
        private readonly ArgumentType type;

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
            this.comparison = comparison;
            this.converter = converter;
            Title = title;
        }

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(type, "値", () => comparison, x => comparison = x),
            };
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            (bool success, T valueT) = converter.Invoke(target);
            if (!success) return MatchResult.Error;
            return valueT.CompareTo(comparison) > 0 ? MatchResult.Matched : MatchResult.NotMatched;
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
        private T comparison;
        private readonly Func<string, (bool success, T valueT)> converter;
        private readonly ArgumentType type;

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
            this.comparison = comparison;
            this.converter = converter;
            Title = title;
        }

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(type, "値", () => comparison, x => comparison = x),
            };
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            (bool success, T valueT) = converter.Invoke(target);
            if (!success) return MatchResult.Error;
            return valueT.CompareTo(comparison) < 0 ? MatchResult.Matched : MatchResult.NotMatched;
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
        private T comparison;
        private readonly Func<string, (bool success, T valueT)> converter;
        private readonly ArgumentType type;

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
            this.comparison = comparison;
            this.converter = converter;
            Title = title;
        }

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(type, "値", () => comparison, x => comparison = x),
            };
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            (bool success, T valueT) = converter.Invoke(target);
            if (!success) return MatchResult.Error;
            return valueT.CompareTo(comparison) >= 0 ? MatchResult.Matched : MatchResult.NotMatched;
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
        private T comparison;
        private readonly Func<string, (bool success, T valueT)> converter;
        private readonly ArgumentType type;

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
            this.comparison = comparison;
            this.converter = converter;
            Title = title;
        }

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(type, "値", () => comparison, x => comparison = x),
            };
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            (bool success, T valueT) = converter.Invoke(target);
            if (!success) return MatchResult.Error;
            return valueT.CompareTo(comparison) <= 0 ? MatchResult.Matched : MatchResult.NotMatched;
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
        private T comparison;
        private readonly Func<string, (bool success, T valueT)> converter;
        private readonly ArgumentType type;

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
            this.comparison = comparison;
            this.converter = converter;
            Title = title;
        }

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(type, "値", () => comparison, x => comparison = x),
            };
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            (bool success, T valueT) = converter.Invoke(target);
            if (!success) return MatchResult.Error;
            return valueT.Equals(comparison) ? MatchResult.Matched : MatchResult.NotMatched;
        }
    }

    /// <summary>
    /// 指定した文字列から始まるかどうかを検証します。
    /// </summary>
    [Serializable]
    internal sealed class StartsWithValueCondition : ValueCondition
    {
        private string comparison = string.Empty;
        private bool caseSensitive = true;

        /// <inheritdoc/>
        public override string? Title => "指定した文字列で始まる";

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.String, "検索文字列", () => comparison, x => comparison = x),
                new ArgumentInfo(ArgumentType.Boolean, "大文字小文字の区別", () => caseSensitive, x => caseSensitive = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (string.IsNullOrEmpty(comparison)) status.Errors.Add(new StatusEntry(Title, Arguments[0], "検索文字列が指定されていません"));
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            return target.StartsWith(comparison, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) ? MatchResult.Matched : MatchResult.NotMatched;
        }
    }

    /// <summary>
    /// 指定した文字列から終わるかどうかを検証します。
    /// </summary>
    [Serializable]
    internal sealed class EndsWithValueCondition : ValueCondition
    {
        private string comparison = string.Empty;
        private bool caseSensitive = true;

        /// <inheritdoc/>
        public override string? Title => "指定した文字列で終わる";

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.String, "検索文字列", () => comparison, x => comparison = x),
                new ArgumentInfo(ArgumentType.Boolean, "大文字小文字の区別", () => caseSensitive, x => caseSensitive = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (string.IsNullOrEmpty(comparison)) status.Errors.Add(new StatusEntry(Title, Arguments[0], "検索文字列が指定されていません"));
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            return target.EndsWith(comparison, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) ? MatchResult.Matched : MatchResult.NotMatched;
        }
    }

    /// <summary>
    /// 指定した文字列を含むかどうかを検証します。
    /// </summary>
    [Serializable]
    internal sealed class ContainsValueCondition : ValueCondition
    {
        private string comparison = string.Empty;
        private bool caseSensitive = true;

        /// <inheritdoc/>
        public override string? Title => "指定した文字列を含む";

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.String, "検索文字列", () => comparison, x => comparison = x),
                new ArgumentInfo(ArgumentType.Boolean, "大文字小文字の区別", () => caseSensitive, x => caseSensitive = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (string.IsNullOrEmpty(comparison)) status.Errors.Add(new StatusEntry(Title, Arguments[0], "検索文字列が指定されていません"));
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            return target.Contains(comparison, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) ? MatchResult.Matched : MatchResult.NotMatched;
        }
    }

    /// <summary>
    /// 指定した文字列と完全一致するどうかを検証します。
    /// </summary>
    [Serializable]
    internal sealed class MatchValueCondition : ValueCondition
    {
        private string comparison = string.Empty;
        private bool caseSensitive = true;

        /// <inheritdoc/>
        public override string? Title => "指定した文字列と一致";

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.String, "文字列", () => comparison, x => comparison = x),
                new ArgumentInfo(ArgumentType.Boolean, "大文字小文字の区別", () => caseSensitive, x => caseSensitive = x),
            };
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            return string.Equals(target, comparison, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) ? MatchResult.Matched : MatchResult.NotMatched;
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
        private string Pattern
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

        private bool caseSensitive = true;

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
                    if (!caseSensitive) options |= RegexOptions.IgnoreCase;
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
                new ArgumentInfo(ArgumentType.Boolean, "大文字小文字の区別", () => caseSensitive, x => caseSensitive = x),
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
