using System.Diagnostics;

namespace KeepAlive;

/// <inheritdoc cref="System.Diagnostics.Stopwatch"/>
/// <remarks>
///     This is a wraper around <see cref="System.Diagnostics.Stopwatch"/>
/// </remarks>
public interface IStopwatch
{
    /// <inheritdoc cref="Stopwatch.Elapsed"/>
    TimeSpan Elapsed { get; }

    /// <inheritdoc cref="Stopwatch.ElapsedMilliseconds"/>
    long ElapsedMilliseconds { get; }

    /// <inheritdoc cref="Stopwatch.ElapsedTicks"/>
    long ElapsedTicks { get; }

    /// <inheritdoc cref="Stopwatch.IsRunning"/>
    bool IsRunning { get; }

    /// <inheritdoc cref="Stopwatch.Reset"/>
    void Reset();

    /// <inheritdoc cref="Stopwatch.Restart"/>
    void Restart();

    /// <inheritdoc cref="Stopwatch.Start"/>
    void Start();

    /// <inheritdoc cref="Stopwatch.Stop"/>
    void Stop();
}