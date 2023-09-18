using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using TextProcessor.Data;
using TextProcessor.Models;
using TextProcessor.Shared;

namespace TextProcessor.ViewModels
{
    /// <summary>
    /// ファイル読み込みページのViewModelのクラスです。
    /// </summary>
    [InjectionRange(InjectionType.Transient)]
    public class LoadViewModel : PageViewModel
    {
        private static readonly string[] ExcelExtensions;
        private static readonly string[] DsvExtensions;

        static LoadViewModel()
        {
            ExcelExtensions = new[] { "xlsx", "xlsm" };
            DsvExtensions = new[] { "txt", "csv", "tsv" };

            Array.Sort(ExcelExtensions, StringComparer.OrdinalIgnoreCase);
            Array.Sort(DsvExtensions, StringComparer.OrdinalIgnoreCase);
        }

        private readonly DialogService dialogService;
        private readonly IOModel ioModel;
        private readonly NotificationService notificationService;

        /// <summary>
        /// サポートする文字エンコードの名称一覧を取得します。
        /// </summary>
        public ICollection<string> EncodingNames => IOModel.EncodingTable.Keys;

        #region Properties

        /// <summary>
        /// 現在読み込んでいるファイルを取得または設定します。
        /// </summary>
        public ReactivePropertySlim<IBrowserFile?> CurrentFile { get; }

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

        #endregion Properties

        #region Commands

        /// <summary>
        /// <inheritdoc cref="OnFileChanged(IBrowserFile?)"/>
        /// </summary>
        public AsyncReactiveCommand<IBrowserFile?> OnFileChangedCommand { get; }

        /// <summary>
        /// <inheritdoc cref="Load(InputFileChangeEventArgs)"/>
        /// </summary>
        public AsyncReactiveCommand LoadCommand { get; }

        #endregion Commands

        /// <summary>
        /// <see cref="LoadViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public LoadViewModel(NavigationManager navigationManager, IOModel ioModel, NotificationService notificationService, DialogService dialogService)
            : base(navigationManager)
        {
            this.dialogService = dialogService;
            this.ioModel = ioModel;
            this.notificationService = notificationService;

            CurrentFile = new ReactivePropertySlim<IBrowserFile?>().AddTo(DisposableList);
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

            OnFileChangedCommand = new AsyncReactiveCommand<IBrowserFile?>().WithSubscribe(OnFileChanged)
                                                                            .AddTo(DisposableList);
            LoadCommand = new AsyncReactiveCommand().WithSubscribe(Load)
                                                    .AddTo(DisposableList);
        }

        /// <summary>
        /// 読み込むファイルが変更されたときに通知されます。
        /// </summary>
        /// <param name="file">読み込むファイル</param>
        private async Task OnFileChanged(IBrowserFile? file)
        {
            CurrentFile.Value = file;
            if (file is not null)
            {
                {
                    string tail4 = file.Name[^4..];
                    if (Array.BinarySearch(ExcelExtensions, tail4, StringComparer.OrdinalIgnoreCase) >= 0) FileType.Value = TableFileType.Excel;
                }
                {
                    string tail3 = file.Name[^3..];
                    if (Array.BinarySearch(DsvExtensions, tail3, StringComparer.OrdinalIgnoreCase) >= 0) FileType.Value = TableFileType.Dsv;
                }
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// ファイルを読み込みます。
        /// </summary>
        private async Task Load()
        {
            IBrowserFile? file = CurrentFile.Value;
            if (file is null) return;

            if (Delimiter.Value == RowDelimiter.Others && string.IsNullOrEmpty(OtherDelimiter.Value))
            {
                notificationService.Notify(NotificationSeverity.Warning, "行区切り文字を指定してください");
                return;
            }

            dialogService.Open<WaitingDialog>(string.Empty, new Dictionary<string, object>()
            {
                [nameof(WaitingDialog.Message)] = "読込中",
            }, new DialogOptions()
            {
                Width = "300px",
                Height = "200px",
                CloseDialogOnEsc = false,
                CloseDialogOnOverlayClick = false,
                ShowClose = false,
            });

            try
            {
                await ioModel.Load(file);
            }
            catch (FileFormatException)
            {
                notificationService.Notify(NotificationSeverity.Error, "ファイルのフォーマットが無効です");
                return;
            }
            catch
            {
#if DEBUG
                throw;
#else
                notificationService.Notify(NotificationSeverity.Error, "ファイル読み込み時にエラーが発生しました");
                return;
#endif
            }
            finally
            {
                dialogService.Close();
            }
            NavigateTo("edit");
        }
    }
}
