#nullable enable

using System.Collections;
using System.Collections.Generic;

namespace Edanoue.TestAPI
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
        List<ITestReport> LatestReports { get; }
    }
}
