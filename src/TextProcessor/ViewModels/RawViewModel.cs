using Microsoft.AspNetCore.Components;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using TextProcessor.Data;
using TextProcessor.Models;

namespace TextProcessor.ViewModels
{
    /// <summary>
    /// 生データ表示画面のViewModelのクラスです。
    /// </summary>
    [InjectionRange(InjectionType.Transient)]
    public class RawViewModel : PageViewModel
    {
        /// <summary>
        /// ファイル一覧を取得します。
        /// </summary>
        public ReadOnlyReactiveCollection<TableFileInfo> Files { get; }

        /// <summary>
        /// 選択中ファイルを取得または設定します。
        /// </summary>
        public ReactivePropertySlim<TableFileInfo?> SelectedData { get; }

        /// <summary>
        /// <see cref="RawViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public RawViewModel(NavigationManager navigationManager, MainModel mainModel)
            : base(navigationManager)
        {
            Files = mainModel.Files.ToReadOnlyReactiveCollection()
                                   .AddTo(DisposableList);
            SelectedData = new ReactivePropertySlim<TableFileInfo?>().AddTo(DisposableList);
        }
    }
}
