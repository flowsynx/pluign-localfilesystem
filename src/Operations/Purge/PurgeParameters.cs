using FlowSynx.PluginCore;

namespace FlowSynx.Plugins.Local.Operations.Purge;

internal class PurgeParameters
{
    [OperationParameterMetadata(Description = "The path to the directory to purge.", IsRequired = true)]
    public string Path { get; set; } = string.Empty;

    [OperationParameterMetadata(Description = "Whether to force the purge operation.", IsRequired = false)]
    public bool? Force { get; set; } = false;
}