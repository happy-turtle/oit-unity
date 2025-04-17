# Changelog

## [5.2.1](https://github.com/happy-turtle/oit-unity/compare/v5.2.0...v5.2.1) (2025-04-17)


### Bug Fixes

* remove broken deferred pass ([#49](https://github.com/happy-turtle/oit-unity/issues/49)) ([6cefa29](https://github.com/happy-turtle/oit-unity/commit/6cefa2934245ca911ff7809424b485427774deb7))

## [5.2.0](https://github.com/happy-turtle/oit-unity/compare/v5.1.0...v5.2.0) (2024-06-20)


### Features

* **hdrp:** add sample Lit shader ([#37](https://github.com/happy-turtle/oit-unity/issues/37)) ([975b5bb](https://github.com/happy-turtle/oit-unity/commit/975b5bbd324c98ef3a9cd9c0f4f1cad8dbc84099))
* lower shader target level to 4.5 ([975b5bb](https://github.com/happy-turtle/oit-unity/commit/975b5bbd324c98ef3a9cd9c0f4f1cad8dbc84099))


### Bug Fixes

* prevent asset import errors by adding appropriate shader package requirements and fallbacks ([975b5bb](https://github.com/happy-turtle/oit-unity/commit/975b5bbd324c98ef3a9cd9c0f4f1cad8dbc84099))
* **urp:** don't setup urp buffers if screen properties are not yet present ([975b5bb](https://github.com/happy-turtle/oit-unity/commit/975b5bbd324c98ef3a9cd9c0f4f1cad8dbc84099))

## [5.1.0](https://github.com/happy-turtle/oit-unity/compare/v5.0.1...v5.1.0) (2024-06-02)


### Features

* add URP Lit shader example ([#35](https://github.com/happy-turtle/oit-unity/issues/35)) ([7f77cd9](https://github.com/happy-turtle/oit-unity/commit/7f77cd938ebc6baef1540d9a4f9545fc471bbf65))

## [5.0.1](https://github.com/happy-turtle/oit-unity/compare/v5.0.0...v5.0.1) (2024-04-26)


### Bug Fixes

* **URP:** fix missing shader on URP build ([#30](https://github.com/happy-turtle/oit-unity/issues/30)) ([91cdcf8](https://github.com/happy-turtle/oit-unity/commit/91cdcf8610cfaf04e6b4c2c7e55d7abb054d4066))

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
