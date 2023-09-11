using System;
using System.Collections.Generic;
using TextProcessor.Logics.Data;

namespace TextProcessor.Logics.Operations.OperationImpl
{
    /// <summary>
    /// 列の選択処理を表します。
    /// </summary>
    [Serializable]
    internal class SelectColumnOperation : Operation
    {
        /// <summary>
        /// 対象の列インデックスを取得または設定します。
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <inheritdoc/>
        public override string Title => "指定した列を抽出";

        /// <summary>
        /// <see cref="SelectColumnOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        public SelectColumnOperation()
        {
        }

        /// <summary>
        /// <see cref="SelectColumnOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected SelectColumnOperation(SelectColumnOperation cloned)
            : base(cloned)
        {
            ColumnIndex = cloned.ColumnIndex;
        }

        /// <inheritdoc/>
        protected override Operation CloneCore() => new SelectColumnOperation(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.Index, "列番号", () => ColumnIndex, x => ColumnIndex = x),
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
            int failCount = 0;

            foreach (List<string> row in list)
            {
                if (ColumnIndex >= row.Count)
                {
                    failCount++;
                    row.Clear();
                    row.Add(string.Empty);
                    continue;
                }
                if (ColumnIndex < row.Count - 1) row.RemoveRange(ColumnIndex + 1, row.Count - ColumnIndex - 1);
                row.RemoveRange(0, ColumnIndex);
            }

            if (failCount == list.Count) status.Warnings.Add(new StatusEntry(Title, null, "列番号が表の範囲外です"));
        }
    }
}
