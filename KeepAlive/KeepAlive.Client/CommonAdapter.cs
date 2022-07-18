using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace KeepAlive;

/// <inheritdoc cref="ICommonAdapter"/>
[ExcludeFromCodeCoverage(Justification = "Adapters do not require coverage.")]
public sealed class CommonAdapter : ICommonAdapter
{
    /// <inheritdoc cref="ICommonAdapter.Pi"/>
    public double Pi => Math.PI;

    /// <inheritdoc cref="ICommonAdapter.Absolute"/>
    public int Absolute(
        int value)
    {
        return Math.Abs(value);
    }

    /// <inheritdoc cref="ICommonAdapter.ConvertToInt"/>
    public int ConvertToInt(
        double value)
    {
        return Convert.ToInt32(value);
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