namespace MedievalLife.Engine.Rendering.Buffers;

using System;
using Silk.NET.OpenGL;

/// <summary>
/// Encapsulates an OpenGL Vertex Array Object (VAO) managing vertex buffer bindings and vertex attributes.
/// </summary>
public sealed class VertexArray : IDisposable
{
    private readonly GL _gl;
    private uint _handle;
    private IndexBuffer? _indexBuffer;
    private bool _disposed;

    public uint Handle => _handle;
    public IndexBuffer? IndexBuffer => _indexBuffer;
    public bool IsDisposed => _disposed;

    public VertexArray(GL gl)
    {
        _gl = gl ?? throw new ArgumentNullException(nameof(gl));
        _handle = _gl.GenVertexArray();
        if (_handle == 0)
        {
            throw new InvalidOperationException("Failed to generate OpenGL Vertex Array handle.");
        }
    }

    public void Bind()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _gl.BindVertexArray(_handle);
    }

    public void Unbind()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _gl.BindVertexArray(0);
    }

    /// <summary>
    /// Attaches a vertex buffer and configures its vertex attributes into this VAO.
    /// </summary>
    public unsafe void AddVertexBuffer<T>(VertexBuffer<T> vertexBuffer, VertexLayout layout) where T : unmanaged
    {
        ArgumentNullException.ThrowIfNull(vertexBuffer);
        ArgumentNullException.ThrowIfNull(layout);
        EnsureNotDisposed();

        Bind();
        vertexBuffer.Bind();

        foreach (var attr in layout.Attributes)
        {
            _gl.EnableVertexAttribArray(attr.Index);
            _gl.VertexAttribPointer(
                attr.Index,
                attr.ComponentCount,
                attr.Type,
                attr.Normalized,
                (uint)layout.Stride,
                (void*)attr.Offset
            );
        }
    }

    /// <summary>
    /// Attaches an element index buffer (EBO) to this VAO.
    /// </summary>
    public void SetIndexBuffer(IndexBuffer indexBuffer)
    {
        ArgumentNullException.ThrowIfNull(indexBuffer);
        EnsureNotDisposed();

        Bind();
        indexBuffer.Bind();
        _indexBuffer = indexBuffer;
    }

    private void EnsureNotDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        if (_handle != 0)
        {
            _gl.DeleteVertexArray(_handle);
            _handle = 0;
        }

        _indexBuffer = null;
        GC.SuppressFinalize(this);
    }
}
