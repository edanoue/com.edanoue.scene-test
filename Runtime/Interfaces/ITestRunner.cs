#nullable enable

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace Edanoue.SceneTest
{
    public interface ITestRunner
    {
        /// <summary>
        /// シーン内にあるすべてのテストケースを開始して待機する
        /// </summary>
        /// <returns></returns>
        IEnumerator Run(RunnerOptions? options = null);

        /// <summary>
        /// テストケースの実行をキャンセルする
        /// </summary>
        void Cancel();

        /// <summary>
        /// 直近で実行したテストの実行結果のレポートのリストを取得
        /// </summary>
        /// <value></value>
        List<ITestResult> LatestReports { get; }
    }
}
