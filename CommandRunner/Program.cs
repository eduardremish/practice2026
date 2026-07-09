using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandLib;

namespace CommandRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            const string dllName = "FileSystemCommands.dll";
            string dllPath = FindDllPath(dllName);

            if (string.IsNullOrEmpty(dllPath))
            {
                Console.WriteLine($"Не найден файл динамической библиотеки: {dllName}");
                return;
            }

            try
            {
                ProcessCommands(dllPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static string FindDllPath(string dllName)
        {
            string exePath = AppDomain.CurrentDomain.BaseDirectory;
            string path1 = Path.Combine(exePath, dllName);
            if (File.Exists(path1)) return path1;

            string path2 = Path.Combine(exePath, "..", "..", "..", "..", "FileSystemCommands", "bin", "Debug", "net10.0", dllName);
            if (File.Exists(path2)) return path2;

            return null;
        }

        static void ProcessCommands(string dllPath)
        {
            Assembly assembly = Assembly.LoadFrom(dllPath);
            string sampleDir = CreateTestDirectory();
            CreateTestFiles(sampleDir);

            var commandTypes = GetCommandTypes(assembly);
            Console.WriteLine($"Найдено команд: {commandTypes.Count}");

            foreach (var type in commandTypes)
            {
                ExecuteCommand(type, sampleDir);
            }

            CleanupTestDirectory(sampleDir);
        }

        static string CreateTestDirectory()
        {
            string sampleDir = Path.Combine(Path.GetTempPath(), $"RunnerTestDir_{Guid.NewGuid()}");
            Directory.CreateDirectory(sampleDir);
            Console.WriteLine($"Тестовая директория: {sampleDir}");
            return sampleDir;
        }

        static void CreateTestFiles(string directory)
        {
            File.WriteAllText(Path.Combine(directory, "document.txt"), "abcdefg");
            File.WriteAllText(Path.Combine(directory, "text.txt"), "Text file");
            Console.WriteLine("Созданы файлы: document.txt, text.txt\n");
        }

        static List<Type> GetCommandTypes(Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(t => typeof(ICommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToList();
        }

        static void ExecuteCommand(Type type, string sampleDir)
        {
            Console.WriteLine(type.Name);
            ICommand commandInstance = CreateCommandInstance(type, sampleDir);

            if (commandInstance is not null)
            {
                RunCommand(commandInstance);
            }

            Console.WriteLine();
        }

        static ICommand CreateCommandInstance(Type type, string sampleDir)
        {
            try
            {
                if (type.Name == "DirectorySizeCommand")
                {
                    var instance = (ICommand)Activator.CreateInstance(type, new object[] { sampleDir });
                    Console.WriteLine($"Создан экземпляр DirectorySizeCommand с директорией: {sampleDir}");
                    return instance;
                }

                if (type.Name == "FindFilesCommand")
                {
                    var instance = (ICommand)Activator.CreateInstance(type, new object[] { sampleDir, "*.txt" });
                    Console.WriteLine($"Создан экземпляр FindFilesCommand с директорией: {sampleDir} и маской: *.txt");
                    return instance;
                }

                return (ICommand)Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return null;
            }
        }

        static void RunCommand(ICommand command)
        {
            try
            {
                Console.WriteLine("Выполнение команды:");
                command.Execute();
                Console.WriteLine("Команда выполнена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static void CleanupTestDirectory(string sampleDir)
        {
            try
            {
                Directory.Delete(sampleDir, true);
                Console.WriteLine($"Директория удалена: {sampleDir}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
