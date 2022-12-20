# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [3.0.0]

### Added

- Add High-Definition Render Pipeline implementation of Order-Independent Transparency
- Add Universal Render Pipeline implementation of Order-Independent Transparency
- Support Vulkan and OpenGL by using SV_SampleIndex for MSAA support

### Changed

- Restructure package for different render pipelines using Assemblies
- Changed package name to avoid using protected names
- Update project description and demo scenes
- Various small shader and interface improvements and updates

## [2.0.0]

### Changed

- Created a custom package and restructured the whole project to make installation with the Unity package manager
  possible
- Improve performance by resetting linked list head buffer on GPU with a ComputeShader
