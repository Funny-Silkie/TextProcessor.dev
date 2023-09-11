using TextProcessor.Logics.Operations;
using TextProcessor.Logics.Operations.Conditions;

namespace Test
{
    /// <summary>
    /// <see cref="ValueCondition"/>のテストを行います。
    /// </summary>
    [TestFixture]
    public class ValueConditionTest
    {
        /// <summary>
        /// <see cref="NotValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void Not1()
        {
            var condition = new NotValueCondition()
            {
                Condition = new IsEmptyValueCondition(),
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("123"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("-123"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("1.4"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("-1e+5"), Is.EqualTo(MatchResult.Matched));
            });
        }

        /// <summary>
        /// <see cref="NotValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void Not2()
        {
            var condition = new NotValueCondition()
            {
                Condition = ValueCondition.Null,
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="OrValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void Or1()
        {
            var condition = new OrValueCondition()
            {
                Conditions = new ValueCondition[]{
                    new IsEmptyValueCondition(),
                    new IsIntegerValueCondition(),
                },
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("123"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("-123"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("1.4"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("-1e+5"), Is.EqualTo(MatchResult.NotMatched));
            });
        }

        /// <summary>
        /// <see cref="OrValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void Or2()
        {
            var condition = new OrValueCondition()
            {
                Conditions = new ValueCondition[]{
                    new IsEmptyValueCondition(),
                    new IsIntegerValueCondition(),
                    ValueCondition.Null,
                },
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="AndValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void And1()
        {
            var condition = new AndValueCondition()
            {
                Conditions = new ValueCondition[]{
                    new IsIntegerValueCondition(),
                    new StartsWithValueCondition()
                    {
                        Comparison = "-",
                    },
                },
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("123"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("-123"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("1.4"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("-1e+5"), Is.EqualTo(MatchResult.NotMatched));
            });
        }

        /// <summary>
        /// <see cref="AndValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void And2()
        {
            var condition = new AndValueCondition()
            {
                Conditions = new ValueCondition[]{
                    new IsEmptyValueCondition(),
                    new IsIntegerValueCondition(),
                    ValueCondition.Null,
                },
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="IsEmptyValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void IsEmpty()
        {
            var condition = new IsEmptyValueCondition();

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("123"), Is.EqualTo(MatchResult.NotMatched));
            });
        }

        /// <summary>
        /// <see cref="IsIntegerValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void IsInteger()
        {
            var condition = new IsIntegerValueCondition();

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("123"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("-123"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("1.4"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("-1e+5"), Is.EqualTo(MatchResult.NotMatched));
            });
        }

        /// <summary>
        /// <see cref="IsDecimalValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void IsDecimal()
        {
            var condition = new IsDecimalValueCondition();

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("123"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("-123"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("1.4"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("-1e+5"), Is.EqualTo(MatchResult.Matched));
            });
        }

        /// <summary>
        /// <see cref="MatchValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void Match1()
        {
            var condition = new MatchValueCondition()
            {
                Comparison = "HogeHoge",
                CaseSensitive = true,
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("hogehoge"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("Hoge"), Is.EqualTo(MatchResult.NotMatched));
            });
        }

        /// <summary>
        /// <see cref="MatchValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void Match2()
        {
            var condition = new MatchValueCondition()
            {
                Comparison = "HogeHoge",
                CaseSensitive = false,
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("hogehoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("Hoge"), Is.EqualTo(MatchResult.NotMatched));
            });
        }

        /// <summary>
        /// <see cref="ContainsValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void Contains1()
        {
            var condition = new ContainsValueCondition()
            {
                Comparison = "Hoge",
                CaseSensitive = true,
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("hogehoge"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("Hoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("Fuga"), Is.EqualTo(MatchResult.NotMatched));
            });
        }

        /// <summary>
        /// <see cref="ContainsValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void Contains2()
        {
            var condition = new ContainsValueCondition()
            {
                Comparison = "Hoge",
                CaseSensitive = false,
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("hogehoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("Hoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("Fuga"), Is.EqualTo(MatchResult.NotMatched));
            });
        }

        /// <summary>
        /// <see cref="ContainsValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void Contains3()
        {
            var condition = new ContainsValueCondition()
            {
                Comparison = "",
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="StartsWithValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void StartsWith1()
        {
            var condition = new StartsWithValueCondition()
            {
                Comparison = "Hoge",
                CaseSensitive = true,
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("HogeFuga"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("FugaHoge"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("hogefuga"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("fugahoge"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("Hoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("Fuga"), Is.EqualTo(MatchResult.NotMatched));
            });
        }

        /// <summary>
        /// <see cref="StartsWithValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void StartsWith2()
        {
            var condition = new StartsWithValueCondition()
            {
                Comparison = "Hoge",
                CaseSensitive = false,
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("HogeFuga"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("FugaHoge"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("hogefuga"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("fugahoge"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("Hoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("Fuga"), Is.EqualTo(MatchResult.NotMatched));
            });
        }

        /// <summary>
        /// <see cref="StartsWithValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void StartsWith3()
        {
            var condition = new StartsWithValueCondition()
            {
                Comparison = "",
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="EndsWithValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void EndsWith1()
        {
            var condition = new EndsWithValueCondition()
            {
                Comparison = "Hoge",
                CaseSensitive = true,
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("HogeFuga"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("FugaHoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("hogefuga"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("fugahoge"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("Hoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("Fuga"), Is.EqualTo(MatchResult.NotMatched));
            });
        }

        /// <summary>
        /// <see cref="EndsWithValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void EndsWith2()
        {
            var condition = new EndsWithValueCondition()
            {
                Comparison = "Hoge",
                CaseSensitive = false,
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("HogeFuga"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("FugaHoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("hogefuga"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("fugahoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("Hoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("Fuga"), Is.EqualTo(MatchResult.NotMatched));
            });
        }

        /// <summary>
        /// <see cref="EndsWithValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void EndsWith3()
        {
            var condition = new EndsWithValueCondition()
            {
                Comparison = "",
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="RegexMatchValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void RegexMatch1()
        {
            var condition = new RegexMatchValueCondition()
            {
                Pattern = "Hoge",
                CaseSensitive = true,
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("HogeFuga"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("hogehoge"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("Hoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("Fuga"), Is.EqualTo(MatchResult.NotMatched));
            });
        }

        /// <summary>
        /// <see cref="RegexMatchValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void RegexMatch2()
        {
            var condition = new RegexMatchValueCondition()
            {
                Pattern = "[A-Z][a-z]{3}",
                CaseSensitive = true,
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("HogeFuga"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("hogehoge"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("Hoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("Fuga"), Is.EqualTo(MatchResult.Matched));
            });
        }

        /// <summary>
        /// <see cref="RegexMatchValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void RegexMatch3()
        {
            var condition = new RegexMatchValueCondition()
            {
                Pattern = "[A-Z][a-z]{3}",
                CaseSensitive = false,
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("HogeFuga"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("hogehoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("Hoge"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("Fuga"), Is.EqualTo(MatchResult.Matched));
            });
        }

        /// <summary>
        /// <see cref="RegexMatchValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void RegexMatch4()
        {
            var condition = new RegexMatchValueCondition()
            {
                Pattern = "",
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="RegexMatchValueCondition"/>のテストを行います。
        /// </summary>
        [Test]
        public void RegexMatch5()
        {
            var condition = new RegexMatchValueCondition()
            {
                Pattern = "(",
            };

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="LargerValueCondition{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void LargerAsInteger()
        {
            LargerValueCondition<long> condition = ValueConditionFactory.LargerAsInteger();
            condition.Comparison = 1;

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("123"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("-123"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("1"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("1.4"), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("-1e+5"), Is.EqualTo(MatchResult.Error));
            });
        }

        /// <summary>
        /// <see cref="LargerValueCondition{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void LargerAsDecimal()
        {
            LargerValueCondition<double> condition = ValueConditionFactory.LargerAsDecimal();
            condition.Comparison = 1.1;

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("123"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("-123"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("1.4"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("-1e+5"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("1.1"), Is.EqualTo(MatchResult.NotMatched));
            });
        }

        /// <summary>
        /// <see cref="LowerValueCondition{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void LowerAsInteger()
        {
            LowerValueCondition<long> condition = ValueConditionFactory.LowerAsInteger();
            condition.Comparison = 1;

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("123"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("-123"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("1"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("1.4"), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("-1e+5"), Is.EqualTo(MatchResult.Error));
            });
        }

        /// <summary>
        /// <see cref="LowerValueCondition{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void LowerAsDecimal()
        {
            LowerValueCondition<double> condition = ValueConditionFactory.LowerAsDecimal();
            condition.Comparison = 1.1;

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("123"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("-123"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("1.4"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("-1e+5"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("1.1"), Is.EqualTo(MatchResult.NotMatched));
            });
        }

        /// <summary>
        /// <see cref="LargerOrEqualValueCondition{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void LargerOrEqualAsInteger()
        {
            LargerOrEqualValueCondition<long> condition = ValueConditionFactory.LargerOrEqualAsInteger();
            condition.Comparison = 1;

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("123"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("-123"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("1"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("1.4"), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("-1e+5"), Is.EqualTo(MatchResult.Error));
            });
        }

        /// <summary>
        /// <see cref="LargerOrEqualValueCondition{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void LargerOrEqualAsDecimal()
        {
            LargerOrEqualValueCondition<double> condition = ValueConditionFactory.LargerOrEqualAsDecimal();
            condition.Comparison = 1.1;

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("123"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("-123"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("1.4"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("-1e+5"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("1.1"), Is.EqualTo(MatchResult.Matched));
            });
        }

        /// <summary>
        /// <see cref="LowerOrEqualValueCondition{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void LowerOrEqualAsInteger()
        {
            LowerOrEqualValueCondition<long> condition = ValueConditionFactory.LowerOrEqualAsInteger();
            condition.Comparison = 1;

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("123"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("-123"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("1"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("1.4"), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("-1e+5"), Is.EqualTo(MatchResult.Error));
            });
        }

        /// <summary>
        /// <see cref="LowerOrEqualValueCondition{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void LowerOrEqualAsDecimal()
        {
            LowerOrEqualValueCondition<double> condition = ValueConditionFactory.LowerOrEqualAsDecimal();
            condition.Comparison = 1.1;

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("123"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("-123"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("1.4"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("-1e+5"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("1.1"), Is.EqualTo(MatchResult.Matched));
            });
        }

        /// <summary>
        /// <see cref="EqualValueCondition{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void EqualAsInteger()
        {
            EqualValueCondition<long> condition = ValueConditionFactory.EqualAsInteger();
            condition.Comparison = 1;

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("123"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("-123"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("1"), Is.EqualTo(MatchResult.Matched));
                Assert.That(condition.Match("1.4"), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("-1e+5"), Is.EqualTo(MatchResult.Error));
            });
        }

        /// <summary>
        /// <see cref="EqualValueCondition{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void EqualAsDecimal()
        {
            EqualValueCondition<double> condition = ValueConditionFactory.EqualAsDecimal();
            condition.Comparison = 1.1;

            ProcessStatus argResult = condition.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            Assert.Multiple(() =>
            {
                Assert.That(condition.Match(""), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("HogeHoge"), Is.EqualTo(MatchResult.Error));
                Assert.That(condition.Match("123"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("-123"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("1.4"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("-1e+5"), Is.EqualTo(MatchResult.NotMatched));
                Assert.That(condition.Match("1.1"), Is.EqualTo(MatchResult.Matched));
            });
        }
    }
}
