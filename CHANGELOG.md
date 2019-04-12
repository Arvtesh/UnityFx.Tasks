# UnityFx.Tasks changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/); this project adheres to [Semantic Versioning](http://semver.org/).

-----------------------
## [0.2.0] - unreleased

### Added
- Added `ConfigureAwait` extensions for built-in Unity async operations.
- Added `ToTask` conversions to `AsyncOperation`.
- Added support for loading arrays of objects from asset bundles via `LoadAllAssetsTaskAsync` extension methds.

### Fixed
- Fixed existing `ToTask` extension methods to unload loaded assets if the operation was cancelled.

-----------------------
## [0.1.0] - 2019.04.07

### Added
- Initial release.
