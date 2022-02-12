#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.TestTools;

namespace Edanoue.SceneTest
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class UnitySceneTestAttribute : NUnitAttribute, ITestBuilder, IImplyFixture, IOuterUnityTestAction
    {
        static readonly Dictionary<string, List<TestMethod>> s_map;

        #region Constructors

        static UnitySceneTestAttribute()
        {
            s_map = new();
            Debug.Log("static const");
        }

        public UnitySceneTestAttribute(string scenePath)
        {
            this._scenePath = scenePath;
            if (!s_map.ContainsKey(scenePath))
            {
                s_map.Add(scenePath, new List<TestMethod>());
            }
        }

        #endregion

        IEnumerable<TestMethod> ITestBuilder.BuildFrom(IMethodInfo method, Test suite)
        {
            if (s_map.TryGetValue(_scenePath, out var tests))
            {
                if (tests.Count > 0)
                {
                    return tests;
                }
            }
            TestMethod initTestMethod = new TestMethod(method, suite);
            initTestMethod.Name = "(実行前のため ケースの収集ができません)";
            return new TestMethod[1] { initTestMethod };
        }

        #region UnityEngine.TestTools.IOuterUnityTestAction impls

        IEnumerator IOuterUnityTestAction.BeforeTest(ITest test)
        {
            // 指定されたシーンのロードを行う
            yield return LoadTestSceneAsync();

            // シーン内にあるテストランナーを検索して実行する
            // 最初に見つかったもの
            ISceneTestRunner? runner = null;
            GameObject? createdRunnerGo = null;

            // ITestRunner 実装コンポーネントをすべて検索する
            foreach (var r in GameObject.FindObjectsOfType<MonoBehaviour>().OfType<ISceneTestRunner>())
            {
                runner = r;
                break;
            }

            // もし見つからなかったら新規で作成する
            if (runner == null)
            {
                createdRunnerGo = new GameObject("__runner__");
                runner = createdRunnerGo.AddComponent<SceneTest.SceneTestRunner>();
                Debug.Log("Created new TestRunner");
            }

            // シーン内に存在する TestCase から TestMethod を生成してキャッシュに置く
            if (s_map.TryGetValue(_scenePath, out var tests))
            {
                // 前回のキャッシュを削除する
                tests.Clear();

                // 現在読込中のシーンから ISceneTestCase 実装コンポーネントをすべて検索する
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
            }

            // Suite の方に記述されてる PropertieBag にアクセスする
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

            // テストを実行する
            yield return runner.Run(new RunnerOptions(timeoutSec));

            // runner を自動生成しているならば手動で削除する
            if (createdRunnerGo != null)
            {
                GameObject.DestroyImmediate(createdRunnerGo);
                Debug.Log("Destroyed TestRunner");
            }

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
            Debug.Log(reportsStr);
        }

        IEnumerator IOuterUnityTestAction.AfterTest(ITest test)
        {
            // 指定されたシーンのアンロードを行う
            yield return UnloadTestSceneAsync();

            // テストレポートの表示を行う
        }

        #endregion

        private readonly string _scenePath;

        private bool IsLoadedTestScene
        {
            get
            {
                // シーンが読み込まれているかどうかを確認するため, パスから Scene を取得する
                var scene = SceneManager.GetSceneByPath(_scenePath);

                // この scene が有効な場合はすでにロードされている
                return scene.IsValid();
            }
        }

        private IEnumerator LoadTestSceneAsync()
        {
            // すでにシーンがロード済みの場合は処理をスキップする
            if (IsLoadedTestScene)
            {
                Debug.LogWarning($"Already loaded scene: {_scenePath}. skip load");
                yield break;
            }

            // テスト用のシーンを加算ロードする
            var loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Additive);

            // シーンロードを行う
            // この際 Active Scene は InitTestScene のまま
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(_scenePath, loadSceneParameters);

            if (IsLoadedTestScene)
            {
                Debug.Log($"Loaded test scene: {_scenePath}");
            }
        }

        private IEnumerator UnloadTestSceneAsync()
        {
            // まだシーンが読み込まれていない場合は処理をスキップする
            if (!IsLoadedTestScene)
            {
                Debug.LogWarning($"Already unloaded scene: {_scenePath}. skip unload");
                yield break;
            }

            // 加算ロードしたテスト用のシーンを破棄する
            yield return SceneManager.UnloadSceneAsync(_scenePath);

            if (!IsLoadedTestScene)
            {
                Debug.Log($"Unloaded test scene: {_scenePath}");
            }
        }

    }
}