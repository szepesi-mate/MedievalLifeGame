namespace MedievalLife.Tests;

using System;
using MedievalLife.Core.Primitives;
using MedievalLife.Engine;
using MedievalLife.Engine.Rendering;
using MedievalLife.Engine.Rendering.Buffers;
using MedievalLife.Engine.Rendering.Scenes;
using Silk.NET.OpenGL;
using Xunit;

public class RenderingAndBuffersTests
{
    [Fact]
    public void VertexLayout_CalculatesCorrectStride()
    {
        var layout = VertexPositionColor.Layout;

        Assert.Equal(28, layout.Stride); // 3 * 4 + 4 * 4 = 28 bytes
        Assert.Equal(2, layout.Attributes.Count);

        Assert.Equal(0u, layout.Attributes[0].Index);
        Assert.Equal(3, layout.Attributes[0].ComponentCount);
        Assert.Equal(VertexAttribPointerType.Float, layout.Attributes[0].Type);
        Assert.Equal(0, layout.Attributes[0].Offset);

        Assert.Equal(1u, layout.Attributes[1].Index);
        Assert.Equal(4, layout.Attributes[1].ComponentCount);
        Assert.Equal(VertexAttribPointerType.Float, layout.Attributes[1].Type);
        Assert.Equal(12, layout.Attributes[1].Offset);
    }

    [Fact]
    public void Renderer_DepthTest_CanBeConfigured()
    {
        using var renderer = new Renderer();

        Assert.True(renderer.DepthTestEnabled);
        renderer.SetDepthTest(false);
        Assert.False(renderer.DepthTestEnabled);
    }

    [Fact]
    public void Engine_SetScene_SetsAndDisposesCleanly()
    {
        var config = new EngineConfiguration();
        using var engine = new Engine(config);

        var scene = new Cube3DScene();
        engine.SetScene(scene);

        Assert.Same(scene, engine.ActiveScene);

        var triangleScene = new TriangleScene();
        engine.SetScene(triangleScene);

        Assert.Same(triangleScene, engine.ActiveScene);
    }

    [Fact]
    public void Scenes_Dispose_CanBeCalledSafely()
    {
        var triangleScene = new TriangleScene();
        triangleScene.Dispose();
        triangleScene.Dispose(); // Idempotent

        var cubeScene = new Cube3DScene();
        cubeScene.Dispose();
        cubeScene.Dispose(); // Idempotent
    }
}
