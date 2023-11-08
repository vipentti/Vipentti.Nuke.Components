using System.Collections.Generic;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Components;

namespace Vipentti.Nuke.Components;

[PublicAPI]
public interface IUseLinters : INukeBuild
{
    IEnumerable<IProvideLinter> Linters { get; }

    // csharpier-ignore
    Target InstallLinters => _ => _
        .Before(Lint)
        .Executes(() =>
        {
            var lintSuccess = true;
            foreach (var item in Linters)
            {
                try
                {
                    item.InstallLinter();
                }
                catch
                {
                    lintSuccess = false;
                }
            }

            Assert.True(lintSuccess);
        });

    // csharpier-ignore
    Target Lint => _ => _
        .AssuredAfterFailure()
        .TryAfter<ICompile>(x => x.Compile)
        .TryAfter<ITest>(x => x.Test)
        .TryBefore<IPack>(x => x.Pack)
        .Executes(() =>
        {
            var lintSuccess = true;
            foreach (var item in Linters)
            {
                try
                {
                    item.ExecuteLinter();
                }
                catch
                {
                    lintSuccess = false;
                }
            }

            Assert.True(lintSuccess);
        });
}
