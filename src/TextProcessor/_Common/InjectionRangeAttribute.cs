using System;

namespace TextProcessor
{
    /// <summary>
    /// Dependency Injectionの範囲を付与します。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class InjectionRangeAttribute : Attribute
    {
        /// <summary>
        /// Dependency Injectionの種類を取得します。
        /// </summary>
        public InjectionType InjectionType { get; }

        /// <summary>
        /// <see cref="InjectionRangeAttribute"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="injectionType">範囲</param>
        public InjectionRangeAttribute(InjectionType injectionType)
        {
            InjectionType = injectionType;
        }
    }
}
