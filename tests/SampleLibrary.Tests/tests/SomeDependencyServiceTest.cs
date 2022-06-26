using SampleLibrary;
using System;
using System.Threading.Tasks;
using NUnit;

namespace SampleLibrary.Tests;

[TestFixture]
public class SomeDependencyServiceTest
{

    public SomeDependencyServiceTest()
    {
    }

    private SomeDependencyService CreateSystemUnderTestInstance()
    {
        return new SomeDependencyService();
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
