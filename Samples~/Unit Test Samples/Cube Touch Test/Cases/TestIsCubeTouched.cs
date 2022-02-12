#nullable enable
#if UNITY_EDITOR

using UnityEngine;
using Edanoue.SceneTest;

/// <summary>
/// シーン内にあるキューブが触れたら成功するテスト
/// </summary>
class TestIsCubeTouched : SceneTestCaseBase
{
    void OnCollisionEnter(Collision _)
    {
        Success("キューブが触れました");
    }
}

#endif