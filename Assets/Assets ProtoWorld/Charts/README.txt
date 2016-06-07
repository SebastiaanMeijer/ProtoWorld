The prefab Chart1, Chart2, Chart3, Chart4 is basically the same prefab.
However, for it to work, each prefab must use an individual Render Texture.

If there will be more than 4 charts then we need to create extra Render Texture.
A Render Texture can be created in the Project view.
The newly created Render Texture will need to be set as Target Texture in the Camera in the Chart-object and in the RawImage as Texture.

The way it works is that the camera will write to the Render Texture based on what it captures.
The rawImage will take that Render Texture and render it on the Canvas.
Similar to http://docs.unity3d.com/Manual/class-RenderTexture.html
