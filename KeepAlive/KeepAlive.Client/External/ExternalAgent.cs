using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using KeepAlive.External.Resources.FormatMessage;
using KeepAlive.External.Resources.GetSystemMetric;
using KeepAlive.External.Resources.GetMonitorFromPoint;
using KeepAlive.External.Resources.SendInputs;
using KeepAlive.External.Resources.TryGetCursorPosition;
using KeepAlive.External.Resources.TryGetMonitorInfo;

namespace KeepAlive.External;

/// <inheritdoc cref="IExternalAgent"/>
public class ExternalAgent : IExternalAgent
{
    private readonly IExternalAdapter _adapter;

    /// <summary>
    ///     Initializes a new instance of <see cref="ExternalAgent"/>.
    /// </summary>
    /// <param name="adapter">
    ///     Adapter used to make external calls.
    /// </param>
    public ExternalAgent(
        IExternalAdapter adapter)
    {
        _adapter = adapter;
    }

    /// <inheritdoc cref="IExternalAgent.GetErrorMessage"/>
    public virtual string? GetErrorMessage()
    {
        const int bufferCapacity = 512;
        var messageId = Marshal.GetLastWin32Error();
        var buffer = new StringBuilder(bufferCapacity);
        return FormatMessage(messageId, buffer) == 0 ?
            null :
            buffer.ToString();
    }

    /// <summary>
    ///     Formats the message into english from the system, and
    ///     writes it to <paramref name="buffer"/>.
    /// </summary>
    /// <param name="messageId">
    ///     The message Id to get the message for.
    /// </param>
    /// <param name="buffer">
    ///     The buffer to write the message to.
    /// </param>
    /// <returns>
    ///     The number of characters written to <paramref name="buffer"/>.
    /// </returns>
    [ExcludeFromCodeCoverage(Justification = "Methods without logic do not require coverage.")]
    internal virtual uint FormatMessage(
        int messageId,
        StringBuilder buffer)
    {
        const uint English = 0x0409;
        return _adapter.FormatMessage(
            FormatOption.FromSystem,
            IntPtr.Zero,
            (uint)messageId,
            English,
            buffer,
            buffer.Capacity,
            IntPtr.Zero);
    }

    /// <inheritdoc cref="IExternalAgent.TryGetCursorPosition"/>
    public virtual bool TryGetCursorPosition(
        [NotNullWhen(true)]
        out int? xPosition,
        [NotNullWhen(true)]
        out int? yPosition)
    {
        xPosition = null;
        yPosition = null;
        if (!_adapter.TryGetCursorPosition(out var position))
            return false;
        xPosition = position.X;
        yPosition = position.Y;
        return true;
    }

    /// <inheritdoc cref="IExternalAgent.TryGetCursorWorkArea"/>
    public virtual bool TryGetCursorWorkArea(
        int xPosition,
        int yPosition,
        out Rectangle workArea)
    {
        workArea = new Rectangle();
        if (!TryGetMonitorHandleFromPosition(
                xPosition,
                yPosition,
                out var monitorHandle))
            return false;
        var monitorInfo = new MonitorInfo();
        if (!_adapter.TryGetMonitorInfo(monitorHandle, ref monitorInfo))
            return false;
        workArea = monitorInfo.WorkArea;
        return true;
    }

    /// <summary>
    ///     Attempts to get the monitor handle from the position.
    /// </summary>
    /// <param name="xPosition">
    ///     The x-axis coordinate to find the monitor from.
    /// </param>
    /// <param name="yPosition">
    ///     The y-axis coordiante to find the monitor from.
    /// </param>
    /// <param name="monitorHandle">
    ///     Output of the handle to the found monitor.
    /// </param>
    /// <returns>
    ///     True if able to get the monitor handle from the position.
    /// </returns>
    internal virtual bool TryGetMonitorHandleFromPosition(
        int xPosition,
        int yPosition,
        out IntPtr monitorHandle)
    {
        const MonitorFromPointFlag flags = MonitorFromPointFlag.DefaultToNearest;
        var position = new Position(xPosition, yPosition);
        monitorHandle = _adapter.GetMonitorFromPosition(
            position,
            flags);
        return monitorHandle != IntPtr.Zero;
    }

