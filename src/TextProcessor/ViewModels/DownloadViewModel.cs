using Radzen;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using TextProcessor.Data;
using TextProcessor.Models;

namespace TextProcessor.ViewModels
{
    /// <summary>
    /// ダウンロード画面のViewModelのクラスです。
    /// </summary>
    [InjectionRange(InjectionType.Transient)]
    public class DownloadViewModel : ViewModelBase
    {
        private readonly DialogService dialogService;

        /// <summary>
        /// サポートする文字エンコードの名称一覧を取得します。
        /// </summary>
        public ICollection<string> EncodingNames => IOModel.EncodingTable.Keys;

        #region Properties

        /// <inheritdoc cref="IOModel.FileType"/>
        public ReactivePropertySlim<TableFileType> FileType { get; }

        /// <summary>
        /// <see cref="FileType"/>で<see cref="TableFileType.Dsv"/>が選択されているかどうかを表す値を取得します。
        /// </summary>
        public ReadOnlyReactivePropertySlim<bool> IsDsvSelected { get; }

        /// <inheritdoc cref="IOModel.Delimiter"/>
        public ReactivePropertySlim<RowDelimiter> Delimiter { get; }

        /// <summary>
        /// その他の行区切り文字かどうかを表す値を取得します。
        /// </summary>
        public ReadOnlyReactivePropertySlim<bool> IsOtherDelimiter { get; }

        /// <inheritdoc cref="IOModel.OtherDelimiter"/>
        public ReactivePropertySlim<string> OtherDelimiter { get; }

        /// <inheritdoc cref="IOModel.HasHeader"/>
        public ReactivePropertySlim<bool> HasHeader { get; }

        /// <inheritdoc cref="IOModel.SelectedEncoding"/>
        public ReactivePropertySlim<string> SelectedEncoding { get; }

        /// <summary>
        /// <see cref="FileType"/>で<see cref="TableFileType.Excel"/>が選択されているかどうかを表す値を取得します。
        /// </summary>
        public ReadOnlyReactivePropertySlim<bool> IsExcelSelected { get; }

        /// <summary>
        /// 全てを文字列として保存するかどうかを表す値を取得または設定します。
        /// </summary>
        public ReactivePropertySlim<bool> AsRowString { get; }

        #endregion Properties

        #region Commands

        /// <inheritdoc cref="Ok"/>
        public AsyncReactiveCommand CommandOk { get; }

        /// <inheritdoc cref="Cancel"/>
        public AsyncReactiveCommand CommandCancel { get; }

        #endregion Commands

        /// <summary>
        /// <see cref="DownloadViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public DownloadViewModel(DialogService dialogService, IOModel ioModel)
        {
            this.dialogService = dialogService;

            FileType = ioModel.FileType.ToReactivePropertySlimAsSynchronized(x => x.Value)
                                         .AddTo(DisposableList);
            IsDsvSelected = ioModel.FileType.Select(x => x == TableFileType.Dsv)
                                              .ToReadOnlyReactivePropertySlim()
                                              .AddTo(DisposableList);
            Delimiter = ioModel.Delimiter.ToReactivePropertySlimAsSynchronized(x => x.Value)
                                           .AddTo(DisposableList);
            IsOtherDelimiter = Delimiter.Select(x => x == RowDelimiter.Others)
                                        .ToReadOnlyReactivePropertySlim()
                                        .AddTo(DisposableList);
            OtherDelimiter = ioModel.OtherDelimiter.ToReactivePropertySlimAsSynchronized(x => x.Value)
                                                     .AddTo(DisposableList);
            HasHeader = ioModel.HasHeader.ToReactivePropertySlimAsSynchronized(x => x.Value)
                                           .AddTo(DisposableList);
            SelectedEncoding = ioModel.SelectedEncoding.ToReactivePropertySlimAsSynchronized(x => x.Value)
                                                         .AddTo(DisposableList);
            IsExcelSelected = ioModel.FileType.Select(x => x == TableFileType.Excel)
                                              .ToReadOnlyReactivePropertySlim()
                                              .AddTo(DisposableList);
            AsRowString = new ReactivePropertySlim<bool>().AddTo(DisposableList);

            CommandOk = new AsyncReactiveCommand().WithSubscribe(Ok)
                                                  .AddTo(DisposableList);
            CommandCancel = new AsyncReactiveCommand().WithSubscribe(Cancel)
                                                      .AddTo(DisposableList);
        }

        /// <summary>
        /// ダイアログを閉じてダウンロード処理を実行します。
        /// </summary>
        private async Task Ok()
        {
            dialogService.Close(this);
            await Task.CompletedTask;
        }

        /// <summary>
        /// ダイアログを閉じてダウンロード処理を中断します。
        /// </summary>
        private async Task Cancel()
        {
            dialogService.Close(null);
            await Task.CompletedTask;
        }
    }
}
