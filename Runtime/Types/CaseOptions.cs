// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable

namespace Edanoue.SceneTest
{
    /// <summary>
    /// 各 TestCase に local に適用されるオプション
    /// </summary>
    public readonly struct CaseOptions
    {
        /// <summary>
        /// TestCase 自体の 実行時間の制限
        /// Runner に設定されている値より 少ない場合こちらが優先されます
        /// ///
        /// </summary>
        public readonly float LocalTimeoutSeconds;

        public CaseOptions(float localTimeoutSeconds)
        {
            LocalTimeoutSeconds = localTimeoutSeconds;
        }
    }
}