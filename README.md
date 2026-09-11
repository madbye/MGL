# MGL – Madbye's Game Library

**MGL** is a lightweight C# game development library built on top of **OpenGL** and **Silk.NET**. It provides a simple abstraction for 3D rendering, shader management, asset loading, input, and basic text rendering.

---

## ✨ Features

- **Modern OpenGL** – leverages shader-based rendering (GLSL) for flexible graphics pipelines.
- **Built-in shaders** – includes PBR, Phong, Unlit, and Blit shaders out of the box.
- **Model & texture loading** – supports common formats through Assimp and StbImage.
- **Text rendering** – font support with a built-in renderer.
- **Lighting system** – directional, point, and spot lights.
- **Camera helpers** – orthographic and perspective cameras.
- **Post‑processing effects** – basic pipeline for screen-space effects.
- **Input** - mouse and keyboard input processing.

---

## 🕹️ Example

```csharp
using System.Drawing;
using System.Numerics;
using MGL;
using MGL.GFX.Common;
using MGL.GFX.Shaders;
using MGL.GFX.Textures;
using MGL.IO;
using MGL.Utils;

var window = new Window(1280, 720, "Window");
window.Initialize();

var vao = MeshGenerator.GenCube();
var texture = Texture2D.FromImage(ImageLoader.GenCheckerboard(Color.Magenta, Color.Black, 8), TextureFilter.Nearest, false);

var camera = new PerspectiveCamera(new(10), Quaternion.Identity, 1280, 720, 45, 0.1f, 1000);

camera.LookAt(Vector3.Zero);

RenderCommand.DepthTest = true;

window.OnResize += (width, height) =>
{
    camera.Width = width;
    camera.Height = height;
};

while (!window.ShouldClose())
{
    window.DoEvents();
    
    RenderCommand.ClearColor(Color.CornflowerBlue);
    RenderCommand.ClearDepth();
    
    DefaultShaders.GetUnlit().Bind();
    DefaultShaders.GetUnlit().SetUniform("model", Matrix4x4.Identity);
    
    camera.SetVPMatricesToProgram(DefaultShaders.GetUnlit());
    
    texture.Bind();
    
    vao.Draw();
    
    window.SwapBuffers();
}

```
