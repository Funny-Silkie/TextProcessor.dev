using System;
using System.Collections.Generic;
using TextProcessor.Logics.Data;

namespace TextProcessor.Logics.Operations.OperationImpl
{
    /// <summary>
    /// 重複を削除する処理のクラスです。
    /// </summary>
    [Serializable]
    internal class DistinctOperation : Operation
    {
        private bool isAll;
        private int key;
        private bool caseSensitive = true;

        /// <inheritdoc/>
        public override string Title => "重複を削除";

        /// <summary>
        /// <see cref="DistinctOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        public DistinctOperation()
        {
        }

        /// <summary>
        /// <see cref="DistinctOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected DistinctOperation(DistinctOperation cloned)
            : base(cloned)
        {
            isAll = cloned.isAll;
            key = cloned.key;
            caseSensitive = cloned.caseSensitive;
        }

        /// <inheritdoc/>
        protected override Operation CloneCore() => new DistinctOperation(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.Boolean, "全ての列を比較", () => isAll, x => isAll = x),
                new ArgumentInfo(ArgumentType.Index, "比較する列番号", () => key, x => key = x),
                new ArgumentInfo(ArgumentType.Boolean, "大文字小文字の区別", () => caseSensitive, x => caseSensitive = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
        }

        /// <inheritdoc/>
        protected override void OperateCore(TextData data, ProcessStatus status)
        {
            List<List<string>> list = data.GetSourceData();
            IEqualityComparer<List<string>> comparer = isAll ? new AllValueRowEqualityComparer(!caseSensitive) : new IndexedRowEqualityComparer(!caseSensitive, key);
            var set = new HashSet<List<string>>(list.Count, comparer);
            int index = 0;
            int removedCount = 0;
            while (index < list.Count)
            {
                List<string> current = list[index];
                if (set.Add(current))
                {
                    index++;
                    continue;
                }
                list.RemoveAt(index);
                removedCount++;
            }

            if (removedCount > 0) status.Messages.Add(new StatusEntry(Title, null, $"{removedCount}個のレコードを削除しました"));
            else status.Messages.Add(new StatusEntry(Title, null, "既に一意のレコードです"));
        }

        /// <summary>
        /// 指定した列を比較する<see cref="IEqualityComparer{T}"/>のクラスです。
        /// </summary>
        private sealed class IndexedRowEqualityComparer : IEqualityComparer<List<string>>
        {
            private readonly int columnIndex;
            private readonly StringComparer comparer;

            /// <summary>
            /// <see cref="IndexedRowEqualityComparer"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="ignoreCase">大文字小文字を無視するかどうか</param>
            /// <param name="columnIndex">列番号</param>
            public IndexedRowEqualityComparer(bool ignoreCase, int columnIndex)
            {
                comparer = ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
                this.columnIndex = columnIndex;
            }

            /// <inheritdoc/>
            public bool Equals(List<string>? x, List<string>? y)
            {
                if (x is null) return y is null;
                if (y is null) return false;
                if (x == y) return true;

                string sx = columnIndex < x.Count ? x[columnIndex] : string.Empty;
                string sy = columnIndex < y.Count ? y[columnIndex] : string.Empty;
                return comparer.Equals(sx, sy);
            }

            /// <inheritdoc/>
            public int GetHashCode(List<string> obj)
            {
                ArgumentNullException.ThrowIfNull(obj);

                if (columnIndex >= obj.Count) return 0;
                string value = obj[columnIndex];
                if (string.IsNullOrEmpty(value)) return 0;
                return comparer.GetHashCode(value);
            }
        }

        /// <summary>
        /// 全ての列を比較する<see cref="IEqualityComparer{T}"/>のクラスです。
        /// </summary>
        private sealed class AllValueRowEqualityComparer : IEqualityComparer<List<string>>
        {
            private readonly StringComparer comparer;

            /// <summary>
            /// <see cref="AllValueRowEqualityComparer"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="ignoreCase">大文字小文字を無視するかどうか</param>
            public AllValueRowEqualityComparer(bool ignoreCase)
            {
                comparer = ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
            }

            /// <summary>
            /// 空白でない最後のインデックスを取得します。
            /// </summary>
            /// <param name="list">検証するリスト</param>
            /// <returns><paramref name="list"/>における最後の空文字でない値のインデックス</returns>
            private static int GetEndIndex(List<string> list)
            {
                return list.FindLastIndex(x => !string.IsNullOrEmpty(x));
            }

            /// <inheritdoc/>
            public bool Equals(List<string>? x, List<string>? y)
            {
                if (x is null) return y is null;
                if (y is null) return false;
                if (x == y) return true;

                int endIndexX = GetEndIndex(x);
                int endIndexY = GetEndIndex(y);
                if (endIndexX != endIndexY) return false;
                for (int i = 0; i < endIndexX; i++)
                    if (!comparer.Equals(x[i], y[i]))
                        return false;
                return true;
            }

            /// <inheritdoc/>
            public int GetHashCode(List<string> obj)
            {
                int endIndex = GetEndIndex(obj);
                var hashCode = new HashCode();
                for (int i = 0; i < endIndex; i++)
                {
                    string current = obj[i];
                    hashCode.Add(string.IsNullOrEmpty(current) ? null : current, comparer);
                }
                return hashCode.ToHashCode();
            }
        }
    }
}
