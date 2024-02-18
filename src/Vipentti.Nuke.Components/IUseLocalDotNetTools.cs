// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/Vipentti.Nuke.Components/blob/main/LICENSE.md

using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Components;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Vipentti.Nuke.Components;

[PublicAPI]
public interface IUseLocalDotNetTools : INukeBuild
{
    AbsolutePath ToolsManifestPath => RootDirectory / ".config" / "dotnet-tools.json";

    sealed bool HasToolsManifest => ToolsManifestPath.Exists("file");

    // csharpier-ignore
    Target RestoreLocalTools => _ => _
        .OnlyWhenDynamic(() => HasToolsManifest)
        .TryDependentFor<IRestore>(x => x.Restore)
        .Executes(() => DotNetToolRestore(_ => _.Apply(ToolRestoreSettingsBase)));

    // csharpier-ignore
    sealed Configure<DotNetToolRestoreSettings> ToolRestoreSettingsBase => _ => _
        .SetToolManifest(ToolsManifestPath);
}
