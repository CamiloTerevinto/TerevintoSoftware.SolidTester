using SampleLibrary;
using System;
using System.Threading.Tasks;
using NUnit;

namespace SampleLibrary.Tests
{

    [TestFixture]
    public class NoDependenciesServiceTest
    {
        private NoDependenciesService CreateSystemUnderTestInstance()
        {
            return new NoDependenciesService();
        }

        [Test]
        public async Task Test_NoOpAsyncMethod()
        {
            // Arrange
            var sut = CreateSystemUnderTestInstance();

            // Act

            // Assert
        }

        [Test]
        public void Test_NoOpSyncMethod()
        {
            // Arrange
            var sut = CreateSystemUnderTestInstance();

            // Act

            // Assert
        }

    }
}
