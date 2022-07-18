using System.Text;
using KeepAlive.External.Resources.FormatMessage;
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
    
    /// <summary>
    ///     Gets the extra message information for the current thread.
    /// </summary>
    /// <returns>
    ///     The extra message information for the current thread.
    /// </returns>
    UIntPtr GetMessageExtraInfo();

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

    bool TryGetMonitorInfo(
        IntPtr monitorHandle,
        ref MonitorInfo monitorInfo);
}