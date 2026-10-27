namespace MedievalLife.Engine.Rendering.Scenes;

using System;
using MedievalLife.Engine.Rendering;

/// <summary>
/// Represents a renderable scene or test demonstration.
/// </summary>
public interface IScene : IDisposable
{
    string Name { get; }
    void Initialize(Renderer renderer);
    void Update(double deltaTime);
    void Render(Renderer renderer, float aspectRatio);
    void Resize(int width, int height);
}
