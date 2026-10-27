namespace MedievalLife.Tests;

using MedievalLife.Core.Primitives;
using MedievalLife.Engine;
using MedievalLife.Engine.Rendering;
using MedievalLife.Engine.Windowing;
using Xunit;

public class ConfigurationAndPrimitivesTests
{
    [Fact]
    public void WindowConfiguration_DefaultValues_AreCorrect()
    {
        var config = new WindowConfiguration();

        Assert.Equal("MedievalLife", config.Title);
        Assert.Equal(1280, config.Width);
        Assert.Equal(720, config.Height);
        Assert.True(config.VSync);
        Assert.Equal(60.0, config.UpdatesPerSecond);
        Assert.Equal(0.0, config.FramesPerSecond);
        Assert.Equal(1280, config.Size.X);
        Assert.Equal(720, config.Size.Y);
    }

    [Fact]
    public void EngineConfiguration_DefaultValues_AreCorrect()
    {
        var config = new EngineConfiguration();

        Assert.NotNull(config.Window);
        Assert.Equal(1280, config.Window.Width);
        Assert.Equal(720, config.Window.Height);
        Assert.Equal(Color4.MedievalDeepBlue, config.ClearColor);
    }

    [Fact]
    public void Color4_PredefinedValues_AreAccurate()
    {
        var black = Color4.Black;
        var white = Color4.White;
        var deepBlue = Color4.MedievalDeepBlue;

        Assert.Equal(0.0f, black.R);
        Assert.Equal(0.0f, black.G);
        Assert.Equal(0.0f, black.B);
        Assert.Equal(1.0f, black.A);

        Assert.Equal(1.0f, white.R);
        Assert.Equal(1.0f, white.G);
        Assert.Equal(1.0f, white.B);
        Assert.Equal(1.0f, white.A);

        Assert.Equal(0.08f, deepBlue.R);
        Assert.Equal(0.12f, deepBlue.G);
        Assert.Equal(0.18f, deepBlue.B);
        Assert.Equal(1.0f, deepBlue.A);
    }

    [Fact]
    public void Renderer_WhenNotInitialized_ThrowsOnRenderingOperations()
    {
        using var renderer = new Renderer();

        Assert.False(renderer.IsInitialized);
        Assert.Throws<InvalidOperationException>(() => renderer.BeginFrame());
        Assert.Throws<InvalidOperationException>(() => renderer.SetViewport(0, 0, 800, 600));
    }

    [Fact]
    public void Renderer_ClearColor_CanBeConfiguredBeforeInit()
    {
        using var renderer = new Renderer();
        var customColor = new Color4(0.5f, 0.2f, 0.8f, 1.0f);

        renderer.SetClearColor(customColor);

        Assert.Equal(customColor, renderer.ClearColor);
    }

    [Fact]
    public void Engine_CanBeInstantiatedAndDisposed()
    {
        var config = new EngineConfiguration();
        var engine = new Engine(config);

        Assert.NotNull(engine.Window);
        Assert.NotNull(engine.Renderer);
        Assert.False(engine.IsRunning);

        engine.Dispose();
        // Disposing again should be idempotent without error
        engine.Dispose();
    }
}
