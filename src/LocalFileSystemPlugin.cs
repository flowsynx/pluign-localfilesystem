using FlowSynx.PluginCore;
using FlowSynx.PluginCore.Extensions;
using FlowSynx.Plugins.Local.Operations.Create;
using FlowSynx.Plugins.Local.Operations.DeleteFile;
using FlowSynx.Plugins.Local.Operations.Exist;
using FlowSynx.Plugins.Local.Operations.List;
using FlowSynx.Plugins.Local.Operations.Purge;
using FlowSynx.Plugins.Local.Operations.Read;
using FlowSynx.Plugins.Local.Operations.Rename;
using FlowSynx.Plugins.Local.Operations.Write;
using FlowSynx.Plugins.Local.Services;

namespace FlowSynx.Plugins.Local;

public class LocalFileSystemPlugin : IPlugin
{
    private readonly IGuidProvider _guidProvider;
    private readonly IReflectionGuard _reflectionGuard;
    private LocalFileSystemSpecifications? _specifications = null;
    private IPluginLogger? _logger;
    private bool _isInitialized;

    public LocalFileSystemPlugin() : this(new GuidProvider(), new DefaultReflectionGuard()) { }

    internal LocalFileSystemPlugin(IGuidProvider guidProvider, IReflectionGuard reflectionGuard)
    {
        _guidProvider = guidProvider ?? throw new ArgumentNullException(nameof(guidProvider));
        _reflectionGuard = reflectionGuard ?? throw new ArgumentNullException(nameof(reflectionGuard));
    }

    public PluginMetadata Metadata => new()
    {
        Id = Guid.Parse("f6304870-0294-453e-9598-a82167ace653"),
        Name = "Local",
        Description = Resources.PluginDescription,
        Version = new Version(1, 1, 0),
        Category = PluginCategory.Storage,
        CompanyName = "FlowSynx",
        Authors = new List<string> { "FlowSynx" },
        Copyright = "© FlowSynx. All rights reserved.",
        Icon = "flowsynx.png",
        ReadMe = "README.md",
        RepositoryUrl = "https://github.com/flowsynx/plugin-json",
        ProjectUrl = "https://flowsynx.io",
        Tags = new List<string>() { "flowSynx", "local", "local-filesystem" },
        MinimumFlowSynxVersion = new Version(1, 3, 0)
    };

    public IPluginSpecifications? Specifications => _specifications;

    public IReadOnlyCollection<IPluginOperation> SupportedOperations => new IPluginOperation[]
    {
        new CreateOperation(),
        new DeleteFileOperation(_logger),
        new ExistOperation(),
        new ListOperation(_logger),
        new PurgeOperation(_logger),
        new ReadOperation(),
        new RenameOperation(_logger),
        new WriteOperation(_logger),
    };

    public Task InitializeAsync(IPluginLogger logger, IDictionary<string, object?>? specifications)
    {
        if (_reflectionGuard.IsCalledViaReflection())
            throw new InvalidOperationException(Resources.ReflectionBasedAccessIsNotAllowed);

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var localFileSystemSpecifications = new LocalFileSystemSpecifications();
        if (specifications != null)
            localFileSystemSpecifications.FromDictionary(specifications);

        localFileSystemSpecifications.Validate();
        _specifications = localFileSystemSpecifications;

        _isInitialized = true;
        return Task.CompletedTask;
    }

    public async Task<object?> ExecuteAsync(
        string? operationName, 
        PluginParameters parameters, 
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_reflectionGuard.IsCalledViaReflection())
            throw new InvalidOperationException(Resources.ReflectionBasedAccessIsNotAllowed);

        if (!_isInitialized)
            throw new InvalidOperationException($"Plugin '{Metadata.Name}' v{Metadata.Version} is not initialized.");

        var operation = SupportedOperations
            .FirstOrDefault(op => string.Equals(op.Name, operationName, StringComparison.OrdinalIgnoreCase))
            ?? throw new NotSupportedException($"Operation '{operationName}' is not supported.");

        return operation.Name.ToLowerInvariant() switch
        {
            "create" => await((CreateOperation)operation)
                            .ExecuteAsync(parameters.ToObject<CreateParameters>(), cancellationToken),

            "delete" => await ((DeleteFileOperation)operation)
                .ExecuteAsync(parameters.ToObject<DeleteFileParameters>(), cancellationToken),

            "exist" => await ((ExistOperation)operation)
                .ExecuteAsync(parameters.ToObject<ExistParameters>(), cancellationToken),

            "list" => await ((ListOperation)operation)
                .ExecuteAsync(parameters.ToObject<ListParameters>(), cancellationToken),

            "purge" => await ((PurgeOperation)operation)
                .ExecuteAsync(parameters.ToObject<PurgeParameters>(), cancellationToken),

            "read" => await ((ReadOperation)operation)
                .ExecuteAsync(parameters.ToObject<ReadParameters>(), cancellationToken),

            "rename" => await ((RenameOperation)operation)
                .ExecuteAsync(parameters.ToObject<RenameParameters>(), cancellationToken),

            "write" => await ((WriteOperation)operation)
                .ExecuteAsync(parameters.ToObject<WriteParameters>(), cancellationToken),

            _ => throw new NotSupportedException($"Unsupported operation: {operation.Name}")
        };
    }
}