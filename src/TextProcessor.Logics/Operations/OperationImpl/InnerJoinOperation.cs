using System;
using System.Collections.Generic;
using System.Linq;
using TextProcessor.Logics.Data;

namespace TextProcessor.Logics.Operations.OperationImpl
{
    /// <summary>
    /// 内部結合を行う処理のクラスです。
    /// </summary>
    [Serializable]
    internal class InnerJoinOperation : Operation
    {
        /// <summary>
        /// キーの列インデックスを取得または設定します。
        /// </summary>
        public int KeyIndex { get; set; }

        /// <summary>
        /// 結合するデータを取得または設定します。
        /// </summary>
        public TextData? Target { get; set; }

        /// <summary>
        /// <see cref="Target"/>におけるキーの列インデックスを取得または設定します。
        /// </summary>
        public int TargetKeyIndex { get; set; }

        /// <summary>
        /// 大文字小文字を区別するかどうかを表す値を取得または設定します。
        /// </summary>
        public bool CaseSensitive { get; set; } = true;

        /// <summary>
        /// 結合後キーの列を削除するかどうかを表す値を取得または設定します。
        /// </summary>
        public bool RemoveKey { get; set; }

        /// <inheritdoc/>
        public override string Title => "他データと内部結合";

        /// <summary>
        /// <see cref="InnerJoinOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        public InnerJoinOperation()
        {
        }

        /// <summary>
        /// <see cref="InnerJoinOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected InnerJoinOperation(InnerJoinOperation cloned)
            : base(cloned)
        {
            KeyIndex = cloned.KeyIndex;
            Target = cloned.Target;
            TargetKeyIndex = cloned.TargetKeyIndex;
            CaseSensitive = cloned.CaseSensitive;
        }

        /// <inheritdoc/>
        protected override Operation CloneCore() => new InnerJoinOperation(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.Index, "キーの列番号", () => KeyIndex, x => KeyIndex = x),
                new ArgumentInfo(ArgumentType.TextData, "結合するファイル", () => Target!, x => Target = x),
                new ArgumentInfo(ArgumentType.Index, "キーの列番号", () => TargetKeyIndex, x => TargetKeyIndex = x),
                new ArgumentInfo(ArgumentType.Boolean, "大文字小文字の区別", () => CaseSensitive, x => CaseSensitive = x),
                new ArgumentInfo(ArgumentType.Boolean, "キー列を削除する", () => RemoveKey, x => RemoveKey = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (Target is null) status.Errors.Add(new StatusEntry(Title, Arguments[1], "結合するファイルが指定されていません"));
        }

        /// <inheritdoc/>
        protected override void OperateCore(TextData data, ProcessStatus status)
        {
            if (Target is null) return;

            IEqualityComparer<string> comparer = CaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
            List<List<string>> dataList = data.GetSourceData();
            List<List<string>> targetList = Target.GetSourceData();
            int startCount = dataList.Count;
            int joinOffset = data.ColumnCount;
            int dataStart = data.HasHeader ? 1 : 0;
            int targetStart = Target.HasHeader ? 1 : 0;

            if (targetStart == 1)
            {
                List<string> dataHeader;
                List<string> targetHeader = targetList[0].ToList();
                if (TargetKeyIndex < targetHeader.Count) targetHeader.RemoveAt(TargetKeyIndex);
                if (dataStart == 0)
                {
                    dataHeader = new List<string>(joinOffset + targetHeader.Count);
                    dataList.Insert(0, dataHeader);
                    var blankArray = new string[joinOffset];
                    Array.Fill(blankArray, string.Empty);

                    dataHeader.AddRange(blankArray);
                    dataHeader.AddRange(targetHeader);
                    data.HasHeader = true;
                    dataStart++;
                }
                else
                {
                    dataHeader = dataList[0];
                    dataHeader.EnsureCapacity(joinOffset + targetHeader.Count);
                    if (dataHeader.Count < joinOffset)
                    {
                        int blankCount = joinOffset - dataHeader.Count;
                        var blankArray = new string[blankCount];
                        Array.Fill(blankArray, string.Empty);

                        dataHeader.AddRange(blankArray);
                    }
                }
                dataHeader.AddRange(targetHeader);
            }

            List<List<string>> resultList = dataList.Skip(dataStart)
                                                    .Join(targetList.Skip(targetStart), x => KeyIndex < x.Count ? x[KeyIndex] : string.Empty, x => TargetKeyIndex < x.Count ? x[TargetKeyIndex] : string.Empty, (x, y) => CombineRow(x, y, joinOffset), comparer)
                                                    .ToList();
            if (data.HasHeader) dataList.RemoveRange(1, dataList.Count - 1);
            else dataList.Clear();
            dataList.AddRange(resultList);

            if (RemoveKey)
                for (int rowIndex = 0; rowIndex < dataList.Count; rowIndex++)
                    dataList[rowIndex].RemoveAt(KeyIndex);

            if (startCount > (data.HasHeader ? 1 : 0) && dataList.Count == dataStart) status.Warnings.Add(new StatusEntry(Title, null, "結合できるレコードがありません"));
        }

        /// <summary>
        /// 列を結合します。
        /// </summary>
        /// <param name="outer">外部セットの列</param>
        /// <param name="inner">内部セットの列</param>
        /// <param name="joinOffset"><paramref name="inner"/>の値を挿入する位置</param>
        /// <returns>結合後の列</returns>
        private List<string> CombineRow(List<string> outer, List<string> inner, int joinOffset)
        {
            List<string> outerList = outer.ToList();
            List<string> innerList = inner.ToList();
            if (TargetKeyIndex < inner.Count) innerList.RemoveAt(TargetKeyIndex);
            outerList.EnsureCapacity(outerList.Count + innerList.Count);
            if (outerList.Count < joinOffset)
            {
                int blankCount = joinOffset - outerList.Count;
                var blankArray = new string[blankCount];
                Array.Fill(blankArray, string.Empty);
                outerList.AddRange(blankArray);
            }
            outerList.AddRange(innerList);
            return outerList;
        }
    }
}
