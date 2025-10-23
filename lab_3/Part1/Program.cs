using DataAccessLevel.DataProviders;
using System.Text.Json;


namespace lab_3;

static class Program
{
    static void Main()
    {
        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };

        Product[] array = [
            new Product("P-001", "Ноутбук", "Dell", 35000.50m, 10),
            new Product("P-002", "Миша", "Logitech", 1200.00m, 150),
            new Product("P-003", "Клавіатура", "Razer", 4500.75m, 75),
            new Product("P-004", "Монітор", "Samsung", 12000.00m, 30),
            new Product("P-005", "Кабель USB-C", "Anker", 650.00m, 500)
        ];

        Product[]? deserialized; // 3. Створити новий масив для десеріалізації

        Console.WriteLine("=============\nJSON Серіалізація\n=============");
        File.WriteAllText("JsonProducts.json", JsonSerializer.Serialize(array, jsonOptions));
        deserialized = JsonSerializer.Deserialize<Product[]>(File.ReadAllText("JsonProducts.json"));
        
        if (deserialized != null)
        {
            foreach (var i in deserialized)
            {
                Console.WriteLine(i.ToString());
            }
        }
        else
        {
            Console.WriteLine("Не вдалося десеріалізувати.");
        }

        Console.WriteLine("=============\nXML Серіалізація\n=============");
        var xmlSerializer = new XmlProvider("XmlProducts.xml");
        xmlSerializer.SaveToFile(array);
        deserialized = xmlSerializer.LoadFromFile<Product>()?.ToArray();
        
        if (deserialized != null)
        {
            foreach (var i in deserialized)
            {
                Console.WriteLine(i.ToString());
            }
        }
        else
        {
            Console.WriteLine("Не вдалося десеріалізувати.");
        }

        Console.WriteLine("=============\nCustom (Власна) Серіалізація\n=============");
        var customSerializer = new CustomProvider("CustomProducts.txt");
        customSerializer.SaveToFile(array);
        deserialized = customSerializer.LoadFromFile<Product>()?.ToArray();
        
        if (deserialized != null)
        {
            foreach (var i in deserialized)
            {
                Console.WriteLine(i.ToString());
            }
        }
        else
        {
            Console.WriteLine("Не вдалося десеріалізувати.");
        }

        Console.WriteLine("=============\nBinary (Бінарна) Серіалізація\n=============");
        var binSerializer = new BinaryProvider("BinaryProducts.dat");
        binSerializer.SaveToFile(array);
        deserialized = binSerializer.LoadFromFile<Product>()?.ToArray();
        
        if (deserialized != null)
        {
            foreach (var i in deserialized)
            {
                Console.WriteLine(i.ToString());
            }
        }
        else
        {
            Console.WriteLine("Не вдалося десеріалізувати.");
        }

        Console.WriteLine("\n=============\nПорівняння: Серіалізація List<T>\n=============");
        List<Product> list = array.ToList();
        File.WriteAllText("ProductList.json", JsonSerializer.Serialize(list, jsonOptions));
        
        Console.WriteLine("Список List<Product> серіалізовано в ProductList.json.");
        Console.WriteLine("Формат JSON для List<T> та Array[] зазвичай ідентичний.");
    }
}