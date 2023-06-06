// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private readonly List<ITestCase> _foundedTestCases = new();

        private readonly List<ITestReport> _lastRunningTestReports = new();

        private bool _bReceivedCacheRequest;
        private bool _isRunning;


        IEnumerator ITestRunner.Run(RunnerOptions? options)
        {
            return RunInternal(options);
        }

        void ITestRunner.Cancel()
        {
            Cancel();
        }

        List<ITestReport> ITestRunner.LatestReports => _lastRunningTestReports;

        private IEnumerator RunInternal(RunnerOptions? inOptions)
        {
            // すでに Runner が実行中なので抜ける
            if (_isRunning)
            {
                Debug.LogWarning("[EdaSceneTestRunner] Already Running", this);
                yield break;
            }

            // ロード中のシーン全てから TestCase を収集する
            FetchAllITestBehaviour();

            switch (_foundedTestCases.Count)
            {
                // 何もキャッシュにないので実行しない
                case 0:
                {
                    Debug.LogWarning(
                        $"[EdaSceneTestRunner] Not founded {nameof(ITestCase)} implemented components. skipped testing.",
                        this);
                    yield break;
                }
                case 1:
                {
                    Debug.Log("[EdaSceneTestRunner] Founded 1 test in Scene", this);
                    break;
                }
                default:
                {
                    Debug.Log($"[EdaSceneTestRunner] Founded {_foundedTestCases.Count} tests in Scene", this);
                    break;
                }
            }

            // テストの開始
            _isRunning = true;
            foreach (var testcase in _foundedTestCases)
            {
                testcase.OnRun();
            }

            var sb = new StringBuilder();
            sb.Append("[EdaSceneTestRunner] Start to tests ...\n");

            // オプションが指定されていないならデフォルトのものを用意する
            // 南: なんとなく Global のタイムアウトは 10秒 としています
            var options = inOptions ?? new RunnerOptions(10.0f);

            // 無限ループ防止のタイマーをセットアップ
            double globalTimeLimit = options.GlobalTimeoutSeconds;
            // 負の値を防止するために, 0.001 秒を最低値として設定しておく
            globalTimeLimit = Math.Max(globalTimeLimit, 0.001d);

            sb.Append($"\t+ globalTimeOutSeconds: {options.GlobalTimeoutSeconds}\n");
            Debug.Log(sb.ToString());

            // ローカルのタイムアウト確認用のタイマーのマップを作成しておく
            Dictionary<ITestCase, double> localTimeoutTimerMap = new();

            var firstFrame = true;
            var timeSinceLevelLoadAsOffset = Time.timeAsDouble;

            while (true)
            {
                // 最初のフレームはいろんなものが立ち上がっていて遅いのでスキップする
                if (firstFrame)
                {
                    // 最初の 1f は待機する
                    yield return null;

                    // 現在の時間を タイマーの Offset として保存しておく
                    timeSinceLevelLoadAsOffset = Time.timeAsDouble;
                    firstFrame = false;

                    // API のアクションを実行する
                    EdaSceneTestStatus._onStartEdaSceneTest?.Invoke();

                    continue;
                }

                // キャンセルの命令が来た場合はループを抜ける
                if (_bReceivedCacheRequest)
                {
                    // 現時点で実行中のテストにキャンセル命令をだす
                    foreach (var test in _foundedTestCases.Where(x => x.IsRunning))
                    {
                        test.OnCancel();
                    }

                    // ループを抜ける
                    break;
                }

                // テストの実行時間を計算
                var timeSinceRunTest = Time.timeAsDouble - timeSinceLevelLoadAsOffset;

                // Global の Timeout に達した場合はループを抜ける
                if (timeSinceRunTest > globalTimeLimit)
                {
                    break;
                }

                // 実行中のテストケース の タイムアウトを確認する
                foreach (var test in _foundedTestCases.Where(x => x.IsRunning))
                {
                    // まだタイマーが作成されていなかったら作成する
                    if (!localTimeoutTimerMap.ContainsKey(test))
                    {
                        double localTimeoutSeconds = test.Options.LocalTimeoutSeconds;
                        // 負の値を防止するために, 0.001 秒を最低値として設定しておく
                        localTimeoutSeconds = Math.Max(localTimeoutSeconds, 0.001d);
                        localTimeoutTimerMap.Add(test, localTimeoutSeconds);
                    }

                    if (!localTimeoutTimerMap.TryGetValue(test, out var localTimeLimit))
                    {
                        continue;
                    }

                    // タイマーを進めて 終了を判定する
                    if (timeSinceRunTest > localTimeLimit)
                    {
                        // TestCase 側でタイムアウトしたため, OnTimeout を実行する
                        test.OnTimeout();
                    }
                }

                // まだ完了していないテストがある場合は次のフレームに
                if (_foundedTestCases.Any(x => x.IsRunning))
                {
                    // 1フレーム待機する
                    yield return null;
                    // ループを続行する
                    continue;
                }

                // すべてのテストが終了したためループを抜ける
                break;
            }

            // まだ終了していないテストはタイムアウトとする (Global の Time limit に達したバアー)
            foreach (var test in _foundedTestCases.Where(x => x.IsRunning))
            {
                test.OnTimeout();
            }

            Debug.Log("[EdaSceneTestRunner] ... Completed to test!", this);

            // Test Report を収集しておく
            _lastRunningTestReports.Clear();
            foreach (var test in _foundedTestCases)
            {
                var report = test.Report;

                // Custom Info に ゲームオブジェクト情報も入れておく
                if (test is MonoBehaviour mb)
                {
                    report.CustomInfos.Add("GameObject", mb.gameObject.name);
                }

                // Custom Info に タイムアウト情報も入れておく
                {
                    var localTimeoutSec = Mathf.Max(test.Options.LocalTimeoutSeconds, 0.001f);

                    if (globalTimeLimit > localTimeoutSec)
                    {
                        report.CustomInfos.Add("timeoutSec", $"{localTimeoutSec} (local)");
                    }
                    else
                    {
                        report.CustomInfos.Add("timeoutSec", $"{globalTimeLimit} (global)");
                    }
                }

                _lastRunningTestReports.Add(report);
            }

            // テスト実行の終了
            _isRunning = false;
            // GC のためにキャッシュを空にしておく
            _foundedTestCases.Clear();
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
            _foundedTestCases.Clear();

            // ロードしているすべてのシーン内から ITestBehaviour 実装コンポーネントを検索する
            var ss = FindObjectsOfType<MonoBehaviour>().OfType<ITestCase>();
            foreach (var s in ss)
            {
                // キャッシュに追加する
                if (!_foundedTestCases.Contains(s))
                {
                    _foundedTestCases.Add(s);
                }
            }
        }
    }
}