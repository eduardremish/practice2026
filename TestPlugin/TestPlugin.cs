using CommandLib;
using System;

namespace TestPlugin
{
    [PluginLoad("")]
    public class AnalyticsCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("Тестовый плагин.");
        }
    }
}