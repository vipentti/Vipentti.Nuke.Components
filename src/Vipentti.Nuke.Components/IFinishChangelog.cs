using System;
using System.IO;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Components;
using static Nuke.Common.ChangeLog.ChangelogTasks;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.Git.GitTasks;

namespace Vipentti.Nuke.Components;

[PublicAPI]
public interface IFinishChangelog : IHazChangelog, IHazGitRepository, IHazGitVersion
{
    Tool VsCode => ToolResolver.GetEnvironmentOrPathTool("code");

    // csharpier-ignore
    Target FinishChangelog => _ => _
        .Requires(() => GitRepository.IsOnMainOrMasterBranch() && GitHasCleanWorkingCopy())
        .Requires(() => Versioning)
        .Executes(() =>
        {
            var MajorMinorPatchVersion = Versioning.MajorMinorPatch;
            var changelogFile = AbsolutePath.Create(this.ChangelogFile);

            var tempFile = TemporaryDirectory / (Path.GetRandomFileName() + ".md");
            try
            {
                CopyFile(changelogFile, tempFile);

                FinalizeChangelog(tempFile, MajorMinorPatchVersion, GitRepository);

                Git(
                    $"diff --no-index {changelogFile} {tempFile}",
                    logOutput: true,
                    logInvocation: true,
                    exitHandler: FinalizeReleaseUtils.NoopExit
                );

                Serilog.Log.Information("Do you want to view the diff in VsCode (y/n)?");

                if (Console.ReadKey(intercept: true).KeyChar == 'y')
                {
                    VsCode(
                        arguments: $"--wait --diff {changelogFile} {tempFile}",
                        logOutput: true,
                        logInvocation: true
                    );
                }

                Serilog
                    .Log
                    .Information(
                        "Preparing changelog {path}. Are you sure you want to continue (y/n)?",
                        changelogFile.Name
                    );
                var yesNo = Console.ReadKey(intercept: true).KeyChar;
                Assert.True(yesNo == 'y');

                // Finalize the actual changelog
                FinalizeChangelog(changelogFile, MajorMinorPatchVersion, GitRepository);
                Serilog.Log.Information("Please review {Path} and press any key to continue...", changelogFile);
                Console.ReadKey(intercept: true);

                Git($"add {changelogFile}");
                Git($"commit -m \"Finalize {Path.GetFileName(changelogFile)} for {MajorMinorPatchVersion}\"");

                return 0;
            }
            finally
            {
                if (tempFile.Exists())
                {
                    Serilog.Log.Debug("Cleaning up {path}", tempFile);
                    tempFile.DeleteFile();
                }
            }
        });
}
