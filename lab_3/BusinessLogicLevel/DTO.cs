using DataAccessLevel.Model;

namespace BusinessLogicLevel.DTOs;
public class ProductDTO
{
    public string Code = string.Empty;
    public string Name = string.Empty;
    public string Manufacturer = string.Empty;
    public decimal Price = 0;
    public int QuantityInBatch = 0;
    public virtual Product ToEntity()
    {
        return new Product(this.Code, this.Name, this.Manufacturer, this.Price, this.QuantityInBatch);
    }
}