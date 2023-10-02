using System;
using System.Collections.Generic;
using System.Linq;
using TextProcessor.Logics.Data;

namespace TextProcessor.Logics.Operations.OperationImpl
{
    /// <summary>
    /// 列の削除を行う処理のクラスです。
    /// </summary>
    [Serializable]
    internal class DeleteColumnOperation : Operation
    {
        /// <summary>
        /// 削除する列を取得または設定します。
        /// </summary>
        public ValueRange Columns { get; set; }

        /// <inheritdoc/>
        public override string Title => "指定した列を削除";

        /// <summary>
        /// <see cref="DeleteColumnOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        public DeleteColumnOperation()
        {
        }

        /// <summary>
        /// <see cref="DeleteColumnOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected DeleteColumnOperation(DeleteColumnOperation cloned)
            : base(cloned)
        {
            Columns = cloned.Columns;
        }

        /// <inheritdoc/>
        protected override Operation CloneCore() => new DeleteColumnOperation(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.Range1Based, "列番号", () => Columns, x => Columns = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
        }

        /// <inheritdoc/>
        protected override void OperateCore(TextData data, ProcessStatus status)
        {
            HashSet<int> indexes = Columns.Distinct().ToHashSet();
            List<List<string>> list = data.GetSourceData();

            bool nonRemoved = false;
            for (int rowIndex = 0; rowIndex < list.Count; rowIndex++)
            {
                List<string> row = list[rowIndex];
                int removedCount = 0;

                var newRow = new List<string>(row.Count);
                for (int columnIndex = 0; columnIndex < row.Count; columnIndex++)
                {
                    if (indexes.Contains(columnIndex))
                    {
                        removedCount++;
                        continue;
                    }
                    newRow.Add(row[columnIndex]);
                }
                if (removedCount == 0) nonRemoved = true;
                else list[rowIndex] = newRow;
            }

            if (nonRemoved) status.Warnings.Add(new StatusEntry(Title, null, "削除された列がありません"));
        }
    }
}
