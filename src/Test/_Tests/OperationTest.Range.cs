using TextProcessor.Logics.Data;
using TextProcessor.Logics.Operations;
using TextProcessor.Logics.Operations.OperationImpl;

namespace Test
{
    public partial class OperationTest
    {
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
                Columns = 2,
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
                Columns = default,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Warnings, Has.Count.GreaterThanOrEqualTo(1));
        }

        /// <summary>
        /// <see cref="SelectColumnOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void SelectColumn3()
        {
            var operation = new SelectColumnOperation()
            {
                Columns = new ValueRange(new[] { 0, 2, 0 }),
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
        public void DeleteColumn1()
        {
            var operation = new DeleteColumnOperation()
            {
                Columns = 1,
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
                Columns = 100,
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
        /// <see cref="DeleteColumnOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void DeleteColumn3()
        {
            var operation = new DeleteColumnOperation()
            {
                Columns = ValueRange.Parse("0-1,3"),
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
        public void DeleteRow1()
        {
            var operation = new DeleteRowOperation()
            {
                Rows = 1,
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
                Rows = 100,
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
        public void DeleteRow3()
        {
            var operation = new DeleteRowOperation()
            {
                Rows = ValueRange.Parse("10-19,30-39"),
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }
    }
}
