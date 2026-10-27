namespace MedievalLife.Engine;

using MedievalLife.Core.Primitives;
using MedievalLife.Engine.Windowing;

/// <summary>
/// Configuration parameters for initializing the game engine.
/// </summary>
public sealed class EngineConfiguration
{
    public WindowConfiguration Window { get; set; } = new();
    public Color4 ClearColor { get; set; } = Color4.MedievalDeepBlue;
}
