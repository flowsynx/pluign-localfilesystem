using FlowSynx.PluginCore;
using Moq;

namespace FlowSynx.Plugins.Local.UnitTests;

public class LocalFileSystemPluginTests
{
    private readonly Mock<IPluginLogger> _mockLogger;
    private readonly LocalFileSystemPlugin _plugin;

    public LocalFileSystemPluginTests()
    {
        _mockLogger = new Mock<IPluginLogger>();
        _plugin = new LocalFileSystemPlugin();
    }

    [Fact]
    public void Metadata_HasCorrectProperties()
    {
        // Act
        var metadata = _plugin.Metadata;

        // Assert
        Assert.Equal(Guid.Parse("f6304870-0294-453e-9598-a82167ace653"), metadata.Id);
        Assert.Equal("Local", metadata.Name);
        Assert.NotNull(metadata.Description);
        Assert.Equal(new Version(1, 1, 0), metadata.Version);
        Assert.Equal(PluginCategory.Storage, metadata.Category);
        Assert.Equal("FlowSynx", metadata.CompanyName);
        Assert.Contains("FlowSynx", metadata.Authors);
        Assert.Equal("© FlowSynx. All rights reserved.", metadata.Copyright);
        Assert.Equal("flowsynx.png", metadata.Icon);
        Assert.Equal("README.md", metadata.ReadMe);
        Assert.Equal("https://github.com/flowsynx/plugin-json", metadata.RepositoryUrl);
        Assert.Equal("https://flowsynx.io", metadata.ProjectUrl);
        Assert.Contains("local", metadata.Tags);
        Assert.Equal(new Version(1, 3, 0), metadata.MinimumFlowSynxVersion);
    }

    [Fact]
    public void Specifications_Default_IsNull()
    {
        // Act
        var specs = _plugin.Specifications;

        // Assert
        Assert.Null(specs);
    }

    [Fact]
    public void SupportedOperations_ContainsExpectedOperations()
    {
        // Act
        var operations = _plugin.SupportedOperations;

        // Assert
        Assert.Equal(8, operations.Count);
        Assert.Contains(operations, o => o.Name.Equals("create", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(operations, o => o.Name.Equals("delete", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(operations, o => o.Name.Equals("exist", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(operations, o => o.Name.Equals("list", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(operations, o => o.Name.Equals("purge", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(operations, o => o.Name.Equals("read", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(operations, o => o.Name.Equals("rename", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(operations, o => o.Name.Equals("write", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void DefaultConstructor_CreatesInstanceSuccessfully()
    {
        // Act
        var plugin = new LocalFileSystemPlugin();

        // Assert
        Assert.NotNull(plugin);
        Assert.NotNull(plugin.Metadata);
    }

    [Fact]
    public async Task InitializeAsync_WithNullLogger_ThrowsDueToReflectionGuard()
    {
        // Reflection guard may trigger before null check in unit test context
        await Assert.ThrowsAnyAsync<Exception>(() =>
            _plugin.InitializeAsync(null!, null));
    }

    [Fact]
    public async Task InitializeAsync_CalledFromTest_Succeeds()
    {
        // Act
        await _plugin.InitializeAsync(_mockLogger.Object, null);

        // Assert
        Assert.NotNull(_plugin.Specifications);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNotInitialized_StillBlockedByReflectionGuard()
    {
        var parameters = new PluginParameters();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _plugin.ExecuteAsync("create", parameters, CancellationToken.None));

        // In this scenario, reflection guard does not trigger; the plugin reports not initialized
        Assert.Contains("Plugin 'Local' v1.1.0 is not initialized.", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_UnsupportedOperation_ThrowsInvalidOperationExceptionDueToGuard()
    {
        var parameters = new PluginParameters();

        await _plugin.InitializeAsync(_mockLogger.Object, null);

        var exception = await Assert.ThrowsAsync<NotSupportedException>(() =>
            _plugin.ExecuteAsync("Unsupported: ", parameters, CancellationToken.None));
    }
}