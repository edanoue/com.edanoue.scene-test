// Copyright Edanoue, Inc. MIT License - see LICENSE.md

#nullable enable
#if UNITY_EDITOR

using Edanoue.SceneTest;

/// <summary>
/// Unity の Inspector のチェックボックスにより可否が決まるテストクラス
/// </summary>
class ExampleUnityPropertyBoolTest : SceneTestCaseBehaviour
{
    public bool テストに成功するかどうか;

    void Awake()
    {
        if (テストに成功するかどうか)
        {
            Success("テストに成功しました");
        }
        else
        {
            Fail("テストに失敗しました");
        }
    }
}
#endif