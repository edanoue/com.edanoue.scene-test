// Copyright Edanoue, Inc. MIT License - see LICENSE.md

#nullable enable

using System;
using UnityEngine;
using NUnit.Framework.Interfaces;
using Edanoue.SceneTest.Interfaces;

namespace Edanoue.SceneTest
{
    /// <summary>
    /// テストケース として振る舞う拡張 MonoBehaviour クラス
    /// </summary>
    public abstract class SceneTestCaseBehaviour : MonoBehaviour, ISceneTestCase
    {
        #region Edanoue.SceneTest.ISceneTestCase impls

        bool ISceneTestCase.IsRunning => this._isRunning;
        void ISceneTestCase.OnRun() => this.OnRun();
        void ISceneTestCase.OnTimeout() => this.OnTimeout();
        ITestResult ISceneTestCase.Result => this.TestResult;
        CaseOptions ISceneTestCase.Options => this.m_testCaseOptions;

        #endregion

        #region Unity 公開 Property

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SerializeField]
        [Tooltip("")]
        private CaseOptions m_testCaseOptions = new()
        {
            CustomCaseName = "",
            LocalTimeoutSeconds = 10f
        };

        #endregion

        // このテストケースの名前
        protected string TestName => string.IsNullOrEmpty(m_testCaseOptions.CustomCaseName) ? this.GetType().Name : m_testCaseOptions.CustomCaseName;

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
        SceneTestCase? _testCase;
        SceneTestCase TestCase
        {
            get
            {
                if (_testCase is null)
                {
                    _testCase = new SceneTestCase(this.TestName);
                }
                return _testCase;
            }
        }
        SceneTestCaseResult? _testResult;
        SceneTestCaseResult TestResult
        {
            get
            {
                if (_testResult is null)
                {
                    _testResult = new SceneTestCaseResult(TestCase);
                }
                return _testResult;
            }
        }

        bool IsSucceedOrFailed
        {
            get
            {
                return TestResult.ResultState == ResultState.Success || TestResult.ResultState == ResultState.Failure;
            }
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
                TestResult.StartTime = System.DateTime.UtcNow;
                _isRunning = true;
            }
        }

        /// <summary>
        /// このテストを成功扱いとする
        /// Runner によりテストが実行されていない状態であっても先に結果の代入は行える
        /// </summary>
        /// <param name="message"></param>
        protected void Success(string? message)
        {
            // すでに結果が確定してたら何もしない
            if (IsSucceedOrFailed) return;

            TestResult.SetResult(ResultState.Success, message);

            // Runner による監視中での実行
            if (_isRunning)
            {
                TestResult.EndTime = DateTime.UtcNow;
                TestResult.Duration = (TestResult.EndTime - TestResult.StartTime).TotalSeconds;
                _isRunning = false;
            }
            // Awake などでの実行
            else
            {
                TestResult.StartTime = DateTime.UtcNow;
                TestResult.EndTime = DateTime.UtcNow;
                TestResult.Duration = 0;
            }
        }

        /// <summary>
        /// このテストを失敗扱いとする
        /// Runner によりテストが実行されていない状態であっても先に結果の代入は行える
        /// </summary>
        /// <param name="message"></param>
        protected void Fail(string? message)
        {
            // すでに結果が確定してたら何もしない
            if (IsSucceedOrFailed) return;

            TestResult.SetResult(ResultState.Failure, message);

            // Runner による監視中での実行
            if (_isRunning)
            {
                TestResult.EndTime = DateTime.UtcNow;
                TestResult.Duration = (TestResult.EndTime - TestResult.StartTime).TotalSeconds;
                _isRunning = false;
            }
            // Awake などでの実行
            else
            {
                TestResult.StartTime = DateTime.UtcNow;
                TestResult.EndTime = DateTime.UtcNow;
                TestResult.Duration = 0;
            }
        }

        #endregion
    }
}
