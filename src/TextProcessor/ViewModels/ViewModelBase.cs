using System;
using System.Reactive.Disposables;

namespace TextProcessor.ViewModels
{
    /// <summary>
    /// ViewModelの基底クラスを表します。
    /// </summary>
    public abstract class ViewModelBase : IDisposable
    {
        /// <summary>
        /// 破棄可能オブジェクトのリストを取得します。
        /// </summary>
        protected CompositeDisposable DisposableList { get; }

        /// <summary>
        /// <see cref="ViewModelBase"/>の新しいインスタンスを初期化します。
        /// </summary>
        protected ViewModelBase()
        {
            DisposableList = new CompositeDisposable();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            DisposableList.Dispose();
        }
    }
}
