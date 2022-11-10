# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

-   Use SV_SampleIndex for MSAA support to run on Vulkan and OpenGL

### Changed

-   [Breaking] Use CommandBuffer in CameraComponent and OitInterface
-   Remove conditional compilations for PostProcessing package

## [2.0.0]

### Changed

-   Created a custom package and restructured the whole project to make installation with the Unity package manager possible
-   Improve performance by resetting linked list head buffer on GPU with a ComputeShader
