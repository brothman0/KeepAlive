﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace KeepAlive.External.Resources.TryGetMonitorInfo;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
[ExcludeFromCodeCoverage(Justification = "Structs with no logic do not require coverage.")]
public struct MonitorInfo
{
    public uint Size;

    public Rectangle Monitor = new();

    public Rectangle WorkArea = new();

    public uint Flags = 0;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string? DeviceName = null;

    public MonitorInfo(uint size)
    {
        Size = size;
    }
}