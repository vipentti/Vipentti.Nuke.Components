// Copyright 2023 Ville Penttinen
// Distributed under the MIT License.
// https://github.com/vipentti/Vipentti.Nuke.Components/blob/main/LICENSE.md

using System.Collections.Generic;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Components;

namespace Vipentti.Nuke.Components;

[PublicAPI]
public interface IUseFormatters : INukeBuild
{
    IEnumerable<IProvideFormatter> Formatters { get; }

    // csharpier-ignore
    Target InstallFormatters => _ => _
        .Before(Format)
        .Executes(() =>
        {
            var success = true;
            foreach (var item in Formatters)
            {
                try
                {
                    item.InstallFormatter();
                }
                catch
                {
                    success = false;
                }
            }

            Assert.True(success);
        });

    // csharpier-ignore
    Target Format => _ => _
        .AssuredAfterFailure()
        .TryBefore<IRestore>(x => x.Restore)
        .TryBefore<ICompile>(x => x.Compile)
        .TryBefore<IUseLinters>(x => x.Lint)
        .Executes(() =>
        {
            var success = true;
            foreach (var item in Formatters)
            {
                try
                {
                    item.ExecuteFormatter();
                }
                catch
                {
                    success = false;
                }
            }

            Assert.True(success);
        });
}
