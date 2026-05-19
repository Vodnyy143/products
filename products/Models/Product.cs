namespace products.Models;

public class Product
{
    public int Id  { get; set; }
    public string Article { get; set; }
    public string Name { get; set; }
    public int? UnitId { get; set; }
    public decimal Price { get; set; }
    public int? SupplierId { get; set; }
    public int? ManufacturerId { get; set; }
    public int? CategoryId { get; set; }
    public int Discount { get; set; } 
    public int Quantity { get; set; }
    public string? Description { get; set; }
    public string? Photo { get; set; }
    
    public Unit? Unit { get; set; }
    public Supplier? Supplier { get; set; }
    public Manufacturer? Manufacturer { get; set; }
    public Category? Category { get; set; }
    public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}