using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace TextProcessor.ViewModels
{
    /// <summary>
    /// ページ用のViewModelのクラスです。
    /// </summary>
    public abstract class PageViewModel : ViewModelBase
    {
        /// <summary>
        /// 使用する<see cref="Microsoft.AspNetCore.Components.NavigationManager"/>のインスタンスを取得します。
        /// </summary>
        protected NavigationManager NavigationManager { get; }

        /// <summary>
        /// <see cref="PageViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        protected PageViewModel(NavigationManager navigationManager)
        {
            NavigationManager = navigationManager;
        }

        /// <summary>
        /// オーバーライドして初期化の処理を記述します。
        /// </summary>
        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 指定したURLへ移動します。
        /// </summary>
        /// <param name="url">移動先URL</param>
        /// <param name="replace">移動履歴を上書きするかどうか</param>
        public void NavigateTo(string url, bool replace = false)
        {
            NavigationManager.NavigateTo(url, replace: replace);
        }
    }
}
