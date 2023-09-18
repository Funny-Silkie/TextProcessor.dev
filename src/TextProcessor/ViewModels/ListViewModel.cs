using Microsoft.AspNetCore.Components;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Threading.Tasks;
using TextProcessor.Data;
using TextProcessor.Models;

namespace TextProcessor.ViewModels
{
    /// <summary>
    /// ファイル管理画面のViewModelのクラスです。
    /// </summary>
    [InjectionRange(InjectionType.Transient)]
    public class ListViewModel : PageViewModel
    {
        private readonly MainModel mainModel;

        #region Properties

        /// <summary>
        /// ファイル一覧を取得します。
        /// </summary>
        public ReadOnlyReactiveCollection<TableFileInfo> Files { get; }

        #endregion Properties

        #region Commands

        /// <inheritdoc cref="View(TableFileInfo)"/>
        public AsyncReactiveCommand<TableFileInfo> ViewCommand { get; }

        /// <inheritdoc cref="Edit(TableFileInfo)"/>
        public AsyncReactiveCommand<TableFileInfo> EditCommand { get; }

        /// <inheritdoc cref="Delete(TableFileInfo)"/>
        public AsyncReactiveCommand<TableFileInfo> DeleteCommand { get; }

        #endregion Commands

        /// <summary>
        /// <see cref="ListViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public ListViewModel(NavigationManager navigationManager, MainModel mainModel)
            : base(navigationManager)
        {
            this.mainModel = mainModel;

            Files = mainModel.Files.ToReadOnlyReactiveCollection()
                                   .AddTo(DisposableList);
            ViewCommand = new AsyncReactiveCommand<TableFileInfo>().WithSubscribe(View)
                                                                 .AddTo(DisposableList);
            EditCommand = new AsyncReactiveCommand<TableFileInfo>().WithSubscribe(Edit)
                                                                 .AddTo(DisposableList);
            DeleteCommand = new AsyncReactiveCommand<TableFileInfo>().WithSubscribe(Delete)
                                                                   .AddTo(DisposableList);
        }

        /// <summary>
        /// 閲覧画面に遷移します。
        /// </summary>
        /// <param name="file">閲覧するファイル</param>
        private async Task View(TableFileInfo file)
        {
            int index = Files.IndexOf(file);
            if (index < 0) NavigationManager.NavigateTo("raw");
            else NavigationManager.NavigateTo($"raw/{index}");
            await Task.CompletedTask;
        }

        /// <summary>
        /// 編集画面に遷移します。
        /// </summary>
        /// <param name="file">編集するファイル</param>
        private async Task Edit(TableFileInfo file)
        {
            mainModel.CurrentEditData.Value = file;
            NavigationManager.NavigateTo("edit");
            await Task.CompletedTask;
        }

        /// <summary>
        /// 削除処理を行います。
        /// </summary>
        /// <param name="file">削除するファイル</param>
        /// <returns></returns>
        private async Task Delete(TableFileInfo file)
        {
            mainModel.Files.RemoveOnScheduler(file);
            if (mainModel.CurrentEditData.Value == file) mainModel.CurrentEditData.Value = null;
            await Task.CompletedTask;
        }
    }
}
