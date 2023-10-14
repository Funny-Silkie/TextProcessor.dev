using System;
using System.Collections.Generic;
using TextProcessor.Logics.Data;
using TextProcessor.Logics.Operations.OperationImpl;

namespace TextProcessor.Logics.Operations
{
    /// <summary>
    /// <see cref="TextData"/>に行う処理の基底クラスです。
    /// </summary>
    [Serializable]
    public abstract class Operation : ICloneable, IHasDefinedSet<Operation>, IHasArguments
    {
        /// <inheritdoc/>
        public abstract string Title { get; }

        /// <summary>
        /// 引数一覧を取得します。
        /// </summary>
        public IList<ArgumentInfo> Arguments => _arguments ??= GenerateArguments();

        [NonSerialized]
        private IList<ArgumentInfo>? _arguments;

        /// <summary>
        /// <see cref="Operation"/>の新しいインスタンスを初期化します。
        /// </summary>
        protected Operation()
        {
        }

        /// <summary>
        /// <see cref="Operation"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected Operation(Operation cloned) : this()
        {
            ArgumentNullException.ThrowIfNull(cloned);
        }

        /// <inheritdoc/>
        public static Operation[] GetDefinedSet()
        {
            return new Operation[]
            {
                new FilterRowOperation(),
                new HeadOperation(),
                new TailOperation(),
                new SkipHeadOperation(),
                new SkipTailOperation(),
                new SelectColumnOperation(),
                new ReplaceOperation(),
                new SortOperation(),
                new DistinctOperation(),
                new AppendOperation(),
                new PrependOperation(),
                new PasteOperation(),
                new InnerJoinOperation(),
                new LeftOuterJoinOperation(),
                new FullOuterJoinOperation(),
                new DeleteColumnOperation(),
                new DeleteRowOperation(),
                new GenerateColumnOperation(),
                new EditColumnOperation(),
                new ChangeHeaderOperation(),
            };
        }

        /// <summary>
        /// 引数情報を生成します。
        /// </summary>
        /// <returns>引数情報一覧</returns>
        protected abstract IList<ArgumentInfo> GenerateArguments();

        /// <inheritdoc cref="IHasArguments.VerifyArguments"/>
        public ProcessStatus VerifyArguments()
        {
            var result = new ProcessStatus();
            VerifyArgumentsCore(result);
            return result;
        }

        /// <summary>
        /// <inheritdoc cref="IHasArguments.VerifyArguments"/>
        /// </summary>
        /// <param name="status">チェック結果</param>
        protected abstract void VerifyArgumentsCore(ProcessStatus status);

        /// <inheritdoc cref="ICloneable.Clone"/>
        public Operation Clone() => CloneCore();

        object ICloneable.Clone() => Clone();

        /// <summary>
        /// 複製処理を行います。
        /// </summary>
        /// <returns>複製されたインスタンス</returns>
        protected abstract Operation CloneCore();

        /// <summary>
        /// 処理を行います。
        /// </summary>
        /// <param name="data">処理するデータ</param>
        /// <returns>処理プロセスのステータス</returns>
        /// <exception cref="ArgumentNullException"><paramref name="data"/>が<see langword="null"/></exception>
        public ProcessStatus Operate(TextData data)
        {
            ArgumentNullException.ThrowIfNull(data);
            var result = new ProcessStatus();
            OperateCore(data, result);
            data.FillBlanks();
            return result;
        }

        /// <summary>
        /// 処理を行います。
        /// </summary>
        /// <param name="data">処理するデータ</param>
        /// <param name="status">処理プロセスのステータス</param>
        protected abstract void OperateCore(TextData data, ProcessStatus status);
    }
}
