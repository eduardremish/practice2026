using CommandLib;
using System.Reflection;

namespace PluginApp;

public class PluginLoader
{
    private readonly List<Type> _allPluginTypes = new();
    private readonly List<Type> _sortedPlugins = new();
    private readonly HashSet<string> _visiting = new();
    private readonly HashSet<string> _visited = new();

    public IReadOnlyList<Type> AllPluginTypes => _allPluginTypes;
    public IReadOnlyList<Type> SortedPlugins => _sortedPlugins;

    public List<Type> LoadPlugins(string pluginsDir)
    {
        _allPluginTypes.Clear();
        _sortedPlugins.Clear();
        _visiting.Clear();
        _visited.Clear();

        if (!Directory.Exists(pluginsDir))
        {
            Directory.CreateDirectory(pluginsDir);
            return _allPluginTypes;
        }

        string[] dllFiles = Directory.GetFiles(pluginsDir, "*.dll");

        foreach (string dllPath in dllFiles)
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(dllPath);

                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && typeof(ICommand).IsAssignableFrom(type))
                    {
                        _allPluginTypes.Add(type);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось загрузить {Path.GetFileName(dllPath)}: {ex.Message}");
            }
        }

        return _allPluginTypes;
    }

    public List<Type> SortPlugins()
    {
        _sortedPlugins.Clear();
        _visiting.Clear();
        _visited.Clear();

        foreach (var type in _allPluginTypes)
        {
            SortPlugin(type);
        }

        return _sortedPlugins;
    }

    private void SortPlugin(Type type)
    {
        if (_visited.Contains(type.Name)) return;

        if (_visiting.Contains(type.Name))
        {
            throw new InvalidOperationException($"Циклическая зависимость: {type.Name}!");
        }

        _visiting.Add(type.Name);

        var attribute = type.GetCustomAttribute<PluginLoadAttribute>();
        if (attribute != null && !string.IsNullOrEmpty(attribute.PluginLoadPastNode))
        {
            foreach (var plugin in _allPluginTypes)
            {
                if (plugin.Name == attribute.PluginLoadPastNode)
                {
                    SortPlugin(plugin);
                    break;
                }
            }
        }

        _visiting.Remove(type.Name);
        _visited.Add(type.Name);
        _sortedPlugins.Add(type);
    }

    public void ExecutePlugins()
    {
        foreach (var type in _sortedPlugins)
        {
            if (Activator.CreateInstance(type) is ICommand instance)
            {
                instance.Execute();
            }
        }
    }
}

class Program
{
    public static void Main()
    {
        string pluginsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");

        Console.WriteLine($"=== PluginApp ===");
        Console.WriteLine($"Поиск плагинов в: {pluginsDir}");

        var loader = new PluginLoader();
        var plugins = loader.LoadPlugins(pluginsDir);

        Console.WriteLine($"Найдено плагинов: {plugins.Count}");

        if (plugins.Count == 0)
        {
            Console.WriteLine("Нет плагинов для выполнения.");
            return;
        }

        try
        {
            loader.SortPlugins();
            loader.ExecutePlugins();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Ошибка: {ex.Message}");
            Console.ResetColor();
        }
    }
}
