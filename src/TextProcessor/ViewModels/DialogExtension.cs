using Radzen;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextProcessor.Data;
using TextProcessor.Shared.Dialog;

namespace TextProcessor.ViewModels
{
    /// <summary>
    /// ダイアログの拡張を表します。
    /// </summary>
    public static class DialogExtension
    {
        /// <summary>
        /// 処理追加用のダイアログを表示します。
        /// </summary>
        /// <param name="dialogService">使用する<see cref="DialogService"/>のインスタンス</param>
        /// <returns>ViewModelのインスタンス</returns>
        public static async Task<AddOperationViewModel?> OpenAddOperationAsync(this DialogService dialogService)
        {
            var options = new DialogOptions()
            {
                Width = "50vw",
                Height = "70vh",
            };
            return await dialogService.OpenAsync<AddOperation>("処理の追加", null, options);
        }

        /// <summary>
        /// ダウンロード用のダイアログを表示します。
        /// </summary>
        /// <param name="dialogService">使用する<see cref="DialogService"/>のインスタンス</param>
        /// <param name="fileData">ファイル</param>
        /// <returns>ViewModelのインスタンス</returns>
        public static async Task<DownloadViewModel?> OpenDownloadAsync(this DialogService dialogService, TableFileInfo? fileData)
        {
            var options = new DialogOptions()
            {
                Width = "40vw",
                Height = "25rem",
            };
            var parameters = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                [nameof(Download.FileData)] = fileData,
            };
            return await dialogService.OpenAsync<Download>("ダウンロード", parameters, options);
        }

        /// <summary>
        /// 待機用のダイアログを表示します。
        /// </summary>
        /// <param name="dialogService">使用する<see cref="DialogService"/>のインスタンス</param>
        public static void OpenWaiting(this DialogService dialogService, string message)
        {
            var options = new DialogOptions()
            {
                Width = "300px",
                Height = "200px",
                CloseDialogOnEsc = false,
                CloseDialogOnOverlayClick = false,
                ShowClose = false,
            };
            var parameters = new Dictionary<string, object?>()
            {
                [nameof(WaitingDialog.Message)] = message,
            };
            dialogService.Open<WaitingDialog>(string.Empty, parameters, options);
        }

        /// <summary>
        /// 待機用のダイアログを表示します。
        /// </summary>
        /// <param name="dialogService">使用する<see cref="DialogService"/>のインスタンス</param>
        public static async Task OpenWaitingAsync(this DialogService dialogService, string message)
        {
            var options = new DialogOptions()
            {
                Width = "300px",
                Height = "200px",
                CloseDialogOnEsc = false,
                CloseDialogOnOverlayClick = false,
                ShowClose = false,
            };
            var parameters = new Dictionary<string, object?>()
            {
                [nameof(WaitingDialog.Message)] = message,
            };
            await dialogService.OpenAsync<WaitingDialog>(string.Empty, parameters, options);
        }
    }
}
