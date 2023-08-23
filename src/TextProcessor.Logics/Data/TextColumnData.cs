using System;
using System.Collections;
using System.Collections.Generic;

namespace TextProcessor.Logics.Data
{
    /// <summary>
    /// <see cref="TextData"/>における列のデータを表します。
    /// </summary>
    [Serializable]
    public class TextColumnData : IEnumerable<string>
    {
        private readonly TextData data;
        private readonly int version;

        /// <summary>
        /// データを取得します。
        /// </summary>
        /// <exception cref="InvalidOperationException">ソースが変更された</exception>
        private List<List<string>> SourceList
        {
            get
            {
                try
                {
                    return data.GetSourceData(version);
                }
                catch (ArgumentException)
                {
                    throw new InvalidOperationException("ソースが変更されました");
                }
            }
        }

        /// <summary>
        /// 列インデックスを取得します。
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// <see cref="TextColumnData"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="data">参照するデータ</param>
        /// <param name="index">列インデックス</param>
        /// <param name="version">インスタンス生成時のバージョン</param>
        internal TextColumnData(TextData data, int index, int version)
        {
            this.data = data;
            Index = index;
            this.version = version;
        }

        /// <inheritdoc/>
        public IEnumerator<string> GetEnumerator()
        {
            for (int i = data.HasHeader ? 1 : 0; i < SourceList.Count; i++)
            {
                List<string> row = SourceList[i];
                yield return Index < row.Count ? row[Index] : string.Empty;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
