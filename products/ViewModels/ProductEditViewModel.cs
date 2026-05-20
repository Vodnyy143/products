using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using products.Models;

namespace products.ViewModels;

public partial class ProductEditViewModel: ObservableObject
{
    private readonly AppDbContext _db = new();

    private readonly bool _isNew;
    private readonly string? _originalPhoto;
    
    [ObservableProperty]
    private Product? _product = null!;

    [ObservableProperty] private string _windowTitle = "";
    
    public List<Category> Categories { get; }
    public List<Manufacturer> Manufacturers { get; }
    public List<Supplier> Suppliers { get; }
    public List<Unit> Units { get; }
    
    public Visibility IdVisibility => _isNew ? Visibility.Collapsed : Visibility.Visible;

    public string PhotoPreview
    {
        get
        {
            if (string.IsNullOrEmpty(Product.Photo))
                return "pack://application:,,,/Resources/picture.png";

            var path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "images", Product.Photo);
            return File.Exists(path) ? path
                : "pack://application:,,,/Resources/picture.png";
        }
    }

    public ProductEditViewModel(Product product)
    {
        Categories = _db.Categories.ToList();
        Manufacturers = _db.Manufacturers.ToList();
        Suppliers = _db.Suppliers.ToList();
        Units = _db.Units.ToList();

        if (product == null)
        {
            _isNew = true;
            // ID не отображается, вычислится автоматически
            Product = new Product
            {
                CategoryId = Categories.FirstOrDefault()?.Id,
                ManufacturerId = Manufacturers.FirstOrDefault()?.Id,
                SupplierId = Suppliers.FirstOrDefault()?.Id,
                UnitId = Units.FirstOrDefault()?.Id
            };
            WindowTitle = "Добавление товара";
        }
        else
        {
            _isNew = false;
            // Загружаем из БД для возможности отмены
            Product = _db.Products.First(p => p.Id == product.Id);
            _originalPhoto = Product.Photo;
            WindowTitle = $"Редактирование товара: {Product.Name}";
        }
    }


}