using Microsoft.Extensions.Logging;
using System.Reflection;
using TerevintoSoftware.SolidTester.Configuration;
using TerevintoSoftware.SolidTester.Templates;
using TerevintoSoftware.SolidTester.Utilities;

namespace TerevintoSoftware.SolidTester;

public class TestGenerator
{
    private readonly ILogger<TestGenerator> _logger;

    public TestGenerator(ILogger<TestGenerator> logger)
    {
        _logger = logger;
    }

    public async Task GenerateTestsAsync(TestGenerationOptions testGenerationOptions)
    {
        var assembly = Assembly.LoadFrom(testGenerationOptions.AssemblyPath);

        var types = ReflectionHelpers.FindTypesInAssembly(assembly, testGenerationOptions.ServicesToFind);
        _logger.LogInformation("Found {TypesCount} classes that can be used for generating tests", types.Count);

#if DEBUG
        await RunSequentially(types, testGenerationOptions);
#else
        await RunInParallel(types, testGenerationOptions);
#endif

        _logger.LogInformation("Finished generating tests");
    }

#if DEBUG
    private async Task RunSequentially(IReadOnlyCollection<Type> types, TestGenerationOptions testGenerationOptions)
    {
        foreach (var typeToGenerate in types)
        {
            await GenerateTestForType(typeToGenerate, testGenerationOptions);
        }
    }
#else
    private async Task RunInParallel(IReadOnlyCollection<Type> types, TestGenerationOptions testGenerationOptions)
    {
        await Parallel.ForEachAsync(types, async (typeToGenerate, ct) =>
        {
            await GenerateTestForType(typeToGenerate, testGenerationOptions);
        });
    }
#endif

    private async Task GenerateTestForType(Type type, TestGenerationOptions generationOptions)
    {
        try
        {
            var model = ReflectionHelpers.BuildModel(generationOptions.BaseTestNamespace, type);

            var template = new FixtureTemplate(model);

            var result = template.GetTemplate();

            var folders = model.ClassNamespace.Replace(generationOptions.BaseTestNamespace, "").Split('.', StringSplitOptions.RemoveEmptyEntries);
            var joinedPath = string.Join(Path.DirectorySeparatorChar, folders);
            var folderPath = Path.Combine(generationOptions.OutputPath, joinedPath);

            Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, $"{model.ClassName}Test.cs");

            await File.WriteAllTextAsync(filePath, result);

            _logger.LogInformation("Test generated: {TestPath}", filePath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Generating a test for the type {Type} failed due to: {Exception}", type.Name, ex.Message);
        }
    }
}
