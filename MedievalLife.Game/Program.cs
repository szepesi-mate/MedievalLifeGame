namespace MedievalLife.Game;

using System;
using MedievalLife.Core.Primitives;
using MedievalLife.Engine;
using MedievalLife.Engine.Rendering.Scenes;
using MedievalLife.Engine.Windowing;

internal static class Program
{
    private static void Main(string[] args)
    {
        var config = new EngineConfiguration
        {
            Window = new WindowConfiguration
            {
                Title = "MedievalLife - 3D Renderer Demo",
                Width = 1280,
                Height = 720,
                VSync = true
            },
            ClearColor = Color4.MedievalDeepBlue
        };

        using var engine = new Engine(config);

        // Run the 3D cube test scene with 8 vertices, 36 indices, perspective projection, and depth testing
        engine.SetScene(new Cube3DScene());

        Console.WriteLine("Starting MedievalLife Engine [1280x720] with 3D Renderer...");
        engine.Run();
        Console.WriteLine("MedievalLife Engine stopped cleanly.");
    }
}
