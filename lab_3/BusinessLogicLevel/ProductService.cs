using DataAccessLevel.Model;
using DataAccessLevel.DataProviders;
using System.Text;

namespace BusinessLogicLevel;

public enum DataProviders
{
    BinaryProvider,
    CustomProvider,
    JsonProvider,
    XmlProvider
}

public class ProductService
{
    private List<Product> products = new();
    private DataProvider dataProvider;

    public string FilePath
    {
        get => this.dataProvider.FilePath;
        set => this.dataProvider.SetPath(value);
    }

    public int ProductCount => this.products.Count;

    public ProductService(DataProvider provider)
    {
        this.dataProvider = provider;
    }

    public ProductService(DataProviders providerType, string filePath)
    {
        this.dataProvider = createProvider(providerType, filePath);
    }

    public ProductService(string providerTypeFullName, string filePath)
    {
        this.dataProvider = createProvider(providerTypeFullName, filePath);
    }

    public void ChangeProviderType(DataProvider newProvider)
    {
        this.dataProvider = newProvider;
    }

    public void ChangeProviderType(DataProviders newType)
    {
        this.dataProvider = createProvider(newType, this.dataProvider.FilePath);
    }

    public void ChangeProviderType(string providerTypeFullName)
    {
        this.dataProvider = createProvider(providerTypeFullName, this.dataProvider.FilePath);
    }

    public void Add(Product product) => this.products.Add(product);
    public bool Remove(int index)
    {
        if (index < 0 || index >= this.products.Count) return false;
        this.products.RemoveAt(index);
        return true;
    }

    public string GetAllProductsInfo()
    {
        var result = new StringBuilder();
        if (this.products.Count == 0)
        {
            return "Список товарів порожній.";
        }
        
        for (int i = 0; i < this.products.Count; i++)
        {
            result.Append($"{i + 1}. {this.products[i]}\n");
        }
        return result.ToString();
    }

    public string GetProductFullInfo(Product product)
    {
        var result = new StringBuilder();
        foreach (var property in product.GetType().GetProperties())
        {
            result.Append($"{property.Name}: {property.GetValue(product)}\n");
        }
        return result.ToString();
    }

    public FileAccessResult SaveToFile()
    {
        if (this.dataProvider.IsLocked) return new FileAccessResult(false, "Файл заблоковано.");
        try
        {
            this.dataProvider.SaveToFile(this.products);
            return new FileAccessResult();
        }
        catch (Exception e)
        {
            return new FileAccessResult(false, e.Message);
        }
    }

    public Product? SearchByName(string name)
    {
        foreach (var p in this.products)
        {
            if (p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return p;
            }
        }
        return null;
    }


    public Product? SearchByIndex(int index)
    {
        if (index < 0 || index >= this.products.Count) return null;
        return this.products[index];
    }

    public void Clear() => this.products.Clear();

    public FileAccessResult LoadFromFile()
    {
        if (this.dataProvider.IsLocked) return new FileAccessResult(false, "Файл заблоковано.");
        try
        {
            ICollection<Product>? loaded = this.dataProvider.LoadFromFile<Product>();
            if (loaded == null) return new FileAccessResult(false, "Провайдер даних повернув null.");
            this.products = loaded.ToList();
            return new FileAccessResult();
        }
        catch (Exception e)
        {
            return new FileAccessResult(false, $"Сталася помилка при завантаженні з файлу: {e.Message}");
        }
    }


    public decimal GetTotalStockValue()
    {
        decimal totalValue = 0;
        foreach (var p in this.products)
        {
            totalValue += p.TotalBatchCost;
        }
        return totalValue;
    }

    public bool IncreaseProductPrice(int index, double percentage)
    {
        Product? product = SearchByIndex(index);
        if (product == null)
        {
            return false;
        }
        
        product.IncreasePrice(percentage);
        return true;
    }

    private static DataProvider createProvider(string providerTypeFullName, string filePath)
    {
        Type type = Type.GetType(providerTypeFullName) ?? throw new TypeLoadException($"Недійсна назва типу '{providerTypeFullName}'");
        if (!typeof(DataProvider).IsAssignableFrom(type)) throw new TypeLoadException($"Тип не є DataProvider");
        return (DataProvider)Activator.CreateInstance(type, [filePath])!;
    }

    private static DataProvider createProvider(DataProviders providerType, string filePath)
    {
        return providerType switch
        {
            DataProviders.BinaryProvider => new BinaryProvider(filePath),
            DataProviders.CustomProvider => new CustomProvider(filePath),
            DataProviders.JsonProvider => new JsonProvider(filePath),
            DataProviders.XmlProvider => new XmlProvider(filePath),
            _ => new JsonProvider(filePath),
        };
    }
}