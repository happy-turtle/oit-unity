# Changelog

## [5.0.0](https://github.com/happy-turtle/oit-unity/compare/v4.0.0...v5.0.0) (2024-01-10)


### ⚠ BREAKING CHANGES

* Update LICENSE ([#25](https://github.com/happy-turtle/oit-unity/issues/25))

### Documentation

* Update LICENSE ([#25](https://github.com/happy-turtle/oit-unity/issues/25)) ([851f89b](https://github.com/happy-turtle/oit-unity/commit/851f89bc361c258b44c319731ee363ecf130cd15))

## [4.0.0](https://github.com/happy-turtle/oit-unity/compare/v3.0.1...v4.0.0) (2023-12-30)


### ⚠ BREAKING CHANGES

* :fire: remove deprecated image effect implementation ([#23](https://github.com/happy-turtle/oit-unity/issues/23))

### Features

* UniversalRP Scene View ([#22](https://github.com/happy-turtle/oit-unity/issues/22)) ([6fb79a9](https://github.com/happy-turtle/oit-unity/commit/6fb79a906a67ca9f5323488241df82cabc4cfdb0))


### Code Refactoring

* :fire: remove deprecated image effect implementation ([#23](https://github.com/happy-turtle/oit-unity/issues/23)) ([0d4459c](https://github.com/happy-turtle/oit-unity/commit/0d4459c94866c500f2bd6a64d600ce9738569635))

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
