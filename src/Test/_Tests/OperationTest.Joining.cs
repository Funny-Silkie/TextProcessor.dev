using TextProcessor.Logics.Data;
using TextProcessor.Logics.Operations;
using TextProcessor.Logics.Operations.Conditions;
using TextProcessor.Logics.Operations.OperationImpl;

namespace Test
{
    public partial class OperationTest
    {
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
    }
}
