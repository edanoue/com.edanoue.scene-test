// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable
#if UNITY_EDITOR

using Edanoue.SceneTest;
using UnityEngine;

/// <summary>
/// シーン内にあるキューブが触れたら成功するテスト
/// Note: "Default" Layer 同士が衝突する という Project Settings を前提としています
/// </summary>
internal class TestIsCubeTouched : EdaTestBehaviour
{
    private void OnCollisionEnter(Collision _)
    {
        Success("キューブが触れました");
    }
}

#endif