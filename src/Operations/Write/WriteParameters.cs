using FlowSynx.PluginCore;

namespace FlowSynx.Plugins.Local.Operations.Write;

internal class WriteParameters
{
    [OperationParameterMetadata(
        Description = "The path to the file to write.", 
        IsRequired = true)
    ]
    public string? Path { get; set; }

    [OperationParameterMetadata(
        Description = "Data to be written to the file; supported types include string, base64 string, and PluginContext.", 
        IsRequired = true)
    ]
    public object? Data { get; set; }

    [OperationParameterMetadata(
        Description = "Whether to overwrite the file if it already exists.", 
        IsRequired = false)
    ]
    public bool Overwrite { get; set; } = false;
}