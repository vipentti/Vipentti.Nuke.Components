// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/Vipentti.Nuke.Components/blob/main/LICENSE.md

using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.GitVersion;
using Nuke.Components;

namespace Vipentti.Nuke.Components;

[DisableDefaultOutputForHost<Terminal>(DefaultOutput.Logo)]
public abstract class StandardNukeBuild
    : NukeBuild,
        IUseStandardReleaseProcess,
        ICreateGitHubRelease
{
    public abstract string OriginalRepositoryName { get; }
    public abstract string MainReleaseBranch { get; }
    public abstract IEnumerable<Project> ProjectsToPack { get; }
    public abstract IEnumerable<IProvideLinter> Linters { get; }
    public abstract IEnumerable<Project> TestProjects { get; }
    public abstract bool SignReleaseTags { get; }

    public GitVersion GitVersion => From<IHazGitVersion>().Versioning;

    public string MajorMinorPatchVersion => GitVersion.MajorMinorPatch;

    public GitRepository CurrentRepository => From<IHazGitRepository>().GitRepository;

    public Solution CurrentSolution => From<IHazSolution>().Solution;

    public IEnumerable<AbsolutePath> NuGetPackages =>
        From<IPack>().PackagesDirectory.GlobFiles("*.nupkg");

    string ICreateGitHubRelease.Name => MajorMinorPatchVersion;

    IEnumerable<AbsolutePath> ICreateGitHubRelease.AssetFiles => NuGetPackages;

    Target ICreateGitHubRelease.CreateGitHubRelease =>
        _ =>
            _.Inherit<ICreateGitHubRelease>()
                .TriggeredBy<IPublish>(x => x.Publish)
                .ProceedAfterFailure()
                .OnlyWhenDynamic(() => From<IPublishPackagesToNuGet>().ShouldPublishToNuGet)
                .OnlyWhenStatic(() => CurrentRepository.IsOnMainOrMasterBranch());

    protected override void OnBuildInitialized()
    {
        var version = GitVersion.InformationalVersion;

        Serilog.Log.Information("BUILD SETUP");
        Serilog
            .Log
            .Information(
                "PackageOwner:   {Owner}",
                From<IPublishPackagesToNuGet>().PackageOwner ?? "N/A"
            );
        Serilog
            .Log
            .Information(
                "Configuration:  {Configuration}",
                From<IHazConfiguration>().Configuration
            );
        Serilog.Log.Information("Version:        {VersionSuffix}", version);
        Serilog
            .Log
            .Information(
                "TagVersion:     {TagVersion}",
                From<IFinalizeRelease>().TagVersion ?? "(null)"
            );
        Serilog
            .Log
            .Information("CommitMessage:  {CommitMessage}", From<IFinalizeRelease>().CommitMessage);
        Serilog
            .Log
            .Information(
                "Publish:        {ShouldPublish}",
                From<IPublishPackagesToNuGet>().ShouldPublishToNuGet
            );
    }

    public T From<T>()
        where T : INukeBuild => (T)(object)this;
}
