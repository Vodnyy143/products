using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace products.Models;

public class Product
{
    public int Id  { get; set; }
    public string Article { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int? UnitId { get; set; }
    public decimal Price { get; set; }
    public int? SupplierId { get; set; }
    public int? ManufacturerId { get; set; }
    public int? CategoryId { get; set; }
    public int Discount { get; set; } = 0;
    public int Quantity { get; set; } = 0;
    public string? Description { get; set; }
    public string? Photo { get; set; }
    
    public Unit? Unit { get; set; }
    public Supplier? Supplier { get; set; }
    public Manufacturer? Manufacturer { get; set; }
    public Category? Category { get; set; }
    public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
    
    [NotMapped]
    public string PhotoPath
    {
        get
        {
            const string placeholder = "pack://application:,,,/Resources/picture.png";

            if (string.IsNullOrEmpty(Photo))
                return placeholder;

            // Путь к загруженным админом фото — папка images рядом с .exe
            var fullPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "images", Photo);

            return File.Exists(fullPath) ? fullPath : placeholder;
        }
    }

    [NotMapped]
    public decimal FinalPrice => Price * (100 - Discount) / 100m;
}