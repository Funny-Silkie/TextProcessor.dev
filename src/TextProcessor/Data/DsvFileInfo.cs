using System.Collections.Generic;
using TextProcessor.Logics.Data;
using TextProcessor.Logics.Operations;

namespace TextProcessor.Data
{
    /// <summary>
    /// DSVファイルの内容を表します。
    /// </summary>
    public class DsvFileInfo
    {
        /// <summary>
        /// ファイル名を取得します。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// テーブルデータを取得します。
        /// </summary>
        public TextData Data { get; }

        /// <summary>
        /// 処理一覧を取得します。
        /// </summary>
        public List<Operation> Operations { get; } = new List<Operation>();

        /// <summary>
        /// <see cref="DsvFileInfo"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="name">ファイル名</param>
        /// <param name="data">テーブルデータ</param>
        public DsvFileInfo(string name, TextData data)
        {
            Name = name;
            Data = data;
        }
    }
}
