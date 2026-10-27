namespace MedievalLife.Engine.Windowing;

using Silk.NET.Maths;

/// <summary>
/// Configuration parameters for initializing a game window.
/// </summary>
public sealed class WindowConfiguration
{
    public string Title { get; set; } = "MedievalLife";
    public int Width { get; set; } = 1280;
    public int Height { get; set; } = 720;
    public bool VSync { get; set; } = true;
    public double UpdatesPerSecond { get; set; } = 60.0;
    public double FramesPerSecond { get; set; } = 0.0; // 0.0 = unlimited / controlled by VSync

    public Vector2D<int> Size => new(Width, Height);
}
