# Order-independent Transparency in Unity

![Comparison](Screenshots/Comparison.gif) ![Animation2](https://user-images.githubusercontent.com/18415215/139141230-207014ab-57eb-4591-9c90-d8c17db93a30.gif)

## Description

This is an implementation of order-independent transparency in Unity's **Built-In Pipeline**. It uses Per-Pixel Linked Lists, implemented with RWStructuredBuffers.
This is a feature requiring Shader Model 5.0 with ComputeBuffers, see the [Unity Manual](https://docs.unity3d.com/Manual/SL-ShaderCompileTargets.html) for supported platforms.
For reference a [presentation by Holger Gruen and Nicolas Thibieroz](https://de.slideshare.net/hgruen/oit-and-indirect-illumination-using-dx11-linked-lists)
was used. The code is based on their suggestions.

## Installation

You can easily install this package with the Unity Package Manager using the project's git url. You can look at [Unity's guide](https://docs.unity3d.com/Manual/upm-ui-giturl.html) for detailed instructions.

## Usage

I recommend using this with the package Post-Processing Stack v2, because then transparent objects will be rendered in Scene View as well. Follow these steps:

1. Add the post-processing override `Order Independent Transparency` to a post-processing volume in your scene.
2. Change the shaders of every object that shall be rendered with order-independent transparency. They have to have a material using a custom shader. Two sample shaders that you can use are included in this project `OrderIndependentTransparency/Unlit` and `OrderIndependentTransparency/Standard` (standard shader has to be set to Rendering Mode Transparent).
3. Run your scene.

### Without Post-Processing Stack v2

If you don't want to use the post-processing stack just skip the first step and instead add the component `OitCameraComponent` to a camera. Transparent objects won't render in the editor scene view using this component.

## Contributions

-   I consider this an open project. If you are interested in this topic or want to improve something please discuss, contribute and feel at home! :house:
-   Feel free to open a [discussion](https://github.com/happy-turtle/oit-unity/discussions) or an issue if you have ideas and improvements in mind.
-   Pull requests are very welcome, see the [issues section](https://github.com/happy-turtle/oit-unity/issues) for open tasks that would improve this project.

## Platforms tested (Unity 2021.3.9f1)

| Platform | Graphics Backend |       Render output       |
| :------- | :--------------: | :-----------------------: |
|          |
| Windows  |    DirectX 12    |    :white_check_mark:     |
|          |    DirectX 11    |    :white_check_mark:     |
|          |      Vulkan      |  :ok: (render artifacts)  |
|          |    OpenGLCore    | :ok: (performance issues) |
|          |    OpenGLES3     |   :x: (editor crashes)    |
|          |
| Linux    |      Vulkan      |        :question:         |
|          |    OpenGLCore    |        :question:         |
|          |
| Mac      |      Metal       |        :question:         |
|          |    OpenGLCore    |        :question:         |
| iOS      |      Metal       |        :question:         |
|          |
| Android  |      Vulkan      |    :white_check_mark:     |
|          |    OpenGLES3     |  :ok: (render artifacts)  |
|          |
| WebGPU   |        -         |      :crystal_ball:       |
| WebGL    |        -         |            :x:            |

## Notes

-   Other platforms than Windows might not work as expected as this is using more unconventional HLSL features. [Let me know of your experience](https://github.com/happy-turtle/oit-unity/discussions) and if you got this running on different platforms.
-   Note that this project currently does **not** include implementations for the Universal Render-Pipeline and the High-Definition Render-Pipeline.

## References

-   https://github.com/GameTechDev/AOIT-Update
-   https://de.slideshare.net/hgruen/oit-and-indirect-illumination-using-dx11-linked-lists
-   Salvi, Marco, and Karthik Vaidyanathan, "Multi-layer Alpha Blending", in Proceedings of the 18th ACM SIGGRAPH Symposium on Interactive 3D Graphics and Games, ACM, pp. 151-158, 2014. https://www.intel.com/content/www/us/en/developer/articles/technical/multi-layer-alpha-blending.html
-   Real-Time Rendering, Fourth Edition, by Tomas Akenine-Möller, Eric Haines, Naty Hoffman, Angelo Pesce, Michał Iwanicki, and Sébastien Hillaire, 1198 pages, from A K Peters/CRC Press, ISBN-13: 978-1138627000, ISBN-10: 1138627003 http://www.realtimerendering.com/
