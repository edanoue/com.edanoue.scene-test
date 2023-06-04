// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable
#if UNITY_EDITOR

using Edanoue.SceneTest;
using UnityEngine;

/// <summary>
/// Unity の Inspector のチェックボックスにより可否が決まるテストクラス
/// </summary>
internal class ExampleUnityPropertyBoolTest : EdaTestBehaviour
{
    [SerializeField]
    private bool m_forcePassedTest;

    private void Awake()
    {
        if (m_forcePassedTest)
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