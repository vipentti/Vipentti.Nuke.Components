using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Components;

namespace Vipentti.Nuke.Components;

[PublicAPI]
public interface IPublishPackagesToNuGet : IPublish, IFinalizeRelease, IValidatePackages
{
    [Parameter("Owner of the package(s)")]
    string PackageOwner => TryGetValue(() => PackageOwner);

    string OriginalRepositoryName { get; }

    bool ShouldPublishToNuGet =>
        GitRepository.IsOnMainOrMasterBranch()
        && IsOriginalRepository
        && RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        && IsFinalizeCommit;

    sealed bool IsOriginalRepository =>
        GitRepository.Identifier == $"{PackageOwner}/{OriginalRepositoryName}";

    // csharpier-ignore
    Target IPublish.Publish => _ => _
        .Requires(() => PackageOwner)
        .OnlyWhenDynamic(() => ShouldPublishToNuGet)
        .DependsOn<IValidatePackages>(x => x.ValidatePackages)
        .Inherit<IPublish>()
        .WhenSkipped(DependencyBehavior.Execute);

    Configure<DotNetNuGetPushSettings> IPublish.PackagePushSettings =>
        _ => _.SetSkipDuplicate(true);
}
