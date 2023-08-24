using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextProcessor.Logics.Operations;

namespace TextProcessor.ViewModels
{
    /// <summary>
    /// テキストファイルへの処理のViewModelのクラスです。
    /// </summary>
    public class OperationViewModel : ViewModelBase
    {
        private readonly EditViewModel editViewModel;

        /// <summary>
        /// <see cref="Logics.Operations.Operation"/>のインスタンスを取得します。s
        /// </summary>
        public Operation Operation { get; }

        /// <inheritdoc cref="Operation.Title"/>
        public string Title => Operation.Title;

        /// <inheritdoc cref="Operation.Arguments"/>
        public IList<ArgumentInfo> Arguments { get; }

        #region Commands

        /// <inheritdoc cref="RemoveSelf"/>
        public AsyncReactiveCommand RemoveSelfCommand { get; }

        /// <inheritdoc cref="DownLocation"/>
        public AsyncReactiveCommand DownLocationCommand { get; }

        /// <inheritdoc cref="UpLocation"/>
        public AsyncReactiveCommand UpLocationCommand { get; }

        #endregion Commands

        /// <summary>
        /// <see cref="OperationViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="editViewModel">編集画面のViewModel</param>
        /// <param name="operation">扱う処理</param>
        public OperationViewModel(EditViewModel editViewModel, Operation operation)
        {
            this.editViewModel = editViewModel;
            Operation = operation;
            Arguments = operation.Arguments;

            RemoveSelfCommand = new AsyncReactiveCommand().WithSubscribe(RemoveSelf)
                                                          .AddTo(DisposableList);
            DownLocationCommand = new AsyncReactiveCommand().WithSubscribe(DownLocation)
                                                            .AddTo(DisposableList);
            UpLocationCommand = new AsyncReactiveCommand().WithSubscribe(UpLocation)
                                                          .AddTo(DisposableList);
        }

        /// <summary>
        /// 自身を削除します。
        /// </summary>
        private async Task RemoveSelf()
        {
            await editViewModel.RemoveOperation(this);
        }

        /// <summary>
        /// 一つ下へ移動します。
        /// </summary>
        private async Task DownLocation()
        {
            await editViewModel.DownLocation(this);
        }

        /// <summary>
        /// 一つ上へ移動します。
        /// </summary>
        private async Task UpLocation()
        {
            await editViewModel.UpLocation(this);
        }
    }
}
