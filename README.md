# OIT-Per-Pixel-Linked-List

Order-independent Transparency Implementation in Unity with Per-Pixel Linked Lists

![Screenshot 2020-10-20 120732 (Benutzerdefiniert) (1)](https://user-images.githubusercontent.com/18415215/96572776-8beb9580-12cd-11eb-9dbe-baa75754f5b2.png)
![Screenshot 2020-10-20 120726 (Benutzerdefiniert)](https://user-images.githubusercontent.com/18415215/96572774-8b52ff00-12cd-11eb-9b1f-84927f101b66.png)

Two transparent quads are rendered. On the left with this implementation and correct color blending. On the right rendered with the standard pipeline.

## Description

This is an implementation of order-independent transparency in Unity. It uses Per-Pixel Linked Lists, implemented with RWStructuredBuffers.
This is a feature **requiring Shader Model 4.5 with ComputeBuffers**, see the [Unity Manual](https://docs.unity3d.com/Manual/SL-ShaderCompileTargets.html) for supported platforms.
For reference a [presentation by Holger Gruen and Nicolas Thibieroz](https://de.slideshare.net/hgruen/oit-and-indirect-illumination-using-dx11-linked-lists)
was used. The code is based on their suggestions.
