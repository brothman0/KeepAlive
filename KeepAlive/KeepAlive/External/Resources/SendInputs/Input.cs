using System.Runtime.InteropServices;

namespace KeepAlive.Client.External.Resources.SendInputs;

/// <summary>
///     Contains information about a simulated mouse, keyboard, or
///     hardware event.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Input
{
    /// <summary>
    ///     The type of input.
    /// </summary>
    public InputType Type = default;

    /// <summary>
    ///     Union of <see cref="MouseInput"/>, <see cref="KeyboardInput"/>,
    ///     and <see cref="HardwareInput"/>.
    /// </summary>
    public InputUnion Union = new();

    /// <summary>
    ///     Initializes a new instance of <see cref="Input"/> used
    ///     to move the cursor.
    /// </summary>
    /// <param name="adapter">
    ///     Adapter used to make external calls.
    /// </param>
    /// <inheritdoc cref="MouseInput(int, int, MouseEventFlag, UIntPtr)"/>
    public Input(
        int xInput,
        int yInput,
        MouseEventFlag flags,
        IExternalAdapter adapter)
    {
        Type = InputType.Mouse;
        var extraInfo = adapter.GetMessageExtraInfo();
        Union.MouseInput = 
            new MouseInput(xInput, yInput, flags, extraInfo);
    }
}