#nullable enable
#if UNITY_EDITOR

using UnityEngine;
using Edanoue.TestAPI;

/// <summary>
/// シーン内にあるキューブが触れたら成功するテスト
/// </summary>
class TestIsCubeTouched : EdaTestBehaviour
{
    void OnCollisionEnter(Collision _)
    {
        Success("キューブが触れました");
    }
}

#endif