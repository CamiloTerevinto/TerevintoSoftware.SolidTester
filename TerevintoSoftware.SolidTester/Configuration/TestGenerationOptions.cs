namespace TerevintoSoftware.SolidTester.Configuration;

public class TestGenerationOptions
{
    /// <summary>
    /// The path to the project's assembly to be used for generation.
    /// </summary>
    public string AssemblyPath { get; }

    /// <summary>
    /// The directory where the generated site will be placed.
    /// </summary>
    public string OutputPath { get; }

    /// <summary>
    /// The base namespace to use for the test classes generated.
    /// </summary>
    public string BaseTestNamespace { get; set; }

    /// <summary>
    /// The types of services to find.
    /// </summary>
    public ServiceSearchStrategy ServicesToFind { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="TestGenerationOptions"/>.
    /// </summary>
    /// <param name="assemblyPath">The path to the assembly to use to find the classes to test in.</param>
    /// <param name="outputPath">The folder path to put the generated tests in.</param>
    /// <param name="baseTestNamespace">The prefix namespace to use in all generated tests.</param>
    /// <param name="servicesToFind">The types of services to find</param>
    public TestGenerationOptions(string assemblyPath, string outputPath, string baseTestNamespace, ServiceSearchStrategy servicesToFind)
    {
        if (string.IsNullOrWhiteSpace(assemblyPath))
        {
            throw new ArgumentNullException(nameof(assemblyPath));
        }
        else if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentNullException(nameof(outputPath));
        }
        else if (string.IsNullOrWhiteSpace(baseTestNamespace))
        {
            throw new ArgumentNullException(nameof(baseTestNamespace));
        }

        AssemblyPath = assemblyPath;
        OutputPath = outputPath;
        BaseTestNamespace = baseTestNamespace;
        ServicesToFind = servicesToFind;
    }
}

/// <summary>
/// The types of service to find, according to accessibility.
/// </summary>
public enum ServiceSearchStrategy
{
    /// <summary>
    /// Only public clases.
    /// </summary>
    PublicClasses = 1,

    /// <summary>
    /// Only internal classes.
    /// </summary>
    InternalClasses = 2,

    /// <summary>
    /// All public and internal classes.
    /// </summary>
    PublicAndInternalClasses = 3
}