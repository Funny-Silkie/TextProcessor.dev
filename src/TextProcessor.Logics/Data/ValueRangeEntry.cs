using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TextProcessor.Logics.Data
{
    /// <summary>
    /// 値の範囲を表します。
    /// </summary>
    [Serializable]
    public readonly struct ValueRangeEntry : IEnumerable<int>, IEquatable<ValueRangeEntry>, IComparable<ValueRangeEntry>, IComparable, ISpanParsable<ValueRangeEntry>, ISpanFormattable
    {
        /// <summary>
        /// 範囲を表すトークン文字
        /// </summary>
        internal const char RangeToken = '-';

        /// <summary>
        /// 開始値を取得します。
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// 終了値を取得します。
        /// </summary>
        public int End { get; }

        /// <summary>
        /// 要素数を取得します。
        /// </summary>
        public int Count => End - Start + 1;

        /// <summary>
        /// 一つの値を表すかどうかの値を取得します。
        /// </summary>
        public bool IsSingleValue => Start == End;

        /// <summary>
        /// <see cref="ValueRangeEntry"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="value">値</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/>が0未満</exception>
        public ValueRangeEntry(int value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "値が0未満です");

            Start = value;
            End = value;
        }

        /// <summary>
        /// <see cref="ValueRangeEntry"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="start">開始値</param>
        /// <param name="end">終了値</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>または<paramref name="end"/>が0未満</exception>
        private ValueRangeEntry(int start, int end)
        {
            if (start < 0) throw new ArgumentOutOfRangeException(nameof(start), "値が0未満です");
            if (end < 0) throw new ArgumentOutOfRangeException(nameof(end), "値が0未満です");

            Start = start;
            End = end;
        }

        /// <summary>
        /// 指定した値の範囲を表すインスタンスを生成します。
        /// </summary>
        /// <param name="value1">値1</param>
        /// <param name="value2">値2</param>
        /// <returns><paramref name="value1"/>と<paramref name="value2"/>の間を表すインスタンス</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value1"/>または<paramref name="value2"/>が0未満</exception>
        public static ValueRangeEntry Between(int value1, int value2)
        {
            if (value1 > value2) return new ValueRangeEntry(value2, value1);
            return new ValueRangeEntry(value1, value2);
        }

        /// <inheritdoc/>
        public static ValueRangeEntry Parse(string s, IFormatProvider? provider = null)
        {
            ArgumentNullException.ThrowIfNull(s);

            return Parse(s.AsSpan(), provider);
        }

        /// <inheritdoc/>
        public static ValueRangeEntry Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null)
        {
            if (s.IsEmpty) ThrowAsInvalidFormat();

            s = s.Trim();
            int tokenIndex = s.IndexOf(RangeToken);
            if (tokenIndex < 0)
            {
                int value = int.Parse(s, provider);
                if (value < 0) ThrowAsNegativeValue();
                return value;
            }
            if (tokenIndex == 0 || tokenIndex == s.Length - 1) ThrowAsInvalidFormat();

            int value1 = int.Parse(s[..tokenIndex], provider);
            if (value1 < 0) ThrowAsNegativeValue();

            int value2 = int.Parse(s[(tokenIndex + 1)..], provider);
            if (value2 < 0) ThrowAsNegativeValue();

            return Between(value1, value2);

            [DoesNotReturn]
            static void ThrowAsInvalidFormat() => throw new FormatException("無効なフォーマットです");

            [DoesNotReturn]
            static void ThrowAsNegativeValue() => throw new FormatException("負の値が設定されています");
        }

        /// <inheritdoc cref="TryParse(ReadOnlySpan{char}, out ValueRangeEntry, IFormatProvider?)"/>
        public static bool TryParse([NotNullWhen(true)] string? s, out ValueRangeEntry result, IFormatProvider? provider = null)
        {
            if (s is null)
            {
                result = default;
                return false;
            }

            return TryParse(s.AsSpan(), out result, provider);
        }

        /// <summary>
        /// 文字列からの変換を試みます。
        /// </summary>
        /// <param name="s">変換する文字列</param>
        /// <param name="result">変換後の値，失敗したら既定値</param>
        /// <param name="provider">使用する<see cref="IFormatProvider"/>のインスタンス</param>
        /// <returns><paramref name="result"/>の変換に成功したら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public static bool TryParse(ReadOnlySpan<char> s, out ValueRangeEntry result, IFormatProvider? provider = null)
        {
            if (s.IsEmpty)
            {
                result = default;
                return false;
            }

            s = s.Trim();
            int tokenIndex = s.IndexOf(RangeToken);
            if (tokenIndex < 0)
            {
                bool success = int.TryParse(s, provider, out int value);
                if (value < 0)
                {
                    result = default;
                    return false;
                }
                result = value;
                return success;
            }
            if (tokenIndex == 0 || tokenIndex == s.Length - 1)
            {
                result = default;
                return false;
            }

            if (!int.TryParse(s[..tokenIndex], provider, out int value1) || value1 < 0)
            {
                result = default;
                return false;
            }

            if (!int.TryParse(s[(tokenIndex + 1)..], provider, out int value2) || value2 < 0)
            {
                result = default;
                return false;
            }

            result = Between(value1, value2);
            return true;
        }

        static bool IParsable<ValueRangeEntry>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out ValueRangeEntry result)
        {
            return TryParse(s, out result, provider);
        }

        static bool ISpanParsable<ValueRangeEntry>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out ValueRangeEntry result)
        {
            return TryParse(s, out result, provider);
        }

        /// <inheritdoc/>
        public int CompareTo(ValueRangeEntry other)
        {
            int result = Start.CompareTo(other.Start);
            if (result != 0) return result;
            result = End.CompareTo(other.End);
            return result;
        }

        int IComparable.CompareTo(object? obj)
        {
            if (obj is ValueRangeEntry other) return CompareTo(other);
            if (obj is int single) return CompareTo(single);
            if (obj is null) return 1;

            throw new ArgumentException("無効な型が渡されました", nameof(obj));
        }

        /// <summary>
        /// 指定した値が含まれるかどうかを検証します。
        /// </summary>
        /// <param name="value">検証する値</param>
        /// <returns><paramref name="value"/>が含まれていたら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public bool Contains(int value) => Start <= value && value <= End;

        /// <inheritdoc/>
        public bool Equals(ValueRangeEntry other) => Start == other.Start && End == other.End;

        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object? obj) => obj is ValueRangeEntry other && Equals(other);

        /// <inheritdoc/>
        public IEnumerator<int> GetEnumerator()
        {
            for (int i = Start; i <= End; i++) yield return i;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Start, End);

        /// <summary>
        /// <inheritdoc cref="ISet{T}.Overlaps(IEnumerable{T})"/>
        /// </summary>
        /// <param name="other"><inheritdoc cref="ISet{T}.Overlaps(IEnumerable{T})"/></param>
        /// <returns><inheritdoc cref="ISet{T}.Overlaps(IEnumerable{T})"/></returns>
        public bool Overlaps(ValueRangeEntry other) => Contains(other.Start) || Contains(other.End);

        /// <inheritdoc/>
        public override string ToString() => ToString(null, null);

        /// <inheritdoc/>
        public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
        {
            if (Start == End) return Start.ToString(format, provider);
            return string.Format("{0}" + RangeToken + "{1}", Start.ToString(format, provider), End.ToString(format, provider));
        }

        /// <inheritdoc/>
        public bool TryFormat(Span<char> destination, out int charsWritten, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
        {
            if (Start == End) return Start.TryFormat(destination, out charsWritten, format, provider);

            if (destination.Length < 3)
            {
                charsWritten = 0;
                return false;
            }

            int charsWrittenSoFar = 0;
            bool innerResult = Start.TryFormat(destination, out int charsWrittenInner, format, provider);
            charsWrittenSoFar += charsWrittenInner;

            if (!innerResult || destination.Length - charsWrittenSoFar < 2)
            {
                charsWritten = charsWrittenSoFar;
                return false;
            }

            destination[charsWrittenSoFar++] = RangeToken;

            innerResult = End.TryFormat(destination, out charsWrittenInner, format, provider);
            charsWrittenSoFar += charsWrittenInner;
            charsWritten = charsWrittenSoFar;

            return innerResult;
        }

        /// <inheritdoc cref="System.Numerics.IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
        public static bool operator ==(ValueRangeEntry left, ValueRangeEntry right) => left.Equals(right);

        /// <inheritdoc cref="System.Numerics.IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
        public static bool operator !=(ValueRangeEntry left, ValueRangeEntry right) => !(left == right);

        /// <inheritdoc cref="System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThan(TSelf, TOther)"/>
        public static bool operator <(ValueRangeEntry left, ValueRangeEntry right) => left.CompareTo(right) < 0;

        /// <inheritdoc cref="System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_LessThanOrEqual(TSelf, TOther)"/>
        public static bool operator <=(ValueRangeEntry left, ValueRangeEntry right) => left.CompareTo(right) <= 0;

        /// <inheritdoc cref="System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThan(TSelf, TOther)"/>
        public static bool operator >(ValueRangeEntry left, ValueRangeEntry right) => left.CompareTo(right) > 0;

        /// <inheritdoc cref="System.Numerics.IComparisonOperators{TSelf, TOther, TResult}.op_GreaterThanOrEqual(TSelf, TOther)"/>
        public static bool operator >=(ValueRangeEntry left, ValueRangeEntry right) => left.CompareTo(right) >= 0;

        /// <summary>
        /// <see cref="int"/>から暗黙的に変換します。
        /// </summary>
        /// <param name="value">変換する値</param>
        public static implicit operator ValueRangeEntry(int value) => new ValueRangeEntry(value);
    }
}
