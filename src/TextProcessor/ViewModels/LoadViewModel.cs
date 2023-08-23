﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using TextProcessor.Data;
using TextProcessor.Logics.Data.Options;
using TextProcessor.Models;

namespace TextProcessor.ViewModels
{
    /// <summary>
    /// ファイル読み込みページのViewModelのクラスです。
    /// </summary>
    [InjectionRange(InjectionType.Transient)]
    public class LoadViewModel : PageViewModel
    {
        private readonly NotificationService notificationService;
        private readonly MainModel mainModel;

        #region Properties

        /// <summary>
        /// 行区切り文字を取得または設定します。
        /// </summary>
        public ReactivePropertySlim<RowDelimiter> Delimiter { get; }

        /// <summary>
        /// その他の行区切り文字かどうかを表す値を取得します。
        /// </summary>
        public ReadOnlyReactivePropertySlim<bool> IsOtherDelimiter { get; }

        /// <summary>
        ///その他の行区切り文字を取得または設定します。
        /// </summary>
        public ReactivePropertySlim<string> OtherDelimiter { get; }

        /// <summary>
        /// 先頭行をヘッダーとして扱うかどうかを表す値を取得または設定します。
        /// </summary>
        public ReactivePropertySlim<bool> HasHeader { get; }

        #endregion Properties

        #region Commands

        /// <summary>
        /// <inheritdoc cref="Load(InputFileChangeEventArgs)"/>
        /// </summary>
        public AsyncReactiveCommand<InputFileChangeEventArgs> LoadCommand { get; }

        #endregion Commands

        /// <summary>
        /// <see cref="LoadViewModel"/>の新しいインスタンスを初期化します。
        /// </summary>
        public LoadViewModel(NavigationManager navigationManager, MainModel mainModel, NotificationService notificationService)
            : base(navigationManager)
        {
            this.mainModel = mainModel;
            this.notificationService = notificationService;

            Delimiter = new ReactivePropertySlim<RowDelimiter>(RowDelimiter.Tab).AddTo(DisposableList);
            IsOtherDelimiter = Delimiter.Select(x => x == RowDelimiter.Others)
                                        .ToReadOnlyReactivePropertySlim()
                                        .AddTo(DisposableList);
            OtherDelimiter = new ReactivePropertySlim<string>().AddTo(DisposableList);
            HasHeader = new ReactivePropertySlim<bool>(true).AddTo(DisposableList);

            LoadCommand = new AsyncReactiveCommand<InputFileChangeEventArgs>().WithSubscribe(Load)
                                                                              .AddTo(DisposableList);
        }

        /// <summary>
        /// 使用する行区切り文字を取得します。
        /// </summary>
        /// <returns>使用する行区切り文字</returns>
        private string GetDelimiter()
        {
            return Delimiter.Value switch
            {
                RowDelimiter.Tab => "\t",
                RowDelimiter.Comma => ",",
                RowDelimiter.Others => OtherDelimiter.Value,
                _ => throw new InvalidOperationException(),
            };
        }

        /// <summary>
        /// ファイルを読み込みます。
        /// </summary>
        /// <param name="e">読み込むファイルのデータ</param>
        private async Task Load(InputFileChangeEventArgs e)
        {
            if (e.FileCount != 1) return;
            string delimiter = GetDelimiter();
            if (string.IsNullOrEmpty(delimiter))
            {
                notificationService.Notify(NotificationSeverity.Warning, "行区切り文字を指定してください");
                return;
            }

            try
            {
                using Stream stream = e.File.OpenReadStream();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                await mainModel.LoadFile(e.File.Name, memoryStream, new TextLoadOptions(HasHeader.Value, delimiter));
            }
            catch
            {
                notificationService.Notify(NotificationSeverity.Error, "ファイル読み込み時にエラーが発生しました");
                return;
            }
            NavigateTo("edit");
        }
    }
}