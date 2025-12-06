using FlowSynx.PluginCore;

namespace FlowSynx.Plugins.Local.Operations.List;

internal class ListParameters
{
    [OperationParameterMetadata(Description = "The path to the directory to list.", IsRequired = true)]
    public string Path { get; set; } = string.Empty;

    [OperationParameterMetadata(Description = "A regular expression used to filter files when listing a directory.", IsRequired = false)]
    public string? Filter { get; set; }

    [OperationParameterMetadata(Description = "Whether to recurse into subdirectories.", IsRequired = false)]
    public bool? Recurse { get; set; } = false;

    [OperationParameterMetadata(Description = "Whether the listing should be case-sensitive.", IsRequired = false)]
    public bool? CaseSensitive { get; set; } = false;

    [OperationParameterMetadata(Description = "Whether to include file metadata in the listing.", IsRequired = false)]
    public bool? IncludeMetadata { get; set; } = false;

    [OperationParameterMetadata(Description = "The maximum number of results to return.", IsRequired = false)]
    public int? MaxResults { get; set; } = int.MaxValue;
}