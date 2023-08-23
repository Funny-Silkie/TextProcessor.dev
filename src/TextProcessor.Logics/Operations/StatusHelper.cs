using TextProcessor.Logics.Operations.Conditions;

namespace TextProcessor.Logics.Operations
{
    /// <summary>
    /// 引数チェックのヘルパークラスです。
    /// </summary>
    internal static class StatusHelper
    {
        /// <summary>
        /// <see cref="ValueCondition"/>をチェックします。
        /// </summary>
        /// <param name="parentName">親プロセス名</param>
        /// <param name="status">追加先</param>
        /// <param name="arg">引数</param>
        /// <param name="target">検証対象</param>
        public static void VerifyValueCondition(string? parentName, ProcessStatus status, ArgumentInfo arg, ValueCondition target)
        {
            if (target.Equals(ValueCondition.Null)) status.Errors.Add(new StatusEntry(parentName, arg, "値の条件を設定してください"));
            foreach (StatusEntry entry in target.VerifyArguments().Errors) status.Errors.Add(entry.CreateAsChildren(parentName));
        }

        /// <summary>
        /// <see cref="RowCondition"/>をチェックします。
        /// </summary>
        /// <param name="parentName">親プロセス名</param>
        /// <param name="status">追加先</param>
        /// <param name="arg">引数</param>
        /// <param name="target">検証対象</param>
        public static void VerifyRowCondition(string? parentName, ProcessStatus status, ArgumentInfo arg, RowCondition target)
        {
            if (target.Equals(RowCondition.Null)) status.Errors.Add(new StatusEntry(parentName, arg, "行の条件を設定してください"));
            foreach (StatusEntry entry in target.VerifyArguments().Errors) status.Errors.Add(entry.CreateAsChildren(parentName));
        }
    }
}
