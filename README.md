# OIT-Per-Pixel-Linked-List
Order-independent Transparency Implementation in Unity with Per-Pixel Linked Lists

![Screenshot 2020-10-20 120732 (Benutzerdefiniert) (1)](https://user-images.githubusercontent.com/18415215/96572776-8beb9580-12cd-11eb-9dbe-baa75754f5b2.png)
![Screenshot 2020-10-20 120726 (Benutzerdefiniert)](https://user-images.githubusercontent.com/18415215/96572774-8b52ff00-12cd-11eb-9b1f-84927f101b66.png)

Two transparent quads are rendered. On the left with this implementation and correct color blending. On the right rendered with the standard pipeline.

Please read the readme first!

## Description

This is an implementation of order-independent transparency in Unity. It uses Per-Pixel Linked Lists, implemented with RWStructuredBuffers.
This is a feature **only available in DirectX 11**!
For reference a [presentation by Holger Gruen and Nicolas Thibieroz](https://de.slideshare.net/hgruen/oit-and-indirect-illumination-using-dx11-linked-lists)
 was used. The code is based on their suggestions. 
This is **not** a polished build. The implementation has bad performance, is not optimized and got some bugs. 
Any discussion on this project is welcomed though.

## Usage

Transparent Objects have to be in the layer 'Transparent'. The rendering only works correctly in Play mode and you might have to
run Play mode once in the beginning to fix errors. It might happen that you don't see the transparent objects in the Game view.
To fix this error you can try to change the size of the Game view window, odd numbers seem to work better. Look in the known bugs section for more.

## Known Bugs

- The rendering only works at some window sizes. This might be caused by the buffer size being dependent on the screen size.
But I don't know the exact reason for the bug, hence I couldn't fix it until now.
- The depth precision is bad and masking is not working correct. This comes from rendering the depth manually into a depth texture.
I tried using the _CameraDepthTexture but couldn't get it to work in this project. The _CameraDepthTextur from the opaque objects seems to be empty.
- Performance is bad. This could be fixed by compressing the color and depth into one or two uint fields instead of multiple float ones.
