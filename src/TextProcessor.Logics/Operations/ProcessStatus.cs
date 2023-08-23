using System;
using System.Collections.Generic;

namespace TextProcessor.Logics.Operations
{
    /// <summary>
    /// 処理ステータスを表します。
    /// </summary>
    [Serializable]
    public class ProcessStatus
    {
        /// <summary>
        /// メッセージ一覧を取得します。
        /// </summary>
        public IList<StatusEntry> Messages { get; }

        /// <summary>
        /// 警告一覧を取得します。
        /// </summary>
        public IList<StatusEntry> Warnings { get; }

        /// <summary>
        /// エラー一覧を取得します。
        /// </summary>
        public IList<StatusEntry> Errors { get; }

        /// <summary>
        /// プロセスに問題がないかどうかを表す値を取得します。
        /// </summary>
        public bool Success => Errors.Count == 0;

        /// <summary>
        /// <see cref="ProcessStatus"/>の新しいインスタンスを初期化します。
        /// </summary>
        public ProcessStatus()
        {
            Messages = new List<StatusEntry>();
            Warnings = new List<StatusEntry>();
            Errors = new List<StatusEntry>();
        }
    }
}
