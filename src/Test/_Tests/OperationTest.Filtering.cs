using TextProcessor.Logics.Data;
using TextProcessor.Logics.Operations;
using TextProcessor.Logics.Operations.Conditions;
using TextProcessor.Logics.Operations.OperationImpl;

namespace Test
{
    public partial class OperationTest
    {
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
    }
}
