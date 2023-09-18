using Microsoft.AspNetCore.Components;
using Radzen;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using TextProcessor.Data;
using TextProcessor.Logics.Data;
using TextProcessor.Logics.Data.Options;
using TextProcessor.Logics.Operations;
using TextProcessor.Messages;
using TextProcessor.Models;

namespace TextProcessor.ViewModels
{
    /// <summary>
    /// 編集画面のViewModelのクラスです。
    /// </summary>
    [InjectionRange(InjectionType.Transient)]
    public class EditViewModel : PageViewModel
    {
        private readonly MainModel mainModel;
        private readonly EditModel editModel;
        private readonly NotificationService notificationService;
        private bool initialized;

        #region Properties

        /// <summary>
        /// ファイル一覧を取得します。
        /// </summary>
        public ReadOnlyReactiveCollection<TableFileInfo> Files { get; }

        /// <summary>
        /// 編集中のファイルを取得または設定します。
        /// </summary>
        public ReactivePropertySlim<TableFileInfo?> EditingFile { get; }

        /// <summary>
        /// 表示データを取得または設定します。
        /// </summary>
        public ReactivePropertySlim<TextData?> ViewData { get; }

        /// <summary>
        /// 処理一覧を取得します。
        /// </summary>
        public ReactiveCollection<OperationViewModel> Operations { get; }

        /// <summary>
        /// ログ一覧を取得します。
        /// </summary>
        public ReadOnlyReactiveCollection<LogInfo> LogList { get; }

        /// <summary>
        /// ログ一覧が閉じられているかどうかを取得または設定します。
        /// </summary>
        public ReactivePropertySlim<bool> LogTableCollapsed { get; }

        #endregion Properties

        #region Commands

        /// <inheritdoc cref="EndAddOperation(AddOperationViewModel)"/>
        public AsyncReactiveCommand<AddOperationViewModel> EndAddOperationCommand { get; }

        /// <inheritdoc cref="Operate"/>
        public AsyncReactiveCommand OperateCommand { get; }

        /// <inheritdoc cref="AsRaw"/>
        public AsyncReactiveCommand AsRawCommand { get; }

        /// <inheritdoc cref="Download"/>
        public AsyncReactiveCommand DownloadCommand { get; }

        #endregion Commands

        /// <summary>
        /// <see cref="EditViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public EditViewModel(NavigationManager navigationManager, MainModel mainModel, EditModel editModel, NotificationService notificationService)
            : base(navigationManager)
        {
            this.mainModel = mainModel;
            this.editModel = editModel;
            this.notificationService = notificationService;

            Operations = new ReactiveCollection<OperationViewModel>().AddTo(DisposableList);
            Files = mainModel.Files.ToReadOnlyReactiveCollection()
                                   .AddTo(DisposableList);
            LogTableCollapsed = editModel.SendLogNotification.ToReactivePropertySlimAsSynchronized(x => x.Value)
                                                             .AddTo(DisposableList);
            EditingFile = new ReactivePropertySlim<TableFileInfo?>(mainModel.CurrentEditData.Value).AddTo(DisposableList);
            ViewData = new ReactivePropertySlim<TextData?>().AddTo(DisposableList);
            EditingFile.Subscribe(OnSelectionChanged).AddTo(DisposableList);
            LogList = editModel.LogList.ToReadOnlyReactiveCollection()
                                       .AddTo(DisposableList);

            EndAddOperationCommand = new AsyncReactiveCommand<AddOperationViewModel>().WithSubscribe(EndAddOperation)
                                                                                      .AddTo(DisposableList);
            OperateCommand = new AsyncReactiveCommand().WithSubscribe(Operate)
                                                       .AddTo(DisposableList);
            AsRawCommand = new AsyncReactiveCommand().WithSubscribe(AsRaw)
                                                     .AddTo(DisposableList);
            DownloadCommand = new AsyncReactiveCommand().WithSubscribe(Download)
                                                        .AddTo(DisposableList);
        }

