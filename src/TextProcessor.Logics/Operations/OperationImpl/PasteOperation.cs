using System;
using System.Collections.Generic;
using TextProcessor.Logics.Data;

namespace TextProcessor.Logics.Operations.OperationImpl
{
    /// <summary>
    /// 横方向にファイルを結合する処理のクラスです。
    /// </summary>
    [Serializable]
    internal class PasteOperation : Operation
    {
        /// <summary>
        /// 連結するデータを取得または設定します。
        /// </summary>
        public TextData? Target { get; set; }

        /// <inheritdoc/>
        public override string Title => "ファイルを横方向に連結";

        /// <summary>
        /// <see cref="PasteOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        public PasteOperation()
        {
        }

        /// <summary>
        /// <see cref="PasteOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected PasteOperation(PasteOperation cloned)
            : base(cloned)
        {
            Target = cloned.Target;
        }

        /// <inheritdoc/>
        protected override Operation CloneCore() => new PasteOperation(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.TextData, "ファイル", () => Target!, x => Target = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (Target is null) status.Errors.Add(new StatusEntry(Title, Arguments[0], "連結するファイルが指定されていません"));
        }

        /// <inheritdoc/>
        protected override void OperateCore(TextData data, ProcessStatus status)
        {
            if (Target is null) return;

            List<List<string>> dataList = data.GetSourceData();
            List<List<string>> targetList = Target.GetSourceData();
            int pastedOffset = data.ColumnCount;

            int dataIndex = data.HasHeader ? 1 : 0;
            int targetIndex = Target.HasHeader ? 1 : 0;

            if (targetIndex == 1)
            {
                List<string> targetHeader = targetList[0];
                if (dataIndex == 0)
                {
                    var dataHeader = new List<string>(pastedOffset + targetHeader.Count);
                    dataList.Insert(0, dataHeader);
                    var blankArray = new string[pastedOffset];
                    Array.Fill(blankArray, string.Empty);
                    dataHeader.AddRange(blankArray);
                    dataHeader.AddRange(targetHeader);
                    dataIndex++;
                    data.HasHeader = true;
                }
                else
                {
                    List<string> dataHeader = dataList[0];
                    dataHeader.EnsureCapacity(dataHeader.Count + targetHeader.Count);
                    if (dataHeader.Count < pastedOffset)
                    {
                        int blankCount = pastedOffset - dataHeader.Count;
                        var blankArray = new string[blankCount];
                        Array.Fill(blankArray, string.Empty);
                        dataHeader.AddRange(blankArray);
                    }
                    dataHeader.AddRange(targetHeader);
                }
            }

            for (; dataIndex < Math.Max(dataList.Count, targetList.Count) && targetIndex < targetList.Count; dataIndex++, targetIndex++)
            {
                List<string> targetRow = targetList[targetIndex];
                List<string> dataRow;
                if (dataIndex < dataList.Count)
                {
                    dataRow = dataList[dataIndex];
                    dataRow.EnsureCapacity(pastedOffset + targetRow.Count);
                }
                else
                {
                    dataRow = new List<string>(pastedOffset + targetRow.Count);
                    dataList.Add(dataRow);
                }

                if (dataRow.Count < pastedOffset)
                {
                    int blankCount = pastedOffset - dataRow.Count;
                    var blankArray = new string[blankCount];
                    Array.Fill(blankArray, string.Empty);
                    dataRow.AddRange(blankArray);
                }
                dataRow.AddRange(targetRow);
            }
        }
    }
}
