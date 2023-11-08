using System;
using System.Collections.Generic;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitHubActions.Configuration;
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
