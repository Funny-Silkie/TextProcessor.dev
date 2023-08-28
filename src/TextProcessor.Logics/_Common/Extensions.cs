﻿using System;
using System.Collections.Generic;

namespace TextProcessor.Logics
{
    /// <summary>
    /// 拡張を記述します。
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// 複製を行います。
        /// </summary>
        /// <typeparam name="T">複製するオブジェクトの型</typeparam>
        /// <param name="cloned">複製するインスタンス</param>
        /// <returns>複製された新しいインスタンス</returns>
        public static T GenericClone<T>(this T cloned)
            where T : ICloneable
        {
            return (T)cloned.Clone();
        }

        /// <summary>
        /// 全ての複製を行います。
        /// </summary>
        /// <typeparam name="T">複製するオブジェクトの型</typeparam>
        /// <param name="array">複製する配列</param>
        /// <returns>要素も複製された新しい配列</returns>
        public static T[] CloneAll<T>(this T[] array)
            where T : ICloneable
        {
            if (array.Length == 0) Array.Empty<T>();
            var result = new T[array.Length];
            for (int i = 0; i < array.Length; i++) result[i] = (T)array[i].Clone();
            return result;
        }

        /// <summary>
        /// 全ての複製を行います。
        /// </summary>
        /// <typeparam name="T">複製するオブジェクトの型</typeparam>
        /// <param name="list">複製するリスト</param>
        /// <returns>要素も複製された新しいリスト</returns>
        public static List<T> CloneAll<T>(this List<T> list)
            where T : ICloneable
        {
            var result = new List<T>(list.Capacity);
            for (int i = 0; i < list.Count; i++) result.Add((T)list[i].Clone());
            return result;
        }
    }
}
