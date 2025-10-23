using BusinessLogicLevel;
using BusinessLogicLevel.DTOs;
using System.Globalization;

namespace PresentationLevel;

public class Menu
{
    private ProductService? productService;
    public bool IsOpen { get; private set; } = false;

    public void InitMenu()
    {
        this.IsOpen = true;
        Console.WriteLine(@"Вітаємо! Будь ласка, оберіть дію:
1. Почати роботу
2. Вихід");
        while (this.IsOpen)
        {
            string? input = Console.ReadLine();
            if (input == "1")
            {
                Console.WriteLine(@"Оберіть тип серіалізатора, або введіть повне ім'я типу DataProvider:
1. Json (Рекомендовано)
2. Xml
3. Binary (Власний бінарний формат)
4. Custom (Власний текстовий формат)");
                input = Console.ReadLine();
                if (input == "1") this.productService = new ProductService(DataProviders.JsonProvider, "");
                else if (input == "2") this.productService = new ProductService(DataProviders.XmlProvider, "");
                else if (input == "3") this.productService = new ProductService(DataProviders.BinaryProvider, "");
                else if (input == "4") this.productService = new ProductService(DataProviders.CustomProvider, "");
                else
                {
                    try
                    {
                        this.productService = new ProductService(input ?? "", "");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Помилка ініціалізації провайдера: {ex.Message}");
                        Console.WriteLine("Будь ласка, спробуйте ще раз.");
                        continue;
                    }
                }
                
                Console.Write("Введіть повний шлях до файлу (напр., C:\\data\\products.json): ");
                while (true)
                {
                    input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input)) break;
                    Console.WriteLine("Невірний ввід! Шлях не може бути порожнім.");
                }
                this.productService.FilePath = input;
                break;
            }
            else if (input == "2")
            {
                this.IsOpen = false;
            }
            else
            {
                Console.WriteLine("Невірний ввід!");
            }
        }
    }

