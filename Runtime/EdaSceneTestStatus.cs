// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable
using System;

namespace Edanoue.SceneTest
{
    public static class EdaSceneTestStatus
    {
        internal static Action? _onStartEdaSceneTest;

        /// <summary>
        /// </summary>
        public static bool IsRunningEdaSceneTest { get; internal set; }

        /// <summary>
        /// </summary>
        public static event Action OnStartEdaSceneTest
        {
            add => _onStartEdaSceneTest += value;
            remove => _onStartEdaSceneTest -= value;
        }
    }
}