using DataAccessLevel.DataProviders;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DataAccessLevel.Model;

public class Product
{

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;

    public decimal Price { get; set; } = 0;

    public int QuantityInBatch { get; set; } = 0;

    public Product() { }

    public Product(string code, string name, string manufacturer, decimal price, int quantity)
    {
        if (string.IsNullOrWhiteSpace(code)) 
            throw new ArgumentException("Код товару не може бути порожнім.", nameof(code));
        if (string.IsNullOrWhiteSpace(name)) 
            throw new ArgumentException("Назва товару не може бути порожньою.", nameof(name));
        if (price < 0) 
            throw new ArgumentException("Ціна не може бути від'ємною.", nameof(price));
        if (quantity < 0) 
            throw new ArgumentException("Кількість у партії не може бути від'ємною.", nameof(quantity));

        this.Code = code;
        this.Name = name;
        this.Manufacturer = manufacturer;
        this.Price = price;
        this.QuantityInBatch = quantity;
    }

    [JsonIgnore]
    [XmlIgnore]
    [CustomProviderIgnore]
    public decimal TotalBatchCost => this.Price * this.QuantityInBatch;

    public void IncreasePrice(double percentage)
    {
        if (percentage < 0)
        {
            throw new ArgumentException("Відсоток для збільшення ціни не може бути від'ємним.");
        }
        
        this.Price *= (decimal)(1.0 + percentage / 100.0);
    }

    public override string ToString()
    {
        return $"Товар: {this.Name} (Код: {this.Code}), Виробник: {this.Manufacturer}, Ціна: {this.Price:C}, Кількість у партії: {this.QuantityInBatch}";
    }
}