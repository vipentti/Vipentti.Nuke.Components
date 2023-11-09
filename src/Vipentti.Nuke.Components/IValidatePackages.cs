// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/Vipentti.Nuke.Components/blob/main/LICENSE.md

using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using Nuke.Components;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Vipentti.Nuke.Components;

[PublicAPI]
public interface IValidatePackages : IPack
{
    // csharpier-ignore
    Target ValidatePackages => _ => _
        .DependsOn<IPack>(x => x.Pack)
        .After<IPack>(x => x.Pack)
        .Executes(() =>
        {
            DotNetToolUpdate(
                _ => _.SetGlobal(true).SetPackageName("Meziantou.Framework.NuGetPackageValidation.Tool")
            );

            static void ValidatePackage(AbsolutePath path)
            {
                ProcessTasks
                    .StartProcess("meziantou.validate-nuget-package", arguments: path, logInvocation: true)
                    .AssertZeroExitCode();
            }

            PackagesDirectory.GlobFiles("*.nupkg").ForEach(ValidatePackage);
        });
}
