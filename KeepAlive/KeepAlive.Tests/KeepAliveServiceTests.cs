using System.Reflection;
using KeepAlive.External;
using Moq;

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

    }
    
    [TestMethod]
    public void KeepAlive_WhenUnableToDrawFigureEight_ShouldCallWaitForInActivity()
    {

    }

    [TestMethod]
    public void GetCursorWorkArea_WhenAbleToGetCursorWorkArea_ShouldReturnWorkArea()
    {

    }

    [TestMethod]
    [ExpectedException(typeof(ExternalException))]
    public void GetCursorWorkArea_WhenNotAbleToGetCursorWorkArea_ShouldThrowExternalException()
    {

    }

    [TestMethod]
    public void GetXStart_WhenLeftIsGreaterThanCurrent_ShouldReturnLeftPlusDiameter()
    {
        
    }

    [TestMethod]
    public void GetXStart_WhenRightIsLessThanCurrent_ShouldReturnRightMinusDiameter()
    {
        
    }

    [TestMethod]
    public void GetXStart_WhenCurrentIsBetweenLeftAndRight_ShouldReturnCurrent()
    {
        
    }

    [TestMethod]
    public void GetYStart_WhenTopIsGreaterThanCurrent_ShouldReturnTopPlusDiameter()
    {
        
    }

    [TestMethod]
    public void GetYStart_WhenBottomIsLessThanCurrent_ShouldReturnBottomMinusDiameter()
    {
        
    }

    [TestMethod]
    public void GetYStart_WhenCurrentIsBetweenTopAndBottom_ShouldReturnCurrent()
    {
        
    }

    [TestMethod]
    public void TryDrawArc_WhenClockwiseAndThereIsNoActivity_ShouldReturnTrueAndLoopClockwise()
    {

    }

    [TestMethod]
    public void TryDrawArc_WhenNotClockwiseAndThereIsNoActivity_ShouldReturnTrueAndLoopCounterClockwise()
    {

    }

    [TestMethod]
    public void TryDrawArc_WhenThereIsActivity_ShouldReturnFalse()
    {

    }

    [TestMethod]
    public void TryMoveNext_WhenAbleToMove_ShouldReturnTrue()
    {

    }

    [TestMethod]
    public void TryMoveNext_WhenDistanceToMoveIsLessThanAPixel_ShouldReturnTrue()
    {

    }

    [TestMethod]
    public void TryMoveNext_WhenTryMoveReturnsFalse_ShouldReturnFalseAndUpdateStart()
    {

    }

    [TestMethod]
    public void TryMove_WhenAbleToMoveAndThereIsNoActivity_ShouldReturnTrue()
    {

    }

    [TestMethod]
    public void TryMove_WhenAbleToMoveAndThereIsActivity_ShouldReturnFalse()
    {

    }

    [TestMethod]
    [ExpectedException(typeof(ExternalException))]
    public void TryMove_WhenUnableToMove_ShouldThrowExternalException()
    {

    }

    [TestMethod]
    public void RelocateCursor_WhenAbleToRelocateCursor_ShouldDoNothing()
    {

    }

    [TestMethod]
    [ExpectedException(typeof(ExternalException))]
    public void RelocateCursor_WhenNotAbleToRelocateCursor_ShouldThrowExternalException()
    {

    }

    [TestMethod]
    public void WaitForInActivity_WhenNoActivityIsDetected_ShouldCallGetCursorPositionExpectedTimes()
    {

    }

    [TestMethod]
    public void WaitForInActivity_WhenNoActivityIsDetected_ShouldCallGetCursorPositionMoreThanExpectedTimes()
    {

    }

    [TestMethod]
    public void GetCursorPosition_WhenAbleToGetCursorPosition_ShouldReturnCursorPosition()
    {

    }

    [TestMethod]
    [ExpectedException(typeof(ExternalException))]
    public void GetCursorPosition_WhenAbleToGetCursorPosition_ShouldThrowExternalException()
    {

    }


    internal Mock<KeepAliveService> GetTarget()
    {
        return new Mock<KeepAliveService>(
            _commonAdapterMock.Object,
            _commonAgentMock.Object,
            _externalAgentMock.Object);
    }
}