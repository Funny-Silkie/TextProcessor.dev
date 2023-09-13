using System;
using TextProcessor.Logics.Operations.Conditions;

namespace TextProcessor.Logics.Operations
{
    /// <summary>
    /// 引数の情報を表します。
    /// </summary>
    [Serializable]
    public sealed class ArgumentInfo
    {
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
        /// <see cref="ArgumentType"/>に対応した既定値を取得します。
        /// </summary>
        /// <param name="type">データ型</param>
        /// <returns><paramref name="type"/>に対応する既定値</returns>
        /// <exception cref="NotSupportedException"><paramref name="type"/>が無効</exception>
        public static object? GetDefaultValue(ArgumentType type)
        {
            type &= (~ArgumentType.Array) & (~ArgumentType.List);
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
                _ => throw new NotSupportedException(),
            };
        }
    }
}
