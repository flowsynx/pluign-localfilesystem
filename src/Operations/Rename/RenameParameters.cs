using FlowSynx.PluginCore;

namespace FlowSynx.Plugins.Local.Operations.Rename;

internal class RenameParameters
{
    [OperationParameterMetadata(Description = "The path to the file or directory to rename.", IsRequired = true)]
    public string Path { get; set; } = string.Empty;

    [OperationParameterMetadata(Description = "The new name for the file or directory.", IsRequired = true)]
    public string TargetPath { get; set; } = string.Empty;
}