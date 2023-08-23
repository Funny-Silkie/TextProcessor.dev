using System;
using System.Collections;
using System.Collections.Generic;

namespace TextProcessor.Logics.Data
{
    /// <summary>
    /// <see cref="TextData"/>における行のデータを表します。
    /// </summary>
    [Serializable]
    public class TextRowData : IEnumerable<string>
    {
        private readonly TextData data;
        private readonly int version;

        /// <summary>
        /// データを取得します。
        /// </summary>
        /// <exception cref="InvalidOperationException">ソースが変更された</exception>
        private List<string> SourceList
        {
            get
            {
                try
                {
                    return data.GetSourceData(version)[Index];
                }
                catch (ArgumentException)
                {
                    throw new InvalidOperationException("ソースが変更されました");
                }
            }
        }

        /// <summary>
        /// 行インデックスを取得します。
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// <see cref="TextRowData"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="data">参照するデータ</param>
        /// <param name="index">行インデックス</param>
        /// <param name="version">インスタンス生成時のバージョン</param>
        internal TextRowData(TextData data, int index, int version)
        {
            this.data = data;
            Index = index;
            this.version = version;
        }

        /// <summary>
        /// 指定した列インデックスの値を取得します。
        /// </summary>
        /// <param name="index">列インデックス</param>
        /// <returns><paramref name="index"/>に対応する値。<paramref name="index"/>が範囲外の場合は<see langword="null"/></returns>
        public string? this[int index]
        {
            get
            {
                List<string> list = SourceList;
                if (index < 0 || list.Count <= index) return null;
                return list[index];
            }
        }

        /// <inheritdoc/>
        public IEnumerator<string> GetEnumerator() => SourceList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
