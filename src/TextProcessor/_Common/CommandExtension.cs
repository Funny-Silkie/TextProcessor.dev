using Reactive.Bindings;
using System;
using System.Threading.Tasks;

namespace TextProcessor
{
    /// <summary>
    /// コマンドの拡張を記述します。
    /// </summary>
    public static class CommandExtension
    {
        /// <summary>
        /// デリゲートに変換します。
        /// </summary>
        /// <param name="command">変換するコマンド</param>
        /// <returns><paramref name="command"/>に対応するデリゲート</returns>
        public static Func<Task> ToAsyncDelegate(this AsyncReactiveCommand command) => command.ExecuteAsync;

        /// <summary>
        /// デリゲートに変換します。
        /// </summary>
        /// <typeparam name="T">コマンドの引数</typeparam>
        /// <param name="command">変換するコマンド</param>
        /// <returns><paramref name="command"/>に対応するデリゲート</returns>
        public static Func<T, Task> ToAsyncDelegate<T>(this AsyncReactiveCommand<T> command) => command.ExecuteAsync;
    }
}
