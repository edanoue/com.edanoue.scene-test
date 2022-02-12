// Copyright Edanoue, Inc. MIT License - see LICENSE.md

#nullable enable

using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Edanoue.SceneTest
{
    /// <summary>
    /// 
    /// </summary>
    public class SceneTestCaseResult : TestResult
    {
        /// <summary>
        /// Construct a TestCaseResult based on a TestMethod
        /// </summary>
        /// <param name="test">A TestMethod to which the result applies.</param>
        public SceneTestCaseResult(ITest test) : base(test) { }

        #region Overrides

        /// <summary>
        /// Gets the number of test cases that failed
        /// when running the test and all its children.
        /// </summary>
        public override int FailCount
        {
            get { return ResultState.Status == TestStatus.Failed ? 1 : 0; }
        }

        /// <summary>
        /// Gets the number of test cases that passed
        /// when running the test and all its children.
        /// </summary>
        public override int PassCount
        {
            get { return ResultState.Status == TestStatus.Passed ? 1 : 0; }
        }

        /// <summary>
        /// Gets the number of test cases that were skipped
        /// when running the test and all its children.
        /// </summary>
        public override int SkipCount
        {
            get { return ResultState.Status == TestStatus.Skipped ? 1 : 0; }
        }

        /// <summary>
        /// Gets the number of test cases that were inconclusive
        /// when running the test and all its children.
        /// </summary>
        public override int InconclusiveCount
        {
            get { return ResultState.Status == TestStatus.Inconclusive ? 1 : 0; }
        }

        /// <summary>
        /// Indicates whether this result has any child results.
        /// </summary>
        public override bool HasChildren
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the collection of child results.
        /// </summary>
        public override IEnumerable<ITestResult> Children
        {
            get { return new ITestResult[0]; }
        }

        #endregion
    }
}
