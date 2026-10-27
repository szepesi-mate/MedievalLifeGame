namespace MedievalLife.Core.Primitives;

/// <summary>
/// Represents an RGBA color with normalized 32-bit floating point components [0.0f, 1.0f].
/// </summary>
public readonly record struct Color4(float R, float G, float B, float A = 1.0f)
{
    public static readonly Color4 Black = new(0.0f, 0.0f, 0.0f, 1.0f);
    public static readonly Color4 White = new(1.0f, 1.0f, 1.0f, 1.0f);
    public static readonly Color4 CornflowerBlue = new(0.392f, 0.584f, 0.929f, 1.0f);
    public static readonly Color4 DarkSlateGray = new(0.12f, 0.15f, 0.18f, 1.0f);
    public static readonly Color4 MedievalDeepBlue = new(0.08f, 0.12f, 0.18f, 1.0f);
}
