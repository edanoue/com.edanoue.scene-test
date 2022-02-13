// Copyright Edanoue, Inc. MIT License - see LICENSE.md

#nullable enable

using NUnit.Framework.Interfaces;

namespace Edanoue.SceneTest.Interfaces
{
    /// <summary>
    /// TestRunner から操作されるのためのインタフェース
    /// </summary>
    public interface ISceneTestCase
    {
        /// <summary>
        /// テストがまだ実行中かどうか
        /// </summary>
        /// <value></value>
        bool IsRunning { get; }

        /// <summary>
        /// 開始時に呼ばれるコールバック
        /// </summary>
        void OnRun();

        /// <summary>
        /// タイムアウト時に呼ばれるコールバック
        /// </summary>
        void OnTimeout();

        /// <summary>
        /// テストレポートを取得する
        /// </summary>
        ITestResult Result { get; }

        /// <summary>
        /// TestCase Local の Option を取得
        /// </summary>
        /// <value></value>
        CaseOptions Options { get; }
    }
}
