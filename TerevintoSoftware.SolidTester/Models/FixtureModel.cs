#nullable disable

namespace TerevintoSoftware.SolidTester.Models;

public class FixtureModel 
{
    public string ClassName { get; set; }
    public string ClassNamespace { get; set; }

    public IReadOnlyCollection<Type> Dependencies { get; set; }
    public IReadOnlyCollection<string> RequiredUsings { get; set; }
    public IReadOnlyCollection<TestableMethod> Methods { get; set; }
}

public class TestableMethod
{
    public string Name { get; set; }
    public bool IsAsync { get; set; }
    public bool IsStatic { get; set; }
}