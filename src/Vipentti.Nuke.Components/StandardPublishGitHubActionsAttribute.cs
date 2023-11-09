// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/Vipentti.Nuke.Components/blob/main/LICENSE.md

using System;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common.CI;
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

    public static readonly GitHubActionsPermissions[] DefaultReadPermissions =
        Array.Empty<GitHubActionsPermissions>();

    public static readonly GitHubActionsPermissions[] DefaultWritePermissions = new[]
    {
        GitHubActionsPermissions.Actions,
        GitHubActionsPermissions.Checks,
        GitHubActionsPermissions.Contents,
        GitHubActionsPermissions.Deployments,
        GitHubActionsPermissions.Issues,
        GitHubActionsPermissions.Discussions,
        GitHubActionsPermissions.Packages,
        GitHubActionsPermissions.Pages,
        GitHubActionsPermissions.Statuses,
    };

    public const string DefaultPublishCondition = "${{ runner.os == 'Windows' }}";

    public StandardPublishGitHubActionsAttribute(
        string name,
        GitHubActionsImage image,
        params GitHubActionsImage[] images
    )
        : base(name, image, images)
    {
        WritePermissions = DefaultWritePermissions;
        PublishArtifacts = true;
        PublishCondition = DefaultPublishCondition;
        EmptyWorkflowTrigger = true;
        FetchDepth = 0;
    }

    public override ConfigurationEntity GetConfiguration(
        IReadOnlyCollection<ExecutableTarget> relevantTargets
    )
    {
        if (IncludeDefaultWritePermissions)
        {
            WritePermissions = WritePermissions
                .Concat(DefaultWritePermissions)
                .Distinct()
                .ToArray();
        }

        if (IncludeDefaultReadPermissions)
        {
            ReadPermissions = ReadPermissions.Concat(DefaultReadPermissions).Distinct().ToArray();
        }

        return base.GetConfiguration(relevantTargets);
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

    public bool IncludeDefaultReadPermissions { get; set; } = true;
    public bool IncludeDefaultWritePermissions { get; set; } = true;
    public bool IncludeDefaultImportSecrets { get; set; } = true;
    public bool IncludeDefaultImportVars { get; set; } = true;
    public bool IncludeDefaultInvokedTargets { get; set; } = true;
}
