using KeepAlive.External;
using KeepAlive.External.Resources.TryGetMonitorInfo;
using Moq;
using static KeepAlive.KeepAliveService;

namespace KeepAlive.Tests;

[TestClass]
public class KeepAliveServiceTests
{
    private static Mock<ICommonAdapter> _commonAdapterMock = null!;
    private static Mock<ICommonAgent> _commonAgentMock = null!;
    private static Mock<IExternalAgent> _externalAgentMock = null!;

    [ClassInitialize]
    public static void ClassInitialize(
        TestContext context)
    {
        _commonAdapterMock = new Mock<ICommonAdapter>();
        _commonAgentMock = new Mock<ICommonAgent>();
        _externalAgentMock = new Mock<IExternalAgent>();
    }

    [TestInitialize]
    public void TestInitialize()
    {
        _commonAdapterMock.Reset();
        _commonAgentMock.Reset();
        _externalAgentMock.Reset();
    }
    
    [TestMethod]
    public void KeepAlive_WhenAbleToDrawFigureEight_ShouldNotCallWaitForInActivity()
    {
        var targetMock = GetTarget();
        targetMock.Setup(x => x.GetStart())
            .Returns((5, 5));
        targetMock.SetupProperty(x => x.IsRunning);
        targetMock.Setup(x => x.TryDrawFigureEight(
                ref It.Ref<int>.IsAny,
                ref It.Ref<int>.IsAny))
            .Returns(true)
            .Callback(() => targetMock.Object.IsRunning = false);
        targetMock.Setup(x => x.KeepAlive()).CallBase();
        var target = targetMock.Object;
        target.IsRunning = true;

        target.KeepAlive();

        targetMock.Verify(x => x.WaitForInActivity(
            ref It.Ref<int>.IsAny,
            ref It.Ref<int>.IsAny), Times.Never);
    }
    
    [TestMethod]
    public void KeepAlive_WhenUnableToDrawFigureEight_ShouldCallWaitForInActivity()
    {
        var targetMock = GetTarget();
        targetMock.Setup(x => x.GetStart())
            .Returns((5, 5));
        targetMock.SetupProperty(x => x.IsRunning);
        targetMock.Setup(x => x.TryDrawFigureEight(
                ref It.Ref<int>.IsAny,
                ref It.Ref<int>.IsAny))
            .Returns(false)
            .Callback(() => targetMock.Object.IsRunning = false);
        targetMock.Setup(x => x.KeepAlive()).CallBase();
        var target = targetMock.Object;
        target.IsRunning = true;

        target.KeepAlive();

        targetMock.Verify(x => x.WaitForInActivity(
            ref It.Ref<int>.IsAny,
            ref It.Ref<int>.IsAny), Times.Once);
    }

    [TestMethod]
    public void GetCursorWorkArea_WhenAbleToGetCursorWorkArea_ShouldReturnWorkArea()
    {
        const int left = 1;
        const int top = 1;
        const int right = 2560;
        const int bottom = 1440;
        var workArea = new Rectangle();
        _externalAgentMock.Setup(x => x.TryGetCursorWorkArea(
                It.IsAny<int>(),
                It.IsAny<int>(),
                out workArea))
            .Returns(true)
            .Callback((int x, int y, out Rectangle z) =>
            {
                z.Left = left;
                z.Top = top;
                z.Right = right;
                z.Bottom = bottom;
            });
        var targetMock = GetTarget();
        targetMock.Setup(x => x.GetCursorWorkArea(
                It.IsAny<int>(),
                It.IsAny<int>()))
            .CallBase();
        var target = targetMock.Object;

        var returnValue = target.GetCursorWorkArea(5, 5);

        Assert.AreEqual(left, returnValue.left);
        Assert.AreEqual(top, returnValue.top);
        Assert.AreEqual(right, returnValue.right);
        Assert.AreEqual(bottom, returnValue.bottom);
    }

    [TestMethod]
    [ExpectedException(typeof(ExternalException))]
    public void GetCursorWorkArea_WhenNotAbleToGetCursorWorkArea_ShouldThrowExternalException()
    {
        var workArea = new Rectangle();
        _externalAgentMock.Setup(x => x.TryGetCursorWorkArea(
                It.IsAny<int>(),
                It.IsAny<int>(),
                out workArea))
            .Returns(false);
        var targetMock = GetTarget();
        targetMock.Setup(x => x.GetCursorWorkArea(
                It.IsAny<int>(),
                It.IsAny<int>()))
            .CallBase();
        var target = targetMock.Object;

        _ = target.GetCursorWorkArea(5, 5);
    }

