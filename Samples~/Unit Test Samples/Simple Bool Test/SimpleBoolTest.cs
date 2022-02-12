#nullable enable

#if UNITY_EDITOR

using System.Collections;
using Edanoue.SceneTest;

class SimpleBoolTest : SceneLoadSuiteBase
{
    [UnitySceneTest("Assets/com.edanoue.scenetest/Samples/Unit Test Samples/Simple Bool Test/SceneTest_SimpleBoolTest.unity")]
    public IEnumerator RunSceneTest()
    {
        yield return RunTestAsync();
    }
}

#endif
