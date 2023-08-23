using System;

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
        public bool IsNumber => Type is ArgumentType.Integer or ArgumentType.Integer64 or ArgumentType.Decimal or ArgumentType.Index;

        /// <summary>
        /// 条件を表すかどうかを取得します。
        /// </summary>
        public bool IsCondition => Type is ArgumentType.ValueCondition or ArgumentType.RowCondition;

        /// <summary>
        /// 引数名を取得します。
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// 値のゲッターを取得します。
        /// </summary>
        public Func<dynamic> Gettter { get; }

        /// <summary>
        /// 値のセッターを取得します。
        /// </summary>
        public Action<dynamic> Setter { get; }

        /// <summary>
        /// 値を取得または設定します。
        /// </summary>
        public dynamic Value
        {
            get => Gettter.Invoke();
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
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="type"/>が未定義</exception>
        public ArgumentInfo(ArgumentType type, string? name, Func<dynamic> gettter, Action<dynamic> setter)
        {
            ArgumentNullException.ThrowIfNull(gettter);
            ArgumentNullException.ThrowIfNull(setter);
            if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));

            Type = type;
            Name = name;
            Gettter = gettter;
            Setter = setter;
        }
    }
}
