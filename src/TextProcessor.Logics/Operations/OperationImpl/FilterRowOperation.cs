using System;
using System.Collections.Generic;
using TextProcessor.Logics.Data;
using TextProcessor.Logics.Operations.Conditions;

namespace TextProcessor.Logics.Operations.OperationImpl
{
    /// <summary>
    /// 行の絞り込みを行う処理のクラスです。
    /// </summary>
    [Serializable]
    internal class FilterRowOperation : Operation
    {
        /// <summary>
        /// 絞込条件を取得または設定します。
        /// </summary>
        public RowCondition Condition { get; set; }

        /// <inheritdoc/>
        public override string Title => "行をフィルタリング";

        /// <summary>
        /// <see cref="FilterRowOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        public FilterRowOperation()
        {
            Condition = RowCondition.Null;
        }

        /// <summary>
        /// <see cref="FilterRowOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected FilterRowOperation(FilterRowOperation cloned)
            : base(cloned)
        {
            Condition = cloned.Condition.Clone();
        }

        /// <inheritdoc/>
        protected override Operation CloneCore() => new FilterRowOperation(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.RowCondition, "条件", () => Condition, x => Condition = (RowCondition)x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            StatusHelper.VerifyRowCondition(Title, status, Arguments[0], Condition);
        }

        /// <inheritdoc/>
        protected override void OperateCore(TextData data, ProcessStatus status)
        {
            List<List<string>> list = data.GetSourceData();
            int firstRowCount = list.Count;
            int offset = data.HasHeader ? 1 : 0;
            int removedCount = 0;
            for (int i = offset; i < list.Count; i++)
                if (Condition.Match(list[i]) != MatchResult.Matched)
                {
                    list.RemoveAt(i--);
                    removedCount++;
                }
            if (firstRowCount > offset)
            {
                if (removedCount == 0) status.Messages.Add(new StatusEntry(Title, null, "全ての行が条件を満たします"));
                if (list.Count == offset) status.Warnings.Add(new StatusEntry(Title, null, "全ての行が条件に適合しません"));
            }
        }
    }
}
