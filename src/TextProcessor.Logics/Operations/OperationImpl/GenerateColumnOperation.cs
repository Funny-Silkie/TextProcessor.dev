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
    internal class GenerateColumnOperation : Operation
    {
        /// <inheritdoc/>
        public override string Title => "既存列のデータから列を生成";

        /// <summary>
        /// 変換処理を取得または設定します。
        /// </summary>
        public ValueConversion Conversion { get; set; }

        /// <summary>
        /// 使用するデータの列番号を取得または設定します。
        /// </summary>
        public int SourceIndex { get; set; }

        /// <summary>
        /// 新たな列の挿入位置を取得または設定します。
        /// </summary>
        public int InsertIndex { get; set; }

        /// <summary>
        /// 列名を取得または設定します。
        /// </summary>
        public string HeaderName { get; set; } = string.Empty;

        /// <summary>
        /// <see cref="GenerateColumnOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        public GenerateColumnOperation()
        {
            Conversion = ValueConversion.Through;
        }

        /// <summary>
        /// <see cref="GenerateColumnOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected GenerateColumnOperation(GenerateColumnOperation cloned)
            : base(cloned)
        {
            Conversion = cloned.Conversion.Clone();
            SourceIndex = cloned.SourceIndex;
            HeaderName = cloned.HeaderName;
            InsertIndex = cloned.InsertIndex;
        }

        /// <inheritdoc/>
        protected override Operation CloneCore() => new GenerateColumnOperation(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.Index, "列番号", () => SourceIndex, x => SourceIndex = x),
                new ArgumentInfo(ArgumentType.String, "列名", () => HeaderName, x => HeaderName = x),
                new ArgumentInfo(ArgumentType.Index, "挿入位置", () => InsertIndex, x => InsertIndex = x),
                new ArgumentInfo(ArgumentType.ValueConversion, "変換操作", () => Conversion, x => Conversion = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            StatusHelper.VerifyValueConversion(Title, status, Arguments[3], Conversion);
        }

        /// <inheritdoc/>
        protected override void OperateCore(TextData data, ProcessStatus status)
        {
            List<List<string>> list = data.GetSourceData();
            int columnCount = data.ColumnCount;
            int insertedIndex = columnCount < InsertIndex ? columnCount : InsertIndex;
            if ((uint)SourceIndex >= (uint)columnCount)
            {
                status.Errors.Add(new StatusEntry(Title, null, "列番号が範囲外です"));
                return;
            }
            if (data.HasHeader) list[0].Insert(insertedIndex, HeaderName);
            else if (!string.IsNullOrEmpty(HeaderName))
            {
                var header = new List<string>(columnCount + 1);
                var array = new string[columnCount + 1];
                Array.Fill(array, string.Empty);
                array[insertedIndex] = HeaderName;
                header.AddRange(array);

                list.Insert(0, header);
                data.HasHeader = true;
            }

            for (int rowIndex = data.HasHeader ? 1 : 0; rowIndex < list.Count; rowIndex++)
            {
                List<string> row = list[rowIndex];
                ProcessStatus currentStatus = Conversion.Convert(row[SourceIndex], out string? converted);
                StatusHelper.MergeAsChild(Title, status, currentStatus);
                if (!status.Success) return;

                row.Insert(insertedIndex, converted!);
            }
        }
    }
}
