// Copyright Edanoue, Inc. All Rights Reserved.

#nullable enable

using System.Collections.Generic;

namespace Edanoue.SceneTest
{
    public interface ITestReport
    {
        string Name { get; }
        SceneTestStatus Status { get; }
        string Message { get; }
        Dictionary<string, string> CustomInfos { get; }
    }
}