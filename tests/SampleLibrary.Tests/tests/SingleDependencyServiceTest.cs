using SampleLibrary;
using System.Threading.Tasks;
using Moq;
using NUnit;

namespace SampleLibrary.Tests;

[TestFixture]
public class SingleDependencyServiceTest
{
    private readonly MockRepository _mockRepository;
    private readonly Mock<ISomeDependencyService> _someDependencyService;

    public SingleDependencyServiceTest()
    {
        _mockRepository = new MockRepository(MockBehavior.Default);
        _someDependencyService = _mockRepository.Create<ISomeDependencyService>();
    }

    private SingleDependencyService CreateSystemUnderTestInstance()
    {
        return new SingleDependencyService(_someDependencyService.Object);
    }

    [Test]
    public async Task Test_ExecuteOperationAsync()
    {
        // Arrange
        var sut = CreateSystemUnderTestInstance();

        // Act

        // Assert
    }

}
