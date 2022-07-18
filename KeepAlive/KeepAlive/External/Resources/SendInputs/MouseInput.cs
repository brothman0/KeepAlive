using System.Runtime.InteropServices;

namespace KeepAlive.Client.External.Resources.SendInputs;

/// <summary>
///     Contains information about a simulated mouse event.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct MouseInput
{
    /// <summary>
    ///     The distance to move the cursor along the x-axis, or the
    ///     absolute position of the cursor along the x-axis, depending
    ///     on <see cref="Flags"/>.
    /// </summary>
    public int XInput;

    /// <summary>
    ///     The distance to move the cursor along the y-axis, or the
    ///     absolute position of the cursor along the y-axis, depending
    ///     on <see cref="Flags"/>.
    /// </summary>
    public int YInput;

    /// <summary>
    ///     Either zero, the the distance to move the scroll wheel, or
    ///     the X1 and/or X2 keys to press, depending on <see cref="Flags"/>.
    /// </summary>
    public int MouseData = 0;

    /// <summary>
    ///     Flags describing the mouse event enact.
    /// </summary>
    public MouseEventFlag Flags;

    /// <summary>
    ///     The timestamp for the event.
    /// </summary>
    public uint Timestamp = 0;

    /// <summary>
    ///     Extra message information for the current thread.
    /// </summary>
    public UIntPtr ExtraInfo;

    /// <summary>
    ///     Initializes an instance of <see cref="MouseInput"/> used
    ///     to move the cursor.
    /// </summary>
    /// <param name="xInput">
    ///     The distance to move the cursor along the x-axis, or the
    ///     absolute position of the cursor along the x-axis, depending
    ///     on <paramref name="flags"/>.
    /// </param>
    /// <param name="yInput">
    ///     The distance to move the cursor along the y-axis, or the
    ///     absolute position of the cursor along the y-axis, depending
    ///     on <paramref name="flags"/>.
    /// </param>
    /// <param name="flags">
    ///     Flags describing the cursor movement to enact.
    /// </param>
    /// <param name="extraInfo">
    ///     Extra message information for the current thread.
    /// </param>
    public MouseInput(
        int xInput,
        int yInput,
        MouseEventFlag flags,
        UIntPtr extraInfo)
    {
        XInput = xInput;
        YInput = yInput;
        Flags = flags;
        ExtraInfo = extraInfo;
    }
}