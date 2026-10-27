namespace MedievalLife.Engine.Rendering.Buffers;

using System;
using Silk.NET.OpenGL;

/// <summary>
/// Type-safe encapsulation of an OpenGL Vertex Buffer Object (VBO).
/// </summary>
/// <typeparam name="T">Unmanaged vertex structure type.</typeparam>
public sealed class VertexBuffer<T> : IDisposable where T : unmanaged
{
    private readonly GL _gl;
    private uint _handle;
    private int _count;
    private bool _disposed;

    public uint Handle => _handle;
    public int Count => _count;
    public bool IsDisposed => _disposed;

    public unsafe VertexBuffer(GL gl, ReadOnlySpan<T> data, BufferUsageARB usage = BufferUsageARB.StaticDraw)
    {
        _gl = gl ?? throw new ArgumentNullException(nameof(gl));
        _handle = _gl.GenBuffer();
        if (_handle == 0)
        {
            throw new InvalidOperationException("Failed to generate OpenGL buffer handle.");
        }

        _count = data.Length;

        Bind();
        fixed (T* ptr = data)
        {
            _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(data.Length * sizeof(T)), ptr, usage);
        }
    }

    public void Bind()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _handle);
    }

    public void Unbind()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
    }

    public unsafe void SetData(ReadOnlySpan<T> data, BufferUsageARB usage = BufferUsageARB.StaticDraw)
    {
        Bind();
        _count = data.Length;
        fixed (T* ptr = data)
        {
            _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(data.Length * sizeof(T)), ptr, usage);
        }
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
            _gl.DeleteBuffer(_handle);
            _handle = 0;
        }

        GC.SuppressFinalize(this);
    }
}
