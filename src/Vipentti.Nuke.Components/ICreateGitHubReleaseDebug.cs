using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.ChangeLog;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Utilities;
using Nuke.Components;

namespace Vipentti.Nuke.Components;

[PublicAPI]
public interface ICreateGitHubReleaseDebug : ICreateGitHubRelease
{
    // csharpier-ignore
    new Target CreateGitHubRelease => _ => _
        .Requires(() => GitHubToken)
        .Executes(async () =>
        {
            async Task<Octokit.Release> GetOrCreateRelease()
            {
                try
                {
                    return await GitHubTasks.GitHubClient.Repository.Release.Create(
                        GitRepository.GetGitHubOwner(),
                        GitRepository.GetGitHubName(),
                        new Octokit.NewRelease(Name)
                        {
                            Name = Name,
                            Prerelease = Prerelease,
                            Draft = Draft,
                            Body = ChangelogTasks.ExtractChangelogSectionNotes(ChangelogFile).JoinNewLine()
                        });
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, "Failed to create a new release");
                    return await GitHubTasks.GitHubClient.Repository.Release.Get(
                        GitRepository.GetGitHubOwner(),
                        GitRepository.GetGitHubName(),
                        Name);
                }
            }

            GitHubTasks.GitHubClient.Credentials = new Octokit.Credentials(GitHubToken.NotNull());

            var release = await GetOrCreateRelease();

            var uploadTasks = AssetFiles.Select(async x =>
            {
                await using var assetFile = File.OpenRead(x);
                var asset = new Octokit.ReleaseAssetUpload
                {
                    FileName = x.Name,
                    ContentType = "application/octet-stream",
                    RawData = assetFile
                };
                await GitHubTasks.GitHubClient.Repository.Release.UploadAsset(release, asset);
            }).ToArray();

            Task.WaitAll(uploadTasks);
        });
}
