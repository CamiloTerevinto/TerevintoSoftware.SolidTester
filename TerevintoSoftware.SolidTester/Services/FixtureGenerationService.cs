using TerevintoSoftware.SolidTester.Models;
using TerevintoSoftware.SolidTester.Templates;
using TerevintoSoftware.SolidTester.Utilities;

namespace TerevintoSoftware.SolidTester.Services;

public class FixtureGenerationService
{

    public async Task GenerateFixturesAsync()
    {
        var model = new FixtureModel
        {
            ClassName = nameof(SampleLibrary.SuperUsefulService),
            ClassNamespace = null!,
            BaseNamespace = "SampleLibrary.Tests",
            RequiredUsings = ReflectionHelpers.FindReferencedUsingsForType(typeof(SampleLibrary.SuperUsefulService)),
            Dependencies = ReflectionHelpers.FindConstructorDependencies(typeof(SampleLibrary.SuperUsefulService)),
        };

        var template = new FixtureTemplate(model);

        var result = template.GetTemplate();

        Console.WriteLine(result);
    }
}
