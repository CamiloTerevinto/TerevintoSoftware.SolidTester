using TerevintoSoftware.SolidTester.Models;
using TerevintoSoftware.SolidTester.Templates;
using TerevintoSoftware.SolidTester.Utilities;

namespace TerevintoSoftware.SolidTester.Services;

public class FixtureGenerationService
{

    public async Task GenerateFixturesAsync()
    {
        var model = ReflectionHelpers.BuildModel("SampleLibrary.Tests", typeof(SampleLibrary.SuperUsefulService));

        var template = new FixtureTemplate(model);

        var result = template.GetTemplate();

        Console.WriteLine(result);
    }
}
