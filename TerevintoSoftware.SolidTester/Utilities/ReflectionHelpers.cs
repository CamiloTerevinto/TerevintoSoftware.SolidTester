using System.Reflection;
using TerevintoSoftware.SolidTester.Configuration;
using TerevintoSoftware.SolidTester.Models;

namespace TerevintoSoftware.SolidTester.Utilities;

internal static class ReflectionHelpers
{
    private const BindingFlags _commonFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

    internal static FixtureModel BuildModel(string baseTestNamespace, Type target)
    {
        return new FixtureModel
        {
            ClassName = target.Name,
            ClassNamespace = GetClassNamespace(target, baseTestNamespace),
            RequiredUsings = FindReferencedUsingsForType(target),
            Dependencies = FindConstructorDependencies(target),
            Methods = FindMethodsInClass(target)
        };
    }

    internal static IReadOnlyCollection<Type> FindConstructorDependencies(Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }
        else if (!type.IsClass)
        {
            throw new ArgumentException($"{nameof(type)} must be a class.", nameof(type));
        }

        var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

        if (constructors.Length != 1)
        {
            return Array.Empty<Type>();
        }

        return constructors.Single()
            .GetParameters()
            .Select(p => p.ParameterType)
            .ToArray();
    }

    internal static IReadOnlyCollection<string> FindReferencedUsingsForType(Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }
        else if (!type.IsClass)
        {
            throw new ArgumentException($"{nameof(type)} must be a class.", nameof(type));
        }

        return type.GetFields(_commonFlags).Select(x => x.FieldType)
            .Concat(type.GetProperties(_commonFlags).Select(x => x.PropertyType))
            .Concat(type.GetInterfaces())
            .Concat(type.GetMethods(_commonFlags).SelectMany(x => x.GetParameters().Select(y => y.ParameterType).Append(x.ReturnType)))
            .Select(x => x.Namespace!)
            .Where(x => x != null)
            .Distinct()
            .ToArray();
    }

    internal static IReadOnlyCollection<TestableMethod> FindMethodsInClass(Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }
        else if (!type.IsClass)
        {
            throw new ArgumentException($"{nameof(type)} must be a class.", nameof(type));
        }

        var taskType = typeof(Task);

        return type.GetMethods(_commonFlags)
            .Select(m => new TestableMethod
            {
                Name = m.Name,
                IsAsync = taskType.IsAssignableFrom(m.ReturnType),
                IsStatic = m.IsStatic
            })
            .ToArray();
    }

    internal static IReadOnlyCollection<Type> FindTypesInAssembly(Assembly assembly, ServiceSearchStrategy serviceSearchStrategy)
    {
        var types = assembly.GetTypes().Where(x => x.IsClass && x.MemberType == MemberTypes.TypeInfo && x.GetInterfaces().Length > 0);

        if (serviceSearchStrategy == ServiceSearchStrategy.PublicClasses)
        {
            types = types.Where(x => x.IsPublic);
        }
        else if (serviceSearchStrategy == ServiceSearchStrategy.InternalClasses)
        {
            types = types.Where(x => x.IsNotPublic);
        }

        return types.ToArray();
    }

    internal static string GetClassNamespace(Type type, string baseTestNamespace)
    {
        var assembly = type.Assembly;
        var baseNamespace = assembly.GetName().Name;
        var typeNamespace = type.Namespace?.Replace(baseNamespace!, "");

        if (string.IsNullOrEmpty(typeNamespace))
        {
            return baseTestNamespace;
        }

        return baseTestNamespace + "." + typeNamespace;
    }
}
