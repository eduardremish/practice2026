using System;
using System.IO;
using System.Reflection;

namespace task09
{
    [DisplayName("Тест")]
    [Version(2, 6)]
    public class SampleTestClass
    {
        public string Data { get; set; }

        public SampleTestClass(string initialData, int count)
        {
            Data = initialData;
        }

        [DisplayName("Метод обработки файлов")]
        public bool ProcessFile(string path, int mode)
        {
            return true;
        }
    }

    class Program
    {
        static void PrintParameters(ParameterInfo[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                Console.Write($"{parameters[i].ParameterType.Name} {parameters[i].Name}");
                if (i < parameters.Length - 1) Console.Write(", ");
            }
        }

        static void Main(string[] args)
        {
            string dllPath = args.Length > 0 ? args[0] : Assembly.GetExecutingAssembly().Location;

            if (!File.Exists(dllPath))
            {
                Console.WriteLine($"Ошибка. Файл не найден по пути: {dllPath}");
                return;
            }

            try
            {
                Assembly assembly = Assembly.LoadFrom(dllPath);
                Type[] types;

                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Console.WriteLine($"Ошибка загрузки типов: {ex.Message}");
                    Console.WriteLine($"Загружено {ex.Types.Length} типов из {ex.LoaderExceptions.Length} с ошибками:");

                    types = ex.Types;

                    for (int i = 0; i < ex.LoaderExceptions.Length; i++)
                    {
                        Console.WriteLine($"  Ошибка {i + 1}: {ex.LoaderExceptions[i]?.Message}");
                    }

                    if (types == null || types.Length == 0)
                    {
                        Console.WriteLine("Не удалось загрузить ни одного типа.");
                        return;
                    }
                }

                foreach (Type type in types)
                {
                    if (type == null) continue;
                    if (!type.IsClass) continue;

                    Console.WriteLine($"Класс: {type.FullName}");

                    PrintClassAttributes(type);
                    PrintConstructors(type);
                    PrintMethods(type);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void PrintClassAttributes(Type type)
        {
            var classAttributes = type.GetCustomAttributes();
            foreach (var attr in classAttributes)
            {
                if (attr.GetType().Name.StartsWith("Nullable"))
                    continue;

                if (attr is DisplayNameAttribute dna)
                    Console.WriteLine($"Атрибут DisplayName = \"{dna.DisplayName}\"");
                else if (attr is VersionAttribute va)
                    Console.WriteLine($"Атрибут Version = {va.Major}.{va.Minor}");
                else
                    Console.WriteLine($"Атрибут {attr.GetType().Name}");
            }
        }

        static void PrintConstructors(Type type)
        {
            Console.WriteLine("Конструкторы:");
            ConstructorInfo[] constructors = type.GetConstructors();

            foreach (var ctor in constructors)
            {
                Console.Write($"- {type.Name}(");
                ParameterInfo[] parameters = ctor.GetParameters();
                PrintParameters(parameters);
                Console.WriteLine(")");
            }
        }

        static void PrintMethods(Type type)
        {
            Console.WriteLine("Методы:");
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (var method in methods)
            {
                if (method.IsSpecialName) continue;

                Console.Write($"- {method.ReturnType.Name} {method.Name}(");
                ParameterInfo[] parameters = method.GetParameters();
                PrintParameters(parameters);
                Console.WriteLine(")");

                var methodAttrs = method.GetCustomAttributes();
                foreach (var attr in methodAttrs)
                {
                    if (attr is DisplayNameAttribute dna)
                        Console.WriteLine($"  Атрибут метода DisplayName = \"{dna.DisplayName}\"");
                }
            }
        }
    }
}
