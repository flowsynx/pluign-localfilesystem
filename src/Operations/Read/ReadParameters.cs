using FlowSynx.PluginCore;

namespace FlowSynx.Plugins.Local.Operations.Read;

internal class ReadParameters
{
    [OperationParameterMetadata(Description = "The path to the file to read.", IsRequired = true)]
    public string Path { get; set; } = string.Empty;
}