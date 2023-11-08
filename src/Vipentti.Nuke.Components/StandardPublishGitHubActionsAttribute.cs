using System.Collections.Generic;
using System.Linq;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitHubActions.Configuration;
using Nuke.Common.Execution;
using Nuke.Components;

namespace Vipentti.Nuke.Components;

public class StandardPublishGitHubActionsAttribute : ExtendedGitHubActionsAttribute
{
    public static readonly string[] DefaultImportSecrets = new[] { nameof(IPublish.NuGetApiKey), };

    public static readonly string[] DefaultImportVars = new[]
    {
        nameof(IPublishPackagesToNuGet.PackageOwner),
        nameof(IPublish.NuGetSource),
    };

    public static readonly string[] DefaultInvokedTargets = new[]
    {
        nameof(ITest.Test),
        nameof(IUseLinters.InstallLinters),
        nameof(IUseLinters.Lint),
        nameof(IPack.Pack),
        nameof(IValidatePackages.ValidatePackages),
        nameof(IPublishPackagesToNuGet.Publish),
    };

    public const string DefaultPublishCondition = "${{ runner.os == 'Windows' }}";

    public StandardPublishGitHubActionsAttribute(
        string name,
        GitHubActionsImage image,
        params GitHubActionsImage[] images
    )
        : base(name, image, images)
    {
        PublishArtifacts = true;
        PublishCondition = DefaultPublishCondition;
        EmptyWorkflowTrigger = true;
        FetchDepth = 0;
    }

    protected override GitHubActionsJob GetJobs(
        GitHubActionsImage image,
        IReadOnlyCollection<ExecutableTarget> relevantTargets
    )
    {
        if (IncludeDefaultInvokedTargets)
        {
            InvokedTargets = InvokedTargets.Concat(DefaultInvokedTargets).Distinct().ToArray();
        }

        if (IncludeDefaultImportSecrets)
        {
            ImportSecrets = ImportSecrets.Concat(DefaultImportSecrets).Distinct().ToArray();
        }

        if (IncludeDefaultImportVars)
        {
            ImportVars = ImportVars.Concat(DefaultImportVars).Distinct().ToArray();
        }

        return base.GetJobs(image, relevantTargets);
    }

    public bool IncludeDefaultImportSecrets { get; set; } = true;
    public bool IncludeDefaultImportVars { get; set; } = true;
    public bool IncludeDefaultInvokedTargets { get; set; } = true;
}
