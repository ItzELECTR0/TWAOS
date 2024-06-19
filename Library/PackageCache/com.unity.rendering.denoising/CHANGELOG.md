# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).


## [1.0.5] - 2023-09-28

### Fixed
 - The native binaries now use the statically-linked runtime library (UNDB-1).

## [1.0.4] - 2023-07-21

### Added
 - Added Windows arm64 support. Only OIDN is supported there.
 - Updated OIDN backend to 1.4.2 on macOS. Windows already uses this version.

## [1.0.3] - 2023-06-09

### Removed
 - Removed Radeon denoiser backend.
 
## [1.0.2] - 2022-12-02

### Fixed
 - Fixed memory leak in command buffer denoiser (UNDB-8).

## [1.0.1] - 2022-05-12

### Fixed
 - OIDN backend now preserves the original alpha channel of the input color buffer (UNDB-4).
 
## [1.0.0] - 2022-01-12

### Fixed
 - Make dynamic libraries load correctly on macOS and Linux.
 - Optix backend, don't use kernel prediction for lightmaps. It was always turned on before, but it is only needed by the HDRP path tracer.

### Added
- Initial package release
