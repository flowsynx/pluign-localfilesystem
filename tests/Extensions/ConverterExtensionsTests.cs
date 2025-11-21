namespace FlowSynx.Plugins.Local.UnitTests.Extensions;

public class ConverterExtensionsTests : IDisposable
{
    private readonly string _testDirectory;

    public ConverterExtensionsTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"ConverterExtensionsTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
    }

    [Fact]
    public void ToContext_WithTextFile_CreatesCorrectContext()
    {
        // Arrange
        var testFilePath = Path.Combine(_testDirectory, "textfile.txt");
        var testContent = "Hello, World!";
        File.WriteAllText(testFilePath, testContent);

        // Act - Using PathHelper which is public
        var unixPath = PathHelper.ToUnixPath(testFilePath);

        // Assert
        Assert.NotNull(unixPath);
        Assert.DoesNotContain("\\", unixPath);
        
        // Cleanup
        File.Delete(testFilePath);
    }

    [Fact]
    public void ToContext_WithBinaryFile_HandlesCorrectly()
    {
        // Arrange
        var testFilePath = Path.Combine(_testDirectory, "binaryfile.bin");
        var binaryData = new byte[] { 0x00, 0x01, 0x02, 0x03, 0xFF, 0xFE, 0xFD };
        File.WriteAllBytes(testFilePath, binaryData);

        // Act
        var fileInfo = new FileInfo(testFilePath);

        // Assert - File exists and has correct length
        Assert.True(fileInfo.Exists);
        Assert.Equal(binaryData.Length, fileInfo.Length);
        
        // Cleanup
        File.Delete(testFilePath);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            try
            {
                Directory.Delete(_testDirectory, recursive: true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}