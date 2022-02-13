// Copyright Edanoue, Inc. MIT License - see LICENSE.md

#nullable enable

using System;

namespace Edanoue.SceneTest.Interfaces
{
    /// <summary>
    /// 各 TestCase に local に適用されるオプション
    /// </summary>
    [Serializable]
    public struct CaseOptions
    {
        public string CustomCaseName;

        /// <summary>
        /// TestCase 自体の 実行時間の制限
        /// Runner に設定されている値より 少ない場合こちらが優先されます
        /// /// </summary>
        public float LocalTimeoutSeconds;
    }
}
