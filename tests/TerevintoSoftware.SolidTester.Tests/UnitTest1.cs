using SampleLibrary;
using Moq;

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
}