        /// <summary>
        /// 全ての処理を行います。
        /// </summary>
        /// <returns>処理結果のデータ</returns>
        private TextData? ExecuteAllOperations()
        {
            if (EditingFile.Value is null) return null;

            List<ProcessStatus> verificationResults = EditingFile.Value.Operations.ConvertAll(x => x.VerifyArguments());
            editModel.LogList.ClearOnScheduler();
            if (Operations.Count == 0) editModel.LogList.AddOnScheduler(new LogInfo(LogType.Info, "処理が記述されていません", string.Empty));
            OutputStatus(verificationResults);
            if (verificationResults.Exists(x => !x.Success)) return null;

            TextData result = EditingFile.Value.Data.Clone();

            var processResults = new List<ProcessStatus>();
            foreach (OperationViewModel operation in Operations) processResults.Add(operation.Operation.Operate(result));
            OutputStatus(processResults);
            if (verificationResults.Exists(x => !x.Success)) return null;
            else if (Operations.Count > 0) editModel.LogList.AddOnScheduler(new LogInfo(LogType.Success, "処理に成功しました", string.Empty));
            return result;
        }

        /// <summary>
        /// 処理結果を出力します。
        /// </summary>
        /// <param name="status">出力する処理結果</param>
        private void OutputStatus(IEnumerable<ProcessStatus> status)
        {
            const int Duration = 2000;

            bool useNotify = LogTableCollapsed.Value && initialized;
            foreach (ProcessStatus currentStatus in status)
            {
                PublishStatus(currentStatus, NotificationSeverity.Error, useNotify);
                PublishStatus(currentStatus, NotificationSeverity.Warning, useNotify);
                PublishStatus(currentStatus, NotificationSeverity.Info, false);
            }

            void PublishStatus(ProcessStatus status, NotificationSeverity severity, bool useNotify)
            {
                IList<StatusEntry> entries;
                LogType level;
                switch (severity)
                {
                    case NotificationSeverity.Error:
                        entries = status.Errors;
                        level = LogType.Error;
                        break;

                    case NotificationSeverity.Warning:
                        entries = status.Warnings;
                        level = LogType.Warning;
                        break;

                    case NotificationSeverity.Info:
                        entries = status.Messages;
                        level = LogType.Info;
                        break;

                    default: throw new ArgumentOutOfRangeException(nameof(severity));
                }
                foreach (StatusEntry entry in entries)
                    if (useNotify)
                        notificationService.Notify(severity, entry.Message, entry.Arg is null ? entry.TargetName : $"{entry.TargetName}/{entry.Arg.Name}", Duration);
                editModel.LogList.AddRangeOnScheduler(entries.Select(x => new LogInfo(level, x.Message, x.Arg is null ? x.TargetName : $"{x.TargetName}/{x.Arg.Name}")));
            }
        }

        /// <summary>
        /// 編集ファイルが変更されたときに通知されます。
        /// </summary>
        /// <param name="value">変更後の値</param>
        private void OnSelectionChanged(TableFileInfo? value)
        {
            TableFileInfo? old = mainModel.CurrentEditData.Value;
            mainModel.CurrentEditData.Value = value;

            if (old is not null)
            {
                Operations.ClearOnScheduler();
                editModel.LogList.ClearOnScheduler();
            }
            if (value is not null)
                foreach (Operation current in value.Operations)
                    Operations.Add(new OperationViewModel(this, current));
            ViewData.Value = ExecuteAllOperations();
            if (ViewData.Value is null) ViewData.Value = EditingFile.Value?.Data;
        }

        /// <summary>
        ///追加処理の終了処理を行います。
        /// </summary>
        /// <param name="vm">処理結果</param>
        private async Task EndAddOperation(AddOperationViewModel vm)
        {
            Operation operation = vm.SelectedOperation.Value.Clone();
            Operations.AddOnScheduler(new OperationViewModel(this, operation));
            EditingFile.Value!.Operations.Add(operation);
            await Task.CompletedTask;
        }

        /// <summary>
        /// 処理を実行します。
        /// </summary>
        /// <returns></returns>
        private async Task Operate()
        {
            TextData? result = ExecuteAllOperations();
            if (result is null) return;
            ViewData.Value = result;
            await Task.CompletedTask;
        }

        /// <summary>
        /// 表示データを処理前のものに変更します。
        /// </summary>
        private async Task AsRaw()
        {
            ViewData.Value = EditingFile.Value?.Data;
            await Task.CompletedTask;
        }

