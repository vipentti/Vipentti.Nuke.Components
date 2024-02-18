// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/Vipentti.Nuke.Components/blob/main/LICENSE.md

using JetBrains.Annotations;
using Nuke.Components;

namespace Vipentti.Nuke.Components;

/// <summary>
/// Standard release process
/// </summary>
[PublicAPI]
public interface IUseStandardReleaseProcess
    : IUseReproducibleBuild,
        IPublishPackagesToNuGet,
        IPackSpecificPackages,
        IValidatePackages,
        IUseDotNetFormat,
        IUseLinters,
        IUseFormatters,
        IUseLocalDotNetTools,
        ITestWithMultipleFrameworks
// csharpier-ignore
{ }
