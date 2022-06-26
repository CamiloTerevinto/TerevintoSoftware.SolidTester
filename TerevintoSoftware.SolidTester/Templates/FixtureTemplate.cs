using System.Text;
using TerevintoSoftware.SolidTester.Models;

namespace TerevintoSoftware.SolidTester.Templates;

internal class FixtureTemplate
{
    private class Context
    {
        public class Dependency
        {
            public Type Type { get; }
            public string Name { get; }

            public Dependency(Type type, string name)
            {
                Type = type;
                Name = name;
            }
        }

        public bool UsingMocks { get; }
        public IReadOnlyCollection<Dependency> Interfaces { get; }
        public IReadOnlyCollection<Dependency> Classes { get; }

        public Context(IReadOnlyCollection<Type> declaredDependencies)
        {
            if (declaredDependencies.Count == 0)
            {
                Interfaces = Array.Empty<Dependency>();
                Classes = Array.Empty<Dependency>();

                return;
            }

            var types = declaredDependencies.Where(x => x.IsInterface || x.IsClass).Select(x =>
            {
                string dependencyName;

                if (x.IsInterface)
                {
                    dependencyName = x.Name[1..];
                }
                else
                {
                    dependencyName = x.Name;
                }

                dependencyName = dependencyName[0].ToString().ToLower() + dependencyName[1..];

                return new Dependency(x, dependencyName);
            }).ToArray();

            Interfaces = types.Where(x => x.Type.IsInterface).ToArray();
            Classes = types.Where(x => x.Type.IsClass).ToArray();
            UsingMocks = Interfaces.Count > 0;
        }
    }

    private readonly StringBuilder _builder = new();
    private readonly Context _context;
    private readonly FixtureModel _fixtureModel;

    public FixtureTemplate(FixtureModel fixtureModel)
    {
        _context = new Context(fixtureModel.Dependencies);
        _fixtureModel = fixtureModel;
    }

    internal string GetTemplate()
    {
        AddUsings(_fixtureModel.RequiredUsings);
        _builder.AppendLine();

        AddNamespace(_fixtureModel.ClassNamespace);
        _builder.AppendLine();

        AddClassDeclaration(_fixtureModel.ClassName);
        BeginBlock(0);

        AddDependenciesAsFields();

        _builder.AppendLine();

        AddConstructor(_fixtureModel.ClassName);

        _builder.AppendLine();

        AddCreateSut(_fixtureModel.ClassName);

        _builder.AppendLine();

        AddTests();

        EndBlock(0);

        return _builder.ToString();
    }

    private void AddUsings(IEnumerable<string> usings)
    {
        const string template = "using {0};";

        foreach (var statement in usings)
        {
            _builder.AppendLine(string.Format(template, statement));
        }

        if (_context.UsingMocks)
        {
            _builder.AppendLine("using Moq;");
        }

        _builder.AppendLine("using NUnit;");
    }

    private void AddNamespace(string ns)
    {
        _builder.AppendLine($"namespace {ns};");
    }

    private void AddClassDeclaration(string className)
    {
        _builder.AppendLine("[TestFixture]");
        _builder.AppendLine($"public class {className}Test");
    }

    private void AddDependenciesAsFields()
    {
        if (_context.Interfaces.Count > 0)
        {
            const string moqRepository = "private readonly MockRepository _mockRepository;";
            const string mockTemplate = "private readonly Mock<{0}> _{1};";

            AddIndented(1, moqRepository);

            foreach (var dependency in _context.Interfaces)
            {
                AddFormatIndented(1, mockTemplate, dependency.Type.Name, dependency.Name);
            }
        }

        if (_context.Classes.Count > 0)
        {
            const string template = "private readonly {0} _{1};";

            foreach (var dependency in _context.Classes)
            {
                AddFormatIndented(1, template, dependency.Type.Name, dependency.Name);
            }
        }
    }

    private void AddConstructor(string className)
    {
        const string template = "public {0}Test()";

        AddFormatIndented(1, template, className);
        BeginBlock(1);

        if (_context.UsingMocks)
        {
            const string mockRepositoryTemplate = "_mockRepository = new MockRepository(MockBehavior.Default);";
            const string mockTemplate = "_{0} = _mockRepository.Create<{1}>();";

            AddIndented(2, mockRepositoryTemplate);

            foreach (var dependency in _context.Interfaces)
            {
                AddFormatIndented(2, mockTemplate, dependency.Name, dependency.Type.Name);
            }
        }

        const string classTemplate = "_{0} = new {1}();";

        foreach (var dependency in _context.Classes)
        {
            AddFormatIndented(2, classTemplate, dependency.Name, dependency.Type.Name);
        }

        EndBlock(1);
    }

    private void AddCreateSut(string className)
    {
        const string methodTemplate = "private {0} CreateSystemUnderTestInstance()";
        const string constructorTemplate = "return new {0}({1});";

        AddFormatIndented(1, methodTemplate, className);
        BeginBlock(1);

        var parameters = new string[_fixtureModel.Dependencies.Count];
        var i = 0;

        foreach (var dependency in _fixtureModel.Dependencies)
        {
            if (dependency.IsInterface)
            {
                var name = _context.Interfaces.Single(x => x.Type == dependency).Name;

                parameters[i++] = $"_{name}.Object";
            }
            else
            {
                var name = _context.Classes.Single(x => x.Type == dependency).Name;

                parameters[i++] = $"_{name}";
            }
        }

        var joined = string.Join(", ", parameters);

        AddFormatIndented(2, constructorTemplate, className, joined);

        EndBlock(1);
    }

    private void AddTests()
    {
        const string testTemplate = "[Test]";
        const string syncMethodTemplate = "public void Test_{0}()";
        const string asyncMethodTemplate = "public async Task Test_{0}()";
        const string arrangeComment = "// Arrange";
        const string createSutInstanceTemplate = "var sut = CreateSystemUnderTestInstance();";
        const string actComment = "// Act";
        const string assertComment = "// Assert";

        foreach (var method in _fixtureModel.Methods)
        {
            var name = method.Name;

            AddIndented(1, testTemplate);

            if (method.IsAsync)
            {
                AddFormatIndented(1, asyncMethodTemplate, name);
            }
            else
            {
                AddFormatIndented(1, syncMethodTemplate, name);
            }

            BeginBlock(1);

            AddIndented(2, arrangeComment);

            AddIndented(2, createSutInstanceTemplate);

            _builder.AppendLine();

            AddIndented(2, actComment);

            _builder.AppendLine();

            AddIndented(2, assertComment);

            EndBlock(1);

            _builder.AppendLine();
        }
    }

    private void BeginBlock(int indentationLevel)
    {
        AddIndented(indentationLevel, "{");
    }

    private void EndBlock(int indentationLevel)
    {
        AddIndented(indentationLevel, "}");
    }

    private void AddIndented(int level, string value)
    {
        _builder.AppendLine(new string(' ', level * 4) + value);
    }

    private void AddFormatIndented(int level, string value, params object[] args)
    {
        _builder.AppendFormat(new string(' ', level * 4) + value + Environment.NewLine, args);
    }
}
