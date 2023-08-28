using System;
using System.Collections.Generic;

namespace TextProcessor.Logics.Operations.Conditions
{
    /// <summary>
    /// 行に対する条件を表します。
    /// </summary>
    [Serializable]
    public abstract class RowCondition : Condition<IList<string>, RowCondition>, IHasDefinedSet<RowCondition>, IHasArguments
    {
        /// <summary>
        /// 空の条件を取得します。
        /// </summary>
        public static RowCondition Null { get; } = new NullConditions();

        /// <inheritdoc/>
        public abstract string? Title { get; }

        /// <summary>
        /// <see cref="RowCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        protected RowCondition()
        {
        }

        /// <summary>
        /// <see cref="RowCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected RowCondition(RowCondition cloned)
            : base(cloned)
        {
        }

        /// <inheritdoc/>
        public static RowCondition[] GetDefinedSet()
        {
            return new RowCondition[]
            {
                Null,
                new NotRowCondition(),
                new OrRowCondition(),
                new AndRowCondition(),
                new CheckValueRowCondition(0, ValueCondition.Null),
                new CheckAllValueRowCondition(ValueCondition.Null),
                new CheckAnyValueRowCondition(ValueCondition.Null),
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
        /// <param name="errors">エラーの登録先</param>
        protected virtual void VerifyArgumentsCore(ProcessStatus status)
        {
        }

        /// <summary>
        /// 空の条件を表します。
        /// </summary>
        [Serializable]
        private sealed class NullConditions : RowCondition
        {
            /// <inheritdoc/>
            public override string? Title => "条件を設定してください";

            /// <inheritdoc/>
            public override bool Equals(object? obj) => obj is NullConditions;

            /// <inheritdoc/>
            public override int GetHashCode() => GetType().Name.GetHashCode();

            /// <inheritdoc/>
            public override MatchResult Match(IList<string> row) => MatchResult.Error;
        }
    }

    /// <summary>
    /// NOT条件を表します。
    /// </summary>
    [Serializable]
    internal sealed class NotRowCondition : RowCondition
    {
        private RowCondition condition;

        /// <inheritdoc/>
        public override string? Title => "NOT";

        /// <summary>
        /// <see cref="NotRowCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        public NotRowCondition()
        {
            condition = Null;
        }

        /// <summary>
        /// <see cref="NotRowCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        private NotRowCondition(NotRowCondition cloned)
            : base(cloned)
        {
            condition = cloned.condition.Clone();
        }

        /// <inheritdoc/>
        protected override RowCondition CloneCore() => new NotRowCondition(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.RowCondition, "条件", () => condition, x => condition = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            StatusHelper.VerifyRowCondition(Title, status, Arguments[0], condition);
        }

        /// <inheritdoc/>
        public override MatchResult Match(IList<string> target)
        {
            MatchResult match = condition.Match(target);
            return match switch
            {
                MatchResult.Matched => MatchResult.NotMatched,
                MatchResult.NotMatched => MatchResult.Matched,
                _ => match,
            };
        }
    }

    /// <summary>
    /// AND条件を表します。
    /// </summary>
    [Serializable]
    internal sealed class AndRowCondition : RowCondition
    {
        private RowCondition[] conditions;

        /// <inheritdoc/>
        public override string? Title => "AND";

        /// <summary>
        /// <see cref="AndRowCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        public AndRowCondition()
        {
            conditions = new RowCondition[2];
            Array.Fill(conditions, Null);
        }

        /// <summary>
        /// <see cref="AndRowCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        private AndRowCondition(AndRowCondition cloned)
            : base(cloned)
        {
            conditions = cloned.conditions.CloneAll();
        }

        /// <inheritdoc/>
        protected override RowCondition CloneCore() => new AndRowCondition(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.RowCondition | ArgumentType.Array, "条件", () => conditions, x => conditions = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                StatusHelper.VerifyRowCondition(Title, status, Arguments[0], conditions[i]);
            }
        }

        /// <inheritdoc/>
        public override MatchResult Match(IList<string> target)
        {
            foreach (RowCondition currentCondition in conditions)
            {
                MatchResult currentResult = currentCondition.Match(target);
                if (currentResult != MatchResult.Matched) return currentResult;
            }

            return MatchResult.Matched;
        }
    }

    /// <summary>
    /// OR条件を表します。
    /// </summary>
    [Serializable]
    internal sealed class OrRowCondition : RowCondition
    {
        private RowCondition[] conditions;

        /// <inheritdoc/>
        public override string? Title => "OR";

        /// <summary>
        /// <see cref="OrRowCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        public OrRowCondition()
        {
            conditions = new RowCondition[2];
            Array.Fill(conditions, Null);
        }

        /// <summary>
        /// <see cref="OrRowCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        private OrRowCondition(OrRowCondition cloned)
            : base(cloned)
        {
            conditions = cloned.conditions.CloneAll();
        }

        /// <inheritdoc/>
        protected override RowCondition CloneCore() => new OrRowCondition(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.RowCondition | ArgumentType.Array, "条件", () => conditions, x => conditions = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                StatusHelper.VerifyRowCondition(Title, status, Arguments[0], conditions[i]);
            }
        }

