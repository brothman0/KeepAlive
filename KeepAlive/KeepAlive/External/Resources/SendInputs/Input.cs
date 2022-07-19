using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace KeepAlive.External.Resources.SendInputs;

/// <summary>
///     Contains information about a simulated mouse, keyboard, or
///     hardware event.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
[ExcludeFromCodeCoverage(Justification = "Structs with no logic do not require coverage.")]
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

    public MouseInput MouseInput
    {
        get => Union.MouseInput;
        set => Union.MouseInput = value;
    }

    public KeyboardInput KeyboardInput
    {
        get => Union.KeyboardInput;
        set => Union.KeyboardInput = value;
    }

    public HardwareInput HardwareInput
    {
        get => Union.HardwareInput;
        set => Union.HardwareInput = value;
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="Input"/> used
    ///     to move the cursor.
    /// </summary>
    /// <inheritdoc cref="MouseInput.MouseInput(int, int, MouseEventFlag, UIntPtr)"/>
    public Input(
        int xInput,
        int yInput,
        MouseEventFlag flags,
        UIntPtr extraInfo)
    {
        Type = InputType.Mouse;
        MouseInput = new MouseInput(xInput, yInput, flags, extraInfo);
    }
}