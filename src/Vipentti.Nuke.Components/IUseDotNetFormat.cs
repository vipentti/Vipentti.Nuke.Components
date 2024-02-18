// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/Vipentti.Nuke.Components/blob/main/LICENSE.md

using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Components;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Vipentti.Nuke.Components;

[PublicAPI]
public interface IUseDotNetFormat : INukeBuild
{
    sealed Configure<DotNetFormatSettings> FormatSettingsBase => _ => _;

    Configure<DotNetFormatSettings> FormatSettings => _ => _;

    // csharpier-ignore
    Target FormatDotNet => _ => _
        .TryBefore<IRestore>(x => x.Restore)
        .TryBefore<ICompile>(x => x.Compile)
        .TryBefore<IUseLinters>(x => x.Lint)
        .Executes(ExecuteDotNetFormat);

    sealed void ExecuteDotNetFormat()
    {
        DotNetFormat(_ => _.Apply(FormatSettingsBase).Apply(FormatSettings));
    }

    sealed IProvideFormatter Formatter => new FormatterDelegate(() => { }, ExecuteDotNetFormat);

    // csharpier-ignore
    Target CheckDotNetFormat => _ => _
        .AssuredAfterFailure()
        .TryAfter<IUseDotNetFormat>(x => x.FormatDotNet)
        .TryAfter<ITest>(x => x.Test)
        .TryBefore<IPack>(x => x.Pack)
        .Executes(ExecuteCheckDotNetFormat);

    sealed void ExecuteCheckDotNetFormat()
    {
        DotNetFormat(_ => _.Apply(CheckDotNetFormatSettingsBase).Apply(CheckDotNetFormatSettings));
    }

    sealed IProvideLinter Linter => new LinterDelegate(() => { }, ExecuteCheckDotNetFormat);

    sealed Configure<DotNetFormatSettings> CheckDotNetFormatSettingsBase =>
        _ => _.SetVerifyNoChanges(true);

    Configure<DotNetFormatSettings> CheckDotNetFormatSettings => _ => _;
}
