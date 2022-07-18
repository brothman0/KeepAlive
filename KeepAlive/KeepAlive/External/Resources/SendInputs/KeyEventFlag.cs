namespace KeepAlive.Client.External.Resources.SendInputs;

/// <summary>
///     Flags describing the keyboard event enact.
/// </summary>
[Flags]
public enum KeyEventFlag : uint
{
    /// <summary>
    ///     No flags applied.
    /// </summary>
    None = 0x0000,

    /// <summary>
    ///     If specified, the scan code was preceded by a prefix
    ///     byte that has the value 0xE0
    /// </summary>
    ExtendedKey = 0x0001,

    /// <summary>
    ///     If specified, the key is being released. If not specified,
    ///     the key is being pressed.
    /// </summary>
    KeyPressOrRelease = 0x0002,

    /// <summary>
    ///     If specified, the keyboard event will output a unicode
    ///     character.
    /// </summary>
    Unicode = 0x0004,

    /// <summary>
    ///     If specified indicates the ScanCode should be used.
    /// </summary>
    ScanCode = 0x0008
}