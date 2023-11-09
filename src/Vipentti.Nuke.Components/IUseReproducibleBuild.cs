// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/Vipentti.Nuke.Components/blob/main/LICENSE.md

using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Components;

namespace Vipentti.Nuke.Components;

[PublicAPI]
public interface IUseReproducibleBuild : INukeBuild, IPack, IHazGitRepository
{
    [Parameter("Force UseReproducible to be set")]
    bool ForceReproducible => (bool)(TryGetValue(() => ForceReproducible as object) ?? false);

    string UseReproducibleBuild => ForceReproducible || IsServerBuild ? "true" : "false";

    // csharpier-ignore
    Configure<DotNetRestoreSettings> IRestore.RestoreSettings => _ => _
        .AddProperty("UseReproducibleBuild", UseReproducibleBuild);

    // csharpier-ignore
    Configure<DotNetBuildSettings> ICompile.CompileSettings => _ => _
        .AddProperty("RepositoryCommit", GitRepository.Commit)
        .AddProperty("RepositoryBranch", GitRepository.Branch)
        .AddProperty("UseReproducibleBuild", UseReproducibleBuild);

    // csharpier-ignore
    Configure<DotNetPublishSettings> ICompile.PublishSettings => _ => _
        .AddProperty("RepositoryCommit", GitRepository.Commit)
        .AddProperty("RepositoryBranch", GitRepository.Branch)
        .AddProperty("UseReproducibleBuild", UseReproducibleBuild);

    // csharpier-ignore
    Configure<DotNetPackSettings> IPack.PackSettings => _ => _
        .AddProperty("RepositoryCommit", GitRepository.Commit)
        .AddProperty("RepositoryBranch", GitRepository.Branch)
        .AddProperty("UseReproducibleBuild", UseReproducibleBuild);
}
