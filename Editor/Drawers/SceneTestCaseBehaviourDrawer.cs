// Copyright Edanoue, Inc. MIT License - see LICENSE.md

#nullable enable

using UnityEditor;

namespace Edanoue.SceneTest.Drawers
{
    /// <summary>
    /// EdaTestBehaviour 継承クラスで表示するカスタムエディタ
    /// </summary>
    [CustomEditor(typeof(SceneTestCaseBehaviour), true)]
    internal class SceneTestCaseBehaviourDrawer : Editor
    {
        /// <summary>
        /// InspectorのGUIを更新
        /// </summary>
        public override void OnInspectorGUI()
        {
            // Helpbox を表示する
            EditorGUILayout.HelpBox("This Component is Scene Test Case", MessageType.Info);

            //元のInspector部分を表示
            base.OnInspectorGUI();
        }
    }
}
