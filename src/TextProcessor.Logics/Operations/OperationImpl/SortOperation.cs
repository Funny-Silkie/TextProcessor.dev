using System;
using System.Collections.Generic;
using TextProcessor.Logics.Data;

namespace TextProcessor.Logics.Operations.OperationImpl
{
    /// <summary>
    /// ソートを行う処理のクラスです。
    /// </summary>
    [Serializable]
    internal class SortOperation : Operation
    {
        /// <summary>
        /// キーの列インデックスを取得または設定します。
        /// </summary>
        public int KeyIndex { get; set; }

        /// <summary>
        /// 数値として比較を行うかどうかを表す値を取得または設定します。
        /// </summary>
        public bool AsNumber { get; set; }

        /// <summary>
        /// 逆順ソートを行うかどうかを表す値を取得または設定します。
        /// </summary>
        public bool AsReversed { get; set; }

        /// <summary>
        /// 大文字小文字を区別するかどうかを表す値を取得または設定します。
        /// </summary>
        public bool CaseSensitive { get; set; } = true;

        /// <inheritdoc/>
        public override string Title => "並び替え";

        /// <summary>
        /// <see cref="SortOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        public SortOperation()
        {
        }

        /// <summary>
        /// <see cref="SortOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected SortOperation(SortOperation cloned)
            : base(cloned)
        {
            KeyIndex = cloned.KeyIndex;
            CaseSensitive = cloned.CaseSensitive;
            AsNumber = cloned.AsNumber;
            AsReversed = cloned.AsReversed;
        }

        /// <inheritdoc/>
        protected override Operation CloneCore()
        {
            return new SortOperation(this);
        }

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.Index, "キーの列番号", () => KeyIndex, x => KeyIndex = x),
                new ArgumentInfo(ArgumentType.Boolean, "逆順ソート", () => AsReversed, x => AsReversed = x),
                new ArgumentInfo(ArgumentType.Boolean, "数値として比較", () => AsNumber, x => AsNumber = x),
                new ArgumentInfo(ArgumentType.Boolean, "大文字小文字の区別", () => CaseSensitive, x => CaseSensitive = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
        }

        /// <inheritdoc/>
        protected override void OperateCore(TextData data, ProcessStatus status)
        {
            IComparer<List<string>> comparer = AsNumber ? new IndexedNumberComparer(KeyIndex) : new IndexedStringComparer(!CaseSensitive, KeyIndex);
            List<List<string>> list = data.GetSourceData();
            int offset = data.HasHeader ? 1 : 0;
            Comparison<List<string>> comparison = AsReversed ? (x, y) => comparer.Compare(y, x) : comparer.Compare;
            list.Sort(offset, list.Count - offset, Comparer<List<string>>.Create(comparison));
        }

        /// <summary>
        /// 指定した列を文字列として比較する<see cref="IComparer{T}"/>のクラスです。
        /// </summary>
        private sealed class IndexedStringComparer : IComparer<List<string>>
        {
            private readonly StringComparer comparer;
            private readonly int columnIndex;

            /// <summary>
            /// <see cref="IndexedStringComparer"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="ignoreCase">大文字小文字を無視するかどうか</param>
            /// <param name="columnIndex">列番号</param>
            public IndexedStringComparer(bool ignoreCase, int columnIndex)
            {
                comparer = ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
                this.columnIndex = columnIndex;
            }

            /// <inheritdoc/>
            public int Compare(List<string>? x, List<string>? y)
            {
                if (x is null) return y is null ? 0 : -1;
                if (y is null) return 1;

                string? sx = columnIndex < x.Count ? x[columnIndex] : null;
                string? sy = columnIndex < y.Count ? y[columnIndex] : null;
                return comparer.Compare(sx, sy);
            }
        }

        /// <summary>
        /// 指定した列を数値として比較する<see cref="IComparer{T}"/>のクラスです。
        /// </summary>
        private sealed class IndexedNumberComparer : IComparer<List<string>>
        {
            private readonly int columnIndex;

            /// <summary>
            /// <see cref="IndexedNumberComparer"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="columnIndex">列番号</param>
            public IndexedNumberComparer(int columnIndex)
            {
                this.columnIndex = columnIndex;
            }

            /// <inheritdoc/>
            public int Compare(List<string>? x, List<string>? y)
            {
                if (x is null) return y is null ? 0 : -1;
                if (y is null) return 1;

                string? sx = columnIndex < x.Count ? x[columnIndex] : null;
                string? sy = columnIndex < y.Count ? y[columnIndex] : null;
                bool isXNumber = double.TryParse(sx, out double dx);
                bool isYNumber = double.TryParse(sy, out double dy);
                if (!isXNumber) dx = double.NaN;
                if (!isYNumber) dy = double.NaN;
                return dx.CompareTo(dy);
            }
        }
    }
}
