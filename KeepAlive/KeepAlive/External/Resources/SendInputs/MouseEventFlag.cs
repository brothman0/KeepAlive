namespace KeepAlive.External.Resources.SendInputs;

/// <summary>
///     Flags describing the mouse event to inact.
/// </summary>
[Flags]
public enum MouseEventFlag : uint
{
    /// <summary>
    ///     Move the cursor either by relative inputs, or, if included
    ///     with <see cref="Absolute"/>, to specified coordinates.
    /// </summary>
    Move = 0x0001,

    /// <summary>
    ///     Click the left mouse button.
    /// </summary>
    LeftClick = 0x0002,

    /// <summary>
    ///     Release the left mouse button.
    /// </summary>
    LeftRelease = 0x0004,

    /// <summary>
    ///     Click the right mouse button.
    /// </summary>
    RightClick = 0x0008,

    /// <summary>
    ///     Release the right mouse button.
    /// </summary>
    RightRelease = 0x0010,

    /// <summary>
    ///     Click the middle mouse button.
    /// </summary>
    MiddleClick = 0x0020,

    /// <summary>
    ///     Release the middle mouse button.
    /// </summary>
    MiddleRelease = 0x0040,

    /// <summary>
    ///     Click either the X1 or X2 mouse button.
    /// </summary>
    XClick = 0x0080,

    /// <summary>
    ///     Release either the X1 or X2 mouse button.
    /// </summary>
    XRelease = 0x0100,

    /// <summary>
    ///     Move the scroll wheel vertically.
    /// </summary>
    VerticalWheelMove = 0x0800,

    /// <summary>
    ///     Move the scroll wheel horizontally.
    /// </summary>
    HorizontalWheelMove = 0x01000,


    /// <summary>
    ///     Move the cursor without coalescing the event.
    /// </summary>
    MoveNoCoalesce = 0x2000,

    /// <summary>
    ///     Maps coordinates to the entire desktop when used
    ///     with <see cref="Absolute"/>.
    /// </summary>
    VirtualDesk = 0x4000,

    /// <summary>
    ///     Indicates that the cursor movement should be to
    ///     coordinates.
    /// </summary>
    Absolute = 0x8000,

    /// <summary>
    ///     Relocate the cursor to specified coordinates.
    /// </summary>
    Relocate = Move | Absolute
}