        /// <inheritdoc/>
        public override MatchResult Match(IList<string> target)
        {
            foreach (RowCondition currentCondition in conditions)
            {
                MatchResult currentResult = currentCondition.Match(target);
                if (currentResult != MatchResult.NotMatched) return currentResult;
            }

            return MatchResult.NotMatched;
        }
    }

    /// <summary>
    /// 指定したインデックスの値の要素を検証します。
    /// </summary>
    [Serializable]
    internal sealed class CheckValueRowCondition : RowCondition
    {
        private int valueIndex;
        private ValueCondition valueCondition;

        /// <inheritdoc/>
        public override string? Title => "指定した列のセルが条件を満たす";

        /// <summary>
        /// <see cref="CheckValueRowCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="valueIndex">検証する値のインデックス</param>
        /// <param name="valueCondition">値に対する条件</param>
        /// <exception cref="ArgumentNullException"><paramref name="valueCondition"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="valueIndex"/>が0未満</exception>
        public CheckValueRowCondition(int valueIndex, ValueCondition valueCondition)
        {
            ArgumentNullException.ThrowIfNull(valueCondition);
            if (valueIndex < 0) throw new ArgumentOutOfRangeException(nameof(valueIndex));

            this.valueIndex = valueIndex;
            this.valueCondition = valueCondition;
        }

        /// <summary>
        /// <see cref="CheckValueRowCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        private CheckValueRowCondition(CheckValueRowCondition cloned)
            : base(cloned)
        {
            valueIndex = cloned.valueIndex;
            valueCondition = cloned.valueCondition.Clone();
        }

        /// <inheritdoc/>
        protected override RowCondition CloneCore() => new CheckValueRowCondition(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.Index, "列番号", () => valueIndex, x => valueIndex = x),
                new ArgumentInfo(ArgumentType.ValueCondition, "条件", () => valueCondition, x => valueCondition = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            StatusHelper.VerifyValueCondition(Title, status, Arguments[1], valueCondition);
        }

        /// <inheritdoc/>
        public override MatchResult Match(IList<string> target)
        {
            if (valueIndex >= target.Count) return valueCondition.Match(string.Empty);
            return valueCondition.Match(target[valueIndex]);
        }
    }

    /// <summary>
    /// 全ての値の要素を検証します。
    /// </summary>
    [Serializable]
    internal sealed class CheckAllValueRowCondition : RowCondition
    {
        private ValueCondition valueCondition;

        /// <inheritdoc/>
        public override string? Title => "行内の全てのセルが条件を満たす";

        /// <summary>
        /// <see cref="CheckAllValueRowCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="valueCondition">値に対する条件</param>
        /// <exception cref="ArgumentNullException"><paramref name="valueCondition"/>が<see langword="null"/></exception>
        public CheckAllValueRowCondition(ValueCondition valueCondition)
        {
            ArgumentNullException.ThrowIfNull(valueCondition);

            this.valueCondition = valueCondition;
        }

        /// <summary>
        /// <see cref="CheckAllValueRowCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        private CheckAllValueRowCondition(CheckAllValueRowCondition cloned)
            : base(cloned)
        {
            valueCondition = cloned.valueCondition.Clone();
        }

        /// <inheritdoc/>
        protected override RowCondition CloneCore() => new CheckAllValueRowCondition(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.ValueCondition, "条件", () => valueCondition, x => valueCondition = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            StatusHelper.VerifyValueCondition(Title, status, Arguments[0], valueCondition);
        }

        /// <inheritdoc/>
        public override MatchResult Match(IList<string> target)
        {
            foreach (string current in target)
            {
                MatchResult match = valueCondition.Match(current);
                if (match != MatchResult.Matched) return match;
            }
            return MatchResult.Matched;
        }
    }

    /// <summary>
    /// 全ての値の要素を検証します。
    /// </summary>
    [Serializable]
    internal sealed class CheckAnyValueRowCondition : RowCondition
    {
        private ValueCondition valueCondition;

        /// <inheritdoc/>
        public override string? Title => "行内のいずれかのセルが条件を満たす";

        /// <summary>
        /// <see cref="CheckAnyValueRowCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="valueCondition">値に対する条件</param>
        /// <exception cref="ArgumentNullException"><paramref name="valueCondition"/>が<see langword="null"/></exception>
        public CheckAnyValueRowCondition(ValueCondition valueCondition)
        {
            ArgumentNullException.ThrowIfNull(valueCondition);

            this.valueCondition = valueCondition;
        }

        /// <summary>
        /// <see cref="CheckAnyValueRowCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        private CheckAnyValueRowCondition(CheckAnyValueRowCondition cloned)
            : base(cloned)
        {
            valueCondition = cloned.valueCondition.Clone();
        }

        /// <inheritdoc/>
        protected override RowCondition CloneCore() => new CheckAnyValueRowCondition(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.ValueCondition, "条件", () => valueCondition, x => valueCondition = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            StatusHelper.VerifyValueCondition(Title, status, Arguments[0], valueCondition);
        }

        /// <inheritdoc/>
        public override MatchResult Match(IList<string> target)
        {
            foreach (string current in target)
            {
                MatchResult match = valueCondition.Match(current);
                if (match != MatchResult.NotMatched) return match;
            }
            return MatchResult.NotMatched;
        }
    }
}
