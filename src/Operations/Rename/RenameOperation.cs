using FlowSynx.PluginCore;
using FlowSynx.PluginCore.Extensions;

namespace FlowSynx.Plugins.Local.Operations.Rename;

internal class RenameOperation : IPluginOperation<RenameParameters, object>
{
    private readonly IPluginLogger? _logger;

    public RenameOperation(IPluginLogger? logger)
    {
        _logger = logger;
    }

    public string Name => "Rename";
    public string Description => "Renames a file or directory.";

    public async Task<object?> ExecuteAsync(RenameParameters parameters, CancellationToken cancellationToken)
    {
        ValidateRenameParameters(parameters);
        RenameEntity(parameters, cancellationToken);
        return await Task.FromResult<object?>(null);
    }

    private static void ValidateRenameParameters(RenameParameters parameters)
    {
        var sourcePath = PathHelper.ToUnixPath(parameters.Path);
        var targetPath = PathHelper.ToUnixPath(parameters.TargetPath);

        if (!File.Exists(sourcePath) && !Directory.Exists(sourcePath))
            throw new ArgumentException(string.Format(Resources.RenameSourcePathDoesNotExist, sourcePath));

        if (File.Exists(targetPath) || Directory.Exists(targetPath))
            throw new ArgumentException(string.Format(Resources.RenameTargetPathIsAlreadyExist, targetPath));
    }

    private void RenameEntity(RenameParameters parameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var sourcePath = PathHelper.ToUnixPath(parameters.Path);
        var targetPath = PathHelper.ToUnixPath(parameters.TargetPath);

        if (File.Exists(sourcePath))
        {
            File.Move(sourcePath, targetPath);
            _logger?.LogInfo(string.Format(Resources.FileRenamed, sourcePath, targetPath));
        }
        else if (Directory.Exists(sourcePath))
        {
            Directory.Move(sourcePath, targetPath);
            _logger?.LogInfo(string.Format(Resources.DirectoryRenamed, sourcePath, targetPath));
        }
    }
}