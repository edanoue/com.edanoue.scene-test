// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Edanoue.SceneTest
{
    /// <summary>
    /// </summary>
    internal sealed class EdaSceneTestRunner : MonoBehaviour, ITestRunner
    {
        /// <summary>
        /// 開始時にキャッシュされるテストの一覧
        /// </summary>
        /// <returns></returns>
        private readonly List<ITestCase> _cachedTestCases = new();

        private readonly List<ITestReport> _lastRunningTestReports = new();

        private bool _bReceivedCacheRequest;
        private bool _isRunning;


        IEnumerator ITestRunner.Run(RunnerOptions? options)
        {
            return Run(options);
        }

        void ITestRunner.Cancel()
        {
            Cancel();
        }

        List<ITestReport> ITestRunner.LatestReports => _lastRunningTestReports;

        private IEnumerator Run(RunnerOptions? inOptions)
        {
            // すでに Runner が実行中なので抜ける
            if (_isRunning)
            {
                Debug.LogWarning("[EdaSceneTestRunner] Already Running", this);
                yield break;
            }

            // ロード中のシーン全てから TestCase を収集する
            FetchAllITestBehaviour();

            // 何もキャッシュにないので実行しない
            if (_cachedTestCases.Count == 0)
            {
                Debug.LogWarning(
                    $"[EdaSceneTestRunner] Not founded {nameof(ITestCase)} implemented components. skipped testing.",
                    this);
                yield break;
            }

            Debug.Log($"[EdaSceneTestRunner] Founding {_cachedTestCases.Count} tests in Scene", this);

            // テストの開始
            _isRunning = true;
            foreach (var testcase in _cachedTestCases)
            {
                testcase.OnRun();
            }

            Debug.Log("[EdaSceneTestRunner] Start to tests ...", this);

            // オプションが指定されていないならデフォルトのものを用意する
            // 南: なんとなく Global のタイムアウトは 10秒 としています
            var options = inOptions is null ? new RunnerOptions(10.0f) : inOptions.Value;

            var optionsStr = "------------------\n";
            optionsStr += "    Options\n";
            optionsStr += "------------------\n";
            optionsStr += $"+ globalTimeOutSeconds: {options.GlobalTimeoutSeconds}\n";
            optionsStr += "\n";
            Debug.Log(optionsStr);

            // 無限ループ防止のタイマーをセットアップ
            var timeoutSeconds = options.GlobalTimeoutSeconds;
            // 負の値を防止するために, 0.001 秒を最低値として設定しておく
            timeoutSeconds = Mathf.Max(timeoutSeconds, 0.001f);
            var globalTimeoutTimer = new WaitForSecondsRealtime(timeoutSeconds);

            // ローカルのタイムアウト確認用のタイマーのマップを作成しておく
            Dictionary<ITestCase, IEnumerator> _localTimeoutTimerMap = new();

            while (globalTimeoutTimer.MoveNext())
            {
                // キャンセルの命令が来た場合はループを抜ける
                if (_bReceivedCacheRequest)
                {
                    // 現時点で実行中のテストにキャンセル命令をだす
                    foreach (var test in _cachedTestCases.Where(x => x.IsRunning))
                    {
                        test.OnCancel();
                    }

                    // ループを抜ける
                    break;
                }

                // 実行中のテストケース の タイムアウトを確認する
                foreach (var test in _cachedTestCases.Where(x => x.IsRunning))
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
                var isAnyTestRunning = _cachedTestCases.Count(x => x.IsRunning) > 0;
                if (isAnyTestRunning)
                {
                    // 1フレーム待機する
                    yield return null;
                    // ループを続行する
                    continue;
                }
                // すべてのテストが終了した

                // ループを抜ける
                break;
            }

            // まだ終了していないテストはタイムアウトとする
            foreach (var test in _cachedTestCases.Where(x => x.IsRunning))
            {
                test.OnTimeout();
            }

            Debug.Log("[EdaSceneTestRunner] ... Completed to test!", this);

            // Test Report を収集しておく
            _lastRunningTestReports.Clear();
            foreach (var test in _cachedTestCases)
            {
                var report = test.Report;

                // Custom Info に ゲームオブジェクト情報も入れておく
                if (test is MonoBehaviour mb)
                {
                    report.CustomInfos.Add("GameObject", mb.gameObject.name);
                }

                // Custom Info に タイムアウト情報も入れておく
                {
                    var globalTimeoutSec = timeoutSeconds;
                    var localTimeoutSec = Mathf.Max(test.Options.LocalTimeoutSeconds, 0.001f);

                    if (globalTimeoutSec > localTimeoutSec)
                    {
                        report.CustomInfos.Add("timeoutSec", $"{localTimeoutSec} (local)");
                    }
                    else
                    {
                        report.CustomInfos.Add("timeoutSec", $"{globalTimeoutSec} (global)");
                    }
                }

                _lastRunningTestReports.Add(report);
            }

            // テスト実行の終了
            _isRunning = false;
            // GC のためにキャッシュを空にしておく
            _cachedTestCases.Clear();
        }

        /// <summary>
        /// ユーザーからのキャンセル命令が来た時
        /// </summary>
        public void Cancel()
        {
            _bReceivedCacheRequest = true;
        }

        private void FetchAllITestBehaviour()
        {
            // 一度キャッシュを空にする
            _cachedTestCases.Clear();

            // ロードしているすべてのシーン内から ITestBehaviour 実装コンポーネントを検索する
            var ss = FindObjectsOfType<MonoBehaviour>().OfType<ITestCase>();
            foreach (var s in ss)
            {
                // キャッシュに追加する
                if (!_cachedTestCases.Contains(s))
                {
                    _cachedTestCases.Add(s);
                }
            }
        }
    }
}