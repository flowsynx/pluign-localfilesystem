using FlowSynx.Plugins.Local.Services;

namespace FlowSynx.Plugins.Local.UnitTests.Services;

public class GuidProviderTests
{
    [Fact]
    public void NewGuid_ReturnsValidGuid()
    {
        // Arrange
        var guidProvider = new GuidProvider();

        // Act
        var result = guidProvider.NewGuid();

        // Assert
        Assert.NotEqual(Guid.Empty, result);
    }

    [Fact]
    public void NewGuid_ReturnsDifferentGuids()
    {
        // Arrange
        var guidProvider = new GuidProvider();

        // Act
        var guid1 = guidProvider.NewGuid();
        var guid2 = guidProvider.NewGuid();

        // Assert
        Assert.NotEqual(guid1, guid2);
    }

    [Fact]
    public void NewGuid_CalledMultipleTimes_ReturnsUniqueGuids()
    {
        // Arrange
        var guidProvider = new GuidProvider();
        var guids = new HashSet<Guid>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            guids.Add(guidProvider.NewGuid());
        }

        // Assert
        Assert.Equal(100, guids.Count); // All GUIDs should be unique
    }
}