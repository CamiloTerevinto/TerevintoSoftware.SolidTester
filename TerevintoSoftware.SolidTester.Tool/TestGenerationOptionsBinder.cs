using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Binding;
using TerevintoSoftware.SolidTester.Configuration;

namespace TerevintoSoftware.SolidTester.Tool;
internal class TestGenerationOptionsBinder : BinderBase<TestGenerationOptions>
{
    private readonly Option<string?> _assemblyPathOption;
    private readonly Option<string> _outputPathOption;
    private readonly Option<string> _baseNamespaceOption;
    private readonly Option<ServiceSearchStrategy> _searchStrategyOption;

    public TestGenerationOptionsBinder()
    {
        _assemblyPathOption = BuildAssemblyPathOption();
        _outputPathOption = BuildOutputPathOption();
        _baseNamespaceOption = BuildBaseNamespaceOption();
        _searchStrategyOption = BuildSearchStrategyOption();
    }

    internal static RootCommand BuildRootCommand()
    {
        var binder = new TestGenerationOptionsBinder();

        var rootCommand = new RootCommand(
            "This .NET tool inspects a .NET project and generates Unit Tests with detected dependencies and methods."
            + Environment.NewLine + "This tool currently only supports NUnit tests with Moq mocks."
            + Environment.NewLine + "Please look at the GitHub project for more information: "
            + "https://github.com/CamiloTerevinto/TerevintoSoftware.SolidTester")
        {
            Name = "solid-tester"
        };

        rootCommand.AddOption(binder._assemblyPathOption);
        rootCommand.AddOption(binder._outputPathOption);
        rootCommand.AddOption(binder._baseNamespaceOption);
        rootCommand.AddOption(binder._searchStrategyOption);

        rootCommand.SetHandler(async (TestGenerationOptions options) =>
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<TestGenerator>();
            var testGenerator = new TestGenerator(logger);

            logger.LogInformation("Processing started...");


            await testGenerator.GenerateTestsAsync(options);
        }, binder);

        return rootCommand;
    }

    protected override TestGenerationOptions GetBoundValue(BindingContext bindingContext)
    {
        return new TestGenerationOptions(
            bindingContext.ParseResult.GetValueForOption(_assemblyPathOption)!,
            bindingContext.ParseResult.GetValueForOption(_outputPathOption)!,
            bindingContext.ParseResult.GetValueForOption(_baseNamespaceOption)!,
            bindingContext.ParseResult.GetValueForOption(_searchStrategyOption));
    }

    private static Option<string> BuildOutputPathOption()
    {
        var outputPathOption = new Option<string>(
            "--output",
            parseArgument: result =>
            {
                if (result.Tokens.Count != 1)
                {
                    result.ErrorMessage = "Missing output path";
                    return null!;
                }

                var outputPath = result.Tokens.Single().Value;

                if (Directory.Exists(outputPath))
                {
                    Directory.Delete(outputPath, true);
                }

                Directory.CreateDirectory(outputPath);

                return outputPath;
            },
            description: "The path to the output directory. All contents in the directory will be deleted")
        {
            IsRequired = true
        };

        return outputPathOption;
    }

    private static Option<string?> BuildAssemblyPathOption()
    {
        var assemblyPathOption =
            new Option<string?>(
                "--assembly",
                parseArgument: result =>
                {
                    if (result.Tokens.Count == 0)
                    {
                        return null;
                    }

                    var assemblyPath = result.Tokens.Single().Value;

                    if (!assemblyPath.EndsWith(".dll"))
                    {
                        result.ErrorMessage = "The assembly path must refer to a .dll";
                        return null;
                    }

                    if (!File.Exists(assemblyPath))
                    {
                        result.ErrorMessage = $"Assembly path '{assemblyPath}' does not exist";
                        return null;
                    }

                    return assemblyPath;
                },
                description: "The path to the assembly to use for the project.")
            {
                IsRequired = true
            };

        return assemblyPathOption;
    }

    private static Option<string> BuildBaseNamespaceOption()
    {
        var baseNamespaceOption = new Option<string>(
            "--base-namespace",
            description: "The prefix to use for the namespaces in generated code.")
        {
            IsRequired = true
        };

        return baseNamespaceOption;
    }

    private static Option<ServiceSearchStrategy> BuildSearchStrategyOption()
    {
        var searchStrategyOption = new Option<ServiceSearchStrategy>(
            "--search-strategy",
            () => ServiceSearchStrategy.PublicAndInternalClasses,
            description: "The types of services to find.");

        return searchStrategyOption;
    }
}