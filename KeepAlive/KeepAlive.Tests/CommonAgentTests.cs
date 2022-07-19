using Moq;

namespace KeepAlive.Tests;

[TestClass]
public class CommonAgentTests
{
    private static Mock<ICommonAdapter> _commonAdapterMock = null!;

    [ClassInitialize]
    public static void ClassInitialize(
        TestContext context)
    {
        _commonAdapterMock = new Mock<ICommonAdapter>();
    }

    [TestInitialize]
    public void TestInitialize()
    {
        _commonAdapterMock.Reset();
    }
}