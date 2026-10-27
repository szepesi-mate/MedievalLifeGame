namespace MedievalLife.Tests;

using System;
using MedievalLife.Core.Primitives;
using MedievalLife.Engine;
using MedievalLife.Engine.Rendering.Scenes;
using MedievalLife.Engine.Windowing;
using Xunit;

public class EngineRenderingIntegrationTests
{
    [Fact]
    public void Engine_CanInitializeAndRenderScenesCleanly()
    {
        var config = new EngineConfiguration
        {
            Window = new WindowConfiguration
            {
                Title = "Test Window",
                Width = 800,
                Height = 600,
                VSync = false
            },
            ClearColor = Color4.Black
        };

        using var engine = new Engine(config);

        bool initializedFired = false;
        int frameCount = 0;

        engine.Initialized += () =>
        {
            initializedFired = true;
            engine.SetScene(new TriangleScene());
        };

        engine.Rendered += (dt) =>
        {
            frameCount++;
            if (frameCount == 2)
            {
                // Switch to 3D cube scene on second frame
                engine.SetScene(new Cube3DScene());
            }
            else if (frameCount >= 5)
            {
                // Stop after 5 successful frames
                engine.Stop();
            }
        };

        try
        {
            engine.Run();

            Assert.True(initializedFired, "Engine Initialized event should fire.");
            Assert.True(frameCount >= 5, "At least 5 frames should have rendered successfully.");
            Assert.NotNull(engine.ActiveScene);
            Assert.IsType<Cube3DScene>(engine.ActiveScene);
        }
        catch (Silk.NET.Core.Loader.SymbolLoadingException)
        {
            // If running on a headless CI without OpenGL driver, skip smoothly
        }
        catch (PlatformNotSupportedException)
        {
            // If headless environment without display server
        }
    }
}
