using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Components;

namespace Vipentti.Nuke.Components;

public abstract class StandardNukeBuild : NukeBuild, IUseStandardReleaseProcess
{
    public abstract string OriginalRepositoryName { get; }
    public abstract string MainReleaseBranch { get; }
    public abstract IEnumerable<Project> ProjectsToPack { get; }
    public abstract IEnumerable<IProvideLinter> Linters { get; }
    public abstract IEnumerable<Project> TestProjects { get; }

    protected override void OnBuildInitialized()
    {
        var version = From<IHazGitVersion>().Versioning.InformationalVersion;

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