    /// <inheritdoc cref="IExternalAgent.TryMoveCursor"/>
    public virtual bool TryMoveCursor(
        int xMove, 
        int yMove)
    {
        const MouseEventFlag flags = MouseEventFlag.Move;
        const int expectedResponse = 1;
        var input = new Input(xMove, yMove, flags, _adapter);
        return SendInput(input) == expectedResponse;
    }

    /// <inheritdoc cref="IExternalAgent.TryRelocateCursor"/>
    public virtual bool TryRelocateCursor(
        int xPosition,
        int yPosition)
    {
        const MouseEventFlag flags = MouseEventFlag.Relocate;
        const int expectedResponse = 1;
        if (!TryGetAbsolutePosition(ref xPosition, ref yPosition))
            return false;
        var input = new Input(xPosition, yPosition, flags, _adapter);
        return SendInput(input) == expectedResponse;
    }
    
    /// <summary>
    ///     Attempt to get the absolute position of the cursor.
    /// </summary>
    /// <param name="xPosition">
    ///     Reference to the position of the cursor on the x-axis. If
    ///     able to get the absolute position, this is updated to the
    ///     absolute position of the cursor on the x-axis.
    /// </param>
    /// <param name="yPosition">
    ///     Reference to the position of the cursor on the y-axis. If
    ///     able to get the aboslute position, this is updated to the
    ///     absolute position of the cursor on the y-axis.
    /// </param>
    /// <returns>
    ///     True if able to get the absolute position of the cursor.
    /// </returns>
    internal virtual bool TryGetAbsolutePosition(
        ref int xPosition,
        ref int yPosition)
    {
        const int absoluteReference = 65536;
        if (!TryGetScreenWidth(out var screenWidth) ||
            !TryGetScreenHeight(out var screenHeight))
            return false;
        xPosition = xPosition * absoluteReference / screenWidth;
        yPosition = yPosition * absoluteReference / screenHeight;
        return true;
    }

    /// <summary>
    ///     Attempt to get the screen width.
    /// </summary>
    /// <param name="screenWidth">
    ///     Output of the screen width.
    /// </param>
    /// <returns>
    ///     True if able to get the screen width.
    /// </returns>
    internal virtual bool TryGetScreenWidth(
        out int screenWidth)
    {
        const SystemSetting setting = SystemSetting.ScreenWidth; 
        screenWidth = _adapter.GetSystemMetrics(setting);
        return screenWidth != 0;
    }

    /// <summary>
    ///     Attempt to get the screen height.
    /// </summary>
    /// <param name="screenHeight">
    ///     Output of the screen height.
    /// </param>
    /// <returns>
    ///     True if able to get the screen height.
    /// </returns>
    internal virtual bool TryGetScreenHeight(
        out int screenHeight)
    {
        const SystemSetting setting = SystemSetting.ScreenHeight;
        screenHeight = _adapter.GetSystemMetrics(setting);
        return screenHeight != 0;
    }
    
    /// <summary>
    ///     Sends a single mouse, keyboard, or hardware input to
    ///     be processed.
    /// </summary>
    /// <param name="input">
    ///     The input to send.
    /// </param>
    /// <returns>
    ///     The number of inputs processed.
    /// </returns>
    [ExcludeFromCodeCoverage(Justification = "Methods without logic do not require coverage.")]
    internal virtual uint SendInput(
        Input input)
    {
        return SendInputs(new[] { input });
    }

    /// <summary>
    ///     Sends an array of mouse, keybaord, or hardware inputs
    ///     to be processed.
    /// </summary>
    /// <param name="inputs">
    ///     The inputs to send.
    /// </param>
    /// <inheritdoc cref="SendInput(Input)"/>
    [ExcludeFromCodeCoverage(Justification = "Methods without logic do not require coverage.")]
    internal virtual uint SendInputs(
        Input[] inputs)
    {
        return _adapter.SendInputs(
            (uint)inputs.Length,
            inputs,
            Marshal.SizeOf<Input>());
    }
}