using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using Nuke.Components;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Vipentti.Nuke.Components;

[PublicAPI]
public interface IPackSpecificPackages : IPack
{
    IEnumerable<Project> ProjectsToPack { get; }

    [Parameter]
    bool CleanPackagesDirectory => true;

    IEnumerable<(Project Project, string Framework)> ICompile.PublishConfigurations =>
        from project in ProjectsToPack
        from framework in project.GetTargetFrameworks()
        select (project, framework);

    // csharpier-ignore
    Target IPack.Pack => _ => _
        .DependsOn<ICompile>(x => x.Compile)
        .TryDependsOn<ITest>(x => x.Test)
        .Produces(PackagesDirectory / "*.nupkg")
        .Executes(() =>
        {
            if (CleanPackagesDirectory)
            {
                PackagesDirectory.CreateOrCleanDirectory();
            }

            DotNetPack(
                _ =>
                    _.Apply(PackSettingsBase)
                        .Apply(PackSettings)
                        .CombineWith(ProjectsToPack, (_, v) => _.SetProject(v))
            );

            ReportSummary(_ => _.AddPair("Packages", PackagesDirectory.GlobFiles("*.nupkg").Count.ToString()));
        });
}
