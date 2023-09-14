using System.Collections.Generic;
using TextProcessor.Logics.Operations;
using TextProcessor.Logics.Operations.Conditions;
using TextProcessor.Logics.Operations.Conversions;

namespace Test
{
    /// <summary>
    /// <see cref="ValueConversion"/>のテストを行います。
    /// </summary>
    [TestFixture]
    public class ValueConversionTest
    {
        /// <summary>
        /// 変換結果を確認します。
        /// </summary>
        /// <param name="conversion">変換処理</param>
        /// <param name="target">変換対象</param>
        /// <param name="expected">予想される結果</param>
        private static void VerifyResult(ValueConversion conversion, string target, string? expected)
        {
            ProcessStatus conversionResult = conversion.Convert(target, out string? actual);
            if (expected is null)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(conversionResult.Success, Is.False);
                    Assert.That(actual, Is.Null);
                });
                return;
            }

            Assert.Multiple(() =>
            {
                Assert.That(conversionResult.Success, Is.True);
                Assert.That(actual, Is.EqualTo(expected));
            });
        }

        /// <summary>
        /// <see cref="ValueConditionTest.Through"/>のテストを行います。
        /// </summary>
        [Test]
        public void Through()
        {
            ValueConversion conversion = ValueConversion.Through;

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", "");
            VerifyResult(conversion, "Hoge", "Hoge");
            VerifyResult(conversion, "123", "123");
        }

        /// <summary>
        /// <see cref="SequentialValueConversion"/>のテストを行います。
        /// </summary>
        [Test]
        public void Sequential()
        {
            var conversion = new SequentialValueConversion()
            {
                Conversions = new List<ValueConversion>()
                {
                    new PrependValueConversion()
                    {
                        Value = "PREFIX_",
                    },
                    new AppendValueConversion()
                    {
                        Value = "_SUFFIX",
                    },
                },
            };

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", "PREFIX__SUFFIX");
            VerifyResult(conversion, "Hoge", "PREFIX_Hoge_SUFFIX");
            VerifyResult(conversion, "123", "PREFIX_123_SUFFIX");
        }

        /// <summary>
        /// <see cref="IfValueConversion"/>のテストを行います。
        /// </summary>
        [Test]
        public void If1()
        {
            var conversion = new IfValueConversion()
            {
                Condition = new IsIntegerValueCondition(),
                TrueConversion = new PrependValueConversion()
                {
                    Value = "TRUE_",
                },
                FalseConversion = new AppendValueConversion()
                {
                    Value = "_FALSE",
                },
            };

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", "_FALSE");
            VerifyResult(conversion, "Hoge", "Hoge_FALSE");
            VerifyResult(conversion, "123", "TRUE_123");
        }

        /// <summary>
        /// <see cref="IfValueConversion"/>のテストを行います。
        /// </summary>
        [Test]
        public void If2()
        {
            var conversion = new IfValueConversion()
            {
                Condition = ValueCondition.Null,
            };

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="PrependValueConversion"/>のテストを行います。
        /// </summary>
        [Test]
        public void Prepend()
        {
            var conversion = new PrependValueConversion()
            {
                Value = "PREFIX_",
            };

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", conversion.Value);
            VerifyResult(conversion, "Hoge", "PREFIX_Hoge");
            VerifyResult(conversion, "123", "PREFIX_123");
        }

        /// <summary>
        /// <see cref="AppendValueConversion"/>のテストを行います。
        /// </summary>
        [Test]
        public void Append()
        {
            var conversion = new AppendValueConversion()
            {
                Value = "_SUFFIX",
            };

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", conversion.Value);
            VerifyResult(conversion, "Hoge", "Hoge_SUFFIX");
            VerifyResult(conversion, "123", "123_SUFFIX");
        }

        /// <summary>
        /// <see cref="OverwriteValueConversion"/>のテストを行います。
        /// </summary>
        [Test]
        public void Overwrite()
        {
            var conversion = new OverwriteValueConversion()
            {
                Value = "VALUE",
            };

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", conversion.Value);
            VerifyResult(conversion, "Hoge", conversion.Value);
            VerifyResult(conversion, "123", conversion.Value);
        }

        /// <summary>
        /// <see cref="AddValueConversion{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void Add1()
        {
            AddValueConversion<long> conversion = ValueConversionFactory.AddAsInteger();
            conversion.Target = 10;

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", null);
            VerifyResult(conversion, "Hoge", null);
            VerifyResult(conversion, "123", 133.ToString());
            VerifyResult(conversion, "-30", (-20).ToString());
            VerifyResult(conversion, "1.3", null);
            VerifyResult(conversion, "-30.03", null);
        }

        /// <summary>
        /// <see cref="AddValueConversion{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void Add2()
        {
            AddValueConversion<double> conversion = ValueConversionFactory.AddAsDecimal();
            conversion.Target = 10;

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", null);
            VerifyResult(conversion, "Hoge", null);
            VerifyResult(conversion, "123", 133.ToString());
            VerifyResult(conversion, "-30", (-20).ToString());
            VerifyResult(conversion, "1.3", 11.3.ToString());
            VerifyResult(conversion, "-30.03", (-20.03).ToString());
        }

        /// <summary>
        /// <see cref="SubtractValueConversion{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void Subtract1()
        {
            SubtractValueConversion<long> conversion = ValueConversionFactory.SubtractAsInteger();
            conversion.Target = 10;

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", null);
            VerifyResult(conversion, "Hoge", null);
            VerifyResult(conversion, "123", 113.ToString());
            VerifyResult(conversion, "-30", (-40).ToString());
            VerifyResult(conversion, "1.3", null);
            VerifyResult(conversion, "-30.03", null);
        }

        /// <summary>
        /// <see cref="SubtractValueConversion{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void Subtract2()
        {
            SubtractValueConversion<double> conversion = ValueConversionFactory.SubtractAsDecimal();
            conversion.Target = 10;

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", null);
            VerifyResult(conversion, "Hoge", null);
            VerifyResult(conversion, "123", 113.ToString());
            VerifyResult(conversion, "-30", (-40).ToString());
            VerifyResult(conversion, "1.3", (-8.7).ToString());
            VerifyResult(conversion, "-30.03", (-40.03).ToString());
        }

        /// <summary>
        /// <see cref="MultiplyValueConversion{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void Multiply1()
        {
            MultiplyValueConversion<long> conversion = ValueConversionFactory.MultiplyAsInteger();
            conversion.Target = 10;

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", null);
            VerifyResult(conversion, "Hoge", null);
            VerifyResult(conversion, "123", 1230.ToString());
            VerifyResult(conversion, "-30", (-300).ToString());
            VerifyResult(conversion, "1.3", null);
            VerifyResult(conversion, "-30.03", null);
        }

        /// <summary>
        /// <see cref="MultiplyValueConversion{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void Multiply2()
        {
            MultiplyValueConversion<double> conversion = ValueConversionFactory.MultiplyAsDecimal();
            conversion.Target = 10;

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", null);
            VerifyResult(conversion, "Hoge", null);
            VerifyResult(conversion, "123", 1230.ToString());
            VerifyResult(conversion, "-30", (-300).ToString());
            VerifyResult(conversion, "1.3", 13.ToString());
            VerifyResult(conversion, "-30.03", (-300.3).ToString());
        }

        /// <summary>
        /// <see cref="DivideValueConversion{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void Divide1()
        {
            DivideValueConversion<long> conversion = ValueConversionFactory.DivideAsInteger();
            conversion.Target = 10;

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", null);
            VerifyResult(conversion, "Hoge", null);
            VerifyResult(conversion, "123", 12.ToString());
            VerifyResult(conversion, "-30", (-3).ToString());
            VerifyResult(conversion, "1.3", null);
            VerifyResult(conversion, "-30.03", null);
        }

        /// <summary>
        /// <see cref="DivideValueConversion{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void Divide2()
        {
            DivideValueConversion<long> conversion = ValueConversionFactory.DivideAsInteger();
            conversion.Target = 0;

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="DivideValueConversion{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void Divide3()
        {
            DivideValueConversion<double> conversion = ValueConversionFactory.DivideAsDecimal();
            conversion.Target = 10;

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", null);
            VerifyResult(conversion, "Hoge", null);
            VerifyResult(conversion, "123", 12.3.ToString());
            VerifyResult(conversion, "-30", (-3).ToString());
            VerifyResult(conversion, "1.3", 0.13.ToString());
            VerifyResult(conversion, "-30.03", (-3.003).ToString());
        }

        /// <summary>
        /// <see cref="DivideValueConversion{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void Divide4()
        {
            DivideValueConversion<double> conversion = ValueConversionFactory.DivideAsDecimal();
            conversion.Target = 0;

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", null);
            VerifyResult(conversion, "Hoge", null);
            VerifyResult(conversion, "123", double.PositiveInfinity.ToString());
            VerifyResult(conversion, "-30", double.NegativeInfinity.ToString());
            VerifyResult(conversion, "1.3", double.PositiveInfinity.ToString());
            VerifyResult(conversion, "-30.03", double.NegativeInfinity.ToString());
            VerifyResult(conversion, "0", double.NaN.ToString());
        }

        /// <summary>
        /// <see cref="ModuloValueConversion{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void Modulo1()
        {
            ModuloValueConversion<long> conversion = ValueConversionFactory.ModuloAsInteger();
            conversion.Target = 10;

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", null);
            VerifyResult(conversion, "Hoge", null);
            VerifyResult(conversion, "123", 3.ToString());
            VerifyResult(conversion, "-30", 0.ToString());
            VerifyResult(conversion, "1.3", null);
            VerifyResult(conversion, "-30.03", null);
        }

        /// <summary>
        /// <see cref="ModuloValueConversion{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void Modulo2()
        {
            ModuloValueConversion<long> conversion = ValueConversionFactory.ModuloAsInteger();
            conversion.Target = 0;

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }

        /// <summary>
        /// <see cref="ModuloValueConversion{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void Modulo3()
        {
            ModuloValueConversion<double> conversion = ValueConversionFactory.ModuloAsDecimal();
            conversion.Target = 10;

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", null);
            VerifyResult(conversion, "Hoge", null);
            VerifyResult(conversion, "123", 3.ToString());
            VerifyResult(conversion, "-30", (-0d).ToString());
            VerifyResult(conversion, "1.3", (1.3 % 10).ToString());
            VerifyResult(conversion, "-30.03", (-30.03 % 10).ToString());
        }

        /// <summary>
        /// <see cref="ModuloValueConversion{T}"/>のテストを行います。
        /// </summary>
        [Test]
        public void Modulo4()
        {
            ModuloValueConversion<double> conversion = ValueConversionFactory.ModuloAsDecimal();
            conversion.Target = 0;

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", null);
            VerifyResult(conversion, "Hoge", null);
            VerifyResult(conversion, "123", double.NaN.ToString());
            VerifyResult(conversion, "-30", double.NaN.ToString());
            VerifyResult(conversion, "1.3", double.NaN.ToString());
            VerifyResult(conversion, "-30.03", double.NaN.ToString());
            VerifyResult(conversion, "0", double.NaN.ToString());
        }

        /// <summary>
        /// <see cref="RoundValueConversion"/>のテストを行います。
        /// </summary>
        [Test]
        public void Round1()
        {
            var conversion = new RoundValueConversion()
            {
                Digits = 0,
            };

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", null);
            VerifyResult(conversion, "Hoge", null);
            VerifyResult(conversion, "123", 123d.ToString());
            VerifyResult(conversion, "1.23456789", 1d.ToString());
        }

        /// <summary>
        /// <see cref="RoundValueConversion"/>のテストを行います。
        /// </summary>
        [Test]
        public void Round2()
        {
            var conversion = new RoundValueConversion()
            {
                Digits = 3,
            };

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", null);
            VerifyResult(conversion, "Hoge", null);
            VerifyResult(conversion, "123", 123d.ToString());
            VerifyResult(conversion, "1.23456789", 1.235d.ToString());
        }

        /// <summary>
        /// <see cref="TruncateValueConversion"/>のテストを行います。
        /// </summary>
        [Test]
        public void Truncate()
        {
            var conversion = new TruncateValueConversion();

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", null);
            VerifyResult(conversion, "Hoge", null);
            VerifyResult(conversion, "123", 123d.ToString());
            VerifyResult(conversion, "1.23456789", 1d.ToString());
        }

        /// <summary>
        /// <see cref="CeilingValueConversion"/>のテストを行います。
        /// </summary>
        [Test]
        public void Ceiling()
        {
            var conversion = new CeilingValueConversion();

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", null);
            VerifyResult(conversion, "Hoge", null);
            VerifyResult(conversion, "123", 123d.ToString());
            VerifyResult(conversion, "1.23456789", 2d.ToString());
        }

        /// <summary>
        /// <see cref="Condition2BooleanValueConversion"/>のテストを行います。
        /// </summary>
        [Test]
        public void Condition2Boolean1()
        {
            var conversion = new Condition2BooleanValueConversion()
            {
                Condition = new OrValueCondition()
                {
                    Conditions = new List<ValueCondition>()
                    {
                        new IsEmptyValueCondition(),
                        new IsIntegerValueCondition(),
                    },
                },
            };

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.True);

            VerifyResult(conversion, "", Condition2BooleanValueConversion.TrueString);
            VerifyResult(conversion, "Hoge", Condition2BooleanValueConversion.FalseString);
            VerifyResult(conversion, "123", Condition2BooleanValueConversion.TrueString);
        }

        /// <summary>
        /// <see cref="Condition2BooleanValueConversion"/>のテストを行います。
        /// </summary>
        [Test]
        public void Condition2Boolean2()
        {
            var conversion = new Condition2BooleanValueConversion()
            {
                Condition = ValueCondition.Null,
            };

            ProcessStatus argResult = conversion.VerifyArguments();
            Assert.That(argResult.Success, Is.False);
        }
    }
}