        /// <summary>
        /// 処理後のファイルをダウンロードします。
        /// </summary>
        private async Task Download()
        {
            TextData? data = ExecuteAllOperations();
            if (data is null)
            {
                notificationService.Notify(NotificationSeverity.Error, "処理に失敗したためダウンロードできません", duration: 10000);
                return;
            }

            ViewData.Value = data;

            Stream? stream = null;
            try
            {
                stream = new MemoryStream(1024 * 1024 * 16); // 16 MB
                using (var writer = new StreamWriter(stream, leaveOpen: true))
                {
                    await data.WriteToAsync(writer, new TextSaveOptions("\t"));
                }
                stream.Position = 0;
            }
            catch
            {
                notificationService.Notify(NotificationSeverity.Error, "ファイル生成時にエラーが発生しました");
                stream?.Dispose();
                return;
            }

            await mainModel.DownloadFile(EditingFile.Value!.Name, stream);
            try
            {
            }
            catch
            {
                notificationService.Notify(NotificationSeverity.Error, "ダウンロード時にエラーが発生しました");
                stream.Dispose();
                return;
            }
            stream.Dispose();
        }

        /// <summary>
        /// 処理を削除します。
        /// </summary>
        /// <param name="operation">削除する処理</param>
        public async Task RemoveOperation(OperationViewModel operation)
        {
            TableFileInfo? data = EditingFile.Value;
            if (data is null) return;
            Operations.RemoveOnScheduler(operation);
            data.Operations.Remove(operation.Operation);
            await AsyncMessageBroker.Default.PublishAsync(new ReRenderMessage()
            {
                Target = "op-list",
            });
        }

        /// <summary>
        /// 処理を一つ下に移動します。
        /// </summary>
        /// <param name="operation">移動する処理</param>
        public async Task DownLocation(OperationViewModel operation)
        {
            if (EditingFile.Value is null) return;
            int index = Operations.IndexOf(operation);
            if (index < 0 || index == Operations.Count - 1) return;
            OperationViewModel swapped = Operations[index + 1];
            Operations.MoveOnScheduler(index, index + 1);
            List<Operation> list = EditingFile.Value.Operations;
            list[index] = swapped.Operation;
            list[index + 1] = operation.Operation;

            await AsyncMessageBroker.Default.PublishAsync(new ReRenderMessage()
            {
                Target = "op-list",
            });
        }

        /// <summary>
        /// 処理を一つ上に移動します。
        /// </summary>
        /// <param name="operation">移動する処理</param>
        public async Task UpLocation(OperationViewModel operation)
        {
            if (EditingFile.Value is null) return;
            int index = Operations.IndexOf(operation);
            if (index <= 0) return;
            OperationViewModel swapped = Operations[index - 1];
            Operations.MoveOnScheduler(index, index - 1);
            List<Operation> list = EditingFile.Value.Operations;
            list[index] = swapped.Operation;
            list[index - 1] = operation.Operation;

            await AsyncMessageBroker.Default.PublishAsync(new ReRenderMessage()
            {
                Target = "op-list",
            });
        }

        /// <inheritdoc/>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            if (initialized) return;
            initialized = true;

#if DEBUG
            if (Files.Count == 0)
            {
                var data1 = TextData.CreateFromRawData(new[]
                {
                    new[] { "Index", "Name", "Age", },
                    new[] { "1", "Tanaka", "20", },
                    new[] { "2", "Sato", "43", },
                    new[] { "3", "Suzuki", "33", },
                    new[] { "4", "Yamada", "16", },
                });
                data1.HasHeader = true;
                var info1 = new TableFileInfo("test1.tsv", data1);
                mainModel.Files.AddOnScheduler(info1);
                var data2 = TextData.CreateFromRawData(new[]
                {
                    new[] { "Index", "Address", },
                    new[] { "3", "Osaka", },
                    new[] { "1", "Nagoya", },
                    new[] { "2", "Tokyo", },
                    new[] { "5", "Yokohama" },
                });
                data2.HasHeader = true;
                var info2 = new TableFileInfo("test2.tsv", data2);
                mainModel.Files.AddOnScheduler(info2);
                EditingFile.Value = info1;
            }
#endif
        }
    }
}
