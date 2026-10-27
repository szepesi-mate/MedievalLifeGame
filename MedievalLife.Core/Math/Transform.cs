namespace MedievalLife.Core.Math;

using System.Numerics;

/// <summary>
/// Represents position, rotation (Euler angles in radians: X = pitch, Y = yaw, Z = roll), and scale in 3D space.
/// </summary>
public struct Transform
{
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
    public Vector3 Scale { get; set; }

    public Transform()
    {
        Position = Vector3.Zero;
        Rotation = Vector3.Zero;
        Scale = Vector3.One;
    }

    public Transform(Vector3 position)
    {
        Position = position;
        Rotation = Vector3.Zero;
        Scale = Vector3.One;
    }

    public Transform(Vector3 position, Vector3 rotation, Vector3 scale)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }

    /// <summary>
    /// Computes the 4x4 model matrix corresponding to this transform.
    /// </summary>
    public readonly Matrix4x4 GetModelMatrix()
    {
        return MathUtils.CreateModelMatrix(Position, Rotation, Scale);
    }

    public static Transform Default => new(Vector3.Zero, Vector3.Zero, Vector3.One);
}
