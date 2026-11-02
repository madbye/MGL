# MGL – Madbye's Game Library

**MGL** is a lightweight C# game development library built on top of **OpenGL** and **Silk.NET**. It provides a simple abstraction for 3D and 2D rendering, shader management, asset loading, input, and basic text rendering.

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
using System.Numerics;
using System.Drawing;
using MGL;
using MGL.Graphics;
using MGL.Utils;

// 1. Initialize Window & Renderer
var window = new Window(1280u, 720u, "MGL Demo");
window.Initialize();

var renderer = window.CreateRenderer();

// 2. Setup Camera
var camera = new PerspectiveCamera(
    position: new Vector3(3f, 3f, 3f),
    Quaternion.Identity,
    width: 1280,
    height: 720,
    fov: 60f,
    0.1f,
    100f
);
camera.LookAt(Vector3.Zero);

// 3. Load Resources (Mesh, Texture, Shader)
using var vao = VertexArray.FromCommonMesh(renderer, MeshGenerator.GenCube());

using var texture = Texture2D.FromImage(
    renderer, 
    CheckerboardGenerator.GenCheckerboard(Color.Magenta, Color.Black, 4), 
    TextureFilter.Nearest, 
    false
);
texture.Bind();

var shader = renderer.DefaultShaders.GetUnlit();
shader.Bind();
shader.SetUniform("model", Matrix4x4.Identity);

renderer.Enable(EnableCap.DepthTest);

// 4. Handle Window Resize
window.OnResize += (newWidth, newHeight) =>
{
    renderer.Viewport(newWidth, newHeight);
    
    camera.Width = newWidth;
    camera.Height = newHeight;
    camera.SetVPMatricesToProgram(shader);
};

// 5. Main Render Loop
while (!window.ShouldClose())
{
    window.DoEvents();

    // Clear screen buffers
    renderer.ClearColor(Color.CornflowerBlue);
    renderer.ClearDepth();
    
    // Draw geometry
    vao.Draw();
    
    window.SwapBuffers();
}

// 6. Cleanup Resources
vao.Dispose();
texture.Dispose();
shader.Dispose();
renderer.Dispose();
```
