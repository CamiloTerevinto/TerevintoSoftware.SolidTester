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

    private int _currentIndentationLevel = 0;

    public FixtureTemplate(FixtureModel fixtureModel)
    {
        _context = new Context(fixtureModel.Dependencies);
        _fixtureModel = fixtureModel;
    }

    internal string GetTemplate()
    {
        AddUsings(_fixtureModel.RequiredUsings);
        AddEmptyLine();

        AddNamespace(_fixtureModel.ClassNamespace);
        BeginBlock();
        AddEmptyLine();

        AddClassDeclaration(_fixtureModel.ClassName);
        BeginBlock();

        AddDependenciesAsFields();

        AddConstructor(_fixtureModel.ClassName);

        AddCreateSut(_fixtureModel.ClassName);

        AddEmptyLine();

        AddTests();

        EndBlock();

        EndBlock();

        return _builder.ToString();
    }

    private void AddUsings(IEnumerable<string> usings)
    {
        const string template = "using {0};";

        foreach (var statement in usings)
        {
            AddIndented(string.Format(template, statement));
        }

        if (_context.UsingMocks)
        {
            AddIndented("using Moq;");
        }

        AddIndented("using NUnit;");
    }

    private void AddNamespace(string ns)
    {
        AddIndented($"namespace {ns}");
    }

    private void AddClassDeclaration(string className)
    {
        AddIndented("[TestFixture]");
        AddIndented($"public class {className}Test");
    }

    private void AddDependenciesAsFields()
    {
        if (_context.Interfaces.Count == 0 && _context.Classes.Count == 0)
        {
            return;
        }

        if (_context.Interfaces.Count > 0)
        {
            const string moqRepository = "private readonly MockRepository _mockRepository;";
            const string mockTemplate = "private readonly Mock<{0}> _{1};";

            AddIndented(moqRepository);

            foreach (var dependency in _context.Interfaces)
            {
                AddFormatIndented(mockTemplate, dependency.Type.Name, dependency.Name);
            }
        }

        if (_context.Classes.Count > 0)
        {
            const string template = "private readonly {0} _{1};";

            foreach (var dependency in _context.Classes)
            {
                AddFormatIndented(template, dependency.Type.Name, dependency.Name);
            }
        }

        AddEmptyLine();

    }

    private void AddConstructor(string className)
    {
        if (_context.Classes.Count == 0 && _context.Interfaces.Count == 0)
        {
            // There's no need for a constructor if there are no dependencies
            return;
        }

        const string template = "public {0}Test()";

        AddFormatIndented(template, className);
        BeginBlock();

        if (_context.UsingMocks)
        {
            const string mockRepositoryTemplate = "_mockRepository = new MockRepository(MockBehavior.Default);";
            const string mockTemplate = "_{0} = _mockRepository.Create<{1}>();";

            AddIndented(mockRepositoryTemplate);

            foreach (var dependency in _context.Interfaces)
            {
                AddFormatIndented(mockTemplate, dependency.Name, dependency.Type.Name);
            }
        }

        const string classTemplate = "_{0} = new {1}();";

        foreach (var dependency in _context.Classes)
        {
            AddFormatIndented(classTemplate, dependency.Name, dependency.Type.Name);
        }

        EndBlock();

        AddEmptyLine();
    }

    private void AddCreateSut(string className)
    {
        const string methodTemplate = "private {0} CreateSystemUnderTestInstance()";
        const string constructorTemplate = "return new {0}({1});";

        AddFormatIndented(methodTemplate, className);
        BeginBlock();

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

        AddFormatIndented(constructorTemplate, className, joined);

        EndBlock();
    }

    private void AddTests()
    {
        const string testTemplate = "[Test]";
        const string syncMethodTemplate = "public void Test_{0}()";
        const string asyncMethodTemplate = "public async Task Test_{0}()";

        foreach (var method in _fixtureModel.Methods)
        {
            var name = method.Name;

            AddIndented(testTemplate);

            if (method.IsAsync)
            {
                AddFormatIndented(asyncMethodTemplate, name);
            }
            else
            {
                AddFormatIndented(syncMethodTemplate, name);
            }

            BeginBlock();

            if (method.IsStatic)
            {
                AddStaticMethodTest(method);
            }
            else
            {
                AddInstanceMethodTest(method);
            }

            EndBlock();

            AddEmptyLine();
        }
    }

    private void AddInstanceMethodTest(TestableMethod method)
    {
        const string arrangeComment = "// Arrange";
        const string actComment = "// Act";
        const string assertComment = "// Assert";
        const string createSutInstanceTemplate = "var sut = CreateSystemUnderTestInstance();";

        AddIndented(arrangeComment);

        AddIndented(createSutInstanceTemplate);

        AddEmptyLine();

        AddIndented(actComment);

        AddEmptyLine();

        AddIndented(assertComment);
    }

    private void AddStaticMethodTest(TestableMethod method)
    {
        const string arrangeComment = "// Arrange";
        const string actComment = "// Act";
        const string assertComment = "// Assert";

        AddIndented(arrangeComment);

        AddEmptyLine();

        AddIndented(actComment);

        AddEmptyLine();

        AddIndented(assertComment);
    }

    private void BeginBlock()
    {
        AddIndented("{");
        _currentIndentationLevel++;
    }

    private void EndBlock()
    {
        _currentIndentationLevel--;
        AddIndented("}");
    }

    private void AddEmptyLine()
    {
        _builder.AppendLine();
    }

    private void AddIndented(string value)
    {
        _builder.AppendLine(new string(' ', _currentIndentationLevel * 4) + value);
    }

    private void AddFormatIndented(string value, params object[] args)
    {
        _builder.AppendFormat(new string(' ', _currentIndentationLevel * 4) + value + Environment.NewLine, args);
    }
}
