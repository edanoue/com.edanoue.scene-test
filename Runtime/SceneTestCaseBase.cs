#nullable enable

using System.Collections;
using UnityEngine;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Edanoue.SceneTest
{
    /// <summary>
    /// テストケース として振る舞う拡張 MonoBehaviour クラス
    /// </summary>
    public abstract class SceneTestCaseBase : MonoBehaviour, ISceneTestCase
    {
        #region Edanoue.SceneTest.ISceneTestCase impls

        bool ISceneTestCase.IsRunning => this._isRunning;
        void ISceneTestCase.OnRun() => this.OnRun();
        void ISceneTestCase.OnCancel() => this.OnCancel();
        void ISceneTestCase.OnTimeout() => this.OnTimeout();
        ITestResult ISceneTestCase.Report => this._testResult;
        CaseOptions ISceneTestCase.Options => this._localOptions;

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
        protected virtual string TestName => string.IsNullOrEmpty(m_customTestName) ? this.GetType().Name : m_customTestName;

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

        bool _isRunning;
        SceneTestCase? _testcase;
        SceneTestCaseResult? _testResult;
        CaseOptions _localOptions;

        private void _makeTestResult()
        {
            _testcase = new(this.TestName);
            _testResult = (SceneTestCaseResult)_testcase.MakeTestResult();
            _testResult.StartTime = System.DateTime.UtcNow;
        }

        /// <summary>
        /// Runner により呼ばれるテスト開始のコールバック
        /// </summary>
        private void OnRun()
        {
            // すでに テストレポートが生成されてたら無視する
            // Runner からの実行前に Awake などで呼ばれているパターン
            if (_testResult is not null)
            {
                return;
            }

            if (!_isRunning)
            {
                _makeTestResult();

                // 実行時点で Inspector に設定されているものからオプションを作成する
                _localOptions = new(
                    localTimeoutSeconds: m_timeoutSeconds
                );

                Debug.Log($"Run {_testResult!.FullName}", this);
                _isRunning = true;
            }
        }

        /// <summary>
        /// Runner により呼ばれるテストキャンセルのコールバック
        /// </summary>
        private void OnCancel()
        {
            if (_isRunning)
            {
                _testResult!.SetResult(ResultState.Cancelled, "Manually canceled");
                _testResult.EndTime = System.DateTime.UtcNow;
                _testResult.Duration = (_testResult.EndTime - _testResult.StartTime).TotalSeconds;
                _isRunning = false;
            }
        }

        /// <summary>
        /// このテストを成功扱いとする
        /// Runner によりテストが実行されていない状態であっても先に結果の代入は行える
        /// </summary>
        /// <param name="message"></param>
        protected void Success(string? message)
        {
            // Awake などで実行されたとき
            if (_testcase is null)
            {
                _makeTestResult();
            }
            // 結果を代入する
            _testResult.SetResult(ResultState.Success, message);
            _testResult.EndTime = System.DateTime.UtcNow;
            _testResult.Duration = (_testResult.EndTime - _testResult.StartTime).TotalSeconds;
            _isRunning = false;
        }

        /// <summary>
        /// このテストを失敗扱いとする
        /// Runner によりテストが実行されていない状態であっても先に結果の代入は行える
        /// </summary>
        /// <param name="message"></param>
        protected void Fail(string? message)
        {
            // Awake などで実行されたとき
            if (_testcase is null)
            {
                _makeTestResult();
            }
            // 結果を代入する
            _testResult.SetResult(ResultState.Failure, message);
            _testResult.EndTime = System.DateTime.UtcNow;
            _testResult.Duration = (_testResult.EndTime - _testResult.StartTime).TotalSeconds;
            _isRunning = false;
        }

        #endregion
    }
}