    public void MainMenu()
    {
        this.IsOpen = true;
        if (this.productService == null) this.InitMenu();

        CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

        while (this.IsOpen)
        {
            Console.WriteLine(@"========================
Головне меню:
1.  Додати товар
2.  Видалити товар
3.  Переглянути всі товари
4.  Повна інформація про товар (за індексом)
5.  Зберегти у файл
6.  Завантажити з файлу
7.  Очистити список
8.  Змінити DataProvider (тип або шлях)
9.  Розрахувати загальну вартість складу
10. Збільшити ціну товару (за індексом)
11. Вихід");
            string? input = Console.ReadLine();
            if (input == "1") addProduct();
            else if (input == "2") removeProduct();
            else if (input == "3") viewAll();
            else if (input == "4") viewFullInfoAboutProduct();
            else if (input == "5") saveToFile();
            else if (input == "6") loadFromFile();
            else if (input == "7") clearList();
            else if (input == "8") editProvider();
            else if (input == "9") calculateTotalStockValue();
            else if (input == "10") increaseProductPrice();
            else if (input == "11") exit();
            else Console.WriteLine("Невірний ввід!");
        }
    }

    private void addProduct()
    {
        try
        {
            Console.WriteLine("--- Додавання нового товару ---");
            Console.Write("Введіть код товару (артикул): ");
            string code = Console.ReadLine() ?? "";
            
            Console.Write("Введіть назву: ");
            string name = Console.ReadLine() ?? "";
            
            Console.Write("Введіть виробника: ");
            string manufacturer = Console.ReadLine() ?? "";
            
            Console.Write("Введіть ціну (напр., 35000.50): ");
            decimal price = decimal.Parse(Console.ReadLine() ?? "0");
            
            Console.Write("Введіть кількість у партії: ");
            int quantity = int.Parse(Console.ReadLine() ?? "0");

            this.productService!.Add(new ProductDTO()
            {
                Code = code,
                Name = name,
                Manufacturer = manufacturer,
                Price = price,
                QuantityInBatch = quantity
            }.ToEntity());
            
            Console.WriteLine(">>> Товар успішно додано!");
        }
        catch (Exception e)
        {
            Console.WriteLine($"!!! Сталася помилка при додаванні товару: {e.Message}");
            return;
        }
    }

    private void removeProduct()
    {
        if (this.productService!.ProductCount == 0)
        {
            Console.WriteLine("!!! Список товарів порожній, нічого видаляти!");
            return;
        }
        Console.WriteLine("Оберіть номер товару, який бажаєте видалити:");
        Console.WriteLine(this.productService.GetAllProductsInfo());
        
        if (int.TryParse(Console.ReadLine(), out int i) && --i > -1 && i < this.productService.ProductCount)
        {
            this.productService.Remove(i);
            Console.WriteLine(">>> Товар успішно видалено!");
        }
        else
        {
            Console.WriteLine("!!! Невірний ввід або індекс.");
        }
    }

    private void viewAll()
    {
        if (this.productService!.ProductCount == 0) Console.WriteLine("--- Список товарів порожній ---");
        else Console.WriteLine(this.productService.GetAllProductsInfo());
    }

    private void viewFullInfoAboutProduct()
    {
        if (this.productService!.ProductCount == 0)
        {
            Console.WriteLine("!!! Список товарів порожній!");
            return;
        }
        Console.WriteLine("Оберіть номер товару для перегляду повної інформації:");
        Console.WriteLine(this.productService.GetAllProductsInfo());
        
        if (int.TryParse(Console.ReadLine(), out int i))
        {
            var p = this.productService.SearchByIndex(--i);
            if (p != null)
            {
                Console.WriteLine("--- Повна інформація ---");
                Console.WriteLine(this.productService.GetProductFullInfo(p));
            }
            else
            {
                Console.WriteLine("!!! Індекс за межами діапазону.");
            }
        }
        else
        {
            Console.WriteLine("!!! Невірний ввід.");
        }
    }

    private void saveToFile()
    {
        string? input;
        if (this.productService!.ProductCount == 0)
        {
            Console.WriteLine("!!! Список порожній. Ця дія !ОЧИСТИТЬ! вміст файлу! Ви впевнені? (так/ні)");
            input = Console.ReadLine()?.ToLower();
            if (input == "так" || input == "y" || input == "yes")
            {
                var result = this.productService.SaveToFile();
                if (result.IsSuccess) Console.WriteLine(">>> Файл успішно очищено!");
                else Console.WriteLine($"!!! Помилка при очищенні файлу: {result.Message}");
            }
            else
            {
                Console.WriteLine("--- Дію скасовано! ---");
            }
            return;
        }

        Console.WriteLine($"Ви бажаєте перезаписати файл {this.productService.ProductCount} товарами? (так/ні)");
        input = Console.ReadLine()?.ToLower();
        if (input == "так" || input == "y" || input == "yes")
        {
            var result = this.productService.SaveToFile();
            if (result.IsSuccess) Console.WriteLine(">>> Файл успішно перезаписано!");
            else Console.WriteLine($"!!! Помилка при перезаписі файлу: {result.Message}");
        }
        else
        {
            Console.WriteLine("--- Дію скасовано ---");
        }
    }

    private void loadFromFile()
    {
        var result = this.productService!.LoadFromFile();
        if (result.IsSuccess) Console.WriteLine($">>> Успішно завантажено {this.productService.ProductCount} товар(ів).");
        else Console.WriteLine($"!!! Помилка при завантаженні з файлу: {result.Message}");
    }

    private void clearList()
    {
        Console.WriteLine("Ви впевнені, що бажаєте очистити список у пам'яті (це не вплине на файл до збереження)? (так/ні)");
        string? input = Console.ReadLine()?.ToLower();
        if (input == "так" || input == "y" || input == "yes")
        {
            this.productService!.Clear();
            Console.WriteLine(">>> Список успішно очищено!");
        }
        else
        {
            Console.WriteLine("--- Дію скасовано! ---");
        }
    }

    private void editProvider()
    {
        Console.WriteLine("Що ви бажаєте змінити? (тип/шлях)");
        string? input = Console.ReadLine()?.ToLower();
        if (input == "тип")
        {
            Console.WriteLine(@"Оберіть тип серіалізатора, або введіть повне ім'я типу DataProvider:
1. Json
2. Xml
3. Binary
4. Custom");
            input = Console.ReadLine();
            try
            {
                if (input == "1") this.productService!.ChangeProviderType(DataProviders.JsonProvider);
                else if (input == "2") this.productService!.ChangeProviderType(DataProviders.XmlProvider);
                else if (input == "3") this.productService!.ChangeProviderType(DataProviders.BinaryProvider);
                else if (input == "4") this.productService!.ChangeProviderType(DataProviders.CustomProvider);
                else
                {
                    this.productService!.ChangeProviderType(input ?? "");
                }
                Console.WriteLine(">>> Тип провайдера успішно змінено.");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"!!! Невірний ввід або помилка зміни типу: {ex.Message}");
            }
        }
        else if (input == "шлях")
        {
            Console.Write("Введіть повний шлях до файлу: ");
            while (true)
            {
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    this.productService!.FilePath = input;
                    Console.WriteLine(">>> Шлях до файлу успішно змінено.");
                    break;
                }
                Console.WriteLine("!!! Невірний ввід! Спробуйте ще раз.");
            }
        }
        else
        {
            Console.WriteLine("!!! Невірний ввід (очікувалося 'тип' або 'шлях').");
        }
    }

    private void calculateTotalStockValue()
    {
        decimal totalValue = this.productService!.GetTotalStockValue();
        Console.WriteLine($"--- Загальна вартість складу (усіх партій): {totalValue:C} ---");
    }

    private void increaseProductPrice()
    {
        if (this.productService!.ProductCount == 0)
        {
            Console.WriteLine("!!! Список товарів порожній!");
            return;
        }

        Console.WriteLine("Оберіть номер товару для збільшення ціни:");
        Console.WriteLine(this.productService.GetAllProductsInfo());
        
        try
        {
            Console.Write("Введіть номер товару: ");
            int index = int.Parse(Console.ReadLine() ?? "-1");

            Console.Write("Введіть відсоток, на який треба збільшити ціну (напр., 10.5): ");
            double percentage = double.Parse(Console.ReadLine() ?? "0", CultureInfo.InvariantCulture);

            if (this.productService.IncreaseProductPrice(index - 1, percentage)) // -1, бо користувач вводить 1-based індекс
            {
                Console.WriteLine(">>> Ціну товару успішно оновлено.");
            }
            else
            {
                Console.WriteLine("!!! Помилка: Невірний індекс товару.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"!!! Помилка вводу: {ex.Message}");
        }
    }

    private void exit() => this.IsOpen = false;
}

static class Program
{
    static void Main()
    {
                new Menu().MainMenu();
    }
}