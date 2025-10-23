using DataAccessLevel.DataProviders;using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace lab_3;


public class Product
{

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;

    public decimal Price { get; set; } = 0;

    public int QuantityInBatch { get; set; } = 0;

    [JsonConstructor]
    public Product() { }

    public Product(string code, string name, string manufacturer, decimal price, int quantity)
    {
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
        if (percentage < 0) return;
        
        this.Price *= (decimal)(1.0 + percentage / 100.0);
    }

    public override string ToString()
    {
        return $"Товар: {this.Name} (Код: {this.Code}), Виробник: {this.Manufacturer}, Ціна: {this.Price:C}, Кількість: {this.QuantityInBatch}, Вартість партії: {this.TotalBatchCost:C}";
    }
}