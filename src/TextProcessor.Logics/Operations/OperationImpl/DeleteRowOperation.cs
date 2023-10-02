using System;
using System.Collections.Generic;
using System.Linq;
using TextProcessor.Logics.Data;

namespace TextProcessor.Logics.Operations.OperationImpl
{
    /// <summary>
    /// 行の削除を行う処理のクラスです。
    /// </summary>
    [Serializable]
    internal class DeleteRowOperation : Operation
    {
        /// <summary>
        /// 削除する行を取得または設定します。
        /// </summary>
        public ValueRange Rows { get; set; }

        /// <inheritdoc/>
        public override string Title => "指定した行を削除";

        /// <summary>
        /// <see cref="DeleteRowOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        public DeleteRowOperation()
        {
        }

        /// <summary>
        /// <see cref="DeleteRowOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected DeleteRowOperation(DeleteRowOperation cloned)
            : base(cloned)
        {
            Rows = cloned.Rows;
        }

        /// <inheritdoc/>
        protected override Operation CloneCore() => new DeleteRowOperation(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.Range1Based, "行番号", () => Rows, x => Rows = x),
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

            IEnumerable<int> indexes = Rows.Distinct();
            if (data.HasHeader) indexes = indexes.Select(x => x + 1);
            HashSet<int> indexset = indexes.ToHashSet();
            if (indexset.Count == 0)
            {
                status.Warnings.Add(new StatusEntry(Title, null, "削除する行が指定されていません"));
                return;
            }

            var newList = new List<List<string>>(list.Count);
            int removedCount = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (indexset.Contains(i))
                {
                    removedCount++;
                    continue;
                }

                List<string> row = list[i];
                newList.Add(row);
            }

            if (removedCount == 0) status.Warnings.Add(new StatusEntry(Title, null, "削除された行がありません"));
            else
            {
                list.Clear();
                list.AddRange(newList);
            }
        }
    }
}
