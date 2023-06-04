// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable

using System;

namespace Edanoue.SceneTest
{
    [Flags]
    public enum SceneTestStatus : byte
    {
        NotRunning = 0,
        Running    = 1 << 0,
        Succeed    = 1 << 1,
        Failed     = 1 << 2,
        Canceled   = 1 << 3,
        Completed  = Succeed | Failed | Canceled
    }
}