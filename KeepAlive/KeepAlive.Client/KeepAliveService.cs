using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using KeepAlive.External;
using Microsoft.Extensions.Hosting;

namespace KeepAlive;

/// <summary>
///     Keeps the host machine alive.
/// </summary>
public class KeepAliveService : IHostedService
{
    private const int _radius = 100;
    private const int _diameter = _radius * 2;
    private const double _circleDegrees = 360;
    private const double _degreeIncrement = 0.5;
    private const double _movesPerCircle = _circleDegrees / _degreeIncrement;
    private const int _defaultPostMoveTicks = 25000;
    private readonly int _targetTicksPerMove;
    private int _postMoveTicks = _defaultPostMoveTicks;
    private readonly ICommonAdapter _commonAdapter;
    private readonly ICommonAgent _commonAgent;
    private readonly IExternalAgent _externalAgent;

    

    public bool IsRunning { get; set; } = true;

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
        const double targetTicksPerCircle = 20000000;
        _commonAdapter = commonAdapter;
        _commonAgent = commonAgent;
        _externalAgent = externalAgent;
        _targetTicksPerMove = _commonAgent.RoundDownToInt(
            targetTicksPerCircle / _movesPerCircle);
    }

    /// <inheritdoc cref="IHostedService.StartAsync(CancellationToken)"/>
    [ExcludeFromCodeCoverage(Justification = "Methods without logic do not require coverage.")]
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
    internal virtual void KeepAlive()
    {
        var (xStart, yStart) = GetStart();
        while (IsRunning)
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
    [ExcludeFromCodeCoverage(Justification = "Methods without logic do not require coverage.")]
    internal virtual (int xStart, int yStart) GetStart()
    {
        var (xCurrent, yCurrent) = GetCursorPosition();
        var (left, top, right, bottom) = GetCursorWorkArea(xCurrent, yCurrent);
        var xStart = GetXStart(xCurrent, left, right);
        var yStart = GetYStart(yCurrent, top, bottom);
        RelocateCursor(xStart, yStart);
        return (xStart, yStart);
    }

    internal virtual (int left, int top, int right, int bottom) GetCursorWorkArea(
        int xPosition,
        int yPosition)
    {
        if (!_externalAgent.TryGetCursorWorkArea(
                xPosition,
                yPosition,
                out var workArea))
            throw new ExternalException(
                _externalAgent,
                "Unable to get the cursor work area.");
        return (workArea.Left, workArea.Top, workArea.Right, workArea.Bottom);
    }

    internal virtual int GetXStart(
        int xCurrent,
        int left,
        int right)
    {
        if (left > xCurrent - _diameter)
            return left + _diameter;
        if (right < xCurrent + _diameter)
            return right - _diameter;
        return xCurrent;
    }

    internal virtual int GetYStart(
        int yCurrent,
        int top,
        int bottom)
    {
        if (top > yCurrent - _diameter)
            return top + _diameter;
        if (bottom < yCurrent + _diameter)
            return bottom + _diameter;
        return yCurrent;
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
        var stopWatch = _commonAdapter.StartStopwatch();
        if (!TryDrawLeftCircle(ref xStart, ref yStart))
            return false;
        stopWatch.Stop();
        Console.WriteLine(stopWatch.Elapsed.TotalSeconds);
        stopWatch = _commonAdapter.StartStopwatch();
        if (!TryDrawRightCircle(ref xStart, ref yStart))
            return false;
        stopWatch.Stop();
        Console.WriteLine(stopWatch.Elapsed.TotalSeconds);
        return true;
    }

    internal virtual bool TryDrawLeftCircle(
        ref int xStart,
        ref int yStart)
    {
        const double start = 0;
        const double degreeEnd = start + _circleDegrees;
        var result = TryDrawArc(
            ref xStart,
            ref yStart,
            start,
            degreeEnd,
            true);
        return result;
    }

    internal virtual bool TryDrawRightCircle(
        ref int xStart,
        ref int yStart)
    {
        const double degreeStart = 180;
        const double end = degreeStart - _circleDegrees;
        return TryDrawArc(
            ref xStart,
            ref yStart,
            degreeStart,
            end,
            false);
    }
    
    internal virtual bool TryDrawArc(
        ref int xStart,
        ref int yStart,
        double degreeStart,
        double degreeEnd,
        bool clockwise)
    {
        bool condition(double x, double y) => clockwise ? x < y : x > y;
        var increment = clockwise ? _degreeIncrement : -_degreeIncrement;
        var centerOffset = clockwise ? -_radius : _radius;
        for (var i = degreeStart; condition(i, degreeEnd); i += increment)
            if (!TryMoveNext(ref xStart, ref yStart, centerOffset, i))
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
        var stopwatch = _commonAdapter.StartStopwatch();
        var (xCenter, yCenter) = (xStart + centerOffset, yStart);
        var (xCurrent, yCurrent) = GetCursorPosition();
        var (xNext, yNext) = GetNext(xCenter, yCenter, degree);
        var xMove = _commonAgent.RoundDownToInt(xNext - xCurrent);
        var yMove = _commonAgent.RoundDownToInt(yNext - yCurrent);
        if (_commonAgent.AbsoluteSum(xMove, yMove) < 1)
            return true;
        if (TryMove(xMove, yMove, out var xNew, out var yNew, stopwatch))
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
    ///     True if the cursor traveled while waiting.
    /// </returns>
    /// <exception cref="ExternalException">
    ///     Thrown if unable to move the cursor.
    /// </exception>
    internal virtual bool TryMove(
        int xMove,
        int yMove,
        out int xNew,
        out int yNew,
        Stopwatch stopwatch)
    {
        if (!_externalAgent.TryMoveCursor(xMove, yMove))
            throw new ExternalException(
                _externalAgent,
                "Unable to move the cursor.");
        var (xCurrent, yCurrent) = GetCursorPosition();
        _commonAgent.Wait(_postMoveTicks);
        (xNew, yNew) = GetCursorPosition();
        stopwatch.Stop();
        _postMoveTicks += _targetTicksPerMove - (int)stopwatch.ElapsedTicks;
        return !DidTravel(xCurrent, yCurrent, xNew, yNew);
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
        const int activityWaitSeconds = 5;
        const int activityChecks = 6;
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
    [ExcludeFromCodeCoverage(Justification = "Methods without logic do not require coverage.")]
    public virtual Task StopAsync(
        CancellationToken cancellationToken)
    {
        IsRunning = false;
        return Task.CompletedTask;
    }
}