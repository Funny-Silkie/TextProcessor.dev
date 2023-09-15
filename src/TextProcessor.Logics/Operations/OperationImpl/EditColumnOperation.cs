using System;
using System.Collections.Generic;
using TextProcessor.Logics.Data;
using TextProcessor.Logics.Operations.Conversions;

namespace TextProcessor.Logics.Operations.OperationImpl
{
    /// <summary>
    /// 列の生成を行う処理のクラスです。
    /// </summary>
    [Serializable]
    internal class EditColumnOperation : Operation
    {
        /// <inheritdoc/>
        public override string Title => "既存列のデータを編集";

        /// <summary>
        /// 変換処理を取得または設定します。
        /// </summary>
        public ValueConversion Conversion { get; set; }

        /// <summary>
        /// 使用するデータの列番号を取得または設定します。
        /// </summary>
        public int SourceIndex { get; set; }

        /// <summary>
        /// <see cref="EditColumnOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        public EditColumnOperation()
        {
            Conversion = ValueConversion.Through;
        }

        /// <summary>
        /// <see cref="EditColumnOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected EditColumnOperation(EditColumnOperation cloned)
            : base(cloned)
        {
            Conversion = cloned.Conversion.Clone();
            SourceIndex = cloned.SourceIndex;
        }

        /// <inheritdoc/>
        protected override Operation CloneCore() => new EditColumnOperation(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.Index, "列番号", () => SourceIndex, x => SourceIndex = x),
                new ArgumentInfo(ArgumentType.ValueConversion, "変換操作", () => Conversion, x => Conversion = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            StatusHelper.VerifyValueConversion(Title, status, Arguments[1], Conversion);
        }

        /// <inheritdoc/>
        protected override void OperateCore(TextData data, ProcessStatus status)
        {
            List<List<string>> list = data.GetSourceData();
            int columnCount = data.ColumnCount;
            if ((uint)SourceIndex >= (uint)columnCount)
            {
                status.Errors.Add(new StatusEntry(Title, null, "列番号が範囲外です"));
                return;
            }

            for (int rowIndex = data.HasHeader ? 1 : 0; rowIndex < list.Count; rowIndex++)
            {
                List<string> row = list[rowIndex];
                ProcessStatus currentStatus = Conversion.Convert(row[SourceIndex], out string? converted);
                StatusHelper.MergeAsChild(Title, status, currentStatus);
                if (!status.Success) return;

                row[SourceIndex] = converted!;
            }
        }
    }
}
