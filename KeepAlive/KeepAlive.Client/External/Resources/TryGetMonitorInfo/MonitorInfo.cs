using System.Runtime.InteropServices;

namespace KeepAlive.External.Resources.TryGetMonitorInfo;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct MonitorInfo
{
    public uint Size = (uint)Marshal.SizeOf<MonitorInfo>();

    public Rectangle Monitor;

    public Rectangle WorkArea;

    public uint Flags;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string DeviceName;
}

public struct Rectangle
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}