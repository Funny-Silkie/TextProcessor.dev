using System;

namespace TextProcessor.Logics.Operations.Conditions
{
    /// <summary>
    /// <see cref="ValueCondition"/>の生成を行います。
    /// </summary>
    internal static class ValueConditionFactory
    {
        /// <summary>
        /// <see cref="long"/>に対する<see cref="LargerValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LargerValueCondition{T}"/>の新しいインスタンス</returns>
        public static LargerValueCondition<long> LargerAsInteger()
        {
            return new LargerValueCondition<long>(ArgumentType.Integer64, "指定した整数より大きい", 0, x => (long.TryParse(x, out long result), result));
        }

        /// <summary>
        /// <see cref="double"/>に対する<see cref="LargerValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LargerValueCondition{T}"/>の新しいインスタンス</returns>
        public static LargerValueCondition<double> LargerAsDecimal()
        {
            return new LargerValueCondition<double>(ArgumentType.Decimal, "指定した数値より大きい", 0, x => (double.TryParse(x, out double result), result));
        }

        /// <summary>
        /// <see cref="DateOnly"/>に対する<see cref="LargerValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LargerValueCondition{T}"/>の新しいインスタンス</returns>
        public static LargerValueCondition<DateOnly> LargerAsDateOnly()
        {
            return new LargerValueCondition<DateOnly>(ArgumentType.DateOnly, "指定した日付より大きい", DateOnly.FromDateTime(DateTime.Now), x => (DateOnly.TryParse(x, out DateOnly result), result));
        }

        /// <summary>
        /// <see cref="long"/>に対する<see cref="LowerValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LowerValueCondition{T}"/>の新しいインスタンス</returns>
        public static LowerValueCondition<long> LowerAsInteger()
        {
            return new LowerValueCondition<long>(ArgumentType.Integer64, "指定した整数より小さい", 0, x => (long.TryParse(x, out long result), result));
        }

        /// <summary>
        /// <see cref="double"/>に対する<see cref="LowerValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LowerValueCondition{T}"/>の新しいインスタンス</returns>
        public static LowerValueCondition<double> LowerAsDecimal()
        {
            return new LowerValueCondition<double>(ArgumentType.Decimal, "指定した数値より小さい", 0, x => (double.TryParse(x, out double result), result));
        }

        /// <summary>
        /// <see cref="DateOnly"/>に対する<see cref="LowerValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LowerValueCondition{T}"/>の新しいインスタンス</returns>
        public static LowerValueCondition<DateOnly> LowerAsDateOnly()
        {
            return new LowerValueCondition<DateOnly>(ArgumentType.DateOnly, "指定した日付より小さい", DateOnly.FromDateTime(DateTime.Now), x => (DateOnly.TryParse(x, out DateOnly result), result));
        }

        /// <summary>
        /// <see cref="long"/>に対する<see cref="LargerOrEqualValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LargerValueCondition{T}"/>の新しいインスタンス</returns>
        public static LargerOrEqualValueCondition<long> LargerOrEqualAsInteger()
        {
            return new LargerOrEqualValueCondition<long>(ArgumentType.Integer64, "指定した整数以上", 0, x => (long.TryParse(x, out long result), result));
        }

        /// <summary>
        /// <see cref="double"/>に対する<see cref="LargerOrEqualValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LargerValueCondition{T}"/>の新しいインスタンス</returns>
        public static LargerOrEqualValueCondition<double> LargerOrEqualAsDecimal()
        {
            return new LargerOrEqualValueCondition<double>(ArgumentType.Decimal, "指定した数値以上", 0, x => (double.TryParse(x, out double result), result));
        }

        /// <summary>
        /// <see cref="DateOnly"/>に対する<see cref="LargerOrEqualValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LargerValueCondition{T}"/>の新しいインスタンス</returns>
        public static LargerOrEqualValueCondition<DateOnly> LargerOrEqualAsDateOnly()
        {
            return new LargerOrEqualValueCondition<DateOnly>(ArgumentType.DateOnly, "指定した日付以上", DateOnly.FromDateTime(DateTime.Now), x => (DateOnly.TryParse(x, out DateOnly result), result));
        }

        /// <summary>
        /// <see cref="long"/>に対する<see cref="LowerOrEqualValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LargerValueCondition{T}"/>の新しいインスタンス</returns>
        public static LowerOrEqualValueCondition<long> LowerOrEqualAsInteger()
        {
            return new LowerOrEqualValueCondition<long>(ArgumentType.Integer64, "指定した整数以下", 0, x => (long.TryParse(x, out long result), result));
        }

        /// <summary>
        /// <see cref="double"/>に対する<see cref="LowerOrEqualValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LargerValueCondition{T}"/>の新しいインスタンス</returns>
        public static LowerOrEqualValueCondition<double> LowerOrEqualAsDecimal()
        {
            return new LowerOrEqualValueCondition<double>(ArgumentType.Decimal, "指定した数値以下", 0, x => (double.TryParse(x, out double result), result));
        }

        /// <summary>
        /// <see cref="DateOnly"/>に対する<see cref="LowerOrEqualValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LargerValueCondition{T}"/>の新しいインスタンス</returns>
        public static LowerOrEqualValueCondition<DateOnly> LowerOrEqualAsDateOnly()
        {
            return new LowerOrEqualValueCondition<DateOnly>(ArgumentType.DateOnly, "指定した日付以下", DateOnly.FromDateTime(DateTime.Now), x => (DateOnly.TryParse(x, out DateOnly result), result));
        }

        /// <summary>
        /// <see cref="long"/>に対する<see cref="EqualValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LargerValueCondition{T}"/>の新しいインスタンス</returns>
        public static EqualValueCondition<long> EqualAsInteger()
        {
            return new EqualValueCondition<long>(ArgumentType.Integer64, "指定した整数と等しい", 0, x => (long.TryParse(x, out long result), result));
        }

        /// <summary>
        /// <see cref="double"/>に対する<see cref="EqualValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LargerValueCondition{T}"/>の新しいインスタンス</returns>
        public static EqualValueCondition<double> EqualAsDecimal()
        {
            return new EqualValueCondition<double>(ArgumentType.Decimal, "指定した数値と等しい", 0, x => (double.TryParse(x, out double result), result));
        }

        /// <summary>
        /// <see cref="DateOnly"/>に対する<see cref="EqualValueCondition{T}"/>のインスタンスを生成します。
        /// </summary>
        /// <returns><see cref="LargerValueCondition{T}"/>の新しいインスタンス</returns>
        public static EqualValueCondition<DateOnly> EqualAsDateOnly()
        {
            return new EqualValueCondition<DateOnly>(ArgumentType.DateOnly, "指定した日付と等しい", DateOnly.FromDateTime(DateTime.Now), x => (DateOnly.TryParse(x, out DateOnly result), result));
        }
    }
}
