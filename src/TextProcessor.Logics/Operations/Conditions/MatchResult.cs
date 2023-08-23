using System;

namespace TextProcessor.Logics.Operations.Conditions
{
    /// <summary>
    /// 条件の検証結果を表します。
    /// </summary>
    [Serializable]
    [Flags]
    public enum MatchResult
    {
        /// <summary>
        /// 条件に適合
        /// </summary>
        Matched = 0,

        /// <summary>
        /// 条件に不適合
        /// </summary>
        NotMatched = 1,

        /// <summary>
        /// 検証不可能
        /// </summary>
        Error = 2,
    }
}
