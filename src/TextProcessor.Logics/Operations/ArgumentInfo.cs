using System;
using System.Collections.Generic;
using TextProcessor.Logics.Operations.Conditions;
using TextProcessor.Logics.Operations.Conversions;

namespace TextProcessor.Logics.Operations
{
    /// <summary>
    /// 引数の情報を表します。
    /// </summary>
    [Serializable]
    public sealed class ArgumentInfo
    {
        #region Parameter Names

        internal const string ParamCtor = "Ctor";

        #endregion Parameter Names

        /// <summary>
        /// 引数の種類を取得します。
        /// </summary>
        public ArgumentType Type { get; }

        /// <summary>
        /// 数値を表すかどうかを取得します。
        /// </summary>
        public bool IsNumber => (Type & (ArgumentType.Integer | ArgumentType.Integer64 | ArgumentType.Decimal | ArgumentType.Index)) != 0;

        /// <summary>
        /// 条件を表すかどうかを取得します。
        /// </summary>
        public bool IsCondition => (Type & (ArgumentType.ValueCondition | ArgumentType.RowCondition)) != 0;

        /// <summary>
        /// 固定長配列を表すかどうかを取得します。
        /// </summary>
        public bool IsArray => Type.HasFlag(ArgumentType.Array);

        /// <summary>
        /// 可変長配列を表すかどうかを取得します。
        /// </summary>
        public bool IsList => Type.HasFlag(ArgumentType.List);

        /// <summary>
        /// 引数名を取得します。
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// 値のゲッターを取得します。
        /// </summary>
        public Func<dynamic> Getter { get; }

        /// <summary>
        /// 値のセッターを取得します。
        /// </summary>
        public Action<dynamic> Setter { get; }

        /// <summary>
        /// 値を取得または設定します。
        /// </summary>
        public dynamic Value
        {
            get => Getter.Invoke();
            set => Setter.Invoke(value);
        }

        /// <summary>
        /// 追加で使用するパラメータを取得します。
        /// </summary>
        public IDictionary<string, dynamic> Parameters => _parameters ??= new Dictionary<string, object>(StringComparer.Ordinal);

        private Dictionary<string, dynamic>? _parameters;

        /// <summary>
        /// <see cref="ArgumentInfo"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="type">引数の種類</param>
        /// <param name="name">名前</param>
        /// <param name="gettter">ゲッター関数</param>
        /// <param name="setter">セッター関数</param>
        /// <exception cref="ArgumentNullException"><paramref name="gettter"/>または<paramref name="setter"/>が<see langword="null"/></exception>
        public ArgumentInfo(ArgumentType type, string? name, Func<dynamic> gettter, Action<dynamic> setter)
        {
            ArgumentNullException.ThrowIfNull(gettter);
            ArgumentNullException.ThrowIfNull(setter);

            Type = type;
            Name = name;
            Getter = gettter;
            Setter = setter;
        }

        /// <summary>
        /// 対応した既定値を取得します。
        /// </summary>
        public object? GetDefaultValue()
        {
            ArgumentType type = Type & (~ArgumentType.Array) & (~ArgumentType.List);
            return type switch
            {
                ArgumentType.String or ArgumentType.StringMultiLine => string.Empty,
                ArgumentType.Integer => 0,
                ArgumentType.Integer64 => 0L,
                ArgumentType.Decimal => 0m,
                ArgumentType.Index => 0,
                ArgumentType.Boolean => false,
                ArgumentType.ValueCondition => ValueCondition.Null,
                ArgumentType.RowCondition => RowCondition.Null,
                ArgumentType.TextData => null,
                ArgumentType.ValueConversion => ValueConversion.Through,
                ArgumentType.Arguments => this.GetCtor().Invoke(),
                _ => throw new NotSupportedException(),
            };
        }
    }

    /// <summary>
    /// <see cref="ArgumentInfo"/>の拡張を記述します。
    /// </summary>
    public static class ArgumentInfoExtension
    {
        /// <summary>
        /// インスタンス生成関数を取得します。
        /// </summary>
        /// <param name="info">対象引数</param>
        /// <returns>インスタンス生成関数</returns>
        /// <exception cref="ArgumentNullException"><paramref name="info"/>が<see langword="null"/></exception>
        public static Func<dynamic> GetCtor(this ArgumentInfo info)
        {
            ArgumentNullException.ThrowIfNull(info);
            return info.Parameters[ArgumentInfo.ParamCtor];
        }

        /// <summary>
        /// インスタンス生成関数を設定します。
        /// </summary>
        /// <param name="info">対象引数</param>
        /// <param name="ctor">設定するインスタンス生成関数</param>
        /// <exception cref="ArgumentNullException"><paramref name="info"/>が<see langword="null"/></exception>
        public static void SetCtor(this ArgumentInfo info, Func<dynamic> ctor)
        {
            ArgumentNullException.ThrowIfNull(info);
            ArgumentNullException.ThrowIfNull(ctor);

            info.Parameters[ArgumentInfo.ParamCtor] = ctor;
        }
    }
}
