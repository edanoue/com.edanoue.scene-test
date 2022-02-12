#nullable enable

using System.Collections.Generic;

namespace Edanoue.TestAPI
{
    /// <summary>
    /// Test の実行結果を保持している構造体
    /// </summary>
    public struct TestReport : ITestReport
    {
        #region IEdaTestReport impls

        string ITestReport.Name => this.TestName;
        Status ITestReport.Status => this.TestStatus;
        string ITestReport.Message => this.Message;

        Dictionary<string, string> ITestReport.CustomInfos => this.CustomInfos;

        #endregion

        public readonly string TestName;
        public readonly string GameObjectName;
        public Status TestStatus;
        public string Message;

        public Dictionary<string, string> CustomInfos;

        /// <summary>
        /// テストレポートを作成する
        /// </summary>
        /// <param name="testName"></param>
        /// <param name="gameObjectName"></param>
        public TestReport(string testName, string gameObjectName = "")
        {
            this.TestName = testName;
            this.GameObjectName = gameObjectName;
            this.TestStatus = Status.Created;
            this.Message = "";
            this.CustomInfos = new();
        }
    }
}