    [TestMethod]
    public void GetXStart_WhenLeftIsGreaterThanCurrentMinusDiameter_ShouldReturnLeftPlusDiameter()
    {
        var targetMock = GetTarget();
        targetMock.Setup(x => x.GetXStart(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .CallBase();
        var target = targetMock.Object;
        const int left = 0;
        const int right = 2560;
        var xCurrent = left + _diameter - 1;

        var returnValue = target.GetXStart(xCurrent, left, right);

        Assert.AreEqual(left + _diameter, returnValue);
    }

    [TestMethod]
    public void GetXStart_WhenRightIsLessThanCurrentPlusDiameter_ShouldReturnRightMinusDiameter()
    {
        var targetMock = GetTarget();
        targetMock.Setup(x => x.GetXStart(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .CallBase();
        var target = targetMock.Object;
        const int left = 0;
        const int right = 2560;
        var xCurrent = right + _diameter + 1;

        var returnValue = target.GetXStart(xCurrent, left, right);

        Assert.AreEqual(right - _diameter, returnValue);
    }

    [TestMethod]
    public void GetXStart_WhenCurrentIsBetweenLeftAndRight_ShouldReturnCurrent()
    {
        var targetMock = GetTarget();
        targetMock.Setup(x => x.GetXStart(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .CallBase();
        var target = targetMock.Object;
        const int left = 0;
        const int right = 2560;
        var xCurrent = left + _diameter;

        var returnValue = target.GetXStart(xCurrent, left, right);

        Assert.AreEqual(xCurrent, returnValue);
    }

    [TestMethod]
    public void GetYStart_WhenTopIsGreaterThanCurrentMinusDiameter_ShouldReturnTopPlusDiameter()
    {
        var targetMock = GetTarget();
        targetMock.Setup(x => x.GetYStart(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .CallBase();
        var target = targetMock.Object;
        const int top = 0;
        const int bottom = 1440;
        var yCurrent = top + _diameter - 1;

        var returnValue = target.GetYStart(yCurrent, top, bottom);

        Assert.AreEqual(top + _diameter, returnValue);
    }

    [TestMethod]
    public void GetYStart_WhenBottomIsLessThanCurrentPlusDiameter_ShouldReturnBottomMinusDiameter()
    {
        var targetMock = GetTarget();
        targetMock.Setup(x => x.GetYStart(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .CallBase();
        var target = targetMock.Object;
        const int top = 0;
        const int bottom = 1440;
        var yCurrent = bottom + _diameter + 1;

        var returnValue = target.GetYStart(yCurrent, top, bottom);

        Assert.AreEqual(bottom - _diameter, returnValue);
    }

    [TestMethod]
    public void GetYStart_WhenCurrentIsBetweenTopAndBottom_ShouldReturnCurrent()
    {
        var targetMock = GetTarget();
        targetMock.Setup(x => x.GetYStart(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .CallBase();
        var target = targetMock.Object;
        const int top = 0;
        const int bottom = 1140;
        var yCurrent = top + _diameter;

        var returnValue = target.GetYStart(yCurrent, top, bottom);

        Assert.AreEqual(yCurrent, returnValue);
    }

    [TestMethod]
    public void TryDrawArc_WhenClockwiseAndThereIsNoActivity_ShouldReturnTrueAndCallRelocateCursor()
    {
        var targetMock = GetTarget();
        targetMock.Setup(x => x.TryMoveNext(
                ref It.Ref<int>.IsAny,
                ref It.Ref<int>.IsAny,
                It.IsAny<int>(),
                It.IsAny<double>()))
            .Returns(true);
        targetMock.Setup(x => x.TryDrawArc(
                ref It.Ref<int>.IsAny,
                ref It.Ref<int>.IsAny,
                It.IsAny<double>(),
                It.IsAny<double>(),
                It.IsAny<bool>()))
            .CallBase();
        var target = targetMock.Object;
        var (xStart, yStart) = (5, 5);

        var returnValue = target.TryDrawArc(ref xStart, ref yStart, 0, 360, true);

        Assert.IsTrue(returnValue);
        targetMock.Verify(x => x.RelocateCursor(
            It.IsAny<int>(),
            It.IsAny<int>()), Times.Once);
    }

    [TestMethod]
    public void TryDrawArc_WhenNotClockwiseAndThereIsNoActivity_ShouldReturnTrueCallRelocateCursor()
    {
        var targetMock = GetTarget();
        targetMock.Setup(x => x.TryMoveNext(
                ref It.Ref<int>.IsAny,
                ref It.Ref<int>.IsAny,
                It.IsAny<int>(),
                It.IsAny<double>()))
            .Returns(true);
        targetMock.Setup(x => x.TryDrawArc(
                ref It.Ref<int>.IsAny,
                ref It.Ref<int>.IsAny,
                It.IsAny<double>(),
                It.IsAny<double>(),
                It.IsAny<bool>()))
            .CallBase();
        var target = targetMock.Object;
        var (xStart, yStart) = (5, 5);

        var returnValue = target.TryDrawArc(ref xStart, ref yStart, -180, 180, false);

        Assert.IsTrue(returnValue);
        targetMock.Verify(x => x.RelocateCursor(
            It.IsAny<int>(),
            It.IsAny<int>()), Times.Once);
    }

    [TestMethod]
    public void TryDrawArc_WhenThereIsActivity_ShouldReturnFalseAndNotCallRelocateCursor()
    {
        var targetMock = GetTarget();
        targetMock.Setup(x => x.TryMoveNext(
                ref It.Ref<int>.IsAny,
                ref It.Ref<int>.IsAny,
                It.IsAny<int>(),
                It.IsAny<double>()))
            .Returns(false);
        targetMock.Setup(x => x.TryDrawArc(
                ref It.Ref<int>.IsAny,
                ref It.Ref<int>.IsAny,
                It.IsAny<double>(),
                It.IsAny<double>(),
                It.IsAny<bool>()))
            .CallBase();
        var target = targetMock.Object;
        var (xStart, yStart) = (5, 5);

        var returnValue = target.TryDrawArc(ref xStart, ref yStart, 180, -180, false);

        Assert.IsFalse(returnValue);
        targetMock.Verify(x => x.RelocateCursor(
            It.IsAny<int>(),
            It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void TryMoveNext_WhenAbleToMove_ShouldReturnTrue()
    {
        _commonAgentMock.Setup(x => x.AbsoluteSum(
                It.IsAny<int[]>()))
            .Returns(1);
        var targetMock = GetTarget();
        var (xNew, yNew) = (0, 0);
        targetMock.Setup(x => x.TryMove(
                It.IsAny<int>(),
                It.IsAny<int>(),
                out xNew,
                out yNew,
                It.IsAny<Stopwatch>()))
            .Returns(true);
        targetMock.Setup(x => x.TryMoveNext(
                ref It.Ref<int>.IsAny,
                ref It.Ref<int>.IsAny,
                It.IsAny<int>(),
                It.IsAny<double>()))
            .CallBase();
        var target = targetMock.Object;
        const int expectedXStart = 128;
        const int expectedYStart = 256;
        var (xStart, yStart) = (expectedXStart, expectedYStart);

        var returnValue = target.TryMoveNext(ref xStart, ref yStart, 10, 5);

        Assert.IsTrue(returnValue);
        Assert.AreEqual(expectedXStart, xStart);
        Assert.AreEqual(expectedYStart, yStart);
    }

    [TestMethod]
    public void TryMoveNext_WhenDistanceToMoveIsLessThanAPixel_ShouldReturnTrue()
    {
        _commonAgentMock.Setup(x => x.AbsoluteSum(
                It.IsAny<int[]>()))
            .Returns(0);
        var targetMock = GetTarget();
        var (xNew, yNew) = (0, 0);
        targetMock.Setup(x => x.TryMove(
                It.IsAny<int>(),
                It.IsAny<int>(),
                out xNew,
                out yNew,
                It.IsAny<IStopwatch>()))
            .Returns(true);
        targetMock.Setup(x => x.TryMoveNext(
                ref It.Ref<int>.IsAny,
                ref It.Ref<int>.IsAny,
                It.IsAny<int>(),
                It.IsAny<double>()))
            .CallBase();
        var target = targetMock.Object;
        const int expectedXStart = 128;
        const int expectedYStart = 256;
        var (xStart, yStart) = (expectedXStart, expectedYStart);

        var returnValue = target.TryMoveNext(ref xStart, ref yStart, 10, 5);

        Assert.IsTrue(returnValue);
        Assert.AreEqual(expectedXStart, xStart);
        Assert.AreEqual(expectedYStart, yStart);
    }

    [TestMethod]
    public void TryMoveNext_WhenTryMoveReturnsFalse_ShouldReturnFalseAndUpdateStart()
    {
        _commonAgentMock.Setup(x => x.AbsoluteSum(
                It.IsAny<int[]>()))
            .Returns(1);
        var targetMock = GetTarget();
        var (xNew, yNew) = (0, 0);
        const int expectedXStart = 128;
        const int expectedYStart = 256;
        targetMock.Setup(x => x.TryMove(
                It.IsAny<int>(),
                It.IsAny<int>(),
                out xNew,
                out yNew,
                It.IsAny<IStopwatch>()))
            .Returns(false)
            .Callback((int a, int b, out int c, out int d, IStopwatch e) =>
            {
                c = expectedXStart;
                d = expectedYStart;
            });
        targetMock.Setup(x => x.TryMoveNext(
                ref It.Ref<int>.IsAny,
                ref It.Ref<int>.IsAny,
                It.IsAny<int>(),
                It.IsAny<double>()))
            .CallBase();
        var target = targetMock.Object;
        var (xStart, yStart) = (0, 0);

        var returnValue = target.TryMoveNext(ref xStart, ref yStart, 10, 5);

        Assert.IsFalse(returnValue);
        Assert.AreEqual(expectedXStart, xStart);
        Assert.AreEqual(expectedYStart, yStart);
    }

    [TestMethod]
    public void TryMove_WhenAbleToMoveAndThereIsNoActivity_ShouldReturnTrueAndUpdatePostMoveTicks()
    {
        var stopwatchMock = new Mock<IStopwatch>();
        stopwatchMock.SetupGet(x => x.ElapsedTicks)
            .Returns(54321);
        var stopwatch = stopwatchMock.Object;
        _externalAgentMock.Setup(x => x.TryMoveCursor(
                It.IsAny<int>(),
                It.IsAny<int>()))
            .Returns(true);
        var targetMock = GetTarget();
        //targetMock.SetupGet(x => x._postMoveTicks)
        //    .CallBase();
        targetMock.Setup(x => x.DidTravel(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .Returns(false);
        var (xNew, yNew) = (0, 0);
        targetMock.Setup(x => x.TryMove(
                It.IsAny<int>(),
                It.IsAny<int>(),
                out xNew,
                out yNew,
                It.IsAny<IStopwatch>()))
            .CallBase();
        var target = targetMock.Object;
        var expectedPostMoveTicks = target._postMoveTicks + (_targetTicksPerMove - (int)stopwatch.ElapsedTicks);

        var returnValue = target.TryMove(1, 1, out xNew, out yNew, stopwatch);

        Assert.IsTrue(returnValue);
        Assert.AreEqual(expectedPostMoveTicks, target._postMoveTicks);
    }

    [TestMethod]
    public void TryMove_WhenAbleToMoveAndThereIsActivity_ShouldReturnFalseAndUpdatePostMoveTicks()
    {
        var stopwatchMock = new Mock<IStopwatch>();
        stopwatchMock.SetupGet(x => x.ElapsedTicks)
            .Returns(54321);
        var stopwatch = stopwatchMock.Object;
        _externalAgentMock.Setup(x => x.TryMoveCursor(
                It.IsAny<int>(),
                It.IsAny<int>()))
            .Returns(true);
        var targetMock = GetTarget();
        targetMock.Setup(x => x.DidTravel(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .Returns(true);
        var (xNew, yNew) = (0, 0);
        targetMock.Setup(x => x.TryMove(
                It.IsAny<int>(),
                It.IsAny<int>(),
                out xNew,
                out yNew,
                It.IsAny<IStopwatch>()))
            .CallBase();
        var target = targetMock.Object;
        var expectedPostMoveTicks = target._postMoveTicks + (_targetTicksPerMove - (int)stopwatch.ElapsedTicks);

        var returnValue = target.TryMove(1, 1, out xNew, out yNew, stopwatch);

        Assert.IsFalse(returnValue);
        Assert.AreEqual(expectedPostMoveTicks, target._postMoveTicks);
    }

    [TestMethod]
    [ExpectedException(typeof(ExternalException))]
    public void TryMove_WhenUnableToMove_ShouldThrowExternalException()
    {
        var stopwatchMock = new Mock<IStopwatch>();
        stopwatchMock.SetupGet(x => x.ElapsedTicks)
            .Returns(54321);
        var stopwatch = stopwatchMock.Object;
        _externalAgentMock.Setup(x => x.TryMoveCursor(
                It.IsAny<int>(),
                It.IsAny<int>()))
            .Returns(false);
        var targetMock = GetTarget();
        var (xNew, yNew) = (0, 0);
        targetMock.Setup(x => x.TryMove(
                It.IsAny<int>(),
                It.IsAny<int>(),
                out xNew,
                out yNew,
                It.IsAny<IStopwatch>()))
            .CallBase();
        var target = targetMock.Object;

        _ = target.TryMove(1, 1, out xNew, out yNew, stopwatch);
    }

    [TestMethod]
    public void RelocateCursor_WhenAbleToRelocateCursor_ShouldNotThrowExternalException()
    {
        _externalAgentMock.Setup(x => x.TryRelocateCursor(
                It.IsAny<int>(),
                It.IsAny<int>()))
            .Returns(true);
        var targetMock = GetTarget();
        targetMock.Setup(x => x.RelocateCursor(
                It.IsAny<int>(),
                It.IsAny<int>()))
            .CallBase();
        var target = targetMock.Object;

        target.RelocateCursor(5, 5);
    }

    [TestMethod]
    [ExpectedException(typeof(ExternalException))]
    public void RelocateCursor_WhenNotAbleToRelocateCursor_ShouldThrowExternalException()
    {
        _externalAgentMock.Setup(x => x.TryRelocateCursor(
                It.IsAny<int>(),
                It.IsAny<int>()))
            .Returns(false);
        var targetMock = GetTarget();
        targetMock.Setup(x => x.RelocateCursor(
                It.IsAny<int>(),
                It.IsAny<int>()))
            .CallBase();
        var target = targetMock.Object;

        target.RelocateCursor(5, 5);
    }

    [TestMethod]
    public void WaitForInActivity_WhenNoActivityIsDetected_ShouldCallGetCursorPositionExpectedTimes()
    {
        var targetMock = GetTarget();
        targetMock.Setup(x => x.DidTravel(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .Returns(false);
        targetMock.Setup(x => x.WaitForInActivity(
                ref It.Ref<int>.IsAny,
                ref It.Ref<int>.IsAny))
            .CallBase();
        var target = targetMock.Object;
        var (xStart, yStart) = (0, 0);

        target.WaitForInActivity(ref xStart, ref yStart);

        targetMock.Verify(x => x.GetCursorPosition(), Times.Exactly(_activityChecks));
    }

    [TestMethod]
    public void WaitForInActivity_WhenActivityIsDetected_ShouldCallGetCursorPositionDoubleTheExpectedTimes()
    {
        var targetMock = GetTarget();
        var didTravelCalls = 0;
        targetMock.Setup(x => x.DidTravel(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .Returns((int a, int b, int c, int d) =>
            {
                didTravelCalls++;
                if (didTravelCalls == _activityChecks)
                    return true;
                return false;
            });
        targetMock.Setup(x => x.WaitForInActivity(
                ref It.Ref<int>.IsAny,
                ref It.Ref<int>.IsAny))
            .CallBase();
        var target = targetMock.Object;
        var (xStart, yStart) = (0, 0);

        target.WaitForInActivity(ref xStart, ref yStart);

        targetMock.Verify(x => x.GetCursorPosition(), Times.Exactly(_activityChecks * 2));
    }

    [TestMethod]
    public void GetCursorPosition_WhenAbleToGetCursorPosition_ShouldReturnCursorPosition()
    {
        const int expectedXPosition = 128;
        const int expectedYPosition = 256;
        var (xPosition, yPosition) = (0 as int?, 0 as int?);
        _externalAgentMock.Setup(x => x.TryGetCursorPosition(
                out xPosition,
                out yPosition))
            .Returns(true)
            .Callback((out int? x, out int? y) =>
            {
                x = expectedXPosition;
                y = expectedYPosition;
            });
        var targetMock = GetTarget();
        targetMock.Setup(x => x.GetCursorPosition())
            .CallBase();
        var target = targetMock.Object;

         (xPosition, yPosition) = target.GetCursorPosition();

        Assert.AreEqual(expectedXPosition, xPosition);
        Assert.AreEqual(expectedYPosition, yPosition);
    }

    [TestMethod]
    [ExpectedException(typeof(ExternalException))]
    public void GetCursorPosition_WhenAbleToGetCursorPosition_ShouldThrowExternalException()
    {
        var (xPosition, yPosition) = (0 as int?, 0 as int?);
        _externalAgentMock.Setup(x => x.TryGetCursorPosition(
                out xPosition,
                out yPosition))
            .Returns(false);
        var targetMock = GetTarget();
        targetMock.Setup(x => x.GetCursorPosition())
            .CallBase();
        var target = targetMock.Object;

        _ = target.GetCursorPosition();
    }


    internal Mock<KeepAliveService> GetTarget()
    {
        return new Mock<KeepAliveService>(
            _commonAdapterMock.Object,
            _commonAgentMock.Object,
            _externalAgentMock.Object);
    }
}