#nullable enable

using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Edanoue.SceneTest
{
    /// <summary>
    ///
    /// </summary>
    public abstract class SceneLoadSuiteBase
    {
        /// <summary>
        /// このシーンの呼び出し元
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected static string ScriptDir([CallerFilePath] string fileName = "")
        {
            var absdir = Path.GetDirectoryName(fileName);
            string pattern = @"Assets[\\/].+";
            var match = Regex.Match(absdir, pattern);
            return match.Success ? match.Value : "";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="timeoutSeconds"></param>
        /// <param name="isAutoLoadUnload"></param>
        /// <returns></returns>
        protected virtual IEnumerator RunTestAsync()
        {
            yield return null;
            /*
            foreach (var report in reports)
            {
                // テストが失敗していた時
                if (report.ResultState == ResultState.Failure)
                {
                    // TODO: まとめてログ出したいよね
                    var message = $"{report.Name}: {report.Message}";
                    Assert.Fail(message);
                }
            }
            */
        }
    }
}
