using System;
using System.Collections.Generic;
using System.Linq;
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
        /// 対象の列を取得または設定します。
        /// </summary>
        public ValueRange Columns { get; set; }

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
            Columns = cloned.Columns;
        }

        /// <inheritdoc/>
        protected override Operation CloneCore() => new SelectColumnOperation(this);

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
            List<List<string>> list = data.GetSourceData();
            var newList = new List<List<string>>(list.Count);

            int columnCount = Columns.AsEntryCollection().Sum(x => x.Count);
            if (columnCount == 0)
            {
                if (data.HasHeader)
                {
                    if (list.Count > 0) list.RemoveRange(1, list.Count - 1);
                }
                else list.Clear();
                status.Warnings.Add(new StatusEntry(Title, null, "列が指定されていません"));
                return;
            }

            foreach (List<string> row in list)
            {
                var newRow = new List<string>(columnCount);
                foreach (int index in Columns)
                {
                    if (index >= row.Count)
                    {
                        newRow.Add(string.Empty);
                        break;
                    }
                    newRow.Add(row[index]);
                }
                newList.Add(newRow);
            }

            list.Clear();
            list.AddRange(newList);
        }
    }
}
