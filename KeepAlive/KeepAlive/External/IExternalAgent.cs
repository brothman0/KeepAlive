using System.Diagnostics.CodeAnalysis;

namespace KeepAlive.Client.External;

/// <summary>
///     Agent used to translate external calls into a more convenient API.
/// </summary>
public interface IExternalAgent
{
    /// <summary>
    ///     Gets the current Win32 error message if exists.
    /// </summary>
    /// <returns>
    ///     The current Win32 error message.
    /// </returns>
    string? GetErrorMessage();

    /// <summary>
    ///     Attempt to get the cursor position.
    /// </summary>
    /// <param name="xPosition">
    ///     Output of the position of the cursor on the x-axis.
    /// </param>
    /// <param name="yPosition">
    ///     Output of the position of the cursor on the y-axis.
    /// </param>
    /// <returns>
    ///     True if able to get the cursor position.
    /// </returns>
    bool TryGetCursorPosition(
        [NotNullWhen(true)]
        out int? xPosition,
        [NotNullWhen(true)]
        out int? yPosition);

    /// <summary>
    ///     Attempt to move the cursor.
    /// </summary>
    /// <param name="xMove">
    ///     The pixels to move the cursor along the x-axis.
    /// </param>
    /// <param name="yMove">
    ///     The pixels to move the cursor along the y-axis.
    /// </param>
    /// <returns>
    ///     True if able to move the cursor.
    /// </returns>
    bool TryMoveCursor(
        int xMove,
        int yMove);

    /// <summary>
    ///     Attempts to relocate the cursor.
    /// </summary>
    /// <param name="xPosition">
    ///     The coordinate on the x-axis to move the cursor to.
    /// </param>
    /// <param name="yPosition">
    ///     The coordinate on the y-axis to move the cursor to.
    /// </param>
    /// <returns>
    ///     True if able to relocate the cursor.
    /// </returns>
    public bool TryRelocateCursor(
        int xPosition,
        int yPosition);
}