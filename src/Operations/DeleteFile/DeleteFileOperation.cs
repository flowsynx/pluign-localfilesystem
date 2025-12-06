using FlowSynx.PluginCore;
using FlowSynx.PluginCore.Extensions;

namespace FlowSynx.Plugins.Local.Operations.DeleteFile;

internal class DeleteFileOperation : IPluginOperation<DeleteFileParameters, object>
{
    private readonly IPluginLogger? _logger;

    public DeleteFileOperation(IPluginLogger? logger)
    {
        // Allow null logger; operations may be inspected before plugin initialization in unit tests.
        _logger = logger;
    }

    public string Name => "Delete";
    public string Description => "Deletes a file.";

    public async Task<object?> ExecuteAsync(DeleteFileParameters parameters, CancellationToken cancellationToken)
    {
        ValidateDeleteParameters(parameters);
        await DeleteFile(parameters, cancellationToken);
        return await Task.FromResult<object?>(null);
    }

    private static void ValidateDeleteParameters(DeleteFileParameters parameters)
    {
        var path = PathHelper.ToUnixPath(parameters.Path);

        if (string.IsNullOrEmpty(path))
            throw new ArgumentException(Resources.TheSpecifiedPathMustBeNotEmpty);

        if (!PathHelper.IsFile(path))
            throw new ArgumentException(Resources.ThePathIsNotFile);

        if (!File.Exists(path))
            throw new ArgumentException(string.Format(Resources.TheSpecifiedPathIsNotExist, path));
    }

    private Task DeleteFile(DeleteFileParameters parameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var path = PathHelper.ToUnixPath(parameters.Path);
        File.Delete(path);
        _logger?.LogInfo(string.Format(Resources.TheSpecifiedPathWasDeleted, path));
        return Task.CompletedTask;
    }
}