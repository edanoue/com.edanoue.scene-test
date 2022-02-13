#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

using Edanoue.SceneTest.Interfaces;

namespace Edanoue.SceneTest
{
    internal class SceneTestRunner : ISceneTestRunner
    {
        #region Constructor

        internal SceneTestRunner(ISceneTestCaseCollecter sceneTestCaseCollecter)
        {
            _sceneTestCaseCollecter = sceneTestCaseCollecter;
        }

        #endregion

        IEnumerator ISceneTestRunner.RunAll(RunnerOptions? inOptions = null)
        {
            if (_isRunning)
            {
                Debug.LogWarning("Already Running");
                yield break;
            }

            // テストの開始
            _isRunning = true;

            // 見つかっている全てのテストを実行する
            foreach (var testcase in _sceneTestCaseCollecter.TestCases)
            {
                testcase.OnRun();
            }

            Debug.Log($"Start to running test with {this.GetType().Name}");

            // オプションが指定されていないならデフォルトのものを用意する
            // 南: なんとなく Global のタイムアウトは 10秒 としています
            RunnerOptions options = inOptions is null ? new RunnerOptions(10.0f) : inOptions.Value;

            string optionsStr = "------------------\n";
            optionsStr += "options\n";
            optionsStr += "------------------\n";
            optionsStr += $"+ globalTimeOutSeconds: {options.GlobalTimeoutSeconds}\n";
            optionsStr += "\n";
            Debug.Log(optionsStr);

            // 無限ループ防止のタイマーをセットアップ
            var _timeoutSeconds = options.GlobalTimeoutSeconds;
            // 負の値を防止するために, 0.001 秒を最低値として設定しておく
            _timeoutSeconds = Mathf.Max(_timeoutSeconds, 0.001f);
            var _globalTimeoutTimer = new WaitForSecondsRealtime(_timeoutSeconds);

            // ローカルのタイムアウト確認用のタイマーのマップを作成しておく
            Dictionary<ISceneTestCase, IEnumerator> _localTimeoutTimerMap = new();

            while (_globalTimeoutTimer.MoveNext())
            {
                // キャンセルの命令が来た場合はループを抜ける
                if (_isReceivedCacheRequest)
                {
                    // 現時点で実行中のテストにキャンセル命令をだす
                    foreach (var test in _sceneTestCaseCollecter.TestCases.Where(x => x.IsRunning))
                    {
                        test.OnCancel();
                    }

                    // ループを抜ける
                    break;
                }

                // 実行中のテストケース の タイムアウトを確認する
                foreach (var test in _sceneTestCaseCollecter.TestCases.Where(x => x.IsRunning))
                {
                    // まだタイマーが作成されていなかったら作成する
                    if (!_localTimeoutTimerMap.ContainsKey(test))
                    {
                        var _localTimeoutSeconds = test.Options.LocalTimeoutSeconds;
                        // 負の値を防止するために, 0.001 秒を最低値として設定しておく
                        _localTimeoutSeconds = Mathf.Max(_localTimeoutSeconds, 0.001f);
                        _localTimeoutTimerMap.Add(test, new WaitForSecondsRealtime(_localTimeoutSeconds));
                    }

                    if (_localTimeoutTimerMap.TryGetValue(test, out var timer))
                    {
                        // タイマーを進めて 終了を判定する
                        if (!timer.MoveNext())
                        {
                            // TestCase 側でタイムアウトしたため, OnTimeout を実行する
                            test.OnTimeout();
                        }
                    }
                }

                // まだ完了していないテストがあるかどうかを確認するフラグ
                bool isAnyTestRunning = _sceneTestCaseCollecter.TestCases.Count(x => x.IsRunning) > 0;
                if (isAnyTestRunning)
                {
                    // 1フレーム待機して
                    yield return null;
                    // ループを続行する
                    continue;
                }
                // すべてのテストが終了した
                else
                {
                    // ループを抜ける
                    break;
                }
            }

            // まだ終了していないテストはタイムアウトとする
            foreach (var test in _sceneTestCaseCollecter.TestCases.Where(x => x.IsRunning))
            {
                test.OnTimeout();
            }

            Debug.Log("Completed to test!");

            // Test Report を収集しておく
            /*
            _lastRunningTestReports.Clear();
            foreach (var test in _sceneTestCaseCollecter.TestCases)
            {
                var report = test.Report;

                // Custon Info に ゲームオブジェクト情報も入れておく
                if (test is MonoBehaviour mb)
                {
                    // report.CustomInfos.Add("GameObject", mb.gameObject.name);
                }

                // Custom Info に タイムアウト情報も入れておく
                {
                    var globalTimeoutSec = _timeoutSeconds;
                    var localTimeoutSec = Mathf.Max(test.Options.LocalTimeoutSeconds, 0.001f);

                    if (globalTimeoutSec > localTimeoutSec)
                    {
                        // report.CustomInfos.Add("timeoutSec", $"{localTimeoutSec} (local)");
                    }
                    else
                    {
                        // report.CustomInfos.Add("timeoutSec", $"{globalTimeoutSec} (global)");
                    }
                }

                _lastRunningTestReports.Add(report);
            }
            */

            // テスト実行の終了
            _isRunning = false;
        }

        IEnumerator ISceneTestRunner.Run(string[] ids, RunnerOptions? options = null)
        {
            yield return null;
        }

        /// <summary>
        /// テストケースの実行をキャンセルする
        /// </summary>
        void ISceneTestRunner.Cancel()
        {
            _isReceivedCacheRequest = true;
        }

        /// <summary>
        /// 直近で実行したテストの実行結果のレポートのリストを取得
        /// </summary>
        /// <value></value>
        IEnumerable<ITestResult> ISceneTestRunner.LatestReports
        {
            get
            {
                return new ITestResult[0];
            }
        }

        #region Helpers

        readonly ISceneTestCaseCollecter _sceneTestCaseCollecter;
        bool _isRunning;
        bool _isReceivedCacheRequest;

        #endregion
    }
}