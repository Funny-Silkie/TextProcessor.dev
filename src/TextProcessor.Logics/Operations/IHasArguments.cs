using System.Collections.Generic;

namespace TextProcessor.Logics.Operations
{
    /// <summary>
    /// 引数の定義を提供するインターフェイスです。
    /// </summary>
    public interface IHasArguments
    {
        /// <summary>
        /// 引数一覧を取得します。
        /// </summary>
        IList<ArgumentInfo> Arguments { get; }

        /// <summary>
        /// 引数チェックを行います。
        /// </summary>
        /// <returns>チェック結果</returns>
        ProcessStatus VerifyArguments();
    }
}
