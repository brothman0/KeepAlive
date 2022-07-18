namespace KeepAlive.Client.External.Resources.FormatMessage;

/// <summary>
///     Indicates options to use when formatting a message.
/// </summary>
public enum FormatOption : uint
{
    /// <summary>
    ///     Indicates that the message should be returned as
    ///     a pointer to the buffer.
    /// </summary>
    AllocateBuffer = 0x00000100,

    /// <summary>
    ///     Indicates the argument array is a pointer to
    ///     an array.
    /// </summary>
    ArgumentArray = 0x00002000,

    /// <summary>
    ///     Indicates that the message should be retrieved
    ///     from the specified location.
    /// </summary>
    FromHModule = 0x00000800,

    /// <summary>
    ///     Indicates that the message should be shown through
    ///     a reference to a string.
    /// </summary>
    FromString = 0x00000400,

    /// <summary>
    ///     Indicates that the message should be retrieved from
    ///     the system message tables.
    /// </summary>
    FromSystem = 0x00001000,

    /// <summary>
    ///     Insert sequences are ignored.
    /// </summary>
    IgnoreInserts = 0x00000200
}