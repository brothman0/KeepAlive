using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using KeepAlive.External;
using Microsoft.Extensions.Hosting;
using ExternalException = KeepAlive.External.ExternalException;

namespace KeepAlive;

/// <summary>
///     Keeps the host machine alive.
/// </summary>
public class KeepAliveService : IHostedService
{
    internal readonly ICommonAdapter _commonAdapter;
    internal readonly ICommonAgent _commonAgent;
    internal readonly IExternalAgent _externalAgent;
    internal const int _radius = 100;
    internal const int _diameter = _radius * 2;
    internal const double _circleDegrees = 360;
    internal const double _degreeIncrement = 0.5;
    internal const double _movesPerCircle = _circleDegrees / _degreeIncrement;
    internal const int _targetTicksPerCircle = 20000000;
    internal const int _targetTicksPerMove = _targetTicksPerCircle / (int)_movesPerCircle;
    internal const int _activityWaitSeconds = 5;
    internal const int _activityChecks = 6;
    internal int _postMoveTicks = 25000;

    [ExcludeFromCodeCoverage(Justification = "Auto properties do not require coverage.")]
    public virtual bool IsRunning { get; set; } = true;

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

    //TODO: Integrate a means of using TryGetIdleTime to determine idle time
    //TODO: After every period of inactivity that the figure eight starts it should be determined if the mouse should be moved.
    //TODO: use y = +/- 1/4 sqrt(16 x^2 - x^4)  or x^4 = 16 (x^2 - y^2) to generate the figure eight
    //TODO: calculate, based on the figure eight size, how many moves are actually required and adjust the wait time between moves accordingly
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

    /// <summary>
    ///     Gets the work area for the monitor the cursor is in.
    /// </summary>
    /// <param name="xPosition">
    ///     The coordinate of the cursor along the x-axis.
    /// </param>
    /// <param name="yPosition">
    ///     The coordinate of the cursor along the y-axis.
    /// </param>
    /// <returns>
    ///     The work area for the monitor the cursor is in.
    /// </returns>
    /// <exception cref="External.ExternalException">
    ///     Thrown if unable to get the cursor work area.
    /// </exception>
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

    /// <summary>
    ///     Gets the starting position of the cursor along the x-axis such that
    ///     the cursor will not intersect the bounds of the work area when drawing
    ///     the figure eight.
    /// </summary>
    /// <param name="xCurrent">
    ///     The current position of the cursor along the x-axis.
    /// </param>
    /// <param name="left">
    ///     The coordinate along the x-axis that describes the left bound of the
    ///     work area.
    /// </param>
    /// <param name="right">
    ///     The coordinate along the x-axis that describes the right bound of the
    ///     work area.
    /// </param>
    /// <returns>
    ///     The starting position along the x-axis for the figure eight.
    /// </returns>
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

    /// <summary>
    ///     Gets the starting position of the cursor along the y-axis such that
    ///     the cursor will not intersect the bounds of the work area when drawing
    ///     the figure eight.
    /// </summary>
    /// <param name="yCurrent">
    ///     The current position of the cursor along the y-axis.
    /// </param>
    /// <param name="top">
    ///     The coordinate along the y-axis that describes the top bound of the
    ///     work area.
    /// </param>
    /// <param name="bottom">
    ///     The coordinate along the y-axis that describes the bottom bound of the
    ///     work area.
    /// </param>
    /// <returns>
    ///     The starting position along the y-axis for the figure eight.
    /// </returns>
    internal virtual int GetYStart(
        int yCurrent,
        int top,
        int bottom)
    {
        if (top > yCurrent - _diameter)
            return top + _diameter;
        if (bottom < yCurrent + _diameter)
            return bottom - _diameter;
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
    [ExcludeFromCodeCoverage(Justification = "Methods without logic do not require coverage.")]
    internal virtual bool TryDrawFigureEight(
        ref int xStart,
        ref int yStart)
    {
        const double leftDegreeStart = 0;
        const double leftDegreeEnd = leftDegreeStart + _circleDegrees;
        const double rightDegreeStart = 180;
        const double rightDegreeEnd = rightDegreeStart - _circleDegrees;
        return TryDrawArc(ref xStart, ref yStart, leftDegreeStart, leftDegreeEnd, true) &&
               TryDrawArc(ref xStart, ref yStart, rightDegreeStart, rightDegreeEnd, false);

    }
    
    /// <summary>
    ///     Attempts to draw an arc with the cursor.
    /// </summary>
    /// <param name="degreeStart">
    ///     The degree of the arc to start at.
    /// </param>
    /// <param name="degreeEnd">
    ///     The degree of the arc to end at.
    /// </param>
    /// <param name="clockwise">
    ///     Indicates if the arc should be drawn clockwise.
    /// </param>
    /// <returns>
    ///     True if able to draw an arc with the cursor.
    /// </returns>
    /// <inheritdoc cref="TryDrawFigureEight"/>
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
    [ExcludeFromCodeCoverage(Justification = "Methods without logic do not require coverage.")]
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
    /// <param name="stopwatch">
    ///     Stopwatch used to determine the time to wait between
    ///     moves.
    /// </param>
    /// <returns>
    ///     True if the cursor did not travel while waiting.
    /// </returns>
    /// <exception cref="ExternalException">
    ///     Thrown if unable to move the cursor.
    /// </exception>
    internal virtual bool TryMove(
        int xMove,
        int yMove,
        out int xNew,
        out int yNew,
        IStopwatch stopwatch)
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
        var activityWait = new TimeSpan(0, 0, _activityWaitSeconds);
        for (var checks = 0; checks < _activityChecks; checks++)
        {
            _commonAgent.Wait(activityWait);
            var (xCurrent, yCurrent) = GetCursorPosition();
            if (DidTravel(xStart, yStart, xCurrent, yCurrent))
                checks = -1;
            (xStart, yStart) = (xCurrent, yCurrent);
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
    [ExcludeFromCodeCoverage(Justification = "Methods without logic do not require coverage.")]
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
    internal virtual (int xPosition, int yPosition) GetCursorPosition()
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