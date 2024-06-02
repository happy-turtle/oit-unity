# Order-independent Transparency in Unity

![Comparison](Screenshots/Comparison.gif) ![Animation2](Screenshots/transparent-statues.gif)

## Description

This is an implementation of order-independent transparency in Unity's render pipelines.
Order-independent transparency allows blending transparent objects correctly according to their actual depth in the
scene.
This is a huge improvement compared to traditional blending of transparent objects in the realtime graphics pipeline.
The implementation uses Per-Pixel Linked Lists, implemented with RWStructuredBuffers.
These are features requiring Shader Model 5.0 with ComputeBuffers, see
the [Unity Manual](https://docs.unity3d.com/Manual/SL-ShaderCompileTargets.html) for supported platforms.
Further technical information and resources are listed in the [references](#References) section.

## Installation

You can easily install this package with the Unity Package Manager using the project's git url. You can look
at [Unity's guide](https://docs.unity3d.com/Manual/upm-ui-giturl.html) for detailed instructions.

## Usage

For every implementation a sample scene is included and available for import with the Unity Package Manager. 
If in doubt try to import the sample you want to use and start from there.

1. Setup the rendering implementation for your chosen pipeline: 
   - **High-Definition Render Pipeline:** Create a [Custom Pass volume](https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@12.1/manual/Custom-Pass-Creating.html) and add `OitRenderPass` to it.
   - **Universal Render Pipeline:** Add the renderer feature `Order Independent Transparency Renderer` to your Universal Renderer Asset. 
   - **Post-Processing Stack v2:** Add the post-processing override `Order Independent Transparency` to a post-processing volume in your scene.
   
2. Change the material of every object that shall be rendered with order-independent transparency. They have to be rendered with a shader writing to the buffer used by the order-independent transparency implementation. Two sample shaders that you can use are included in this project: `OrderIndependentTransparency/Unlit` for all pipelines, `OrderIndependentTransparency/Standard` for the built-in pipeline and `OrderIndependentTransparency/URP/Lit` for the Universal Render Pipeline.
   
3. Run your scene.

## Platforms tested (Unity 2022.3)

| Platform | Graphics Backend |      Render output      |
| :------- | :--------------: |:-----------------------:|
|          |
| Windows  |    DirectX 12    |   :white_check_mark:    |
|          |    DirectX 11    |   :white_check_mark:    |
|          |      Vulkan      |   :white_check_mark:    |
|          |    OpenGLCore    | :ok: (poor performance) |
|          |    OpenGLES3     |  :x: (editor crashes)   |
|          |
| Linux    |      Vulkan      |   :white_check_mark:    |
|          |    OpenGLCore    | :ok: (poor performance) |
|          |
| Mac      |      Metal       |       :question:        |
|          |    OpenGLCore    |       :question:        |
| iOS      |      Metal       |       :question:        |
|          |
| Android  |      Vulkan      |   :white_check_mark:    |
|          |    OpenGLES3     | :ok: (render artifacts) |
|          |
| WebGPU   |        -         |     :crystal_ball:      |
| WebGL    |        -         |           :x:           |

## Feedback and Contribution

- I consider this an open project. If you are interested in this topic or want to improve something please discuss,
  contribute and feel at home! :house:
- Feel free to open a [discussion](https://github.com/happy-turtle/oit-unity/discussions) or an issue if you have ideas
  and improvements in mind.
- Pull requests are very welcome, see the [issues section](https://github.com/happy-turtle/oit-unity/issues) for open
  tasks that would improve this project.
  
## References

- https://github.com/GameTechDev/AOIT-Update
- Holger Gruen and Nicolas Thibieroz, "OIT and Indirect Illumination using DX11 Linked
  Lists" https://de.slideshare.net/hgruen/oit-and-indirect-illumination-using-dx11-linked-lists
- Salvi, Marco, and Karthik Vaidyanathan, "Multi-layer Alpha Blending", in Proceedings of the 18th ACM SIGGRAPH
  Symposium on Interactive 3D Graphics and Games, ACM, pp. 151-158,
    2014. https://www.intel.com/content/www/us/en/developer/articles/technical/multi-layer-alpha-blending.html
- Real-Time Rendering, Fourth Edition, by Tomas Akenine-Möller, Eric Haines, Naty Hoffman, Angelo Pesce, Michał
  Iwanicki, and Sébastien Hillaire, 1198 pages, from A K Peters/CRC Press, ISBN-13: 978-1138627000, ISBN-10:
  1138627003 http://www.realtimerendering.com/
