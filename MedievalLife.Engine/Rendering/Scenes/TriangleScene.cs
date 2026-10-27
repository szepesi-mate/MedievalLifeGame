namespace MedievalLife.Engine.Rendering.Scenes;

using System;
using System.Numerics;
using MedievalLife.Core.Primitives;
using MedievalLife.Engine.Rendering;
using MedievalLife.Engine.Rendering.Buffers;
using MedievalLife.Engine.Rendering.Shaders;

/// <summary>
/// Test scene rendering a colored 2D triangle using GPU buffers and shaders.
/// </summary>
public sealed class TriangleScene : IScene
{
    private Shader? _shader;
    private VertexBuffer<VertexPositionColor>? _vertexBuffer;
    private VertexArray? _vertexArray;
    private bool _disposed;

    public string Name => "Colored Triangle Scene (Test 1)";

    private const string VertexShaderSource = @"#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec4 aColor;

out vec4 vColor;

void main()
{
    gl_Position = vec4(aPosition, 1.0);
    vColor = aColor;
}
";

    private const string FragmentShaderSource = @"#version 330 core
in vec4 vColor;
out vec4 FragColor;

void main()
{
    FragColor = vColor;
}
";

    public void Initialize(Renderer renderer)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(renderer);

        _shader = new Shader(renderer.GL, VertexShaderSource, FragmentShaderSource);

        VertexPositionColor[] vertices =
        [
            new VertexPositionColor(new Vector3(0.0f, 0.5f, 0.0f), new Color4(1.0f, 0.2f, 0.2f, 1.0f)),   // Top - Red
            new VertexPositionColor(new Vector3(0.5f, -0.5f, 0.0f), new Color4(0.2f, 1.0f, 0.2f, 1.0f)),  // Bottom Right - Green
            new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.0f), new Color4(0.2f, 0.4f, 1.0f, 1.0f))  // Bottom Left - Blue
        ];

        _vertexBuffer = new VertexBuffer<VertexPositionColor>(renderer.GL, vertices);
        _vertexArray = new VertexArray(renderer.GL);
        _vertexArray.AddVertexBuffer(_vertexBuffer, VertexPositionColor.Layout);
    }

    public void Update(double deltaTime)
    {
        // No per-frame transformation update required for static triangle
    }

    public void Render(Renderer renderer, float aspectRatio)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(renderer);

        if (_shader == null || _vertexArray == null)
        {
            return;
        }

        renderer.SetDepthTest(false);
        renderer.UseShader(_shader);
        renderer.Draw(_vertexArray, 3);
    }

    public void Resize(int width, int height)
    {
        // Viewport resize handled by Renderer
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _vertexArray?.Dispose();
        _vertexBuffer?.Dispose();
        _shader?.Dispose();

        _vertexArray = null;
        _vertexBuffer = null;
        _shader = null;

        GC.SuppressFinalize(this);
    }
}
