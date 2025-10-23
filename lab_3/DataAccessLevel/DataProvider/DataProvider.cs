using DataAccessLevel; // Додано для доступу до DataAccessException

namespace DataAccessLevel.DataProviders;

public abstract class DataProvider(string filePath)
{
    public bool IsLocked { get; protected set; } = false;
    public string FilePath { get; private set; } = filePath;

    protected abstract void saveToFileLogic<T>(ICollection<T> objects) where T : new();

    protected abstract ICollection<T>? loadFromFileLogic<T>() where T : new();

    public void SaveToFile<T>(ICollection<T> objcets) where T : new()
    {
        if (this.IsLocked) throw new DataAccessException("Файл заблоковано. Операція неможлива.");
        this.IsLocked = true;
        try
        {
            this.saveToFileLogic(objcets);
        }
        finally // Гарантуємо, що файл буде розблоковано, навіть якщо станеться помилка
        {
            this.IsLocked = false;
        }
    }

    public ICollection<T>? LoadFromFile<T>() where T : new()
    {
        if (this.IsLocked) throw new DataAccessException("Файл заблоковано. Операція неможлива.");
        this.IsLocked = true;
        try
        {
            ICollection<T>? objects = this.loadFromFileLogic<T>();
            return objects;
        }
        finally
        {
            this.IsLocked = false;
        }
    }

    public bool SetPath(string newPath)
    {
        if (this.IsLocked) return false;
        if (string.IsNullOrWhiteSpace(newPath)) return false;
        this.FilePath = newPath;
        return true;
    }
}