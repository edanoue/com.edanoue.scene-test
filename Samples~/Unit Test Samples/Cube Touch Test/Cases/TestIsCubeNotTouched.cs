#nullable enable
#if UNITY_EDITOR

using UnityEngine;
using Edanoue.TestAPI;

/// <summary>
/// シーン内にあるキューブが触れたら失敗するテスト
/// Timeout したときにのみ成功扱いとする
/// </summary>
public class TestIsCubeNotTouched : EdaTestBehaviour
{
    // テスト名を C# 側で強制的に設定する
    protected override string TestName => "キューブが触れたら失敗のテスト";

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
