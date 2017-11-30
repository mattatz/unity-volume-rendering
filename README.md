unity-volume-rendering
=====================

Volume rendering by object space raymarching for Unity.

<img src="https://raw.githubusercontent.com/mattatz/unity-volume-rendering/master/Captures/Demo.gif">

VolumeRendering.shader cut a volume each axes by _SliceMin, _SliceMax properties.

## Object space raymarching

<img src="https://raw.githubusercontent.com/mattatz/unity-volume-rendering/master/Captures/Geometry.png">

VolumeRendering component generates a Cube geometry which has 1.0 length edges.
By object space raymarching techniques, rendering a volume with a MeshRenderer. (See references)

## VolumeAssetBuilder

<img src="https://raw.githubusercontent.com/mattatz/unity-volume-rendering/master/Captures/VolumeAssetBuilder.png">

VolumeAssetBuilder builds a 3D texture asset from a pvm raw file. (volume raw data)

## Sources

- primitive: blog | object space raymarching - http://i-saint.hatenablog.com/entry/2015/08/24/225254
- The Volume Library - http://lgdv.cs.fau.de/External/vollib/
- Graphics Runner : Volume Rendering 101 - http://graphicsrunner.blogspot.jp/2009/01/volume-rendering-101.html
