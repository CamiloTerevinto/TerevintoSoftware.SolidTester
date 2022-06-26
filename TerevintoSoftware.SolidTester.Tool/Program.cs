using System.CommandLine;
using TerevintoSoftware.SolidTester.Tool;

var rootCommand = TestGenerationOptionsBinder.BuildRootCommand();

return await rootCommand.InvokeAsync(args);