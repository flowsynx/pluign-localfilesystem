using FlowSynx.PluginCore;
using FlowSynx.Plugins.Local.Extensions;

namespace FlowSynx.Plugins.Local.Operations.Read;

internal class ReadOperation : IPluginOperation<ReadParameters, PluginContext>
{
    public string Name => "Read";
    public string Description => "Reads a file.";

    public async Task<PluginContext?> ExecuteAsync(ReadParameters parameters, CancellationToken cancellationToken)
    {
        ValidateReadParameters(parameters);
        return await ReadFile(parameters, cancellationToken);
    }

    private static void ValidateReadParameters(ReadParameters parameters)
    {
        var path = PathHelper.ToUnixPath(parameters.Path);

        if (string.IsNullOrEmpty(path))
            throw new ArgumentException(Resources.TheSpecifiedPathMustBeNotEmpty);

        if (!PathHelper.IsFile(path))
            throw new ArgumentException(Resources.ThePathIsNotFile);

        if (!File.Exists(path))
            throw new ArgumentException(string.Format(Resources.TheSpecifiedPathIsNotExist, path));
    }

    private static Task<PluginContext> ReadFile(ReadParameters parameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var path = PathHelper.ToUnixPath(parameters.Path);
        var fileInfo = new FileInfo(path);
        var entity = fileInfo.ToContext(true);

        return Task.FromResult(entity);
    }
}