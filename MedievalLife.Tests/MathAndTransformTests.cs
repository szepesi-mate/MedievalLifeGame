namespace MedievalLife.Tests;

using System;
using System.Numerics;
using MedievalLife.Core.Math;
using Xunit;

public class MathAndTransformTests
{
    [Fact]
    public void MathUtils_AngleConversions_AreAccurate()
    {
        float deg180 = 180.0f;
        float radPi = MathUtils.ToRadians(deg180);
        Assert.Equal(MathF.PI, radPi, 0.0001f);

        float backToDeg = MathUtils.ToDegrees(radPi);
        Assert.Equal(180.0f, backToDeg, 0.0001f);
    }

    [Fact]
    public void MathUtils_CreatePerspective_MapsNearAndFarToOpenGLNDC()
    {
        float fov = MathUtils.ToRadians(60.0f);
        float aspect = 16.0f / 9.0f;
        float near = 0.1f;
        float far = 100.0f;

        Matrix4x4 proj = MathUtils.CreatePerspective(fov, aspect, near, far);

        // Vector on near plane along -Z in eye/view space: (0, 0, -near, 1)
        Vector4 nearEye = new(0.0f, 0.0f, -near, 1.0f);
        Vector4 nearClip = Vector4.Transform(nearEye, proj);
        float nearNdcZ = nearClip.Z / nearClip.W;

        // In OpenGL NDC, near plane should map to -1.0
        Assert.Equal(-1.0f, nearNdcZ, 0.001f);

        // Vector on far plane along -Z in eye/view space: (0, 0, -far, 1)
        Vector4 farEye = new(0.0f, 0.0f, -far, 1.0f);
        Vector4 farClip = Vector4.Transform(farEye, proj);
        float farNdcZ = farClip.Z / farClip.W;

        // In OpenGL NDC, far plane should map to +1.0
        Assert.Equal(1.0f, farNdcZ, 0.001f);
    }

    [Fact]
    public void MathUtils_CreatePerspective_ThrowsOnInvalidParameters()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => MathUtils.CreatePerspective(0.0f, 1.33f, 0.1f, 100.0f));
        Assert.Throws<ArgumentOutOfRangeException>(() => MathUtils.CreatePerspective(MathF.PI, 1.33f, 0.1f, 100.0f));
        Assert.Throws<ArgumentOutOfRangeException>(() => MathUtils.CreatePerspective(1.0f, -1.0f, 0.1f, 100.0f));
        Assert.Throws<ArgumentException>(() => MathUtils.CreatePerspective(1.0f, 1.33f, 0.0f, 100.0f));
        Assert.Throws<ArgumentException>(() => MathUtils.CreatePerspective(1.0f, 1.33f, 10.0f, 5.0f));
    }

    [Fact]
    public void Transform_GetModelMatrix_TransformsCorrectly()
    {
        var transform = new Transform
        {
            Position = new Vector3(10.0f, 5.0f, -2.0f),
            Rotation = Vector3.Zero,
            Scale = new Vector3(2.0f, 2.0f, 2.0f)
        };

        Matrix4x4 model = transform.GetModelMatrix();

        Vector4 localPoint = new(1.0f, 0.0f, 0.0f, 1.0f);
        Vector4 transformedPoint = Vector4.Transform(localPoint, model);

        // Point (1, 0, 0) scaled by 2 -> (2, 0, 0), translated by (10, 5, -2) -> (12, 5, -2)
        Assert.Equal(12.0f, transformedPoint.X, 0.001f);
        Assert.Equal(5.0f, transformedPoint.Y, 0.001f);
        Assert.Equal(-2.0f, transformedPoint.Z, 0.001f);
        Assert.Equal(1.0f, transformedPoint.W, 0.001f);
    }
}
