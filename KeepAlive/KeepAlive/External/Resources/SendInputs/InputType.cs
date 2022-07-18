namespace KeepAlive.Client.External.Resources.SendInputs;

/// <summary>
///     Indicates the type of event.
/// </summary>
public enum InputType : uint
{
    /// <summary>
    ///     Indicates the event is a mouse event.
    /// </summary>
    Mouse,

    /// <summary>
    ///     Indicates the event is a keybaord event.
    /// </summary>
    Keyboard,

    /// <summary>
    ///     Indicates the event is a hardware event.
    /// </summary>
    Hardware
}