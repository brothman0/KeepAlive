using System.Diagnostics.CodeAnalysis;

namespace KeepAlive.External.Resources.TryGetMonitorInfo;

[ExcludeFromCodeCoverage(Justification = "Structs with no logic do not require coverage.")]
public struct Rectangle
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}