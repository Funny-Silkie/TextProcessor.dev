using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;
using System;
using TextProcessor.Data;
using TextProcessor.Models;

namespace TextProcessor.ViewModels
{
    /// <summary>
    /// ログ一覧におけるViewModelのクラスです。
    /// </summary>
    [InjectionRange(InjectionType.Transient)]
    public class LogListViewModel : ViewModelBase
    {
        private readonly IFilteredReadOnlyObservableCollection<LogInfo> filterableLogList;

        #region Properties

        /// <summary>
        /// ログ一覧を取得します。
        /// </summary>
        public ReadOnlyReactiveCollection<LogInfo> LogList { get; }

        /// <summary>
        /// <see cref="LogType.Info"/>のメッセージを表示するかどうかを表す値を取得または設定します。
        /// </summary>
        public ReactivePropertySlim<bool> ShowInfo { get; }

        /// <summary>
        /// <see cref="LogType.Warning"/>のメッセージを表示するかどうかを表す値を取得または設定します。
        /// </summary>
        public ReactivePropertySlim<bool> ShowWarning { get; }

        /// <summary>
        /// <see cref="LogType.Error"/>のメッセージを表示するかどうかを表す値を取得または設定します。
        /// </summary>
        public ReactivePropertySlim<bool> ShowError { get; }

        /// <summary>
        /// <see cref="LogType.Success"/>のメッセージを表示するかどうかを表す値を取得または設定します。
        /// </summary>
        public ReactivePropertySlim<bool> ShowSuccess { get; }

        #endregion Properties

        /// <summary>
        /// <see cref="LogListViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public LogListViewModel(EditModel editModel)
        {
            filterableLogList = editModel.LogList.ToFilteredReadOnlyObservableCollection(FilterMessage)
                                                 .AddTo(DisposableList);
            LogList = filterableLogList.ToReadOnlyReactiveCollection()
                                       .AddTo(DisposableList);
            ShowInfo = new ReactivePropertySlim<bool>(true).AddTo(DisposableList);
            ShowInfo.Subscribe(UpdateFilter);
            ShowWarning = new ReactivePropertySlim<bool>(true).AddTo(DisposableList);
            ShowWarning.Subscribe(UpdateFilter);
            ShowError = new ReactivePropertySlim<bool>(true).AddTo(DisposableList);
            ShowError.Subscribe(UpdateFilter);
            ShowSuccess = new ReactivePropertySlim<bool>(true).AddTo(DisposableList);
            ShowSuccess.Subscribe(UpdateFilter);
        }

        /// <summary>
        /// メッセージフィルターを表します。
        /// </summary>
        /// <param name="log">メッセージ</param>
        /// <returns><paramref name="log"/>を表示する場合は<see langword="true"/>，それ以外で<see langword="false"/></returns>
        private bool FilterMessage(LogInfo log)
        {
            return log.Type switch
            {
                LogType.Info => ShowInfo.Value,
                LogType.Warning => ShowWarning.Value,
                LogType.Error => ShowError.Value,
                LogType.Success => ShowSuccess.Value,
                _ => false,
            };
        }

        /// <summary>
        /// フィルターの更新を行います。
        /// </summary>
        private void UpdateFilter(bool _)
        {
            filterableLogList.Refresh(FilterMessage);
        }
    }
}
