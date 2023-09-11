using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        [MemberNotNullWhen(true, nameof(Header))]
        public bool HasHeader { get; set; }

        /// <summary>
        /// 行数を取得します。
        /// </summary>
        public int RowCount => items.Count;

        /// <summary>
        /// 列数を取得します。
        /// </summary>
        public int ColumnCount => items.Count == 0 ? 0 : items.Max(x => x.Count);

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
        /// ファイルを読み込んで<see cref="TextData"/>のインスタンスを生成します。
        /// </summary>
        /// <param name="fileName">読み込むファイルパス</param>
        /// <param name="options">読み込み時のオプション</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="fileName"/>または<paramref name="options"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentException"><paramref name="fileName"/>が無効</exception>
        /// <exception cref="DirectoryNotFoundException"><paramref name="fileName"/>のディレクトリが存在しない</exception>
        /// <exception cref="FileNotFoundException"><paramref name="fileName"/>が存在しない</exception>
        /// <exception cref="System.Security.SecurityException">アクセス権限がない</exception>
        /// <exception cref="IOException">I/Oエラーが発生した</exception>
        /// <exception cref="OutOfMemoryException">メモリ不足</exception>
        /// <exception cref="System.Text.RegularExpressions.RegexMatchTimeoutException">正規表現検索時にタイムアウトが発生</exception>
        public static TextData Create(string fileName, TextLoadOptions options)
        {
            using var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Create(stream, options);
        }

        /// <inheritdoc cref="Create(Stream, TextLoadOptions)"/>
        public static async Task<TextData> CreateAsync(string fileName, TextLoadOptions options)
        {
            using var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return await CreateAsync(stream, options);
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
            int capacity = 0;
            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                List<string> values = logic.Split(line, options.Separator, capacity);
                capacity = Math.Max(capacity, values.Capacity);
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
            int capacity = 0;
            string? line;
            while ((line = await reader.ReadLineAsync()) is not null)
            {
                List<string> values = logic.Split(line, options.Separator, capacity);
                capacity = Math.Max(capacity, values.Capacity);
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
            for (int i = 0; i < ColumnCount; i++) yield return new TextColumnData(this, i, version);
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
