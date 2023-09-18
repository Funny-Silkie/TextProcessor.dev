using System.Collections.Generic;
using TextProcessor.Logics.Data;
using TextProcessor.Logics.Operations;
using TextProcessor.Logics.Operations.OperationImpl;

namespace Test
{
    public partial class OperationTest
    {
        /// <summary>
        /// <see cref="SortOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void Sort1()
        {
            var operation = new SortOperation()
            {
                Entries = new List<SortOperation.SortEntry>
                {
                    new SortOperation.SortEntry()
                    {
                        KeyIndex = 0,
                    },
                }
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
                Entries = new List<SortOperation.SortEntry>
                {
                    new SortOperation.SortEntry()
                    {
                        KeyIndex = 0,
                        AsNumber = true,
                        AsReversed = true,
                    }
                },
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
                Entries = new List<SortOperation.SortEntry>()
                {
                    new SortOperation.SortEntry()
                    {
                        KeyIndex = 2,
                        CaseSensitive = false,
                    },
                },
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
        public void Sort4()
        {
            var operation = new SortOperation()
            {
                Entries = new List<SortOperation.SortEntry>()
                {
                    new SortOperation.SortEntry()
                    {
                        KeyIndex = 1,
                        AsNumber = true,
                        AsReversed = true,
                    },
                    new SortOperation.SortEntry()
                    {
                        KeyIndex = 2,
                    },
                },
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
        public void Sort5()
        {
            var operation = new SortOperation()
            {
                Entries = new List<SortOperation.SortEntry>(),
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }
    }
}
