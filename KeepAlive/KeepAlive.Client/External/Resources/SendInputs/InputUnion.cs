using System.Runtime.InteropServices;

namespace KeepAlive.External.Resources.SendInputs;

/// <summary>
///     Union of <see cref="MouseInput"/>, <see cref="KeyboardInput"/>,
///     and <see cref="HardwareInput"/>.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct InputUnion
{
    /// <summary>
    ///     The cursor input.
    /// </summary>
    [FieldOffset(0)]
    public MouseInput MouseInput = new();

    /// <summary>
    ///     The keyboard input.
    /// </summary>
    [FieldOffset(0)]
    public KeyboardInput KeyboardInput = new();

    /// <summary>
    ///     The hardware input.
    /// </summary>
    [FieldOffset(0)]
    public HardwareInput HardwareInput = new();

    public InputUnion()
    {
    }
}