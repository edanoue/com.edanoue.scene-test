// Copyright Edanoue, Inc. All Rights Reserved.

using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle("Edanoue.SceneTest")]
[assembly: AssemblyCopyright("Copyright Edanoue, Inc. All Rights Reserved.")]

// アセンブリのバージョン情報は、以下の 4 つの値で構成されています:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// すべての値を指定するか、以下のように '*' を使用してビルド番号とリビジョン番号を 
// 既定値にすることができます:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// この assembly "Edanoue.SceneTest.dll" に Internal アクセスできる Friend Assembly
// InternalsVisibleTo を書き連ねて行ってください
// Edanoue.TestAPI.Editor
[assembly: InternalsVisibleTo("Edanoue.SceneTest.Editor")]