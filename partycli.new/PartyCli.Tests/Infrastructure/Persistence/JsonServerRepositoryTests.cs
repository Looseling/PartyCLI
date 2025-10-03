using Moq;
using PartyCli.Domain.Models;
using PartyCli.Infrastructure.Persistence;
using Shouldly;

namespace PartyCli.Tests.Infrastructure.Persistence;

public class JsonServerRepositoryTests : IDisposable
{
    private readonly string _testFilePath;
    private readonly JsonServerRepository _repository;
    
    public JsonServerRepositoryTests()
    {
        _testFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        
        var mockPathProvider = new Mock<IFilePathProvider>();
        mockPathProvider.Setup(p => p.GetServerFilePath())
            .Returns(_testFilePath);
        
        _repository = new JsonServerRepository(mockPathProvider.Object);
    }

    [Fact]
    public async Task SaveServersAsync_SavesServersToFile()
    {
        // Arrange
        var servers = new List<Server>
        {
            new("us1.test.com", 50, "online"),
            new("us2.test.com", 30, "online")
        };

        // Act
        await _repository.SaveServersAsync(servers);

        // Assert
        File.Exists(_testFilePath).ShouldBeTrue();
        var savedServers = await _repository.GetServersAsync();
        savedServers.Count.ShouldBe(2);
        savedServers.First().Name.ShouldBe("us1.test.com");
    }

    [Fact]
    public async Task GetServersAsync_WhenFileDoesNotExist_ReturnsEmptyList()
    {
        // Act
        var servers = await _repository.GetServersAsync();

        // Assert
        servers.ShouldBeEmpty();
    }

    [Fact]
    public async Task HasServersAsync_WhenFileExists_ReturnsTrue()
    {
        // Arrange
        var servers = new List<Server>
        {
            new("test.com", 50, "online")
        };
        await _repository.SaveServersAsync(servers);

        // Act
        var hasServers = await _repository.HasServersAsync();

        // Assert
        hasServers.ShouldBeTrue();
    }

    [Fact]
    public async Task HasServersAsync_WhenFileDoesNotExist_ReturnsFalse()
    {
        // Act
        var hasServers = await _repository.HasServersAsync();

        // Assert
        hasServers.ShouldBeFalse();
    }

    public void Dispose()
    {
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }
}