// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/Vipentti.Nuke.Components/blob/main/LICENSE.md

using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Components;

namespace Vipentti.Nuke.Components;

[PublicAPI]
public interface ITestWithMultipleFrameworks : ITest
{
    // csharpier-ignore
    Target ITest.Test => _ => _
        // NOTE: Order here is significant
        // this currently executes the cleanup first and then tests
        // so the TestResultDirectory only contains the latest results
        .Executes(() => TestResultDirectory.CreateOrCleanDirectory())
        .Inherit<ITest>();

    Configure<DotNetTestSettings, Project> ITest.TestProjectSettings =>
        (_, v) =>
            _.RemoveLoggers($"trx;LogFileName={v.Name}.trx")
                .AddLoggers($"trx;LogFilePrefix={v.Name}");
}
