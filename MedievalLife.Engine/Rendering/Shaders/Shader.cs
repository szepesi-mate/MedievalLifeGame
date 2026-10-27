namespace MedievalLife.Engine.Rendering.Shaders;

using System;
using System.Collections.Generic;
using System.Numerics;
using MedievalLife.Core.Primitives;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

/// <summary>
/// Encapsulates an OpenGL shader program with uniform variables support and diagnostic error handling.
/// </summary>
public sealed class Shader : IDisposable
{
    private readonly GL _gl;
    private readonly Dictionary<string, int> _uniformLocationCache = new(StringComparer.Ordinal);
    private uint _handle;
    private bool _disposed;

    public uint Handle => _handle;
    public bool IsDisposed => _disposed;

    public Shader(GL gl, string vertexSource, string fragmentSource)
    {
        _gl = gl ?? throw new ArgumentNullException(nameof(gl));
        ArgumentException.ThrowIfNullOrWhiteSpace(vertexSource);
        ArgumentException.ThrowIfNullOrWhiteSpace(fragmentSource);

        uint vertexShader = CompileShader(ShaderType.VertexShader, vertexSource);
        uint fragmentShader = 0;

        try
        {
            fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentSource);
            _handle = _gl.CreateProgram();

            if (_handle == 0)
            {
                throw new InvalidOperationException("Failed to allocate OpenGL shader program handle.");
            }

            _gl.AttachShader(_handle, vertexShader);
            _gl.AttachShader(_handle, fragmentShader);
            _gl.LinkProgram(_handle);

            _gl.GetProgram(_handle, ProgramPropertyARB.LinkStatus, out int linkStatus);
            if (linkStatus == 0)
            {
                string infoLog = _gl.GetProgramInfoLog(_handle);
                throw new ShaderLinkException(string.IsNullOrWhiteSpace(infoLog) ? "Unknown program linking error." : infoLog);
            }
        }
        finally
        {
            if (vertexShader != 0)
            {
                if (_handle != 0)
                {
                    _gl.DetachShader(_handle, vertexShader);
                }
                _gl.DeleteShader(vertexShader);
            }

            if (fragmentShader != 0)
            {
                if (_handle != 0)
                {
                    _gl.DetachShader(_handle, fragmentShader);
                }
                _gl.DeleteShader(fragmentShader);
            }
        }
    }

    private uint CompileShader(ShaderType type, string source)
    {
        uint shader = _gl.CreateShader(type);
        if (shader == 0)
        {
            throw new InvalidOperationException($"Failed to allocate OpenGL shader handle for {type}.");
        }

        _gl.ShaderSource(shader, source);
        _gl.CompileShader(shader);

        _gl.GetShader(shader, ShaderParameterName.CompileStatus, out int status);
        if (status == 0)
        {
            string infoLog = _gl.GetShaderInfoLog(shader);
            _gl.DeleteShader(shader);
            throw new ShaderCompilationException(type, string.IsNullOrWhiteSpace(infoLog) ? "Unknown compilation error." : infoLog);
        }

        return shader;
    }

    /// <summary>
    /// Binds this shader program for subsequent draw calls.
    /// </summary>
    public void Use()
    {
        EnsureNotDisposed();
        _gl.UseProgram(_handle);
    }

    /// <summary>
    /// Unbinds any active shader program.
    /// </summary>
    public void Unbind()
    {
        EnsureNotDisposed();
        _gl.UseProgram(0);
    }

    /// <summary>
    /// Gets the cached location of the specified uniform variable.
    /// </summary>
    public int GetUniformLocation(string name)
    {
        EnsureNotDisposed();
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (_uniformLocationCache.TryGetValue(name, out int location))
        {
            return location;
        }

        location = _gl.GetUniformLocation(_handle, name);
        _uniformLocationCache[name] = location;
        return location;
    }

    public void SetUniform(string name, int value)
    {
        int location = GetUniformLocation(name);
        if (location != -1)
        {
            _gl.Uniform1(location, value);
        }
    }

    public void SetUniform(string name, float value)
    {
        int location = GetUniformLocation(name);
        if (location != -1)
        {
            _gl.Uniform1(location, value);
        }
    }

    public void SetUniform(string name, bool value)
    {
        int location = GetUniformLocation(name);
        if (location != -1)
        {
            _gl.Uniform1(location, value ? 1 : 0);
        }
    }

    public void SetUniform(string name, Vector2 value)
    {
        int location = GetUniformLocation(name);
        if (location != -1)
        {
            _gl.Uniform2(location, value.X, value.Y);
        }
    }

    public void SetUniform(string name, Vector3 value)
    {
        int location = GetUniformLocation(name);
        if (location != -1)
        {
            _gl.Uniform3(location, value.X, value.Y, value.Z);
        }
    }

    public void SetUniform(string name, Vector4 value)
    {
        int location = GetUniformLocation(name);
        if (location != -1)
        {
            _gl.Uniform4(location, value.X, value.Y, value.Z, value.W);
        }
    }

    public void SetUniform(string name, Color4 color)
    {
        int location = GetUniformLocation(name);
        if (location != -1)
        {
            _gl.Uniform4(location, color.R, color.G, color.B, color.A);
        }
    }

    public unsafe void SetUniform(string name, Matrix4x4 matrix, bool transpose = false)
    {
        int location = GetUniformLocation(name);
        if (location != -1)
        {
            _gl.UniformMatrix4(location, 1, transpose, (float*)&matrix);
        }
    }

    public unsafe void SetUniform(string name, Matrix4X4<float> matrix, bool transpose = false)
    {
        int location = GetUniformLocation(name);
        if (location != -1)
        {
            _gl.UniformMatrix4(location, 1, transpose, (float*)&matrix);
        }
    }

    private void EnsureNotDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        if (_handle != 0)
        {
            _gl.DeleteProgram(_handle);
            _handle = 0;
        }

        _uniformLocationCache.Clear();
        GC.SuppressFinalize(this);
    }
}
