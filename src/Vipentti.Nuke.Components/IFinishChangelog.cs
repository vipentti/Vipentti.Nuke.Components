// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/Vipentti.Nuke.Components/blob/main/LICENSE.md

using System;
using System.IO;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Components;
using static Nuke.Common.ChangeLog.ChangelogTasks;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.Git.GitTasks;

namespace Vipentti.Nuke.Components;

[PublicAPI]
public interface IFinishChangelog : IHazChangelog, IHazGitRepository, IHazGitVersion
{
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
                changelogFile.Copy(tempFile);

                FinalizeChangelog(tempFile, MajorMinorPatchVersion, GitRepository);

                Git(
                    $"diff --no-index {changelogFile} {tempFile}",
                    logOutput: true,
                    logInvocation: true,
                    exitHandler: FinalizeReleaseUtils.NoopExit
                );

                Serilog.Log.Information("Do you want to view the diff using git difftool (y/n)?");

                if (Console.ReadKey(intercept: true).KeyChar == 'y')
                {
                    Git(
                        $"difftool --no-prompt --no-index {changelogFile} {tempFile}",
                        logOutput: true,
                        logInvocation: true,
                        exitHandler: FinalizeReleaseUtils.NoopExit
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
