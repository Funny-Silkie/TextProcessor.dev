using System;
using System.Collections.Generic;

namespace TextProcessor.Logics.Operations.Conditions
{
    /// <summary>
    /// NOT条件を表します。
    /// </summary>
    [Serializable]
    internal sealed class NotValueCondition : ValueCondition
    {
        /// <summary>
        /// 反転する条件を取得または設定します。
        /// </summary>
        public ValueCondition Condition { get; set; }

        /// <inheritdoc/>
        public override string? Title => "NOT";

        /// <summary>
        /// <see cref="NotValueCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        public NotValueCondition()
        {
            Condition = Null;
        }

        /// <summary>
        /// <see cref="NotValueCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        private NotValueCondition(NotValueCondition cloned)
            : base(cloned)
        {
            Condition = cloned.Condition.Clone();
        }

        /// <inheritdoc/>
        protected override ValueCondition CloneCore() => new NotValueCondition(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.ValueCondition, "条件", () => Condition, x => Condition = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            StatusHelper.VerifyValueCondition(Title, status, Arguments[0], Condition);
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
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
    internal sealed class AndValueCondition : ValueCondition
    {
        /// <summary>
        /// 対象の条件一覧を取得または設定します。
        /// </summary>
        public List<ValueCondition> Conditions { get; set; }

        /// <inheritdoc/>
        public override string? Title => "AND";

        /// <summary>
        /// <see cref="AndValueCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        public AndValueCondition()
        {
            Conditions = new List<ValueCondition> { Null, Null };
        }

        /// <summary>
        /// <see cref="AndValueCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        private AndValueCondition(AndValueCondition cloned)
            : base(cloned)
        {
            Conditions = cloned.Conditions.CloneAll();
        }

        /// <inheritdoc/>
        protected override ValueCondition CloneCore() => new AndValueCondition(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.ValueCondition | ArgumentType.List, "条件", () => Conditions, x => Conditions = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            for (int i = 0; i < Conditions.Count; i++)
            {
                StatusHelper.VerifyValueCondition(Title, status, Arguments[0], Conditions[i]);
            }
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            foreach (ValueCondition currentCondition in Conditions)
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
    internal sealed class OrValueCondition : ValueCondition
    {
        /// <summary>
        /// 対象の条件一覧を取得または設定します。
        /// </summary>
        public List<ValueCondition> Conditions { get; set; }

        /// <inheritdoc/>
        public override string? Title => "OR";

        /// <summary>
        /// <see cref="OrValueCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        public OrValueCondition()
        {
            Conditions = new List<ValueCondition> { Null, Null };
        }

        /// <summary>
        /// <see cref="OrValueCondition"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        private OrValueCondition(OrValueCondition cloned)
            : base(cloned)
        {
            Conditions = cloned.Conditions.CloneAll();
        }

        /// <inheritdoc/>
        protected override ValueCondition CloneCore() => new OrValueCondition(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.ValueCondition | ArgumentType.List, "条件", () => Conditions, x => Conditions = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            for (int i = 0; i < Conditions.Count; i++)
            {
                StatusHelper.VerifyValueCondition(Title, status, Arguments[0], Conditions[i]);
            }
        }

        /// <inheritdoc/>
        public override MatchResult Match(string target)
        {
            foreach (ValueCondition currentCondition in Conditions)
            {
                MatchResult currentResult = currentCondition.Match(target);
                if (currentResult != MatchResult.NotMatched) return currentResult;
            }

            return MatchResult.NotMatched;
        }
    }

    /// <summary>
    /// 空文字かどうかを検証する条件のクラスです。
    /// </summary>
    [Serializable]
    internal sealed class IsEmptyValueCondition : ValueCondition
    {
        /// <inheritdoc/>
        public override string? Title => "空欄である";

        /// <inheritdoc/>
        public override MatchResult Match(string target) => string.IsNullOrEmpty(target) ? MatchResult.Matched : MatchResult.NotMatched;
    }

    /// <summary>
    /// 整数かどうかを検証します。
    /// </summary>
    [Serializable]
    internal sealed class IsIntegerValueCondition : ValueCondition
    {
        /// <inheritdoc/>
        public override string? Title => "整数である";

        /// <inheritdoc/>
        public override MatchResult Match(string target) => long.TryParse(target, out _) ? MatchResult.Matched : MatchResult.NotMatched;
    }

    /// <summary>
    /// 小数かどうかを検証します。
    /// </summary>
    [Serializable]
    internal sealed class IsDecimalValueCondition : ValueCondition
    {
        /// <inheritdoc/>
        public override string? Title => "数値である";

        /// <inheritdoc/>
        public override MatchResult Match(string target) => double.TryParse(target, out _) ? MatchResult.Matched : MatchResult.NotMatched;
    }
}
