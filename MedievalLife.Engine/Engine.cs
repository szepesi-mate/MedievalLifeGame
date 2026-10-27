namespace MedievalLife.Engine;

using System;
using MedievalLife.Engine.Rendering;
using MedievalLife.Engine.Rendering.Scenes;
using MedievalLife.Engine.Windowing;
using Silk.NET.Maths;

/// <summary>
/// Core engine orchestrator that coordinates the window lifecycle, update loop, scene execution, and rendering pipeline.
/// </summary>
public class Engine : IDisposable
{
    private readonly EngineConfiguration _configuration;
    private readonly GameWindow _window;
    private readonly Renderer _renderer;
    private IScene? _activeScene;
    private bool _disposed;
    private bool _isRunning;

    public GameWindow Window => _window;
    public Renderer Renderer => _renderer;
    public IScene? ActiveScene => _activeScene;
    public bool IsRunning => _isRunning;

    public event Action? Initialized;
    public event Action<double>? Updated;
    public event Action<double>? Rendered;
    public event Action<Vector2D<int>>? Resized;
    public event Action? ShuttingDown;

    public Engine(EngineConfiguration? configuration = null)
    {
        _configuration = configuration ?? new EngineConfiguration();
        _window = new GameWindow(_configuration.Window);
        _renderer = new Renderer();

        _window.Load += HandleLoad;
        _window.Update += HandleUpdate;
        _window.Render += HandleRender;
        _window.Resize += HandleResize;
        _window.Closing += HandleClosing;
    }

    /// <summary>
    /// Sets the active scene. If the renderer is already initialized, the scene is immediately initialized.
    /// </summary>
    public void SetScene(IScene? scene)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (ReferenceEquals(_activeScene, scene))
        {
            return;
        }

        _activeScene?.Dispose();
        _activeScene = scene;

        if (_renderer.IsInitialized && _activeScene != null)
        {
            _activeScene.Initialize(_renderer);
        }
    }

    /// <summary>
    /// Starts the engine loop and displays the main window.
    /// </summary>
    public void Run()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _isRunning = true;
        _window.Run();
    }

    /// <summary>
    /// Requests the engine and window to stop and close.
    /// </summary>
    public void Stop()
    {
        if (!_disposed && _isRunning)
        {
            _window.Close();
        }
    }

    private void HandleLoad()
    {
        _renderer.Initialize(_window);
        _renderer.SetClearColor(_configuration.ClearColor);
        _renderer.SetViewport(_window.FramebufferSize);

        if (_activeScene != null)
        {
            _activeScene.Initialize(_renderer);
        }

        OnInitialize();
        Initialized?.Invoke();
    }

    private void HandleUpdate(double deltaTime)
    {
        _activeScene?.Update(deltaTime);
        OnUpdate(deltaTime);
        Updated?.Invoke(deltaTime);
    }

    private void HandleRender(double deltaTime)
    {
        _renderer.BeginFrame();

        if (_activeScene != null)
        {
            float aspectRatio = _window.FramebufferSize.Y > 0
                ? (float)_window.FramebufferSize.X / _window.FramebufferSize.Y
                : 1.0f;
            _activeScene.Render(_renderer, aspectRatio);
        }

        OnRender(deltaTime);
        Rendered?.Invoke(deltaTime);

        _renderer.EndFrame();
    }

    private void HandleResize(Vector2D<int> newSize)
    {
        _renderer.SetViewport(newSize);
        _activeScene?.Resize(newSize.X, newSize.Y);

        OnResize(newSize);
        Resized?.Invoke(newSize);
    }

    private void HandleClosing()
    {
        _isRunning = false;

        ShuttingDown?.Invoke();
        OnShutdown();
    }

    /// <summary>
    /// Called when the engine and graphics context are initialized.
    /// </summary>
    protected virtual void OnInitialize() { }

    /// <summary>
    /// Called once per frame/tick for game logic, physics, and state updates.
    /// </summary>
    /// <param name="deltaTime">Elapsed time since the last update tick in seconds.</param>
    protected virtual void OnUpdate(double deltaTime) { }

    /// <summary>
    /// Called once per frame for rendering operations.
    /// </summary>
    /// <param name="deltaTime">Elapsed time since the last render frame in seconds.</param>
    protected virtual void OnRender(double deltaTime) { }

    /// <summary>
    /// Called when the window/framebuffer is resized.
    /// </summary>
    /// <param name="newSize">The new size in pixels.</param>
    protected virtual void OnResize(Vector2D<int> newSize) { }

    /// <summary>
    /// Called before the engine shuts down and graphics resources are disposed.
    /// </summary>
    protected virtual void OnShutdown() { }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _isRunning = false;

        _window.Load -= HandleLoad;
        _window.Update -= HandleUpdate;
        _window.Render -= HandleRender;
        _window.Resize -= HandleResize;
        _window.Closing -= HandleClosing;

        _activeScene?.Dispose();
        _activeScene = null;

        _renderer.Dispose();
        _window.Dispose();

        GC.SuppressFinalize(this);
    }
}
