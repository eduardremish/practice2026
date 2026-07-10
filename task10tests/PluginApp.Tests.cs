using CommandLib;
using PluginApp;
using System.Reflection;
using Xunit;

namespace PluginApp.Tests
{
    public class PluginLoaderTests
    {
        
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

