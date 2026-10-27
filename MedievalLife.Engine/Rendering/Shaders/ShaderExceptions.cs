namespace MedievalLife.Engine.Rendering.Shaders;

using System;
using Silk.NET.OpenGL;

/// <summary>
/// Exception thrown when a shader fails to compile.
/// </summary>
public sealed class ShaderCompilationException : Exception
{
    public ShaderType ShaderType { get; }
    public string DiagnosticLog { get; }

    public ShaderCompilationException(ShaderType shaderType, string diagnosticLog)
        : base($"Failed to compile {shaderType}: {diagnosticLog}")
    {
        ShaderType = shaderType;
        DiagnosticLog = diagnosticLog;
    }
}

/// <summary>
/// Exception thrown when a shader program fails to link.
/// </summary>
public sealed class ShaderLinkException : Exception
{
    public string DiagnosticLog { get; }

    public ShaderLinkException(string diagnosticLog)
        : base($"Failed to link shader program: {diagnosticLog}")
    {
        DiagnosticLog = diagnosticLog;
    }
}
