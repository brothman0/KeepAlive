﻿using System.Runtime.InteropServices;

namespace KeepAlive.Client.External.Resources.SendInputs;

/// <summary>
///     Contains information about a simulated keyboard event.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct KeyboardInput
{
    /// <summary>
    ///     Input used for non-unicode.
    /// </summary>
    public KeyInput KeyInput;

    /// <summary>
    ///     Input used for unicode.
    /// </summary>
    public ScanCode ScanCode;

    /// <summary>
    ///     Flags describing the keyboard event enact.
    /// </summary>
    public KeyEventFlag Flag;

    /// <summary>
    ///     The timestamp for the event.
    /// </summary>
    public int Time = 0;

    /// <summary>
    ///     Extra message information for the current thread.
    /// </summary>
    public UIntPtr ExtraInfo;
}