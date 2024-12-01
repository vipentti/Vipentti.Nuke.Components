// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/Vipentti.Nuke.Components/blob/main/LICENSE.md

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nuke.Common.Tooling;
using static Nuke.Common.Tools.Git.GitTasks;

namespace Vipentti.Nuke.Components;

public static partial class FinalizeReleaseUtils
{
    [GeneratedRegex(@"\d+\.\d+\.\d+", RegexOptions.Compiled)]
    private static partial Regex TaggedBuildRegexHelper();

    [GeneratedRegex(@"Finalize CHANGELOG\.md for \d+\.\d+\.\d+", RegexOptions.Compiled)]
    private static partial Regex FinalizeChangeLogRegexHelper();

    public static readonly Func<IProcess, object?> NoopExit = (_) => null;
    public static readonly Regex FinalizeChangeLogRegex = FinalizeChangeLogRegexHelper();

    public static readonly Regex TaggedBuildRegex = TaggedBuildRegexHelper();

    public static string? GetTagVersion(IEnumerable<string> tags) =>
        tags.SingleOrDefault(TaggedBuildRegex.IsMatch);

    public static bool GetIsFinalizeCommit(string commitHash)
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

    public static IEnumerable<string> GetTagsPointingAtCommit(string hash)
    {
        var output = Git(
            $"tag --points-at {hash}",
            logOutput: false,
            logInvocation: false,
            exitHandler: NoopExit
        );

        var stdout = output.StdToText() ?? "";

        return stdout.Split(
            Environment.NewLine,
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
        );
    }

    public static string GetCommitHashForTag(string tag)
    {
        var output = Git(
            $"rev-list -1 {tag}",
            logOutput: false,
            logInvocation: false,
            exitHandler: NoopExit
        );

        return output.StdToText() ?? "";
    }
}
