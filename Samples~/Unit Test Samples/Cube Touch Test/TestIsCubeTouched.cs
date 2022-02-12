#nullable enable

#if UNITY_EDITOR
namespace Edanoue.TestAPI.Samples
{
    /// <summary>
    /// シーン内にあるキューブが触れたら成功するテスト
    /// </summary>
    class TestIsCubeTouched : EdaTestBehaviour
    {
        void OnCollisionEnter(Collision _)
        {
            Success("キューブが触れました");
        }
    }
}
#endif