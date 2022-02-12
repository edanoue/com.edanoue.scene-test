#nullable enable

#if UNITY_EDITOR

using System.Collections;
using UnityEngine.TestTools;
using Edanoue.TestAPI;

class SimpleBoolTest : SceneLoadSuiteBase
{
    protected override string ScenePath => $"{ScriptDir()}/SceneTest_SimpleBoolTest.unity";

    [UnityTest]
    public IEnumerator RunSceneTest()
    {
        yield return RunTestAsync();
    }
}

#endif
