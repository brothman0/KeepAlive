using System.Runtime.InteropServices;

namespace KeepAlive.External.Resources.TryGetMonitorInfo;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct MonitorInfo
{
    public uint Size = (uint)Marshal.SizeOf<MonitorInfo>();

    public Rectangle Monitor = new();

    public Rectangle WorkArea = new();

    public uint Flags = 0;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string? DeviceName = null;

    public MonitorInfo()
    {
    }
}