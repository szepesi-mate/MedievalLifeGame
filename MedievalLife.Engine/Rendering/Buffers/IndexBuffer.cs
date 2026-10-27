namespace MedievalLife.Engine.Rendering.Buffers;

using System;
using Silk.NET.OpenGL;

/// <summary>
/// Type-safe encapsulation of an OpenGL Index Buffer Object / Element Buffer Object (EBO).
/// </summary>
public sealed class IndexBuffer : IDisposable
{
    private readonly GL _gl;
    private uint _handle;
    private uint _count;
    private DrawElementsType _indexType;
    private bool _disposed;

    public uint Handle => _handle;
    public uint Count => _count;
    public DrawElementsType IndexType => _indexType;
    public bool IsDisposed => _disposed;

    public unsafe IndexBuffer(GL gl, ReadOnlySpan<uint> indices, BufferUsageARB usage = BufferUsageARB.StaticDraw)
    {
        _gl = gl ?? throw new ArgumentNullException(nameof(gl));
        _handle = _gl.GenBuffer();
        if (_handle == 0)
        {
            throw new InvalidOperationException("Failed to generate OpenGL buffer handle.");
        }

        _count = (uint)indices.Length;
        _indexType = DrawElementsType.UnsignedInt;

        Bind();
        fixed (uint* ptr = indices)
        {
            _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(uint)), ptr, usage);
        }
    }

    public unsafe IndexBuffer(GL gl, ReadOnlySpan<ushort> indices, BufferUsageARB usage = BufferUsageARB.StaticDraw)
    {
        _gl = gl ?? throw new ArgumentNullException(nameof(gl));
        _handle = _gl.GenBuffer();
        if (_handle == 0)
        {
            throw new InvalidOperationException("Failed to generate OpenGL buffer handle.");
        }

        _count = (uint)indices.Length;
        _indexType = DrawElementsType.UnsignedShort;

        Bind();
        fixed (ushort* ptr = indices)
        {
            _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(ushort)), ptr, usage);
        }
    }

    public void Bind()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _handle);
    }

    public void Unbind()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
    }

    public unsafe void SetData(ReadOnlySpan<uint> indices, BufferUsageARB usage = BufferUsageARB.StaticDraw)
    {
        Bind();
        _count = (uint)indices.Length;
        _indexType = DrawElementsType.UnsignedInt;
        fixed (uint* ptr = indices)
        {
            _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(uint)), ptr, usage);
        }
    }

    public unsafe void SetData(ReadOnlySpan<ushort> indices, BufferUsageARB usage = BufferUsageARB.StaticDraw)
    {
        Bind();
        _count = (uint)indices.Length;
        _indexType = DrawElementsType.UnsignedShort;
        fixed (ushort* ptr = indices)
        {
            _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indices.Length * sizeof(ushort)), ptr, usage);
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
