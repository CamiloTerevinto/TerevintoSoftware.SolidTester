using SampleLibrary;
using System.Threading.Tasks;
using Moq;
using NUnit;

namespace SampleLibrary.Tests;

[TestFixture]
public class SuperUsefulServiceTest
{
    private readonly MockRepository _mockRepository;
    private readonly Mock<ISomeDependencyService> _someDependencyService;
    private readonly SuperOptions _superOptions;

    public SuperUsefulServiceTest()
    {
        _mockRepository = new MockRepository(MockBehavior.Default);
        _someDependencyService = _mockRepository.Create<ISomeDependencyService>();
        _superOptions = new SuperOptions();
    }

    private SuperUsefulService CreateSystemUnderTestInstance()
    {
        return new SuperUsefulService(_someDependencyService.Object, _superOptions);
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