﻿using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Components;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace Vipentti.Nuke.Components;

[PublicAPI]
public interface IUseDotNetFormat : INukeBuild
{
    // csharpier-ignore
    Target Format => _ => _
        .Executes(() =>
        {
            DotNetFormat(_ => _
                .Apply(FormatSettingsBase)
                .Apply(FormatSettings));
        });

    sealed Configure<DotNetFormatSettings> FormatSettingsBase => _ => _;

    Configure<DotNetFormatSettings> FormatSettings => _ => _;

    // csharpier-ignore
    Target ValidateFormat => _ => _
        .AssuredAfterFailure()
        .TryAfter<IUseDotNetFormat>(x => x.Format)
        .TryAfter<ITest>(x => x.Test)
        .TryBefore<IPack>(x => x.Pack)
        .Executes(ExecuteValidateFormat);

    sealed void ExecuteValidateFormat()
    {
        DotNetFormat(_ => _.Apply(ValidateFormatSettingsBase).Apply(ValidateFormatSettings));
    }

    sealed IProvideLinter Linter => new LinterDelegate(() => { }, ExecuteValidateFormat);

    sealed Configure<DotNetFormatSettings> ValidateFormatSettingsBase =>
        _ => _.SetVerifyNoChanges(true);

    Configure<DotNetFormatSettings> ValidateFormatSettings => _ => _;
}
