// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/Vipentti.Nuke.Components/blob/main/LICENSE.md

using Nuke.Common;

namespace Vipentti.Nuke.Components;

public class DisableDefaultOutputForHostAttribute<T> : DisableDefaultOutputAttribute
    where T : Host
{
    public DisableDefaultOutputForHostAttribute(params DefaultOutput[] disabledOutputs)
        : base(disabledOutputs) { }

    public override bool IsApplicable(INukeBuild build) => build.Host is T;
}
