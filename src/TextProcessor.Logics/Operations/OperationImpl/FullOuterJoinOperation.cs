﻿using System;
using System.Collections.Generic;
using System.Linq;
using TextProcessor.Logics.Data;

namespace TextProcessor.Logics.Operations.OperationImpl
{
    /// <summary>
    /// 完全外部結合を行う処理のクラスです。
    /// </summary>
    [Serializable]
    internal class FullOuterJoinOperation : Operation
    {
        private int keyIndex;
        private TextData? target;
        private int targetKeyIndex;
        private bool caseSensitive = true;
        private bool removeKey;

        /// <inheritdoc/>
        public override string Title => "他データと完全外部結合";

        /// <summary>
        /// <see cref="FullOuterJoinOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        public FullOuterJoinOperation()
        {
        }

        /// <summary>
        /// <see cref="FullOuterJoinOperation"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="cloned">複製するインスタンス</param>
        /// <remarks>クローン用</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="cloned"/>が<see langword="null"/></exception>
        protected FullOuterJoinOperation(FullOuterJoinOperation cloned)
            : base(cloned)
        {
            keyIndex = cloned.keyIndex;
            target = cloned.target;
            targetKeyIndex = cloned.targetKeyIndex;
            caseSensitive = cloned.caseSensitive;
        }

        /// <inheritdoc/>
        protected override Operation CloneCore() => new FullOuterJoinOperation(this);

        /// <inheritdoc/>
        protected override IList<ArgumentInfo> GenerateArguments()
        {
            return new[]
            {
                new ArgumentInfo(ArgumentType.Index, "キーの列番号", () => keyIndex, x => keyIndex = x),
                new ArgumentInfo(ArgumentType.TextData, "結合するファイル", () => target!, x => target = x),
                new ArgumentInfo(ArgumentType.Index, "キーの列番号", () => targetKeyIndex, x => targetKeyIndex = x),
                new ArgumentInfo(ArgumentType.Boolean, "大文字小文字の区別", () => caseSensitive, x => caseSensitive = x),
                new ArgumentInfo(ArgumentType.Boolean, "キー列を削除する", () => removeKey, x => removeKey = x),
            };
        }

        /// <inheritdoc/>
        protected override void VerifyArgumentsCore(ProcessStatus status)
        {
            if (target is null) status.Errors.Add(new StatusEntry(Title, Arguments[1], "結合するファイルが指定されていません"));
        }

        /// <inheritdoc/>
        protected override void OperateCore(TextData data, ProcessStatus status)
        {
            if (target is null) return;

            IEqualityComparer<string> comparer = caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
            List<List<string>> dataList = data.GetSourceData();
            List<List<string>> targetList = target.GetSourceData();
            int joinOffset = data.ColumnCount;
            int dataStart = data.HasHeader ? 1 : 0;
            int targetStart = target.HasHeader ? 1 : 0;

            if (targetStart == 1)
            {
                List<string> dataHeader;
                List<string> targetHeader = targetList[0].ToList();
                if (targetKeyIndex < targetHeader.Count) targetHeader.RemoveAt(targetKeyIndex);
                if (dataStart == 0)
                {
                    dataHeader = new List<string>(joinOffset + targetHeader.Count);
                    dataList.Insert(0, dataHeader);
                    var blankArray = new string[joinOffset];
                    Array.Fill(blankArray, string.Empty);

                    dataHeader.AddRange(blankArray);
                    dataHeader.AddRange(targetHeader);
                    data.HasHeader = true;
                    dataStart++;
                }
                else
                {
                    dataHeader = dataList[0];
                    dataHeader.EnsureCapacity(joinOffset + targetHeader.Count);
                    if (dataHeader.Count < joinOffset)
                    {
                        int blankCount = joinOffset - dataHeader.Count;
                        var blankArray = new string[blankCount];
                        Array.Fill(blankArray, string.Empty);

                        dataHeader.AddRange(blankArray);
                    }
                }
                dataHeader.AddRange(targetHeader);
            }

            var keys = new HashSet<string>(dataList.Count + targetList.Count, caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
            for (int i = dataStart; i < dataList.Count; i++)
            {
                List<string> dataRow = dataList[i];
                keys.Add(keyIndex < dataRow.Count ? dataRow[keyIndex] : string.Empty);
            }
            for (int i = targetStart; i < targetList.Count; i++)
            {
                List<string> targetRow = targetList[i];
                keys.Add(targetKeyIndex < targetRow.Count ? targetRow[targetKeyIndex] : string.Empty);
            }

            List<List<string>> resultList = dataList.Skip(dataStart)
                                                    .GroupJoin(targetList.Skip(targetStart), x => keyIndex < x.Count ? x[keyIndex] : string.Empty, x => targetKeyIndex < x.Count ? x[targetKeyIndex] : string.Empty, (x, y) => y.DefaultIfEmpty().Select(z => CombineRow(x, z, joinOffset, keys)), comparer)
                                                    .SelectMany(x => x)
                                                    .ToList();

            for (int i = targetStart; i < targetList.Count; i++)
            {
                List<string> targetRow = targetList[i].ToList();
                if (targetKeyIndex >= targetRow.Count) continue;
                string key = targetRow[targetKeyIndex];
                if (!keys.Remove(key)) continue;
                targetRow.RemoveAt(targetKeyIndex);
                var appended = new List<string>(joinOffset + targetRow.Count);
                for (int j = 0; j < joinOffset; j++) appended.Add(string.Empty);
                if (!removeKey) appended[keyIndex] = key;
                appended.InsertRange(joinOffset, targetRow);
                resultList.Add(appended);
            }

            if (data.HasHeader) dataList.RemoveRange(1, dataList.Count - 1);
            else dataList.Clear();
            dataList.AddRange(resultList);

            if (removeKey)
                for (int rowIndex = 0; rowIndex < dataList.Count; rowIndex++)
                {
                    List<string> row = dataList[rowIndex];
                    if (keyIndex < row.Count) row.RemoveAt(keyIndex);
                }
        }

        /// <summary>
        /// 列を結合します。
        /// </summary>
        /// <param name="outer">外部セットの列</param>
        /// <param name="inner">完全外部セットの列</param>
        /// <param name="joinOffset"><paramref name="inner"/>の値を挿入する位置</param>
        /// <param name="keys">キー一覧</param>
        /// <returns>結合後の列</returns>
        private List<string> CombineRow(List<string> outer, List<string>? inner, int joinOffset, HashSet<string> keys)
        {
            List<string> outerList = outer.ToList();
            if (inner is null) return outerList;
            List<string> innerList = inner.ToList();
            if (targetKeyIndex < inner.Count) innerList.RemoveAt(targetKeyIndex);
            keys.Remove(outerList[keyIndex]);
            outerList.EnsureCapacity(outerList.Count + innerList.Count);
            if (outerList.Count < joinOffset)
            {
                int blankCount = joinOffset - outerList.Count;
                var blankArray = new string[blankCount];
                Array.Fill(blankArray, string.Empty);
                outerList.AddRange(blankArray);
            }
            outerList.AddRange(innerList);
            return outerList;
        }
    }
}
