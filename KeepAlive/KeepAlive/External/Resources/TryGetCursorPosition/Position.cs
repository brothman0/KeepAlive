using System.Runtime.InteropServices;

namespace KeepAlive.Client.External.Resources.TryGetCursorPosition;

/// <summary>
///     Used to indicate the position of the cursor.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Position
{
    /// <summary>
    ///     The x-axis coordinate of the cursor.
    /// </summary>
    public int X;

    /// <summary>
    ///     The y-axis coordinate of the cursor.
    /// </summary>
    public int Y;
}