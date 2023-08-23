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
        public ReadOnlyReactiveCollection<DsvFileInfo> Files { get; }

        /// <summary>
        /// 選択中ファイルを取得または設定します。
        /// </summary>
        public ReactivePropertySlim<DsvFileInfo?> SelectedData { get; }

        /// <summary>
        /// <see cref="RawViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public RawViewModel(NavigationManager navigationManager, MainModel mainModel)
            : base(navigationManager)
        {
            Files = mainModel.Files.ToReadOnlyReactiveCollection()
                                   .AddTo(DisposableList);
            SelectedData = new ReactivePropertySlim<DsvFileInfo?>().AddTo(DisposableList);
        }
    }
}
