using System;

namespace Vipentti.Nuke.Components;

public class LinterDelegate : IProvideLinter
{
    private readonly Action _install;
    private readonly Action _execute;

    public LinterDelegate(Action install, Action execute)
    {
        _install = install;
        _execute = execute;
    }

    public void ExecuteLinter() => _execute();

    public void InstallLinter() => _install();
}
