using Microsoft.JSInterop;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;
using TextProcessor.Data;
using TextProcessor.Logics.Data;
using TextProcessor.Logics.Data.Options;

namespace TextProcessor.Models
{
    /// <summary>
    /// メインのモデルクラスです。
    /// </summary>
    [InjectionRange(InjectionType.Scoped)]
    public class MainModel : ModelBase
    {
        private readonly IJSRuntime js;

        /// <summary>
        /// ファイル一覧を取得します。
        /// </summary>
        public ReactiveCollection<TableFileInfo> Files { get; }

        /// <summary>
        /// 直近に編集したデータを取得または設定します。
        /// </summary>
        public ReactiveProperty<TableFileInfo?> CurrentEditData { get; }

        /// <summary>
        /// <see cref="MainModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public MainModel(IJSRuntime js)
        {
            this.js = js;

            CurrentEditData = new ReactiveProperty<TableFileInfo?>().AddTo(DisposableList);
            Files = new ReactiveCollection<TableFileInfo>().AddTo(DisposableList);
        }

        /// <summary>
        /// テキストファイルを読み込みます。
        /// </summary>
        /// <param name="name">ファイル名</param>
        /// <param name="stream">読み込むファイルのストリームオブジェクト</param>
        /// <param name="options">読み込むオプション</param>
        /// <returns>読み込んだファイル</returns>
        public async Task LoadDsvFile(string name, Stream stream, TextLoadOptions options)
        {
            var info = new TableFileInfo(name, await TextData.CreateFromDsvAsync(stream, options));
            CurrentEditData.Value = info;
            Files.AddOnScheduler(info);
        }

        /// <summary>
        /// Excelのファイルを読み込みます。
        /// </summary>
        /// <param name="name">ファイル名</param>
        /// <param name="stream">読み込むファイルのストリームオブジェクト</param>
        /// <param name="options">読み込むオプション</param>
        /// <returns>読み込んだファイル</returns>
        public async Task LoadExcelFile(string name, Stream stream, ExcelLoadOptions options)
        {
            (string sheetName, TextData data)[] data = await Task.Run(() => TextData.CreateFromExcel(stream, options));
            if (data.Length == 0) return;
            TableFileInfo[] converted = Array.ConvertAll(data, x => new TableFileInfo($"{name}/{x.sheetName}", x.data));
            CurrentEditData.Value = converted[0];
            Files.AddRangeOnScheduler(converted);
        }

        /// <summary>
        /// ファイルをダウンロードします。
        /// </summary>
        /// <param name="name">ファイル名</param>
        /// <param name="stream">ストリームオブジェクト</param>
        public async Task DownloadFile(string name, Stream stream)
        {
            using var streamRef = new DotNetStreamReference(stream, leaveOpen: true);
            await js.InvokeAsync<IJSObjectReference>("import", "./js/helper.js");
            await js.InvokeVoidAsync("downloadFileFromStream", name, streamRef);
        }
    }
}
