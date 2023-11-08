using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
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
        .Executes(() => DotNetToolRestore());

    // csharpier-ignore
    sealed Configure<DotNetToolRestoreSettings> ToolRestoreSettingsBase => _ => _
        .SetConfigFile(ToolsManifestPath);
}
