#nullable enable

#if UNITY_EDITOR
namespace Edanoue.TestAPI.Samples
{
    class TestSuiteB : SceneLoadSuiteBase
    {
        protected override string ScenePath => $"${ScriptDir()}/TestApiExampleB.unity";

        [UnityTest]
        public IEnumerator TestApiExampleB()
        {
            // Runner の Timeout を デフォルト 10 秒から 5秒 に変える
            var options = new RunnerOptions(
                globalTimeoutSeconds: 5.0f
            );

            // シーン内テストを実行する
            yield return RunTestAsync(options: options);
        }
    }
}
#endif