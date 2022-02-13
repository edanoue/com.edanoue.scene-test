// Copyright Edanoue, Inc. MIT License - see LICENSE.md

#nullable enable

using System.Collections;

namespace Edanoue.SceneTest.Interfaces
{
    public interface ISceneTestRunner
    {
        IEnumerator RunAll(RunnerOptions? options = null);

        IEnumerator Run(string[] ids, RunnerOptions? options = null);
    }
}
