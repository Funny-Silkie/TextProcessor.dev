using System.Reactive.Disposables;

namespace TextProcessor.Models
{
    /// <summary>
    /// モデルの基底クラスを表します。
    /// </summary>
    public abstract class ModelBase
    {
        /// <summary>
        /// 破棄可能オブジェクトのリストを取得します。
        /// </summary>
        protected CompositeDisposable DisposableList { get; }

        /// <summary>
        /// <see cref="ModelBase"/>の新しいインスタンスを初期化します。
        /// </summary>
        protected ModelBase()
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
