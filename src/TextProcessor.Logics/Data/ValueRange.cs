using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace TextProcessor.Logics.Data
{
    /// <summary>
    /// 値の範囲一覧を表します。
    /// </summary>
    [Serializable]
    public readonly struct ValueRange : IEnumerable<int>, IEquatable<ValueRange>, ISpanParsable<ValueRange>, ISpanFormattable
    {
        [StringSyntax(StringSyntaxAttribute.Regex)]
        internal const string FormatRegex = @"\d+(-\d+)?(\s*,\s*\d+(-\d+)?)*";

        internal const char DelimiterChar = ',';

        private ValueRangeEntry[] Items => _items ?? Array.Empty<ValueRangeEntry>();
        private readonly ValueRangeEntry[]? _items;

        /// <summary>
        /// <see cref="ValueRange"/>の新しいインスタンスを初期化します。
        /// </summary>
        public ValueRange()
        {
            _items = Array.Empty<ValueRangeEntry>();
        }

        /// <summary>
        /// <see cref="ValueRange"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="source">使用する値</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="source"/>が0未満</exception>
        public ValueRange(int source)
        {
            _items = new ValueRangeEntry[1] { source };
        }

        /// <summary>
        /// <see cref="ValueRange"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="source">使用するデータ</param>
        private ValueRange(ValueRangeEntry[] source)
        {
            _items = source;
        }

        /// <summary>
        /// <see cref="ValueRange"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="source">使用するデータ</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>が<see langword="false"/></exception>
        public ValueRange(IEnumerable<int> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            if (source is ValueRange other) _items = other.Items.GenericClone();
            else if (source is ValueRangeEntry entry) _items = new ValueRangeEntry[1] { entry };
            else _items = ToRange(source).ToArray();
        }

        /// <inheritdoc/>
        public static ValueRange Parse(string s, IFormatProvider? provider = null)
        {
            ArgumentNullException.ThrowIfNull(s);
            return Parse(s.AsSpan(), provider);
        }

        /// <inheritdoc/>
        public static ValueRange Parse(ReadOnlySpan<char> s, IFormatProvider? provider = null)
        {
            if (s.Length == 0) return default;

            int delimiterIndex = s.IndexOf(DelimiterChar);
            if (delimiterIndex < 0)
            {
                ValueRangeEntry single = ValueRangeEntry.Parse(s, provider);
                return new ValueRange(new[] { single });
            }

            var list = new List<ValueRangeEntry>();

            for (int prevDelimiterIndex = -1; delimiterIndex < s.Length;)
            {
                ReadOnlySpan<char> currentSpan = delimiterIndex < 0 ? s[(prevDelimiterIndex + 1)..] : s[(prevDelimiterIndex + 1)..delimiterIndex];
                ValueRangeEntry currentEntry = ValueRangeEntry.Parse(currentSpan, provider);
                if (list.Count > 0)
                {
                    ValueRangeEntry last = list[^1];
                    if (last.End + 1 == currentEntry.Start) list[^1] = ValueRangeEntry.Between(last.Start, currentEntry.End);
                    else list.Add(currentEntry);
                }
                else list.Add(currentEntry);

                if (delimiterIndex < 0) break;
                prevDelimiterIndex = delimiterIndex;
                int nextIndex = s[(delimiterIndex + 1)..].IndexOf(DelimiterChar);
                if (nextIndex < 0) delimiterIndex = -1;
                else delimiterIndex += nextIndex + 1;
            }

            return new ValueRange(list.ToArray());
        }

        /// <summary>
        /// コレクションを<see cref="ValueRangeEntry"/>のシーケンスに変換します。
        /// </summary>
        /// <param name="source">変換するコレクション</param>
        /// <returns><see cref="ValueRangeEntry"/>のシーケンス</returns>
        private static IEnumerable<ValueRangeEntry> ToRange(IEnumerable<int> source)
        {
            using IEnumerator<int> enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext()) yield break;
            int start = enumerator.Current;
            int current = enumerator.Current;
            for (int old = start; enumerator.MoveNext(); old = current)
            {
                current = enumerator.Current;
                if (current - old == 1) continue;
                yield return ValueRangeEntry.Between(start, old);
                start = current;
            }
            yield return ValueRangeEntry.Between(start, current);
        }

        /// <inheritdoc cref="TryParse(ReadOnlySpan{char}, out ValueRange, IFormatProvider?)"/>
        public static bool TryParse([NotNullWhen(true)] string? s, out ValueRange result, IFormatProvider? provider = null)
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
        public static bool TryParse(ReadOnlySpan<char> s, out ValueRange result, IFormatProvider? provider = null)
        {
            if (s.Length == 0)
            {
                result = default;
                return true;
            }

            int delimiterIndex = s.IndexOf(DelimiterChar);
            if (delimiterIndex < 0)
            {
                if (!ValueRangeEntry.TryParse(s, out ValueRangeEntry single, provider))
                {
                    result = default;
                    return false;
                }
                result = new ValueRange(new[] { single });
                return true;
            }

            var list = new List<ValueRangeEntry>();

            for (int prevDelimiterIndex = -1; delimiterIndex < s.Length;)
            {
                ReadOnlySpan<char> currentSpan = delimiterIndex < 0 ? s[(prevDelimiterIndex + 1)..] : s[(prevDelimiterIndex + 1)..delimiterIndex];
                if (!ValueRangeEntry.TryParse(currentSpan, out ValueRangeEntry currentEntry, provider))
                {
                    result = default;
                    return false;
                }
                if (list.Count > 0)
                {
                    ValueRangeEntry last = list[^1];
                    if (last.End + 1 == currentEntry.Start) list[^1] = ValueRangeEntry.Between(last.Start, currentEntry.End);
                    else list.Add(currentEntry);
                }
                else list.Add(currentEntry);

                if (delimiterIndex < 0) break;
                prevDelimiterIndex = delimiterIndex;
                int nextIndex = s[(delimiterIndex + 1)..].IndexOf(DelimiterChar);
                if (nextIndex < 0) delimiterIndex = -1;
                else delimiterIndex += nextIndex + 1;
            }

            result = new ValueRange(list.ToArray());
            return true;
        }

        static bool IParsable<ValueRange>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out ValueRange result)
        {
            return TryParse(s, out result, provider);
        }

        static bool ISpanParsable<ValueRange>.TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out ValueRange result)
        {
            return TryParse(s, out result, provider);
        }

        /// <summary>
        /// <see cref="ValueRangeEntry"/>のコレクションに変換します。
        /// </summary>
        /// <returns><see cref="ValueRangeEntry"/>のコレクション</returns>
        public IList<ValueRangeEntry> AsEntryCollection() => Array.AsReadOnly(Items);

        /// <summary>
        /// 指定した値が含まれるかどうかを検証します。
        /// </summary>
        /// <param name="value">検証する値</param>
        /// <returns><paramref name="value"/>が含まれていたら<see langword="true"/>，それ以外で<see langword="false"/></returns>
        public bool Contains(int value)
        {
            if (value < 0) return false;
            for (int i = 0; i < Items.Length; i++)
                if (Items[i].Contains(value))
                    return true;
            return false;
        }

        /// <inheritdoc/>
        public bool Equals(ValueRange other) => Items == other.Items || Items.AsSpan().SequenceEqual(other.Items);

        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object? obj) => obj is ValueRange other && Equals(other);

        /// <inheritdoc/>
        public IEnumerator<int> GetEnumerator()
        {
            for (int i = 0; i < Items.Length; i++)
                foreach (int current in Items[i])
                    yield return current;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public override int GetHashCode() => ToString().GetHashCode();

        /// <inheritdoc/>
        public override string ToString() => ToString(null, null);

        /// <inheritdoc/>
        public string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format = null, IFormatProvider? provider = null)
        {
            if (Items.Length == 0) return string.Empty;
            var builder = new StringBuilder(Items.Length * 4);
            builder.Append(Items[0].ToString(format, provider));
            for (int i = 1; i < Items.Length; i++)
            {
                builder.Append(DelimiterChar);
                builder.Append(Items[i].ToString(format, provider));
            }
            return builder.ToString();
        }

        /// <inheritdoc/>
        public bool TryFormat(Span<char> destination, out int charsWritten, [StringSyntax(StringSyntaxAttribute.NumericFormat)] ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
        {
            if (Items.Length == 0)
            {
                charsWritten = 0;
                return true;
            }

            if (Items.Length == 1) return Items[0].TryFormat(destination, out charsWritten, format, provider);

            if (destination.Length < Items.Length * 2 - 1)
            {
                charsWritten = 0;
                return false;
            }

            if (!Items[0].TryFormat(destination, out int charsWrittenSoFar, format, provider))
            {
                charsWritten = charsWrittenSoFar;
                return false;
            }

            for (int i = 1; i < Items.Length; i++)
            {
                if (destination.Length < (Items.Length - i) * 2 - 1)
                {
                    charsWritten = charsWrittenSoFar;
                    return false;
                }
                destination[charsWrittenSoFar++] = DelimiterChar;
                bool innerResult = !Items[i].TryFormat(destination, out int charsWrittenInner, format, provider);
                charsWrittenSoFar += charsWrittenInner;
                if (!innerResult)
                {
                    charsWritten = charsWrittenSoFar;
                    return false;
                }
            }

            charsWritten = charsWrittenSoFar;
            return true;
        }

        /// <inheritdoc cref="System.Numerics.IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
        public static bool operator ==(ValueRange left, ValueRange right) => left.Equals(right);

        /// <inheritdoc cref="System.Numerics.IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
        public static bool operator !=(ValueRange left, ValueRange right) => !(left == right);

        /// <summary>
        /// <see cref="int"/>から暗黙的に変換します。
        /// </summary>
        /// <param name="value">変換する値</param>
        public static implicit operator ValueRange(int value) => new ValueRange(value);

        /// <summary>
        /// <see cref="ValueRangeEntry"/>から暗黙的に変換します。
        /// </summary>
        /// <param name="value">変換する値</param>
        public static implicit operator ValueRange(ValueRangeEntry value) => new ValueRange(value);
    }
}
