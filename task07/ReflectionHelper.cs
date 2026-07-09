using System;
using System.Reflection;
using System.Linq;

namespace task07
{
    public static class ReflectionHelper
    {
        public static void PrintTypeInfo(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            var classDisplay = type.GetCustomAttribute<DisplayNameAttribute>();
            var classVersion = type.GetCustomAttribute<VersionAttribute>();

            if (classDisplay is not null)
                Console.WriteLine($"Отображаемое имя класса: {classDisplay.DisplayName}");

            if (classVersion is not null)
                Console.WriteLine($"Версия класса: {classVersion.Major}.{classVersion.Minor}");

            Console.WriteLine("Свойства:");
            var properties = type.GetProperties();
            foreach (var prop in properties)
            {
                var propDisplay = prop.GetCustomAttribute<DisplayNameAttribute>();
                if (propDisplay is not null)
                    Console.WriteLine($"{prop.Name}: {propDisplay.DisplayName}");
            }

            Console.WriteLine("Методы:");
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                var methodDisplay = method.GetCustomAttribute<DisplayNameAttribute>();
                if (methodDisplay is not null)
                    Console.WriteLine($"{method.Name}: {methodDisplay.DisplayName}");
            }
        }
    }
}
