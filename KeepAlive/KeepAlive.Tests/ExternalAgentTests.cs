using System.Text;
using KeepAlive.External;
using KeepAlive.External.Resources.TryGetCursorPosition;
using Moq;

namespace KeepAlive.Tests;

[TestClass]
public class ExternalAgentTests
{
    private static Mock<IExternalAdapter> _adapterMock = null!;

    [ClassInitialize]
    public static void ClassInitialize(
        TestContext context)
    {
        _adapterMock = new Mock<IExternalAdapter>();
    }

    [TestInitialize]
    public void TestInitialize()
    {
        _adapterMock.Reset();
    }
    
    [TestMethod]
    public void GetErrorMessage_WhenFormatMessageReturnIsNotZero_ShouldReturnBufferString()
    {
        const string bufferString = "someMessage";
        _adapterMock.Setup(x => x.GetLastWin32Error()).Returns(1);
        var targetMock = GetTarget();
        targetMock.Setup(x => x.FormatMessage(
                It.IsAny<int>(),
                It.IsAny<StringBuilder>()))
            .Returns((uint)bufferString.Length)
            .Callback(
                (int x, StringBuilder y) =>
                {
                    y.Append(bufferString);
                });
        targetMock.Setup(x => x.GetErrorMessage())
            .CallBase();
        var target = targetMock.Object;

        var returnValue = target.GetErrorMessage();

        Assert.AreEqual(bufferString, returnValue);
    }

    [TestMethod]
    public void GetErrorMessage_WhenFormatMessageReturnsZero_ShouldReturnNull()
    {
        _adapterMock.Setup(x => x.GetLastWin32Error()).Returns(1);
        var targetMock = GetTarget();
        targetMock.Setup(x => x.FormatMessage(
                It.IsAny<int>(),
                It.IsAny<StringBuilder>()))
            .Returns(0);
        targetMock.Setup(x => x.GetErrorMessage())
            .CallBase();
        var target = targetMock.Object;

        var returnValue = target.GetErrorMessage();

        Assert.IsNull(returnValue);
    }

    [TestMethod]
    public void TryGetCursorPosition_WhenAbleToGetCursorPosition_ShouldReturnTrueAndOutPosition()
    {
        var (xPosition, yPosition) = (null as int?, null as int?);
        var (xExpectedPosition, yExpectedPosition) = (20 as int?, 20 as int?);
        var position = new Position();
        var callback = (out Position x) =>
        {
            x.X = xExpectedPosition.Value;
            x.Y = yExpectedPosition.Value;
        };
        _adapterMock.Setup(x => x.TryGetCursorPosition(
                out position))
            .Returns(true)
            .Callback(callback);
        var targetMock = GetTarget();
        targetMock.Setup(x => x.TryGetCursorPosition(
                out xPosition,
                out xPosition))
            .CallBase();
        var target = targetMock.Object;

        var returnValue = target.TryGetCursorPosition(out xPosition, out yPosition);

        Assert.IsTrue(returnValue);
        Assert.AreEqual(xExpectedPosition, xPosition);
        Assert.AreEqual(yExpectedPosition, yPosition);
    }

    [TestMethod]
    public void TryGetCursorPosition_WhenNotAbleToGetCursorPosition_ShouldReturnFalseAndOutNull()
    {
        var (xPosition, yPosition) = (null as int?, null as int?);
        var position = new Position();
        _adapterMock.Setup(x => x.TryGetCursorPosition(
                out position))
            .Returns(false);
        var targetMock = GetTarget();
        targetMock.Setup(x => x.TryGetCursorPosition(
                out xPosition,
                out xPosition))
            .CallBase();
        var target = targetMock.Object;

        var returnValue = target.TryGetCursorPosition(out xPosition, out yPosition);

        Assert.IsFalse(returnValue);
        Assert.IsNull(xPosition);
        Assert.IsNull(yPosition);
    }

    [TestMethod]
    public void TryGetCursorWorkArea_WhenAbleToGetWorkArea_ShouldReturnTrueAndOutWorkArea()
    {

    }

    [TestMethod]
    public void TryGetCursorWorkArea_WhenNotAbleToGetMonitorInfo_ShouldReturnFalseAndOutNewRectangle()
    {

    }

    [TestMethod]
    public void TryMoveCursor_WhenInputIsProcessed_ShouldReturnTrue()
    {

    }

    [TestMethod]
    public void TryMoveCursor_WhenInputIsNotProcessed_ShouldReturnFalse()
    {

    }

    [TestMethod]
    public void TryRelocateCursor_WhenInputIsProcessed_ShouldReturnTrue()
    {

    }

    [TestMethod]
    public void TryRelocateCursor_WhenUnableToGetAbsolutePosiiton_ShouldReturnFalse()
    {

    }

    [TestMethod]
    public void TryRelocateCursor_WhenInputIsNotProcessed_ShouldReturnFalse()
    {

    }

    [TestMethod]
    public void TryGetAbsolutePosition_WhenAbleToGetScreenWidthAndHeight_ShouldReturnTrueAndUpdatePositions()
    {

    }

    [TestMethod]
    public void TryGetAbsolutePosition_WhenNotAbleToGetScreenWidthAndHeight_ShouldReturnFalseAndNotUpdatePositions()
    {

    }

    [TestMethod]
    public void TryGetScreenWidth_WhenScrenWidthIsNotZero_ShouldReturnTrue()
    {

    }

    [TestMethod]
    public void TryGetScreenWidth_WhenScreenWidthIsZero_ShouldReturnFalse()
    {

    }

    [TestMethod]
    public void TryGetScreenHeight_WhenScreenHeightIsNotZero_ShouldReturnTrue()
    {

    }

    [TestMethod]
    public void TryGetScreenHeight_WhenScreenHeightIsZero_ShouldReturnFalse()
    {

    }

    internal Mock<ExternalAgent> GetTarget()
    {
        return new Mock<ExternalAgent>(
            _adapterMock.Object);
    }
}