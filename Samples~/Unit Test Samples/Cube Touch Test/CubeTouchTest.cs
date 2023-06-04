// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable
#if UNITY_EDITOR

using System.Collections;
using Edanoue.SceneTest;
using UnityEngine.TestTools;

internal class CubeTouchTest : SceneLoadSuiteBase
{
    protected override string ScenePath => $"{GetCalledScriptDir()}/SceneTest_CubeTouchTest.unity";

    [UnityTest]
    public IEnumerator RunSceneTest()
    {
        // Runner の Timeout を デフォルト 10 秒から 5秒 に変える
        var options = new RunnerOptions(
            5.0f
        );

        // シーン内テストを実行する
        yield return RunTestAsync(options);
    }
}

#endif