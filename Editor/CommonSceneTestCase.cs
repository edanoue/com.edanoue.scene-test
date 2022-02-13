#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Edanoue.SceneTest.Interfaces;

namespace Edanoue.SceneTest
{
    public static class CommonSceneTestCase
    {
        private static class Log
        {
            public static void Info(string msg)
            {
                UnityEngine.Debug.Log($"[{nameof(SceneTest)}] {msg}");
            }
            public static void Warning(string msg)
            {
                UnityEngine.Debug.LogWarning($"[{nameof(SceneTest)}] {msg}");
            }
        }

        public static IEnumerator RunTest(string sceneAbsPath)
        {
            // 指定されたシーンを読み込む
            yield return LoadTestSceneAsync(sceneAbsPath);

            // ロードされている全てのシーン内から TestCase を収集する
            ISceneTestCaseCollecter caseCollecter = new SceneTestCaseCollecter();
            if (!caseCollecter.Collect())
            {
                Log.Warning($"Not founded any {nameof(ISceneTestCase)} implemented components. skipped testing.");
                yield break;
            }

            // 見つかったテストから Nunit 用の TestMethod を生成して Json で Library 以下にシリアライズする
            /*
            foreach (var r in GameObject.FindObjectsOfType<MonoBehaviour>().OfType<ISceneTestCase>())
            {
                // 独自の TestMethod を生成する
                var suiteMethodInfo = test.Method;
                //
                var parentSuite = (Test)test.Parent;

                TestMethod testMethod = new TestMethod(test.Method, parentSuite);
                testMethod.Name = "mock";
                tests.Add(testMethod);
            }
            */

            // テストランナーを生成する
            ISceneTestRunner runner = new SceneTestRunner(caseCollecter);

            // Suite の方に記述されてる PropertieBag にアクセスする
            /*
            var testSuiteProperties = test.Parent.Properties;
            // Timeout を取得する
            // Default は 10 秒としておく
            float timeoutSec = 10f;

            // Timeout が Suite に存在したら
            if (testSuiteProperties.ContainsKey("Timeout"))
            {
                // こちらでも 設定する
                // ms が指定されているので, s に変換する
                var timeoutMs = (int)testSuiteProperties.Get("Timeout");
                timeoutSec = (float)timeoutMs / 1000f;
                // こちら側は 0.1 秒だけ短くしておく
                timeoutSec = Mathf.Max(0.01f, timeoutSec - 0.1f);
            }
            */

            // テストを実行する
            yield return runner.RunAll(new RunnerOptions(10));

            // テスト結果を取得しておく
            var reports = runner.LatestReports;

            // テストレポートの表示 を行う
            string reportsStr = "";
            reportsStr += "==========================\n";
            reportsStr += "        Test Report       \n";
            reportsStr += "==========================\n";

            foreach (var report in reports)
            {
                reportsStr += $"{report.Name}: {report.ResultState}\n";
                reportsStr += $"msg: {report.Message}\n";
                /*
                foreach (var pair in report.CustomInfos)
                {
                    reportsStr += $"{pair.Key}: {pair.Value}\n";
                }
                */
                reportsStr += $"duration: {report.Duration}\n";
                reportsStr += $"--------------------------\n";
            }
            Log.Info(reportsStr);

            // 指定されたシーンのアンロードを行う
            yield return UnloadTestSceneAsync(sceneAbsPath);
        }

        private static bool IsLoadedTestScene(string scenePath)
        {
            // シーンが読み込まれているかどうかを確認するため, パスから Scene を取得する
            var scene = SceneManager.GetSceneByPath(scenePath);

            // この scene が有効な場合はすでにロードされている
            return scene.IsValid();
        }

        private static IEnumerator UnloadTestSceneAsync(string scenePath)
        {
            // まだシーンが読み込まれていない場合は処理をスキップする
            if (!IsLoadedTestScene(scenePath))
            {
                Log.Warning($"Already unloaded scene: {scenePath}. skip unload");
                yield break;
            }

            // 加算ロードしたテスト用のシーンを破棄する
            yield return SceneManager.UnloadSceneAsync(scenePath);

            if (!IsLoadedTestScene(scenePath))
            {
                Log.Info($"Unloaded test scene: {scenePath}");
            }
        }

        private static IEnumerator LoadTestSceneAsync(string scenePath)
        {
            // すでにシーンがロード済みの場合は処理をスキップする
            if (IsLoadedTestScene(scenePath))
            {
                Log.Warning($"Already loaded scene: {scenePath}. skip load");
                yield break;
            }

            // テスト用のシーンを加算ロードする
            var loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Additive);

            // シーンロードを行う
            // この際 Active Scene は InitTestScene のまま
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(scenePath, loadSceneParameters);

            if (IsLoadedTestScene(scenePath))
            {
                Log.Info($"Loaded test scene: {scenePath}");
            }
        }
    }

}
