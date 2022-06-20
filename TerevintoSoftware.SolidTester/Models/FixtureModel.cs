namespace TerevintoSoftware.SolidTester.Models;

public class FixtureModel 
{
    public string BaseNamespace { get; set; }
    public string ClassName { get; set; }
    public string? ClassNamespace { get; set; }

    public IReadOnlyCollection<Type> Dependencies { get; set; }
    public IReadOnlyCollection<string> RequiredUsings { get; set; }

    public FixtureModel()
    {
    }
}