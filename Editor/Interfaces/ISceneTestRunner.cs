#nullable enable

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

namespace Edanoue.SceneTest.Interfaces
{
    public interface ISceneTestRunner
    {
        IEnumerator RunAll(RunnerOptions? options = null);

        IEnumerator Run(string[] ids, RunnerOptions? options = null);

        /// <summary>
        /// テストケースの実行をキャンセルする
        /// </summary>
        void Cancel();

        /// <summary>
        /// 直近で実行したテストの実行結果のレポートのリストを取得
        /// </summary>
        /// <value></value>
        IEnumerable<ITestResult> LatestReports { get; }
    }
}
