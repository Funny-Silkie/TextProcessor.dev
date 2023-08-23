using System;
using System.Collections.Generic;
using TextProcessor.Logics.Data;

namespace TextProcessor.Logics.Operations.OperationImpl
{
    /// <summary>
    /// 先頭の行を取得する処理を表します。
    /// </summary>
    [Serializable]
    internal class HeadOperation : Operation
    {
        private int count = 1;

        /// <inheritdoc/>
        public override string Title => "先頭から指定した行数ぶん取得";

        /// <summary>
        /// <see cref="HeadOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        public HeadOperation()
        {
        }

        /// <summary>
        /// <see cref="HeadOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected HeadOperation(HeadOperation cloned)
            : base(cloned)
        {
            count = cloned.count;
        }

        /// <inheritdoc/>
        protected override Operation CloneCore() => new HeadOperation(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.Integer, "行数", () => count, x => count = x),
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
            int actualCount = count;
            if (data.HasHeader) actualCount++;

            if (actualCount >= list.Count) return;
            list.RemoveRange(actualCount, list.Count - actualCount);
        }
    }
}
