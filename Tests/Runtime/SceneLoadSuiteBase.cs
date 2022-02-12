using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Edanoue.SceneTest;

namespace SceneLoadSuiteBase関連
{
    class シーンロード関連のテストケース : SceneLoadSuiteBase
    {
        protected override string ScenePath => $"{ScriptDir()}/Scenes/__EmptyScene.unity";

        [UnityTest]
        public IEnumerator LoadTestSceneAsyncTest()
        {
            // シーンのロードを行う
            yield return LoadTestSceneAsync();

            // シーンが読み込まれているかどうかを確認するため, パスから Scene を取得する
            var scene = SceneManager.GetSceneByPath(ScenePath);

            // この scene が有効な場合はすでにロードされている
            Assert.That(scene.IsValid(), Is.True);

            // アンロードしておく
            yield return SceneManager.UnloadSceneAsync(ScenePath);
        }

        [UnityTest]
        public IEnumerator UnloadTestSceneAsyncTest()
        {
            // シーンのロードを行う
            // この関数が成功することは他のテストにより保証されている
            yield return LoadTestSceneAsync();

            // シーンのアンロードを行う
            yield return UnloadTestSceneAsync();

            // シーンが読み込まれているかどうかを確認するため, パスから Scene を取得する
            var scene = SceneManager.GetSceneByPath(ScenePath);

            // アンロードされていることを確認する
            Assert.That(scene.IsValid(), Is.False);
        }

        [UnityTest]
        public IEnumerator LoadTestSceneAsyncで重複してシーンが読み込まれない()
        {
            // 現在のシーンのカウント数を取得しておく
            int oldSceneCount = SceneManager.sceneCount;

            // シーンのロードを行う
            yield return LoadTestSceneAsync();
            // 重複してロード関数を実行する
            yield return LoadTestSceneAsync();

            // シーンは一つしか読み込まれていない
            int newSceneCount = SceneManager.sceneCount;
            Assert.That(newSceneCount, Is.EqualTo(oldSceneCount + 1));

            // アンロードしておく
            yield return UnloadTestSceneAsync();
        }
    }

    class 空のシーンが指定されたテストケース : SceneLoadSuiteBase
    {
        protected override string ScenePath => $"{ScriptDir()}/Scenes/__EmptyScene.unity";

        [UnityTest]
        public IEnumerator テスト実行時にRunnerが存在しないシーンなら自動で生成が行われる()
        {
            // シーンのロードを行う
            yield return LoadTestSceneAsync();

            // 現在指定しているシーンは空のシーンのため, Test Runner が存在していない
            // Run を実行すると Runner が自動で生成される
            // Coroutine で生成, 実行, 破棄まで一気に行ってしまうため
            // ログが出現したかどうかを確認する
            LogAssert.Expect(LogType.Log, "Created new TestRunner");
            LogAssert.Expect(LogType.Log, "Destroyed TestRunner");
            yield return RunTestAsync(isAutoLoadUnload: false);

            // アンロードしておく
            yield return UnloadTestSceneAsync();
        }

        [UnityTest]
        public IEnumerator 何もTestCaseが配置されていないシーンならワーニングが出る()
        {
            // シーンのロードを行う
            yield return LoadTestSceneAsync();

            // 現在指定しているシーンは空のシーンのため なにもテストが実行されない
            // このばあい, Log に Warning がでていることを確認する
            LogAssert.Expect(LogType.Warning, $"Not founded {nameof(ISceneTestCase)} implemented components. skipped testing.");
            yield return RunTestAsync(isAutoLoadUnload: false);

            // アンロードしておく
            yield return UnloadTestSceneAsync();
        }
    }

    // 南: このテスト動かしたかったのですが, Editor にある SceneManager の LoadScene に失敗したときのエラーハンドリングが良くわからなかったのでコメントアウトしています
    //     一応実際にエラーが出ることは確認しています
    // class ScenePathを指定していない場合Run時にエラーが出る : SceneLoadSuiteBase
    // {
    //     protected override string ScenePath => "INVALID_PATH";

    //     [UnityTest]
    //     public IEnumerator ScenePathを指定していない場合Run時にエラーが出るテスト()
    //     {
    //         LogAssert.Expect(LogType.Error, "[Error] LoadSceneAsyncInPlayMode expects a valid full path. The provided path was 'INVALID_PATH'");
    //         // シーン内テストを実行する
    //         yield return RunTestAsync();
    //     }
    // }
}
