// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/Vipentti.Nuke.Components/blob/main/LICENSE.md

using System;
using System.Collections.Generic;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitHubActions.Configuration;
using Nuke.Common.Execution;
using Nuke.Common.Utilities;

namespace Vipentti.Nuke.Components;

public class ExtendedGitHubActionsAttribute : GitHubActionsAttribute
{
    public ExtendedGitHubActionsAttribute(
        string name,
        GitHubActionsImage image,
        params GitHubActionsImage[] images
    )
        : base(name, image, images) { }

    public bool EmptyWorkflowTrigger { get; set; }

    public string[] ImportVars { get; set; } = Array.Empty<string>();

    public string[] SetupDotnetVersions { get; set; } = Array.Empty<string>();

    protected override IEnumerable<(string Key, string Value)> GetImports()
    {
        foreach (var import in base.GetImports())
        {
            yield return import;
        }

        foreach (var variable in ImportVars)
        {
            yield return (variable, GetVariableValue(variable));
        }

        static string GetVariableValue(string variable) =>
            $"${{{{ vars.{variable.SplitCamelHumpsWithKnownWords().JoinUnderscore().ToUpperInvariant()} }}}}";
    }

    protected override GitHubActionsJob GetJobs(
        GitHubActionsImage image,
        IReadOnlyCollection<ExecutableTarget> relevantTargets
    )
    {
        var job = base.GetJobs(image, relevantTargets);

        if (SetupDotnetVersions.Length > 0)
        {
            var versionStep = new SetupDotnetVersionsStep(SetupDotnetVersions);
            job.Steps = [job.Steps[0], versionStep, .. job.Steps[1..]];
        }

        return job;
    }

    protected override IEnumerable<GitHubActionsDetailedTrigger> GetTriggers()
    {
        foreach (var trigger in base.GetTriggers())
        {
            yield return trigger;
        }

        if (EmptyWorkflowTrigger)
        {
            yield return new EmptyGitHubActionsWorkflowDispatchTrigger();
        }
    }

    class SetupDotnetVersionsStep(string[] Versions) : GitHubActionsStep
    {
        public override void Write(CustomFileWriter writer)
        {
            writer.WriteLine("- name: Setup dotnet");
            writer.WriteLine("  uses: actions/setup-dotnet@v3");
            writer.WriteLine("  with:");
            using (writer.Indent())
            using (writer.Indent())
            {
                writer.WriteLine("dotnet-version: |");
                using (writer.Indent())
                {
                    foreach (var item in Versions)
                    {
                        writer.WriteLine(item);
                    }
                }
            }
        }
    }

    class EmptyGitHubActionsWorkflowDispatchTrigger : GitHubActionsDetailedTrigger
    {
        public override void Write(CustomFileWriter writer)
        {
            writer.WriteLine(
                "workflow_dispatch: # Allow running the workflow manually from the GitHub UI"
            );
        }
    }
}
