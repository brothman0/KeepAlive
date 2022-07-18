﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace KeepAlive.Client;

/// <summary>
///     Used as an adapter for common non-mockable instance
///     or static methods.
/// </summary>
public interface ICommonAdapter
{
    /// <inheritdoc cref="Math.PI"/>
    double Pi { get; }

    /// <inheritdoc cref="Math.Abs(int)"/>
    int Absolute(
        int value);

    /// <inheritdoc cref="Convert.ToInt32(double)"/>
    int ConvertToInt(
        double value);

    /// <inheritdoc cref="Math.Floor(double)"/>
    double Floor(
        double d);

    /// <inheritdoc cref="Math.SinCos"/>
    (double sine, double cosine) SineCosine(
        double x);

    /// <inheritdoc cref="Stopwatch.StartNew"/>
    Stopwatch StartStopwatch();
}