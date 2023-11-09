using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Git;
using static Nuke.Common.Tools.Git.GitTasks;

namespace Vipentti.Nuke.Components;

[PublicAPI]
public interface IFinalizeRelease : IFinishChangelog
{
    [Parameter]
    public string GitRemoteName => TryGetValue(() => GitRemoteName) ?? GitRepository.RemoteName;

    public string MainReleaseBranch { get; }

    public sealed string CommitMessage =>
        FinalizeReleaseUtils.GetCommitMessage(GitRepository.Commit);

    public sealed bool IsFinalizeCommit =>
        FinalizeReleaseUtils.IsFinalizeCommit(GitRepository.Commit);

    public sealed string? TagVersion => FinalizeReleaseUtils.TagVersion(GitRepository.Tags);

    public sealed bool IsTaggedBuild => !string.IsNullOrWhiteSpace(TagVersion);

    // csharpier-ignore
    public Target FinalizeRelease => _ => _
        .DependsOn(FinishChangelog)
        .Requires(() => GitHasCleanWorkingCopy() && GitRepository.IsOnMainOrMasterBranch())
        .Requires(() => Versioning)
        .Requires(() => GitRemoteName)
        .Executes(() =>
        {
            var MajorMinorPatchVersion = Versioning.MajorMinorPatch;
            Serilog.Log.Information("Using remote = {Remote}", GitRemoteName);
            Git($"tag -a {MajorMinorPatchVersion} -m \"Release {MajorMinorPatchVersion}\"");
            Git($"push {GitRemoteName} {MainReleaseBranch} {MajorMinorPatchVersion}");
        })
        .WhenSkipped(DependencyBehavior.Skip);
}
