using System;
using System.Collections.Generic;
using TextProcessor.Logics.Data;

namespace TextProcessor.Logics.Operations.OperationImpl
{
    /// <summary>
    /// 末尾の行を取得する処理を表します。
    /// </summary>
    [Serializable]
    internal class TailOperation : Operation
    {
        /// <summary>
        /// 切り出す行数を取得または設定します。
        /// </summary>
        public int Count { get; set; } = 1;

        /// <inheritdoc/>
        public override string Title => "末尾から指定した行数ぶん取得";

        /// <summary>
        /// <see cref="TailOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        public TailOperation()
        {
        }

        /// <summary>
        /// <see cref="TailOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected TailOperation(TailOperation cloned)
            : base(cloned)
        {
            Count = cloned.Count;
        }

        /// <inheritdoc/>
        protected override Operation CloneCore() => new TailOperation(this);

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

            int offset = data.HasHeader ? 1 : 0;
            if (Count + offset >= list.Count) return;
            list.RemoveRange(offset, list.Count - Count - offset);
        }
    }
}
