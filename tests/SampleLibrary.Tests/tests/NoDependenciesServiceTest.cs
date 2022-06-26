using SampleLibrary;
using System.Threading.Tasks;
using NUnit;

namespace SampleLibrary.Tests;

[TestFixture]
public class NoDependenciesServiceTest
{

    public NoDependenciesServiceTest()
    {
    }

    private NoDependenciesService CreateSystemUnderTestInstance()
    {
        return new NoDependenciesService();
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
