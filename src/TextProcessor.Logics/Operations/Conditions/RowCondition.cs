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
                new CheckValueRowCondition(),
                new CheckAllValueRowCondition(),
                new CheckAnyValueRowCondition(),
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
        /// <param name="errors">チェック結果</param>
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
        /// <summary>
        /// 反転する条件を取得または設定します。
        /// </summary>
        public RowCondition Condition { get; set; }

        /// <inheritdoc/>
        public override string? Title => "NOT";

        /// <summary>
        /// <see cref="NotRowCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        public NotRowCondition()
        {
            Condition = Null;
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
            Condition = cloned.Condition.Clone();
        }

        /// <inheritdoc/>
        protected override RowCondition CloneCore() => new NotRowCondition(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.RowCondition, "条件", () => Condition, x => Condition = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            StatusHelper.VerifyRowCondition(Title, status, Arguments[0], Condition);
        }

        /// <inheritdoc/>
        public override MatchResult Match(IList<string> target)
        {
            MatchResult match = Condition.Match(target);
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
        /// <summary>
        /// 対象の条件一覧を取得または設定します。
        /// </summary>
        public List<RowCondition> Conditions { get; set; }

        /// <inheritdoc/>
        public override string? Title => "AND";

        /// <summary>
        /// <see cref="AndRowCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        public AndRowCondition()
        {
            Conditions = new List<RowCondition>() { Null, Null };
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
            Conditions = cloned.Conditions.CloneAll();
        }

        /// <inheritdoc/>
        protected override RowCondition CloneCore() => new AndRowCondition(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.RowCondition | ArgumentType.List, "条件", () => Conditions, x => Conditions = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            for (int i = 0; i < Conditions.Count; i++)
            {
                StatusHelper.VerifyRowCondition(Title, status, Arguments[0], Conditions[i]);
            }
        }

        /// <inheritdoc/>
        public override MatchResult Match(IList<string> target)
        {
            foreach (RowCondition currentCondition in Conditions)
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
        /// <summary>
        /// 対象の条件一覧を取得または設定します。
        /// </summary>
        public List<RowCondition> Conditions { get; set; }

        /// <inheritdoc/>
        public override string? Title => "OR";

        /// <summary>
        /// <see cref="OrRowCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        public OrRowCondition()
        {
            Conditions = new List<RowCondition>() { Null, Null };
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
            Conditions = cloned.Conditions.CloneAll();
        }

        /// <inheritdoc/>
        protected override RowCondition CloneCore() => new OrRowCondition(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.RowCondition | ArgumentType.List, "条件", () => Conditions, x => Conditions = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            for (int i = 0; i < Conditions.Count; i++)
            {
                StatusHelper.VerifyRowCondition(Title, status, Arguments[0], Conditions[i]);
            }
        }

        /// <inheritdoc/>
        public override MatchResult Match(IList<string> target)
        {
            foreach (RowCondition currentCondition in Conditions)
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
        /// <summary>
        /// 対象の列インデックスを取得または設定します。
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        /// 値に適用する条件を取得または設定します。
        /// </summary>
        public ValueCondition ValueCondition { get; set; }

        /// <inheritdoc/>
        public override string? Title => "指定した列のセルが条件を満たす";

        /// <summary>
        /// <see cref="CheckValueRowCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        public CheckValueRowCondition()
        {
            ColumnIndex = 0;
            ValueCondition = ValueCondition.Null;
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
            ColumnIndex = cloned.ColumnIndex;
            ValueCondition = cloned.ValueCondition.Clone();
        }

        /// <inheritdoc/>
        protected override RowCondition CloneCore() => new CheckValueRowCondition(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.Index, "列番号", () => ColumnIndex, x => ColumnIndex = x),
                new ArgumentInfo(ArgumentType.ValueCondition, "条件", () => ValueCondition, x => ValueCondition = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            StatusHelper.VerifyValueCondition(Title, status, Arguments[1], ValueCondition);
        }

        /// <inheritdoc/>
        public override MatchResult Match(IList<string> target)
        {
            if (ColumnIndex >= target.Count) return ValueCondition.Match(string.Empty);
            return ValueCondition.Match(target[ColumnIndex]);
        }
    }

    /// <summary>
    /// 全ての値の要素を検証します。
    /// </summary>
    [Serializable]
    internal sealed class CheckAllValueRowCondition : RowCondition
    {
        /// <summary>
        /// 値に適用する条件を取得または設定します。
        /// </summary>
        public ValueCondition ValueCondition { get; set; }

        /// <inheritdoc/>
        public override string? Title => "行内の全てのセルが条件を満たす";

        /// <summary>
        /// <see cref="CheckAllValueRowCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        public CheckAllValueRowCondition()
        {
            ValueCondition = ValueCondition.Null;
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
            ValueCondition = cloned.ValueCondition.Clone();
        }

        /// <inheritdoc/>
        protected override RowCondition CloneCore() => new CheckAllValueRowCondition(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.ValueCondition, "条件", () => ValueCondition, x => ValueCondition = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            StatusHelper.VerifyValueCondition(Title, status, Arguments[0], ValueCondition);
        }

        /// <inheritdoc/>
        public override MatchResult Match(IList<string> target)
        {
            foreach (string current in target)
            {
                MatchResult match = ValueCondition.Match(current);
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
        /// <summary>
        /// 値に適用する条件を取得または設定します。
        /// </summary>
        public ValueCondition ValueCondition { get; set; }

        /// <inheritdoc/>
        public override string? Title => "行内のいずれかのセルが条件を満たす";

        /// <summary>
        /// <see cref="CheckAnyValueRowCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        public CheckAnyValueRowCondition()
        {
            ValueCondition = ValueCondition.Null;
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
            ValueCondition = cloned.ValueCondition.Clone();
        }

        /// <inheritdoc/>
        protected override RowCondition CloneCore() => new CheckAnyValueRowCondition(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.ValueCondition, "条件", () => ValueCondition, x => ValueCondition = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            StatusHelper.VerifyValueCondition(Title, status, Arguments[0], ValueCondition);
        }

        /// <inheritdoc/>
        public override MatchResult Match(IList<string> target)
        {
            foreach (string current in target)
            {
                MatchResult match = ValueCondition.Match(current);
                if (match != MatchResult.NotMatched) return match;
            }
            return MatchResult.NotMatched;
        }
    }
}
