namespace lab2;

public class Product : IComparable<Product>
{
    public int Code { get; set; }
    public string Name { get; set; } = "";
    public string Manufacturer { get; set; } = "";
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public Product(int code, string name, string manufacturer, decimal price, int quantity)
    {
        Code = code;
        Name = name;
        Manufacturer = manufacturer;
        Price = price;
        Quantity = quantity;
    }

    // Загальна вартість партії
    public decimal TotalCost => Price * Quantity;

    // Збільшення вартості на певний процент
    public void IncreasePrice(decimal percent)
    {
        if (percent < 0) return;
        Price += Price * percent / 100;
    }

    // Виведення інформації про товар
    public string Info()
    {
        return $"Product [Code: {Code}, Name: {Name}, Manufacturer: {Manufacturer}, Price: {Price}, Quantity: {Quantity}, TotalCost: {TotalCost}]";
    }

    // Порівняння за ціною (для дерева)
    public int CompareTo(Product? other)
    {
        if (other == null) return 1;
        return Price.CompareTo(other.Price);
    }
}
