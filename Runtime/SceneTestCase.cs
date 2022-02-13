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
    internal class SceneTestCase : Test
    {
        public SceneTestCase(string name) : base(name) { }

        public override bool HasChildren
        {
            get { return false; }
        }

        public override IList<ITest> Tests
        {
            get { return new ITest[0]; }
        }

        public override string XmlElementName
        {
            get { return "test-case"; }
        }

        public override TNode AddToXml(TNode parentNode, bool recursive)
        {
            TNode thisNode = parentNode.AddElement(XmlElementName);

            PopulateTestNode(thisNode, recursive);

            thisNode.AddAttribute("seed", this.Seed.ToString());

            return thisNode;
        }

        public override TestResult MakeTestResult()
        {
            return new SceneTestCaseResult(this);
        }

    }
}