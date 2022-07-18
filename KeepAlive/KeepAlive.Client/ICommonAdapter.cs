using System.Diagnostics;

namespace KeepAlive;

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

    /// <inheritdoc cref="Math.Abs(double)"/>
    double Absolute(
        double value);

    /// <inheritdoc cref="Convert.ToByte(double)"/>
    byte ConvertToByte(
        double value);

    /// <inheritdoc cref="Convert.ToInt32(double)"/>
    int ConvertToInt(
        double value);
    
    /// <inheritdoc cref="Convert.ToInt64(double)"/>
    long ConvertToLong(
        double value);

    /// <inheritdoc cref="Convert.ToInt16(double)"/>
    short ConvertToShort(
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