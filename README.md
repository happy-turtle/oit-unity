# Order-independent Transparency in Unity using Per-Pixel Linked Lists

![Animation2](https://user-images.githubusercontent.com/18415215/139141230-207014ab-57eb-4591-9c90-d8c17db93a30.gif)

---

![Animation](https://user-images.githubusercontent.com/18415215/139123916-2dce99a6-aefe-437a-9caf-cb105015e654.gif)
![Animation2](https://user-images.githubusercontent.com/18415215/139123918-600efbe8-96ab-475e-be68-51e2e026d434.gif)

Both animations show three stacked transparent cubes. The first one with this implementation of order-independent transparency and correct color blending. The second one is rendered with the standard shader.

## Description

This is an implementation of order-independent transparency in Unity's **Built-In Pipeline**. It uses Per-Pixel Linked Lists, implemented with RWStructuredBuffers.
This is a feature requiring Shader Model 5.0 with ComputeBuffers, see the [Unity Manual](https://docs.unity3d.com/Manual/SL-ShaderCompileTargets.html) for supported platforms.
For reference a [presentation by Holger Gruen and Nicolas Thibieroz](https://de.slideshare.net/hgruen/oit-and-indirect-illumination-using-dx11-linked-lists)
was used. The code is based on their suggestions.

## Usage

1. Add the component `OrderIndependentTransparency` to the scene camera.
2. Change the shaders of every object that shall be rendered with order-independent transparency. They have to have a material using a custom shader. Two sample shaders that you can use are included in this project `OitUnlit` and `OitStandard`.
3. Run your scene.

Note that this project currently does **not** include implementations for the Universal Render-Pipeline and the High-Definition Render-Pipeline.

### Post-Processing Compatibility

If you are using this with the package Post-Processing Stack v2 you have to add the post-processing override `Order Independent Transparency PP` to your post-processing volume.
