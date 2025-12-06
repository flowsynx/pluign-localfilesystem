using FlowSynx.PluginCore;
using FlowSynx.PluginCore.Extensions;
using FlowSynx.Plugins.Local.Extensions;
using System.Text.RegularExpressions;

namespace FlowSynx.Plugins.Local.Operations.List;

internal class ListOperation : IPluginOperation<ListParameters, IEnumerable<PluginContext>>
{
    private readonly IPluginLogger? _logger;

    public ListOperation(IPluginLogger? logger)
    {
        // Allow null logger; SupportedOperations may be accessed before initialization.
        _logger = logger;
    }

    public string Name => "List";
    public string Description => "Lists files in a directory.";

    public async Task<IEnumerable<PluginContext>?> ExecuteAsync(ListParameters parameters, CancellationToken cancellationToken)
    {
        ValidateListParameters(parameters);
        return await ListEntities(parameters, cancellationToken);
    }

    private static void ValidateListParameters(ListParameters parameters)
    {
        var path = PathHelper.ToUnixPath(parameters.Path);
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException(Resources.TheSpecifiedPathMustBeNotEmpty);

        if (!PathHelper.IsDirectory(path))
            throw new ArgumentException(Resources.ThePathIsNotDirectory);

        if (!Directory.Exists(path))
            throw new ArgumentException(string.Format(Resources.TheSpecifiedPathIsNotExist, path));
    }

    private Task<IEnumerable<PluginContext>> ListEntities(
        ListParameters parameters,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var path = PathHelper.ToUnixPath(parameters.Path);
        var directoryInfo = new DirectoryInfo(path);

        var fileEntities = FindFiles(directoryInfo, _logger, parameters)
                           .Select(file => file.ToContext(parameters.IncludeMetadata))
                           .ToList();

        return Task.FromResult<IEnumerable<PluginContext>>(fileEntities);
    }

    private static IEnumerable<FileInfo> FindFiles(
        DirectoryInfo directoryInfo,
        IPluginLogger? logger,
        ListParameters listParameters)
    {
        ValidateDirectory(directoryInfo);

        var searchOption = listParameters.Recurse is true ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var regex = CreateRegex(listParameters);
        var files = directoryInfo.EnumerateFiles("*", searchOption);

        var result = new List<FileInfo>();
        int resultCount = 0;

        foreach (var file in files)
        {
            if (!ShouldIncludeFile(file, regex))
                continue;
            try
            {
                result.Add(file);
                resultCount++;

                // Stop once we reach the maxResults
                if (listParameters.MaxResults.HasValue && resultCount >= listParameters.MaxResults)
                    break;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.Message);
            }
        }

        return result;
    }
    private static void ValidateDirectory(DirectoryInfo directoryInfo)
    {
        if (directoryInfo == null)
            throw new ArgumentNullException(nameof(directoryInfo), Resources.TheDirectoryCouldNotBeNull);

        if (!directoryInfo.Exists)
            throw new DirectoryNotFoundException(string.Format(Resources.TheDirectoryDoesNotExist, directoryInfo.FullName));
    }

    private static Regex? CreateRegex(ListParameters listParameters)
    {
        if (string.IsNullOrEmpty(listParameters.Filter))
            return null;

        var regexOptions = listParameters.CaseSensitive == true ?
            RegexOptions.None :
            RegexOptions.IgnoreCase;

        var timeout = TimeSpan.FromSeconds(5);

        return new Regex(listParameters.Filter, regexOptions, timeout);
    }
    private static bool ShouldIncludeFile(FileInfo file, Regex? regex)
    {
        if (regex == null)
            return true;

        return regex.IsMatch(file.FullName);
    }
}