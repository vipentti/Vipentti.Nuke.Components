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
        ITest
// csharpier-ignore
{ }
