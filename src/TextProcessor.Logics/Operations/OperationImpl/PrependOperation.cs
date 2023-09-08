using System;
using System.Collections.Generic;
using System.Linq;
using TextProcessor.Logics.Data;

namespace TextProcessor.Logics.Operations.OperationImpl
{
    /// <summary>
    /// 先頭へのファイルの連結を行う処理のクラスです。
    /// </summary>
    [Serializable]
    internal class PrependOperation : Operation
    {
        /// <summary>
        /// 連結するデータを取得または設定します。
        /// </summary>
        public TextData? Prepended { get; set; }

        /// <inheritdoc/>
        public override string Title => "ファイルを前に連結";

        /// <summary>
        /// <see cref="PrependOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        public PrependOperation()
        {
        }

        /// <summary>
        /// <see cref="PrependOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected PrependOperation(PrependOperation cloned)
            : base(cloned)
        {
            Prepended = cloned.Prepended;
        }

        /// <inheritdoc/>
        protected override Operation CloneCore() => new PrependOperation(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.TextData, "連結するファイル", () => Prepended!, x => Prepended = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (Prepended is null) status.Errors.Add(new StatusEntry(Title, Arguments[0], "連結するファイルが指定されていません"));
        }

        /// <inheritdoc/>
        protected override void OperateCore(TextData data, ProcessStatus status)
        {
            if (Prepended is null) return;

            List<List<string>> dataList = data.GetSourceData();
            List<List<string>> prependedList = Prepended.GetSourceData();
            dataList.EnsureCapacity(dataList.Count + prependedList.Count);
            int prependedIndex = Prepended.HasHeader ? 1 : 0;
            int prependedCount = prependedList.Count - prependedIndex;
            List<string>[] prependedValues = prependedList.Skip(prependedIndex).Take(prependedCount).Select(x => x.ToList()).ToArray();
            dataList.InsertRange(data.HasHeader ? 1 : 0, prependedValues);
        }
    }
}
