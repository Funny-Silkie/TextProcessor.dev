using System;
using System.Collections.Generic;

namespace TextProcessor.Logics.Operations.Conditions
{
    /// <summary>
    /// 値を検証する条件を表します。
    /// </summary>
    /// <typeparam name="TTarget">検証する値の型</typeparam>
    /// <typeparam name="TSelf">自身の型</typeparam>
    [Serializable]
    public abstract class Condition<TTarget, TSelf> : ICloneable
        where TSelf : Condition<TTarget, TSelf>
    {
        /// <summary>
        /// 引数一覧を取得します。
        /// </summary>
        public IList<ArgumentInfo> Arguments => _arguments ??= GenerateArguments();

        [NonSerialized]
        private IList<ArgumentInfo>? _arguments;

        /// <summary>
        /// <see cref="Condition{TTarget, TSelf}"/>の新しいインスタンスを初期化します。
        /// </summary>
        protected Condition()
        {
        }

        /// <summary>
        /// <see cref="Condition{TTarget, TSelf}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected Condition(Condition<TTarget, TSelf> cloned) : this()
        {
            ArgumentNullException.ThrowIfNull(cloned);
        }

        /// <summary>
        /// 引数情報を生成します。
        /// </summary>
        /// <returns>引数情報一覧</returns>
        protected virtual IList<ArgumentInfo> GenerateArguments() => Array.Empty<ArgumentInfo>();

        /// <inheritdoc cref="ICloneable.Clone"/>
        public TSelf Clone() => CloneCore();

        object ICloneable.Clone() => Clone();

        /// <summary>
        /// 複製処理を行います。
        /// </summary>
        /// <returns>複製されたインスタンス</returns>
        protected virtual TSelf CloneCore() => (TSelf)MemberwiseClone();

        /// <summary>
        /// 条件に適合するかどうかを検証します。
        /// </summary>
        /// <param name="target">検証対象</param>
        /// <returns>検証結果</returns>
        public abstract MatchResult Match(TTarget target);
    }
}
