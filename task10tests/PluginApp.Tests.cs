using CommandLib;
using PluginApp;
using System.Reflection;
using Xunit;

namespace PluginApp.Tests
{
    public class PluginLoaderTests
    {
        [Fact]
        public void LoadPlugins_ShouldLoadAllPluginsFromDirectory()
        {

            var loader = new PluginLoader();

            string pluginsDir = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "..", "..", "..", "..",
                "PluginApp", "bin", "Debug", "net10.0", "plugins"
            );


            if (!Directory.Exists(pluginsDir))
            {
                Directory.CreateDirectory(pluginsDir);


                string sourceDll = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "..", "..", "..", "..",
                    "TestPlugin", "bin", "Debug", "net10.0", "TestPlugin.dll"
                );

                if (File.Exists(sourceDll))
                {
                    File.Copy(sourceDll, Path.Combine(pluginsDir, "TestPlugin.dll"), true);
                }
            }


            var plugins = loader.LoadPlugins(pluginsDir);


            Assert.NotEmpty(plugins);
        }

        [Fact]
        public void LoadPlugins_ShouldReturnEmpty_WhenNoDlls()
        {

            var loader = new PluginLoader();
            string emptyDir = Path.Combine(Path.GetTempPath(), "empty_plugins_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(emptyDir);


            var plugins = loader.LoadPlugins(emptyDir);


            Assert.Empty(plugins);


            Directory.Delete(emptyDir, true);
        }

    }
}

