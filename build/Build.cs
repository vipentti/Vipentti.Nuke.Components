﻿using System.Collections.Generic;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Components;
using Vipentti.Nuke.Components;
using static Vipentti.Nuke.Components.StandardNames;

namespace build;

[GitHubActions(
    "pull-request",
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    GitHubActionsImage.MacOsLatest,
    OnPullRequestBranches = new[] { MainBranch, DevelopBranch },
    PublishArtifacts = false,
    FetchDepth = 0, // fetch full history
    InvokedTargets = new[]
    {
        nameof(ITest.Test),
        nameof(IUseLinters.InstallLinters),
        nameof(IUseLinters.Lint),
        nameof(IValidatePackages.ValidatePackages),
    })]
[StandardPublishGitHubActions(
    "publish",
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    GitHubActionsImage.MacOsLatest
    , OnPushBranches = new[] { MainBranch }
    , EnableGitHubToken = true
)]
class Build : StandardNukeBuild, IUseCsharpier, ICreateGitHubRelease
{
    public override string OriginalRepositoryName { get; } = "Vipentti.Nuke.Components";
    public override string MainReleaseBranch { get; } = MainBranch;
    public override IEnumerable<Project> ProjectsToPack =>
        From<IHazSolution>().Solution.GetAllProjects("Vipentti.Nuke.Components");
    public override IEnumerable<IProvideLinter> Linters => new[]
    {
        From<IUseDotNetFormat>().Linter,
        From<IUseCsharpier>().Linter,
    };
    bool IUseCsharpier.UseGlobalTool { get; } = true;

    public override IEnumerable<Project> TestProjects { get; } = Enumerable.Empty<Project>();

    string ICreateGitHubRelease.Name => From<IHazGitVersion>().Versioning.MajorMinorPatch;
    IEnumerable<AbsolutePath> ICreateGitHubRelease.AssetFiles => From<IPack>().PackagesDirectory.GlobFiles("*.nupkg");
    Target ICreateGitHubRelease.CreateGitHubRelease => _ => _
        .Inherit<ICreateGitHubRelease>()
        .TriggeredBy<IPublish>(x => x.Publish)
        .ProceedAfterFailure()
        .OnlyWhenDynamic(() => From<IPublishPackagesToNuGet>().ShouldPublishToNuGet)
        .OnlyWhenStatic(() => From<IHazGitRepository>().GitRepository.IsOnMainOrMasterBranch());

    // Support plugins are available for:
    //   - JetBrains ReSharper        https://nuke.build/resharper
    //   - JetBrains Rider            https://nuke.build/rider
    //   - Microsoft VisualStudio     https://nuke.build/visualstudio
    //   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.From<ICompile>().Compile);
}
