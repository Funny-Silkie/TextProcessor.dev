using System;
using System.Collections.Generic;
using System.Numerics;

namespace TextProcessor.Logics.Operations.Conversions
{
    /// <summary>
    /// 加算を行う変換を表します。
    /// </summary>
    /// <typeparam name="T">計算する型</typeparam>
    [Serializable]
    internal sealed class AddValueConversion<T> : ValueConversion
        where T : IAdditionOperators<T, T, T>
    {
        private readonly Func<string, (bool success, T valueT)> converter;
        private readonly ArgumentType type;

        /// <summary>
        /// 演算対象の値を取得または設定します。
        /// </summary>
        public T Target { get; set; }

        /// <inheritdoc/>
        public override string? Title { get; }

        /// <summary>
        /// <see cref="AddValueConversion{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="type">要素の種類</param>
        /// <param name="title">表示名</param>
        /// <param name="target">比較対象</param>
        /// <param name="converter">文字列からの変換関数</param>
        /// <exception cref="ArgumentNullException"><paramref name="converter"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="type"/>が未定義の値</exception>
        public AddValueConversion(ArgumentType type, string? title, T target, Func<string, (bool, T)> converter)
        {
            ArgumentNullException.ThrowIfNull(converter);
            if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));

            this.type = type;
            this.converter = converter;
            Target = target;
            Title = title;
        }

        /// <summary>
        /// <see cref="AddValueConversion{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        private AddValueConversion(AddValueConversion<T> cloned)
            : base(cloned)
        {
            converter = cloned.converter;
            type = cloned.type;
            Target = cloned.Target is ICloneable c ? (T)c.Clone() : cloned.Target;
            Title = cloned.Title;
        }

        /// <inheritdoc/>
        protected override ValueConversion CloneCore() => new AddValueConversion<T>(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(type, "加算する値", () => Target, x => Target = x),
            };
        }

        /// <inheritdoc/>
        protected override string? ConvertCore(string value, ProcessStatus status)
        {
            (bool success, T valueT) = converter.Invoke(value);
            if (!success)
            {
                status.Errors.Add(new StatusEntry(Title, null, $"'{value}'が無効なフォーマットです"));
                return null;
            }
            return (valueT + Target).ToString();
        }
    }

    /// <summary>
    /// 減算を行う変換を表します。
    /// </summary>
    /// <typeparam name="T">計算する型</typeparam>
    [Serializable]
    internal sealed class SubtractValueConversion<T> : ValueConversion
        where T : ISubtractionOperators<T, T, T>
    {
        private readonly Func<string, (bool success, T valueT)> converter;
        private readonly ArgumentType type;

        /// <summary>
        /// 演算対象の値を取得または設定します。
        /// </summary>
        public T Target { get; set; }

        /// <inheritdoc/>
        public override string? Title { get; }

        /// <summary>
        /// <see cref="SubtractValueConversion{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="type">要素の種類</param>
        /// <param name="title">表示名</param>
        /// <param name="target">比較対象</param>
        /// <param name="converter">文字列からの変換関数</param>
        /// <exception cref="ArgumentNullException"><paramref name="converter"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="type"/>が未定義の値</exception>
        public SubtractValueConversion(ArgumentType type, string? title, T target, Func<string, (bool, T)> converter)
        {
            ArgumentNullException.ThrowIfNull(converter);
            if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));

            this.type = type;
            this.converter = converter;
            Target = target;
            Title = title;
        }

        /// <summary>
        /// <see cref="SubtractValueConversion{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        private SubtractValueConversion(SubtractValueConversion<T> cloned)
            : base(cloned)
        {
            converter = cloned.converter;
            type = cloned.type;
            Target = cloned.Target is ICloneable c ? (T)c.Clone() : cloned.Target;
            Title = cloned.Title;
        }

        /// <inheritdoc/>
        protected override ValueConversion CloneCore() => new SubtractValueConversion<T>(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(type, "減算する値", () => Target, x => Target = x),
            };
        }

        /// <inheritdoc/>
        protected override string? ConvertCore(string value, ProcessStatus status)
        {
            (bool success, T valueT) = converter.Invoke(value);
            if (!success)
            {
                status.Errors.Add(new StatusEntry(Title, null, $"'{value}'が無効なフォーマットです"));
                return null;
            }
            return (valueT - Target).ToString();
        }
    }

    /// <summary>
    /// 乗算を行う変換を表します。
    /// </summary>
    /// <typeparam name="T">計算する型</typeparam>
    [Serializable]
    internal sealed class MultiplyValueConversion<T> : ValueConversion
        where T : IMultiplyOperators<T, T, T>
    {
        private readonly Func<string, (bool success, T valueT)> converter;
        private readonly ArgumentType type;

        /// <summary>
        /// 演算対象の値を取得または設定します。
        /// </summary>
        public T Target { get; set; }

        /// <inheritdoc/>
        public override string? Title { get; }

        /// <summary>
        /// <see cref="MultiplyValueConversion{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="type">要素の種類</param>
        /// <param name="title">表示名</param>
        /// <param name="target">比較対象</param>
        /// <param name="converter">文字列からの変換関数</param>
        /// <exception cref="ArgumentNullException"><paramref name="converter"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="type"/>が未定義の値</exception>
        public MultiplyValueConversion(ArgumentType type, string? title, T target, Func<string, (bool, T)> converter)
        {
            ArgumentNullException.ThrowIfNull(converter);
            if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));

            this.type = type;
            this.converter = converter;
            Target = target;
            Title = title;
        }

        /// <summary>
        /// <see cref="MultiplyValueConversion{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        private MultiplyValueConversion(MultiplyValueConversion<T> cloned)
            : base(cloned)
        {
            converter = cloned.converter;
            type = cloned.type;
            Target = cloned.Target is ICloneable c ? (T)c.Clone() : cloned.Target;
            Title = cloned.Title;
        }

        /// <inheritdoc/>
        protected override ValueConversion CloneCore() => new MultiplyValueConversion<T>(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(type, "乗算する値", () => Target, x => Target = x),
            };
        }

        /// <inheritdoc/>
        protected override string? ConvertCore(string value, ProcessStatus status)
        {
            (bool success, T valueT) = converter.Invoke(value);
            if (!success)
            {
                status.Errors.Add(new StatusEntry(Title, null, $"'{value}'が無効なフォーマットです"));
                return null;
            }
            return (valueT * Target).ToString();
        }
    }

    /// <summary>
    /// 除算を行う変換を表します。
    /// </summary>
    /// <typeparam name="T">計算する型</typeparam>
    [Serializable]
    internal sealed class DivideValueConversion<T> : ValueConversion
        where T : INumberBase<T>
    {
        private readonly bool allowZeroDivision;
        private readonly Func<string, (bool success, T valueT)> converter;
        private readonly ArgumentType type;

        /// <summary>
        /// 演算対象の値を取得または設定します。
        /// </summary>
        public T Target { get; set; }

        /// <inheritdoc/>
        public override string? Title { get; }

        /// <summary>
        /// <see cref="DivideValueConversion{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="type">要素の種類</param>
        /// <param name="title">表示名</param>
        /// <param name="target">比較対象</param>
        /// <param name="converter">文字列からの変換関数</param>
        /// <param name="allowZeroDivision">ゼロ除算を許容するかどうかを表す値</param>
        /// <exception cref="ArgumentNullException"><paramref name="converter"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="type"/>が未定義の値</exception>
        public DivideValueConversion(ArgumentType type, string? title, T target, Func<string, (bool, T)> converter, bool allowZeroDivision)
        {
            ArgumentNullException.ThrowIfNull(converter);
            if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));

            this.type = type;
            this.converter = converter;
            Target = target;
            Title = title;
            this.allowZeroDivision = allowZeroDivision;
        }

        /// <summary>
        /// <see cref="DivideValueConversion{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        private DivideValueConversion(DivideValueConversion<T> cloned)
            : base(cloned)
        {
            allowZeroDivision = cloned.allowZeroDivision;
            converter = cloned.converter;
            type = cloned.type;
            Target = cloned.Target is ICloneable c ? (T)c.Clone() : cloned.Target;
            Title = cloned.Title;
        }

        /// <inheritdoc/>
        protected override ValueConversion CloneCore() => new DivideValueConversion<T>(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(type, "除算する値", () => Target, x => Target = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (!allowZeroDivision && Target == T.Zero) status.Errors.Add(new StatusEntry(Title, Arguments[0], "ゼロで除算できません"));
        }

        /// <inheritdoc/>
        protected override string? ConvertCore(string value, ProcessStatus status)
        {
            (bool success, T valueT) = converter.Invoke(value);
            if (!success)
            {
                status.Errors.Add(new StatusEntry(Title, null, $"'{value}'が無効なフォーマットです"));
                return null;
            }
            if (!allowZeroDivision && Target == T.Zero)
            {
                status.Errors.Add(new StatusEntry(Title, null, $"ゼロで除算できません"));
                return null;
            }
            return (valueT / Target).ToString();
        }
    }

    /// <summary>
    /// 剰余算を行う変換を表します。
    /// </summary>
    /// <typeparam name="T">計算する型</typeparam>
    [Serializable]
    internal sealed class ModuloValueConversion<T> : ValueConversion
        where T : INumberBase<T>, IModulusOperators<T, T, T>
    {
        private readonly bool allowZeroDivision;
        private readonly Func<string, (bool success, T valueT)> converter;
        private readonly ArgumentType type;

        /// <summary>
        /// 演算対象の値を取得または設定します。
        /// </summary>
        public T Target { get; set; }

        /// <inheritdoc/>
        public override string? Title { get; }

        /// <summary>
        /// <see cref="ModuloValueConversion{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="type">要素の種類</param>
        /// <param name="title">表示名</param>
        /// <param name="target">比較対象</param>
        /// <param name="converter">文字列からの変換関数</param>
        /// <param name="allowZeroDivision">ゼロ剰余算を許容するかどうかを表す値</param>
        /// <exception cref="ArgumentNullException"><paramref name="converter"/>が<see langword="null"/></exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="type"/>が未定義の値</exception>
        public ModuloValueConversion(ArgumentType type, string? title, T target, Func<string, (bool, T)> converter, bool allowZeroDivision)
        {
            ArgumentNullException.ThrowIfNull(converter);
            if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));

            this.type = type;
            this.converter = converter;
            Target = target;
            Title = title;
            this.allowZeroDivision = allowZeroDivision;
        }

        /// <summary>
        /// <see cref="ModuloValueConversion{T}"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        private ModuloValueConversion(ModuloValueConversion<T> cloned)
            : base(cloned)
        {
            allowZeroDivision = cloned.allowZeroDivision;
            converter = cloned.converter;
            type = cloned.type;
            Target = cloned.Target is ICloneable c ? (T)c.Clone() : cloned.Target;
            Title = cloned.Title;
        }

        /// <inheritdoc/>
        protected override ValueConversion CloneCore() => new ModuloValueConversion<T>(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(type, "剰余算する値", () => Target, x => Target = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (!allowZeroDivision && Target == T.Zero) status.Errors.Add(new StatusEntry(Title, Arguments[0], "ゼロで剰余算できません"));
        }

        /// <inheritdoc/>
        protected override string? ConvertCore(string value, ProcessStatus status)
        {
            (bool success, T valueT) = converter.Invoke(value);
            if (!success)
            {
                status.Errors.Add(new StatusEntry(Title, null, $"'{value}'が無効なフォーマットです"));
                return null;
            }
            if (!allowZeroDivision && Target == T.Zero)
            {
                status.Errors.Add(new StatusEntry(Title, null, $"ゼロで剰余算できません"));
                return null;
            }
            return (valueT % Target).ToString();
        }
    }

    /// <summary>
    /// 桁丸めを行う変換を表します。
    /// </summary>
    [Serializable]
    internal sealed class RoundValueConversion : ValueConversion
    {
        /// <inheritdoc/>
        public override string? Title => "桁丸め";

        /// <summary>
        /// 桁数を取得または設定します。
        /// </summary>
        public int Digits { get; set; }

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.Integer, "桁数", () => Digits, x => Digits = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (Digits < 0) status.Errors.Add(new StatusEntry(Title, Arguments[0], "桁数が負の値です"));
        }

        /// <inheritdoc/>
        protected override string? ConvertCore(string value, ProcessStatus status)
        {
            if (!double.TryParse(value, out double num))
            {
                status.Errors.Add(new StatusEntry(Title, null, $"'{value}'は数値ではありません"));
                return null;
            }

            return Math.Round(num, Digits, MidpointRounding.AwayFromZero).ToString();
        }
    }

    /// <summary>
    /// 切り捨てを行う変換を表します。
    /// </summary>
    [Serializable]
    internal sealed class TruncateValueConversion : ValueConversion
    {
        /// <inheritdoc/>
        public override string? Title => "小数点切り捨て";

        /// <inheritdoc/>
        protected override string? ConvertCore(string value, ProcessStatus status)
        {
            if (!double.TryParse(value, out double num))
            {
                status.Errors.Add(new StatusEntry(Title, null, $"'{value}'は数値ではありません"));
                return null;
            }

            return Math.Truncate(num).ToString();
        }
    }

    /// <summary>
    /// 切り上げを行う変換を表します。
    /// </summary>
    [Serializable]
    internal sealed class CeilingValueConversion : ValueConversion
    {
        /// <inheritdoc/>
        public override string? Title => "小数点切り上げ";

        /// <inheritdoc/>
        protected override string? ConvertCore(string value, ProcessStatus status)
        {
            if (!double.TryParse(value, out double num))
            {
                status.Errors.Add(new StatusEntry(Title, null, $"'{value}'は数値ではありません"));
                return null;
            }

            return Math.Ceiling(num).ToString();
        }
    }
}
