using UnityEditor;
using UnityEngine;


namespace Edanoue.SceneTest
{
    /// <summary>
    /// EdaTestBehaviour 継承クラスで表示するカスタムエディタ
    /// </summary>
    [CustomEditor(typeof(SceneTestCaseBase), true)]
    internal class SceneTestCaseDrawer : Editor
    {
        /// <summary>
        /// InspectorのGUIを更新
        /// </summary>
        public override void OnInspectorGUI()
        {
            //元のInspector部分を表示
            base.OnInspectorGUI();
        }
    }
}
