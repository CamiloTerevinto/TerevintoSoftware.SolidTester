using System.Reflection;

namespace TerevintoSoftware.SolidTester.Utilities;

internal static class ReflectionHelpers
{
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

        var typeInfo = type.GetTypeInfo();

        const BindingFlags commonFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        
        return
            typeInfo.GetFields(commonFlags).Select(x => x.FieldType)
            .Concat(typeInfo.GetProperties(commonFlags).Select(x => x.PropertyType))
            .Concat(typeInfo.GetInterfaces())
            .Select(x => x.Namespace!)
            .Where(x => x != null)
            .Distinct()
            .ToArray();
    }
}
