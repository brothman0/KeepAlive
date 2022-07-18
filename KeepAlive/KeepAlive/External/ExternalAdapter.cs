using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using KeepAlive.Client.External.Resources.FormatMessage;
using KeepAlive.Client.External.Resources.GetSystemMetric;
using KeepAlive.Client.External.Resources.SendInputs;
using KeepAlive.Client.External.Resources.TryGetCursorPosition;

namespace KeepAlive.Client.External;

/// <inheritdoc cref="IExternalAdapter"/>
[ExcludeFromCodeCoverage(Justification = "Adapters do not require coverage.")]
public class ExternalAdapter : IExternalAdapter
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

    /// <inheritdoc cref="IExternalAdapter.GetMessageExtraInfo"/>
    public UIntPtr GetMessageExtraInfo()
    {
        return ExternalGetMessageExtraInfo();
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
    
    /// <inheritdoc cref="IExternalAdapter.TryGetCursorPosition"/>
    public bool TryGetCursorPosition(
        out Position position)
    {
        return ExternalTryGetCursorPosition(
            out position);
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
    [DllImport("User32.dll", SetLastError = true, EntryPoint = "GetCursorPos")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ExternalTryGetCursorPosition(
        out Position position);
}