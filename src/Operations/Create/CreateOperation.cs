using FlowSynx.PluginCore;

namespace FlowSynx.Plugins.Local.Operations.Create;

internal class CreateOperation : IPluginOperation<CreateParameters, object>
{
    public string Name => "Create";
    public string Description => "Creates a new directory.";

    public async Task<object?> ExecuteAsync(CreateParameters parameters, CancellationToken cancellationToken)
    {
        ValidateCreateParameters(parameters);
        CreateDirectory(parameters, cancellationToken);
        return await Task.FromResult<object?>(null);
    }

    private static void ValidateCreateParameters(CreateParameters parameters)
    {
        var path = PathHelper.ToUnixPath(parameters.Path);

        if (string.IsNullOrEmpty(path))
            throw new ArgumentException(Resources.TheSpecifiedPathMustBeNotEmpty);

        if (!PathHelper.IsDirectory(path))
            throw new ArgumentException(Resources.ThePathIsNotDirectory);
    }

    private static void CreateDirectory(CreateParameters parameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var path = PathHelper.ToUnixPath(parameters.Path);

        var directory = Directory.CreateDirectory(path);

        if (parameters.Hidden == true)
            directory.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
    }
}