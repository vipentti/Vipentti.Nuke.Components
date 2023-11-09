// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/Vipentti.Nuke.Components/blob/main/LICENSE.md

using System.Collections.Generic;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Vipentti.Nuke.Components;

[PublicAPI]
public interface IUseCsharpier : INukeBuild
{
    bool UseGlobalTool { get; }

    IEnumerable<AbsolutePath> DirectoriesToFormat => new[] { RootDirectory };

    // csharpier-ignore
    Target CheckCsharpier => _ => _
        .Executes(() =>
        {
            RunCsharpier(check: true);
        });

    // csharpier-ignore
    Target InstallCsharpier => _ => _
        .OnlyWhenDynamic(() => UseGlobalTool)
        .Executes(ExecuteInstallGlobalCsharpier);

    // csharpier-ignore
    Target FormatCsharpier => _ => _
        .TryBefore<IUseDotNetFormat>(x => x.Format)
        .TryDependentFor<IUseDotNetFormat>(x => x.Format)
        .Executes(() =>
        {
            RunCsharpier(check: false);
        });

    sealed IProvideLinter Linter =>
        new LinterDelegate(ExecuteInstallGlobalCsharpier, () => RunCsharpier(check: true));

    sealed void ExecuteInstallGlobalCsharpier()
    {
        if (UseGlobalTool)
        {
            DotNetToolUpdate(_ => _.SetGlobal(true).SetPackageName("csharpier"));
        }
    }

    sealed void RunCsharpier(bool check)
    {
        var toolname = UseGlobalTool ? "csharpier" : "tool run dotnet-csharpier";

        DirectoriesToFormat.ForEach(RunFormat);

        void RunFormat(AbsolutePath path)
        {
            DotNet(
                arguments: $"{toolname} {path}" + (check ? " --check" : ""),
                logInvocation: true
            );
        }
    }
}
