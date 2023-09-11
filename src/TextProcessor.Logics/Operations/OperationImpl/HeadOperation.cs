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
        /// <summary>
        /// 切り出す行数を取得または設定します。
        /// </summary>
        public int Count { get; set; } = 1;

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
            Count = cloned.Count;
        }

        /// <inheritdoc/>
        protected override Operation CloneCore() => new HeadOperation(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.Integer, "行数", () => Count, x => Count = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (Count < 0) status.Errors.Add(new StatusEntry(Title, Arguments[0], "行数が負の値です"));
        }

        /// <inheritdoc/>
        protected override void OperateCore(TextData data, ProcessStatus status)
        {
            if (Count < 0) return;

            List<List<string>> list = data.GetSourceData();
            int actualCount = Count;
            if (data.HasHeader) actualCount++;

            if (actualCount >= list.Count) return;
            list.RemoveRange(actualCount, list.Count - actualCount);
        }
    }
}
