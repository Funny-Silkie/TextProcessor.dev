using Microsoft.JSInterop;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
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
        public ReactiveCollection<DsvFileInfo> Files { get; }

        /// <summary>
        /// 直近に編集したデータを取得または設定します。
        /// </summary>
        public ReactiveProperty<DsvFileInfo?> CurrentEditData { get; }

        /// <summary>
        /// <see cref="MainModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public MainModel(IJSRuntime js)
        {
            this.js = js;

            CurrentEditData = new ReactiveProperty<DsvFileInfo?>().AddTo(DisposableList);
            Files = new ReactiveCollection<DsvFileInfo>().AddTo(DisposableList);
        }

        /// <summary>
        /// テキストファイルを読み込みます。
        /// </summary>
        /// <param name="name">ファイル名</param>
        /// <param name="stream">読み込むファイルのストリームオブジェクト</param>
        /// <param name="options">読み込むオプション</param>
        /// <returns>読み込んだファイル</returns>
        public async Task LoadFile(string name, Stream stream, TextLoadOptions options)
        {
            var info = new DsvFileInfo(name, await TextData.CreateAsync(stream, options));
            CurrentEditData.Value = info;
            Files.AddOnScheduler(info);
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
