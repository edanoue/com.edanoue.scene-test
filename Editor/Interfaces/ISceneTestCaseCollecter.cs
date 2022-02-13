// Copyright Edanoue, Inc. MIT License - see LICENSE.md

#nullable enable

using System.Collections.Generic;

namespace Edanoue.SceneTest.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    interface ISceneTestCaseCollecter
    {
        internal IEnumerable<ISceneTestCase> TestCases { get; }
        internal bool Collect();
    }
}