using TextProcessor.Logics.Operations.Conditions;
using TextProcessor.Logics.Operations.Conversions;

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

            ProcessStatus verificationResult = target.VerifyArguments();
            MergeAsChild(parentName, status, verificationResult);
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

            ProcessStatus verificationResult = target.VerifyArguments();
            MergeAsChild(parentName, status, verificationResult);
        }

        /// <summary>
        /// <see cref="ValueConversion"/>をチェックします。
        /// </summary>
        /// <param name="parentName">親プロセス名</param>
        /// <param name="status">追加先</param>
        /// <param name="arg">引数</param>
        /// <param name="target">検証対象</param>
        public static void VerifyValueConversion(string? parentName, ProcessStatus status, ArgumentInfo arg, ValueConversion target)
        {
            ProcessStatus verificationResult = target.VerifyArguments();
            MergeAsChild(parentName, status, verificationResult);
        }

        /// <summary>
        /// 子の結果を結合します。
        /// </summary>
        /// <param name="parentName">親プロセス名</param>
        /// <param name="status">追加先</param>
        /// <param name="childStatus">子プロセスの結果</param>
        public static void MergeAsChild(string? parentName, ProcessStatus status, ProcessStatus childStatus)
        {
            foreach (StatusEntry entry in childStatus.Errors) status.Errors.Add(entry.CreateAsChildren(parentName));
            foreach (StatusEntry entry in childStatus.Warnings) status.Warnings.Add(entry.CreateAsChildren(parentName));
            foreach (StatusEntry entry in childStatus.Messages) status.Messages.Add(entry.CreateAsChildren(parentName));
        }
    }
}
