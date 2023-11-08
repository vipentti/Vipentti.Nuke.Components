using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nuke.Common.Tooling;
using static Nuke.Common.Tools.Git.GitTasks;

namespace Vipentti.Nuke.Components;

public static class FinalizeReleaseUtils
{
    public static readonly Action<IProcess> NoopExit = (_) => { };
    public static readonly Regex FinalizeChangeLogRegex =
        new(@"Finalize CHANGELOG\.md for \d+\.\d+\.\d+", RegexOptions.Compiled);
    public static readonly Regex TaggedBuildRegex = new(@"\d+\.\d+\.\d+", RegexOptions.Compiled);

    public static string? TagVersion(IEnumerable<string> tags) =>
        tags.SingleOrDefault(TaggedBuildRegex.IsMatch);

    public static bool IsFinalizeCommit(string commitHash)
    {
        var message = GetCommitMessage(commitHash);
        return FinalizeChangeLogRegex.IsMatch(message);
    }

    public static string GetCommitMessage(string hash)
    {
        var output = Git(
            $"show --pretty=format:\"%s\" -s \"{hash}\"",
            logOutput: false,
            logInvocation: false,
            exitHandler: NoopExit
        );
        return output.StdToText() ?? "";
    }
}
