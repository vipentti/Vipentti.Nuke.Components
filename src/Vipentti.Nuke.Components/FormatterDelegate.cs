﻿// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/Vipentti.Nuke.Components/blob/main/LICENSE.md

using System;
using JetBrains.Annotations;

namespace Vipentti.Nuke.Components;

[PublicAPI]
public class FormatterDelegate : IProvideFormatter
{
    private readonly Action _install;
    private readonly Action _execute;

    public FormatterDelegate(Action install, Action execute)
    {
        _install = install;
        _execute = execute;
    }

    public void ExecuteFormatter() => _execute();

    public void InstallFormatter() => _install();
}
