# Order-independent Transparency in Unity using Per-Pixel Linked Lists

![Animation2](https://user-images.githubusercontent.com/18415215/139141230-207014ab-57eb-4591-9c90-d8c17db93a30.gif)

---------------

![Animation](https://user-images.githubusercontent.com/18415215/139123916-2dce99a6-aefe-437a-9caf-cb105015e654.gif)
![Animation2](https://user-images.githubusercontent.com/18415215/139123918-600efbe8-96ab-475e-be68-51e2e026d434.gif)

Both animations show three stacked transparent cubes. On the left with this implementation of order-independent transparency and correct color blending. On the right rendered with the standard shader.

## Description

This is an implementation of order-independent transparency in Unity's Built-In Pipeline. It uses Per-Pixel Linked Lists, implemented with RWStructuredBuffers.
This is a feature requiring Shader Model 5.0 with ComputeBuffers, see the [Unity Manual](https://docs.unity3d.com/Manual/SL-ShaderCompileTargets.html) for supported platforms.
For reference a [presentation by Holger Gruen and Nicolas Thibieroz](https://de.slideshare.net/hgruen/oit-and-indirect-illumination-using-dx11-linked-lists)
was used. The code is based on their suggestions.
