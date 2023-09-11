using TextProcessor.Logics.Data;
using TextProcessor.Logics.Operations;
using TextProcessor.Logics.Operations.OperationImpl;

namespace Test
{
    public partial class OperationTest
    {
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
    }
}
