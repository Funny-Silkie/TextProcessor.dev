using System;
using System.Collections.Generic;
using System.Linq;
using TextProcessor.Logics.Data;

namespace TextProcessor.Logics.Operations.OperationImpl
{
    /// <summary>
    /// 末尾へのファイルの連結を行う処理のクラスです。
    /// </summary>
    [Serializable]
    internal class AppendOperation : Operation
    {
        /// <summary>
        /// 連結するデータを取得または設定します。
        /// </summary>
        public TextData? Appended { get; set; }

        /// <inheritdoc/>
        public override string Title => "ファイルを後ろに連結";

        /// <summary>
        /// <see cref="AppendOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        public AppendOperation()
        {
        }

        /// <summary>
        /// <see cref="AppendOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected AppendOperation(AppendOperation cloned)
            : base(cloned)
        {
            Appended = cloned.Appended;
        }

        /// <inheritdoc/>
        protected override Operation CloneCore() => new AppendOperation(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.TextData, "連結するファイル", () => Appended!, x => Appended = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (Appended is null) status.Errors.Add(new StatusEntry(Title, Arguments[0], "連結するファイルが指定されていません"));
        }

        /// <inheritdoc/>
        protected override void OperateCore(TextData data, ProcessStatus status)
        {
            if (Appended is null) return;

            List<List<string>> dataList = data.GetSourceData();
            List<List<string>> appendedList = Appended.GetSourceData();
            dataList.EnsureCapacity(dataList.Count + appendedList.Count);
            int appendedIndex = Appended.HasHeader ? 1 : 0;
            int appendedCount = appendedList.Count - appendedIndex;
            for (int i = appendedIndex; i < appendedIndex + appendedCount; i++) dataList.Add(appendedList[i].ToList());
        }
    }
}
