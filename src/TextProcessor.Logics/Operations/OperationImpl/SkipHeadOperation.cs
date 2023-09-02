using System;
using System.Collections.Generic;
using TextProcessor.Logics.Data;

namespace TextProcessor.Logics.Operations.OperationImpl
{
    /// <summary>
    /// 先頭の行をスキップする処理を表します。
    /// </summary>
    [Serializable]
    internal class SkipHeadOperation : Operation
    {
        private int count = 1;

        /// <inheritdoc/>
        public override string Title => "先頭から指定した行数ぶんスキップ";

        /// <summary>
        /// <see cref="SkipHeadOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        public SkipHeadOperation()
        {
        }

        /// <summary>
        /// <see cref="SkipHeadOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected SkipHeadOperation(SkipHeadOperation cloned)
            : base(cloned)
        {
            count = cloned.count;
        }

        /// <inheritdoc/>
        protected override Operation CloneCore() => new SkipHeadOperation(this);

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
            if (count < 0) status.Errors.Add(new StatusEntry(Title, Arguments[0], "行数が負の値です"));
        }

        /// <inheritdoc/>
        protected override void OperateCore(TextData data, ProcessStatus status)
        {
            if (count < 0) return;
            if (count == 0)
            {
                status.Warnings.Add(new StatusEntry(Title, null, "スキップされた行はありません"));
                return;
            }

            List<List<string>> list = data.GetSourceData();
            int startIndex = data.HasHeader ? 1 : 0;
            if (startIndex + count >= list.Count)
            {
                list.RemoveRange(startIndex, list.Count - startIndex);
                status.Warnings.Add(new StatusEntry(Title, null, "全ての行がスキップされました"));
                return;
            }
            list.RemoveRange(startIndex, count);
        }
    }
}
