using System;
using System.Collections.Generic;
using TextProcessor.Logics.Data;

namespace TextProcessor.Logics.Operations.OperationImpl
{
    /// <summary>
    /// 行の削除を行う処理のクラスです。
    /// </summary>
    [Serializable]
    internal class DeleteRowOperation : Operation
    {
        private int index;

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
            index = cloned.index;
        }

        /// <inheritdoc/>
        protected override Operation CloneCore() => new DeleteRowOperation(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.Index, "行番号", () => index, x => index = x),
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
            int deletedIndex = index;
            if (data.HasHeader) deletedIndex++;
            if (deletedIndex < list.Count) list.RemoveAt(deletedIndex);
            else status.Warnings.Add(new StatusEntry(Title, null, $"行番号'{index + 1}'の行は存在しません"));
        }
    }
}
