using System.Runtime.InteropServices;

namespace KeepAlive.External.Resources.TryGetCursorPosition;

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

    /// <summary>
    ///     Initializes a new instance of <see cref="Position"/>.
    /// </summary>
    /// <param name="x">
    ///     The x-axis coordinate of the cursor.
    /// </param>
    /// <param name="y">
    ///     The y-axis coordinate of the cursor.
    /// </param>
    public Position(
        int x,
        int y)
    {
        X = x;
        Y = y;
    }
}