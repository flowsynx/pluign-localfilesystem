using FlowSynx.PluginCore;

namespace FlowSynx.Plugins.Local.Operations.Exist;

internal class ExistOperation : IPluginOperation<ExistParameters, bool>
{
    public string Name => "Exist";
    public string Description => "Checks if a file or directory exists.";

    public async Task<bool> ExecuteAsync(ExistParameters parameters, CancellationToken cancellationToken)
    {
        ValidateExistParameters(parameters);
        return await CheckExistence(parameters, cancellationToken);
    }

    private static void ValidateExistParameters(ExistParameters parameters)
    {
        var path = PathHelper.ToUnixPath(parameters.Path);
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException(Resources.TheSpecifiedPathMustBeNotEmpty);
    }

    private static Task<bool> CheckExistence(ExistParameters parameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var path = PathHelper.ToUnixPath(parameters.Path);
        var isExist = PathHelper.IsDirectory(path) ? Directory.Exists(path) : File.Exists(path);

        return Task.FromResult(isExist);
    }
}