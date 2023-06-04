// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable
namespace Edanoue.SceneTest
{
    /// <summary>
    /// TestRunner から操作されるのためのインタフェース
    /// </summary>
    public interface ITestCase
    {
        /// <summary>
        /// テストがまだ実行中かどうか
        /// </summary>
        /// <value></value>
        bool IsRunning { get; }

        /// <summary>
        /// テストレポートを取得する
        /// </summary>
        ITestReport Report { get; }

        /// <summary>
        /// TestCase Local の Option を取得
        /// </summary>
        /// <value></value>
        CaseOptions Options { get; }

        /// <summary>
        /// 開始時に呼ばれるコールバック
        /// </summary>
        void OnRun();

        /// <summary>
        /// キャンセル時に呼ばれるコールバック
        /// </summary>
        void OnCancel();

        /// <summary>
        /// タイムアウト時に呼ばれるコールバック
        /// </summary>
        void OnTimeout();
    }
}