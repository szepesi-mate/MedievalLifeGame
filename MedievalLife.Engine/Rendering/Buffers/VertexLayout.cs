namespace MedievalLife.Engine.Rendering.Buffers;

using System;
using Silk.NET.OpenGL;

/// <summary>
/// Describes a single vertex attribute layout component within a vertex buffer.
/// </summary>
public readonly struct VertexAttribute
{
    public uint Index { get; }
    public int ComponentCount { get; }
    public VertexAttribPointerType Type { get; }
    public bool Normalized { get; }
    public int Offset { get; }

    public VertexAttribute(uint index, int componentCount, VertexAttribPointerType type, bool normalized, int offset)
    {
        Index = index;
        ComponentCount = componentCount;
        Type = type;
        Normalized = normalized;
        Offset = offset;
    }
}

/// <summary>
/// Defines the overall layout and byte stride for vertex structures.
/// </summary>
public sealed class VertexLayout
{
    public IReadOnlyList<VertexAttribute> Attributes { get; }
    public int Stride { get; }

    public VertexLayout(params VertexAttribute[] attributes)
    {
        Attributes = attributes ?? Array.Empty<VertexAttribute>();

        int computedStride = 0;
        foreach (var attr in Attributes)
        {
            computedStride += attr.ComponentCount * GetTypeSize(attr.Type);
        }

        Stride = computedStride;
    }

    public VertexLayout(int stride, params VertexAttribute[] attributes)
    {
        Stride = stride;
        Attributes = attributes ?? Array.Empty<VertexAttribute>();
    }

    public static int GetTypeSize(VertexAttribPointerType type) => type switch
    {
        VertexAttribPointerType.Float => sizeof(float),
        VertexAttribPointerType.Int => sizeof(int),
        VertexAttribPointerType.UnsignedInt => sizeof(uint),
        VertexAttribPointerType.Byte => sizeof(byte),
        VertexAttribPointerType.UnsignedByte => sizeof(byte),
        VertexAttribPointerType.Short => sizeof(short),
        VertexAttribPointerType.UnsignedShort => sizeof(ushort),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported vertex attribute type.")
    };
}
