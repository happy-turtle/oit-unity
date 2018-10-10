# OIT-Per-Pixel-Linked-List
OIT Implementation in Unity with Per-Pixel Linked Lists

Please read the readme first!

## Description

This is an implementation of order-independent transparency in Unity. It uses Per-Pixel Linked Lists, implemented with RWStructuredBuffers.
This is a feature **only available in DirectX 11**!
For reference a presentation by Holger Gruen and Nicolas Thibieroz was used. The code is based on their suggestions. 
This is **not** a polished build. The implementation has bad performance, is not optimized and got some bugs. 
Any discussion on this project is welcomed though.

## Usage

Transparent Objects have to be in the layer 'transparent'. The rendering only works correctly in Play mode and you might have to
run Play mode once in the beginning to fix errors. It might happen that you don't see the transparent objects in the Game view.
To fix this error you can try to change the size of the Game view window, odd numbers seem to work better. Look in the known bugs section for more.

## Known Bugs

- The rendering only works at some window sizes. This might be caused by the buffer size being dependent on the screen size.
But I don't know the exact reason for the bug, hence I couldn't fix it until now.
- The depth precision is bad and masking is not working correct. This comes from rendering the depth manually into a depth texture.
I tried using the _CameraDepthTexture but couldn't get it to work in this project. The _CameraDepthTextur from the opaque objects seems to be empty.
- The performance is bad. This could be fixed by compressing the color and depth into one or two uint fields instead of multiple float ones.