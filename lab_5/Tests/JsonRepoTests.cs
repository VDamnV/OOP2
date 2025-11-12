using lab_5.DataAccessLevel.FileRepositories;
using Xunit;

namespace lab_5.Tests;

public class JsonRepoTests
{
    [Fact]
    public void SaveAndLoad_Works()
    {
        var tempFile = Path.GetTempFileName();
        var repo = new JsonRepository(tempFile);
        ICollection<int> data = [1, 2, 3];

        repo.SaveToFile(data);
        ICollection<int>? loaded = repo.GetFromFile<int>();
        File.Delete(tempFile);

        Assert.Equal(data, loaded);
    }
    
    [Fact]
    public void GetFromFile_ReturnsNull()
    {
        var repo = new JsonRepository("con");
        
        var data = repo.GetFromFile<int>();

        Assert.Null(data);
    }
}