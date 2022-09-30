using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace KeepAlive.External.Resources.GetLastInputInfo;

[StructLayout(LayoutKind.Sequential)]
[ExcludeFromCodeCoverage(Justification = "Structs with no logic do not require coverage.")]
public struct LastInputInfo
{
    [MarshalAs(UnmanagedType.U4)]
    public uint Size;

    [MarshalAs(UnmanagedType.U4)]
    public uint Ticks = default;

    public LastInputInfo(uint size)
    {
        Size = size;
    }
}