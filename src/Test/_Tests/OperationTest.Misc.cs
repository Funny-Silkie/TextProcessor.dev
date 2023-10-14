using TextProcessor.Logics.Data;
using TextProcessor.Logics.Operations;
using TextProcessor.Logics.Operations.OperationImpl;

namespace Test
{
    public partial class OperationTest
    {
        /// <summary>
        /// <see cref="ChangeHeaderOperation"/>のテストを行います。
        /// </summary>
        [Test]
        public void ChangeHeader()
        {
            var operation = new ChangeHeaderOperation()
            {
                HasHeader = false,
            };

            ProcessStatus argResult = operation.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            TextData data = PrefectureTable;
            ProcessStatus opResult = operation.Operate(data);

            Assert.That(opResult.Success, Is.True);
            Assert.That(data.HasHeader, Is.False);
        }
    }
}
