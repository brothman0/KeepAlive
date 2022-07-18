using System.Diagnostics.CodeAnalysis;
using KeepAlive.Client.External;
using Microsoft.Extensions.Hosting;

namespace KeepAlive.Client;

/// <summary>
///     Keeps the host machine alive.
/// </summary>
public class KeepAliveService : IHostedService
{
    private const int _radius = 100;
    private readonly ICommonAdapter _commonAdapter;
    private readonly ICommonAgent _commonAgent;
    private readonly IExternalAgent _externalAgent;

    /// <summary>
    ///     Initializes a new instance of <see cref="KeepAliveService"/>
    /// </summary>
    /// <param name="commonAdapter">
    ///     Adapter for common non-mockable instance or static
    ///     methods.
    /// </param>
    /// <param name="commonAgent">
    ///     Agent for common methods.
    /// </param>
    /// <param name="externalAgent">
    ///     Agent used to translate external calls into a more convenient API.
    /// </param>
    public KeepAliveService(
        ICommonAdapter commonAdapter,
        ICommonAgent commonAgent,
        IExternalAgent externalAgent)
    {
        _commonAdapter = commonAdapter;
        _commonAgent = commonAgent;
        _externalAgent = externalAgent;
    }

    /// <inheritdoc cref="IHostedService.StartAsync(CancellationToken)"/>
    public virtual Task StartAsync(
        CancellationToken cancellationToken)
    {
        var factory = new TaskFactory(cancellationToken);
        _ = factory.StartNew(KeepAlive, cancellationToken);
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Keeps the host machine alive by moving the cursor when it detects
    ///     inactivity.
    /// </summary>
    [SuppressMessage("ReSharper", "FunctionNeverReturns", Justification = "Intentional.")]
    internal virtual void KeepAlive()
    {
        var (xStart, yStart) = GetStart();
        while (true)
            if (!TryDrawFigureEight(
                    ref xStart,
                    ref yStart))
                WaitForInActivity(
                    ref xStart,
                    ref yStart);
    }

    /// <summary>
    ///     Gets the coordinates to start the figure eight from.
    /// </summary>
    /// <returns>
    ///     The coordinates to start the figure eight from.
    /// </returns>
    internal virtual (int xStart, int yStart) GetStart()
    {
        var (xPosition, yPosition) = GetCursorPosition();
        RelocateCursor(xPosition, yPosition);
        return (xPosition, yPosition);
    }

    /// <summary>
    ///     Attempts to draw a figure eight with the cursor.
    /// </summary>
    /// <param name="xStart">
    ///     The x coordinate to start the figure eight from.
    /// </param>
    /// <param name="yStart">
    ///     The y coordinate to start the figure eight from.
    /// </param>
    /// <returns>
    ///     True if able to draw a figure eight with the cursor.
    /// </returns>
    internal virtual bool TryDrawFigureEight(
        ref int xStart,
        ref int yStart)
    {
        for (var degree = 0.0; degree < 360; degree += 0.05)
            if (!TryMoveNext(
                    ref xStart,
                    ref yStart,
                    -_radius,
                    degree))
                return false;
        RelocateCursor(xStart, yStart);
        for (var degree = 180.0; degree > -180; degree += -0.05)
            if (!TryMoveNext(
                         ref xStart,
                         ref yStart,
                         _radius,
                         degree))
                return false;
        RelocateCursor(xStart, yStart);
        return true;
    }
    
    /// <summary>
    ///     Attempt to move to the next coordinates.
    /// </summary>
    /// <param name="centerOffset">
    ///     The offset of <paramref name="xStart"/> from the center of
    ///     the circle to draw.
    /// </param>
    /// <param name="degree">
    ///     The degree on the circle to find the next coordinates.
    /// </param>
    /// <returns>
    ///     True if able to move to the next coordinates or if there is
    ///     no move necessary.
    /// </returns>
    /// <inheritdoc cref="TryDrawFigureEight"/>
    internal virtual bool TryMoveNext(
        ref int xStart,
        ref int yStart,
        int centerOffset,
        double degree)
    {
        var (xCenter, yCenter) = (xStart + centerOffset, yStart);
        var (xCurrent, yCurrent) = GetCursorPosition();
        var (xNext, yNext) = GetNext(xCenter, yCenter, degree);
        var xMove = _commonAgent.RoundDown(xNext - xCurrent);
        var yMove = _commonAgent.RoundDown(yNext - yCurrent);
        if (_commonAgent.AbsoluteSum(xMove, yMove) < 1)
            return true;
        if (TryMove(
                xCurrent,
                yCurrent,
                xMove,
                yMove,
                out var xNew,
                out var yNew))
            return true;
        (xStart, yStart) = (xNew, yNew);
        return false;
    }

    /// <summary>
    ///     Gets the next coordinates.
    /// </summary>
    /// <param name="xCenter">
    ///     The x coordinate of the center of the circle to draw.
    /// </param>
    /// <param name="yCenter">
    ///     The x coordinate of the center of the circle to draw.
    /// </param>
    /// <returns>
    ///     The next coordinates.
    /// </returns>
    /// <inheritdoc cref="TryMoveNext"/>
    internal virtual (double xPosition, double yPosition) GetNext(
        int xCenter,
        int yCenter,
        double degree)
    {
        var angle = _commonAgent.ConvertToRadian(degree);
        var (sine, cosine) = _commonAdapter.SineCosine(angle);
        return (xCenter + _radius * cosine, yCenter + _radius * sine);
    }

    /// <summary>
    ///     Attempt to move the cursor.
    /// </summary>
    /// <param name="xCurrent">
    ///     The current x coordinate of the cursor.
    /// </param>
    /// <param name="yCurrent">
    ///     The current y coordinate of the cursor.
    /// </param>
    /// <param name="xMove">
    ///     The pixels to move the cursor along the x-axis.
    /// </param>
    /// <param name="yMove">
    ///     The pixels to move the cursor along the y-axis.
    /// </param>
    /// <param name="xNew">
    ///     Output of the new x coordinate of the cursor.
    /// </param>
    /// <param name="yNew">
    ///     Output of the new y coordinate of the cursor.
    /// </param>
    /// <returns>
    ///     True if able to move the cursor and the cursor moved
    ///     to the expected coordinates.
    /// </returns>
    /// <exception cref="ExternalException">
    ///     Thrown if unable to move the cursor.
    /// </exception>
    internal virtual bool TryMove(
        int xCurrent,
        int yCurrent,
        int xMove,
        int yMove,
        out int xNew,
        out int yNew)
    {
        const int postMoveWaitTicks = 25000;
        var (xExpected, yExpected) = (xCurrent + xMove, yCurrent + yMove);
        if (!_externalAgent.TryMoveCursor(xMove, yMove))
            throw new ExternalException(
                _externalAgent,
                "Unable to move the cursor.");
        (xNew, yNew) = GetCursorPosition();
        if (xExpected != xNew || yExpected != yNew)
            return false;
        _commonAgent.Wait(postMoveWaitTicks);
        return true;
    }

    /// <summary>
    ///     Relocates the cursor.
    /// </summary>
    /// <param name="xPosition">
    ///     The x coordinate to relocate the cursor to.
    /// </param>
    /// <param name="yPosition">
    ///     The y coordiante to relocate the cursor to.
    /// </param>
    /// <exception cref="ExternalException">
    ///     Thrown if unable to relocate the cursor.
    /// </exception>
    internal virtual void RelocateCursor(
        int xPosition,
        int yPosition)
    {
        if (!_externalAgent.TryRelocateCursor(
                xPosition,
                yPosition))
            throw new ExternalException(
                _externalAgent,
                "Unable to relocate the cursor.");
    }

    /// <summary>
    ///     Wait until there is a sufficiently long period of
    ///     inactivity.
    /// </summary>
    /// <param name="xStart">
    ///     The x coordinate to start the figure eight from.
    /// </param>
    /// <param name="yStart">
    ///     The y coordinate to start the figure eight from.
    /// </param>
    internal virtual void WaitForInActivity(
        ref int xStart,
        ref int yStart)
    {
        const int activityWaitSeconds = 15;
        const int activityChecks = 20;
        var activityWait = new TimeSpan(0, 0, activityWaitSeconds);
        for (var checks = 0; checks < activityChecks; checks++)
        {
            _commonAgent.Wait(activityWait);
            var (xCurrent, yCurrent) = GetCursorPosition();
            if (DidTravel(xStart, yStart, xCurrent, yCurrent))
                checks = 0;
            xStart = xCurrent;
            yStart = yCurrent;
        }
    }

    /// <summary>
    ///     Determines if there was travel between the start and
    ///     current coordinates.
    /// </summary>
    /// <returns>
    ///     True if there was travel between the start and current
    ///     coordinates.
    /// </returns>
    /// <inheritdoc cref="WaitForInActivity"/>
    /// <inheritdoc cref="TryMove"/>
    internal virtual bool DidTravel(
        int xStart,
        int yStart,
        int xCurrent,
        int yCurrent)
    {
        return _commonAgent
            .AbsoluteSum(
                xStart - xCurrent,
                yStart - yCurrent) > 0;
    }

    /// <summary>
    ///     Gets the current cursor position.
    /// </summary>
    /// <returns>
    ///     The current cursor position.
    /// </returns>
    /// <exception cref="ExternalException">
    ///     Thrown if unable to get the cursor position.
    /// </exception>
    internal virtual (int, int) GetCursorPosition()
    {
        if (!_externalAgent.TryGetCursorPosition(
                out var xPosition,
                out var yPosition))
            throw new ExternalException(
                _externalAgent,
                "Unable to get the cursor position.");
        return (xPosition.Value, yPosition.Value);
    }
    
    /// <inheritdoc cref="IHostedService.StopAsync(CancellationToken)"/>
    public virtual Task StopAsync(
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}