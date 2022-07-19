using System.Diagnostics.CodeAnalysis;

namespace KeepAlive;

/// <inheritdoc cref="IStopwatch"/>
[ExcludeFromCodeCoverage(Justification = "Wrappers do not require code coverage.")]
public class Stopwatch : IStopwatch
{
    private readonly System.Diagnostics.Stopwatch _stopwatch;

    /// <inheritdoc cref="IStopwatch.Elapsed"/>
    public TimeSpan Elapsed => _stopwatch.Elapsed;

    /// <inheritdoc cref="IStopwatch.ElapsedMilliseconds"/>
    public long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;

    /// <inheritdoc cref="IStopwatch.ElapsedTicks"/>
    public long ElapsedTicks => _stopwatch.ElapsedTicks;

    /// <inheritdoc cref="IStopwatch.IsRunning"/>
    public bool IsRunning => _stopwatch.IsRunning;

    /// <summary>
    ///     Initializes a new instance of <see cref="Stopwatch"/>.
    /// </summary>
    /// <param name="stopwatch">
    ///     The stopwatch this is a wraper of.
    /// </param>
    public Stopwatch()
    {
        _stopwatch = System.Diagnostics.Stopwatch.StartNew();
    }
    
    /// <inheritdoc cref="IStopwatch.Reset"/>
    public void Reset() => _stopwatch.Reset();

    /// <inheritdoc cref="IStopwatch.Restart"/>
    public void Restart() => _stopwatch.Restart();

    /// <inheritdoc cref="IStopwatch.Start"/>
    public void Start() => _stopwatch.Start();

    /// <inheritdoc cref="IStopwatch.Stop"/>
    public void Stop() => _stopwatch.Stop();
}