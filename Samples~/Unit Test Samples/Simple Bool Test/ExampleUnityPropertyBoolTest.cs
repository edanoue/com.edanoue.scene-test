#nullable enable

#if UNITY_EDITOR
namespace Edanoue.TestAPI.Samples
{
    /// <summary>
    /// Unity の Inspector のチェックボックスにより可否が決まるテストクラス
    /// </summary>
    class ExampleUnityPropertyBoolTest : EdaTestBehaviour
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
}
#endif