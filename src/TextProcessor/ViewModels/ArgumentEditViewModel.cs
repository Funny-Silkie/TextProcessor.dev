using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using TextProcessor.Data;
using TextProcessor.Models;

namespace TextProcessor.ViewModels
{
    /// <summary>
    /// 引数編集画面のViewModelのクラスです。
    /// </summary>
    [InjectionRange(InjectionType.Transient)]
    public class ArgumentEditViewModel : ViewModelBase
    {
        #region Properties

        /// <summary>
        /// ファイル一覧を取得します。
        /// </summary>
        public ReadOnlyReactiveCollection<TableFileInfo> Files { get; }

        #endregion Properties

        /// <summary>
        /// <see cref="ArgumentEditViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public ArgumentEditViewModel(MainModel mainModel)
        {
            Files = mainModel.Files.ToReadOnlyReactiveCollection()
                                   .AddTo(DisposableList);
        }
    }
}
