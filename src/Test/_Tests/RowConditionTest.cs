using System;
using TextProcessor.Logics.Operations;
using TextProcessor.Logics.Operations.Conditions;

namespace Test
{
    /// <summary>
    /// <see cref="RowCondition"/>のテストを行います。
    /// </summary>
    [TestFixture]
    public class RowConditionTest
    {
        /// <summary>
        /// <see cref="NotRowCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void Not1()
        {
            var condition = new NotRowCondition()
            {
                Condition = new CheckValueRowCondition()
                {
                    ColumnIndex = 1,
                    ValueCondition = new IsEmptyValueCondition(),
                },
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(Array.Empty<string>()), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match(new[] { "", "", "" }), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match(new[] { "", "HogeHoge", "123" }), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match(new[] { "HogeHoge", "123", "" }), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match(new[] { "123", "", "HogeHoge" }), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match(new[] { "+++", "HogeHoge", "123" }), Is.EqualTo(MatchResult.Matched));
            });
        }

        /// <summary>
        /// <see cref="NotRowCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void Not2()
        {
            var condition = new NotRowCondition()
            {
                Condition = RowCondition.Null,
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="OrRowCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void Or1()
        {
            var condition = new OrRowCondition()
            {
                Conditions = new RowCondition[]
                {
                    new CheckValueRowCondition()
                    {
                        ColumnIndex = 1,
                        ValueCondition = new IsEmptyValueCondition(),
                    },
                    new CheckAnyValueRowCondition()
                    {
                        ValueCondition = new MatchValueCondition()
                        {
                            Comparison = "+++",
                        },
                    },
                },
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(Array.Empty<string>()), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match(new[] { "", "", "" }), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match(new[] { "", "HogeHoge", "123" }), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match(new[] { "HogeHoge", "123", "" }), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match(new[] { "123", "", "HogeHoge" }), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match(new[] { "+++", "HogeHoge", "123" }), Is.EqualTo(MatchResult.Matched));
            });
        }

        /// <summary>
        /// <see cref="OrRowCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void Or2()
        {
            var condition = new OrRowCondition()
            {
                Conditions = new RowCondition[]
                {
                    new CheckValueRowCondition()
                    {
                        ColumnIndex = 1,
                        ValueCondition = new IsEmptyValueCondition(),
                    },
                    new CheckAnyValueRowCondition()
                    {
                        ValueCondition = new MatchValueCondition()
                        {
                            Comparison = "+++",
                        },
                    },
                    RowCondition.Null,
                },
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="AndRowCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void And1()
        {
            var condition = new AndRowCondition()
            {
                Conditions = new RowCondition[]
                {
                    new CheckValueRowCondition()
                    {
                        ColumnIndex = 1,
                        ValueCondition = new IsEmptyValueCondition(),
                    },
                    new CheckAnyValueRowCondition()
                    {
                        ValueCondition = new IsIntegerValueCondition(),
                    },
                },
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(Array.Empty<string>()), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match(new[] { "", "", "" }), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match(new[] { "", "HogeHoge", "123" }), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match(new[] { "HogeHoge", "123", "" }), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match(new[] { "123", "", "HogeHoge" }), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match(new[] { "+++", "HogeHoge", "123" }), Is.EqualTo(MatchResult.NotMatched));
            });
        }

        /// <summary>
        /// <see cref="AndRowCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void And2()
        {
            var condition = new AndRowCondition()
            {
                Conditions = new RowCondition[]
                {
                    new CheckValueRowCondition()
                    {
                        ColumnIndex = 1,
                        ValueCondition = new IsEmptyValueCondition(),
                    },
                    new CheckAnyValueRowCondition()
                    {
                        ValueCondition = new IsIntegerValueCondition(),
                    },
                    RowCondition.Null,
                },
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="CheckValueRowCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void CheckValueRow1()
        {
            var condition = new CheckValueRowCondition()
            {
                ColumnIndex = 1,
                ValueCondition = new IsEmptyValueCondition(),
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(Array.Empty<string>()), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match(new[] { "", "", "" }), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match(new[] { "", "HogeHoge", "123" }), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match(new[] { "HogeHoge", "123", "" }), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match(new[] { "123", "", "HogeHoge" }), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match(new[] { "+++", "HogeHoge", "123" }), Is.EqualTo(MatchResult.NotMatched));
            });
        }

        /// <summary>
        /// <see cref="CheckValueRowCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void CheckValueRow2()
        {
            var condition = new CheckValueRowCondition()
            {
                ValueCondition = ValueCondition.Null,
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="CheckValueRowCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void CheckValueRow3()
        {
            var condition = new CheckValueRowCondition()
            {
                ColumnIndex = -1,
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="CheckAllValueRowCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void CheckAllValueRow1()
        {
            var condition = new CheckAllValueRowCondition()
            {
                ValueCondition = new IsEmptyValueCondition(),
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(Array.Empty<string>()), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match(new[] { "", "", "" }), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match(new[] { "", "HogeHoge", "123" }), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match(new[] { "HogeHoge", "123", "" }), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match(new[] { "123", "", "HogeHoge" }), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match(new[] { "+++", "HogeHoge", "123" }), Is.EqualTo(MatchResult.NotMatched));
            });
        }

        /// <summary>
        /// <see cref="CheckAllValueRowCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void CheckAllValueRow2()
        {
            var condition = new CheckAllValueRowCondition()
            {
                ValueCondition = ValueCondition.Null,
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="CheckAnyValueRowCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void CheckAnyValueRow1()
        {
            var condition = new CheckAnyValueRowCondition()
            {
                ValueCondition = new IsEmptyValueCondition(),
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(Array.Empty<string>()), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match(new[] { "", "", "" }), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match(new[] { "", "HogeHoge", "123" }), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match(new[] { "HogeHoge", "123", "" }), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match(new[] { "123", "", "HogeHoge" }), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match(new[] { "+++", "HogeHoge", "123" }), Is.EqualTo(MatchResult.NotMatched));
            });
        }

        /// <summary>
        /// <see cref="CheckAnyValueRowCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void CheckAnyValueRow2()
        {
            var condition = new CheckAnyValueRowCondition()
            {
                ValueCondition = ValueCondition.Null,
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }
    }
}
