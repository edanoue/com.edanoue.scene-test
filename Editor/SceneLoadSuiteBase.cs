#nullable enable

using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Edanoue.SceneTest
{

    /// <summary>
    ///
    /// </summary>
    public abstract class SceneLoadSuiteBase
    {
        /// <summary>
        /// ロードするテスト用のシーンのパス (Assets/foo/bar.unity という形式で指定すること)
        /// </summary>
        /// <value></value>
        protected abstract string ScenePath { get; }

        /// <summary>
        /// このシーンの呼び出し元
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected static string ScriptDir([CallerFilePath] string fileName = "")
        {
            var absdir = Path.GetDirectoryName(fileName);
            string pattern = @"Assets[\\/].+";
            var match = Regex.Match(absdir, pattern);
            return match.Success ? match.Value : "";
        }

        bool IsLoadedTestScene
        {
            get
            {
                // シーンが読み込まれているかどうかを確認するため, パスから Scene を取得する
                var scene = SceneManager.GetSceneByPath(ScenePath);

                // この scene が有効な場合はすでにロードされている
                return scene.IsValid();
            }
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
        ///
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
            foreach (var r in GameObject.FindObjectsOfType<MonoBehaviour>().OfType<ITestRunner>())
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

            // テストを実行する
            yield return runner.Run(options);

            // テスト結果を取得しておく
            var reports = runner.LatestReports;

            // runner を自動生成しているならば手動で削除する
            if (createdRunnerGo != null)
            {
                GameObject.DestroyImmediate(createdRunnerGo);
                Debug.Log("Destroyed TestRunner");
            }

            // 自動ロード・アンロードのオプションが指定されていたら シーンのアンロードを行う
            if (isAutoLoadUnload)
            {
                yield return UnloadTestSceneAsync();
            }

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
                reportsStr += $"--------------------------\n";
            }

            Debug.Log(reportsStr);

            foreach (var report in reports)
            {
                // テストが失敗していた時
                if (report.ResultState == ResultState.Failure)
                {
                    // TODO: まとめてログ出したいよね
                    var message = $"{report.Name}: {report.Message}";
                    Assert.Fail(message);
                }
            }
        }
    }
}
