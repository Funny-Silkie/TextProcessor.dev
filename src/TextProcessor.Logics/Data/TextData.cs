using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TextProcessor.Logics.Data.Options;

namespace TextProcessor.Logics.Data
{
    /// <summary>
    /// テキストファイルのデータを表します。
    /// </summary>
    [Serializable]
    public class TextData : ICloneable
    {
        private readonly List<List<string>> items;
        private int version;

        /// <summary>
        /// 先頭行をヘッダーとするかどうかを表す値を取得または設定します。
        /// </summary>
        public bool HasHeader { get; set; }

        /// <summary>
        /// ヘッダー行を取得します。
        /// </summary>
        public TextRowData? Header => HasHeader ? new TextRowData(this, 0, version) : null;

        /// <summary>
        /// <see cref="TextData"/>の新しいインスタンスを初期化します。
        /// </summary>
        public TextData()
        {
            items = new List<List<string>>();
        }

        /// <summary>
        /// <see cref="TextData"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="items"><see cref="items"/></param>
        /// <remarks>クローン用コンストラクタ</remarks>
        private TextData(List<List<string>> items)
        {
            this.items = items;
        }

        /// <summary>
        /// ストリームオブジェクトを読み込んで<see cref="TextData"/>のインスタンスを生成します。
        /// </summary>
        /// <param name="stream">読み込むストリームオブジェクト</param>
        /// <param name="options">読み込み時のオプション</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/>または<paramref name="options"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/>が読み取りをサポートしない</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="stream"/>が既に破棄されている</exception>
        /// <exception cref="IOException">I/Oエラーが発生した</exception>
        /// <exception cref="OutOfMemoryException">メモリ不足</exception>
        /// <exception cref="System.Text.RegularExpressions.RegexMatchTimeoutException">正規表現検索時にタイムアウトが発生</exception>
        public static TextData Create(Stream stream, TextLoadOptions options)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(options);

            var result = new TextData()
            {
                HasHeader = options.HasHeader,
            };
            var logic = new DsvLogic();

            using var reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine()!;
                List<string> values = logic.Split(line, options.Separator);
                result.items.Add(values);
            }

            return result;
        }

        /// <inheritdoc cref="Create(Stream, TextLoadOptions)"/>
        public static async Task<TextData> CreateAsync(Stream stream, TextLoadOptions options)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(options);

            var result = new TextData()
            {
                HasHeader = options.HasHeader,
            };
            var logic = new DsvLogic();

            using var reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                string line = (await reader.ReadLineAsync())!;
                List<string> values = logic.Split(line, options.Separator);
                result.items.Add(values);
            }

            return result;
        }

        /// <summary>
        /// 生データから<see cref="TextData"/>のインスタンスを生成します。
        /// </summary>
        /// <param name="source">使用するデータ</param>
        /// <returns><paramref name="source"/>を基に作成された<see cref="TextData"/>の新しいインスタンス</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>が<see langword="null"/></exception>
        public static TextData CreateFromRawData(IEnumerable<IEnumerable<string>> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            return new TextData(source.Select(x => x.ToList()).ToList());
        }

        /// <inheritdoc cref="ICloneable.Clone"/>
        public TextData Clone()
        {
            var list = new List<List<string>>(items.Capacity);
            for (int rowIndex = 0; rowIndex < items.Count; rowIndex++)
            {
                List<string> row = items[rowIndex];
                list.Add(new List<string>(row));
            }
            return new TextData(list)
            {
                version = version,
                HasHeader = HasHeader,
            };
        }

        object ICloneable.Clone() => Clone();

        /// <summary>
        /// <see cref="items"/>を取得します。
        /// </summary>
        /// <param name="version">照合するバージョン</param>
        /// <returns><see cref="items"/>のインスタンス</returns>
        /// <exception cref="ArgumentException"><paramref name="version"/>が無効</exception>
        internal List<List<string>> GetSourceData(int version)
        {
            if (this.version != version) throw new ArgumentException("バージョンが異なります", nameof(version));
            return items;
        }

        /// <summary>
        /// <see cref="items"/>を取得します。
        /// </summary>
        /// <returns><see cref="items"/>のインスタンス</returns>
        internal List<List<string>> GetSourceData() => items;

        /// <summary>
        /// 全ての行を取得します。
        /// </summary>
        /// <returns>全ての行</returns>
        public IEnumerable<TextRowData> GetAllRows()
        {
            for (int i = 0; i < items.Count; i++) yield return new TextRowData(this, i, version);
        }

        /// <summary>
        /// データ行を取得します。
        /// </summary>
        /// <returns>データ行一覧</returns>
        public IEnumerable<TextRowData> GetDataRows()
        {
            for (int i = HasHeader ? 1 : 0; i < items.Count; i++) yield return new TextRowData(this, i, version);
        }

        /// <summary>
        /// 全ての列を取得します。
        /// </summary>
        /// <returns>全ての列</returns>
        public IEnumerable<TextColumnData> GetColumns()
        {
            for (int i = 0; i < items.Max(x => x.Count); i++) yield return new TextColumnData(this, i, version);
        }

        /// <summary>
        /// ファイルを出力します。
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>または<paramref name="options"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="writer"/>が空文字</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        /// <exception cref="IOException">I/Oエラーが発生した</exception>
        public void WriteTo(TextWriter writer, TextSaveOptions options)
        {
            ArgumentNullException.ThrowIfNull(writer);
            ArgumentNullException.ThrowIfNull(options);

            var logic = new DsvLogic();
            foreach (List<string> row in items) logic.WriteRow(writer, row, "\t");
        }

        /// <summary>
        /// ファイルを出力します。
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/>または<paramref name="options"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="writer"/>が空文字</exception>
        /// <exception cref="ObjectDisposedException"><paramref name="writer"/>が既に破棄されている</exception>
        /// <exception cref="IOException">I/Oエラーが発生した</exception>
        public async Task WriteToAsync(TextWriter writer, TextSaveOptions options)
        {
            ArgumentNullException.ThrowIfNull(writer);
            ArgumentNullException.ThrowIfNull(options);

            var logic = new DsvLogic();
            foreach (List<string> row in items) await logic.WriteRowAsync(writer, row, "\t");
        }
    }
}
