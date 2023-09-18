using Radzen;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Threading.Tasks;
using TextProcessor.Logics.Operations;

namespace TextProcessor.ViewModels
{
    /// <summary>
    /// 処理追加画面のViewModelのクラスです。
    /// </summary>
    [InjectionRange(InjectionType.Transient)]
    public class AddOperationViewModel : ViewModelBase
    {
        private readonly DialogService dialogService;

        /// <summary>
        /// ソースを取得します。
        /// </summary>
        public Operation[] Source { get; }

        #region Properties

        /// <summary>
        /// 選択されている処理を取得または設定します。
        /// </summary>
        public ReactivePropertySlim<Operation> SelectedOperation { get; }

        #endregion Properties

        #region Commands

        /// <inheritdoc cref="Ok"/>
        public AsyncReactiveCommand CommandOk { get; }

        /// <inheritdoc cref="Cancel"/>
        public AsyncReactiveCommand CommandCancel { get; }

        #endregion Commands

        /// <summary>
        /// <see cref="AddOperationViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public AddOperationViewModel(DialogService dialogService)
        {
            this.dialogService = dialogService;

            Source = Operation.GetDefinedSet();
            SelectedOperation = new ReactivePropertySlim<Operation>(Source[0]).AddTo(DisposableList);

            CommandOk = new AsyncReactiveCommand().WithSubscribe(Ok)
                                                  .AddTo(DisposableList);
            CommandCancel = new AsyncReactiveCommand().WithSubscribe(Cancel)
                                                      .AddTo(DisposableList);
        }

        /// <summary>
        /// ダイアログを閉じて追加処理を実行します。
        /// </summary>
        private async Task Ok()
        {
            dialogService.Close(this);
            await Task.CompletedTask;
        }

        /// <summary>
        /// ダイアログを閉じて追加処理を中断します。
        /// </summary>
        private async Task Cancel()
        {
            dialogService.Close(null);
            await Task.CompletedTask;
        }
    }
}
