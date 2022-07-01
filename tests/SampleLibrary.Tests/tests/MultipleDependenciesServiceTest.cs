using SampleLibrary;
using System.Threading.Tasks;
using System;
using Moq;
using NUnit;

namespace SampleLibrary.Tests
{

    [TestFixture]
    public class MultipleDependenciesServiceTest
    {
        private readonly MockRepository _mockRepository;
        private readonly Mock<INoDependenciesService> _noDependenciesService;
        private readonly SuperOptions _superOptions;

        public MultipleDependenciesServiceTest()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _noDependenciesService = _mockRepository.Create<INoDependenciesService>();
            _superOptions = new SuperOptions();
        }

        private MultipleDependenciesService CreateSystemUnderTestInstance()
        {
            return new MultipleDependenciesService(_noDependenciesService.Object, _superOptions);
        }

        [Test]
        public async Task Test_ExecuteOperationAsync()
        {
            // Arrange
            var sut = CreateSystemUnderTestInstance();

            // Act

            // Assert
        }

        [Test]
        public void Test_SomeStaticMethod()
        {
            // Arrange

            // Act

            // Assert
        }

    }
}
