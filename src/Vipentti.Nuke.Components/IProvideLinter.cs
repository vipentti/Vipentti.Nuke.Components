using JetBrains.Annotations;

namespace Vipentti.Nuke.Components;

[PublicAPI]
public interface IProvideLinter
{
    void InstallLinter();
    void ExecuteLinter();
}
