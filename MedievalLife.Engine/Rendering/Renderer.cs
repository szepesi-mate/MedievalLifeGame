namespace MedievalLife.Engine.Rendering;

using System;
using MedievalLife.Core.Primitives;
using MedievalLife.Engine.Rendering.Buffers;
using MedievalLife.Engine.Rendering.Shaders;
using Silk.NET.Core.Contexts;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Shader = MedievalLife.Engine.Rendering.Shaders.Shader;
using VertexArray = MedievalLife.Engine.Rendering.Buffers.VertexArray;

/// <summary>
/// Handles low-level OpenGL graphics rendering, GPU state management, and draw operations.
/// </summary>
public sealed class Renderer : IDisposable
{
    private GL? _gl;
    private bool _disposed;
    private Color4 _clearColor = Color4.MedievalDeepBlue;
    private Shader? _activeShader;
    private bool _depthTestEnabled = true;

    public bool IsInitialized => _gl != null && !_disposed;
    public Color4 ClearColor => _clearColor;
    public bool DepthTestEnabled => _depthTestEnabled;
    public Shader? ActiveShader => _activeShader;

    /// <summary>
    /// Gets the underlying OpenGL API instance.
    /// </summary>
    public GL GL
    {
        get
        {
            EnsureInitialized();
            return _gl!;
        }
    }

    /// <summary>
    /// Initializes the OpenGL context from the provided context source.
    /// </summary>
    /// <param name="contextSource">The window or context source providing the OpenGL context.</param>
    public void Initialize(IGLContextSource contextSource)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_gl != null)
        {
            throw new InvalidOperationException("Renderer is already initialized.");
        }

        ArgumentNullException.ThrowIfNull(contextSource);

        _gl = GL.GetApi(contextSource);

        ApplyClearColor();
        SetDepthTest(_depthTestEnabled);
    }

    /// <summary>
    /// Sets the viewport dimensions.
    /// </summary>
    public void SetViewport(int x, int y, int width, int height)
    {
        EnsureInitialized();
        _gl!.Viewport(x, y, (uint)Math.Max(1, width), (uint)Math.Max(1, height));
    }

    /// <summary>
    /// Sets the viewport dimensions from a vector size.
    /// </summary>
    public void SetViewport(Vector2D<int> size)
    {
        SetViewport(0, 0, size.X, size.Y);
    }

    /// <summary>
    /// Sets the background clear color.
    /// </summary>
    public void SetClearColor(Color4 color)
    {
        _clearColor = color;
        if (_gl != null)
        {
            ApplyClearColor();
        }
    }

    /// <summary>
    /// Sets the background clear color with individual float RGBA channels.
    /// </summary>
    public void SetClearColor(float r, float g, float b, float a = 1.0f)
    {
        SetClearColor(new Color4(r, g, b, a));
    }

    /// <summary>
    /// Enables or disables depth testing in OpenGL.
    /// </summary>
    public void SetDepthTest(bool enabled)
    {
        _depthTestEnabled = enabled;
        if (_gl != null)
        {
            if (enabled)
            {
                _gl.Enable(EnableCap.DepthTest);
                _gl.DepthFunc(DepthFunction.Less);
            }
            else
            {
                _gl.Disable(EnableCap.DepthTest);
            }
        }
    }

    /// <summary>
    /// Sets the depth comparison function.
    /// </summary>
    public void SetDepthFunction(DepthFunction func)
    {
        EnsureInitialized();
        _gl!.DepthFunc(func);
    }

    /// <summary>
    /// Enables or disables face culling.
    /// </summary>
    public void SetCullFace(bool enabled, TriangleFace mode = TriangleFace.Back)
    {
        EnsureInitialized();
        if (enabled)
        {
            _gl!.Enable(EnableCap.CullFace);
            _gl.CullFace(mode);
        }
        else
        {
            _gl!.Disable(EnableCap.CullFace);
        }
    }

    /// <summary>
    /// Binds an active shader for subsequent drawing operations.
    /// </summary>
    public void UseShader(Shader? shader)
    {
        EnsureInitialized();
        _activeShader = shader;
        if (shader != null)
        {
            shader.Use();
        }
        else
        {
            _gl!.UseProgram(0);
        }
    }

    /// <summary>
    /// Prepares the rendering pipeline for a new frame by clearing color and depth buffers.
    /// </summary>
    public void BeginFrame()
    {
        EnsureInitialized();
        _gl!.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));
    }

    /// <summary>
    /// Clears the frame buffers using the specified clear color.
    /// </summary>
    public void Clear(Color4 color)
    {
        SetClearColor(color);
        BeginFrame();
    }

    /// <summary>
    /// Finalizes the current rendering frame.
    /// </summary>
    public void EndFrame()
    {
        EnsureInitialized();
    }

    /// <summary>
    /// Issues an unindexed draw call using the currently bound vertex array.
    /// </summary>
    public void DrawArrays(PrimitiveType mode, int first, uint count)
    {
        EnsureInitialized();
        _gl!.DrawArrays(mode, first, count);
    }

    /// <summary>
    /// Binds the given vertex array and draws the specified number of non-indexed vertices.
    /// </summary>
    public void Draw(VertexArray vao, int vertexCount, PrimitiveType mode = PrimitiveType.Triangles)
    {
        ArgumentNullException.ThrowIfNull(vao);
        EnsureInitialized();

        vao.Bind();
        _gl!.DrawArrays(mode, 0, (uint)vertexCount);
    }

    /// <summary>
    /// Binds the given vertex array and draws its indexed geometry.
    /// </summary>
    public unsafe void DrawIndexed(VertexArray vao, PrimitiveType mode = PrimitiveType.Triangles)
    {
        ArgumentNullException.ThrowIfNull(vao);
        if (vao.IndexBuffer == null)
        {
            throw new InvalidOperationException("The provided VertexArray does not have an attached IndexBuffer for DrawIndexed.");
        }

        EnsureInitialized();

        vao.Bind();
        _gl!.DrawElements(mode, vao.IndexBuffer.Count, vao.IndexBuffer.IndexType, (void*)0);
    }

    private void ApplyClearColor()
    {
        _gl!.ClearColor(_clearColor.R, _clearColor.G, _clearColor.B, _clearColor.A);
    }

    private void EnsureInitialized()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_gl == null)
        {
            throw new InvalidOperationException("Renderer is not initialized. Call Initialize() before performing rendering operations.");
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _activeShader = null;
        _gl?.Dispose();
        _gl = null;

        GC.SuppressFinalize(this);
    }
}
