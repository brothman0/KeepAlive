using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace KeepAlive;

/// <inheritdoc cref="ICommonAdapter"/>
[ExcludeFromCodeCoverage(Justification = "Adapters do not require coverage.")]
public sealed class CommonAdapter : ICommonAdapter
{
    /// <inheritdoc cref="ICommonAdapter.Pi"/>
    public double Pi => Math.PI;

    /// <inheritdoc cref="ICommonAdapter.Absolute(int)"/>
    public int Absolute(
        int value)
    {
        return Math.Abs(value);
    }

    /// <inheritdoc cref="ICommonAdapter.Absolute(double)"/>
    public double Absolute(
        double value)
    {
        return Math.Abs(value);
    }

    /// <inheritdoc cref="ICommonAdapter.ConvertToByte"/>
    public byte ConvertToByte(
        double value)
    {
        return Convert.ToByte(value);
    }

    /// <inheritdoc cref="ICommonAdapter.ConvertToInt"/>
    public int ConvertToInt(
        double value)
    {
        return Convert.ToInt32(value);
    }

    /// <inheritdoc cref="ICommonAdapter.ConvertToLong"/>
    public long ConvertToLong(
        double value)
    {
        return Convert.ToInt64(value);
    }

    /// <inheritdoc cref="ICommonAdapter.ConvertToShort"/>
    public short ConvertToShort(
        double value)
    {
        return Convert.ToInt16(value);
    }

    /// <inheritdoc cref="ICommonAdapter.Floor"/>
    public double Floor(
        double d)
    {
        return Math.Floor(d);
    }

    /// <inheritdoc cref="ICommonAdapter.SineCosine"/>
    public (double sine, double cosine) SineCosine(
        double x)
    {
        return Math.SinCos(x);
    }

    /// <inheritdoc cref="ICommonAdapter.Stopwatch"/>
    public Stopwatch StartStopwatch()
    {
        return Stopwatch.StartNew();
    }
}