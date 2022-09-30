using System.Text;
using System.Runtime.InteropServices;
using KeepAlive.External.Resources.FormatMessage;
using KeepAlive.External.Resources.GetLastInputInfo;
using KeepAlive.External.Resources.GetSystemMetric;
using KeepAlive.External.Resources.GetMonitorFromPoint;
using KeepAlive.External.Resources.SendInputs;
using KeepAlive.External.Resources.TryGetCursorPosition;
using KeepAlive.External.Resources.TryGetMonitorInfo;

namespace KeepAlive.External;

/// <summary>
///     Adapter used to make external calls.
/// </summary>
public interface IExternalAdapter
{
    /// <summary>
    ///     Formats a message string.
    /// </summary>
    /// <param name="formatOption">
    ///     The options to use when formatting the message.
    /// </param>
    /// <param name="sourceLocation">
    ///     The location of the message definition.
    /// </param>
    /// <param name="messageId">
    ///     The message Id to format.
    /// </param>
    /// <param name="languageId">
    ///     The language to format the message in.
    /// </param>
    /// <param name="buffer">
    ///     The buffer to write the message to.
    /// </param>
    /// <param name="size">
    ///     The size of the buffer.
    /// </param>
    /// <param name="arguments">
    ///     Array of values to insert into the message.
    /// </param>
    /// <returns>
    ///     The number of characters written to <paramref name="buffer"/>.
    /// </returns>
    uint FormatMessage(
        FormatOption formatOption,
        IntPtr sourceLocation,
        uint messageId,
        uint languageId,
        StringBuilder buffer,
        int size,
        IntPtr arguments);

    /// <inheritdoc cref="Marshal.GetLastWin32Error()"/>
    int GetLastWin32Error();

    /// <summary>
    ///     Gets the extra message information for the current thread.
    /// </summary>
    /// <returns>
    ///     The extra message information for the current thread.
    /// </returns>
    UIntPtr GetMessageExtraInfo();

    /// <summary>
    ///     Gets the monitor handle from <paramref name="position"/>.
    /// </summary>
    /// <param name="position">
    ///     The position to get the monitor handle for.
    /// </param>
    /// <param name="flags">
    ///     Flags that define what handle to return if a monitor is not
    ///     found that contains <paramref name="position"/>.
    /// </param>
    /// <returns>
    ///     The monitor handle from <paramref name="position"/>.
    /// </returns>
    IntPtr GetMonitorFromPosition(
        Position position,
        MonitorFromPointFlag flags);

    /// <summary>
    ///     Gets the system metric or system configuration
    ///     setting specified by <paramref name="setting"/>.
    /// </summary>
    /// <param name="setting">
    ///     The setting to get the metric of.
    /// </param>
    /// <returns>
    ///     The system metric or system configuration setting
    ///     specified by <paramref name="setting"/>.
    /// </returns>
    int GetSystemMetrics(
        SystemSetting setting);

    /// <summary>
    ///     Sends mouse, keyboard, or hardware inputs.
    /// </summary>
    /// <param name="inputCount">
    ///     The length of <paramref name="inputs"/>.
    /// </param>
    /// <param name="inputs">
    ///     The inputs to send to the mouse, keyboard, or hardware
    ///     controllers.
    /// </param>
    /// <param name="inputSize">
    ///     The size of of <see cref="Input"/>.
    /// </param>
    /// <returns>
    ///     The number of inputs processed.
    /// </returns>
    uint SendInputs(
        uint inputCount,
        Input[] inputs,
        int inputSize);

    /// <inheritdoc cref="Marshal.SizeOf{T}()"/>
    int SizeOf<T>();

    /// <summary>
    ///     Attempt to get the cursor position.
    /// </summary>
    /// <param name="position">
    ///     Output of the position of the cursor.
    /// </param>
    /// <returns>
    ///     True if able to get the cursor position.
    /// </returns>
    bool TryGetCursorPosition(
        out Position position);

    /// <summary>
    ///     Attempt to get the monitor info for <paramref name="monitorHandle"/>.
    /// </summary>
    /// <param name="monitorHandle">
    ///     The handle of the monitor to get the info for.
    /// </param>
    /// <param name="monitorInfo">
    ///     Output of the monitor info for the monitor with a handle of
    ///     <paramref name="monitorHandle"/>.
    /// </param>
    /// <returns>
    ///     True if able to get the monitor info.
    /// </returns>
    bool TryGetMonitorInfo(
        IntPtr monitorHandle,
        ref MonitorInfo monitorInfo);

    /// <summary>
    ///     Attempt to get the last input info.
    /// </summary>
    /// <param name="lastInputInfo">
    ///     Output of the last input info.
    /// </param>
    /// <returns>
    ///     True if able to get the last input info.
    /// </returns>
    bool TryGetLastInputInfo(
        ref LastInputInfo lastInputInfo);
}