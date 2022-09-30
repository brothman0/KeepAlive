using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using KeepAlive.External.Resources.FormatMessage;
using KeepAlive.External.Resources.GetLastInputInfo;
using KeepAlive.External.Resources.GetSystemMetric;
using KeepAlive.External.Resources.GetMonitorFromPoint;
using KeepAlive.External.Resources.SendInputs;
using KeepAlive.External.Resources.TryGetCursorPosition;
using KeepAlive.External.Resources.TryGetMonitorInfo;

namespace KeepAlive.External;

/// <inheritdoc cref="IExternalAdapter"/>
[ExcludeFromCodeCoverage(Justification = "Adapters do not require coverage.")]
public sealed class ExternalAdapter : IExternalAdapter
{
    /// <inheritdoc cref="IExternalAdapter.FormatMessage"/>
    public uint FormatMessage(
        FormatOption formatOption,
        IntPtr sourceLocation,
        uint messageId,
        uint languageId,
        StringBuilder buffer,
        int size,
        IntPtr arguments)
    {
        return ExternalFormatMessage(
            formatOption,
            sourceLocation,
            messageId,
            languageId,
            buffer,
            size,
            arguments);
    }

    /// <inheritdoc cref="IExternalAdapter.GetLastWin32Error"/>
    public int GetLastWin32Error()
    {
        return Marshal.GetLastWin32Error();
    }

    /// <inheritdoc cref="IExternalAdapter.GetMessageExtraInfo"/>
    public UIntPtr GetMessageExtraInfo()
    {
        return ExternalGetMessageExtraInfo();
    }

    /// <inheritdoc cref="IExternalAdapter.GetMonitorFromPosition"/>
    public IntPtr GetMonitorFromPosition(
        Position position,
        MonitorFromPointFlag flags)
    {
        return ExternalGetMonitorFromPosition(
            position,
            flags);
    }

    /// <inheritdoc cref="IExternalAdapter.GetSystemMetrics"/>
    public int GetSystemMetrics(
        SystemSetting setting)
    {
        return ExternalGetSystemMetrics(
            setting);
    }
    
    /// <inheritdoc cref="IExternalAdapter.SendInputs"/>
    public uint SendInputs(
        uint inputCount,
        Input[] inputs,
        int inputSize)
    {
        return ExternalSendInputs(
            inputCount, 
            inputs, 
            inputSize);
    }

    /// <inheritdoc cref="IExternalAdapter.SizeOf{T}()"/>
    public int SizeOf<T>()
    {
        return Marshal.SizeOf<T>();
    }

    /// <inheritdoc cref="IExternalAdapter.TryGetCursorPosition"/>
    public bool TryGetCursorPosition(
        out Position position)
    {
        return ExternalTryGetCursorPosition(
            out position);
    }

    /// <inheritdoc cref="IExternalAdapter.TryGetMonitorInfo"/>
    public bool TryGetMonitorInfo(
        IntPtr monitorHandle,
        ref MonitorInfo monitorInfo)
    {
        return ExternalTryGetMonitorInfo(
            monitorHandle,
            ref monitorInfo);
    }

    /// <inheritdoc cref="IExternalAdapter.TryGetLastInputInfo"/>
    public bool TryGetLastInputInfo(
        ref LastInputInfo lastInputInfo)
    {
        return ExternalGetLastInputInfo(
            ref lastInputInfo);
    }

    /// <inheritdoc cref="IExternalAdapter.FormatMessage"/>
    [DllImport("Kernel32.dll", EntryPoint = "FormatMessage", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern uint ExternalFormatMessage(
        FormatOption formatOption,
        IntPtr sourceLocation,
        uint messageId,
        uint languageId,
        StringBuilder buffer,
        int size,
        IntPtr arguments);
    
    /// <inheritdoc cref="IExternalAdapter.GetMessageExtraInfo"/>
    [DllImport("user32.dll", EntryPoint = "GetMessageExtraInfo")]
    private static extern UIntPtr ExternalGetMessageExtraInfo();

    /// <inheritdoc cref="IExternalAdapter.GetMonitorFromPosition"/>
    [DllImport("user32.dll", SetLastError = true, EntryPoint = "MonitorFromPoint")]
    private static extern IntPtr ExternalGetMonitorFromPosition(
        Position position,
        MonitorFromPointFlag flags);

    /// <inheritdoc cref="IExternalAdapter.GetSystemMetrics"/>
    [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
    private static extern int ExternalGetSystemMetrics(
        SystemSetting setting);

    /// <inheritdoc cref="IExternalAdapter.SendInputs"/>
    [DllImport("user32.dll", SetLastError = true, EntryPoint = "SendInput")]
    private static extern uint ExternalSendInputs(
        uint inputCount, 
        Input[] inputs, 
        int inputSize);

    /// <inheritdoc cref="IExternalAdapter.TryGetCursorPosition"/>
    [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetCursorPos")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ExternalTryGetCursorPosition(
        out Position position);

    /// <inheritdoc cref="IExternalAdapter.TryGetMonitorInfo"/>
    [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetMonitorInfoW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ExternalTryGetMonitorInfo(
        IntPtr monitorHandle,
        ref MonitorInfo monitorInfo);

    /// <inheritdoc cref="IExternalAdapter.TryGetLastInputInfo"/>
    [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetLastInputInfo")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ExternalGetLastInputInfo(
        ref LastInputInfo lastInputInfo);
}