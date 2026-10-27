namespace MedievalLife.Engine.Rendering.Buffers;

using System.Numerics;
using System.Runtime.InteropServices;
using MedievalLife.Core.Primitives;
using Silk.NET.OpenGL;

/// <summary>
/// Vertex structure holding 3D position and RGBA color attributes.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct VertexPositionColor
{
    public Vector3 Position;
    public Color4 Color;

    public VertexPositionColor(Vector3 position, Color4 color)
    {
        Position = position;
        Color = color;
    }

    public VertexPositionColor(float x, float y, float z, Color4 color)
    {
        Position = new Vector3(x, y, z);
        Color = color;
    }

    public static readonly VertexLayout Layout = new(
        sizeof(float) * 7, // 3 floats for Vector3 + 4 floats for Color4
        new VertexAttribute(0, 3, VertexAttribPointerType.Float, normalized: false, offset: 0),
        new VertexAttribute(1, 4, VertexAttribPointerType.Float, normalized: false, offset: sizeof(float) * 3)
    );
}
