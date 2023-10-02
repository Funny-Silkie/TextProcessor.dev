using System;
using System.Linq;
using TextProcessor.Logics.Data;

namespace Test
{
    /// <summary>
    /// <see cref="ValueRange"/>のテストを行います。
    /// </summary>
    [TestFixture]
    public class ValueRangeTest
    {
        /// <summary>
        /// 文字列からの変換のテストを行います。
        /// </summary>
        [Test]
        public void Parse()
        {
            Assert.That(ValueRangeEntry.Parse("0-3"), Is.EqualTo(ValueRangeEntry.Between(0, 3)));
            Assert.That(ValueRangeEntry.Parse("4-2"), Is.EqualTo(ValueRangeEntry.Between(2, 4)));
            Assert.That(ValueRangeEntry.Parse("1"), Is.EqualTo(new ValueRangeEntry(1)));
            Assert.Catch<FormatException>(() => ValueRangeEntry.Parse("-"));
            Assert.Catch<FormatException>(() => ValueRangeEntry.Parse("abc"));
            Assert.Catch<OverflowException>(() => ValueRangeEntry.Parse(ulong.MaxValue.ToString()));

            Assert.That(ValueRange.Parse("1").SequenceEqual(new[] { 1 }), Is.True);
            Assert.That(ValueRange.Parse("1-3").SequenceEqual(new[] { 1, 2, 3 }), Is.True);
            Assert.That(ValueRange.Parse("1-3,5,10").SequenceEqual(new[] { 1, 2, 3, 5, 10 }), Is.True);
            Assert.That(ValueRange.Parse("1-3 , 5-10").SequenceEqual(new[] { 1, 2, 3, 5, 6, 7, 8, 9, 10 }), Is.True);
            Assert.Catch<FormatException>(() => ValueRange.Parse("-"));
            Assert.Catch<FormatException>(() => ValueRange.Parse("abc"));
            Assert.Catch<FormatException>(() => ValueRange.Parse("1-3,abc"));
            Assert.Catch<FormatException>(() => ValueRange.Parse("1-3,5-10,"));
        }
    }
}
