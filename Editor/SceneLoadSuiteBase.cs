// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable

using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Edanoue.SceneTest
{
    /// <summary>
    /// </summary>
    public abstract class SceneLoadSuiteBase
    {
        /// <summary>
        /// ロードするテスト用のシーンのパス (Assets/foo/bar.unity という形式で指定すること)
        /// </summary>
        /// <value></value>
        protected abstract string ScenePath { get; }

        private bool IsLoadedTestScene
        {
            get
            {
                // シーンが読み込まれているかどうかを確認するため, パスから Scene を取得する
                var scene = SceneManager.GetSceneByPath(ScenePath);

                // この scene が有効な場合はすでにロードされている
                return scene.IsValid();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        protected static string ScriptDir([CallerFilePath] string fileName = "")
        {
            var directoryName = Path.GetDirectoryName(fileName);
            const string pattern = @"Assets[\\/].+";
            var match = Regex.Match(directoryName, pattern);
            return match.Success ? match.Value : "";
        }

        protected virtual IEnumerator LoadTestSceneAsync()
        {
            // すでにシーンがロード済みの場合は処理をスキップする
            if (IsLoadedTestScene)
            {
                Debug.Log($"Already loaded scene: {ScenePath}");
                yield break;
            }

            // テスト用のシーンを加算ロードする
            var loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Additive);

            // シーンロードを行う
            // この際 Active Scene は InitTestScene のまま
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(ScenePath, loadSceneParameters);

            if (IsLoadedTestScene)
            {
                Debug.Log($"Loaded test scene: {ScenePath}");
            }
        }

        protected virtual IEnumerator UnloadTestSceneAsync()
        {
            // まだシーンが読み込まれていない場合は処理をスキップする
            if (!IsLoadedTestScene)
            {
                Debug.Log($"Already unloaded scene: {ScenePath}");
                yield break;
            }

            // 加算ロードしたテスト用のシーンを破棄する
            yield return SceneManager.UnloadSceneAsync(ScenePath);

            if (!IsLoadedTestScene)
            {
                Debug.Log($"Unloaded test scene: {ScenePath}");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="timeoutSeconds"></param>
        /// <param name="isAutoLoadUnload"></param>
        /// <returns></returns>
        protected virtual IEnumerator RunTestAsync(RunnerOptions? options = null, bool isAutoLoadUnload = true)
        {
            // 自動ロード・アンロードのオプションが指定されていたら シーンのロードを行う
            if (isAutoLoadUnload)
            {
                yield return LoadTestSceneAsync();
            }

            // シーン内にあるテストランナーを検索して実行する
            // 最初に見つかったもの
            ITestRunner? runner = null;
            GameObject? createdRunnerGo = null;

            // ITestRunner 実装コンポーネントをすべて検索する
            foreach (var r in Object.FindObjectsOfType<MonoBehaviour>().OfType<ITestRunner>())
            {
                runner = r;
                break;
            }

            // もし見つからなかったら新規で作成する
            if (runner == null)
            {
                createdRunnerGo = new GameObject("__runner__");
                runner = createdRunnerGo.AddComponent<TestRunner>();
                Debug.Log("Created new TestRunner");
            }

            // テストを実行する
            yield return runner.Run(options);

            // テスト結果を取得しておく
            var reports = runner.LatestReports;

            // runner を自動生成しているならば手動で削除する
            if (createdRunnerGo != null)
            {
                Object.DestroyImmediate(createdRunnerGo);
                Debug.Log("Destroyed TestRunner");
            }

            // 自動ロード・アンロードのオプションが指定されていたら シーンのアンロードを行う
            if (isAutoLoadUnload)
            {
                yield return UnloadTestSceneAsync();
            }

            // テストレポートの表示 を行う
            var reportsStr = "";
            reportsStr += "==========================\n";
            reportsStr += "        Test Report       \n";
            reportsStr += "==========================\n";

            foreach (var report in reports)
            {
                reportsStr += $"{report.Name}: {report.Status}\n";
                reportsStr += $"msg: {report.Message}\n";
                foreach (var pair in report.CustomInfos)
                {
                    reportsStr += $"{pair.Key}: {pair.Value}\n";
                }

                reportsStr += "--------------------------\n";
            }

            Debug.Log(reportsStr);

            foreach (var report in reports)
            {
                // テストが失敗していた時
                if (report.Status == SceneTestStatus.Failed)
                {
                    // TODO: まとめてログ出したいよね
                    var message = $"{report.Name}: {report.Message}";
                    Assert.Fail(message);
                }
            }
        }
    }
}