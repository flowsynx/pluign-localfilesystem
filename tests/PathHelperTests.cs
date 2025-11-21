using FlowSynx.Plugins.Local;

namespace FlowSynx.Plugins.Local.UnitTests;

public class PathHelperTests
{
    [Theory]
    [InlineData("/path/to/directory/", true)]
    [InlineData("/path/to/directory", false)]
    [InlineData("C:/folder/", true)]
    [InlineData("C:/folder", false)]
    [InlineData("/", true)]
    public void IsDirectory_WithVariousPaths_ReturnsExpectedResult(string path, bool expected)
    {
        // Act
        var result = PathHelper.IsDirectory(path);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("/path/to/file.txt", true)]
    [InlineData("/path/to/directory/", false)]
    [InlineData("C:/folder/file.doc", true)]
    [InlineData("C:/folder/", false)]
    public void IsFile_WithVariousPaths_ReturnsExpectedResult(string path, bool expected)
    {
        // Act
        var result = PathHelper.IsFile(path);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("C:\\path\\to\\file.txt", "C:/path/to/file.txt")]
    [InlineData("C:\\folder\\subfolder\\", "C:/folder/subfolder/")]
    [InlineData("/unix/path/file", "/unix/path/file")]
    [InlineData("mixed\\path/to\\file", "mixed/path/to/file")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void ToUnixPath_WithVariousPaths_ReturnsUnixStylePath(string? input, string expected)
    {
        // Act
        var result = PathHelper.ToUnixPath(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void PathSeparator_IsForwardSlash()
    {
        // Assert
        Assert.Equal('/', PathHelper.PathSeparator);
    }
}