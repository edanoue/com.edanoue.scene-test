#nullable enable
#if UNITY_EDITOR

using System.Collections;
using Edanoue.SceneTest;
using NUnit.Framework;

class CubeTouchTest : SceneLoadSuiteBase
{
    [UnitySceneTest("Assets/com.edanoue.scenetest/Samples/Unit Test Samples/Cube Touch Test/SceneTest_CubeTouchTest.unity")]
    [Timeout(1000)]
    public IEnumerator RunSceneTest()
    {
        // シーン内テストを実行する
        yield return RunTestAsync();
    }
}

#endif