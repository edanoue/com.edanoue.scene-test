// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable

#if UNITY_EDITOR

using System.Collections;
using Edanoue.SceneTest;
using UnityEngine.TestTools;

internal class SimpleBoolTest : SceneLoadSuiteBase
{
    protected override string ScenePath => $"{GetCalledScriptDir()}/SceneTest_SimpleBoolTest.unity";

    [UnityTest]
    public IEnumerator RunSceneTest()
    {
        yield return RunTestAsync();
    }
}

#endif