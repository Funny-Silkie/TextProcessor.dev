using Microsoft.AspNetCore.Components.Forms;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TextProcessor.Data;
using TextProcessor.Logics.Data.Options;

namespace TextProcessor.Models
{
    /// <summary>
    /// 読み込み画面のモデルのクラスです。
    /// </summary>
    [InjectionRange(InjectionType.Scoped)]
    public class LoadModel : ModelBase
    {
        /// <summary>
        /// サポートするエンコード一覧を取得します。
        /// </summary>
        public static Dictionary<string, EncodingInfo> EncodingTable { get; }

        static LoadModel()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            EncodingInfo[] encodings = Encoding.GetEncodings();
            Array.Sort(encodings, (x, y) => x.Name.CompareTo(y.Name));

            EncodingTable = new Dictionary<string, EncodingInfo>(encodings.Length, StringComparer.Ordinal);
            for (int i = 0; i < encodings.Length; i++)
            {
                EncodingInfo currentEncoding = encodings[i];
                EncodingTable.Add($"{currentEncoding.Name} ({currentEncoding.CodePage})", currentEncoding);
            }
        }

        private readonly MainModel mainModel;

        #region Properties

        /// <summary>
        /// 読み込むファイルの種類を取得または設定します。
        /// </summary>
        public ReactiveProperty<TableFileType> FileType { get; }

        /// <summary>
        /// 行区切り文字を取得または設定します。
        /// </summary>
        public ReactiveProperty<RowDelimiter> Delimiter { get; }

        /// <summary>
        ///その他の行区切り文字を取得または設定します。
        /// </summary>
        public ReactiveProperty<string> OtherDelimiter { get; }

        /// <summary>
        /// 先頭行をヘッダーとして扱うかどうかを表す値を取得または設定します。
        /// </summary>
        public ReactiveProperty<bool> HasHeader { get; }

        /// <summary>
        /// 選択されている文字エンコードを取得または設定します。
        /// </summary>
        public ReactiveProperty<string> SelectedEncoding { get; }

        #endregion Properties

        /// <summary>
        /// <see cref="LoadModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public LoadModel(MainModel mainModel)
        {
            this.mainModel = mainModel;

            FileType = new ReactiveProperty<TableFileType>(TableFileType.Dsv).AddTo(DisposableList);
            Delimiter = new ReactiveProperty<RowDelimiter>(RowDelimiter.Tab).AddTo(DisposableList);
            OtherDelimiter = new ReactiveProperty<string>().AddTo(DisposableList);
            HasHeader = new ReactiveProperty<bool>(true).AddTo(DisposableList);
            SelectedEncoding = new ReactiveProperty<string>("utf-8 (65001)").AddTo(DisposableList);
        }

        /// <summary>
        /// 使用する行区切り文字を取得します。
        /// </summary>
        /// <returns>使用する行区切り文字</returns>
        private string GetDelimiter()
        {
            return Delimiter.Value switch
            {
                RowDelimiter.Tab => "\t",
                RowDelimiter.Comma => ",",
                RowDelimiter.Others => OtherDelimiter.Value,
                _ => throw new InvalidOperationException(),
            };
        }

        /// <summary>
        /// ファイルを読み込みます。
        /// </summary>
        public async Task Load(IBrowserFile file)
        {
            string delimiter = GetDelimiter();
            EncodingInfo encodingInfo = EncodingTable[SelectedEncoding.Value];

            using Stream stream = file.OpenReadStream(file.Size);
            switch (FileType.Value)
            {
                case TableFileType.Dsv:
                    {
                        await mainModel.LoadDsvFile(file.Name, stream, new TextLoadOptions(HasHeader.Value, delimiter, Encoding.GetEncoding(encodingInfo.CodePage)));
                    }
                    break;

                case TableFileType.Excel:
                    {
                        using var destStream = new MemoryStream();
                        await stream.CopyToAsync(destStream);

                        await mainModel.LoadExcelFile(file.Name, destStream, new ExcelLoadOptions(HasHeader.Value));
                    }
                    break;
            }
        }
    }
}
