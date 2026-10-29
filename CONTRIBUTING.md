# Contributing to Bimo.Tools

Thanks for helping improve the BIMO public SDK.

1. Open an issue describing the public API change, compatibility impact and use case.
2. Keep changes limited to host-neutral SDK contracts. Do not contribute proprietary Revit/BIMO implementation code, customer materials, credentials or binaries.
3. Run `dotnet build Bimo.Tools.csproj -c Release` and both smoke consumer modes before submitting a PR: `-p:UsePublishedPackage=false` (reviewed source) and `-p:UsePublishedPackage=true` (published SDK 1.3.0).
4. Document any behavior or package compatibility change. Start with the [developer guide](docs/command-authoring.md) and [Hello BIMO Quick Start](docs/quick-start.md) when updating examples.

**Stage 1 source policy:** the private BIMO monorepo remains the canonical source. A maintainer reviews public PRs and ports accepted changes upstream; a subsequent reviewed export can then publish them here. Direct public PR merges must not trigger NuGet publication. This is temporary until an explicit source-of-truth migration.

The package follows the MIT license in [LICENSE](LICENSE).
