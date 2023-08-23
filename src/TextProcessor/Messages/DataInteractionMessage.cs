using System;
using System.Collections.Generic;

namespace TextProcessor.Messages
{
    /// <summary>
    /// データのやり取りに用いられるメッセージを表します。
    /// </summary>
    public class DataInteractionMessage
    {
        /// <summary>
        /// 授受するデータを取得します。
        /// </summary>
        public Dictionary<string, object?> Data { get; }

        /// <summary>
        /// <see cref="DataInteractionMessage"/>の新しいインスタンスを初期化します。
        /// </summary>
        public DataInteractionMessage()
        {
            Data = new Dictionary<string, object?>();
        }

        /// <summary>
        /// <see cref="DataInteractionMessage"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="data">使用するデータ</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/>がnull</exception>
        public DataInteractionMessage(IDictionary<string, object?> data)
        {
            Data = new Dictionary<string, object?>(data);
        }
    }
}
