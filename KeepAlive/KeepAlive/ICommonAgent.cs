namespace KeepAlive.Client;

/// <summary>
///     Used as a agent for common methods.
/// </summary>
public interface ICommonAgent
{
    /// <summary>
    ///     Calcualtes the absolute sum of <paramref name="values"/>.
    /// </summary>
    /// <param name="values">
    ///     The values to get calcualte the absolute sum of.
    /// </param>
    /// <returns>
    ///     The absolute sum of <paramref name="values"/>.
    /// </returns>
    int AbsoluteSum(
        params int[] values);
    
    /// <summary>
    ///     Converts <paramref name="degrees"/> to radian.
    /// </summary>
    /// <param name="degrees">
    ///     The degrees to convert.
    /// </param>
    /// <returns>
    ///     <paramref name="degrees"/> as radian.
    /// </returns>
    double ConvertToRadian(
        double degrees);
    
    /// <summary>
    ///     Rounds <paramref name="value"/> down and converts to
    ///     <see cref="int"/>.
    /// </summary>
    /// <param name="value">
    ///     The value to round down.
    /// </param>
    /// <returns>
    ///     <paramref name="value"/> rounded down as an <see cref="int"/>.
    /// </returns>
    int RoundDown(
        double value);

    /// <summary>
    ///     Blocks the thread, waiting for <paramref name="ticks"/> ticks
    ///     elapse.
    /// </summary>
    /// <param name="ticks">
    ///     The ticks to wait.
    /// </param>
    void Wait(
        long ticks);

    /// <summary>
    ///     Blocks the thread, waiting for <paramref name="waitTime"/> time
    ///     to pass.
    /// </summary>
    /// <param name="waitTime">
    ///     The time to wait.
    /// </param>
    void Wait(
        TimeSpan waitTime);
}