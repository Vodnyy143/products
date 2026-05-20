using ClosedXML.Excel;
using products.Models;

namespace products.Services;

public class ProductImportService
{
    private readonly AppDbContext _db = new();

    public void Import(string filePath)
    {
        using var workbook = new XLWorkbook(filePath);
        
        var worksheet = workbook.Worksheet(1);
        
        var rows =  worksheet.RowsUsed().Skip(1);

        foreach (var row in rows)
        {
            var name = row.Cell(2).GetString();
            var article = row.Cell(1).GetString();
            var price = row.Cell(4).GetValue<decimal>();
            var discount = row.Cell(8).GetValue<int>();
            var quantity = row.Cell(9).GetValue<int>();
            var description = row.Cell(10).GetString();
            var photo = row.Cell(11).GetString();
            
            if(_db.Products.Any(x => x.Article == article)) 
                continue;
            
            var unitName = row.Cell(3).GetString();
            var supplierName = row.Cell(5).GetString();
            var manufacturerName = row.Cell(6).GetString();
            var categoryName = row.Cell(7).GetString();
            
            
            var unit  = this.GetOrCreateUnit(unitName);
            var supplier = this.GetOrCreateSupplier(supplierName);
            var manufacturer = this.GetOrCreateManufacturer(manufacturerName);
            var category = this.GetOrCreateCategory(categoryName);
            
            var product = new Product
            {
                Article = article,
                Name = name,
                UnitId = unit.Id,
                Price = price,
                SupplierId = supplier.Id,
                ManufacturerId = manufacturer.Id,
                CategoryId = category.Id,
                Discount = discount,
                Quantity = quantity,
                Description = description,
                Photo = photo,
            };
            
            _db.Products.Add(product);
        }
        
        _db.SaveChanges();
    }
    
    private Unit  GetOrCreateUnit(string name)
    {
        var unit = _db.Units.FirstOrDefault(x => x.Name == name);

        if (unit != null)
        {
            return unit;
        }

        unit = new Unit
        {
            Name = name,
        };

        _db.Units.Add(unit);
        _db.SaveChanges();
        return unit;
    }
    
    private Supplier GetOrCreateSupplier(string name)
    {
        var supplier = _db.Suppliers.FirstOrDefault(x => x.Name == name);

        if (supplier != null)
        {
            return supplier;
        }

        supplier = new Supplier
        {
            Name = name,
        };

        _db.Suppliers.Add(supplier);
        _db.SaveChanges();
        return supplier;
    }
    
    private Manufacturer GetOrCreateManufacturer(string name)
    {
        var manufacturer = _db.Manufacturers.FirstOrDefault(x => x.Name == name);

        if (manufacturer != null)
        {
            return manufacturer;
        }

        manufacturer = new Manufacturer
        {
            Name = name,
        };

        _db.Manufacturers.Add(manufacturer);
        _db.SaveChanges();
        return manufacturer;
    }
    
    private Category GetOrCreateCategory(string name)
    {
        var category = _db.Categories.FirstOrDefault(x => x.Name == name);

        if (category != null)
        {
            return category;
        }

        category = new Category
        {
            Name = name,
        };

        _db.Categories.Add(category);
        _db.SaveChanges();
        return category;
    }
}