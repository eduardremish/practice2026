using System;

namespace CommandLib;

[AttributeUsage(AttributeTargets.Class)]
public class PluginLoadAttribute : Attribute
{
    public string PluginLoadPastNode { get; set; }

    public PluginLoadAttribute(string pluginLoadPastNode = "")
    {
        PluginLoadPastNode = pluginLoadPastNode;
    }
}
