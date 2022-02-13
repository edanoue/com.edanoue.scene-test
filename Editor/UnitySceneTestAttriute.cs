// Copyright Edanoue, Inc. MIT License - see LICENSE.md

#nullable enable

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

namespace Edanoue.SceneTest
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class UnitySceneTestAttribute :
        NUnitAttribute,
        IFixtureBuilder
    {

        #region Constructors

        public UnitySceneTestAttribute(string scenePath)
        {
            _scenePath = scenePath;
        }

        #endregion

        private readonly NUnitTestFixtureBuilder _fixtureBuilder = new();
        private readonly NUnitTestCaseBuilder _testBuilder = new();

        // Unity に認識される
        IEnumerable<TestSuite> IFixtureBuilder.BuildFrom(ITypeInfo typeInfo)
        {
            var suite = _fixtureBuilder.BuildFrom(typeInfo);

            // 引数に指定されたパスを確認する
            string abspath;
            var guid = AssetDatabase.AssetPathToGUID(_scenePath);
            if (string.IsNullOrEmpty(guid))
            {
                // GUID が見つからなかったので検索する
                // 拡張子を取る
                var filename = Path.GetFileNameWithoutExtension(_scenePath);
                var result = AssetDatabase.FindAssets($"t:scene {filename}");

                if (result == null || result.Length == 0)
                {
                    // 指定されたシーンが見つからなかったのでエラー
                    suite.RunState = RunState.NotRunnable;
                    throw new ArgumentException($"Not founded scene: {_scenePath}");
                }
                else if (result.Length > 1)
                {
                    // 複数個シーンが見つかったのでエラー
                    suite.RunState = RunState.NotRunnable;
                    throw new ArgumentException($"Founded multiple scenes: {_scenePath}. Please use abs path");
                }
                else
                {
                    suite.RunState = RunState.Runnable;
                    abspath = AssetDatabase.GUIDToAssetPath(result[0]);
                }
            }
            else
            {
                // Asset/path/scene.unity 
                suite.RunState = RunState.Runnable;
                abspath = _scenePath;
            }

            suite.Name = $"Scene Test ({_sceneName}.unity)";

            // method を差し替える
            var method = new MethodWrapper(typeof(Foo), "RunTest");

            // これやると Unity void 以外の戻り方に が怒らなくなる
            TestCaseParameters parms = new TestCaseParameters(args: new object[1] { abspath })
            {
                ExpectedResult = new object(),
                HasExpectedResult = true
            };
            var test = _testBuilder.BuildTestMethod(method, suite, parms);
            if (test.parms != null)
            {
                test.parms.HasExpectedResult = false;
            }
            // Hack ここまで

            // 名前がうるさいのでシンプルに
            test.Name = "Run Test";
            suite.Add(test);

            yield return suite;
        }

        private readonly string _scenePath;
        private string _sceneName => Path.GetFileNameWithoutExtension(_scenePath);

    }
}