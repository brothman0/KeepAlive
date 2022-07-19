using KeepAlive.External;
using Moq;

namespace KeepAlive.Tests;

[TestClass]
public class ExternalExceptionTests
{
    private static Mock<IExternalAgent> _externalAgentMock = null!;

    [ClassInitialize]
    public static void ClassInitialize(
        TestContext context)
    {
        _externalAgentMock = new Mock<IExternalAgent>();
    }

    [TestInitialize]
    public void TestInitialize()
    {
        _externalAgentMock.Reset();
    }

    [TestMethod]
    public void Constructor_WhenGetErrorMessageReturnIsNotNull_ShouldUseReturnedMessage()
    {
        const string expectedMessage = "someMessage";
        _externalAgentMock.Setup(x => x.GetErrorMessage())
            .Returns(expectedMessage);
        
        var exception = new ExternalException(
            _externalAgentMock.Object,
            "someOtherMessage");

        Assert.AreEqual(expectedMessage, exception.Message);
    }

    [TestMethod]
    public void Constructor_WhenGetErrorMessageReturnsNull_ShouldUseErrorMessage()
    {
        const string expectedMessage = "someOtherMessage";
        _externalAgentMock.Setup(x => x.GetErrorMessage())
            .Returns(null as string);
        
        var exception = new ExternalException(
            _externalAgentMock.Object,
            expectedMessage);

        Assert.AreEqual(expectedMessage, exception.Message);
    }
}