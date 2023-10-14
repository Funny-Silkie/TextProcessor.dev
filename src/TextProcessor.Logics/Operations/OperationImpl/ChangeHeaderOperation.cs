using System;
using System.Collections.Generic;
using TextProcessor.Logics.Data;

namespace TextProcessor.Logics.Operations.OperationImpl
{
    /// <summary>
    /// ヘッダー状態変更処理を表します。
    /// </summary>
    [Serializable]
    internal class ChangeHeaderOperation : Operation
    {
        /// <inheritdoc/>
        public override string Title => "ヘッダー状態の変更";

        /// <summary>
        /// 先頭行をヘッダーとして扱うかどうかを表す値を取得または設定します。
        /// </summary>
        public bool HasHeader { get; set; } = true;

        /// <summary>
        /// <see cref="ChangeHeaderOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        public ChangeHeaderOperation()
        {
        }

        /// <summary>
        /// <see cref="ChangeHeaderOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected ChangeHeaderOperation(ChangeHeaderOperation cloned)
            : base(cloned)
        {
            HasHeader = cloned.HasHeader;
        }

        /// <inheritdoc/>
        protected override Operation CloneCore() => new ChangeHeaderOperation(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.Boolean, "先頭行をヘッダーとして扱う", () => HasHeader, x => HasHeader = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
        }

        /// <inheritdoc/>
        protected override void OperateCore(TextData data, ProcessStatus status)
        {
            data.HasHeader = HasHeader;
        }
    }
}
