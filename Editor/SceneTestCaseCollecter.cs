#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

using Edanoue.SceneTest.Interfaces;

namespace Edanoue.SceneTest
{
    internal class SceneTestCaseCollecter : ISceneTestCaseCollecter
    {
        IEnumerable<ISceneTestCase> ISceneTestCaseCollecter.TestCases => _cachedTestCases;
        bool ISceneTestCaseCollecter.Collect() => _FetchAllITestBehaviour();

        #region Helpers

        private readonly List<ISceneTestCase> _cachedTestCases = new();

        private bool _FetchAllITestBehaviour()
        {
            // 一度キャッシュを空にする
            _cachedTestCases.Clear();

            // ロードしているすべてのシーン内から ITestBehaviour 実装コンポーネントを検索する
            var ss = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>().OfType<ISceneTestCase>();
            foreach (var s in ss)
            {
                // キャッシュに追加する
                if (!_cachedTestCases.Contains(s))
                {
                    _cachedTestCases.Add(s);
                }
            }
            return _cachedTestCases.Count > 0;
        }

        #endregion
    }
}