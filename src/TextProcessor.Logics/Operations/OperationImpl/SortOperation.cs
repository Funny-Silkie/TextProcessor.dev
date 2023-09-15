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
        /// <inheritdoc/>
        public override string Title => "並び替え";

        /// <summary>
        /// ソート条件一覧を取得または設定します。
        /// </summary>
        public List<SortEntry> Entries { get; set; }

        /// <summary>
        /// <see cref="SortOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        public SortOperation()
        {
            Entries = new List<SortEntry>()
            {
                new SortEntry(),
            };
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
            Entries = cloned.Entries.CloneAll();
        }

        /// <inheritdoc/>
        protected override Operation CloneCore()
        {
            return new SortOperation(this);
        }

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            var arg0 = new ArgumentInfo(ArgumentType.Arguments | ArgumentType.List, "ソート条件", () => Entries, x => Entries = x);
            arg0.SetCtor(() => new SortEntry());
            return new[]
            {
                arg0,
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (Entries.Count == 0) status.Errors.Add(new StatusEntry(Title, Arguments[0], "ソート条件が記述されていません"));
        }

        /// <inheritdoc/>
        protected override void OperateCore(TextData data, ProcessStatus status)
        {
            if (Entries.Count == 0) return;

            List<IComparer<List<string>>> comparers = Entries.ConvertAll(x => x.GetComparer());
            List<List<string>> list = data.GetSourceData();
            int offset = data.HasHeader ? 1 : 0;
            Comparison<List<string>> comparison = (x, y) =>
            {
                foreach (IComparer<List<string>> current in comparers)
                {
                    int result = current.Compare(x, y);
                    if (result != 0) return result;
                }
                return 0;
            };
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

        /// <summary>
        /// ソート条件を表します。
        /// </summary>
        [Serializable]
        public sealed class SortEntry : ICloneable, IHasArguments
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

            /// <summary>
            /// 引数一覧を取得します。
            /// </summary>
            public IList<ArgumentInfo> Arguments => _arguments ??= GenerateArguments();

            [NonSerialized]
            private IList<ArgumentInfo>? _arguments;

            /// <summary>
            /// <see cref="SortEntry"/>の新しいインスタンスを初期化します。
            /// </summary>
            public SortEntry()
            {
            }

            /// <summary>
            /// <see cref="SortEntry"/>の新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="cloned">複製するインスタンス</param>
            /// <remarks>クローン用</remarks>
            /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
            private SortEntry(SortEntry cloned)
            {
                ArgumentNullException.ThrowIfNull(cloned);
            }

            /// <inheritdoc cref="ICloneable.Clone"/>
            public SortEntry Clone() => new SortEntry(this);

            object ICloneable.Clone() => Clone();

            /// <summary>
            /// 引数情報を生成します。
            /// </summary>
            /// <returns>引数情報一覧</returns>
            private IList<ArgumentInfo> GenerateArguments()
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
            public ProcessStatus VerifyArguments()
            {
                return new ProcessStatus();
            }

            /// <summary>
            /// 対応する<see cref="IComparer{T}"/>を取得します。
            /// </summary>
            /// <returns>対応する<see cref="IComparer{T}"/>のインスタンス</returns>
            internal IComparer<List<string>> GetComparer()
            {
                IComparer<List<string>> result = AsNumber ? new IndexedNumberComparer(KeyIndex) : new IndexedStringComparer(!CaseSensitive, KeyIndex);
                return AsReversed ? Comparer<List<string>>.Create((x, y) => -result.Compare(x, y)) : result;
            }
        }
    }
}
