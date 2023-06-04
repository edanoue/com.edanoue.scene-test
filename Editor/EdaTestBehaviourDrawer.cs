// Copyright Edanoue, Inc. All Rights Reserved.

using UnityEditor;

namespace Edanoue.SceneTest
{
    /// <summary>
    /// EdaTestBehaviour 継承クラスで表示するカスタムエディタ
    /// </summary>
    [CustomEditor(typeof(EdaTestBehaviour), true)]
    public class EdaTestBehaviourDrawer : Editor
    {
        /// <summary>
        /// InspectorのGUIを更新
        /// </summary>
        public override void OnInspectorGUI()
        {
            //元のInspector部分を表示
            base.OnInspectorGUI();

            // //ボタンを表示
            // if (GUILayout.Button("Run this test"))
            // {

            //     Debug.Log("押した!");
            // }
        }
    }
}