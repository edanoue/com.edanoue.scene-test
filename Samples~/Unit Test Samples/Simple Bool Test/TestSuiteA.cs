#nullable enable

#if UNITY_EDITOR
namespace Edanoue.TestAPI.Samples
{
    class TestSuiteA : SceneLoadSuiteBase
    {
        protected override string ScenePath => $"{ScriptDir()}/TestApiExampleA.unity";

        [UnityTest]
        public IEnumerator TestApiExampleA()
        {
            yield return RunTestAsync();
        }
    }
}
#endif
