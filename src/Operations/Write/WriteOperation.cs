using FlowSynx.PluginCore;
using FlowSynx.PluginCore.Extensions;
using System.Text;

namespace FlowSynx.Plugins.Local.Operations.Write;

internal class WriteOperation : IPluginOperation<WriteParameters, object>
{
    private readonly IPluginLogger? _logger;

    public WriteOperation(IPluginLogger? logger)
    {
        // Allow null logger; SupportedOperations may be accessed before initialization.
        _logger = logger;
    }

    public string Name => "Write";
    public string Description => "Writes data to a file.";

    public async Task<object?> ExecuteAsync(WriteParameters parameters, CancellationToken cancellationToken)
    {
        ValidateWriteParameters(parameters);
        WriteEntity(parameters, cancellationToken);
        return await Task.FromResult<object?>(null);
    }

    private static void ValidateWriteParameters(WriteParameters parameters)
    {
        var path = PathHelper.ToUnixPath(parameters.Path);

        if (string.IsNullOrEmpty(path))
            throw new ArgumentException(Resources.TheSpecifiedPathMustBeNotEmpty);
    }

    private void WriteEntity(
        WriteParameters writeParameters,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var path = PathHelper.ToUnixPath(writeParameters.Path);
        var dataValue = writeParameters.Data;
        var pluginContextes = new List<PluginContext>();

        if (dataValue is PluginContext pluginContext)
        {
            if (!PathHelper.IsFile(path))
                throw new ArgumentException(Resources.ThePathIsNotFile);

            pluginContextes.Add(pluginContext);
        }
        else if (dataValue is IEnumerable<PluginContext> pluginContextDataList)
        {
            if (!PathHelper.IsDirectory(path))
                throw new ArgumentException(Resources.ThePathIsNotDirectory);

            pluginContextes.AddRange(pluginContextDataList);
        }
        else if (dataValue is string data)
        {
            if (!PathHelper.IsFile(path))
                throw new ArgumentException(Resources.ThePathIsNotFile);

            var contextData = CreateContextDataFromStringData(path, data);
            pluginContextes.Add(contextData);
        }
        else
        {
            throw new NotSupportedException(Resources.EnteredDataIsNotSupported);
        }

        foreach (var contextData in pluginContextes)
        {
            WriteEntityFromContextData(path, contextData, writeParameters.Overwrite, cancellationToken);
        }
    }

    private static PluginContext CreateContextDataFromStringData(
        string path,
        string data)
    {
        var root = Path.GetPathRoot(path) ?? string.Empty;
        var relativePath = Path.GetRelativePath(root, path);
        var dataBytesArray = IsBase64String(data) ? Base64ToByteArray(data) : ToByteArray(data);

        return new PluginContext(relativePath, "File")
        {
            RawData = dataBytesArray,
        };
    }

    private void WriteEntityFromContextData(
        string path,
        PluginContext pluginContext,
        bool overwrite,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        byte[] dataToWrite;

        if (pluginContext.RawData is not null)
            dataToWrite = pluginContext.RawData;
        else if (pluginContext.Content is not null)
            dataToWrite = Encoding.UTF8.GetBytes(pluginContext.Content);
        else
            throw new InvalidDataException(string.Format(Resources.TheEnteredDataIsInvalid, pluginContext.Id));

        var rootPath = Path.GetPathRoot(pluginContext.Id);
        var relativePath = pluginContext.Id;

        if (!string.IsNullOrEmpty(rootPath))
            relativePath = Path.GetRelativePath(rootPath, pluginContext.Id);

        var fullPath = PathHelper.IsDirectory(path) ? Path.Combine(path, relativePath) : path;

        var parentDirectory = Path.GetDirectoryName(fullPath);
        if (parentDirectory != null)
            Directory.CreateDirectory(parentDirectory);

        if (!PathHelper.IsFile(fullPath))
            throw new ArgumentException(Resources.ThePathIsNotFile);

        if (File.Exists(fullPath) && !overwrite)
            throw new ArgumentException(string.Format(Resources.FileIsAlreadyExistAndCannotBeOverwritten, fullPath));

        if (File.Exists(fullPath))
        {
            if (!overwrite)
                throw new ArgumentException(string.Format(Resources.FileIsAlreadyExistAndCannotBeOverwritten, fullPath));
            else
                DeleteEntity(fullPath, cancellationToken);
        }

        File.WriteAllBytes(fullPath, dataToWrite);
    }

    private void DeleteEntity(
        string path,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        path = PathHelper.ToUnixPath(path);
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException(Resources.TheSpecifiedPathMustBeNotEmpty);

        if (!PathHelper.IsFile(path))
            throw new ArgumentException(Resources.ThePathIsNotFile);

        if (!File.Exists(path))
            throw new ArgumentException(string.Format(Resources.TheSpecifiedPathIsNotExist, path));

        File.Delete(path);
        _logger?.LogInfo(string.Format(Resources.TheSpecifiedPathWasDeleted, path));
    }

    private static bool IsBase64String(string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length % 4 != 0
                                        || value.Contains(' ') || value.Contains('\t')
                                        || value.Contains('\r') || value.Contains('\n'))
            return false;

        var index = value.Length - 1;

        if (value[index] == '=')
            index--;

        if (value[index] == '=')
            index--;

        for (var i = 0; i <= index; i++)
            if (IsInvalid(value[i]))
                return false;

        return true;
    }

    private static bool IsInvalid(char value)
    {
        var intValue = (int)value;
        switch (intValue)
        {
            case >= 48 and <= 57:
            case >= 65 and <= 90:
            case >= 97 and <= 122:
                return false;
            default:
                return intValue != 43 && intValue != 47;
        }
    }

    private static byte[] Base64ToByteArray(string value)
    {
        return Convert.FromBase64String(value);
    }

    private static byte[] ToByteArray(string value)
    {
        return Encoding.UTF8.GetBytes(value);
    }
}