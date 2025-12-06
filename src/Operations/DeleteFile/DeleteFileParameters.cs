using FlowSynx.PluginCore;

namespace FlowSynx.Plugins.Local.Operations.DeleteFile;

internal class DeleteFileParameters
{
    [OperationParameterMetadata(Description = "The path to the file to delete.", IsRequired = true)]
    public string Path { get; set; } = string.Empty;
}