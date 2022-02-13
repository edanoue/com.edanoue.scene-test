// Copyright Edanoue, Inc. MIT License - see LICENSE.md

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using UnityEditor;

namespace Edanoue.SceneTest
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class UnitySceneTestAttribute : NUnitAttribute, IFixtureBuilder
    {

        #region Constructors

        public UnitySceneTestAttribute(string scenePath)
        {
            _scenePath = scenePath;
        }

        #endregion


        #region NUnit.Framework.Interfaces.IFixtureBuilder impls

        IEnumerable<TestSuite> IFixtureBuilder.BuildFrom(ITypeInfo typeInfo)
        {
            var fixture = _fixtureBuilder.BuildFrom(typeInfo);

            // Attribute の 引数に指定されたパスを確認する
            string abspath;

            // AssetDatabase を利用して GUID への変換を行う
            var guid = AssetDatabase.AssetPathToGUID(_scenePath);

            // GUIDが 空文字の場合は 指定されたパスの解決に失敗している
            if (string.IsNullOrEmpty(guid))
            {
                // GUID が見つからなかったので検索する
                // 拡張子を取る
                var filename = Path.GetFileNameWithoutExtension(_scenePath);
                var result = AssetDatabase.FindAssets($"t:scene {filename}");

                if (result == null || result.Length == 0)
                {
                    // 指定されたシーンが見つからなかったのでエラー
                    var errorMsg = $"Not founded scene: {_scenePath}";
                    {
                        fixture.RunState = RunState.NotRunnable;
                        fixture.Properties.Add(PropertyNames.SkipReason, errorMsg);
                    }
                    throw new ArgumentException(errorMsg, "scenePath");
                }
                if (result.Length > 1)
                {
                    // 複数個シーンが見つかったのでエラー
                    var errorMsg = $"There are multiple scenes with the same name: {_scenePath}. Please use absolute paths (e.g. Assets/your/scene.unity)";
                    {
                        fixture.RunState = RunState.NotRunnable;
                        fixture.Properties.Add(PropertyNames.SkipReason, errorMsg);
                    }
                    throw new ArgumentException(errorMsg, "scenePath");
                }

                fixture.RunState = RunState.Runnable;
                abspath = AssetDatabase.GUIDToAssetPath(result[0]);
            }

            // GUID が取得できたばあいは そのまま引数のものをシーンパスとする
            else
            {
                // Asset/path/scene.unity 
                fixture.RunState = RunState.Runnable;
                abspath = _scenePath;
            }

            // Fixture の 名前を上書きする
            fixture.Name = $"Scene Test ({_sceneName}.unity)";

            // Fixuture で実行するメソッド を差し替える
            var method = new MethodWrapper(typeof(CommonSceneTestCase), "RunTest");

            // Hack for Unity Test Runner
            // この処理をやることで PlayMode Test が動作するようになる
            TestCaseParameters parms = new TestCaseParameters(args: new object[1] { abspath })
            {
                ExpectedResult = new object(),
                HasExpectedResult = true
            };
            var test = _testBuilder.BuildTestMethod(method, fixture, parms);
            if (test.parms != null)
            {
                test.parms.HasExpectedResult = false;
            }
            // End Hack

            // 名前がうるさいのでシンプルに
            test.Name = "Run Test";

            // Fixture にこのテストを追加する
            fixture.Add(test);

            yield return fixture;
        }

        #endregion

        #region Helpers

        private readonly NUnitTestFixtureBuilder _fixtureBuilder = new();
        private readonly NUnitTestCaseBuilder _testBuilder = new();

        private readonly string _scenePath;
        private string _sceneName => Path.GetFileNameWithoutExtension(_scenePath);

        #endregion

    }
}