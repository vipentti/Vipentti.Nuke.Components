// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/Vipentti.Nuke.Components/blob/main/LICENSE.md

using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Git;
using static Nuke.Common.Tools.Git.GitTasks;
using static Vipentti.Nuke.Components.FinalizeReleaseUtils;

namespace Vipentti.Nuke.Components;

[PublicAPI]
public interface IFinalizeRelease : IFinishChangelog
{
    [Parameter]
    public string GitRemoteName => TryGetValue(() => GitRemoteName) ?? GitRepository.RemoteName;

    public string MainReleaseBranch { get; }

    public bool SignReleaseTags { get; }

    public sealed string CommitMessage => GetCommitMessage(GitRepository.Commit);

    public sealed bool IsFinalizeCommit => GetIsFinalizeCommit(GitRepository.Commit);

    public sealed string? TagVersion =>
        GetTagVersion(GetTagsPointingAtCommit(GitRepository.Commit));

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

            if (SignReleaseTags)
            {
                Git($"tag -s {MajorMinorPatchVersion} -m \"Release {MajorMinorPatchVersion}\"");
                Git($"tag -v {MajorMinorPatchVersion}");
            }
            else
            {
                Git($"tag -a {MajorMinorPatchVersion} -m \"Release {MajorMinorPatchVersion}\"");
            }

            Git($"push {GitRemoteName} {MainReleaseBranch} {MajorMinorPatchVersion}");
        })
        .WhenSkipped(DependencyBehavior.Skip);
}
