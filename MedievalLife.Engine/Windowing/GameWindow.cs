namespace MedievalLife.Engine.Windowing;

using System;
using Silk.NET.Core.Contexts;
using Silk.NET.Maths;
using Silk.NET.Windowing;

/// <summary>
/// Window management layer encapsulating the Silk.NET windowing subsystem.
/// </summary>
public sealed class GameWindow : IDisposable, IGLContextSource
{
    private readonly IWindow _window;
    private bool _disposed;

    public event Action? Load;
    public event Action<double>? Update;
    public event Action<double>? Render;
    public event Action<Vector2D<int>>? Resize;
    public event Action? Closing;

    public IWindow NativeWindow => _window;
    public IGLContext? GLContext => _window.GLContext;

    public Vector2D<int> Size => _window.Size;
    public Vector2D<int> FramebufferSize => _window.FramebufferSize;
    public string Title
    {
        get => _window.Title;
        set => _window.Title = value;
    }

    public GameWindow(WindowConfiguration? configuration = null)
    {
        var config = configuration ?? new WindowConfiguration();

        var options = WindowOptions.Default;
        options.Title = config.Title;
        options.Size = config.Size;
        options.VSync = config.VSync;
        options.UpdatesPerSecond = config.UpdatesPerSecond;
        options.FramesPerSecond = config.FramesPerSecond;
        options.API = new GraphicsAPI(
            ContextAPI.OpenGL,
            ContextProfile.Core,
            ContextFlags.ForwardCompatible,
            new APIVersion(3, 3)
        );

        _window = Window.Create(options);

        _window.Load += HandleLoad;
        _window.Update += HandleUpdate;
        _window.Render += HandleRender;
        _window.FramebufferResize += HandleResize;
        _window.Closing += HandleClosing;
    }

    public void Run()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _window.Run();
    }

    public void Close()
    {
        if (!_disposed && _window.IsClosing == false)
        {
            _window.Close();
        }
    }

    private void HandleLoad()
    {
        Load?.Invoke();
    }

    private void HandleUpdate(double deltaTime)
    {
        Update?.Invoke(deltaTime);
    }

    private void HandleRender(double deltaTime)
    {
        Render?.Invoke(deltaTime);
    }

    private void HandleResize(Vector2D<int> newSize)
    {
        Resize?.Invoke(newSize);
    }

    private void HandleClosing()
    {
        Closing?.Invoke();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        _window.Load -= HandleLoad;
        _window.Update -= HandleUpdate;
        _window.Render -= HandleRender;
        _window.FramebufferResize -= HandleResize;
        _window.Closing -= HandleClosing;

        _window.Dispose();
        GC.SuppressFinalize(this);
    }
}
