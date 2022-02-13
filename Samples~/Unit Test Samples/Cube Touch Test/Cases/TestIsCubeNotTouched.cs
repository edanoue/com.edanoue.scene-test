// Copyright Edanoue, Inc. MIT License - see LICENSE.md

#nullable enable
#if UNITY_EDITOR

using UnityEngine;
using Edanoue.SceneTest;

/// <summary>
/// シーン内にあるキューブが触れたら失敗するテスト
/// Timeout したときにのみ成功扱いとする
/// </summary>
public class TestIsCubeNotTouched : SceneTestCaseBehaviour
{
    // なにか触れたら失敗とする
    void OnCollisionEnter(Collision _)
    {
        Fail("キューブが触れました");
    }

    // タイムアウトしたら成功とする
    protected override void OnTimeout()
    {
        Success("キューブが触れませんでした");
    }
}

#endif
