using System;

namespace TextProcessor.Logics.Operations.Conditions
{
    /// <summary>
    /// 値に対する条件を表します。
    /// </summary>
    [Serializable]
    public abstract class ValueCondition : Condition<string, ValueCondition>, IHasDefinedSet<ValueCondition>, IHasArguments
    {
        /// <summary>
        /// 空の条件を取得します。
        /// </summary>
        public static ValueCondition Null { get; } = new NullConditions();

        /// <inheritdoc/>
        public abstract string? Title { get; }

        /// <summary>
        /// <see cref="ValueCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        protected ValueCondition()
        {
        }

        /// <summary>
        /// <see cref="ValueCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected ValueCondition(ValueCondition cloned)
            : base(cloned)
        {
        }

        /// <inheritdoc/>
        public static ValueCondition[] GetDefinedSet()
        {
            return new ValueCondition[]
            {
                Null,
                new NotValueCondition(),
                new OrValueCondition(),
                new AndValueCondition(),
                new IsEmptyValueCondition(),
                new IsIntegerValueCondition(),
                new IsDecimalValueCondition(),
                new MatchValueCondition(),
                new ContainsValueCondition(),
                new StartsWithValueCondition(),
                new EndsWithValueCondition(),
                new RegexMatchValueCondition(),
                ValueConditionFactory.LargerAsInteger(),
                ValueConditionFactory.LargerAsDecimal(),
                ValueConditionFactory.LargerAsDateOnly(),
                ValueConditionFactory.LowerAsInteger(),
                ValueConditionFactory.LowerAsDecimal(),
                ValueConditionFactory.LowerAsDateOnly(),
                ValueConditionFactory.LargerOrEqualAsInteger(),
                ValueConditionFactory.LargerOrEqualAsDecimal(),
                ValueConditionFactory.LargerOrEqualAsDateOnly(),
                ValueConditionFactory.LowerOrEqualAsInteger(),
                ValueConditionFactory.LowerOrEqualAsDecimal(),
                ValueConditionFactory.LowerOrEqualAsDateOnly(),
                ValueConditionFactory.EqualAsInteger(),
                ValueConditionFactory.EqualAsDecimal(),
                ValueConditionFactory.EqualAsDateOnly(),
            };
        }

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
        protected virtual void VerifyArgumentsCore(ProcessStatus status)
        {
        }

        /// <summary>
        /// 空の条件を表します。
        /// </summary>
        [Serializable]
        private sealed class NullConditions : ValueCondition
        {
            /// <inheritdoc/>
            public override string? Title => "条件を設定してください";

            /// <inheritdoc/>
            public override bool Equals(object? obj) => obj is NullConditions;

            /// <inheritdoc/>
            public override int GetHashCode() => GetType().Name.GetHashCode();

            /// <inheritdoc/>
            public override MatchResult Match(string target) => MatchResult.Error;
        }
    }
}
