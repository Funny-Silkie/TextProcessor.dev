using System;
using System.Collections.Generic;
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
        /// 削除する列のインデックスを取得または設定します。
        /// </summary>
        public int Index { get; set; }

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
            Index = cloned.Index;
        }

        /// <inheritdoc/>
        protected override Operation CloneCore() => new DeleteColumnOperation(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.Index, "列番号", () => Index, x => Index = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
        }

        /// <inheritdoc/>
        protected override void OperateCore(TextData data, ProcessStatus status)
        {
            int removedCount = 0;

            foreach (List<string> row in data.GetSourceData())
                if (Index < row.Count)
                {
                    row.RemoveAt(Index);
                    removedCount++;
                }
            if (removedCount == 0) status.Warnings.Add(new StatusEntry(Title, null, $"列番号'{Index + 1}'の列が存在しません"));
        }
    }
}
