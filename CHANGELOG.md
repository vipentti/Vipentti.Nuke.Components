# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.5.1] / 2024-10-28
- Update Nuke.* packages to 8.1.2

## [0.5.0] / 2024-02-18
- BREAKING: Renamed IUseDotNetFormat targets and properties
- BREAKING: Enable formatters and local tools by default in StandardNukeBuild
- Added IUseFormatters component
- Updated Nuke.* packages to 8.x

## [0.4.0] / 2023-11-15
- Add .NET 8 as a target framework option
- Implement support for installing specific .NET SDKS in GitHub actions

## [0.3.4] / 2023-11-10
- Fix links in README

## [0.3.3] / 2023-11-10
- Fix TagVersion detection and require tagged build for publishing

## [0.3.2] / 2023-11-10
- Support test results for multiple target frameworks

## [0.3.1] / 2023-11-10
- Improve ordering of Format target dependencies

## [0.3.0] / 2023-11-09
- Remove GitHub release debugging
- Enable GitHub releases by default in StandardNukeBuild

## [0.2.3] / 2023-11-09
- Enable default write permissions in StandardPublishGitHubActions
- Add DisableDefaultOutputForHostAttribute
- Add support for signing release tags

## [0.2.2] / 2023-11-09
- Revert changes: When using ICreateGitHubRelease do not create tag manually in IFinalizeRelease
- Debugging GitHub release creation

## [0.2.1] / 2023-11-09
- Provide some common properties in StandardNukeBuild
- When using ICreateGitHubRelease do not create tag manually in IFinalizeRelease

## [0.2.0] / 2023-11-09
- Remove VsCode as dependency in IFinishChangelog

## [0.1.0] / 2023-11-08
- Initial release

[Unreleased]: https://github.com/vipentti/Vipentti.Nuke.Components/compare/0.5.1...HEAD
[0.5.1]: https://github.com/vipentti/Vipentti.Nuke.Components/compare/0.5.0...0.5.1
[0.5.0]: https://github.com/vipentti/Vipentti.Nuke.Components/compare/0.4.0...0.5.0
[0.4.0]: https://github.com/vipentti/Vipentti.Nuke.Components/compare/0.3.4...0.4.0
[0.3.4]: https://github.com/vipentti/Vipentti.Nuke.Components/compare/0.3.3...0.3.4
[0.3.3]: https://github.com/vipentti/Vipentti.Nuke.Components/compare/0.3.2...0.3.3
[0.3.2]: https://github.com/vipentti/Vipentti.Nuke.Components/compare/0.3.1...0.3.2
[0.3.1]: https://github.com/vipentti/Vipentti.Nuke.Components/compare/0.3.0...0.3.1
[0.3.0]: https://github.com/vipentti/Vipentti.Nuke.Components/compare/0.2.3...0.3.0
[0.2.3]: https://github.com/vipentti/Vipentti.Nuke.Components/compare/0.2.2...0.2.3
[0.2.2]: https://github.com/vipentti/Vipentti.Nuke.Components/compare/0.2.1...0.2.2
[0.2.1]: https://github.com/vipentti/Vipentti.Nuke.Components/compare/0.2.0...0.2.1
[0.2.0]: https://github.com/vipentti/Vipentti.Nuke.Components/compare/0.1.0...0.2.0
[0.1.0]: https://github.com/vipentti/Vipentti.Nuke.Components/tree/0.1.0
