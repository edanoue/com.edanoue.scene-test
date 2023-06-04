// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable

using System.Collections.Generic;

namespace Edanoue.SceneTest
{
    /// <summary>
    /// Test の実行結果を保持している構造体
    /// </summary>
    public struct TestReport : ITestReport
    {
        string ITestReport.Name => TestName;
        SceneTestStatus ITestReport.Status => TestStatus;
        string ITestReport.Message => Message;

        Dictionary<string, string> ITestReport.CustomInfos => CustomInfos;


        public readonly string          TestName;
        public readonly string          GameObjectName;
        public          SceneTestStatus TestStatus;
        public          string          Message;

        public readonly Dictionary<string, string> CustomInfos;

        /// <summary>
        /// テストレポートを作成する
        /// </summary>
        /// <param name="testName"></param>
        /// <param name="gameObjectName"></param>
        public TestReport(string testName, string gameObjectName = "")
        {
            TestName = testName;
            GameObjectName = gameObjectName;
            TestStatus = SceneTestStatus.Created;
            Message = "";
            CustomInfos = new Dictionary<string, string>();
        }
    }
}