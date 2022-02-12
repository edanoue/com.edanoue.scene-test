#nullable enable

using System.Collections;
using UnityEngine;

namespace Edanoue.TestAPI
{
    /// <summary>
    /// テストケース として振る舞う拡張 MonoBehaviour クラス
    /// </summary>
    public abstract class EdaTestBehaviour : MonoBehaviour, ITestCase
    {
        #region IEdaTestCase

        bool ITestCase.IsRunning => this.IsRunning;
        void ITestCase.OnRun() => this.OnRun();
        void ITestCase.OnCancel() => this.OnCancel();
        void ITestCase.OnTimeout() => this.OnTimeout();
        ITestReport ITestCase.Report => this._testReport;
        CaseOptions ITestCase.Options => this._localOptions;

        #endregion

        #region Unity 公開 Property

        /// <summary>
        /// Inspector から設定できるテスト名
        /// 指定されていない場合クラス名が使用される
        /// </summary>
        [SerializeField]
        private string m_customTestName = "";


        /// <summary>
        /// Inspector から設定できるこのテスト独自のTimeout時間
        /// Runner 側に指定されている時間より短い場合, こちらの時間が使用されます
        /// <summary>
        [SerializeField]
        private float m_timeoutSeconds = 10f;

        #endregion

        /// <summary>
        /// 継承先でオーバーライドできるテスト名
        /// デフォルトではクラス名を使用する
        /// </summary>
        protected virtual string TestName => m_customTestName == "" ? this.GetType().Name : m_customTestName;

        // 成功したときのコールバック, ここでは空にしておく
        protected virtual void OnSuccess() { }

        // 失敗したときのコールバック, ここでは空にしておく
        protected virtual void OnFail() { }

        /// <summary>
        /// Runner により呼ばれる タイムアウトされたときのコールバック
        /// </summary>
        protected virtual void OnTimeout()
        {
            // デフォルトではタイムアウトの場合は Fail する
            Fail("Timeouted");
        }

        #region 内部処理用

        TestReport _testReport;
        CaseOptions _localOptions;

        bool IsCreated => _testReport.TestStatus == Status.Created;
        bool IsRunning => _testReport.TestStatus == Status.Running;
        bool IsSucceeded => _testReport.TestStatus == Status.Succeed;
        bool IsFailed => _testReport.TestStatus == Status.Failed;
        bool IsCanceled => _testReport.TestStatus == Status.Canceled;
        bool IsCompleted => IsSucceeded || IsFailed || IsCanceled;

        // テストレポートの初期化
        private void _createNewTestReport()
        {
            // Create new test report
            //   TestCaseName:   class name
            //   GameObjectName: attatched gameobject name
            _testReport = new(TestName, this.gameObject.name);
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
            _localOptions = new(
                localTimeoutSeconds: m_timeoutSeconds
            );

            // Set Status to Running
            _testReport.TestStatus = Status.Running;

            Debug.Log($"Run {_testReport.TestName}", this);
        }

        /// <summary>
        /// Runner により呼ばれるテストキャンセルのコールバック
        /// </summary>
        protected virtual void OnCancel()
        {
            if (IsRunning)
            {
                _testReport.TestStatus = Status.Canceled;
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

            if (!IsCompleted)
            {
                _testReport.TestStatus = Status.Succeed;
                _testReport.Message = message;
                OnSuccess();
            }
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

            if (!IsCompleted)
            {
                _testReport.TestStatus = Status.Failed;
                _testReport.Message = message;
                OnFail();
            }
        }

        #endregion
    }
}
