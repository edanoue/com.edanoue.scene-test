using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Edanoue.SceneTest;

namespace SceneLoadSuiteBase関連
{
    class 空のシーンが指定されたテストケース : SceneLoadSuiteBase
    {
        [UnitySceneTest("Assets/com.edanoue.scenetest/Tests/Runtime/Scenes/__EmptyScene.unity")]
        public IEnumerator テスト実行時にRunnerが存在しないシーンなら自動で生成が行われる()
        {
            // 現在指定しているシーンは空のシーンのため, Test Runner が存在していない
            // Run を実行すると Runner が自動で生成される
            // Coroutine で生成, 実行, 破棄まで一気に行ってしまうため
            // ログが出現したかどうかを確認する
            LogAssert.Expect(LogType.Log, "Created new TestRunner");
            LogAssert.Expect(LogType.Log, "Destroyed TestRunner");
            yield return RunTestAsync();
        }

        [UnitySceneTest("Assets/com.edanoue.scenetest/Tests/Runtime/Scenes/__EmptyScene.unity")]
        public IEnumerator 何もTestCaseが配置されていないシーンならワーニングが出る()
        {
            // 現在指定しているシーンは空のシーンのため なにもテストが実行されない
            // このばあい, Log に Warning がでていることを確認する
            LogAssert.Expect(LogType.Warning, $"Not founded {nameof(ISceneTestCase)} implemented components. skipped testing.");
            yield return RunTestAsync();
        }
    }
}
