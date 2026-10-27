namespace MedievalLife.Core.Math;

using System;
using System.Numerics;

/// <summary>
/// Provides mathematical utilities for 3D graphics and OpenGL-compatible matrix transformations.
/// </summary>
public static class MathUtils
{
    public const float DegreesToRadians = MathF.PI / 180.0f;
    public const float RadiansToDegrees = 180.0f / MathF.PI;

    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    public static float ToRadians(float degrees) => degrees * DegreesToRadians;

    /// <summary>
    /// Converts radians to degrees.
    /// </summary>
    public static float ToDegrees(float radians) => radians * RadiansToDegrees;

    /// <summary>
    /// Creates an OpenGL-compatible right-handed perspective projection matrix.
    /// The resulting depth range in normalized device coordinates (NDC) is [-1, 1].
    /// </summary>
    /// <param name="fovYRadians">Vertical field of view in radians.</param>
    /// <param name="aspectRatio">Aspect ratio (width / height).</param>
    /// <param name="nearPlane">Distance to the near clipping plane.</param>
    /// <param name="farPlane">Distance to the far clipping plane.</param>
    /// <returns>Perspective projection matrix.</returns>
    public static Matrix4x4 CreatePerspective(float fovYRadians, float aspectRatio, float nearPlane, float farPlane)
    {
        if (fovYRadians <= 0.0f || fovYRadians >= MathF.PI)
        {
            throw new ArgumentOutOfRangeException(nameof(fovYRadians), "Field of view must be between 0 and PI radians.");
        }

        if (aspectRatio <= 0.0f)
        {
            throw new ArgumentOutOfRangeException(nameof(aspectRatio), "Aspect ratio must be greater than zero.");
        }

        if (nearPlane <= 0.0f || farPlane <= nearPlane)
        {
            throw new ArgumentException("Near plane must be > 0 and Far plane must be > Near plane.");
        }

        float tanHalfFov = MathF.Tan(fovYRadians * 0.5f);

        Matrix4x4 result = Matrix4x4.Identity;
        result.M11 = 1.0f / (aspectRatio * tanHalfFov);
        result.M12 = 0.0f;
        result.M13 = 0.0f;
        result.M14 = 0.0f;

        result.M21 = 0.0f;
        result.M22 = 1.0f / tanHalfFov;
        result.M23 = 0.0f;
        result.M24 = 0.0f;

        result.M31 = 0.0f;
        result.M32 = 0.0f;
        result.M33 = -(farPlane + nearPlane) / (farPlane - nearPlane);
        result.M34 = -1.0f;

        result.M41 = 0.0f;
        result.M42 = 0.0f;
        result.M43 = -(2.0f * farPlane * nearPlane) / (farPlane - nearPlane);
        result.M44 = 0.0f;

        return result;
    }

    /// <summary>
    /// Creates a right-handed view matrix looking from the camera position towards the target position.
    /// </summary>
    /// <param name="cameraPosition">The position of the camera.</param>
    /// <param name="cameraTarget">The target position being looked at.</param>
    /// <param name="cameraUpVector">The up vector in world space (typically (0, 1, 0)).</param>
    /// <returns>Right-handed view matrix.</returns>
    public static Matrix4x4 CreateLookAt(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
    {
        return Matrix4x4.CreateLookAt(cameraPosition, cameraTarget, cameraUpVector);
    }

    /// <summary>
    /// Creates a model transformation matrix combining translation, rotation (in Euler angles: yaw, pitch, roll in radians), and scaling.
    /// </summary>
    public static Matrix4x4 CreateModelMatrix(Vector3 position, Vector3 rotationRadians, Vector3 scale)
    {
        Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(scale);
        Matrix4x4 rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(rotationRadians.Y, rotationRadians.X, rotationRadians.Z);
        Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(position);

        // Transformation order: Scale -> Rotate -> Translate
        return scaleMatrix * rotationMatrix * translationMatrix;
    }
}
