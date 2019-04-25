# UnityFx.Tasks changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/); this project adheres to [Semantic Versioning](http://semver.org/).

-----------------------
## [0.2.0] - unreleased

### Added
- Added `ConfigureAwait` extensions for built-in Unity async operations.
- Added `ToTask` conversions to `AsyncOperation`.
- Added support for loading arrays of objects from asset bundles via `LoadAllAssetsTaskAsync` extension methds.
- Added `TaskUtility.YieldToUnityThread` method that can yield execution to Unity thread.
- Added examples.

### Fixed
- Fixed existing `ToTask` extension methods to unload loaded assets if the operation was cancelled.
- Fixed several issues in `TaskUtility.LoadSceneAsync` implementation.

### Removed
- Removed default `SynchronizationContext` implementation (Unity 2017.2 already has one).

-----------------------
## [0.1.0] - 2019.04.07

### Added
- Initial release.
