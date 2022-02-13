// Copyright Edanoue, Inc. MIT License - see LICENSE.md

#nullable enable
#if UNITY_EDITOR

using UnityEngine;
using Edanoue.SceneTest;

/// <summary>
/// シーン内にあるキューブが触れたら成功するテスト
/// </summary>
class TestIsCubeTouched : SceneTestCaseBehaviour
{
    void OnCollisionEnter(Collision _)
    {
        Success("キューブが触れました");
    }
}

#endif