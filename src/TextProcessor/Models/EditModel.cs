using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using TextProcessor.Data;

namespace TextProcessor.Models
{
    /// <summary>
    /// 編集画面のモデルのクラスです。
    /// </summary>
    [InjectionRange(InjectionType.Scoped)]
    public class EditModel : ModelBase
    {
        /// <summary>
        /// ログ一覧を取得します。
        /// </summary>
        public ReactiveCollection<LogInfo> LogList { get; }

        /// <summary>
        /// ログ通知を送るかどうかを取得または設定します。
        /// </summary>
        public ReactiveProperty<bool> SendLogNotification { get; }

        /// <summary>
        /// <see cref="EditModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public EditModel()
        {
            LogList = new ReactiveCollection<LogInfo>().AddTo(DisposableList);
            SendLogNotification = new ReactiveProperty<bool>(false).AddTo(DisposableList);
        }
    }
}
