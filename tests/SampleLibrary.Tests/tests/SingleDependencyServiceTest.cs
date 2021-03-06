using SampleLibrary;
using System.Threading.Tasks;
using Moq;
using NUnit;

namespace SampleLibrary.Tests
{

    [TestFixture]
    public class SingleDependencyServiceTest
    {
        private readonly MockRepository _mockRepository;
        private readonly Mock<INoDependenciesService> _noDependenciesService;

        public SingleDependencyServiceTest()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _noDependenciesService = _mockRepository.Create<INoDependenciesService>();
        }

        private SingleDependencyService CreateSystemUnderTestInstance()
        {
            return new SingleDependencyService(_noDependenciesService.Object);
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
}
