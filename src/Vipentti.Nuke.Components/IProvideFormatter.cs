// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/Vipentti.Nuke.Components/blob/main/LICENSE.md

using JetBrains.Annotations;

namespace Vipentti.Nuke.Components;

[PublicAPI]
public interface IProvideFormatter
{
    void InstallFormatter();
    void ExecuteFormatter();
}
