using FlowSynx.PluginCore;
using FlowSynx.PluginCore.Extensions;
using FlowSynx.Plugins.Local.Models;
using FlowSynx.Plugins.Local.Services;

namespace FlowSynx.Plugins.Local;

public class LocalFileSystemPlugin : IPlugin
{
    private readonly IGuidProvider _guidProvider;
    private readonly IReflectionGuard _reflectionGuard;
    private IPluginLogger? _logger;
    private bool _isInitialized;
    private ILocalFileManager _manager = null!;

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
        Version = new Version(1, 0, 0),
        Category = PluginCategory.Storage,
        CompanyName = "FlowSynx",
        Authors = new List<string> { "FlowSynx" },
        Copyright = "© FlowSynx. All rights reserved.",
        Icon = "flowsynx.png",
        ReadMe = "README.md",
        RepositoryUrl = "https://github.com/flowsynx/plugin-json",
        ProjectUrl = "https://flowsynx.io",
        Tags = new List<string>() { "flowSynx", "local", "local-filesystem" },
        MinimumFlowSynxVersion = new Version(1, 1, 1)
    };

    public PluginSpecifications? Specifications { get; set; }
    public Type SpecificationsType => typeof(LocalFileSystemSpecifications);

    public Task Initialize(IPluginLogger logger)
    {
        if (_reflectionGuard.IsCalledViaReflection())
            throw new InvalidOperationException(Resources.ReflectionBasedAccessIsNotAllowed);

        ArgumentNullException.ThrowIfNull(logger);
        _manager = new LocalFileManager(logger);
        _isInitialized = true;
        return Task.CompletedTask;
    }

    public Task<object?> ExecuteAsync(PluginParameters parameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_reflectionGuard.IsCalledViaReflection())
            throw new InvalidOperationException(Resources.ReflectionBasedAccessIsNotAllowed);

        if (!_isInitialized)
            throw new InvalidOperationException($"Plugin '{Metadata.Name}' v{Metadata.Version} is not initialized.");

        var operationParameter = parameters.ToObject<OperationParameter>();
        var operation = operationParameter.Operation;

        if (OperationMap.TryGetValue(operation, out var handler))
        {
            return handler(parameters, cancellationToken);
        }

        throw new NotSupportedException(string.Format(Resources.OperationIsNotSupported, operation));
    }

    private Dictionary<string, Func<PluginParameters, CancellationToken, Task<object?>>> OperationMap => new(StringComparer.OrdinalIgnoreCase)
    {
        ["create"] = async (parameters, cancellationToken) => { await _manager.Create(parameters, cancellationToken); return null; },
        ["delete"] = async (parameters, cancellationToken) => { await _manager.Delete(parameters, cancellationToken); return null; },
        ["exist"] = async (parameters, cancellationToken) => await _manager.Exist(parameters, cancellationToken),
        ["list"] = async (parameters, cancellationToken) => await _manager.List(parameters, cancellationToken),
        ["purge"] = async (parameters, cancellationToken) => { await _manager.Purge(parameters, cancellationToken); return null; },
        ["read"] = async (parameters, cancellationToken) => await _manager.Read(parameters, cancellationToken),
        ["write"] = async (parameters, cancellationToken) => { await _manager.Write(parameters, cancellationToken); return null; },
    };

    public IReadOnlyCollection<string> SupportedOperations => OperationMap.Keys;
}