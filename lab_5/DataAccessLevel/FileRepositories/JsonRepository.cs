using Newtonsoft.Json;

namespace lab_5.DataAccessLevel.FileRepositories;

public class JsonRepository(string filePath) : IRepository
{
    private readonly JsonSerializerSettings settings = new()
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.Auto
    };

    public string FilePath { get; set; } = filePath;
    
    public void SaveToFile<T>(ICollection<T> objects)
    {
        File.WriteAllText(this.FilePath, JsonConvert.SerializeObject(objects, this.settings));
    }
    
    public ICollection<T>? GetFromFile<T>()
    {
        if (!File.Exists(this.FilePath)) return null;
        return JsonConvert.DeserializeObject<ICollection<T>>(File.ReadAllText(this.FilePath), this.settings);
    }
}