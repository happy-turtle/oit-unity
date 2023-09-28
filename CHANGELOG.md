# Changelog

## [3.0.1](https://github.com/happy-turtle/oit-unity/compare/3.0.0...v3.0.1) (2023-09-28)


### Bug Fixes

* add missing post process effect to samples ([#17](https://github.com/happy-turtle/oit-unity/issues/17)) ([7984a98](https://github.com/happy-turtle/oit-unity/commit/7984a98e737c2abd36fc14f6dcfc2c40d07292bc))

## 3.0.0

### Added

- Add High-Definition Render Pipeline implementation of Order-Independent Transparency
- Add Universal Render Pipeline implementation of Order-Independent Transparency
- Support Vulkan and OpenGL by using SV_SampleIndex for MSAA support

### Changed

- Restructure package for different render pipelines using Assemblies
- Changed package name to avoid using protected names
- Update project description and demo scenes
- Various small shader and interface improvements and updates

## 2.0.0

### Changed

- Created a custom package and restructured the whole project to make installation with the Unity package manager
  possible
- Improve performance by resetting linked list head buffer on GPU with a ComputeShader
