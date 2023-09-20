using System;
using System.Collections.Generic;

namespace TextProcessor.Logics.Operations.Conditions
{
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
}
