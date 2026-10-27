namespace MedievalLife.Engine.Rendering.Scenes;

using System;
using System.Numerics;
using MedievalLife.Core.Math;
using MedievalLife.Core.Primitives;
using MedievalLife.Engine.Rendering;
using MedievalLife.Engine.Rendering.Buffers;
using MedievalLife.Engine.Rendering.Shaders;

/// <summary>
/// Test scene rendering a rotating 3D cube with 8 vertices, 36 indices, perspective projection, and depth testing.
/// </summary>
public sealed class Cube3DScene : IScene
{
    private Shader? _shader;
    private VertexBuffer<VertexPositionColor>? _vertexBuffer;
    private IndexBuffer? _indexBuffer;
    private VertexArray? _vertexArray;
    private bool _disposed;

    public string Name => "3D Cube Scene (Test 2)";
    public Transform Transform { get; set; } = new(Vector3.Zero, Vector3.Zero, new Vector3(0.75f));

    private const string VertexShaderSource = @"#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec4 aColor;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

out vec4 vColor;

void main()
{
    gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
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

        // 8 cube corner vertices with distinct colors
        VertexPositionColor[] vertices =
        [
            new VertexPositionColor(new Vector3(-1.0f, -1.0f,  1.0f), new Color4(1.0f, 0.1f, 0.1f, 1.0f)), // 0: Front-Bottom-Left (Red)
            new VertexPositionColor(new Vector3( 1.0f, -1.0f,  1.0f), new Color4(0.1f, 1.0f, 0.1f, 1.0f)), // 1: Front-Bottom-Right (Green)
            new VertexPositionColor(new Vector3( 1.0f,  1.0f,  1.0f), new Color4(0.1f, 0.3f, 1.0f, 1.0f)), // 2: Front-Top-Right (Blue)
            new VertexPositionColor(new Vector3(-1.0f,  1.0f,  1.0f), new Color4(1.0f, 1.0f, 0.1f, 1.0f)), // 3: Front-Top-Left (Yellow)
            new VertexPositionColor(new Vector3(-1.0f, -1.0f, -1.0f), new Color4(1.0f, 0.1f, 1.0f, 1.0f)), // 4: Back-Bottom-Left (Magenta)
            new VertexPositionColor(new Vector3( 1.0f, -1.0f, -1.0f), new Color4(0.1f, 1.0f, 1.0f, 1.0f)), // 5: Back-Bottom-Right (Cyan)
            new VertexPositionColor(new Vector3( 1.0f,  1.0f, -1.0f), new Color4(1.0f, 1.0f, 1.0f, 1.0f)), // 6: Back-Top-Right (White)
            new VertexPositionColor(new Vector3(-1.0f,  1.0f, -1.0f), new Color4(1.0f, 0.5f, 0.1f, 1.0f))  // 7: Back-Top-Left (Orange)
        ];

        // 36 indices forming the 12 triangles of the 6 cube faces
        uint[] indices =
        [
            // Front face (+Z)
            0, 1, 2,  2, 3, 0,
            // Right face (+X)
            1, 5, 6,  6, 2, 1,
            // Back face (-Z)
            7, 6, 5,  5, 4, 7,
            // Left face (-X)
            4, 0, 3,  3, 7, 4,
            // Top face (+Y)
            3, 2, 6,  6, 7, 3,
            // Bottom face (-Y)
            4, 5, 1,  1, 0, 4
        ];

        _vertexBuffer = new VertexBuffer<VertexPositionColor>(renderer.GL, vertices);
        _indexBuffer = new IndexBuffer(renderer.GL, indices);

        _vertexArray = new VertexArray(renderer.GL);
        _vertexArray.AddVertexBuffer(_vertexBuffer, VertexPositionColor.Layout);
        _vertexArray.SetIndexBuffer(_indexBuffer);
    }

    public void Update(double deltaTime)
    {
        // Smooth rotation along X, Y, Z axes
        var rot = Transform.Rotation;
        rot.X += (float)deltaTime * 0.7f;
        rot.Y += (float)deltaTime * 1.1f;
        rot.Z += (float)deltaTime * 0.4f;

        var transform = Transform;
        transform.Rotation = rot;
        Transform = transform;
    }

    public void Render(Renderer renderer, float aspectRatio)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(renderer);

        if (_shader == null || _vertexArray == null)
        {
            return;
        }

        renderer.SetDepthTest(true);

        Matrix4x4 model = Transform.GetModelMatrix();
        Matrix4x4 view = MathUtils.CreateLookAt(new Vector3(0.0f, 1.5f, 3.5f), Vector3.Zero, Vector3.UnitY);
        Matrix4x4 projection = MathUtils.CreatePerspective(MathUtils.ToRadians(60.0f), Math.Max(0.001f, aspectRatio), 0.1f, 100.0f);

        renderer.UseShader(_shader);
        _shader.SetUniform("uModel", model);
        _shader.SetUniform("uView", view);
        _shader.SetUniform("uProjection", projection);

        renderer.DrawIndexed(_vertexArray);
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
        _indexBuffer?.Dispose();
        _vertexBuffer?.Dispose();
        _shader?.Dispose();

        _vertexArray = null;
        _indexBuffer = null;
        _vertexBuffer = null;
        _shader = null;

        GC.SuppressFinalize(this);
    }
}
