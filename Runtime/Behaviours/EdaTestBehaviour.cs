// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable

using UnityEngine;

namespace Edanoue.SceneTest
{
    /// <summary>
    /// テストケース として振る舞う拡張 MonoBehaviour クラス
    /// </summary>
    public abstract class EdaTestBehaviour : MonoBehaviour, ITestCase
    {
        /// <summary>
        /// Inspector から設定できるテスト名
        /// 指定されていない場合クラス名が使用される
        /// </summary>
        [SerializeField]
        private string m_customTestName = "";


        /// <summary>
        /// Inspector から設定できるこのテスト独自のTimeout時間
        /// Runner 側に指定されている時間より短い場合, こちらの時間が使用されます
        /// </summary>
        [SerializeField]
        private float m_timeoutSeconds = 10f;

        /// <summary>
        /// 継承先でオーバーライドできるテスト名
        /// デフォルトではクラス名を使用する
        /// </summary>
        protected virtual string TestName => m_customTestName == "" ? GetType().Name : m_customTestName;


        bool ITestCase.IsRunning => IsRunning;

        void ITestCase.OnRun()
        {
            OnRun();
        }

        void ITestCase.OnCancel()
        {
            OnCancel();
        }

        void ITestCase.OnTimeout()
        {
            OnTimeout();
        }

        ITestReport ITestCase.Report => _testReport;
        CaseOptions ITestCase.Options => _localOptions;

        // 成功したときのコールバック, ここでは空にしておく
        protected virtual void OnSuccess()
        {
        }

        // 失敗したときのコールバック, ここでは空にしておく
        protected virtual void OnFail()
        {
        }

        /// <summary>
        /// Runner により呼ばれる タイムアウトされたときのコールバック
        /// </summary>
        protected virtual void OnTimeout()
        {
            // デフォルトではタイムアウトの場合は Fail する
            Fail("Timeout");
        }


        #region 内部処理用

        private TestReport  _testReport;
        private CaseOptions _localOptions;

        private bool IsCreated => _testReport.TestStatus == SceneTestStatus.Created;
        private bool IsRunning => _testReport.TestStatus == SceneTestStatus.Running;
        private bool IsSucceeded => _testReport.TestStatus == SceneTestStatus.Succeed;
        private bool IsFailed => _testReport.TestStatus == SceneTestStatus.Failed;
        private bool IsCanceled => _testReport.TestStatus == SceneTestStatus.Canceled;
        private bool IsCompleted => IsSucceeded || IsFailed || IsCanceled;

        // テストレポートの初期化
        private void _createNewTestReport()
        {
            // Create new test report
            //   TestCaseName:   class name
            //   GameObjectName: attached game object name
            _testReport = new TestReport(TestName, gameObject.name);
        }

        /// <summary>
        /// Runner により呼ばれるテスト開始のコールバック
        /// </summary>
        protected virtual void OnRun()
        {
            // 実行中の場合は無視する
            if (IsRunning)
            {
                return;
            }

            // すでに結果が決まっている場合はどうするか
            // たとえば Awake 内などで すでに結果が代入されている場合がある
            // 何回もテストを実行するよりも, Awake の柔軟性を出したほうが良さそう
            // TODO: この実装にしてしまうと, 一度結果が確定したテストはもう動かなくなってしまう
            // TODO: 何回も実行したい場合は, Reset などのAPI を用意することで対応する
            if (IsCompleted)
            {
                return;
            }

            _createNewTestReport();

            // 実行時点で Inspector に設定されているものからオプションを作成する
            _localOptions = new CaseOptions(
                m_timeoutSeconds
            );

            // Set Status to Running
            _testReport.TestStatus = SceneTestStatus.Running;

            Debug.Log($"Run {_testReport.TestName}", this);
        }

        /// <summary>
        /// Runner により呼ばれるテストキャンセルのコールバック
        /// </summary>
        protected virtual void OnCancel()
        {
            if (IsRunning)
            {
                _testReport.TestStatus = SceneTestStatus.Canceled;
                _testReport.Message = "Manually canceled";
            }
        }

        /// <summary>
        /// このテストを成功扱いとする
        /// Runner によりテストが実行されていない状態であっても先に結果の代入は行える
        /// </summary>
        /// <param name="message"></param>
        protected void Success(string message = "")
        {
            // まだRunner によりテストが実行されていない状態
            // 例えば Awake などですでにテスト結果が決まっているときなど
            if (IsCreated)
            {
                // まだテストレポートが作成されていないので新規に作成する
                _createNewTestReport();
            }

            if (IsCompleted)
            {
                return;
            }

            _testReport.TestStatus = SceneTestStatus.Succeed;
            _testReport.Message = message;
            OnSuccess();
        }

        /// <summary>
        /// このテストを失敗扱いとする
        /// Runner によりテストが実行されていない状態であっても先に結果の代入は行える
        /// </summary>
        /// <param name="message"></param>
        protected void Fail(string message = "")
        {
            // まだRunner によりテストが実行されていない状態
            // 例えば Awake などですでにテスト結果が決まっているときなど
            if (IsCreated)
            {
                // まだテストレポートが作成されていないので新規に作成する
                _createNewTestReport();
            }

            if (IsCompleted)
            {
                return;
            }

            _testReport.TestStatus = SceneTestStatus.Failed;
            _testReport.Message = message;
            OnFail();
        }

        #endregion
    }
}