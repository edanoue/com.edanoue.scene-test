#nullable enable

using System.Collections.Generic;

namespace Edanoue.TestAPI
{
    public interface ITestReport
    {
        string Name { get; }
        Status Status { get; }
        string Message { get; }
        Dictionary<string, string> CustomInfos { get; }
    }
}
