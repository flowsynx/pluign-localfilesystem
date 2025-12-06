using FlowSynx.PluginCore;
using FlowSynx.PluginCore.Extensions;

namespace FlowSynx.Plugins.Local.Operations.Purge;

internal class PurgeOperation : IPluginOperation<PurgeParameters, object>
{
    private readonly IPluginLogger? _logger;

    public PurgeOperation(IPluginLogger? logger)
    {
        // Allow null logger; SupportedOperations may be accessed before initialization.
        _logger = logger;
    }

    public string Name => "Purge";
    public string Description => "Deletes directory.";

    public async Task<object?> ExecuteAsync(PurgeParameters parameters, CancellationToken cancellationToken)
    {
        ValidatePurgeParameters(parameters);
        PurgeDirectory(parameters, cancellationToken);
        return await Task.FromResult<object?>(null);
    }

    private static void ValidatePurgeParameters(PurgeParameters parameters)
    {
        var path = PathHelper.ToUnixPath(parameters.Path);
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException(Resources.TheSpecifiedPathMustBeNotEmpty);

        if (!PathHelper.IsDirectory(path))
            throw new ArgumentException(string.Format(Resources.TheSpecifiedDirectoryPathIsNotDirectory, path));

        if (!Directory.Exists(path))
            throw new ArgumentException(string.Format(Resources.TheSpecifiedPathIsNotExist, path));
    }

    private void PurgeDirectory(PurgeParameters parameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var path = PathHelper.ToUnixPath(parameters.Path);
        var directoryInfo = new DirectoryInfo(path);
        var deleteRecursively = parameters.Force ?? false;

        directoryInfo.Delete(deleteRecursively);
        _logger?.LogInfo(string.Format(Resources.TheSpecifiedPathWasPurged, path));
    }
}