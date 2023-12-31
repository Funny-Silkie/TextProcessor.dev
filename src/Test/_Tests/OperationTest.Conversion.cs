﻿using TextProcessor.Logics.Data;
using TextProcessor.Logics.Operations;
using TextProcessor.Logics.Operations.Conversions;
using TextProcessor.Logics.Operations.OperationImpl;

namespace Test
{
    public partial class OperationTest
    {
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
        /// <see cref="GenerateColumnOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void GenerateColumn()
        {
            MultiplyValueConversion<long> conversion = ValueConversionFactory.MultiplyAsInteger();
            conversion.Target = 2;

            var operation = new GenerateColumnOperation()
            {
                Conversion = conversion,
                HeaderName = "New",
                InsertIndex = 3,
                SourceIndex = 0,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            CheckTextDataEquality(data, LoadResultData());
        }

        /// <summary>
        /// <see cref="EditColumnOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void EditColumn()
        {
            MultiplyValueConversion<long> conversion = ValueConversionFactory.MultiplyAsInteger();
            conversion.Target = 2;

            var operation = new EditColumnOperation()
            {
                Conversion = conversion,
                SourceIndex = 0,
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
