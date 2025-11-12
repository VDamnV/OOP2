namespace lab_5.DataAccessLevel.FileRepositories;

public interface IRepository
{
    string FilePath { get; set; }
    void SaveToFile<T>(ICollection<T> objects);
    ICollection<T>? GetFromFile<T>();
}