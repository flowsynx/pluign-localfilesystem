using FlowSynx.PluginCore;

namespace FlowSynx.Plugins.Local.Operations.Exist;

internal class ExistParameters
{
    [OperationParameterMetadata(Description = "The path to the file or directory to check for existence.", IsRequired = true)]
    public string Path { get; set; } = string.Empty;
}