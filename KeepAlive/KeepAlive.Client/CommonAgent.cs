namespace KeepAlive;

/// <inheritdoc cref="ICommonAgent"/>
public class CommonAgent : ICommonAgent
{
    private readonly ICommonAdapter _adapter;

    /// <summary>
    ///     Initializes a new instance of <see cref="CommonAgent"/>.
    /// </summary>
    /// <param name="adapter">
    ///     common adapter for non-mockable instance or static
    ///     methods.
    /// </param>
    public CommonAgent(
        ICommonAdapter adapter)
    {
        _adapter = adapter;
    }

    /// <inheritdoc cref="ICommonAgent.AbsoluteSum"/>
    public virtual int AbsoluteSum(
        params int[] values)
    {
        return values.Select(x => _adapter.Absolute(x)).Sum();
    }

    /// <inheritdoc cref="ICommonAgent.ConvertToRadian"/>
    public virtual double ConvertToRadian(
        double degrees)
    {
        const double piRadian = 180;
        return _adapter.Pi * degrees / piRadian;
    }
    
    /// <inheritdoc cref="ICommonAgent.RoundDown"/>
    public virtual int RoundDown(
        double value)
    {
        return _adapter.ConvertToInt(
            _adapter.Floor(value));
    }

    /// <inheritdoc cref="ICommonAgent.Wait(long)"/>
    public virtual void Wait(
        long ticks)
    {
        var stopwatch = _adapter.StartStopwatch();
        while (stopwatch.ElapsedTicks < ticks)
        {
        }
    }
    
    /// <inheritdoc cref="ICommonAgent.Wait(TimeSpan)"/>
    public virtual void Wait(
        TimeSpan waitTime)
    {
        Wait(waitTime.Ticks);
    }
}