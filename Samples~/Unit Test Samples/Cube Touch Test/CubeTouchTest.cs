#nullable enable
#if UNITY_EDITOR

using System.Collections;
using UnityEngine.TestTools;
using Edanoue.TestAPI;

class CubeTouchTest : SceneLoadSuiteBase
{
    protected override string ScenePath => $"{ScriptDir()}/SceneTest_CubeTouchTest.unity";

    [UnityTest]
    public IEnumerator RunSceneTest()
    {
        // Runner の Timeout を デフォルト 10 秒から 5秒 に変える
        var options = new RunnerOptions(
            globalTimeoutSeconds: 5.0f
        );

        // シーン内テストを実行する
        yield return RunTestAsync(options: options);
    }
}

#endif