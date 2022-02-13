// Copyright Edanoue, Inc. MIT License - see LICENSE.md

#nullable enable

namespace Edanoue.SceneTest.Interfaces
{
    public readonly struct RunnerOptions
    {
        /// <summary>
        /// Runner 自体の 実行時間の制限
        /// 各テストケースでローカルに指定されてたらそちらが優先されます
        /// </summary>
        public readonly float GlobalTimeoutSeconds;

        public RunnerOptions(float globalTimeoutSeconds)
        {
            GlobalTimeoutSeconds = globalTimeoutSeconds;
        }
    }
}
