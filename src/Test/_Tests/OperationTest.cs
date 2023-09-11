using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TextProcessor.Logics.Data;
using TextProcessor.Logics.Data.Options;
using TextProcessor.Logics.Operations;
using TextProcessor.Logics.Operations.Conditions;
using TextProcessor.Logics.Operations.OperationImpl;

namespace Test
{
    /// <summary>
    /// <see cref="Operation"/>のテストを行います。
    /// </summary>
    [TestFixture]
    public class OperationTest
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

        /// <summary>
        /// <see cref="FilterRowOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void FilterRow1()
        {
            EqualValueCondition<long> valueCondition = ValueConditionFactory.EqualAsInteger();
            valueCondition.Comparison = 3;
            var rowCondition = new CheckValueRowCondition()
            {
                ColumnIndex = 1,
                ValueCondition = valueCondition,
            };
            var operation = new FilterRowOperation()
            {
                Condition = rowCondition,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="FilterRowOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void FilterRow2()
        {
            EqualValueCondition<long> valueCondition = ValueConditionFactory.EqualAsInteger();
            valueCondition.Comparison = 3;
            var rowCondition = new CheckValueRowCondition()
            {
                ColumnIndex = 1,
                ValueCondition = ValueCondition.Null,
            };
            var operation = new FilterRowOperation()
            {
                Condition = rowCondition,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="HeadOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Head1()
        {
            var operation = new HeadOperation()
            {
                Count = 2,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="HeadOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Head2()
        {
            var operation = new HeadOperation()
            {
                Count = 0,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            TextData comparison = ExtractHeader(data);
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, comparison);
        }

        /// <summary>
        /// <see cref="HeadOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Head3()
        {
            var operation = new HeadOperation()
            {
                Count = 100,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            TextData comparison = data.Clone();
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, comparison);
        }

        /// <summary>
        /// <see cref="HeadOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Head4()
        {
            var operation = new HeadOperation()
            {
                Count = -1,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="TailOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Tail1()
        {
            var operation = new TailOperation()
            {
                Count = 2,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="TailOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Tail2()
        {
            var operation = new TailOperation()
            {
                Count = 0,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            TextData comparison = ExtractHeader(data);
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, comparison);
        }

        /// <summary>
        /// <see cref="TailOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Tail3()
        {
            var operation = new TailOperation()
            {
                Count = 100,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            TextData comparison = data.Clone();
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, comparison);
        }

        /// <summary>
        /// <see cref="TailOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Tail4()
        {
            var operation = new TailOperation()
            {
                Count = -1,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="SkipHeadOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void SkipHead1()
        {
            var operation = new SkipHeadOperation()
            {
                Count = 2,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="SkipHeadOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void SkipHead2()
        {
            var operation = new SkipHeadOperation()
            {
                Count = 0,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            TextData comparison = data.Clone();
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, comparison);
        }

        /// <summary>
        /// <see cref="SkipHeadOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void SkipHead3()
        {
            var operation = new SkipHeadOperation()
            {
                Count = 100,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            TextData comparison = ExtractHeader(data);
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, comparison);
        }

        /// <summary>
        /// <see cref="SkipHeadOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void SkipHead4()
        {
            var operation = new SkipHeadOperation()
            {
                Count = -1,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="SkipTailOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void SkipTail1()
        {
            var operation = new SkipTailOperation()
            {
                Count = 2,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="SkipTailOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void SkipTail2()
        {
            var operation = new SkipTailOperation()
            {
                Count = 0,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            TextData comparison = data.Clone();
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, comparison);
        }

        /// <summary>
        /// <see cref="SkipTailOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void SkipTail3()
        {
            var operation = new SkipTailOperation()
            {
                Count = 100,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            TextData comparison = ExtractHeader(data);
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, comparison);
        }

        /// <summary>
        /// <see cref="SkipTailOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void SkipTail4()
        {
            var operation = new SkipTailOperation()
            {
                Count = -1,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="SelectColumnOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void SelectColumn1()
        {
            var operation = new SelectColumnOperation()
            {
                ColumnIndex = 2,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="SelectColumnOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void SelectColumn2()
        {
            var operation = new SelectColumnOperation()
            {
                ColumnIndex = 100,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Warnings, Has.Count.GreaterThanOrEqualTo(1));
        }

        /// <summary>
        /// <see cref="ReplaceOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Replace1()
        {
            var operation = new ReplaceOperation()
            {
                QueryText = "o",
                ReplacerText = "0",
                TargetColumnIndex = 2,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="ReplaceOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Replace2()
        {
            var operation = new ReplaceOperation()
            {
                QueryText = "o",
                ReplacerText = "0",
                IsAll = true,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="ReplaceOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Replace3()
        {
            var operation = new ReplaceOperation()
            {
                QueryText = "o",
                ReplacerText = "0",
                TargetColumnIndex = 2,
                CaseSensitive = false,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="ReplaceOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Replace4()
        {
            var operation = new ReplaceOperation()
            {
                QueryText = @"\d",
                ReplacerText = @"\d",
                TargetColumnIndex = 0,
                UseRegex = true,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="ReplaceOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Replace5()
        {
            var operation = new ReplaceOperation()
            {
                QueryText = "",
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="SortOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Sort1()
        {
            var operation = new SortOperation()
            {
                KeyIndex = 0,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="SortOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Sort2()
        {
            var operation = new SortOperation()
            {
                KeyIndex = 0,
                AsNumber = true,
                AsReversed = true,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="SortOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Sort3()
        {
            var operation = new SortOperation()
            {
                KeyIndex = 2,
                CaseSensitive = false,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="DistinctOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Distinct1()
        {
            var operation = new DistinctOperation()
            {
                KeyIndex = 0,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PersonTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="DistinctOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Distinct2()
        {
            var operation = new DistinctOperation()
            {
                KeyIndex = 2,
                CaseSensitive = false,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PersonTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="DistinctOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Distinct3()
        {
            var operation = new DistinctOperation()
            {
                IsAll = true,
                CaseSensitive = false,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PersonTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="AppendOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Append1()
        {
            var operation = new AppendOperation()
            {
                Appended = RegionTable.Clone(),
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="AppendOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Append2()
        {
            var operation = new AppendOperation()
            {
                Appended = null,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="PrependOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Prepend1()
        {
            var operation = new PrependOperation()
            {
                Prepended = RegionTable.Clone(),
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="PrependOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Prepend2()
        {
            var operation = new PrependOperation()
            {
                Prepended = null,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="PasteOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Paste1()
        {
            var operation = new PasteOperation()
            {
                Target = RegionTable.Clone(),
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="PasteOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Paste2()
        {
            var operation = new PasteOperation()
            {
                Target = null,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="InnerJoinOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void InnerJoin1()
        {
            TextData joined = RegionTable.Clone();
            var operation = new InnerJoinOperation()
            {
                KeyIndex = 1,
                Target = joined,
                TargetKeyIndex = 0,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="InnerJoinOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void InnerJoin2()
        {
            var operation = new InnerJoinOperation()
            {
                Target = null,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="InnerJoinOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void InnerJoin3()
        {
            TextData joined = RegionTable.Clone();
            var operation = new InnerJoinOperation()
            {
                KeyIndex = 1,
                Target = joined,
                TargetKeyIndex = 0,
                RemoveKey = true,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="InnerJoinOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void InnerJoin4()
        {
            TextData joined = RegionTable.Clone();
            new HeadOperation()
            {
                Count = 4,
            }.Operate(joined);
            var operation = new InnerJoinOperation()
            {
                KeyIndex = 1,
                Target = joined,
                TargetKeyIndex = 0,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            new FilterRowOperation()
            {
                Condition = new CheckValueRowCondition()
                {
                    ColumnIndex = 2,
                    ValueCondition = new RegexMatchValueCondition()
                    {
                        Pattern = "[A-I].+",
                    },
                },
            }.Operate(data);
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="InnerJoinOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void InnerJoin5()
        {
            TextData joined = PrefectureTable.Clone();
            var operation = new InnerJoinOperation()
            {
                KeyIndex = 2,
                Target = joined,
                TargetKeyIndex = 3,
                CaseSensitive = false,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PersonTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="LeftOuterJoinOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void LeftOuterJoin1()
        {
            TextData joined = RegionTable.Clone();
            var operation = new LeftOuterJoinOperation()
            {
                KeyIndex = 1,
                Target = joined,
                TargetKeyIndex = 0,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="LeftOuterJoinOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void LeftOuterJoin2()
        {
            var operation = new LeftOuterJoinOperation()
            {
                Target = null,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="LeftOuterJoinOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void LeftOuterJoin3()
        {
            TextData joined = RegionTable.Clone();
            var operation = new LeftOuterJoinOperation()
            {
                KeyIndex = 1,
                Target = joined,
                TargetKeyIndex = 0,
                RemoveKey = true,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="LeftOuterJoinOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void LeftOuterJoin4()
        {
            TextData joined = RegionTable.Clone();
            new HeadOperation()
            {
                Count = 4,
            }.Operate(joined);
            var operation = new LeftOuterJoinOperation()
            {
                KeyIndex = 1,
                Target = joined,
                TargetKeyIndex = 0,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            new FilterRowOperation()
            {
                Condition = new CheckValueRowCondition()
                {
                    ColumnIndex = 2,
                    ValueCondition = new RegexMatchValueCondition()
                    {
                        Pattern = "[A-I].+",
                    },
                },
            }.Operate(data);
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="LeftOuterJoinOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void LeftOuterJoin5()
        {
            TextData joined = PrefectureTable.Clone();
            var operation = new LeftOuterJoinOperation()
            {
                KeyIndex = 2,
                Target = joined,
                TargetKeyIndex = 3,
                CaseSensitive = false,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PersonTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="FullOuterJoinOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void FullOuterJoin1()
        {
            TextData joined = RegionTable.Clone();
            var operation = new FullOuterJoinOperation()
            {
                KeyIndex = 1,
                Target = joined,
                TargetKeyIndex = 0,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="FullOuterJoinOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void FullOuterJoin2()
        {
            var operation = new FullOuterJoinOperation()
            {
                Target = null,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="FullOuterJoinOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void FullOuterJoin3()
        {
            TextData joined = RegionTable.Clone();
            var operation = new FullOuterJoinOperation()
            {
                KeyIndex = 1,
                Target = joined,
                TargetKeyIndex = 0,
                RemoveKey = true,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="FullOuterJoinOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void FullOuterJoin4()
        {
            TextData joined = RegionTable.Clone();
            new HeadOperation()
            {
                Count = 4,
            }.Operate(joined);
            var operation = new FullOuterJoinOperation()
            {
                KeyIndex = 1,
                Target = joined,
                TargetKeyIndex = 0,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            new FilterRowOperation()
            {
                Condition = new CheckValueRowCondition()
                {
                    ColumnIndex = 2,
                    ValueCondition = new RegexMatchValueCondition()
                    {
                        Pattern = "[A-I].+",
                    },
                },
            }.Operate(data);
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="FullOuterJoinOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void FullOuterJoin5()
        {
            TextData joined = PrefectureTable.Clone();
            var operation = new FullOuterJoinOperation()
            {
                KeyIndex = 2,
                Target = joined,
                TargetKeyIndex = 3,
                CaseSensitive = false,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PersonTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="DeleteColumnOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void DeleteColumn1()
        {
            var operation = new DeleteColumnOperation()
            {
                Index = 1,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="DeleteColumnOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void DeleteColumn2()
        {
            var operation = new DeleteColumnOperation()
            {
                Index = 100,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            TextData comparison = PrefectureTable.Clone();
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, comparison);
        }

        /// <summary>
        /// <see cref="DeleteRowOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void DeleteRow1()
        {
            var operation = new DeleteRowOperation()
            {
                Index = 1,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="DeleteRowOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void DeleteRow2()
        {
            var operation = new DeleteRowOperation()
            {
                Index = 100,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            TextData comparison = PrefectureTable.Clone();
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, comparison);
        }
    }
}
