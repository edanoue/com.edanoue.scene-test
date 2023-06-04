// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable
#if UNITY_EDITOR

using Edanoue.SceneTest;
using UnityEngine;

/// <summary>
/// シーン内にあるキューブが触れたら失敗するテスト
/// Timeout したときにのみ成功扱いとする
/// Note: "Default" Layer 同士が衝突する という Project Settings を前提としています
/// </summary>
public class TestIsCubeNotTouched : EdaTestBehaviour
{
    // テスト名を C# 側で強制的に設定する
    protected override string TestName => "キューブが触れたら失敗のテスト";

    // なにか触れたら失敗とする
    private void OnCollisionEnter(Collision _)
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