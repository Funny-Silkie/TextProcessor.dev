using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TextProcessor.Logics.Data;
using TextProcessor.Logics.Data.Options;
using TextProcessor.Logics.Operations;

namespace Test
{
    /// <summary>
    /// <see cref="Operation"/>のテストを行います。
    /// </summary>
    [TestFixture]
    public partial class OperationTest
    {
        private TextData PersonTable;
        private TextData PrefectureTable;
        private TextData RegionTable;

        /// <summary>
        /// 初期化を行います。
        /// </summary>
        [SetUp]
        public void Initialize()
        {
            PersonTable = Util.LoadFile(SR.Source_PersonalTable);
            PrefectureTable = Util.LoadFile(SR.Source_PrefectureTable);
            RegionTable = Util.LoadFile(SR.Source_RegionTable);
        }

        /// <summary>
        /// <see cref="TextData"/>同士の等価性をチェックします。
        /// </summary>
        /// <param name="x">検証する一つ目のインスタンス</param>
        /// <param name="y">検証する二つ目のインスタンス</param>
        private static void CheckTextDataEquality(TextData x, TextData y)
        {
            if (x == y) return;

            List<List<string>> xList = x.GetSourceData();
            List<List<string>> yList = y.GetSourceData();

            Assert.That(yList, Has.Count.EqualTo(xList.Count));
            for (int rowIndex = 0; rowIndex < xList.Count; rowIndex++)
            {
                Span<string> xRow = Trim(CollectionsMarshal.AsSpan(xList[rowIndex]));
                Span<string> yRow = Trim(CollectionsMarshal.AsSpan(yList[rowIndex]));

                Assert.That(xRow.Length, Is.EqualTo(yRow.Length));
                for (int columnIndex = 0; columnIndex < xRow.Length; columnIndex++) Assert.That(yRow[columnIndex], Is.EqualTo(xRow[columnIndex]));
            }

            // 末尾の空文字部分を無視
            static Span<string> Trim(Span<string> row)
            {
                if (row.Length == 0 || !string.IsNullOrEmpty(row[^1])) return row;
                int blankCount = 0;
                for (int i = 1; i <= row.Length; i++)
                {
                    if (!string.IsNullOrEmpty(row[^i])) break;
                    blankCount++;
                }
                return row[..^blankCount];
            }
        }

        /// <summary>
        /// ヘッダー部分のみのデータを取得します。
        /// </summary>
        /// <param name="data">対象データ</param>
        /// <returns>ヘッダー部分のみのデータ</returns>
        private static TextData ExtractHeader(TextData data)
        {
            if (!data.HasHeader) return TextData.CreateFromRawData(Array.Empty<IEnumerable<string>>());
            return TextData.CreateFromRawData(new[] { data.Header });
        }

        /// <summary>
        /// 比較対象のデータを読み込みます。
        /// </summary>
        /// <param name="options">読み込み時のオプション</param>
        /// <param name="caller">呼び出し元の名前</param>
        /// <returns>比較対象のデータ</returns>
        private static TextData LoadResultData(TextLoadOptions? options = null, [CallerMemberName] string? caller = null)
        {
            return Util.LoadFile($"{SR.Result_Dir}Operation/{caller}.tsv", options);
        }
    }
}
