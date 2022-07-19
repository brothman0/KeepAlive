using System.Text;
using System.Xml.XPath;
using KeepAlive.External;
using KeepAlive.External.Resources.GetSystemMetric;
using KeepAlive.External.Resources.SendInputs;
using KeepAlive.External.Resources.TryGetCursorPosition;
using KeepAlive.External.Resources.TryGetMonitorInfo;
using Moq;
using static KeepAlive.External.ExternalAgent;

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
        _adapterMock.Setup(x => x.TryGetCursorPosition(
                out position))
            .Returns(true)
            .Callback((out Position x) =>
            {
                x.X = xExpectedPosition.Value;
                x.Y = yExpectedPosition.Value;
            });
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
        const int left = 1;
        const int top = 1;
        const int right = 2560;
        const int bottom = 1440;
        _adapterMock.Setup(x => x.SizeOf<It.IsAnyType>())
            .Returns(5);
        _adapterMock.Setup(x => x.TryGetMonitorInfo(
                It.IsAny<IntPtr>(),
                ref It.Ref<MonitorInfo>.IsAny))
            .Returns(true)
            .Callback((IntPtr x, ref MonitorInfo y) =>
            {
                y.WorkArea = new Rectangle
                {
                    Left = left,
                    Top = top,
                    Right = right,
                    Bottom = bottom
                };
            });
        var targetMock = GetTarget();
        targetMock.Setup(x => x.GetMonitorHandleFromPosition(
                It.IsAny<int>(),
                It.IsAny<int>()))
            .Returns(IntPtr.Zero);
        var workArea = new Rectangle();
        targetMock.Setup(x => x.TryGetCursorWorkArea(
                It.IsAny<int>(),
                It.IsAny<int>(),
                out workArea))
            .CallBase();
        var target = targetMock.Object;

        var returnValue = target.TryGetCursorWorkArea(1, 1, out workArea);

        Assert.IsTrue(returnValue);
        Assert.AreEqual(left, workArea.Left);
        Assert.AreEqual(top, workArea.Top);
        Assert.AreEqual(right, workArea.Right);
        Assert.AreEqual(bottom, workArea.Bottom);
    }

    [TestMethod]
    public void TryGetCursorWorkArea_WhenNotAbleToGetMonitorInfo_ShouldReturnFalseAndOutWorkArea()
    {
        const int zero = 0;
        _adapterMock.Setup(x => x.SizeOf<It.IsAnyType>())
            .Returns(5);
        _adapterMock.Setup(x => x.TryGetMonitorInfo(
                It.IsAny<IntPtr>(),
                ref It.Ref<MonitorInfo>.IsAny))
            .Returns(false);
        var targetMock = GetTarget();
        targetMock.Setup(x => x.GetMonitorHandleFromPosition(
                It.IsAny<int>(),
                It.IsAny<int>()))
            .Returns(IntPtr.Zero);
        var workArea = new Rectangle();
        targetMock.Setup(x => x.TryGetCursorWorkArea(
                It.IsAny<int>(),
                It.IsAny<int>(),
                out workArea))
            .CallBase();
        var target = targetMock.Object;

        var returnValue = target.TryGetCursorWorkArea(1, 1, out workArea);

        Assert.IsFalse(returnValue);
        Assert.AreEqual(zero, workArea.Left);
        Assert.AreEqual(zero, workArea.Top);
        Assert.AreEqual(zero, workArea.Right);
        Assert.AreEqual(zero, workArea.Bottom);
    }

    [TestMethod]
    public void TryMoveCursor_WhenInputIsProcessed_ShouldReturnTrue()
    {
        _adapterMock.Setup(x => x.GetMessageExtraInfo())
            .Returns(UIntPtr.Zero);
        var targetMock = GetTarget();
        const int expectedResponse = 1;
        targetMock.Setup(x => x.SendInput(
                It.IsAny<Input>()))
            .Returns(expectedResponse);
        targetMock.Setup(x => x.TryMoveCursor(
                It.IsAny<int>(),
                It.IsAny<int>()))
            .CallBase();
        var target = targetMock.Object;

        var returnValue = target.TryMoveCursor(1, 1);

        Assert.IsTrue(returnValue);
    }

    [TestMethod]
    public void TryMoveCursor_WhenInputIsNotProcessed_ShouldReturnFalse()
    {
        _adapterMock.Setup(x => x.GetMessageExtraInfo())
            .Returns(UIntPtr.Zero);
        var targetMock = GetTarget();
        const int expectedResponse = 0;
        targetMock.Setup(x => x.SendInput(
                It.IsAny<Input>()))
            .Returns(expectedResponse);
        targetMock.Setup(x => x.TryMoveCursor(
                It.IsAny<int>(),
                It.IsAny<int>()))
            .CallBase();
        var target = targetMock.Object;

        var returnValue = target.TryMoveCursor(1, 1);

        Assert.IsFalse(returnValue);
    }

    [TestMethod]
    public void TryRelocateCursor_WhenInputIsProcessed_ShouldReturnTrue()
    {
        _adapterMock.Setup(x => x.GetMessageExtraInfo())
            .Returns(UIntPtr.Zero);
        var targetMock = GetTarget();
        const int expectedResponse = 1;
        targetMock.Setup(x => x.TryGetAbsolutePosition(
                ref It.Ref<int>.IsAny,
                ref It.Ref<int>.IsAny))
            .Returns(true);
        targetMock.Setup(x => x.SendInput(
                It.IsAny<Input>()))
            .Returns(expectedResponse);
        targetMock.Setup(x => x.TryRelocateCursor(
                It.IsAny<int>(),
                It.IsAny<int>()))
            .CallBase();
        var target = targetMock.Object;

        var returnValue = target.TryRelocateCursor(1, 1);

        Assert.IsTrue(returnValue);
    }

    [TestMethod]
    public void TryRelocateCursor_WhenUnableToGetAbsolutePosiiton_ShouldReturnFalse()
    {
        _adapterMock.Setup(x => x.GetMessageExtraInfo())
            .Returns(UIntPtr.Zero);
        var targetMock = GetTarget();
        const int expectedResponse = 1;
        targetMock.Setup(x => x.TryGetAbsolutePosition(
                ref It.Ref<int>.IsAny,
                ref It.Ref<int>.IsAny))
            .Returns(false);
        targetMock.Setup(x => x.SendInput(
                It.IsAny<Input>()))
            .Returns(expectedResponse);
        targetMock.Setup(x => x.TryRelocateCursor(
                It.IsAny<int>(),
                It.IsAny<int>()))
            .CallBase();
        var target = targetMock.Object;

        var returnValue = target.TryRelocateCursor(1, 1);

        Assert.IsFalse(returnValue);
    }

    [TestMethod]
    public void TryRelocateCursor_WhenInputIsNotProcessed_ShouldReturnFalse()
    {
        _adapterMock.Setup(x => x.GetMessageExtraInfo())
            .Returns(UIntPtr.Zero);
        var targetMock = GetTarget();
        const int expectedResponse = 0;
        targetMock.Setup(x => x.TryGetAbsolutePosition(
                ref It.Ref<int>.IsAny,
                ref It.Ref<int>.IsAny))
            .Returns(true);
        targetMock.Setup(x => x.SendInput(
                It.IsAny<Input>()))
            .Returns(expectedResponse);
        targetMock.Setup(x => x.TryRelocateCursor(
                It.IsAny<int>(),
                It.IsAny<int>()))
            .CallBase();
        var target = targetMock.Object;

        var returnValue = target.TryRelocateCursor(1, 1);

        Assert.IsFalse(returnValue);
    }

    [TestMethod]
    public void TryGetAbsolutePosition_WhenAbleToGetScreenWidthAndHeight_ShouldReturnTrueAndUpdatePositions()
    {
        var targetMock = GetTarget();
        const int expectedScreenWidth = 2560;
        var screenWidth = 0;
        targetMock.Setup(x => x.TryGetScreenWidth(
                out screenWidth))
            .Returns(true)
            .Callback((out int x) =>
            {
                x = expectedScreenWidth;
            });
        const int expectedScreenHeight = 1440;
        var screenHeight = 0;
        targetMock.Setup(x => x.TryGetScreenHeight(
                out screenHeight))
            .Returns(true)
            .Callback((out int y) =>
            {
                y = expectedScreenHeight;
            });
        const int incomingXPosition = 128;
        const int incomingYPosition = 256;
        const int expectedXPosition = incomingXPosition * _absoluteReference / expectedScreenWidth;
        const int expectedYPosition = incomingYPosition * _absoluteReference / expectedScreenHeight;
        targetMock.Setup(x => x.TryGetAbsolutePosition(
                ref It.Ref<int>.IsAny,
                ref It.Ref<int>.IsAny))
            .CallBase();
        var target = targetMock.Object;
        var xPosition = 128;
        var yPosition = 256;

        var returnValue = target.TryGetAbsolutePosition(ref xPosition, ref yPosition);

        Assert.IsTrue(returnValue);
        Assert.AreEqual(expectedXPosition, xPosition);
        Assert.AreEqual(expectedYPosition, yPosition);
    }

    [TestMethod]
    public void TryGetAbsolutePosition_WhenNotAbleToGetScreenWidthOrHeight_ShouldReturnFalseAndNotUpdatePositions()
    {
        var targetMock = GetTarget();
        var screenWidth = 0;
        targetMock.Setup(x => x.TryGetScreenWidth(
                out screenWidth))
            .Returns(false);
        const int incomingXPosition = 128;
        const int incomingYPosition = 256;
        targetMock.Setup(x => x.TryGetAbsolutePosition(
                ref It.Ref<int>.IsAny,
                ref It.Ref<int>.IsAny))
            .CallBase();
        var target = targetMock.Object;
        var xPosition = 128;
        var yPosition = 256;

        var returnValue = target.TryGetAbsolutePosition(ref xPosition, ref yPosition);

        Assert.IsFalse(returnValue);
        Assert.AreEqual(incomingXPosition, xPosition);
        Assert.AreEqual(incomingYPosition, yPosition);
    }

    [TestMethod]
    public void TryGetScreenWidth_WhenScrenWidthIsNotZero_ShouldReturnTrueAndOutScreenWidth()
    {
        const int expectedScreenWidth = 2560;
        _adapterMock.Setup(x => x.GetSystemMetrics(
                It.IsAny<SystemSetting>()))
            .Returns(expectedScreenWidth);
        var targetMock = GetTarget();
        var screenWidth = 0;
        targetMock.Setup(x => x.TryGetScreenWidth(
                out screenWidth))
            .CallBase();
        var target = targetMock.Object;

        var returnValue = target.TryGetScreenWidth(out screenWidth);

        Assert.IsTrue(returnValue);
        Assert.AreEqual(expectedScreenWidth, screenWidth);
    }

    [TestMethod]
    public void TryGetScreenWidth_WhenScreenWidthIsZero_ShouldReturnFalseAndOutZero()
    {
        const int expectedScreenWidth = 0;
        _adapterMock.Setup(x => x.GetSystemMetrics(
                It.IsAny<SystemSetting>()))
            .Returns(expectedScreenWidth);
        var targetMock = GetTarget();
        var screenWidth = -1;
        targetMock.Setup(x => x.TryGetScreenWidth(
                out screenWidth))
            .CallBase();
        var target = targetMock.Object;

        var returnValue = target.TryGetScreenWidth(out screenWidth);

        Assert.IsFalse(returnValue);
        Assert.AreEqual(expectedScreenWidth, screenWidth);
    }

    [TestMethod]
    public void TryGetScreenHeight_WhenScreenHeightIsNotZero_ShouldReturnTrue()
    {
        const int expectedScreenHeight = 1440;
        _adapterMock.Setup(x => x.GetSystemMetrics(
                It.IsAny<SystemSetting>()))
            .Returns(expectedScreenHeight);
        var targetMock = GetTarget();
        var screenHeight = 0;
        targetMock.Setup(x => x.TryGetScreenHeight(
                out screenHeight))
            .CallBase();
        var target = targetMock.Object;

        var returnValue = target.TryGetScreenHeight(out screenHeight);

        Assert.IsTrue(returnValue);
        Assert.AreEqual(expectedScreenHeight, screenHeight);
    }

    [TestMethod]
    public void TryGetScreenHeight_WhenScreenHeightIsZero_ShouldReturnFalse()
    {
        const int expectedScreenHeight = 0;
        _adapterMock.Setup(x => x.GetSystemMetrics(
                It.IsAny<SystemSetting>()))
            .Returns(expectedScreenHeight);
        var targetMock = GetTarget();
        var screenHeight = -1;
        targetMock.Setup(x => x.TryGetScreenHeight(
                out screenHeight))
            .CallBase();
        var target = targetMock.Object;

        var returnValue = target.TryGetScreenHeight(out screenHeight);

        Assert.IsFalse(returnValue);
        Assert.AreEqual(expectedScreenHeight, screenHeight);
    }

    internal Mock<ExternalAgent> GetTarget()
    {
        return new Mock<ExternalAgent>(
            _adapterMock.Object);
    }
}