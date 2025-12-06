using FlowSynx.PluginCore;

namespace FlowSynx.Plugins.Local.Operations.Create;

internal class CreateParameters
{
    [OperationParameterMetadata(Description = "The path to the directory to create.", IsRequired = true)]
    public string Path { get; set; } = string.Empty;

    [OperationParameterMetadata(Description = "Whether the created directory should be hidden.", IsRequired = false)]
    public bool? Hidden { get; set; } = false;
}